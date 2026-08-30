using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class PuntoDeDisparoOnline : MonoBehaviour
{
    [Header("Ajustes")]
    [SerializeField] public float Sensibilidad = 100f;

    [Header("Estados")]
    [SerializeField] public float RotacionHorizontal = 0f;
    [SerializeField] public float RotacionVertical = 0f;

    private NetworkObject netObjRaiz;

    private void Start()
    {
        netObjRaiz = GetComponentInParent<NetworkObject>();
        if (netObjRaiz != null && !netObjRaiz.IsOwner)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (netObjRaiz != null && !netObjRaiz.IsOwner) return;

        float ValorY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;
        RotacionVertical -= ValorY;
        RotacionVertical = math.clamp(RotacionVertical, -80f, 80f);

        transform.localRotation = Quaternion.Euler(RotacionVertical, 0f, 0f);
    }
}