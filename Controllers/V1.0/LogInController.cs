using TownhomeQuestionnaire.API.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TownhomeQuestionnaire.API.Helper;
using Services;
using TownhomeQuestionnaire.API.Model;

namespace TownhomeQuestionnaire.API.Controllers
{
    [Route("api/{version:apiVersion}")]
    [ApiVersion("1.0")]
    [ApiController]
    [AllowAnonymous]
    public class LogInController : ControllerBase
    {
        private readonly IServiceFactory ServiceFactory;
        private readonly JwtSettings jwtSettings;

        public LogInController(JwtSettings jwtSettings)
        {
            ServiceFactory = new ServiceFactory();
            this.jwtSettings = jwtSettings;
        }

        /// <summary>
        /// Login to system, for valid user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> LogIn(LogIn login)
        {
            try
            {
                if (login == null)
                {
                    throw new ArgumentNullException(nameof(login), "Body can not be empty.");
                }
                var result = await ServiceFactory.LogInService.LogInUser(login);
                if (result)
                {
                    var token = JwtHelpers.GenTokenKey(new UserTokens()
                    {
                        EmailId = login.UserName,
                        GuidId = Guid.NewGuid(),
                        UserName = login.UserName,
                    }, jwtSettings);
                    var refreshToken = JwtHelpers.GenerateRefreshToken();

                    return Ok(new {message = "User logged in successfully", accessToken = token.Token, refreshToken = refreshToken });
                }
                else
                {
                    throw new InvalidDataException("");
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = "Error occured while logging in!", error = ex.Message });
            }
        }

        /// <summary>
        /// Logging out the current user which is logged in
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> LogOut()
        {
            return Ok(new { value = "Logged out successfully" });
        }

    }
}
