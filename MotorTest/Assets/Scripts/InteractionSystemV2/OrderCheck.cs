using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Animations;
using System.Linq;

public class OrderCheck : MonoBehaviour
{
    public GameObject[] SocketGameObj;
    public PlacementPoint[] PP_Array;
    public bool m_OrderCorrect;
    public bool m_CanTakeOut;

    public OrderCheckList item;

    void Start()
    {
        //Get All Socket OBJ
        SocketGameObj = GameObject.FindGameObjectsWithTag("Socket");

        PP_Array = new PlacementPoint[SocketGameObj.Length];
        for (int i = 0; i < SocketGameObj.Length; i++)
        {
            PP_Array[i] = SocketGameObj[i].GetComponent<PlacementPoint>();
        }
        Array.Clear(SocketGameObj, 0, SocketGameObj.Length);
        SocketGameObj = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlacementRoot"))
        {
            Physics.IgnoreCollision(GetComponent<BoxCollider>(), other.GetComponent<BoxCollider>());
        }
        //Check if Constraint has more than 1 source and change weights according to the gameobject
        if (other.transform.parent.transform.parent != null && other.transform.parent.transform.parent.GetComponent<ParentConstraint>().sourceCount > 1)
        {
            if (other.transform.parent.transform.parent.GetComponent<Placeable>().m_ID == gameObject.GetComponent<PlacementPoint>().m_PlaceableID)
            {
                ParentConstraint TempConst = other.transform.parent.transform.parent.GetComponent<ParentConstraint>();
                List<ConstraintSource> TempOriginalSources = new List<ConstraintSource>();
                TempConst.GetSources(TempOriginalSources);
                List<ConstraintSource> TempUpdatedSources = new List<ConstraintSource>();
                foreach (ConstraintSource Constr in TempOriginalSources)
                {
                    ConstraintSource TempSource = Constr;
                    if (gameObject.name == TempSource.sourceTransform.name)
                    {
                        TempSource.weight = 1f;
                        TempUpdatedSources.Add(TempSource);
                    }
                    else
                    {
                        TempSource.weight = 0f;
                        TempUpdatedSources.Add(TempSource);
                    }
                }
                TempConst.SetSources(TempUpdatedSources);
            }
        }
        if (other.transform.parent.transform.parent != null)
        {
            if (other.transform.parent.transform.parent.GetComponent<Placeable>().m_ID == gameObject.GetComponent<PlacementPoint>().m_PlaceableID)
            {
                List<OrderCheckList> TempList = new List<OrderCheckList>();
                for (int i = 0; i < PP_Array.Length; i++)
                {
                    item = new OrderCheckList
                    {
                        m_Obj_ID = PP_Array[i].m_PlaceableID,
                        m_isPlaced = PP_Array[i].m_IsOccupied
                    };
                    TempList.Add(item);
                }
                if (TempList != null)
                {
                    for (int i = 0; i < TempList.Count; i++)
                    {
                        OrderCheckList tempOrder = TempList[i];
                        if (tempOrder.m_Obj_ID == other.transform.parent.transform.parent.GetComponent<Placeable>().m_ID)
                        {
                            m_OrderCorrect = true;
                        }
                        else
                        {
                            if (tempOrder.m_Obj_ID >= 0 && tempOrder.m_Obj_ID == other.transform.parent.transform.parent.GetComponent<Placeable>().m_ID - 1)
                            {
                                Debug.Log(tempOrder.m_Obj_ID);
                                if (tempOrder.m_isPlaced == true)
                                {
                                    m_OrderCorrect = true;
                                }
                                else
                                {
                                    m_OrderCorrect = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
