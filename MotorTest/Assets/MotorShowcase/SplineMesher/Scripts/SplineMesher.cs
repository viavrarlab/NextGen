using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SplineMesher {
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(LineManager))]
    [ExecuteInEditMode]
    public class SplineMesher : MonoBehaviour {

        #region public parameters
        public bool isRectangle = true;
        public bool cap = true;
        public bool flip = false;
        
        public int sides = 4;
        public Vector2 rectSize = new Vector2(1.0f, 2.0f);
        public float tubeRadius = 1.0f;
        
        public bool centerPivot = true;
        public bool smooth = false;
        public bool cornerCorrection = false;
        public float rotate = 0.0f;
        public float twist = 0.0f;

        public AnimationCurve radiusAdCurve; // range: [0.0-1.0]
        public float curveMultiply = 1.0f;
        public bool onlyDisplaceTopVertex = false;

        public Material mat;

        [HideInInspector]
        public int UVmethod = 1;

        public Vector2 UVTile = Vector2.one;
        public Vector2 UVOffset = Vector2.zero;
        public Vector2 CapUVTile = new Vector2(0.0f, 1.0f);
        #endregion

        #region private parameters
        Mesh newMesh;
        MeshRenderer renderComp;
        MeshFilter meshFilterComp;
        public LineManager lineMgrComp;
        
        UnityAction methodDelegate;
        bool updateFlag = false;
        #endregion

        public void ManualUpdate() {
            OnValidate();
        }

        void Update() {
            if (updateFlag) {
                updateFlag = false;
                meshFilterComp.mesh = newMesh;
            }
        }

        void OnValidate() {
            ManualValidate();
        }

        void ManualValidate()
        {
            ParamConstraints();
            UpdateMesh();
        }

        void ParamConstraints() {
            if (lineMgrComp == null) lineMgrComp = this.GetComponent<LineManager>();
            if (meshFilterComp == null) meshFilterComp = this.GetComponent<MeshFilter>();
            if (renderComp == null) renderComp = this.GetComponent<MeshRenderer>();
            if (renderComp.sharedMaterial == null || renderComp.sharedMaterial != mat) renderComp.sharedMaterial = mat;

            if (radiusAdCurve == null) {
                radiusAdCurve = new AnimationCurve();
                radiusAdCurve.AddKey(0.0f, 0.0f);
                radiusAdCurve.AddKey(1.0f, 0.0f);
            }

            if (isRectangle) sides = 4;
            else if (sides < 3) sides = 3;
        }

        void UpdateMesh() {
            newMesh = new Mesh();
            List<Vector3> newVertex = new List<Vector3>();
            List<Vector3> newNormal = new List<Vector3>();
            List<Vector2> newUV = new List<Vector2>();
            List<int> newTriangles = new List<int>();

            CalcVertex(ref newVertex);
            CalcUV(ref newUV, newVertex);
            CalcTriangle(ref newTriangles);

            newMesh.SetVertices(newVertex);
            newMesh.SetUVs(0, newUV);
            newMesh.SetTriangles(newTriangles, 0);
            if (smooth) {
                CalcNormal(ref newNormal, ref newVertex);
                newMesh.SetNormals(newNormal);
            } else newMesh.RecalculateNormals();
            if (!centerPivot) {
                PivotAdjust(ref newVertex);
                newMesh.SetVertices(newVertex);
            }
            newMesh.RecalculateBounds();
            //newMesh.RecalculateTangents();

            newMesh.name = this.gameObject.name + "_SplineMesh";
            updateFlag = true;
        }

        void CalcNormal(ref List<Vector3> normalList, ref List<Vector3> newVertex) {
            List<Vector3> knotList = lineMgrComp.GetLineKnots();
            List<Vector3> tangentList = lineMgrComp.GetTangent();

            if (knotList == null) return;
            if (knotList.Count <= 1) return;
            int n = knotList.Count; int len = (2 * n - 2);
            for (int i = 0; i < sides * 2; i++) for (int j = 0; j < len; j++) {
                int index = i * len + j;
                normalList.Add((newVertex[index] - knotList[(j + 1) / 2]).normalized * (flip ? -1 : 1));
            }
            if (cap) {
                // bottom face
                for (int i = 0; i < sides; i++) normalList.Add(tangentList[0]);
                // top face
                for (int i = 0; i < sides; i++) normalList.Add(-tangentList[n - 1]);
            }
        }

        void CalcTriangle(ref List<int> triangleList) {
            if (lineMgrComp.GetLineKnots() == null) return;
            if (lineMgrComp.GetLineKnots().Count <= 1) return;
            int n = lineMgrComp.GetLineKnots().Count;
            int len = (2 * n - 2);
            for (int i = 0; i < sides * 2; i++) {
                if (i % 2 != 0) continue;
                int p = i * len;
                for (int j = 1; j <= len; j++) {
                    int num = p + j + len - 1;
                    if (j % 2 != 0) {
                        if (flip) {
                            triangleList.Add(p + j - 1);
                            triangleList.Add(p + j);
                            triangleList.Add(num);
                        } else {
                            triangleList.Add(p + j);
                            triangleList.Add(p + j - 1);
                            triangleList.Add(num);
                        }
                    } else {
                        if (flip) {
                            triangleList.Add(num - 1);
                            triangleList.Add(p + j - 1);
                            triangleList.Add(num);
                        } else {
                            triangleList.Add(p + j - 1);
                            triangleList.Add(num - 1);
                            triangleList.Add(num);
                        }
                    }
                }
            }
            if (cap) {
                // bottom face
                int bottomId = len * sides * 2;
                for (int i = 1; i < sides - 1; i++) {
                    triangleList.Add(bottomId);
                    triangleList.Add(bottomId + i);
                    triangleList.Add(bottomId + i + 1);
                }
                // top face
                bottomId += sides;
                for (int i = 1; i < sides - 1; i++) {
                    triangleList.Add(bottomId);
                    triangleList.Add(bottomId + i + 1);
                    triangleList.Add(bottomId + i);
                }
            }
        }
        
        void CalcUV(ref List<Vector2> UVList, List<Vector3> vertexList) {
            if (lineMgrComp.GetLineKnots() == null) return;
            if (lineMgrComp.GetLineKnots().Count <= 1) return;
            int len = lineMgrComp.GetLineKnots().Count * 2 - 2;

            if (UVmethod == 2) {
                for (int sideCon = 0; sideCon < sides * 2; sideCon++) {
                    float uv_x = (float)((sideCon + 1) / 2) / sides + UVOffset.x; float uv_y = 0.0f;
                    for (int i = 0; i < len; i++) {
                        int index = sideCon * len + i;
                        if (sideCon == 0) uv_y = (i % 2 == 0 ? 0.0f : Vector3.Distance(vertexList[index], vertexList[index - 1]) * UVTile.y) + UVOffset.y;
                        else if (sideCon % 2 == 0) {
                            UVList.Add(UVList[index - len]);
                            continue;
                        } else if (i % 2 == 0) {
                            int indexA = index - len; int indexB = indexA + 1;
                            float d1 = MathUtils.PointLineDistance(vertexList[indexA], vertexList[index], vertexList[index + 1]);
                            float d2 = Vector3.Distance(vertexList[indexA], vertexList[index]);
                            float theta = Mathf.Acos(d1 / d2);
                            float offset = Mathf.Sin(theta) * d2;
                            float angle = Vector3.Angle(vertexList[indexB] - vertexList[indexA], vertexList[index] - vertexList[indexA]);
                            int sign = angle <= 90.0f ? 1 : -1;
                            uv_y = UVList[indexA].y + offset * sign * UVTile.y;
                        } else {
                            uv_y = UVList[index - 1].y + Vector3.Distance(vertexList[index], vertexList[index - 1]) * UVTile.y;
                        }
                        UVList.Add(new Vector2(uv_x * UVTile.x, uv_y));
                    }
                }
            } else {
                for (int sideCon = 0; sideCon < sides * 2; sideCon++) {
                    float uv_x = (float)((sideCon + 1) / 2) / sides; float totDist = 0.0f;
                    for (int i = 0; i < len; i++) {
                        int index = sideCon * len + i;
                        float uv_y = UVmethod == 1 ? totDist : (float)i / (len - 1);
                        totDist += i % 2 != 0 ? 0 : Vector3.Distance(vertexList[index], vertexList[index + 1]);
                        UVList.Add(new Vector2(uv_x * UVTile.x, uv_y * UVTile.y) + UVOffset);
                    }
                }
            }

            List<Vector2> bottomFace = new List<Vector2>();
            float m_Theta = 2.0f * Mathf.PI / sides;
            float m_Radius = tubeRadius * 2.0f * CapUVTile.y;
            for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta) {
                float x = m_Radius * Mathf.Cos(theta + CapUVTile.x * Mathf.Deg2Rad);
                float z = m_Radius * Mathf.Sin(theta + CapUVTile.x * Mathf.Deg2Rad);
                bottomFace.Add(new Vector3(x, z));
            }

            if (cap) {
                // bottom face
                for (int i = 0; i < sides; i++) UVList.Add(bottomFace[i]);
                // top face
                for (int i = 0; i < sides; i++) UVList.Add(bottomFace[i]);
            }
        }

        void CalcVertex(ref List<Vector3> vertexList) {
            List<Vector3> knotList = lineMgrComp.GetLineKnots();
            if (knotList == null) return; if (knotList.Count <= 1) return;
            List<Vector3> normalList = lineMgrComp.GetNormal();
            List<Vector3> tangentList = lineMgrComp.GetTangent();
            List<Vector3> bitangentList = lineMgrComp.GetBitangent();

            // calc vertex of each edge
            List<Vector3> topVertex = new List<Vector3>();
            List<Vector3> bottomVertex = new List<Vector3>();
            Vector3 dirVec, startPoint, iPoint = Vector3.zero;
            float theta = 360.0f / sides;
            int edgeLen = knotList.Count * 2 - 2;
            for (int eg = 0; eg < sides; eg++) {
                if (isRectangle) startPoint = knotList[0] + (eg < 2 ? 1 : -1) * normalList[0] * rectSize.x +
                                 (eg > 0 && eg < 3 ? 1 : -1) * bitangentList[0] * rectSize.y;
                else startPoint = knotList[0] + normalList[0] * tubeRadius;
                startPoint = MathUtils.RotateAround(startPoint, knotList[0], tangentList[0], (isRectangle ? 0 : theta * eg) + rotate);

                float percent = 1.0f / knotList.Count;
                iPoint = startPoint + curveMultiply * radiusAdCurve.Evaluate(percent) * (!onlyDisplaceTopVertex ? (startPoint - knotList[0]).normalized :
                    bitangentList[0] * (Vector3.Angle(startPoint - knotList[0], bitangentList[0]) < 90.0f ? 1.0f : 0.0f) );
                iPoint = MathUtils.RotateAround(iPoint, knotList[1], tangentList[1], twist * percent);
                topVertex.Add(iPoint); vertexList.Add(iPoint);
                for (int i = 0; i < knotList.Count - 1; i++) {
                    dirVec = (knotList[i + 1] - knotList[i]).normalized;
                    for (int ti = i + 2; ti < knotList.Count && dirVec.magnitude == 0; ti++) dirVec = (knotList[ti] - knotList[i]).normalized;
                    iPoint = MathUtils.RayPlaneIntersect(startPoint, dirVec, tangentList[i + 1], knotList[i + 1] + normalList[i + 1]);
                    startPoint = iPoint;
                    // corner correction
                    if (cornerCorrection && i < knotList.Count - 2) {
                        Vector3 cornerVec = iPoint - knotList[i + 1];
                        Vector3 prevVec = knotList[i] - knotList[i + 1];
                        Vector3 nextVec = knotList[i + 2] - knotList[i + 1];
                        if (Vector3.Angle(prevVec, nextVec) < 90.0f)
                        if (cornerVec.magnitude > prevVec.magnitude && cornerVec.magnitude > nextVec.magnitude)
                            iPoint = knotList[i + 1] + (cornerVec).normalized * (prevVec.magnitude + nextVec.magnitude) / 2.0f;
                    }
                    // twist and radius ad
                    percent = (float)(i + 1) / knotList.Count;
                    iPoint = iPoint + curveMultiply * radiusAdCurve.Evaluate(percent) * (!onlyDisplaceTopVertex ? (iPoint - knotList[i + 1]).normalized : 
                        bitangentList[i + 1] * (Vector3.Angle(startPoint - knotList[i + 1], bitangentList[i + 1]) < 90.0f ? 1.0f : 0.0f) );
                    iPoint = MathUtils.RotateAround(iPoint, knotList[i + 1], tangentList[i + 1], twist * percent);
                    vertexList.Add(iPoint);
                    // push it once again to split normal
                    if (i != knotList.Count - 2) vertexList.Add(iPoint);
                }
                bottomVertex.Add(iPoint);
                int tempLen = vertexList.Count;
                for (int i = vertexList.Count - edgeLen; i < tempLen && eg > 0; i++)
                    vertexList.Add(vertexList[i]);
            }
            // push first edge once again
            for (int i = 0; i < edgeLen; i++) vertexList.Add(vertexList[i]);

            // add top & bottom vertex
            if (cap) {
                foreach (Vector3 vtx in bottomVertex) vertexList.Add(vtx);
                foreach (Vector3 vtx in topVertex) vertexList.Add(vtx);
            }
        }

        void PivotAdjust(ref List<Vector3> vertexList) {
            List<Vector3> bitangentList = lineMgrComp.GetBitangent();
            for (int i = 0; i < vertexList.Count; i++) vertexList[i] += bitangentList[0] * (isRectangle ? rectSize.y : tubeRadius);
        }
//#if UNITY_EDITOR

        void OnRenderObject() {
            if (lineMgrComp.HasChanged()) ManualValidate();
        }
//#endif
    }
}
