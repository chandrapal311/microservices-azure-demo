
using order_processor_function.DTOs;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_processor_function.Clients
{
    public interface IAuthApi
    {
        [Post("api/auth/login")]
        Task<LoginResponse> LoginAsync([Body] LoginRequest request);
    }
}
