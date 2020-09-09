using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoxLabel : MonoBehaviour
{
    public TextMeshProUGUI m_Text;
    public string m_Label = "Tukšs";

    void Start()
    {
        if (m_Text.text != m_Label)
        {
            UpdateText();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateText()
    {
        if (m_Text != null)
        {
            m_Text.text = m_Label;
        }
    }

    private void OnValidate()
    {

        UpdateText();
    }
}
