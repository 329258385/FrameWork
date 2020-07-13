using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassFP_Manager : MonoBehaviour {
	public static float BasicHeight = 10000;

	public static GrassFP_Manager Instance;
	public Transform player;
	public Vector4 aabb;
	public Camera cam;

	private GrassFP_Printer printer;
	private GrassFP_Trailer trailer;
	private GrassFP_Burner burner;
	private RenderTexture rt;

	void Awake()
	{
#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
        return;
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
		printer = GetComponentInChildren<GrassFP_Printer> ();
		trailer = GetComponentInChildren<GrassFP_Trailer> ();
		burner = GetComponentInChildren<GrassFP_Burner> ();

	
		rt = new RenderTexture (sizeTier[TARDSwitches.Quality], sizeTier[TARDSwitches.Quality], 0, RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear);
		rt.name = "Lsy RT-GrassFP";
		rt.useMipMap = false;	
		rt.autoGenerateMips = false;
		rt.anisoLevel = 0;
		cam.targetTexture = rt;
		Shader.SetGlobalTexture ("_footprintTex", rt);
		//Shader.SetGlobalFloat ("_fpSpread", 0.9f);

        DontDestroyOnLoad (this);
	}
	void Update () {
#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
        var mainCam = Camera.main;
		if (mainCam != null)
			mainCam.cullingMask &= ~(1 << 9);

		if (player == null) {
			player = LsyCommon.FindPlayer ();
		}

		if (player == null)
			return;
		transform.position = new Vector3(player.position.x,BasicHeight,player.position.z);

		//No rotation for aabb yet
		//transform.eulerAngles = new Vector3 (0, player.eulerAngles.y, 0); 

		var p = transform.position;
		aabb = new Vector4 (
			p.x-cam.orthographicSize,
			p.x+cam.orthographicSize,
			p.z-cam.orthographicSize,
			p.z+cam.orthographicSize);

		Shader.SetGlobalVector ("_footprintAABB", aabb);
	}


	void OnDrawGizmosSelected()
	{
#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
        float f = aabb.y - aabb.x;
		Gizmos.DrawCube (transform.position,new Vector3(f,0.1f,f));
	}

	public bool AddPrint(Vector3 pos,float size)
    {
        if (!IsCut(pos) && IsOnGrass(pos))
        {
            printer.Add(pos, size);

            return true;
        }
        else
        {
            return false;
        }
	}
	public bool AddBurn(Vector3 pos,float size)
	{
        if (IsOnGrass(pos))
        {
            burner.Add(pos, size);

            return true;
        }
        else
        {
            return false;
        }
	}

	public bool IsOnGrass(Vector3 pos)
	{
	//	GrassRenderer.Instance.IsOnGrass (pos);

		var mRay = new Vector3 (pos.x, pos.y + 500, pos.z);
		RaycastHit mHitInfo;
		if (Physics.Raycast (mRay,Vector3.down, out mHitInfo, 2000,1 << 28 | 1 << 17)) {
			TerrainGrassInfo info = mHitInfo.collider.GetComponentInParent<TerrainGrassInfo> ();
			if (info == null || info.grassTex ==null)
				return false;
			var mPos = mHitInfo.point;
			var texUV = mHitInfo.textureCoord;
			var channel = info.grassTex.GetPixel ((int)(texUV.x * (float)info.width), (int)(texUV.y * (float)info.height)).r;

			//Return true just when it is possible to have grass
			if (channel > 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsCut(Vector3 pos)
	{
		return false;
//		var rt = cam.targetTexture;
//		Vector2 uv = AABB_UV (pos);
//		int x = (int)(uv.x * rt.width);
//		int y = (int)(uv.y * rt.height);
//		x = Mathf.Clamp (x, 0, rt.width);
//		y = Mathf.Clamp (y, 0, rt.height);
//
//		Color color = RTColorCapturer.Capture (rt,x,y);
//		bool c = color.b <= 0.175f;
//		return c;
	}

	Vector2 AABB_UV(Vector3 worldPos)
	{
		Vector4 ab = aabb;
		float uv_x = (worldPos.x - ab.x)/(ab.y - ab.x);
		float uv_y = (worldPos.z - ab.z)/(ab.w - ab.z);
		return new Vector2(uv_x,uv_y);
	}

	public void AddPrint_Skill(Vector3 pos,Vector3 dir,GrassSkillData data)
	{
		GrassSkillCut.Instance.PlaySkill (pos, dir, data);
	}


	#region Quality
	//512抖动太厉害,且脚踩幅度有较大变化，因此固定为1024
	public static int[] sizeTier = { 1024, 1024, 1024 };

	public void SetQuality(int q)
	{
		//Can not modify under runtime
		//rt.width = sizeTier [q];
		//rt.height = sizeTier [q];
	}
	#endregion
}
