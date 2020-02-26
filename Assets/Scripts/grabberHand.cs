using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class grabberHand : MonoBehaviour
{
    GameObject parent;
    GameObject jointParent;
    Rigidbody rb;
    public float targetGrab;
    public float grab;
    public float prevGrab;
    private float origScale;
    Slider grabSlider;
    bool firstRun;
    ConfigurableJoint cj;
    JointDrive drive;
    int waitCount;
    public GameObject grabbed;
    public float moveBy;
    // Start is called before the first frame update
    void Start()
    {
	moveBy = 1f;
	grabbed = this.gameObject;
	waitCount = 0;
    	parent = GameObject.Find("grabber");
	jointParent = GameObject.Find("3");
        rb = this.gameObject.AddComponent<Rigidbody>();
	rb.useGravity = false;
	grab = 0;
	prevGrab = 0;
	float origScale = this.transform.localScale.z;
	firstRun = true; 
	//rb.isKinematic = true;
	//string slider ="Slider (" + (GameObject.Find("0").GetComponent<RoboJoint>()) + ")"; 

    }

    // Update is called once per frame
    void FixedUpdate()
    {
	if (firstRun){	
		//joint.connectedBody = parent.GetComponent<Rigidbody>();
		string sliderNo ="Slider (" + (GameObject.Find("0").GetComponent<RoboJoint>().DHTable.Length/2).ToString() + ")"; 
		grabSlider = GameObject.Find(sliderNo).GetComponent<Slider>();
		firstRun = false; 
		Debug.Log(this.transform.localScale.x/2);
		//this.gameObject.transform.SetParent(parent.transform);
		if (this.name == "hand 1"){
			this.transform.position = (parent.transform.position + new Vector3(parent.transform.localScale.x/2 + this.transform.localScale.x/2, 0,-parent.transform.localScale.z/2 + this.transform.localScale.z/2));
		}

		else if (this.name == "hand 2"){
			this.transform.position = (parent.transform.position + new Vector3(parent.transform.localScale.x/2 + this.transform.localScale.x/2, 0, parent.transform.localScale.z/2 - this.transform.localScale.z/2));
		}
	}
	if (waitCount == 200){
		if (this.name == "hand 1"){
			this.transform.position = (parent.transform.position + new Vector3(parent.transform.localScale.x/2 + this.transform.localScale.x/2, 0,-parent.transform.localScale.z/2 + this.transform.localScale.z/2));
		}

		else if (this.name == "hand 2"){
			this.transform.position = (parent.transform.position + new Vector3(parent.transform.localScale.x/2 + this.transform.localScale.x/2, 0, parent.transform.localScale.z/2 - this.transform.localScale.z/2));
		}
		cj = this.gameObject.AddComponent<ConfigurableJoint>();
		cj.connectedBody = jointParent.GetComponent<Rigidbody>();
		cj.xMotion = ConfigurableJointMotion.Locked;
		cj.yMotion = ConfigurableJointMotion.Locked;
		//cj.zMotion = ConfigurableJointMotion.Limited;
		cj.zMotion = ConfigurableJointMotion.Locked;
		SoftJointLimit linLim = new SoftJointLimit();
       		linLim.limit =  parent.transform.localScale.z/2;;
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
		rb.isKinematic = false;
	}
	waitCount ++;
	targetGrab = grabSlider.value; // both these should be created from the robi file
	if (grab + 1 <= targetGrab){
		prevGrab = grab;
		grab += 1f;
	}
	else if (grab - 1 >= targetGrab){
		prevGrab = grab;
		grab -= 1f;
	}
        //this.transform.localScale = tempScale;
	//rb.MovePosition(parent.transform.position + transform.TransformDirection(new Vector3 (0.1f, 0f ,0f)));// change to half scale later
	//Vector3 moveTo = this.transform.TransformPoint(new Vector3(0f,0f,0f));
	//rb.MovePosition(moveTo);// change to half scale later
	if (this.name == "hand 1"){
		ConfigurableJoint grabbedJoint = null;
		if (grabbed == GameObject.Find("hand 2").GetComponent<grabberHand>().grabbed){
			if (grabbed.GetComponent<ConfigurableJoint>() == null || grabbed.GetComponent<ConfigurableJoint>().connectedBody != rb){
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
				grab = prevGrab;
				targetGrab = prevGrab;
			}
		}
	}

	if (this.name == "hand 1" && grabbed != GameObject.Find("hand 2").GetComponent<grabberHand>().grabbed){
		if (grabbed.GetComponent<ConfigurableJoint>() != null && grabbed.GetComponent<ConfigurableJoint>().connectedBody == rb){
			Destroy(grabbed.GetComponent<ConfigurableJoint>());
			Debug.Log("destroy");
			grab = prevGrab;
			targetGrab = prevGrab;
		}
	}
	if (waitCount > 200){
		//cj.targetPosition = Math.Sign(this.transform.localPosition.z)*tempScale*(parent.transform.lossyScale.z)/2;
		cj.autoConfigureConnectedAnchor = false;
		Vector3 tempScale = cj.connectedAnchor;
		if (this.name == "hand 1"){
			tempScale.z = -grab/100f * (parent.transform.localScale.z/2 + this.transform.localScale.z/2);
			cj.connectedAnchor = tempScale; 
		}

		else if (this.name == "hand 2"){
			tempScale.z = grab/100f * (parent.transform.localScale.z/2 + this.transform.localScale.z/2);
			cj.connectedAnchor = tempScale; 
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
    }


	private void OnCollisionStay(Collision col){
		Debug.Log( "this: " + this.name + " is inside : " + col.collider.gameObject.name);
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
		prevGrab ++;
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
       
	}
/*	private void OnCollisionEnter(Collision col){
		Debug.Log( "this: " + this.name + " hit : " + col.collider.gameObject.name);
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
	}*/
}
