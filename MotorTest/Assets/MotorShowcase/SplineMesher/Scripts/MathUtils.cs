using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMesher {
    public class MathUtils {
        /// <summary>
        /// Computing normal, tangent and bitangent for input spline knots
        /// </summary>
        public static void CalcVectors(List<Vector3> actKnotList, ref List<Vector3> normalList, ref List<Vector3> tangentList, ref List<Vector3> bitangentList) {
            Vector3 prev, next, tangent, normal, bitangent;

            if (actKnotList == null) return;
            if (actKnotList.Count <= 1) return;
            normalList.Clear(); tangentList.Clear(); bitangentList.Clear();
            for (int i = 0; i < actKnotList.Count; i++) {
                if (i == 0) prev = next = actKnotList[i + 1] - actKnotList[i];
                else if (i == actKnotList.Count - 1) next = prev = actKnotList[i] - actKnotList[i - 1];
                else {
                    prev = (actKnotList[i] - actKnotList[i - 1]).normalized;
                    next = (actKnotList[i + 1] - actKnotList[i]).normalized;
                }
                tangent = prev == -next ? next : (prev + next).normalized;
                for (int ti = i + 2; ti < actKnotList.Count && tangent.magnitude == 0; ti++) tangent = (actKnotList[ti] - actKnotList[i]).normalized;
                normal = Vector3.Cross(tangent == Vector3.up || tangent == Vector3.down ? new Vector3(0.0f, 1.0f, 0.01f) : Vector3.up, tangent).normalized;
                bitangent = Vector3.Cross(tangent, normal);

                normalList.Add(normal); tangentList.Add(tangent); bitangentList.Add(bitangent);
            }
        }

        /// <summary>
        /// Computing the intersection of ray and plane
        /// </summary>
        public static Vector3 RayPlaneIntersect(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint) {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct, planeNormal);
            return (d * direct + point);
        }

        /// <summary>
        /// Calculate the distance between a point and a straight line
        /// </summary>
        public static float PointLineDistance(Vector3 point, Vector3 linePoint1, Vector3 linePoint2) {
            float fProj = Vector3.Dot(point - linePoint1, (linePoint1 - linePoint2).normalized);
            return Mathf.Sqrt((point - linePoint1).sqrMagnitude - fProj * fProj);
        }

        /// <summary>
        /// Rotate a given point around a specified axis
        /// </summary>
        public static Vector3 RotateAround(Vector3 position, Vector3 center, Vector3 axis, float angle) {
            Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
            Vector3 resultVec3 = center + point;
            return resultVec3;
        }
    }
}