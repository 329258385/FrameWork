namespace VolumeMixer
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class VMManager:MonoBehaviour{
		public static VMManager Instance;

		protected VMVolume v;
		protected List<VMVolume> volumes;
		protected VMEvent events;
		protected Transform player;
		protected List<VMVolume> settedVolumes;

		public void Awake()
		{
			Instance = this;
			v = GetComponent<VMVolume> ();
			volumes = new List<VMVolume>();
			settedVolumes = new List<VMVolume> ();
		}

		public void SetSettedVolumes(float rate)
		{
			for (int i = 0; i < settedVolumes.Count; i++) {
				settedVolumes [i].pcg *= rate;
			}
		}
		public void Regi(VMEvent _v)
		{
			events = _v;
		}
		public void UnRegi(VMEvent _v)
		{
			events = null;
		}
		public void Regi(VMVolume _v)
		{
			if (v == _v)
				return;
			if (volumes.Contains (_v))
				return;
			
			if (_v.isGlobal)
				volumes.Insert (0, _v);
			else
				volumes.Add (_v);
		}
		public void UnRegi(VMVolume _v)
		{
			if (v == _v)
				return;
			if(volumes.Contains(_v))
				volumes.Remove (_v);
		}

		public void Update()
		{
			player = LsyCommon.FindPlayer ();
			if(player == null)
				player = Camera.main.transform;
			UpdateVolume ();
			UpdateEvent ();
		}
		public void UpdateVolume()
		{
			if (volumes.Count == 0)
				return;
			settedVolumes.Clear ();
			var triggerPos = Vector3.zero;
			triggerPos = player.position;

			for (int i = 0; i < volumes.Count; i++) {
				var volume = volumes [i];
				if (!volume.isActiveAndEnabled || volume.weight <= 0f)
					continue;

				if (i == 0) {
					v.Override (volume);
					settedVolumes.Add (volume);
					SetSettedVolumes (1);
				} else {
					float blend = v.Mix (volume, triggerPos);
					SetSettedVolumes (1-blend);
					volume.pcg = blend;
				}
			}
		}


		public void UpdateEvent()
		{
			if (events == null || !events.IsOn())
				return;
			float blend = v.MixEvent (events);
			SetSettedVolumes (1-blend);
			events.pcg = blend;
		}
	}
}