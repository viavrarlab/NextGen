using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SplineMesher {
    [CustomEditor(typeof(LineManager))]
    public class SceneGUIBezierInspector : Editor {
        public override void OnInspectorGUI() {
            LineManager mainComp = (LineManager)target;
            if (GUILayout.Button("+ knot")) {
                mainComp.knotNum++;
                mainComp.ManualUpdate();
            }
            if (GUILayout.Button("- knot")) {
                mainComp.knotNum--;
                mainComp.ManualUpdate();
            }
            base.OnInspectorGUI();
        }

        void OnSceneGUI() {
            var script = (LineManager)target;

            // draw handles ---------------------------------------------------------------------------------------------
            Handles.DrawWireArc(script.transform.position, script.transform.up, script.transform.forward, 360.0f, script.transform.lossyScale.x);
            Matrix4x4 transMatrix = Matrix4x4.TRS(script.transform.position, script.transform.rotation, script.transform.lossyScale);
            Matrix4x4 inverseMatrix = Matrix4x4.Inverse(transMatrix);

            // draw knots ------------------------------------------------------------------------------------------------
            int len = script.knotList.Count;
            for (int i = 0, controllerCount = 0; i < len; i++) {
                if (script.editSpline) {
                    Vector3 viewPos = transMatrix.MultiplyPoint3x4(script.knotList[i]);
                    Vector3 newPos = Handles.PositionHandle(viewPos, Quaternion.identity);
                    newPos = inverseMatrix.MultiplyPoint3x4(newPos);
                    if (newPos != script.knotList[i]) {
                        // record undo action
                        Undo.RecordObject(script, "changeKnotPos");
                        Vector3 deltaPos = newPos - script.knotList[i];
                        // update knots in script
                        if (i == 0 || i == len - 1) {
                            script.controllerList[controllerCount] += deltaPos;
                        } else {
                            script.controllerList[controllerCount] += deltaPos;
                            script.controllerList[controllerCount + 1] += deltaPos;
                        }
                        script.knotList[i] = newPos;
                        script.ManualUpdate_EditorDataTransfer(i, deltaPos);
                    }
                    if (i == 0) controllerCount++;
                    else controllerCount += 2;
                }

                if (i != len - 1 && !script.bezierCurve)
                    Handles.DrawLine(transMatrix.MultiplyPoint3x4(script.knotList[i]), transMatrix.MultiplyPoint3x4(script.knotList[i + 1]));
            }

            // draw bezier controllers -----------------------------------------------------------------------------------
            if (script.bezierCurve) {
                List<Vector3> curveKnots = script.GetLineKnots();
                len = curveKnots.Count;
                Handles.color = Color.yellow;
                for (int i = 0; i < len; i++) if (i != len - 1)
                    Handles.DrawLine(transMatrix.MultiplyPoint3x4(curveKnots[i]), transMatrix.MultiplyPoint3x4(curveKnots[i + 1]));
                
                len = script.controllerList.Count;
                Handles.color = Color.red;
                Handles.DrawLine(transMatrix.MultiplyPoint3x4(script.knotList[0]), transMatrix.MultiplyPoint3x4(script.controllerList[0]));
                for (int i = 1; i < len - 1; i += 2) {
                    int knotID = Mathf.CeilToInt((float)i / 2);
                    Handles.DrawLine(transMatrix.MultiplyPoint3x4(script.knotList[knotID]), transMatrix.MultiplyPoint3x4(script.controllerList[i]));
                    Handles.DrawLine(transMatrix.MultiplyPoint3x4(script.knotList[knotID]), transMatrix.MultiplyPoint3x4(script.controllerList[i + 1]));
                }
                Handles.DrawLine(transMatrix.MultiplyPoint3x4(script.knotList[script.knotList.Count - 1]), transMatrix.MultiplyPoint3x4(script.controllerList[script.controllerList.Count - 1]));

                if (script.editController) {
                    for (int i = 0; i < len; i ++) {
                        Vector3 viewPos = transMatrix.MultiplyPoint3x4(script.controllerList[i]);
                        Vector3 newPos = Handles.PositionHandle(viewPos, Quaternion.identity);
                        newPos = inverseMatrix.MultiplyPoint3x4(newPos);
                        if (newPos != script.controllerList[i]) {
                            // record undo action
                            Undo.RecordObject(script, "changeBezierConPos");
                            script.controllerList[i] = newPos;
                            // controller constraint
                            if (script.controllerConstraint) {
                                if(i != 0 && i != (len - 1)) {
                                    int counterpartID = (i % 2 == 0) ? (i - 1) : (i + 1);
                                    int knotID = Mathf.CeilToInt((float)i / 2);
                                    Vector3 dirVec = (script.knotList[knotID] - script.controllerList[i]);
                                    script.controllerList[counterpartID] = script.knotList[knotID] + dirVec;
                                }
                            }
                            script.ManualUpdate();
                        }
                    }
                }
            }

        }
    }
}