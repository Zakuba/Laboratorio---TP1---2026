using UnityEngine;

public class SlimeCameraEffect : MonoBehaviour
{
    [SerializeField] private float fovNormal = 60f;
    [SerializeField] private float fovSlime = 40f;
    [SerializeField] private float velocidadCambio = 5f;

    private Camera camaraJugador;
    private bool dentroDelSlime = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerMovimientoOnline jugador =
            other.GetComponentInParent<PlayerMovimientoOnline>();

        if (jugador != null && jugador.IsOwner)
        {
            camaraJugador =
                jugador.GetComponentInChildren<Camera>();

            if (camaraJugador != null)
                dentroDelSlime = true;
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
            dentroDelSlime = false;
        }
    }

    private void Update()
    {
        if (camaraJugador == null)
            return;

        float fovObjetivo = dentroDelSlime ? fovSlime : fovNormal;

        camaraJugador.fieldOfView = Mathf.Lerp(
            camaraJugador.fieldOfView,
            fovObjetivo,
            velocidadCambio * Time.deltaTime
        );
    }
}