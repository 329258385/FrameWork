using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEventToParent : MonoBehaviour {
	public void OnCollisionEnter(Collision col)
	{
		if(transform.parent!=null)
			transform.parent.SendMessageUpwards("OnCollisionEnter",col,SendMessageOptions.DontRequireReceiver);
	}
	public void OnCollisionStay(Collision col)
	{
		if(transform.parent!=null)
			transform.parent.SendMessageUpwards("OnCollisionStay",col,SendMessageOptions.DontRequireReceiver);
	}
	public void OnCollisionExit(Collision col)
	{
		if(transform.parent!=null)
			transform.parent.SendMessageUpwards("OnCollisionExit",col,SendMessageOptions.DontRequireReceiver);
	}
		
	public void OnTriggerEnter(Collider col)
	{
		if(transform.parent!=null)
			transform.parent.SendMessageUpwards("OnTriggerEnter",col,SendMessageOptions.DontRequireReceiver);
	}
	public void OnTriggerStay(Collider col)
	{
		if(transform.parent!=null)
			transform.parent.SendMessageUpwards("OnTriggerStay",col,SendMessageOptions.DontRequireReceiver);
	}
	public void OnTriggerExit(Collider col)
	{
		if(transform.parent!=null)
			transform.parent.SendMessageUpwards("OnTriggerExit",col,SendMessageOptions.DontRequireReceiver);
	}
}
