using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public class Roomcomponent : MonoBehaviour
{	
	
	
	public roomsimple room;
	public List<Texture2D> previewslist = new List<Texture2D>();
	public List<GameObject> modelarray = new List<GameObject>(); 
	public List<Vector3> wallstobuild = new List<Vector3>();
	
	public void Createpreviewfolder()
	{
		string path = Application.dataPath + "/asset_previews";
		 try 
        {
            // Determine whether the directory exists. 
            if (Directory.Exists(path)) 
            {
                Debug.Log("That path exists already.");
                return;
            }

            // Try to create the directory.
            DirectoryInfo di = Directory.CreateDirectory(path);
            Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));

        } 
        catch (Exception e) 
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
        } 
        finally {}
    }

	
	
	public void UpdatePreviews()
	{
	
	previewslist.Clear();
	
	foreach (GameObject model in modelarray){
		Texture2D texturetoload = new Texture2D(100,100);	
		string name = model.name;
		 using (BinaryReader reader = new BinaryReader(File.Open(Application.dataPath +"/asset_previews/" + name, FileMode.Open)))
			{
			texturetoload.LoadImage(reader.ReadBytes((int)reader.BaseStream.Length));
			}	
		
			previewslist.Add(texturetoload);
		}
		
	}
	
	
	// this cant be on awake, we need this to happen at editortime...oncreation?enable maybe?
	public void Awake(){
	//modelarray.AddRange(GameObject.Find("Interpreter").GetComponent<InGamePythonInterpreter4>().modelarray);
	
	
		
	}
	
	
	
	IEnumerator generatepreviews(GameObject testcam){
				
				
				yield return new WaitForEndOfFrame();
			
				
				testcam.AddComponent<Camera>();
				RenderTexture preview = new RenderTexture(100,100,24);
				testcam.camera.targetTexture = preview;	
				testcam.camera.backgroundColor = new Color(1,1,1);
				Texture2D btex = new Texture2D(100,100);
		
		
		foreach (GameObject model in modelarray){
				// make sure the texture does not already exsit, if it does then we only need
				// to add a reference to it inside of the previewlist on this roomcomponent
				if (!File.Exists(Application.dataPath +"/asset_previews/" +model.name)){
			
			
			
				foreach (GameObject model2 in modelarray){
					foreach (Transform child in model2.transform){
					child.gameObject.SetActiveRecursively(false);
					}
			}
			
			
				// move camera, look at model, create a new texture
				
				
				
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
				byte[] texturetosave = btex.EncodeToPNG();
				File.WriteAllBytes(Application.dataPath + "/asset_previews/"+model.name ,texturetosave);
				//previewslist.Add(btex);	
				
				
				
				}	
			
				
			}
		
		
			
			
			DestroyImmediate(btex);
			GameObject.DestroyImmediate(testcam);
			Debug.Log("Destroyed testcam");
			UpdatePreviews();
			
		}	
	
	
	
	
	public void load_models(){
		
			// this cant be on awake, we need this to happen at editortime...oncreation?enable maybe?
		if (modelarray.Count <= 0)
			{
			modelarray.AddRange(GameObject.Find("Interpreter").GetComponent<InGamePythonInterpreter4>().modelarray);
			}
		Createpreviewfolder();
		
		GameObject testcam = new GameObject();
		
		StartCoroutine(generatepreviews(testcam));
		
			}


}
