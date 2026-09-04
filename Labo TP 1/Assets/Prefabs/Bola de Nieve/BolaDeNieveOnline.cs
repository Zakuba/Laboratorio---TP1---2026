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
            // El servidor maneja las físicas, los clientes solo lo ven moverse
            rigidbodyBola.isKinematic = !IsServer;
        }

        // 1. CAMBIO VISUAL: Si yo disparé esto (!IsServer && IsOwner), ya estoy viendo mi bola falsa.
        // Por lo tanto, apago el modelo 3D de esta bola real para no ver doble.
        if (IsOwner && !IsServer)
        {
            Renderer renderBola = GetComponent<Renderer>();
            if (renderBola != null)
            {
                renderBola.enabled = false;
            }
        }

        if (IsServer)
        {
            Invoke(nameof(DestruirEnRed), tiempoDeVida);
        }
    }

    private void OnCollisionEnter(Collision colision)
    {
        if (colision.gameObject.CompareTag("Player"))
        {
            NetworkObject netObjGolpeado = colision.gameObject.GetComponentInParent<NetworkObject>();

            if (netObjGolpeado != null)
            {
                Vector3 direccionEmpuje = netObjGolpeado.transform.position - transform.position;
                direccionEmpuje.y = Mathf.Max(direccionEmpuje.y, 0.35f); 
                Vector3 fuerzaFinal = direccionEmpuje.normalized * fuerzaEmpuje;

                // PREDICCIÓN LOCAL
                if (IsClient && netObjGolpeado.IsOwner)
                {
                    var movimiento = netObjGolpeado.GetComponent<PlayerMovimientoOnline>();
                    if (movimiento != null)
                    {
                        movimiento.AplicarKnockbackPredictedLocal(fuerzaFinal);
                    }
                }

                // AUTORIDAD DEL SERVIDOR
                if (IsServer)
                {
                    var movimiento = netObjGolpeado.GetComponent<PlayerMovimientoOnline>();
                    if (movimiento != null)
                    {
                        movimiento.AplicarKnockbackServerAuthoritative(fuerzaFinal);
                    }
                }
            }
        }

        // 2. CAMBIO DE EXPLOSIÓN: Solo el servidor llama al RPC de explotar y luego destruye la bola
        if (IsServer)
        {
            // Avisamos a todos los clientes que reproduzcan las partículas antes de borrar la bola
            ReproducirExplosionClientRpc(transform.position);
            DestruirEnRed();
        }
    }

    [ClientRpc]
    private void ReproducirExplosionClientRpc(Vector3 posicion)
    {
        if (particulasExplosion == null) return;

        GameObject explosion = Instantiate(particulasExplosion, posicion, Quaternion.identity);

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