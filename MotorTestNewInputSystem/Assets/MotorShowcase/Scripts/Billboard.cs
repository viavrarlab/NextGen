using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, /*m_Camera.transform.rotation * */Vector3.up);
        //transform.LookAt(m_Camera.transform, transform.up);
        //transform.LookAt(new Vector3(m_Camera.transform.position.x, m_Camera.transform.position.y, transform.position.z));
    }
}
