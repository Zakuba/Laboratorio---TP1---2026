using UnityEngine;

public class PlayerShot : MonoBehaviour
{
public GameObject snowballPrefab;
    public Transform puntoDeDisparo; 
    public float fuerzaLanzamiento = 25f;
    public float tiempoEntreDisparos = 1.5f; // Segundos que debe esperar
    private float tiempoUltimoDisparo = 0f; // Guarda cuándo fue el último tiro

    void Update()
    {
        // Detectar el clic izquierdo Y comprobar si ya pasó el tiempo necesario
        if (Input.GetMouseButtonDown(0))
        {
            // Verificamos si la hora actual del juego es mayor al momento en que 
            // disparamos por última vez + el tiempo de espera.
            if (Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
            {
                LanzarBolaDeNieve();
                
                // Registramos el momento exacto en que acabamos de disparar
                tiempoUltimoDisparo = Time.time;
            }
            else
            {
                Debug.Log("¡Aún recargando bola de nieve!");
            }
        }
    }

    void LanzarBolaDeNieve()
    {
        GameObject bola = Instantiate(snowballPrefab, puntoDeDisparo.position, puntoDeDisparo.rotation);

        Rigidbody rbBola = bola.GetComponent<Rigidbody>();
        if (rbBola != null)
        {
            rbBola.linearVelocity = puntoDeDisparo.forward * fuerzaLanzamiento;
        }
    }
}
