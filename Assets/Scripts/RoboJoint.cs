﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoboJoint : MonoBehaviour
{
	public float[,] DHTable;
	[Range(-180f, 180f)]
	public float theta;
	float smooth = 100.0f;
	[Range(-180f, 180f)]
	public float alpha;
	Slider thetaSlider;
	Slider alphaSlider;
	private float targetTheta;
	private float targetAlpha;
	public Quaternion q;
	Quaternion finalRot;
	public bool stop;
	int waitCount;
	public bool prevStop;
	Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
	waitCount = 0;
	stop = false;
	prevStop = false;
	thetaSlider = GameObject.Find("Slider").GetComponent<Slider>();
	theta = thetaSlider.value; // both these should be created from the robi file
	alphaSlider = GameObject.Find("Slider (1)").GetComponent<Slider>();
	alpha =alphaSlider.value;
	DHTable = new float[,]{{theta,0,0,alpha},{0,0,0,0},{0,0,0,0},{0,0,0,0}}; // theta, d, length, alpha
	rb = this.gameObject.AddComponent<Rigidbody>();
	rb.useGravity = false;
	rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	rb.isKinematic=true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	    if (prevStop == true && stop == true){
		    stop = false;
	    }
	    prevStop = stop;
	targetTheta = GameObject.Find("Slider").GetComponent<Slider>().value; // both these should be created from the robi file
	targetAlpha = GameObject.Find("Slider (1)").GetComponent<Slider>().value;

	if (theta + 1 < targetTheta){
		theta++;
	}
	else if (theta - 1 > targetTheta){
		theta--;
	}

	if (alpha + 1 < targetAlpha){
		alpha++;
	}
	else if (alpha - 1 > targetAlpha){
		alpha--;
	}
	DHTable[0,0] = theta;
	DHTable[0,3] = alpha;
	float[,] arr = trans(DHTable[0,0], DHTable[0,1], DHTable[0,2], DHTable[0,3]);
	if (waitCount > 100){
		rb.isKinematic=false;
	}
	waitCount++;
	/*int rowLength = arr.GetLength(0);
        int colLength = arr.GetLength(1);
	string str = "";

        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < colLength; j++)
            {
                str += (string.Format("{0} ", arr[i, j]));
            }
            str += ("\n");
        }
	Debug.Log(str); */
	q = QuaternionFromMatrix(arr);
	finalRot = Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
	rb.MoveRotation(finalRot);	
	//StartCoroutine(Example());
    }

	public float r2d(float deg){
		return deg/180*(float)Math.PI;
	}

	public float[,] trans(float theta, float d, float a, float alpha){
		float[,] matrix = {{Mathf.Cos(r2d(theta)), -Mathf.Sin(r2d(theta)), 0, a},
				{Mathf.Sin(r2d(theta))*Mathf.Cos(r2d(alpha)), Mathf.Cos(r2d(theta))*Mathf.Cos(r2d(alpha)), -Mathf.Sin(r2d(alpha)), -Mathf.Sin(r2d(alpha))*d},
				{Mathf.Sin(r2d(theta))*Mathf.Sin(r2d(alpha)), Mathf.Cos(r2d(theta))*Mathf.Sin(r2d(alpha)), Mathf.Cos(r2d(alpha)), Mathf.Cos(r2d(alpha))*d},
				{0,0,0,1}};
		//double[,] matrix2 = {{Math.Cos((double)r2d(theta)), Math.Sin((double)r2d(theta))*Math.Cos((double)r2d(alpha)), Math.Sin((double)r2d(theta))*Math.Sin((double)r2d(alpha))}};
	return matrix;
	}

	public static Quaternion QuaternionFromMatrix(float[,] m) {
		// Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
		Quaternion q = new Quaternion();
		q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] + m[1,1] + m[2,2] ) ) / 2; 
		q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] - m[1,1] - m[2,2] ) ) / 2; 
		q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] + m[1,1] - m[2,2] ) ) / 2; 
		q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] - m[1,1] + m[2,2] ) ) / 2; 
		q.x *= Mathf.Sign( q.x * ( m[2,1] - m[1,2] ) );
		q.y *= Mathf.Sign( q.y * ( m[0,2] - m[2,0] ) );
		q.z *= Mathf.Sign( q.z * ( m[1,0] - m[0,1] ) );
		return q;
	}

	/*IEnumerator Example()
    	{
        	yield return new WaitForFixedUpdate();
		if (GameObject.Find("0").GetComponent<RoboJoint>().stop){
        		GameObject.Find("0").GetComponent<RoboJoint>().stop = false;	
			Debug.Log("stop to false");
		}
    	}*/

}
