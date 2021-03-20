using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ModelOutline;
using Outline = ModelOutline.Outline;

public class GradingController : MonoBehaviour
{
    [SerializeField]
    ControllerScript m_ControllerScript = null;

    ControllerScript[] BothControllers = null;

    [SerializeField]
    Canvas m_ResultUI = null;

    [SerializeField]
    GameObject m_TotaltimerResultUI = null;

    [SerializeField]
    GameObject m_TotalTimerWhiteBoardUI = null;
    Text m_WhiteBoardTimer = null;
    [SerializeField]
    GameObject m_WhiteboardPartCounterUI = null;
    Text m_WhiteBoardPartCounter;

    CorrectOrderTests m_CorrOrder;

    [SerializeField]
    GameObject m_CorrectPlacedUI = null;

    [SerializeField]
    GameObject m_IncorrectPlacedUI = null;

    [SerializeField]
    GameObject m_ListContent = null;

    Button m_Refresh;

    [SerializeField]
    GameObject m_ResultRow = null;

    Object[] m_Thumbnails;

    public List<PlacedOrder> PlacedOrder;

    public Results m_CurrPickUpObjRes;

    public List<Results> m_Results;

    [SerializeField]
    bool MethodExecuted;

    public int PlacedOrderID;

    public int m_CorrectPlacedCount;
    public int m_IncorrectPlacedCount;
    int m_PlacedPartsCount;

    private static GradingController _instance;
    public static GradingController Instance { get { return _instance; } }

    SetEnable m_SetEnable;

    //---Total Timer---
    float time;
    float sec;
    float min;

    [SerializeField]
    string m_TotalTimerText;

    private void Awake()
    {
        m_SetEnable = FindObjectOfType<SetEnable>();
        m_ControllerScript = FindObjectOfType<ControllerScript>();
        BothControllers = FindObjectsOfType<ControllerScript>();
        m_CorrOrder = FindObjectOfType<CorrectOrderTests>();
        m_Refresh = m_ResultUI.GetComponentInChildren<Button>();
        m_WhiteBoardTimer = m_TotalTimerWhiteBoardUI.GetComponent<Text>();
        m_WhiteBoardPartCounter = m_WhiteboardPartCounterUI.GetComponent<Text>();
        m_Refresh.onClick.AddListener(DisplayList);
        StartCoroutine(TotalTimer());
        
        m_Results = new List<Results>();

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        m_ResultUI.enabled = false;
    }
    private void Update()
    {
        if (BothControllers.FirstOrDefault(x => x.TriggerPush || x.GrabPush))
        {
            m_ControllerScript = BothControllers.FirstOrDefault(x => x.TriggerPush || x.GrabPush);
        }
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
                }
            }
            else
            {
                if (m_CurrPickUpObjRes != null)
                {
                    m_CurrPickUpObjRes.isPaused = true;
                }
                m_CurrPickUpObjRes = null;
                MethodExecuted = false;
            }
        }
        if(m_SetEnable != null)
        {
            if (m_SetEnable.M_ModelComplete)
            {
                if (!m_ResultUI.enabled)
                {
                    m_ResultUI.enabled = true;
                    DisplayList();
                    StopAllCoroutines();
                    m_CorrectPlacedUI.GetComponent<Text>().text += m_CorrectPlacedCount.ToString();
                    m_IncorrectPlacedUI.GetComponent<Text>().text += m_IncorrectPlacedCount.ToString();
                    m_TotaltimerResultUI.GetComponent<Text>().text += m_TotalTimerText;
                }
            }
        }
        m_WhiteBoardTimer.text = m_TotalTimerText;
    }
    public void AddResToList(GameObject go)
    {
        if (MethodExecuted != true)
        {
            MethodExecuted = true;
        }
        if (m_Results.Count == 0)
        {
            if (go.GetComponent<Placeable>() != null)
            {
                Results TempRes = new Results
                {
                    partName = go.name,
                    PickUpCount = 0,
                    isPaused = false,
                    CorrectPlacementOrder = go.GetComponent<Placeable>().m_ID,
                    ActualPlacementID = 0
                };
                m_Results.Add(TempRes);
            }
        }
        else
        {
            bool Contains = m_Results.Any(x => x.partName == go.name);

            if (Contains)
            {
                m_CurrPickUpObjRes.isPaused = false;
            }
            else
            {
                if(go.GetComponent<Placeable>()!= null)
                {
                    Results TempRes = new Results
                    {
                        partName = go.name,
                        PickUpCount = 0,
                        isPaused = false,
                        CorrectPlacementOrder = go.GetComponent<Placeable>().m_ID,
                        ActualPlacementID = 0
                    };
                    m_Results.Add(TempRes);
                    return;
                }
            }
        }
    }
    void TimerCountControl(GameObject go)
    {
        if (MethodExecuted != true)
        {
            MethodExecuted = true;
        }
        if (m_Results.Count >= 1)
        {
            bool Contains = m_Results.Any(x => x.partName == go.name);
            Results TempRes = m_Results.Find(x => x.partName == go.name);

            if (Contains)
            {
                TempRes.PickUpCount++;
                TempRes.StartTimer(this);
                TempRes.isPaused = false;
            }
            else if(Contains)
            {
                TempRes.StopTimer(this);
                TempRes.isPaused = true;
            }
            TempRes = null;
        }
    }
    public void DisplayList()
    {
        List<Results> SortedList = new List<Results>();
        if (FindObjectsOfType<ListEntryValues>() != null)
        {
            ListEntryValues[] TempListItems = FindObjectsOfType<ListEntryValues>();
            for (int i = 0; i < TempListItems.Length; i++)
            {
                Destroy(TempListItems[i].gameObject);
            }
        }
        m_Thumbnails = Resources.LoadAll("Thumbnails", typeof(Sprite));

        PlacedOrderIdUpdate();

        if (m_Results != null)
        {
            SortedList = m_Results.OrderBy(x => x.CorrectPlacementOrder).ToList();
            foreach (Results res in SortedList)
            {
                if (m_Thumbnails != null)
                {
                    foreach (Sprite img in m_Thumbnails)
                    {
                        if (img.name == res.partName + "_Thumbnail")
                        {
                            m_ResultRow.GetComponent<ListEntryValues>().UpdateTextsAndImage(img, res.partName.Replace('_',' '), res.PickUpCount.ToString(), res.PartPickTime.ToString(), res.CorrectPlacementOrder.ToString(), res.ActualPlacementID.ToString());
                        }
                    }
                }
                else
                {
                    m_ResultRow.GetComponent<ListEntryValues>().UpdateTexts(res.partName, res.PickUpCount.ToString(), res.PartPickTime.ToString(), res.CorrectPlacementOrder.ToString(), res.ActualPlacementID.ToString());
                }
                Instantiate(m_ResultRow, m_ListContent.transform);
            }
            SortedList.Clear();
        }
    }
    public void PlacedOrderIdUpdate()
    {
        for (int i = 0; i < m_Results.Count; i++)
        {
            for (int j = 0; j < PlacedOrder.Count; j++)
            {
                if (m_Results[i].partName == PlacedOrder[j].Partname)
                {
                    m_Results[i].ActualPlacementID = PlacedOrder[j].PlacedId;
                }
            }
        }
    }

    public void ObjectPlaced(GameObject Go, int ObjID)
    {
        if (Instance.PlacedOrder.Count == 0)
        {
            Instance.PlacedOrder.Add(new PlacedOrder { Partname = Go.name, PlacedId = 0, ObjID = ObjID });
            Instance.PlacedOrderID = 1;
        }
        else
        {
            for (int i = 0; i < Instance.PlacedOrder.Count; i++)
            {
                if (!Instance.PlacedOrder.Any(x => x.Partname == Go.name))
                {
                    if (Instance.PlacedOrder.Any(x => x.ObjID == ObjID))
                    {
                        Debug.Log("same id ");
                        int TempInt = Instance.PlacedOrder.Find(x => x.ObjID == ObjID).PlacedId;
                        Instance.PlacedOrder.Add(new PlacedOrder { Partname = Go.name, PlacedId = TempInt, ObjID = ObjID });
                        m_CorrectPlacedCount++;
                        return;
                    }
                    else
                    {
                        Instance.PlacedOrder.Add(new PlacedOrder { Partname = Go.name, PlacedId = Instance.PlacedOrderID, ObjID = ObjID });
                        Instance.PlacedOrderID++;
                    }
                }
            }
        }
    }
    public void PlacedCounterAdd(int ObjId)
    {
        if(PlacedOrderID == ObjId)
        {
            m_CorrectPlacedCount++;
        }
        else
        {
            m_IncorrectPlacedCount++;
        }
    }
    public void PlacedCounterRemove(int ObjId)
    {
        if(PlacedOrderID == ObjId)
        {
            m_CorrectPlacedCount--;
        }
        else
        {
            m_IncorrectPlacedCount--;
        }
    }
    public void ObjectRemoved(GameObject Go)
    {
        if(m_Results != null)
        {
            Instance.m_Results.Find(x => x.partName == Go.name).ActualPlacementID = 0;
        }
        for (int i = 0; i < Instance.PlacedOrder.Count; i++)
        {
            if (Instance.PlacedOrder[i].Partname == Go.name)
            { 
                Instance.PlacedOrder.Remove(Instance.PlacedOrder[i]);
                if (Instance.PlacedOrderID > 0)
                {
                    Instance.PlacedOrderID--;
                }
            }
        }
    }
    public void ShowHintOutline(GameObject GO, int ObjID)
    {
        StartCoroutine(HintOutlineCorutine(GO, ObjID));
    }
    IEnumerator HintOutlineCorutine(GameObject GO,int ObjID)
    {

        int PlacedID = Instance.PlacedOrder.Find(x => x.Partname == GO.name).PlacedId;
        if (GO.GetComponent<Outline>() != null)
        {

                if (GO.GetComponent<Outline>().enabled != true)
                {
                    if (PlacedID == ObjID)
                    {
                        GO.GetComponent<Outline>().OutlineWidth = 4f;
                        GO.GetComponent<Outline>().OutlineColor = Color.green;
                        GO.GetComponent<Outline>().enabled = true;
                    }
                    else
                    {
                        GO.GetComponent<Outline>().OutlineWidth = 4f;
                        GO.GetComponent<Outline>().OutlineColor = Color.red;
                        GO.GetComponent<Outline>().enabled = true;
                    }
                }
            yield return new WaitForSeconds(1f);
                GO.GetComponent<Outline>().enabled = false;
                GO.GetComponent<Outline>().OutlineWidth = 2f;
                GO.GetComponent<Outline>().OutlineColor = Color.white;
            
        }
        yield return null;
    }
    void PlacedPartCounterWhiteBoard(bool Placed)
    {

        if (Placed)
        {
            m_PlacedPartsCount++;
        }
        else
        {
            m_PlacedPartsCount--;
        }
    }
    public void UpdateWhiteBoardCounter(bool Placed)
    {
        PlacedPartCounterWhiteBoard(Placed);
        m_WhiteBoardPartCounter.text = "Placed parts = " + m_PlacedPartsCount.ToString() + "/" + m_CorrOrder.Parts.Count.ToString();
    }
    public IEnumerator TotalTimer()
    {
        while (true)
        {
            time += Time.deltaTime;
            sec = (int)(time % 60);
            min = (int)(time / 60);

            m_TotalTimerText = " Time elapsed - " + string.Format("{0:00}:{1:00}", min, sec);

            yield return m_TotalTimerText;
        }
    }
}
[System.Serializable]
public class PlacedOrder
{
    public string Partname;
    public int PlacedId;
    public int ObjID;

}
