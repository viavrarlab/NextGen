using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetComplete : MonoBehaviour
{
    public bool complete = false;
    public int SetID;
    private int childrenCount;
    //private int placedobjs = 0;
    public List<GameObject> childs;
    private void Awake()
    {
        foreach(Transform child in this.transform)
        {
            childs.Add(child.gameObject);
        }
        childs.ToArray();
    }
    private void Update()
    {
        checkPlacedObject();
    }
    public void checkPlacedObject()
    {
        for(int i = 0; i <= childs.Count; i++)
        {

                if (childs[0].GetComponent<FixedJoint>().connectedBody != null && childs[childs.Count - 1].GetComponent<FixedJoint>().connectedBody != null)
                {
                    complete = true;
                }
                else
                {
                    complete = false;
                }
            Debug.Log("set object count - " + childs.Count.ToString());
        }
        //foreach (Transform child in this.transform)
        //{
        //    if (placedobjs >= 0)
        //    {
        //        if (child.GetComponent<FixedJoint>().connectedBody != null)
        //        {
        //            placedobjs++;
        //        }
        //        if (child.GetComponent<FixedJoint>().connectedBody == null)
        //        {
        //            placedobjs--;
        //        }
        //    }
        //}
        //print("In set placed = " + placedobjs.ToString() + "objects");
        ////Debug.Log(placedobjs.ToString());
        //if (placedobjs == childrenCount)
        //{
        //    complete = true;
        //}
        //else
        //{
        //    complete = false;
        //}
    }
}
