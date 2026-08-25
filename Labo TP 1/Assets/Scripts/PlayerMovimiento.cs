using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovimiento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform camara;
    private CharacterController controlador;

    [Header("Movimiento Base")]
    [SerializeField] private float velocidadMaxima = 6f; 
    
    [Header("Aceleración y Desaceleración")]
    [SerializeField] private float aceleracion = 25f; // Qué tan rápido llega a la velocidad máxima
    [SerializeField] private float desaceleracion = 30f; // Qué tan rápido frena al soltar las teclas
    
    // Guardamos la velocidad actual en la que se mueve el jugador en el piso (X, Z)
    private Vector3 velocidadPlanoActual;

    [Header("Superficies")]
    [SerializeField] private float distanciaDeteccionSuperficie = 1.3f;

    private float multiplicadorVelocidadSuperficie = 1f;
    private float multiplicadorAceleracionSuperficie = 1f;
    private float multiplicadorFrenadoSuperficie = 1f;

    [Header("Reaparición")]
    [SerializeField] private Transform puntoReaparicionInicial;
    private Transform ultimoPuntoReaparicion;
    [SerializeField] private float limiteCaida = -10f;

    [Header("Salto y Gravedad")]
    [SerializeField] private float alturaSalto = 1.5f;
    [SerializeField] private float gravedad = -15f;
    [SerializeField, Range(1f, 3f)] private float multiplicadorCaida = 2f;
    [SerializeField] private float velocidadTerminal = -50f;
    
    // Guardamos solo la velocidad vertical (Y)
    private float velocidadVertical;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();

        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }

        ultimoPuntoReaparicion = puntoReaparicionInicial;

    }

    private void Update()
    {
        // Si el jugador cayó y reapareció, terminamos este frame
        if (DetectarCaida())
        {
            return;
        }

        // Detectamos sobre qué superficie está parado el jugador
        DetectarSuperficie();

        // 1. Calculamos el movimiento horizontal
        Vector3 movimientoPlano = CalcularMovimientoEnPlano();

        // 2. Calculamos la gravedad y el salto
        AplicarGravedadYSalto();

        // 3. Combinamos movimiento horizontal y vertical
        Vector3 movimientoFinal =
            movimientoPlano + (Vector3.up * velocidadVertical);

        // 4. Movemos al personaje
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

    private void Reaparecer()
    {
        if (ultimoPuntoReaparicion == null)
        {
            Debug.LogWarning(
                "PlayerMovimiento: asigna un punto de reaparición."
            );
            return;
        }

        velocidadVertical = 0f;
        velocidadPlanoActual = Vector3.zero;

        controlador.enabled = false;

        transform.position = ultimoPuntoReaparicion.position;

        controlador.enabled = true;
    }

    private void DetectarSuperficie()
    {
        // Por defecto usamos los valores normales
        multiplicadorVelocidadSuperficie = 1f;
        multiplicadorAceleracionSuperficie = 1f;
        multiplicadorFrenadoSuperficie = 1f;

        // Elevamos el origen del rayo ligeramente
        Vector3 origen = transform.position + Vector3.up * 0.2f;

        // Disparamos un rayo invisible hacia abajo (Raycast)
        if (Physics.Raycast(
            origen, // Punto de partida del rayo
            Vector3.down, // Dirección (hacia abajo)
            out RaycastHit hit,
            distanciaDeteccionSuperficie))
        {
            // Intentamos obtener el script 'SuperficieMovimiento' del objeto que pisamos
            SuperficieMovimiento superficie =
                hit.collider.GetComponentInParent<SuperficieMovimiento>();

            // Si el suelo efectivamente tiene ese script, aplicamos sus modificadores
            if (superficie != null)
            {
                multiplicadorVelocidadSuperficie =
                    superficie.MultiplicadorVelocidad;

                multiplicadorAceleracionSuperficie =
                    superficie.MultiplicadorAceleracion;

                multiplicadorFrenadoSuperficie =
                    superficie.MultiplicadorFrenado;
            }
        }
    }

    private Vector3 CalcularMovimientoEnPlano()
    {
        float valorHorizontal = Input.GetAxisRaw("Horizontal");
        float valorVertical = Input.GetAxisRaw("Vertical");

        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;

        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        Vector3 direccionDeseada = (derechaCamara * valorHorizontal + adelanteCamara * valorVertical);

        if (direccionDeseada.sqrMagnitude > 1f)
        {
            direccionDeseada.Normalize();
        }

        // Hacia dónde queremos ir y a qué velocidad máxima
        Vector3 velocidadObjetivo = direccionDeseada * velocidadMaxima * multiplicadorVelocidadSuperficie;

        // Determinamos si el jugador está intentando moverse o si soltó los controles
        bool seEstaMoviendo = direccionDeseada.sqrMagnitude > 0.1f;

        // Elegimos si aplicamos el valor de acelerar o de frenar
        float tasaDeCambio = seEstaMoviendo ? aceleracion * multiplicadorAceleracionSuperficie : desaceleracion * multiplicadorFrenadoSuperficie;

        // MoveTowards cambia gradualmente 'velocidadPlanoActual' hacia 'velocidadObjetivo'
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