using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Products.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SecurityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [Route("token")]
        [HttpGet]
        public async Task<IActionResult> GetToken()
        {
            using var client = new HttpClient();
            try
            {
                var clientId = _configuration.GetSection("OAuth:ClientId").Value;
                var clientSecret = _configuration.GetSection("OAuth:ClientSecret").Value;
                var tokenUrl = _configuration.GetSection("OAuth:Authority").Value + "connect/token";
                var formData = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret }
                };
                var content = new FormUrlEncodedContent(formData); 
                var response = await client.PostAsync(tokenUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserialize into a typed object
                var token = JsonSerializer.Deserialize<TokenResponse>(responseString);
                return Ok(token);    
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
