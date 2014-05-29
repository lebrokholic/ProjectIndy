//============================================================================
// Name        : csCamera.cs
// Author      : Temirlan Zhumanov
// Description : All Rights Reserved
//============================================================================

using UnityEngine;
using System.Collections;

public class csCamera : MonoBehaviour {

	public Transform _target; // Player
	public float _smoothTime = 0.2f; // Time for smooth
	
	private Vector3 _cameraDisplacement = 2*Vector3.up; // Displacement related to the target
	private Transform _transform; // Transform of camera
	private Vector2 _velocity; // Vector for SmoothDamp
	
	private const float _epsilon = 0.01f; // Calculation errors control
	private const float _zposition = -30f; // Z-coordinate of camera's position
	
	// Use this for initialization
	void Start () {
		_transform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		// Follow the target
		_transform.position = new Vector3(
			Mathf.SmoothDamp(_transform.position.x, _target.position.x+_cameraDisplacement.x, ref _velocity.x, _smoothTime/csPlayer._speedScale),
			Mathf.SmoothDamp(_transform.position.y, _target.position.y+_cameraDisplacement.y, ref _velocity.y, _smoothTime/csPlayer._speedScale),
			_zposition);	
	}
}
