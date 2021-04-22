using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;   //必须添加，不然DllImport报错
using System.Text;

namespace navimeshtest
{
    class CPPDLL
    {
        [DllImport("NavigationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool recast_init();
        [DllImport("NavigationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void recast_fini();
        [DllImport("NavigationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool recast_loadmap(int id, StringBuilder path);
        [DllImport("NavigationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool recast_freemap(int id);
        [DllImport("NavigationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 recast_findpath(int id, ref float startPos, ref float endPost);
        [DllImport("NavigationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool recast_smooth(int id, float step_size, float slop);
        [DllImport("NavigationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int recast_getcountsmooth(int id);
        [DllImport("NavigationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        
        public static extern IntPtr recast_getpathsmooth(int id);
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool bVal = CPPDLL.recast_init();
            if (bVal)
            {
                StringBuilder strSrc = new StringBuilder("meshdata/solo_navmesh.bin");
                bVal = CPPDLL.recast_loadmap(1, strSrc);
                if (bVal)
                {
                    int a = 0;
                    a++;

                    float[] startPos = new float[3] { -33f, 0.172f, -2.8f };
                    float[] endPos = new float[3] {87f, 0.172f, 59.86f };

                    UInt32 nRet = CPPDLL.recast_findpath(1, ref startPos[0], ref endPos[0]);

                    if (nRet == (1u << 30))
                    {
                        //成功
                        bVal = CPPDLL.recast_smooth(1, 0, 0);

                        int nPahtPointNum = CPPDLL.recast_getcountsmooth(1);
                        IntPtr ptr = CPPDLL.recast_getpathsmooth(1);
                        float[] arrSmooths = new float[nPahtPointNum * 3];
                        Marshal.Copy(ptr, arrSmooths, 0, nPahtPointNum * 3);
                        for (int i=0; i<nPahtPointNum;i++)
                        {
                            int idx = i * 3;
                            float x = arrSmooths[idx];
                            float y = arrSmooths[idx+1];
                            float z = arrSmooths[idx+2];
                            Console.WriteLine("{0}, {1}, {2} \n", x, y, z);
                        }
                        //float* fVal = CPPDLL.recast_getpathsmooth(1);

                        //for (int i = 0; i < nPahtPointNum; i++)
                        //{
                        //    int idx = i * 3;
                        //    float x = fVal[idx];
                        //    float y = fVal[idx + 1];
                        //    float z = fVal[idx + 2];
                        //    printf("%f, %f, %f \n", x, y, z);
                        //}
                    }
                    else if (nRet == (1u<<31))
                    {
                        //失败
                    }

                    bVal = CPPDLL.recast_freemap(1);
                }
                CPPDLL.recast_fini();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
