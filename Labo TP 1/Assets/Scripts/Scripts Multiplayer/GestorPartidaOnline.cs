using Unity.Netcode;
using UnityEngine;
using System.Collections;

public enum EstadoPartida
{
    Esperando,
    CuentaRegresiva,
    Jugando,
    Finalizada
}

public enum ResultadoPartida
{
    Ninguno,
    Victoria,
    TiempoAgotado
}

public class GestorPartidaOnline : NetworkBehaviour
{
    [Header("Límite de tiempo")]
    [SerializeField] private bool usarTiempoLimite = true;

    [Header("Referencias UI")]
    [SerializeField] private GameObject panelPausa;

    [SerializeField]
    [Min(1f)]
    private float duracionPartida = 180f;

    public NetworkVariable<EstadoPartida> Estado =
        new(
            EstadoPartida.Esperando,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<int> CuentaRegresiva =
        new(
            5,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<ResultadoPartida> Resultado =
        new(
            ResultadoPartida.Ninguno,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<ulong> ClienteGanador =
        new(
            ulong.MaxValue,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<bool> TiempoLimiteActivo =
        new(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<double> TiempoFinServidor =
        new(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );


    // =====================================================
    // INICIO
    // =====================================================

    public override void OnNetworkSpawn()
    {
        // Todos los clientes escuchan si se desconecta
        // el servidor/Host.
        NetworkManager.OnClientDisconnectCallback +=
            AlDesconectarCliente;

        // A partir de acá solamente trabaja el servidor.
        if (!IsServer)
            return;

        TiempoLimiteActivo.Value = usarTiempoLimite;
        Estado.Value = EstadoPartida.Esperando;
        Resultado.Value = ResultadoPartida.Ninguno;

        NetworkManager.OnClientConnectedCallback +=
            AlConectarCliente;

        IntentarIniciarPartida();
    }


    public override void OnNetworkDespawn()
    {
        if (NetworkManager != null)
        {
            NetworkManager.OnClientDisconnectCallback -=
                AlDesconectarCliente;

            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -=
                    AlConectarCliente;
            }
        }
    }


    // =====================================================
    // DESCONEXIÓN DEL HOST
    // =====================================================

private void AlDesconectarCliente(ulong clientId)
    {
        if (IsServer)
            return;

        Debug.Log("El Host se desconectó. Volviendo al menú...");

        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        // Restaura las cámaras llamando al script Victoria de la escena
        Victoria scriptVictoria = FindAnyObjectByType<Victoria>();
        if (scriptVictoria != null)
        {
            scriptVictoria.RestaurarCamarasMenu();
        }

        ControladorMenu menu = FindAnyObjectByType<ControladorMenu>();
        if (menu != null)
        {
            menu.VolverAlMenuPorDesconexion();
        }
        else
        {
            Debug.LogWarning("No se encontró ControladorMenu en Nivel1.");
        }

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }


    // =====================================================
    // ACTUALIZACIÓN DE PARTIDA
    // =====================================================

    private void Update()
    {
        if (!IsServer)
            return;

        if (Estado.Value != EstadoPartida.Jugando)
            return;

        if (!TiempoLimiteActivo.Value)
            return;

        if (NetworkManager.ServerTime.Time >=
            TiempoFinServidor.Value)
        {
            FinalizarPorTiempo();
        }
    }


    // =====================================================
    // CONEXIÓN DE CLIENTES
    // =====================================================

    private void AlConectarCliente(ulong clientId)
    {
        IntentarIniciarPartida();
    }


    private void IntentarIniciarPartida()
    {
        if (!IsServer)
            return;

        if (Estado.Value != EstadoPartida.Esperando)
            return;

        if (NetworkManager.ConnectedClients.Count < 2)
            return;

        StartCoroutine(SecuenciaDeInicio());
    }


    // =====================================================
    // CUENTA REGRESIVA
    // =====================================================

    private IEnumerator SecuenciaDeInicio()
    {
        Estado.Value = EstadoPartida.CuentaRegresiva;
        CuentaRegresiva.Value = 5;

        // Damos tiempo al cliente para cargar visualmente.
        yield return new WaitForSeconds(1f);

        while (CuentaRegresiva.Value > 1)
        {
            yield return new WaitForSeconds(1f);

            CuentaRegresiva.Value--;
        }

        yield return new WaitForSeconds(1f);

        CuentaRegresiva.Value = 0;

        IniciarPartida();
    }


    // =====================================================
    // INICIO DE PARTIDA
    // =====================================================

    private void IniciarPartida()
    {
        if (!IsServer)
            return;

        if (Estado.Value != EstadoPartida.CuentaRegresiva)
            return;

        Estado.Value = EstadoPartida.Jugando;
        Resultado.Value = ResultadoPartida.Ninguno;

        if (TiempoLimiteActivo.Value)
        {
            TiempoFinServidor.Value =
                NetworkManager.ServerTime.Time +
                duracionPartida;
        }
    }


    // =====================================================
    // FINALIZACIÓN POR TIEMPO
    // =====================================================

    private void FinalizarPorTiempo()
    {
        if (Estado.Value != EstadoPartida.Jugando)
            return;

        Estado.Value = EstadoPartida.Finalizada;
        Resultado.Value = ResultadoPartida.TiempoAgotado;
    }


    // =====================================================
    // TIEMPO RESTANTE
    // =====================================================

    public int ObtenerSegundosRestantes()
    {
        if (!IsSpawned)
            return 0;

        if (!TiempoLimiteActivo.Value)
            return 0;

        double restante =
            TiempoFinServidor.Value -
            NetworkManager.ServerTime.Time;

        return Mathf.Max(
            0,
            Mathf.CeilToInt((float)restante)
        );
    }


    // =====================================================
    // DECLARAR VICTORIA
    // =====================================================

    public bool IntentarDeclararVictoria(
        ulong clienteGanadorId)
    {
        if (!IsServer)
            return false;

        if (Estado.Value != EstadoPartida.Jugando)
            return false;

        Estado.Value = EstadoPartida.Finalizada;
        Resultado.Value = ResultadoPartida.Victoria;

        ClienteGanador.Value = clienteGanadorId;

        return true;
    }

}
