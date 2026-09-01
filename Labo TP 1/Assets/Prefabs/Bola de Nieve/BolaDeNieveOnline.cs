using UnityEngine;
using Unity.Netcode;

public class BolaDeNieveOnline : NetworkBehaviour
{
    public float fuerzaEmpuje = 15f;
    public float tiempoDeVida = 5f;
    
    [HideInInspector] 
    public ulong idDisparador; // Aquí guardamos el ID del que disparó

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Invoke(nameof(DestruirEnRed), tiempoDeVida);
        }
    }

    private void OnCollisionEnter(Collision colision)
    {
        if (!IsServer) return;

        // Buscamos el script de movimiento en el objeto golpeado o en sus padres (Infalible)
        PlayerMovimientoOnline jugador = colision.gameObject.GetComponentInParent<PlayerMovimientoOnline>();
        
        if (jugador != null)
        {
            // ¡MAGIA! Si el jugador golpeado es el MISMO que disparó, ignoramos la colisión
            if (jugador.OwnerClientId == idDisparador) 
            {
                return; // La bola sigue su camino, no se destruye
            }

            Debug.Log($"¡Impacto exitoso contra el Jugador {jugador.OwnerClientId}!");

        // Calculamos el empuje
        Vector3 direccionEmpuje = (colision.transform.position - transform.position).normalized;
        direccionEmpuje.y = 0.5f; 

        // Configuramos a quién le vamos a enviar la orden (solo al dueño del jugador golpeado)
        ClientRpcParams parametrosRpc = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { jugador.OwnerClientId }
            }
        };

        // Enviamos el RPC dirigido
        jugador.AplicarEmpujeClientRpc(direccionEmpuje.normalized * fuerzaEmpuje, parametrosRpc);
        }
        else
        {
            // Esto te ayudará a saber si la bola choca contra el suelo o una pared
            Debug.Log("La bola chocó contra algo que no es jugador: " + colision.gameObject.name);
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