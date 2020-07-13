using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;


public class MatEXEquationAttAttribute:Attribute
{
	public string title;
	public string description;
	public MatEXEquationAttAttribute(string _title,string _description)
	{
		title = _title;
		description = _description;
	}
}

/// <summary>
/// Equation 
/// x,y both range [0,1] 
/// </summary>
public class MatEXEquationLib{
	protected static float PI{get{ return Mathf.PI;}}
	protected static float Sin(float x)
	{
		return Mathf.Sin (x);
	}
	protected static float Cos(float x)
	{
		return Mathf.Cos (x);
	}



	[MatEXEquationAtt("静止","0.5f")]
	public static float Equation_Still(float x)
	{
		return 0.5f;
	}
	[MatEXEquationAtt("平方","x * x")]
	public static float Equation_1(float x)
	{
		return x * x;
	}

	[MatEXEquationAtt("Sin","0.5f+0.5f*Sin(x*PI*2)")]
	public static float Equation_Sin(float x)
	{
		return 0.5f+0.5f*Sin(x*PI*2);
	}
	[MatEXEquationAtt("Sin x 2","float x2PI = x * PI * 2;\nfloat y = 0.8f * Sin (x2PI) + 0.2f * Sin (x2PI * 10f);\nreturn 0.5f+0.5f*y;")]
	public static float Equation_SinX2(float x)
	{
		float x2PI = x * PI * 2;
		float y = 0.8f * Sin (x2PI) + 0.2f * Sin (x2PI * 10f);
		return 0.5f+0.5f*y;
	}

	[MatEXEquationAtt("Cos","0.5f+0.5f*Cos(x*PI*2)")]
	public static float Equation_Cos(float x)
	{
		return 0.5f+0.5f*Cos(x*PI*2);
	}
	[MatEXEquationAtt("渐弱波","0.5f+0.5f*Sin(x*PI*2*5)*(1-x)")]
	public static float Equation_SinWeak(float x)
	{
		return 0.5f+0.5f*Sin(x*PI*2*5)*(1-x);
	}
	[MatEXEquationAtt("渐强波","0.5f+0.5f*Sin(x*PI*2*5)*x")]
	public static float Equation_SinStrong(float x)
	{
		return 0.5f+0.5f*Sin(x*PI*2*5)*x;
	}
}
