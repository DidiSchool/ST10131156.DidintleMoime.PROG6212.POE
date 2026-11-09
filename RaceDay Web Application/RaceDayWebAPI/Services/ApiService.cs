using System.Net.Http.Json;
using RaceDayWebAPI.ViewModels;

namespace RaceDayWebAPI.Services
   
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EventViewModel>?> GetEventsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<EventViewModel>>("api/events");
        }

        public async Task<EventViewModel?> GetEventByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<EventViewModel>($"api/events/{id}");
        }

        public async Task<HttpResponseMessage> RegisterUserAsync(object registerDto)
        {
            return await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
        }

        public async Task<HttpResponseMessage> LoginUserAsync(object loginDto)
        {
            return await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
        }

        public async Task<HttpResponseMessage> CreateEventAsync(MultipartFormDataContent content)
        {
            return await _httpClient.PostAsync("api/events", content);
        }
    }
}
