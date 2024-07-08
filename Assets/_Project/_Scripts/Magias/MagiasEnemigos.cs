using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MagiasEnemigos : Magias
{
    public MagiasDisponibles magiaAnterior;
    public EstadosDisponibles estado;
    [SerializeField] private float rangoEfecto;
    [SerializeField] private LayerMask whatCanElectrocutar;
    public ParticleSystem electrocutadoParticle;
    [SerializeField] private GameObject estadoUI;
    public TextMeshProUGUI textoEstado;
    public float tiempoAturdido;

    private AIUnit aiUnit;

    private void Start()
    {
        aiUnit = GetComponent<AIUnit>();

        estadoUI.SetActive(false);
    }
    public override void CambiarMagia(MagiasDisponibles magiaHit)
    {
        magiaAnterior = magia;
        magia = magiaHit;
    }

    public override void CambiarEstado()
    {
        estadoUI.SetActive(true);
        if ((magia == MagiasDisponibles.Agua) && (magiaAnterior ==MagiasDisponibles.Null))
        {
            estado = EstadosDisponibles.Mojado;
            textoEstado.text = estado.ToString();
        }
        else if ((magia == MagiasDisponibles.Fuego) && (magiaAnterior != MagiasDisponibles.Agua))
        {
            estado = EstadosDisponibles.EnLlamas;
            textoEstado.text = estado.ToString();
        }
        else if ((magia == MagiasDisponibles.Rayo) && (magiaAnterior != MagiasDisponibles.Agua))
        {
            estado = EstadosDisponibles.ConCarga;
            textoEstado.text = estado.ToString();
        }
        else if ((magiaAnterior == MagiasDisponibles.Agua && magia == MagiasDisponibles.Rayo) || (magiaAnterior == MagiasDisponibles.Rayo && magia == MagiasDisponibles.Agua))
        {
            estado = EstadosDisponibles.Electrocutado;
            textoEstado.text = estado.ToString();
        }
        else if ((magiaAnterior == MagiasDisponibles.Fuego && magia == MagiasDisponibles.Agua) || (magiaAnterior == MagiasDisponibles.Agua && magia == MagiasDisponibles.Fuego))
        {
            estado = EstadosDisponibles.Evaporacion;
            textoEstado.text = estado.ToString();
        }
        else
        {
            estado = EstadosDisponibles.Null;
        }

        ComprobarEstado();
    }

    public override void ComprobarEstado()
    {
        if (estado == EstadosDisponibles.Evaporacion)
        {
            Aturdido();
        }
        else if (estado == EstadosDisponibles.Electrocutado)
        {
            aiUnit.RecibirDaño(999f);
            electrocutadoParticle.gameObject.SetActive(true);
            electrocutadoParticle.Play();
            StartCoroutine(ElectrocutaUnidadesEnRango());
        }
        else if (estado == EstadosDisponibles.Mojado)
        {

        }
    }
    private IEnumerator ElectrocutaUnidadesEnRango()
    {
        float startTime = Time.time;
        while (Time.time < startTime + 2f)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, rangoEfecto, whatCanElectrocutar);
            foreach (var collider in colliders)
            {
                MagiasEnemigos enemigo = collider.GetComponentInParent<MagiasEnemigos>();
                if (enemigo != null && enemigo.estado != EstadosDisponibles.Electrocutado && enemigo.estado == EstadosDisponibles.Mojado)
                {
                    enemigo.estado = EstadosDisponibles.Electrocutado;
                    //enemigo.ComprobarEstado();
                }
            }
            yield return new WaitForSeconds(0.5f); // Ajusta la frecuencia del daño según sea necesario.
        }
    }
    private IEnumerator Aturdido()
    {
        float _rangoVision = aiUnit.rangoVision;

        aiUnit.rangoVision = 0;
        yield return new WaitForSeconds(tiempoAturdido);
        aiUnit.rangoVision = _rangoVision;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoEfecto);
    }
}
