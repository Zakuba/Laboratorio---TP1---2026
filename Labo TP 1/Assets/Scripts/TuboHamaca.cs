using UnityEngine;

public class TuboHamaca : MonoBehaviour
{
    public float angulo = 35f;
    public float velocidad = 2f;

    private Quaternion rotacionInicial;

    void Start()
    {
        rotacionInicial = transform.localRotation;
    }

    void Update()
    {
        float movimiento = Mathf.Sin(Time.time * velocidad) * angulo;

        transform.localRotation =
            rotacionInicial * Quaternion.Euler(0, 0, movimiento);
    }
}