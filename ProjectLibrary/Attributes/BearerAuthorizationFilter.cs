using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLibrary.Attributes
{
    public class BearerAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                
                
                context.Result = new UnauthorizedResult(); // Якщо токен відсутній або некоректний, повертаємо 401
                return;
            }

            // Тут ви можете додати додаткові перевірки токену, наприклад, його розшифрування та перевірку підпису

            base.OnActionExecuting(context);
        }
    }
}
