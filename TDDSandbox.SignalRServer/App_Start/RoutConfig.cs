using System.Web.Routing;
using Owin;

namespace TDDSandbox.SignalRServer
{
    public static class RoutConfig
    {
        
        public static void RegisterRoutes(IAppBuilder appBuilder)
        {
            if (appBuilder == null)
            {
                return;
            }
            appBuilder.MapSignalR<ChatConnection>("chat");
        }
    }
}