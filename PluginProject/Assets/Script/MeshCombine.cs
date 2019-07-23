using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshCombine : ScriptableObject
{
    [MenuItem("Tools/MergeMesh")]
    static void MergeMesh()
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

    [MenuItem("Tools/MergeTexture")]
    static void MergeTexture()
    {
        GameObject[] SelectObjArray = Selection.gameObjects;
        Texture2D[] Textures = new Texture2D[SelectObjArray.Length]; 
        for(int i = 0; i < SelectObjArray.Length; ++i)
        {
            MeshRenderer Materials = SelectObjArray[i].GetComponent<MeshRenderer>();
            Textures[i] = (Texture2D)Materials.sharedMaterial.mainTexture;
        }
        //定制时初始化修改参数
        Texture2D atlas = new Texture2D(256, 256);
        Rect[] rects =  atlas.PackTextures(Textures, 0);
        atlas.Resize(512, 256);

        System.IO.File.WriteAllBytes("Assets/TextureContainer.png", atlas.EncodeToPNG());
        AssetDatabase.Refresh();
    }

    //设置纹理大小 根据GetPixelBilinear设置对应纹素
    //https://blog.csdn.net/WPAPA/article/details/63684585



}