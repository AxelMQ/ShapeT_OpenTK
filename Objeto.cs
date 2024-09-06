using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

public class Objeto
{
    private List<Parte> _partes;
    private Vector3 _posicion;
    private Vector3 _rotacion;
    private Vector3 _centroDeMasa;

    private Matrix4 _modelMatrix;
    private int _shaderProgram;

    public Objeto(int shaderProgram)
    {
        _partes = new List<Parte>();
        _posicion = Vector3.Zero;
        _rotacion = Vector3.Zero;
        _modelMatrix = Matrix4.Identity;
        _shaderProgram = shaderProgram;
        CalcularCentroDeMasa();
    }

    public void AgregarParte(Parte parte)
    {
        _partes.Add(parte);
        CalcularCentroDeMasa();
    }

    public void InicializarRender()
    {
        GL.ClearColor(Color4.CornflowerBlue);
        GL.Enable(EnableCap.DepthTest);
    }

    public void Dibujar(Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        AplicarTransformaciones();

        foreach (var parte in _partes)
        {
            parte.Dibujar(_modelMatrix, viewMatrix, projectionMatrix, _shaderProgram);
        }
    }

    public void AplicarTransformaciones()
    {
        _modelMatrix = Matrix4.Identity;

        _modelMatrix *= Matrix4.CreateTranslation(_posicion);
        _modelMatrix *= Matrix4.CreateRotationZ(_rotacion.Z);
        _modelMatrix *= Matrix4.CreateRotationY(_rotacion.Y);
        _modelMatrix *= Matrix4.CreateRotationX(_rotacion.X);

        foreach (var parte in _partes)
        {
            parte.AplicarTransformacion(_modelMatrix);
        }
    }
    public void AplicarTransformacion(Matrix4 transformacion)
    {
        _modelMatrix *= transformacion;
        foreach (var parte in _partes)
        {
            parte.AplicarTransformacion(_modelMatrix);
        }
    }

    public void Trasladar(Vector3 desplazamiento)
    {
        _posicion += desplazamiento;
        AplicarTransformaciones();
    }

    public void TrasladarX(float cantidad)
    {
        _posicion.X += cantidad;
        AplicarTransformaciones();
    }

    public void TrasladarY(float cantidad)
    {
        _posicion.Y += cantidad;
        AplicarTransformaciones();
    }


    public void EstablecerPosicion(Vector3 nuevaPosicion)
    {
        _posicion = nuevaPosicion;
        ActualizarModelo();
    }

    private void ActualizarModelo()
    {
        _modelMatrix = Matrix4.CreateTranslation(_posicion) *
                       Matrix4.CreateRotationX(_rotacion.X) *
                       Matrix4.CreateRotationY(_rotacion.Y) *
                       Matrix4.CreateRotationZ(_rotacion.Z);
    }

    public void Rotar(float angulo, Vector3 eje)
    {
        // Crear una matriz de rotación para el ángulo dado y el eje especificado
        var rotacion = Matrix4.CreateFromAxisAngle(eje, angulo);

        // Aplicar la rotación a la matriz del modelo
        _modelMatrix *= rotacion;

        // Aplicar la rotación a cada parte del objeto
        foreach (var parte in _partes)
        {
            parte.AplicarTransformacion(_modelMatrix);
        }
    }
    private void CalcularCentroDeMasa()
    {
        Vector3 sumaCentros = Vector3.Zero;
        int conteoVertices = 0;

        foreach (var parte in _partes)
        {
            foreach (var poligono in parte.GetPoligonos())
            {
                foreach (var vertex in poligono.GetVertices())
                {
                    sumaCentros += vertex;
                    conteoVertices++;
                }
            }
        }

        if (conteoVertices > 0)
        {
            _centroDeMasa = sumaCentros / conteoVertices;
        }
        else
        {
            _centroDeMasa = Vector3.Zero; // Por si no hay vértices
        }
    }
    public List<Parte> GetPartes()
    {
        return _partes;
    }
    public void Dispose()
    {
        foreach (var parte in _partes)
        {
            parte.ResetearTransformaciones();
        }
        GL.DeleteProgram(_shaderProgram);
    }
}
