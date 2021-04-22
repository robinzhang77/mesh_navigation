using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/**
 客户端生成地图数据 供服务器使用
 实现原理及步骤
 
 1 在unity中生成navmesh（把地图+静态阻挡等 bake出一个navmesh）
 2 编写插件，把生成的navmesh导出顶点+索引，以下称为（地图数据）
   （1，2部需要在unity中完成）
 3 编写c++库读取这个（地图数据），按顶点+索引 生成三角色及邻角关系，并计算每个三角形中心到其邻角们的距离，并保存为G值，之后在计算中直接读取
 4 根据AStar算法 寻路 F = G + H，其中G值是初始就算好的，H值为当前三角形到终点的直线距离
 5 对寻路结果做 拐点算法 平滑路径
 
 问题思考：
 
    移动单位在移动过程因为临时阻挡，都可以通过避让系统对当前的移动单位的velocity做调整，但前提是这个临时阻挡没有阻断（堵死）之前的寻径路径
    阻断路径情况：
        1） navmesh寻路方案
            需要把这个阻挡物的mesh（或是变形后的方块或是圆形）变成navmesh的obstacle，也就是要把它与之前的顶点+索引融合在一起（雕刻进地图数据中），
            然后重新计算上面的第3部，代价是昂贵的（优化方法：可以通过四叉树，对局部的三角形重新重新初始化）
            初始完成后，还需要再次做寻径处理+拐点处理得到新的路径点
        2） 如果地图数据不是三角形的，而是在c++库读取时转换成了方格的地图数据，那就是根据阻挡物的体积换算成占用的格子，把格子的属性标记为阻挡即可

  如果游戏中不存在这种情况，那这个问题 可以忽略

mesh寻径和格子寻径的差异
    相同点：
        mesh寻径与格子寻径 原理上都是采用astar 一个三角形的寻路，一个是正方形的寻路
    不同点：
        mesh寻路的拐点算法 效率高于 格子对路径的平滑算法
        mesh寻径更贴合地形
        mesh寻径可计算高度
        临时阻挡物对格子寻径影响小，对mesh寻径影响大

偿试安案：
    通过四叉树对地图数据中的三角形做管理，当某个地区的三角型因为阻挡物发生改变了，可能透过四叉树快速定位到被影响的那些三角形，做局部初始化
 */

public class NavgataionTest : MonoBehaviour
{
    public Transform InitPos;
    private NavMeshAgent agent = null;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.destination = InitPos.position;
        agent.Warp(InitPos.position);


        //NavMeshTriangulation mmtris = NavMesh.CalculateTriangulation();
        //mmtris.

    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}
