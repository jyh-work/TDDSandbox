using Microsoft.AspNet.SignalR;
using Owin;

namespace TDDSandbox.SignalRServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            hubConfiguration.EnableJavaScriptProxies = false;
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR<PersistentConnection>("chat", hubConfiguration);
        }
    }
}