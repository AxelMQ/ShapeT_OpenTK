using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

public class Poligono
{
    private List<Vector3> _vertices;
    private List<uint> _indices;
    private int _vertexArray;
    private int _vertexBuffer;
    private int _elementBuffer;
    private int _shaderProgram;
    private int _indexCount;

    public Poligono(List<Vector3> vertices, List<uint> indices, int shaderProgram)
    {
        _vertices = vertices;
        _indices = indices;
        _shaderProgram = shaderProgram;
        _indexCount = _indices.Count;

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

    public void Dibujar(Matrix4 modelMatrix, Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        GL.UseProgram(_shaderProgram);

        // Enviar las matrices al shader
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "model"), false, ref modelMatrix);
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "view"), false, ref viewMatrix);
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "projection"), false, ref projectionMatrix);

        GL.BindVertexArray(_vertexArray);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0); // Desvincular el VAO después de dibujar
    }

    public void AplicarTransformacion(Matrix4 transformacion)
    {
        for (int i = 0; i < _vertices.Count; i++)
        {
            _vertices[i] = Vector3.TransformPosition(_vertices[i], transformacion);
        }
        ActualizarBuffers();
    }

    private void ActualizarBuffers()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray());
    }

    public IEnumerable<Vector3> GetVertices()
    {
        return _vertices;
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(_vertexArray);
        GL.DeleteBuffer(_vertexBuffer);
        GL.DeleteBuffer(_elementBuffer);
    }
}
