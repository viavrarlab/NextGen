using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityScript.Core;

public class SetEnable : MonoBehaviour
{
    int count = 0;
    public List<GameObject> Sets;
    public GameObject PlacementSocketRoot;
    private void Start()
    {
        foreach(Transform child in PlacementSocketRoot.transform)
        {
            Sets.Add(child.gameObject);
        }
        disableNextSets();
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
                    Debug.Log(Sets[count].name.ToString());
                    Sets[count].SetActive(false);

                    count--;
                }
            }
        }
        Debug.Log(count);
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
