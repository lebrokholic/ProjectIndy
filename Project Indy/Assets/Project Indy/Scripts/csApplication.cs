//============================================================================
// Name        : csApplication.cs
// Author      : Temirlan Zhumanov
// Description : All Rights Reserved
//============================================================================

using UnityEngine;
using System.Collections;

public class csApplication : MonoBehaviour {

	public static int _MAINMENU = 0;
	public static int _GAME = 1;
	public static int _scene = _MAINMENU;

	public void ChangeScene(int _index)
	{
		_scene = _index;
		if (_index == _MAINMENU)
		{

		}
		else if (_index == _GAME)
		{
			(transform.Find("objPlayer").GetComponent("csPlayer") as csPlayer).enabled = true;
		}
	}
}
