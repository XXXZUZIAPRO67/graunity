using System.Collections;
using UnityEngine;

public class AntlerDecorator : MonoBehaviour
{
    [Header("Kwiatki ukryte na porożu")]
    public GameObject[] flowersOnAntlers;

    [Header("Ustawienia machania głową")]
    public float shakeThreshold = 1.8f;
    public float timeToAttach = 1.0f;

    [Header("Efekty ukończenia (100% kwiatków)")]
    public AudioSource attachSound;       // Dźwięk pojedynczego kwiatka
    public AudioSource completionSound;   // Dźwięk ukończenia całości (fanfara/magia)
    public ParticleSystem completionParticles; // Cząsteczki (iskierki/płatki) przy porożu
    public float pulseScaleMultiplier = 1.5f;  // Jak mocno kwiatki spuchną w animacji

    private bool isInFlowers = false;
    private float shakeTimer = 0f;
    private int currentFlowerIndex = 0;
    private Quaternion lastRotation;
    private bool isCompleted = false;

    void Start()
    {
        lastRotation = transform.rotation;
        
        foreach (GameObject flower in flowersOnAntlers)
        {
            if (flower != null) flower.SetActive(false);
        }
    }

    void Update()
    {
        // Jeśli już udekorowano całe poroże, przerywamy działanie
        if (isCompleted) return;

        if (isInFlowers && currentFlowerIndex < flowersOnAntlers.Length)
        {
            float angleDelta = Quaternion.Angle(lastRotation, transform.rotation);
            float shakeSpeed = angleDelta / Time.deltaTime / 100f;

            if (shakeSpeed > shakeThreshold)
            {
                shakeTimer += Time.deltaTime;

                if (shakeTimer >= timeToAttach)
                {
                    AttachNextFlower();
                    shakeTimer = 0f;
                }
            }
        }

        lastRotation = transform.rotation;
    }

    private void AttachNextFlower()
    {
        if (currentFlowerIndex < flowersOnAntlers.Length)
        {
            flowersOnAntlers[currentFlowerIndex].SetActive(true);
            currentFlowerIndex++;

            // Sprawdzamy, czy to był OSTATNI kwiatek
            if (currentFlowerIndex >= flowersOnAntlers.Length)
            {
                isCompleted = true;
                StartCoroutine(PlayCompletionAnimation());
            }
            else if (attachSound != null)
            {
                attachSound.Play();
            }
        }
    }

    private IEnumerator PlayCompletionAnimation()
    {
        // 1. Odtwórz dźwięk ukończenia i cząsteczki
        if (completionSound != null) completionSound.Play();
        if (completionParticles != null) completionParticles.Play();

        // 2. Zapisz oryginalne rozmiary wszystkich kwiatków
        Vector3[] originalScales = new Vector3[flowersOnAntlers.Length];
        for (int i = 0; i < flowersOnAntlers.Length; i++)
        {
            if (flowersOnAntlers[i] != null)
                originalScales[i] = flowersOnAntlers[i].transform.localScale;
        }

        // 3. Animacja: dwa razy pulsowanie (powiększenie i powrót)
        for (int pulse = 0; pulse < 2; pulse++)
        {
            float timer = 0f;
            float duration = 0.25f;

            // Powiększanie
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float progress = timer / duration;
                for (int i = 0; i < flowersOnAntlers.Length; i++)
                {
                    if (flowersOnAntlers[i] != null)
                        flowersOnAntlers[i].transform.localScale = Vector3.Lerp(originalScales[i], originalScales[i] * pulseScaleMultiplier, progress);
                }
                yield return null;
            }

            timer = 0f;
            // Powrót do normalnego rozmiaru
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float progress = timer / duration;
                for (int i = 0; i < flowersOnAntlers.Length; i++)
                {
                    if (flowersOnAntlers[i] != null)
                        flowersOnAntlers[i].transform.localScale = Vector3.Lerp(originalScales[i] * pulseScaleMultiplier, originalScales[i], progress);
                }
                yield return null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FlowerBush"))
        {
            isInFlowers = true;
            shakeTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FlowerBush"))
        {
            isInFlowers = false;
            shakeTimer = 0f;
        }
    }
}