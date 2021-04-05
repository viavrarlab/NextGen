using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Valve.VR;
using DG.Tweening;
//using HTC.UnityPlugin.Vive;

public class Teleport : MonoBehaviour
{
    public GameObject m_Marker;
    public Transform m_CameraRig;
    public Transform m_Camera;

    public LayerMask m_TeleportLayers = -1;

    private bool m_TeleportStarted = false;

    private bool m_HasPosition = false;

    private bool m_IsTeleporting = false;
    public float m_FadeTime = 0.25f;

    RaycastHit m_Hit;

    //Grading
    public int TeleportCount;
    public float TeleportDistance;

    void Start()
    {
        m_Marker = GameObject.Find("NextGen_TP_Marker");
    }

    void FixedUpdate()
    {
        if (m_TeleportStarted)
        {
            m_HasPosition = UpdatePointer();
        }
    }

    public void TeleportStart()
    {
        m_TeleportStarted = true;
        m_Marker.SetActive(true);
    }

    public void TeleportEnd()
    {
        m_TeleportStarted = false;
        TryTeleport();
    }

    private void TryTeleport()
    {
        if (!m_HasPosition || m_IsTeleporting)
        {
            return;
        }

        var headVector = Vector3.ProjectOnPlane(m_Camera.position - m_CameraRig.position, m_CameraRig.up);
        var targetPos = m_Hit.point - headVector;

        StartCoroutine(DoTeleport(m_CameraRig, /*m_Marker.transform*/targetPos));
    }

    private IEnumerator DoTeleport(Transform cameraRig, Vector3 targetPos)
    {
        m_IsTeleporting = true;

        yield return new WaitForEndOfFrame();
        TeleportDistance += Vector3.Distance(cameraRig.position, targetPos);
        TeleportCount++;
        if(GradingController.Instance != null)
        {
            GradingController.Instance.TeleportCounterAndDistance(TeleportCount, TeleportDistance);
        }
        cameraRig.position = targetPos;

        m_IsTeleporting = false;
        m_Marker.SetActive(false);

    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f, m_TeleportLayers))
        {
            m_Hit = hit;
            m_Marker.transform.position = hit.point;
            return true;
        }

        return false;
    }
}
