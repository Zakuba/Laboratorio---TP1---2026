using UnityEngine;
using Unity.Netcode;

public class BolaDeNieveOnline : NetworkBehaviour
{
    public float fuerzaEmpuje = 15f;
    public float tiempoDeVida = 5f;

    [SerializeField] private GameObject particulasExplosion;

    public override void OnNetworkSpawn()
    {
        Rigidbody rigidbodyBola = GetComponent<Rigidbody>();

        if (rigidbodyBola != null)
        {
            rigidbodyBola.isKinematic = !IsServer;
        }

        if (IsServer)
        {
            Invoke(nameof(DestruirEnRed), tiempoDeVida);
        }
    }

    private void OnCollisionEnter(Collision colision)
    {
        if (!IsServer) return;

        // Obtenemos el punto exacto donde ocurrió el impacto
        Vector3 posicionImpacto = colision.contacts[0].point;

        // Avisamos a todos los clientes que reproduzcan la explosión
        ReproducirExplosionClientRpc(posicionImpacto);

        if (colision.gameObject.CompareTag("Player"))
        {
            PlayerMovimientoOnline jugadorGolpeado =
                colision.gameObject.GetComponentInParent<PlayerMovimientoOnline>();

            if (jugadorGolpeado != null)
            {
                Vector3 direccionEmpuje =
                    jugadorGolpeado.transform.position - transform.position;

                direccionEmpuje.y = Mathf.Max(direccionEmpuje.y, 0.35f);

                if (direccionEmpuje.sqrMagnitude > 0.0001f)
                {
                    jugadorGolpeado.AplicarKnockbackDesdeServidor(
                        direccionEmpuje.normalized * fuerzaEmpuje
                    );
                }
            }
        }

        DestruirEnRed();
    }

    [ClientRpc]
    private void ReproducirExplosionClientRpc(Vector3 posicion)
    {
        if (particulasExplosion == null) return;

        GameObject explosion =
            Instantiate(particulasExplosion, posicion, Quaternion.identity);

        // Si el Particle System no se destruye solo
        Destroy(explosion, 2f);
    }

    private void DestruirEnRed()
    {
        CancelInvoke(nameof(DestruirEnRed));

        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
}