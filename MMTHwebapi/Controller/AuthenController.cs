using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using TechLineCaseAPI.Model;

namespace TechLineCaseAPI.Controller
{
    public class AuthenController : ApiController
    {


        [HttpPost]
        [Route("api/authen/login")]
        public ResultModel Post([FromBody] Authen model)
        {
            try
            {
                var js = new JavaScriptSerializer();
                // var json = HttpContext.Current.Request.Form["Model"];

                //Authen model = js.Deserialize<Authen>(json);

                if (model.Username == null) new ResultMessage() { Status = "E", Message = "Please input username" };
                if (model.Password == null) new ResultMessage() { Status = "E", Message = "Please input password" };


                ResultModel authenResut = AuthenADFS(model);

                return authenResut;
            }
            catch (Exception ex)
            {
                return new ResultModel() { Status = "E", Message = ex.Message };
            }
        }

        private static string AssignStringData(string source, string data)
        {
            switch (data)
            {
                //case "": return null;
                case null: return source;
                default: return data;
            }
        }

        private ResultModel AuthenADFS(Authen au)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
               
              // var cc= GlobalParam._TimeOccurList;
                string url = "";
                string client_id = "";
                if (TechLineCaseAPI.Properties.Settings.Default.DEV)
                {
                    url = TechLineCaseAPI.Properties.Settings.Default.adfs_url_HMS;
                    client_id = TechLineCaseAPI.Properties.Settings.Default.client_id_HMS;
                }
                else
                {
                    url = TechLineCaseAPI.Properties.Settings.Default.adfs_url_MMTH;
                    client_id = TechLineCaseAPI.Properties.Settings.Default.client_id_MMTH;
                }
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("client_id", client_id);
                request.AddParameter("scope", "openid");
                request.AddParameter("grant_type", "password");
                request.AddParameter("username", au.Username);
                request.AddParameter("password", au.Password);
                request.AddParameter("response_mode", "form_post");
                IRestResponse response = client.Execute(request);
                //Console.WriteLine(response.Content);
                sb.Append("1");
                JObject joResponse = JObject.Parse(response.Content);
                sb.Append("1");
                if (joResponse["error"] != null)
                {
                    return new ResultModel() { Status = "e", Message = joResponse["error_description"].ToString(), };
                }
                else
                {
                    var stream = joResponse["id_token"].ToString();
                    // a sample jwt encoded token string which is supposed to be extracted from 'Authorization' HTTP header in your Web Api controller
                    var tokenString = stream;
                    var jwtEncodedString = tokenString; // trim 'Bearer ' from the start since its just a prefix for the token string
                    sb.Append("1");
                    var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                    string fname = "";
                    string lname = "";
                    string upn = "";
                    sb.Append("1");
                    try
                    {
                        //Console.WriteLine("upn => " + token.Claims.First(c => c.Type == "upn").Value);
                        try
                        {
                             fname = token.Claims.First(c => c.Type == "given_name") == null ? "" : token.Claims.First(c => c.Type == "given_name").Value;
                        }
                        catch (Exception e)
                        {

                        }
                        try
                        {
                             lname = token.Claims.First(c => c.Type == "family_name") == null ? "" : token.Claims.First(c => c.Type == "family_name").Value;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            upn = token.Claims.First(c => c.Type == "upn") == null ? "" : token.Claims.First(c => c.Type == "upn").Value;
                        }
                        catch (Exception e)
                        {

                        }
                        sb.Append("1");


                        var dD = upn.Split('@');
                        var d = dD[0].Split('.');
                        string dealercode = "110059";
                        if (d.Length > 1)
                        {
                            dealercode = d[0];
                        }
                        UserMitsu u = IsExistingUser(au.Username);
                        if (u.id == null)
                        {
                            var dealer =
                            CreateNewUser(au.Username, fname.ToString(), lname.ToString(), joResponse["access_token"].ToString(), joResponse["id_token"].ToString(), joResponse["refresh_token"].ToString(), dealercode, au.MobileKey,au.Pin);
                        }
                        else
                        {
                            int userId = int.Parse(u.id);
                            using (mmthapiEntities entity = new mmthapiEntities())
                            {
                                var rec = entity.ro_user.Where(o => o.id == userId).FirstOrDefault();

                                rec.MOBILE_KEY = AssignStringData(rec.MOBILE_KEY, u.mobile_key);
                                rec.PIN = AssignStringData(rec.PIN, u.pin);
                                
                                

                                //entity.ro_case.Attach(record);
                                //entity.ObjectStateManager.ChangeObjectState(record, System.Data.EntityState.Modified);
                                entity.SaveChanges();
                                entity.Refresh(RefreshMode.StoreWins, rec);


                            }
                        }
                        sb.Append("1");
                        var uo = IsExistingUser(upn);
                        var json = JsonConvert.SerializeObject(uo);
                        return new ResultModel() { Status = "S", Message = "Logged in", Result = json };
                    }
                    catch (Exception e)
                    {
                        sb.Append("1");
                        return new ResultModel() { Status = "E", Message = "Error inside", Result = "" };
                    }
                    // return json;



                }


                //JArray array = (JArray)ojObject["chats"];
                //  int id = Convert.ToInt32(array[0].toString());
                sb.Append("1");

            }
            catch (Exception ex)
            {
                return new ResultModel() { Status = "E", Message = ex.Message+sb.ToString() };
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        private UserMitsu IsExistingUser(string userMail)
        {
            UserMitsu u = new UserMitsu();
            try
            {
                string conString = "";

                if (TechLineCaseAPI.Properties.Settings.Default.DEV)
                {
                    conString = TechLineCaseAPI.Properties.Settings.Default.ConStringHMS;

                }
                else
                {
                    conString = TechLineCaseAPI.Properties.Settings.Default.ConStringMMTH;

                }



                //string conString = TechLineCaseAPI.Properties.Settings.Default.ConStringMMTH;
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(
                        "SELECT top 1 ro_user.*,isnull(ro_dealer.name,'') dealername FROM ro_user  left outer join ro_dealer on ro_dealer.code=ro_user.dealer  where [user_mail]='" + userMail + "' ",
                        connection))
                    {
                        //
                        // Instance methods can be used on the SqlCommand.
                        // ... These read data.
                        //
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                u.id = reader.GetValue(0) + "";
                                u.user_mail = reader.GetValue(1) + "";
                                u.first_name = reader.GetValue(2) + "";
                                u.last_name = reader.GetValue(3) + "";
                                u.profile_photo_url = reader.GetValue(4) + "";
                                u.token = reader.GetValue(5) + "";
                                u.id_token = reader.GetValue(6) + "";
                                u.refresh_token = reader.GetValue(7) + "";
                                u.dealer = reader.GetValue(8) + "";
                                u.mobile_key=reader.GetValue(14) + "";
                                u.pin = reader.GetValue(15) + "";
                                u.dealer_name = reader.GetValue(16) + "";
                                // for (int i = 0; i < reader.FieldCount; i++)
                                //  {
                                //  Console.WriteLine(reader.GetValue(i));
                                //  }
                            }
                            return u;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return u;
            }


        }
        private bool CreateNewUser(string user_mail, string first_name, string last_name, string token, string id_token, string refresh_token, string dealer,string mobilekey,string  pin)
        {
            try
            {

                string conString = "";

                if (TechLineCaseAPI.Properties.Settings.Default.DEV)
                {
                    conString = TechLineCaseAPI.Properties.Settings.Default.ConStringHMS;

                }
                else
                {
                    conString = TechLineCaseAPI.Properties.Settings.Default.ConStringMMTH;

                }


                using (SqlConnection connection = new SqlConnection(conString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(
                        "INSERT INTO ro_user([user_mail],[first_name],[last_name] ,[token],id_token,refresh_token,[dealer],CREATED_BY,CREATED_ON,MODIFIED_BY,MODIFIED_ON,STATUS_CODE,MOBILE_KEY,PIN)   VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,1,getdate(),1,getdate(),0,@param8,@param9)",
                        connection))
                    {


                        //
                        // Instance methods can be used on the SqlCommand.
                        // ... These read data.
                        //
                        command.Parameters.AddWithValue("@param1", user_mail);
                        command.Parameters.AddWithValue("@param2", first_name);
                        command.Parameters.AddWithValue("@param3", last_name);
                        command.Parameters.AddWithValue("@param4", token);
                        command.Parameters.AddWithValue("@param5", id_token);
                        command.Parameters.AddWithValue("@param6", refresh_token);
                        command.Parameters.AddWithValue("@param7", dealer);
                        command.Parameters.AddWithValue("@param8", mobilekey==null?"": mobilekey);
                        command.Parameters.AddWithValue("@param9", pin == null ? "" : pin);
                        SqlParameter param = new SqlParameter("@ID", SqlDbType.Int, 4);
                        param.Direction = ParameterDirection.Output;
                        command.Parameters.Add(param);
                        var retId = command.ExecuteNonQuery();
                        return true;

                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }


        }
        private bool UpdateROCase(string caseID, string ROCode)
        {
            try
            {
                string conString = "";

                if (TechLineCaseAPI.Properties.Settings.Default.DEV)
                {
                    conString = TechLineCaseAPI.Properties.Settings.Default.ConStringHMS;

                }
                else
                {
                    conString = TechLineCaseAPI.Properties.Settings.Default.ConStringMMTH;

                }
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(
                        "UPDATE ro_case  set caseid='" + caseID + "'  where out_rocode='" + ROCode + "'",
                        connection))
                    {

                        var retId = command.ExecuteNonQuery();
                        return true;

                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }


        }
    }
}