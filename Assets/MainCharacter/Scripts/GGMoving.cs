using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Leguar.LowHealth;

public class GGMoving : MonoBehaviour
{
    [Header("Shader Effects")]
    [SerializeField] [Range(0, 1)] private float maxVisionLoss = 0.7f;
    [SerializeField] [Range(0, 1)] private float maxDetailLoss = 0.5f;
    [SerializeField] [Range(0, 1)] private float maxColorLoss = 1f;
    [SerializeField] [Range(0, 1)] private float maxDoubleVision = 0.4f;

    [Header("Infection Settings")]
    [SerializeField] private float infectionDuration = 15f * 60f; // 15 minutes
    [SerializeField] private float npcAvoidanceDuration = 5f * 60f; 
    [SerializeField] private float mutationDuration = 10f * 60f; 
    private bool isInfected = false;
    private bool isMutated = false;
    private float originalSpeed;
    private float originalJumpPower;
    //public LowHealthDirectAccess shaderAccessScript;
    //public LowHealthController shaderControllerScript;
    [SerializeField] private Image infectionEffectImage;

    [Header("\t==============\n\tMovement Settings\n\t==============")]
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _jumpPower = 5.0f;
    [SerializeField] private float _speedRun = 10.0f;
    [SerializeField] private float _speedSit = 2.0f;

    [Header("\t==============\n\tStamina Settings\n\t==============")]
    [SerializeField] private float RunCost = 2f;
    [SerializeField] private float jumpCost = 4f;
    [SerializeField] private float MaxStamina = 50f;
    [SerializeField] private float StaminaRegenDelay = 2f;
    [SerializeField] private float StaminaRegenRate = 5f;
    [SerializeField] private Slider StaminaBar; 
    [SerializeField] private Image StaminaFill; 
    [SerializeField] private float Current_Stamina;


    [Header("\t==============\n\tAudio Settings\n\t==============")]
    AudioSource audioSource;
    public AudioClip[] WalkSounds;
    public AudioClip jumpSound;

    [Header("\t==============\n\tPlayer Life Settings\n\t==============")]
    public float timerOfPlayerLive;
    //[SerializeField] private Text healthCount;
    [SerializeField] private float MaxTimeOfPlay = 6000f;
    
[Header("Step Settings")]
[SerializeField] private float walkStepInterval = 0.5f;
[SerializeField] private float runStepInterval = 0.3f;
[SerializeField] private float crouchStepInterval = 0.7f;
private float stepTimer;

    private float wakingUp1;
	private float wakingUp2;
	private float takingDamage;
    private float beingDizzy;
    private float NumOfSound;
    private Vector3 _walkDirection;
    private Vector3 _velocity;
    private float _speedWalk;
    private CharacterController _characterController;
    private bool isSprinting = false;
    private bool isJumping = false;
    private Coroutine regenCoroutine;

    

    void Start()
    {
        //shaderControllerScript.SetPlayerHealthSmoothly(1f, 1f); // 40% здоровья
        _speedWalk = _speed;
        _characterController = GetComponent<CharacterController>();
        Current_Stamina = MaxStamina;
        StaminaBar.gameObject.SetActive(false);
        StartCoroutine(InfectionTimer());
        infectionEffectImage.color = new Color(1, 0, 0, 0);
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        timerOfPlayerLive += Time.deltaTime;

        //if(timerOfPlayerLive > MaxTimeOfPlay)
        //{
         //   transform.tag = "noPlayerMore";
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //}

        float horizontal_input = Input.GetAxis("Horizontal");
        float vertical_input = Input.GetAxis("Vertical");

        _walkDirection = transform.forward * vertical_input + transform.right * horizontal_input;

        HandleStamina();
        UpdateStaminaUI();    
        Jump(_characterController.isGrounded && Input.GetKey(KeyCode.Space) && Current_Stamina >= jumpCost);
        ChangeMoveSpeed();
    }

    private void FixedUpdate()
    {
        Walk(_walkDirection);
        Gravity(_characterController.isGrounded);
        ChangeSound();
    }
    //
    //Игрок идёт
    //
    private void Walk(Vector3 direction)
    {
        _characterController.Move(direction * _speed * Time.fixedDeltaTime);
        ChangeSound();
    }
    //
    //Гравитация
    //
    private void Gravity(bool isGrounded)
{
    if (isGrounded && _velocity.y < 0)
    {
        _velocity.y = -1f;
    }
    _velocity.y -= _gravity * Time.fixedDeltaTime;
    _characterController.Move(_velocity * Time.fixedDeltaTime);
}
    //
    //Прыжок
    //
   private void Jump(bool canJump)
{
    if (canJump && !isJumping) // Добавляем проверку на состояние прыжка
    {
        isJumping = true;
        Debug.Log("Jumping");
        
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        audioSource.PlayOneShot(jumpSound);
        _velocity.y = Mathf.Sqrt(_jumpPower * 2f * _gravity); // Правильная формула прыжка
        Current_Stamina -= jumpCost;
        StartCoroutine(JumpCooldown());
    }
}

private IEnumerator JumpCooldown()
{
    yield return new WaitForSeconds(0.5f);
    isJumping = false;
}

    //
    //Скорость движения
    //
    private void ChangeMoveSpeed()
{
    bool isCrouching = Input.GetKey(KeyCode.LeftControl);
    bool wantSprint = Input.GetKey(KeyCode.LeftShift);

    if (isCrouching)
    {
        // При приседании
        _characterController.height = Mathf.Lerp(_characterController.height, 1f, Time.deltaTime * 5f);
        _speed = _speedSit;
        isSprinting = false; // Блокируем спринт
    }
    else if (wantSprint && Current_Stamina >= RunCost)
    {
        // Только если не присели и есть стамина
        _characterController.height = Mathf.Lerp(_characterController.height, 2f, Time.deltaTime * 5f);
        _speed = _speedRun;
        isSprinting = true;
    }
    else
    {
        // Обычное состояние
        _characterController.height = Mathf.Lerp(_characterController.height, 2f, Time.deltaTime * 5f);
        _speed = _speedWalk;
        isSprinting = false;
    }
}          
    //
    //Выносливость
    //
    private void HandleStamina()
    {
        if (Current_Stamina < RunCost)
        {
            isSprinting = false;
        }

        if (isSprinting)
        {
            Current_Stamina -= RunCost;
            StaminaBar.gameObject.SetActive(true);

            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
                regenCoroutine = null;
            }
        }

        else if (Current_Stamina < MaxStamina && regenCoroutine == null && !isSprinting)
        {
            regenCoroutine = StartCoroutine(RegenStamina());
        }

        
        
        Current_Stamina = Math.Clamp(Current_Stamina, 0, MaxStamina);
    }
    //
    //Корутина восстановления
    //
    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(StaminaRegenDelay);

        while (Current_Stamina < MaxStamina && !isSprinting)
        {
            Current_Stamina += StaminaRegenRate*Time.deltaTime;
            Current_Stamina = Math.Clamp(Current_Stamina, 0, MaxStamina);
            yield return null;
        }
    } 
    //
    //Обновление UI-шкалы выносливости
    //
    private void UpdateStaminaUI()
    {
        StaminaBar.value = Current_Stamina;

        // Плавное изменение видимости
        float targetAlpha = (Current_Stamina < MaxStamina) ? 1f : 0f;
        CanvasGroup staminaCG = StaminaBar.GetComponent<CanvasGroup>();
        if (staminaCG == null) staminaCG = StaminaBar.gameObject.AddComponent<CanvasGroup>();

        staminaCG.alpha = Mathf.Lerp(staminaCG.alpha, targetAlpha, Time.deltaTime * 5f);

        // Изменение цвета
        StaminaFill.color = Current_Stamina <= 26f ?
            Color.Lerp(StaminaFill.color, Color.red, Time.deltaTime * 5f) :
            Color.Lerp(StaminaFill.color, Color.white, Time.deltaTime * 5f);

        // Скрываем шкалу, если стамина полностью восстановлена
        if (Current_Stamina >= MaxStamina && staminaCG.alpha < 0.1f)
        {
            StaminaBar.gameObject.SetActive(false);
        }
    }
    //
    //Звуки
    //
    private void ChangeSound()
{
    bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || 
                   Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

    if (isMoving && _characterController.isGrounded)
    {
        float currentInterval = isSprinting ? runStepInterval : 
                              (Input.GetKey(KeyCode.LeftControl) ? crouchStepInterval : walkStepInterval);

        stepTimer -= Time.deltaTime;
        
        if (stepTimer <= 0)
        {
            audioSource.pitch = isSprinting ? 1.2f : 
                               (Input.GetKey(KeyCode.LeftControl) ? 0.8f : 1f);
            audioSource.PlayOneShot(WalkSounds[UnityEngine.Random.Range(0, WalkSounds.Length)]);
            stepTimer = currentInterval;
        }
    }
    else
    {
        stepTimer = 0;
        if (!audioSource.isPlaying) audioSource.Stop();
    }
}
    //
    //таймер инфекции
    //
    private IEnumerator InfectionTimer()
    {
        Debug.Log($"currentTime InfectionTimer: {timerOfPlayerLive}" );
        yield return new WaitForSeconds(infectionDuration);
        Debug.Log($"currentTime InfectionTimer: {timerOfPlayerLive}" );
        if (!isInfected)
        {
            Debug.Log("AAAAAA");
            StartCoroutine(ApplyInfectionEffects());
            InfectionPlayer();
        }
    }
    //
    //Инфекция игрока по истечении времени
    //
    private void InfectionPlayer()
{
    Debug.Log($"Time: {timerOfPlayerLive} InfectionPlayer 40f");
    isInfected = true;
    _speed *= 0.75f;
    _jumpPower *= 0.75f; 
    //shaderControllerScript.SetPlayerHealthSmoothly(0.4f, 1f); // 40% здоровья
    StartCoroutine(NPCAvoidanceTimer());
}
    //
    //Таймер чтобы игрока не трогали NPC
    //
    private IEnumerator NPCAvoidanceTimer()
{
    yield return new WaitForSeconds(npcAvoidanceDuration);
    
    Debug.Log($"Time: {timerOfPlayerLive} Avoidance 20f");
    //shaderControllerScript.SetPlayerHealthSmoothly(0.2f, 1f); // 20% здоровья
    transform.tag = "noPlayerMore";
    StartCoroutine(MutationTimer());
}
    //
    //Таймер мутации
    //
    private IEnumerator MutationTimer()
{
    yield return new WaitForSeconds(mutationDuration);
    if (isInfected)
    {
        PerformMutation();
    }
}
    //
    //Исполнение мутации
    //
    private void PerformMutation()
{
    isMutated = true;
    //shaderControllerScript.SetPlayerHealthSmoothly(0.1f, 1f); // 10% здоровья




        // Шейдерные эффекты
        /*shaderAccessScript.colorLossEffect = 1f;
        shaderAccessScript.colorLossTowardRed = 1f;
        shaderAccessScript.visionLossEffect = 0.8f;
        shaderAccessScript.detailLossEffect = 0.6f;
        shaderAccessScript.doubleVisionEffect = 0.5f;
        shaderAccessScript.UpdateShaderProperties();

        // Визуальные изменения
       // _characterController.height = 1f;
        //infectionEffectImage.color = new Color(1, 0, 0, 0.3f); */
}    

    

    //
    //Эффекты мутаций
    //


    
    //
    //              ВСПОМОГАТЕЛЬНАЯ
    //Для эффектов
    //
    private float smoothCurve(float time) {
			if (time>=1f) {
				return 0f;
			}
			float t;
			if (time<0.1f) {
				t = time*5f;
			} else {
				t = 0.5f+(time-0.1f)/0.9f*0.5f;
			}
			float sin = Mathf.Sin(Mathf.PI*t);
			return sin;
		}
        private IEnumerator ApplyInfectionEffects()
    {
        float elapsedTime = 0f;
        //float startVision = shaderAccessScript.visionLossEffect;
        //float startDetail = shaderAccessScript.detailLossEffect;
        //float startColor = shaderAccessScript.colorLossEffect;
        //float startDouble = shaderAccessScript.doubleVisionEffect;

        while (elapsedTime < infectionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / infectionDuration;

            //shaderAccessScript.visionLossEffect = Mathf.Lerp(startVision, maxVisionLoss, t);
            //shaderAccessScript.detailLossEffect = Mathf.Lerp(startDetail, maxDetailLoss, t);
            //shaderAccessScript.colorLossEffect = Mathf.Lerp(startColor, maxColorLoss, t);
            //shaderAccessScript.doubleVisionEffect = Mathf.Lerp(startDouble, maxDoubleVision, t);
            
            //shaderAccessScript.UpdateShaderProperties();
            yield return null;
        }
    }
    
}

