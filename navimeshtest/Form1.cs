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
        private MeshNavigation meshnav = new MeshNavigation();
        private AStarathFind4Tri asf = null;
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
            asf = new AStarathFind4Tri();
            asf.SetMeshData(meshnav);
            this.Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Black);

            foreach (var triInfo in meshnav.triangles)
            {
                Triangle tri = triInfo.Value;
                DrawTriangle_1(g, tri.a.GetStartPos(meshnav.newVectexts), tri.b.GetStartPos(meshnav.newVectexts), tri.c.GetStartPos(meshnav.newVectexts));
            }

            DrawPath(g);
        }

        private void DrawPath(Graphics g)
        {
            if (outPaths.Count == 0) return;
            
            List<Point> arrPoints = new List<Point>();
            arrPoints.Add(startPos);
            foreach (var tri in outPaths)
            {
                Vector3 v = tri.CenterPos(meshnav.newVectexts);
                Point p = ConvertVector3ToPoint(v);
                arrPoints.Add(p);
            }
            arrPoints.Add(endPos);

            Pen pen = new Pen(Color.Yellow);
            g.DrawLines(pen, arrPoints.ToArray());
        }

        private Point ConvertVector3ToPoint(Vector3 v)
        {
            int ClientWidth = this.Width;
            int ClientHeight = this.Height;
            float fWidthOffset = ClientWidth / 2;
            float fHeightOffset = ClientHeight / 2;
            float xScale = (this.Width) / meshnav.meshWidth;
            float zScale = (this.Height) / meshnav.meshHeight;
            float fScale = Math.Min(xScale, zScale);
            fScale = fScale * 0.6f;

            int x1 = Convert.ToInt32(v.x * fScale + fWidthOffset);
            int y1 = Convert.ToInt32(v.z * fScale + fHeightOffset);
            return new Point(x1, y1);
        }

        // 绘制三角形      
        private void DrawTriangle_1(Graphics g, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Point point1 = ConvertVector3ToPoint(v1);
            Point point2 = ConvertVector3ToPoint(v2);
            Point point3 = ConvertVector3ToPoint(v3);
            Point[] pntArr = { point1, point2, point3 };
            g.DrawPolygon(new Pen(Color.Red), pntArr);
        }

        public Vector3 ConvertClientPointToVector3(Point p)
        {
            int ClientWidth = this.Width;
            int ClientHeight = this.Height;
            float fWidthOffset = ClientWidth / 2;
            float fHeightOffset = ClientHeight / 2;
            float xScale = (this.Width) / meshnav.meshWidth;
            float zScale = (this.Height) / meshnav.meshHeight;
            float fScale = Math.Min(xScale, zScale);
            fScale = fScale * 0.6f;

            float xx = (p.X - fWidthOffset) / fScale;
            float zz = (p.Y - fHeightOffset) / fScale;
            Vector3 v = new Vector3();
            v.x = xx;
            v.z = zz;
            v.y = 0;
            return v;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
           
        }

        private Point startPos;
        private Point endPos;
        List<Triangle> outPaths = new List<Triangle>();
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPos = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                endPos = e.Location;

                Vector3 v1 = ConvertClientPointToVector3(startPos);
                Vector3 v2 = ConvertClientPointToVector3(endPos);
                outPaths.Clear();
                bool bVal = asf.FindPaths(v1, v2, outPaths);
                if (bVal)
                {
                    this.Refresh();
                }
            }
        }
    }
}
