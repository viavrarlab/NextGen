using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlacementPointHighlight : MonoBehaviour
{
    public MeshRenderer m_MeshRenderer;
    private MaterialPropertyBlock m_MaterialPropertyBlock;

    void Start()
    {
        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        UpdateMaterial(0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        FadeAlphaTo(1f);
    }

    private void OnTriggerExit(Collider other)
    {
        FadeAlphaTo(0f);
    }

    void FadeAlphaTo(float to)
    {
        float val = 1f - to;
        DOTween.To(() => val, x => val = x, to, 0.5f).OnUpdate(() =>
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
}
