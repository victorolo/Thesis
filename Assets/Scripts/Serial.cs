﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System;
using UnityEngine.UI;

public class Serial : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM4", 9600);
    InputField iField;
    Button send;
    public bool connected = false;
    Slider slider0;
    Slider slider1;
    Slider slider2;
    Slider grab;
    // Start is called before the first frame update
    void Start()
    {
    	sp.ReadTimeout = 1;
	try
        {
            sp.Open();
            Debug.Log("Serial port opened on COM4");
	    connected = true;
        }
        catch (Exception e)
        {
            Debug.Log("Couldn't open serial port: " + e.Message);
	    connected = false;
        }
	iField = GameObject.Find("InputField").GetComponent<InputField>();
	send = GameObject.Find("SerialButt").GetComponent<Button>();
	send.GetComponent<Button>().onClick.AddListener(sendCommand);
	slider0 = GameObject.Find("Slider (0)").GetComponent<Slider>(); 
	slider1 = GameObject.Find("Slider (0)").GetComponent<Slider>(); 
	slider2 = GameObject.Find("Slider (2)").GetComponent<Slider>(); 
	grab = GameObject.Find("Slider (6)").GetComponent<Slider>();
    }

    void sendCommand(){
	Debug.Log ("From input field: " + iField.text);
	if (connected){
		sp.WriteLine(iField.text);
		sp.BaseStream.Flush();
	}
	string[] rawInput = iField.text.Split(null);
	
	if (rawInput.Length == 3 && rawInput[0].Length == 1 && rawInput[0][0] == 76){
		int arm = -1;
		int angle = -999999999;
		bool found = false;

		Debug.Log("working");
		if (rawInput[1].Length == 1 && int.TryParse(rawInput[1], out _)){
			arm = Int32.Parse(rawInput[1]);
		}


		if (int.TryParse(rawInput[2], out _)){
			angle = Int32.Parse(rawInput[2]);
			found = true;
		}

		if (arm != -1 && found){
			Debug.Log("arm: " + arm + " angle: " + angle);
			if (arm == 1 && slider1.value + angle <= 180 && slider1.value + angle >= -180){
				slider1.value += angle;
			}
			if (arm == 2 && slider2.value + angle <= 180 && slider2.value + angle >= -180){
				slider2.value += angle;
			}
		}
	}

	if (rawInput.Length == 2 && rawInput[0].Length == 1 && rawInput[0][0] == 83){
		bool found = false;
		Debug.Log("working");
		int angle = -99999999;

		if (int.TryParse(rawInput[1], out _)){
			angle = Int32.Parse(rawInput[1]);
			found = true;
		}

		if (found){
			if (angle == 0){
				grab.value = 100;
			}
			if (angle == 1){
				grab.value = 30;
			}
		}
	}
	iField.text = "";
    }

    // Update is called once per frame
    void Update()
    {
	try
	{
		//Debug.Log ("Read " + sp.ReadLine ());
		// do other stuff with the data
	}
	catch (TimeoutException e)
	{
	}
        
    }
}
