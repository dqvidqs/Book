using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Collections.Generic;
using System;

namespace BookAPI.Auth
{
    public class AuthorizeFilter : IAuthorizationFilter
    {
        readonly string[] _claim;

        public AuthorizeFilter(params string[] claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var IsAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            var claimsIndentity = context.HttpContext.User.Identity as ClaimsIdentity;

            if (IsAuthenticated)
            {
                bool flagClaim = false;
                foreach (var item in _claim)
                {
                    if (context.HttpContext.User.HasClaim("ACCESS_LEVEL", item))
                        flagClaim = true;
                }
                if (!flagClaim)
                    context.Result = new RedirectToActionResult("NoPer", "Auth", null);
            }
            else
            {
                context.Result = new RedirectToActionResult("Home", "Auth", null);
            }
            return;
        }
    }
}
