﻿using UnityEngine;
using System.Collections;

public class DoorScript : Invokable
{
    private Vector3 initialPos;
    private Vector3 goalPos;
    private float lerp = 0;
    private bool initialized = false;

    public AudioClip openingSound;
    public AudioClip closingSound;
    public AudioClip doorHaulted;

    public float lerpSpeed = 0.1f;

    private float glideTorwards(float val, float desired, float increment)
    {
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
        initialized = true;
        initialPos = transform.localPosition;
        goalPos = initialPos + new Vector3(0, 2, 0);
    }

    public override void OnActiveChanged(bool newState)
    {
        if (newState)
        {
            AudioSource.PlayClipAtPoint(openingSound, transform.position,1);
        }
        else
        {
            AudioSource.PlayClipAtPoint(closingSound, transform.position,1);
        }
    }

    public void Update()
    {
        if (initialized)
        {
            if (isActive)
            {
                lerp = glideTorwards(lerp, 1, lerpSpeed);
            }
            else
            {
                lerp = glideTorwards(lerp, 0, lerpSpeed);
            }
            float x = initialPos.x + ((goalPos.x - initialPos.x) * lerp);
            float y = initialPos.y + ((goalPos.y - initialPos.y) * lerp);
            transform.localPosition = new Vector3(x, y, 0);
        }
    }

    public void OnCollisionEnter2D(Collision2D hit)
    {
        // See if we can crush the player with the door.
        if (hit.gameObject.name == "Player")
        {
            Player player = GameObject.FindObjectOfType<Player>();
            if (!player.Dead)
            {
                Vector3 pos = transform.localPosition;
                Vector3 size = transform.localScale;
                float minX = pos.x - (size.x / 2);
                float maxX = pos.x + (size.x / 2);
                float bottomY = pos.y - (size.y / 2);
                foreach (ContactPoint2D contact in hit.contacts)
                {
                    if (contact.point.x >= minX && contact.point.x <= maxX)
                    {
                        if (contact.point.y <= bottomY)
                        {
                            Debug.Log("SQUASH");
                            player.Kill("Watch out for doors!\nThey will crush you if given the opportunity.");
                            player.Dead = true;
                        }
                    }
                }
            }
        }
    }
}
