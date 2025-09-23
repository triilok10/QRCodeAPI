using BLL.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace QRCodeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {

        public readonly ICommon _common;
        public CommonController(ICommon common) 
        { 
         _common = common;
        }


        [HttpGet("GetStateList")]
        public async Task<OperationResult<List<StateList>>> GetStateList()
        {
            var result = await _common.GetStateList();

            if (result.State == OperationState.Success)
            {
                return result;
            }
            else
            {
                return result;
            }
        }
    }
}
