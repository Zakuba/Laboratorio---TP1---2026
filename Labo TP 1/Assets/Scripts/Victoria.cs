using UnityEngine;
using Unity.Netcode;

public class Victoria : NetworkBehaviour
{
    [Header("Victoria")]
    [SerializeField] private Transform puntoVictoria;
    [SerializeField] private Camera camVictoria;

    [Header("Derrota")]
    [SerializeField] private Transform puntoDerrota;
    [SerializeField] private Camera camDerrota;

    [Header("Cámara menú")]
    [SerializeField] private Camera camaraMenu;

    [Header("Gestor de partida")]
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

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SolicitarVictoriaServerRpc(
        RpcParams rpcParams = default)
    {
        if (gestorPartida == null)
            return;

        ulong clientIdGanador =
            rpcParams.Receive.SenderClientId;

        bool victoriaAceptada =
            gestorPartida.IntentarDeclararVictoria(
                clientIdGanador
            );

        if (!victoriaAceptada)
        {
            Debug.Log(
                "VICTORIA RECHAZADA: la partida ya finalizó."
            );

            return;
        }

        Debug.Log(
            "VICTORIA ACEPTADA. Ganador: " +
            clientIdGanador
        );

        // -------------------------------------------------
        // GANADOR
        // -------------------------------------------------

        ClientRpcParams parametrosGanador =
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds =
                        new[] { clientIdGanador }
                }
            };

        EjecutarVictoriaClientRpc(parametrosGanador);


        // -------------------------------------------------
        // PERDEDORES
        // -------------------------------------------------

        // Recorremos todos los clientes conectados
        foreach (ulong clientId in
                 NetworkManager.Singleton.ConnectedClientsIds)
        {
            // El ganador no es perdedor
            if (clientId == clientIdGanador)
                continue;

            ClientRpcParams parametrosPerdedor =
                new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds =
                            new[] { clientId }
                    }
                };

            EjecutarDerrotaClientRpc(parametrosPerdedor);
        }
    }


    // =====================================================
    // VICTORIA
    // =====================================================

    [ClientRpc]
    private void EjecutarVictoriaClientRpc(
        ClientRpcParams clientRpcParams = default)
    {
        PlayerMovimientoOnline jugador =
            NetworkManager.Singleton.LocalClient.PlayerObject
                .GetComponent<PlayerMovimientoOnline>();

        if (jugador == null)
            return;

        Debug.Log(
            "VICTORIA: jugador confirmado por servidor"
        );

        // Teletransportar al punto de victoria
        if (puntoVictoria != null)
        {
            jugador.Teletransportar(puntoVictoria);
        }

        // Bloquear movimiento
        jugador.BloquearMovimiento();

        // Desactivar cámara del jugador
        DesactivarCamaraJugador();

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


    // =====================================================
    // DERROTA
    // =====================================================

    [ClientRpc]
    private void EjecutarDerrotaClientRpc(
        ClientRpcParams clientRpcParams = default)
    {
        PlayerMovimientoOnline jugador =
            NetworkManager.Singleton.LocalClient.PlayerObject
                .GetComponent<PlayerMovimientoOnline>();

        if (jugador == null)
            return;

        Debug.Log(
            "DERROTA: jugador confirmado por servidor"
        );

        // Teletransportar al punto de derrota
        if (puntoDerrota != null)
        {
            jugador.Teletransportar(puntoDerrota);
        }

        // Bloquear movimiento
        jugador.BloquearMovimiento();

        // Desactivar cámara del jugador
        DesactivarCamaraJugador();

        // Desactivar cámara del menú
        if (camaraMenu != null)
        {
            camaraMenu.enabled = false;

            Debug.Log(
                "DERROTA: cámara menú desactivada"
            );
        }

        // Activar cámara de derrota
        if (camDerrota != null)
        {
            Debug.Log(
                "DERROTA: ACTIVANDO CAMARA DERROTA"
            );

            camDerrota.gameObject.SetActive(true);
            camDerrota.enabled = true;
        }
    }


    // =====================================================
    // CÁMARA DEL JUGADOR
    // =====================================================

    private void DesactivarCamaraJugador()
    {
        PlayerMovimientoOnline jugador =
            NetworkManager.Singleton.LocalClient.PlayerObject
                .GetComponent<PlayerMovimientoOnline>();

        if (jugador == null)
            return;

        CameraFollowOnline camaraJugador =
            jugador.GetComponentInChildren<CameraFollowOnline>();

        if (camaraJugador != null)
        {
            Debug.Log(
                "CÁMARA: cámara del jugador encontrada"
            );

            camaraJugador.enabled = false;

            Camera camaraNormal =
                camaraJugador.GetComponent<Camera>();

            if (camaraNormal != null)
            {
                camaraNormal.enabled = false;

                Debug.Log(
                    "CÁMARA: cámara del jugador desactivada"
                );
            }
        }
    }

    // =====================================================
    // RESTAURAR CÁMARAS
    // =====================================================
    public void RestaurarCamarasMenu()
    {
        if (camVictoria != null)
        {
            camVictoria.enabled = false;
            camVictoria.gameObject.SetActive(false);
        }

        if (camDerrota != null)
        {
            camDerrota.enabled = false;
            camDerrota.gameObject.SetActive(false);
        }

        if (camaraMenu != null)
        {
            camaraMenu.gameObject.SetActive(true);
            camaraMenu.enabled = true;
        }
    }
}
