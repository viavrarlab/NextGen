using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetComplete : MonoBehaviour
{
    public bool complete = false;
    public int SetID;
    private void Update()
    {
        foreach(Transform child in this.transform)
        {
            if(child.GetComponent<Slot>().m_CurrentSocketState == Slot.SocketState.Snapped)
            {
                complete = true;
            }
            else
            {
                complete = false;
            }
        }
    }
}
