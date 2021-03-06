﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class grabberHand : MonoBehaviour
{
    public string sliderNo;
    public GameObject parent;
    public GameObject jointParent;
    Rigidbody rb;
    public float targetGrab;
    public float grab;
    public float[] prevGrab;
    public Vector3 prevPos;
    public Quaternion prevRot;
    private float origScale;
    Slider grabSlider;
    bool firstRun;
    ConfigurableJoint cj;
    JointDrive drive;
    int waitCount;
    public GameObject grabbed;
    public float moveBy;
    public float xscale;
    Vector3 sizes;
    Vector3 sizes2;
    public float otherScale;
    private bool thisStop;
    public bool justCollided;
    public Vector3 offsets = new Vector3(1f,1f,1f);
    // Start is called before the first frame update
    void Start()
    {
	thisStop = true;
	justCollided = false;
	moveBy = 1f;
	grabbed = this.gameObject;
	waitCount = 0;
    	//parent = GameObject.Find("grabber");
	//jointParent = GameObject.Find("3");
        rb = this.gameObject.AddComponent<Rigidbody>();
	rb.useGravity = false;
	grab = 0;
	prevGrab = new float[2];
	prevGrab[0] = 0;
	prevPos = new Vector3(0f,0f,0f);
	prevRot = this.transform.rotation;
	float origScale = this.transform.localScale.z;
	firstRun = true; 
	//float otherScale = parentJoint.getComponent<Robo
	this.transform.localScale = new Vector3(xscale, 1f, 1f);	
	rb.isKinematic = true;
	sizes = new Vector3(0f,0f,0f);//= GetDimensions(gameObject);
	sizes2 = sizes;//= GetDimensions(parent);
	//string slider ="Slider (" + (GameObject.Find("0").GetComponent<RoboJoint>()) + ")"; 

    }

    // Update is called once per frame
    void FixedUpdate()
    {
	//if (GameObject.Find("0").GetComponent<RoboJoint>().stop && thisStop){
	//	globalCollision(rb);
	//}	
	thisStop = true;
	if (firstRun){	
		//joint.connectedBody = parent.GetComponent<Rigidbody>();
		//string sliderNo ="Slider (" + (GameObject.Find("0").GetComponent<RoboJoint>().DHTable.Length/2).ToString() + ")"; 
		Debug.Log(parent);
		otherScale = parent.GetComponent<RoboJoint3>().otherScale;
		this.transform.localScale = new Vector3(otherScale, otherScale, otherScale);	
		grabSlider = GameObject.Find(sliderNo).GetComponent<Slider>();
		firstRun = false; 
		Debug.Log(this.transform.localScale.x/2);
		//this.gameObject.transform.SetParent(parent.transform);
		sizes = GetDimensions(gameObject);
		sizes2 = GetDimensions(parent);
		Debug.Log((-sizes2.z/2 - sizes.z/2) + "    " + (sizes2.z/2 + sizes.z/2));
		//Debug.Log(sizes);
		if (this.name == "hand 1"){
			this.transform.position = (parent.transform.position + new Vector3(1f + sizes2.x/2 + sizes.x/2, 0,-sizes2.z/2 - sizes.z/2));
		}

		else if (this.name == "hand 2"){
			//this.transform.eulerAngles = new Vector3(180f, 0f,0f);
			this.transform.position = (parent.transform.position + new Vector3(0.1f + sizes2.x/2 + sizes.x/2, 0, sizes2.z/2 + sizes.z/2));
		}
	}
	if (waitCount == 200){
		//Debug.Log((parent.transform.localScale.x/2 + this.transform.localScale.x/2) + "        " +  (parent.transform.localScale.x/2 + this.transform.localScale.x/2));

		//Debug.Log(sizes.z/2 + "    " + sizes2.z/2);
		sizes = GetDimensions(gameObject);
		sizes2 = GetDimensions(parent);
		//Debug.Log((parent.transform.position + new Vector3(sizes2.x/2 + sizes.x/2, 0,-sizes2.z/2 - sizes.z/2)) + "    " + (parent.transform.position + new Vector3(0.1f + sizes2.x/2 + sizes.x/2, 0,sizes2.z/2 + sizes.z/2)));
		//Debug.Log(" DEBUG " + (-sizes2.z/2 - sizes.z/2) + "    " + (sizes2.z/2 + sizes.z/2));
		//Debug.Log(sizes);
		if (this.name == "hand 1"){
			//this.transform.position = (parent.transform.position + new Vector3(0.15f + sizes2.x/2 + sizes.x/2, 0,-sizes2.z/2 - sizes.z/2));
			//this.transform.position = (parent.transform.position + new Vector3(sizes2.x/2, (-sizes2.y/2 - sizes.y/2)*0.7f,-sizes2.z/2 - sizes.z/2));
			this.transform.position = (parent.transform.position + new Vector3(sizes2.x/2*offsets.x, (-sizes2.y/2 - sizes.y/2)*offsets.y,-sizes2.z/2 - sizes.z/2));
		}

		else if (this.name == "hand 2"){
			//this.transform.eulerAngles = new Vector3(180f, 0f,0f);
			//this.transform.position = (parent.transform.position + new Vector3(0.15f + sizes2.x/2 + sizes.x/2, 0, sizes2.z/2 + sizes.z/2));
			//this.transform.position = (parent.transform.position + new Vector3(sizes2.x/2, (-sizes2.y/2 - sizes.y/2)*0.7f, sizes2.z/2 + sizes.z/2));
			this.transform.position = (parent.transform.position + new Vector3(sizes2.x/2*offsets.x, (-sizes2.y/2 - sizes.y/2)*offsets.y, sizes2.z/2 + sizes.z/2));
		}
		rb.isKinematic = false;
		this.transform.rotation = (parent.transform.rotation);
	}
	if (waitCount >= 200){
		if (cj == null && !justCollided){
			this.transform.rotation = (parent.transform.rotation);
			cj = this.gameObject.AddComponent<ConfigurableJoint>();
			cj.connectedBody = jointParent.GetComponent<Rigidbody>();
			cj.xMotion = ConfigurableJointMotion.Locked;
			cj.yMotion = ConfigurableJointMotion.Locked;
			//cj.zMotion = ConfigurableJointMotion.Limited;
			cj.zMotion = ConfigurableJointMotion.Locked;
			SoftJointLimit linLim = new SoftJointLimit();
			linLim.limit =  sizes2.z/2*offsets.z;
			cj.linearLimit = linLim;
			cj.angularXMotion = ConfigurableJointMotion.Locked;
			cj.angularYMotion = ConfigurableJointMotion.Locked;
			cj.angularZMotion = ConfigurableJointMotion.Locked;
			cj.anchor = new Vector3(0f, 0f, 0f);
			cj.enableCollision = true;
			drive = new JointDrive();
			drive.positionSpring = 100;
			drive.positionDamper = 50;
			drive.maximumForce = 10;
			cj.zDrive = drive;
			cj.connectedMassScale = 0;
		}
	}
	if (waitCount == 300){

	}
	waitCount ++;
	Physics.IgnoreCollision(parent.GetComponent<Collider>(), this.GetComponent<Collider>(), true);
	
	targetGrab = grabSlider.value; // both these should be created from the robi file
	prevRot = this.transform.rotation;
	prevPos = this.transform.position;
	if (grab + 2 <= targetGrab){
		if (!justCollided){
			prevGrab[1] = prevGrab[0];
			prevGrab[0] = grab;
		}
		grab += 2f;
		prevPos = this.transform.position;
	}
	else if (grab - 2 >= targetGrab){
		if (!justCollided){
			prevGrab[1] = prevGrab[0];
			prevGrab[0] = grab;
		}
		grab -= 2f;
		prevPos = this.transform.position;
	}
        //this.transform.localScale = tempScale;
	//rb.MovePosition(parent.transform.position + transform.TransformDirection(new Vector3 (0.1f, 0f ,0f)));// change to half scale later
	//Vector3 moveTo = this.transform.TransformPoint(new Vector3(0f,0f,0f));
	//rb.MovePosition(moveTo);// change to half scale later
	if (this.name == "hand 1"){
		ConfigurableJoint grabbedJoint = null;
		if (grabbed == GameObject.Find("hand 2").GetComponent<grabberHand>().grabbed && grabbed != this.gameObject){
			if (grabbed.GetComponent<ConfigurableJoint>() == null || grabbed.GetComponent<ConfigurableJoint>().connectedBody !=rb){
				Debug.Log(grabbed.name);
				grabbedJoint = grabbed.AddComponent<ConfigurableJoint>();
				grabbedJoint.connectedBody = rb;
				grabbedJoint.xMotion = ConfigurableJointMotion.Locked;
				grabbedJoint.yMotion = ConfigurableJointMotion.Locked;
				grabbedJoint.zMotion = ConfigurableJointMotion.Locked;
				grabbedJoint.angularXMotion = ConfigurableJointMotion.Locked;
				grabbedJoint.angularYMotion = ConfigurableJointMotion.Locked;
				grabbedJoint.angularZMotion = ConfigurableJointMotion.Locked;
			}
		}
		else {
			if (grabbed.GetComponent<ConfigurableJoint>() != null && grabbed.GetComponent<ConfigurableJoint>().connectedBody == rb){
				Destroy(grabbed.GetComponent<ConfigurableJoint>());
				Debug.Log("destroy");
				//grab = prevGrab[0];
				targetGrab = prevGrab[0];
				grabSlider.value = prevGrab[0];
			}
		}
	}

	if (this.name == "hand 1" && grabbed != GameObject.Find("hand 2").GetComponent<grabberHand>().grabbed){
		if (grabbed.GetComponent<ConfigurableJoint>() != null && grabbed.GetComponent<ConfigurableJoint>().connectedBody == rb){
			Destroy(grabbed.GetComponent<ConfigurableJoint>());
			Debug.Log("destroy");
			//grab = prevGrab[0];
			targetGrab = prevGrab[0];
			grabSlider.value = prevGrab[0];
		}
	}
	if (waitCount > 200){
		/*rb.MoveRotation(parent.transform.rotation);
		if (this.name == "hand 1"){
			rb.MovePosition(parent.transform.TransformPoint(new Vector3(sizes2.x/2/otherScale, (-sizes2.y/2 - sizes.y/2)*1.1f/otherScale,(-sizes2.z/2 - sizes.z/2)*grab/100f/otherScale)));
			//Debug.Log(parent.transform.TransformPoint(new Vector3(sizes2.x/2, (-sizes2.y/2 - sizes.y/2)*1.1f,(-sizes2.z/2 - sizes.z/2)*grab/100f/otherScale)));
		}

		else if (this.name == "hand 2"){
			rb.MovePosition(parent.transform.TransformPoint(new Vector3(sizes2.x/2/otherScale, (-sizes2.y/2 - sizes.y/2)*1.1f/otherScale, (sizes2.z/2 + sizes.z/2)*grab/100f/otherScale)));
		}*/
		//cj.targetPosition = Math.Sign(this.transform.localPosition.z)*tempScale*(parent.transform.lossyScale.z)/2;
		if (!justCollided && cj != null){
			cj.autoConfigureConnectedAnchor = false;
			Vector3 tempScale = cj.connectedAnchor;
			if (this.name == "hand 1"){
				tempScale.z = -grab/100f * (sizes2.z/2 + sizes.z/2)/otherScale*offsets.z;
				cj.connectedAnchor = tempScale; 
			}

			else if (this.name == "hand 2"){
				tempScale.z = grab/100f * (sizes2.z/2 + sizes.z/2)/otherScale*offsets.z;
				cj.connectedAnchor = tempScale; 
			}
		}
		//if (grab < prevGrab + 5 && prevGrab > prevGrab -5){
		//	cj.zMotion = ConfigurableJointMotion.Limited;
		//}	
		//else{
			//cj.zMotion = ConfigurableJointMotion.Locked;
			//cj.connectedBody = null;
			//cj.connectedBody = jointParent.GetComponent<Rigidbody>();
		//}
		moveBy = 1f;
	}


	grabbed = this.gameObject;
	justCollided = false;
    }
	private void globalCollision(Rigidbody rb){
		Debug.Log( "this: " + this.name + " ran global");
		this.transform.position = prevPos;
		this.transform.rotation = prevRot;

		thisStop = true;
	
	}


	private void OnCollisionStay(Collision col){
		Debug.Log( "this: " + this.name + " is inside : " + col.collider.gameObject.name);

		if (col.collider.gameObject.name == "Terrain"){
			GameObject.Find("0").GetComponent<RoboJoint>().stop = true;
			//this.transform.position = prevPos;
			//this.transform.rotation = prevRot;
			this.transform.rotation = (parent.transform.rotation);

			thisStop = true;
			Debug.Log("ran stop");
		}
		if (col.collider.gameObject.name != "Terrain"){
			if (this.GetComponent<Collider>().transform.root == col.collider.transform.root && this.name == "hand 1" && col.collider.gameObject.name != "hand 2"){
				GameObject.Find("0").GetComponent<RoboJoint>().stop = true;
			}
			justCollided= true;
			Debug.Log(grab);
			if (this.GetComponent<Collider>().transform.root == col.collider.transform.root){
				Destroy(cj);
				rb.MoveRotation(parent.transform.rotation);
				if (this.name == "hand 1"){
					rb.MovePosition(parent.transform.TransformPoint(new Vector3(sizes2.x/2/otherScale*offsets.x, (-sizes2.y/2 - sizes.y/2)/otherScale*offsets.y,(-sizes2.z/2 - sizes.z/2)*grab/100f/otherScale*offsets.z)));
					//Debug.Log(parent.transform.TransformPoint(new Vector3(sizes2.x/2, (-sizes2.y/2 - sizes.y/2)*1.1f,(-sizes2.z/2 - sizes.z/2)*grab/100f/otherScale)));
				}

				else if (this.name == "hand 2"){
					rb.MovePosition(parent.transform.TransformPoint(new Vector3(sizes2.x/2/otherScale*offsets.x, (-sizes2.y/2 - sizes.y/2)*offsets.y/otherScale, (sizes2.z/2 + sizes.z/2)*grab/100f/otherScale*offsets.z)));
				}
			}
			//grabSlider.value = prevGrab;
			//targetGrab = prevGrab[0];
			//grabSlider.value = prevGrab[0];
			//this.transform.position = prevPos;
			//this.transform.rotation = prevRot;
			//this.transform.eulerAngles = prevRot;
			else{
				Vector3 tempScale = cj.connectedAnchor;
				if (this.name == "hand 1"){
					tempScale.z = -grab/100f * (sizes2.z/2 + sizes.z/2)/otherScale*offsets.z;
					cj.connectedAnchor = tempScale; 
				}

				else if (this.name == "hand 2"){
					tempScale.z = grab/100f * (sizes2.z/2 + sizes.z/2)/otherScale*offsets.z;
					cj.connectedAnchor = tempScale; 
				}
			}
			//prevGrab[0] ++;
			//grab = targetGrab;
			grab = prevGrab[0];
			prevGrab[0] = prevGrab[1];

			Vector3 normal = col.contacts[0].normal;
	   
			thisStop = false;
	       
			if (col.collider.gameObject != parent){
				if(vecCompare(normal, transform.forward))
				{
					Debug.Log("WORKED FORWARD " + this.name);
					if (this.name == "hand 1"){
						grabbed = col.gameObject;
						moveBy = 5f;
					}
				}
		       
				else if(vecCompare(normal,-(transform.forward)))
				{
					Debug.Log("WORKED BACKWARD");
					if (this.name == "hand 2"){
						grabbed = col.gameObject;
						moveBy = 5f;
					}
				}
			}
		}
	}
	private void OnCollisionEnter(Collision col){
		Debug.Log( "this: " + this.name + " is inside : " + col.collider.gameObject.name);
		if (col.collider.gameObject.name == "Terrain"){
			GameObject.Find("0").GetComponent<RoboJoint>().stop = true;
			//this.transform.position = prevPos;
			//this.transform.rotation = prevRot;
			this.transform.rotation = (parent.transform.rotation);

			thisStop = true;
			Debug.Log("ran stop");
		}
		if (col.collider.gameObject.name != "Terrain"){

		if (this.GetComponent<Collider>().transform.root == col.collider.transform.root && this.name == "hand 1" && col.collider.gameObject.name != "hand 2"){
			GameObject.Find("0").GetComponent<RoboJoint>().stop = true;
		}
		thisStop = false;
		/*Debug.Log( "this: " + this.name + " hit : " + col.collider.gameObject.name);
		//grabSlider.value = prevGrab;
		targetGrab = prevGrab;
		Vector3 tempScale = cj.connectedAnchor;
		if (this.name == "hand 1"){
			tempScale.z = -prevGrab/100f * (parent.transform.localScale.z/2 + this.transform.localScale.z/2);
			cj.connectedAnchor = tempScale; 
		}

		else if (this.name == "hand 2"){
			tempScale.z = prevGrab/100f * (parent.transform.localScale.z/2 + this.transform.localScale.z/2);
			cj.connectedAnchor = tempScale; 
		}
		grab = targetGrab;

		Vector3 normal = col.contacts[0].normal;

		if(normal == transform.forward)
		{
			//Debug.Log("WORKED FORWARD " + this.name);
			if (this.name == "hand 2"){
				grabbed = col.gameObject;
				moveBy = 5f;
			}
		}
       
		else if(normal == -(transform.forward))
		{
			//Debug.Log("WORKED BACKWARD");
			if (this.name == "hand 1"){
				grabbed = col.gameObject;
				moveBy = 5f;
			}
		}
	*/}
	}
	private Vector3 GetDimensions(GameObject obj)
	{
	Vector3 min = Vector3.one * Mathf.Infinity;
	Vector3 max = Vector3.one * Mathf.NegativeInfinity;

	Mesh mesh = obj.GetComponent<MeshFilter>().mesh;

	for (int i = 0; i < mesh.vertices.Length; i++)
		{
			Vector3 vert = mesh.vertices[i];
			min = Vector3.Min(min, vert);
			max = Vector3.Max(max, vert);
		}

		// the size is max-min multiplied by the object scale:
		return Vector3.Scale(max - min, obj.transform.localScale);
	}

	private static bool vecCompare(Vector3 me, Vector3 other)
        {
            var dx = me.x - other.x;
	    float allowedDifference = 0.5f;
            if (Mathf.Abs(dx) > allowedDifference)
                return false;
 
            var dy = me.y - other.y;
            if (Mathf.Abs(dy) > allowedDifference)
                return false;

            var dz = me.z - other.z;

            return Mathf.Abs(dz) >= allowedDifference; 
	}	
}
