using Unity.Netcode;
using Unity.Mathematics;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Ajustes")]
    [SerializeField] public float Sensibilidad = 100;
    [SerializeField] public Transform Player;

    [Header("Estados")]
    [SerializeField] public float RotacionHorizontal = 0;
    [SerializeField] public float RotacionVertical = 0;

    private NetworkObject networkObjectDueño;

    void Start()
    {
        // Buscamos el NetworkObject del personaje al que pertenece esta cámara
        // (asumimos que "Camara" ahora es un hijo dentro del prefab Player).
        networkObjectDueño = GetComponentInParent<NetworkObject>();

        // Si esta cámara pertenece a un personaje de red y NO es el nuestro,
        // la apagamos: no queremos ver por los ojos de otro jugador ni robarle
        // el control del cursor a nuestra propia cámara.
        if (networkObjectDueño != null && !networkObjectDueño.IsOwner)
        {
            if (TryGetComponent<Camera>(out var camaraComponente))
            {
                camaraComponente.enabled = false;
            }

            if (TryGetComponent<AudioListener>(out var listener))
            {
                listener.enabled = false;
            }

            enabled = false; // apaga este script (Update ya no se ejecuta)
            return;
        }

        // Si Player no fue asignado a mano en el Inspector, usamos el padre
        // (el propio personaje al que pertenece esta cámara).
        if (Player == null)
        {
            Player = transform.parent;
        }

        // Bloquea el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;

        // Oculta el cursor mientras juegas
        Cursor.visible = false;
    }

    void Update()
    {
        //Nos dan los valores del mause para mover 
        float ValorX = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float ValorY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;

        //Guarda el valor y queda en el valor para seguir
        RotacionHorizontal += ValorX;
        RotacionVertical -= ValorY;

        //Limita en 80 grados
        RotacionVertical = math.clamp(RotacionVertical, -80, 80);


        //Hace la rotaion vertical fluida
        transform.localRotation = Quaternion.Euler(RotacionVertical, 0, 0);


        //Hace la rotacion horizontal
        if (Player != null)
        {
            Player.Rotate(Vector3.up * ValorX);
        }
        else
        {
            //Si no tiene asignado el player avisas en consola
            Debug.LogWarning("Camara: asigna (Player) en el Inspector.");
        }
    }
}