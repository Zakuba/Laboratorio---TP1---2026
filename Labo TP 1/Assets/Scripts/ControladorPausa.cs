using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ControladorPausa : MonoBehaviour
{
    [Header("Asigna aquí el Panel de Pausa desde el Inspector")]
    public GameObject panelPausa;

    [Header("Nombre de la escena de Menú")]
    [SerializeField] private string nombreEscenaMenu = "Menu";

    private bool juegoPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                ReanudarJuego();
            }
            else
            {
                PausarJuego();
            }
        }
    }

    public void PausarJuego()
    {
        panelPausa.SetActive(true);
        juegoPausado = true;
        
        // En multijugador solo liberamos el cursor localmente (no usamos Time.timeScale = 0)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        panelPausa.SetActive(false);
        juegoPausado = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void VolverAlMenuPrincipal()
    {
        // Cerramos la sesión de red si está activa
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Restablecemos el cursor antes de volver al menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(nombreEscenaMenu);
    }
}