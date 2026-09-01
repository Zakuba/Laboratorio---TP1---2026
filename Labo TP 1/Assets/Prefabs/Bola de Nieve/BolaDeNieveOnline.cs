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
        if (!IsServer) return;

        if (colision.gameObject.CompareTag("Player"))
        {
            Debug.Log("¡Un jugador fue golpeado por la bola!");
            
            // Obtenemos el script del jugador golpeado
            PlayerMovimientoOnline jugador = colision.gameObject.GetComponent<PlayerMovimientoOnline>();
            
            if (jugador != null)
            {
                // Calculamos la dirección (desde la bola hacia el jugador)
                Vector3 direccionEmpuje = (colision.transform.position - transform.position).normalized;
                
                // Opcional: Le damos un pequeño empujón hacia arriba para que se note más el impacto
                direccionEmpuje.y = 0.5f; 
                
                // Le decimos al cliente dueño de ese jugador que se aplique la fuerza
                jugador.AplicarEmpujeClientRpc(direccionEmpuje.normalized * fuerzaEmpuje);
            }
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