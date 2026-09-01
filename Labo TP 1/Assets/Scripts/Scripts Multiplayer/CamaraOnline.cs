using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode; // 1. Importamos Netcode

// 2. Heredamos de NetworkBehaviour en lugar de MonoBehaviour
public class CameraFollowOnline : NetworkBehaviour 
{
    [Header("Ajustes")]
    [SerializeField] public float Sensibilidad = 100;
    [SerializeField] public Transform Player;

    [Header("Estados")]
    [SerializeField] public float RotacionHorizontal = 0;
    [SerializeField] public float RotacionVertical = 0;

    // 3. Usamos OnNetworkSpawn en lugar de Start para inicializar cosas de red
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Si este es MI jugador, bloqueo y oculto el cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Si es el jugador de otro cliente, le apago la cámara y el audio
            Camera cam = GetComponent<Camera>();
            if (cam != null) cam.enabled = false;

            AudioListener listener = GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;

            // Apago este script para que no se ejecute en los clones
            this.enabled = false; 
        }
    }

    void Update()
    {
        // 4. Si no soy el dueño de este jugador, ignoro los inputs de mouse
        if (!IsOwner) return;

        //Nos dan los valores del mouse para mover 
        float ValorX = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float ValorY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;

        //Guarda el valor y queda en el valor para seguir
        RotacionHorizontal += ValorX;
        RotacionVertical -= ValorY;

        //Limita en 80 grados
        RotacionVertical = math.clamp(RotacionVertical, -80, 80);


        //Hace la rotacion vertical fluida
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