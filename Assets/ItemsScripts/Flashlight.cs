using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Настройки фонарика")]
    [SerializeField] private Light flashlight; 
    [SerializeField] private KeyCode toggleKey = KeyCode.F; 
    [SerializeField] private AudioClip toggleSound; 
    [SerializeField] private float batteryLife = 60f; 
    
    private AudioSource audioSource;
    private bool isOn;
    private float currentBattery;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentBattery = batteryLife;
        flashlight.enabled = false; // Выключаем свет при старте
    }

    void Update()
    {
        // Включение/выключение по нажатию клавиши
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }

        // Расход заряда батареи
        if (isOn)
        {
            currentBattery -= Time.deltaTime;
            if (currentBattery <= 0)
            {
                currentBattery = 0;
                TurnOff();
            }
        }
    }

    void ToggleFlashlight()
    {
        if (currentBattery > 0)
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
            
            if (toggleSound != null)
            {
                audioSource.PlayOneShot(toggleSound);
            }
        }
    }

    void TurnOff()
    {
        isOn = false;
        flashlight.enabled = false;
    }

    // Метод для подзарядки (можно вызывать из других скриптов)
    public void RechargeBattery(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0, batteryLife);
    }
}