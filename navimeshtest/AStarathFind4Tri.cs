using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navimeshtest
{
    using TriangleList = List<Triangle>;
    class AStarathFind4Tri
    {
        private TriangleList openedList;
        private TriangleList closedList;
        private MeshNavigation meshData = null;
        
        public AStarathFind4Tri()
        {
            openedList = new TriangleList();
            closedList = new TriangleList();
        }

        public void SetMeshData(MeshNavigation data)
        {
            meshData = data;
        }

        public bool FindPaths(Vector3 startPos, Vector3 endPos, TriangleList outPaths)
        {
            if (meshData == null) return false;
            
            openedList.Clear();
            closedList.Clear();

            Triangle triStart = meshData.GetOwnerTriangle(startPos);
            Triangle triEnd = meshData.GetOwnerTriangle(endPos);

            if (triStart == null || triEnd == null) return false;

            //初始化开始点和终点的g和h值
            triStart.runtimeGVal = 0.0f;
            triStart.runtimeHVal = 0.0f;
            triEnd.runtimeGVal = 0.0f;
            triEnd.runtimeHVal = 0.0f;

            return Search(triStart, triEnd, outPaths);
        }


        private bool Search(Triangle triStart, Triangle triEnd, TriangleList outPaths)
        {
            Triangle triCurr = triStart;
            while (triCurr != triEnd)
            {
                TriangleList arrTirs = new TriangleList();
                meshData.GetNeightborsAndBuildGVal(triCurr.idx, arrTirs);
                
                foreach (var tri in arrTirs)
                {
                    float gVal = tri.neighborGVal + triCurr.runtimeGVal;
                    float hVal = meshData.GetDistance(tri, triEnd);
                    float fVal = gVal + hVal;
                    
                    if (IsOpenOrClosed(tri))
                    {
                        if (tri.GetRuntimeFVal() > fVal)
                        {
                            tri.runtimeGVal = gVal;
                            tri.runtimeHVal = hVal;
                            tri.parentIdx = triCurr.idx;
                        }
                    }
                    else
                    {
                        tri.runtimeGVal = gVal;
                        tri.runtimeHVal = hVal;
                        tri.parentIdx = triCurr.idx;
                        openedList.Add(tri);
                    }
                }

                closedList.Add(triCurr);

                if (openedList.Count == 0)
                {
                    Console.WriteLine("no path found");
                    return false;
                }

                triCurr = ShiftMinCostOpenTri();
            }

            BuildPath(triStart, triEnd, outPaths);
            return true;
        }
        private void BuildPath(Triangle triStart, Triangle triEnd, TriangleList outPaths)
        {
            outPaths.Clear();
            outPaths.Add(triEnd);
            int parentIdx = triEnd.parentIdx;
            
            while (parentIdx != triStart.idx && parentIdx != -1)
            {
                Triangle tri = meshData.GetTriangle(parentIdx);
                if (tri != null)
                {
                    outPaths.Add(tri);
                    parentIdx = tri.parentIdx;
                }
            }

            outPaths.Reverse();
        }

        private bool IsOpenOrClosed(Triangle tri)
        {
            return IsOpen(tri) || IsClosed(tri);
        }

        private bool IsOpen(Triangle tri)
        {
            return openedList.Contains(tri);
        }

        private bool IsClosed(Triangle tri)
        {
            return closedList.Contains(tri);
        }

        private Triangle ShiftMinCostOpenTri()
        {
            openedList.Sort(SortByCost);
            Triangle shift = openedList[0];
            openedList.RemoveAt(0);
            return shift;
        }

        private int SortByCost(Triangle a, Triangle b)
        {
            float v = a.GetRuntimeFVal() - b.GetRuntimeFVal();
            if (v > 0)
            {
                return 1;
            }
            else if (v < 0)
            {
                return -1;
            }
            return 0;
        }
    }
}
