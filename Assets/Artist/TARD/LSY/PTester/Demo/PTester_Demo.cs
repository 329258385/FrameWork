using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTester_Demo :PTester {
	protected override void Init ()
	{
		base.Init ();
		gameObject.AddComponent<TimeConsume> ();

		testGroup1_title = "G1";
		testGroup2_title = "G2";
		testGroup3_title = "G3";
	}

	protected override void OnGUI_Mission1 ()
	{
		base.OnGUI_Mission1 ();

		TesterGroup ("Test1","On", delegate() {
			TimeConsume.loop = 100;
		}, "Off", delegate() {
			TimeConsume.loop = 0;
		}, "Offx", delegate() {
			TimeConsume.loop = 0;
		});

	}

	protected override void OnGUI_Mission2 ()
	{
		base.OnGUI_Mission2 ();


		Tester ("Test2","T on", delegate() {
			TimeConsume.loop =100;
		});
	}

	protected override void OnGUI_Mission3 ()
	{
		base.OnGUI_Mission3 ();


		Tester ("Test3","T on", delegate() {
			TimeConsume.loop =100;
		});
	}
}
