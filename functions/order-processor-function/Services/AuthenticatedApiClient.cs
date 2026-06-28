using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace order_processor_function.Services
{
    public class AuthenticatedApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly TokenProvider _tokenProvider;

        public AuthenticatedApiClient(
            IHttpClientFactory factory,
            TokenProvider tokenProvider)
        {
            _httpClient = factory.CreateClient("ResilientClient");
            _tokenProvider = tokenProvider;
        }

        public async Task<HttpResponseMessage> PostAsync(
        string url,
        string json,
        string correlationId)
        {
            var token = await _tokenProvider.GetTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Headers.Add("x-correlation-id", correlationId);

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PutAsync(
        string url,
        string correlationId)
        {
            var token = await _tokenProvider.GetTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Put, url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Headers.Add("x-correlation-id", correlationId);

            return await _httpClient.SendAsync(request);
        }
    }
}
