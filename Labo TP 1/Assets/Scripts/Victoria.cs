using UnityEngine;

public class Victoria : MonoBehaviour
{
    [SerializeField] private Transform puntoVictoria;
    [SerializeField] private Camera camVictoria;
    [SerializeField] private Camera camaraMenu;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerMovimientoOnline jugador =
            other.GetComponentInParent<PlayerMovimientoOnline>();

        if (jugador != null && jugador.IsOwner)
        {
            Debug.Log("VICTORIA: jugador detectado");

            // Teletransportar
            jugador.Teletransportar(puntoVictoria);

            // Bloquear movimiento
            jugador.BloquearMovimiento();

            // Desactivar cámara del jugador
            CameraFollowOnline camaraJugador =
                jugador.GetComponentInChildren<CameraFollowOnline>();

            if (camaraJugador != null)
            {
                Debug.Log("VICTORIA: cámara del jugador encontrada");

                camaraJugador.enabled = false;

                Camera camaraNormal =
                    camaraJugador.GetComponent<Camera>();

                if (camaraNormal != null)
                {
                    camaraNormal.enabled = false;
                    Debug.Log("VICTORIA: cámara del jugador desactivada");
                }
            }

            // Desactivar cámara del menú
            if (camaraMenu != null)
            {
                camaraMenu.enabled = false;
                Debug.Log("VICTORIA: cámara menú desactivada");
            }

            // Activar cámara de victoria
            if (camVictoria != null)
            {
                Debug.Log("VICTORIA: ACTIVANDO CAMARA VICTORIA");

                camVictoria.gameObject.SetActive(true);
                camVictoria.enabled = true;

                Debug.Log(
                    "CamVictoria GameObject activo = " +
                    camVictoria.gameObject.activeInHierarchy
                );

                Debug.Log(
                    "CamVictoria enabled = " +
                    camVictoria.enabled
                );
            }
        }
    }
}