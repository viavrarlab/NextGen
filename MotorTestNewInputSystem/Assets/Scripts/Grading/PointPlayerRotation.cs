using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPlayerRotation : MonoBehaviour
{
    public Transform m_PlayerTransform;
    public Transform UIelement;
    [SerializeField]
    bool FlipUI;

    // Update is called once per frame
    void Update()
    {
        LookAtPlayer();
    }
    private void LookAtPlayer()
    {
        if (FlipUI)
        {
            var lookPos = m_PlayerTransform.position - UIelement.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(-lookPos);
            UIelement.rotation = Quaternion.Slerp(UIelement.rotation, rotation, Time.deltaTime * 10f);
        }
        else
        {
            var lookPos = m_PlayerTransform.position - UIelement.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            UIelement.rotation = Quaternion.Slerp(UIelement.rotation, rotation, Time.deltaTime * 10f);
        }
        
    }
}
