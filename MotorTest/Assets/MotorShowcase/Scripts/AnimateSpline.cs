using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesher;
using DG.Tweening;
using NaughtyAttributes;

public class AnimateSpline : MonoBehaviour
{
    public LineManager m_Spline;

    public Transform m_RestPosition;
    public Transform m_EndPosition;

    public float m_AnimationLength = 1f;

    private bool m_IsAtRest = false;

    

    // Start is called before the first frame update
    void Start()
    {
        if (m_Spline == null)
        {
            m_Spline = GetComponent<LineManager>();
        }

        AnimateToRest(0.001f);
    }

    [Button]
    public void AnimateToRest()
    {

        ToRest(m_AnimationLength);
    }

    public void AnimateToRest(float time)
    {

        ToRest(time);
    }

    void ToRest(float time)
    {
        Vector3 prevPos = m_Spline.knotList[m_Spline.knotList.Count - 1];
        DOTween.To(() => m_Spline.knotList[m_Spline.knotList.Count - 1], x => m_Spline.knotList[m_Spline.knotList.Count - 1] = x, m_RestPosition.localPosition, time).OnUpdate(() =>
        {
            Vector3 deltaPos = prevPos - m_Spline.knotList[m_Spline.knotList.Count - 1]; // Get knot delta position
            m_Spline.controllerList[m_Spline.controllerList.Count - 1] -= deltaPos; // Add/Subtract the delta position to the last spline controller (if it's the last knot, there will only be one controller)

            m_Spline.ManualUpdate();

            prevPos = m_Spline.knotList[m_Spline.knotList.Count - 1];
        }).OnComplete(() =>
        {
            m_IsAtRest = true;
        });
    }

    [Button]
    public void AnimateToEnd()
    {
        ToEnd(m_AnimationLength);
    }

    public void AnimateToEnd(float time)
    {
        ToEnd(time);
    }

    void ToEnd(float time)
    {
        Vector3 prevPos = m_Spline.knotList[m_Spline.knotList.Count - 1];
        DOTween.To(() => m_Spline.knotList[m_Spline.knotList.Count - 1], x => m_Spline.knotList[m_Spline.knotList.Count - 1] = x, m_EndPosition.localPosition, time).OnUpdate(() =>
        {
            Vector3 deltaPos = prevPos - m_Spline.knotList[m_Spline.knotList.Count - 1];
            m_Spline.controllerList[m_Spline.controllerList.Count - 1] -= deltaPos;

            m_Spline.ManualUpdate();

            prevPos = m_Spline.knotList[m_Spline.knotList.Count - 1];
        }).OnComplete(() =>
        {
            m_IsAtRest = false;
        });
    }
}
