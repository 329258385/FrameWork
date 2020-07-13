using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSkillShape_Quad : GrassSkillShape {
	public override void Play (Vector3 pos, Vector3 dir, GrassSkillInfo _info)
	{
		base.Play (pos, dir, _info);
		transform.position += transform.forward * info.scale.z*0.4f;
	}
	protected override void SetState (float pcg)
	{
		base.SetState (pcg);
		//301 front
		if (info.prefabID == "301") {
			transform.GetChild(0).localScale = new Vector3 (1, 1, pcg);
		}
		//302 back
		else if (info.prefabID == "302") {
			transform.GetChild(0).localScale = new Vector3 (1, 1, pcg);
		}
		//303 split up
		else if (info.prefabID == "303") {
			transform.localScale = new Vector3 (initScale.x*pcg, initScale.y, initScale.z);
		}

		GrassSkillFX.Instance.CreateQuad (this, info, pcg);
	}
}
