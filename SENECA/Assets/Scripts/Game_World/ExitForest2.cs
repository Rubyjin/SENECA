﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChrsUtils.ChrsEventSystem.EventsManager;
using SenecaEvents;

public class ExitForest2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


	}

	void OnTriggerEnter2D(Collider2D coll){

		if (coll.gameObject.tag == "Player") 
		{
			Services.Events.Fire(new SceneChangeEvent("Seneca_Meadow"));
			TransitionData.Instance.SENECA_FORK.position = coll.transform.position;
			TransitionData.Instance.SENECA_FORK.scale = coll.transform.localScale;
			Services.Scenes.Swap<SenecaMeadowSceneSript>(TransitionData.Instance);
		}
	}
}
