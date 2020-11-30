using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetComplete : MonoBehaviour
{
    public bool complete = false;
    public int SetID;

    public GameObject SetMesh;

    MeshRenderer SetMR;

    public List<GameObject> childs;
    public List<GameObject> SetModels;
    CorrectOrderTests COT;
    private void Awake()
    {
        foreach (Transform child in this.transform)
        {
            childs.Add(child.gameObject);
        }
        childs.ToArray();

        if (GameObject.Find(gameObject.name).tag == "SetGrab")
        {
            SetMesh = GameObject.Find(gameObject.name);
        }
        else
        {
            return;
        }

        if(SetMesh.GetComponent<MeshRenderer>() != null)
        {
            SetMR = SetMesh.GetComponent<MeshRenderer>();
            if (SetMR.enabled == true)
            {
                SetMR.enabled = false;
            } 
        }
        COT = GetComponentInParent<CorrectOrderTests>();
        foreach(CustomListClass GO in COT.Parts)
        {
            if(GO.set == SetID)
            {
                SetModels.Add(GO.obj);
            }
        }
    }
    private void Update()
    {
        checkPlacedObject();
        if(SetMR.enabled == false && complete == true)
        {
            EnableSetModelMesh();
        }
        if(SetMR.enabled == true && complete == false)
        {
            DisableSetModelMesh();
        }
    }
    public void checkPlacedObject()
    {
        for(int i = 0; i < childs.Count; i++)
        {

            if (childs[i].GetComponent<PlacementPoint>().m_IsOccupied)
            {
                complete = true;
            }
            else
            {
                complete = false;
                break;
            }

        }
    }
    public void EnableSetModelMesh() 
    {
        SetMR.enabled = true;
        foreach(GameObject GO in SetModels)
        {
            GO.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    public void DisableSetModelMesh()
    {
        SetMR.enabled = false;
        foreach (GameObject GO in SetModels)
        {
            GO.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
