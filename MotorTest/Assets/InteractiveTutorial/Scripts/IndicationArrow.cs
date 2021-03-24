using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IndicationArrow : MonoBehaviour
{
    public Transform m_ArrowGraphics;
    public bool m_AutoPlay = true;

    public Transform m_PlayerTransform;

    private Sequence m_Sequence;

    public static IndicationArrow i;

    private void Awake ()
    {
        i = this;
    }

    // Start is called before the first frame update
    void Start ()
    {
        m_Sequence = DOTween.Sequence ();
        SetUpSequence ();
        if (m_AutoPlay) Tween_Play ();
    }

    private void Update ()
    {
        LookAtPlayer ();
    }

    private void LookAtPlayer ()
    {
        var lookPos = m_PlayerTransform.position - m_ArrowGraphics.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation (lookPos);
        m_ArrowGraphics.rotation = Quaternion.Slerp (m_ArrowGraphics.rotation, rotation, Time.deltaTime * 10f);
    }

    private void SetUpSequence ()
    {
        m_Sequence.Append (m_ArrowGraphics.DOLocalMoveY (0.05f + m_ArrowGraphics.position.y, 0.5f));
        m_Sequence.Append (m_ArrowGraphics.DOLocalMoveY (0f + m_ArrowGraphics.position.y, 0.15f));
        m_Sequence.SetLoops (-1, LoopType.Yoyo);
    }

    public void Tween_Play ()
    {
        m_Sequence.Play ();
    }

    public void Tween_Pause ()
    {
        m_Sequence.Pause ();
    }

    public void Move (Transform target)
    {
        transform.DOMove (target.position, 0.1f);

    }

    public void Move (Vector3 target)
    {
        transform.DOMove (target, 0.1f);

    }

}