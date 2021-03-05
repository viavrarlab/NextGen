using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class SwitchController : MonoBehaviour
{
    public Transform m_SwitchObject;
    public GameObject m_LEDObject;

    public Vector3 m_StateAngle_ON = Vector3.zero;
    public Vector3 m_StateAngle_OFF = Vector3.zero;

    public bool m_HasElectricity = false;
    public bool m_IsOn = false;

    public Color m_SwitchLEDColor_ON = Color.green;
    public Color m_SwitchLEDColor_OFF = Color.red;

    public UnityEvent m_SwitchToggleAction_ON = new UnityEvent();
    public UnityEvent m_SwitchToggleAction_OFF = new UnityEvent();

    private Renderer m_LEDRenderer;
    private MaterialPropertyBlock m_LEDPropBlock;

    // Start is called before the first frame update
    void Start()
    {
        m_LEDRenderer = m_LEDObject.GetComponent<Renderer>();
        m_LEDPropBlock = new MaterialPropertyBlock();

        ToggleSwitch(m_IsOn);
        ToggleLEDOnOff(m_IsOn);
    }

    [Button]
    public void TestToggle()
    {
        ToggleSwitch(!m_IsOn);
    }

    public void ToggleSwitch(bool state)
    {
        if(m_HasElectricity == false)
        {
            m_SwitchObject.rotation = Quaternion.Euler(m_StateAngle_OFF);
            // TODO: Add some functionality to show that there is no power
            return;
        }
        m_IsOn = state;
        if(state == true)
        {
            m_SwitchObject.rotation = Quaternion.Euler(m_StateAngle_ON);
            if(m_SwitchToggleAction_ON != null)
            {
                m_SwitchToggleAction_ON.Invoke();
            }
        }
        else
        {
            m_SwitchObject.rotation = Quaternion.Euler(m_StateAngle_OFF);
            if (m_SwitchToggleAction_OFF != null)
            {
                m_SwitchToggleAction_OFF.Invoke();
            }
        }
        ToggleLEDColor(state);
    }

    private void ToggleLEDColor(bool state)
    {
        Color col = (state == true) ? m_SwitchLEDColor_ON : m_SwitchLEDColor_OFF;

        m_LEDRenderer.GetPropertyBlock(m_LEDPropBlock);

        m_LEDPropBlock.SetColor("_EmissionColor", col);

        m_LEDRenderer.SetPropertyBlock(m_LEDPropBlock);
    }

    public void ToggleLEDOnOff(bool state)
    {
        Color col = (state == true) ? (m_IsOn ? m_SwitchLEDColor_ON : m_SwitchLEDColor_OFF) : Color.black;
        m_LEDRenderer.GetPropertyBlock(m_LEDPropBlock);

        m_LEDPropBlock.SetColor("_EmissionColor", col);

        m_LEDRenderer.SetPropertyBlock(m_LEDPropBlock);
    }

    private void OnMouseDown()
    {
        ToggleSwitch(!m_IsOn);
    }

    public void ToggleElectricity(bool value)
    {
        

        if(value == false)
        {
            ToggleSwitch(false);
        }

        m_HasElectricity = value;

        ToggleLEDOnOff(value);
    }

}
