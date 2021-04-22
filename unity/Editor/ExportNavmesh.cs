using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System.IO;

public class ExportNavmesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [MenuItem("Tools/ExportNavmesh")]
    public static void Export()
    {
        NavMeshTriangulation msehInfo = NavMesh.CalculateTriangulation();
        string strFilePath = Application.dataPath + "/mapdata.data";
        FileStream fs = new FileStream(strFilePath, FileMode.OpenOrCreate);

        fs.Seek(0, SeekOrigin.Begin);

        WriteInt(fs, msehInfo.indices.Length);
        WriteInt(fs, msehInfo.vertices.Length);

        foreach (int idx in msehInfo.indices)
        {
            WriteInt(fs, idx);
        }

        foreach (Vector3 v in msehInfo.vertices)
        {
            WriteFloat(fs, v.x);
            WriteFloat(fs, v.y);
            WriteFloat(fs, v.z);
        }

        fs.Flush();
        fs.Dispose();

        EditorUtility.DisplayDialog("提示", "导出成功", "OK");
    }

    private static void WriteFloat(FileStream fs, float fVal)
    {
        byte[] bs = System.BitConverter.GetBytes(fVal);
        fs.Write(bs, 0, bs.Length);
        fs.Seek(bs.Length, SeekOrigin.Current);
    }

    private static void WriteInt(FileStream fs, int nVal)
    {
        byte[] bs = System.BitConverter.GetBytes(nVal);
        fs.Write(bs, 0, bs.Length);
        fs.Seek(bs.Length, SeekOrigin.Current);
    }
}
