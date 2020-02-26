using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGUI : MonoBehaviour
{
    public float thet0, thet1, thet2, thet3;

    // Start is called before the first frame update
    void OnGUI()
    {
	thet0 = GUI.HorizontalSlider(new Rect(25, 25, 100, 10), thet0, -180.0F, 180.0F);
	thet1 = GUI.HorizontalSlider(new Rect(25, 45, 100, 10), thet1, -180.0F, 180.0F);
	thet2 = GUI.HorizontalSlider(new Rect(25, 65, 100, 10), thet2, -180.0F, 180.0F);
	thet3 = GUI.HorizontalSlider(new Rect(25, 85, 100, 10), thet3, -180.0F, 180.0F);
    }
}
