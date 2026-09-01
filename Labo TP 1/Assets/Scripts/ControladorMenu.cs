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
        NetworkManager.Singleton.StartHost(); 
        OcultarMenuPrincipal();               
        
        // Encendemos la mira al instante de iniciar
        if (miraHUD != null) miraHUD.SetActive(true); 
    }

    public void UnirseHost()
    {
        Debug.Log("Uniéndose a partida...");
        NetworkManager.Singleton.StartClient(); 
        OcultarMenuPrincipal();                 
        
        // Encendemos la mira al instante de unirnos
        if (miraHUD != null) miraHUD.SetActive(true); 
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