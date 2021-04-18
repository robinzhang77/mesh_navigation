using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace navimeshtest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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

        class Vector3
        {
            public float x;
            public float y;
            public float z;
        }

        List<int> indices = new List<int>();
        Dictionary<int, Vector3> vectexts = new Dictionary<int, Vector3>();
        float fMinX = .0f;
        float fMinZ = .0f;
        float fMaxX = .0f;
        float fMaxZ = .0f;
        float meshWith = .0f;
        float meshHeight = .0f;
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strFilepath = "D:/work/UnityProjects/navmesh/Assets/mapdata.data";
            FileStream fs = new FileStream(strFilepath, FileMode.Open);
            if (fs == null)
            {
                return;
            }

            fs.Seek(0, SeekOrigin.Begin);
            int nIndexNum = ReadInt(fs);
            int nVectextNum = ReadInt(fs);

            for (int i=0; i<nIndexNum; i++)
            {
                int nVal = ReadInt(fs);
                indices.Add(nVal);
            }

            
            for (int i=0; i<nVectextNum; i++)
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

            fMinX = Math.Abs(fMinX) + 50;
            fMinZ = Math.Abs(fMinZ) + 50;

            fs.Close();
            this.Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            

            for (int i = 0; i < indices.Count();)
            {
                Vector3 v1 = vectexts[indices[i]];
                i++;
                Vector3 v2 = vectexts[indices[i]];
                i++;
                Vector3 v3 = vectexts[indices[i]];
                i++;
                DrawTriangle_1(g, v1, v2, v3);
            }
        }

        // 绘制三角形      
        private void DrawTriangle_1(Graphics g, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int ClientWidth = this.Width;
            int ClientHeight = this.Height;
            float fWidthOffset = ClientWidth / 2;
            float fHeightOffset = ClientHeight / 2;
            float xScale = (this.Width) / meshWith;
            float zScale = (this.Height) / meshHeight;
            float fScale = Math.Min(xScale, zScale);
            fScale = fScale * 0.6f;
            //fWidthOffset = fHeightOffset = 0;
            int x1 = Convert.ToInt32(v1.x * fScale + fWidthOffset);
            int y1 = Convert.ToInt32(v1.z * fScale + fHeightOffset);
            int x2 = Convert.ToInt32(v2.x* fScale + fWidthOffset);
            int y2 = Convert.ToInt32(v2.z* fScale + fHeightOffset);
            int x3 = Convert.ToInt32(v3.x* fScale + fWidthOffset);
            int y3 = Convert.ToInt32(v3.z* fScale + fHeightOffset);

            Point point1 = new Point(x1, y1);
            Point point2 = new Point(x2, y2);
            Point point3 = new Point(x3, y3);
            Point[] pntArr = { point1, point2, point3 };
            g.DrawPolygon(new Pen(Color.Red), pntArr);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }
}
