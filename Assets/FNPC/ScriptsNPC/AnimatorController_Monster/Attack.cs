using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEditor;

public class AttackBehaviour : StateMachineBehaviour
{
    private Transform m_player;
    private Text healthCount;
    private Slider HealthBar;
    //private Slider StaminaBar;
    private float AttackRadius = 6f; 
    private int damageAmount = 15; 
    private float timer = 0f;

    Transform EnemyEye;
    [Range(0, 360)] float ViewAngle = 165f;
    
    float ViewDistance = 75f;
    bool IsAttackUge;

    [SerializeField] GameObject menuOfDeath;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        HealthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        healthCount = GameObject.Find("HealthCount").GetComponent<Text>();
        EnemyEye = GameObject.FindGameObjectWithTag("Eye").transform;
        IsAttackUge = false;
        menuOfDeath = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(5).gameObject;


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        animator.transform.LookAt(m_player);
        float distance = Vector3.Distance(animator.transform.position, m_player.position);

        if (distance > AttackRadius || (!IsInView() && distance > AttackRadius) )
        {
            animator.SetBool("IsAttack", false);
        }

        else if (distance <= AttackRadius )
        {
            if (IsInView())
            {
                if (IsAttackUge == true)
                {
                    timer += Time.deltaTime;
            
                    if (timer >= 2f)
                    {
                        timer = 0f;
                        PerformAttack();
                        
                    }
                }

                else
                { 
                    timer += Time.deltaTime;
            
                    if (timer >= 1.2f)
                    {
                        timer = 0f;
                        PerformAttack();
                        IsAttackUge = true; 
                        
                    }
                }
            }

            

            else 
            {
                animator.SetBool("IsAttack", false);
            }
        }

        //PlayAttack_Sound();
                
        
        

        
    }

    private void PerformAttack()
    {

        if (int.Parse(healthCount.text) - damageAmount > 0)
        {
            timer = 0f;
            float new_health = int.Parse(healthCount.text) - damageAmount;
            HealthBar.value = new_health;
            healthCount.text = new_health.ToString();
            
        }
        else
        {

            GameOver();
        }
    }

    private void GameOver()
    {

        Debug.Log("Игра окончена!");
        //PerformAttack();
        // Перезагрузить текущую сцену
        Debug.Log($"{menuOfDeath.name}");
        menuOfDeath.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // SceneManager.LoadScene("GameOverScene"); //Перезагрузить нужную сцену
    }
//
//Находится ли в поле зрения
//
private bool IsInView() 
    {
        float currentAngle = Vector3.Angle(EnemyEye.forward, m_player.position - EnemyEye.position);
        RaycastHit hit;

        // Проверяем, виден ли игрок с использованием Physics.Raycast
        if (Physics.Raycast(EnemyEye.position, m_player.position - EnemyEye.position, out hit, ViewDistance))
        {
            if (currentAngle < ViewAngle / 2f && hit.transform == m_player.transform)
            {
                return true; // Игрок виден
            }
        }
        return false; // Игрок скрыт
    }
//
//Звук
//
    /*private void PlayAttack_Sound()
    {
        if (!intimidate_audioSource.isPlaying)
        {
            intimidate_audioSource.PlayOneShot(intimidateSound[Random.Range(0, intimidateSound.Length)]);
        }
    }*/

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;
        IsAttackUge = false;
    }

}