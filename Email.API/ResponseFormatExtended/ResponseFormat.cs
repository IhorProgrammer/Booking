using BookingLibrary.JsonResponce;
using System.Net;

namespace Email.API.ResponseFormatExtended
{
    public class ResponseFormat : BookingLibrary.JsonResponce.ResponseFormat
    {
        public static readonly ResponseFormatData EMAIL_TOKEN_CREATED = new() { Message = "Token Added", Code = HttpStatusCode.Created };
        public static readonly ResponseFormatData EMAIL_TOKEN_CHANGED = new() { Message = "The email address has been change", Code = HttpStatusCode.OK };
        public static readonly ResponseFormatData EMAIL_USER_DONT_FIND = new() { Message = "No user found", Code = HttpStatusCode.BadRequest };
        public static readonly ResponseFormatData EMAIL_TOKEN_DELETED = new() { Message = "deleted", Code = HttpStatusCode.OK };


        //Critical Error
        public static readonly JsonResponseFormatException APP_SETTINGS_EmailFromPassword_ERROR = new(APP_SETTINGS_EmailFromPassword) { LogLevel = LogLevel.Critical };
        public static readonly JsonResponseFormatException APP_SETTINGS_EmailFrom_ERROR = new(APP_SETTINGS_EmailFrom) { LogLevel = LogLevel.Critical };
        public static readonly JsonResponseFormatException APP_SETTINGS_FromName_ERROR = new(APP_SETTINGS_FromName) { LogLevel = LogLevel.Critical };



        public static readonly ResponseFormatData APP_SETTINGS_EmailFromPassword = new() { Message = "APP_SETTINGS_EmailFromPassword null", Code = HttpStatusCode.InternalServerError };
        public static readonly ResponseFormatData APP_SETTINGS_EmailFrom = new() { Message = "APP_SETTINGS_EmailFrom null", Code = HttpStatusCode.InternalServerError };
        public static readonly ResponseFormatData APP_SETTINGS_FromName = new() { Message = "APP_SETTINGS_EmailFrom null", Code = HttpStatusCode.InternalServerError };


        public static readonly JsonResponseFormatException EMAIL_MESSAGE_DONT_SEND_ERROR = new(EMAIL_MESSAGE_DONT_SEND) { LogLevel = LogLevel.Critical };
        public static readonly ResponseFormatData EMAIL_MESSAGE_DONT_SEND = new() { Message = "message dont send", Code = HttpStatusCode.InternalServerError };
        
        public static readonly ResponseFormatData EMAIL_EMAIL_DONT_FIND = new() { Message = "email dont find", Code = HttpStatusCode.BadRequest };

    }
}
