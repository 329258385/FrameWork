using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTester_FPS : MonoBehaviour {
	const float fpsMeasurePeriod = 0.5f;
	private int m_FpsAccumulator = 0;
	private float m_FpsNextPeriod = 0;

	public static int FPS;
	public static float MS;


	private void Start()
	{

		m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
	}
	private void Update()
	{
		// measure average frames per second
		m_FpsAccumulator++;
		if (Time.realtimeSinceStartup > m_FpsNextPeriod)
		{
			FPS = (int) (m_FpsAccumulator/fpsMeasurePeriod);
			m_FpsAccumulator = 0;
			m_FpsNextPeriod += fpsMeasurePeriod;
		}
			
		MS = 1 / (float)FPS*1000f;
	}
}
