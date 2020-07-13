//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-20
// Author: LJP
// Date: 2020-05-20
// Description: LightMap依赖
//---------------------------------------------------------------------------------------
using UnityEngine;
using ActClient;
using System.Collections.Generic;





namespace ActEditor
{
    public class ResDepLightMap : ResDepBase
    {
        /// <summary>
        /// 条目
        /// </summary>
        class Item
        {
            public Renderer     renderer;
            public Texture2D    color;
            public Texture2D    dir;
        }
        List<Item> _items = new List<Item>();

        /// <summary>
        /// 资源类型
        /// </summary>
        override public ResType resType { get { return ResType.Texture; } }

        /// <summary>
        /// 收集依赖
        /// </summary>
        public static ResDepLightMap CollectDependencies(GameObject go, bool bCollectChildren)
        {
            if(LightmapSettings.lightmaps.Length < 1)
            {
                return null;
            }

            // 收集LightMap索引
            var renderers = new List<Renderer>();
            EditorUtil.VisitComponents<Renderer>(go, bCollectChildren, renderer =>
            {
                var index = renderer.lightmapIndex;
                if(index < 0 || index >= LightmapSettings.lightmaps.Length)
                {
                    return;
                }
                renderers.Add(renderer);
            });

           
            if(renderers.Count < 1)
            {
                return null;
            }

            ResDepLightMap dep = null;
            for(int i = 0, ci = renderers.Count; i < ci; ++i)
            {
                var renderer = renderers[i];
                var data = LightmapSettings.lightmaps[renderer.lightmapIndex];
                if(null == data)
                {
                    continue;
                }

                if(null != data.lightmapColor || null != data.lightmapDir)
                {
                    var item = new Item { renderer = renderer, color = data.lightmapColor, dir = data.lightmapDir };
                    if(null == dep)
                    {
                        dep = new ResDepLightMap();
                    }
                    dep._items.Add(item);
                }
            }

            return dep;
        }

        /// <summary>
        /// 遍历资源
        /// </summary>
        override protected void VisitAssets(ResDescCollector resCollector, VisitResDelegate visitor)
        {
            for(int i = 0, ci = _items.Count; i < ci; ++i)
            {
                var item = _items[i];
                if(null != item.color)
                {
                    var resName = visitor(item.color);
                    resCollector.AddLightMap(resName, item.renderer, true);
                }

                if(null != item.dir)
                {
                    var resName = visitor(item.dir);
                    resCollector.AddLightMap(resName, item.renderer, false);
                }
            }
        }

        /// <summary>
        /// 移除依赖
        /// </summary>
        override public void RemoveDependencies()
        {
            for(int i = 0, ci = _items.Count; i < ci; ++i)
            {
                var item = _items[i];

                // 置空贴图
                var lightMaps       = LightmapSettings.lightmaps;
                var data            = lightMaps[item.renderer.lightmapIndex];
                data.lightmapColor  = null;
                data.lightmapDir    = null;
                data.shadowMask     = null;
                lightMaps[item.renderer.lightmapIndex] = data;
                LightmapSettings.lightmaps = lightMaps;
            }
        }
    }
}