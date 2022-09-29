using TownhomeQuestionnaire.API.JWT;
using TownhomeQuestionnaire.API.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TownhomeQuestionnaire.API.Helper;
using Services;

namespace TownhomeQuestionnaire.API.Controllers
{
    [Route("api/{version:apiVersion}")]
    [ApiVersion("1")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IServiceFactory ServiceFactory;
        public UserController()
        {
            ServiceFactory = new ServiceFactory();
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var userList = await ServiceFactory.UserService.GetAllUsers();
                if(userList is not null)
                {
                    return Ok(new {message = "User data fetched successfully", data = userList.Users.Select(x=> new { x.Name, x.Email}) });
                }
                else
                {
                    throw new Exception("user data not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error occured while fetching users",error = ex.Message });
            }
        }

        [HttpGet("{email}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetUserBymail(string email)
        {
            try
            {
                var user = await ServiceFactory.UserService.GetUserByEmail(email);
                if(user is not null)
                {
                    return Ok(new {message = "User data fetched successfully", data = new { user.Name, user.Email} });
                }
                else
                {
                    throw new Exception("user data not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error occured while fetching users",error = ex.Message });
            }
        }


        [HttpPut("{email}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateUser(string email)
        {
            try
            {

                var user = await ServiceFactory.UserService.GetUserByEmail(email);

                var stream = new StreamReader(Request.Body);
                var updateJson = await stream.ReadToEndAsync();
                var updateJObject = JObject.Parse(updateJson);

                var isUpdate = ConversionHelper.PathchObject(updateJObject, user);
                if (isUpdate)
                {
                    var result = await ServiceFactory.UserService.UpdateUserByEmail(email, user);
                    if (result)
                    {
                        return Ok(new { message = "User data updated successfully" });
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    return Ok(new { message = "User data is up to date!" });

                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error occured while fetching users", error = ex.Message });
            }
        }

    }
}
