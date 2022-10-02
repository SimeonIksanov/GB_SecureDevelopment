using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace CardStorageService.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IValidator<AuthenticationRequest> _authenticationRequestValidator;

        #region Constructors
        public AuthenticationController(IAuthenticationService authenticationService,
                                        IValidator<AuthenticationRequest> authenticationRequestValidator)
        {
            _authenticationService = authenticationService;
            _authenticationRequestValidator = authenticationRequestValidator;
        }

        #endregion

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            ValidationResult validationResult = _authenticationRequestValidator.Validate(authenticationRequest);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            AuthenticationResponse authenticationResponse = _authenticationService.Login(authenticationRequest);
            if (authenticationResponse.Status == Models.AuthenticationStatus.Success)
            {
                Response.Headers.Add("X-Session-Token", authenticationResponse.SessionInfo.SessionToken);
            }
            return Ok(authenticationResponse);
        }

        [HttpGet("session")]
        public IActionResult GetSessionInfo()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var sessionToken = headerValue.Parameter;
                if (string.IsNullOrWhiteSpace(sessionToken))
                {
                    return Unauthorized();
                }

                SessionInfo? sessionInfo = _authenticationService.GetSessionInfo(sessionToken);
                if (sessionInfo == null)
                {
                    return Unauthorized();
                }

                return Ok(sessionInfo);
            }
            return Unauthorized();
        }
    }
}
