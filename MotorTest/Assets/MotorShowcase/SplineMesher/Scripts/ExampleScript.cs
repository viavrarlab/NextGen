using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMesher {
    public class ExampleScript : MonoBehaviour {

        public Material matForMesh;

        private LineManager lineMgrComp;
        private SplineMesher spMeshComp;

        private void Start() {
            // create empty gameobject
            GameObject splineMesh = new GameObject();
            splineMesh.name = "spline mesh";

            // add components to the empty gameobject
            lineMgrComp = splineMesh.AddComponent<LineManager>();
            spMeshComp = splineMesh.AddComponent<SplineMesher>();

            // add knot to spline
            lineMgrComp.knotNum++;
            // *** update is needed ***
            lineMgrComp.ManualUpdate();

            // change position of the knot which we added above
            // get knot list first
            // *** note: the coordinates in knotList obtained by using lineMgrComp.GetLineKnots() is in local space ***
            // *** note: if you need world space coordinates, using knotList = lineMgrComp.GetActualLineKnots(); instead ***
            List<Vector3> knotList = lineMgrComp.GetLineKnots();
            knotList[knotList.Count - 1] = knotList[knotList.Count - 2] + Vector3.up;
            // *** update is needed ***
            lineMgrComp.ManualUpdate();

            // assign material to spline mesh
            spMeshComp.mat = matForMesh;

            // set other parameters of spline mesh
            spMeshComp.isRectangle = false;
            spMeshComp.tubeRadius = 0.5f;
            spMeshComp.sides = 24;
        }

    }
}