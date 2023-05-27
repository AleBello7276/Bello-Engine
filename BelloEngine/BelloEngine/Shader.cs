
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

public class Shader
{
    int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        int VertexShader;
        int FragmentShader;

        string VertexShaderSource = File.ReadAllText(vertexPath);
        string FragmentShaderSource = File.ReadAllText(fragmentPath);

        //Generate and bind source to shaders
        VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(VertexShader, VertexShaderSource);
        FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(FragmentShader, FragmentShaderSource);




        //Compile the shader
        GL.CompileShader(VertexShader);

        GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(VertexShader);
            Console.WriteLine(infoLog);
        }
        GL.CompileShader(FragmentShader);
        GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int success1);
        if (success1 == 0)
        {
            string infoLog = GL.GetShaderInfoLog(FragmentShader);
            Console.WriteLine(infoLog);
        }


        //link the program
        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, VertexShader);
        GL.AttachShader(Handle, FragmentShader);

        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success2);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(Handle);
            Console.WriteLine(infoLog);
        }

        //delete useless old stuff 
        GL.DetachShader(Handle, VertexShader);
        GL.DetachShader(Handle, FragmentShader);
        GL.DeleteShader(FragmentShader);
        GL.DeleteShader(VertexShader);
    }


    public void Use()
    {
        GL.UseProgram(Handle);
    }




    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            GL.DeleteProgram(Handle);

            disposedValue = true;
        }
    }


    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);
        int location = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(location, true,ref data);
    }


    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);

        GL.Uniform1(location, value);
    }

    ~Shader()
    {
        GL.DeleteProgram(Handle);
        if (disposedValue == false)
        {
            Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public int GetAttribLocation(String attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }


}