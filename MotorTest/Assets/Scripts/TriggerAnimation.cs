using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{

    public Animator m_AnimatorController;
    public string m_TriggerName = "";


    public void Trigger()
    {
        m_AnimatorController.SetTrigger(m_TriggerName);
    }
}
