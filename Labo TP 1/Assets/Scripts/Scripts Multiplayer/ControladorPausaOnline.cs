using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode; // 1. Agregamos Netcode

public class ControladorPausaOnline : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelPausa;
    public GameObject mira; // 2. Agregamos la referencia de la mira

    private bool juegoPausado = false;

    void Start()
    {
        // Nos aseguramos de que el panel empiece apagado para que no tape el menú principal
        if (panelPausa != null) panelPausa.SetActive(false);
    }

    void Update()
    {
        // 3. REGLA DE ORO: Solo permitimos pausar si el jugador ya está en la partida.
        // Evita que al tocar ESC en el Menú Principal salte la pausa.
        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer))
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
    }

    public void PausarJuego()
    {
        panelPausa.SetActive(true); 
        
        // Ocultamos la mira al abrir la pausa
        if (mira != null) mira.SetActive(false); 

        // ELIMINADO: Time.timeScale = 0f; (En multijugador el mundo debe seguir girando)
        juegoPausado = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        panelPausa.SetActive(false); 
        
        // Volvemos a mostrar la mira
        if (mira != null) mira.SetActive(true);

        // ELIMINADO: Time.timeScale = 1f;
        juegoPausado = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void VolverAlMenuPrincipal()
    {
        // 4. Apagamos la red correctamente antes de salir
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Dejamos el mouse libre para poder usar el Menú Principal
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Recargamos la escena para resetear todo a su estado original (el Menú)
        SceneManager.LoadScene("Nivel1"); 
    }
}