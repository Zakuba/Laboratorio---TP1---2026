using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovimiento : NetworkBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform camara;
    private CharacterController controlador;

    [Header("Movimiento Base")]
    [SerializeField] private float velocidadMaxima = 6f; 
    
    [Header("Aceleración y Desaceleración")]
    [SerializeField] private float aceleracion = 25f;
    [SerializeField] private float desaceleracion = 30f;
    
    private Vector3 velocidadPlanoActual;

    [Header("Superficies")]
    [SerializeField] private float distanciaDeteccionSuperficie = 1.3f;

    private float multiplicadorVelocidadSuperficie = 1f;
    private float multiplicadorAceleracionSuperficie = 1f;
    private float multiplicadorFrenadoSuperficie = 1f;

    [Header("Reaparición")]
    [SerializeField] private Transform puntoReaparicionInicial;
    private Transform ultimoPuntoReaparicion;
    [SerializeField] private float limiteCaida = -15f;

    [Header("Salto y Gravedad")]
    [SerializeField] private float alturaSalto = 1.5f;
    [SerializeField] private float gravedad = -15f;
    [SerializeField, Range(1f, 3f)] private float multiplicadorCaida = 2f;
    [SerializeField] private float velocidadTerminal = -50f;
    
    private float velocidadVertical;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();

        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (DetectarCaida())
        {
            return;
        }

        DetectarSuperficie();

        Vector3 movimientoPlano = CalcularMovimientoEnPlano();
        AplicarGravedadYSalto();

        Vector3 movimientoFinal = movimientoPlano + (Vector3.up * velocidadVertical);
        controlador.Move(movimientoFinal * Time.deltaTime);
    }

    private bool DetectarCaida()
    {
        if (transform.position.y < limiteCaida)
        {
            Reaparecer();
            return true;
        }

        return false;
    }

    public void Reaparecer()
    {
        if (ultimoPuntoReaparicion == null)
        {
            GameObject spawn = GameObject.Find("SpawnPoint");
            if (spawn != null) ultimoPuntoReaparicion = spawn.transform;
        }

        if (ultimoPuntoReaparicion != null)
        {
            velocidadVertical = 0f;
            velocidadPlanoActual = Vector3.zero;

            controlador.enabled = false;
            transform.position = ultimoPuntoReaparicion.position + Vector3.up * 0.5f;
            controlador.enabled = true;
        }
    }

    private void DetectarSuperficie()
    {
        multiplicadorVelocidadSuperficie = 1f;
        multiplicadorAceleracionSuperficie = 1f;
        multiplicadorFrenadoSuperficie = 1f;

        Vector3 origen = transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(origen, Vector3.down, out RaycastHit hit, distanciaDeteccionSuperficie))
        {
            SuperficieMovimiento superficie = hit.collider.GetComponentInParent<SuperficieMovimiento>();

            if (superficie != null)
            {
                multiplicadorVelocidadSuperficie = superficie.MultiplicadorVelocidad;
                multiplicadorAceleracionSuperficie = superficie.MultiplicadorAceleracion;
                multiplicadorFrenadoSuperficie = superficie.MultiplicadorFrenado;
            }
        }
    }

    private Vector3 CalcularMovimientoEnPlano()
    {
        float valorHorizontal = Input.GetAxisRaw("Horizontal");
        float valorVertical = Input.GetAxisRaw("Vertical");

        Vector3 adelanteCamara = camara != null ? camara.forward : transform.forward;
        Vector3 derechaCamara = camara != null ? camara.right : transform.right;

        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;

        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        Vector3 direccionDeseada = (derechaCamara * valorHorizontal + adelanteCamara * valorVertical);

        if (direccionDeseada.sqrMagnitude > 1f)
        {
            direccionDeseada.Normalize();
        }

        Vector3 velocidadObjetivo = direccionDeseada * velocidadMaxima * multiplicadorVelocidadSuperficie;
        bool seEstaMoviendo = direccionDeseada.sqrMagnitude > 0.1f;
        float tasaDeCambio = seEstaMoviendo ? aceleracion * multiplicadorAceleracionSuperficie : desaceleracion * multiplicadorFrenadoSuperficie;

        velocidadPlanoActual = Vector3.MoveTowards(
            velocidadPlanoActual, 
            velocidadObjetivo, 
            tasaDeCambio * Time.deltaTime
        );

        return velocidadPlanoActual;
    }

    private void AplicarGravedadYSalto()
    {
        if (controlador.isGrounded)
        {
            if (velocidadVertical < 0)
            {
                velocidadVertical = -2f;
            }

            if (Input.GetButtonDown("Jump"))
            {
                velocidadVertical = Mathf.Sqrt(alturaSalto * -2f * gravedad);
            }
        }

        float gravedadAplicada = gravedad;

        if (velocidadVertical < 0 && !controlador.isGrounded)
        {
            gravedadAplicada *= multiplicadorCaida;
        }

        velocidadVertical += gravedadAplicada * Time.deltaTime;

        if (velocidadVertical < velocidadTerminal)
        {
            velocidadVertical = velocidadTerminal;
        }
    }

    public void ActualizarCheckpoint(Transform nuevoPuntoReaparicion)
    {
        ultimoPuntoReaparicion = nuevoPuntoReaparicion;
        Debug.Log("Checkpoint actualizado.");
    }
}