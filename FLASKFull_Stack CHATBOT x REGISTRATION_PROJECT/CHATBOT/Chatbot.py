import os
import pyodbc
import openpyxl
import re
import math
from datetime import datetime
from fastapi import FastAPI, HTTPException
from fastapi.responses import JSONResponse
from fastapi.middleware.cors import CORSMiddleware
from fastapi.staticfiles import StaticFiles
from pydantic import BaseModel
import csv
import time
from reportlab.lib.pagesizes import letter
from reportlab.pdfgen import canvas
from groq import Groq
from langchain_core.prompts import ChatPromptTemplate, MessagesPlaceholder
from langchain_community.chat_message_histories import ChatMessageHistory
from langchain_core.chat_history import BaseChatMessageHistory
from langchain_core.runnables.history import RunnableWithMessageHistory
from langchain_groq import ChatGroq
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

# Configuration
DB_CONNECTION_STRING = os.getenv('DB_CONNECTION_STRING')
GROQ_API_KEY = os.getenv('GROQ_API_KEY')
DOWNLOAD_DIRECTORY = "downloads"
os.makedirs(DOWNLOAD_DIRECTORY, exist_ok=True)

app = FastAPI()
app.mount("/downloads", StaticFiles(directory=DOWNLOAD_DIRECTORY), name="downloads")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Initialize Groq client with LangChain integration
llm = ChatGroq(
    groq_api_key=GROQ_API_KEY,
    model_name="mixtral-8x7b-32768"
)

# Define a prompt template with LangChain
prompt_template = ChatPromptTemplate.from_messages([
    ("system", "You are a helpful assistant. Use the provided employee data and conversation history to answer questions in a structured way."),
    MessagesPlaceholder(variable_name="history"),
    ("human", "{input}\nEmployee Data:\n{employee_data}")
])

# In-memory store for chat histories (session-based)
store = {}

def get_session_history(session_id: str) -> BaseChatMessageHistory:
    if session_id not in store:
        store[session_id] = ChatMessageHistory(max_messages=20)  # Limit to 20 messages
    return store[session_id]

# Create a chain with message history
chain_with_history = RunnableWithMessageHistory(
    runnable=prompt_template | llm,
    get_session_history=get_session_history,
    input_messages_key="input",
    history_messages_key="history",
)

class ChatRequest(BaseModel):
    message: str
    session_id: str = "default"  # Default session ID, can be overridden by client

def fetch_employee_data():
    try:
        conn = pyodbc.connect(DB_CONNECTION_STRING)
        cursor = conn.cursor()
        query = "SELECT Name, Age, DOJ FROM Employees"
        cursor.execute(query)
        employees = cursor.fetchall()
        conn.close()

        employee_list = []
        for row in employees:
            name, age, doj = row
            if age is None or isinstance(age, (float, int)) and (math.isnan(age) or math.isinf(age)):
                age = 0
            doj_date = datetime.strptime(str(doj), "%Y-%m-%d")
            employee_list.append({
                "Name": name,
                "Age": age,
                "Year": doj_date.year,
                "Month": doj_date.month,
                "Day": doj_date.day
            })
        return employee_list
    except Exception as e:
        return str(e)

def handle_greeting(user_input: str):
    greetings = {
        r'\b(hi|hello|hey|hola|good morning|good afternoon|good evening)\b': [
            "Hello! How can I assist you today?",
            "Hi there! What can I help you with?",
            "Greetings! I'm ready to help."
        ],
        r'\b(thanks|thank you|appreciated|thx|thnx)\b': [
            "You're welcome! Is there anything else I can help you with?",
            "Glad I could help! Do you need further assistance?",
            "My pleasure! Feel free to ask if you need more information."
        ],
        r'\b(bye|goodbye|see you|take care)\b': [
            "Goodbye! Have a great day.",
            "See you later! Feel free to come back if you need help.",
            "Take care! I'm here whenever you need assistance."
        ]
    }
    for pattern, responses in greetings.items():
        if re.search(pattern, user_input, re.IGNORECASE):
            return responses[hash(user_input) % len(responses)]
    return None

def validate_json(data):
    if isinstance(data, dict):
        return {k: validate_json(v) for k, v in data.items()}
    elif isinstance(data, list):
        return [validate_json(item) for item in data]
    elif isinstance(data, (float, int)) and (math.isnan(data) or math.isinf(data)):
        return 0
    return data

@app.post("/chat")
async def chat(request: ChatRequest):
    user_input = request.message
    session_id = request.session_id
    print(f"User Input: {user_input} (Session ID: {session_id})")

    if not user_input:
        raise HTTPException(status_code=400, detail="No message provided")

    greeting_response = handle_greeting(user_input)
    if greeting_response:
        return JSONResponse(content={
            "response": greeting_response,
            "hasTable": False,
            "downloadLink": None,
            "memory": [msg.content for msg in get_session_history(session_id).messages]
        })

    isCSV = any(word in user_input.lower() for word in ["csv", "c s v"])
    isExcel = any(word in user_input.lower() for word in ["excel", "xlsx", "xls"])
    isPDF = any(word in user_input.lower() for word in ["pdf", "p d f"])

    employee_data = fetch_employee_data()
    if isinstance(employee_data, str):
        raise HTTPException(status_code=500, detail=f"Database Error: {employee_data}")

    data_table = [
        {
            "Name": emp["Name"],
            "Age": emp["Age"],
            "Date of Joining": f"{emp['Year']}-{emp['Month']:02d}-{emp['Day']:02d}"
        }
        for emp in employee_data
    ]

    download_link = None
    if isCSV or isExcel or isPDF:
        if isCSV:
            file_path = export_csv(data_table)
            file_type = "CSV"
        elif isExcel:
            file_path = export_excel(data_table)
            file_type = "Excel"
        else:  # isPDF
            file_path = export_pdf(data_table)
            file_type = "PDF"
        
        download_link = f"http://127.0.0.1:8080/downloads/{os.path.basename(file_path)}"
        response_text = f"Your {file_type} file is ready. Click here to download: {download_link}"
    else:
        # Use LangChain chain to generate response
        employee_data_str = "\n".join([f"{emp['Name']}: Age {emp['Age']}, DOJ {emp['Year']}-{emp['Month']:02d}-{emp['Day']:02d}" for emp in employee_data])
        response = chain_with_history.invoke(
            {"input": user_input, "employee_data": employee_data_str},
            config={"configurable": {"session_id": session_id}}
        )
        response_text = response.content

    return JSONResponse(content={
        "response": response_text,
        "hasTable": not (isCSV or isExcel or isPDF),
        "downloadLink": download_link,
        "tableData":  None,
        "memory": [msg.content for msg in get_session_history(session_id).messages]
    })

def export_csv(data_table):
    timestamp = time.strftime("%Y%m%d_%H%M%S")
    filename = os.path.join(DOWNLOAD_DIRECTORY, f"employees_{timestamp}.csv")
    with open(filename, mode="w", newline="", encoding="utf-8") as file:
        writer = csv.DictWriter(file, fieldnames=["Name", "Age", "Date of Joining"])
        writer.writeheader()
        writer.writerows(data_table)
    return filename

def export_excel(data_table):
    timestamp = time.strftime("%Y%m%d_%H%M%S")
    filename = os.path.join(DOWNLOAD_DIRECTORY, f"employees_{timestamp}.xlsx")
    workbook = openpyxl.Workbook()
    sheet = workbook.active
    sheet.title = "Employees"
    sheet.append(["Name", "Age", "Date of Joining"])
    for row in data_table:
        sheet.append([row["Name"], row["Age"], row["Date of Joining"]])
    workbook.save(filename)
    return filename

def export_pdf(data_table):
    timestamp = time.strftime("%Y%m%d_%H%M%S")
    filename = os.path.join(DOWNLOAD_DIRECTORY, f"employees_{timestamp}.pdf")
    pdf = canvas.Canvas(filename, pagesize=letter)
    pdf.setFont("Helvetica", 12)
    pdf.drawString(50, 750, "Name")
    pdf.drawString(200, 750, "Age")
    pdf.drawString(350, 750, "Date of Joining")
    y = 730
    for row in data_table:
        pdf.drawString(50, y, row["Name"])
        pdf.drawString(200, y, str(row["Age"]))
        pdf.drawString(350, y, row["Date of Joining"])
        y -= 20
    pdf.save()
    return filename

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=8080, reload=True)