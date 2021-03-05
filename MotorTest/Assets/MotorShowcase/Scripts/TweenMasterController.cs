using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//using NaughtyAttributes;

public class TweenMasterController : MonoBehaviour
{

    private float m_TweenTimeScale = 1f;

    public float TweenTimeScale 
    { 
        get { return m_TweenTimeScale; }
        set 
        {
            if (m_TweenTimeScale == value) return;
            m_TweenTimeScale = value;
            //if (OnVariableChange != null)
            //    OnVariableChange(m_TweenTimeScale);
        }
    }
    public delegate void OnVariableChangeDelegate(float newVal);
    public event OnVariableChangeDelegate OnVariableChange;


    public void UpdateTweenTimescale(float timescale)
    {
        TweenTimeScale = timescale;
        SetTweenTimescale();
    }

    private void SetTweenTimescale()
    {
        DOTween.timeScale = TweenTimeScale;
    }

}
