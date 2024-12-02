using UnityEngine;
using extOSC;

public class ReceptorPieza : MonoBehaviour
{
    public OSCReceiver receptorOSC;
    private GameObject piezaActual;
    private bool usarAcelerometro = false;

    private float[] posicionesX = { -0.2f, 0.0f, 0.2f };  // Tres posiciones posibles en X
    private int indiceActual = 1;                         // Posición inicial en el centro
    private float posicionZFija;
    public float velocidadDeslizamiento = 0.05f;          // Reducir la velocidad para mayor precisión
    public float sensibilidadMovimiento = 0.5f;          // Ajustar sensibilidad del cambio de carril

    void Start()
    {
        receptorOSC.Bind("/pieza/moverX", RecibirMovimientoX);
    }

    public void AsignarPiezaActual(GameObject pieza, bool esPrimeraPieza)
    {
        piezaActual = pieza;
        usarAcelerometro = !esPrimeraPieza;
        posicionZFija = pieza.transform.position.z;
    }

    void Update()
    {
        if (piezaActual != null)
        {
            // Mover suavemente hacia la posición del carril actual
            Vector3 posicionObjetivo = new Vector3(posicionesX[indiceActual], piezaActual.transform.position.y, posicionZFija);
            piezaActual.transform.position = Vector3.MoveTowards(piezaActual.transform.position, posicionObjetivo, velocidadDeslizamiento * Time.deltaTime);
        }
    }

    void RecibirMovimientoX(OSCMessage mensaje)
    {
        if (!usarAcelerometro || piezaActual == null) return;

        if (mensaje.ToFloat(out float movimientoX))
        {
            if (movimientoX > sensibilidadMovimiento && indiceActual < 2)
            {
                indiceActual++;
            }
            else if (movimientoX < -sensibilidadMovimiento && indiceActual > 0)
            {
                indiceActual--;
            }

            Debug.Log($"Moviendo a posición X del carril: {posicionesX[indiceActual]}");
        }
    }

    public void DesactivarAcelerometro()
    {
        usarAcelerometro = false;
        piezaActual = null;
        Debug.Log("Acelerómetro desactivado.");
    }
}
