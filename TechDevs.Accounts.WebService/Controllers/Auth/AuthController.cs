﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechDevs.Accounts.WebService.Controllers
{
    public abstract class AuthController<TAuthUser> : Controller where TAuthUser : AuthUser, new()
    {
        private readonly IAuthTokenService<TAuthUser> _tokenService;
        private readonly IAuthUserService<TAuthUser> _accountService;
        
        protected AuthController(IAuthTokenService<TAuthUser> tokenService, IAuthUserService<TAuthUser> accountService)
        {
            _tokenService = tokenService;
            _accountService = accountService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> EmployeeLogin([FromBody] LoginRequest req, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            switch (req.Provider)
            {
                case "TechDevs":
                    return await LoginWithTechDevs(req.Email, req.Password, clientId);
                case "Google":
                    return await LoginWithGoogle(req.ProviderIdToken, clientId);
                default:
                    return new BadRequestObjectResult("Unsupported auth provider");
            }
        }

        private async Task<IActionResult> LoginWithTechDevs(string email, string password, string clientId)
        {
            var valid = await _accountService.ValidatePassword(email, password, clientId);
            if (!valid) return new UnauthorizedResult();
            var user = await _accountService.GetByEmail(email, clientId);
            var token = await _tokenService.CreateToken(user.Id, "profile", clientId);
            return new OkObjectResult(token);
        }

        private async Task<IActionResult> LoginWithGoogle(string idToken, string clientId)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                var user = await _accountService.GetByProvider("Google", payload.Subject, clientId);
                var token = await _tokenService.CreateToken(user.Id, "profile", clientId);
                return new OkObjectResult(token);
            }
            catch (InvalidJwtException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception)
            {
                return new UnauthorizedResult();
            }
        }
    }

    [Produces("application/json")]
    [Route("api/v1/employee/auth")]
    public class EmployeAuthController : AuthController<Employee>
    {
        public EmployeAuthController(IAuthTokenService<Employee> tokenService, IAuthUserService<Employee> accountService) 
            : base(tokenService, accountService)
        {
        }
    }

    [Produces("application/json")]
    [Route("api/v1/customer/auth")]
    public class CustomerAuthController : AuthController<Customer>
    {
        public CustomerAuthController(IAuthTokenService<Customer> tokenService, IAuthUserService<Customer> accountService)
            : base(tokenService, accountService)
        {
        }
    }
}