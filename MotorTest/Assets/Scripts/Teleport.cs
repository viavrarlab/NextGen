using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleport : MonoBehaviour
{
    public GameObject m_Marker;
    public SteamVR_Action_Boolean m_TeleportAction;

    public LayerMask m_TeleportLayers = -1;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private bool m_HasPosition = false;

    private bool m_IsTeleporting = false;
    public float m_FadeTime = 0.25f;

    void Start()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
    }

    void FixedUpdate()
    {
        m_HasPosition = UpdatePointer();
        m_Marker.SetActive(m_HasPosition);

        if (m_TeleportAction.GetStateUp(m_Pose.inputSource))
        {
            TryTeleport();
        }
    }

    private void TryTeleport()
    {
        if(!m_HasPosition || m_IsTeleporting)
        {
            return;
        }

        Transform cameraRig = SteamVR_Render.Top().origin;
        Vector3 headPosition = SteamVR_Render.Top().head.position;
        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        Vector3 translateVector = m_Marker.transform.position - groundPosition;
        StartCoroutine(MoveRig(cameraRig, translateVector));
    }

    private IEnumerator MoveRig(Transform cameraRig, Vector3 translation)
    {
        m_IsTeleporting = true;

        //SteamVR_Fade.Start(Color.black, m_FadeTime, true);
        SteamVR_Fade.View(Color.black, m_FadeTime);

        yield return new WaitForSeconds(m_FadeTime);
        cameraRig.position += translation;

        //SteamVR_Fade.Start(Color.clear, m_FadeTime, true);
        SteamVR_Fade.View(Color.clear, m_FadeTime);

        m_IsTeleporting = false;

    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 50f, m_TeleportLayers) )
        {
            m_Marker.transform.position = hit.point;
            return true;
        }

        return false;
    }
}
