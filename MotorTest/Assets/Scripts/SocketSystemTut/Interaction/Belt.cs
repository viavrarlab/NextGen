using UnityEngine;
// using Valve.VR;
using HTC.UnityPlugin.Vive;

public class Belt : MonoBehaviour
{
    [Range(0.5f, 0.75f)] public float m_Height = 0.5f;

    public Transform m_Head = null;

    private void Start()
    {
        
    }

    private void Update()
    {
        PositionUnderHead();
        RotateWithHead();
    }

    private void PositionUnderHead()
    {
        Vector3 adjustedHeight = m_Head.position;
        adjustedHeight.y = Mathf.Lerp(0f, adjustedHeight.y, m_Height);

        transform.position = adjustedHeight;
    }

    private void RotateWithHead()
    {
        Vector3 adjustedRotation = m_Head.eulerAngles;

        adjustedRotation.x = 0;
        adjustedRotation.z = 0;

        transform.eulerAngles = adjustedRotation;
    }
}
