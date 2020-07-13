
1.0  资源命名规则【暂时要求，因为这根导出树有关】
	1.1 所有资源文件名字小写。
	1.2 整个文件根目录下的文件名字不能出现相同名字。

2.0  客户端项目配置(必须)
	2.1 Layer 管理器
	      增加 Default 、Invisible和 Terrain 层。

3.0  编辑器项目配置
	3.1 Tag 管理器
	      增加 EditorTerrain、Terrain 和 EditorOnly。
	3.2 Layer 管理器
	      增加 Default 、Invisible和 Terrain 层。

4.0 资源导出目录规划【暂时未定】
	4.1 Unity 地形切割后的目录 Export/TerrainPatch
									xxxx ------ 场景名
										切片资源
										
	4.2 切片后的地形转成网格目录 Export/T4MOBJ
									Terrain 				---- 根目录下存放地形转 mesh 后的prefab
										Material
										Meshes
										Texture
										
	4.3 unity 场景转四叉树原始场景备份目录 
									Export/Back
										output 				---- 下存放所有操作前的 *.unity 场景
										
	4.4 导出四叉树配置目录【临时 Export/Config / 场景名 /  setting.xml】
		
				
5.0 基本代码结构
	5.1 ------四叉树资源代码管理结构
	ActClient		------ [ Editor 和 GameClient 共同依赖部分]
	     ---- EditorCompent
	     ---- Misc
	     ---- ScriptObject
	     ---- Utility 
	GameClient
	     ---- 未开发
	     ---- 未开发
	Editor
	    ---- Window
	    ---- ResExport 

6.0 美术资源目录增加 --- Artist 目录
	所有美术资源防止目录
	

7.0 草是同过  EditorSceneObject 组件，放到特殊的四叉树里
	
		例如 cfg.specSceneTree   = "grass"， 表示放到草的四叉里。