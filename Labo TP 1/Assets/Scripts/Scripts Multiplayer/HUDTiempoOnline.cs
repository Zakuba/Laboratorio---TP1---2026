using TMPro;
using UnityEngine;

public class HUDTiempoOnline : MonoBehaviour
{
    [SerializeField]
    private GestorPartidaOnline gestorPartida;

    [SerializeField]
    private TMP_Text textoTiempo;

    [SerializeField]
    private TMP_Text textoResultado;

    private void Start()
    {
        if (textoResultado != null)
        {
            textoResultado.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (gestorPartida == null || !gestorPartida.IsSpawned)
        {
            return;
        }

        ActualizarTiempo();
        ActualizarResultado();
    }

private void ActualizarTiempo()
{
    if (!gestorPartida.TiempoLimiteActivo.Value)
    {
        textoTiempo.gameObject.SetActive(false);
        return;
    }

    if (gestorPartida.Estado.Value == EstadoPartida.Esperando)
    {
        textoTiempo.gameObject.SetActive(false);
        return;
    }

    textoTiempo.gameObject.SetActive(true);

    int segundosTotales =
        gestorPartida.ObtenerSegundosRestantes();

    int minutos = segundosTotales / 60;
    int segundos = segundosTotales % 60;

    textoTiempo.text =
        $"{minutos:00}:{segundos:00}";
}

    private void ActualizarResultado()
    {
        bool tiempoAgotado =
            gestorPartida.Estado.Value == EstadoPartida.Finalizada &&
            gestorPartida.Resultado.Value == ResultadoPartida.TiempoAgotado;

        textoResultado.gameObject.SetActive(tiempoAgotado);

        if (tiempoAgotado)
        {
            textoResultado.text = "TIEMPO AGOTADO";
        }
    }
}