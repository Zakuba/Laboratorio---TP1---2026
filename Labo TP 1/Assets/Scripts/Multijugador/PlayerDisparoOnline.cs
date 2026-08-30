using Unity.Netcode;
using UnityEngine;

public class PlayerShotOnline : NetworkBehaviour
{
    [Header("Referencias")]
    public GameObject snowballPrefab; // Debe tener NetworkObject
    public Transform puntoDeDisparo;

    [Header("Configuración")]
    public float fuerzaLanzamiento = 25f;
    public float tiempoEntreDisparos = 1.5f;
    private float tiempoUltimoDisparo = 0f;

    void Update()
    {
        // Solo el jugador dueño de este personaje procesa el clic
        if (!IsOwner) return;

        // Si el juego está pausado, no hacemos nada
        if (Time.timeScale == 0f)
        {
            return;
        }

        // Detectar clic izquierdo y cooldown
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
            {
                // Enviamos la orden al servidor para instanciar y sincronizar el proyectil
                LanzarBolaDeNieveServerRpc(puntoDeDisparo.position, puntoDeDisparo.rotation, puntoDeDisparo.forward);

                tiempoUltimoDisparo = Time.time;
            }
            else
            {
                Debug.Log("¡Aún recargando bola de nieve!");
            }
        }
    }

    [ServerRpc]
    private void LanzarBolaDeNieveServerRpc(Vector3 posicion, Quaternion rotacion, Vector3 direccion)
    {
        // 1. Instanciamos el prefab en el servidor
        GameObject bola = Instantiate(snowballPrefab, posicion, rotacion);

        // 2. Aplicamos la velocidad física
        Rigidbody rbBola = bola.GetComponent<Rigidbody>();
        if (rbBola != null)
        {
            rbBola.linearVelocity = direccion * fuerzaLanzamiento;
        }

        // 3. Sincronizamos la entidad en la red para todos los clientes
        NetworkObject netObj = bola.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
        else
        {
            Debug.LogError("El prefab snowballPrefab debe tener un componente NetworkObject.");
        }
    }
}