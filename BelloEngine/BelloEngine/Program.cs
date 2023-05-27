using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;


namespace BelloEngine
{
    class Program
    {

        static void Main(string[] args) 
        {
            using (Game game = new Game(800, 600, "Bello Engine"))
            {
                game.Run();
                Window wnd = new Window();
            }
        }
    }
}
