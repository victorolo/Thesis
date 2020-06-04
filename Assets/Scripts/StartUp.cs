using System.Collections;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.UI;
using System.Globalization;
using UnityEngine.SceneManagement;

public class StartUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
	UnityEngine.Object bPrefab = Resources.Load("Button");
	GameObject newButt = (GameObject)GameObject.Instantiate(bPrefab, new Vector3(-109.8f, -31.2f, 0), Quaternion.identity);
       	newButt.transform.SetParent(GameObject.Find("Canvas").transform, false);
	newButt.GetComponent<Button>().onClick.AddListener(delegate { Load(); });
	newButt.transform.GetComponent<RectTransform>().anchorMin = new Vector2(1,1);
	newButt.transform.GetComponent<RectTransform>().anchorMax = new Vector2(1,1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load()
    {
	FileBrowser.SetDefaultFilter( ".txt" );
	FileBrowser.SetFilters( false, new FileBrowser.Filter( "Text Files", ".txt" ) );
	FileBrowser.ShowLoadDialog( (path) => { LoadBot(path); }, null, false, null, "Load", "Select" );
 
    }

    void LoadBot(string path){
	//SceneManager.LoadScene("empty");

	
	GameObject canvas = GameObject.Find("Canvas");
	canvas.name = "Canvas Del";
	GameObject.DestroyImmediate(canvas);
	UnityEngine.Object cPrefab = Resources.Load("Canvas");
	canvas = (GameObject)GameObject.Instantiate(cPrefab, new Vector3(0, 0, 0), Quaternion.identity);
	canvas.name = "Canvas";
	Debug.Log(path);
	GameObject parentObj = GameObject.Find("GameObject");
	parentObj.name = "GameObject Del";
	int childs = parentObj.transform.childCount;
	/*for (int i = childs - 1; i >= 0; i--)
	{
		GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
	}*/
	GameObject.DestroyImmediate(parentObj);
	parentObj = new GameObject("GameObject");
	parentObj.AddComponent<Serial>();
	parentObj.name = "GameObject";
	InverseKinematics ik = parentObj.AddComponent<InverseKinematics>();
	//DontDestroyOnLoad(parentObj);
	UnityEngine.Object bPrefab = Resources.Load("Button");
	GameObject newButt = (GameObject)GameObject.Instantiate(bPrefab, new Vector3(-109f , -75, 0), Quaternion.identity);
       	newButt.transform.SetParent(GameObject.Find("Canvas").transform, false);
	newButt.GetComponentInChildren<Text>().text = "Inverse Kinematics";
	newButt.name = "IK";
	newButt.transform.GetComponent<RectTransform>().anchorMin = new Vector2(1,1);
	newButt.transform.GetComponent<RectTransform>().anchorMax = new Vector2(1,1);
	//newButt.GetComponent<Button>().onClick.AddListener(delegate { Load(); });
	newButt.GetComponent<Button>().onClick.AddListener(() => ik.btn_StartIK());
	var sr = new StreamReader(path);
     	var fileContents = sr.ReadToEnd();
     	sr.Close();
 
	string [,] inputs = new string[50, 10];
	var lines = fileContents.Split("\n"[0]);
	int count = 0;
    	foreach (string line in lines) {
		var lineElements = line.Split(" "[0]);
		int count2 = 0;
		foreach (string word in lineElements){
       			//Debug.Log(word);
			inputs[count, count2] = word;
			count2 ++;
		}	
       		Debug.Log(count + " " + line);
		count ++;
    	}

	int sliderCount = 0;
	int armsNo = Int32.Parse(inputs[0,0]);
	ik.jointCount = armsNo + 1;
	for (int i = 0; i < armsNo; i ++){
		//Debug.Log(inputs[3*armsNo+1+1,0]);
		//Debug.Log(ToLiteral(inputs[3*armsNo+1+1,0]));
		//Debug.Log("C:/Users/Victor/Documents/untitled.obj");
		//Debug.Log(ToLiteral("C:/Users/Victor/Documents/untitled.obj"));
		GameObject obj = loadAndDisplayMesh(inputs[3*armsNo+1+1+i,0].Replace("\r", ""));
		obj.transform.SetParent(parentObj.transform);
		obj.name = i.ToString() + "," + (i+1).ToString();
		//obj.AddComponent<MeshCollider>();
    		//obj.GetComponent<MeshCollider>().convex = true;
		var colli = obj.AddComponent<NonConvexMeshCollider>();
		colli.avoidExceedingMesh = true;
		colli.boxesPerEdge = 20;
		colli.Calculate();
		Limb lmb = obj.AddComponent<Limb>();
		lmb.otherScale = Int32.Parse(inputs[i+1, 0]);
		Debug.Log(inputs[5*armsNo+9+i, 0]);;
		lmb.overlap = float.Parse(inputs[5*armsNo+9+i, 0], CultureInfo.InvariantCulture);
		lmb.offsetX = float.Parse(inputs[6*armsNo+9+i, 0], CultureInfo.InvariantCulture);
		GameObject obj2 = loadAndDisplayMesh(inputs[4*armsNo+3,0].Replace("\r", ""));
		//obj2.transform.Rotate(new Vector3(Int32.Parse(inputs[armsNo*2+1+i,0]), Int32.Parse(inputs[armsNo*2+1+i,1]), Int32.Parse(inputs[armsNo*2+1+i,2])));
		obj2.AddComponent<MeshCollider>();
    		obj2.GetComponent<MeshCollider>().convex = true;
		obj2.transform.SetParent(parentObj.transform);
		obj2.name = i.ToString();
		if (i == 0){
			RoboJoint rbj2 = obj2.AddComponent<RoboJoint>();
		}
		else{
			RoboJoint3 rbj2 = obj2.AddComponent<RoboJoint3>();
			rbj2.length = Int32.Parse(inputs[armsNo+i,0]);
			rbj2.d = float.Parse(inputs[armsNo+i,1], CultureInfo.InvariantCulture);
			rbj2.xRot = Int32.Parse(inputs[armsNo*2+1+i,0]);
			rbj2.yRot = Int32.Parse(inputs[armsNo*2+1+i,1]);
			rbj2.zRot = Int32.Parse(inputs[armsNo*2+1+i,2]);
			//rbj2.theta = float.Parse(inputs[armsNo+i,1], CultureInfo.InvariantCulture);
			//rbj2.alpha = float.Parse(inputs[armsNo+i,2], CultureInfo.InvariantCulture);

		}
			
		Debug.Log("Joint to render: " + i);
	        Debug.Log(" value:  " + inputs[7*armsNo+11,i]);
		if (inputs[7*armsNo+11,i].Trim() == "0"){
			obj2.GetComponent<MeshRenderer>().enabled = false;
			obj2.GetComponent<Collider>().enabled = false;
		}
		UnityEngine.Object pPrefab = Resources.Load("Slider (1)"); // note: not .prefab!
		GameObject newSlider = (GameObject)GameObject.Instantiate(pPrefab, new Vector3(98, -10 -sliderCount*20, 0), Quaternion.identity);
		//GameObject newSlider = Instantiate(Slider) as GameObject;
		sliderCount ++;
        	newSlider.transform.SetParent(GameObject.Find("Canvas").transform, false);
		newSlider.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
		newSlider.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
		newSlider.name = "Slider (" + (2*i).ToString() + ")";
		//newSlider.GetComponent<Slider>().label = "Slider (" + (i).ToString() + ") Theta";  
		if (inputs[4*armsNo + 4 + i, 0] == "0"){
			newSlider.GetComponent<Slider>().interactable = false;
			newSlider.GetComponent<RectTransform>().localScale = new Vector3(0f,0f,0f);
;
			sliderCount --;
		}
		else {
			GameObject textObj = new GameObject("myTextGO");
			textObj.transform.position = new Vector3(229.4f, -30.4f -sliderCount*20, 0);
        		textObj.transform.SetParent(GameObject.Find("Canvas").transform, false);
			//textObj.transform.SetParent(newSlider.transform);	
			Text myText = textObj.AddComponent<Text>();
			myText.text = "Arm " + (i).ToString() + "- Theta";
			myText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf"); 
			textObj.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
			textObj.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
			if (i < armsNo){
			//if (i >0){
				//newSlider.GetComponent<Slider>().value = float.Parse(inputs[armsNo+i,2], CultureInfo.InvariantCulture);
				newSlider.GetComponent<Slider>().value = float.Parse(inputs[armsNo+i+1,2], CultureInfo.InvariantCulture);
		
			}
		}
		GameObject newSlider2 = (GameObject)GameObject.Instantiate(pPrefab, new Vector3(98, -10 - sliderCount*20, 0), Quaternion.identity);
		//GameObject newSlider = Instantiate(Slider) as GameObject;
		sliderCount ++;
        	newSlider2.transform.SetParent(GameObject.Find("Canvas").transform, false);
		newSlider2.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
		newSlider2.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
		newSlider2.name = "Slider (" + (2*i+1).ToString() + ")";
		//newSlider2.label = "Slider (" + (i).ToString() + ") Alpha";  
		if (inputs[4*armsNo + 4 + i, 1] == "0"){
			sliderCount --;
			newSlider2.GetComponent<Slider>().interactable = false;
			newSlider2.GetComponent<RectTransform>().localScale = new Vector3(0f,0f,0f);
		}
		else {
			GameObject textObj = new GameObject("myTextGO");
			textObj.transform.position = new Vector3(229.4f, -30.4f -sliderCount*20, 0);
        		textObj.transform.SetParent(GameObject.Find("Canvas").transform, false);
			//textObj.transform.SetParent(newSlider2.transform);
			//textObj.transform.localPosition = new Vector3(198.6f,-43f,0f);
			Debug.Log(newSlider2.name);
			Text myText = textObj.AddComponent<Text>();
			myText.text = "Arm " + (i).ToString() + "- Alpha";
			myText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
			textObj.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
			textObj.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
			//if (i > 0){	
				//newSlider2.GetComponent<Slider>().value = float.Parse(inputs[armsNo+i,3], CultureInfo.InvariantCulture);
			if (i < armsNo){	
				newSlider2.GetComponent<Slider>().value = float.Parse(inputs[armsNo+i+1,3], CultureInfo.InvariantCulture);
		
			}
		}
		GameObject newSlider8 = (GameObject)GameObject.Instantiate(pPrefab, new Vector3(98, -10 - sliderCount*20, 0), Quaternion.identity);
		//GameObject newSlider = Instantiate(Slider) as GameObject;
		//sliderCount ++;
        	//newSlider8.transform.SetParent(GameObject.Find("Canvas").transform, false);
		newSlider8.name = "Slider (l" + (2*i).ToString() + ")";
		newSlider8.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
		newSlider8.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
		//newSlider2.label = "Slider (" + (i).ToString() + ") Alpha";  
		if (inputs[4*armsNo + 4 + i, 2] == "0"){
			sliderCount --;
			newSlider8.GetComponent<Slider>().interactable = false;
			newSlider8.GetComponent<RectTransform>().localScale = new Vector3(0f,0f,0f);
			newSlider8.GetComponent<Slider>().maxValue = 2*float.Parse(inputs[armsNo+i,0], CultureInfo.InvariantCulture);
			newSlider8.GetComponent<Slider>().minValue = float.Parse(inputs[armsNo+i,0], CultureInfo.InvariantCulture);
			newSlider8.GetComponent<Slider>().value = float.Parse(inputs[armsNo+i,0], CultureInfo.InvariantCulture);
;
		}
		else {
			//GameObject textObj = new GameObject("myTextGO");
			//textObj.transform.position = new Vector3(229.4f, -30.4f -sliderCount*20, 0);
        		//textObj.transform.SetParent(GameObject.Find("Canvas").transform, false);
			//textObj.transform.SetParent(newSlider8.transform);
			//textObj.transform.localPosition = new Vector3(198.6f,-43f,0f);
			//Text myText = textObj.AddComponent<Text>();
			//myText.text = "Slider (" + (i).ToString() + ") length";
			//myText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
			//myText.horizontalOverflow = HorizontalWrapMode.Overflow;
			//textObj.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
			//textObj.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
			//newSlider8.GetComponent<RectTransform>().SetSizeWithCurrentAnchors( RectTranform.Axis.Horizontal, 150);
			}
		}
	
		GameObject obj3 = loadAndDisplayMesh(inputs[5*armsNo+6,0].Replace("\r", ""));
		obj3.AddComponent<MeshCollider>();
    		obj3.GetComponent<MeshCollider>().convex = true;
		obj3.name = (armsNo).ToString();
		//obj3.AddComponent<MeshCollider>();
		RoboJoint3 rbj3 = obj3.AddComponent<RoboJoint3>();
		rbj3.length = Int32.Parse(inputs[armsNo*2,0]);
		Debug.Log(inputs[armsNo*2, 1]); 
		rbj3.d = float.Parse(inputs[armsNo*2 ,1], CultureInfo.InvariantCulture);
		rbj3.otherScale = Int32.Parse(inputs[armsNo, 2]);
		obj3.transform.SetParent(parentObj.transform);
		//Debug.Log(" HERHE " + (inputs[7*armsNo+12,0].Contains("0")));
		//Debug.Log(" HERHE " + (String.Compare(inputs[7*armsNo+12,0],"1")));
		if (inputs[7*armsNo+12,0].Contains("1")){
			rbj3.rotGripper = false;	
		}
		UnityEngine.Object pPrefab2 = Resources.Load("Slider (1)"); // note: not .prefab!
		GameObject newSlider3 = (GameObject)GameObject.Instantiate(pPrefab2, new Vector3(98, -10 -sliderCount*20, 0), Quaternion.identity);
		sliderCount ++;
		//GameObject newSlider = Instantiate(Slider) as GameObject;
        	newSlider3.transform.SetParent(GameObject.Find("Canvas").transform, false);
		newSlider3.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
		newSlider3.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
		newSlider3.name = "Slider (" + (2*armsNo).ToString() + ")";
		//newSlider3.label = "Slider (" + (armsNo).ToString() + ") Theta";  
		if (inputs[4*armsNo + 4 + armsNo, 0] == "0"){
			sliderCount --;
			newSlider3.GetComponent<Slider>().interactable = false;
			newSlider3.GetComponent<RectTransform>().localScale = new Vector3(0f,0f,0f);
		}
		else {
			GameObject textObj = new GameObject("myTextGO");
			textObj.transform.position = new Vector3(229.4f, -30.4f -sliderCount*20, 0);
        		textObj.transform.SetParent(GameObject.Find("Canvas").transform, false);
			//textObj.transform.SetParent(newSlider3.transform);
			//textObj.transform.localPosition = new Vector3(198.6f,-43f,0f);
			Text myText = textObj.AddComponent<Text>();
			myText.text = "Hand " + (armsNo).ToString() + "- Theta";
			myText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf"); 
			textObj.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
			textObj.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
			newSlider3.GetComponent<Slider>().value = float.Parse(inputs[armsNo*2,2], CultureInfo.InvariantCulture);
		}
		GameObject newSlider4 = (GameObject)GameObject.Instantiate(pPrefab2, new Vector3(98, -10 - sliderCount*20, 0), Quaternion.identity);
		sliderCount ++;
		//GameObject newSlider = Instantiate(Slider) as GameObject;
        	newSlider4.transform.SetParent(GameObject.Find("Canvas").transform, false);
		newSlider4.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
		newSlider4.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
		newSlider4.name = "Slider (" + (2*armsNo +1).ToString() + ")";
		//newSlider4.label = "Slider (" + (armsNo).ToString() + ") Alpha"; 
		if (inputs[4*armsNo + 4 + armsNo, 1] == "0"){
			sliderCount --;
			newSlider4.GetComponent<Slider>().interactable = false;
			newSlider4.GetComponent<RectTransform>().localScale = new Vector3(0f,0f,0f);
		}
		else {
			GameObject textObj = new GameObject("myTextGO");
			textObj.transform.position = new Vector3(229.4f, -30.4f -sliderCount*20, 0);
        		textObj.transform.SetParent(GameObject.Find("Canvas").transform, false);
			//textObj.transform.SetParent(newSlider4.transform);
			//textObj.transform.localPosition = new Vector3(198.6f,-43f,0f);
			Text myText = textObj.AddComponent<Text>();
			myText.text = "Hand " + (armsNo).ToString() + "- Alpha";
			myText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf"); 
			textObj.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
			textObj.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
			newSlider4.GetComponent<Slider>().value = float.Parse(inputs[armsNo*2,2], CultureInfo.InvariantCulture);
		}


		//Debug.Log(inputs[5*armsNo+6,0].Replace("\r", ""));
		/*GameObject obj4 = loadAndDisplayMesh(inputs[4*armsNo+7,0].Replace("\r", ""));
		obj4.AddComponent<MeshCollider>();
    		obj4.GetComponent<MeshCollider>().convex = true;
		obj4.name = "grabber";
		//obj3.AddComponent<MeshCollider>();
		grabber g4 = obj4.AddComponent<grabber>();
		//rbj3.length = Int32.Parse(inputs[armsNo*2,0]);
		//rbj3.d = Int32.Parse(inputs[armsNo*2,1]);
		obj4.transform.SetParent(parentObj.transform);
		g4.parent = obj3;*/

		GameObject newSlider5 = (GameObject)GameObject.Instantiate(pPrefab2, new Vector3(98, -10 - sliderCount*20, 0), Quaternion.identity);
		sliderCount ++;
		//GameObject newSlider = Instantiate(Slider) as GameObject;
        	newSlider5.transform.SetParent(GameObject.Find("Canvas").transform, false);
		newSlider5.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
		newSlider5.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
		newSlider5.name = "Slider (" + (2*armsNo +2).ToString() + ")";
		newSlider5.GetComponent<Slider>().maxValue = 100f;
		newSlider5.GetComponent<Slider>().minValue = 30f;
		newSlider5.GetComponent<Slider>().value = 100f;
		//newSlider4.label = "Slider (" + (armsNo).ToString() + ") Alpha"; 
		GameObject textObj2 = new GameObject("myTextGO");
		textObj2.transform.position = new Vector3(229.4f, -30.4f -sliderCount*20, 0);
        	textObj2.transform.SetParent(GameObject.Find("Canvas").transform, false);
		//textObj2.transform.SetParent(newSlider5.transform);
		//textObj2.transform.localPosition = new Vector3(198.6f,-43f,0f);
		Text myText2 = textObj2.AddComponent<Text>();
		myText2.text = "Slider grab";
		myText2.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf"); 
		textObj2.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
		textObj2.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
		
		GameObject newSlider9 = (GameObject)GameObject.Instantiate(pPrefab2, new Vector3(98, -10 - sliderCount*20, 0), Quaternion.identity);
		//GameObject newSlider = Instantiate(Slider) as GameObject;
		//sliderCount ++;
        	//newSlider9.transform.SetParent(GameObject.Find("Canvas").transform, false);
		newSlider9.name = "Slider (l" + (2*armsNo).ToString() + ")";
		newSlider9.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
		newSlider9.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
		//newSlider2.label = "Slider (" + (i).ToString() + ") Alpha";  
		Debug.Log(" ASDasdas" + inputs[5*armsNo + 4, 2]);
		if (inputs[5*armsNo + 4, 2] == "0"){
			sliderCount --;
			newSlider9.GetComponent<Slider>().interactable = false;
			newSlider9.GetComponent<RectTransform>().localScale = new Vector3(0f,0f,0f);
;
		}
		else {
			//GameObject textObj = new GameObject("myTextGO");
			//textObj.transform.position = new Vector3(229.4f, -30.4f -sliderCount*20, 0);
        		//textObj.transform.SetParent(GameObject.Find("Canvas").transform, false);
			//textObj.transform.SetParent(newSlider9.transform);
			//textObj.transform.localPosition = new Vector3(198.6f,-43f,0f);
			//Text myText = textObj.AddComponent<Text>();
			//myText.text = "Slider (" + (armsNo).ToString() + ") length";
			//myText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
			//myText.horizontalOverflow = HorizontalWrapMode.Overflow;
			//textObj.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
			//textObj.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
			newSlider9.GetComponent<Slider>().maxValue = 2*float.Parse(inputs[armsNo*2,0], CultureInfo.InvariantCulture);
			newSlider9.GetComponent<Slider>().minValue = float.Parse(inputs[armsNo*2,0], CultureInfo.InvariantCulture);
			newSlider9.GetComponent<Slider>().value = float.Parse(inputs[armsNo*2,0], CultureInfo.InvariantCulture);
			//newSlider8.GetComponent<RectTransform>().SetSizeWithCurrentAnchors( RectTranform.Axis.Horizontal, 150);
			}
		


		GameObject obj5 = loadAndDisplayMesh(inputs[5*armsNo+7,0].Replace("\r", ""));
		obj5.AddComponent<MeshCollider>();
    		obj5.GetComponent<MeshCollider>().convex = true;
		obj5.name = "hand 1";
		//obj3.AddComponent<MeshCollider>();
		grabberHand gh5 = obj5.AddComponent<grabberHand>();
		//rbj3.length = Int32.Parse(inputs[armsNo*2,0]);
		//rbj3.d = Int32.Parse(inputs[armsNo*2,1]);
		obj5.transform.SetParent(parentObj.transform);
		gh5.parent = obj3;
		gh5.jointParent = obj3;
		//gh5.xscale = 2f;

		gh5.sliderNo = newSlider5.name;

		GameObject obj6 = loadAndDisplayMesh(inputs[5*armsNo+8,0].Replace("\r", ""));
		obj6.AddComponent<MeshCollider>();
    		obj6.GetComponent<MeshCollider>().convex = true;
		obj6.name = "hand 2";
		//obj3.AddComponent<MeshCollider>();
		grabberHand gh6 = obj6.AddComponent<grabberHand>();
		//rbj3.length = Int32.Parse(inputs[armsNo*2,0]);
		//rbj3.d = Int32.Parse(inputs[armsNo*2,1]);
		obj6.transform.SetParent(parentObj.transform);
		gh6.parent = obj3;
		gh6.jointParent = obj3;
		gh6.xscale = 2f;
		gh6.sliderNo = gh5.sliderNo;


		GameObject obj7 = loadAndDisplayMesh(inputs[4*armsNo+2,0].Replace("\r", ""));
		//obj7.AddComponent<MeshCollider>();
    		//obj7.GetComponent<MeshCollider>().convex = true;
		obj7.name = "base";
		//obj3.AddComponent<MeshCollider>();
		var colli7 = obj7.AddComponent<NonConvexMeshCollider>();
		colli7.avoidExceedingMesh = true;
		colli7.boxesPerEdge = 30;
		//colli7.Calculate();
		Base b7 = obj7.AddComponent<Base>();
		b7.offset.x = float.Parse(inputs[7*armsNo+9, 0], CultureInfo.InvariantCulture);
		b7.offset.y = float.Parse(inputs[7*armsNo+9, 1], CultureInfo.InvariantCulture);
		b7.offset.z = float.Parse(inputs[7*armsNo+9, 2], CultureInfo.InvariantCulture);
		b7.scale = float.Parse(inputs[7*armsNo+10, 0], CultureInfo.InvariantCulture);

		int bitsNo = Int32.Parse(inputs[7*armsNo+13,0]);
		GameObject obj8;
		for (int k = 0; k < bitsNo; k ++){
			Debug.Log(inputs[7*armsNo+14 + k,0].Replace("\r", ""));
			obj8 = loadAndDisplayMesh(inputs[7*armsNo+14 + k,0].Replace("\r", ""));
			//obj8.AddComponent<MeshCollider>();
			//obj8.GetComponent<MeshCollider>().convex = true;
			obj8.name = "bit" + k.ToString();
			Bits bit = obj8.AddComponent<Bits>();
			bit.jointNum = (inputs[7*armsNo+14 + k,1].Replace("\r", ""));
			bit.jointRotNum = Int32.Parse(inputs[7*armsNo+14 + k,2].Replace("\r", ""));
			bit.scale = float.Parse(inputs[7*armsNo+14 + k,3].Replace("\r", ""));
			bit.offset.x = float.Parse(inputs[7*armsNo+14 + k,4].Replace("\r", ""));
			bit.offset.y = float.Parse(inputs[7*armsNo+14 + k,5].Replace("\r", ""));
			bit.offset.z = float.Parse(inputs[7*armsNo+14 + k,6].Replace("\r", ""));
			bit.useLocal = Int32.Parse(inputs[7*armsNo+14 + k,7].Replace("\r", ""));
		}
		
    }

    void attachMeshFilter(GameObject target, Mesh mesh)
{
    MeshFilter mF = target.AddComponent<MeshFilter>();
    mF.mesh = mesh;
}

Material createMaterial()
{
    Material mat = new Material(Shader.Find("Standard"));
    return mat;
}

void attachMeshRenderer(GameObject target, Material mat)
{
    MeshRenderer mR = target.AddComponent<MeshRenderer>();
    mR.material = mat;
}

GameObject loadAndDisplayMesh(string path)
{
    //Create new GameObject to hold it
    GameObject meshHolder = new GameObject("Loaded Mesh");

    //Load Mesh
    //Debug.Log(Application.dataPath);
    //string newPath = Application.dataPath + "\\Robots\\" + path;
    //string newPath = "C:\\Users\\Victor\\Saved Games" + "\\Robots\\" + path;
    //string newPath = "C:\\Users\\Victor\\Documents\\" +  path;
    //newPath = newPath.Replace("\\", "/");
    Mesh mesh = FastObjImporter.Instance.ImportFile(path);

    //return null;

    //Attach Mesh Filter
    attachMeshFilter(meshHolder, mesh);

    //Create Material
    Material mat = createMaterial();

    //Attach Mesh Renderer
    attachMeshRenderer(meshHolder, mat);


    return meshHolder;
}
private static string ToLiteral(string input)
{
    using (var writer = new StringWriter())
    {
        using (var provider = CodeDomProvider.CreateProvider("CSharp"))
        {
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
            return writer.ToString();
        }
    }
}

}
