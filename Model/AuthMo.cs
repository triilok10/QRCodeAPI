using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class AuthMo
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; } = false;
        public string MobileNo { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int StateMasterId { get; set; }
        public string State { get; set; } = string.Empty;
        public string MenuIds { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string ProfileImageName { get; set; } = string.Empty;
        public string ProfileImageBase64 { get; set; } = string.Empty;
        public string RegisterProcess { get; set; } = "Manual";//GoogleAuth,Manual
        public bool ActiveStatus { get; set; } = false;
        public bool DeleteStatus { get; set; } = false;
        public bool TermsAgreed { get; set; } = false;
        
    }

    public class LoginMo
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class JWT
    {
        public string Username { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public bool ActiveStatus { get; set; }
        public string AuthToken { get; set; }
    }
}
