using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ControladorMenu : MonoBehaviour
{
    [Header("Paneles de Interfaz")]
    public GameObject panelMenuPrincipal; 
    public GameObject panelMuestaMuestraDeControles;
    
    [Header("HUD del Juego")]
    [Tooltip("Asigna aquí la Mira del Canvas para que aparezca al jugar")]
    public GameObject miraHUD; // <-- NUEVA VARIABLE PARA LA MIRA

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

        if (!networkManager.StartHost())
        {
            networkManager.OnClientConnectedCallback -= AlConectarHostLocal;
            return;
        }

        OcultarMenuPrincipal();
    }

    public void UnirseHost()
    {
        Debug.Log("Uniéndose a partida...");
        NetworkManager networkManager = NetworkManager.Singleton;

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
