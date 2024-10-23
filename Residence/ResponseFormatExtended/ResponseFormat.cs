using BookingLibrary.JsonResponce;
using System.Net;

namespace Residence.API.ResponseFormatExtended
{
    public class ResponseFormat : BookingLibrary.JsonResponce.ResponseFormat
    {
        public static readonly ResponseFormatData RESIDENCE_CATEGORIES_GET = new() { Message = "all categories get", Code= HttpStatusCode.OK };
        public static readonly ResponseFormatData RESIDENCE_SEARCH_GET = new() { Message = "search ok", Code = HttpStatusCode.OK };

    }
}
