using UnityEngine;

public class Snowball : MonoBehaviour
{
    public float fuerzaEmpuje = 15f;
    public float tiempoDeVida = 5f; // Despawnea tras 5s si se lanza al vacío

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verificamos si el objeto impactado tiene la etiqueta "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rbObjetivo = collision.gameObject.GetComponent<Rigidbody>();

            if (rbObjetivo != null)
            {
                // Calculamos la dirección del empuje (desde la bola hacia el jugador)
                Vector3 direccionEmpuje = collision.transform.position - transform.position;
                
                // Levantamos ligeramente el vector en el eje Y para que el empuje lo levante del suelo
                direccionEmpuje.y = 0.5f; 
                direccionEmpuje.Normalize();

                // Aplicamos la fuerza de empuje como un impacto repentino (Impulse)
                rbObjetivo.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode.Impulse);
            }
        }

        // Si choca con el jugador o con cualquier otra cosa, despawnea.
        Destroy(gameObject);
    }
}