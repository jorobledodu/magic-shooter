using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent))]
[DefaultExecutionOrder(1)]
public class AIUnit : MonoBehaviour
{
    //Referencias
    private NavMeshAgent agent;
    private Animator animator;
    public GameObject jugador;
    private Ragdoll ragdoll;
    //public AudioSource AIAudioSource;
    private Magias magias;
    public bool muerto;

    //Parametros

    //Estadisticas
    public GameObject canvas;
    private string estadoInicial; 
    public string element;
    [SerializeField] private GameObject estadoUI;
    [SerializeField] private GameObject vidaUI;
    [SerializeField] private float vidaMaxima;
    public float vidaActual;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Gradient healthBarGradient;
    [SerializeField] private float timeBeforeRegenHealth = 3.0f;
    [SerializeField] private float healthValueIncrement = 1.0f;
    [SerializeField] private float healthTimeIncrement = 0.1f;
    private Coroutine regeneratingHealth;
    public float rangoVision;
    [SerializeField] private float rangoAtaque;
    public bool canMover = true;
    public bool inRangoVision, inRangoAtaque, inGolpeado, inAtaque;
    public LayerMask playerLayer;
    [SerializeField] private LayerMask floorLayer;
    public AudioClip audioVivo, audioGolpeado, audioAtaque, audioMuerto;
    [SerializeField] private bool estaRotando;
    [SerializeField] private float distanciaDetectarEncima = 1.0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ragdoll = GetComponent<Ragdoll>();
        magias = GetComponent<Magias>();
    }

    private void Start()
    {
        jugador = FPS_Controller.instance.gameObject;

        ElegirEstadoInicial();

        vidaActual = vidaMaxima;

        vidaUI.SetActive(false);
    }
    private void Update()
    {
        if (muerto)
        {
            canvas.SetActive(false);
            return; // Si está muerto, no hacer nada
        }

        DetectarSuelo();

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
        else if (jugador != null && inRangoAtaque)
        {
            RotarHaciaObjetivo(jugador);
            
            //Solo ataca si esta alineado con el jugador
            if (!inAtaque && Quaternion.Angle(transform.rotation, Quaternion.LookRotation((jugador.transform.position - transform.position).normalized)) < 5f)
            {
                Ataque(jugador);
            }
        }

        EvitarJugadorEncima(); 
    }
    private void EvitarJugadorEncima()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, distanciaDetectarEncima, playerLayer))
        {
            // Ajustar la posición del enemigo para evitar que el jugador quede encima
            Vector3 direccionEmpuje = (transform.position - hit.transform.position).normalized;
            transform.position += direccionEmpuje * 0.5f; // Ajusta la distancia de empuje según sea necesario
        }
    }
    private void RotarHaciaObjetivo(GameObject objetivo)
    {
        Vector3 direccion = (objetivo.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direccion.x, 0, direccion.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * 360); // Ajusta la velocidad de rotación según sea necesario
    }
    private void DetectarSuelo()
    {
        if (magias.magia != MagiasDisponibles.Agua)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3, floorLayer, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.CompareTag("Floor/Wet"))
                {
                    magias.CambiarMagia(MagiasDisponibles.Agua);
                    magias.CambiarEstado();
                }
            }
        }
        else
        {
            return;
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

        ////TODOO: SONIDO DE IDLE
    }
    private void Patrulla()
    {
        //agent.destination = ////TODOO: PATRULLA

        animator.SetFloat("speed", agent.velocity.magnitude);

        ////TODOO: SONIDO DE CAMINAR
    }
    private void Perseguir(GameObject objetivo)
    {
        //agent.destination = objetivo.transform.position;
        agent.SetDestination(objetivo.transform.position);


        animator.SetFloat("speed", agent.velocity.magnitude);

        ////TODOO: Sonido de correr
    }
    private void Ataque(GameObject objetivo)
    {
        if (inAtaque) return; // Evita múltiples llamadas a Ataque

        inAtaque = true;
        agent.SetDestination(this.transform.position);
        animator.SetBool("isAtaque", true);

        ////TODO: Sonido de zombie atacando
    }
    private void Golpeado()
    {
        vidaUI.SetActive(true);

        canMover = false;
        //agent.destination = this.transform.position;
        agent.SetDestination(this.transform.position);

        //rangoVision = 100;

        animator.SetBool("isGolpeado", true);

        ////TODOO: Sonido al ser golpeado
    }
    private void Morir()
    {
        vidaUI.SetActive(false);
        estadoUI.SetActive(false);
        ////TODOO: Sonido de muerte

        canMover = false;

        //agent.destination = this.transform.position;
        agent.SetDestination(this.transform.position);

        ragdoll.ActivarRagdoll();

        muerto = true;

        GameManager.enemigosDerrotados++;
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

    public void AtaqueRaycast()
    {
        //Raycast de ataque y bajar la vida al "player", esto solamente se llama desde la animacion de ataque
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, rangoAtaque, playerLayer))
        {
            rayHit.collider.GetComponent<FP_Controller>().TakeDamage(30);
        }
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
    public void IsAtaque()
    {
        //Al terrminar la animacion de ataque hay un evento que llama a esta funcion, esto permite que a partir de ahí se pueda mover 
        animator.SetBool("isAtaque", false);
        inAtaque = false;
        canMover = true; // Permitir movimiento después del ataque
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoVision);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}
