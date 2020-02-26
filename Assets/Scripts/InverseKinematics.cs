using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InverseKinematics : MonoBehaviour
{
    public GameObject[] joints;
    private GameObject[] jointRots;
    public GameObject target;

    public float EPS = 0.5f;
    public float step = 1;//0.015f;

    private float[] angles;
    private int count = 0;
    public int countMax = 1000;
    private bool contIK = false;

    // Start is called before the first frame update
    void Start()
    {
	target = GameObject.Find("Cube (1)");
	int i = 0;
	GameObject currJoint = GameObject.Find(i.ToString());
	joints = new GameObject[4];
	while (currJoint != null){
		joints[i] = currJoint;
		i ++;
		currJoint = GameObject.Find(i.ToString());
	}
    }

    // Update is called once per frame
    void Update()
    {
       if (contIK) {
            iterate_IK();
        }
    }

    public void btn_StartIK() {
        start_IK();
    }

    private void start_IK()
    {
        count = 0;
        contIK = true;
	angles = new float[joints.Length];
	angles[0] = joints[0].GetComponent<RoboJoint>().theta;
	for (int i = 1; i < joints.Length; i ++){
		angles[i] = joints[i].GetComponent<RoboJoint3>().theta;
	}
    }
	
    private void iterate_IK()
    {
	if (Mathf.Abs(Vector3.Distance(joints[joints.Length - 1].transform.position, target.transform.position)) > EPS)
	{
		JacobianIK();
 	}
	else
	{
		Debug.Log("Cycle Count: " + count.ToString());
		contIK = false;
	}
	if (count >= countMax)
	{
		Debug.Log("Hit Cycle Count: " + count.ToString());
		contIK = false;
	}
    }

    private void JacobianIK()
    {
	float[] dO = new float[angles.Length];
	float[] angleDiff = new float[angles.Length];
	dO = GetDeltaOrientation();
	for (int i = 0; i < dO.Length; i++) {
		angles[i] = dO[i] * step/(dO.Length - i);
		angleDiff[i] = dO[i] * step/(dO.Length - i);
	}
        
	// update angles
	rotateLinks(angleDiff);

	count++;
    }

    private float[] GetDeltaOrientation()
    {
	float[,] Jt = GetJacobianTranspose();

	Vector3 V = (target.transform.position- joints[joints.Length - 1].transform.position);

	float[,] dO = Tools.M_Multiply(Jt, new float[,] { { V.x }, { V.y }, { V.z  } });
	Debug.Log( Jt);
	Debug.Log(V);//dO[0, 0] + " " + dO[1, 0] + " " + dO[2, 0]);
	return new float[] { dO[0, 0], dO[1, 0], dO[2, 0] };
    }

    private float[,] GetJacobianTranspose() 
    {
	Vector3[] jacRows = new Vector3[joints.Length];

	for (int i = 0; i < jacRows.Length; i++){
		jacRows[i] = Vector3.Cross(joints[i].transform.forward, (joints[joints.Length - 1].transform.position - joints[i].transform.position));

	}

        float[,] matrix = new float[3,jacRows.Length];
	
	matrix = Tools.M_Populate(matrix, jacRows);
	
	return Tools.M_Transpose(matrix);
    }

    private void rotateLinks(float[] angleDiff)
    {
        float newAngle = angleDiff[0];
	GameObject.Find("Slider").GetComponent<Slider>().value = angleDiff[0] + joints[0].GetComponent<RoboJoint>().theta;
	for (int i = 1; i < joints.Length; i++)
	{            
            //Vector3 upDir = joints[i].transform.right;

            //Vector3 crossAxis = Vector3.Cross(upDir, (joints[i + 1].transform.position - joints[i].transform.position).normalized);
            //float currAngle = calculateAngle(Vector3.up, joints[i + 1].transform.position, joints[i].transform.position);
		newAngle = angleDiff[i]*(i+1);

		if (newAngle != 0){
		GameObject.Find("Slider (" + (i*2).ToString() + ")").GetComponent<Slider>().value = angleDiff[0] + joints[0].GetComponent<RoboJoint>().theta;
		}
	}
    }




}
