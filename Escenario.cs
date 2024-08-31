using OpenTK.Mathematics;
using System.Collections.Generic;

public class Escenario
{
    private List<Objeto> _objetos;

    public Escenario()
    {
        _objetos = new List<Objeto>();
    }

    public void AgregarObjeto(Objeto objeto)
    {
        _objetos.Add(objeto);
    }

    public void Dibujar(Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        foreach (var objeto in _objetos)
        {
            objeto.Dibujar(viewMatrix, projectionMatrix);
        }
    }

    public void Actualizar()
    {
        foreach (var objeto in _objetos)
        {
            objeto.AplicarTransformaciones();
        }
    }
}
