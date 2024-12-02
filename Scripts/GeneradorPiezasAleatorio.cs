using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;

public class GeneradorPiezasAleatorio : MonoBehaviour
{
    public GameObject[] misObjetos;                // Prefabs de piezas
    public ARRaycastManager administradorRayosAR;  // Para detectar el plano AR
    public ARPlaneManager administradorPlanos;     // Para gestionar los planos AR
    public ReceptorPieza receptorPieza;            // Referencia al script ReceptorPieza
    private List<ARRaycastHit> impactos = new List<ARRaycastHit>();
    private ARPlane planoDetectado;                // Plano AR detectado
    private bool planoDetectadoFlag = false;       // Bandera para detectar solo un plano
    private bool puedeGenerarNuevaPieza = true;    // Controla si se puede generar una nueva pieza
    private float alturaGeneracion = 5.0f;         // Altura inicial de generación de las piezas
    private Vector3 posicionFijaGeneracion;        // Posición fija en el centro del plano

    // Variables de avalancha
    private int contadorColisiones = 0;            // Contador de colisiones
    public int limiteColisionesAvalancha = 5;      // Colisiones para iniciar avalancha
    public float intervaloAvalancha = 0.2f;        // Intervalo entre piezas en avalancha
    private bool enAvalancha = false;              // Estado de la avalancha

    void Update()
    {
        if (!planoDetectadoFlag && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            DetectarPlano();
        }
    }

    void DetectarPlano()
    {
        if (administradorRayosAR.Raycast(Input.GetTouch(0).position, impactos, TrackableType.Planes))
        {
            Pose posicionImpacto = impactos[0].pose;
            planoDetectado = administradorRayosAR.GetComponent<ARPlaneManager>().GetPlane(impactos[0].trackableId);

            if (!planoDetectadoFlag)
            {
                posicionFijaGeneracion = new Vector3(
                    planoDetectado.center.x,
                    planoDetectado.center.y + alturaGeneracion,
                    planoDetectado.center.z
                );

                planoDetectadoFlag = true;
                administradorPlanos.enabled = false;  // Desactivar la detección de nuevos planos
                Debug.Log("Plano detectado y búsqueda de planos desactivada.");

                // Generar la primera pieza automáticamente
                GenerarNuevaPieza();
            }
        }
    }

    public void GenerarNuevaPieza()
    {
        if (planoDetectado != null)
        {
            Vector3 posicionGeneracion = posicionFijaGeneracion;
            int indiceAleatorio = Random.Range(0, misObjetos.Length);
            GameObject nuevoObjeto = Instantiate(misObjetos[indiceAleatorio], posicionGeneracion, Quaternion.identity);

            Rigidbody rb = nuevoObjeto.GetComponent<Rigidbody>();
            if (rb == null) rb = nuevoObjeto.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            PiezaTetris piezaTetris = nuevoObjeto.AddComponent<PiezaTetris>();
            piezaTetris.AsignarSpawner(this);
            receptorPieza.AsignarPiezaActual(nuevoObjeto, false);

            puedeGenerarNuevaPieza = false;
        }
    }

    public void PermitirNuevaPieza()
    {
        contadorColisiones++;

        if (contadorColisiones >= limiteColisionesAvalancha && !enAvalancha)
        {
            IniciarAvalancha();
        }
        else
        {
            GenerarNuevaPieza();  // Generar una nueva pieza automáticamente
        }
    }

    private void IniciarAvalancha()
    {
        Debug.Log("¡Avalancha iniciada!");
        enAvalancha = true;
        StartCoroutine(GenerarPiezasAvalancha());
    }

    private IEnumerator GenerarPiezasAvalancha()
    {
        while (true)
        {
            Vector3 posicionGeneracion = posicionFijaGeneracion;
            int indiceAleatorio = Random.Range(0, misObjetos.Length);
            GameObject nuevoObjeto = Instantiate(misObjetos[indiceAleatorio], posicionGeneracion, Quaternion.identity);

            Rigidbody rb = nuevoObjeto.GetComponent<Rigidbody>();
            if (rb == null) rb = nuevoObjeto.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            receptorPieza.AsignarPiezaActual(nuevoObjeto, false);
            yield return new WaitForSeconds(intervaloAvalancha);
        }
    }

    public void ResetearEstado()
    {
        // Detener la avalancha
        StopAllCoroutines();
        enAvalancha = false;

        // Reiniciar variables internas
        contadorColisiones = 0;
        puedeGenerarNuevaPieza = true;

        // Cambiar el estado del juego a Normal
        GameManager.Instance.estadoActual = EstadoJuego.Normal;

        Debug.Log("Estado del generador reiniciado, listo para comenzar desde cero.");
    }



}
