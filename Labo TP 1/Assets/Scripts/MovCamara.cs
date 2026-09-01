using UnityEngine;

public class MovCamara : MonoBehaviour
{
    [Header("Movimiento vertical")]
    public float amplitud = 0.05f;
    public float velocidad = 0.5f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        float movimientoY = Mathf.Sin(Time.time * velocidad) * amplitud;

        transform.localPosition = posicionInicial + new Vector3(0, movimientoY, 0);
    }
}