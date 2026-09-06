using UnityEngine;
using Unity.Netcode;

// Este script NO necesita ningún trigger físico en el mapa. La derrota no
// pasa "en un lugar" — pasa automáticamente cuando OTRO jugador gana.
// Por eso, en vez de OnTriggerEnter, escuchamos el cambio de la variable
// sincronizada Resultado del GestorPartidaOnline. Cada cliente ejecuta este
// script y decide POR SU CUENTA si a él le toca la derrota, comparando su
// propio ClientId contra el ganador que guardó el servidor.
public class Derrota : NetworkBehaviour
{
    [Header("Referencias")]
    [Tooltip("Punto opcional donde teletransportar al perdedor. Podés dejarlo vacío si no querés moverlo.")]
    [SerializeField] private Transform puntoDerrota;

    [Tooltip("Cámara que se activa para el perdedor (equivalente a camVictoria en Victoria.cs)")]
    [SerializeField] private Camera camDerrota;

    [SerializeField] private Camera camaraMenu;

    [SerializeField] private GestorPartidaOnline gestorPartida;

    // Evita ejecutar la lógica de derrota más de una vez por partida.
    private bool yaProcesado = false;

    public override void OnNetworkSpawn()
    {
        if (gestorPartida == null)
        {
            Debug.LogWarning("Derrota: falta asignar 'gestorPartida' en el Inspector.");
            return;
        }

        // Nos suscribimos a los cambios de Resultado. OnValueChanged se
        // dispara automáticamente en TODOS los clientes cuando el servidor
        // cambia el valor, porque la variable es una NetworkVariable.
        gestorPartida.Resultado.OnValueChanged += AlCambiarResultado;

        // Por si este objeto se spawnea DESPUÉS de que la partida ya
        // terminó (ej. un cliente que se reconecta tarde), chequeamos
        // el estado actual una vez apenas arrancamos.
        RevisarResultadoActual();
    }

    public override void OnNetworkDespawn()
    {
        if (gestorPartida != null)
        {
            gestorPartida.Resultado.OnValueChanged -= AlCambiarResultado;
        }
    }

    private void AlCambiarResultado(ResultadoPartida anterior, ResultadoPartida nuevo)
    {
        RevisarResultadoActual();
    }

    private void RevisarResultadoActual()
    {
        if (yaProcesado)
            return;

        if (gestorPartida.Estado.Value != EstadoPartida.Finalizada)
            return;

        // Esta versión de Derrota.cs solo cubre el caso "alguien llegó a la
        // meta". El caso de TIEMPO AGOTADO ya se muestra aparte en el HUD
        // (HUDTiempoOnline), porque ahí no hay un "ganador" individual.
        if (gestorPartida.Resultado.Value != ResultadoPartida.Victoria)
            return;

        ulong miClientId = NetworkManager.Singleton.LocalClientId;

        // Si el ganador soy YO, esta lógica no me corresponde a mí
        // (de mi experiencia de victoria ya se encarga Victoria.cs).
        if (gestorPartida.ClienteGanador.Value == miClientId)
            return;

        yaProcesado = true;
        EjecutarDerrotaLocal();
    }

    private void EjecutarDerrotaLocal()
    {
        NetworkObject jugadorObj =
            NetworkManager.Singleton.LocalClient.PlayerObject;

        if (jugadorObj == null)
            return;

        PlayerMovimientoOnline jugador =
            jugadorObj.GetComponent<PlayerMovimientoOnline>();

        if (jugador == null)
            return;

        Debug.Log("DERROTA: confirmado localmente como jugador perdedor");

        // Teletransportar al punto de derrota (opcional)
        if (puntoDerrota != null)
        {
            jugador.Teletransportar(puntoDerrota);
        }

        // Bloquear movimiento (mismo método que ya usa Victoria.cs)
        jugador.BloquearMovimiento();

        // Desactivar la cámara normal del jugador
        CameraFollowOnline camaraJugador =
            jugador.GetComponentInChildren<CameraFollowOnline>();

        if (camaraJugador != null)
        {
            camaraJugador.enabled = false;

            Camera camaraNormal = camaraJugador.GetComponent<Camera>();
            if (camaraNormal != null)
            {
                camaraNormal.enabled = false;
            }
        }

        // Desactivar cámara de menú, si corresponde
        if (camaraMenu != null)
        {
            camaraMenu.enabled = false;
        }

        // Activar la cámara de derrota
        if (camDerrota != null)
        {
            camDerrota.gameObject.SetActive(true);
            camDerrota.enabled = true;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}