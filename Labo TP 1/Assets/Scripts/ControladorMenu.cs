using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ControladorMenu : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [SerializeField] private string nombreEscenaJuego = "Nivel1"; // Cambia por el nombre de tu escena jugable

    [Header("Asigna aquí el Panel de Muestra de Controles desde el Inspector")]
    public GameObject panelMuestaMuestraDeControles;

    // Función para el botón "SinglePlayer"
    public void JugarSinglePlayer()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    // Función para crear la sala y cargar el mapa para todos
    public void IniciarHost()
    {
        Debug.Log("Iniciando como Host...");
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene(nombreEscenaJuego, LoadSceneMode.Single);
        }
    }

    // Función para unirse a una partida existente
    public void UnirseHost()
    {
        Debug.Log("Buscando partida...");
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
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

    // Función para el botón "Salir"
    public void SalirJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }
}