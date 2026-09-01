using UnityEngine;
using System.Collections;

public class ObjMovil : MonoBehaviour
{
    public Transform puntoInicio;
    public Transform puntoFinal;

    [Header("Movimiento")]
    public float velocidadMaxima = 5f;
    public float aceleracion = 2f;

    [Header("Pausas")]
    public float pausaEnInicio = 1f;
    public float pausaEnFinal = 1f;

    void Start()
    {
        StartCoroutine(MoverObjeto());
    }

    IEnumerator MoverObjeto()
    {
        while (true)
        {
            // Espera en el inicio
            yield return new WaitForSeconds(pausaEnInicio);

            // INICIO → FINAL
            yield return StartCoroutine(MoverConAceleracion(puntoFinal));

            // Espera en el final
            yield return new WaitForSeconds(pausaEnFinal);

            // FINAL → INICIO
            yield return StartCoroutine(MoverConAceleracion(puntoInicio));
        }
    }

    IEnumerator MoverConAceleracion(Transform destino)
    {
        float velocidadActual = 0f;

        while (Vector3.Distance(transform.position, destino.position) > 0.01f)
        {
            // Acelera progresivamente
            velocidadActual += aceleracion * Time.deltaTime;

            // No supera la velocidad máxima
            velocidadActual = Mathf.Min(
                velocidadActual,
                velocidadMaxima
            );

            // Se mueve usando la velocidad actual
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino.position,
                velocidadActual * Time.deltaTime
            );

            yield return null;
        }

        // Llega exactamente al nodo y se detiene DE GOLPE
        transform.position = destino.position;
    }
}