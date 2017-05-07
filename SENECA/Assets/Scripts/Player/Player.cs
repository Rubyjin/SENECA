﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SenecaEvents;
using ChrsUtils.ChrsEventSystem.EventsManager;
using ChrsUtils.ChrsEventSystem.GameEvents;
using ChrsUtils.ChrsExtensionMethods;

public class Player : MonoBehaviour 
{
	public const string NPC = "NPC";
	public const string HORIZONTAL_AXIS = "Horizontal";
	public const string VERICLE_AXIS = "Vertical";
	public const string WALL = "Wall";
	public const string WITCHLIGHT = "Witchlight";

	public bool diableMovement;
	public bool facingLeft;
	public string npcAstridIsTalkingTo;
	public KeyCode upKey = KeyCode.W;
	public KeyCode downKey = KeyCode.S;
	public KeyCode leftKey = KeyCode.A;
	public KeyCode rightKey = KeyCode.D;
	
	public float moveSpeed;
	public Animator animator;

	private const float MAX_SCALE = 0.7f;
	private const float MIN_SCALE = 0.2f;
	private AudioSource _audioSource;
	private AudioClip _clip;

	private Rigidbody2D _rigidBody2D;
	private DisablePlayerMovementEvent.Handler onToggleDisableMovement;
	private ToggleHARTOEvent.Handler onToggleHARTO;
	private ClosingHARTOForTheFirstTimeEvent.Handler onClosingHARTOForTheFirstTime;

	// Use this for initialization
	void Start () 
	{
		_rigidBody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();

		onToggleDisableMovement = new DisablePlayerMovementEvent.Handler(OnToggleDisableMovement);
		onToggleHARTO = new ToggleHARTOEvent.Handler(OnToggleHARTO);
		onClosingHARTOForTheFirstTime = new ClosingHARTOForTheFirstTimeEvent.Handler(OnClosingHARTOForTheFirstTime);

		Services.Events.Register<DisablePlayerMovementEvent>(onToggleDisableMovement);
		Services.Events.Register<ToggleHARTOEvent>(onToggleHARTO);
		Services.Events.Register<ClosingHARTOForTheFirstTimeEvent>(onClosingHARTOForTheFirstTime);
	}

	void OnToggleDisableMovement(GameEvent e)
	{
		diableMovement = ((DisablePlayerMovementEvent)e).disableMovement;
	}

	void OnToggleHARTO(GameEvent e)
	{
		//_animator.SetBool("HARTOActive", !HARTO_UI_Interface.HARTOSystem.isHARTOActive);
	}

	void OnClosingHARTOForTheFirstTime(GameEvent e)
	{
		npcAstridIsTalkingTo = "";
		
	}

	/*--------------------------------------------------------------------------------------*/
	/*																						*/
	/*	Move: Handles player movement														*/
	/*		param: float dx - Horizontal Input												*/
	/*			   float dy - Vertical Input												*/
	/*																						*/
	/*--------------------------------------------------------------------------------------*/
	void Move(float dx, float dy)
	{
		//	Adds force to rigidbody based on the input
		_rigidBody2D.velocity = new Vector2(dx * moveSpeed, dy * moveSpeed);
		animator.SetFloat("SpeedX", Mathf.Abs(dx));
		animator.SetFloat("SpeedY", dy);

		if (dx > 0 && !facingLeft)
		{
			
			// Flips player
			Flip ();
		}
		// Otherwise player should be facing left
		else if (dx < 0 && facingLeft)
		{
			
			// Flips player
			Flip ();
		}

	}

	public void PlayFootStepAudio()
	{
		_clip = Resources.Load("Audio/SFX/Footstep01") as AudioClip;

		if(!_audioSource.isPlaying)
		{
			_audioSource.pitch = Random.Range(0.85f, 1.2f);
			_audioSource.PlayOneShot(_clip);
		}
	}



	/*--------------------------------------------------------------------------------------*/
	/*																						*/
	/*	Flip: FLips player sprite right or left												*/
	/*																						*/
	/*--------------------------------------------------------------------------------------*/
	public void Flip ()
	{
		// Switch the way the player is labeled as facing
		facingLeft = !facingLeft;

		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		float x = Input.GetAxis(HORIZONTAL_AXIS);
		float y = Input.GetAxis(VERICLE_AXIS);
		animator.SetBool("HARTOActive", HARTO_UI_Interface.HARTOSystem.isHARTOActive);
		if(!diableMovement)
		{
			Move(x, y);
		}
	}

	void OnTriggerEnter2D(Collider2D other) 
	{

		if (other.gameObject.tag == NPC)
		{
			npcAstridIsTalkingTo = other.gameObject.name;
		}

		if (other.gameObject.name == WITCHLIGHT) 
		{
			Debug.Log ("touch");
		}
			
	}
}
