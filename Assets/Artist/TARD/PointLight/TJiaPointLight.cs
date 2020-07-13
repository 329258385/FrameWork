using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TJiaPointLight : MonoBehaviour {

    [ColorUsage(true,true)] public Color color = Color.white;
    public float range = 10;
    public float intensity = 1;
    [HideInInspector] public Mesh GizmoMesh;
    [HideInInspector] public Mesh LightGizmo;
    private TJiaPointLightManager Manager;
    private float mRandomPeriode;
    private Vector3 mOrigin;
    internal bool HeroLight = false;
    private bool TestHeroLight = false;

    // Use this for initialization
    void OnEnable () {
        Manager = FindObjectOfType<TJiaPointLightManager>();
        if (Manager == null)
        {
            Manager = new GameObject().AddComponent<TJiaPointLightManager>();
            Manager.PointLights = new List<TJiaPointLight>();
            Manager.PointLights.Add(this);
            Manager.name = "TJiaPointLightManager";
        }
        if (!Manager.PointLights.Contains(this))
        {
            Manager.PointLights.Add(this);
        }
        Manager.enabled = true;
        Manager.UpdateLights = true;
        mRandomPeriode = Random.Range(1f, 5f);
        mOrigin = transform.position;
        if (transform.root.GetComponentInChildren<Hero>()
            && transform.root.GetComponentInChildren<Hero>().transform.GetComponentInChildren<TJiaPointLight>()
            && transform.root.GetComponentInChildren<Hero>().transform.GetComponentInChildren<TJiaPointLight>() == this)
        {
            HeroLight = true;
            Shader.SetGlobalColor("_HeroLightColor", color);
            Shader.SetGlobalVector("_HeroLightRangeAndIntensity", new Vector2(range, intensity));
        }
    }

    private void OnDisable()
    {
        if (Manager.PointLights.Contains(this))
        {
            Manager.PointLights.Remove(this);
        }
        Manager.UpdateLights = true;
    }

    // Update is called once per frame
    void Update () {
        if (Manager == null || !Manager.PointLights.Contains(this))
        {
            OnEnable();
        }
        //transform.position = mOrigin + Vector3.one * Mathf.Sin(Time.time * mRandomPeriode) * 0.5f;
        if (!HeroLight && TestHeroLight == false && Time.timeSinceLevelLoad > 5)
        {
            TestHeroLight = true;
            if (transform.root.GetComponentInChildren<Hero>()
                && transform.root.GetComponentInChildren<Hero>().transform.GetComponentInChildren<TJiaPointLight>()
                && transform.root.GetComponentInChildren<Hero>().transform.GetComponentInChildren<TJiaPointLight>() == this)
            {
                HeroLight = true;
                Shader.SetGlobalColor("_HeroLightColor", color);
                Shader.SetGlobalVector("_HeroLightRangeAndIntensity", new Vector2(range, intensity));
            }
        }
        if (HeroLight)
        {
            Shader.SetGlobalVector("_HeroLightPos", transform.position);
        }
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawMesh(LightGizmo, transform.position, Quaternion.identity, Vector3.one);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireMesh(GizmoMesh,transform.position,Quaternion.identity,Vector3.one * range * 2); 
    }
}
