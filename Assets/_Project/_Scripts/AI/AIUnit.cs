using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
[DefaultExecutionOrder(1)]
public class AIUnit : MonoBehaviour
{
    //Referencias
    private NavMeshAgent agent;
    private Animator animator;
    public GameObject jugador;
    public TextMeshPro textoEstados;
    private Ragdoll ragdoll;
    public Collider[] brazosColliders;

    //Parametros

    //Estadisticas
    public string estadoInicial; 
    public string element;
    [SerializeField] private float vidaMaxima;
    public float vidaActual;
    [SerializeField] private float rangoVision;
    [SerializeField] private float rangoAtaque;
    public bool canMover = true;
    public bool inRangoVision, inRangoAtaque, inGolpeado, inAtaque;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ragdoll = GetComponent<Ragdoll>();
    }

    private void Start()
    {
        ElegirEstadoInicial();

        vidaActual = vidaMaxima;
    }
    private void Update()
    {
        inRangoAtaque = Vector3.Distance(transform.position, jugador.transform.position) <= rangoAtaque;
        inRangoVision = Vector3.Distance(transform.position, jugador.transform.position) <= rangoVision;

        if (!inRangoVision && !inRangoAtaque && estadoInicial == "Inactivo")
        {
            Inactivo();
        }
        else if (!inRangoVision && !inRangoAtaque && canMover && estadoInicial == "Patrulla")
        {
            Patrulla();
        }
        else if (jugador != null && !inRangoAtaque && !inAtaque && inRangoVision && canMover)
        {
            Perseguir(jugador);
        }
        else if (jugador != null && !inAtaque && inRangoAtaque)
        {
            Ataque(jugador);
        }
    }

    private void ElegirEstadoInicial()
    {
        int value = Random.Range(0, 2);
        switch (value)
        {
            case 0:
                estadoInicial = "Inactivo";
                break;
            case 1:
                estadoInicial = "Patrulla";
                break;
        }
    }
    private void Inactivo()
    {
        agent.destination = this.transform.position;

        animator.SetFloat("speed", agent.velocity.magnitude);
        textoEstados.text = "Inactivo";
    }
    private void Patrulla()
    {
        //agent.destination = ////TODOO: PATRULLA

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
        inAtaque = true;
        textoEstados.text = "Ataque";
        agent.destination = this.transform.position;
        animator.SetBool("isAtaque", true);

        ////TOODO: Raycast de ataque y bajar la vida al "player"
        //if (Physics.Raycast(transform.position, transform.transform.forward, out rayHit, rangoAtaque))
        //{

        //}
    }
    private void Golpeado()
    {
        canMover = false;
        agent.destination = this.transform.position;

        rangoVision = 100;

        animator.SetBool("isGolpeado", true);
        textoEstados.text = "Golpeado";
    }
    private void Morir()
    {
        canMover = false;

        ragdoll.ActivarRagdoll();
    }
    public void IsGolpeadoFalse()
    {
        //Funcion para llamar desde animacion de enemigo golpeado;
        animator.SetBool("isGolpeado", false);
    }
    public void CanMoverTrue()
    {
        canMover = true;
    }
    public void RecibirDaño(float cantidad)
    {
        vidaActual -= cantidad;

        Golpeado();

        if (vidaActual <= 0.0f)
        {
            Morir();
        }
    }
    public void LookAt(GameObject objetivo)
    {
        //Solo hacer todo esto si el feedback del enemigo no es bueno
        ////TODOO: QUATERNION ROTATE
        ///TODOO: Llamar esta funcion en el inicio de la animacion de ataque (para ver como queda)
        ///TODOO:  Llamar esta funcion desde 
    }
    public void Golpe()
    {
        //Esta funcion se tiene que llamar en el frame de la animacion que queremos que se castee el raycast (cuando hace el golpe)

        ////TODO: ATAQUE (RAYCAST HACIA DELANTE) 
    }
    public void IsAtaque()
    {
        //Al terrminar la animacion de ataque hay un evento que llama a esta funcion, esto permite que a partir de ahí se pueda mover 
        animator.SetBool("isAtaque", false);
        inAtaque = false;
    }
    public void DesactivarColliderBrazos()
    {
        foreach (var brazosCollider in brazosColliders)
        {
            brazosCollider.isTrigger = true;
        }
    }
    public void ActivarColliderBrazos()
    {
        foreach (var brazosCollider in brazosColliders)
        {
            brazosCollider.isTrigger = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoVision);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}
