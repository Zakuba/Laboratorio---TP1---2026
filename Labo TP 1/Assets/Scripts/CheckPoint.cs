using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Transform puntoReaparicion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovimiento jugador = other.GetComponent<PlayerMovimiento>();

            if (jugador != null)
            {
                jugador.ActualizarCheckpoint(puntoReaparicion);
            }
        }
    }
}