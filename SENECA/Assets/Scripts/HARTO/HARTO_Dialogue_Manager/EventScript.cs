﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SenecaEvents;

#region EventScript.cs Overview
/************************************************************************************************************************/
/*                                                                                                                      */
/*    This script determines who talks using a Coroutine.                                                               */
/*    This scipt is a part of the Dialogue System and is only called by DialogueManger.cs                               */
/*                                                                                                                      */
/*    Function List as of 5/20/2017:                                                                                    */
/*          private:                                                                                                    */
/*                 private void Start()                                                                                 */
/*                 private void FindBrocaParticles()                                                                    */
/*                 public void InitResponseScriptWith(string characterName, bool astridTalksFirst)                      */
/*                 public IEnumerator PlayEventDialogue(string characterName)                                           */
/*                 private void Update()                                                                                */
/*                                                                                                                      */
/************************************************************************************************************************/
#endregion
public class EventScript : MonoBehaviour 
{
	public bool waitingForEmotionalInput;                       //  Bool to check if we are waiting on emotional input

	public const string ASTRID = "Astrid";                      //  Reference to player
	public const string VO = "VO";                              //  
	public const string NO_EMOTION_SELECTED = "None";
	public const string HARTO = "HARTO";
	public const string GIBBERISH = "Gibberish";
	public const string BROCA_PARTICLES = "BrocaParticles";
	public string scene;                                        //  The current scene
	public string topicName;                                    //  The name of the Topic/Event
	public string characterSearchKey;                           //  Find this character in this Event  
	public int totalResponses;                                  //  Total responses for this topic
	public int astridLines;                                     //  Number of lines by Astrid for a Topic/Event
	public int npcLines;                                        //  Number of lines by the other character for a Topic/event
	public int totalLines;                                      //  The number of lines that have been said
	public float nextTimeToSearch = 0;				            //	How long unitl the camera searches for the target again

	public ResponseScript response;                             //  
	public GameObject thisResponse;
	public List<AudioSource> myCharacters;	
	public AudioController gibberishPlayer;
	public AudioSource[] thisEventsAudioSources;
	private HARTO astridHARTO;

	public GameObject Priya;
    public GameObject Ruth;

    #region Overview private void Start()
    /************************************************************************************************************************/
    /*    Responsible for:                                                                                                  */
    /*      Initalizing variables. Runs once at the beginning of the program                                                */
    /*                                                                                                                      */
    /*    Parameters:                                                                                                       */
    /*          None                                                                                                        */
    /*                                                                                                                      */
    /*    Returns:                                                                                                          */
    /*          Nothing                                                                                                     */
    /*                                                                                                                      */
    /************************************************************************************************************************/
    #endregion
    private void Start () 
	{
        //  Audio for each line is played by the Dialogue Manager System's Events
        //  This way we can elimnate any overlapping lines
		thisEventsAudioSources = GetComponentsInChildren<AudioSource>();
		for(int i = 0; i < thisEventsAudioSources.Length; i++)
		{
			myCharacters.Add(thisEventsAudioSources[i]);
		}

		astridHARTO = GameObject.FindGameObjectWithTag("HARTO").GetComponent<HARTO>();

		scene = transform.parent.name;
        //  They are called Events in the heirarchy but for Seneca they ae considered topics
        //  We rmove the "Event_" part of the name because each Event/Topic is it's own folder
        //  This is done so we can advance through every line of dialogue using an incrementing
        //  integer.
		topicName = transform.name.Replace("Event_", "");

		gibberishPlayer = GameObject.Find(BROCA_PARTICLES).GetComponent<AudioController>();
	}

    #region Overview private void FindBrocaParticles()
    /************************************************************************************************************************/
    /*                                                                                                                      */
    /*      Responsible for:                                                                                                */
    /*          Finding the the BrocaParticles if the BrocaParticles reference is null                                      */
    /*                                                                                                                      */
    /*      Parameters:                                                                                                     */
    /*          None                                                                                                        */
    /*                                                                                                                      */
    /*      Returns:                                                                                                        */
    /*          Nothing                                                                                                     */
    /*                                                                                                                      */
    /************************************************************************************************************************/
    #endregion
    private void FindBrocaParticles()
	{
		if (nextTimeToSearch <= Time.time)
		{
			GameObject result = GameObject.FindGameObjectWithTag (BROCA_PARTICLES);
			if (result != null)
			{
				gibberishPlayer = result.GetComponent<AudioController>();
			}
				nextTimeToSearch = Time.time + 2.0f;
		}
	}

    #region Overview public void InitResponseScriptWith(string characterName, bool astridTalksFirst)
    /************************************************************************************************************************/
    /*                                                                                                                      */
    /*      Responsible for:                                                                                                */
    /*          Starting the conversatuion between the player and another character```                                      */
    /*                                                                                                                      */
    /*      Parameters:                                                                                                     */
    /*          string characterName: Name of the character you are talking to                                              */
    /*          bool astridTalksFirst: when true, Astrid talks first                                                        */
    /*                                                                                                                      */
    /*      Returns:                                                                                                        */
    /*          Nothing                                                                                                     */
    /*                                                                                                                      */
    /************************************************************************************************************************/
    #endregion
    public void InitResponseScriptWith(string characterName, bool astridTalksFirst)
	{
		if(!topicName.Contains("Start_Game"))
		{
			Services.Events.Fire(new BeginDialogueEvent());
		}

        //  Reset eash line number when starting a new conversation
		totalLines = 0;
		astridLines = 0;
		npcLines = 0;
        
        characterSearchKey = characterName;
        
        //  If you can have a conversation with that character regarding that topic,
        //  the line number will populate
		if (transform.FindChild(characterSearchKey))
		{
            for (int i = 0; i < myCharacters.Count; i++)
			{
				if (myCharacters[i].name  == characterSearchKey || myCharacters[i].name  == ASTRID)
				{
                    //  total responses is both Astrid's lines and the other character's lines
					totalResponses += myCharacters[i].transform.childCount;
				}
			}

            //  Play Astrid's line first is she talks first
			if (astridTalksFirst)
			{
				astridLines++;
				GameObject firstResponse = GameObject.Find("Astrid_VO_" + astridLines+ "_" + scene + "_" + topicName).gameObject;
				if (firstResponse.transform.childCount > 1)
				{
					response = firstResponse.GetComponent<EmotionalResponseScript>();
					waitingForEmotionalInput = true;
				}
				else
				{
					response = firstResponse.GetComponent<ResponseScript>();
				}	
			}
			else
			{
				npcLines++;
				GameObject firstResponse = GameObject.Find(characterName + "_" + VO + "_" + npcLines + "_" + scene + "_" + topicName).gameObject;
				if (firstResponse.transform.childCount > 1)
				{
					response = firstResponse.GetComponent<EmotionalResponseScript>();
					waitingForEmotionalInput = true;
				}
				else
				{
					response = firstResponse.GetComponent<ResponseScript>();
				}
			}

            //  This goes through the topic for both characters incrementing the line number by one
            //  Use a Task Management system to allow pausing in the middle of a conversation
			StartCoroutine(PlayEventDialogue(characterName));
		}
	}

    #region Overview public  IEnumerator PlayEventDialogue(string characterName)
    /************************************************************************************************************************/
    /*                                                                                                                      */
    /*      Responsible for:                                                                                                */
    /*          Playing each line of dialogue. Look at this funcytion as a discrete Update loop. It zigzags bewteen         */
    /*          two charcaters finding the next dialogue line in the smae file hiaracrhy and incrementing the VO line.      */
    /*                                                                                                                      */
    /*      Parameters:                                                                                                     */
    /*          string characterName: Name of the character you are talking to                                              */
    /*                                                                                                                      */
    /*      Returns:                                                                                                        */
    /*          The type of objects to enumerate.                                                                           */
    /*                                                                                                                      */
    /************************************************************************************************************************/
    #endregion
    public IEnumerator PlayEventDialogue(string characterName)
	{
        //  go through the Topic/Event until the lines said  == the total responses
		while(totalLines < totalResponses)
		{
            //  increment lines to find them in the file heirarchy
			totalLines++;

            //  All emotional responses have more than 1 child (because you have options
			if (response.transform.childCount > 1)
			{
				waitingForEmotionalInput = true;
				Priya = GameObject.FindGameObjectWithTag("Priya");
				Priya.GetComponent<Animator>().SetBool("IsTalking", false);
			}

            //  If no emotion has been selecetd you stay in this while loop
			while(astridHARTO.CurrentEmotion.ToString() == NO_EMOTION_SELECTED && response.transform.childCount > 1)
			{
                if (scene == "SCENE_1" && !GameManager.instance.playerAnimationLock)
                {
                    GameManager.instance.player_Astrid._animator.SetBool("HARTOActive", true);
                    GameManager.instance.player_Astrid._animator.SetBool("IsTalking", true);
                }
                GameManager.instance.waitingForInput = waitingForEmotionalInput;
				yield return new WaitForFixedUpdate();
				Priya = GameObject.FindGameObjectWithTag("Priya");
				Priya.GetComponent<Animator>().SetBool("IsTalking", false);
			}

            //  This plays emotion response lines
			if (response.transform.childCount > 1)
			{
				((EmotionalResponseScript)response).PlayEmotionLine(astridHARTO.CurrentEmotion, HARTO, scene, topicName);
				waitingForEmotionalInput = false;
				GameManager.instance.waitingForInput = waitingForEmotionalInput;
				Priya = GameObject.FindGameObjectWithTag("Priya");
				Priya.GetComponent<Animator>().SetBool("IsTalking", false);
			}
			else
			{
                //  Otherwise play a regular line
				response.PlayLine(HARTO, scene, topicName);
			}
            
            //  Handles talking animations
            while (response.characterAudioSource.isPlaying)
			{
				if (response.characterName == "Astrid")
				{
                    if (scene == "SCENE_1" && !GameManager.instance.playerAnimationLock && topicName != "Start_Game")
                    {
                        GameManager.instance.player_Astrid._animator.SetBool("HARTOActive", true);
                        GameManager.instance.player_Astrid._animator.SetBool("IsTalking", true);
                    }
                    else
                    {
                        GameManager.instance.player_Astrid._animator.SetBool("IsTalking", true);
                    }

                    if (GameObject.FindGameObjectWithTag("Priya") != null)
                    {
                        Priya = GameObject.FindGameObjectWithTag("Priya");
                        Priya.GetComponent<Animator>().SetBool("IsTalking", false);
                    }

                    if (GameObject.FindGameObjectWithTag("Ruth") != null)
                    {
                        Ruth = GameObject.FindGameObjectWithTag("Ruth");
                        Ruth.GetComponent<Animator>().SetBool("IsTalking", false);
                    }
                } 
				else if (response.characterName == "Priya") 
				{
                    if (!GameManager.instance.playerAnimationLock)
                    {
                        GameManager.instance.player_Astrid._animator.SetBool("HARTOActive", false);
                        GameManager.instance.player_Astrid._animator.SetBool("IsTalking", false);
                    }
                    // other character istalking is true;
                    Priya = GameObject.FindGameObjectWithTag ("Priya");
					Priya.GetComponent<Animator>().SetBool("IsTalking", true);
                    Services.Events.Fire(new InteractableEvent(false, false, false));

                }
                else if (response.characterName == "Ruth")
                {
                    if (!GameManager.instance.playerAnimationLock)
                    {
                        GameManager.instance.player_Astrid._animator.SetBool("HARTOActive", false);
                        GameManager.instance.player_Astrid._animator.SetBool("IsTalking", false);
                    }
                    // other character istalking is true;
                    Ruth = GameObject.FindGameObjectWithTag("Ruth");
                    Ruth.GetComponent<Animator>().SetBool("IsTalking", true);

                }
                //  No gibberish dialogue in Untan
                if (!GameManager.instance.inUtan)
				{
					gibberishPlayer.GetComponent<AudioSource>().volume = 0f;
				}
				yield return new WaitForFixedUpdate();	
			}
            
            //  If no one is talking we shouldn't hear gibberish dialogue
			gibberishPlayer.GetComponent<AudioSource>().volume = 0.0f;

            //  If the last line was said by Astrid we should play the other character's line
			if (response.characterName == ASTRID)
			{
                //  We try to find the line of dialogue based on the character's name, the line of dialogue in the Event/Topic,
                //  the scene number, and then the topic
				try
				{
					npcLines++;
					thisResponse = GameObject.Find(characterName + "_" + VO + "_" + npcLines + "_" + scene + "_" + topicName).gameObject;
					if (thisResponse.transform.childCount > 1)
					{
						response = thisResponse.GetComponent<EmotionalResponseScript>();
					}
					else
					{
						response = thisResponse.GetComponent<ResponseScript>();
					}
				}
				catch (Exception e)
				{
                    //  Lets us know we didn't find the line
					Debug.Log ("Could not find " + characterName + "_" + VO + "_" + npcLines + "_" + scene + "_" + topicName);
				}
			}
            //  If the last line was said by the other charcter we need to play Astrid's line
			else
			{
                //  We try to find the line of dialogue based on the character's name, the line of dialogue in the Event/Topic,
                //  the scene number, and then the topic
                try
                {
					astridLines++;
					thisResponse = GameObject.Find("Astrid_VO_" + astridLines + "_" + scene + "_" + topicName).gameObject;

					if (thisResponse.transform.childCount > 1)
					{
						response = thisResponse.GetComponent<EmotionalResponseScript>();
						waitingForEmotionalInput = true;
					}
					else
					{
						response = thisResponse.GetComponent<ResponseScript>();
					}
                    //  Sets the current emotion to default
					astridHARTO.CurrentEmotion = Emotions.None;
				}
				catch (Exception e)
				{
                    Debug.Log("Could not find " + "Astrid_VO_" + astridLines + "_" + scene + "_" + topicName);
                }
			}

			if( totalLines == totalResponses)
			{
                //  Stop this loop when all the lines have been said
				break;
			}
		}

		if(topicName != "Start_Game" && !scene.Contains("2"))
		{
			Services.Events.Fire(new EndDialogueEvent(topicName));
			Priya = GameObject.FindGameObjectWithTag("Priya");
			Priya.GetComponent<Animator>().SetBool("IsTalking", false);
			if (topicName == "Exit")
			{
                GameManager.instance.tutorialIsDone = true;
				HARTO_UI_Interface.HARTOSystem.WaitForExitScript();
				Services.Events.Fire(new ToggleHARTOEvent());
			}
		}
        else if(scene.Contains("2") && topicName == "Ruth")
        {
            GameManager.instance.endGame = true;
        }
		else
		{
			if(!GameManager.instance.tabUIOnScreen)
			{
				//SenecaCampsiteSceneScript.MakeTabAppear ();
				Services.Events.Fire(new TABUIButtonAppearEvent());
				Priya = GameObject.FindGameObjectWithTag("Priya");
				Priya.GetComponent<Animator>().SetBool("IsTalking", false);

			}
		}
		yield return null;
	}

    #region Overview private void Update()
    /************************************************************************************************************************/
    /*                                                                                                                      */
    /*      Responsible for:                                                                                                */
    /*          Running once per frame					                                                                    */
    /*                                                                                                                      */
    /*      Parameters:                                                                                                     */
    /*          None                                                                                                        */
    /*                                                                                                                      */
    /*      Returns:                                                                                                        */
    /*          Nothing                                                                                                     */
    /*                                                                                                                      */
    /************************************************************************************************************************/
    #endregion
    private void Update()
    {
        if (GameManager.instance.inUtan)
        {
            if (name.Contains("SCENE_1"))
            {
                gameObject.SetActive(false);
            }
        }

        // CHEAT. SPACE TO SPEED THROUGH THE DIALOGUE

        if (GameManager.instance.cheatSpace)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (response != null)
                {
                    response.StopLine();
                }
            }
        }

        if (gibberishPlayer == null)
        {
            FindBrocaParticles();
            return;
        }
    }
}
