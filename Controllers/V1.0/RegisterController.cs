using TownhomeQuestionnaire.API.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using TownhomeQuestionnaire.API.Model;

namespace TownhomeQuestionnaire.API.Controllers.V1._0
{
    [Route("api/{version:apiVersion}")]
    [ApiVersion("1.0")]
    [ApiController]
    [AllowAnonymous]
    public class RegisterController : ControllerBase
    {

        private readonly IServiceFactory ServiceFactory;
        public RegisterController()
        {
            ServiceFactory = new ServiceFactory();
        }


        /// <summary>
        /// Creating user in system
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(Register register)
        {
            try
            {
                if (register == null)
                {
                    throw new ArgumentNullException(nameof(register), "Body can not be empty.");
                }
                var result = await ServiceFactory.RegisterService.RegisterUser(register);
                if (result)
                {
                    return Ok(new { message = "User register successfully!" });
                }
                else
                {
                    throw new InvalidDataException("");
                }

            }
            catch(Exception ex)
            {
                return BadRequest(new { message = "Error occured while registering!", error = ex.Message});
            }
        }

    }
}
