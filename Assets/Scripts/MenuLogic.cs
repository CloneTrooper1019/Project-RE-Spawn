﻿using UnityEngine;
using System.Collections;

public class MenuLogic : MonoBehaviour 
{
    public AudioClip music;
    public Camera myCamera;
    public GUITexture logo;
    public GUITexture guy;
    public Texture[] guyAnim = new Texture[5];
    public GUITexture casualHatGames;
    public bool playIntro = true;
    public float timeElapsed = 0;
    public LevelInfo[] levels;
    public float buttonWidth = 300;
    public float buttonHeight = 50;
    public float buttonSpacing = 10;

    private float actualTimeElapsed = 0;
    private float logoTransparency = 0;
    private bool showingLogo = false;
    private bool inLevelMenu = false;
    private int currentFrame = 0;
    private float lastUpdate = 0;

    private float glideTorwards(float val, float desired, float increment)
    {
        // Takes a number, and tries to move it to a desired value using +/- increment and Mathf.Max/Min
        if (val > desired)
        {
            return Mathf.Max(val - increment, desired);
        }
        else if (val < desired)
        {
            return Mathf.Min(val + increment, desired);
        }
        else
        {
            return desired;
        }
    }

    public void Start()
    {
        Debug.Log("Started");
        Time.timeScale = 1;
        ReturnToMenu isReturning = GameObject.FindObjectOfType<ReturnToMenu>();
        if (isReturning != null)
        {
            playIntro = false;
        }
        AudioSource.PlayClipAtPoint(music, myCamera.transform.localPosition);
        if (playIntro)
        {
            Animation hatAnim = casualHatGames.gameObject.GetComponent<Animation>();
            hatAnim.Play();
        }
        else
        {
            showingLogo = true;
        }
    }

    public void OnGUI()
    {
        if (!inLevelMenu)
        {
            if (showingLogo)
            {
                bool pressingPlay = GUI.Button(new Rect(Screen.width * 0.2f, Screen.height * 0.6f, 200, 60), "Start Game");
                if (pressingPlay)
                {
                    showingLogo = false;
                    inLevelMenu = true;
                }
                bool pressingQuit = GUI.Button(new Rect(Screen.width * 0.2f, Screen.height * 0.8f, 200, 60), "Quit");
                if (pressingQuit)
                {
                    Application.Quit();
                }
            }
        }
        else
        {
            float spread = buttonHeight + buttonSpacing;
            float totalHeight = ((float)levels.Length * spread);
            float cornerX = ((Screen.width/2) - (buttonWidth / 2));
            float cornerY = ((Screen.height/2) - (totalHeight / 2));
            for (int i = 0; i < levels.Length; i++)
            {
                LevelInfo level = levels[i];
                bool pressing = GUI.Button(new Rect(cornerX, cornerY + (spread * i), buttonWidth, buttonHeight), "#" + level.levelId + ": " + level.levelName);
                if (pressing)
                {
                    Application.LoadLevel("Level" + level.levelId);
                }
            }
        }
    }

    public void Update()
    {
        // Update Time Elapse
        actualTimeElapsed = actualTimeElapsed + Time.deltaTime;
        timeElapsed = actualTimeElapsed;
        // Do things
        if (timeElapsed > 5 && !showingLogo && !inLevelMenu)
        {
            showingLogo = true;
        }
        float goal = 0;
        if (showingLogo)
        {
            goal = 0.5f;
            if ((timeElapsed - lastUpdate) > 0.07)
            {
                lastUpdate = timeElapsed;
                currentFrame = (currentFrame + 1) % 8;
                guy.texture = guyAnim[currentFrame];
            }
        }
        logoTransparency = glideTorwards(logoTransparency, goal, 0.1f);
        logo.color = new Color(0.5f, 0.5f, 0.5f, logoTransparency);
        guy.color = new Color(0.5f, 0.5f, 0.5f, logoTransparency);
    }
}
