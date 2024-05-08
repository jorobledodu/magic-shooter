using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChaseState : State
{
    //public static ChaseState instance;

    private AttackState attackState;

    public bool isInAttackRange;

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

        attackState = GetComponent<AttackState>();

        animator = GetComponent<Animator>();
    }

    public override State RunCurrentState()
    {
        if (isInAttackRange) //Attack
        {
            
            return attackState;
        }
        else //Chase
        {
            Chase();
            return this;
        }
    }

    private void Chase()
    {
        stateText.text = "Chase";
    }
}
