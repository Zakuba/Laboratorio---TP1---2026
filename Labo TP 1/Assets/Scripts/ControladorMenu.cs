using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro; // <-- NUEVO: Necesario para usar el campo de texto

public class ControladorMenu : MonoBehaviour
{
    [Header("Paneles de Interfaz")]
    public GameObject panelMenuPrincipal; 
    public GameObject panelMuestaMuestraDeControles;
    public GameObject panelEsperaCliente;
    
    [Header("Conexión Multijugador")]
    [Tooltip("Asigna aquí el Input Field de TextMeshPro donde el jugador escribe la IP")]
    public TMP_InputField campoIP; // <-- NUEVA VARIABLE: Para leer la IP que escriba tu amigo

    [Header("HUD del Juego")]
    [Tooltip("Asigna aquí la Mira del Canvas para que aparezca al jugar")]
    public GameObject miraHUD;

    public void JugarSinglePlayer()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void IniciarHost()
    {
        Debug.Log("Iniciando como Host...");
        NetworkManager networkManager = NetworkManager.Singleton;
        
        // Suscripción para mostrar la mira del Host
        networkManager.OnClientConnectedCallback -= AlConectarHostLocal;
        networkManager.OnClientConnectedCallback += AlConectarHostLocal;

        // Suscripción para detectar cuando entra el Cliente 2
        networkManager.OnClientConnectedCallback -= AlConectarNuevoCliente;
        networkManager.OnClientConnectedCallback += AlConectarNuevoCliente;

        if (!networkManager.StartHost())
        {
            networkManager.OnClientConnectedCallback -= AlConectarHostLocal;
            networkManager.OnClientConnectedCallback -= AlConectarNuevoCliente;
            return;
        }

        OcultarMenuPrincipal();
        
        // Activamos el panel de espera al iniciar el Host
        if (panelEsperaCliente != null)
        {
            panelEsperaCliente.SetActive(true);
        }
    }

    public void UnirseHost()
    {
        Debug.Log("Uniéndose a partida...");
        NetworkManager networkManager = NetworkManager.Singleton;

        // <-- NUEVO: Leemos lo que el usuario escribió en la interfaz
        string ipIngresada = "127.0.0.1"; // Por defecto, busca en la misma computadora
        
        if (campoIP != null && !string.IsNullOrWhiteSpace(campoIP.text))
        {
            ipIngresada = campoIP.text; // Si escribió algo, usamos esa IP (la de Hamachi)
        }

        // Asignamos la IP al transportador de Unity
        NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().SetConnectionData(ipIngresada, 7777);

        if (!networkManager.StartClient())
        {
            return;
        }

        networkManager.SceneManager.OnSynchronizeComplete -= AlCompletarSincronizacion;
        networkManager.SceneManager.OnSynchronizeComplete += AlCompletarSincronizacion;
        OcultarMenuPrincipal();                 
    }

    private void AlConectarHostLocal(ulong clientId)
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null || clientId != networkManager.LocalClientId)
        {
            return;
        }

        MostrarMira();
        networkManager.OnClientConnectedCallback -= AlConectarHostLocal;
    }

    private void AlConectarNuevoCliente(ulong clientId)
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        
        if (networkManager != null && clientId != networkManager.LocalClientId)
        {
            Debug.Log("¡El cliente se ha conectado! Ocultando panel de espera.");
            
            if (panelEsperaCliente != null)
            {
                panelEsperaCliente.SetActive(false);
            }
            
            networkManager.OnClientConnectedCallback -= AlConectarNuevoCliente;
        }
    }

    private void AlCompletarSincronizacion(ulong clientId)
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null || clientId != networkManager.LocalClientId)
        {
            return;
        }

        MostrarMira();
        networkManager.SceneManager.OnSynchronizeComplete -= AlCompletarSincronizacion;
    }

    private void MostrarMira()
    {
        if (miraHUD == null)
        {
            return;
        }

        NetworkObject jugadorLocal = NetworkManager.Singleton?.LocalClient?.PlayerObject;
        Camera camaraLocal = jugadorLocal != null
            ? jugadorLocal.GetComponentInChildren<Camera>(true)
            : null;
        Canvas canvasHUD = miraHUD.GetComponentInParent<Canvas>();

        if (canvasHUD != null && camaraLocal != null)
        {
            canvasHUD.worldCamera = camaraLocal;
        }

        miraHUD.SetActive(true);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= AlConectarHostLocal;
            NetworkManager.Singleton.OnClientConnectedCallback -= AlConectarNuevoCliente; 

            if (NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnSynchronizeComplete -= AlCompletarSincronizacion;
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