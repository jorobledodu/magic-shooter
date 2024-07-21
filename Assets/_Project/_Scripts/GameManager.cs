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
    [SerializeField] private float tiempoTranscurrido;

    [Header("Contador regresivo")]
    public bool contadorRegresivo = false;
    [Tooltip("Convertir a segundos (X min * 60)")]
    [SerializeField] private float tiempoMaximo = 300f;
    private float tiempoInicial;
    [SerializeField] private Color colorAlarma = Color.red;
    [Range(0, 1)]
    [SerializeField] private float porcentajeAlarma = 0.4f; // 40% del tiempo máximo

    [Header("Enemigos")]
    public GameObject enemigoPrefab;
    public GameObject[] spawners;
    public GameObject[] fuegos;
    public GameObject[] linternas;
    public List<AIUnit> aiUnitsInScene;
    public static int enemigosDerrotados;

    private void Awake()
    {
        #region Instance
        instance = this;
        #endregion

        tiempoInicial = tiempoMaximo;
    }
    private void Start()
    {
        if (contadorRegresivo == true)
        {
            FormatoTextoContador(tiempoMaximo);
        }

        SpawnEnemigos();
    }
    private void Update()
    {
        Timer();
    }
    
    private void Timer()
    {
        if (gameStarted)
        {
            if (contadorRegresivo == true)
            {
                if (tiempoMaximo > 0)
                {
                    tiempoMaximo -= Time.deltaTime;

                    // Si el tiempo restante es inferior a un porcentaje del tiempo inicial, cambiar el color del texto
                    if (tiempoMaximo < porcentajeAlarma * tiempoInicial)
                    {
                        textoContador.color = colorAlarma;
                    }
                    else
                    {
                        textoContador.color = colorPorDefecto;
                    }
                }
                else if (tiempoMaximo <= 0)
                {
                    tiempoMaximo = 0;
                    // Condición: si los enemigos están muertos, ganas; si no, pierdes
                    //VerificarEventosDeVictoria();
                }

                FormatoTextoContador(tiempoMaximo);
            }
        }
    }
    private void FormatoTextoContador(float tiempo)
    {
        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        textoContador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    private void ResetAll()
    {

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

    }
    private void SpawnLinternas()
    {

    }

    private void CalcularPuntosDeVictoria()
    {

    }
}
