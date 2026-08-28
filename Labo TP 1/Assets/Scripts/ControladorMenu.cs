using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMenu : MonoBehaviour
{
        [Header("Asigna aquí el Panel de Muestra de Controles desde el Inspector")]
    public GameObject panelMuestaMuestraDeControles;
    // Función para el botón "SinglePlayer"
    public void JugarSinglePlayer()
    {
        // Carga la escena de juego. Asegúrate de que su nombre sea exactamente "SampleScene"
        SceneManager.LoadScene("SampleScene");
    }

    // Funciones base para los botones de red (tendrás que ampliarlas cuando uses un sistema como Netcode o Photon)
    public void IniciarHost()
    {
        Debug.Log("Iniciando como Host...");
        // Lógica de red aquí
    }

    public void UnirseHost()
    {
        Debug.Log("Buscando partida...");
        // Lógica de red aquí
    }

    public void VerControles()
    {
        panelMuestaMuestraDeControles.SetActive(true); // Muestra la interfaz verde
        // Desbloquear y mostrar el cursor si en tu juego 3D lo tienes oculto
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

     public void VolveraMenu()
    {
        panelMuestaMuestraDeControles.SetActive(false); // Muestra la interfaz verde
        // Desbloquear y mostrar el cursor si en tu juego 3D lo tienes oculto
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
