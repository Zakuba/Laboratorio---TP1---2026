using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode; // 1. Importar Netcode

// 2. Heredar de NetworkBehaviour
public class PuntoDeDisparoOnline : NetworkBehaviour 
{
    [Header("Ajustes")]
    [SerializeField] public float Sensibilidad = 100;

    [Header("Estados")]
    [SerializeField] public float RotacionHorizontal = 0;
    [SerializeField] public float RotacionVertical = 0;

    // 3. Usar OnNetworkSpawn en lugar de Start
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Bloquea el cursor en el centro de la pantalla
            Cursor.lockState = CursorLockMode.Locked;

            // Oculta el cursor mientras juegas
            Cursor.visible = false;
        }
        else
        {
            // Si es el clon de otro jugador, apagamos este script
            this.enabled = false;
        }
    }

    void Update()
    {
        // 4. Si no soy el dueño, no proceso el movimiento del mouse
        if (!IsOwner) return;

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
    }
}