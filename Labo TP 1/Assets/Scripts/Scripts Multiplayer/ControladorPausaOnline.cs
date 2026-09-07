using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ControladorPausaOnline : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelPausa;
    public GameObject mira;

    private bool juegoPausado = false;

    void Start()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
    }

    void Update()
    {
        if (NetworkManager.Singleton != null &&
            (NetworkManager.Singleton.IsClient ||
             NetworkManager.Singleton.IsServer))
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
        if (panelPausa != null)
        {
            panelPausa.SetActive(true);
        }

        if (mira != null)
        {
            mira.SetActive(false);
        }

        juegoPausado = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        if (mira != null)
        {
            mira.SetActive(true);
        }

        juegoPausado = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void VolverAlMenuPrincipal()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // El menú está dentro de Nivel1.
        SceneManager.LoadScene("Nivel1");
    }

    public void ResetearPausa()
    {
        juegoPausado = false;

        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        if (mira != null)
        {
            mira.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
