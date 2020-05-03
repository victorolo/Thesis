using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public Vector3 offset = new Vector3(0,0,0);
    public float scale = 1f;
    GameObject child;
    // Start is called before the first frame update
    void Start()
    {
	child = GameObject.Find("GameObject").gameObject;
	Vector3 location =  child.transform.position + offset;
	this.transform.position = location;
	this.transform.localScale = new Vector3(scale, scale, scale);
	this.transform.SetParent(child.transform, false);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       // add locked motion configurebale joint after 300 wait count to child
    }
}
