using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EfectoHielo : MonoBehaviour
{
    [SerializeField] private GameObject canvasHielo;
    [SerializeField] private Image imagenHielo;

    [Header("Transparencia")]
    [SerializeField, Range(0f, 1f)] private float opacidadMaxima = 0.35f;

    [Header("Transición")]
    [SerializeField] private float velocidadFade = 2f;

    private Coroutine transicion;

    private void Start()
    {
        // Empieza apagado
        canvasHielo.SetActive(false);

        Color color = imagenHielo.color;
        color.a = 0f;
        imagenHielo.color = color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerMovimientoOnline jugador =
            other.GetComponentInParent<PlayerMovimientoOnline>();

        if (jugador != null && jugador.IsOwner)
        {
            canvasHielo.SetActive(true);

            IniciarTransicion(opacidadMaxima);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerMovimientoOnline jugador =
            other.GetComponentInParent<PlayerMovimientoOnline>();

        if (jugador != null && jugador.IsOwner)
        {
            IniciarTransicion(0f);
        }
    }

    private void IniciarTransicion(float opacidadObjetivo)
    {
        if (transicion != null)
            StopCoroutine(transicion);

        transicion = StartCoroutine(CambiarOpacidad(opacidadObjetivo));
    }

    private IEnumerator CambiarOpacidad(float opacidadObjetivo)
    {
        Color color = imagenHielo.color;

        while (!Mathf.Approximately(color.a, opacidadObjetivo))
        {
            color.a = Mathf.MoveTowards(
                color.a,
                opacidadObjetivo,
                velocidadFade * Time.deltaTime
            );

            imagenHielo.color = color;

            yield return null;
        }

        color.a = opacidadObjetivo;
        imagenHielo.color = color;

        // Cuando terminó de desaparecer, apagamos el Canvas
        if (opacidadObjetivo == 0f)
        {
            canvasHielo.SetActive(false);
        }

        transicion = null;
    }
}