using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class CameraFollowOnline : MonoBehaviour
{
    [Header("Ajustes")]
    [SerializeField] public float Sensibilidad = 100f;
    [SerializeField] public Transform Player;

    [Header("Referencias")]
    [SerializeField] private Camera miCamara;
    [SerializeField] private AudioListener miAudioListener;

    [Header("Estados")]
    [SerializeField] public float RotacionHorizontal = 0f;
    [SerializeField] public float RotacionVertical = 0f;

    private NetworkObject netObjRaiz;

    private void Awake()
    {
        if (miCamara == null) miCamara = GetComponent<Camera>();
        if (miAudioListener == null) miAudioListener = GetComponent<AudioListener>();

        netObjRaiz = GetComponentInParent<NetworkObject>();
        if (Player == null && netObjRaiz != null)
        {
            Player = netObjRaiz.transform;
        }
    }

    private void Start()
    {
        // Si este jugador no es el nuestro en la red, desactivamos su cámara y audio
        if (netObjRaiz != null && !netObjRaiz.IsOwner)
        {
            if (miCamara != null) miCamara.enabled = false;
            if (miAudioListener != null) miAudioListener.enabled = false;
            enabled = false;
            return;
        }

        // Activamos nuestra propia cámara y aseguramos el AudioListener único
        if (miCamara != null)
        {
            miCamara.enabled = true;
        }
        if (miAudioListener != null)
        {
            miAudioListener.enabled = true;
        }

        // Destruimos cualquier otra cámara o AudioListener sobrante en la escena para evitar el conflicto
        Camera[] todasLasCamaras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include);
        foreach (Camera cam in todasLasCamaras)
        {
            if (cam != miCamara)
            {
                Destroy(cam.gameObject);
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (netObjRaiz != null && !netObjRaiz.IsOwner) return;

        float ValorX = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float ValorY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;

        RotacionHorizontal += ValorX;
        RotacionVertical -= ValorY;
        RotacionVertical = math.clamp(RotacionVertical, -80f, 80f);

        transform.localRotation = Quaternion.Euler(RotacionVertical, 0f, 0f);

        if (Player != null)
        {
            Player.Rotate(Vector3.up * ValorX);
        }
    }
}