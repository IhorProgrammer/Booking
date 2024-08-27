using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectLibrary.Services.JsonResponce
{
    public class ResponseFormat
    {

        public static void SettingResponce( HttpRequest httpRequest )
        {
            httpRequest.ContentType = "application/json";
        }

        public static JsonResponseFormat<T> OK<T>(string message, T data)
        {
            return GetResponceJson(HttpStatusCode.OK, message, data);
        }

        public static JsonResponseFormat<string> BadRequest(string message)
        {
            return GetResponceJson(HttpStatusCode.BadRequest, message, "");
        }

        public static JsonResponseFormat<string> Unauthorized(string message)
        {
            return GetResponceJson(HttpStatusCode.Unauthorized, message, "");
        }
        public static JsonResponseFormat<string> InternalServerError(string message)
        {
            return GetResponceJson(HttpStatusCode.InternalServerError, message, "");
        }
        public static JsonResponseFormat<T> Created<T>(string message, T data)
        {
            return GetResponceJson(HttpStatusCode.Created, message, data);
        }
        
        private static JsonResponseFormat<T> GetResponceJson<T>(HttpStatusCode code, string message, T data)
        {
            var jf = new JsonResponseFormat<T>();
            jf.Data = data;
            jf.Meta = new JsonResponseFormat<T>.MetaData((int)code, message);
            return jf;
        }

    }
}
