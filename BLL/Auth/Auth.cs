using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using System.Data;

namespace BLL.Auth
{
    public class Auth : IAuth
    {
        public readonly string _connectionString;
        public Auth(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DBConnection");
        }

        public async Task<ServiceResponse> Register(AuthMo pAuth)
        {
            ServiceResponse res = new ServiceResponse();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("usp_AuthLogin", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Username", pAuth.Username);
                        cmd.Parameters.AddWithValue("@FullName", pAuth.FullName);
                        cmd.Parameters.AddWithValue("@Email", pAuth.Email);
                        cmd.Parameters.AddWithValue("@IsEmailVerified", pAuth.IsEmailVerified);
                        cmd.Parameters.AddWithValue("@Gender", pAuth.Gender);
                        cmd.Parameters.AddWithValue("@MobileNo", pAuth.MobileNo);
                        cmd.Parameters.AddWithValue("@StateMasterId", pAuth.StateMasterId);
                        cmd.Parameters.AddWithValue("@MenuIds", pAuth.MenuIds);
                        cmd.Parameters.AddWithValue("@ProfileImage", pAuth.ProfileImage);
                        cmd.Parameters.AddWithValue("@ActiveStatus", pAuth.ActiveStatus);
                        cmd.Parameters.AddWithValue("@DeleteStatus", pAuth.DeleteStatus);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds); 
                            res.Data = new
                            {
                                StateMaster = ds.Tables[0],    // First table
                                UserDetail = ds.Tables[1],     // Second table
                                StatusMessage = ds.Tables[2]   // Third table
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.Status = 500;
                res.Message = ex.Message;
            }

            return res;
        }

    }
}
