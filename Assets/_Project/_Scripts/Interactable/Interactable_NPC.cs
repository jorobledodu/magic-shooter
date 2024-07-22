using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactable_NPC : Interactable
{
    //public static Interactable_NPC instance;

    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private TextMeshProUGUI textDialogo;
    [SerializeField] private DialogosScriptableObject dialogosScriptableObject;
    [SerializeField] private float tiempoEntreCaracteres;
    [SerializeField] private float tiempoEntreDialogos;
    [SerializeField] private Animator animator;

    private int indexDialogo;

    public override void OnFocus() { }

    public override void OnInteract()
    {
        panelDialogo.SetActive(true);
        animator.SetBool("hablando", true); 
        IniciarDialogo();

        Player_InputHandle.instance.enabled = false;
    }

    public override void OnLoseFocus() { }

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
        else //Dialogo terminado
        {
            panelDialogo.SetActive(false);
            animator.SetBool("hablando", false);
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
