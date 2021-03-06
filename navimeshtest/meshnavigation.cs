using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace navimeshtest
{
    using Vertexts = Dictionary<int, Vector3>;
    using Triangles = List<Triangle>;

    public class Vector3
    {
        public float x;
        public float y;
        public float z;

        public bool IsEqual(Vector3 v)
        {
            return (v.x == x && v.y == y && v.z == z);
        }

        public static Vector3 operator+(Vector3 a, Vector3 b)
        {
            Vector3 v = new Vector3();
            v.x = a.x + b.x;
            v.y = a.y + b.y;
            v.z = a.z + b.z;
            return v;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            Vector3 v = new Vector3();
            v.x = b.x - a.x;
            v.y = b.y - a.y;
            v.z = b.z - a.z;
            return v;
        }

        public static Vector3 operator/(Vector3 a, float fVal)
        {
            Vector3 v = new Vector3();
            v.x = a.x / fVal;
            v.y = a.y / fVal;
            v.z = a.z / fVal;
            return v;
        }

        public float Dot(Vector3 v)
        {
            return x * v.x +  y * v.y + z * v.z;
        }

        public Vector3 Cross(Vector3 v)
        {
            Vector3 vv = new Vector3();
            v.x = y * v.z - z * v.y;
            v.y = z * v.x - x * v.z;
            v.z = x * v.y - y * v.x;
            return vv;
        }

        public float Distance(Vector3 v)
        {
            return (v.x - x) * (v.x - x) + (v.z - z) * (v.z - z);
        }
    }

    class Triangle
    {
        public Edge a;
        public Edge b;
        public Edge c;
        public Vector3 center = null;
        public int idx;
        public float neighborGVal = 0.0f; //相对于邻居的g值
        public float runtimeGVal = 0.0f; //从起点到这个点的消耗值 
        public float runtimeHVal = 0.0f; //从这个点到终点的消耗值
        public int parentIdx = -1;
        public List<int> samePointTriangles = new List<int>();
        
        public Vector3 CenterPos(Vertexts vers)
        {
            if (center == null)
            {
                center = (a.GetStartPos(vers) + b.GetStartPos(vers) + c.GetStartPos(vers)) / 3;
            }

            return center;
        }

        public float GetRuntimeFVal()
        {
            return runtimeGVal + runtimeHVal;
        }

        public float Distance(Vertexts vers, Triangle tri)
        {
            Vector3 centerPos = this.CenterPos(vers);
            Vector3 tarCenterPos = tri.CenterPos(vers);

            double dVal = Math.Pow((tarCenterPos.x - center.x), 2.0) + Math.Pow((tarCenterPos.z - center.z), 2.0);
            return Convert.ToSingle(dVal);
        }
        
        //重心法
        public bool PointIsInTriangle(Vertexts vers, Vector3 P)
        {
            Vector3 A = a.GetStartPos(vers);
            Vector3 B = b.GetStartPos(vers);
            Vector3 C = c.GetStartPos(vers);

            Vector3 v0 = C - A;
            Vector3 v1 = B - A;
            Vector3 v2 = P - A;

            float dot00 = v0.Dot(v0);
            float dot01 = v0.Dot(v1);
            float dot02 = v0.Dot(v2);
            float dot11 = v1.Dot(v1);
            float dot12 = v1.Dot(v2);

            float inverDeno = 1 / (dot00 * dot11 - dot01 * dot01);

            float u = (dot11 * dot02 - dot01 * dot12) * inverDeno;
            if (u < 0 || u > 1) // if u out of range, return directly
            {
                return false;
            }

            float v = (dot00 * dot12 - dot01 * dot02) * inverDeno;
            if (v < 0 || v > 1) // if v out of range, return directly
            {
                return false;
            }

            return u + v <= 1;
        }

        //判断两个三角形是否有共公的点
        public bool HasSamePoint(Triangle tri)
        {
            return (a.startIdx == tri.a.startIdx ||
                a.startIdx == tri.b.startIdx ||
                a.startIdx == tri.c.startIdx ||
                b.startIdx == tri.a.startIdx ||
                b.startIdx == tri.b.startIdx ||
                b.startIdx == tri.c.startIdx ||
                c.startIdx == tri.a.startIdx ||
                c.startIdx == tri.b.startIdx ||
                c.startIdx == tri.c.startIdx);
        }
    }

    class Edge
    {
        public int startIdx = -1;
        public int endIdx = -1;
        public int triangleIdx = -1;
        public float gVal = 0.0f;

        public bool IsEqual(Edge d)
        {
            if ((startIdx == d.startIdx && endIdx == d.endIdx) ||
                (endIdx == d.startIdx && startIdx == d.endIdx))
            {
                return true;
            }
            return false;
        }

        public Vector3 GetStartPos(Vertexts vers)
        {
            return vers[startIdx];
        }

        public Vector3 GetEndPos(Vertexts vers)
        {
            return vers[endIdx];
        }

        public Vector3 GetVector(Vertexts vers)
        {
            return GetEndPos(vers) - GetStartPos(vers);
        }
    }

    class MeshNavigation
    {
        public List<int> indices { private set; get; }
        public Vertexts newVectexts { private set; get; }
        public float meshWidth { private set; get; }
        public float meshHeight { private set; get; }
        public Dictionary<int, Triangle> triangles { private set; get; }

        private Vertexts tempVectexts;

        public MeshNavigation()
        {
            indices = new List<int>();
            newVectexts = new Vertexts();
            meshWidth = 0;
            meshHeight = 0;
            triangles = new Dictionary<int, Triangle>();
            tempVectexts = new Vertexts();
        }

        public void LoadRecastMeshData()
        {
            bool bVal = CPPDLL.recast_init();
            if (bVal)
            {
                StringBuilder strSrc = new StringBuilder("meshdata/solo_navmesh.bin");
                bVal = CPPDLL.recast_loadmap(1, strSrc);
                if (bVal)
                {
                }
            }
        }

        public void LoadMeshData(string strFilepath)
        {
            LoadRecastMeshData();
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
                tempVectexts[i] = v;
                if (v.x < fMinX) fMinX = v.x;
                if (v.z < fMinZ) fMinZ = v.z;
                if (v.x > fMaxX) fMaxX = v.x;
                if (v.z > fMaxZ) fMaxZ = v.z;
            }
            meshWidth = (fMaxX - fMinX);
            meshHeight = (fMaxZ - fMinZ);
            fs.Close();

            DuplicateRemovalIndices();
            BuildTriangles();
            BuildNeighbor();

            Vector3 vv = new Vector3();
            vv.x = 40;
            vv.y = 0;
            vv.z = 40.5f;
            Triangle tri = GetOwnerTriangle(vv);
            if (tri == null)
            {
                int a = 100;
                a++;
            }
            else
            {
                int a = 100;
                a++;
            }

        }

        //需要优化，目前暂时遍历
        public Triangle GetOwnerTriangle(Vector3 v)
        {
            foreach(var tri in triangles)
            {
                if (tri.Value.PointIsInTriangle(newVectexts, v))
                {
                    return tri.Value;
                }
            }
            return null;
        }

        public void GetNeightborsAndBuildGVal(int nTriIdx, Triangles outTris)
        {
            if (triangles.ContainsKey(nTriIdx))
            {
                Triangle tri = triangles[nTriIdx];
                if (tri.a.triangleIdx >= 0)
                {
                    Triangle subTri = triangles[tri.a.triangleIdx];
                    subTri.neighborGVal = tri.a.gVal;
                    outTris.Add(subTri);
                }
                if (tri.b.triangleIdx >= 0)
                {
                    Triangle subTri = triangles[tri.b.triangleIdx];
                    subTri.neighborGVal = tri.b.gVal;
                    outTris.Add(subTri);
                }
                if (tri.c.triangleIdx >= 0)
                {
                    Triangle subTri = triangles[tri.c.triangleIdx];
                    subTri.neighborGVal = tri.c.gVal;
                    outTris.Add(subTri);
                }

                foreach (var t in tri.samePointTriangles)
                {
                    Triangle subTri = triangles[t];
                    outTris.Add(subTri);
                }
            }
        }

        public Triangle GetTriangle(int idx)
        {
            if (triangles.ContainsKey(idx))
            {
                return triangles[idx];
            }
            return null;
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
            for (int i = 0; i < tempVectexts.Count; i++)
            {
                Vector3 v = tempVectexts[i];
                int foundIdx = -1;

                if (!IsExistIn(v, out foundIdx))
                {
                    newVectexts[i] = v;
                }
                else
                {
                    ReplaceIndices(i, foundIdx);
                }
            }
        }

        private void ReplaceIndices(int from, int to)
        {
            for (int i=0; i<indices.Count; i++)
            {
                if (indices[i] == from)
                {
                    indices[i] = to;
                    Console.WriteLine("{0}===>{1}", from, to);
                }
            }
        }

        private bool IsExistIn(Vector3 v, out int idx)
        {
            idx = -1;
            foreach (var vv in newVectexts)
            {
                bool bVal = vv.Value.IsEqual(v);
                if (bVal) 
                {
                    idx = vv.Key;
                    return true;
                } 
            }
            return false;
        }

        private void BuildTriangles()
        {
            int idx = 0;
            for (int i = 0; i < indices.Count();)
            {
                int a = indices[i++];
                int b = indices[i++];
                int c = indices[i++];

                Edge edge1 = new Edge();
                edge1.startIdx = a;
                edge1.endIdx = b;

                Edge edge2 = new Edge();
                edge2.startIdx = b;
                edge2.endIdx = c;

                Edge edge3 = new Edge();
                edge3.startIdx = c;
                edge3.endIdx = a;

                Triangle tri = new Triangle();
                tri.a = edge1;
                tri.b = edge2;
                tri.c = edge3;

                triangles[idx] = tri;
                tri.idx = idx;
                idx++;
            }
        }

        //建立邻居关系(三角形的每条边地应哪个三角形，且计算G值)
        private void BuildNeighbor()
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                Triangle tri = triangles[i];
                BuildSingleTriangle(tri);
            }
        }

        public float GetDistance(Triangle a, Triangle b)
        {
            return a.Distance(newVectexts, b);
        }

        //根据三角形的中心点~~到目标点的距离 
        public float GetDistanceByTriangleCenter(Triangle a, Vector3 b)
        {
            Vector3 aPos = a.CenterPos(newVectexts);
            float fVal = (b.x - aPos.x) * (b.x - aPos.x) + (b.z - aPos.z) * (b.z - aPos.z);
            return fVal;
        }

        //计算 取三角形三个角 到目标点最近的距离
        public float GetDistanceByTrianglePoint(Triangle a, Vector3 b)
        {
            float aDistance = a.a.GetStartPos(newVectexts).Distance(b);
            float bDistance = a.b.GetStartPos(newVectexts).Distance(b);
            float cDistance = a.c.GetStartPos(newVectexts).Distance(b);

            return Math.Min(Math.Min(aDistance, bDistance), cDistance);
            //return aDistance + bDistance + cDistance;
        }

        private void BuildSingleTriangle(Triangle inTri)
        {
            for (int i=0; i<triangles.Count; i++)
            {
                Triangle tri = triangles[i];
                if (tri.idx == inTri.idx)
                {
                    continue;
                }

                float fDistance = tri.Distance(newVectexts, inTri);

                if (tri.a.IsEqual(inTri.a))
                {
                    tri.a.triangleIdx = inTri.idx;
                    inTri.a.triangleIdx = tri.idx;
                    tri.a.gVal = inTri.a.gVal = fDistance;
                }
                else if(tri.a.IsEqual(inTri.b))
                {
                    tri.a.triangleIdx = inTri.idx;
                    inTri.b.triangleIdx = tri.idx;
                    tri.a.gVal = inTri.b.gVal = fDistance;
                }
                else if (tri.a.IsEqual(inTri.c))
                {
                    tri.a.triangleIdx = inTri.idx;
                    inTri.c.triangleIdx = tri.idx;
                    tri.a.gVal = inTri.c.gVal = fDistance;
                }

                else if (tri.b.IsEqual(inTri.a))
                {
                    tri.b.triangleIdx = inTri.idx;
                    inTri.a.triangleIdx = tri.idx;
                    tri.b.gVal = inTri.a.gVal = fDistance;
                }
                else if (tri.b.IsEqual(inTri.b))
                {
                    tri.b.triangleIdx = inTri.idx;
                    inTri.b.triangleIdx = tri.idx;
                    tri.b.gVal = inTri.b.gVal = fDistance;
                }
                else if (tri.b.IsEqual(inTri.c))
                {
                    tri.b.triangleIdx = inTri.idx;
                    inTri.c.triangleIdx = tri.idx;
                    tri.b.gVal = inTri.c.gVal = fDistance;
                }

                else if (tri.c.IsEqual(inTri.a))
                {
                    tri.c.triangleIdx = inTri.idx;
                    inTri.a.triangleIdx = tri.idx;
                    tri.c.gVal = inTri.a.gVal = fDistance;
                }
                else if (tri.c.IsEqual(inTri.b))
                {
                    tri.c.triangleIdx = inTri.idx;
                    inTri.b.triangleIdx = tri.idx;
                    tri.c.gVal = inTri.b.gVal = fDistance;
                }
                else if (tri.c.IsEqual(inTri.c))
                {
                    tri.c.triangleIdx = inTri.idx;
                    inTri.c.triangleIdx = tri.idx;
                    tri.c.gVal = inTri.c.gVal = fDistance;
                }
                //是否有公共的点
                //else if (tri.HasSamePoint(inTri))
                //{
                //    tri.samePointTriangles.Add(inTri.idx);
                //}
            }
        }
    }
}
