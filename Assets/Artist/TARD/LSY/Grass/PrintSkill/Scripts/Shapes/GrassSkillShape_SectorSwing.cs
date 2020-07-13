using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSkillShape_SectorSwing : GrassSkillShape {
	protected override void SetState (float pcg)
	{
		base.SetState (pcg);
		SetAngle (pcg);

		GrassSkillFX.Instance.CreateSwing (this, info, pcg);
	}
	void SetAngle(float pcg)
	{
		float angle1 = 0;
		float angle2 = 0;
		if (info.prefabID == "201") {
			angle1 = info.angle1;
			angle2 = angle1 + (info.angle2 - info.angle1) * pcg;
		}
		else if (info.prefabID == "202") {
			angle2 = info.angle2;
			angle1 = angle2 + (info.angle1 - info.angle2) * pcg;
		}
		SetSector (angle1, angle2);
	}
}
