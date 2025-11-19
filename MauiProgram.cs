using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace Calculator;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
            .ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(windows =>
                {
                    windows.OnWindowCreated(window =>
                    {
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                        if (appWindow != null)
                        {
                            appWindow.Resize(new Windows.Graphics.SizeInt32(500, 900));
                            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
                            if (presenter != null)
                            {
                                presenter.IsResizable = false;
                                presenter.IsMaximizable = false;
                            }
                        }
                    });
                });
#endif
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
