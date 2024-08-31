using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

public class Program
{
    static void Main(String[] args)
    {
        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(900, 700),
            Title = "OpenGL Window"
        };

        using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
        {
            window.Run();
        }
    }
}