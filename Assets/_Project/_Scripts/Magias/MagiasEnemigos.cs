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
    public ParticleSystem electrocutadoParticula;
    public GameObject vaporParticula;
    [SerializeField] private GameObject estadoUI;
    public TextMeshProUGUI textoEstado;
    public float tiempoAturdido;
    public float cleanEstadoTiempo;
    private Coroutine cleanEstadoCorrutina = null;

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

        estado = DeterminarNuevoEstado(magia, magiaAnterior);
        if (estado.ToString() == "Null")
        {
            textoEstado.text = " ";
        }
        else textoEstado.text = estado.ToString();

        ComprobarEstado();
    }

    private EstadosDisponibles DeterminarNuevoEstado(MagiasDisponibles magia, MagiasDisponibles magiaAnterior)
    {
        if ((magia == MagiasDisponibles.Rayo && magiaAnterior == MagiasDisponibles.Agua) ||
            (magia == MagiasDisponibles.Agua && magiaAnterior == MagiasDisponibles.Rayo))
        {
            return EstadosDisponibles.Electrocutado;
        }
        if ((magia == MagiasDisponibles.Fuego && magiaAnterior == MagiasDisponibles.Agua) ||
            (magia == MagiasDisponibles.Agua && magiaAnterior == MagiasDisponibles.Fuego))
        {
            return EstadosDisponibles.Evaporacion;
        }
        if (magia == MagiasDisponibles.Agua)
        {
            return EstadosDisponibles.Mojado;
        }
        if (magia == MagiasDisponibles.Fuego)
        {
            return EstadosDisponibles.EnLlamas;
        }
        if (magia == MagiasDisponibles.Rayo)
        {
            return EstadosDisponibles.ConCarga;
        }

        return EstadosDisponibles.Null;
    }


    public override void ComprobarEstado()
    {
        switch (estado)
        {
            case EstadosDisponibles.Evaporacion:
                StartCoroutine(Aturdido());
                CleanEstado();
                magia = MagiasDisponibles.Null;
                magiaAnterior = MagiasDisponibles.Null;
                break;
            case EstadosDisponibles.Electrocutado:
                AplicarEfectoElectrocutado();
                magia = MagiasDisponibles.Null;
                magiaAnterior = MagiasDisponibles.Null;
                break;
                // Agregar más casos si es necesario
        }
    }

    private void AplicarEfectoElectrocutado()
    {
        aiUnit.RecibirDaño(999f);
        electrocutadoParticula.gameObject.SetActive(true);
        electrocutadoParticula.Play();
        StartCoroutine(ElectrocutaUnidadesEnRango());
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

    private IEnumerator Aturdido()
    {
        float rangoVisionOriginal = aiUnit.rangoVision;

        vaporParticula.SetActive(true);
        aiUnit.rangoVision = 0;
        yield return new WaitForSeconds(tiempoAturdido);
        vaporParticula.SetActive(false);
        aiUnit.rangoVision = rangoVisionOriginal;
    }
    public override void CleanEstado()
    {
        if (cleanEstadoCorrutina != null)
        {
            StopCoroutine(cleanEstadoCorrutina);
        }
        cleanEstadoCorrutina = StartCoroutine(CleanEstadoCorrutina());
    }
    private IEnumerator CleanEstadoCorrutina()
    {
        yield return new WaitForSeconds(cleanEstadoTiempo);
        estado = EstadosDisponibles.Null;  // Asegúrate de tener acceso a `estado`
        cleanEstadoCorrutina = null;

        if (estado.ToString() == "Null")
        {
            textoEstado.text = " ";
        }
        else textoEstado.text = estado.ToString();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoEfecto);
    }
}
