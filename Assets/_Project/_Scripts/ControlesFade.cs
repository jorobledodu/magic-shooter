using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlesFade : MonoBehaviour
{
    public CanvasGroup _canvasGroup;

    void Start()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float fadeTime = 5;

        while (fadeTime > 0)
        {
            _canvasGroup.alpha -= Time.deltaTime;
        }

        yield return new WaitForSeconds(fadeTime);

        if (_canvasGroup.alpha >= 0)
        {
            _canvasGroup.gameObject.SetActive(false);
            _canvasGroup.alpha = 1;
        }
        
    }
}
