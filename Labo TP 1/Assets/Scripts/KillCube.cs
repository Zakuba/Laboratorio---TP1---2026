using UnityEngine;

public class KillCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("KillCube detectó: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("¡MATANDO AL JUGADOR!");

            PlayerMovimiento jugador = other.GetComponent<PlayerMovimiento>();

            if (jugador != null)
            {
                jugador.Reaparecer();
            }
        }
    }
}