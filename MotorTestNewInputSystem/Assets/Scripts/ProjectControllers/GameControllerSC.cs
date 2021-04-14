using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameControllerSC : MonoBehaviour
{
    [SerializeField]
    GameObject LoadingCircle = null;
    [SerializeField]
    GameObject StartFreeRoamExperience = null;
    [SerializeField]
    GameObject StartInteractiveTutorial = null;
    [SerializeField]
    GameObject StartMotorShowCase = null;
    [SerializeField]
    GameObject BackToMainFromFreeRoam = null;

    //[SerializeField]
    //Toggle AngleCheck = null;
    [SerializeField]
    GameObject OrderCheck = null;
    [SerializeField]
    GameObject HintEnable = null;

    public GameObject[] Instructions;


    public bool SetAngleCheck;
    public bool SetOrderCheck;
    public bool EnableHint;

    private bool m_IsSnapping;
    public bool ObjectIsSnapping { get => m_IsSnapping; set => m_IsSnapping = value; }

    private static GameControllerSC _instance;
    public static GameControllerSC Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            Instructions = GameObject.FindGameObjectsWithTag("Instructions");
            if (Instructions != null)
            {
                foreach(GameObject go in Instructions)
                {
                    if (!go.activeInHierarchy)
                    {
                        go.SetActive(true);
                    }
                }
            }

            LoadingCircle = GameObject.Find("LoadingCanva");
            if (LoadingCircle != null)
            {
                LoadingCircle.SetActive(false);
            }
            StartFreeRoamExperience = GameObject.Find("FreeRoamButton");
            StartInteractiveTutorial = GameObject.Find("InteractiveTuTButton");
            StartMotorShowCase = GameObject.Find("ShowCaseButton");
            OrderCheck = GameObject.Find("OrderToggle");
            HintEnable = GameObject.Find("HintToggle");
            StartFreeRoamExperience.GetComponent<Button>().onClick.AddListener(FreeRoamExperiance);
            StartInteractiveTutorial.GetComponent<Button>().onClick.AddListener(InteractiveTutorial);
            StartMotorShowCase.GetComponent<Button>().onClick.AddListener(MotorShowCase);
            //AngleCheck.onValueChanged.AddListener(AngleCheckToggle);
            OrderCheck.GetComponent<Toggle>().onValueChanged.AddListener(OrderCheckToggle);
            HintEnable.GetComponent<Toggle>().onValueChanged.AddListener(HintToggle);
            SetOrderCheck = true;
        }
        if (scene.buildIndex == 2 || scene.buildIndex == 4 || scene.buildIndex == 3)
        {
            Instructions = GameObject.FindGameObjectsWithTag("Instructions");
            if (Instructions != null)
            {
                foreach (GameObject go in Instructions)
                {
                    if (go.activeInHierarchy)
                    {
                        go.SetActive(false);
                    }
                }
            }
            LoadingCircle = GameObject.Find("LoadingCanva");
            if(LoadingCircle != null)
            {
                LoadingCircle.SetActive(false);
            }
            BackToMainFromFreeRoam = GameObject.Find("Back_To_Menu");
            BackToMainFromFreeRoam.GetComponent<Button>().onClick.AddListener(BackToMain);
        }
    }
    public void FreeRoamExperiance()
    {
        LoadingCircle.SetActive(true);
        StartCoroutine(LoadSceneAsync(2));
    }
    public void InteractiveTutorial()
    {
        LoadingCircle.SetActive(true);
        StartCoroutine(LoadSceneAsync(3));
    }
    public void MotorShowCase()
    {
        LoadingCircle.SetActive(true);
        StartCoroutine(LoadSceneAsync(4));
    }
    public void BackToMain()
    {
        LoadingCircle.SetActive(true);
        StartCoroutine(LoadSceneAsync(1));
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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneNumber,LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
