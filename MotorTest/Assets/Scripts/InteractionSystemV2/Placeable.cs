using ModelOutline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Placeable : MonoBehaviour
{
    public int m_ID = 0;
    public bool m_IsPlaced = false;
    public bool isHeld;
    public bool CanTakeOut;
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
