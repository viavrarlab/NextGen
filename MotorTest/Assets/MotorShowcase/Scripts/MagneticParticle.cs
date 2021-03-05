using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NaughtyAttributes;

public class MagneticParticle : MonoBehaviour
{
    public int m_Charge = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(MagneticFieldManager.instance.m_PositiveFieldTag))
        {

        }
        else if (other.CompareTag(MagneticFieldManager.instance.m_NegativeFieldTag))
        {

        }
    }
}
