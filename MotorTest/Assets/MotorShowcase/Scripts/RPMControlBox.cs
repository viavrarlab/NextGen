using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;

public class RPMControlBox : MonoBehaviour
{
    public Transform m_Target;
    public List<Transform> m_AdditionalTargets;

    public Slider m_Slider;

    public float m_CurrentMaxRPM = 30f;
    [ReadOnly]
    public float m_CurrentRPM = 0f;

    public float m_MaxMotorRPM = 1500f;

    public float m_StopStartTime = 0.05f;

    public Vector3 m_RotationAxis = Vector3.zero;

    public bool m_CanBeTurnedOn = false;
    public bool m_IsOn = false;

    private float velocity = 0f;

    [HorizontalLine(color: EColor.Blue)]
    public AudioSource m_AudioSource;
    public float m_MinPitch = 0.75f;
    public float m_MaxPitch = 3f;
    private float m_PitchModifier = 1f;

    private bool turnOnTweenInProgress = false;
    private bool turnOffTweenInProgress = false;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_PitchModifier = m_MaxPitch - m_MinPitch;
        m_Slider.maxValue = m_MaxMotorRPM;
    }

    public void ToggleElectricityConnection(bool value)
    {
        m_CanBeTurnedOn = value;
    }

    public void TurnOn()
    {
        if (m_CanBeTurnedOn)
        {
            if (m_IsOn == false)
            {
                m_IsOn = true;               
            }
        }
    }

    public void TurnOff()
    {
        if (m_IsOn == true)
        {
            m_IsOn = false;
        }
    }

    void Update()
    {
        if(m_CanBeTurnedOn == false)
        {
            // TODO: Turn off electricity more gracefully - 

            //return;
        }
        if (m_IsOn)
        {
            m_CurrentRPM = Mathf.SmoothDamp(m_CurrentRPM, m_CurrentMaxRPM, ref velocity, m_StopStartTime);
            if(m_AudioSource != null && m_AudioSource.isPlaying == false && turnOnTweenInProgress == false)
            {
                m_AudioSource.DOFade(1f, 0.75f).OnStart(()=> { turnOnTweenInProgress = true; m_AudioSource.Play(); }).OnComplete(() =>
                {
                    
                    turnOnTweenInProgress = false;
                });
            }
            //if (m_CurrentRPM < m_CurrentMaxRPM)
            //{
            //    //m_CurrentRPM = Mathf.Lerp(m_CurrentRPM, m_MaxRPM, Time.deltaTime);
                
            //}
            
        }
        else
        {
            if(m_CurrentRPM > 0f)
            {
                m_CurrentRPM = Mathf.SmoothDamp(m_CurrentRPM, 0f, ref velocity, m_StopStartTime);
                if(m_CurrentRPM <= 0.1f)
                {
                    m_CurrentRPM = 0f;
                    if (m_AudioSource != null && m_AudioSource.isPlaying == true && turnOffTweenInProgress == false)
                    {
                        m_AudioSource.DOFade(0f, 0.75f).OnStart(()=> { turnOffTweenInProgress = true; }).OnComplete(() =>
                        {
                            m_AudioSource.Stop();
                            turnOffTweenInProgress = false;
                        });
                        
                    }
                }
            }
        }
        if (m_CurrentRPM > 0f && m_IsOn)
        {
            m_Target.localRotation *= Quaternion.AngleAxis((m_CurrentRPM * 6f) * Time.deltaTime, m_RotationAxis);
            foreach(Transform t in m_AdditionalTargets)
            {
                //t.localRotation *= Quaternion.AngleAxis((m_CurrentRPM * 6f) * Time.deltaTime, m_RotationAxis);
                t.RotateAround(m_Target.position, m_RotationAxis, (m_CurrentRPM * 6f) * Time.deltaTime);
            }
        }
        if (m_AudioSource != null)
        {
            //m_AudioSource.pitch = m_MinPitch + (m_CurrentRPM / m_CurrentMaxRPM) * m_PitchModifier;
            float pitch = (m_CurrentRPM > 0.1f) ? m_CurrentRPM.Remap(0f, m_MaxMotorRPM, m_MinPitch, m_MaxPitch) : 0f;
            m_AudioSource.pitch = pitch;
        }
    }

    public void SetRPMFromUI(float value)
    {
        m_CurrentMaxRPM = value * 100f;
    }

    public void SetRPM(float value)
    {
        m_CurrentMaxRPM = value;
    }
}
