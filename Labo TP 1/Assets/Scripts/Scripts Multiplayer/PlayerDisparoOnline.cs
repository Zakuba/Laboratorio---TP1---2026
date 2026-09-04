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
    private PlayerMovimientoOnline movimientoJugador; // <-- NUEVA VARIABLE

public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        controladorBarraRecarga = FindAnyObjectByType<ControladorBarraRecarga>();
            
        // CAMBIO: Usamos GetComponentInParent para que lo encuentre aunque el 
        // script de disparo esté puesto en un objeto hijo (como la cámara o el arma).
        movimientoJugador = GetComponentInParent<PlayerMovimientoOnline>();

        // Agregamos esta alerta para saber si sigue sin encontrarlo
        if (movimientoJugador == null)
        {
            Debug.LogError("ERROR: PlayerShotOnline no encontró PlayerMovimientoOnline en este jugador.");
        }
    }

    void Update()
    {
        if (!IsOwner) return;
        if (Time.timeScale == 0f) return;

        // Si el jugador está bloqueado, no procesa el clic del mouse
        if (movimientoJugador != null)
        {
            if (movimientoJugador.EstaBloqueado())
            {
                // Si quieres confirmar que funciona, descomenta la siguiente línea:
                // Debug.Log("No puedes disparar, estás bloqueado.");
                return; 
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
            {
                if (!IsServer)
                {
                    LanzarBolaVisualLocal(transform.forward);
                }

                LanzarBolaDeNieveServerRpc(transform.forward);

                tiempoUltimoDisparo = Time.time;

                if (controladorBarraRecarga == null)
                {
                    controladorBarraRecarga = FindAnyObjectByType<ControladorBarraRecarga>();
                }

                if (controladorBarraRecarga != null)
                {
                    controladorBarraRecarga.IniciarRecarga(tiempoEntreDisparos);
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