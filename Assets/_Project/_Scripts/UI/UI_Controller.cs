using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    public static UI_Controller instance;
    public InputSystemUIInputModule uiInput;
    public EventSystem eventSystem;

    [Header("General")]
    public GameObject playerUI;
    public GameObject pauseUI;

    [Header("Inicio")]
    public GameObject inicioMenu;
    public GameObject inicioMenuFirstOption;
    [Space(3)]

    [Header("Opciones")]
    public GameObject opcionesMenu;
    public GameObject opcionesMenuFirstOption;
    public Slider sensibilidadXSlider;
    public Slider sensibilidadYSlider;
    [SerializeField] private AudioMixer audioMixer;
    public Slider musicaSlider;
    public Slider vfxSlider;
    [Space(3)]

    [Header("GRAFICOS")]
    public GameObject graficosMenu;
    public GameObject graficosMenuFirstOption;
    public TMP_Dropdown modoPantallaDropdown;
    public TMP_Dropdown resolucionDropdown;
    public TextMeshProUGUI resolucionText;

    [Header("Usuario")]
    public GameObject usuarioMenu;
    public GameObject usuarioMenuFirstOption;
    public TMP_InputField usuarioInputField;
    public static string activeUser;

    [Header("Fin de Partida")]
    public bool finDePartidaBool;
    public GameObject finDePartidaUI;
    public GameObject finDePartidaFirsOption;
    public TextMeshProUGUI enemigosDerrotadosText;
    public GameObject leaderboardMenu;

    [Header("Dialogos")]
    [SerializeField] private GameObject panelDialogo;

    public bool pausa;

    public static int puntos;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        CrearListaResoluciones();
        CrearListaModosPantalla();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            sensibilidadXSlider.value = PlayerPrefs.GetFloat("SensibilidadX", 2.0f);
            sensibilidadYSlider.value = PlayerPrefs.GetFloat("SensibilidadY", 2.0f);
        }
        vfxSlider.value = PlayerPrefs.GetFloat("VolumenSFX", 1.0f);
        musicaSlider.value = PlayerPrefs.GetFloat("VolumenMusica", 1.0f);
    }
    private void Update()
    {
        //resolucionText.text = Screen.width + " x " + Screen.height;
    }
    private void OnEnable()
    {
        SubscribeInputs();
    }
    private void OnDisable()
    {
        UnsubscribeInputs();
    }

    private void SubscribeInputs()
    {
        uiInput.cancel.action.performed += ESC;
    }
    private void UnsubscribeInputs()
    {
        uiInput.cancel.action.performed -= ESC;
    }

    private void ESC(InputAction.CallbackContext context)
    {
        if (!finDePartidaBool)
        {
            Scene _currentScene = SceneManager.GetActiveScene();
            int _sceneIndex = _currentScene.buildIndex;

            if (_sceneIndex == 0)
            {
                Atras();
            }
            else if (_sceneIndex == 1)
            {
                Pausa();
            }
        }
    }
    public void Pausa()
    {
        pausa = !pausa;

        if (pausa)
        {
            StopAllCoroutines();

            Player_InputHandle.instance.enabled = false;

            pauseUI.SetActive(true);

            Time.timeScale = 0f;

            PrimeraOpcion();
        }
        else if (!pausa)
        {
            if (panelDialogo.activeInHierarchy)
            {
                Player_InputHandle.instance.enabled = false;
            }
            else
            {
                Player_InputHandle.instance.enabled = true;
            }

            pauseUI.SetActive(false);

            Time.timeScale = 1f;
        }
    }
    public void Atras()
    {
        if (inicioMenu.activeInHierarchy && !opcionesMenu.activeInHierarchy && !graficosMenu.activeInHierarchy)
        {
            Pausa();

            playerUI.SetActive(true);
            pauseUI.SetActive(false);
        }
        else if (!inicioMenu.activeInHierarchy && opcionesMenu.activeInHierarchy && !graficosMenu.activeInHierarchy && !usuarioMenu.activeInHierarchy)
        {
            inicioMenu.SetActive(true);
            opcionesMenu.SetActive(false);
            graficosMenu.SetActive(false);
            usuarioMenu.SetActive(false);

            PrimeraOpcion();
        }
        else if (!inicioMenu.activeInHierarchy && !opcionesMenu.activeInHierarchy && graficosMenu.activeInHierarchy && !usuarioMenu.activeInHierarchy)
        {
            inicioMenu.SetActive(false);
            opcionesMenu.SetActive(true);
            graficosMenu.SetActive(false);
            usuarioMenu.SetActive(false);

            PrimeraOpcion();
        }
        else if (!inicioMenu.activeInHierarchy && !opcionesMenu.activeInHierarchy && !graficosMenu.activeInHierarchy && usuarioMenu.activeInHierarchy)
        {
            usuarioInputField.text = " ";

            inicioMenu.SetActive(true);
            opcionesMenu.SetActive(false);
            graficosMenu.SetActive(false);
            usuarioMenu.SetActive(false);

            PrimeraOpcion();
        }
    }

    public void OnClickEnableDisable(GameObject obj)
    {
        inicioMenu.SetActive(false);
        opcionesMenu.SetActive(false);
        graficosMenu.SetActive(false);
        usuarioMenu.SetActive(false);

        obj.SetActive(true);

        PrimeraOpcion();
    }
    public void PrimeraOpcion()
    {
        if (inicioMenu.activeInHierarchy == true)
        {
            eventSystem.SetSelectedGameObject(inicioMenuFirstOption);
        }
        else if (opcionesMenu.activeInHierarchy == true)
        {
            eventSystem.SetSelectedGameObject(opcionesMenuFirstOption);
        }
        else if (graficosMenu.activeInHierarchy == true)
        {
            eventSystem.SetSelectedGameObject(graficosMenuFirstOption);
        }
        else if (usuarioMenu.activeInHierarchy == true)
        {
            eventSystem.SetSelectedGameObject(usuarioMenuFirstOption);
        }
    }
    public void StartGame(int indexEscena)
    {
        string usuarioNombre = usuarioInputField.text.Trim();

        if (string.IsNullOrEmpty(usuarioNombre))
        {
            StartCoroutine(ErrorInputField());
        }
        else
        {
            activeUser = usuarioNombre;

            SceneLoader.LoadScene(indexEscena);
        }
    }
    IEnumerator ErrorInputField()
    {
        eventSystem.gameObject.SetActive(false);

        usuarioInputField.image.color = Color.red;
        yield return new WaitForSeconds(2f);
        usuarioInputField.image.color = Color.white;

        eventSystem.gameObject.SetActive(true);
    }
    public void ChangeScene(int indexEscena)
    {
        Time.timeScale = 1;

        SceneLoader.LoadScene(indexEscena);
    }
    public void Salir()
    {
        activeUser = " ";

        Application.Quit();
    }

    public void finPartida()
    {
        if (!finDePartidaBool)
        {
            eventSystem.SetSelectedGameObject(finDePartidaFirsOption);
        }

        finDePartidaBool = true;

        finDePartidaUI.SetActive(true);
        leaderboardMenu.SetActive(true);

        Player_InputHandle.instance.enabled = false;

        enemigosDerrotadosText.text = "Enemigos derrotados: " + GameManager.enemigosDerrotados.ToString();

        Leaderboard_Controller.Instance.SetEntry(activeUser, GameManager.enemigosDerrotados);
        Leaderboard_Controller.Instance.LoadEntries();
    }

    public void OnChangeSensibilidadX()
    {
        PlayerPrefs.SetFloat("SensibilidadX", sensibilidadXSlider.value);
        FP_Controller.lookSpeedHorizontal = sensibilidadXSlider.value;
    }
    public void OnChangeSensibilidadY()
    {
        PlayerPrefs.SetFloat("SensibilidadY", sensibilidadYSlider.value);
        FP_Controller.lookSpeedVertical = sensibilidadYSlider.value;
    }

    public void OnChangeVolumenSFX(float volumen)
    {
        PlayerPrefs.SetFloat("VolumenSFX", vfxSlider.value);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volumen) * 20f);
    }
    public void OnChangeVolumenMusica(float volumen)
    {
        PlayerPrefs.SetFloat("VolumenMusica", musicaSlider.value);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volumen) * 20f);
    }

    #region Opciones graficas y de pantalla
    private List<Resolution> resolucionesComunes = new List<Resolution>
    {
        new Resolution { width = 3840, height = 2160 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1440, height = 900 },
        new Resolution { width = 1366, height = 768 },
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1024, height = 768 },
        // Añade más resoluciones comunes si es necesario
    };
    private void CrearListaResoluciones()
    {
        resolucionDropdown.ClearOptions();

        List<string> opciones = new List<string>();
        int resolucionActual = -1;

        // Obtener la resolución actual del juego
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;

        // Verificar si la resolución actual está en la lista de resoluciones comunes
        if (!resolucionesComunes.Any(res => res.width == currentWidth && res.height == currentHeight))
        {
            // Añadir la resolución actual a la lista
            resolucionesComunes.Add(new Resolution { width = currentWidth, height = currentHeight });
            // Ordenar la lista por ancho y luego por alto
            resolucionesComunes = resolucionesComunes.OrderByDescending(res => res.width).ThenBy(res => res.height).ToList();
        }

        // Crear las opciones del Dropdown
        for (int i = 0; i < resolucionesComunes.Count; i++)
        {
            string opcion = resolucionesComunes[i].width + " x " + resolucionesComunes[i].height;
            opciones.Add(opcion);

            if (resolucionesComunes[i].width == currentWidth && resolucionesComunes[i].height == currentHeight)
            {
                resolucionActual = i;
            }
        }

        // Añadir opciones al Dropdown
        resolucionDropdown.AddOptions(opciones);

        // Establecer el valor del Dropdown a la resolución actual del juego
        resolucionDropdown.value = resolucionActual;
        resolucionDropdown.RefreshShownValue();

        // Añadir listener para manejar cambios en la resolución
        resolucionDropdown.onValueChanged.AddListener(delegate { CambiarResolucion(resolucionDropdown.value); });
    }

    public void CambiarResolucion(int indiceResolucion)
    {
        // Obtener la resolución seleccionada del dropdown
        Resolution resolucionSeleccionada = resolucionesComunes[indiceResolucion];
        bool resolucionDisponible = Screen.resolutions.Any(res => res.width == resolucionSeleccionada.width && res.height == resolucionSeleccionada.height);

        // Si la resolución seleccionada no está disponible, selecciona la más grande disponible
        if (!resolucionDisponible)
        {
            resolucionSeleccionada = Screen.resolutions.OrderByDescending(res => res.width * res.height).First();
        }

        // Ajustar la resolución de la pantalla
        Screen.SetResolution(resolucionSeleccionada.width, resolucionSeleccionada.height, Screen.fullScreen);

        // Guardar la resolución seleccionada en PlayerPrefs
        PlayerPrefs.SetInt("indiceResolucion", indiceResolucion);
    }

    private void CrearListaModosPantalla()
    {
        modoPantallaDropdown.ClearOptions();

        List<string> opciones = new List<string>
        {
            "Pantalla Completa",
            "Ventana"
        };

        modoPantallaDropdown.AddOptions(opciones);

        // Establecer el valor del Dropdown al modo de pantalla actual
        modoPantallaDropdown.value = Screen.fullScreen ? 0 : 1;
        modoPantallaDropdown.RefreshShownValue();

        // Añadir listener para manejar cambios en el modo de pantalla
        modoPantallaDropdown.onValueChanged.AddListener(delegate { CambiarModoPantalla(modoPantallaDropdown.value); });
    }
    public void CambiarModoPantalla(int modoPantalla)
    {
        // Obtener la resolución actual del Dropdown
        int indiceResolucion = resolucionDropdown.value;
        Resolution resolucion = resolucionesComunes[indiceResolucion];

        // Cambiar el modo de pantalla
        bool esFullscreen = modoPantalla == 0;
        Screen.SetResolution(resolucion.width, resolucion.height, esFullscreen);

        // Ajustar el Dropdown al valor actual de la resolución
        resolucionDropdown.value = indiceResolucion;
        resolucionDropdown.RefreshShownValue();
    }
    #endregion
}
