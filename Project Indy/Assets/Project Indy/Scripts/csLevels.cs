//============================================================================
// Name        : csLevels.cs
// Author      : Temirlan Zhumanov
// Description : All Rights Reserved
//============================================================================

using UnityEngine;
using System.Collections;

public class csLevels : MonoBehaviour {

	public GameObject[] _levelsPrefab; // Prefabs of levels
	public Vector3[] _levelsDisplacement; // Actually width and height of levels
	public float[] _levelsTime; // Time to finish the level
	[HideInInspector]
	public GameObject[] _levels; // Levels on scene
	[HideInInspector]
	public bool[] _levelsActive; // Are the levels active

	[HideInInspector]
	public int _level; // Current level
	[HideInInspector]
	public bool _nextlevel = true; // Should level be changed to the next one
	
	/*private GameObject _GUI; // Container of GUI elements
	private GUIText _guiCheckpoint; // GUI of "Checkpoint" label
	private GUIText _guiSpeedUp; // GUI of "Speed Up" label
	private GUIText _guiScore; // GUI of "Score" label
	private GUIText _guiHighScore; // GUI of "High Score" label
	private GUIText _guiNewRecord; // GUI of "New Record" label*/
	
	private Vector3 _position;
	private Transform _camera;

	private static Vector3 _startPosition = Vector3.zero;
	private static float _minDifference = 15f;
	
	// Use this for initialization
	void Start () {
		/*_GUI = GameObject.Find("GUI").gameObject;
		_guiCheckpoint = _GUI.transform.Find("guiCheckpoint").guiText;
		_guiSpeedUp = _GUI.transform.Find("guiSpeedUp").guiText;
		_guiScore = _GUI.transform.Find("guiScore").guiText;
		_guiHighScore = _GUI.transform.Find("guiHighScore").guiText;
		_guiNewRecord = _GUI.transform.Find("guiNewRecord").guiText;

		_guiHighScore.text = "Best: "+PlayerPrefs.GetInt("_HS", 0);*/
		_position = _startPosition;
		_camera = Camera.main.transform;

		_level = -1;
		_levels = new GameObject[_levelsPrefab.Length];
		_levelsActive = new bool[_levelsPrefab.Length];
		for (int i = 0; i<_levelsPrefab.Length; i++)
		{
			_levels[i] = Instantiate(_levelsPrefab[i], -Vector3.right*100f, Quaternion.identity) as GameObject;
			_levels[i].transform.parent = transform;
			_levels[i].transform.name = "_level"+i;
			_levelsActive[i] = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// If change level
		if (_nextlevel)
		{
			LevelChange();
		}

		// Object Pooling
		if (_position.x-_camera.position.x <= _minDifference)
		{
			// Choose and activate level
			int _index = (int)Random.Range(0, _levelsPrefab.Length);
			int _startIndex = _index;
			while (_levelsActive[_index]!=false)
			{
				_index = (_index+1)%_levelsPrefab.Length;
				if (_index == _startIndex) break;
			}
			_levels[_index].transform.position = _position;
			_levelsActive[_index] = true;
			_position += _levelsDisplacement[_index];

			// Reset level
			Transform _objStart = _levels[_index].transform.Find("containerWalls").Find("objStartL");
			_objStart.gameObject.layer = 2;
			for (int i = 0; i<_levels[_index].transform.Find("containerCoins").childCount; i++)
			{
				GameObject _go = _levels[_index].transform.Find("containerCoins").GetChild(i).gameObject;
				_go.transform.GetComponent<Animator>().SetInteger("deleteCoin", 0);
				_go.collider.enabled = true;
				_go.transform.Find("_sprite").localPosition = Vector3.zero;
				_go.transform.Find("_sprite").renderer.material.SetColor("_Color", Color.white);
			}
		}
	}

	// Change level
	void LevelChange()
	{
		// Check that changed
		_nextlevel = false;

		// Increase level
		csPlayer._score += 100;
		csPlayer._time += _levelsTime[_level]/csPlayer._speedScale;
	
		if (_level % 10 == 0)
		{
			csPlayer._speedScale = Mathf.Min(csPlayer._speedScale+0.2f, 4f);
		}
			
		/*_guiCheckpoint.gameObject.SetActive(true);
		StartCoroutine(WaitAndDeactivate(2.0f*Time.timeScale, _guiCheckpoint.gameObject));*/
		
		// Procedures with new "objStart"
		Transform _objStart = transform.Find("_level"+_level).Find("containerWalls").Find("objStartL");
		_objStart.gameObject.layer = 0;
	}
	
	IEnumerator WaitAndDeactivate(float _waitTime, GameObject _go)
	{
        yield return new WaitForSeconds(_waitTime);
        _go.SetActive(false);
    }
}
