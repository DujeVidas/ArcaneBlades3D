using System.Collections;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    Transform player;
    Rigidbody rb;
    public float speed = 0.5f;
    public float attackRange = 2.3f;
    public float attackDelay = 1.0f; // Time in seconds to wait between attacks

    Boss boss;
    private int lastAttackIndex = -1; // Variable to store the last chosen attack
    private bool isAttacking = false; // To check if an attack is already in progress

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody>();
        boss = animator.GetComponent<Boss>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer();

        if (Vector3.Distance(player.position, rb.position) > attackRange)
        {
            Vector3 target = new Vector3(player.position.x, rb.position.y, player.position.z);
            Vector3 newPos = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
       

        if (Vector3.Distance(player.position, rb.position) <= attackRange && !isAttacking)
        {
            boss.StartCoroutine(PerformAttack(animator));
        }
    }

    // Coroutine to handle attack with delay
    private IEnumerator PerformAttack(Animator animator)
    {
        isAttacking = true;

        // Randomly choose between attack 1, attack 2, or attack 3
        lastAttackIndex = Random.Range(1, 4); // Generates a random integer between 1 and 3

        // Trigger the chosen attack
        switch (lastAttackIndex)
        {
            case 1:
                animator.SetTrigger("Attack");
                break;
            case 2:
                animator.SetTrigger("Attack2");
                break;
            case 3:
                animator.SetTrigger("Attack3");
                break;
        }

        // Wait for the duration of the attack delay
        yield return new WaitForSeconds(attackDelay);

        // Reset the trigger that was set during the state
        switch (lastAttackIndex)
        {
            case 1:
                animator.ResetTrigger("Attack");
                break;
            case 2:
                animator.ResetTrigger("Attack2");
                break;
            case 3:
                animator.ResetTrigger("Attack3");
                break;
        }

        // Reset the attack index and allow for the next attack
        lastAttackIndex = -1;
        isAttacking = false;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset the attack triggers if the state is exited before the attack is finished
        if (isAttacking)
        {
            switch (lastAttackIndex)
            {
                case 1:
                    animator.ResetTrigger("Attack");
                    break;
                case 2:
                    animator.ResetTrigger("Attack2");
                    break;
                case 3:
                    animator.ResetTrigger("Attack3");
                    break;
            }
        }

        // Reset the attack index
        lastAttackIndex = -1;
        isAttacking = false;
    }
}
