using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class ChatController : Controller
    {
        private readonly HttpClient _httpClient;

        public ChatController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:8080/");
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            if (string.IsNullOrEmpty(message?.Text))
            {
                return BadRequest("Message cannot be empty.");
            }

            Console.WriteLine($"User Input: {message.Text}");

            try
            {
                var json = JsonConvert.SerializeObject(new { message = message.Text });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("chat", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from FastAPI:");
                Console.WriteLine(responseContent);

                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new DoubleJsonConverter());
                var result = JsonConvert.DeserializeObject<ChatResponse>(responseContent, settings);

                if (result == null)
                {
                    return StatusCode(500, "Error: Received an empty response from the chatbot.");
                }

                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error communicating with the chatbot: {ex.Message}");
            }
            catch (JsonReaderException ex)
            {
                return StatusCode(500, $"Error parsing JSON response: {ex.Message}");
            }
        }

        public class ChatMessage
        {
            public string Text { get; set; }
        }

        public class ChatResponse
        {
            public string Response { get; set; }
            public bool HasTable { get; set; }
            public object TableData { get; set; }
            public string[] Memory { get; set; }
            [JsonProperty("downloadLink")]
            public string DownloadLink { get; set; }
        }

        public class DoubleJsonConverter : JsonConverter<double>
        {
            public override double ReadJson(JsonReader reader, Type objectType, double existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.String && reader.Value.ToString() == "NaN")
                {
                    return 0;
                }
                return Convert.ToDouble(reader.Value);
            }

            public override void WriteJson(JsonWriter writer, double value, JsonSerializer serializer)
            {
                writer.WriteValue(value);
            }
        }
    }
}