using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovimientoOnline : NetworkBehaviour
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
    [SerializeField] private float limiteCaida = -10f;

    [Header("Salto y Gravedad")]
    [SerializeField] private float alturaSalto = 1.5f;
    [SerializeField] private float gravedad = -15f;
    [SerializeField, Range(1f, 3f)] private float multiplicadorCaida = 2f;
    [SerializeField] private float velocidadTerminal = -50f;
    
    private float velocidadVertical;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        // Solo el dueño local maneja la cámara, los inputs y la posición inicial
        if (!IsOwner) return;

        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }

        // Nos suscribimos a los eventos de carga de escena de Netcode
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += EscucharEventosDeEscena;
        }

        // Intento de posicionamiento inmediato
        BuscarPuntoSpawnYTeletransportar();
    }

    public override void OnNetworkDespawn()
    {
        // Desuscribirse para evitar fugas de memoria
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= EscucharEventosDeEscena;
        }
    }

    private void EscucharEventosDeEscena(SceneEvent sceneEvent)
    {
        // Cuando la escena termina de cargar completamente en el cliente o servidor
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted ||
            sceneEvent.SceneEventType == SceneEventType.SynchronizeComplete)
        {
            BuscarPuntoSpawnYTeletransportar();
        }
    }

    private void BuscarPuntoSpawnYTeletransportar()
    {
        if (puntoReaparicionInicial == null)
        {
            GameObject spawnEncontrado = GameObject.Find("SpawnPoint");
            if (spawnEncontrado != null)
            {
                puntoReaparicionInicial = spawnEncontrado.transform;
            }
        }

        ultimoPuntoReaparicion = puntoReaparicionInicial;

        if (ultimoPuntoReaparicion != null)
        {
            // Apagar temporalmente el CharacterController para aplicar la posición directamente
            controlador.enabled = false;
            
            // Se le suma 0.5f en Y para que no colisione con el suelo al inicio
            transform.position = ultimoPuntoReaparicion.position + Vector3.up * 0.5f;
            transform.rotation = ultimoPuntoReaparicion.rotation;
            
            velocidadVertical = 0f;
            velocidadPlanoActual = Vector3.zero;
            
            controlador.enabled = true;
        }
    }

    private void Update()
    {
        // Si no es el dueño local, no procesa input ni movimiento
        if (NetworkManager.Singleton != null && !IsOwner) return;

        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }

        // Si cayó al vacío, reaparece y corta este frame
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
            BuscarPuntoSpawnYTeletransportar();
            if (ultimoPuntoReaparicion == null)
            {
                Debug.LogWarning("PlayerMovimientoOnline: No se encontró 'SpawnPoint' en la escena.");
                return;
            }
        }

        velocidadVertical = 0f;
        velocidadPlanoActual = Vector3.zero;

        controlador.enabled = false;
        transform.position = ultimoPuntoReaparicion.position + Vector3.up * 0.5f;
        transform.rotation = ultimoPuntoReaparicion.rotation;
        controlador.enabled = true;
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