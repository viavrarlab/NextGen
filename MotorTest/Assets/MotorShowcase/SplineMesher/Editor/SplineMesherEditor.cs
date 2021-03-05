using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SplineMesher {
    [CustomEditor(typeof(SplineMesher))]
    public class SplineMesherEditor : Editor {
        int oldMethod = 1;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            SplineMesher mainComp = (SplineMesher)target;
            
            string[] typeOptions = new string[] { "simple01", "edge vertex distance", "projection" };
            mainComp.UVmethod = EditorGUILayout.Popup("UV Tile Method", mainComp.UVmethod, typeOptions);
            if (oldMethod != mainComp.UVmethod) {
                oldMethod = mainComp.UVmethod;
                mainComp.ManualUpdate();
            }
        }

    }
}