using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
// using Valve.VR;

public class Socket : MonoBehaviour
{



    [SerializeField]
    private Moveable m_StoredObject = null;
    private FixedJoint m_FixedJoint = null;

    private void Awake()
    {
        m_FixedJoint = GetComponent<FixedJoint>();
    }

    public void Attach(Moveable newObject)
    {
        print($"Current stored object in socket <{gameObject.name}> is <{m_StoredObject}>");
        if (m_StoredObject)
        {
            return;
        }

        m_StoredObject = newObject;

        //m_StoredObject.transform.position = transform.position;
        m_StoredObject.transform.DOMove(transform.position, 0.05f).OnStart(() =>
        {

            //m_StoredObject.transform.DORotate(transform.rotation.eulerAngles, 0.05f, RotateMode.FastBeyond360);
        }).OnComplete(() =>
        {
            m_StoredObject.transform.rotation = transform.rotation;
            Rigidbody targetBody = m_StoredObject.gameObject.GetComponent<Rigidbody>();
            m_FixedJoint.connectedBody = targetBody;
        });

        //m_StoredObject.transform.rotation = Quaternion.identity;


    }

    public void Detach()
    {
        if (!m_StoredObject)
        {
            return;
        }

        m_FixedJoint.connectedBody = null;
        m_StoredObject = null;
    }

    public Moveable GetStoredObject()
    {
        return m_StoredObject;
    }
}