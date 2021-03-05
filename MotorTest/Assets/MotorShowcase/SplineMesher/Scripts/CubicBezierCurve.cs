using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMesher {
    public class CubicBezierCurve {
        public static List<Vector3> ComputeCurve(List<Vector3> knotList, List<Vector3> controllers, int nodeCount) {
            List<Vector3> curveNodes = new List<Vector3>();
            int ctrlCount = 0;
            for(int i = 0; i < knotList.Count - 1; i++) {
                for (int k = 0; k < nodeCount; k++) {
                    if (k == nodeCount - 1 && i != knotList.Count - 2) break;
                    float interpolation = (float)k / (nodeCount - 1);
                    Vector3 intPoint = CalculateCubicBezierPoint(interpolation, knotList[i], controllers[ctrlCount], controllers[ctrlCount + 1], knotList[i + 1]);
                    curveNodes.Add(intPoint);
                }
                ctrlCount += 2;
            }
            return curveNodes;
        }

        private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }
    }
}