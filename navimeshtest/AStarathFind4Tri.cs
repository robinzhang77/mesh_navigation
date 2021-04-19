using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navimeshtest
{
    using Paths = List<Vector3>;
    using TriangleList = List<int>;
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

        public bool FindPaths(Vector3 startPos, Vector3 endPos, Paths ps)
        {
            if (meshData == null) return false;

            ps.Clear();
            openedList.Clear();
            closedList.Clear();

            Triangle triStart = meshData.GetOwnerTriangle(startPos);
            Triangle triEnd = meshData.GetOwnerTriangle(endPos);

            if (triStart == null || triEnd == null) return false;

            

            return true;
        }
    }
}
