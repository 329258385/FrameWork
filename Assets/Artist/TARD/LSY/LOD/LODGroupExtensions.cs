namespace LsyLOD
{
	using UnityEngine;

	public static class LODGroupExtensions
	{
		public static int GetMaxLOD(this LODGroup lodGroup)
		{
			return lodGroup.lodCount - 1;
		}

		public static int GetCurrentLOD(this LODGroup lodGroup, Camera camera = null)
		{
			var lods = lodGroup.GetLODs();
			var relativeHeight = lodGroup.GetRelativeHeight(camera ?? Camera.current);

			var lodIndex = GetCurrentLOD(lods, lodGroup.GetMaxLOD(), relativeHeight, camera);

			return lodIndex;
		}

		static int GetCurrentLOD(LOD[] lods, int maxLOD, float relativeHeight, Camera camera = null)
		{
			var lodIndex = maxLOD;

			for (var i = 0; i < lods.Length; i++)
			{
				var lod = lods[i];

				if (relativeHeight >= lod.screenRelativeTransitionHeight)
				{
					lodIndex = i;
					break;
				}
			}

			return lodIndex;
		}


	    public static float GetWorldSpaceSize(this LODGroup lodGroup)
	    {
	        return GetWorldSpaceScale(lodGroup.transform) * lodGroup.size;
	    }
		

		public static float GetWorldSpaceScale(Transform t)
	    {
	        var scale = t.lossyScale;
	        float largestAxis = Mathf.Abs(scale.x);
	        largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.y));
	        largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.z));
	        return largestAxis;
	    }

		public static float GetRelativeHeight(this LODGroup lodGroup, Camera camera)
	    {
	        var distance = (lodGroup.transform.TransformPoint(lodGroup.localReferencePoint) - camera.transform.position).magnitude;
	        return DistanceToRelativeHeight(camera, distance, lodGroup);
	    }

		public static float DistanceToRelativeHeight(Camera camera, float distance, LODGroup group)
		{
			float size = group.GetWorldSpaceSize ();
			if (camera.orthographic)
				return size * 0.5F / camera.orthographicSize;

			var halfAngle = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5F);
			var relativeHeight = size * 0.5F / (distance * halfAngle);
			return relativeHeight;
		}
		public static float RelativeHeightToDistance(Camera camera, float relativeHeight,  LODGroup group)
		{
			float size = group.GetWorldSpaceSize ();
			if (camera.orthographic)
				return 0;

			var halfAngle = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5F);
			var distance = size * 0.5F / (relativeHeight * halfAngle);
			return distance;
		}
	}
}
