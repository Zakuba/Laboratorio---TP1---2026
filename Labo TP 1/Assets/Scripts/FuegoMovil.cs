using UnityEngine;
using System.Collections;

public class FuegoMovil : MonoBehaviour
{
    [Header("Nodos")]
    public Transform puntoInicio;
    public Transform puntoFinal;

    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Pausas")]
    public float pausaEnInicio = 1f;
    public float pausaEnFinal = 1f;

    [Header("Rastro")]
    public GameObject particulaFuego;
    public float frecuenciaRastro = 0.05f;
    public float vidaRastro = 0.5f;

    private bool dejandoRastro = false;

    void Start()
    {
        transform.position = puntoInicio.position;
        StartCoroutine(MoverFuego());
    }

    IEnumerator MoverFuego()
    {
        while (true)
        {
            // 🔥 Espera en el inicio
            dejandoRastro = false;

            yield return new WaitForSeconds(pausaEnInicio);

            // =========================================
            // 🔥 INICIO → FIN
            // =========================================

            dejandoRastro = true;

            while (Vector3.Distance(transform.position, puntoFinal.position) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    puntoFinal.position,
                    velocidad * Time.deltaTime
                );

                // 🔥 Deja rastro mientras avanza
                if (dejandoRastro)
                {
                    GameObject rastro = Instantiate(
                        particulaFuego,
                        transform.position,
                        transform.rotation
                    );

                    Destroy(rastro, vidaRastro);
                }

                yield return new WaitForSeconds(frecuenciaRastro);
            }

            transform.position = puntoFinal.position;

            // =========================================
            // 🔥 LLEGÓ AL FINAL
            // =========================================

            dejandoRastro = false;

            yield return new WaitForSeconds(pausaEnFinal);

            // =========================================
            // 🔥 TELETRANSPORTE AL INICIO
            // =========================================

            transform.position = puntoInicio.position;
        }
    }
}