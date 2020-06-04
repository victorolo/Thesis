using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bits : MonoBehaviour
{
    public string jointNum = "0";
    public Vector3 offset = new Vector3(0,0,0);
    public int jointRotNum = 0;
    GameObject joint;
    GameObject jointRot;
    public float scale = 1;
    public int useLocal = 0;
    // Start is called before the first frame update
    void Start()
    {
	this.transform.SetParent(GameObject.Find("GameObject").transform);
        joint = GameObject.Find(jointNum);
	if (jointRotNum >= 0){
		jointRot = GameObject.Find(jointRotNum.ToString());
	}
	if (useLocal == 1){
		GameObject parentObject = new GameObject(); //create an 'empty' object
		parentObject.transform.SetParent(GameObject.Find("GameObject").transform);
		parentObject.transform.SetParent(joint.transform);
		parentObject.name = "Scaler " + this.name;
		parentObject.transform.position = joint.transform.TransformPoint(offset);
		//this.transform.position = joint.transform.TransformPoint(offset);
		this.transform.position = joint.transform.position + offset;// + joint.transform.InverseTransformPoint(offset);
		this.transform.SetParent(parentObject.transform);
	}
	this.transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
	if (useLocal == 0){
		this.transform.position = joint.transform.position + offset;// + joint.transform.InverseTransformPoint(offset);
	}
	else{
		//this.transform.position = joint.transform.TransformPoint(offset/scale);
	}
	if (jointRotNum >= 0){
		this.transform.rotation = jointRot.transform.rotation;
	}
	else {
		//this.transform.rotation = Quaternion.Euler(new Vector3(0f, joint.transform.rotation.y, 0f));
		this.transform.rotation = Quaternion.Euler(new Vector3( 0, joint.transform.rotation.eulerAngles.y, 0));
	}
    }
}
