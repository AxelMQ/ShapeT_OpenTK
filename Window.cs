using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Globalization;


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

    private readonly string _vertexShaderPath = "vertex_shader.glsl";
    private readonly string _fragmentShaderPath = "fragment_shader.glsl";

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        // Inicializar el escenario
        _escenario = new Escenario();

        // Crear el shader program
        int shaderProgram = CrearShaderProgram();

        ObjetoT(shaderProgram);

        ObjetoEjeCartesiano(shaderProgram);


    }

    protected override void OnLoad()
    {
        base.OnLoad();

        // Configurar las matrices de perspectiva y vista
        _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            (float)900 / 700,0.1f,100f);

        _viewMatrix = Matrix4.LookAt(new Vector3(3f, 2f, 3f),Vector3.Zero,Vector3.UnitY);

        GL.ClearColor(Color4.CornflowerBlue);
        GL.Enable(EnableCap.DepthTest);

        //Abrir("C:\\Users\\usuario\\Desktop\\Prog Grafica\\OpenTK\\FIGURAS\\path_to_save_file.obj");
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Dibujar el escenario
        _escenario.Dibujar(_viewMatrix, _projectionMatrix);

        SwapBuffers();
    }
    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Keys.G) // Por ejemplo, presiona 'G' para guardar los vértices
        {
            GuardarVertices("figuraOpenTK.obj");
        }
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
    private void ObjetoT(int shaderProgram)
    {
        // Crear el objeto y agregar las partes al escenario
        _objeto = new Objeto(shaderProgram);
        var barraSuperior = CrearParte(shaderProgram, DefinirVerticesBarraSuperior());
        barraSuperior.SetColor(new Vector3(0.5f, 0.2f, 0.7f));
        _objeto.AgregarParte(barraSuperior); 

        var barraVertical = CrearParte(shaderProgram, DefinirVerticesBarraVertical());
        barraVertical.SetColor(new Vector3(0.8f, 0.3f, 0.8f));
        _objeto.AgregarParte(barraVertical);

        _objeto.EstablecerPosicion(new Vector3(0.00000f, 0.0f, 0.0f));
        _escenario.AgregarObjeto(_objeto);
    }
    private void ObjetoEjeCartesiano(int shaderProgram)
    {
        // Crear el segundo objeto
        _objeto2 = new Objeto(shaderProgram);

        var ejeX = CrearParte(shaderProgram, DefinirVerticesEjeX(), PrimitiveType.Lines);
        ejeX.SetColor(new Vector3(1.0f, 1.0f, 0.0f)); // Amarillo
        _objeto2.AgregarParte(ejeX);

        var ejeY = CrearParte(shaderProgram, DefinirVerticesEjeY(), PrimitiveType.Lines);
        ejeY.SetColor(new Vector3(1.0f, 0.0f, 1.0f)); // Lila
        _objeto2.AgregarParte(ejeY);

        _escenario.AgregarObjeto(_objeto2);
    }
    private Parte CrearParte(int shaderProgram, (List<Vector3> vertices, List<uint> indices) definicionVerticesIndices, PrimitiveType primitiveType = PrimitiveType.Triangles)
    {
        // Crear el polígono
        Poligono poligono = new Poligono(
            definicionVerticesIndices.vertices,
            definicionVerticesIndices.indices,
            shaderProgram,
            new Vector3(1.0f, 0.0f, 0.0f),
            primitiveType
         ); // Color rojo por defecto

        // Crear la parte y agregar el polígono
        Parte parte = new Parte();
        parte.AgregarPoligono(poligono);

        return parte;

    }
    private (List<Vector3> vertices, List<uint> indices) DefinirVerticesEjeX()
    {
        var verticesEjeX = new List<Vector3>
        {
            new Vector3(-10.0f, 0.0f, 0.0f),
            new Vector3(10.0f, 0.0f, 0.0f)
        };

        var indicesEjeX = new List<uint>
        {
            0, 1
        };

        return (verticesEjeX, indicesEjeX);
    }
    private (List<Vector3> vertices, List<uint> indices) DefinirVerticesEjeY()
    {
        var verticesEjeY = new List<Vector3>
        {
            new Vector3(0.0f, -10.0f, 0.0f),
            new Vector3(0.0f, 10.0f, 0.0f)
        };

        var indicesEjeY = new List<uint>
        {
            0, 1
        };

        return (verticesEjeY, indicesEjeY);
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
    private int CrearShaderProgram()
    {
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

        // Cargar los shaders desde archivos
        GL.ShaderSource(vertexShader, File.ReadAllText(_vertexShaderPath));
        GL.ShaderSource(fragmentShader, File.ReadAllText(_fragmentShaderPath));

        GL.CompileShader(vertexShader);
        GL.CompileShader(fragmentShader);

        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return shaderProgram;
    }
    public void GuardarVertices(string path)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                int vertexOffset = 0; // Necesario para manejar múltiples objetos con diferentes vértices

                foreach (var objeto in _escenario.GetObjetos()) 
                {
                    foreach (var parte in objeto.GetPartes()) 
                    {
                        foreach (var poligono in parte.GetPoligonos())
                        {
                            // Verificar el tipo de primitiva antes de continuar
                            if (poligono.GetPrimitiveType() == PrimitiveType.Lines)
                            {
                                Console.WriteLine("Omitiendo el guardado de líneas en el archivo .obj");
                                continue; // Omitir figuras lineales
                            }

                            // 1. Guardar vértices
                            var vertices = poligono.GetVertices();
                            foreach (var vertice in vertices)
                            {
                                // Formatear los números con '.' como separador decimal.
                                writer.WriteLine($"v {vertice.X.ToString(CultureInfo.InvariantCulture)} {vertice.Y.ToString(CultureInfo.InvariantCulture)} {vertice.Z.ToString(CultureInfo.InvariantCulture)}");
                            }

                            // 2. Guardar índices (caras) ajustando por el `vertexOffset`
                            var indices = poligono.GetIndices().ToList();

                            // Asegurarnos de que sean triángulos
                            if (indices.Count % 3 != 0)
                            {
                                throw new InvalidOperationException("Los índices no forman triángulos válidos.");
                            }

                            for (int i = 0; i < indices.Count; i += 3)
                            {
                                writer.WriteLine($"f {indices[i] + 1 + vertexOffset} {indices[i + 1] + 1 + vertexOffset} {indices[i + 2] + 1 + vertexOffset}");
                            }

                            // Aumentar el offset de vértices para el siguiente polígono
                            vertexOffset += vertices.Count;
                        }
                    }
                }

                Console.WriteLine($"Vértices e índices guardados correctamente en {path}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar el archivo: {ex.Message}");
        }
    }
    public void Abrir(string path)
    {
        try
        {
            // Variables para almacenar los datos leídos
            List<Vector3> vertices = new List<Vector3>();
            List<uint> indices = new List<uint>();
            List<List<uint>> tempFaces = new List<List<uint>>(); // Para almacenar caras temporales
            PrimitiveType currentPrimitiveType = PrimitiveType.Triangles; // Por defecto

            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length == 0) continue;

                    switch (tokens[0])
                    {
                        case "v":
                            // Leer vértices
                            if (tokens.Length == 4 &&
                                float.TryParse(tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
                                float.TryParse(tokens[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var y) &&
                                float.TryParse(tokens[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
                            {
                                vertices.Add(new Vector3(x, y, z));
                            }
                            break;

                        case "f":
                            // Leer caras
                            List<uint> faceIndices = new List<uint>();
                            for (int i = 1; i < tokens.Length; i++)
                            {
                                if (uint.TryParse(tokens[i].Split('/')[0], out var index))
                                {
                                    faceIndices.Add(index - 1);
                                }
                            }
                            if (faceIndices.Count > 0)
                            {
                                tempFaces.Add(faceIndices);
                            }
                            break;

                        case "p":
                            // Polilíneas (si es necesario)
                            currentPrimitiveType = PrimitiveType.Lines;
                            break;

                        default:
                            // Manejar otros casos si es necesario
                            break;
                    }
                }
            }

            // Crear polígonos a partir de los datos leídos
            foreach (var face in tempFaces)
            {

                // Convertir las caras a triángulos si es necesario
                if (face.Count == 3)
                {
                    indices.AddRange(face);
                }
                else if (face.Count > 3)
                {
                    // Triangulación simple para caras de más de 3 vértices
                    for (int i = 1; i < face.Count - 1; i++)
                    {
                        indices.Add(face[0]);
                        indices.Add(face[i]);
                        indices.Add(face[i + 1]);
                    }
                }
            }

            // Crear el polígono y agregarlo a la escena
            var shaderProgram = CrearShaderProgram();
            var color = new Vector3(1.0f, 1.0f, 1.0f); // Color blanco
            var poligono = new Poligono(vertices, indices, shaderProgram, color, currentPrimitiveType);

            var parte = new Parte();
            parte.AgregarPoligono(poligono);

            var objeto = new Objeto(shaderProgram);
            objeto.AgregarParte(parte);

            _escenario.AgregarObjeto(objeto);

            Console.WriteLine($"Archivo {path} cargado correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al abrir el archivo: {ex.Message}");
        }
    }


}