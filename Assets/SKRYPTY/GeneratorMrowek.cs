using UnityEngine;

public class GeneratorMrowek : MonoBehaviour
{
    public GameObject prefabMrowki;     // Wzór (Prefab) Twojej mrówki
    public Transform[] punktyTrasy;     // Lista wszystkich punktów trasy
    public float odstepCzasowy = 1.2f;  // Co ile sekund wychodzi kolejna mrówka
    public int maksymalnaLiczba = 20;   // Limit mrówek na scenie jednocześnie (ważne dla VR)

    private float licznikCzasu = 0f;

    void Update()
    {
        if (prefabMrowki == null || punktyTrasy.Length == 0) return;

        licznikCzasu += Time.deltaTime;

        // Wypuść nową mrówkę, jeśli minął odpowiedni czas i nie przekroczono limitu
        if (licznikCzasu >= odstepCzasowy && transform.childCount < maksymalnaLiczba)
        {
            WypuscMrowke();
            licznikCzasu = 0f;
        }
    }

    void WypuscMrowke()
    {
        // Tworzy mrówkę dokładnie w miejscu pierwszego punktu (punktyTrasy[0])
        GameObject nowaMrowka = Instantiate(
            prefabMrowki, 
            punktyTrasy[0].position, 
            Quaternion.identity, 
            transform // Ustawia spawner jako rodzica (porządek w Hierarchy)
        );

        RuchMrowki skryptRuchu = nowaMrowka.GetComponent<RuchMrowki>();
        if (skryptRuchu != null)
        {
            skryptRuchu.punktyTrasy = punktyTrasy;
        }
    }
}