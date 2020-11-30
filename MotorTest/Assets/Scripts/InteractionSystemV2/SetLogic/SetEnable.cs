using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityScript.Core;

public class SetEnable : MonoBehaviour
{
    int count = 0;
    public List<GameObject> Sets;


    private void Start()
    {
        foreach(Transform child in this.transform)
        {
            if (child.childCount > 1)
            {
                for(int i = 0; i < child.childCount; i++)
                {
                    if (child.GetChild(i).gameObject.layer == 15)
                    {
                        Sets.Add(child.GetChild(i).gameObject);
                    }
                }
            }
        }
        disableNextSets();
    }
    private void Update()
    {
        ActivateNextSet();
    }
    public void ActivateNextSet()
    {
        foreach (GameObject set in Sets)
        {
            if (set.GetComponent<SetComplete>().complete == true && set.GetComponent<SetComplete>().SetID == count)
            {
                Sets[count + 1].SetActive(true);
                count++;
            }
            else
            {
                if(count != 0 && Sets[count-1].GetComponent<SetComplete>().complete == false)
                {
                    //Debug.Log(Sets[count].name.ToString());
                    Sets[count].SetActive(false);

                    count--;
                }
            }
        }
    }
    public void disableNextSets()
    {
        foreach(GameObject set in Sets)
        {
            if(set.GetComponent<SetComplete>().SetID != 0)
            {
                set.SetActive(false);

            }
        }
    }
}
