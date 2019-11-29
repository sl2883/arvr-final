﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PraciceSceneController : MonoBehaviour
{

    public TextMeshProUGUI display;
    public AudioSource audioSource;
    public AudioClip initiating, letsGo;
    private string letsGoText = "LET'S GO!!";
    private bool m_timerRunning;
    private bool m_timerStarted;
    private bool m_timerEnded;
    private int m_timerCount = 3;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunInitiator());
        m_timerRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timerStarted&&m_timerRunning)
            StartCoroutine(RunTimer());
        else if(m_timerEnded)
        {
            //code to call practise punches or call the individual scenes
        }
    }

    private IEnumerator RunInitiator()
    {
        audioSource.PlayOneShot(initiating);
        yield return new WaitForSeconds(4.0f);
        m_timerStarted = true;
    }

    private IEnumerator RunTimer()
    {
        m_timerRunning = false;
        while (m_timerCount > -1 && !m_timerEnded)
        {
            if (m_timerCount > 0)
                display.text = "" + m_timerCount--;
            else
            {
                display.text = letsGoText;
                audioSource.PlayOneShot(letsGo);
                m_timerEnded = true;
                
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator DisplayLetsGo()
    {
        yield return new WaitForSeconds(2.0f);
        display.text = letsGoText;
        audioSource.PlayOneShot(letsGo);
        m_timerEnded = true;
        m_timerRunning = false;
    }   
}