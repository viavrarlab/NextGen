//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: This object won't be destroyed when a new scene is loaded
//
//=============================================================================

using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class DontDestroyOnLoad : MonoBehaviour
	{
		//-------------------------------------------------
		void Awake()
		{
			GameObject[] Obj = GameObject.FindGameObjectsWithTag("VRRIG");
 			if(Obj.Length > 1)
            {
				Destroy(this.gameObject);
            }
			DontDestroyOnLoad( this );
		}
	}
}
