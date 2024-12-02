using UnityEngine;

public class PiezaTetris : MonoBehaviour
{
    private GeneradorPiezasAleatorio spawner;
    private bool colisionDetectada = false;
    private AudioSource audioSource;
    public float multiplicadorVelocidadCaida = 0.01f;  // Disminuir este valor ralentiza la ca�da
    public AudioClip sonidoColision;  // Sonido espec�fico para la colisi�n

    void Start()
    {
        // Configuraci�n del AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sonidoColision;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;  // Hacer el sonido 3D (opcional)

        // Ajustar la gravedad para que sea m�s lenta
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.drag = 2f; // Ajusta este valor: mayor drag = ca�da m�s lenta
        }
    }

    public void AsignarSpawner(GeneradorPiezasAleatorio spawner)
    {
        this.spawner = spawner;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (colisionDetectada) return;

        if (collision.gameObject.CompareTag("Base") || collision.gameObject.CompareTag("Pieza"))
        {
            colisionDetectada = true;

            // Reproducir el sonido al colisionar
            if (audioSource != null && sonidoColision != null)
            {
                audioSource.Play();
            }

            // Notificar al spawner para generar otra pieza
            if (spawner != null)
            {
                spawner.PermitirNuevaPieza();
            }

            Destroy(this);  // Detener la l�gica de esta pieza
        }
    }
}

