﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public Vector3 offset = new Vector3(0,0,0);
    public float scale = 1f;
    GameObject child;
    GameObject baseJoint;
    Quaternion baseQ;
    // Start is called before the first frame update
    void Start()
    {
	baseJoint = GameObject.Find("0").gameObject;
	child = GameObject.Find("GameObject").gameObject;
	Vector3 location =  child.transform.position + offset;
	this.transform.position = location;
	this.transform.localScale = new Vector3(scale, scale, scale);
	this.transform.SetParent(child.transform, false);
       	baseQ = Quaternion.Inverse(baseJoint.transform.rotation); 
	Vector3 pos = location;
	pos.y = Terrain.activeTerrain.SampleHeight(transform.position) + Tools.GetDimensions(gameObject).y/2;
	this.transform.position = pos;
	baseJoint.GetComponent<RoboJoint>().yOff = pos.y + location.y;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
       // add locked motion configurebale joint after 300 wait count to child
      //this.transform.rotation = baseJoint.transform.rotation*baseQ;//;Quaternion.Euler(new Vector3( 0, baseJoint.transform.rotation.eulerAngles.z, 0)); 
      this.transform.rotation = Quaternion.Euler(new Vector3( 0, baseJoint.transform.rotation.eulerAngles.y, 0)); 
    }
}
