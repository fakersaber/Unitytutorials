using UnityEngine;
using UnityEditor;

public class MeshCombine : ScriptableObject
{
    [MenuItem("Tools/MergeMesh")]
    static void DoIt()
    {
        GameObject[] SelectObjArray = Selection.gameObjects;
        CombineInstance[] combineInstances = new CombineInstance[SelectObjArray.Length];

        for (int i = 0; i < SelectObjArray.Length; ++i)
        {
            var obj = SelectObjArray[i].GetComponent<MeshFilter>();
            combineInstances[i].mesh = obj.mesh;
            combineInstances[i].transform = obj.transform.localToWorldMatrix;
        }

        Mesh CombineMesh = new Mesh();
        CombineMesh.CombineMeshes(combineInstances);
        var CombineMeshRender = GameObject.FindWithTag("MeshContainer").AddComponent<MeshFilter>();
        CombineMeshRender.sharedMesh = CombineMesh;
    }
}