using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Window : GameWindow
{
    private Escenario _escenario;
    private Matrix4 _viewMatrix;
    private Matrix4 _projectionMatrix;

    private float _movementX = 0.000001f;
    private float _movementY = 0.000001f; 
    private float _rotationSpeedX = 0.002f;
    private float _rotationSpeedY = 0.002f;

    private Objeto _objeto; 
    private Objeto _objeto2;


    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        // Inicializar el escenario
        _escenario = new Escenario();

        // Crear el shader program
        int shaderProgram = CrearShaderProgram();

        // Crear el objeto y agregar las partes al escenario
        _objeto = new Objeto(shaderProgram);
        _objeto.AgregarParte(CrearParte(shaderProgram, DefinirVerticesBarraSuperior()));
        _objeto.AgregarParte(CrearParte(shaderProgram, DefinirVerticesBarraVertical()));
        _escenario.AgregarObjeto(_objeto);

        //// Crear el segundo objeto
        //_objeto2 = new Objeto(shaderProgram);
        //_objeto2.AgregarParte(CrearParte(shaderProgram, DefinirVerticesBarraSuperior()));
        //_objeto2.AgregarParte(CrearParte(shaderProgram, DefinirVerticesBarraVertical()));

        //// Establecer una posición diferente para el segundo objeto
        //_objeto2.EstablecerPosicion(new Vector3(0.00005f, 0.00000f, 0.0f));
        //_escenario.AgregarObjeto(_objeto2);

    }

    protected override void OnLoad()
    {
        base.OnLoad();

        // Configurar las matrices de perspectiva y vista
        _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            (float)900 / 700,
            0.1f,
            100f);
        _viewMatrix = Matrix4.LookAt(
            new Vector3(3f, 2f, 3f),
            Vector3.Zero,
            Vector3.UnitY);

        GL.ClearColor(Color4.CornflowerBlue);
        GL.Enable(EnableCap.DepthTest);
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Dibujar el escenario
        _escenario.Dibujar(_viewMatrix, _projectionMatrix);

        SwapBuffers();
    }

    private Parte CrearParte(int shaderProgram, (List<Vector3> vertices, List<uint> indices) definicionVerticesIndices)
    {
        // Crear el polígono
        Poligono poligono = new Poligono(
            definicionVerticesIndices.vertices, 
            definicionVerticesIndices.indices, shaderProgram);

        // Crear la parte y agregar el polígono
        Parte parte = new Parte();
        parte.AgregarPoligono(poligono);

        return parte;

    }

    private (List<Vector3> vertices, List<uint> indices) DefinirVerticesBarraSuperior()
    {
        var verticesBarraSuperior = new List<Vector3>
    {
        new Vector3(-0.3f, 0.2f, 0.0f), // V0
        new Vector3(0.3f, 0.2f, 0.0f), // V1
        new Vector3(-0.3f, 0.0f, 0.0f), // V2
        new Vector3(0.3f, 0.0f, 0.0f), // V3

        new Vector3(-0.3f, 0.2f, -0.1f), // V4
        new Vector3(0.3f, 0.2f, -0.1f), // V5
        new Vector3(-0.3f, 0.0f, -0.1f), // V6
        new Vector3(0.3f, 0.0f, -0.1f), // V7
    };

        var indicesBarraSuperior = new List<uint>
    {
        0, 1, 2, 2, 1, 3,  // Frente
        4, 5, 6, 6, 5, 7,  // Atrás
        0, 2, 4, 4, 2, 6,  // Lado izquierdo
        1, 3, 5, 5, 3, 7,  // Lado derecho
        0, 1, 4, 4, 1, 5,  // Superior
        2, 3, 6, 6, 3, 7,  // Inferior
    };

        return (verticesBarraSuperior, indicesBarraSuperior);
    }

    private (List<Vector3> vertices, List<uint> indices) DefinirVerticesBarraVertical()
    {
        var verticesBarraVertical = new List<Vector3>
    {
        new Vector3(-0.08f, 0.0f, 0.0f), // V8
        new Vector3(0.08f, 0.0f, 0.0f), // V9
        new Vector3(-0.08f, -0.6f, 0.0f), // V10
        new Vector3(0.08f, -0.6f, 0.0f), // V11

        new Vector3(-0.08f, 0.0f, -0.1f), // V12
        new Vector3(0.08f, 0.0f, -0.1f), // V13
        new Vector3(-0.08f, -0.6f, -0.1f), // V14 
        new Vector3(0.08f, -0.6f, -0.1f), // V15
    };

        var indicesBarraVertical = new List<uint>
    {
        0, 1, 2, 2, 1, 3,  // Frente
        4, 5, 6, 6, 5, 7,  // Atrás
        0, 2, 4, 4, 2, 6,  // Lado izquierdo
        1, 3, 5, 5, 3, 7,  // Lado derecho
        0, 1, 4, 4, 1, 5,  // Superior
        2, 3, 6, 6, 3, 7,  // Inferior
    };

        return (verticesBarraVertical, indicesBarraVertical);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (KeyboardState.IsKeyDown(Keys.Right))
        {
            _objeto.TrasladarX(_movementX);
        }
        if (KeyboardState.IsKeyDown(Keys.Left))
        {
            _objeto.TrasladarX(-_movementX);
        }
        if (KeyboardState.IsKeyDown(Keys.Up))
        {
            _objeto.TrasladarY(_movementY);
        }
        if (KeyboardState.IsKeyDown(Keys.Down))
        {
            _objeto.TrasladarY(-_movementY);
        }

        // Rotación relativa usando las teclas A y D
        if (KeyboardState.IsKeyDown(Keys.A))
        {
            _objeto.Rotar(_rotationSpeedY, Vector3.UnitY);
        }
        if (KeyboardState.IsKeyDown(Keys.D))
        {
            _objeto.Rotar(-_rotationSpeedY, Vector3.UnitY);
        }

        // Rotación sobre el eje X usando las teclas W y S
        if (KeyboardState.IsKeyDown(Keys.W))
        {
            _objeto.Rotar(_rotationSpeedX, Vector3.UnitX);
        }
        if (KeyboardState.IsKeyDown(Keys.S))
        {
            _objeto.Rotar(-_rotationSpeedX, Vector3.UnitX);
        }
    }

    private int CrearShaderProgram()
    {
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

        // Cargar los shaders desde archivos
        GL.ShaderSource(vertexShader, File.ReadAllText("C:\\Users\\usuario\\Desktop\\Prog Grafica\\OpenTK\\ShapeT_OpenTK\\bin\\Debug\\net8.0\\vertex_shader.glsl"));
        GL.ShaderSource(fragmentShader, File.ReadAllText("C:\\Users\\usuario\\Desktop\\Prog Grafica\\OpenTK\\ShapeT_OpenTK\\bin\\Debug\\net8.0\\fragment_shader.glsl"));

        GL.CompileShader(vertexShader);
        GL.CompileShader(fragmentShader);

        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);

        GL.DetachShader(shaderProgram, vertexShader);
        GL.DetachShader(shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return shaderProgram;
    }


}