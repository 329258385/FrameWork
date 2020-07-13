using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFrame : MonoBehaviour {

    public float UpdateInterval = 0.01f;
    private float _lastInterval;
    private int _frames;
    public float _fps;

    private float AUpdateInterval = 1.0f;
    private float _AFrames = 0.0f;
    private float Accum = 0.0f;
    public float _AFps;
    private float ATimeLeft;

    private static GetFrame Instance;

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
    }

    // Use this for initialization
    void Start () {
        //UpdateInterval = Time.realtimeSinceStartup;
        _frames = 0;
        InvokeRepeating("Record",0.1f,0.5f);
        DontDestroyOnLoad(gameObject);
	}

    private void LateUpdate()
    {
        ATimeLeft -= Time.deltaTime;
        Accum += Time.timeScale / Time.deltaTime;
        ++_AFrames;
        if (ATimeLeft <= 0.0f)
        {
            ATimeLeft = AUpdateInterval;
            Accum = 0.0f;
            _AFrames = 0.0f;
        }
    }
    void Record()
    {
        if (_AFrames != 0)
        {
            _AFps = Accum / _AFrames;
        }
    }
    // Update is called once per frame
    void Update () {
        ++_frames;
        if (Time.realtimeSinceStartup>_lastInterval+UpdateInterval)
        {
            _fps = _frames / (Time.realtimeSinceStartup - _lastInterval);
            _frames = 0;
            _lastInterval = Time.realtimeSinceStartup;
        }
	}
}
