using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill : MonoBehaviour {
	public GameObject prefab;
	public GameObject prefabFire;
	public float size;
	public float sizeBurn;
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.F)) {
			StartCoroutine (Print ());
		}
		if (Input.GetKeyDown (KeyCode.G)) {
			StartCoroutine (Burn ());
		}
	}

	IEnumerator Print()
	{
		GameObject go = Instantiate (prefab);
		go.transform.position = transform.position+ new Vector3(0,0.5f,0);

		float time = 0;
		float tMax = 0.3f;
		while (time < tMax) {
			time += Time.deltaTime;
			go.transform.eulerAngles = new Vector3(0,360*time/tMax,0);
			yield return null;
		}
		Destroy (go);
		GrassFP_Manager.Instance.AddPrint (go.transform.position,size);
	}
	IEnumerator Burn()
	{
		GameObject go = Instantiate (prefabFire);
		Vector3 v = transform.forward;
		v = new Vector3 (v.x * 3, 0, v.z * 3);
		go.transform.position = transform.position + v+ new Vector3(0,1.4f,0);
		yield return new WaitForSeconds(0.1f);
		GrassFP_Manager.Instance.AddBurn (go.transform.position,sizeBurn);
	}
}
