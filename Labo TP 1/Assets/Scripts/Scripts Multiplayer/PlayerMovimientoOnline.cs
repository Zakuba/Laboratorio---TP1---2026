using UnityEngine;

using Unity.Netcode;

using UnityEngine.SceneManagement;



[RequireComponent(typeof(CharacterController))]

// 2. Heredar de NetworkBehaviour

public class PlayerMovimientoOnline : NetworkBehaviour

{
    [Header("Efectos")]
    private Vector3 fuerzaEmpujeActual = Vector3.zero;
    [SerializeField] private float amortiguacionEmpuje = 5f; // Qué tan rápido frena después de ser empujado

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



        if (camara == null && Camera.main != null)

        {

            camara = Camera.main.transform;

        }



        // Si la referencia se perdió al spawnear en red, buscamos el objeto en la escena por su nombre

        if (puntoReaparicionInicial == null)

        {

            // Busca un objeto que se llame exactamente "SpawnPoint" en tu jerarquía

            GameObject spawnEnEscena = GameObject.Find("SpawnPoint");

            if (spawnEnEscena != null)

            {

                puntoReaparicionInicial = spawnEnEscena.transform;

            }

        }



        ultimoPuntoReaparicion = puntoReaparicionInicial;

    }



    private void Update()

    {

        // 3. ¡MUY IMPORTANTE! Si no soy el dueño, no proceso movimiento ni físicas.

        if (!IsOwner) return;



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

        // --- NUEVO: Reducimos el empuje suavemente hacia cero ---
        fuerzaEmpujeActual = Vector3.Lerp(fuerzaEmpujeActual, Vector3.zero, amortiguacionEmpuje * Time.deltaTime);

        // 3. Combinamos movimiento horizontal, vertical Y EL EMPUJE
        Vector3 movimientoFinal = movimientoPlano + (Vector3.up * velocidadVertical) + fuerzaEmpujeActual;

        // 4. Movemos al personaje
        controlador.Move(movimientoFinal * Time.deltaTime);

    }

    public override void OnNetworkSpawn()

    {

        // Le indicamos al jugador que escuche cuando cambie una escena

        SceneManager.sceneLoaded += AlCargarEscena;

        // Si este personaje no es el mío, apago su CharacterController.

        // El Capsule Collider será el que reciba los disparos en su lugar.

        if (!IsOwner)

        {

            controlador.enabled = false;

        }

    }



    public override void OnNetworkDespawn()

    {

        // Limpiamos el evento por seguridad si el jugador se desconecta

        SceneManager.sceneLoaded -= AlCargarEscena;

    }



    private void AlCargarEscena(Scene escena, LoadSceneMode modo)

    {

        if (!IsOwner) return;



        // Cuando la escena "Nivel1" termina de cargar, vaciamos el punto anterior

        // para forzar al método Reaparecer() a buscar el nuevo SpawnPoint.

        ultimoPuntoReaparicion = null;

        Reaparecer();

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

        multiplicadorVelocidadSuperficie = 1f;

        multiplicadorAceleracionSuperficie = 1f;

        multiplicadorFrenadoSuperficie = 1f;



        Vector3 origen = transform.position + Vector3.up * 0.2f;



        if (Physics.Raycast(

            origen,

            Vector3.down,

            out RaycastHit hit,

            distanciaDeteccionSuperficie))

        {

            SuperficieMovimiento superficie =

                hit.collider.GetComponentInParent<SuperficieMovimiento>();



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

// [ClientRpc] hace que el Servidor envíe esta orden a los Clientes
    [ClientRpc]
    public void AplicarEmpujeClientRpc(Vector3 fuerza)
    {
        // Solo el dueño de este personaje aplica el empuje físicamente
        if (!IsOwner) return;

        fuerzaEmpujeActual += fuerza;
    }
} 

