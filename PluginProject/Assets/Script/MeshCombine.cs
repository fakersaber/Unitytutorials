using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class MeshCombine : EditorWindow
{
    GameObject[] SelectObjArray;
    int TextureSize = 0;



    [MenuItem("Tools/Show Window")]
    static void ShowWindow()
    {
        var win = EditorWindow.GetWindow<MeshCombine>();
        win.Show();
    }



    private void OnSelectionChange()
    {
        //选中更改时更新ui
        Repaint();
    }



    //每次窗口收到消息更新
    private void OnGUI()
    {
        TextureSize = EditorGUILayout.IntField("TextureSize", TextureSize);
        SelectObjArray = Selection.gameObjects;
        for(int i = 0;i< SelectObjArray.Length; ++i)
        {
            MeshFilter[] CurMesh = SelectObjArray[i].GetComponentsInChildren<MeshFilter>();
            for (int j = 0; j < CurMesh.Length; ++j)
            {
                EditorGUILayout.ObjectField("CombineMesh", CurMesh[j].sharedMesh, typeof(Mesh), false);
            }
        }

        if (GUILayout.Button("Combine"))
        {
            MergeMesh();
        }
    }


    //[MenuItem("Tools/MergeMesh")]
    private void MergeMesh()
    {
        if(SelectObjArray.Length == 0)
        {
            return;
        }
        //SelectObjArray = Selection.gameObjects;
        List<CombineInstance> NewCombineInstance = new List<CombineInstance>();
        List<Texture2D> Textures = new List<Texture2D>();
        for (int index = 0; index < SelectObjArray.Length; ++index)
        {
            MeshRenderer[] MeshRenders = SelectObjArray[index].GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < MeshRenders.Length; ++j)
            {
                Textures.Add((Texture2D)MeshRenders[j].sharedMaterial.mainTexture);
            }
        }
        Texture2D atlas = new Texture2D(2048, 2048);
        Rect[] rects = atlas.PackTextures(Textures.ToArray(), 0);
        Texture2D ConvertAtlas = ConvertTexture(atlas, TextureSize, TextureSize);
        //当前贴图组对应下标
        int CurRectGoupIndex = 0;
        //一个mesh对应一张贴图
        for (int i = 0; i < SelectObjArray.Length; ++i)
        {
            MeshFilter[] MeshFilterNodes = SelectObjArray[i].GetComponentsInChildren<MeshFilter>();
            for (int mesh_index = 0; mesh_index < MeshFilterNodes.Length; ++mesh_index)
            {
                Vector2[] TempUV = new Vector2[MeshFilterNodes[mesh_index].sharedMesh.uv.Length];
                for (int j = 0; j < MeshFilterNodes[mesh_index].sharedMesh.uv.Length; ++j)
                {
                    TempUV[j].x = rects[CurRectGoupIndex + mesh_index].x +
                        MeshFilterNodes[mesh_index].sharedMesh.uv[j].x * rects[CurRectGoupIndex + mesh_index].width;
                    TempUV[j].y = rects[CurRectGoupIndex + mesh_index].y +
                        MeshFilterNodes[mesh_index].sharedMesh.uv[j].y * rects[CurRectGoupIndex + mesh_index].height;
                }
                //MeshFilterNodes[mesh_index].sharedMesh.uv = TempUV;
                //不改变原始对象mesh,复制一个mesh
                Mesh TempMesh = Instantiate<Mesh>(MeshFilterNodes[mesh_index].sharedMesh);
                TempMesh.uv = TempUV;

                CombineInstance CombineNode = new CombineInstance();
                //CombineNode.mesh = MeshFilterNodes[mesh_index].sharedMesh;
                CombineNode.mesh = TempMesh;
                CombineNode.transform = MeshFilterNodes[mesh_index].transform.localToWorldMatrix;
                NewCombineInstance.Add(CombineNode);
            }
            CurRectGoupIndex += MeshFilterNodes.Length;
        }

        GameObject combineMesh = new GameObject("MeshContainer");
        combineMesh.AddComponent<MeshFilter>().sharedMesh = new Mesh();
        combineMesh.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(NewCombineInstance.ToArray(), true);
        var MeshRender = combineMesh.AddComponent<MeshRenderer>();
        //新建材质,默认使用Standard
        MeshRender.sharedMaterial = new Material(Shader.Find("Standard"));
        MeshRender.sharedMaterial.mainTexture = ConvertAtlas;

        //写入资源,合并后的贴图为png时导出材质不会自动识别
        AssetDatabase.CreateAsset(combineMesh.GetComponent<MeshFilter>().sharedMesh, "Assets/MeshContainer.asset");
        AssetDatabase.CreateAsset(ConvertAtlas, "Assets/TextureContainer.asset");
        AssetDatabase.CreateAsset(MeshRender.sharedMaterial, "Assets/MaterialContainer.mat");
        PrefabUtility.SaveAsPrefabAsset(combineMesh, "Assets/MeshContainer.prefab");

        //写出png文件
        System.IO.File.WriteAllBytes("Assets/TextureContainer.png", ConvertAtlas.EncodeToPNG());
        AssetDatabase.Refresh();
    }



    private static Texture2D ConvertTexture(Texture2D atlas,int Width,int Height)
    {
        Texture2D result = new Texture2D(Width,Height, atlas.format,false);
        for (int i = 0; i < result.width; ++i)
        {
            for(int j = 0; j < result.height; ++j)
            {
                result.SetPixel(i, j,
                    atlas.GetPixelBilinear((float)i / (float)result.width, (float)j / (float)result.height)
                    );
            }
        }
        result.Apply();
        return result;
    }

}