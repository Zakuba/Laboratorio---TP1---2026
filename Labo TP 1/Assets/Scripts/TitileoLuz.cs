using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
    public Light luz;

    public float tiempoMinimo = 0.05f;
    public float tiempoMaximo = 0.3f;

    void Start()
    {
        StartCoroutine(Parpadear());
    }

    IEnumerator Parpadear()
    {
        while (true)
        {
            luz.enabled = !luz.enabled;

            float tiempo = Random.Range(tiempoMinimo, tiempoMaximo);
            yield return new WaitForSeconds(tiempo);
        }
    }
}