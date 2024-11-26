using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AttackBehaviour : StateMachineBehaviour
{
    Transform player;
    float AttackRadius = 6f;
    private Text healthCount;
    private int damageAmount = 5; 
    private float timer = 0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        healthCount = GameObject.FindGameObjectWithTag("healthBar").transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Text>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.LookAt(player);
        float distance = Vector3.Distance(animator.transform.position, player.position);

        if (distance > AttackRadius)
        {
            animator.SetBool("IsAttack", false);
        }

        else
        {
            
            timer += Time.deltaTime;
            
            if (timer >= 1.7f)
            {
                
                PerformAttack();
            }
        }
    }

    private void PerformAttack()
    {

        if (int.Parse(healthCount.text) - damageAmount > 0)
        {
            timer = 0f;
            float new_health = int.Parse(healthCount.text) - damageAmount;
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

        // Перезагрузить текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    
        // SceneManager.LoadScene("GameOverScene"); //Перезагрузить нужную сцену
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}