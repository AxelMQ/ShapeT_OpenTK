using OpenTK.Mathematics;
using System.Collections.Generic;

public class Parte
{
    private List<Poligono> _poligonos;
    private Matrix4 _modelMatrix;
    private Vector3 _posicion;
    private Vector3 _rotacion;

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


    public void Dibujar(Matrix4 modelMatrix, Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        // Lógica de renderizado
        foreach (var poligono in _poligonos)
        {
            poligono.Dibujar(modelMatrix, viewMatrix, projectionMatrix);
        }
    }

    public void AplicarTransformacion(Matrix4 transformacion)
    {
        foreach (var poligono in _poligonos)
        {
            poligono.AplicarTransformacion(transformacion);
        }
        ActualizarMatrizModelo();
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
