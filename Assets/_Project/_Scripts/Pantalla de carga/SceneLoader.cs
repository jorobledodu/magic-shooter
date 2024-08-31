using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static int nextScene;

    public static void LoadScene(int sceneIndex)
    {
        nextScene = sceneIndex;

        SceneManager.LoadScene("LoadingScene");
    }
}
