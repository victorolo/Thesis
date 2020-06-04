using System;
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
	public float length = 5;
	public float d = 0;
	public int xRot = 0;
	public int yRot = 0;
	public int zRot = 0;
	Quaternion modelRot;
	public float targetTheta;
	//private float targetTheta;
	[Range(-180f, 180f)]
	public float alpha;
	private float targetAlpha;
	public float[] prevTheta = new float[3];
	private float[] prevAlpha = new float[3];
	public Vector3[] prevPos = new Vector3[3];
	private Quaternion[] prevRot = new Quaternion[3];
	public bool collided;
	public GameObject colWith;
	private int lastMove;
	Slider thetaSlider;
	Slider alphaSlider;
	Slider lengthSlider;
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
	public float otherScale = 1f;
	public static bool IKReverse;
	private int partNo;
	public bool justCollided;
	public bool rotGripper = true;
	public Quaternion q2;
	GameObject baseJoint;
    // Start is called before the first frame update
    void Start()
    {
	baseJoint = GameObject.Find("0");	
	q2 = Quaternion.Euler(0f,0f,0f);
	this.GetComponent<Collider>().enabled = false;
	prevRot[0] = Quaternion.Euler(0,0,0); 
	IKReverse = false;
	waitCount = 0;
	collided = false;
	justCollided = false;
	//length = 5;
	//d = 0;
	partNo = Int32.Parse(this.name);
	prevJoint = GameObject.Find((partNo-1).ToString());
	nextJoint = GameObject.Find((partNo+1).ToString());
	thetaSlider = GameObject.Find("Slider (" + (partNo*2).ToString() + ")").GetComponent<Slider>();
	theta = thetaSlider.value; // both these should be created from the robi file
	alphaSlider = GameObject.Find("Slider (" + (partNo*2 + 1).ToString() + ")").GetComponent<Slider>();
	Debug.Log("Slider (l" + (partNo*2).ToString() + ")");
	//lengthSlider = GameObject.Find("Slider (l" + (partNo*2).ToString() + ")").GetComponent<Slider>();

	alpha = alphaSlider.value;
	DHTable = new float[,]{{theta,d,length,alpha},{0,0,0,0},{0,0,0,0},{0,0,0,0}};
	rb = this.gameObject.AddComponent<Rigidbody>();
	rb.useGravity = false;
	rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	rb.isKinematic=true;
	rb.mass = (float)Math.Pow(10, 5-partNo);
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
	//prevRot[0] = q; 
	Vector3 transl;
	transl.x = mat.m03;
	transl.y = mat.m13;
	transl.z = mat.m23;
        Vector3 currPos = this.transform.position;	
	prevPos[0] = newPos;
	prevRot[0] = q;
	thisStop = true;
	newPos = prevJoint.transform.TransformPoint(transl);
	//this.transform.position = newPos;
	finalRot = Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
	//this.transform.rotation = (finalRot);
	//this.transform.position = transl;	
	//this.transform.position = GameObject.Find("0").transform.position + transl;	
	//rb.MovePosition(GameObject.Find("0").transform.position + transl);	
	//this.transform.rotation =  Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
	//rb.MoveRotation(Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth));
	//parentObject = createScaler(this, GameObject.Find("GameObject"), xRot, yRot, zRot);
	modelRot = Quaternion.Euler(new Vector3(xRot, yRot, zRot));

	//this.transform.position = transl;	
	//length = 1;
	this.transform.localScale = new Vector3(otherScale, otherScale, otherScale);
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
		//cj.xMotion = ConfigurableJointMotion.Locked;
		//cj.yMotion = ConfigurableJointMotion.Locked;
		//cj.zMotion = ConfigurableJointMotion.Locked;
		cj.anchor = new Vector3(0f, 0f, 0f);
		cj.enableCollision = true; 
		cj.connectedMassScale = 0;
	}
	if (waitCount > 100){
		//rb.isKinematic=false;
	}
	waitCount++;
	/*if (prevTheta != theta){
		prevTheta = theta;
	}
	if (prevAlpha != alpha){
		prevAlpha = alpha;
	}*/
	
	if (!justCollided && Mathf.Abs(prevTheta[0]-theta) >= 1.5f){
		prevTheta[1] = prevTheta[0];
		prevRot[1] = prevRot[0];
		prevPos[1] = prevPos[0];
		prevTheta[0] = theta;
		prevRot[0] = finalRot.normalized;
		prevPos[0] = newPos;
	}
	if (!justCollided && Mathf.Abs(prevAlpha[0] - alpha) >= 1.5f){
		prevAlpha[1] = prevAlpha[0];
		prevRot[1] = prevRot[0];
		prevPos[1] = prevPos[0];
		prevAlpha[0] = alpha;
		prevRot[0] = finalRot.normalized;
		prevPos[0] = newPos;
	}

	/*if (targetTheta > theta){
		prevTheta = theta -1;
	}
	else {
		prevTheta = theta +1;
	}
	if (targetAlpha > alpha){
		prevAlpha = alpha -1;
	}
	else {
		prevAlpha = alpha +1;
	}*/


	/*if (!vecCompare(prevPos, newPos)){
		prevPos = newPos;
	}
	if (Mathf.Abs(Quaternion.Angle(prevRot, finalRot)) > 0.99){
		prevRot = finalRot;
	}*/
	targetTheta = thetaSlider.value; // both these should be created from the robi file
	targetAlpha = alphaSlider.value;
	//length = lengthSlider.value;

	if (waitCount >= 400){
		if (nextJoint == null){
			this.GetComponent<Collider>().enabled = true;
		}
		if (!this.GetComponent<Collider>().transform.root.GetComponent<InverseKinematics>().contIK){
			if (theta + 0.99 <= targetTheta){
				theta++;
			}
			else if (theta - 0.99 >= targetTheta){
				theta--;
			}
		
			if (alpha + 1 < targetAlpha){
				alpha++;
			}
			else if (alpha - 1 > targetAlpha){
				alpha--;
			}
		}

		else{
			float turnTheta = CalcShortestRot(theta, targetTheta);
			//Debug.Log(turnTheta);
			//if (theta + 0.99 <= targetTheta){
			if (Mathf.Abs(theta - targetTheta) > 0.99f){
				if (turnTheta > 0){
					theta++;
				}
				else if (turnTheta < 0){
					theta--;
				}
			}
			float turnAlpha = CalcShortestRot(alpha, targetAlpha);
			//Debug.Log(turnTheta);
			//if (theta + 0.99 <= targetTheta){
			if (Mathf.Abs(alpha - targetAlpha) > 0.99f){
				if (turnAlpha > 0){
					alpha++;
				}
				else if (turnAlpha < 0){
					alpha--;
				}
			}
		}
		rb.isKinematic=false;
	}
	int partNo = Int32.Parse(this.name);
	// should really have everythings collided set to true then only the limb that moved last should move back
	// for some reason when moving back, any attached limbs who have had their theta changed will change theta aswell
	// code doesnt work for alpha
	DHTable[0,0] = theta;
	DHTable[0,1] = d;
	DHTable[0,2] = length;
	DHTable[0,3] = alpha;
	Matrix4x4 mat; 
	if (this.name == "1"){
		mat = trans(DHTable[0,0], DHTable[0,1], DHTable[0,2], alpha);//prevJoint.GetComponent<RoboJoint>().alpha);
	}
	else
	{
		mat = trans(DHTable[0,0], DHTable[0,1], DHTable[0,2], alpha);//prevJoint.GetComponent<RoboJoint3>().alpha);
	}

	if (this.name == "1"){
		q = QuaternionFromMatrix(mat);
		Quaternion q3 = Quaternion.Inverse(prevJoint.GetComponent<RoboJoint>().q2);
		q = (prevJoint.GetComponent<RoboJoint>().q*q3*q).normalized;
	}
	else
	{
		q = QuaternionFromMatrix(mat);
		//q = (prevJoint.GetComponent<RoboJoint3>().q*q).normalized;
		Quaternion q3 = Quaternion.Inverse(prevJoint.GetComponent<RoboJoint3>().q2);
		q = (prevJoint.GetComponent<RoboJoint3>().q*q3*q).normalized;
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
	Vector3 dir = (newPos - transform.position);
	//if (waitCount < 500) transform.position = newPos;
	rb.MovePosition(newPos);
	/*else {//rb.MovePosition(newPos);
	if (dir.magnitude < 0.01f){
		//rb.velocity = dir;
		rb.AddForce(dir, ForceMode.VelocityChange);
	} else {
		//rb.velocity = dir.normalized*dir.magnitude*100;
		rb.AddForce(dir.normalized, ForceMode.VelocityChange);
	}
	}*/
	//rb.MovePosition(transform.position+dir*5.0f*Time.deltaTime);
	//Debug.Log(this.name + "  " + newPos + "  " + transform.position+ " " + dir.magnitude + " " + rb.velocity); 
	Vector3 v = prevJoint.transform.forward;
	finalRot = Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
	if (this.name == "1"){
		rb.MoveRotation((finalRot*modelRot).normalized);
	}
	else{
		if (!rotGripper){
			//rb.MoveRotation(modelRot.normalized);//*Quaternion.Inverse(prevJoint.GetComponent<RoboJoint3>().modelRot));
			Quaternion q5 = Quaternion.Euler(baseJoint.transform.rotation.eulerAngles);
			rb.MoveRotation((modelRot*q5*Quaternion.Inverse(baseJoint.GetComponent<RoboJoint>().q2)).normalized);
		}
		else {
			rb.MoveRotation((finalRot*modelRot).normalized);//*Quaternion.Inverse(prevJoint.GetComponent<RoboJoint3>().modelRot));
		}
		//rb.MoveRotation(finalRot);
	}
	//this.transform.position = GameObject.Find("0").transform.position + transl;	
	//this.transform.rotation =  Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
       	//Debug.Log("1 " + Time.time);
	justCollided = false;
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

	private static GameObject createScaler(RoboJoint3 obj, GameObject newParent, int xRot, int yRot, int zRot){
		GameObject parentObject = new GameObject(); //create an 'empty' object
		parentObject.name = "Scaler " + obj.name;
		parentObject.transform.parent = newParent.transform;
		//parentObject.transform.localScale = obj.transform.localScale;
		//Vector3 currScale = newParent.transform.parent.transform.localScale;
		//parentObject.transform.localScale = new Vector3((float)(1/currScale.x),(float)(1/currScale.y),(float)(1/currScale.z));//obj.transform.localScale;
		//parentObject.transform.localPosition = new Vector3(0,0,0);
		obj.transform.parent = parentObject.transform;
		//obj.transform.localScale = new Vector3(1,1,1);
		parentObject.transform.Rotate(xRot, yRot, zRot);
		return parentObject;

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
		 /*if (me.x != other.x){
			 return false;
		 }

		 if (me.y != other.y){
			 return false;
		 }

		 if (me.y != other.y){
			 return false;
		 }
		 return true;*/
         }

	private void globalCollision(Rigidbody rb){
		//Debug.Log( "this: " + this.name + " ran global");
		//Debug.Log( "THETA " + theta + " " + prevTheta[0]);
		//rb.velocity = new Vector3(0,0,0);
		//return; 
		thetaSlider.value = prevTheta[1];//theta + 10*(theta-prevTheta[0]);
		alphaSlider.value = prevAlpha[1];//prevAlpha[0] + 10*(alpha-prevAlpha[0]);
		rb.MovePosition(prevPos[0]);
		rb.MoveRotation(prevRot[0].normalized);
		alpha = prevAlpha[0];
		theta = prevTheta[0];
		newPos = prevPos[0];
		finalRot = prevRot[0];
		prevAlpha[0] = prevAlpha[1];
		prevTheta[0] = prevTheta[1];
		prevPos[0] = prevPos[1];
		prevRot[0] = prevRot[1];
		//if(this.name == "2")
		//Debug.Log(prevTheta[0]);
		thisStop = true;
		justCollided = true;
	
	}

	private void OnCollisionStay(Collision col){
			//Debug.Log( "this: " + this.name + " is inside : " + col.collider.gameObject.name);
		//rb.velocity = new Vector3(0,0,0);
		//return; 
		if (this.GetComponent<Collider>().transform.root == col.collider.transform.root){
			Debug.Log( "this: " + this.name + " is inside : " + col.collider.gameObject.name);
			Vector3 normal = col.contacts[0].normal;
			if (IKReverse){
				Debug.Log("found");
				IKReverse = false;
				InverseKinematics.collided = true;
			}
			Vector3 colPoint = transform.InverseTransformPoint(col.transform.position);
			if (IKReverse && 0 == 1){
				Debug.Log("found");
				IKReverse = false;
				InverseKinematics.collided = true;
				rb.MovePosition(prevPos[0]);
				rb.MoveRotation(prevRot[0]);
				alpha = prevAlpha[0];
				theta = prevTheta[0];
				newPos = prevPos[0];
				finalRot = prevRot[0];
				if (partNo < Int32.Parse(col.collider.gameObject.transform.parent.parent.name)){ 
					Debug.Log(transform.InverseTransformPoint(col.transform.position));
					if(colPoint.z < 0)
					{
						Debug.Log("WORKED FORWARD " + this.name);
					}
			 
					else if(colPoint.z > 0)
					{
						Debug.Log("WORKED BACKWARD");
					} 
					if(colPoint.x > 0)
					{
						Debug.Log("WORKED RIGHT");
						thetaSlider.value = -180 + (theta - 10 + 180)%360;
							GameObject.Find("Slider").GetComponent<Slider>().value = -180 + (GameObject.Find("Slider").GetComponent<Slider>().value - 10 + 180)%360;
					}
		       
					else if(colPoint.x < 0)
					{
						Debug.Log("WORKED LEFT");
						thetaSlider.value = -180 + (theta + 10 + 180)%360;
						GameObject.Find("Slider").GetComponent<Slider>().value = -180 + (GameObject.Find("Slider").GetComponent<Slider>().value + 10 + 180)%360;
					}
				}	
				else{
					if(colPoint.z < 0)
					{
						Debug.Log("WORKED FORWARD " + this.name);
					}
			 
					else if(colPoint.z > 0)
					{
						Debug.Log("WORKED BACKWARD");
					} 
					if(colPoint.x > 0)
					{
						Debug.Log("WORKED RIGHT");
						thetaSlider.value = -180 + (theta - 10 + 180)%360;
					}
		       
					else if(colPoint.x < 0)
					{
						Debug.Log("WORKED LEFT");
						thetaSlider.value = -180 + (theta + 10 + 180)%360;
					}
				}
			}
			else{
				thetaSlider.value = prevTheta[0];//theta + 10*(prevTheta-theta);
				alphaSlider.value = prevAlpha[0];//alpha + 10*(prevAlpha-alpha);
				rb.MovePosition(prevPos[0]);
				rb.MoveRotation(prevRot[0].normalized);
				alpha = prevAlpha[0];
				theta = prevTheta[0];
				newPos = prevPos[0];
				finalRot = prevRot[0];
				prevAlpha[0] = prevAlpha[1];
				prevTheta[0] = prevTheta[1];
				prevPos[0] = prevPos[1];
				prevRot[0] = prevRot[1];
				GameObject.Find("0").GetComponent<RoboJoint>().stop = true;
				thisStop = false;
				justCollided = true;
			}
		}
	}
	private void OnCollisionEnter(Collision col){
		Debug.Log( "this: " + this.name + " collided with : " + col.collider.gameObject.name);
		//rb.velocity = new Vector3(0,0,0);
		//return; 
		if (this.GetComponent<Collider>().transform.root == col.collider.transform.root){
			//Debug.Log( "this: " + this.name + " collided with : " + col.collider.gameObject.name);
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
			Vector3 normal = col.contacts[0].normal;
			/*if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y) && Mathf.Abs(normal.x) > Mathf.Abs(normal.z) ){
				normal = new Vector3(normal.x, 0, 0);
			}
			else if (Mathf.Abs(normal.y) > Mathf.Abs(normal.z)){
				normal = new Vector3(0, normal.y, 0);
			}
			else{
				normal = new Vector3(0,0,normal.z);
			}
			normal = normal.normalized;*/
			/*if (IKReverse){
				InverseKinematics.collided = true;
				if (partNo < Int32.Parse(col.collider.gameObject.transform.parent.parent.name)){ 
					Debug.Log(partNo);
					Debug.Log(normal);
					if(normal == transform.forward)
					{
						//Debug.Log("WORKED FORWARD " + this.name);
					}
			 
					else if(normal == -(transform.forward))
					{
						//Debug.Log("WORKED BACKWARD");
					} 
					else if(normal == transform.right)
					{
						Debug.Log("WORKED RIGHT");
						thetaSlider.value = -180 + (theta - 90 + 180)%360;
						targetTheta = thetaSlider.value;
					}
		       
					else if(normal == -(transform.right))
					{
						Debug.Log("WORKED LEFT");
						thetaSlider.value = -180 + (theta + 90 + 180)%360;
						targetTheta = thetaSlider.value;
					}
				}	
				rb.MovePosition(prevPos);
				rb.MoveRotation(prevRot);
				alpha = prevAlpha;
				theta = prevTheta;
				newPos = prevPos;
				finalRot = prevRot;
			}*/
			if (IKReverse){
				Debug.Log("found");
				IKReverse = false;
				InverseKinematics.collided = true;
			}
			Vector3 colPoint = transform.InverseTransformPoint(col.transform.position);
			if (IKReverse && 0 == 1){
				Debug.Log("found");
				InverseKinematics.collided = true;
				rb.MovePosition(prevPos[0]);
				rb.MoveRotation(prevRot[0]);
				alpha = prevAlpha[0];
				theta = prevTheta[0];
				newPos = prevPos[0];
				finalRot = prevRot[0];
				if (partNo < Int32.Parse(col.collider.gameObject.transform.parent.parent.name)){ 
				Debug.Log(transform.InverseTransformPoint(col.transform.position));
					if(colPoint.z < 0)
					{
						//Debug.Log("WORKED FORWARD " + this.name);
					}
			 
					else if(colPoint.z > 0)
					{
						//Debug.Log("WORKED BACKWARD");
					} 
					if(colPoint.x > 0)
					{
						Debug.Log("WORKED RIGHT");
						thetaSlider.value = -180 + (theta - 10 + 180)%360;
						targetTheta = thetaSlider.value;
						GameObject.Find("Slider").GetComponent<Slider>().value = -180 + (GameObject.Find("Slider").GetComponent<Slider>().value - 10 + 180)%360;
					}
		       
					else if(colPoint.x < 0)
					{
						Debug.Log("WORKED LEFT");
						thetaSlider.value = -180 + (theta + 10 + 180)%360;
						targetTheta = thetaSlider.value;
						GameObject.Find("Slider").GetComponent<Slider>().value = -180 + (GameObject.Find("Slider").GetComponent<Slider>().value + 10 + 180)%360;
					}
				}	
				else{
					if(colPoint.z < 0)
					{
						//Debug.Log("WORKED FORWARD " + this.name);
					}
			 
					else if(colPoint.z > 0)
					{
						//Debug.Log("WORKED BACKWARD");
					} 
					if(colPoint.x > 0)
					{
						Debug.Log("WORKED RIGHT");
						thetaSlider.value = -180 + (theta - 10 + 180)%360;
						targetTheta = thetaSlider.value;
					}
		       
					else if(colPoint.x < 0)
					{
						Debug.Log("WORKED LEFT");
						thetaSlider.value = -180 + (theta + 10 + 180)%360;
						targetTheta = thetaSlider.value;
					}
				}
			}
			else{
				thetaSlider.value = prevTheta[0];//theta + 10*(prevTheta-theta);
				alphaSlider.value = prevAlpha[0];//alpha + 10*(prevAlpha-alpha);
				rb.MovePosition(prevPos[0]);
				rb.MoveRotation(prevRot[0].normalized);
				alpha = prevAlpha[0];
				theta = prevTheta[0];
				newPos = prevPos[0];
				finalRot = prevRot[0];
				prevAlpha[0] = prevAlpha[1];
				prevTheta[0] = prevTheta[1];
				prevPos[0] = prevPos[1];
				prevRot[0] = prevRot[1];
				GameObject.Find("0").GetComponent<RoboJoint>().stop = true;
				thisStop = false;
				justCollided = true;
			}
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

	public static void IKReverseTrue(){
		IKReverse = true;
	}
	float CalcShortestRot(float from, float to)
	{
		// If from or to is a negative, we have to recalculate them.
		// For an example, if from = -45 then from(-45) + 360 = 315.
		if(from < 0) {
			from += 360;
		}

		if(to < 0) {
			to += 360;
		}

		// Do not rotate if from == to.
		if(from == to ||
			from == 0  && to == 360 ||
			from == 360 && to == 0)
		{
			return 0;
		}

		// Pre-calculate left and right.
		float left = (360 - from) + to;
		float right = from - to;
		// If from < to, re-calculate left and right.
		if(from < to)  {
			if(to > 0) {
			left = to - from;
			right = (360 - to) + from;
			} else {
			left = (360 - to) + from;
			right = to - from;
			}
		}

		// Determine the shortest direction.
		return ((left <= right) ? left : (right * -1));
	}
}

			/*if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y) && Mathf.Abs(normal.x) > Mathf.Abs(normal.z) ){
				normal = new Vector3(normal.x, 0, 0);
			}
			else if (Mathf.Abs(normal.y) > Mathf.Abs(normal.z)){
				normal = new Vector3(0, normal.y, 0);
			}
			else{
				normal = new Vector3(0,0,normal.z);
			}*/
			//normal = normal.normalized;
			//Debug.Log(normal);
			//Debug.Log(normal.magnitude);
			//Debug.Log(transform.InverseTransformPoint(col.transform.position));
			/*if (IKReverse){
				InverseKinematics.collided = true;
				if (partNo < Int32.Parse(col.collider.gameObject.transform.parent.parent.name)){ 
					Debug.Log(partNo);
					Debug.Log(normal);
					Debug.Log(this.transform.GetChild(0).GetChild(0).right);
					Debug.Log(transform.right);
					//Debug.Log(transform.InverseTransformPoint(col.transform.position));
					if(normal == transform.forward)
					{
						Debug.Log("WORKED FORWARD " + this.name);
					}
			 
					else if(normal == -(transform.forward))
					{
						Debug.Log("WORKED BACKWARD");
					} 
					else if(normal == transform.right)
						{
						Debug.Log("WORKED RIGHT");
						thetaSlider.value = -180 + (theta - 90 + 180)%360;
						targetTheta = thetaSlider.value;
					}
	       		
					else if(normal == -(transform.right))
					{
						Debug.Log("WORKED LEFT");
						thetaSlider.value = -180 + (theta + 90 + 180)%360;
						targetTheta = thetaSlider.value;
					}	
				}
				rb.MovePosition(prevPos);
				rb.MoveRotation(prevRot);
				alpha = prevAlpha;
				theta = prevTheta;
				newPos = prevPos;
				finalRot = prevRot;
			}*/
