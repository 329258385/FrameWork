using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GG
{
	public List<GameObject> gos;
}

public class IntroTrigger_1 : MonoBehaviour {
	public float gap=0.2f;
	public float downh=-0.2f;

	[SerializeField]
	public List<GG> fires;

	void Awake()
	{
		for (int i = 0; i < fires.Count; i++) {
			var item =  fires [i];
			foreach (var f in item.gos) {
				f.SetActive (false);
			}
		}
	}
	void OnTriggerEnter(Collider col)
	{
		Debug.Log (col.name);
		StartCoroutine (Ani ());
	}

	IEnumerator Ani()
	{
		transform.position += new Vector3 (0, downh, 0);
		yield return null;
		for (int i = 0; i < fires.Count; i++) {
			var item =  fires [i];
			foreach (var f in item.gos) {
				f.SetActive (true);
			}
			yield return new WaitForSeconds(gap);
		}
	}
}
