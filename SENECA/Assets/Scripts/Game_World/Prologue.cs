﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChrsUtils.ChrsEventSystem;
using ChrsUtils.ChrsEventSystem.EventsManager;
using UnityEngine.SceneManagement;
using SenecaEvents;

public class Prologue : MonoBehaviour 
{
	public AudioClip clip;
	private AudioSource audioSource;
	// Use this for initialization
	void Start () 
	{
		clip = Resources.Load("Audio/VO/Prologue") as AudioClip;
		audioSource = GetComponent<AudioSource>();

		audioSource.PlayOneShot(clip);
		GameManager.instance.inConversation = true;	
	}
	
	IEnumerator LoadNextScene()
	{
		yield return new WaitForSeconds(2.0f);
		GameEventsManager.Instance.Fire(new SceneChangeEvent("Seneca_Campsite"));
		SceneManager.LoadScene("Seneca_Campsite");
	}

	// Update is called once per frame
	void Update () 
	{
		if(!audioSource.isPlaying)
		{
			GameManager.instance.inConversation = false;
			StartCoroutine(LoadNextScene());	
		}	
	}
}
