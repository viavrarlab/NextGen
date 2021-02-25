using ModelOutline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Placeable : MonoBehaviour
{
    public int m_ID = 0;

    public bool isHeld;
    public bool CanTakeOut;
    [SerializeField]
    private bool M_isPlaced;

    public bool m_IsPlaced
    {
        get { return M_isPlaced; }
        set
        {
            if (M_isPlaced == value) return;
            M_isPlaced = value;
            if (M_isPlaced)
            {
                GradingController.Instance.ObjectPlaced(gameObject, m_ID);
                GradingController.Instance.ShowHintOutline(gameObject, m_ID);
            }
            else
            {
                GradingController.Instance.ObjectRemoved(gameObject);
            }
        }
    }

    public Placeable(int _id)
    {
        m_ID = _id;
    }
    public static Placeable CreateComponentReturn(GameObject _where, int _id)
    {
        Placeable myC = _where.AddComponent<Placeable>();
        myC.m_ID = _id;
        return myC;
    }
    public static void CreateComponent(GameObject _where, int _id)
    {
        Placeable myC = _where.AddComponent<Placeable>();
        myC.m_ID = _id;
    }
}
