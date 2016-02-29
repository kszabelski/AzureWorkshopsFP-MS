using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Common;
using Microsoft.ApplicationInsights.Extensibility;

namespace AzureConstructionsProgressTracker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var instrumentationKey = ConfigurationManager.AppSettings["AppInsightsInstrumentationKey"];
            if (!string.IsNullOrWhiteSpace(instrumentationKey))
            {
                TelemetryConfiguration.Active.InstrumentationKey = instrumentationKey;
            }

            ServiceBusManager.CreateDefault()?.CreateQueue();
        }
    }
}
