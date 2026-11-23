using UnityEngine;

public class RuchMrowki : MonoBehaviour
{
    [HideInInspector] public Transform[] punktyTrasy; // Wypełniane automatycznie przez spawner
    public float predkosc = 1.5f;
    public float predkoscObrotu = 8f;

    private int aktualnyPunkt = 0;

    void Update()
    {
        if (punktyTrasy == null || punktyTrasy.Length == 0) return;

        Transform cel = punktyTrasy[aktualnyPunkt];

        // 1. Ruch w stronę aktualnego punktu na trasie
        transform.position = Vector3.MoveTowards(
            transform.position,
            cel.position,
            predkosc * Time.deltaTime
        );

        // 2. Obracanie mrówki przodem do kolejnego punktu
        Vector3 kierunek = cel.position - transform.position;
        kierunek.y = 0;

        if (kierunek != Vector3.zero)
        {
            Quaternion docelowyObrot = Quaternion.LookRotation(kierunek);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                docelowyObrot,
                predkoscObrotu * Time.deltaTime
            );
        }

        // 3. Sprawdzenie, czy mrówka dotarła do punktu
        if (Vector3.Distance(transform.position, cel.position) < 0.05f)
        {
            aktualnyPunkt++;

            // Jeśli to był ostatni punkt, usuń mrówkę
            if (aktualnyPunkt >= punktyTrasy.Length)
            {
                Destroy(gameObject);
            }
        }
    }
}