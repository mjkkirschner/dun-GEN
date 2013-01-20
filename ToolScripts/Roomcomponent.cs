using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Roomcomponent : MonoBehaviour
{	
	
	
	public roomsimple room;
	public List<Texture2D> previewslist = new List<Texture2D>();
	public List<GameObject> modelarray; 
	public bool textureflag;
	public List<Vector3> wallstobuild = new List<Vector3>();
	
	
	public void Awake(){
	textureflag = false;	
	modelarray = GameObject.Find("Interpreter").GetComponent<InGamePythonInterpreter4>().modelarray;
	
		
		
	}
	
	
	
	IEnumerator generatepreviews(GameObject testcam){
				
				
				yield return new WaitForEndOfFrame();
			
				
				testcam.AddComponent<Camera>();
				RenderTexture preview = new RenderTexture(100,100,24);
				testcam.camera.targetTexture = preview;	
				testcam.camera.backgroundColor = new Color(1,1,1);
		
		
		
		foreach (GameObject model in modelarray){
				
			
				foreach (GameObject model2 in modelarray){
					foreach (Transform child in model2.transform){
					child.gameObject.SetActiveRecursively(false);
					}
			}
			
			
				// move camera, look at model, create a new texture
				
				Texture2D btex = new Texture2D(100,100);
				testcam.transform.localPosition = model.transform.localPosition;
				testcam.transform.localPosition += new Vector3(-1,1,-1);
				testcam.transform.LookAt(model.transform);
					
				// turn model on
	
				model.active = true;
				foreach (Transform child in model.transform){
				child.gameObject.SetActiveRecursively(true);
					}
			
			
				testcam.camera.Render();
				RenderTexture.active = preview;
				btex.ReadPixels(new Rect(0, 0, preview.width, preview.height), 0, 0);
				btex.Apply();		
				
				previewslist.Add(btex);	
				
				
			
			}
			
			textureflag = true;
		
		//if (textureflag == true){
			GameObject.Destroy(testcam);
			Debug.Log("Destroyed testcam");
			
			//}
	
		}	
	
	
	
	
	public void Start(){
		
		
		
		
		GameObject testcam = new GameObject();
		
		StartCoroutine(generatepreviews(testcam));
		
			}


}
