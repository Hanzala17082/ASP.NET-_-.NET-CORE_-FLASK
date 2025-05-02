using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext directly in Program.cs
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer("Server=HANZALA\\SQLEXPRESS; Database=CrudADOdb; Trusted_Connection=True"));

// Register Employee model and controllers
builder.Services.AddControllers();

// Enable Swagger
builder.Services.AddSwaggerGen();

// Add CORS policy (can be restricted for production)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authorization setup (if required)
app.UseAuthorization();

// Enable CORS globally
app.UseCors("AllowAny");

app.MapControllers();

app.Run();
