using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

public class ControladorMenu : MonoBehaviour
{
    [Header("Paneles de Interfaz")]
    public GameObject panelMenuPrincipal;
    public GameObject panelMuestaMuestraDeControles;
    public GameObject panelEsperaCliente;

    [Header("Conexión Multijugador")]
    [Tooltip("Asigna aquí el Input Field de TextMeshPro donde el jugador escribe la IP")]
    public TMP_InputField campoIP;

    [Header("HUD del Juego")]
    [Tooltip("Mira del jugador")]
    public GameObject miraHUD;

    [Tooltip("Timer de la partida")]
    public GameObject timerHUD;

    [Header("Pausa")]
    [Tooltip("Panel de pausa del juego")]
    public GameObject panelPausa;

    [Header("Cámara del Menú")]
    [Tooltip("Cámara que muestra el escenario detrás del menú")]
    public Camera camaraMenu;

    private bool volviendoAlMenu = false;

    public void JugarSinglePlayer()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void IniciarHost()
    {
        Debug.Log("Iniciando como Host...");

        NetworkManager networkManager = NetworkManager.Singleton;

        networkManager.OnClientConnectedCallback -= AlConectarHostLocal;
        networkManager.OnClientConnectedCallback += AlConectarHostLocal;

        networkManager.OnClientConnectedCallback -= AlConectarNuevoCliente;
        networkManager.OnClientConnectedCallback += AlConectarNuevoCliente;

        if (!networkManager.StartHost())
        {
            networkManager.OnClientConnectedCallback -= AlConectarHostLocal;
            networkManager.OnClientConnectedCallback -= AlConectarNuevoCliente;
            return;
        }

        OcultarMenuPrincipal();

        if (panelEsperaCliente != null)
        {
            panelEsperaCliente.SetActive(true);
        }
    }

    public void UnirseHost()
    {
        Debug.Log("Uniéndose a partida...");

        NetworkManager networkManager = NetworkManager.Singleton;

        string ipIngresada = "127.0.0.1";

        if (campoIP != null && !string.IsNullOrWhiteSpace(campoIP.text))
        {
            ipIngresada = campoIP.text;
        }

        NetworkManager.Singleton
            .GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>()
            .SetConnectionData(ipIngresada, 7777);

        if (!networkManager.StartClient())
        {
            return;
        }

        networkManager.SceneManager.OnSynchronizeComplete -=
            AlCompletarSincronizacion;

        networkManager.SceneManager.OnSynchronizeComplete +=
            AlCompletarSincronizacion;

        OcultarMenuPrincipal();
    }

    private void AlConectarHostLocal(ulong clientId)
    {
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager == null ||
            clientId != networkManager.LocalClientId)
        {
            return;
        }

        networkManager.OnClientConnectedCallback -=
            AlConectarHostLocal;
    }

    private void AlConectarNuevoCliente(ulong clientId)
    {
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager != null &&
            clientId != networkManager.LocalClientId)
        {
            Debug.Log(
                "¡El cliente se ha conectado! Ocultando panel de espera."
            );

            if (panelEsperaCliente != null)
            {
                panelEsperaCliente.SetActive(false);
                MostrarMira();
            }

            networkManager.OnClientConnectedCallback -=
                AlConectarNuevoCliente;
        }
    }

    private void AlCompletarSincronizacion(ulong clientId)
    {
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager == null ||
            clientId != networkManager.LocalClientId)
        {
            return;
        }

        MostrarMira();

        networkManager.SceneManager.OnSynchronizeComplete -=
            AlCompletarSincronizacion;
    }

    private void MostrarMira()
    {
        if (miraHUD == null)
        {
            return;
        }

        NetworkObject jugadorLocal =
            NetworkManager.Singleton?.LocalClient?.PlayerObject;

        Camera camaraLocal = jugadorLocal != null
            ? jugadorLocal.GetComponentInChildren<Camera>(true)
            : null;

        Canvas canvasHUD =
            miraHUD.GetComponentInParent<Canvas>();

        if (canvasHUD != null && camaraLocal != null)
        {
            canvasHUD.worldCamera = camaraLocal;
        }

        miraHUD.SetActive(true);
    }

    // ============================================================
    // VOLVER AL MENÚ CUANDO EL HOST SE DESCONECTA
    // ============================================================

    public void VolverAlMenuPorDesconexion()
    {
        if (volviendoAlMenu)
        {
            return;
        }

        volviendoAlMenu = true;

        Debug.Log(
            "El Host se desconectó. Restaurando menú principal..."
        );

        // --------------------------------------------------------
        // APAGAR HUD
        // --------------------------------------------------------

        if (miraHUD != null)
        {
            miraHUD.SetActive(false);
        }

        GameObject panelTiempo = GameObject.Find("PanelTiempo");

        if (panelTiempo != null)
        {
            panelTiempo.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "No se encontró el objeto PanelTiempo."
            );
        }

        // --------------------------------------------------------
        // APAGAR PAUSA
        // --------------------------------------------------------

        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        // --------------------------------------------------------
        // APAGAR PANEL DE ESPERA
        // --------------------------------------------------------

        if (panelEsperaCliente != null)
        {
            panelEsperaCliente.SetActive(false);
        }

        // --------------------------------------------------------
        // APAGAR PANEL DE CONTROLES
        // --------------------------------------------------------

        if (panelMuestaMuestraDeControles != null)
        {
            panelMuestaMuestraDeControles.SetActive(false);
        }

        // --------------------------------------------------------
        // MOSTRAR MENÚ PRINCIPAL
        // --------------------------------------------------------

        if (panelMenuPrincipal != null)
        {
            panelMenuPrincipal.SetActive(true);
        }

        // --------------------------------------------------------
        // ACTIVAR CÁMARA DEL MENÚ
        // --------------------------------------------------------

        if (camaraMenu != null)
        {
            camaraMenu.gameObject.SetActive(true);
            camaraMenu.enabled = true;

            Debug.Log("Cámara del menú activada.");
        }

        // --------------------------------------------------------
        // LIBERAR CURSOR
        // --------------------------------------------------------

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Menú restaurado correctamente.");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -=
                AlConectarHostLocal;

            NetworkManager.Singleton.OnClientConnectedCallback -=
                AlConectarNuevoCliente;

            if (NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager
                    .OnSynchronizeComplete -= AlCompletarSincronizacion;
            }
        }
    }

    private void OcultarMenuPrincipal()
    {
        if (panelMenuPrincipal != null)
        {
            panelMenuPrincipal.SetActive(false);
        }
    }

    public void VerControles()
    {
        panelMuestaMuestraDeControles.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void VolveraMenu()
    {
        panelMuestaMuestraDeControles.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SalirJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }
}
