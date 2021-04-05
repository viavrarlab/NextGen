using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIValueToTextUI : MonoBehaviour
{
    public TextMeshProUGUI m_Text;
    public float m_InputMultiplier = 0f;

    public string m_StartingText = "";
    public string m_Affix = "";
    public string m_Suffix = "";

    public enum DecimalPlaces { F0, F1,F2,F3}
    public DecimalPlaces m_DecimalPlaces = DecimalPlaces.F0;

    void Start()
    {
        m_Text.text = $"{m_Affix} {m_StartingText} {m_Suffix}";
    }

    public void UpdateTextFromSlider(float input)
    {

        m_Text.text = m_Affix + (input * m_InputMultiplier).ToString(m_DecimalPlaces.ToString()) + m_Suffix;
    }
}
