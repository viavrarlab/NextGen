using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform m_Target;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Target.rotation * Vector3.forward, /*m_Camera.transform.rotation * */Vector3.up);
        //transform.LookAt(m_Camera.transform, transform.up);
        //transform.LookAt(new Vector3(m_Camera.transform.position.x, m_Camera.transform.position.y, transform.position.z));
    }
}
