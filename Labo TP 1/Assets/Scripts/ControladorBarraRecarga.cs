using UnityEngine;
using UnityEngine.UI;

public class ControladorBarraRecarga : MonoBehaviour
{
    [SerializeField] private Image barraRecarga;
    [SerializeField] private float tiempoRecarga = 1.5f;

    private float tiempoInicioRecarga;
    private bool recargando;

    private void Start()
    {
        barraRecarga.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!recargando)
            return;

        float tiempoTranscurrido = Time.time - tiempoInicioRecarga;

        float progreso = tiempoTranscurrido / tiempoRecarga;

        // Se va vaciando de izquierda a derecha
        barraRecarga.fillAmount = 1f - progreso;

        if (tiempoTranscurrido >= tiempoRecarga)
        {
            recargando = false;
            barraRecarga.fillAmount = 0f;
            barraRecarga.gameObject.SetActive(false);
        }
    }

    public void IniciarRecarga(float duracion)
    {
        tiempoInicioRecarga = Time.time;
        tiempoRecarga = duracion;
        recargando = true;

        barraRecarga.gameObject.SetActive(true);
        barraRecarga.fillAmount = 1f;
    }
}