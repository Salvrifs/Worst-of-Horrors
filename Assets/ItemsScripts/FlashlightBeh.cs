using UnityEngine;

public class FlashlightBeh : MonoBehaviour
{
    [Header("Настройки фонарика")]
    [SerializeField] private Light flashlight; 
    [SerializeField] private KeyCode toggleKey = KeyCode.M; 
    [SerializeField] private AudioClip toggleSound; 
    [SerializeField] private AudioClip RechargeSound;
    [SerializeField] private float batteryLife = 90f; 
    [SerializeField] private Vector3 offsetPosition = new Vector3(0, 0, 0.5f);
    private AudioSource audioSource;
    private bool isOn;
    private float currentBattery;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentBattery = batteryLife;
        flashlight.enabled = false;
    }

    /*void Update()
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
    }*/

    public void ToggleFlashlight()
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
        audioSource.PlayOneShot(toggleSound);
    }

    // Метод для подзарядки (можно вызывать из других скриптов)
    public void RechargeBattery()
    {
        currentBattery = batteryLife;
        audioSource.PlayOneShot(toggleSound);
    }
    public void AttachToPlayer(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = offsetPosition;
        transform.localRotation = Quaternion.identity;
    }
}