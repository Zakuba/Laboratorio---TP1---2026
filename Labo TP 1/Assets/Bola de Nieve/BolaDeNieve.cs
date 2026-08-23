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
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rbObjetivo =
                collision.gameObject.GetComponent<Rigidbody>();

            if (rbObjetivo != null)
            {
                Vector3 direccionEmpuje =
                    collision.transform.position - transform.position;

                direccionEmpuje.y = 0.5f;
                direccionEmpuje.Normalize();

                rbObjetivo.AddForce(
                    direccionEmpuje * fuerzaEmpuje,
                    ForceMode.Impulse
                );
            }
        }

        Destroy(gameObject);
    }
}