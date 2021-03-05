using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MaterialReplacer : MonoBehaviour
{
    public string m_MaterialName_OLD;
    public Material m_Material_NEW;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void ReplaceMaterials()
    {
        foreach(Transform t in transform)
        {
            Renderer rend = t.GetComponent<Renderer>();
            if(rend.sharedMaterial.name.Contains(m_MaterialName_OLD))
            {
                rend.sharedMaterial = m_Material_NEW;
            }
        }
    }
}
