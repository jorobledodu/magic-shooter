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
    private Collider[] brazosColliders;
    public AudioSource AIAudioSource;
    private Magias magias;
    public Transform magiasHandle;

    //Parametros

    //Estadisticas
    private string estadoInicial; 
    public string element;
    [SerializeField] private float vidaMaxima;
    public float vidaActual;
    [SerializeField] private float rangoVision;
    [SerializeField] private float rangoAtaque;
    public bool canMover = true;
    public bool inRangoVision, inRangoAtaque, inGolpeado, inAtaque;
    public LayerMask playerLayer;
    public AudioClip audioVivo, audioGolpeado, audioAtaque, audioMuerto;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ragdoll = GetComponent<Ragdoll>();
        magias = GetComponent<Magias>();
    }

    private void Start()
    {
        ElegirEstadoInicial();

        vidaActual = vidaMaxima;

        AIAudioSource.Play();
    }
    private void Update()
    {
        if (magias.magiaActual == MagiasDisponibles.Fuego)
        {
            magiasHandle.GetChild(0).gameObject.SetActive(true);
            magiasHandle.GetChild(1).gameObject.SetActive(false);
            magiasHandle.GetChild(2).gameObject.SetActive(false);
        }
        else if (magias.magiaActual == MagiasDisponibles.Rayo)
        {
            magiasHandle.GetChild(0).gameObject.SetActive(false);
            magiasHandle.GetChild(1).gameObject.SetActive(true);
            magiasHandle.GetChild(2).gameObject.SetActive(false);
        }
        else if (magias.magiaActual == MagiasDisponibles.Agua)
        {
            magiasHandle.GetChild(0).gameObject.SetActive(false);
            magiasHandle.GetChild(1).gameObject.SetActive(false);
            magiasHandle.GetChild(2).gameObject.SetActive(true);
        }
        else if (magias.magiaActual == MagiasDisponibles.Null)
        {
            magiasHandle.GetChild(0).gameObject.SetActive(false);
            magiasHandle.GetChild(1).gameObject.SetActive(false);
            magiasHandle.GetChild(2).gameObject.SetActive(false);
        }

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

        ////TODOO: SONIDO DE IDLE
        AIAudioSource.clip = audioVivo;
        //AIAudioSource.PlayDelayed(1f);
    }
    private void Patrulla()
    {
        //agent.destination = ////TODOO: PATRULLA

        animator.SetFloat("speed", agent.velocity.magnitude);
        textoEstados.text = "Patrulla";

        ////TODOO: SONIDO DE CAMINAR
        //AIAudioSource.clip = audioVivo;
        //AIAudioSource.PlayDelayed(1f);
        //AIAudioSource.PlayOneShot(audioVivo);
    }
    private void Perseguir(GameObject objetivo)
    {
        agent.destination = objetivo.transform.position;
        animator.SetFloat("speed", agent.velocity.magnitude);
        textoEstados.text = "Perseguir";

        ////TODOO: Sonido de correr
        //AIAudioSource.clip = audioVivo;
        //AIAudioSource.PlayDelayed(1f);
        //AIAudioSource.PlayOneShot(audioVivo);
    }
    private void Ataque(GameObject objetivo)
    {
        inAtaque = true;
        textoEstados.text = "Ataque";
        agent.destination = this.transform.position;
        animator.SetBool("isAtaque", true);

        ////TODOO: Sonido de zombie atacando
        AIAudioSource.PlayOneShot(audioAtaque);
    }
    public void AtaqueRaycast()
    {
        //Raycast de ataque y bajar la vida al "player", esto solamente se llama desde la animacion de ataque
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, rangoAtaque, playerLayer))
        {
            rayHit.collider.GetComponent<FP_Controller>().TakeDamage(30);
        }
    }
    private void Golpeado()
    {
        canMover = false;
        agent.destination = this.transform.position;

        //rangoVision = 100;

        animator.SetBool("isGolpeado", true);
        textoEstados.text = "Golpeado";

        ////TODOO: Sonido al ser golpeado
        AIAudioSource.PlayOneShot(audioGolpeado);
    }
    private void Morir()
    {
        ////TODOO: Contador para destuir el game object
        ////TODOO: Sonido de muerte
        AIAudioSource.PlayOneShot(audioMuerto);

        canMover = false;
        agent.destination = this.transform.position;
        ragdoll.ActivarRagdoll();
        textoEstados.text = "Muerto";
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
    public void RecibirDaño(float daño)
    {
        vidaActual -= daño;

        if (vidaActual <= 0.0f)
        {
            Morir();
        }
        else
        {
            Golpeado();
        }
    }
    public void LookAt(GameObject objetivo)
    {
        //Solo hacer todo esto si el feedback del enemigo no es bueno
        ////TODOO: QUATERNION ROTATE
        ///TODOO: Llamar esta funcion en el inicio de la animacion de ataque (para ver como queda)
        ///TODOO:  Llamar esta funcion desde 
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
