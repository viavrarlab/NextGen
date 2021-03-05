using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialTweener : MonoBehaviour
{
    public string m_MaterialProperty = "_OffsetY";
    public int m_OffsetDirection = 1;
    public float m_TweenLength = 30f;
    public int m_LoopAmount = 1;

    
    public List<TweenProperties> m_TweenPropertyList;

    private bool m_Enabled = false;

    [SerializeField]
    private float m_Time = 0f;
    private int m_LoopsDone = 0;

    private Renderer m_Renderer;
    private MaterialPropertyBlock m_PropBlock;

    private List<Tween> m_TweenList = new List<Tween>();

    // Start is called before the first frame update
    void Start()
    {
        m_PropBlock = new MaterialPropertyBlock();
        m_Renderer = GetComponent<Renderer>();
        m_Renderer.GetPropertyBlock(m_PropBlock);
        m_PropBlock.SetFloat(m_MaterialProperty, 0f);
        m_Renderer.SetPropertyBlock(m_PropBlock);

        //Tween();
    }

    public void Setup(List<TweenProperties> properties)
    {
        m_TweenPropertyList = properties;
    }

    private void Tween(Color ColWhite, Color ColBlack)
    {
        //print("Tween start called");

        m_PropBlock.SetColor("_Color_White", ColWhite);
        m_PropBlock.SetColor("_Color_Black", ColBlack);
        

        foreach (TweenProperties tp in m_TweenPropertyList)
        {
            m_PropBlock.SetFloat(tp.m_MaterialProperty, 0f);
            float val = m_PropBlock.GetFloat(tp.m_MaterialProperty);

            Tween tween;

            tween = DOTween.To(() => val, x => val = x, tp.m_OffsetDirection * 100f, tp.m_TweenLength).SetAutoKill(false);
            tween.SetEase(Ease.Linear);
            tween.OnUpdate(() =>
            {
                m_PropBlock.SetFloat(tp.m_MaterialProperty, val);
                m_Renderer.SetPropertyBlock(m_PropBlock);
            });
            tween.OnComplete(() =>
            {
                tween.Restart();
            });

            m_TweenList.Add(tween);
        }




        //seq.Append(DOTween.To(() => val, x => val = x, m_OffsetDirection * 100f, m_TweenLength).SetEase(Ease.Linear).SetLoops(m_LoopAmount).OnUpdate(() =>
        //{
        //    m_PropBlock.SetFloat(m_MaterialProperty, val);
        //    m_Renderer.SetPropertyBlock(m_PropBlock);
        //}));

        
    }

    public void StopTween()
    {

        //print("Stopping tween");
        foreach(Tween t in m_TweenList)
        {
            t.OnComplete(null);
            t.SetAutoKill(true);
            t.Complete();
        }

    }

    public void StartTween(Color ColWhite, Color ColBlack)
    {
        Tween(ColWhite, ColBlack);
    }

    public void SetDisabledColor(Color _color)
    {
        m_Renderer.GetPropertyBlock(m_PropBlock);
        m_PropBlock.SetColor("_Color_Black", _color);
        m_PropBlock.SetColor("_Color_White", _color);
        m_Renderer.SetPropertyBlock(m_PropBlock);
    }
}
