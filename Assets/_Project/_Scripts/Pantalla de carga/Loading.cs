using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    private void Start()
    {
        int sceneToLoad = SceneLoader.nextScene;

        StartCoroutine(MakeTheLoad(sceneToLoad));
    }

    IEnumerator MakeTheLoad(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        
        while (operation.isDone == false)
        {
            yield return operation;
        }
    }
}
