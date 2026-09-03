using UnityEngine;

public class KillCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("KillCube detectó: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("¡MATANDO AL JUGADOR!");

            PlayerMovimientoOnline jugador =
                other.GetComponentInParent<PlayerMovimientoOnline>();

            if (jugador != null && jugador.IsOwner)
            {
                jugador.Reaparecer();
            }
        }
    }
}