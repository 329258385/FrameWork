using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
public class FSMessager {
	public static LuaFunction luaFunc = null;
	public static LuaFunction luaFuncOff = null;
    public static LuaFunction luaGM = null;
	public static void TurnOn()
	{
		if(luaFunc != null)
		{
			luaFunc.Call ();
		}
	}
	public static void TurnOff()
	{
		if(luaFuncOff != null)
		{
			luaFuncOff.Call ();
		}
	}
    public static void SendGM(string gmTxt)
    {
        if (luaGM != null)
        {
            luaGM.Call(gmTxt);
        }
    }
    public static void ReceiveMsg(string txt)
	{
#if UNITY_EDITOR
        Debug.LogError ("lsyReceiveMsg:" + txt);
#endif
        FSMessagerManager.Instance.ReceiveMsg (txt);
	}
}
