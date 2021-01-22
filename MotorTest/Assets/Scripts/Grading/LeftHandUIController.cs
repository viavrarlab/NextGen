using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftHandUIController : MonoBehaviour
{
    [SerializeField]
    Canvas m_ControllerUI;

    private void Awake()
    {
        m_ControllerUI = gameObject.GetComponentInChildren<Canvas>();
        m_ControllerUI.enabled = false;
    }
    public void EnableUI()
    {
        if (m_ControllerUI.enabled)
        {
            m_ControllerUI.enabled = false;
        }
        else
        {
            m_ControllerUI.enabled = true;
        }
    }

}
