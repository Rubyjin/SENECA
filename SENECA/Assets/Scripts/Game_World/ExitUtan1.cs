﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChrsUtils.ChrsEventSystem.EventsManager;
using SenecaEvents;

public class ExitUtan1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


	}

	void OnTriggerEnter2D(Collider2D coll){

		if (coll.gameObject.tag == "Player") 
		{
			Services.Events.Fire(new SceneChangeEvent("Utan_Meadow"));

			TransitionData.Instance.UTAN_FORK.position = coll.transform.position;
			TransitionData.Instance.UTAN_FORK.scale = coll.transform.localScale;
			Services.Scenes.Swap<UtanRoadSceneScript>(TransitionData.Instance);
		}
	}
}
