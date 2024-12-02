using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    public GeneradorPiezasAleatorio generadorPiezas;

    void Start()
    {
        if (generadorPiezas == null)
        {
            generadorPiezas = FindObjectOfType<GeneradorPiezasAleatorio>();
        }
    }

    public void ReiniciarJuego()
    {
        // Limpiar todas las piezas activas en la escena
        foreach (GameObject pieza in GameObject.FindGameObjectsWithTag("Pieza"))
        {
            Destroy(pieza);
        }

        // Resetear el generador para empezar desde cero
        generadorPiezas.ResetearEstado();

        // Generar la primera pieza como al inicio
        generadorPiezas.GenerarNuevaPieza();

        Debug.Log("Juego reiniciado desde el principio.");
    }
}

