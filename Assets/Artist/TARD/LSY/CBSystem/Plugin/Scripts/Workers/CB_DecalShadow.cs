using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public enum DecaShadowType
{
	point
}

[ExecuteInEditMode]
public class CB_DecalShadow  : CB_WorkerBase {
	public DecaShadowType type;
}
