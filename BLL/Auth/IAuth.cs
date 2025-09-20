using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Auth
{
    public interface IAuth
    {
        Task<OperationResult<string>> Register(AuthMo pAuth);
        Task<OperationResult<JWT>> Login(LoginMo pLoginMo);
    }
}
