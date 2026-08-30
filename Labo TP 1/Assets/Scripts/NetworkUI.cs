using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El objeto que contiene los botones 'Crear partida' y 'Unirse a partida' (por ejemplo, un panel/objeto vacío que sea padre de ambos botones).")]
    [SerializeField] private GameObject panelMenu;

    // Llamar desde el botón "Crear partida"
    public void OnClickHost()
    {
        NetworkManager.Singleton.StartHost();
        OcultarMenu();
    }

    // Llamar desde el botón "Unirse a partida"
    public void OnClickClient()
    {
        NetworkManager.Singleton.StartClient();
        OcultarMenu();
    }

    private void OcultarMenu()
    {
        if (panelMenu != null)
        {
            panelMenu.SetActive(false);
        }
    }
}