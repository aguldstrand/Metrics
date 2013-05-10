using Collector.Util;
using System.Web.Mvc;

namespace Collector
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CompressAttribute());
        }
    }
}