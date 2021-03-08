using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLoadingScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadSceneAsync(1));
    }
    IEnumerator LoadSceneAsync(int SceneNumber)
    {
        yield return new WaitForSeconds(2f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneNumber, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
