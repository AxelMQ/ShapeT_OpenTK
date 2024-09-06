using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
public class Parte
{
    private List<Poligono> _poligonos;
    private Matrix4 _modelMatrix;
    private Vector3 _posicion;
    private Vector3 _rotacion;

    private Vector3 color;

    public Parte()
    {
        _poligonos = new List<Poligono>();
        _modelMatrix = Matrix4.Identity;
        _posicion = Vector3.Zero;
        _rotacion = Vector3.Zero;
    }

    public void AgregarPoligono(Poligono poligono)
    {
        _poligonos.Add(poligono);
    }
    public IEnumerable<Poligono> GetPoligonos()
    {
        return _poligonos;
    }
    public void SetColor(Vector3 color)
    {
        foreach (var poligono in _poligonos)
        {
            poligono.SetColor(color);
        }
    }
    public void Dibujar(Matrix4 matrizModelo, Matrix4 matrizVista, Matrix4 matrizProyeccion, int shaderProgram)
    {
        ActualizarMatrizModelo();

        GL.UseProgram(shaderProgram);

        Matrix4 finalModelMatrix = matrizModelo * _modelMatrix;

        foreach (var poligono in _poligonos)
        {
            poligono.Dibujar(finalModelMatrix, matrizVista, matrizProyeccion);
        }
    }

    public void AplicarTransformacion(Matrix4 transformacion)
    {
        _modelMatrix *= transformacion;

        foreach (var poligono in _poligonos)
        {
            poligono.AplicarTransformacion(_modelMatrix);
        }
    }
    private void ActualizarMatrizModelo()
    {
        _modelMatrix = Matrix4.CreateTranslation(_posicion) *
                       Matrix4.CreateRotationX(_rotacion.X) *
                       Matrix4.CreateRotationY(_rotacion.Y) *
                       Matrix4.CreateRotationZ(_rotacion.Z);
    }

    public void ResetearTransformaciones()
    {
        _posicion = Vector3.Zero;
        _rotacion = Vector3.Zero;
        _modelMatrix = Matrix4.Identity;
    }
}