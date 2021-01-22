using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GradingController : MonoBehaviour
{
    [SerializeField]
    ControllerScript m_ControllerScript;
    [SerializeField]
    Canvas m_ControllerUI;
    [SerializeField]
    public Results m_CurrPickUpObjRes;
    public List<Results> m_Results;
    [SerializeField]
    bool MethodExecuted;


    //---Total Timer---
    float time;
    float sec;
    float min;

    Text m_TotalTimerText;
    private void Awake()
    {
        m_ControllerScript = FindObjectOfType<ControllerScript>();
        m_TotalTimerText = m_ControllerUI.GetComponentInChildren<Text>();
        StartCoroutine(TotalTimer());
        m_Results = new List<Results>();
    }
    private void Update()
    {
        if (m_ControllerScript != null)
        {
            if (m_ControllerScript.objectinhand != null)
            {
                if (m_CurrPickUpObjRes == null)
                {
                    m_CurrPickUpObjRes = m_Results.Find(x => x.partName == m_ControllerScript.objectinhand.name);
                }
                if (!MethodExecuted)
                {
                    AddResToList(m_ControllerScript.objectinhand);
                    TimerCountControl(m_ControllerScript.objectinhand);
                    if(m_CurrPickUpObjRes != null)
                    {
                        print(m_CurrPickUpObjRes.PartPickTime);
                    }
                }
            }
            else
            {
                if(m_CurrPickUpObjRes != null)
                {
                    m_CurrPickUpObjRes.isPaused = true;
                }
                m_CurrPickUpObjRes = null;
                MethodExecuted = false;
            }
        }

    }
    void AddResToList(GameObject go)
    {
        MethodExecuted = true;

        if (m_Results.Count == 0)
        {
            Results TempRes = new Results()
            {
                partName = go.name,
                PickUpCount = 0,
                isPaused = false,
            };
            //TempRes.StartTimer(this);
            m_Results.Add(TempRes);
        }
        else
        {
            bool Contains = m_Results.Any(x => x.partName == go.name);

            if (Contains)
            {
                m_CurrPickUpObjRes.isPaused = false;
                //m_CurrPickUpObjRes.PickUpCount++;
            }
            else
            {
                print(go.name);
                Results TempRes = new Results
                {
                    partName = go.name,
                    PickUpCount = 0,
                    isPaused = false
                };
                //TempRes.StartTimer(this);
                m_Results.Add(TempRes);
                return;
            }
        }
    }
    void TimerCountControl(GameObject go)
    {
        if(m_Results.Count >= 1)
        {
            bool Contains = m_Results.Any(x => x.partName == go.name);
            Results TempRes = m_Results.Find(x => x.partName == go.name);

            if (Contains)
            {
                TempRes.PickUpCount++;
                TempRes.StartTimer(this);
                TempRes.isPaused = false;
            }
            else
            {
                TempRes.StopTimer(this);
                TempRes.isPaused = true;
            }
            TempRes = null;
        }
    }
    public IEnumerator TotalTimer()
    {

        while (true)
        {
            time += Time.deltaTime;
            sec = (int)(time % 60);
            min = (int)(time / 60);

            m_TotalTimerText.text = "Time elapsed - " + string.Format("{0:00}:{1:00}", min, sec);

            yield return m_TotalTimerText;
        }
    }
}
