using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace navimeshtest
{
    class Vector3
    {
        public float x;
        public float y;
        public float z;
    }

    class meshnavigation
    {
        public List<int> indices = new List<int>();
        public Dictionary<int, Vector3> vectexts = new Dictionary<int, Vector3>();
        public float meshWith = .0f;
        public float meshHeight = .0f;

        public void LoadMeshData(string strFilepath)
        {
            FileStream fs = new FileStream(strFilepath, FileMode.Open);
            if (fs == null)
            {
                return;
            }

            fs.Seek(0, SeekOrigin.Begin);
            int nIndexNum = ReadInt(fs);
            int nVectextNum = ReadInt(fs);

            for (int i = 0; i < nIndexNum; i++)
            {
                int nVal = ReadInt(fs);
                indices.Add(nVal);
            }

            float fMinX = .0f;
            float fMinZ = .0f;
            float fMaxX = .0f;
            float fMaxZ = .0f;

            for (int i = 0; i < nVectextNum; i++)
            {
                Vector3 v = new Vector3();
                v.x = ReadFloat(fs);
                v.y = ReadFloat(fs);
                v.z = ReadFloat(fs);
                vectexts[i] = v;
                if (v.x < fMinX) fMinX = v.x;
                if (v.z < fMinZ) fMinZ = v.z;
                if (v.x > fMaxX) fMaxX = v.x;
                if (v.z > fMaxZ) fMaxZ = v.z;
            }
            meshWith = (fMaxX - fMinX);
            meshHeight = (fMaxZ - fMinZ);
            fs.Close();
        }

        private int ReadInt(FileStream fs)
        {
            byte[] bs = new byte[4];
            fs.Read(bs, 0, sizeof(int));
            fs.Seek(sizeof(int), SeekOrigin.Current);
            return System.BitConverter.ToInt32(bs, 0);
        }

        private float ReadFloat(FileStream fs)
        {
            byte[] bs = new byte[4];
            fs.Read(bs, 0, sizeof(float));
            fs.Seek(sizeof(float), SeekOrigin.Current);
            return System.BitConverter.ToSingle(bs, 0);
        }

        private void DuplicateRemovalIndices()
        {

        }
    }
}
