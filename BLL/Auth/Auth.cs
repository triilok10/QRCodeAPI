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


        #region "Register"
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

                        using (SqlDataReader rdr = cmd.ExecuteReader()) 
                        {
                            while (rdr.Read()) 
                            {
                                res.Status = Convert.ToInt32(rdr["Status"]);
                                res.Message = Convert.ToString(rdr["Message"]);
                            }
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

        #endregion

        #region "Login"
        public async Task<ServiceResponse> Login(LoginMo pLoginMo)
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

                        cmd.Parameters.AddWithValue("@Action", "Login");
                        cmd.Parameters.AddWithValue("@Username", pLoginMo.Username);
                        cmd.Parameters.AddWithValue("@Password", pLoginMo.Password);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);


                            if(ds.Tables.Count> 0)
                            {
                                DataTable dt = new DataTable();
                                DataTable dtJWT = new DataTable();
                                dt = ds.Tables[0];
                                dtJWT = ds.Tables[1];

                                if (dt.Rows.Count > 0)
                                {
                                    var Status = Convert.ToInt32(dt.Rows[0]["Status"]);

                                    if(Status == 200)
                                    {
                                        if (dtJWT.Rows.Count > 0)
                                        {

                                        }
                                    }
                                }
                            }
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
        #endregion

    }
}
