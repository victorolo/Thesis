using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboJoint2 : MonoBehaviour
{
	public float[,] DHTable;
	[Range(-180f, 180f)]
	public float theta;
	float smooth = 100.0f;
	public float length;
	[Range(-180f, 180f)]
	public float alpha;
	private float prevTheta;
	private float prevAlpha;
	public bool collided;
	private int lastMove;
	Slider thetaSlider;
	Slider alphaSlider;
    // Start is called before the first frame update
    void Start()
    {
	DHTable = GameObject.Find("0").GetComponent<RoboJoint>().DHTable;
	length = 1;
	collided = false;
	lastMove = 0;
	int partNo = Int32.Parse(this.name);
	thetaSlider = GameObject.Find("Slider (" + (partNo*2).ToString() + ")").GetComponent<Slider>();
	theta = thetaSlider.value; // both these should be created from the robi file
	alphaSlider = GameObject.Find("Slider (" + (partNo*2 + 1).ToString() + ")").GetComponent<Slider>();
	alpha = alphaSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
	theta = thetaSlider.value; // both these should be created from the robi file
	alpha = alphaSlider.value;
	int partNo = Int32.Parse(this.name);
	DHTable = GameObject.Find("0").GetComponent<RoboJoint>().DHTable;
	if (theta > prevTheta){
		lastMove = -1;
	}
	else if (theta < prevTheta){
		lastMove = 1;
	}
	else if (alpha != prevAlpha){
		lastMove = 2;
	}
	else if (length != length){
		lastMove = 3;
	}
	// should really have everythings collided set to true then only the limb that moved last should move back
	// for some reason when moving back, any attached limbs who have had their theta changed will change theta aswell
	// code doesnt work for alpha
	if (collided){
		Debug.Log(lastMove);
		if (Math.Abs(lastMove) == 1){
			DHTable[partNo,0] = prevTheta + lastMove;
			theta = prevTheta + lastMove;
		}
		else if (lastMove == 2){
			DHTable[partNo,3] = prevAlpha - 1;
			alpha = prevAlpha - 1;
		}
		DHTable[partNo,2] = length;
	}
	else{
		DHTable[partNo,0] = theta;
		DHTable[partNo,2] = length;
		DHTable[partNo,3] = alpha;
	}
	Matrix4x4 mat = trans(DHTable[0,0], DHTable[0,1], DHTable[0,2], DHTable[0,3]);
	for (int i = 1; i <= partNo; i ++){
		mat = mat*trans(DHTable[i,0], DHTable[i,1], DHTable[i,2], DHTable[i,3]);
	}

	/*int rowLength = 4;//arr.GetLength(0);
        int colLength = 4;//arr.GetLength(1);
	string str = "";

        for (int i = 0; i < rowLength; i++)
        {
           for (int j = 0; j < colLength; j++)
            {
                str += (string.Format("{0} ", mat[i, j]));
            }
            str += ("  -  \n");
        }*/
	//str += string.Format("{0} ", GameObject.Find("Cylinder (1)").transform.position.x); 
	//str += string.Format("{0} ", GameObject.Find("Cylinder (1)").transform.position.y); 
	//str += string.Format("{0} ", GameObject.Find("Cylinder (1)").transform.position.z); 
	//str += string.Format("{0} ", mat.m03);
	//str += string.Format("{0} ", mat.m13);
	//str += string.Format("{0} ", mat.m23);
	//str += ("  -  \n");
	//Debug.Log(str);
	Quaternion q = QuaternionFromMatrix(mat);
	//Vector3 transl = this.transform.TransformDirection(new Vector3 (mat.m03, mat.m13, mat.m23));
	Vector3 transl;
	transl.x = mat.m03;
	transl.y = mat.m13;
	transl.z = mat.m23;
	//str = string.Format("{0} ", transl.x) + string.Format("{0} ", transl.y) + string.Format("{0} ", transl.z);
	//Debug.Log(str);
        Vector3 currPos = this.transform.position;	
	Vector3 newPos = GameObject.Find("0").transform.position + transl;	
	Vector3 direction = newPos - currPos;
	Ray ray = new Ray(currPos, direction);
	RaycastHit hit;
	if (!Physics.Raycast(ray,out hit,direction.magnitude) || !hit.collider.isTrigger){
		//if (partNo > 1 && GameObject.Find((partNo - 1).ToString()).GetComponent<RoboJoint2>().collided == false){
		this.transform.position = GameObject.Find("0").transform.position + transl;	
		this.transform.rotation =  Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
		//}
		//else if (partNo == 1){

		//this.transform.position = GameObject.Find("0").transform.position + transl;	
		//this.transform.rotation =  Quaternion.Slerp(this.transform.rotation, q,  Time.deltaTime * smooth);
		//}
	}
	else{
		//Debug.Log("hit");
		//Debug.Log(hit.collider.gameObject.name + " " + this.name);
		theta = 0;
		alpha = prevAlpha;

	}
	prevTheta = theta;
	prevAlpha = alpha;
	//this.transform.position = GameObject.Find("0").transform.position + transl;	
	
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
}

