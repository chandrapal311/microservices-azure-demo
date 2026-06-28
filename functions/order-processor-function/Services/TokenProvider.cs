using order_processor_function.Clients;
using order_processor_function.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace order_processor_function.Services
{
    public class TokenProvider
    {
        private string? _cachedToken;

        private DateTime _expiresAt;

        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly IAuthApi _authApi;

        public TokenProvider(IAuthApi authApi)
        {
            _authApi = authApi;
        }

        //public async Task<string> GetTokenAsync()
        //{
        //    var response = await _authApi.LoginAsync(
        //        new LoginRequest
        //        {
        //            Username = "function-service",
        //            Password = "func123"
        //        });

        //    return response.Token;
        //}

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken)
                && DateTime.UtcNow < _expiresAt)
            {
                return _cachedToken;
            }

            await _lock.WaitAsync();

            try
            {
                // Double-check after acquiring the lock
                if (!string.IsNullOrEmpty(_cachedToken)
                    && DateTime.UtcNow < _expiresAt)
                {
                    return _cachedToken;
                }

                var response = await _authApi.LoginAsync(
                    new LoginRequest
                    {
                        Username = "function-service",
                        Password = "func123"
                    });

                _cachedToken = response.Token;

                var handler = new JwtSecurityTokenHandler();

                var jwt = handler.ReadJwtToken(response.Token);

                _expiresAt = jwt.ValidTo.AddMinutes(-2);

                return _cachedToken;
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
