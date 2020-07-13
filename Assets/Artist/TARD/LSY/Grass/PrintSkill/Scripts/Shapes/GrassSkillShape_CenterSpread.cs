using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSkillShape_CenterSpread : GrassSkillShape {
	public override void Play (Vector3 pos, Vector3 dir, GrassSkillInfo _info)
	{
		base.Play (pos, dir, _info);
		//Sector
		SetSector (info.angle1, info.angle2);
	}

	//Spread
	protected override void SetState (float pcg)
	{
		base.SetState (pcg);
		SetShaderValue (attribute, 0, 1, pcg);
		GrassSkillFX.Instance.CreateRad (this, info, pcg);
	}
}
