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

        public JsonResponseFormatException(ResponseFormatData jsonResponse)
        {
            JsonResponse = jsonResponse;
        }
    }
}
