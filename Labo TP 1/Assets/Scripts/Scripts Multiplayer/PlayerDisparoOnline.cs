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

        if (Time.timeScale == 0f)
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
            {
                // Llamamos a un ÚNICO ServerRpc para instanciar la bola
                LanzarBolaDeNieveServerRpc();
                
                tiempoUltimoDisparo = Time.time;
            }
            else
            {
                Debug.Log("¡Aún recargando bola de nieve!");
            }
        }
    }

    [ServerRpc]
    void LanzarBolaDeNieveServerRpc()
    {
        // 1. El servidor instancia la bola usando tu variable snowballPrefab
        GameObject bola = Instantiate(snowballPrefab, puntoDeDisparo.position, puntoDeDisparo.rotation);

        // 2. Le aplicamos la fuerza física
        Rigidbody rbBola = bola.GetComponent<Rigidbody>();
        if (rbBola != null)
        {
            rbBola.linearVelocity = puntoDeDisparo.forward * fuerzaLanzamiento;
        }

        // 3. Sincronizamos en la red ASIGNANDO LA PROPIEDAD a quien disparó
        NetworkObject netObj = bola.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            // Esto es lo que soluciona el problema de que desaparezca:
            netObj.SpawnWithOwnership(OwnerClientId);
        }
        else
        {
            Debug.LogWarning("El prefab de la bola de nieve necesita un componente NetworkObject para verse en red.");
        }
    }
}