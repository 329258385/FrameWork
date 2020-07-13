namespace VolumeMixer
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using DG.Tweening;
	public class VMEvent : MonoBehaviour {
		public float pcg;
		public float duration;
		public float blend;
		public AnimationCurve curve = new AnimationCurve(new Keyframe[]{new Keyframe(0,0),new Keyframe(1,1)});

		public VMValue vmValue;


		#region Add/Remove
		void Start()
		{
			blend = 0;
			vmValue = GetComponent<VMValue> ();
			Add ();
		}
		void OnDestroy()
		{
			Remove ();
		}
		void Add()
		{
			if(VMManager.Instance!=null)
				VMManager.Instance.Regi (this);
		}
		void Remove()
		{
			if(VMManager.Instance!=null)
				VMManager.Instance.UnRegi (this);
		}
		#endregion

		public void Play()
		{
			DOTween.To(()=> blend,x=>blend=x,1,duration);
		}

		public void Kill()
		{
			DOTween.To(()=> blend,x=>blend=x,0,duration);
		}
		public bool IsOn()
		{
			return blend > 0;
		}
		public float CalBlend()
		{
			return blend;
		}
	}
}