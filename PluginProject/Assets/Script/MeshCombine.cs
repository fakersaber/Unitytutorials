using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshCombine : ScriptableObject
{
    //合并选中模型，将其贴图合并为一张，然后在新建的mesh重新设置纹理坐标
    [MenuItem("Tools/MergeMesh")]
    public static void MergeMesh()
    {
        GameObject[] SelectObjArray = Selection.gameObjects;
        List<CombineInstance> NewCombineInstance = new List<CombineInstance>();
        List<Texture2D> Textures = new List<Texture2D>();

        //贴图合并部分
        for (int index = 0; index < SelectObjArray.Length; ++index)
        {
            MeshRenderer[] MeshRenders = SelectObjArray[index].GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < MeshRenders.Length; ++j)
            {
                Textures.Add((Texture2D)MeshRenders[j].sharedMaterial.mainTexture);
            }
        }
        Texture2D atlas = new Texture2D(256, 256);
        Rect[] rects = atlas.PackTextures(Textures.ToArray(), 0);

        Texture2D ConvertAtlas = ConvertTexture(atlas, 256, 256);
        //当前贴图组对应下标
        int CurRectGoupIndex = 0;
        //一个mesh对应一张贴图
        for (int i = 0; i < SelectObjArray.Length; ++i)
        {
            MeshFilter[] MeshFilterNodes = SelectObjArray[i].GetComponentsInChildren<MeshFilter>();
            for (int mesh_index = 0; mesh_index < MeshFilterNodes.Length; ++mesh_index)
            {
                Vector2[] TempUV = new Vector2[MeshFilterNodes[mesh_index].mesh.uv.Length];
                for (int j = 0; j < MeshFilterNodes[mesh_index].mesh.uv.Length; ++j)
                {
                    TempUV[j].x = rects[CurRectGoupIndex + mesh_index].x +
                        MeshFilterNodes[mesh_index].mesh.uv[j].x * rects[CurRectGoupIndex + mesh_index].width;
                    TempUV[j].y = rects[CurRectGoupIndex + mesh_index].y +
                        MeshFilterNodes[mesh_index].mesh.uv[j].y * rects[CurRectGoupIndex + mesh_index].height;
                }
                MeshFilterNodes[mesh_index].mesh.uv = TempUV;
                CombineInstance CombineNode = new CombineInstance();
                CombineNode.mesh = MeshFilterNodes[mesh_index].mesh;
                CombineNode.transform = MeshFilterNodes[mesh_index].transform.localToWorldMatrix;
                NewCombineInstance.Add(CombineNode);
            }
            CurRectGoupIndex += MeshFilterNodes.Length;
        }

        GameObject combineMesh = new GameObject("MeshContainer");
        combineMesh.AddComponent<MeshFilter>().mesh = new Mesh();
        combineMesh.GetComponent<MeshFilter>().mesh.CombineMeshes(NewCombineInstance.ToArray(), true);
        var MeshRender = combineMesh.AddComponent<MeshRenderer>();
        System.IO.File.WriteAllBytes("Assets/TextureContainer.png", ConvertAtlas.EncodeToPNG());
        AssetDatabase.Refresh();

        //新建材质,默认使用Standard
        MeshRender.sharedMaterial = new Material(Shader.Find("Standard"));
        MeshRender.sharedMaterial.mainTexture = ConvertAtlas;
        //MeshRender.sharedMaterial.SetTexture("_MainTex", atlas);
        AssetDatabase.CreateAsset(combineMesh.GetComponent<MeshFilter>().sharedMesh, "Assets/MeshContainer.asset");
        PrefabUtility.SaveAsPrefabAsset(combineMesh, "Assets/MeshContainer.prefab");
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