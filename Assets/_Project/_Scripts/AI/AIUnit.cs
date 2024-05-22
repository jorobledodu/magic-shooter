using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[DefaultExecutionOrder(1)]
public class AIUnit : MonoBehaviour
{
    //Referencias
    private NavMeshAgent agent;
    private Animator animator;
    public GameObject player;
    public TextMeshPro textoEstados;

    //Parametros

    //Estadisticas
    public string element;
    [SerializeField] private float life;
    [SerializeField] private float rangoVision;
    [SerializeField] private float rangoAtaque;
    private bool goPatrulla;
    private bool inRangoVision, inRangoAtaque;




    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        inRangoVision = Vector3.Distance(transform.position, player.transform.position) <= rangoVision;
        inRangoAtaque = Vector3.Distance(transform.position, player.transform.position) <= rangoAtaque;

        Invoke("Pensar", 5);
    }

    private void Pensar()
    {
        if (!inRangoVision && !inRangoAtaque)
        {
            Idle();
        }
        else if (goPatrulla)
        {
            Patrulla();
        }
        else if (player != null && inRangoVision)
        {
            Chase(player);
        }
        else if (player != null && inRangoAtaque)
        {
            Ataque(player);
        }
    }

    private void LookAt(GameObject target)
    {

    }
    private void Idle()
    {
        animator.SetBool("isIdle", true);
    }
    private void Patrulla()
    {
        animator.SetBool("isWalking", true);
    }
    private void Chase(GameObject target)
    {
        animator.SetBool("isWalking", true);
    }
    private void Ataque(GameObject target)
    {
        animator.SetBool("isAttaking", true);
    }
}
