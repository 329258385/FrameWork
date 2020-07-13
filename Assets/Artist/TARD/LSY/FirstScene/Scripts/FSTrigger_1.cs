using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FSTriggerGroup
{
	public float time = -1;
	public string ani;
	public List<GameObject> deactivate;
	public List<GameObject> gos;
	public List<GameObject> prefabs;
}

public class FSTrigger_1 : MonoBehaviour {
	[SerializeField]
	public List<FSTriggerGroup> fires;

	void Start()
	{
		fires.Sort (delegate(FSTriggerGroup x, FSTriggerGroup y) {
			return (int)((x.time - y.time)*100);
		});

		StartCoroutine (Ani ());
	}

	IEnumerator Ani()
	{
		for (int i = 0; i < fires.Count; i++) {
			//1 Wait
			float waitTime = 0;
			if (i == 0) {
				waitTime = fires [i].time;
			} else {
				waitTime = fires [i].time -fires [i-1].time;
			}
			yield return new WaitForSeconds(waitTime);


			//2 Create
			var item =  fires [i];
			foreach (var prefab in item.prefabs) {
				var go = GameObject.Instantiate (prefab);
				item.gos.Add (go);
			}
				

			//3 Active DeActive
			foreach (var f in item.gos) {
				if (f == null)
					continue;
				f.SetActive (true);
			}
			foreach (var f in item.deactivate) {
				if (f == null)
					continue;
				f.SetActive (false);
			}


			//4 Animator
			if (!string.IsNullOrEmpty (item.ani)) {
				foreach (var xx in item.gos) {
					Animator ani = xx.GetComponentInChildren<Animator> ();
					if (ani != null)
						ani.Play (item.ani);
				}
			}
		}
	}
}