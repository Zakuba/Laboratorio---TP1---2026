using UnityEngine;
using Unity.Netcode; // 1. Importar Netcode

// 2. Heredar de NetworkBehaviour
public class PlayerShotOnline : NetworkBehaviour
{
    public GameObject snowballPrefab;
    public Transform puntoDeDisparo; 
    public float fuerzaLanzamiento = 25f;
    public float tiempoEntreDisparos = 1.5f; 
    private float tiempoUltimoDisparo = 0f; 

    void Update()
    {
        // 3. Si no es mi jugador, ignoro los clics para no disparar por otros
        if (!IsOwner) return;

        if (Time.timeScale == 0f)
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
            {
                // 4. Llamamos a un ServerRpc para que el Host instancie la bola
                LanzarBolaDeNieveServerRpc(transform.forward);
                
                tiempoUltimoDisparo = Time.time;
            }
            else
            {
                Debug.Log("¡Aún recargando bola de nieve!");
            }
        }
    }

    // 5. El ServerRpc le dice al servidor que ejecute este código
    [ServerRpc]
    void LanzarBolaDeNieveServerRpc(Vector3 direccionAim)
    {
        if (!DireccionValida(direccionAim))
        {
            return;
        }

        direccionAim.Normalize();

        GameObject bola = Instantiate(
            snowballPrefab,
            puntoDeDisparo.position,
            Quaternion.LookRotation(direccionAim, Vector3.up)
        );

        Rigidbody rbBola = bola.GetComponent<Rigidbody>();
        if (rbBola != null)
        {
            rbBola.linearVelocity = direccionAim * fuerzaLanzamiento;
        }

        // 6. Spawn() sincroniza el objeto en la red para que todos lo vean
        NetworkObject netObj = bola.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
        else
        {
            Debug.LogWarning("El prefab de la bola de nieve necesita un componente NetworkObject para verse en red.");
        }
    }

    private static bool DireccionValida(Vector3 direccion)
    {
        return !float.IsNaN(direccion.x)
            && !float.IsNaN(direccion.y)
            && !float.IsNaN(direccion.z)
            && !float.IsInfinity(direccion.x)
            && !float.IsInfinity(direccion.y)
            && !float.IsInfinity(direccion.z)
            && direccion.sqrMagnitude > 0.0001f;
    }
}
