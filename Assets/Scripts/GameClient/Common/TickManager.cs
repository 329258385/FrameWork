//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-22
// Author: LJP
// Date: 2020-05-22
// Description: 时间管理器
//---------------------------------------------------------------------------------------
using System;





namespace ActClient
{
    static public class TickManager
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        static DateTime         _startTime;

        /// <summary>
        /// 当前帧的Tick
        /// </summary>
        static uint             _frameTick;

        /// <summary>
        /// Delta
        /// </summary>
        static float            _deltaTime;
        static uint             _deltaTick;

        /// <summary>
        /// 开始
        /// </summary>
        public static void Start()
        {
            _startTime = DateTime.Now;
            _frameTick = realTick;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public static void Update(float deltaTime)
        {
            var rt = realTick;
            
            // 第一次更新
            if(rt <= _frameTick)
            {
                _frameTick = rt;
                _deltaTime = deltaTime;
                _deltaTick = TimeToTick(_deltaTime);
                return;
            }

            _deltaTick = rt - _frameTick;
            _deltaTime = _deltaTick * 0.001f;
            _frameTick = rt;
        }

        /// <summary>
        /// Second -> Tick
        /// </summary>
        public static uint TimeToTick(float t) { return (uint)(t * 1000); }

        /// <summary>
        /// Tick -> Second
        /// </summary>
        public static float TickToTime(uint t) { return t * 0.001f; }

        /// <summary>
        /// 从指定时间经过的时间（毫秒）
        /// </summary>
        public static uint TickSince(uint t)
        {
            if(_frameTick < t)
            {
                return 0;
            }
            return _frameTick - t;
        }

        /// <summary>
        /// 从指定时间经过的时间（秒）
        /// </summary>
        public static float TimeSince(uint t)
        {
            var d = TickSince(t);
            return TickToTime(d);
        }

        /// <summary>
        /// 计算时间差
        /// </summary>
        public static float TimeDelta(uint t1, uint t2)
        {
            return TickDelta(t1, t2) * 0.001f;
        }

        /// <summary>
        /// 时间差
        /// </summary>
        public static int TickDelta(uint t1, uint t2)
        {
            return t1 > t2 ? (int)(t1 - t2) : -(int)(t2 - t1);
        }

        /// <summary>
        /// 当前的真实TICK
        /// </summary>
        /// <returns></returns>
        public static uint      realTick { get { return (uint)((DateTime.Now - _startTime).Ticks / 10000); } }

        /// <summary>
        /// 当前帧的TICK
        /// </summary>
        public static uint      tick { get { return _frameTick; } }

        /// <summary>
        /// Delta
        /// </summary>
        public static uint      deltaTick { get { return _deltaTick; } }
        public static float     deltaTime { get { return _deltaTime; } }
    }
}