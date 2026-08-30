using UnityEngine;
using Unity.Netcode;

public class PlayerShot : NetworkBehaviour
{
    public GameObject snowballPrefab;
    public Transform puntoDeDisparo; 
    public float fuerzaLanzamiento = 25f;
    public float tiempoEntreDisparos = 1.5f; // Segundos que debe esperar
    private float tiempoUltimoDisparo = 0f; // Guarda cuándo fue el último tiro

    void Update()
    {
        // Solo el dueño del objeto puede procesar el input
        if (!IsOwner) return;

        // Si el juego está pausado (el tiempo está congelado), salimos del Update y no hacemos nada
        if (Time.timeScale == 0f)
        {
            return;
        }
        
        // Detectar el clic izquierdo Y comprobar si ya pasó el tiempo necesario
        if (Input.GetMouseButtonDown(0))
        {
            // Verificamos si la hora actual del juego es mayor al momento en que 
            // disparamos por última vez + el tiempo de espera.
            if (Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
            {
                LanzarBolaDeNieveRpc(puntoDeDisparo.position, puntoDeDisparo.rotation);
                
                // Registramos el momento exacto en que acabamos de disparar
                tiempoUltimoDisparo = Time.time;
            }
            else
            {
                Debug.Log("¡Aún recargando bola de nieve!");
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void LanzarBolaDeNieveRpc(Vector3 posicion, Quaternion rotacion)
    {
        GameObject bola = Instantiate(snowballPrefab, posicion, rotacion);

        // Se hace spawn en la red para que aparezca en todos los clientes
        NetworkObject netObj = bola.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }

        Rigidbody rbBola = bola.GetComponent<Rigidbody>();
        if (rbBola != null)
        {
            rbBola.linearVelocity = rotacion * Vector3.forward * fuerzaLanzamiento;
        }
    }
}