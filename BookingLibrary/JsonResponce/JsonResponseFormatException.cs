using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BookingLibrary.JsonResponce
{
    public class JsonResponseFormatException : Exception
    {
        public ResponseFormatData JsonResponse { get; set; }
        public LogLevel LogLevel { get; set; }

        public JsonResponseFormatException LogLevelSet(LogLevel level) { LogLevel = level; return this; }

        public JsonResponseFormatException(ResponseFormatData jsonResponse) : base(jsonResponse?.Message ?? "")
        {
            if( jsonResponse != null ) JsonResponse = jsonResponse;
        }

        public JsonResponseFormatException(ResponseFormatData jsonResponse, Exception innerException) : base(innerException.Message, innerException)
        {
            JsonResponse = jsonResponse;
        }
      
    }
}
