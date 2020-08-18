using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectOrderTests : MonoBehaviour
{
    //public string Name;
    public List<CustomListClass> Parts;

    public void GetPartsNoArmature()
    {
        foreach (Transform child in transform)
        {
            MeshRenderer rend = child.GetComponent<MeshRenderer>();
            if (rend != null)
            {
                CustomListClass item = new CustomListClass
                {
                    obj = child.gameObject,
                    set = 0,
                    OrderNotMandatory = false
                };

                Parts.Add(item);
            }
        }
    }
}


