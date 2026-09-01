using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPausa : MonoBehaviour
{
    [Header("Asigna aquí el Panel de Pausa desde el Inspector")]
    public GameObject panelPausa;

    private bool juegoPausado = false;

    void Update()
    {
        // Detectar si se presiona la tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                ReanudarJuego(); // Si ya está pausado, lo reanuda (es lo mismo que tocar el botón "Volver")
            }
            else
            {
                PausarJuego(); // Si no está pausado, abre el menú
            }
        }
    }

    public void PausarJuego()
    {
        panelPausa.SetActive(true); // Muestra la interfaz verde
        Time.timeScale = 0f;        // Congela el tiempo del juego (físicas, animaciones, etc.)
        juegoPausado = true;
        
        // Opcional: Desbloquear y mostrar el cursor si en tu juego 3D lo tienes oculto
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Esta es la función para el botón "Volver"
    public void ReanudarJuego()
    {
        panelPausa.SetActive(false); // Oculta la interfaz
        Time.timeScale = 1f;         // Restaura el tiempo a la normalidad
        juegoPausado = false;
        
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Esta es la función para el botón "Menu"
    public void VolverAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Nivel1"); // Carga tu escena inicial
    }
}