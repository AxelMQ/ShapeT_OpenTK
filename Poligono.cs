using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

public class Poligono
{
    private List<Vector3> _vertices;
    private List<uint> _indices;
    private int _vertexArray;
    private int _vertexBuffer;
    private int _elementBuffer;
    private int _shaderProgram;
    private int _indexCount;
    private Vector3 _color;
    private Matrix4 _matrizTransformacion = Matrix4.Identity;
    private PrimitiveType _primitiveType;

    public Poligono(List<Vector3> vertices, List<uint> indices, int shaderProgram, Vector3 color, PrimitiveType primitiveType = PrimitiveType.Triangles)
    {
        _vertices = vertices;
        _indices = indices;
        _shaderProgram = shaderProgram;
        _indexCount = _indices.Count;
        _primitiveType = primitiveType;
        _color = color;
        InicializarBuffers();
    }

    private void InicializarBuffers()
    {
        _vertexArray = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArray);

        _vertexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);

        _elementBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBuffer);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);

        int vertexLocation = GL.GetAttribLocation(_shaderProgram, "aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

        GL.BindVertexArray(0); // Desvincular el VAO
    }

    public void Dibujar(Matrix4 matrizModelo, Matrix4 matrizVista, Matrix4 matrizProyeccion)
    {
        GL.UseProgram(_shaderProgram);

        // Calcular la matriz final para este polígono
        Matrix4 matrizFinal = _matrizTransformacion;

        // Enviar las matrices al shader
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "model"), false, ref matrizModelo);
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "view"), false, ref matrizVista);
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "projection"), false, ref matrizProyeccion);

        int colorLocation = GL.GetUniformLocation(_shaderProgram, "color");
        GL.Uniform3(colorLocation, ref _color);

        GL.BindVertexArray(_vertexArray);
        GL.DrawElements(_primitiveType, _indexCount, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0); // Desvincular el VAO después de dibujar
    }
    public void AplicarTransformacion(Matrix4 transformacion)
    {
        _matrizTransformacion *= transformacion;

        for (int i = 0; i < _vertices.Count; i++)
        {
            _vertices[i] = Vector3.TransformPosition(_vertices[i], transformacion);
        }
        ActualizarBuffers();
    }
    public void SetModelMatrix(Matrix4 modelMatrix)
    {
        _matrizTransformacion = modelMatrix;
    }
    public void SetColor(Vector3 color)
    {
        _color = color;
    }

    private void ActualizarBuffers()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.BufferSubData(BufferTarget.ArrayBuffer, 
                        IntPtr.Zero,
                        _vertices.Count * Vector3.SizeInBytes, 
                        _vertices.ToArray());
    }
    public Vector3 CalcularCentroDeMasa()
    {
        Vector3 centroDeMasa = Vector3.Zero;
        foreach (var vertice in _vertices)
        {
            centroDeMasa += vertice;
        }
        return centroDeMasa / _vertices.Count;
    }
    public List<Vector3> GetVertices()
    {
        return _vertices;
    }
    public List<uint> GetIndices()
    {
        return _indices;
    }
    public PrimitiveType GetPrimitiveType()
    {
        return _primitiveType;
    }
    public void Dispose()
    {
        GL.DeleteVertexArray(_vertexArray);
        GL.DeleteBuffer(_vertexBuffer);
        GL.DeleteBuffer(_elementBuffer);
    }
}
