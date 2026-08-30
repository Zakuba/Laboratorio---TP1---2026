using UnityEngine;
using Unity.Netcode;

public class Snowball : NetworkBehaviour
{
    public float fuerzaEmpuje = 15f;
    public float tiempoDeVida = 5f; // Despawnea tras 5s si se lanza al vacío

    public override void OnNetworkSpawn()
    {
        // Solo el servidor gestiona la destrucción por tiempo
        if (IsServer)
        {
            Invoke(nameof(DespawnBola), tiempoDeVida);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Solo el servidor procesa el impacto y destruye el objeto
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rbObjetivo =
                collision.gameObject.GetComponent<Rigidbody>();

            if (rbObjetivo != null)
            {
                Vector3 direccionEmpuje =
                    collision.transform.position - transform.position;

                direccionEmpuje.y = 0.5f;
                direccionEmpuje.Normalize();

                rbObjetivo.AddForce(
                    direccionEmpuje * fuerzaEmpuje,
                    ForceMode.Impulse
                );
            }
        }

        DespawnBola();
    }

    private void DespawnBola()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
}