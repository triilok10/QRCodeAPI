using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public enum OperationState
    {
        Success,
        Error,
        Warning
    }

    [DataContract]
    public class OperationResult<TResult>
    {
        [DataMember]
        [JsonProperty(PropertyName = "Data")]
        public TResult Data { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "State")]
        public OperationState State { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "SuccessMessage")]
        public string SuccessMessage { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        public OperationResult()
        {
            State = OperationState.Error;
        }

        public static OperationResult<TResult> Success(TResult data, string message = "Operation successful")
        {
            return new OperationResult<TResult>
            {
                Data = data,
                State = OperationState.Success,
                SuccessMessage = message
            };
        }

        public static OperationResult<TResult> Failure(string errorMessage)
        {
            return new OperationResult<TResult>
            {
                State = OperationState.Error,
                ErrorMessage = errorMessage
            };
        }
    }
}
