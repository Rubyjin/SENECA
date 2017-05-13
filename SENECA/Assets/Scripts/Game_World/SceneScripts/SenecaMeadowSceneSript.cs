﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScenes;
using ChrsUtils.ChrsCamera;

public class SenecaMeadowSceneSript : Scene<TransitionData>
{
	public Player player;

	internal override void OnEnter(TransitionData data)
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraFollow2D> ().xPosBoundary = 0f;

	}

    public float nextTimeToSearch = 0;

    void FindPlayer()
    {
        if (nextTimeToSearch <= Time.time)
        {
            GameObject result = GameObject.FindGameObjectWithTag("Player");
            if (result != null)
            {
                player = result.GetComponent<Player>();
            }
            nextTimeToSearch = Time.time + 2.0f;
        }
    }

    void Update()
    {

        if (player == null)
        {
            FindPlayer();
            return;
        }
    }

    internal override void OnExit()
	{
		TransitionData.Instance.SENECA_MEADOW.position = player.transform.position;
		TransitionData.Instance.SENECA_MEADOW.scale = player.transform.localScale;
		TransitionData.Instance.SENECA_MEADOW.visitedScene = true;
	}
}
