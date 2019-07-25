using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[System.Serializable]
public class ScriptObject : ScriptableObject
{
    public List<OriginObjData> ObjListInfo;
    //public Dictionary<string, Transform> OriginObjData;

}


[System.Serializable]
public class OriginObjData
{
    public string Name;
    public Vector3 Position;
    public Vector3 Rotaiton;
    public Vector3 Scale;

    //数据均为绝对数据，若需要可以保存父节点数据
    public OriginObjData(string Name, Transform objTransform)
    {
        this.Name = Name;
        this.Position = objTransform.position;
        this.Rotaiton = objTransform.eulerAngles;
        this.Scale = objTransform.localScale;
    }

}