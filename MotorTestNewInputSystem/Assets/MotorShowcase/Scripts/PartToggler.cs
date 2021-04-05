using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using ShowcaseV2;
//using NaughtyAttributes;

public class PartToggler : MonoBehaviour
{
    public string m_ShaderNormal_N = "Universal Render Pipeline/Lit";
    public string m_ShaderTransparent_N = "Shader Graphs/Fresnel";

    public GameObject m_UITogglePrefab;
    public Transform m_UIContentRoot_AllParts;
    //public Transform m_UIContentRoot_Groups;


    public Transform m_Target;
    //private List<Transform> m_TargetObjects = new List<Transform>();

    private ShowcaseControllerV2 m_ShowcaseController;
    private Shader m_ShaderNormal;
    private Shader m_ShaderTransparent;

    private List<Toggle> m_AllToggles = new List<Toggle>();

    private void Awake()
    {
        //foreach(Transform t in m_Target)
        //{
        //    m_TargetObjects.Add(t);
        //}

        m_ShowcaseController = GameObject.FindObjectOfType<ShowcaseControllerV2>();
        m_ShaderNormal = Shader.Find(m_ShaderNormal_N);
        m_ShaderTransparent = Shader.Find(m_ShaderTransparent_N);
        
    }

    void Start()
    {
        SetUpUI();
    }

    void Update()
    {
        
    }

    public void SetUpUI()
    {
        foreach(Transform obj in m_Target.transform/*m_ShowcaseController.m_AllObjects*/)
        {
            //Transform root = (obj.childCount > 1) ? m_UIContentRoot_Groups : m_UIContentRoot_AllParts;
            GameObject newObject = Instantiate(m_UITogglePrefab, m_UIContentRoot_AllParts);
            Toggle toggle = newObject.GetComponent<Toggle>();


            toggle.onValueChanged.AddListener(delegate {
                ToggleShader(obj.gameObject);
            });

            m_AllToggles.Add(toggle);


            TextMeshProUGUI text = newObject.GetComponentInChildren<TextMeshProUGUI>();
            string formattedText = obj.name.Replace("_", " ");
            text.text = formattedText;
            RawImage prev = newObject.transform.Find("preview").GetComponent<RawImage>();

            RuntimePreviewGenerator.OrthographicMode = false;
            RuntimePreviewGenerator.BackgroundColor = new Color(1f, 1f, 1f, 0f);

            Texture prevImg = RuntimePreviewGenerator.GenerateModelPreviewWithShader(obj.transform, Shader.Find("Unlit/Texture"), string.Empty, 128, 128, true);
            prevImg.name = obj.gameObject.name + "_Thumb";

            prev.texture = prevImg;
        }
    }

    public void ToggleShader(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if(rend != null)
        {
            if(rend.material.shader == m_ShaderNormal)
            {
                rend.material.shader = m_ShaderTransparent;
            }
            else if (rend.material.shader == m_ShaderTransparent)
            {
                rend.material.shader = m_ShaderNormal;
            }
        }
    }

    //[Button]
    public void ToggleAll_On()
    {
        foreach(Toggle t in m_AllToggles)
        {
            //t.Select();
            t.isOn = true;
            //t.onValueChanged.Invoke(true);
        }
    }

    //[Button]
    public void ToggleAll_Off()
    {
        foreach (Toggle t in m_AllToggles)
        {
            //t.Select();
            t.isOn = false;
            //t.onValueChanged.Invoke(true);
        }
    }
}
