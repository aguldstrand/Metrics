using System.Web.Mvc;

namespace Collector.Util
{
    public class CorsEnabledAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}