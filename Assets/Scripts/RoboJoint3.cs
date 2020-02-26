﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoboJoint3 : MonoBehaviour
{
	private float[,] DHTable;
	[Range(-180f, 180f)]
	public float theta;
	//float smooth = 1.0f;
	float smooth = 100.0f;
	public float length;
	public float targetTheta;
	//private float targetTheta;
	[Range(-180f, 180f)]
	public float alpha;
	private float targetAlpha;
	private float prevTheta;
	private float prevAlpha;
	private Vector3 prevPos;
	private Quaternion prevRot;
	public bool collided;
	private int lastMove;
	Slider thetaSlider;
	Slider alphaSlider;
	Rigidbody rb;
	GameObject parentObject;
	int waitCount;
	GameObject prevJoint;
	GameObject nextJoint;
	public Quaternion q;
	public Vector3 newPos;
	public Quaternion finalRot;
	private bool thisStop;
	ConfigurableJoint cj;
    // Start is called before the first frame update
    void Start()
    {
	waitCount = 0;
	collided = false;
	int partNo = Int32.Parse(this.name);
	prevJoint = GameObject.Find((partNo-1).ToString());
	nextJoint = GameObject.Find((partNo+1).ToString());
	thetaSlider = GameObject.Find("Slider (" + (partNo*2).ToString() + ")").GetComponent<Slider>();
	theta = thetaSlider.value; // both these should be created from the robi file
	alphaSlider = GameObject.Find("Slider (" + (partNo*2 + 1).ToString() + ")").GetComponent<Slider>();
	alpha = alphaSlider.value;
	length = 5;
	DHTable = new float[,]{{theta,0,length,alpha},{0,0,0,0},{0,0,0,0},{0,0,0,0}};
	rb = this.gameObject.AddComponent<Rigidbody>();
	rb.useGravity = false;
	rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	rb.isKinematic=true;
	//DHTable[0,0] = theta;
	//DHTable[0,2] = length;
	//DHTable[0,3] = alpha;
	Matrix4x4 mat = trans(DHTable[0,0], DHTable[0,1], DHTable[0,2], DHTable[0,3]);

	if (this.name == "1"){
	q = QuaternionFromMatrix(mat);
	q = prevJoint.GetComponent<RoboJoint>().q*q;
	}
	else
	{
	q = QuaternionFromMatrix(mat);
	q = prevJoint.GetComponent<RoboJoint3>().q*q;
	}
	Vector3 transl;
	transl.x = mat.m03;
	transl.y = mat.m13;
	transl.z = mat.m23;
        Vector3 currPos = this.transform.position;	
	prevPos = newPos;
	prevRot = q;
	thisStop = true;
	newPos = new Vector3(0f,0f,length);//prevJoint.transform.TransformPoint(transl);
	this.transform.position = newPos;
	finalRot = Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
	this.transform.rotation = (finalRot);
	//this.transform.position = transl;	
	//this.transform.position = GameObject.Find("0").transform.position + transl;	
	//rb.MovePosition(GameObject.Find("0").transform.position + transl);	
	//this.transform.rotation =  Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
	//rb.MoveRotation(Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth));
	//parentObject = createScaler(this, GameObject.Find(((partNo-1).ToString() )));
	//this.transform.position = transl;	
	//length = 1;
	    }

    // Update is called once per frame
    void FixedUpdate()
    {
	if (GameObject.Find("0").GetComponent<RoboJoint>().stop && thisStop){
		globalCollision(rb);
	}
	thisStop = true;
	if (waitCount == 100){
		//HingeJoint j = this.gameObject.AddComponent<HingeJoint>();
		//j.connectedBody = nextJoint.GetComponent<Rigidbody>();
		cj = this.gameObject.AddComponent<ConfigurableJoint>();
		cj.connectedBody = prevJoint.GetComponent<Rigidbody>();
		cj.xMotion = ConfigurableJointMotion.Locked;
		cj.yMotion = ConfigurableJointMotion.Locked;
		cj.zMotion = ConfigurableJointMotion.Locked;
		cj.anchor = new Vector3(0f, 0f, 0f);
		cj.enableCollision = true; 
		cj.connectedMassScale = 0;
	}
	if (waitCount > 100){
		rb.isKinematic=false;
		//fj.connectedBody = prevJoint.GetComponent<Rigidbody>();
		//rb.centerOfMass = this.transform.position;
	}
	waitCount++;
	if (prevTheta != theta){
		prevTheta = theta;
	}
	if (prevAlpha != alpha){
		prevAlpha = alpha;
	}
	if (!vecCompare(prevPos, newPos)){
		prevPos = newPos;
	}
	if (Mathf.Abs(Quaternion.Angle(prevRot, finalRot)) > 0.99){
		prevRot = finalRot;
	}
	targetTheta = thetaSlider.value; // both these should be created from the robi file
	targetAlpha = alphaSlider.value;

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
	int partNo = Int32.Parse(this.name);
	// should really have everythings collided set to true then only the limb that moved last should move back
	// for some reason when moving back, any attached limbs who have had their theta changed will change theta aswell
	// code doesnt work for alpha
	DHTable[0,0] = theta;
	DHTable[0,2] = length;
	DHTable[0,3] = alpha;
	Matrix4x4 mat = trans(DHTable[0,0], DHTable[0,1], DHTable[0,2], DHTable[0,3]);

	if (this.name == "1"){
	q = QuaternionFromMatrix(mat);
	q = (prevJoint.GetComponent<RoboJoint>().q*q).normalized;
	}
	else
	{
	q = QuaternionFromMatrix(mat);
	q = (prevJoint.GetComponent<RoboJoint3>().q*q).normalized;
	}
	Vector3 transl;
	transl.x = mat.m03;
	transl.y = mat.m13;
	transl.z = mat.m23;
        Vector3 currPos = this.transform.position;	
	Vector3 localPos = this.transform.localPosition;
	//Vector3 newPos = GameObject.Find("0").transform.position + transl;	
	//Vector3 newPos = prevJoint.transform.TransformDirection(transl);
	newPos = prevJoint.transform.TransformPoint(transl);
	//rb.MovePosition(GameObject.Find("0").transform.position + transl);	
	//string str = string.Format("{0} ", dir.x) + string.Format("{0} ", dir.y) + string.Format("{0} ", dir.z);
	//if (this.name == "2"){
	//	Debug.Log(str + "    " + this.name);
	//}
	Vector3 dir = (newPos - transform.position).normalized;
	rb.MovePosition(newPos);
	//rb.MovePosition(transform.position+dir*5.0f*Time.deltaTime);
	Vector3 v = prevJoint.transform.forward;
	finalRot = Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
	rb.MoveRotation(finalRot);
	//this.transform.position = GameObject.Find("0").transform.position + transl;	
	//this.transform.rotation =  Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
       	//Debug.Log("1 " + Time.time);
    }

	public float r2d(float deg){
		return deg/180*(float)Math.PI;
	}

	public Matrix4x4 trans(float theta, float d, float a, float alpha){
		float[,] matrix = {{Mathf.Cos(r2d(theta)), -Mathf.Sin(r2d(theta)), 0, a},
				{Mathf.Sin(r2d(theta))*Mathf.Cos(r2d(alpha)), Mathf.Cos(r2d(theta))*Mathf.Cos(r2d(alpha)), -Mathf.Sin(r2d(alpha)), -Mathf.Sin(r2d(alpha))*d},
				{Mathf.Sin(r2d(theta))*Mathf.Sin(r2d(alpha)), Mathf.Cos(r2d(theta))*Mathf.Sin(r2d(alpha)), Mathf.Cos(r2d(alpha)), Mathf.Cos(r2d(alpha))*d},
				{0,0,0,1}};
		//float[,] matrix = {{Mathf.Cos(r2d(theta)), -Mathf.Sin(r2d(theta)), 0, a},
		Matrix4x4 matrix2 = new Matrix4x4();
		matrix2.m00 = matrix[0,0]; matrix2.m01 = matrix[0,1]; matrix2.m02 = matrix[0,2]; matrix2.m03 = matrix[0,3];
		matrix2.m10 = matrix[1,0]; matrix2.m11 = matrix[1,1]; matrix2.m12 = matrix[1,2]; matrix2.m13 = matrix[1,3];
		matrix2.m20 = matrix[2,0]; matrix2.m21 = matrix[2,1]; matrix2.m22 = matrix[2,2]; matrix2.m23 = matrix[2,3];
		matrix2.m30 = matrix[3,0]; matrix2.m31 = matrix[3,1]; matrix2.m32 = matrix[3,2]; matrix2.m33 = matrix[3,3];
		//double[,] matrix2 = {{Math.Cos((double)r2d(theta)), Math.Sin((double)r2d(theta))*Math.Cos((double)r2d(alpha)), Math.Sin((double)r2d(theta))*Math.Sin((double)r2d(alpha))}};
	return matrix2;
	}

	public static Quaternion QuaternionFromMatrix(Matrix4x4 m) {
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

	private static GameObject createScaler(RoboJoint3 obj, GameObject newParent){
		GameObject parentObject = new GameObject(); //create an 'empty' object
		parentObject.name = "Scaler " + obj.name;
		parentObject.transform.parent = newParent.transform;
		//parentObject.transform.localScale = obj.transform.localScale;
		Vector3 currScale = newParent.transform.parent.transform.localScale;
		parentObject.transform.localScale = new Vector3((float)(1/currScale.x),(float)(1/currScale.y),(float)(1/currScale.z));//obj.transform.localScale;
		parentObject.transform.localPosition = new Vector3(0,0,0);
		obj.transform.parent = parentObject.transform;
		obj.transform.localScale = new Vector3(1,1,1);
		return parentObject;

	}

	 private static bool vecCompare(Vector3 me, Vector3 other)
         {
             /*var dx = me.x - other.x;
             if (Mathf.Abs(dx) > allowedDifference)
                 return false;
 
             var dy = me.y - other.y;
             if (Mathf.Abs(dy) > allowedDifference)
                 return false;
 
             var dz = me.z - other.z;
 
             return Mathf.Abs(dz) >= allowedDifference;*/ 
		 if (me.x != other.x){
			 return false;
		 }

		 if (me.y != other.y){
			 return false;
		 }

		 if (me.y != other.y){
			 return false;
		 }
		 return true;
         }

	private void globalCollision(Rigidbody rb){
		Debug.Log( "this: " + this.name + " ran global");
		thetaSlider.value = prevTheta;
		alphaSlider.value = prevAlpha;
		rb.MovePosition(prevPos);
		rb.MoveRotation(prevRot);
		alpha = prevAlpha;
		theta = prevTheta;
		newPos = prevPos;
		finalRot = prevRot;
		thisStop = true;
	
	}

	private void OnCollisionStay(Collision col){
			Debug.Log( "this: " + this.name + " is inside : " + col.collider.gameObject.name);
		if (this.GetComponent<Collider>().transform.root == col.collider.transform.root){
			Debug.Log( "this: " + this.name + " is inside : " + col.collider.gameObject.name);
			thetaSlider.value = prevTheta;
			alphaSlider.value = prevAlpha;
			rb.MovePosition(prevPos);
			rb.MoveRotation(prevRot);
			alpha = prevAlpha;
			theta = prevTheta;
			newPos = prevPos;
			finalRot = prevRot;
			GameObject.Find("0").GetComponent<RoboJoint>().stop = true;
			thisStop = false;
		}
	}
	private void OnCollisionEnter(Collision col){
			Debug.Log( "this: " + this.name + " collided with : " + col.collider.gameObject.name);
		if (this.GetComponent<Collider>().transform.root == col.collider.transform.root){
			Debug.Log( "this: " + this.name + " collided with : " + col.collider.gameObject.name);
        	//Debug.Log("2 " + Time.time);
	//if (waitCount > 200){
		//collided = true;
		//GameObject.Find(points[1]).GetComponent<RoboJoint2>().collided = true;
	//	Debug.Log("hit------------------------------------");
		//Debug.Log(hit.collider.gameObject.name + " " + this.name);
		//rb.MovePosition(hit.point);
		//rb.MovePosition(newPos);
		//rb.velocity = new Vector3(0,0,0);
	
		//if (waitCount > 300){
		//targetTheta = prevTheta;
		//targetAlpha = prevAlpha;
		//Debug.Log("theta: " + theta + " prev " + prevTheta);
		//Debug.Log("alpha: " + alpha + " prev " + prevAlpha);
		
			thetaSlider.value = prevTheta;
			alphaSlider.value = prevAlpha;
			rb.MovePosition(prevPos);
			rb.MoveRotation(prevRot);
			alpha = prevAlpha;
			theta = prevTheta;
			newPos = prevPos;
			finalRot = prevRot;
			GameObject.Find("0").GetComponent<RoboJoint>().stop = true;
			thisStop = false;
		}
		//StartCoroutine(Example(prevRot, prevPos, rb));
		//Time.timeScale=0;
	//}
	//Debug.Log( "collide (tag) : " + col.collider.gameObject.tag );
	//rb.MovePosition(this.transform.position);
	//this.enabled = false;
	//rb.MovePosition(oldPos);
	}
/*	
	IEnumerator Example(Quaternion q, Vector3 pos, Rigidbody rb)
    	{
        	yield return new WaitForFixedUpdate();
        	Debug.Log("3 " + Time.time);
		//rb.MovePosition(pos);
		//rb.MoveRotation(q);
    	}
*/
}

