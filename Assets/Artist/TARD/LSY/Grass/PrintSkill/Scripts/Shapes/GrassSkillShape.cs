using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSkillShape : MonoBehaviour {
	public string attribute;
	protected Material mat;
	protected GrassSkillInfo info;
	protected Vector3 initScale;

	public Vector3 posOrigin;
	public Vector3 sectorDir;
	public float sectorRange;
    private Coroutine playCoroutine;

    public virtual void Play(Vector3 pos,Vector3 dir,GrassSkillInfo _info)
	{
        Reset();
        posOrigin = pos;
		info = _info;
		transform.position = new Vector3(pos.x,GrassFP_Manager.BasicHeight+1.5f,pos.z);
		transform.localEulerAngles = dir;
		transform.localScale = info.scale;
		transform.position += transform.forward * info.offset.y;
		transform.position += transform.right * info.offset.x;
			
		initScale = transform.localScale;
		mat = GetComponentInChildren<MeshRenderer> ().material;
        playCoroutine = StartCoroutine(IEPlay ());
	}

	private IEnumerator IEPlay()
	{
		yield return null;
		float time = 0;
		while (time < info.delay) {
			time += Time.deltaTime;
			yield return null;
		}
		time = 0;

		while (time < info.duration) {
			time += Time.deltaTime;
			float pcg = time / info.duration;
			SetState (pcg);
			yield return null;
		}
		SetState (1);
        playCoroutine = null;
    }


	protected virtual void SetState(float pcg)
	{
		
	}



	protected void SetShaderValue(string attribute,float start,float end,float pcg)
	{
		float v = start + (end - start) * pcg;
		mat.SetFloat (attribute,v);
	}

	protected void SetSector(float angle1, float angle2)
	{
		float a = (angle1 + angle2) * 0.5f;
		float angle = Mathf.Abs(angle1 - angle2) * 0.5f;
		Vector3 v = Quaternion.Euler (0, a, 0) * Vector3.forward;
		mat.SetVector ("_Dir", new Vector4 (v.x, v.z, 0, 0));
		mat.SetFloat ("_Angle", angle*Mathf.Deg2Rad);

		sectorDir = Quaternion.Euler (0, a, 0) *transform.forward;
		sectorRange = angle;
	}

    protected void Reset()
    {
        if(playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }
    }
}
