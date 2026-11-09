using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProcessAutomation.Business.DTO.Constants;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Code;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.DTO.IDAO;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Helper;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.RegularExpressions;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomation.Business.Service
{
    public class SecurityBM : ISecurityBM
    {
        const string LOWER_CASE = "abcdefghijklmnopqursuvwxyz";
        const string UPPER_CAES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string NUMBERS = "123456789";
        const string SPECIALS = @"!@$&#";

        private readonly ITokenDAO tokenDAO = null;
        private readonly IUserLoginDAO userLoginDAO = null;
        private readonly ICatDeviceDAO deviceDAO;
        private readonly ICatUserDAO userDAO = null;
        private readonly ICatStatusDAO statusDAO = null;
        private readonly ICatTokenTypeDAO tokenTypeDAO = null;
        private readonly JwtSecurityTokenHandler tokenHandler = null;
        //private bool isSecurityVariablesOK = false;

        public SecurityBM(ITokenDAO tokenDAO, IUserLoginDAO userLoginDAO, ICatUserDAO userDAO,
            ICatStatusDAO statusDAO, ICatDeviceDAO deviceDAO,
            ICatTokenTypeDAO tokenTypeDAO)
        {
            this.tokenDAO = tokenDAO;
            this.userLoginDAO = userLoginDAO;
            this.userDAO = userDAO;
            this.statusDAO = statusDAO;
            this.deviceDAO = deviceDAO;
            this.tokenTypeDAO = tokenTypeDAO;
            tokenHandler = new JwtSecurityTokenHandler();
        }

        //private void SetSecurityVariables(string authorizationHeader)
        //{
        //    authorizationHeader = Base64Helper.Base64Decode(authorizationHeader);
        //    if (authorizationHeader != null)
        //    {
        //        BaseRepository.GetInstance().contextScopeInfo = JsonConvert.DeserializeObject<ContextScopeInfo>(EncryptHelper.Decrypt(authorizationHeader));

        //        if (BaseRepository.GetInstance().contextScopeInfo.Token != null
        //            && BaseRepository.GetInstance().contextScopeInfo.IdUser > 0)
        //        {
        //            isSecurityVariablesOK = true;
        //        }
        //    }
        //}

        //private bool OmittedController(string controller, string action)
        //{
        //    bool isOmitted = false;
        //    string[] omittedControllerList = null;

        //    if (SecurityConstant.OmittedControllerAction != null && SecurityConstant.OmittedControllerAction.Trim().Length > 0)
        //    {
        //        omittedControllerList = SecurityConstant.OmittedControllerAction.Split(',');
        //        if (omittedControllerList != null && omittedControllerList.Length > 0)
        //        {
        //            string actionController = string.Format("{0}/{1}", controller, action);
        //            foreach (string omittedController in omittedControllerList)
        //            {
        //                if (actionController == omittedController.Trim())
        //                {
        //                    isOmitted = true;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    return isOmitted;
        //}

        //private void HttpException(ref ActionExecutingContext context, string message, HttpStatusCode httpStatusCode)
        //{
        //    context.Result = new ObjectResult(new { message = message, httpStatusCode = httpStatusCode });
        //}

        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    string controller = context.RouteData.Values["controller"].ToString();
        //    string action = context.RouteData.Values["action"].ToString();

        //    //context.HttpContext.Request.Method
        //    if (OmittedController(controller, action))
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        string authorizationHeader = context.HttpContext.Request.Headers["X-MERP-API-Token"];
        //        if (authorizationHeader != null)
        //        {
        //            if (!isSecurityVariablesOK) SetSecurityVariables(authorizationHeader);

        //            if (BaseRepository.GetInstance().contextScopeInfo.Token != string.Empty && BaseRepository.GetInstance().contextScopeInfo.IdUser > 0)
        //            {
        //                ResponseInfo<bool> responseToken = ValidateToken(BaseRepository.GetInstance().contextScopeInfo.Token, BaseRepository.GetInstance().contextScopeInfo.IdUser);
        //                if (responseToken == null || responseToken.HasError() || !responseToken.Data)
        //                {
        //                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
        //                    {
        //                        controller = SecurityConstant.DeniedController,
        //                        action = SecurityConstant.DeniedAction,
        //                        securityInfo = new SecurityInfo
        //                        {
        //                            Failed = true,
        //                            Url = SecurityConstant.DeniedUrl
        //                        }
        //                    }));
        //                }
        //            }
        //            else
        //                HttpException(ref context, "Sin Autorización", HttpStatusCode.Unauthorized);
        //        }
        //        else
        //            HttpException(ref context, "Sin Autorización", HttpStatusCode.Unauthorized);
        //    }
        //}

        //public ResponseInfo<bool> ValidateToken(string token, int idUser)
        //{
        //    ResponseInfo<bool> responseValid = new ResponseInfo<bool>();
        //    responseValid.Data = false;

        //    ResponseInfo<TOKEN> responseToken = tokenDAO.RetrieveByToken(token);

        //    if (responseToken != null && responseToken.Data != null &&!responseToken.HasError())
        //    {
        //        if (responseToken.Data.Expiration_Date >= DateTime.Now)
        //        {
        //            ResponseInfo<USER_LOGIN> responseUser = userLoginDAO.RetrieveByTokenId(
        //                (BaseRepository.GetInstance().contextScopeInfo.IdToken = responseToken.Data.ID_Token), idUser);

        //            if (responseUser != null && responseUser.Data != null && !responseUser.HasError())
        //                responseValid.Data = (responseUser.Data.Logout_Date == null);
        //        }
        //    }
        //    return responseValid;
        //}

        public ResponseInfo<CAT_USER> UserRegister(string email, string password, string name = null, string lastName = null)
        {
            ResponseInfo<CAT_USER> messageInfo = new ResponseInfo<CAT_USER>();

            try
            {
                if (!userDAO.ExistsByEmail(email))
                {
                    name = (name == null ? email : name);
                    lastName = (lastName == null ? string.Empty : lastName);

                    bool userNameExists = true;
                    string userName = null;

                    while (userNameExists)
                    {
                        userName = GenerateUserName(email);
                        if (!(userNameExists = userDAO.ExistsByCode(userName)))
                        {
                            CAT_USER userInfo = new CAT_USER
                            {
                                Email = email,
                                User_Name = userName,
                                Password = Encryption.Encrypt(password),
                                Name = name,
                                Last_Name = lastName,
                                Creation_Date = DateTime.Now,
                                Modification_Date = DateTime.Now,
                                ID_User_Created = 0,
                                Active = true
                            };

                            messageInfo = userDAO.Create(userInfo);
                        }
                    }
                }
                else
                {
                    messageInfo.Response = "Usuario existente";
                }
            }
            catch (Exception)
            {
                messageInfo.Response = "Error al crear usuario";
            }

            return messageInfo;
        }

        public ResponseInfo<string> UserChangePassword(string email, string oldPassword, string newPassword)
        {
            ResponseInfo<string> messageInfo = new ResponseInfo<string>();

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(oldPassword)
                     && !string.IsNullOrWhiteSpace(newPassword))
            {
                try
                {
                    if (userDAO.ExistsByEmail(email))
                    {
                        if (Regex.IsMatch(newPassword, FunctionConstants.PASSWORD_REGULAR_EXPRESSION))
                        {
                            string emaildEncrypted = Encryption.Encrypt(email);
                            string oldPasswordEncrypted = Encryption.Encrypt(oldPassword);
                            string newPasswordEncrypted = Encryption.Encrypt(newPassword);

                            CAT_USER user = userDAO.RetrieveByEmail(email.ToUpper()).Data;
                            if (user != null && user.Password == oldPasswordEncrypted)
                            {
                                user.Password = newPasswordEncrypted;
                                userDAO.Update(user);

                                messageInfo.Response = "Contraseña cambiada exitosamente";
                            }
                            else
                            {
                                messageInfo.Response = "Error al cambiar la contraseña";
                            }
                        }
                        else
                        {
                            messageInfo.Response = "La contraseña debe de tener al menos 8 caracteres y 2 de ellos deben de ser números";
                        }
                    }
                    else
                    {
                        messageInfo.Response = "Error al cambiar la contraseña";
                    }
                }
                catch (Exception)
                {
                    messageInfo.Response = "Error al crear usuario";
                }
            }
            else
            {
                messageInfo.Response = "Error al capturar información";
            }

            return messageInfo;
        }

        //private ClaimsPrincipal CreateClaim(string email, string name)
        //{
        //    ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        //    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
        //    identity.AddClaim(new Claim(ClaimTypes.Name, name));
        //    return new ClaimsPrincipal(identity);
        //}

        public ResponseInfo<CAT_USER> RetrieveUserByEmail(string email)
        {
            ResponseInfo<CAT_USER> userInfo = new ResponseInfo<CAT_USER>();

            try
            {
                userInfo = userDAO.RetrieveByEmail(email);

                if (userInfo.Data == null)
                {
                    userInfo.Response = "Cuenta de usuario incorrecto";
                }
            }
            catch (Exception)
            {
                userInfo.Response = "Error al obtener usuario";
            }

            return userInfo;
        }

        public static string GeneratePassword(bool useLowercase, bool useUppercase, bool useNumbers, bool useSpecial,
            int passwordSize)
        {
            char[] _password = new char[passwordSize];
            string charSet = ""; // Initialise to blank
            System.Random _random = new Random();
            int counter;

            // Build up the character set to choose from
            if (useLowercase) charSet += LOWER_CASE;

            if (useUppercase) charSet += UPPER_CAES;

            if (useNumbers) charSet += NUMBERS;

            if (useSpecial) charSet += SPECIALS;

            for (counter = 0; counter < passwordSize; counter++)
            {
                _password[counter] = charSet[_random.Next(charSet.Length - 1)];
            }

            return string.Join(null, _password);
        }

        public static string RemoveSpecialCharacters(string value)
        {
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(value))
            {
                foreach (char c in value)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    {
                        result += c;
                    }
                }
            }

            return result;
        }

        public static string GenerateUserName(string email, bool randomPosition = false)
        {
            string result = null;

            if (!string.IsNullOrWhiteSpace(email) && email.Contains('@'))
            {
                result = RemoveSpecialCharacters(email.Split('@')[0]);

                Random random = new Random();
                int position = 0;
                string randomChar = (random.Next(1, 1000)).ToString();

                if (randomPosition)
                {
                    position = random.Next(result.Length - 1);
                    result = result.Insert(position, randomChar);
                }
                else
                {
                    result += randomChar;
                }

                result = result.ToUpper();
            }

            return result;
        }

        public ContextUserInfo GetContextToken(string token)
        {   
            JwtSecurityToken securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            return JsonConvert.DeserializeObject<ContextUserInfo>(securityToken.Claims.First(claim => claim.Type == SecurityConstants.CONTEXT_TOKEN).Value);
        }

        public void DeleteToken(string token) 
        {
            //JwtSecurityToken securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            //tokenHandler.WriteToken(securityToken);
        }

        private string CreateToken(ContextUserInfo contextToken)
        {
            Dictionary<string, object> claims = new Dictionary<string, object>
                {
                    { SecurityConstants.CONTEXT_TOKEN, System.Text.Json.JsonSerializer.Serialize(contextToken) }
                };

            //var sofCid = new ClaimsIdentity(new List<Claim> { new Claim("MyClaimType", "MyClaimValue") }, "CUSTOM_AUTHENTICATION");
            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecurityConstants.SECURITY_KEY.PadRight(64, '#')));
            //SigningCredentials credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SigningCredentials credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                //Subject = sofCid,
                Claims = claims,
                Expires = contextToken.ExpirationDate, //DateTime.UtcNow.AddDays(30),
                SigningCredentials = credential
            };

            SecurityToken jwt = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(jwt);
        }

        public ResponseInfo<CAT_USER> UserLogin(string userNameOrEmail, string password, string deviceCode,
            string latitude = "0", string longitude = "0")
        {
            ResponseInfo<CAT_USER> responseLogin = null;
            if (((responseLogin = userDAO.RetrieveByEmail(userNameOrEmail)) != null && responseLogin.Data != null)
                || ((responseLogin = userDAO.RetrieveByUserName(userNameOrEmail)) != null && responseLogin.Data != null)
                && !responseLogin.HasError())
            {
                if (responseLogin.Data.Password == password)
                {
                    TOKEN token = null;
                    ResponseInfo<bool> responseUserLog = SaveUserLogIn(responseLogin.Data.ID_User, deviceCode, ref token, latitude, longitude, StatusNumber.USER_LOGIN);

                    ContextUserInfo contextInfo = new ContextUserInfo
                    {
                        IdUser = responseLogin.Data.ID_User,
                        UserName = responseLogin.Data.User_Name,
                        Name = responseLogin.Data.Name,
                        Email = responseLogin.Data.Email,
                        DeviceCode = deviceCode,
                        LanguageCode = null,
                        PageViewCode = null,
                        Latitude = latitude,
                        Longitude = longitude,
                        Token = token.Token,
                        IdToken = token.ID_Token, //LUIS VERA: EVITAR ENVIAR EL ID TOKEN Y EVITAR SEGURIDAD CON ID TOKEN, HACERLO POR EL TOKEN
                        ExpirationDate = token.Expiration_Date
                    };

                    //responseLogin.Data.ContextToken = Base64Helper.Base64Encode(EncryptHelper.Encrypt(JsonConvert.SerializeObject(contextInfo)));

                    responseLogin.Data.ContextToken = Base64Helper.Base64Encode(CreateToken(contextInfo));
                    //responseLogin.Data.ContextToken = CreateToken(contextInfo);
                }
                else
                {
                    responseLogin.Error = "Error al hacer login";
                }
                responseLogin.Data.Password = null;
            }
            else
            {
                responseLogin.Error = "Error en usuario y/o contraseña";
            }
            return responseLogin;
        }
        public ResponseInfo<bool> UserLogout(string token, int idUser, string deviceCode, string latitude = "0", string longitude = "0")
        {
            ResponseInfo<bool> responseUserLog = null;
            ResponseInfo<CAT_USER> responseUser = userDAO.Retrieve(idUser);
            if (responseUser != null && !responseUser.HasError())
            {
                ResponseInfo<TOKEN> tokenInfo = tokenDAO.RetrieveByToken(token);
                if (tokenInfo?.Data != null && !tokenInfo.HasError())
                {
                    TOKEN tokenAux = tokenInfo.Data;
                    responseUserLog = SaveUserLogIn(idUser, deviceCode, ref tokenAux, latitude, longitude, StatusNumber.USER_LOGOUT);
                }
            }
            return responseUserLog;
        }
        private TOKEN ProcessTokenLogIn(int idUser, int statusCode)
        {
            TOKEN tokenReturned = null;
            ResponseInfo<int> responseTokenType = null;
            string createdToken = null;
            DateTime tokenExpirationDate;
            if (statusCode == StatusNumber.USER_LOGIN)
            {
                responseTokenType = tokenTypeDAO.RetrieveIdentifierByCode(TokenTypeCode.AUTHENTICATION);
                if (responseTokenType != null && !responseTokenType.HasError())
                {
                    if ((createdToken = TokenHelper.CreateToken(idUser.ToString())) != null)
                    {
                        tokenExpirationDate = DateTime.Now.AddDays(30);
                        TOKEN token = new TOKEN
                        {
                            Token = createdToken,
                            Expiration_Date = tokenExpirationDate,
                            ID_Token_Type = responseTokenType.Data,
                            ID_User_Created = 1,
                            Creation_Date = DateTime.Now,
                            Active = true
                        };
                        ResponseInfo<TOKEN> tokenAfterInsert = tokenDAO.Create(token);
                        if (tokenAfterInsert != null && !tokenAfterInsert.HasError())
                        {
                            tokenReturned = tokenAfterInsert.Data;
                        }
                    }
                }
            }
            return tokenReturned;
        }

        private int ProcessTokenLogOut(int idUser, int statusCode, string tokenString)
        {
            int idToken = 0;
            if (statusCode == StatusNumber.USER_LOGOUT)
            {
                ResponseInfo<TOKEN> responseToken = tokenDAO.RetrieveByToken(tokenString);
                if (responseToken != null && !responseToken.HasError())
                {
                    responseToken.Data.Expiration_Date = DateTime.Now;
                    responseToken.Data.Finish_Date = DateTime.Now;
                    responseToken.Data.Active = false;
                    tokenDAO.Update(responseToken.Data);
                    idToken = responseToken.Data.ID_Token;
                }
            }
            return idToken;
        }

        private ResponseInfo<bool> SaveUserLogIn(int idUser, string deviceCode, ref TOKEN token, string latitude, string longitude, int statusCode)
        {
            ResponseInfo<bool> returnResponse = new ResponseInfo<bool>();
            ResponseInfo<int> catStatus = statusDAO.RetrieveIdentifierByNumber(statusCode, StatusTypeCode.AUTHENTICATION);

            if (catStatus != null && !catStatus.HasError())
            {
                ResponseInfo<USER_LOGIN> responseUserLogin = null;
                if (statusCode == StatusNumber.USER_LOGIN)
                {
                    TOKEN tokenReturned = ProcessTokenLogIn(idUser, statusCode);
                    if (tokenReturned != null)
                    {
                        CAT_DEVICE catDevice = deviceDAO.RetrieveByCode(deviceCode)?.Data;
                        if (catDevice != null)
                        {
                            int idDevice = catDevice.ID_Device;
                            USER_LOGIN userLogin = new USER_LOGIN
                            {
                                ID_User = idUser,
                                ID_Device = idDevice,
                                Login_Date = DateTime.Now,
                                Login_Latitude = latitude,
                                Login_Longitude = longitude,
                                ID_Token = tokenReturned.ID_Token,
                                ID_User_Created = idUser,
                                Creation_Date = DateTime.Now,
                            };
                            responseUserLogin = userLoginDAO.Create(userLogin);
                            token = tokenReturned;
                        }
                    }
                }
                else if (statusCode == StatusNumber.USER_LOGOUT)
                {
                    int idToken = ProcessTokenLogOut(idUser, statusCode, token.Token);
                    if (idToken > 0)
                    {
                        if (token != null)
                        {
                            responseUserLogin = userLoginDAO.RetrieveByTokenId(token.ID_Token, idUser);
                            if (responseUserLogin != null && !responseUserLogin.HasError())
                            {
                                responseUserLogin.Data.Logout_Date = DateTime.Now;
                                responseUserLogin.Data.Logout_Latitude = latitude;
                                responseUserLogin.Data.Logout_Longitude = longitude;
                                responseUserLogin.Data.Alteration_Counter++;
                                responseUserLogin.Data.Modification_Date = DateTime.Now;
                                responseUserLogin.Data.ID_User_Modified = idUser;
                                responseUserLogin = userLoginDAO.Update(responseUserLogin.Data);
                            }
                        }
                    }
                }

                if (responseUserLogin != null && !responseUserLogin.HasError())
                {
                    returnResponse.Response = responseUserLogin.Response;
                    returnResponse.Error = responseUserLogin.Error;
                    returnResponse.Data = (responseUserLogin != null && !responseUserLogin.HasError());
                }
            }

            return returnResponse;
        }
    }
}