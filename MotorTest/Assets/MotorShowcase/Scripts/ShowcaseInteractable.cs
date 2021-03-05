using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NaughtyAttributes;
using TMPro;
using UnityEngine.Events;
using UnityEngine.XR;

public class ShowcaseInteractable : MonoBehaviour
{
    //[HorizontalLine(color: EColor.Blue)]

    [NaughtyAttributes.Tag]
    public string m_TagToLookFor;
    public bool m_IsFirstInteractable = false;

    //[HorizontalLine(color: EColor.Blue)]

    public float m_CustomActionDelay = 0f;
    public UnityEvent m_CustomInteractAction = new UnityEvent();

    //[HorizontalLine(color: EColor.Blue)]
    public ShowcaseInteractable m_NextInteractable;


    private Collider m_Collider;
    private POIController m_POI;


    private void Awake()
    {
        if (m_Collider == null) // if this component reference is null/empty, find the component on current gameobject
        {
            m_Collider = GetComponent<Collider>();
        }
        if (m_Collider == null) // if this component reference is still null/empty, find the component in current objects children
        {
            m_Collider = GetComponentInChildren<Collider>();
        }

        if (m_POI == null)
        {
            m_POI = GetComponent<POIController>();
        }
        if (m_POI == null)
        {
            m_POI = GetComponentInChildren<POIController>();
        }

        //if (XRSettings.enabled == false)
        //{
        //print($"POI   {m_POI}");
        //print($"poi button   {m_POI.m_Button.gameObject.name}");
        //print($"poi button click   {m_POI.m_Button.onClick}");
        m_POI.m_Button.onClick.AddListener(() => { Interact(); });
        //}
    }

    void Start()
    {


        if (m_IsFirstInteractable == true)
        {
            Enable();
        }
        else
        {
            Disable();
        }


    }

    public void Enable()
    {
        //print("interactable enable");
        m_POI.TurnOn();
        m_Collider.enabled = true;
    }

    public void Disable()
    {
        //print("interactable disable");
        m_POI.TurnOff();
        m_Collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (XRSettings.enabled == true)
        {
            if (other.CompareTag(m_TagToLookFor))
            {
                if (m_POI != null)
                {
                    m_POI.TurnOff();
                }

                Invoke("OnInteractEnd", m_CustomActionDelay);
            }
        }
    }

    public void Interact()
    {
        if (m_POI != null)
        {
            m_POI.TurnOff();
        }
        //print("interact got trigered");
        Invoke("OnInteractEnd", m_CustomActionDelay);
    }

    private void OnInteractEnd()
    {
        if (m_NextInteractable != null)
        {
            m_NextInteractable.Enable();
        }
        if (m_CustomInteractAction != null)
        {
            m_CustomInteractAction.Invoke();
        }
        m_Collider.enabled = false;

    }
}
