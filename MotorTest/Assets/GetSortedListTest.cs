using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSortedListTest : MonoBehaviour
{
    CorrectOrderTests cororder;
    // Start is called before the first frame update
    void Start()
    {
        cororder = GetComponent<CorrectOrderTests>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var x in cororder.Parts)
        {
            print(x.obj);
        }
    }
}
