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
            MeshFilter ObjMeshFilter = SelectObjArray[i].GetComponent<MeshFilter>();
            combineInstances[i].mesh = ObjMeshFilter.sharedMesh;
            combineInstances[i].transform = ObjMeshFilter.transform.localToWorldMatrix;
        }
        GameObject combineMesh = new GameObject("MeshContainer");
        combineMesh.AddComponent<MeshFilter>().mesh = new Mesh();
        combineMesh.GetComponent<MeshFilter>().mesh.CombineMeshes(combineInstances);
        combineMesh.AddComponent<MeshRenderer>();
        AssetDatabase.CreateAsset(combineMesh.GetComponent<MeshFilter>().sharedMesh, "Assets/MeshContainer.asset");
        PrefabUtility.SaveAsPrefabAsset(combineMesh, "Assets/MeshContainer.prefab");

    }
    LightProbes
}