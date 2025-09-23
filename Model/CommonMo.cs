using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CommonMo
    {
    }

    public class ServiceResponse
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public dynamic Data { get; set; }
    }

    public class StateList
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }
}
