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
                // 1. NUEVO: Si somos un cliente puro, instanciamos la visual falsa al instante para no tener lag
                if (!IsServer)
                {
                    LanzarBolaVisualLocal(transform.forward);
                }

                // 2. Llamamos al ServerRpc para generar la bola real (físicas y daños)
                LanzarBolaDeNieveServerRpc(transform.forward);

                tiempoUltimoDisparo = Time.time;
            }
            else
            {
                Debug.Log("¡Aún recargando bola de nieve!");
            }
        }
    }

    // NUEVO METODO: Crea un "fantasma" que solo tú ves, para dar la sensación de disparo instantáneo
    private void LanzarBolaVisualLocal(Vector3 direccionAim)
    {
        direccionAim.Normalize();
        GameObject bolaFalsa = Instantiate(snowballPrefab, puntoDeDisparo.position, Quaternion.LookRotation(direccionAim, Vector3.up));

        // Destruimos la lógica de red y online para que sea un simple adorno local
        if (bolaFalsa.TryGetComponent(out NetworkObject netObj)) Destroy(netObj);
        if (bolaFalsa.TryGetComponent(out BolaDeNieveOnline scriptOnline)) Destroy(scriptOnline);

        // Hacemos que sea un trigger para que la falsa no te empuje objetos físicamente en tu pantalla local
        if (bolaFalsa.TryGetComponent(out Collider col)) col.isTrigger = true;

        Rigidbody rbFalsa = bolaFalsa.GetComponent<Rigidbody>();
        if (rbFalsa != null)
        {
            rbFalsa.isKinematic = false;
            rbFalsa.linearVelocity = direccionAim * fuerzaLanzamiento;
        }

        // Destruimos la falsa localmente antes de que llegue muy lejos
        Destroy(bolaFalsa, 1.5f);
    }

    [ServerRpc]
    void LanzarBolaDeNieveServerRpc(Vector3 direccionAim)
    {
        if (!DireccionValida(direccionAim)) return;

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

        NetworkObject netObj = bola.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            // CAMBIO CLAVE: Le damos la "propiedad" de la bola real al cliente que apretó el botón
            netObj.SpawnWithOwnership(OwnerClientId);
        }
    }

    private static bool DireccionValida(Vector3 direccion)
    {
        return !float.IsNaN(direccion.x) && !float.IsNaN(direccion.y) && !float.IsNaN(direccion.z)
            && !float.IsInfinity(direccion.x) && !float.IsInfinity(direccion.y) && !float.IsInfinity(direccion.z)
            && direccion.sqrMagnitude > 0.0001f;
    }
}