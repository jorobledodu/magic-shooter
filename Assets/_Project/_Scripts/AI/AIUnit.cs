using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
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
    public AudioSource AIAudioSource;
    private Magias magias;
    public ParticleSystem mojadoParticleText;
    public ParticleSystem conCargaParticleText;
         
    //Parametros

    //Estadisticas
    private string estadoInicial; 
    public string element;
    [SerializeField] private float vidaMaxima;
    public float vidaActual;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Gradient healthBarGradient;
    [SerializeField] private float timeBeforeRegenHealth = 3.0f;
    [SerializeField] private float healthValueIncrement = 1.0f;
    [SerializeField] private float healthTimeIncrement = 0.1f;
    private Coroutine regeneratingHealth;
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
    }
    private void Update()
    {
        if (magias.magia == MagiasDisponibles.Agua)
        {
            mojadoParticleText.gameObject.SetActive(true);
            conCargaParticleText.gameObject.SetActive(false);
            StartCoroutine(MojadoTextCoroutine());
        }
        else if (magias.magia == MagiasDisponibles.Rayo)
        {
            mojadoParticleText.gameObject.SetActive(false);
            conCargaParticleText.gameObject.SetActive(true);
            StartCoroutine(ConCargaTextCoroutine());
        }
        else
        {
            mojadoParticleText.gameObject.SetActive(false);
            conCargaParticleText.gameObject.SetActive(false);
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
        //agent.destination = this.transform.position;
        agent.SetDestination(this.transform.position);

        animator.SetFloat("speed", agent.velocity.magnitude);
        textoEstados.text = "Inactivo";

        ////TODOO: SONIDO DE IDLE
    }
    private void Patrulla()
    {
        //agent.destination = ////TODOO: PATRULLA

        animator.SetFloat("speed", agent.velocity.magnitude);
        textoEstados.text = "Patrulla";

        ////TODOO: SONIDO DE CAMINAR
    }
    private void Perseguir(GameObject objetivo)
    {
        //agent.destination = objetivo.transform.position;
        agent.SetDestination(objetivo.transform.position);


        animator.SetFloat("speed", agent.velocity.magnitude);
        textoEstados.text = "Perseguir";

        ////TODOO: Sonido de correr
    }
    private void Ataque(GameObject objetivo)
    {
        inAtaque = true;
        textoEstados.text = "Ataque";
        
        //agent.destination = this.transform.position;
        agent.SetDestination(this.transform.position);

        animator.SetBool("isAtaque", true);

        ////TODOO: Sonido de zombie atacando
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
        //agent.destination = this.transform.position;
        agent.SetDestination(this.transform.position);

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

        //agent.destination = this.transform.position;
        agent.SetDestination(this.transform.position);

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

            if (regeneratingHealth != null)
            {
                StopCoroutine(regeneratingHealth);
            }
        }
        else if (vidaActual > 0.0f)
        {
            Golpeado();

            if (regeneratingHealth != null)
            {
                StopCoroutine(regeneratingHealth);
            }
        }

        regeneratingHealth = StartCoroutine(RegenerateHealth());
        UpdateHealthBar();
    }
    private void UpdateHealthBar()
    {
        float targetFillAmount = vidaActual / vidaMaxima;
        healthBarFill.fillAmount = targetFillAmount;
        healthBarFill.color = healthBarGradient.Evaluate(targetFillAmount);
    }

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenHealth);
        WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

        while (vidaActual < vidaMaxima)
        {
            vidaActual += healthValueIncrement;

            if (vidaActual >= vidaMaxima)
            {
                vidaActual = vidaMaxima;
            }

            yield return timeToWait;

            UpdateHealthBar();
        }

        regeneratingHealth = null;
    }

    private IEnumerator MojadoTextCoroutine()
    {
        mojadoParticleText.Play();
        yield return new WaitForSeconds(2f);
        mojadoParticleText.Play();
    }
    private IEnumerator ConCargaTextCoroutine()
    {
        conCargaParticleText.Play();
        yield return new WaitForSeconds(2f);
        conCargaParticleText.Play();
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoVision);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}
