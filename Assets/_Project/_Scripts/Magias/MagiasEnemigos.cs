using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class MagiasEnemigos : Magias
{
    public MagiasDisponibles magiaAnterior;
    public EstadosDisponibles estado;
    [SerializeField] private float rangoEfecto;
    [SerializeField] private LayerMask whatCanElectrocutar;
    public ParticleSystem electrocutadoParticle;

    private AIManager aiManager;
    private AIUnit aiUnit;

    private void Start()
    {
        aiUnit = GetComponent<AIUnit>();
        aiManager = FindAnyObjectByType<AIManager>();
    }
    public override void CambiarMagia(MagiasDisponibles magiaHit)
    {
        magiaAnterior = magia;
        magia = magiaHit;
    }

    public override void CambiarEstado()
    {
        if (magiaAnterior != MagiasDisponibles.Agua && magia == MagiasDisponibles.Fuego)
        {
            estado = EstadosDisponibles.EnLlamas;
        }
        else if (magiaAnterior == MagiasDisponibles.Agua && magia == MagiasDisponibles.Rayo)
        {
            estado = EstadosDisponibles.Electrocutado;
        }
        else if (magiaAnterior != MagiasDisponibles.Agua && magia == MagiasDisponibles.Agua)
        {
            estado = EstadosDisponibles.Mojado;
        }
        else
        {
            estado = EstadosDisponibles.Null;
        }
    }

    public override void ComprobarEstado()
    {
        if (estado == EstadosDisponibles.EnLlamas)
        {

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
                    enemigo.ComprobarEstado();
                }
            }
            yield return new WaitForSeconds(0.5f); // Ajusta la frecuencia del daño según sea necesario.
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoEfecto);
    }
}
