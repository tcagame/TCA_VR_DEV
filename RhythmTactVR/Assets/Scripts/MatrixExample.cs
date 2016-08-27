using UnityEngine;
using System.Collections;

public class MatrixExample : MonoBehaviour {
    public float rotAngle;
    public float stretch;
    private MeshFilter mf;
    private Vector3[] origVerts;
    private Vector3[] newVerts;
    void Start() {
        mf = GetComponent<MeshFilter>();
        origVerts = mf.mesh.vertices;
        newVerts = new Vector3[origVerts.Length];
    }
    void Update() {
        Quaternion rot = Quaternion.Euler(rotAngle, 0, 0);
        Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
        Matrix4x4 inv = m.inverse;
        int i = 0;
        while (i < origVerts.Length) {
            Vector3 pt = m.MultiplyPoint3x4(origVerts[i]);
            pt.y *= stretch;
            newVerts[i] = inv.MultiplyPoint3x4(pt);
            i++;
        }
        mf.mesh.vertices = newVerts;
    }
}
