using BLL.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BLL.Auth
{
    public class Auth : IAuth
    {
        public readonly string _connectionString;
        private readonly ICommon _common;

        public Auth(IConfiguration configuration, ICommon Common)
        {
            _connectionString = configuration.GetConnectionString("DBConnection");
            _common = Common;
        }

        #region "Register"
        public async Task<OperationResult<string>> Register(AuthMo pAuth)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("usp_AuthLogin", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Action", "Register");
                        cmd.Parameters.AddWithValue("@Username", pAuth.Username);
                        cmd.Parameters.AddWithValue("@FullName", pAuth.FullName);
                        cmd.Parameters.AddWithValue("@Email", pAuth.Email);
                        cmd.Parameters.AddWithValue("@MobileNo", pAuth.MobileNo);
                        cmd.Parameters.AddWithValue("@Gender", pAuth.Gender);
                        cmd.Parameters.AddWithValue("@IsEmailVerified", pAuth.IsEmailVerified);
                        cmd.Parameters.AddWithValue("@StateMasterId", pAuth.StateMasterId);
                        cmd.Parameters.AddWithValue("@MenuIds", pAuth.MenuIds);
                        cmd.Parameters.AddWithValue("@ProfileImage", pAuth.ProfileImage);
                        cmd.Parameters.AddWithValue("@Password", pAuth.Password);

                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                int status = Convert.ToInt32(rdr["Status"]);
                                string message = Convert.ToString(rdr["Message"]);

                                if (status == 200)
                                {
                                    return OperationResult<string>.Success("Registered successfully", message);
                                }
                                else
                                {
                                    return OperationResult<string>.Failure(message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _common.LogError(ex, "Register", this.GetType().Name);
                return OperationResult<string>.Failure(ex.Message);
            }

            return OperationResult<string>.Failure("Unknown error occurred during registration");
        }
        #endregion

        #region "Login"
        public async Task<OperationResult<JWT>> Login(LoginMo pLoginMo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("usp_AuthLogin", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Action", "Login");
                        cmd.Parameters.AddWithValue("@Username", pLoginMo.Username);
                        cmd.Parameters.AddWithValue("@Password", pLoginMo.Password);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            if (ds.Tables.Count > 1)
                            {
                                DataTable dt = ds.Tables[0];
                                DataTable dtJWT = ds.Tables[1];

                                if (dt.Rows.Count > 0)
                                {
                                    int status = Convert.ToInt32(dt.Rows[0]["Status"]);

                                    if (status == 200 && dtJWT.Rows.Count > 0)
                                    {
                                        DataRow row = dtJWT.Rows[0];

                                        JWT pJWT = new JWT
                                        {
                                            UserID = Convert.ToInt32(row["UserID"]),
                                            Username = Convert.ToString(row["Username"]),
                                            RoleID = Convert.ToInt32(row["RoleID"]),
                                            ActiveStatus = Convert.ToBoolean(row["ActiveStatus"])
                                        };

                                        var token = await CreateAuthenticationToken(pJWT);
                                        return OperationResult<JWT>.Success(token, "Login successful");
                                    }
                                    else
                                    {
                                        return OperationResult<JWT>.Failure("Error");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _common.LogError(ex, "Login", this.GetType().Name);
                return OperationResult<JWT>.Failure(ex.Message);
            }

            return OperationResult<JWT>.Failure("Login failed");
        }
        #endregion

        #region "Auth Token Generate"
        public async Task<JWT> CreateAuthenticationToken(JWT pJWT)
        {
            var key = Encoding.ASCII.GetBytes("fed0e14e-a076-4e77-9c2d-14545ce6fde3");
            var JWToken = new JwtSecurityToken(
                issuer: "Qrcode.com",
                audience: "Qrcode.com",
                claims: GetClaims(pJWT),
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            pJWT.AuthToken = token;
            return pJWT;
        }

        public IEnumerable<Claim> GetClaims(JWT ppJWT)
        {
            var claims = new List<Claim>
            {
                new Claim("RoleId", ppJWT.RoleID.ToString()),
                new Claim("Username", ppJWT.Username ?? ""),
                new Claim("UserID", ppJWT.UserID.ToString()),
                new Claim("ActiveStatus", ppJWT.ActiveStatus.ToString())
            };

            return claims;
        }
        #endregion
    }
}
