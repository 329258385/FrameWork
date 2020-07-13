/*
               #########                       
              ############                     
              #############                    
             ##  ###########                   
            ###  ###### #####                  
            ### #######   ####                 
           ###  ########## ####                
          ####  ########### ####               
         ####   ###########  #####             
        #####   ### ########   #####           
       #####   ###   ########   ######         
      ######   ###  ###########   ######       
     ######   #### ##############  ######      
    #######  #####################  ######     
    #######  ######################  ######    
   #######  ###### #################  ######   
   #######  ###### ###### #########   ######   
   #######    ##  ######   ######     ######   
   #######        ######    #####     #####    
    ######        #####     #####     ####     
     #####        ####      #####     ###      
      #####       ###        ###      #        
        ###       ###        ###               
         ##       ###        ###               
__________#_______####_______####______________
                我们的未来没有BUG                
*/
using System;
using System.Collections.Generic;
using UnityEngine;




namespace ActClient
{
    //---------------------------------------------------------------------
    // 对象池
    //---------------------------------------------------------------------
    public class ObjectPool
    {
        /// <summary>
        /// 每种类型最大数量
        /// </summary>
        const int MAX_OBJ_PER_TYPE = 4096;
        static Dictionary<Type, List<object>> _objs = new Dictionary<Type, List<object>>();

        
        static public void Clear()
        {
            _objs.Clear();
        }

   
        /// <summary>
        /// 分配
        /// </summary>
        static public T New<T>() where T : class, IDisposable, new()
        {
            // 从空闲队列取
            List<object> freeList;
            if (_objs.TryGetValue(typeof(T), out freeList))
            {
                var count = freeList.Count;
                if (count > 0)
                {
                    var obj = freeList[count - 1];
                    freeList.RemoveAt(count - 1);
                    return obj as T;
                }
            }

            // 创建
            return new T();
        }

        /// <summary>
        /// 释放
        /// </summary>
        static public void Release<T>(ref T obj) where T : class, IDisposable, new()
        {
            if (null == obj)
            {
                return;
            }

            // 查找
            var t = obj.GetType();
            List<object> freeList;
            if (!_objs.TryGetValue(t, out freeList))
            {
                freeList = new List<object>();
                _objs.Add(t, freeList);
            }

            // 限制数量
            if (freeList.Count < MAX_OBJ_PER_TYPE)
            {
                // 放入列表
                freeList.Add(obj);
            }
            else
            {
                Debug.LogWarningFormat("Pool<{0}> overfollow!", typeof(T).FullName);
            }

            // 施放对象
            obj.Dispose();
            obj = null;
        }
    }
}
