using TMPro;
using UnityEngine;
using System.Collections; // Necesario para las corrutinas

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
    
    [SerializeField] 
    private GameObject panelTiempo;

    [Header("Cuenta Regresiva")]
    [SerializeField]
    private TMP_Text textoCentral; // Asigna aquí el texto gigante que dirá "5", "4" y "ESCAPE"
    private bool mensajeEscapeMostrado = false;

    private void Start()
    {
        if (panelTiempo != null) panelTiempo.SetActive(false);
        if (panelResultado != null) panelResultado.SetActive(false);
        if (textoCentral != null) textoCentral.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gestorPartida == null || !gestorPartida.IsSpawned)
        {
            return;
        }

        ActualizarTiempo();
        ActualizarResultado();
        ActualizarCuentaRegresiva(); // <-- Agregamos la verificación de la cuenta
    }

    private void ActualizarTiempo()
    {
        if (!gestorPartida.TiempoLimiteActivo.Value)
        {
            if (panelTiempo != null) panelTiempo.SetActive(false);
            return;
        }

        if (gestorPartida.Estado.Value != EstadoPartida.Jugando)
        {
            if (panelTiempo != null) panelTiempo.SetActive(false);
            return;
        }

        if (panelTiempo != null) panelTiempo.SetActive(true);
        if (textoTiempo != null) textoTiempo.gameObject.SetActive(true);

        int segundosTotales = gestorPartida.ObtenerSegundosRestantes();
        int minutos = segundosTotales / 60;
        int segundos = segundosTotales % 60;

        textoTiempo.text = $"{minutos:00}:{segundos:00}";
    }

    private void ActualizarResultado()
    {
        bool tiempoAgotado =
            gestorPartida.Estado.Value == EstadoPartida.Finalizada &&
            gestorPartida.Resultado.Value == ResultadoPartida.TiempoAgotado;

        if (panelResultado != null) panelResultado.SetActive(tiempoAgotado);

        if (tiempoAgotado)
        {
            if (panelTiempo != null) panelTiempo.SetActive(false);
            textoResultado.text = "TIEMPO AGOTADO";
        }
    }

    // --- NUEVO: Lógica de la interfaz de la cuenta regresiva ---
    private void ActualizarCuentaRegresiva()
    {
        // Si estamos en la fase de cuenta regresiva (asegúrate de tener este estado en tu enum)
        if (gestorPartida.Estado.Value == EstadoPartida.CuentaRegresiva)
        {
            if (textoCentral != null)
            {
                textoCentral.gameObject.SetActive(true);
                textoCentral.text = gestorPartida.CuentaRegresiva.Value.ToString();
                mensajeEscapeMostrado = false; // Reiniciamos por si se reinicia la partida
            }
        }
        // Si el juego acaba de pasar a estado "Jugando" y aún no mostramos la palabra ESCAPE
        else if (gestorPartida.Estado.Value == EstadoPartida.Jugando && !mensajeEscapeMostrado)
        {
            mensajeEscapeMostrado = true;
            if (textoCentral != null)
            {
                textoCentral.text = "¡ESCAPE!";
                textoCentral.color = Color.green;
                StartCoroutine(OcultarTextoCentral());
            }
        }
    }

    private IEnumerator OcultarTextoCentral()
    {
        yield return new WaitForSeconds(2f);
        if (textoCentral != null)
        {
            textoCentral.gameObject.SetActive(false);
        }
    }
}