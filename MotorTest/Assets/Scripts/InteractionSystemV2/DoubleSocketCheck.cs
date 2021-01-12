using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSocketCheck : MonoBehaviour
{

    Placeable m_placeable;
    public List<Collider> NearSockets;
    Collider m_ObjCollider;
    public List<Vector3> ObjectDistance;
    private void Start()
    {
        m_placeable = GetComponent<Placeable>();
        NearSockets = new List<Collider>();
        ObjectDistance = new List<Vector3>();
        m_ObjCollider = GetComponentInChildren<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlacementPoint>() != null)
        {
            if (m_placeable.m_ID == other.gameObject.GetComponent<PlacementPoint>().m_PlaceableID)
            {
                NearSockets.Add(other.gameObject.GetComponent<Collider>());
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(NearSockets.Count > 1)
        {
            if(ObjectDistance.Count != NearSockets.Count)
            {
                for (int i = 0; i < NearSockets.Count; i++)
                {
                    Vector3 TempVector = NearSockets[i].bounds.center - m_ObjCollider.bounds.center;
                    ObjectDistance.Add(TempVector);
                }
                if (ObjectDistance[0].sqrMagnitude > ObjectDistance[1].sqrMagnitude)
                {
                    NearSockets[0].gameObject.GetComponent<PlacementPoint>().m_SnappableObject = gameObject.GetComponent<Placeable>();
                    NearSockets[1].gameObject.GetComponent<PlacementPoint>().m_SnappableObject = null ;
                }
                else
                {
                    NearSockets[1].gameObject.GetComponent<PlacementPoint>().m_SnappableObject = gameObject.GetComponent<Placeable>();
                    NearSockets[0].gameObject.GetComponent<PlacementPoint>().m_SnappableObject = null;
                }
                ObjectDistance.Clear();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        NearSockets.Remove(other.gameObject.GetComponent<Collider>());
        ObjectDistance.Clear();
    }
    public void checkCenter()
    {

    }
}
