﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Oculus.Avatar;

public class CalibrationScript : MonoBehaviour
{
    // Start is called before the first frame update
    private enum State { Start = 1, Height, Arms, Final };
    private enum AudioState { Intro = 1, HeightIntro, ArmsIntro, End };
    State currentState = State.Start;
    public TextMeshProUGUI display;
    public GameObject button, eyeCamera, rightHand, floor;
    public GameObject audio1, audio2, audio3, audio4;
    public GameObject DistanceTextPrefab;
    private LineRenderer m_lineHandleRenderer;

    public Text heightText, widthText;
    private string height = "PLEASE STAND STRAIGHT AND PRESS BUTTON BELOW!";
    private string arms = "PLEASE EXTEND ARMS FORWARD AND PRESS BUTTON BELOW!";
    private string success = "CALIBRATION SUCCESS!";
    private string hellYeah = "HELL YEAH!";
    private string getHeight = "GET HEIGHT!";
    private string getWidth = "GET LENGTH!";
    private string buttonSuccess = "DONE!";

    


    GameObject textMeshObj;

    void Start()
    {

    }

    public void Awake()
    {
        m_lineHandleRenderer = gameObject.GetComponent<LineRenderer>();
        m_lineHandleRenderer.startWidth = 0.01f;
        m_lineHandleRenderer.endWidth = 0.01f;
        m_lineHandleRenderer.positionCount = 0;
        m_lineHandleRenderer.sortingOrder = 1;
        m_lineHandleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        m_lineHandleRenderer.material.color = new Color(1, 0.8f, 0.0f, 0.7f);

        textMeshObj = Instantiate(DistanceTextPrefab, Vector3.one, Quaternion.Euler(0, 0, 180));
        textMeshObj.SetActive(false);

        playAudio(AudioState.Intro);

    }

    // Update is called once per frame
    void Update()
    {
        int test = 1;
        TextMesh textMesh = textMeshObj.GetComponentInChildren<TextMesh>();

        switch (currentState)
        {
            case State.Height:

                m_lineHandleRenderer.positionCount = 2;

                Vector3 newBottom = eyeCamera.transform.position;
                newBottom.y = floor.transform.position.y;

                m_lineHandleRenderer.SetPosition(0, newBottom + new Vector3(0.0f, 0.1f, 1f));
                m_lineHandleRenderer.SetPosition(1, eyeCamera.transform.position + new Vector3(0.0f, 0.1f, 1f));

                Vector3 midpoint = (floor.transform.position + eyeCamera.transform.position) / 2 + new Vector3(0.0f, 0.1f, 1f);
                textMeshObj.transform.position = midpoint;

                textMeshObj.transform.LookAt(2*textMeshObj.transform.position - eyeCamera.transform.position);

                textMesh.text = Vector3.Distance(floor.transform.position, eyeCamera.transform.position).ToString(".0#") + " m";
                textMesh.fontSize = 30;

                textMeshObj.SetActive(true);

                break;

            case State.Arms:

                m_lineHandleRenderer.positionCount = 2;

                m_lineHandleRenderer.SetPosition(0, eyeCamera.transform.position + new Vector3(0.15f, -0.15f, -0.06f));
                m_lineHandleRenderer.SetPosition(1, rightHand.transform.position);

                Vector3 midpoint2 = (eyeCamera.transform.position + new Vector3(0.15f, -0.15f, -0.01f) + rightHand.transform.position) / 2;
                textMeshObj.transform.position = midpoint2;

                textMeshObj.transform.LookAt(2 * textMeshObj.transform.position - eyeCamera.transform.position);

                textMesh.text = Vector3.Distance(eyeCamera.transform.position, rightHand.transform.position).ToString(".0#") + " m";
                textMesh.fontSize = 30;


                textMeshObj.SetActive(true);

                break;

            default:

                m_lineHandleRenderer.positionCount = 0;

                textMeshObj.SetActive(false);

                break;

        }
    }


    /**
     * Code to enable haptic
     **/

    public void PlayShoot(bool rightHanded)
    {
        if (rightHanded) StartCoroutine(Haptics(1, 1, 0.3f, true, false));
        else StartCoroutine(Haptics(1, 1, 0.3f, false, true));
    }

    IEnumerator Haptics(float frequency, float amplitude, float duration, bool rightHand, bool leftHand)
    {
        if (rightHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);

        yield return new WaitForSeconds(duration);

        if (rightHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }


    public void UpdateState()
    {
        switch (currentState)
        {
            case State.Start:
                //enter text for height
                display.text = height;
                button.GetComponentInChildren<Text>().text = getHeight;
                currentState = State.Height;
                playAudio(AudioState.HeightIntro);
                PlayShoot(true);
                break;
            case State.Height:
                display.text = arms;
                button.GetComponentInChildren<Text>().text = getWidth;
                float height1 = Vector3.Distance(floor.transform.position, eyeCamera.transform.position);
                Globals.height = height1;
                heightText.text = "Height: " + height1.ToString(".0##") + "m";
                currentState = State.Arms;
                playAudio(AudioState.ArmsIntro);
                PlayShoot(true);
                break;
            case State.Arms:
                display.text = success;
                //initia
                button.GetComponentInChildren<Text>().text = buttonSuccess;
                float distance = Vector3.Distance(eyeCamera.transform.position ,rightHand.transform.position);
                Globals.armLength = distance;
                widthText.text = "Length: " + distance.ToString(".0##") + "m";
                currentState = State.Final;
                playAudio(AudioState.End);
                PlayShoot(true);
                break;
            case State.Final:
                SceneManager.LoadScene(Globals.MainMenu);
                break;
            default:
                display.text = "OK.. TATA.";
                break;
        }
    }

    public void OnHover()
    {
       
        float distance = Vector3.Distance(eyeCamera.transform.position, rightHand.transform.position);
        float height =   Vector3.Distance(new Vector3(0,0,0), eyeCamera.transform.position);
        widthText.text = "Length: " + distance.ToString(".0##") + "m";
        heightText.text = "Height: " + height.ToString(".0##") + "m";
        
                
        
    }

    private void playAudio(AudioState num)
    {
        switch(num)
        {
            case AudioState.Intro:
                audio1.SetActive(true);
                audio2.SetActive(false);
                audio3.SetActive(false);
                audio4.SetActive(false);
                break;
            case AudioState.HeightIntro:
                audio1.SetActive(false);
                audio2.SetActive(true);
                audio3.SetActive(false);
                audio4.SetActive(false);
                break;
            case AudioState.ArmsIntro:
                audio1.SetActive(false);
                audio2.SetActive(false);
                audio3.SetActive(true);
                audio4.SetActive(false);
                break;
            case AudioState.End:
                audio1.SetActive(false);
                audio2.SetActive(false);
                audio3.SetActive(false);
                audio4.SetActive(true);
                break;
            default:
                audio1.SetActive(false);
                audio2.SetActive(false);
                audio3.SetActive(false);
                audio4.SetActive(false);
                break;
        }
    }

}
