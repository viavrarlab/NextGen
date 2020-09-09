using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Valve.VR;
using DG.Tweening;
using HTC.UnityPlugin.Vive;

public class Teleport : MonoBehaviour
{
    public GameObject m_Marker;
    public Transform m_CameraRig;
    public Transform m_Camera;
    //public SteamVR_Action_Boolean m_TeleportAction;

    public LayerMask m_TeleportLayers = -1;

    //public HandRole m_HandRole;
    //public ControllerButton m_ControllerButton;

    private bool m_TeleportStarted = false;

    //private SteamVR_Behaviour_Pose m_Pose = null;
    private bool m_HasPosition = false;

    private bool m_IsTeleporting = false;
    public float m_FadeTime = 0.25f;

    private MeshRenderer m_Renderer;
    private Material m_Material;
    RaycastHit m_Hit;

    void Start()
    {
        //m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Marker.SetActive(false);
    }

    void FixedUpdate()
    {
        if (m_TeleportStarted)
        {
            m_HasPosition = UpdatePointer();
        }

        // THIS IS THE LAST USED:  =================

        //if (ViveInput.GetPressUp(m_HandRole, m_ControllerButton))
        //{
        //    TryTeleport();
        //}

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


        //SteamVR_Fade.Start(Color.black, m_FadeTime, true);
        //SteamVR_Fade.View(Color.black, m_FadeTime);

        //yield return new WaitForSeconds(m_FadeTime);
        yield return new WaitForEndOfFrame();
        //Vector3 posRig = new Vector3(targetPos.position.x, cameraRig.position.y, targetPos.position.z);
        //Vector3 posCam = new Vector3(targetPos.position.x, m_Camera.position.y, targetPos.position.z);
        cameraRig.position = targetPos;
        //m_Camera.position = posCam;


        //SteamVR_Fade.Start(Color.clear, m_FadeTime, true);
        //SteamVR_Fade.View(Color.clear, m_FadeTime);

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
