using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Transform puntoReaparicion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovimientoOnline jugador =
                other.GetComponentInParent<PlayerMovimientoOnline>();

            if (jugador != null && jugador.IsOwner)
            {
                jugador.ActualizarCheckpoint(puntoReaparicion);
            }
        }
    }
}