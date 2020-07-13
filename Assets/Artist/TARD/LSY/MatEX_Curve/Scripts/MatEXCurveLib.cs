using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LibCurveData
{
	public string title;
	public string description;
	public AnimationCurve curve;
}


public class MatEXCurveLib:ScriptableObject{
	public List<LibCurveData> data; 
}
