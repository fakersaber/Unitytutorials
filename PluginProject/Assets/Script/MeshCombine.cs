using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshCombine : ScriptableObject
{
    //如果选中不关联的物体合并，最好是算出每一个物体然后一次合并 每一个都变成单独的submesh
    //如果使用合并的贴图采样，会覆盖每一个独特材质的shader效果，没有意义，所以应该保留源材质效果
    //这种合并方式只是网格合并，并没有进行材质合并，不会降低drawcall

    //所以需要合并材质，当然只有在光照效果以及参数相同下才能合并，贴图可以不同再映射
    //这里就做一个对合并后贴图采样，但是合并的贴图如果使用一样不会保存。。
    [MenuItem("Tools/MergeMesh")]
    static void MergeMesh()
    {
        GameObject SelectObj = Selection.activeGameObject;
        MeshFilter[] MeshFilterNodes = SelectObj.GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] MeshRenders = SelectObj.GetComponentsInChildren<MeshRenderer>();
        CombineInstance[] NewCombineInstance = new CombineInstance[MeshFilterNodes.Length];
        //每个object一个material的情况
        Material[] Materials = new Material[MeshRenders.Length];

        for (int i = 0; i < MeshFilterNodes.Length; ++i)
        {
            NewCombineInstance[i].mesh = MeshFilterNodes[i].sharedMesh;
            NewCombineInstance[i].transform = MeshFilterNodes[i].transform.localToWorldMatrix;
            Materials[i] = MeshRenders[i].sharedMaterial;
        }
        GameObject combineMesh = new GameObject("MeshContainer");
        combineMesh.AddComponent<MeshFilter>().mesh = new Mesh();
        //多个submesh，方便材质分离
        combineMesh.GetComponent<MeshFilter>().mesh.CombineMeshes(NewCombineInstance,false);
        var MeshRender = combineMesh.AddComponent<MeshRenderer>();
        MeshRender.sharedMaterials = Materials;
        AssetDatabase.CreateAsset(combineMesh.GetComponent<MeshFilter>().sharedMesh, "Assets/MeshContainer.asset");
        PrefabUtility.SaveAsPrefabAsset(combineMesh, "Assets/MeshContainer.prefab");
    }

    [MenuItem("Tools/MergeTexture")]
    static void MergeTexture()
    {
        GameObject SelectObj = Selection.activeGameObject;
        List<Texture2D> Textures = new List<Texture2D>();
        MeshRenderer[] Materials = SelectObj.GetComponentsInChildren<MeshRenderer>();
        for (int j = 0; j < Materials.Length; ++j)
        {
            Textures.Add((Texture2D)Materials[j].sharedMaterial.mainTexture);
        }


        Texture2D atlas = new Texture2D(256, 256);
        Rect[] rects =  atlas.PackTextures(Textures.ToArray(), 0);
        foreach(var te in rects)
        {
            Debug.Log(te);
        }
        System.IO.File.WriteAllBytes("Assets/TextureContainer.png", atlas.EncodeToPNG());
        AssetDatabase.Refresh();
    }




}