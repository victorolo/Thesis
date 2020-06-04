using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
	private float speed = 5.0f;
        private float zoomSpeed = 2.0f;

        public float minX = -360.0f;
        public float maxX = 360.0f;

        public float minY = -360.0f;
        public float maxY = 360.0f;

        public float sensX = 400.0f;
        public float sensY = 400.0f;

        float rotationY = 0.0f;
        float rotationX = 0.0f;

	public Camera m_camera;
	void Awake(){
		m_camera = Camera.main;
	}
	     void OnPreCull()
	     {
		 m_camera.ResetWorldToCameraMatrix();
		 m_camera.ResetProjectionMatrix();
		 m_camera.projectionMatrix = m_camera.projectionMatrix * Matrix4x4.Scale(new Vector3(-1, 1, 1));
	     }
	 
	     void OnPreRender()
	     {
		 GL.SetRevertBackfacing(true);
	     }
	 
	     void OnPostRender()
	     {
		 GL.SetRevertBackfacing(false);
	     }
        void Update () {
		float scroll = Input.GetAxis("Mouse ScrollWheel");
                transform.Translate(transform.forward*scroll*zoomSpeed, Space.World);//0, scroll * zoomSpeed, scroll * zoomSpeed, Space.World);
		

                if (Input.GetKey(KeyCode.RightArrow)){
                        transform.position -= transform.right * speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.LeftArrow)){
                        transform.position += transform.right * speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.UpArrow)){
                        transform.position += transform.up * speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.DownArrow)){
                        transform.position -= transform.up * speed * Time.deltaTime;
                }

                if (Input.GetMouseButton (1)) {
                        rotationX -= Input.GetAxis ("Mouse X") * sensX * Time.deltaTime;
                        rotationY += Input.GetAxis ("Mouse Y") * sensY * Time.deltaTime;
                        rotationY = Mathf.Clamp (rotationY, minY, maxY);
                        transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
                }
        }
}
