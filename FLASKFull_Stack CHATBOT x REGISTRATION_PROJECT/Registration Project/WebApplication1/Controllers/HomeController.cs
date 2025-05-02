//////using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebApplication1.Models;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _secretKey;

        public HomeController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7294/";
            _secretKey = _configuration["ApiSettings:SecretKey"] ?? throw new InvalidOperationException("Secret key not found in configuration.");
        }

        private HttpClient CreateClient()
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_apiBaseUrl);

            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                // Generate a new token if it doesn't exist
                token = GenerateToken();
                HttpContext.Session.SetString("JwtToken", token);
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private string GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "EmployeeAPI",
                audience: "EmployeeAPIUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        public async Task<IActionResult>
    Index()
        {
            try
            {
                using var client = CreateClient();
                var response = await client.GetAsync("api/Employee/GetAllEmployees");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JwtToken");
                    return RedirectToAction("Index"); // Regenerate token and retry
                }

                var jsonString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var employees = JsonConvert.DeserializeObject<List<Employees>>(jsonString) ?? new List<Employees>();
                    return View(employees);
                }

                ViewBag.ErrorMessage = "Failed to retrieve employees.";
                return View(new List<Employees>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View(new List<Employees>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.GetAsync($"api/Employee/GetEmployeeById/{id}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JwtToken");
                    return RedirectToAction("Index");
                }

                var jsonString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var employee = JsonConvert.DeserializeObject<Employees>(jsonString);
                    return View(employee);
                }

                ViewBag.ErrorMessage = "Failed to retrieve employee details.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Employees employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            try
            {
                using var client = CreateClient();
                var json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"api/Employee/Edit/{id}", content);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JwtToken");
                    return RedirectToAction("Index");
                }

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Failed to update employee.";
                return View(employee);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View(employee);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employees employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            try
            {
                using var client = CreateClient();
                var json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/Employee/Create", content);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JwtToken");
                    return RedirectToAction("Index");
                }

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Failed to create employee.";
                return View(employee);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View(employee);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchName, string sortColumn = "Name", string sortDirection = "ASC", int page = 1)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.GetAsync($"api/Employee/Search?name={searchName}&sortColumn={sortColumn}&sortDirection={sortDirection}&page={page}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JwtToken");
                    return RedirectToAction("Index");
                }

                var jsonString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var employees = JsonConvert.DeserializeObject<List<Employees>>(jsonString) ?? new List<Employees>();
                    ViewBag.TotalPages = 10; // Replace with actual total pages from API
                    return View("Index", employees);
                }

                ViewBag.ErrorMessage = "Failed to retrieve search results.";
                return View("Index", new List<Employees>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View("Index", new List<Employees>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.DeleteAsync($"api/Employee/Delete/{id}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JwtToken");
                    return RedirectToAction("Index");
                }

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Failed to delete employee.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
