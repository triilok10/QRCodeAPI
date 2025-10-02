using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace BLL.Common
{
    public class Common : ICommon
    {
        #region "Connection String"
        public readonly string _ConnectionString;
        private readonly IHttpContextAccessor _httpContextAccessor; // Keep this for request context
       

        public Common(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _ConnectionString = configuration.GetConnectionString("DBConnection");
            _httpContextAccessor = httpContextAccessor;
        }



        #endregion
        public async Task LogError(Exception ex, string methodName = null, string className = null, object additionalData = null)
        {
            string requestUrl = null;
            string httpMethod = null;
            string userAgent = null;
            string ipAddress = null;
            int? loggedInUserId = null;

            try
            {

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    requestUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
                    httpMethod = httpContext.Request.Method;
                    userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                    ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

                }

                using (SqlConnection con = new SqlConnection(_ConnectionString))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("SP_LogApplicationError", con); // Call the new SP
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ExceptionType", ex.GetType().Name);
                    cmd.Parameters.AddWithValue("@Message", ex.Message);
                    cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InnerExceptionMessage", ex.InnerException?.Message ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MethodName", methodName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ClassName", className ?? this.GetType().Name);
                    cmd.Parameters.AddWithValue("@RequestUrl", requestUrl ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@HttpMethod", httpMethod ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserAgent", userAgent ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoggedInUserId", loggedInUserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AdditionalData", additionalData != null ? System.Text.Json.JsonSerializer.Serialize(additionalData) : (object)DBNull.Value);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"FATAL ERROR: Failed to log exception to database. Original exception: {ex.Message}. Logging exception: {logEx.Message}");
            }
        }


        #region Get State List
        public async Task<OperationResult<List<StateList>>> GetStateList()
        {
            List<StateList> lstState = new List<StateList>();
            try
            {
                using (SqlConnection con = new SqlConnection(_ConnectionString))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("Usp_GetCommon", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "GetStateList");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            if (ds.Tables.Count > 0)
                            {
                                DataTable dt = ds.Tables[0];

                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    lstState = dt.AsEnumerable().Select(row => new StateList
                                    {
                                        StateID = row.Field<int>("StateID"),
                                        StateName = row.Field<string>("StateName")
                                    }).ToList();
                                }
                            }
                        }
                    }
                }

                return OperationResult<List<StateList>>.Success(lstState, "State list fetched successfully");
            }
            catch (Exception ex)
            {
                await LogError(ex, "GetStateList", this.GetType().Name);
                return OperationResult<List<StateList>>.Failure(ex.Message);
            }
        }


        #endregion


        //Used to upload the file
        #region "File Upload"
        public  string UploadFile(string base64, string fileName, string fileExtension)
        {
            
            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            base64 = regex.Replace(base64, string.Empty);
            byte[] fileBytes = Convert.FromBase64String(base64);

            // folder path
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

            // filename
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string uniqueId = unixTimestamp + "_" + new Random().Next(1000, 9999);

            string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName)
                                              .Replace(" ", "_");

            if (!fileExtension.StartsWith("."))
                fileExtension = "_" + fileExtension;

            string finalFileName = $"{uniqueId}_{nameWithoutExt}{fileExtension}";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, finalFileName);

            System.IO.File.WriteAllBytes(filePath, fileBytes);

            return finalFileName;
        }
        #endregion

    }
}