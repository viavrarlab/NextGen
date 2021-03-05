using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CrossSectionPlane : MonoBehaviour
{
    public float m_PositionLimit_MIN = 0f;
    public float m_PositionLimit_MAX = 1f;

    public Vector3 m_PlanePosition = Vector3.zero;
    public Vector3 m_PlaneNormal = Vector3.zero;
    
    //public Material[] m_Materials;

    //public List<Renderer> m_AllRenderers;
    public List<Material> m_Materials;

    void Start()
    {
        foreach(Transform t1 in transform)
        {
            foreach(Transform t2 in t1.transform)
            {
                Renderer r = t2.GetComponent<Renderer>();
                if (!m_Materials.Contains(r.sharedMaterial))
                {
                    m_Materials.Add(r.sharedMaterial);
                }
            }
        }

        UpdatePlanePosition(1f);
    }

    public void UpdatePlanePosition(float value)
    {
        m_PlanePosition = m_PlaneNormal * value.Remap(0f, 1f, m_PositionLimit_MIN, m_PositionLimit_MAX);

        UpdateMaterials();
    }

    void UpdateMaterials()
    {
        foreach (Material mat in m_Materials)
        {
            mat.SetVector("_PlanePosition", transform.TransformPoint(m_PlanePosition));
            mat.SetVector("_PlaneNormal", m_PlaneNormal);
        }
    }
}
