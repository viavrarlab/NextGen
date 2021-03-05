using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SplineMesher {
    [ExecuteInEditMode]
    public class LineManager : MonoBehaviour {

        #region public parameters
        public bool editSpline = false;
        public bool editController = false;
        public int knotNum = 2;
        public bool bezierCurve = false;
        public bool controllerConstraint = true;
        public int smooth = 20;

        public List<Vector3> knotList = new List<Vector3>();
        public List<Vector3> controllerList = new List<Vector3>();
        #endregion

        #region private parameters
        private List<Vector3> bezierKnots = null;
        private List<Vector3> actual_bezierKnots  = new List<Vector3>();
        private List<Vector3> actual_knotList = new List<Vector3>();

        private List<Vector3> normalList = new List<Vector3>();
        private List<Vector3> tangentList = new List<Vector3>();
        private List<Vector3> bitangentList = new List<Vector3>();

        private bool changeFlag = false;
        private int lastValidKnotID = -1;
        private Vector3 deltaPos = Vector3.zero;
        #endregion

        public bool HasChanged() {
            bool flag = changeFlag;
            changeFlag = false;
            return flag;
        }

        public List<Vector3> GetActualLineKnots() {
            if (actual_bezierKnots.Count < 1 || actual_knotList.Count < 1) CalcActualKnots();
            if (bezierCurve) return actual_bezierKnots;
            else return actual_knotList;
        }
        public List<Vector3> GetLineKnots() {
            if (bezierCurve) return bezierKnots;
            else return knotList;
        }
        private void CalcActualKnots() {
            actual_bezierKnots.Clear();
            actual_knotList.Clear();

            Matrix4x4 transMatrix1 = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            if (bezierCurve) for (int i = bezierKnots.Count - 1; i >= 0; i--) actual_bezierKnots.Add(transMatrix1.MultiplyPoint3x4(bezierKnots[i]));
            else for (int i = knotList.Count - 1; i >= 0; i--) actual_knotList.Add(transMatrix1.MultiplyPoint3x4(knotList[i]));
        }

        public List<Vector3> GetNormal() {
            if (normalList.Count < 1) MathUtils.CalcVectors(GetLineKnots(), ref normalList, ref tangentList, ref bitangentList);
            return normalList;
        }
        public List<Vector3> GetTangent() {
            if (tangentList.Count < 1) MathUtils.CalcVectors(GetLineKnots(), ref normalList, ref tangentList, ref bitangentList);
            return tangentList;
        }
        public List<Vector3> GetBitangent() {
            if (bitangentList.Count < 1) MathUtils.CalcVectors(GetLineKnots(), ref normalList, ref tangentList, ref bitangentList);
            return bitangentList;
        }

        private void UpdateControllerList() {
            int desiredNum;
            if (knotList.Count == 2) desiredNum = 2;
            else desiredNum = knotList.Count * 2 - 2;

            int actualNum = controllerList.Count;
            if (actualNum == desiredNum) return;
            else {
                if (actualNum > desiredNum) {
                    int delNum = actualNum - desiredNum;
                    controllerList.RemoveRange(desiredNum, delNum);
                } else if(desiredNum > actualNum) {
                    int addNum = desiredNum - actualNum;
                    int startID = knotList.Count - 1;
                    int endID = startID;
                    addNum--;
                    while (addNum > 0) {
                        addNum -= 2;
                        startID--;
                    }
                    Vector3 newCon;
                    newCon = knotList[startID] + Vector3.right;
                    controllerList.Add(newCon);
                    for(startID += 1; startID < endID; startID++) {
                        newCon = knotList[startID] + Vector3.left;
                        controllerList.Add(newCon);
                        newCon = knotList[startID] + Vector3.right;
                        controllerList.Add(newCon);
                    }
                    newCon = knotList[endID] + Vector3.left;
                    controllerList.Add(newCon);
                }
            }
        }

        public void ManualUpdate() { /*OnValidate();*/ ManualUpdateValidate(); }
        public void ManualUpdate_EditorDataTransfer(int knotID, Vector3 deltaPos) {
            lastValidKnotID = knotID;
            this.deltaPos = deltaPos;
            OnValidate();
        }
        void OnValidate() {
            ManualUpdateValidate();
        }

        void ManualUpdateValidate()
        {
            ParamConstraints();

            if (knotList.Count > knotNum)
            {
                int delNum = knotList.Count - knotNum;
                knotList.RemoveRange(knotNum, delNum);
                UpdateControllerList();
            }
            else if (knotList.Count < knotNum)
            {
                int addNum = knotNum - knotList.Count;
                while (addNum-- > 0)
                {
                    int len = knotList.Count - 1;
                    if (len <= 0) return;
                    Vector3 dir = knotList[len] - knotList[len - 1];
                    knotList.Add(knotList[len] + dir);
                }
                UpdateControllerList();
            }

            if (bezierCurve) bezierKnots = CubicBezierCurve.ComputeCurve(knotList, controllerList, smooth + 1);

            CalcActualKnots();
            MathUtils.CalcVectors(GetLineKnots(), ref normalList, ref tangentList, ref bitangentList);
            changeFlag = true;
        }

        void ParamConstraints() {
            if (knotNum < 2) knotNum = 2;
            if (smooth < 1) smooth = 1;
            else if (smooth > 255) smooth = 255;

            if (knotList.Count < 1) {
                knotList.Add(Vector3.zero);
                knotList.Add(new Vector3(1.0f, 0.0f, 0.0f));

                UpdateControllerList();
            }
        }
        
        void OnDrawGizmosSelected() {
            if (transform.hasChanged) {
                CalcActualKnots();
                MathUtils.CalcVectors(GetLineKnots(), ref normalList, ref tangentList, ref bitangentList);
                changeFlag = true;
                transform.hasChanged = false;
            }

            // draw gizmos
            Gizmos.color = Color.yellow;
            Matrix4x4 transMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            foreach (Vector3 knot in knotList) Gizmos.DrawSphere(transMatrix.MultiplyPoint3x4(knot), 0.01f);

            if (bezierCurve) {
                Gizmos.color = Color.red;
                foreach (Vector3 knot in controllerList) Gizmos.DrawCube(transMatrix.MultiplyPoint3x4(knot), Vector3.one * 0.05f);
            }
        }

    }
}