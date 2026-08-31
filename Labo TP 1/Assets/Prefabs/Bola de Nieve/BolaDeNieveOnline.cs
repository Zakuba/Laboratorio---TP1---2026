using UnityEngine;
using Unity.Netcode; // 1. Importar Netcode

// 2. Heredar de NetworkBehaviour
public class BolaDeNieveOnline : NetworkBehaviour 
{
    public float fuerzaEmpuje = 15f;
    public float tiempoDeVida = 5f;

    public override void OnNetworkSpawn()
    {
        // 3. SOLO el Servidor cuenta el tiempo de vida del proyectil
        if (IsServer)
        {
            // Llama a la función de destruir después de 'tiempoDeVida' segundos
            Invoke(nameof(DestruirEnRed), tiempoDeVida);
        }
    }

    private void OnCollisionEnter(Collision colision)
    {
        // 4. SOLO el Servidor procesa los impactos. 
        // Evita que 2 clientes registren el mismo golpe al mismo tiempo.
        if (!IsServer) return;

        // Aquí puedes poner un Debug o la lógica de empuje
        if (colision.gameObject.CompareTag("Player"))
        {
            Debug.Log("¡Un jugador fue golpeado por la bola!");
            // (Más adelante aquí agregarás el ClientRpc para empujar al jugador)
        }

        // Destruimos la bola de nieve al chocar contra cualquier cosa
        DestruirEnRed();
    }

    private void DestruirEnRed()
    {
        // Cancelamos el temporizador por si chocó antes de tiempo
        CancelInvoke(nameof(DestruirEnRed)); 

        // 5. Despawn elimina el objeto de todos los clientes de forma sincronizada
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(); 
        }
    }
}