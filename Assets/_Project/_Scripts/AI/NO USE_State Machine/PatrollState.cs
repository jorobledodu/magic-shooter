using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrollState : State
{
    //public static PatrollState instance;

    private IdleState idleState;
    private ChaseState chaseState;
    private AttackState attackState;

    public float patrollTimer;

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

        idleState = GetComponent<IdleState>();
        chaseState = GetComponent<ChaseState>();
        attackState = GetComponent<AttackState>();

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        _AIUnit = GetComponent<AIUnit>();
    }

    public override State RunCurrentState()
    {
        if (patrollTimer <= 0) //Idle
        {
            animator.SetBool("isWalking", false);
            return idleState;
        }
        else if (idleState.canSeePlayer) //Chase
        {
            animator.SetBool("isRunning", true);
            return chaseState;
        }
        else if (chaseState.isInAttackRange) //Attack
        {
            animator.SetBool("isAttaking", true);
            return attackState;
        }
        else //Patroll
        {
            Patroll();
            return this;
        }
    }

    private void Patroll()
    {
        stateText.text = "Patroll";
    }
}
