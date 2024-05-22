using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class IdleState : State
{
    //public static IdleState instance;

    private PatrollState patrollState;
    private ChaseState chaseState;
    private AttackState attackState;

    public float idleTimer;
    public bool canSeePlayer;

    public TextMeshPro stateText;
    private Animator animator;
    private NavMeshAgent agent;
    private AIUnit _AIUnit;

    private void Awake()
    {
        #region Instance
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
        #endregion

        patrollState = GetComponent<PatrollState>();
        chaseState = GetComponent<ChaseState>();
        attackState = GetComponent<AttackState>();

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        _AIUnit = GetComponent<AIUnit>();
    }

    public override State RunCurrentState()
    {
        if (idleTimer <= 0) //Patroll
        {
            animator.SetBool("isWalking", true);
            return patrollState;
        }
        else if (canSeePlayer) //Chase
        {
            animator.SetBool("isRunning", true);
            return chaseState;
        }
        else if (chaseState.isInAttackRange) //Attack
        {
            animator.SetBool("isAttaking", true);
            return attackState;
        }
        else //Idle
        {
            Idle();
            return this;
        }
    }

    private void Idle()
    {
        stateText.text = "Idle";

        agent.destination = transform.position;
    }
}
