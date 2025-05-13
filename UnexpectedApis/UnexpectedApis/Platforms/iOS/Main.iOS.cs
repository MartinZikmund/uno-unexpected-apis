using UIKit;
using UnexpectedApis;
using Uno.UI.Hosting;

App.InitializeLogging();

var host = UnoPlatformHostBuilder.Create()
    .App(() => new App())
    .UseAppleUIKit()
    .Build();

host.Run();
