using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ButtonVr : MonoBehaviour
{
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    
    public string sceneName; 
    public string targetTag = "Hand";
    public float delay = 0.5f;

    [Tooltip("O ile w dół ma zjechać guzik (np. 0.02)")]
    public float pressDistance = 0.02f;

    private float originalY; // Komputer sam zapamięta początkową wysokość
    GameObject presser;
    AudioSource sound;
    bool isPressed;

    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
        
        // Gra zapamiętuje oryginalną pozycję guzika przy starcie
        if (button != null)
        {
            originalY = button.transform.localPosition.y;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.CompareTag(targetTag))
        {
            // Obniża pozycję względem oryginału
            button.transform.localPosition = new Vector3(button.transform.localPosition.x, originalY - pressDistance, button.transform.localPosition.z);
            presser = other.gameObject;
            onPress.Invoke();
            
            if (sound != null) sound.Play();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser)
        {
            // Wraca idealnie do oryginalnej pozycji
            button.transform.localPosition = new Vector3(button.transform.localPosition.x, originalY, button.transform.localPosition.z);
            onRelease.Invoke();
            isPressed = false;
        }
    }

    public void LoadGameScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Invoke("ExecuteLoadScene", delay);
        }
    }

    private void ExecuteLoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}