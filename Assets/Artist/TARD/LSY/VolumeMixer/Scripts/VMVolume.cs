namespace VolumeMixer
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class VMVolume : MonoBehaviour {
		public float pcg;
		public bool isGlobal;
		[Range(0,1)]
		public float weight=1;
		public float blendDistance;
		public AnimationCurve curve = new AnimationCurve(new Keyframe[]{new Keyframe(0,0),new Keyframe(1,1)});

		protected VMValue vmValue;
		protected List<Collider> colliders;


		#region Add/Remove
		void Start()
		{
			vmValue = GetComponent<VMValue> ();
			colliders = new List<Collider> ();
			colliders.AddRange(gameObject.GetComponentsInChildren<Collider> ());
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


		#region Mix
		public float Mix(VMVolume a,Vector3 triggerPos)
		{
			float blend = a.CalBlend (triggerPos);
			blend = a.curve.Evaluate (blend);
			vmValue.Mix (a.vmValue, blend);
			return blend;
		}
		public float MixEvent(VMEvent a)
		{
			float blend = a.CalBlend();
			blend = a.curve.Evaluate (blend);
			vmValue.Mix (a.vmValue, blend);
			return blend;
		}

		public void Override(VMVolume a)
		{
			a.pcg = 1;
			vmValue.Mix (a.vmValue, 1);
		}

		public float CalBlend(Vector3 triggerPos)
		{
			if (isGlobal)
				return weight;

			if (colliders.Count == 0)
				return 0;

			// Find closest distance to volume, 0 means it's inside it
			float closestDistanceSqr = float.PositiveInfinity;
			foreach (var collider in colliders)
			{
				if (!collider.enabled)
					continue;

				var closestPoint = collider.ClosestPoint(triggerPos); // 5.6-only API
				var d = ((closestPoint - triggerPos) / 2f).sqrMagnitude;

				if (d < closestDistanceSqr)
					closestDistanceSqr = d;
			}
			//colliders.Clear();
			float blendDistSqr = blendDistance * blendDistance;

			// Volume has no influence, ignore it
			// Note: Volume doesn't do anything when `closestDistanceSqr = blendDistSqr` but
			//       we can't use a >= comparison as blendDistSqr could be set to 0 in which
			//       case volume would have total influence
			if (closestDistanceSqr > blendDistSqr)
				return 0;

			// Volume has influence
			float interpFactor = 1f;

			if (blendDistSqr > 0f)
				interpFactor = 1f - (closestDistanceSqr / blendDistSqr);

			return interpFactor * Mathf.Clamp01 (weight);
		}
		#endregion


		void OnDrawGizmos()
		{
			if (isGlobal || colliders == null)
				return;
			Gizmos.color = Color.blue;
			var scale = transform.localScale;
			var invScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);

			// Draw a separate gizmo for each collider
			foreach (var collider in colliders)
			{
				if (!collider.enabled)
					continue;
				var type = collider.GetType();

				if (type == typeof(BoxCollider))
				{
					var c = (BoxCollider)collider;
					Gizmos.DrawCube(c.center, c.size);
					Gizmos.DrawWireCube(c.center, c.size + invScale * blendDistance * 4f);
				}
				else if (type == typeof(SphereCollider))
				{
					var c = (SphereCollider)collider;
					Gizmos.DrawSphere(c.center, c.radius);
					Gizmos.DrawWireSphere(c.center, c.radius + invScale.x * blendDistance * 2f);
				}
				else if (type == typeof(MeshCollider))
				{
					var c = (MeshCollider)collider;

					// Only convex mesh colliders are allowed
					if (!c.convex)
						c.convex = true;

					// Mesh pivot should be centered or this won't work
					Gizmos.DrawMesh(c.sharedMesh);
					Gizmos.DrawWireMesh(c.sharedMesh, Vector3.zero, Quaternion.identity, Vector3.one + invScale * blendDistance * 4f);
				}
			}
			Gizmos.color = Color.white;
			//colliders.Clear();
		}
	}
}