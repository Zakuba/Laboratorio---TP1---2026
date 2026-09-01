using UnityEngine;
using Unity.Netcode;

public class BolaDeNieveOnline : NetworkBehaviour 
{
    public float fuerzaEmpuje = 15f;
    public float tiempoDeVida = 5f;

    public override void OnNetworkSpawn()
    {
        Rigidbody rigidbodyBola = GetComponent<Rigidbody>();
        if (rigidbodyBola != null)
        {
            rigidbodyBola.isKinematic = !IsServer;
        }

        // CAMBIO CLAVE: Si yo disparé esto (!IsServer && IsOwner), ya estoy viendo mi bola visual falsa.
        // Por lo tanto, oculto esta bola "real" que me manda el servidor para no verla doble ni lagueada.
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
        if (!IsServer) return;

        if (colision.gameObject.CompareTag("Player"))
        {
            PlayerMovimientoOnline jugadorGolpeado = colision.gameObject.GetComponentInParent<PlayerMovimientoOnline>();

            if (jugadorGolpeado != null)
            {
                Vector3 direccionEmpuje = jugadorGolpeado.transform.position - transform.position;
                direccionEmpuje.y = Mathf.Max(direccionEmpuje.y, 0.35f);

                if (direccionEmpuje.sqrMagnitude > 0.0001f)
                {
                    jugadorGolpeado.AplicarKnockbackDesdeServidor(direccionEmpuje.normalized * fuerzaEmpuje);
                }
            }
        }

        DestruirEnRed();
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