using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumeMixer;

public class XFFect :VMValue {
	public float val;


	public override void Mix(VMValue _a,float blend)
	{
		XFFect a = (XFFect)_a;

		val = Mathf.Lerp (val, a.val,blend);
	}
}
