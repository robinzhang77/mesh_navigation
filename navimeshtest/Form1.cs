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
        MeshNavigation meshnav = new MeshNavigation();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string strFilepath = "D:/work/UnityProjects/navmesh/Assets/mapdata.data";
            string strFilepath = "./mapdata.data";
            meshnav.LoadMeshData(strFilepath);
            this.Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Black);

            //for (int i = 0; i < meshnav.indices.Count();)
            //{
            //    Vector3 v1 = meshnav.newVectexts[meshnav.indices[i]];
            //    i++;
            //    Vector3 v2 = meshnav.newVectexts[meshnav.indices[i]];
            //    i++;
            //    Vector3 v3 = meshnav.newVectexts[meshnav.indices[i]];
            //    i++;
            //    DrawTriangle_1(g, v1, v2, v3);
            //}

            foreach (Triangle tri in meshnav.triangles)
            {
                DrawTriangle_1(g, tri.a.GetStartPos(meshnav.newVectexts), tri.b.GetStartPos(meshnav.newVectexts), tri.c.GetStartPos(meshnav.newVectexts));
            }
        }

        // 绘制三角形      
        private void DrawTriangle_1(Graphics g, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int ClientWidth = this.Width;
            int ClientHeight = this.Height;
            float fWidthOffset = ClientWidth / 2;
            float fHeightOffset = ClientHeight / 2;
            float xScale = (this.Width) / meshnav.meshWeith;
            float zScale = (this.Height) / meshnav.meshHeight;
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
