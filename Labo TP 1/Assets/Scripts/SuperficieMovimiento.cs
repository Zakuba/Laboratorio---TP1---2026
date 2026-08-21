using UnityEngine;

public class SuperficieMovimiento : MonoBehaviour
{
    [Header("Comportamiento de la superficie")]
    [SerializeField] private float multiplicadorVelocidad = 1f;
    [SerializeField] private float multiplicadorAceleracion = 1f;
    [SerializeField] private float multiplicadorFrenado = 1f;

    [Header("Documentación")]
    [SerializeField, TextArea] private string descripcion;

    public float MultiplicadorVelocidad => multiplicadorVelocidad;
    public float MultiplicadorAceleracion => multiplicadorAceleracion;
    public float MultiplicadorFrenado => multiplicadorFrenado;
}