using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMenu : MonoBehaviour
{
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
        Debug.Log("Mostrando pantalla de controles...");
        // Aquí podrías activar un panel que muestre los controles
    }

    // Función para el botón "Salir"
    public void SalirJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }
}
