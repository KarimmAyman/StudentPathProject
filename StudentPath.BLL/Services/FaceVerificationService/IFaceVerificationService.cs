using StudentPath.BLL.Dtoes.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.FaceVerificationService
{
    public interface IFaceVerificationService
    {
        Task<bool> VerifyFacesAsync(string idPhotoUrl, string personalPhotoUrl);
    }

    public class FaceVerificationService : IFaceVerificationService
    {
        private readonly HttpClient _httpClient;

        public FaceVerificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:8000/");
        }


        public async Task<bool> VerifyFacesAsync(string idPhotoUrl, string personalPhotoUrl)
        {
            try
            {
                // Prepare the request payload (adjust based on your API's expected format)
                var requestBody = new
                {
                    id_url = idPhotoUrl,
                    ref_url = personalPhotoUrl
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                // Send POST request to your API
                var response = await _httpClient.PostAsync("verify", content);
                response.EnsureSuccessStatusCode();

                // Parse the response
                var responseBody = await response.Content.ReadFromJsonAsync<VerificationResponse>();
                if (responseBody == null || !responseBody.Match)
                {
                    return false; // Return false if response is invalid or IsMatch is null
                }

                return responseBody.Match; // Return the boolean result
            }
            catch (Exception ex)
            {
                // Log error and assume non-match for safety
                Console.WriteLine($"Face verification error: {ex.Message}");
                return false;
            }
        }
    }
}
