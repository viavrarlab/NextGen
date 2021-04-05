using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class VRInputModule : BaseInputModule
{
    public Camera m_Camera;
    //public SteamVR_Input_Sources m_TargetSource;
    //public SteamVR_Action_Boolean m_ClickAction;

    private GameObject m_CurrentObject = null;
    private PointerEventData m_Data = null;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        m_Data = new PointerEventData(eventSystem);
    }

    public override void Process()
    {
        //Reset data, set camera
        m_Data.Reset();
        m_Data.position = new Vector2(m_Camera.pixelWidth / 2f, m_Camera.pixelHeight / 2f);

        // Raycast
        eventSystem.RaycastAll(m_Data, m_RaycastResultCache);
        m_Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_CurrentObject = m_Data.pointerCurrentRaycast.gameObject;

        // Clear
        m_RaycastResultCache.Clear();

        // Hover
        HandlePointerExitAndEnter(m_Data, m_CurrentObject);

        //// Press
        //if (m_ClickAction.GetStateDown(m_TargetSource))
        //{
        //    ProcessPress(m_Data);
        //}

        //// Release
        //if (m_ClickAction.GetStateUp(m_TargetSource))
        //{
        //    ProcessRelease(m_Data);
        //}
    }

    public PointerEventData GetData()
    {
        return m_Data;
    }

    private void ProcessPress(PointerEventData _data)
    {
        // set raycast
        _data.pointerPressRaycast = _data.pointerCurrentRaycast;

        // check for object hit, get down handler, call
        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(m_CurrentObject, _data, ExecuteEvents.pointerDownHandler);

        // if no down handler, try and get click handler
        if(newPointerPress == null)
        {
            newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentObject);
        }

        // set data
        _data.pressPosition = _data.position;
        _data.pointerPress = newPointerPress;
        _data.rawPointerPress = m_CurrentObject;

    }

    private void ProcessRelease(PointerEventData _data)
    {
        // execute pointer up
        ExecuteEvents.Execute(_data.pointerPress, _data, ExecuteEvents.pointerUpHandler);

        // check for click handler
        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentObject);

        // check if actual 
        if(_data.pointerPress == pointerUpHandler)
        {
            ExecuteEvents.Execute(_data.pointerPress, _data, ExecuteEvents.pointerClickHandler);
        }

        // clear selected gameobject
        eventSystem.SetSelectedGameObject(null);

        // reset data
        _data.pressPosition = Vector2.zero;
        _data.pointerPress = null;
        _data.rawPointerPress = null;

    }
}
