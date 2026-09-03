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
    [SerializeField]
    private GameObject panelResultado;
    [SerializeField] private GameObject panelTiempo;

private void Start()
{
    if (panelTiempo != null)
    {
        panelTiempo.SetActive(false);
    }

    if (panelResultado != null)
    {
        panelResultado.SetActive(false);
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
        if (panelTiempo != null)
            panelTiempo.SetActive(false);

        return;
    }

    if (gestorPartida.Estado.Value != EstadoPartida.Jugando)
    {
        if (panelTiempo != null)
            panelTiempo.SetActive(false);

        return;
    }

    if (panelTiempo != null)
        panelTiempo.SetActive(true);

    if (textoTiempo != null)
        textoTiempo.gameObject.SetActive(true);

    int segundosTotales =
        gestorPartida.ObtenerSegundosRestantes();

    int minutos = segundosTotales / 60;
    int segundos = segundosTotales % 60;

    textoTiempo.text = $"{minutos:00}:{segundos:00}";
}

private void ActualizarResultado()
{
    bool tiempoAgotado =
        gestorPartida.Estado.Value == EstadoPartida.Finalizada &&
        gestorPartida.Resultado.Value == ResultadoPartida.TiempoAgotado;

    if (panelResultado != null)
        panelResultado.SetActive(tiempoAgotado);

    if (tiempoAgotado)
    {
        if (panelTiempo != null)
            panelTiempo.SetActive(false);

        textoResultado.text = "TIEMPO AGOTADO";
    }
}
}