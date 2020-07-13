//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-09
// Author: LJP
// Date: 2020-05-09
// Description: 编辑器菜单项
//---------------------------------------------------------------------------------------
namespace ActEditor
{
    public class ActEditorMenuItem
    {
        public const string ExportTerrainSlicing        = "ActClient/Step1 分割地形";
        public const int ExportTerrainSlicingPriority   = 22001;

        public const string ExportTerrainMerge          = "ActClient/Step2 合并地形";
        public const int ExportTerrainMergePriority     = 22002;

        public const string SplitWater                  = "ActClient/Step2 分割水面";
        public const int SplitWaterPriority             = 22003;
        
        public const string ExportTerrainFenGeMesh      = "ActClient/Step3 转化网格";
        public const int ExportTerrainFenGeMeshPriority = 22004;

        public const string ExportScene                 = "ActClient/Step4 导出场景...";
        public const int ExportScenePriority            = 22005;

        public const string TestExportScene             = "ActClient/测试 导出场景...";
        public const int TestExportScenePriority        = 22006;
    }
}
