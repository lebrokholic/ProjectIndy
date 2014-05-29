//============================================================================
// Name        : csInput.cs
// Author      : Temirlan Zhumanov
// Description : All Rights Reserved
//============================================================================

using UnityEngine;
using System.Collections;

public class csInput : MonoBehaviour {

	public TextMesh _guiPlay;
	public TextMesh _guiQuit;

	private Transform _player; // Transform of player
	private GameObject _GUI; // Container of GUI elements
	private GUIText _guiPause; // GUI of "Pause" button
	
	private const float _epsilon = 0.01f; // Calculation errors control

	// Use this for initialization
	void Start () {
		_player = GameObject.Find("objPlayer").transform;
		_GUI = GameObject.Find("GUI").gameObject;
		_guiPause = _GUI.transform.Find("guiPause").guiText;
	}
	
	// Update is called once per frame
	void Update () {
		// PC/Mac Input
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		if (csApplication._scene == csApplication._GAME)
		{
			if (Input.GetKeyDown("p")) inputPause(); // Pause
			if (Input.GetKeyDown("space")) inputJump(); // Jump
			_guiPlay.color = Color.white;
			_guiQuit.color = Color.white;
		}
		else if (csApplication._scene == csApplication._MAINMENU)
		{
			Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit _hit;
			if (Physics.Raycast(_ray, out _hit, Mathf.Infinity))
			{
				switch (_hit.transform.name)
				{
				case "_guiPlay":
					if (Input.GetMouseButtonDown(0))
						(GameObject.Find("Application").GetComponent("csApplication") as csApplication).ChangeScene(csApplication._GAME);
					else
						_guiPlay.color = Color.gray;
					break;
				case "_guiQuit":
					if (Input.GetMouseButtonDown(0))
						Application.Quit();
					else
						_guiQuit.color = Color.gray;
					break;
				default:
					_guiPlay.color = Color.white;
					_guiQuit.color = Color.white;
					break;
				}
			}
		}
		if (Input.GetKeyDown("s")) (GameObject.Find("Application").GetComponent("csApplication") as csApplication).ChangeScene(csApplication._GAME);
		#endif

		// Smartphones and Tablets Input
		#if UNITY_IPHONE || UNITY_ANDROID
		if (Input.touchCount > 0 )
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch _touch = Input.GetTouch(i);
				if (csApplication._scene == csApplication._GAME)
				{
					if (_touch.phase == TouchPhase.Began && _guiPause.HitTest(_touch.position))
					{
						inputPause(); // Pause
					}
					else if (_touch.phase == TouchPhase.Began) // Bug Fix (double jump on high speed)
					{
						inputJump(); // Jump
					}
				}
				else if (csApplication._scene == csApplication._MAINMENU)				
				{
					int j = 0;
					while (j < Input.touchCount)
					{
						if (Input.GetTouch(j).phase == TouchPhase.Began)
						{
							Ray _ray = Camera.main.ScreenPointToRay(Input.GetTouch(j).position);
							RaycastHit _hit;
							if (Physics.Raycast(_ray, out _hit, Mathf.Infinity))
							{
								switch (_hit.transform.name)
								{
									case "_guiPlay":
										(GameObject.Find("Application").GetComponent("csApplication") as csApplication).ChangeScene(csApplication._GAME);
										break;
									case "_guiQuit":
										Application.Quit();
										break;
									case "":
										
										break;
								}
							}
						}
						++j;
					}
				}
			}
		}
		#endif
		
		// Common
		if (csApplication._scene == csApplication._GAME)
		{
			if (Input.GetKeyDown("escape"))
			{
				if (csApplication._scene == csApplication._MAINMENU) Application.Quit();
				else if (csApplication._scene == csApplication._GAME) (GameObject.Find("Application").GetComponent("csApplication") as csApplication).ChangeScene(csApplication._MAINMENU);
			}
		}
	}
	
	// Pause Game
	void inputPause()
	{
		if (Time.timeScale > _epsilon)
		{
			_guiPause.text = "Paused";
			Time.timeScale = 0;
		}
		else
		{
			_guiPause.text = "Pause";
			Time.timeScale = 1.0f;
		}
	}
	
	// Jumping of player
	void inputJump()
	{
		if (Time.timeScale > _epsilon)
		{
			csPlayer _comp = _player.GetComponent("csPlayer") as csPlayer;
			_comp._jump = true;
			_comp._jumpButtonTime = Time.time;
		}
	}
}
