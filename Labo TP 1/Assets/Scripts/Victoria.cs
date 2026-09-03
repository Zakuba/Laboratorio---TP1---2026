using UnityEngine;
using Unity.Netcode;

public class Victoria : NetworkBehaviour
{
    [SerializeField] private Transform puntoVictoria;
    [SerializeField] private Camera camVictoria;
    [SerializeField] private Camera camaraMenu;
    [SerializeField] private GestorPartidaOnline gestorPartida;

private void OnTriggerEnter(Collider other)
{
    if (!other.CompareTag("Player"))
        return;

    PlayerMovimientoOnline jugador =
        other.GetComponentInParent<PlayerMovimientoOnline>();

    if (jugador == null || !jugador.IsOwner)
        return;

    SolicitarVictoriaServerRpc();
}

[ServerRpc(RequireOwnership = false)]
private void SolicitarVictoriaServerRpc(
    ServerRpcParams rpcParams = default)
{
    if (gestorPartida == null)
        return;

    bool victoriaAceptada =
        gestorPartida.IntentarDeclararVictoria();

    if (!victoriaAceptada)
    {
        Debug.Log(
            "VICTORIA RECHAZADA: la partida ya finalizó."
        );
        return;
    }

    ulong clientIdGanador =
        rpcParams.Receive.SenderClientId;

    ClientRpcParams parametrosCliente =
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds =
                    new[] { clientIdGanador }
            }
        };

    EjecutarVictoriaClientRpc(parametrosCliente);
}
[ClientRpc]
private void EjecutarVictoriaClientRpc(
    ClientRpcParams clientRpcParams = default)
{
    PlayerMovimientoOnline jugador =
        NetworkManager.Singleton.LocalClient.PlayerObject
            .GetComponent<PlayerMovimientoOnline>();

    if (jugador == null)
        return;

    Debug.Log("VICTORIA: jugador confirmado por servidor");

    // Teletransportar
    jugador.Teletransportar(puntoVictoria);

    // Bloquear movimiento
    jugador.BloquearMovimiento();

    // Desactivar cámara del jugador
    CameraFollowOnline camaraJugador =
        jugador.GetComponentInChildren<CameraFollowOnline>();

    if (camaraJugador != null)
    {
        Debug.Log("VICTORIA: cámara del jugador encontrada");

        camaraJugador.enabled = false;

        Camera camaraNormal =
            camaraJugador.GetComponent<Camera>();

        if (camaraNormal != null)
        {
            camaraNormal.enabled = false;

            Debug.Log(
                "VICTORIA: cámara del jugador desactivada"
            );
        }
    }

    // Desactivar cámara del menú
    if (camaraMenu != null)
    {
        camaraMenu.enabled = false;

        Debug.Log(
            "VICTORIA: cámara menú desactivada"
        );
    }

    // Activar cámara de victoria
    if (camVictoria != null)
    {
        Debug.Log(
            "VICTORIA: ACTIVANDO CAMARA VICTORIA"
        );

        camVictoria.gameObject.SetActive(true);
        camVictoria.enabled = true;
    }
}
}