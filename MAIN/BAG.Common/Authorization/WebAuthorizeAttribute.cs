using BAG.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class WebAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            _Globals.Instance.EnableCookies = true;
            new AuthorizationRules().CheckCookie(true);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class WebAnonymousAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {                        
            new AuthorizationRules().CheckCookie(false);
            new AuthorizationRules().Anonymous();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class WebApiAuthorizeAttribute : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        UnitOfWork unit = new UnitOfWork();
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            new AuthorizationRules().CheckCookie(true);
            Authorization();
            base.OnAuthorization(actionContext);
        }


        public bool Authorization()
        {
            var result = false;
            var Request = System.Web.HttpContext.Current.Request;
            if (Request.Headers["Authorization"] != null)
            {
                var auth = Request.Headers["Authorization"];
                if (!string.IsNullOrWhiteSpace(auth))
                {
                    var list = Request.Headers;
                    var item = list["From"];
                    if (item != null)
                    {
                        var userId = item;
                        if (!string.IsNullOrWhiteSpace(userId))
                        {
                            var id = Guid.Parse(userId);
                            var unitUser = unit.UserRepository.GetByID(id);
                            var authparts = auth.Split(':');
                            if (unitUser != null && unitUser.Password.CompareTo(authparts[1]) == 0)
                            {
                                _Globals.Instance.CurrentLoginUserId = unitUser.Id;
                                _Globals.Instance.CurrentLoginUserName = unitUser.UserName;
                                _Globals.Instance.CurrentDisplayName = unitUser.DisplayName;
                                //_Globals.Instance.CurrentSelectedUserId = unitUser.Id;
                                _Globals.Instance.CurrentAccountId = unitUser.OwnerId;
                                _Globals.Instance.CurrentLanguage = unitUser.Language;
                                result = true;
                            }

                        }
                    }
                }
            }
            return result;
        }
    }

}
