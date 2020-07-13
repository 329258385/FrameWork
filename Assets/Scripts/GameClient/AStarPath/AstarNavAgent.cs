//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-07-09
// Author: LJP
// Date: 2020-07-09
// Description: 接入 AstarPathfindingProject 导航插件,整合unity navmesh
//---------------------------------------------------------------------------------------
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;






public class AstarNavAgent : MonoBehaviour
{
    /// <summary>
    /// 寻路对象
    /// </summary>
    public Seeker               _astarSeeker;
    public NavMeshAgent         _navmeshAgent;
    #if NODEMOVE
    public NavMove              navMove;
    #endif

    public float                remainingDistance
    {
        get
        {
            #if NODEMOVE
            if (agent.enabled)
            {
                return pathRemainingDistance + agent.remainingDistance;
            }
            else
            {
                return pathRemainingDistance + navMove.remainingDistance;
            }
            #else
            return pathRemainingDistance + _navmeshAgent.remainingDistance;
            #endif
        }
    }
    
    /// <summary>
    /// 和上面区别在于navmeshagent.remainingDistance可能会返回 Infinity 的Unknowed值
    /// </summary>
    public float                realRemainingDistance
    {
        get
        {
            if (float.IsInfinity(_navmeshAgent.remainingDistance))
            {
                return pathRemainingDistance + Vector3.Distance(tranSelf.position, _navmeshAgent.destination);
            }
            else
            {
                return pathRemainingDistance + _navmeshAgent.remainingDistance;
            }
        }
    }

    public float                maxAcceleration
    {
        get
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            return _navmeshAgent.acceleration;
        }
        set
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            _navmeshAgent.acceleration = value;
        }
    }

    public float                rotationSpeed
    {
        get
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            return _navmeshAgent.angularSpeed;
        }
        set
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            _navmeshAgent.angularSpeed = value;
        }
    }

    public float                radius
    {
        get
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            return _navmeshAgent.radius;
        }
        set
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            _navmeshAgent.radius = value;
        }
    }

    public float                height
    {
        get
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            return _navmeshAgent.height;
        }
        set
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            _navmeshAgent.height = value;
        }
    }

    public float                maxSpeed
    {
        get
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            return _navmeshAgent.speed;
        }
        set
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            _navmeshAgent.speed = value;
        }
    }

    public bool                 isStopped
    {
        get
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            return _navmeshAgent.isStopped;
        }
        set
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            #if NODEMOVE
            if (agent.enabled)
            {
                agent.isStopped = value;
            }
            #endif
            if (_navmeshAgent.enabled)
            {
                _navmeshAgent.isStopped = value;
            }
            _hasPath = false;
        }
    }

    public bool                 pathPending
    {
        get
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            return _navmeshAgent.pathPending || waitingForPathCalculation;
        }
    }

    private bool                _hasPath = false;
    public bool                 hasPath
    {
        get
        {
            if (_navmeshAgent == null)
            {
                InitAgent();
            }
            return _hasPath || _navmeshAgent.hasPath;
        }
    }
    private List<Vector3>       _pathPoints = new List<Vector3>();
    public List<Vector3>        PathPoints
    {
        get
        {
            _pathPoints.Clear();
            if (totalPathPoints.Count > 0)
            {
                if (segmentIndex == totalPathPoints.Count - 1)
                {
                    //当路点只有一个时 totalPathPoints.Count - 1 == 0
                    //当时最后一个路点时
                    _pathPoints.AddRange(_navmeshAgent.path.corners);
                }
                else
                {
                    _pathPoints.AddRange(_navmeshAgent.path.corners);
                    //最后一个是目标点，不添加进去
                    for (int i = segmentIndex; i < totalPathPoints.Count - 1; i++)
                    {
                        if (!_pathPoints.Contains(totalPathPoints[i]))
                        {
                            _pathPoints.Add(totalPathPoints[i]);
                        }
                    }
                    List<NavMeshPath> endPath = NavHelper.GetRealPath(totalPathPoints[totalPathPoints.Count - 2], totalPathPoints[totalPathPoints.Count - 1]);
                    for (int i = 0; i < endPath.Count; i++)
                    {
                        for (int j = 0; j < endPath[i].corners.Length; j++)
                        {
                            if (!_pathPoints.Contains(endPath[i].corners[j]))
                            {
                                _pathPoints.Add(endPath[i].corners[j]);
                            }
                        }
                    }
                }
            }

            return _pathPoints;
        }
    }

    /// <summary>
    /// 路点随机范围半径
    /// </summary>
    public float                randomRadius = 0;
    public float                nextPointDis = 0.3f;

    /// <summary>
    /// 当前路径的所有路点
    /// </summary>
    protected List<Vector3>     totalPathPoints = new List<Vector3>();
   
    /// <summary>
    /// 路点寻路的最后一个点
    /// </summary>
    protected Vector3           endPathPoint;
    
    /// <summary>
    /// 导航的最后一个点
    /// </summary>
    protected Vector3           endNavPoint;
    
    /// <summary>
    /// 
    /// </summary>
    protected int               segmentIndex;
    protected Transform         tranSelf;

    /// <summary>
    /// 
    /// </summary>
    protected float             pathRemainingDistance;
    private bool                waitingForPathCalculation;

    /// <summary>
    /// 
    /// </summary>
    private List<NavMeshPath>   canReachPointLst;
   
    
    // Use this for initialization
    void Awake()
    {
        tranSelf        = transform;
        InitAgent();
    }


    private void OnEnable()
    {
        _navmeshAgent.enabled = true;
    }

    private void OnDisable()
    {
        waitingForPathCalculation = false;
        _navmeshAgent.enabled = false;
        _hasPath = false;
    }

 
    void Update()
    {
        if (_navmeshAgent != null && _navmeshAgent.enabled && hasPath)
        {
            if (!_navmeshAgent.pathPending && _navmeshAgent.remainingDistance < nextPointDis)
            {
                //判断，当前位置与终点的直线距离是否小于endPointCalc（参数）
                if (AstarPathGo.Instance.endPointCalc > 0)
                {
                    if (totalPathPoints.Count > 0)
                    {
                        Vector3 dis = totalPathPoints[totalPathPoints.Count - 1] - tranSelf.position;
                        if (!AstarPathGo.Instance.calcY)
                        {
                            dis.y = 0;
                        }
                        if (!_navmeshAgent.pathPending && Vector3.SqrMagnitude(dis) < AstarPathGo.Instance.SqrEndPointCalc)
                        {
                            SetNextDestination(true);
                            return;
                        }
                    }
                }
                SetNextDestination();
            }
        }
        #if NODEMOVE
        if (navMove != null && navMove.enabled && hasPath)
        {
            if (navMove.remainingDistance < nextPointDis)
            {
                SetNextDestination();
            }
        }
        #endif
    }

    void InitAgent()
    {
        _navmeshAgent = GetComponent<NavMeshAgent>();
        if (_navmeshAgent == null)
        {
            _navmeshAgent = gameObject.AddComponent<NavMeshAgent>();
            _navmeshAgent.enabled = false;
        }
        #if NODEMOVE
        if (navMove == null)
        {
            navMove = GetComponent<NavMove>();
            if (navMove == null)
            {
                navMove = gameObject.AddComponent<NavMove>();
                navMove.enabled = false;
            }
        }
        #endif
        if (_astarSeeker == null)
        {
            _astarSeeker = GetComponent<Seeker>();
            if (_astarSeeker == null)
            {
                _astarSeeker = gameObject.AddComponent<Seeker>();
            }
            _astarSeeker.pathCallback += OnPathComplete;
        }
    }

    void OnPathComplete(Path newPath)
    {
        //异步操作，如果当前这个组件已经enabled = false就不继续往下执行了
        if (!enabled) { return; }

        ABPath p = newPath as ABPath;
        if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");
        
        p.Claim(this);

        //如果最后一个点不能到达目标点的话，那么玩家跑到最后一个点
        //这样的话就不用删除最后能直达目标点的路径点
        //默认能够到最后目标点
        bool lastEndPointToTarget = true;
        //和上一次算的路径一样时
        if (SamePath(p))
        {
            //路径点目标点一样，不需要继续
            if (endNavPoint == p.originalEndPoint) { return; }

            if (endPathPoint == endNavPoint)
            {
                //上一次寻路的目标点是路点路径的最后一个点时
                //应该是路点路径的最后一个点不能到目标点
                lastEndPointToTarget = NavHelper.CanReachPoint(p.vectorPath[p.vectorPath.Count - 1], p.originalEndPoint, canReachPointLst);
                if (lastEndPointToTarget)
                {
                    //继续接下来的计算
                }
                else
                {
                    //同样不能到，继续跑最后的点
                    return;
                }
            }
            else
            {
                //当正好走到最后一个点时，最后一个点更新
                if (segmentIndex == totalPathPoints.Count - 1)
                {
                    endNavPoint = p.originalEndPoint;
                    totalPathPoints[totalPathPoints.Count - 1] = p.originalEndPoint;
                    _navmeshAgent.SetDestination(p.originalEndPoint);
                }
                else
                {
                    //还没到最后一个点直接替换就可以了
                    totalPathPoints[totalPathPoints.Count - 1] = p.originalEndPoint;
                }
                return;
            }


        }
        if (p.vectorPath.Count <= 0)
        {
            waitingForPathCalculation = false;
            p.Release(this);
            //当前路径不可达
            pathRemainingDistance = -9999;
            return;
        }
        else if (p.vectorPath.Count == 1 || (p.vectorPath.Count == 2 && (p.vectorPath[0] == p.vectorPath[1])))
        {
            totalPathPoints.Clear();
            totalPathPoints.Add(p.vectorPath[0]);
            endPathPoint = p.vectorPath[0];
        }
        else
        {
            endPathPoint = p.vectorPath[p.vectorPath.Count - 1];
            //如果最后一个点不能到达目标点的话，那么玩家跑到最后一个点
            //这样的话就不用删除最后能直达目标点的路径点
            if (canReachPointLst == null) { canReachPointLst = NavHelper.GetEmptyPath(); }
            lastEndPointToTarget = NavHelper.CanReachPoint(p.vectorPath[p.vectorPath.Count - 1], p.originalEndPoint, canReachPointLst);

            //按照路径点计算到目标点起始点
            //如果中间点能直接到目标点或起始点
            //删除中间点
            if ((AstarPathGo.Instance.pointNavSetting & AstarPathGo.PointNavModel.NAV_OPEN_MIDDLE_DIRECT_REACH) != 0)
            {
                int startPoint = FindFirstPoint(p);
                int endPoint = FindEndPoint(p);
                if (lastEndPointToTarget)
                {
                    //能直接到达目标点的最早一个点
                    if (endPoint < p.vectorPath.Count - 1)
                    {
                        p.vectorPath.RemoveRange(endPoint + 1, p.vectorPath.Count - (endPoint + 1));
                    }
                }

                //
                if (startPoint > 0)
                {
                    //能清除的点有重叠，要保留一个
                    if (startPoint >= p.vectorPath.Count - 1)
                    {
                        Vector3 lastPoint = p.vectorPath[p.vectorPath.Count - 1];
                        p.vectorPath.Clear();
                        p.vectorPath.Add(lastPoint);
                    }
                    else
                    {
                        p.vectorPath.RemoveRange(0, startPoint);
                    }
                }
            }

            #region 路点偏移
            if (randomRadius > 0)
            {
                /// 暂时不做路径点偏移
                //float tempRadius = UnityEngine.Random.Range(0, randomRadius);
                //float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
                ////第一个点不做偏移
                //for (int i = 1; i < p.vectorPath.Count; i++)
                //{
                //    //float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
                //    p.vectorPath[i] = p.vectorPath[i] + new Vector3(Mathf.Cos(angle) * tempRadius, 0, Mathf.Sin(angle) * tempRadius);
                //}
                totalPathPoints.Clear();
                for (int i = 0; i < p.vectorPath.Count; i++)
                {
                    totalPathPoints.Add(p.vectorPath[i]);
                }
            }
            #endregion
        }

        //起始位置附近的点，如果在其实半径范围内计算目标，选离目标最近的
        if (AstarPathGo.Instance.startPointCalc > 0 && totalPathPoints.Count > 0)
        {
            Vector3 pathEndPoint = p.originalEndPoint;
            if (!lastEndPointToTarget)
            {
                pathEndPoint = totalPathPoints[totalPathPoints.Count - 1];
            }

            float min = 99999999999;
            int minIndex = -1;
            for (int i = 0; i < totalPathPoints.Count; i++)
            {
                Vector3 dis_end = pathEndPoint - totalPathPoints[i];
                Vector3 dis_start = totalPathPoints[i] - p.originalStartPoint;
                if (!AstarPathGo.Instance.calcY)
                {
                    dis_end.y = 0;
                    dis_start.y = 0;
                }
                float curDis = Vector3.SqrMagnitude(dis_end);
                //如果点在范围起始范围之内，才剔除
                if (Vector3.SqrMagnitude(dis_start) < AstarPathGo.Instance.SqrStartPointCalc && curDis < min)
                {
                    if (AstarPathGo.Instance.calcStartCanReach)
                    {
                        if (NavHelper.FastRaycast(totalPathPoints[i], p.originalStartPoint))
                        {
                            min = curDis;
                            minIndex = i;
                        }
                    }
                    else
                    {
                        min = curDis;
                        minIndex = i;
                    }
                }
            }
            //将离目标最近的点移除
            if (minIndex > 0)
            {
                totalPathPoints.RemoveRange(0, minIndex + 1);
            }
        }

        p.Release(this);
        _hasPath = true;
        if (lastEndPointToTarget)
        {
            //添加目标点给路点
            totalPathPoints.Add(p.originalEndPoint);
        }

        #region 折返处理
        if ((AstarPathGo.Instance.pointNavSetting & AstarPathGo.PointNavModel.NAV_OPEN_POINT_FIRST_END_DEL) != 0)
        {
            //最后再做折返处理的
            if (totalPathPoints.Count >= 2)
            {
                float point_Angle = Vector3.Angle(totalPathPoints[1] - totalPathPoints[0], p.originalStartPoint - totalPathPoints[0]);
                if (point_Angle <= 90)
                {
                    totalPathPoints.RemoveAt(0);
                }
            }
            if (totalPathPoints.Count >= 2)
            {
                float point_Angle = 360;
                if (totalPathPoints.Count == 2)
                {
                    point_Angle = Vector3.Angle(totalPathPoints[totalPathPoints.Count - 1] - totalPathPoints[totalPathPoints.Count - 2], p.originalStartPoint - totalPathPoints[totalPathPoints.Count - 2]);
                }
                else
                {
                    point_Angle = Vector3.Angle(totalPathPoints[totalPathPoints.Count - 1] - totalPathPoints[totalPathPoints.Count - 2], totalPathPoints[totalPathPoints.Count - 3] - totalPathPoints[totalPathPoints.Count - 2]);
                }
                if (point_Angle <= 90)
                {
                    totalPathPoints.RemoveAt(totalPathPoints.Count - 2);
                }
            }
        }
        #endregion

        endNavPoint = totalPathPoints[totalPathPoints.Count - 1];
        #if UNITY_EDITOR
        _astarSeeker.lastCompletedVectorPath = totalPathPoints;
        #endif
        waitingForPathCalculation = false;
        SetSegmentIndex(0);
    }

    /// <summary>
    /// 查找能直接到起始点的最远路点
    /// </summary>
    public int FindFirstPoint(ABPath p)
    {
        int lastPoint = 0;
        for (int i = 1; i < p.vectorPath.Count; i++)
        {
            if (NavHelper.FastRaycast(p.originalStartPoint, p.vectorPath[i]))
            {
                lastPoint = i;
            }
            else
            {
                break;
            }
        }
        return lastPoint;
    }

    /// <summary>
    /// 查找能直接到目标点的最远路点
    /// </summary>
    public int FindEndPoint(ABPath p)
    {
        int lastPoint = p.vectorPath.Count - 1;
        for (int i = p.vectorPath.Count - 2; i >= 0; i--)
        {
            if (NavHelper.FastRaycast(p.originalEndPoint, p.vectorPath[i]))
            {
                lastPoint = i;
            }
        }
        return lastPoint;
    }

    public void ResetPath()
    {
        if (_navmeshAgent == null)
        {
            InitAgent();
        }
        _navmeshAgent.ResetPath();
        _hasPath = false;
    }

    private Vector3 lastDestination = Vector3.zero;
    /// <summary>
    /// 设置目标点
    /// </summary>
    public void SetDestination(Vector3 target, bool onlyNav = false)
    {
        //当前的目标点和上一个目标点一样，且上一个路径还没有算完，直接使用上一个计算
        if (lastDestination == target)
        {
            if (waitingForPathCalculation)
            {
                return;
            }
        }

        lastDestination = target;
        waitingForPathCalculation = false;

        //只使用navmesh
        if (onlyNav)
        {
            SinglePointPath(target);
            return;
        }


        //没有astar时
        if (!NavHelper.HasAStarPoint())
        {
            SinglePointPath(target);
            return;
        }

        //当前路径不是用navmesh计算的的话
        //开启距离小于多少时使用navmesh功能
        if ((AstarPathGo.Instance.pointNavSetting & AstarPathGo.PointNavModel.POINT_OPEN_LESS_DIRECT_REACH) != 0)
        {
            Vector3 dis = target - tranSelf.position;
            if (!AstarPathGo.Instance.calcY)
            {
                dis.y = 0;
            }
            if (Vector3.SqrMagnitude(dis) < AstarPathGo.Instance.SqrStartPointCalc)
            {
                SinglePointPath(target);
                return;
            }
        }


        //当前路径不是用navmesh计算,能直达的话，清除现有路径
        //开启起始点目标点直接到达时使用navmesh
        if ((AstarPathGo.Instance.pointNavSetting & AstarPathGo.PointNavModel.POINT_OPEN_START_END_DIRECT_REACH) != 0)
        {
            if (NavHelper.FastRaycast(tranSelf.position, target))
            {
                SinglePointPath(target);
                return;
            }
        }

        InitAgent();
        waitingForPathCalculation = true;
        pathRemainingDistance = 99999;
        _astarSeeker.StartPath(tranSelf.position, target, null);
    }

    public void SinglePointPath(Vector3 point)
    {
        totalPathPoints.Clear();
        totalPathPoints.Add(point);
        SetSegmentIndex(0);
    }

    /// <summary>
    /// 只有第一个才是使用SetDestination，其他的使用SetNextDestination
    /// </summary>
    public void SetSegmentIndex(int index)
    {
        segmentIndex = index;
        _navmeshAgent.enabled = true;
        _navmeshAgent.SetDestination(totalPathPoints[segmentIndex]);
        UpdateRemainingDistance();
    }

    void SetNextDestination(bool toEnd = false)
    {
        if (segmentIndex < totalPathPoints.Count - 1)
        {
            segmentIndex++;
            if (toEnd) { segmentIndex = totalPathPoints.Count - 1; }
            #if NODEMOVE
            navMove.destination = totalPathPoints[segmentIndex];
            navMove.velocity = GetVelocity();
            agent.enabled = false;
            navMove.enabled = true;
            #else
            _navmeshAgent.updateRotation = true;
            _navmeshAgent.enabled = true;
            _navmeshAgent.SetDestination(totalPathPoints[segmentIndex]);
            if (segmentIndex == totalPathPoints.Count - 1)
            {
                //agent.autoBraking = true;
            }
            else
            {
                //agent.autoBraking = false;
            }
            #endif

        }
        UpdateRemainingDistance();
    }

    void UpdateRemainingDistance()
    {
        pathRemainingDistance = 0;
        for (int i = segmentIndex; i < totalPathPoints.Count; i++)
        {
            if (i + 1 < totalPathPoints.Count)
            {
                pathRemainingDistance += Vector3.Distance(totalPathPoints[i], totalPathPoints[i + 1]);
            }
        }
    }
   
    public bool SamePath(Path _path)
    {
        if (_hasPath)
        {
            if (_path.vectorPath.Count > 0 && _path.vectorPath[_path.vectorPath.Count - 1] == endPathPoint)
            {
                return true;
            }
        }
        return false;
    }
}
