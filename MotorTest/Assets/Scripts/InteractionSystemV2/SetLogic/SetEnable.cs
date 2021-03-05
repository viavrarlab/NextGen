using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;

public class SetEnable : MonoBehaviour
{
    int count = 0;
    public List<GameObject> Sets;

    GameControllerSC GController;

    [SerializeField]
    bool m_ModelComplete;

    public bool M_ModelComplete
    {
        get { return m_ModelComplete; }
        set
        {
            if (M_ModelComplete == value) return;
            M_ModelComplete = value;
            if (M_ModelComplete)
            {
                
            }
        }
    }
    private void Start()
    {
        GController = FindObjectOfType<GameControllerSC>();
        foreach (Transform child in this.transform)
        {
            if (child.childCount > 1)
            {
                for (int i = 0; i < child.childCount; i++)
                {
                    if (child.GetChild(i).gameObject.layer == 15)
                    {
                        Sets.Add(child.GetChild(i).gameObject);
                    }
                }
            }
        }
        if (GController != null)
        {
            if (GController.SetOrderCheck)
            {
                disableNextSets();
            }
        }
        else
        {
            disableNextSets();
        }

    }
    private void Update()
    {
        if (GController != null)
        {
            if (GController.SetOrderCheck)
            {
                ActivateNextSet();
            }
        }
        else
        {
            ActivateNextSet();
        }
    }
    public void ActivateNextSet()
    {
        if (Sets.All(x => x.GetComponent<SetComplete>().complete == true))
        {
            m_ModelComplete = true;
            return;
        }
        else
        {
            m_ModelComplete = false;
        }
        if (Sets.Any(x => x.GetComponent<SetComplete>().complete == false))
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
                    if (count != 0 && Sets[count - 1].GetComponent<SetComplete>().complete == false)
                    {
                        Sets[count].SetActive(false);
                        count--;
                    }
                }

            }
        }
    }
    public void disableNextSets()
    {
        foreach (GameObject set in Sets)
        {
            if (set.GetComponent<SetComplete>().SetID != 0)
            {
                set.SetActive(false);
            }
        }
    }
}
