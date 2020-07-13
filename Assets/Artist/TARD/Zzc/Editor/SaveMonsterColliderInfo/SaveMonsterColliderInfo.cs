//#if UNITY_EDITOR
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;
//using Excel;
//using System.Text.RegularExpressions;
//using System.Data;
//using OfficeOpenXml;
//using UnityEditor;
//using System.Reflection;
//using System;

//public class SaveMonsterColliderInfo:EditorWindow {
//    public static string myPath;
//    public static string monsterPath;
//    private string value;
//    private string all;
//    private static float R;
//    private static float H;
//    private static List<string> paths ;
//    private static List<string> nameList ;
//    private static List<string> colliderInfo;

//    SaveMonsterColliderInfo()
//    {
//        this.titleContent = new GUIContent("创建Monster碰撞信息");
//    }

//    [MenuItem("Tools/创建Monster碰撞信息")]
//    static void showWindow()
//    {
//        EditorWindow.GetWindow(typeof(SaveMonsterColliderInfo));
//    }

//    void OnGUI()
//    {
//        GUILayout.BeginVertical();
//        GUILayout.Space(10);
//        //GUI.skin.label.fontSize = 24;
//        //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
//        GUILayout.Label("创建Monster碰撞信息");
//        GUILayout.Space(10);
//        monsterPath = EditorGUILayout.TextField("monster表的绝对路径",monsterPath, GUILayout.Width(1000f));
//        myPath = EditorGUILayout.TextField("信息导出表的绝对路径",myPath, GUILayout.Width(1000f));
//        if (GUILayout.Button("导出碰撞信息"))
//        {
//            GetSelectPrafabFilePath();
//        }
//    }

//    private static void ReadExcel()
//    {
//        paths = new List<string>();
//        FileStream stream = File.Open(monsterPath, FileMode.Open, FileAccess.Read);
//        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
//        DataSet result = excelReader.AsDataSet();

//        //do
//        //{
//        //    Debug.Log(excelReader.Name);
//        //    while (excelReader.Read())
//        //    {
//        //        for (int i = 0; i < excelReader.FieldCount; i++)
//        //        {
//        //            string value = excelReader.IsDBNull(i) ? "" : excelReader.GetString(i);
//        //            Debug.Log(value);
//        //        }
//        //    }
//        //} while (excelReader.NextResult());

//        //DataSet result = excelReader.AsDataSet();

//        int columns = result.Tables[1].Columns.Count;
//        int rows = result.Tables[1].Rows.Count;
//        for (int i = 0; i < rows; i++)
//        {
//            if (result.Tables[1].Rows[i][27] == null)
//            {
//                paths.Add("");
//            }
//            else if (result.Tables[1].Rows[i][27].ToString() == "")
//            {
//                paths.Add("");
//            }
//            else
//            {
//                paths.Add(result.Tables[1].Rows[i][27].ToString());
//            }
//            //Debug.Log(result.Tables[1].Rows[i][27]+"---------"+(i+1).ToString());
//        }
//        stream.Close();
//        //return excelData;
//    }

//    private static void WriteExcel()
//    {
//        //MyExcel excel = ExcelHelper.LoadExcel(myPath);
//        //excel.Tables[0].SetValue(1, 1, "000");
//        //ExcelHelper.SaveExcel(excel, myPath);

//        FileInfo newFile = new FileInfo(myPath);
//        if (newFile.Exists)
//        {
//            newFile.Delete();
//            newFile = new FileInfo(myPath);
//        }
//        using (ExcelPackage package=new ExcelPackage(newFile))
//        {
//            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("test");
//            for (int i = 0; i < paths.Count; i++)
//            {
//                worksheet.Cells[i+1, 1].Value = paths[i];
//                worksheet.Cells[i + 1, 2].Value = "1,1";
//                if (paths[i] == "")
//                {
//                    worksheet.Cells[i + 1, 2].Value = "";
//                    continue;
//                }
//                for (int j = 0; j < nameList.Count; j++)
//                {
//                    if (nameList[j]=="Assets/Artist/Prefabs/"+paths[i]+".prefab")
//                    {
//                        worksheet.Cells[i + 1, 2].Value = colliderInfo[j];
//                    }
//                }
//            }
//            worksheet.Cells[1, 2].Value = " ";
//            worksheet.Cells[2, 2].Value = " ";
//            package.Save();
//            Debug.Log("导出碰撞信息complete");
//        }
        
//    }

//    //[MenuItem("Assets/创建碰撞信息Excel")]
//    private static void GetSelectPrafabFilePath()
//    {
//        myPath = myPath.Replace('\\', '/').Replace(" ","");
//        monsterPath = monsterPath.Replace('\\', '/').Replace(" ", "");

//        if (!File.Exists(myPath))
//        {
//            Debug.Log("导出monster碰撞信息的excel文件路径不存在");
//            return;
//        }
//        if (!File.Exists(monsterPath))
//        {
//            Debug.Log("monster表路径不存在");
//            return;
//        }

//        nameList = new List<string>();
//        colliderInfo = new List<string>();
//        string p = "/Artist/Prefabs/Character";
//        //Director(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]), nameList);
//        Director(Application.dataPath + p, nameList);
//        Debug.Log("获取到"+ Application.dataPath + p + "下带胶囊碰撞体的prefab数量:"+ nameList.Count);
//        Debug.Log("获取到的碰撞信息的数量:" + colliderInfo.Count);

//        ReadExcel();
//        Debug.Log("Read Complete");
//        WriteExcel();
//        Debug.Log("Write Complete");

//    }

//    public static void Director(string dir,List<string> list)
//    {
//        DirectoryInfo d = new DirectoryInfo(dir);
//        FileInfo[] files = d.GetFiles();
//        DirectoryInfo[] directs = d.GetDirectories();
//        foreach (FileInfo item in files)
//        {
//            if (item.Name.EndsWith(".prefab"))
//            {
//                string[] assetPath = item.FullName.Replace('\\','/').Split(new string[] { "/Assets" },StringSplitOptions.RemoveEmptyEntries);                
//                GameObject colliderInfoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets" + assetPath[assetPath.Length - 1]);
//                if (colliderInfoPrefab.GetComponent<CapsuleCollider>()==null)
//                {
//                    continue;
//                }
//                list.Add("Assets" + assetPath[assetPath.Length - 1]);
//                CapsuleCollider colliderInfoCollider = colliderInfoPrefab.GetComponent<CapsuleCollider>();
//                if (colliderInfoCollider.direction==1)
//                {
//                    R = colliderInfoCollider.radius;
//                    H = colliderInfoCollider.height;
//                }
//                else
//                {
//                    R = colliderInfoCollider.height * 0.5f;
//                    H = colliderInfoCollider.radius * 2.0f;
//                }
//                string colliderInfoStr= R.ToString() + "," + H.ToString();
//                colliderInfo.Add(colliderInfoStr);
//            }            
//        }
//        foreach (DirectoryInfo dd in directs)
//        {
//            Director(dd.FullName, list);
//        }
//    }

//}
//#endif