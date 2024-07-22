using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool gameStarted = false;

    [Header("Contador")]
    [SerializeField] private TextMeshProUGUI textoContador;
    [SerializeField] private Color colorPorDefecto = Color.white;

    [Header("Contador regresivo")]
    public bool contadorRegresivo = false;
    [Tooltip("Tiempo en MINUTOS")]
    public float tiempo;
    [Tooltip("Tiempo en MINUTOS")]
    [SerializeField]private float tiempoInicial;
    [SerializeField] private Color colorAlarma = Color.red;
    [Range(0, 1)]
    [SerializeField] private float porcentajeAlarma = 0.4f; // 40% del tiempo máximo

    [Header("Entorno")]
    [Tooltip("Tiempo en MINUTOS")]
    [SerializeField] private float resetEntorno;
    [Tooltip("Tiempo en SEGUNDOS")]
    [SerializeField] private float tiempoRecompensa;
    [SerializeField] private GameObject enemigoPrefab;
    [SerializeField] private GameObject[] spawners;
    [SerializeField] private GameObject[] fuegos;
    [SerializeField] private GameObject[] linternas;
    public List<AIUnit> aiUnitsInScene;
    public static int enemigosDerrotados;

    [Header("Info")]
    public GameObject dialogoPanel;
    public TextMeshProUGUI dialogoText;
    [SerializeField] private DialogosScriptableObject infoInicioScriptableObject;
    [SerializeField] private DialogosScriptableObject infoSpawnScriptableObject;
    [SerializeField] private float tiempoEntreCaracteres;
    private int indexDialogo;

    private bool _noLoopResetEntorno = false;

    private void Awake()
    {
        #region Instance
        instance = this;
        #endregion

        //Convertir a segundos (X min * 60)
        tiempoInicial = tiempoInicial * 60;
        resetEntorno = resetEntorno * 60;

        tiempo = tiempoInicial;
    }
    private void Start()
    {
        if (contadorRegresivo == true)
        {
            FormatoTextoContador(tiempo);
        }

        SpawnEnemigos();
    }
    private void Update()
    {
        Timer();
        // Comprobar si el juego ha comenzado
        if (gameStarted && !_noLoopResetEntorno)
        {
            Debug.Log("Debug solo 1 vez");
            _noLoopResetEntorno = true; // Evitar que esto se ejecute múltiples veces
            InvokeRepeating("ResetAll", resetEntorno, resetEntorno);

            dialogoPanel.SetActive(true);
            IniciarDialogo(infoInicioScriptableObject);
        }
    }
    
    private void Timer()
    {
        if (gameStarted)
        {
            if (contadorRegresivo == true)
            {
                if (tiempo > 0)
                {
                    tiempo -= Time.deltaTime;

                    // Si el tiempo restante es inferior a un porcentaje del tiempo inicial, cambiar el color del texto
                    if (tiempo < porcentajeAlarma * tiempoInicial)
                    {
                        textoContador.color = colorAlarma;
                    }
                    else
                    {
                        textoContador.color = colorPorDefecto;
                    }
                }
                else if (tiempo <= 0)
                {
                    tiempo = 0;

                    UI_Controller.instance.finPartida();
                }

                FormatoTextoContador(tiempo);
            }
        }
    }
    private void FormatoTextoContador(float tiempo)
    {
        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        textoContador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }
    public void MasTiempo()
    {
        tiempo += tiempoRecompensa;
    }

    private void ResetAll()
    {
        if (gameStarted)
        {
            SpawnEnemigos();
            SpawnFuego();
            SpawnLinternas();

            dialogoPanel.SetActive(true);
            IniciarDialogo(infoSpawnScriptableObject);
        }
    }

    private void SpawnEnemigos()
    {
        // Destruir todas las unidades muertas y quitarlas de la lista
        for (int i = aiUnitsInScene.Count - 1; i >= 0; i--)
        {
            if (aiUnitsInScene[i].muerto)
            {
                Destroy(aiUnitsInScene[i].gameObject);
                aiUnitsInScene.RemoveAt(i);
            }
        }

        // Instanciar nuevos enemigos en cada spawner y añadirlos a la lista
        foreach (GameObject spawner in spawners)
        {
            GameObject enemigo = Instantiate(enemigoPrefab, spawner.transform.position, spawner.transform.rotation);
            AIUnit aiUnit = enemigo.GetComponent<AIUnit>();
            if (aiUnit != null)
            {
                aiUnitsInScene.Add(aiUnit);
            }
        }

        // Opcional: Imprimir el número de objetos encontrados
        Debug.Log("Número de objetos encontrados: " + aiUnitsInScene.Count);
    }

    private void SpawnFuego()
    {
        int numToActivate = Random.Range(0, fuegos.Length);

        int fuegosActivos = 0;

        foreach (GameObject fuego in fuegos)
        {
            OnOffParticle onOff = fuego.GetComponent<OnOffParticle>();
            if (onOff != null && !onOff.isOn && fuegosActivos < numToActivate)
            {
                onOff.StartParticleSystem();
                fuegosActivos++;
            }
        }
    }

    private void SpawnLinternas()
    {
        int numToDeactivate = Random.Range(0, linternas.Length);

        int linternasApagadas = 0;

        foreach (GameObject linterna in linternas)
        {
            OnOffParticle onOff = linterna.GetComponent<OnOffParticle>();
            if (onOff != null && onOff.isOn && linternasApagadas < numToDeactivate)
            {
                onOff.StopParticleSystem();
                linternasApagadas++;
            }
        }
    }

    private void IniciarDialogo(DialogosScriptableObject _dialogosScriptableObject)
    {
        indexDialogo = 0;
        MostrarSiguienteDialogo(_dialogosScriptableObject);
    }

    private void MostrarSiguienteDialogo(DialogosScriptableObject _dialogosScriptableObject)
    {
        if (indexDialogo < _dialogosScriptableObject.dialogo.Length)
        {
            StartCoroutine(EscribirLinea(_dialogosScriptableObject.dialogo[indexDialogo]));
        }
    }

    private IEnumerator EscribirLinea(Dialogo dialogo)
    {
        dialogoText.text = $"{dialogo.Persona}";
        foreach (char c in dialogo.Contenido)
        {
            dialogoText.text += c;
            yield return new WaitForSeconds(tiempoEntreCaracteres);
        }

        yield return new WaitForSeconds(0.5f);
        dialogoPanel.SetActive(false);
    }
}
