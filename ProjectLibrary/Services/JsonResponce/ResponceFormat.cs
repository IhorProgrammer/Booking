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
    public class ResponceFormat
    {

        public static void SettingResponce( HttpRequest httpRequest )
        {
            httpRequest.ContentType = "application/json";
        }

        public static JsonResponceFormat<T> OK<T>(string message, T data)
        {
            return GetResponceJson(HttpStatusCode.OK, message, data);
        }

        public static JsonResponceFormat<string> BadRequest(string message)
        {
            return GetResponceJson(HttpStatusCode.BadRequest, message, "");
        }

        public static JsonResponceFormat<string> Unauthorized(string message)
        {
            return GetResponceJson(HttpStatusCode.Unauthorized, message, "");
        }
        public static JsonResponceFormat<string> InternalServerError(string message)
        {
            return GetResponceJson(HttpStatusCode.InternalServerError, message, "");
        }
        public static JsonResponceFormat<T> Created<T>(string message, T data)
        {
            return GetResponceJson(HttpStatusCode.Created, message, data);
        }
        
        private static JsonResponceFormat<T> GetResponceJson<T>(HttpStatusCode code, string message, T data)
        {
            var jf = new JsonResponceFormat<T>();
            jf.Data = data;
            jf.Meta = new JsonResponceFormat<T>.MetaData((int)code, message);
            return jf;
        }

    }
}
