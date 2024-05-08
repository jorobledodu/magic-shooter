using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackState : State
{
    //public static AttackState instance;

    private ChaseState chaseState;

    public TextMeshPro stateText;
    private Animator animator;

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

        chaseState = GetComponent<ChaseState>();

        animator = GetComponent<Animator>();
    }

    public override State RunCurrentState()
    {
        if (!chaseState.isInAttackRange) //Chase
        {
            animator.SetBool("isRunning", true);
            return chaseState;
        }
        else //Attack
        {
            Attack();
            return this;
        }
    }

    private void Attack()
    {
        animator.SetBool("isAttaking", true);
        stateText.text = "Attack";
    }
}
