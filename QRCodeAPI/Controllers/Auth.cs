using BLL.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using System.Threading.Tasks;

namespace QRCodeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        #region "Constructor"
        private readonly IAuth _auth;

        public Auth(IAuth auth, IConfiguration configuration)
        {
            _auth = auth;
        }
        #endregion

        #region "Register"
        [HttpPost("register")]
        public async Task<ActionResult<OperationResult<string>>> Register(AuthMo pAuth)
        {
            var result = await _auth.Register(pAuth);

            if (result.State == OperationState.Success)
            {
                return Ok(result);
            }
            else
            {
                return Ok(result);
            }

               
        }
        #endregion

        #region "Login"
        [HttpPost("login")]
        public async Task<ActionResult<OperationResult<JWT>>> Login(LoginMo pLoginMo)
        {
            var result = await _auth.Login(pLoginMo);

            if (result.State == OperationState.Success)
            {
                return Ok(result);
            }
            else
            {
                return Ok(result);
            }
        }
        #endregion
    }
}
