using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ControladorMenu : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [SerializeField] private string nombreEscenaJuego = "Nivel1";

    [Header("Asigna aquí el Panel de Muestra de Controles desde el Inspector")]
    public GameObject panelMuestaMuestraDeControles;

    public void JugarSinglePlayer()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void IniciarHost()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Nivel1", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("No se encontró NetworkManager en la escena.");
        }
    }

    public void UnirseHost()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.LogError("No se encontró NetworkManager en la escena.");
        }
    }

    public void VerControles()
    {
        if (panelMuestaMuestraDeControles != null)
        {
            panelMuestaMuestraDeControles.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void VolveraMenu()
    {
        if (panelMuestaMuestraDeControles != null)
        {
            panelMuestaMuestraDeControles.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SalirJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }
}