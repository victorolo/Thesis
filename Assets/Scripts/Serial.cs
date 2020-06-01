using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System;

public class Serial : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM4", 9600);
    // Start is called before the first frame update
    void Start()
    {
    	sp.ReadTimeout = 1;
	sp.Open();
    }

    // Update is called once per frame
    void Update()
    {
	try
	{
		Debug.Log ("Read " + sp.ReadLine ());
		// do other stuff with the data
	}
	catch (TimeoutException e)
	{
	}
        
    }
}
