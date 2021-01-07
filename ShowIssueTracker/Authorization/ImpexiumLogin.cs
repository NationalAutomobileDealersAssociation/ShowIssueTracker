using ShowIssueTracker.Data;
using ShowIssueTracker.Models;
using ShowIssueTracker.Models.AccountViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ShowIssueTracker.Authorization
{
    public interface IImpexiumLogin
    {
        Task<SignInResult> UserLoginToken(LoginViewModel model);
        Task<ApplicationUser> PostUserByPassingImpexiumEmail(string email);
    }

    public class ImpexiumLogin : IImpexiumLogin
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ImpexiumProperties _impexiumProperties;
        private readonly IWebHostEnvironment _env;

        public ImpexiumLogin(UserManager<ApplicationUser> userManager,
            IOptions<ImpexiumProperties> impexiumProperties, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _impexiumProperties = impexiumProperties.Value;
            _env = env;
        }

        [HttpGet]
        public string GeneratePassword()
        {
            var options = _userManager.Options.Password;

            int length = options.RequiredLength;

            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString();
        }

     
        public async Task<bool> addUpdateUserCommittees(List<object> usrCommittees)
        {

            try
            {
                List<string> userCommittees = new List<string>();
                var hasAccess = false;
                if (usrCommittees != null)
                {
                    foreach (var mem in (dynamic)(usrCommittees))
                    {
                        var committee = GetDictionary(mem);
                        var endDateCommitee = committee["endDate"];
                        if (endDateCommitee != null)
                        {
                            DateTime Dt = Convert.ToDateTime(endDateCommitee);
                            if (Dt > DateTime.Now)
                            {
                                endDateCommitee = null;
                            }
                        }
                        if (endDateCommitee == null)
                        {
                            var indList = committee["committee"];
                            foreach (var p in (dynamic)(indList))
                            {
                                if (p.Key == "name")
                                {
                                    userCommittees.Add(p.Value);
                                }
                            }

                        }

                        var position = committee["position"];
                        userCommittees.Add(position);

                    }
                }
                if (userCommittees.Count > 0)
                {
                    foreach (var committee in userCommittees)
                    {
                        switch (committee.ToLower())
                        {
                            case "showissuetracker":
                                hasAccess = true;
                           break;
  
                        } 
                    }
                }
                  
               return hasAccess;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }

        }

        private async Task<IdentityResult> PostManager(string email, string firstName , string lastName)
        {
            try
            {
               

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName
                };
                var result = await _userManager.CreateAsync(user, GeneratePassword());



                if (result.Succeeded)
                {
                    var userManger = await _userManager.FindByEmailAsync(email);
                    await _userManager.AddToRoleAsync(userManger, SeedData.RoleConstants.approver);
                     

                }

                return result.Succeeded ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception e)
            {
                // Console.WriteLine(e);
                throw e;
            }
        }

      

        private async Task<IdentityResult> PostUser(IReadOnlyList<string> tokenList)
        {
            try
            {
                var json = await GetImpexiumUserInfo(tokenList);

                var jsonRelationships = await GetImpexiumUserRelations(tokenList);

                switch (json)
                {
                    case "Unauthorized":
                    case "Not Found":
                    case "ProtocolError":
                        return IdentityResult.Failed();
                }

                dynamic userdetails = JsonConvert.DeserializeObject(json);

                var dataList = userdetails.dataList[0];
                string firstName = dataList.firstName;
                string lastName = dataList.lastName;
                string email = dataList.email;

                var indResult = ParseJsonToDictionary(json);
                var lst = GetList(indResult["dataList"]);
                var individual = GetDictionary(lst[0]);
                 

                ///////////////////////////////////////////////////////////

                List<object> usrCommittees = GetList(individual["committees"]);
                var hasAccess = await addUpdateUserCommittees(usrCommittees);
                if(hasAccess)
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName
                    };
                    var result = await _userManager.CreateAsync(user, GeneratePassword());
                    return IdentityResult.Success;


                }
                else
                {
                    return IdentityResult.Failed();
                }

                ///////////////////////////////////////////////////////////
 
            }
            catch (Exception e)
            {
                // Console.WriteLine(e);
                return IdentityResult.Failed();
                throw e;
            }
        }


       
        private static async Task<string> GetImpexiumUserInfo(IReadOnlyList<string> tokenList)
        {
            try
            {
                var appToken = tokenList[0];
                var userToken = tokenList[1];
                var baseUri = tokenList[2];
                var userId = tokenList[3];
                var findByUserId = "/Individuals/Profile/" + userId + "/1?includeDetails=true&loadRelationships=true";

                var headerDictionary = new Dictionary<string, string>
                {
                    { "AppToken", appToken },
                    { "UserToken", userToken }
                };

                var client = new RestClientNew(
                    baseUri + findByUserId,
                    HttpVerb.GET,
                    null,
                    headerDictionary);
                var json = await client.MakeRequest();

                return json;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static async Task<string> GetImpexiumUserRelations(IReadOnlyList<string> tokenList)
        {
            try
            {
                var appToken = tokenList[0];
                var userToken = tokenList[1];
                var baseUri = tokenList[2];
                var userId = tokenList[3];
                var findByUserId = "/Individuals/" + userId + "/Relationships/1?includeDetails=true";

                var headerDictionary = new Dictionary<string, string>
                {
                    { "AppToken", appToken },
                    { "UserToken", userToken }
                };

                var client = new RestClientNew(
                    baseUri + findByUserId,
                    HttpVerb.GET,
                    null,
                    headerDictionary);
                var json = await client.MakeRequest();

                return json;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<string> GetImpexiumAccessToken()
        {
            try
            {

                // Step 1 : Get ApiEndPoint and AccessToken
                // POST api/v1/WebApiUrl

                var client = new RestClientNew(
                    _impexiumProperties.AccessEndPoint,
                    HttpVerb.POST,
                    "{\"AppName\":\"" + _impexiumProperties.AppId + "\",\"AppKey\":\"" +
                    _impexiumProperties.AppKey + "\"}");
                var json = await client.MakeRequest();

                switch (json)
                {
                    case "ProtocolError":
                        return "ProtocolError";
                    case "Unauthorized":
                        return "Unauthorized";
                    case "Not Found":
                        return "Not Found";
                }

                dynamic result = JsonConvert.DeserializeObject(json);
                string apiEndPoint = result.uri;
                string accessToken = result.accessToken;

                // add to local storage 
                var local = accessToken + "|" + apiEndPoint +
                            "|" + DateTime.Now;
                //WriteToFile(local);
                return local;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<string>> GetImpexiumUserToken(string accessToken, string apiEndPoint, string userEmail, string userPassword)
        {
            try
            {
                var tokensList = new List<string>();
                // Step 2: Get AppToken or UserToken or Both
                // POST api/v1/Signup/Authenticate
                var headerDictionary = new Dictionary<string, string> { { "AccessToken", accessToken } };

                var client = new RestClientNew(
                    apiEndPoint,
                    HttpVerb.POST,
                    "{\"AppId\":\"" + _impexiumProperties.AppId + "\",\"AppPassword\":\"" +
                    _impexiumProperties.AppKey
                    + "\",\"AppUserEmail\":\"" + userEmail + "\",\"AppUserPassword\":\"" + userPassword + "\"}",
                    headerDictionary);
                var json = await client.MakeRequest();



                switch (json)
                {
                    case "ProtocolError":
                        tokensList.Add("ProtocolError");
                        return tokensList;
                    case "Unauthorized":
                        tokensList.Add("Unauthorized");
                        return tokensList;
                    case "Not Found":
                        tokensList.Add("Not Found");
                        return tokensList;
                }


                dynamic result = JsonConvert.DeserializeObject(json);

                string appToken = result.appToken;
                string userToken = result.userToken;
                string uri = result.uri;
                string userId = result.userId;

                tokensList.Add(appToken);
                tokensList.Add(userToken);
                tokensList.Add(uri);
                tokensList.Add(userId);

                return tokensList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void WriteToFile(string dataTobewritten)
        {
            try
            {
                const string path = "/Token/token.txt";
                if (!System.IO.File.Exists(_env.WebRootPath + path))
                {
                    System.IO.File.Create(_env.WebRootPath + path);
                }
                else
                {
                    using (var fs = new FileStream(_env.WebRootPath + path, FileMode.Truncate))
                    {
                    }
                }

                using (var w = System.IO.File.AppendText(_env.WebRootPath + path))
                {
                    var err = dataTobewritten;
                    w.WriteLine(err);
                    w.Flush();
                }
            }

            catch (Exception)
            {
                //Response.Write(ex.Message, "When updating ErrorMessage");
            }
        }
        private string ReadfileAndReturnString()
        {
            string text;
            const string path = "/Token/token.txt";
            if (!System.IO.File.Exists(_env.WebRootPath + path))
            {
                return null;
            }
            var fileStream = new FileStream(_env.WebRootPath + path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
                text = text.TrimEnd('\r', '\n');
            }

            return string.IsNullOrEmpty(text) ? null : text;
        }
        public async Task<SignInResult> UserLoginToken(LoginViewModel model)
        {
            try
            {
                var token = await GetImpexiumAccessToken();

                if (string.IsNullOrEmpty(token))
                {
                
                    token = await GetImpexiumAccessToken(); //ReadfileAndReturnString();
                }

                var p = token.Split('|');

                var tokenExpirationTime = p[p.Length - 1];

                var dtCurrentToken = Convert.ToDateTime((string)tokenExpirationTime);
                var dtCurrent = DateTime.Now;

               // if (!((dtCurrent - dtCurrentToken).TotalHours >= 21))
                {
                   
                    token = await GetImpexiumAccessToken(); //ReadfileAndReturnString();
                    p = token.Split('|');
                }

                var accessToken = p[0];
                var apiEndPoint = p[1];

                var tokenList = await GetImpexiumUserToken(accessToken, apiEndPoint, model.Email, model.Password);

                if (tokenList.Count < 2)
                {
                    return SignInResult.Failed;
                }

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var json = await GetImpexiumUserInfo(tokenList);

                    switch (json)
                    {
                        case "Unauthorized":
                        case "Not Found":
                        case "ProtocolError":
                            return SignInResult.Success;
                    }

                    var jsonRelationships = await GetImpexiumUserRelations(tokenList);
                    dynamic userdetails = JsonConvert.DeserializeObject(jsonRelationships);

                    var indResult = ParseJsonToDictionary(jsonRelationships);
                    var dataList = GetList(indResult["dataList"]);
                    var individual = GetDictionary(dataList[0]);

                    var dataListUser = userdetails.dataList[0];
                    string firstName = dataListUser.firstName;
                    string lastName = dataListUser.lastName;
                    string email = dataListUser.email;

                    
                    ///////////////////////////////////////////////////////////

                    List<object> usrCommittees = GetList(individual["committees"]);
                    var hasAccess = await addUpdateUserCommittees(usrCommittees);
                    if (hasAccess)
                    {
                        
                        return SignInResult.Success;


                    }
                    else
                    {
                        return SignInResult.Failed;
                    }

                    ///////////////////////////////////////////////////////////
                     
                }
                else
                {

                    var result = await PostUser(tokenList);

                    return !result.Succeeded ? SignInResult.Failed : SignInResult.Success;
                }
            }
            catch (Exception ex)
            {
                return SignInResult.Failed;
            }
        }

        public async Task<ApplicationUser> PostUserByPassingImpexiumEmail(string email)
        {
            try
            {
                var getUserJson = await findUserByEmailInImpexium(email.ToString().Trim());

                if (getUserJson != "")
                {

                    // Parse Result
                    var result = ParseJsonToDictionary(getUserJson);
                    if (result != null && result.Count > 0 && result.ContainsKey("dataList"))
                    {
                        dynamic dResult = JsonConvert.DeserializeObject(getUserJson);
                        dynamic userdetails = JsonConvert.DeserializeObject(getUserJson);
                        var dataList = userdetails.dataList[0];
                        string id = dataList.id;
                        var user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            FirstName = dataList.firstName,
                            LastName = dataList.lastName
                        };
                       
                        var resultUserCreated = await _userManager.CreateAsync(user, GeneratePassword());

                        if (resultUserCreated.Succeeded)
                        {
                            var userManger = await _userManager.FindByEmailAsync(email);
                            await _userManager.AddToRoleAsync(userManger, SeedData.RoleConstants.guest);

                            return userManger;
                        }

                    }
                    return null;
                }

                return null;
            }
            catch (Exception e)
            {
               
                return null;


            }


        }



        public static Dictionary<string, object> GetDictionary(object input)
        {
            return input as Dictionary<string, object>;
        }

        public static KeyValuePair<T, V> CastFrom<T, V>(Object obj)
        {
            return (KeyValuePair<T, V>)obj;
        }

        public static List<object> GetList(object input)
        {
            return input as List<object>;
        }

        public static Dictionary<string, object> ParseJsonToDictionary(string input)
        {
            // This utility method converts a JSON string into a dictionary of names and values for easy access
            try
            {
                if (input != null)
                {
                    var initial = JsonConvert.DeserializeObject<Dictionary<string, object>>(input);
                    return initial.ToDictionary(
                        d => d.Key,
                        d =>
                        d.Value is JArray
                            ? ParseJsonToList(d.Value.ToString())
                            : (d.Value is JObject ? ParseJsonToDictionary(d.Value.ToString()) : d.Value));
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }


        }

        public static List<object> ParseJsonToList(string input)
        {
            // This utility method converts a JSON string into a dictionary of names and values for easy access
            if (input != null)
            {
                var initial = JsonConvert.DeserializeObject<List<object>>(input);
                return
                    initial.Select(
                        d =>
                        d is JArray
                            ? ParseJsonToList(d.ToString())
                            : (d is JObject ? ParseJsonToDictionary(d.ToString()) : d)).ToList();
            }

            return null;
        }
        public async Task<string> GetImpexiumUserByEmail(string email)
        { 
            try
            {
                var managerEmail = "";
                var token = await GetImpexiumAccessToken(); // ReadfileAndReturnString();

                var p = token.Split('|');
                var accessToken = p[0];
                var apiEndPoint = p[1];
                string userEmail = _impexiumProperties.ApiAccessEmail;
                string userPswd = _impexiumProperties.ApiAccessPassword;

                var tokenList = await GetImpexiumUserToken(accessToken, apiEndPoint, userEmail, userPswd);

                if (tokenList.Count >= 4)
                {
                    var appToken = tokenList[0];
                    var userToken = tokenList[1];
                    var baseUri = tokenList[2];
                    var userId = tokenList[3];
                    var findOrgs = "/Individuals/FindByEmail/" + email + "?includeDetails=true";
                    var headerDictionary = new Dictionary<string, string> {
                        { "AppToken", appToken },
                        { "UserToken", userToken }
                    };
                    var client = new RestClientNew(
                        baseUri + findOrgs,
                        HttpVerb.GET,
                        null,
                        headerDictionary);
                    var json = await client.MakeRequest();

                    // Parse Result
                    var result = ParseJsonToDictionary(json);

                   
                    if (result != null && result.Count > 0 && result.ContainsKey("dataList"))
                    {
                        dynamic dResult = JsonConvert.DeserializeObject(json);
                       // List<object> dataList = GetList(result["dataList"]);

                        dynamic userdetails = JsonConvert.DeserializeObject(json);
                         var dataList = userdetails.dataList[0];
                        string id = dataList.id;
                        ///get User relationships 
                        if(id != null)
                        {
                            //GET /api/v1/Individuals/{id}/Relationships/{pageNumber}
                            var indRelationship = "/Individuals/" + id + "/Relationships/1?includeDetails=true";
                            var clientRel = new RestClientNew(
                               baseUri + indRelationship,
                               HttpVerb.GET,
                               null,
                               headerDictionary);

                            var jsonRelations = await clientRel.MakeRequest();
                            var resultRelations = ParseJsonToDictionary(jsonRelations);
                            if (resultRelations != null && resultRelations.Count > 0 && resultRelations.ContainsKey("dataList"))
                            {
                                /// Relationships 
                                dynamic userReldetails = JsonConvert.DeserializeObject(jsonRelations);
                                var indRelResult = ParseJsonToDictionary(jsonRelations);
                                var dataRelList = GetList(indRelResult["dataList"]);
                                var individualRel = GetDictionary(dataRelList[0]);
                                for (var app = 0; app <= dataRelList.Count - 1; app++)
                                {
                                    individualRel = GetDictionary(dataRelList[app]);
                                    if (individualRel != null)
                                    {
                                        var relationName = individualRel["reciprocalRelationshipName"].ToString();
                                        switch (relationName.ToLower())
                                        {
                                             case "twenty group team leader":

                                                Dictionary<string, object> indRelatedCustomer = GetDictionary(
                                           individualRel["relatedToCustomer"]);
                                                if (indRelatedCustomer != null)
                                                {
                                                    bool isManager = false;
                                                    //  relationshipName
                                                    foreach (var mem in indRelatedCustomer)
                                                    {
                                                        if (mem.Key == "title")
                                                        {
                                                            var temp = mem.Value.ToString();
                                                            if (temp == "20 Group Consultant Manager")
                                                            {
                                                                isManager = true;
                                                                break;
                                                            }

                                                        }
                                                    }
                                                    if (isManager)
                                                    {
                                                        
                                                        var userEmail2 = GetList(indRelatedCustomer["emails"]);
                                                        Dictionary<string, object> userEmailManager = GetDictionary(userEmail2[0]);
                                                        if (userEmailManager != null)
                                                        {
                                                            foreach (var mem in userEmailManager)
                                                            {
                                                                if (mem.Key == "address")
                                                                {
                                                                    managerEmail = mem.Value.ToString();
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        /// manger is not null 
                                                        if (managerEmail != "")
                                                        {
                                                            var userManger = await _userManager.FindByEmailAsync(managerEmail);
 
                                                            if (userManger == null)
                                                            {
                                                                var fullName = indRelatedCustomer["name"].ToString();
                                                                string[] names = fullName.Split(' ');
                                                                if (names.Length > 1)
                                                                {
                                                                    await PostManager(managerEmail, names[0].ToString(), names[1].ToString());
                                                                }
                                                                else
                                                                {
                                                                    await PostManager(managerEmail, fullName, fullName);
                                                                }
                                                                
                                                            }
                                                        }


                                                    }
                                                }
                                                //await _userManager.AddToRoleAsync(user, SeedData.RoleConstants.consultant);
                                                break;
                                        }

                                        //MEMBER_TYPE
                                    }

                                }

                                ///end 

                            } 

                        }

                       
                    }
                }
                return managerEmail;
                 
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
          
        }
      
        public async Task<string> findUserByEmailInImpexium(string email)
        {
            var token = await GetImpexiumAccessToken(); //ReadfileAndReturnString();

            var p = token.Split('|');
            var accessToken = p[0];
            var apiEndPoint = p[1];
            string userEmail = _impexiumProperties.ApiAccessEmail;
            string userPswd = _impexiumProperties.ApiAccessPassword;

            var tokenList = await GetImpexiumUserToken(accessToken, apiEndPoint, userEmail, userPswd);

            if (tokenList.Count >= 4)
            {
                var appToken = tokenList[0];
                var userToken = tokenList[1];
                var baseUri = tokenList[2];
                var userId = tokenList[3];
                var findOrgs = "/Individuals/FindByEmail/" + email + "?includeDetails=true";
                var headerDictionary = new Dictionary<string, string> {
                        { "AppToken", appToken },
                        { "UserToken", userToken }
                    };
                var client = new RestClientNew(
                    baseUri + findOrgs,
                    HttpVerb.GET,
                    null,
                    headerDictionary);
                var json = await client.MakeRequest();

                // Parse Result
                return json;

            }
            else
            {
                return "";
            }
        }
        public class Phone
        {
            public string number { get; set; }
            public string typeName { get; set; }
        }
    }
}
