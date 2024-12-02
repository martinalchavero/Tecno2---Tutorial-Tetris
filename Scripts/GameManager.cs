using UnityEngine;

public enum EstadoJuego { Normal, Avalancha }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void AcelerometroDesactivadoHandler();
    public event AcelerometroDesactivadoHandler OnAcelerometroDesactivado;

    public EstadoJuego estadoActual = EstadoJuego.Normal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DesactivarAcelerometro()
    {
        OnAcelerometroDesactivado?.Invoke();
        Debug.Log("Acelerómetro desactivado mediante GameManager.");
    }

    public void IniciarAvalancha()
    {
        estadoActual = EstadoJuego.Avalancha;
        DesactivarAcelerometro();
        Debug.Log("¡Modo avalancha activado!");
    }
}
