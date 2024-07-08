using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<AIUnit> aiUnitsInScene;
    [SerializeField] private TextMeshProUGUI textoContador;
    [SerializeField] private float tiempoMaximo = 300f; // 5 minutos
    private float tiempoInicial;
    public bool gameStarted = false;
    [SerializeField] private Color colorPorDefecto = Color.white;
    [SerializeField] private Color colorAlarma = Color.red;
    [Range(0, 1)]
    [SerializeField] private float porcentajeAlarma = 0.4f; // 40% del tiempo máximo

    private void Awake()
    {
        #region Instance
        instance = this;
        #endregion

        tiempoInicial = tiempoMaximo;
    }
    private void Start()
    {
        FormatoTextoContador(tiempoMaximo);

        // Inicializar la lista
        aiUnitsInScene = new List<AIUnit>();

        // Encontrar todos los objetos con el script MyScript
        AIUnit[] foundObjects = FindObjectsOfType<AIUnit>();

        // Añadirlos a la lista
        foreach (AIUnit obj in foundObjects)
        {
            aiUnitsInScene.Add(obj);
        }

        // Opcional: Imprimir el número de objetos encontrados
        Debug.Log("Número de objetos encontrados: " + aiUnitsInScene.Count);
    }

    private void Update()
    {
        Timer(); 
    }

    private void Timer()
    {
        if (gameStarted)
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
                VerificarCondicionVictoria();
            }

            FormatoTextoContador(tiempoMaximo);
        }
    }
    private void FormatoTextoContador(float tiempo)
    {
        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        textoContador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    private void VerificarCondicionVictoria()
    {
        // Aquí debes implementar la lógica para verificar si los enemigos están muertos
        // y definir si el jugador gana o pierde.
        // Ejemplo:
        // if (todosLosEnemigosEstanMuertos)
        // {
        //     MostrarMensajeVictoria();
        // }
        // else
        // {
        //     MostrarMensajeDerrota();
        // }
    }
}
