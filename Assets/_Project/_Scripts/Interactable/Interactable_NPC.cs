using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactable_NPC : Interactable
{
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private TextMeshProUGUI textDialogo;
    [SerializeField] private DialogosScriptableObject dialogosScriptableObject;
    [SerializeField] private float tiempoEntreCaracteres;
    [SerializeField] private float tiempoEntreDialogos;

    private int indexDialogo;

    public override void OnFocus()
    {
        Debug.Log("Looking at " + gameObject.name);
    }

    public override void OnInteract()
    {
        Debug.Log("Interacting with " + gameObject.name);
        panelDialogo.SetActive(true);
        IniciarDialogo();

        Player_InputHandle.instance.enabled = false;
    }

    public override void OnLoseFocus()
    {
        Debug.Log("Stop Looking at " + gameObject.name);
    }

    private void IniciarDialogo()
    {
        indexDialogo = 0;
        MostrarSiguienteDialogo();
    }

    private void MostrarSiguienteDialogo()
    {
        if (indexDialogo < dialogosScriptableObject.dialogo.Length)
        {
            StartCoroutine(EscribirLinea(dialogosScriptableObject.dialogo[indexDialogo]));
        }
        else
        {
            Debug.Log("Diálogo terminado");
            panelDialogo.SetActive(false);

            Player_InputHandle.instance.enabled = true;
        }
    }

    private IEnumerator EscribirLinea(Dialogo dialogo)
    {
        textDialogo.text = $"{dialogo.Persona}: ";
        foreach (char c in dialogo.Contenido)
        {
            textDialogo.text += c;
            yield return new WaitForSeconds(tiempoEntreCaracteres);
        }
        yield return new WaitForSeconds(tiempoEntreDialogos); // Tiempo entre dialogos

        indexDialogo++; // Mover al siguiente diálogo
        MostrarSiguienteDialogo(); // Mostrar el siguiente diálogo
    }
}
