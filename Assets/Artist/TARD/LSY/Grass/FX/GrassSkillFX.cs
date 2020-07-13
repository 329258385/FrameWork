using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GrassSkillFX : MonoBehaviour {
	public static GrassSkillFX Instance;
	public static bool useRaycast = true;
	public LayerMask groundLayerMask;

	public Transform root;
	public GameObject prefab;
	private float height = 0.5f;
	int count = 3;
	float dura = 0.4f;

	void Awake()
	{
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
        Instance = this;
		var rootGo = new GameObject ("GarssFX Root");
		root = rootGo.transform;
		root.parent = transform;
	}

	float GetHeight(Vector3 pos,float orginHeight)
	{
		if (useRaycast) {
			RaycastHit info;
			if (Physics.Raycast (new Ray (new Vector3 (pos.x, orginHeight+30, pos.z), Vector3.down), out info, 100, groundLayerMask)) {
				return info.point.y+height;
			}
		}
		return orginHeight+height;
	}
//	Vector3 RaycastPos(Vector3 pos)
//	{
//		RaycastHit info;
//		if (Physics.Raycast (new Ray (pos+new Vector3 (0, 30, 0), Vector3.down),out info,100, groundLayerMask)) {
//			return info.point;
//		}
//		return pos;
//	}
	public void CreateRad(GrassSkillShape shape,GrassSkillInfo info, float pcg)
	{
		count = GetCount_Tri (info.scale,info.duration,info.angle1,info.angle2);

		float rad = info.scale.x*0.5f;
		rad *= pcg;
		for (int i = 0; i < count; i++) {
			Vector2 v = Random.insideUnitCircle;
			Vector3 dir = new Vector3 (v.x, 0, v.y);
			dir.Normalize ();

			float angleBe = Vector3.Angle (dir, shape.sectorDir);
			if (angleBe < shape.sectorRange) {
				Vector3 p = shape.transform.position+dir*rad;
				float h = GetHeight (p, shape.posOrigin.y);
				p = new Vector3 (p.x, h, p.z);
				CreateFX (shape,p);
			}
		}
	}
	public void CreateSwing(GrassSkillShape shape,GrassSkillInfo info, float pcg)
	{
		count = GetCount_Tri (info.scale,info.duration,info.angle1,info.angle2);

		float rad = info.scale.x*0.5f;
		rad *= Random.Range(0f,1f);
		for (int i = 0; i < count; i++) {
			Vector2 v = Random.insideUnitCircle;
			Vector3 dir = new Vector3 (v.x, 0, v.y);
			dir.Normalize ();

			float angleBe = Vector3.Angle (dir, shape.sectorDir);
			if (angleBe < shape.sectorRange) {
				Vector3 p = shape.transform.position+dir*rad;
				float h = GetHeight (p, shape.posOrigin.y);
				p = new Vector3 (p.x, h, p.z);
				CreateFX (shape,p);
			}
		}
	}
	public void CreateQuad(GrassSkillShape shape,GrassSkillInfo info, float pcg)
	{
		Transform tOri = null;
		Vector3 scale = Vector3.one;
		//301 front
		if (info.prefabID == "301") {
			scale = shape.transform.GetChild (0).localScale;

			tOri = shape.transform.GetChild (0).GetChild (0);
		}
		//302 back
		else if (info.prefabID == "302") {
			scale = shape.transform.GetChild (0).localScale;
			tOri = shape.transform.GetChild (0).GetChild (0);
		}
		//303 split up
		else if (info.prefabID == "303") {
			scale = shape.transform.localScale;
			tOri = shape.transform;
		}
		count = GetCount_Quad (info.scale,info.duration);

		for (int i = 0; i < count; i++) {
			Vector3 offset = RandomInQuad (scale);
			Vector3 p = tOri.position;
			p += tOri.forward * offset.z*0.5f;
			p += tOri.right * offset.x*0.5f;

			float h = GetHeight (p, shape.posOrigin.y);

			p = new Vector3 (p.x, h, p.z);
			CreateFX (shape, p);
		}
	}

	Vector3 RandomInQuad(Vector3 size)
	{
		Vector3 v = new Vector3 (Random.Range (-size.x, size.x), Random.Range (-size.y, size.y), Random.Range (-size.z, size.z));
		return v;
	}


	private void CreateFX(GrassSkillShape shape,Vector3 pos)
	{
        if (PengLuaKit.GameWorld.MainCamera == null)
            return;
        float thisDura = dura * Random.Range (1f, 1.5f);
		GameObject go = Instantiate (prefab) as GameObject;
		var t = go.transform;
		t.parent = root;
		t.position = pos;

		t.forward = PengLuaKit.GameWorld.MainCamera.transform.forward;
		t.localScale = Vector3.one * Random.Range (1f, 1.5f);

		Vector3 moveDir = pos - shape.posOrigin;
		moveDir = Vector3.up + moveDir;
		moveDir.Normalize ();
		t.DOMove (t.position + moveDir * Random.Range (0.9f, 1.5f), dura);
		t.GetChild (0).DOLocalRotate (new Vector3 (0, 0, Random.Range (-720, 720)), dura,RotateMode.LocalAxisAdd);
		t.DOScale (0, dura);
		Destroy (go, dura);

		SetMaterial (t);
	}

	void SetMaterial(Transform t)
	{
		
		if (GrassRenderer.Instance == null || GrassBuilder.Instance.settings.grassFXMat == null)
			return;
		var mr = t.GetComponentInChildren<MeshRenderer> ();
		mr.sharedMaterial = GrassBuilder.Instance.settings.grassFXMat;
	}

	int GetCount_Tri(Vector3 scale,float duration,float a1,float a2)
	{
		float angle = Mathf.Abs(a2-a1);
		if (angle < 1) {
			angle = 360;
		}
		float numAll = (scale.x * scale.z) * 5 * angle/360;
		return NumAllToNum (numAll,duration);
	}
	int GetCount_Quad(Vector3 scale,float duration)
	{
		float numAll = (scale.x * scale.z) * 2f;
		return NumAllToNum (numAll,duration);
	}

	int NumAllToNum(float numAll,float duration)
	{
		int num = (int)(numAll * (Time.deltaTime / duration));
		num = Mathf.Clamp (num,1, 10);
		//Debug.LogError ("num:"+num);
		return num;
	}
}
