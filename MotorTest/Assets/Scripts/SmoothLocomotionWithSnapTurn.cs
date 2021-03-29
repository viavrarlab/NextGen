using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class SmoothLocomotionWithSnapTurn : MonoBehaviour
{
    public HandRole m_Hand = new HandRole ();
    [Space (10f)]
    public Transform m_RigTransform;
    public Transform m_CameraTransform;
    [Space (10f)]
    public bool m_EnableMovement = true;
    public float m_MoveSpeed = 10f;
    public bool m_InvertDirection = false;
    [Space (10f)]
    public bool m_EnableSnapTurn = false;
    public float m_TurnThreshold = 0.9f;
    public float m_TurnResetThreshold = 0.1f;
    public float m_TurnAngle = 45f;
    [Space (10f)]
    public float m_CharacterRadius = 0.1f;
    public float m_CharacterHeight = 2f;

    public float m_DistanceTravelled = 0f;

    private bool m_HasTurned = false;
    private CharacterController m_CharController;

    private Vector3 m_LastPosition = Vector3.zero;

    private void Start ()
    {
        m_CharController = (m_RigTransform.gameObject.GetComponent<CharacterController> () != null) ? m_RigTransform.gameObject.GetComponent<CharacterController> () : m_RigTransform.gameObject.AddComponent<CharacterController> ();
        m_LastPosition = m_RigTransform.position;
        SetUpCharacterController ();
    }

    void SetUpCharacterController ()
    {
        if (m_CharController != null)
        {
            m_CharController.radius = m_CharacterRadius;
            m_CharController.height = m_CharacterHeight;
            m_CharController.center = new Vector3 (0f, (m_CharController.height / 2f), 0f);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        Vector2 input = ViveInput.GetPadAxis (m_Hand);

        if (m_EnableMovement)
        {
            Move (input);
        }

        if (m_EnableSnapTurn)
        {
            SnapTurn (input);
            if (Mathf.Abs (input.x) < m_TurnResetThreshold && m_HasTurned)
            {
                m_HasTurned = false;
            }
        }
    }

    private void Move (Vector2 input)
    {
        Vector3 rawDirection = new Vector3 (input.x, 0f, input.y);
        Vector3 adjustedDirection = m_CameraTransform.TransformDirection (rawDirection);
        adjustedDirection.y = 0f;

        float directionInvert = m_InvertDirection ? -1f : 1f;
        m_CharController.SimpleMove ((adjustedDirection * directionInvert) * m_MoveSpeed * Time.deltaTime);
        // m_CharController.Move((adjustedDirection * directionInvert) * m_MoveSpeed * Time.deltaTime);
        
        CalculateTraveledDistance();
    }

    private void SnapTurn (Vector2 input)
    {
        if (Mathf.Abs (input.x) > m_TurnThreshold && m_HasTurned == false)
        {
            m_RigTransform.Rotate (input.x * m_RigTransform.up * m_TurnAngle);
            m_HasTurned = true;
        }
    }

    private void CalculateTraveledDistance ()
    {
        m_DistanceTravelled += Vector3.Distance(m_RigTransform.position, m_LastPosition);
        m_LastPosition = m_RigTransform.position;
    }
}