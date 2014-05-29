//============================================================================
// Name        : csPlayer.cs
// Author      : Temirlan Zhumanov
// Description : All Rights Reserved
//============================================================================

using UnityEngine;
using System.Collections;

public class csPlayer : MonoBehaviour {

	public static float _speedScale; // Scale of player's speed
	public static int _score; // Player's current score amount
	public static int _lives; // Player's lives number
	public static float _time; // Player's remained time

	public Vector3 _velocity; // Movement velocity (horizontal, jump, none)

	public TextMesh _guiScore;
	public TextMesh _guiTimer;
	
	[HideInInspector]
	public Vector3 _direction; // Horizontal direction of movement (right or left)
	[HideInInspector]
	public bool _jump = false; // Should the player jump
	[HideInInspector]
	public float _jumpButtonTime = 0; // Time when "Jump" button pressed
	
	private float _semiheight; // Half of the height
	private float _semiwidth; // Half of the width
	private bool _grounded = false; // Is on ground
	private float _jumpButtonDelay = 0.2f; // Allow to press "Jump" before grounding
	
	private Animator _animator; // Animator of texture
	private int _animationIndex = 0; // Index of playing animation

	private Transform _transform; // Transform of player
	private Rigidbody _rigidbody; // Rigidbody of player
	private TrailRenderer _trail; // Trail renderer of player
	
	private const float _epsilon = 0.01f; // Calculation errors control
	
	// Use this for initialization
	void Start () {
		// Set up initial values
		_transform = transform;
		_rigidbody = rigidbody;
		_direction = Vector3.right;
		_semiheight = _transform.localScale.y/2;
		_semiwidth = _transform.localScale.x/2;
		_animator = _transform.Find("_texture").GetComponent<Animator>();
	 	_trail = _transform.Find("_trail").GetComponent("TrailRenderer") as TrailRenderer;
			
		// Unpause the game
		Time.timeScale = 1.0f;

	 	// Player characteristics
	 	_speedScale = 1.0f;
	 	_score = 0;
	 	_lives = 1;
		_time = 10f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//-------------------------
		// OPTIMIZE
		// - Once remember transform.position and rigidbody.velocity
		//-------------------------

		if (Time.timeScale > _epsilon) // If not pause
		{
			// New velocity of rigidbody
			Vector3 _newVelocity;
		
			// Collision with walls
	 	 	float _float = _semiwidth+Mathf.Abs(_rigidbody.velocity.x)*Time.fixedDeltaTime*1.1f;
	 	 	Vector3 _vector3 = Vector3.up*(_semiheight-0.2f);
			if (Physics.Raycast(_transform.position+_vector3, _direction, _float)) Flip();
				else if (Physics.Raycast(transform.position-_vector3, _direction, _float)) Flip();
				
	 	 	// Collision with ceiling
			if (!_grounded &&
			    Physics.Raycast(_transform.position, Vector3.up, _semiheight+
			    	Mathf.Abs(_rigidbody.velocity.y)*Time.fixedDeltaTime))
						_rigidbody.velocity = Vector3.right*_rigidbody.velocity.x;
			
			// Movement
			_newVelocity = Vector3.right*_direction.x*_velocity.x*_speedScale + Vector3.up*_rigidbody.velocity.y;
			_transform.localEulerAngles = Vector3.up*((1-_direction.x)*90f);
	 	 	Physics.gravity = -Vector3.up*9.81f*_speedScale*_speedScale;
			if (_newVelocity.x!=_rigidbody.velocity.x && Mathf.Sign(_newVelocity.x)==Mathf.Sign(_rigidbody.velocity.x))
				_newVelocity = Vector3.Slerp(rigidbody.velocity, _newVelocity, 0.2f);
			 
			// Is grounded
			if (Physics.Raycast(_transform.position-0.0f*_direction, -Vector3.up, _semiheight)) _grounded = true;
				else _grounded = false;
			
			// Update velocity of rigidbody
			_rigidbody.velocity = _newVelocity;
			
			// Jump
			if (Time.time-_jumpButtonTime <= _jumpButtonDelay*Time.timeScale)
			{
				if (_jump && _grounded)
				{
					Jump();
				}
			}
			else _jump = false;
			
			// Wind
			Collider[] _hitColliders = Physics.OverlapSphere(_transform.position-3f*_semiwidth*_direction, _semiwidth);
			int i = 0;
			while (i < _hitColliders.Length)
			{
				if (_hitColliders[i].tag == "_windAffectable")
				{
					_hitColliders[i].rigidbody.AddForce(_direction * 1000f);
				}
				i++;
			}
			
			// Trail
			_trail.time = Mathf.Lerp(_trail.time, Mathf.Max(Mathf.Min(0.5f, (_speedScale-1f)/10), 0f), 0.2f);
			
			// Animation
			if (_grounded && _animationIndex!=1)
			{
				_animationIndex = 1;
				_animator.SetInteger("_index", 1);
			}
			else if (!_grounded && _animationIndex!=2)
			{
				_animationIndex = 2;
				_animator.SetInteger("_index", 2);
			}

			// GUI Score
			_guiScore.text = ""+_score;
			if (_score > PlayerPrefs.GetInt("_HS", 1))
			{
				PlayerPrefs.SetInt("_HS", _score);
			}
			
			// GUI Time
			_guiTimer.text = ""+(int)_time/600+((int)_time/60)%10+":"+((int)_time%60)/10+((int)_time%60)%10;
			_time = Mathf.Max(0, _time-Time.deltaTime);
			if (_time == 0)
			{
				GameOver();
			}

			// GUI Game
			Transform _guiGame = GameObject.Find("Application").transform.Find("GUIGame");
			if (_guiGame)
			{
				if (_guiGame.position.x <= Camera.main.transform.position.x)
				{
					_guiGame.parent = Camera.main.transform;
					_guiGame.localPosition = Vector3.zero;
				}
			}
		}
	}
	
	// Change horizontal direction
	void Flip()
	{
		_direction = -_direction;
	}
	
	// Jumping of the player
	void Jump()
	{
		_jump = false;
		_jumpButtonTime = 0f;
		_rigidbody.velocity = Vector3.right*_rigidbody.velocity.x + Vector3.up*_velocity.y*_speedScale;
	}

	// Game Over
	void GameOver()
	{
		Time.timeScale = 0f;
		//Camera.main.transform.Find("_guiGameOver").gameObject.SetActive(true);
	}
	
	// Triggering with objects (exit)
	void OnTriggerExit(Collider other)
	{
		// With new level's start
        if (other.gameObject.name == "objStartR")
		{
			csLevels _comp = GameObject.Find("containerLevels").GetComponent("csLevels") as csLevels;
			int _level = int.Parse(other.transform.parent.parent.name.Substring(6));

			if (_level != _comp._level)
			{
				_comp._level = _level;
				_comp._nextlevel = true;
				/*int _temp = _comp._level;
				while (true)
				{
					_temp++;
					if (GameObject.Find("containerLevels/_level"+_temp))
					{
						GameObject.Find("containerLevels/_level"+_temp+"/containerWalls/objStartL").layer = 2;
						GameObject.Find("containerLevels/_level"+_temp+"/containerWalls/objStartR").layer = 2;
					} else break;
				}*/
			}
		}		
    }
	
	// Triggering with objects (enter)
	void OnTriggerEnter(Collider other)
	{
		// With coins
		if (other.gameObject.name == "prefabCoin")
		{
			_score += 10;		
			other.collider.enabled = false;
			other.transform.GetComponent<Animator>().SetInteger("deleteCoin", 1);
			other.audio.Play();
			//Destroy(other.gameObject, 3f);
			//StartCoroutine(WaitAndDeactivate(3.0f, other.gameObject));
		}
	}

	IEnumerator WaitAndDeactivate(float _waitTime, GameObject _go)
	{
		yield return new WaitForSeconds(_waitTime);
		_go.SetActive(false);
	}
}
