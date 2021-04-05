using ModelOutline;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    public GameObject collidingObjectToBePickedUp;
    public GameObject objectinhand;

    private Placeable CurrentPickUpOBJ;
    public CorrectOrderTests m_CorrectOrder;
    public GameObject[] m_PlaceArray;
    public bool Outline;
    [SerializeField]
    private bool m_triggerPush;
    [SerializeField]
    private bool m_GrabPush;
    [SerializeField]
    private bool m_triggerAxisPull;
    public bool TriggerPush { get => m_triggerPush; set => m_triggerPush = value; }
    public bool TriggerAxisPull { get => m_triggerAxisPull; set => m_triggerAxisPull = value; }
    public bool GrabPush { get => m_GrabPush; set => m_GrabPush = value; }

    public List<GameObject> CollidingObj = new List<GameObject>();

    private void Awake()
    {
        m_CorrectOrder = FindObjectOfType<CorrectOrderTests>();
        if(m_CorrectOrder != null)
        {
            m_PlaceArray = new GameObject[m_CorrectOrder.Parts.Count];
            for (int i = 0; i < m_CorrectOrder.Parts.Count; i++)
            {
                if (m_CorrectOrder.Parts[i].obj != null)
                {
                    m_PlaceArray[i] = m_CorrectOrder.Parts[i].obj;
                }
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Socket"))
        {
            Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), other);
        }
        if (other.CompareTag("MotorCollider") || other.CompareTag("SetGrab") || other.CompareTag("PlacementRoot") || other.CompareTag("Box"))
        {
            if (other.CompareTag("PlacementRoot") || other.CompareTag("Box"))
            {
                CollidingObj.Add(other.gameObject);
            }
            if (other.CompareTag("MotorCollider") || other.CompareTag("SetGrab"))
            {
                //add object to pickup array
                CollidingObj.Add(other.transform.parent.transform.parent.gameObject);
                //check if the obj can be taken out
                if (other.transform.parent.transform.parent.GetComponent<Placeable>() != null && other.transform.parent.transform.parent.GetComponent<Placeable>().m_IsPlaced == true)
                {
                    for (int i = 0; i < m_PlaceArray.Length; i++)
                    {
                        if (m_PlaceArray[i] != null && m_PlaceArray[i].GetComponent<Placeable>() != null && i < m_PlaceArray.Length -1)
                        {
                            if (other.transform.parent.transform.parent.gameObject.GetComponent<Placeable>().m_ID == m_PlaceArray[i].GetComponent<Placeable>().m_ID && m_PlaceArray[i + 1].GetComponent<Placeable>().m_IsPlaced == false)
                            {
                                m_PlaceArray[i].GetComponent<Placeable>().CanTakeOut = true;
                                return;
                            }
                            else
                            {
                                m_PlaceArray[i].GetComponent<Placeable>().CanTakeOut = false;
                            }
                        }
                    }
                }
            }
        }
    }
    public void OnTriggerStay(Collider other)
    {
        foreach (GameObject GO in CollidingObj)
        {
            if(GO.GetComponent<Outline>() != null)
            {
                if (GO == CollidingObj.Last())
                {
                    GO.GetComponent<Outline>().OutlineColor = Color.white;
                    GO.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    GO.GetComponent<Outline>().enabled = false;
                }
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlacementRoot") || other.CompareTag("Box"))
        {
            if (other.GetComponent<Outline>() != null)
            {
                other.GetComponent<Outline>().enabled = false;
            }
            CollidingObj.Remove(other.gameObject);
        }
        else
        {
            other.transform.parent.transform.parent.gameObject.GetComponent<Outline>().enabled = false;
            other.transform.parent.transform.parent.gameObject.GetComponent<Placeable>().CanTakeOut = false;
            CollidingObj.Remove(other.transform.parent.transform.parent.gameObject);
        }

        if (!collidingObjectToBePickedUp)
        {
            return;
        }
        collidingObjectToBePickedUp = null;

    }
    public void PickUPSet()
    {
        foreach (GameObject go in CollidingObj)
        {
            if (!go.CompareTag("MotorPart"))
            {
                if (go.CompareTag("SetGrab") || go.CompareTag("PlacementRoot") || go.CompareTag("Box"))
                {
                    collidingObjectToBePickedUp = go;
                    break;
                }
            }
        }
        if (collidingObjectToBePickedUp != null)
        {
            GrabObject();
        }
    }
    public void PickUpObj()
    {
        foreach (GameObject go in CollidingObj)
        {
            if (!go.CompareTag("PlacementRoot") || !go.CompareTag("SetGrab"))
            {
                if (go.CompareTag("MotorPart") && CollidingObj.Last())
                {
                    collidingObjectToBePickedUp = go;
                }
            }
        }
        if (collidingObjectToBePickedUp != null)
        {
            GrabObject();
        }
    }
    void GrabObject()
    {
        objectinhand = collidingObjectToBePickedUp;
        FixedJoint joint = GetComponent<FixedJoint>();
        joint.connectedBody = objectinhand.GetComponent<Rigidbody>();
        CurrentPickUpOBJ = objectinhand.GetComponent<Placeable>();
    }

    public void ReleaseObject()
    {
        FixedJoint fj = GetComponent<FixedJoint>();
        if (fj)
        {
            fj.connectedBody = null;
        }
        objectinhand = null;
        CurrentPickUpOBJ = null;
        collidingObjectToBePickedUp = null;
        TriggerPush = false;
    }
}
