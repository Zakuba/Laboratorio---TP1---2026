using UnityEngine;
using Unity.Netcode;

public class PlayerShotOnline : NetworkBehaviour
{
    public GameObject snowballPrefab;
    public Transform puntoDeDisparo;
    public float fuerzaLanzamiento = 25f;
    public float tiempoEntreDisparos = 1.5f;
    private float tiempoUltimoDisparo = 0f;

    private ControladorBarraRecarga controladorBarraRecarga;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        controladorBarraRecarga =
            FindFirstObjectByType<ControladorBarraRecarga>();
    }

    void Update()
    {
        if (!IsOwner) return;
        if (Time.timeScale == 0f) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
            {
                // Si somos un cliente puro, mostramos la bola visual al instante
                if (!IsServer)
                {
                    LanzarBolaVisualLocal(transform.forward);
                }

                // Generamos la bola real en el servidor
                LanzarBolaDeNieveServerRpc(transform.forward);

                tiempoUltimoDisparo = Time.time;

                // Iniciamos la barra de recarga
                if (controladorBarraRecarga == null)
                {
                    controladorBarraRecarga = FindFirstObjectByType<ControladorBarraRecarga>();
                }

                if (controladorBarraRecarga != null)
                {
                    controladorBarraRecarga.IniciarRecarga(tiempoEntreDisparos);
                }
                else
                {
                    Debug.LogError("NO SE ENCONTRÓ ControladorBarraRecarga EN LA ESCENA");
                }
            }
            else
            {
                Debug.Log("¡Aún recargando bola de nieve!");
            }
        }
    }

    private void LanzarBolaVisualLocal(Vector3 direccionAim)
    {
        direccionAim.Normalize();

        GameObject bolaFalsa = Instantiate(
            snowballPrefab,
            puntoDeDisparo.position,
            Quaternion.LookRotation(direccionAim, Vector3.up)
        );

        // Destruimos la lógica de red y online
        if (bolaFalsa.TryGetComponent(out NetworkObject netObj))
            Destroy(netObj);

        if (bolaFalsa.TryGetComponent(out BolaDeNieveOnline scriptOnline))
            Destroy(scriptOnline);

        // La falsa no debe tener físicas de colisión
        if (bolaFalsa.TryGetComponent(out Collider col))
            col.isTrigger = true;

        Rigidbody rbFalsa = bolaFalsa.GetComponent<Rigidbody>();

        if (rbFalsa != null)
        {
            rbFalsa.isKinematic = false;
            rbFalsa.linearVelocity =
                direccionAim * fuerzaLanzamiento;
        }

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
            rbBola.linearVelocity =
                direccionAim * fuerzaLanzamiento;
        }

        NetworkObject netObj =
            bola.GetComponent<NetworkObject>();

        if (netObj != null)
        {
            // La bola real pertenece al cliente que disparó
            netObj.SpawnWithOwnership(OwnerClientId);
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