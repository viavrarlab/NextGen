using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Linq;
using UnityEngine.Animations;

public class SystemGeneratorScript : MonoBehaviour
{
    public Material m_SocketMaterial;
    public List<MeshRenderer> m_MeshList;
    public CorrectOrderTests m_CorrOrder;
    public List<SocketPair> m_SocketPairs;
}

