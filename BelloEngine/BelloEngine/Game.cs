using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using ImGuiNET;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using OpenTK.Compute.OpenCL;
using System.Reflection.Metadata;
using System.Xml.Linq;


namespace BelloEngine
{

    


    class Game : GameWindow
    {
        public Game(int width, int height, string title) 
            : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) 
        {
            
        }

        ImGuiController _guiController;
        int VertexBufferObject;
        int VertexArrayObject;

        private Shader shader;
        private Texture _texture;
        private Texture texture2;
        private Camera camera;

        private double _time;
        private bool _firstMove = true;
        private Vector2 _lastPos;


        //Level dimensions
        int LevelWitdh = 32;
        int LevelDepth = 10;
        int LevelHeight = 5;




        //Update 
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }


            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }



            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                camera.Position += camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                camera.Position -= camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.Position -= camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.Position += camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                camera.Position += camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.Position -= camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        

        //Start (like unity)
        protected override void OnLoad()
        {
            base.OnLoad();

            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);

            _guiController = new ImGuiController(ClientSize.X, ClientSize.Y);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);



            

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Model.vertices.Length * sizeof(float), Model.vertices, BufferUsageHint.StaticDraw);

            

            // The shaders have been modified to include the texture coordinates, check them out after finishing the OnLoad function.
            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();
            

            
            // This will now pass the new vertex array to the buffer.
            var vertexLocation = shader.GetAttribLocation("aPosition");                  //"Shader was null" NioOOooOOOopoOOOooO i don t careeer343 pls workds  :=|
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);


            //Same thing as vertexLocation 
            var texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            //load the container tex
            _texture = Texture.LoadFromFile("res/container.jpg");
            _texture.Use(TextureUnit.Texture0);
            //load the face tex
            texture2 = Texture.LoadFromFile("res/awesomeface.png");
            texture2.Use(TextureUnit.Texture1);

            shader.SetInt("texture0", 0);
            shader.SetInt("texture1", 1);

            //create the camera
            camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            //Lock the cursor
            CursorState = CursorState.Grabbed;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            //DISPODE!!
            shader.Dispose();
        }

        //Render on every frame
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _guiController.Update(this, (float)e.Time);
            GL.ClearColor(new Color4(0, 32, 48, 255));



            //ImGui.ShowDemoWindow();
            //_guiController.Render();
            //ImGuiController.CheckGLError("End of frame");



            _time += 25.0 * e.Time;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(VertexArrayObject);

            _texture.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);
            shader.Use();




            KeyboardState input = KeyboardState;
            //Enable / Disable Wireframe
            if (input.IsKeyDown(Keys.E))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else { GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill); }






            //Rendering of the scene

            int w = LevelWitdh;
            int d = LevelDepth;
            int h = LevelHeight;

            for (int x = 0; x < w; x++)
                for (int z = 0; z < d; z++)
                    for(int y = 0; y < h; y++)
                {
                    //move to the x axis
                    Matrix4 model = Matrix4.CreateTranslation(x, 0, 0);
                    //move all of the row stacked on the same row along the z axis 
                    model = Matrix4.CreateTranslation(x, 0, z);
                    model = Matrix4.CreateTranslation(x, y, z);

                    //draw 
                    shader.SetMatrix4("model", model);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }




            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());



            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);


            SwapBuffers();
        }
        

        //useless but usefull :)
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            //update opengl viewport
            GL.Viewport(0, 0, e.Width, e.Height);
            camera.AspectRatio = Size.X / (float)Size.Y;

            // Tell ImGui of the new size
            _guiController.WindowResized(ClientSize.X, ClientSize.Y);
        }

        //Take input from the keyboard for ImGui
        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);


            _guiController.PressChar((char)e.Unicode);
        }

        //MouseWheel
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            camera.Fov -= e.OffsetY;

            _guiController.MouseScroll(e.Offset);
        }



    }
}
