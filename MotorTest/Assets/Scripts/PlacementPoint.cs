using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Valve.VR;


/// <summary>
/// This could be automated when creating armature/object IDs - to each ID'd object we add this and configure it accordingly (everything, including mesh, its transforms, trigger bounds, etc)
/// </summary>
public class PlacementPoint : MonoBehaviour
{
    public int m_PlaceableID;   // This must correspond to the Placeable objects' ID that you want to place here
    public Vector3 m_CorrectPlacementAngle = Vector3.zero; // TODO: make this automated somehow. Maybe when generating placement points / ObjectConfig.. IDK
    public bool m_IsOccupied = false; // If this socket/point has something socketed 
    public float m_CorrectAngleThreshold = 5f; // How precise does the Placeable objects' rotation has to be in order to allow snapping into socket. Used in CheckAngle(). Measures as angle degrees.

    
    public Color m_ValidObjectColor = Color.green;
    public Color m_InvalidObjectColor = Color.red;
    public Color m_ValidRotationColor = Color.cyan;
    public Color m_InvalidRotationColor = Color.yellow;


    private MeshRenderer m_MeshRenderer; // PlacementPoints' graphics. For now they are on the same object and the MR is assigned automatically
    private MaterialPropertyBlock m_MaterialPropertyBlock; // Allows to use a single material, but be able to change each objects' parameters separately (ex., Object1 - blue, Object2 - orange)

    private Placeable m_SnappableObject;    // The object that is currently available to snap and/or is snapped.

    private GrabTests m_GrabScript; // TODO:  MAKE A UNIFIED GRAB SCRIPT THAT IS ADDED TO THE SCENE ONCE!

    void Start()
    {
        m_GrabScript = GameObject.FindObjectOfType<GrabTests>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_MaterialPropertyBlock = new MaterialPropertyBlock();

        UpdateMaterial(0f); // Hide the graphics
        UpdateMaterial(m_ValidObjectColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the socket is not occupied, allow the entered Placeable object to be processed.
        if (!m_IsOccupied)
        {
            Placeable p = other.gameObject.GetComponent<Placeable>(); // Get the Placeable component on incoming object.
            if (p != null)
            {
                FadeAlphaTo(1f);

                if (p.m_ID == m_PlaceableID) // Check if the incoming Placeable objects ID is what this socket wants
                {
                    if (m_SnappableObject != p) // Kind of redundant, but whatever. Just setting the member variable to incoming objects Placeable script reference.
                    {
                        m_SnappableObject = p;
                    }
                    //m_IsOccupied = true;
                    UpdateMaterial(m_ValidObjectColor);
                }
                else
                {
                    UpdateMaterial(m_InvalidObjectColor);
                }
            }
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_SnappableObject != null && !m_SnappableObject.m_IsPlaced) { // If we have a valid Placeable object and it isn't already placed
            if (CheckAngle(m_SnappableObject.transform.rotation))   // Check if user has rotated the object correctly
            {
                UpdateMaterial(m_ValidRotationColor);   // change color to valid
                if (m_GrabScript.grabAction.GetLastStateUp(m_GrabScript.handType) && !m_IsOccupied) // If user has released the grab button and the socket is not already occupied
                {
                    SnapObject();   // snap the object to the socket
                    UpdateMaterial(m_ValidObjectColor);
                }
            }
            else
            {
                UpdateMaterial(m_InvalidRotationColor); // change color to invalid
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_IsOccupied)
        {
            if(other.gameObject != m_SnappableObject.gameObject)
            {
                return;
            }
            m_SnappableObject.m_IsPlaced = false;
            m_IsOccupied = false;
            m_SnappableObject = null;
        }
        if (GetMaterialAlpha() > 0f)
        {
            FadeAlphaTo(0f);
        }
    }

    void SnapObject()
    {
        m_SnappableObject.transform.position = transform.position;
        m_SnappableObject.transform.rotation = Quaternion.Euler(m_CorrectPlacementAngle);
        m_SnappableObject.m_IsPlaced = true;
        m_IsOccupied = true;
        FadeAlphaTo(0f);
    }

    void FadeAlphaTo(float to)
    {
        float val = 1f - to;
        DOTween.To(() => val, x => val = x, to, 0.25f).OnUpdate(() =>
        {
            UpdateMaterial(val);
        });
    }

    void UpdateMaterial(float alphaValue)
    {
        if(m_MeshRenderer == null)
        {
            Debug.LogWarning("No MeshRenderer component assigned.");
            return;
        }
        m_MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
        m_MaterialPropertyBlock.SetFloat("_AlphaValue", alphaValue);
        m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }
    void UpdateMaterial(Color _col)
    {
        if (m_MeshRenderer == null)
        {
            Debug.LogWarning("No MeshRenderer component assigned.");
            return;
        }
        m_MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
        m_MaterialPropertyBlock.SetColor("_Color", _col);
        m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

    float GetMaterialAlpha()
    {
        if (m_MeshRenderer == null)
        {
            Debug.LogWarning("No MeshRenderer component assigned.");
            return 0f;
        }
        m_MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
        return m_MaterialPropertyBlock.GetFloat("_AlphaValue");
    }

    bool CheckAngle(Quaternion _incomingRotation)
    {
        Quaternion angleA = Quaternion.Euler(m_CorrectPlacementAngle);  // Convert Euler angles to Quaternion for the correct/expected rotation and store it in a variable.
        Quaternion angleB = _incomingRotation;  // Store incoming Placeable objects' rotation in variable

        float angle = Quaternion.Angle(angleA, angleB); // Calculate difference between the 2 angles

        bool isSameRotation = Mathf.Abs(angle) < m_CorrectAngleThreshold;   // If the difference is less than a threshold then we accept that it's the same rotation
        return isSameRotation;
    }
}
