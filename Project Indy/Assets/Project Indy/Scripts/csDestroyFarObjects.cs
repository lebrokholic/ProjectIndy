//============================================================================
// Name        : csDestroyFarObjects.cs
// Author      : Temirlan Zhumanov
// Description : All Rights Reserved
//============================================================================

using UnityEngine;
using System.Collections;

public class csDestroyFarObjects : MonoBehaviour {

	private Transform _camera;
	private static float _minDifference = 50f;

	private Vector3 HEAPPOSITION = -Vector3.right*100f;

	// Use this for initialization
	void Start () {
		_camera = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (_camera.position.x-transform.position.x >= _minDifference)
		{
			csLevels _comp = GameObject.Find("containerLevels").GetComponent("csLevels") as csLevels;
			int _index = int.Parse(transform.name.Substring(6));
			_comp._levelsActive[_index] = false;
			_comp._levels[_index].transform.position = HEAPPOSITION;
		}
	}
}
