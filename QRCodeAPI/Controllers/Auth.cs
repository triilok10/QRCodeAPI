using BLL.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;


namespace QRCodeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        #region "Constructor"
        public readonly IAuth _auth;
        public Auth(IAuth auth, IConfiguration configuration)
        {
            _auth = auth;          
        }
        #endregion


        #region "Register"
        [HttpPost("register")]
        public async Task<ServiceResponse> Register(AuthMo pAuth)
        {
            var result = await _auth.Register(pAuth);
            return result;

        }
        #endregion

        #region "Login"
        [HttpPost("Login")]
        public async Task<ServiceResponse> Login(LoginMo pLoginMo)
        {
            var result = await _auth.Login(pLoginMo);
            return result;

        }
        #endregion
    }
}
