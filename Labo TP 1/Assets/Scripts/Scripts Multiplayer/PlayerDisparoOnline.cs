using UnityEngine;
using Unity.Netcode; 

public class PlayerShotOnline : NetworkBehaviour
{
    public GameObject snowballPrefab;
    public Transform puntoDeDisparo; 
    public float fuerzaLanzamiento = 25f;
    public float tiempoEntreDisparos = 1.5f; 
    private float tiempoUltimoDisparo = 0f; 

void Update()
    {
        if (!IsOwner) return;

        if (Time.timeScale == 0f) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
            {
                // 1. CAMBIO AQUÍ: Le enviamos la posición y rotación exactas al servidor
                LanzarBolaDeNieveServerRpc(puntoDeDisparo.position, puntoDeDisparo.rotation);
                
                tiempoUltimoDisparo = Time.time;
            }
            else
            {
                Debug.Log("¡Aún recargando bola de nieve!");
            }
        }
    }

    // 2. CAMBIO AQUÍ: El ServerRpc ahora recibe la posición y rotación
    [ServerRpc]
    void LanzarBolaDeNieveServerRpc(Vector3 posicionCliente, Quaternion rotacionCliente, ServerRpcParams rpcParams = default)
    {
        // Usamos las coordenadas que mandó el cliente, no las locales del servidor
        GameObject bola = Instantiate(snowballPrefab, posicionCliente, rotacionCliente);

        BolaDeNieveOnline scriptBola = bola.GetComponent<BolaDeNieveOnline>();
        if (scriptBola != null)
        {
            scriptBola.idDisparador = rpcParams.Receive.SenderClientId; 
        }

        NetworkObject netObj = bola.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn(true);
        }

        Rigidbody rbBola = bola.GetComponent<Rigidbody>();
        if (rbBola != null)
        {
            // Usamos la rotación del cliente para saber hacia dónde mirar
            rbBola.linearVelocity = rotacionCliente * Vector3.forward * fuerzaLanzamiento;
        }
    }
}