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

public class GGMoving : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _jumpPower = 5.0f;
    [SerializeField] private float _speedRun = 10.0f;
    [SerializeField] private float _speedSit = 2.0f;

    [Header("Stamina Settings")]
    [SerializeField] private float RunCost = 2f;
    [SerializeField] private float jumpCost = 4f;
    [SerializeField] private float MaxStamina = 50f;
    [SerializeField] private float StaminaRegenDelay = 2f;
    [SerializeField] private float StaminaRegenRate = 5f;
    [SerializeField] private Slider StaminaBar; // Должен быть настроен как горизонтальный Slider
    [SerializeField] private Image StaminaFill; // Ссылка на Image типа Fill

    [Header("Audio Settings")]
    public AudioSource WalkSound1;
    public AudioSource WalkSound2;
    public AudioSource WalkSound3;
    public AudioSource jumpSound;

    [Header("Player Life Settings")]
    public float timerOfPlayerLive;
    [SerializeField] private float MaxTimeOfPlay = 6000f;


    private float Current_Stamina;
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
        _speedWalk = _speed;
        _characterController = GetComponent<CharacterController>();
        Current_Stamina = MaxStamina;
        StaminaBar.enabled = false;
        
    }

    void Update()
    {
        timerOfPlayerLive += Time.deltaTime;

        if(timerOfPlayerLive > MaxTimeOfPlay)
        {
            transform.tag = "noPlayerMore";
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        float horizontal_input = Input.GetAxis("Horizontal");
        float vertical_input = Input.GetAxis("Vertical");

        _walkDirection = transform.forward * vertical_input + transform.right * horizontal_input;

        Jump(_characterController.isGrounded && Input.GetKey(KeyCode.Space));
        ChangeMoveSpeed(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl));
        HandleStamina();
        UpdateStaminaUI();        
    }

    private void FixedUpdate()
    {
        Walk(_walkDirection);
        Gravity(_characterController.isGrounded);
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
        if (canJump)
        {
            jumpSound.Play();
            _velocity.y = _jumpPower;
            Current_Stamina -= jumpCost*Time.deltaTime;

        }


    }
    //
    //Скорость движения
    //
    private void ChangeMoveSpeed(bool changeMoveSpeed)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            StaminaBar.enabled = true;
            _speed = changeMoveSpeed ? _speedRun : _speedWalk;
            isSprinting = true;
        }
        
        else
        {
            _characterController.height = changeMoveSpeed ? 1f : 2f;
            _speed = changeMoveSpeed ? _speedSit : _speedWalk;
            isSprinting = false;
        }
    }           
    //
    //Выносливость
    //
    private void HandleStamina()
    {
        if (isSprinting && _characterController.isGrounded)
        {
            Current_Stamina -= RunCost*Time.deltaTime;
            StaminaBar.gameObject.SetActive(true);

            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
                regenCoroutine = null;
            }
        }

        else if (Current_Stamina < MaxStamina && regenCoroutine == null)
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
        StaminaFill.color = Current_Stamina <= 17f ?
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
        NumOfSound = NumOfSound==1 ? 2 : 1;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            switch (NumOfSound)
            {
                case 1:
                {
                    if (WalkSound1.isPlaying || WalkSound2.isPlaying || WalkSound3.isPlaying)
                    {
                        return;
                    }

                    WalkSound1.Play();
                    break;
                }

                case 2:
                {
                    if (WalkSound1.isPlaying || WalkSound2.isPlaying || WalkSound3.isPlaying)
                    {
                        return;
                    }

                    WalkSound2.Play();
                    break;
                }

            }
        }
    }
}
