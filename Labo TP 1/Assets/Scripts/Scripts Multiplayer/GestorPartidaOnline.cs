using Unity.Netcode;
using UnityEngine;

public enum EstadoPartida
{
    Esperando,
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

    [SerializeField]
    [Min(1f)]
    private float duracionPartida = 180f;

    public NetworkVariable<EstadoPartida> Estado =
        new(
            EstadoPartida.Esperando,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<ResultadoPartida> Resultado =
        new(
            ResultadoPartida.Ninguno,
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

public override void OnNetworkSpawn()
{
    if (!IsServer)
        return;

    TiempoLimiteActivo.Value = usarTiempoLimite;
    Estado.Value = EstadoPartida.Esperando;
    Resultado.Value = ResultadoPartida.Ninguno;

    NetworkManager.OnClientConnectedCallback += AlConectarCliente;

    IntentarIniciarPartida();
}
public override void OnNetworkDespawn()
{
    if (NetworkManager != null && IsServer)
    {
        NetworkManager.OnClientConnectedCallback -= AlConectarCliente;
    }
}

    private void Update()
    {
        if (!IsServer)
            return;

        if (Estado.Value != EstadoPartida.Jugando)
            return;

        if (!TiempoLimiteActivo.Value)
            return;

        if (NetworkManager.ServerTime.Time >= TiempoFinServidor.Value)
        {
            FinalizarPorTiempo();
        }
    }

private void IniciarPartida()
{
    if (!IsServer)
        return;

    if (Estado.Value != EstadoPartida.Esperando)
        return;

    Estado.Value = EstadoPartida.Jugando;
    Resultado.Value = ResultadoPartida.Ninguno;

    if (TiempoLimiteActivo.Value)
    {
        TiempoFinServidor.Value =
            NetworkManager.ServerTime.Time + duracionPartida;
    }
}

    private void FinalizarPorTiempo()
    {
        if (Estado.Value != EstadoPartida.Jugando)
            return;

        Estado.Value = EstadoPartida.Finalizada;
        Resultado.Value = ResultadoPartida.TiempoAgotado;
    }

    public int ObtenerSegundosRestantes()
    {
        if (!IsSpawned)
            return 0;

        if (!TiempoLimiteActivo.Value)
            return 0;

        double restante =
            TiempoFinServidor.Value - NetworkManager.ServerTime.Time;

        return Mathf.Max(0, Mathf.CeilToInt((float)restante));
    }

    public bool IntentarDeclararVictoria()
    {
        if (!IsServer)
            return false;

        if (Estado.Value != EstadoPartida.Jugando)
            return false;

        Estado.Value = EstadoPartida.Finalizada;
        Resultado.Value = ResultadoPartida.Victoria;

        return true;
    }

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

    IniciarPartida();
}
}