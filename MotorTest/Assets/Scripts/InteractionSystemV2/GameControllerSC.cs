using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControllerSC : MonoBehaviour
{
    [SerializeField]
    Button StartExperience;
    [SerializeField]
    Toggle AngleCheck;
    [SerializeField]
    Toggle OrderCheck;
    [SerializeField]
    Toggle HintEnable;

    public bool SetAngleCheck;
    public bool SetOrderCheck;
    public bool EnableHint;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        StartExperience.onClick.AddListener(ChangeScene);
        AngleCheck.onValueChanged.AddListener(AngleCheckToggle);
        OrderCheck.onValueChanged.AddListener(OrderCheckToggle);
        HintEnable.onValueChanged.AddListener(HintToggle);

    }
    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
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
}
