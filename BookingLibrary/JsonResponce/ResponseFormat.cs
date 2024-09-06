using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingLibrary.JsonResponce
{


    public class ResponseFormat
    {
        
        public static readonly ResponseFormatData JWT_INVALID = new(){ Message = "JWT invalid", Code = HttpStatusCode.BadRequest };
        public static readonly ResponseFormatData JWT_NOT_FOUND =  new(){ Message = "JWT not found", Code = HttpStatusCode.Unauthorized };
        public static readonly ResponseFormatData TOKEN_ID_INVALID =  new(){ Message = "JWT invalid", Code = HttpStatusCode.BadRequest };
        public static readonly ResponseFormatData TOKEN_DATA_NULL =  new(){ Message = "Information for user not found", Code = HttpStatusCode.InternalServerError };
        public static readonly ResponseFormatData TOKEN_INVALID = new() { Message = "JWT invalid", Code = HttpStatusCode.BadRequest };
        public static readonly ResponseFormatData TOKEN_NOT_SIGNED = new() { Message = "The user is not signed in", Code = HttpStatusCode.Unauthorized };
        public static readonly ResponseFormatData TOKEN_VALID = new() { Message = "Token valid", Code = HttpStatusCode.OK };
        public static readonly ResponseFormatData TOKEN_GENERATION_RESPOSE_ERROR = new() { Message = "Server error", Code = HttpStatusCode.InternalServerError };
        public static readonly ResponseFormatData USER_AGENT_EMPTY = new() { Message = "UserAgent empty", Code = HttpStatusCode.BadRequest };
        public static readonly ResponseFormatData TOKEN_GENERATED = new() { Message = "Token generated", Code = HttpStatusCode.Created };
        public static readonly ResponseFormatData SERVER_ERROR = new() { Message = "Server error", Code = HttpStatusCode.InternalServerError};
        public static readonly ResponseFormatData TOKEN_EMAIL_SUB = new() { Message = "token and email sub", Code = HttpStatusCode.OK };
        public static readonly ResponseFormatData LOGIN_AGAIN = new() { Message = "JWT invalid", Code = HttpStatusCode.Unauthorized, sendEmail = true };
        public static readonly ResponseFormatData DECRYPTION_NULL = new() { Message = "data or iv invalid", Code = HttpStatusCode.BadRequest,};
        public static readonly ResponseFormatData DATA_DECODED = new() { Message = "data decoded", Code = HttpStatusCode.OK,};
        public static readonly ResponseFormatData EMAIL_NULL = new() { Message = "Email null", Code = HttpStatusCode.InternalServerError, };

        //Google Token
        public static readonly ResponseFormatData GOOGLE_URL_REDIRECT = new() { Message = "URL google", Code = HttpStatusCode.OK, };
        public static readonly ResponseFormatData GOOGLE_URL_REDIRECT_EXCEPTION = new() { Message = "Token invalid", Code = HttpStatusCode.BadRequest, };

        //AppSettings
        public static readonly ResponseFormatData APP_SETTINGS_SECRET_NULL = new() { Message = "Server error", Code = HttpStatusCode.InternalServerError, };
        public static readonly ResponseFormatData APP_SETTINGS_ISSUER_NULL = new() { Message = "Server error", Code = HttpStatusCode.InternalServerError, };
        public static readonly ResponseFormatData APP_SETTINGS_AUDIENCE_NULL = new() { Message = "Server error", Code = HttpStatusCode.InternalServerError, };
        //Google Session
        public static readonly ResponseFormatData GET_SESSION_OK = new() { Message = "sessions", Code = HttpStatusCode.OK, };
        public static readonly ResponseFormatData REMOVE_SESSION_OK = new() { Message = "session removed", Code = HttpStatusCode.OK, };
        public static readonly ResponseFormatData REMOVE_SESSION_FAIL = new() { Message = "server error", Code = HttpStatusCode.InternalServerError, };
        
        //Auth
        public static readonly ResponseFormatData AUTH_INVALID = new() { Message = "login or password invalid", Code = HttpStatusCode.BadRequest, };
        public static readonly ResponseFormatData AUTHORIZATION_VALID = new() { Message = "authorization ok", Code = HttpStatusCode.OK, };
        //Reg
        public static readonly ResponseFormatData REGISTRATION_VALID = new() { Message = "registration ok", Code = HttpStatusCode.OK, };
        public static readonly ResponseFormatData REGISTRATION_INVALID = new() { Message = "registration invalid", Code = HttpStatusCode.BadRequest, };

        //TokenServer
        public static readonly ResponseFormatData TOKEN_SERVER_CONNECTION_NULL = new() { Message = "server error", Code = HttpStatusCode.InternalServerError, };
        
        public static readonly ResponseFormatData DECRYPTION_SERVER_EMPTY_STRING = new () { Message = "decryption error", Code = HttpStatusCode.BadRequest, };


        public static JsonResponseFormat<T?> GetResponceJson<T>(ResponseFormatData responseFormatData, T? data)
        {
            var jf = new JsonResponseFormat<T?>();
            jf.Data = data;
            jf.Meta = new JsonResponseFormat<T?>.MetaData((int)responseFormatData.Code, responseFormatData.Message);
            return jf;
        }

    }

    public class ResponseFormatData
    {
        public string Message { get; set; } = default!;
        public HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
        public bool sendEmail { get; set; } = false;

        public JsonResponseFormatException Exception { 
            get => new JsonResponseFormatException(this); 
        }

        public JsonResponseFormat<string?> Responce { 
            get {
                var jf = new JsonResponseFormat<string?>();
                jf.Data = null;
                jf.Meta = new JsonResponseFormat<string?>.MetaData((int)Code, Message);
                return jf;
            }
        }
    }



}
