using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAniParams
{
	public float activeTime=-1f;
	public float startIntensity;
	public float startIntensityCB;
}

public class LightAni : MonoBehaviour {
	public List<Light> lights;
	public List<CB_WorkerBase> workers;
	public List<LightAniParams> lightParams;
	public AnimationCurve curveAppear;
	public AnimationCurve curve;
	[Range(0,1)]
	public float durationAppear;
	[Range(0,10)]
	public float duration;
	[Range(0,10)]
	public float amplitude;
	[Range(0,10)]
	public float amplitudeWorker;
	protected float startIntensity;
	// Use this for initialization
	void Start () {
		if (!FSManager.IsON)
			return;
		
		lightParams = new List<LightAniParams> ();
		for (int i = 0; i < lights.Count; i++) {
			var p = new LightAniParams ();
			lightParams.Add (p);
			var l = lights [i];
			var w = workers [i];
			Init (l, w, p);
		}
	}

	void Update () {
		if (!FSManager.IsON)
			return;

		for (int i = 0; i < lights.Count; i++) {
			var l = lights [i];
			var w = workers [i];
			var p = lightParams[i];

			if (l.gameObject.activeInHierarchy) {
				Show (l, w, p);
				UpdateIntensity (l, w, p);
			}
		}
	}

	void Init(Light l,CB_WorkerBase w,LightAniParams p)
	{
		p.startIntensity = l.intensity;
		p.startIntensityCB = w.matParams [0].internsity;
		l.intensity = 0;
		w.matParams [0].internsity=0;
	}
	void Show(Light l,CB_WorkerBase w,LightAniParams p)
	{
		if (p.activeTime < 0) {
			p.activeTime = Time.time;
		}
	}

	void UpdateIntensity(Light l,CB_WorkerBase w,LightAniParams p)
	{
		float intensityMod = 0;
		if (Time.time - p.activeTime < durationAppear) {
			intensityMod = curveAppear.Evaluate ((Time.time - p.activeTime) / durationAppear);
			SetLightsIntensity (l, w, p, intensityMod,1,1);
		} else {
			float t = Time.time - p.activeTime;
			intensityMod = curve.Evaluate (t * 1/duration);
			SetLightsIntensity (l, w, p, intensityMod,amplitude,amplitudeWorker);
		}
	}
	void SetLightsIntensity(Light l,CB_WorkerBase w,LightAniParams p,float intensityMod,float amplitude,float amplitudeWorker)
	{
		l.intensity = p.startIntensity * Mathf.Lerp (1, intensityMod, amplitude);
		w.matParams[0].internsity = p.startIntensityCB * Mathf.Lerp (1, intensityMod, amplitudeWorker);
		w.MatSyncAll ();
	}
}




//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public struct LightAniParams
//{
//	public float curveStartTime;
//	public float startIntensity;
//	public float startIntensityCB;
//}
//
//public class LightAni : MonoBehaviour {
//	public List<Light> lights;
//	public List<CB_WorkerBase> workers;
//	public List<LightAniParams> lightParams;
//	public AnimationCurve curveAppear;
//	public AnimationCurve curve;
//	[Range(0,10)]
//	public float duration;
//	[Range(0,10)]
//	public float amplitude;
//	[Range(0,10)]
//	public float amplitudeWorker;
//	protected float startIntensity;
//	// Use this for initialization
//	void Start () {
//		lightParams = new List<LightAniParams> ();
//		for (int i = 0; i < lights.Count; i++) {
//			var p = new LightAniParams ();
//			p.curveStartTime = Random.Range (0f, 1f);
//			p.startIntensity = lights [i].intensity;
//			p.startIntensityCB = workers [i].matParams [0].internsity;
//			lightParams.Add (p);
//		}
//	}
//		
//	void Update () {
//		for (int i = 0; i < lights.Count; i++) {
//			var p = lightParams[i];
//			var inten =  curve.Evaluate (p.curveStartTime+Time.time * 1/duration);
//			lights [i].intensity = p.startIntensity * Mathf.Lerp (1, inten, amplitude);;
//			workers[i].matParams[0].internsity = p.startIntensityCB * Mathf.Lerp (1, inten, amplitudeWorker);;
//		}
//	}
//}





//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class LightAni : MonoBehaviour {
//	public Light _light;
//	public AnimationCurve curve;
//	public float duration;
//	protected float startIntensity;
//	// Use this for initialization
//	void Start () {
//		startIntensity = _light.intensity;
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		_light.intensity = startIntensity * curve.Evaluate (Time.time * 1/duration);
//	}
//}
