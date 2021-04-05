using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    public float m_DefaultLength = 5f;
    public GameObject m_Dot;
    public VRInputModule m_InputModule;

    private LineRenderer m_LineRenderer;


    void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        UpdateLine();
    }

    void UpdateLine()
    {
        PointerEventData data = m_InputModule.GetData();
        float targetLength = 0f;
        if (data.pointerCurrentRaycast.distance == 0)
        {
            m_LineRenderer.enabled = false;
            m_Dot.SetActive(false);
            return;
        }
        else
        {
            m_LineRenderer.enabled = true;
            m_Dot.SetActive(true);
            targetLength = data.pointerCurrentRaycast.distance;
        }

        //targetLength = data.pointerCurrentRaycast.distance == 0 ? m_DefaultLength : data.pointerCurrentRaycast.distance;

        RaycastHit hit = CreateRaycast(targetLength);

        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        if(hit.collider != null)
        {
            endPosition = hit.point;
        }

        m_Dot.transform.position = endPosition;

        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, endPosition);
    }

    RaycastHit CreateRaycast(float _length)
    {
        RaycastHit hit;

        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, _length);

        return hit;
    }
}
