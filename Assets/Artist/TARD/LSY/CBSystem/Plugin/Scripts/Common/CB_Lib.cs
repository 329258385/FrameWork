using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CB_Lib{
	public static bool MatNeedCreate(Material mat,Material src)
	{
		return mat == null || mat.shader != src.shader;
	}
}
