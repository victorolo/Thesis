using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class grabber : MonoBehaviour
{
    public GameObject parent;
    Renderer rend;
    int waitCount;
    Rigidbody rb;
    public bool move;
    public float grab;
    private float origScale;
    //Slider grabSlider;
    bool firstRun;
    Vector3 thisSizes;
    ConfigurableJoint cj;
    // Start is called before the first frame update
    void Start()
    {
	waitCount = 0;
	Renderer thisRend = GetComponent<Renderer>();
	thisSizes = thisRend.bounds.max;
    	//parent = this.transform.parent.gameObject;
	//parent = GameObject.Find("3");
        rb = this.gameObject.AddComponent<Rigidbody>();
	rb.useGravity = false;
	//rb.MovePosition(parent.transform.position + new Vector3(parent.transform.localScale.x/2 + this.transform.localScale.x/2,0,0));
	Vector3 sizes2 = Tools.GetDimensions(gameObject);
	Vector3 sizes = Tools.GetDimensions(parent);
	//this.transform.position = (parent.transform.position + new Vector3(parent.transform.localScale.x/2 + this.transform.localScale.x/2,0,0));
	this.transform.position = (parent.transform.position + new Vector3(sizes.x,0,0));
	//joint = GetComponent<ConfigurableJoint>();
	//grab = 10;
	rb.isKinematic = true;
	float origScale = this.transform.localScale.z;
	firstRun = true;
	this.GetComponent<MeshRenderer>().enabled = false;
	this.GetComponent<Collider>().enabled = false;
	//string slider ="Slider (" + (GameObject.Find("0").GetComponent<RoboJoint>()) + ")"; 

    }

    // Update is called once per frame
    void FixedUpdate()
    {
	if (firstRun){	
		//string sliderNo ="Slider (" + (GameObject.Find("0").GetComponent<RoboJoint>().DHTable.Length/2).ToString() + ")"; 
		//grabSlider = GameObject.Find(sliderNo).GetComponent<Slider>();
		firstRun = false;
		//this.gameObject.transform.SetParent(parent.transform);
	}
	else {
		rend = parent.GetComponent<Renderer>();
		Renderer thisRend = GetComponent<Renderer>();
		//Vector3 sizes = rend.bounds.max;
		//Vector3 thisSizes = thisRend.bounds.max;
		if (waitCount < 150){
		//this.transform.position = (parent.transform.position + new Vector3(parent.transform.localScale.x/2 + this.transform.localScale.x/2,0,0));
			Vector3 sizes2 = Tools.GetDimensions(gameObject);
			Vector3 sizes = Tools.GetDimensions(parent);
			Debug.Log(parent.transform.position + new Vector3(sizes.x,0,0));
			this.transform.position = (parent.transform.position + new Vector3(sizes.x,0,0));
	//		this.transform.position = (parent.transform.position + new Vector3(0.1f + parent.transform.localScale.x/2 + this.transform.localScale.x/2,0,0));
			//this.transform.position = new Vector3(sizes.x + this.transform.localScale.x/2 + 0.1f,0,0);
			
		}
		if (waitCount == 200){
			//this.gameObject.transform.SetParent(parent.transform);
			cj = this.gameObject.AddComponent<ConfigurableJoint>();
			cj.connectedBody = parent.GetComponent<Rigidbody>();
			cj.xMotion = ConfigurableJointMotion.Locked;
			cj.yMotion = ConfigurableJointMotion.Locked;
			cj.zMotion = ConfigurableJointMotion.Locked;
			//cj.anchor = new Vector3(0f, 0f, 0f);
			cj.enableCollision = true; 
			rb.isKinematic = false;
		}
		waitCount++;
		//grab = grabSlider.value; // both these should be created from the robi file
		Vector3 tempScale = this.transform.localScale;
		//tempScale.z = (grab+10)/100f;
	//this.transform.localScale = tempScale;
		//rb.MovePosition(parent.transform.position + transform.TransformDirection(new Vector3 (0.1f, 0f ,0f)));// change to half scale later
		//rb.AddForce(parent.transform.position + transform.TransformDirection(new Vector3 (0.1f, 0f ,0f)) - this.transform.position, ForceMode.VelocityChange);
		//rb.velocity = (parent.transform.position + transform.TransformDirection(new Vector3 (parent.transform.localScale.x/2 + this.transform.localScale.x/2, 0f ,0f)) - this.transform.position)*20f;
		//joint.targetPosition = tempScale;
		if (move){
			//rb.AddTorque(transform.forward * 2.5f);
			//Debug.Log(move);
		}
	}
    }
}
