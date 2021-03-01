using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class InteractiveTutorialController : MonoBehaviour
{
    public int m_CurrentActiveIndex = 0;
    public InteractableObject m_CurrentActiveObject;

    public List<InteractableObject> m_ObjectListOrdered = new List<InteractableObject>();
    public List<SocketObject> m_AllSockets = new List<SocketObject>();

    public List<CustomPartAction> m_CustomActions = new List<CustomPartAction>();

    public IndicationArrow m_IndicationArrow;

    public TextMeshProUGUI m_InstructionText;
    
    public GameObject m_StartUIButtons;
    public GameObject m_CompletedUIButtons;

    public ObjectScatterer m_Scatterer;

    public Transform m_TargetObject;

    public List<MotorPart> m_MotorPartsConfig;
    public string m_CSVConfigFileName = "configList.csv";

    public static InteractiveTutorialController i;

    private void Awake()
    {
        i = this;
    }

    void Start()
    {
        ResetTutorial();
    }

    private void Init()
    {
        m_Scatterer.PerformReset();
        m_Scatterer.PerformScatter();
        m_CurrentActiveIndex = 0;
        DisableAllSockets();
        GetSocketForObject(m_ObjectListOrdered[m_CurrentActiveIndex]).ToggleSocketActiveState(true);
        SetInstructionText(m_ObjectListOrdered[m_CurrentActiveIndex].m_MotorPartInformation.m_UIName);
        IndicationArrow.i.Move(m_ObjectListOrdered[m_CurrentActiveIndex].transform);
        
    }

    public void BeginTutorial()
    {
        m_CompletedUIButtons.SetActive(false);
        m_StartUIButtons.SetActive(false);
        Init();
    }

    public void EndTutorial()
    {
        SetInstructionText("Motor completed!", true);
        IndicationArrow.i.Move(Vector3.up * 500f);
        m_CompletedUIButtons.SetActive(true);
    }

    public void ResetTutorial()
    {
        SetInstructionText("Press 'Start' to begin!", true);
        m_StartUIButtons.SetActive(true);
        m_CompletedUIButtons.SetActive(false);
    }

    private void DisableAllSockets()
    {
        foreach (SocketObject s in m_AllSockets)
        {
            s.ToggleSocketActiveState(false);
        }
    }

    public void GetTargetObjects()
    {
        m_ObjectListOrdered.Clear();
        m_AllSockets.Clear();

        // Get interactable objects
        foreach (Transform t in m_TargetObject)
        {
            if (t.gameObject.name.Contains("Sockets") == false)
            {
                m_ObjectListOrdered.Add(t.gameObject.GetComponent<InteractableObject>());
            }
        }

        // Get sockets
        Transform socketRoot = m_TargetObject.Find("Sockets");
        foreach (Transform t in socketRoot)
        {
            m_AllSockets.Add(t.GetComponent<SocketObject>());
        }
    }

    public void LoadMotorPartConfigCSV()
    {
        string loadPath = Application.dataPath + "/Resources/" + m_CSVConfigFileName;

        m_MotorPartsConfig = Sinbad.CsvUtil.LoadObjects<MotorPart>(loadPath);

        List<InteractableObject> tempObjectList = new List<InteractableObject>();
        tempObjectList = m_ObjectListOrdered;

        InteractableObject[] tempSortList = new InteractableObject[m_MotorPartsConfig.Count];

        foreach (InteractableObject go in tempObjectList)
        {
            foreach (MotorPart motorPart in m_MotorPartsConfig)
            {
                if (go.gameObject.name == motorPart.m_GameObjectName)
                {
                    int indexConfig = m_MotorPartsConfig.IndexOf(motorPart);
                    tempSortList[indexConfig] = go;
                    continue;
                }
            }
        }
        m_ObjectListOrdered = tempSortList.ToList();
    }

    public void PopulateInteractableObjectPartInformation()
    {
        for (int i = 0; i < m_ObjectListOrdered.Count; i++)
        {
            m_ObjectListOrdered[i].m_MotorPartInformation = m_MotorPartsConfig[i];
        }
    }

    public void ActivateNextSocket()
    {
        GetSocketForObject(m_ObjectListOrdered[m_CurrentActiveIndex]).ToggleSocketActiveState(false);
        CheckAndExecuteCustomActions(m_ObjectListOrdered[m_CurrentActiveIndex]);

        if (m_CurrentActiveIndex < m_ObjectListOrdered.Count - 1)
        {
            m_CurrentActiveIndex += 1;
            GetSocketForObject(m_ObjectListOrdered[m_CurrentActiveIndex]).ToggleSocketActiveState(true);
            SetInstructionText(m_ObjectListOrdered[m_CurrentActiveIndex].m_MotorPartInformation.m_UIName);
            IndicationArrow.i.Move(m_ObjectListOrdered[m_CurrentActiveIndex].transform);
        }
        else
        {
            EndTutorial();
        }
    }

    private void CheckAndExecuteCustomActions(InteractableObject interactableObject)
    {
        foreach(CustomPartAction action in m_CustomActions)
        {
            if (action.m_UseIndividualObjectOrder)
            {
                if (action.m_IndividualObjectOrderID == m_CurrentActiveIndex)
                {
                    foreach (InteractableObject intObj in GetAllGroupObjects(action.m_TargetGroupID)) // Get all objects in the target group and move them
                    {
                        intObj.LocalMove(action.m_MoveOffsetFromOrigin);
                    }
                }
            }
            else
            {
                if (interactableObject.m_MotorPartInformation.m_GroupID == action.m_CheckGroupID) // If groupID for object and action are equal
                {
                    int tempNextActiveID = m_CurrentActiveIndex + 1;
                    if (m_ObjectListOrdered[tempNextActiveID].m_MotorPartInformation.m_GroupID != interactableObject.m_MotorPartInformation.m_GroupID) // If the current object is the last one in the group
                    {
                        foreach (InteractableObject intObj in GetAllGroupObjects(action.m_TargetGroupID)) // Get all objects in the target group and move them
                        {
                            intObj.LocalMove(action.m_MoveOffsetFromOrigin);
                        }
                    }
                }
            }

        }
    }

    private List<InteractableObject> GetAllGroupObjects(int groupID)
    {
        List<InteractableObject> tempList = new List<InteractableObject>();
        foreach (InteractableObject interactableObject in m_ObjectListOrdered)
        {
            if(interactableObject.m_MotorPartInformation.m_GroupID == groupID)
            {
                tempList.Add(interactableObject);
            }
        }

        return tempList;
    }

    public void SetInstructionText(string text, bool useOnlyArgumentString = false)
    {
        
        m_InstructionText.text = useOnlyArgumentString == true ? text : $"Pick up the '{text}' and place it.";
    }

    private SocketObject GetSocketForObject(InteractableObject obj)
    {
        foreach (SocketObject s in m_AllSockets)
        {
            if (s.m_ExpectedObject == obj)
            {
                return s;
            }
        }
        return null;
    }
}

[System.Serializable]
public class CustomPartAction
{
    
    public bool m_UseIndividualObjectOrder = false; // Toggles between m_CheckGroupID and m_IndividualObjectOrderID to designate when to execute action
    public int m_CheckGroupID;
    public int m_IndividualObjectOrderID;
    public int m_TargetGroupID; // The group to use in the action
    public Vector3 m_MoveOffsetFromOrigin;
}
