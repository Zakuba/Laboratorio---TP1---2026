using UnityEngine;

public class KillCubeSingleplayer : MonoBehaviour
{
    [Header("Punto de reaparición")]
    [SerializeField] private Transform puntoReaparicion;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        CharacterController controlador =
            other.GetComponent<CharacterController>();

        if (controlador == null)
            return;

        if (puntoReaparicion == null)
        {
            Debug.LogWarning(
                "KillCubeSingleplayer: no hay un punto de reaparición asignado."
            );
            return;
        }

        controlador.enabled = false;

        other.transform.position = puntoReaparicion.position;

        controlador.enabled = true;

        Debug.Log("Jugador reaparecido.");
    }
}