using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControllerSC : MonoBehaviour
{
    [SerializeField]
    Button StartFreeRoamExperience;
    [SerializeField]
    Button StartInteractiveTutorial;
    [SerializeField]
    Toggle AngleCheck;
    [SerializeField]
    Toggle OrderCheck;
    [SerializeField]
    Toggle HintEnable;

    private int nextScene;

    public bool SetAngleCheck;
    public bool SetOrderCheck;
    public bool EnableHint;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        StartFreeRoamExperience.onClick.AddListener(FreeRoamExperiance);
        StartInteractiveTutorial.onClick.AddListener(InteractiveTutorial);
        AngleCheck.onValueChanged.AddListener(AngleCheckToggle);
        OrderCheck.onValueChanged.AddListener(OrderCheckToggle);
        HintEnable.onValueChanged.AddListener(HintToggle);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("ieladee loading ainu");
        if(scene.buildIndex == 1)
        {
            switch (nextScene)
            {
                case 2:
                    StartCoroutine(LoadSceneAsync(2));
                    break;
                case 3:
                    StartCoroutine(LoadSceneAsync(3));
                    break;
            }
        }
    }
    public void FreeRoamExperiance()
    {
        SceneManager.LoadScene(1);
        nextScene = 2;
    }
    public void InteractiveTutorial()
    {
        SceneManager.LoadScene(1);
        nextScene = 3;
    }
    public void AngleCheckToggle(bool angle)
    {
        SetAngleCheck = angle;
    }
    void OrderCheckToggle(bool order)
    {
        SetOrderCheck = order;
    }
    void HintToggle(bool hint)
    {
        EnableHint = hint;
    }

    IEnumerator LoadSceneAsync(int SceneNumber)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneNumber);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
