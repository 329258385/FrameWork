//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-13
// Author: LJP
// Date: 2020-05-13
// Description: 场景资源回复管理
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;





namespace ActEditor
{
    /// <summary>
    /// 备份资源
    /// </summary>
    struct BackupAsset
    {
        string _path;
        string _backupPath;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path"></param>
        public BackupAsset(string path)
        {
            _path = path;
            _backupPath = null;
        }

        /// <summary>
        /// 备份
        /// </summary>
        /// <param name="dir"></param>
        public void Backup(string dir)
        {
            // 不是资源
            if(string.IsNullOrEmpty(_path))
            {
                return;
            }

            // 文件名
            var fileName = Path.GetFileNameWithoutExtension(_path);
            if(string.IsNullOrEmpty(fileName))
            {
                return;
            }
            var ext = Path.GetExtension(_path);

            // 拷贝
            var backupPath = string.Format("{0}/{1}{2}", dir, fileName, ext);
            int n = 0;
            while(File.Exists(backupPath))
            {
                var newName = string.Format("{0}_{1}", fileName, n++);
                backupPath = string.Format("{0}/{1}{2}", dir, newName, ext);
                Debug.LogErrorFormat("{0} is repeat, set to {1}", fileName, newName);
            }

            File.Copy(_path, backupPath);
            _backupPath = backupPath;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void Recover()
        {
            // 不是资源
            if(string.IsNullOrEmpty(_path))
            {
                return;
            }

            // 没有备份
            if(string.IsNullOrEmpty(_backupPath))
            {
                return;
            }

            // 还原
            File.Delete(_path);
            File.Copy(_backupPath, _path);
            _backupPath = null;
        }

        /// <summary>
        /// 路径
        /// </summary>
        public string path { get { return _path; } }
    }

    /// <summary>
    /// 资源备份器
    /// </summary>
    public class AssetsBackuper
    {
        /// <summary>
        /// 备份目录
        /// </summary>
        static readonly string BackupDir = string.Format("{0}/../__Temp", Application.dataPath);

        /// <summary>
        /// 资源字典
        /// </summary>
        Dictionary<string, BackupAsset> _dict = new Dictionary<string, BackupAsset>();

        /// <summary>
        /// 单例
        /// </summary>
        static AssetsBackuper _inst;
        public static AssetsBackuper inst 
        { 
            get
            {
                if(null == _inst)
                {
                    _inst = new AssetsBackuper();
                }
                return _inst;
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            _dict.Clear();

            if(Directory.Exists(BackupDir))
            {
                Directory.Delete(BackupDir, true);
            }
        }

        /// <summary>
        /// 备份
        /// </summary>
        /// <param name="asset"></param>
        public void Backup(UnityEngine.Object asset)
        {
            // 不是资源
            var path = AssetDatabase.GetAssetPath(asset);
            if(string.IsNullOrEmpty(path))
            {
                return;
            }

            // 已经备份过了
            if(_dict.ContainsKey(path))
            {
                return;
            }

            // 创建目录
            if(!Directory.Exists(BackupDir))
            {
                Directory.CreateDirectory(BackupDir);
            }

            // 备份
            var ba = new BackupAsset(path);
            ba.Backup(BackupDir);

            // 加入字典
            _dict.Add(path, ba);
        }

        /// <summary>
        /// 还原
        /// </summary>
        public void Recover()
        {
            foreach(var pair in _dict)
            {
                pair.Value.Recover();
            }
            _dict.Clear();

            // 删除目录
            if(Directory.Exists(BackupDir))
            {
                Directory.Delete(BackupDir, true);
            }
        }
    }
}