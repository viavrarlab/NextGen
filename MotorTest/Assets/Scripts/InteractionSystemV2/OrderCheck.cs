using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Animations;
public class OrderCheck : MonoBehaviour
{
    public GameObject[] SocketGameObj;
    public PlacementPoint[] PP_Array;
    void Start()
    {
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
        if (other.transform.parent.transform.parent.GetComponent<ParentConstraint>().sourceCount > 1)
        {
            ParentConstraint TempConst = other.transform.parent.transform.parent.GetComponent<ParentConstraint>();
            List<ConstraintSource> TempOriginalSources = new List<ConstraintSource>();
            TempConst.GetSources(TempOriginalSources);
            List<ConstraintSource> TempUpdatedSources = new List<ConstraintSource>();
            foreach (ConstraintSource Constr in TempOriginalSources)
            {
                ConstraintSource TempSource = Constr;
                Debug.Log(TempSource.sourceTransform.name);
                if (gameObject.name == TempSource.sourceTransform.name)
                {
                    Debug.Log(gameObject.name);
                    TempSource.weight = 1f;
                    TempUpdatedSources.Add(TempSource);
                }
                else
                {
                    TempSource.weight = 0f;
                    TempUpdatedSources.Add(TempSource);
                }
                Debug.Log(TempSource.weight);
            }
            TempConst.SetSources(TempUpdatedSources);
        }
    }
    private void OnTriggerStay(Collider other)
    {

    }
    private void OnTriggerExit(Collider other)
    {

    }
}
