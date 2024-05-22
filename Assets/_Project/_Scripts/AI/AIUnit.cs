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
    public GameObject jugador;
    public TextMeshPro textoEstados;

    //Parametros

    //Estadisticas
    public string estadoInicial; 
    public string element;
    [SerializeField] private float life;
    [SerializeField] private float rangoVision;
    [SerializeField] private float rangoAtaque;
    public bool goPatrulla;
    public bool inRangoVision, inRangoAtaque, inGolpeado;

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
        inRangoAtaque = Vector3.Distance(transform.position, jugador.transform.position) <= rangoAtaque;
        inRangoVision = Vector3.Distance(transform.position, jugador.transform.position) <= rangoVision;

        if (!inRangoVision && !inRangoAtaque && estadoInicial == "Inactivo")
        {
            Inactivo();
        }
        else if (!inRangoVision && !inRangoAtaque && estadoInicial == "Patrulla")
        {
            Patrulla();
        }
        else if (jugador != null && !inRangoAtaque && inRangoVision)
        {
            Perseguir(jugador);
        }
        else if (jugador != null && inRangoAtaque)
        {
            Ataque(jugador);
        }
    }

    private void LookAt(GameObject objetivo)
    {

    }
    public void TakeDamage(int daño)
    {
        Golpeado();
    }
    private void Golpeado()
    {
        animator.SetBool("isGolpeado", true);
        textoEstados.text = "Golpeado";
    }
    private void Inactivo()
    {
        agent.destination = this.transform.position;

        animator.SetFloat("speed", agent.velocity.magnitude);
        textoEstados.text = "Inactivo";
    }
    private void Patrulla()
    {
        //agent.destination = ////TODO: PATRULLA

        animator.SetFloat("speed", agent.velocity.magnitude);
        textoEstados.text = "Patrulla";
    }
    private void Perseguir(GameObject objetivo)
    {
        agent.destination = objetivo.transform.position;
        animator.SetFloat("speed", agent.velocity.magnitude);

        textoEstados.text = "Perseguir";
    }
    private void Ataque(GameObject objetivo)
    {
        agent.destination = this.transform.position;

        animator.SetBool("isAtaque", true);
        textoEstados.text = "Ataque";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoVision);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}
