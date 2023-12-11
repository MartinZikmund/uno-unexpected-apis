using UnexpectedApisDemo.Shared;

namespace UnexpectedApis;

public class App : Application
{
    public Window? MainWindow { get; private set; }

    protected IHost? Host { get; private set; }

    protected ShellPage? Shell { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .ConfigureServices((context, services) =>
                {
                    // TODO: Register your services
                    //services.AddSingleton<IMyService, MyService>();
                })
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.EnableHotReload();
#endif

        Host = builder.Build();

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not ShellPage shell)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            Shell = new ShellPage();

            // Place the frame in the current Window
            MainWindow.Content = Shell;
        }

        // Ensure the current window is active
        MainWindow.Activate();
    }
}
