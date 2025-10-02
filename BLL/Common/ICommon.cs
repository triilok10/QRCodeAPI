using Azure;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Common
{
    public interface ICommon
    {
        Task LogError(Exception ex, string methodName = null, string className = null, object additionalData = null);
        Task<OperationResult<List<StateList>>> GetStateList();
         string UploadFile(string base64, string fileName, string fileExtension);
    }
}
