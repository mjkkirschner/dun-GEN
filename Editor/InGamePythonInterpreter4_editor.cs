using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;	




[CustomEditor (typeof( InGamePythonInterpreter4))]
public class InGamePythonInterpreter4_editor : Editor
{
	static string[] filePaths;
	static string[] filePathsShort;
	public int levelindex = 0;
	public levelobject curlevel;
	
	public void OnEnable()
	{
		
		filePaths = Directory.GetFiles(Application.dataPath,"file*");
		filePathsShort = Directory.GetFiles(Application.dataPath,"file*");

		for (int i = 0;i < filePaths.Length;i++){
			string[] strarray = filePaths[i].Split('/');
			filePathsShort[i] = strarray[strarray.Length-1];
		}
		
		
		
	}
	
	
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();
		

		InGamePythonInterpreter4 interpreter = target as InGamePythonInterpreter4;

		
		
		EditorGUILayout.BeginVertical();
		
		
		EditorGUILayout.LabelField("Texture/Atlas Options");

		Material mat = (target as InGamePythonInterpreter4).atlastMat;
		
		(target as InGamePythonInterpreter4).atlastMat = (Material)EditorGUILayout.ObjectField(mat, typeof(Material),true);
		
		
		if (GUILayout.Button("Destroy Atlas"))
			{
			
			
			(target as InGamePythonInterpreter4).atlasDirty = true;
			
			}
		
		if (GUILayout.Button("Destroy Textures"))
			{
			
			
			(target as InGamePythonInterpreter4).atlasDirty = true;
			string[] filePaths = Directory.GetFiles(Application.dataPath +"/asset_textures/"); 	
	
		foreach(string filename in filePaths){
			
				File.Delete(filename);
			
			}
		
		foreach(GameObject tile in (GameObject.FindGameObjectsWithTag("tiles")))
				{
				
				if(tile.GetComponent<genFlatTexTile>()){
					
					genFlatTexTile gencomponent = tile.GetComponent<genFlatTexTile>();
				
					gencomponent.textures.Clear();
					
					}
				}
				
			
			}
		
		
		
		
		
		interpreter.texResolution = EditorGUILayout.IntSlider("Tile Resolution",interpreter.texResolution,32,512);
		
		
		
		EditorGUILayout.LabelField("Size Options");
		
		
		
		interpreter.iterations = EditorGUILayout.IntField("Iterations",interpreter.iterations); 
		interpreter.xcenter = EditorGUILayout.IntSlider("x_room_sizes_center",interpreter.xcenter,1,20);
		interpreter.ycenter = EditorGUILayout.IntSlider("y_room_sizes_center",interpreter.ycenter,1,20);
		interpreter.xsd = EditorGUILayout.IntSlider("x_room_sizes_deviation",interpreter.xsd,0,10);
		interpreter.ysd = EditorGUILayout.IntSlider("y_room_sizes_deviation",interpreter.ysd,0,10);
		interpreter.flatlevel = EditorGUILayout.Toggle("flat_level",interpreter.flatlevel);
		interpreter.heightmin = EditorGUILayout.IntSlider("height_between_rooms_Min",interpreter.heightmin,0,interpreter.heightmax);
		interpreter.heightmax= EditorGUILayout.IntSlider("height_between_rooms_Max",interpreter.heightmax,interpreter.heightmin,4);
		
		
		EditorGUILayout.LabelField("wall+Door Generation Options");
		interpreter.wallheight = EditorGUILayout.IntSlider("wallheight_units",interpreter.wallheight,0,6);
		interpreter.doormaxalways = EditorGUILayout.Toggle("MaxDoorSize",interpreter.doormaxalways);
		interpreter.doorminalways = EditorGUILayout.Toggle("MinDoorSize",interpreter.doorminalways);
		interpreter.doorrandomize = EditorGUILayout.Toggle("Randomize_Door_Size",interpreter.doorrandomize);
		interpreter.desiredDoorWidth = EditorGUILayout.IntSlider("Desired_Door_size",interpreter.desiredDoorWidth,1,10);
		
		
		
		//first blank space
		//interpreter.modelarray.Add((GameObject)EditorGUILayout.ObjectField(null, typeof(GameObject),true));
		
		for (int index = 0; index< (target as InGamePythonInterpreter4).modelarray.Count;index++){
		GameObject model =  (target as InGamePythonInterpreter4).modelarray[index];
		(target as InGamePythonInterpreter4).modelarray[index] = (GameObject)EditorGUILayout.ObjectField(model, typeof(GameObject),true);
		}
		
		if (GUILayout.Button("Add Tile"))
		{
		if (interpreter.modelarray.Count>0)
			{	
		(target as InGamePythonInterpreter4).modelarray.Add((target as InGamePythonInterpreter4).modelarray[0]);
			//(target as InGamePythonInterpreter4).Start();
			}
		else
			{	
			interpreter.modelarray.Add(null);
			}	
			
		Repaint();		
		}
		
		if (GUILayout.Button("Remove Tile"))
		{
		(target as InGamePythonInterpreter4).modelarray.RemoveAt((target as InGamePythonInterpreter4).modelarray.Count-1);
		//(target as InGamePythonInterpreter4).Start();
		Repaint();		
		}
		
		if (GUILayout.Button("Clear Tiles"))
		{
		(target as InGamePythonInterpreter4).modelarray.Clear();
		//(target as InGamePythonInterpreter4).Start();
		Repaint();		
		}
		
		
		
		
		if (GUI.changed)
		{
		//(target as InGamePythonInterpreter5).Start();
		Repaint();	
		}
		
		
		
		EditorGUILayout.EndVertical();
		
		
		
		
		
		
		if (GUILayout.Button("Combine_All_Rooms"))
		{
				
			foreach (GameObject room in (target as InGamePythonInterpreter4).roomparents)
			{
					foreach(Transform wall in room.transform)
					{
						(target as InGamePythonInterpreter4).combineSubMeshCheck(wall.gameObject);
						wall.gameObject.AddComponent<CombineChildren>();
					}	
					
				
					room.AddComponent<CombineChildren>();
					room.GetComponent<CombineChildren>().CallCombineOnAllChilds();
					
			
			}
		
		}
		
		
		if (GUILayout.Button("Load_Level"))
			{
			
			
			(target as InGamePythonInterpreter4).loadlevel();
			
			}
		
		if (GUILayout.Button("Run/Generate"))
			{
			
			Debug.Log(interpreter.oldResolution);
				// if the resolution has changed since we last ran generate
			if (interpreter.oldResolution != interpreter.texResolution)
			{
			// then set the old resolution to this new resolution, set the dirty flag, and destroy all textures	
			interpreter.oldResolution = interpreter.texResolution;
			interpreter.atlasDirty = true;
			Debug.Log(interpreter.oldResolution);

				
			string[] filePaths_run = Directory.GetFiles(Application.dataPath +"/asset_textures/"); 	
	
			foreach(string filename in filePaths_run)
				{
			
				File.Delete(filename);	
				Debug.Log("deleting" + filename);
				}
			
			foreach(GameObject tile in (GameObject.FindGameObjectsWithTag("tiles")))
				{
				
				if(tile.GetComponent<genFlatTexTile>()){
					
					genFlatTexTile gencomponent = tile.GetComponent<genFlatTexTile>();
				
					gencomponent.textures.Clear();
					
					}
				}
			
			
			
			}
				
			(target as InGamePythonInterpreter4).Run();
			
			AssetDatabase.ImportAsset("Assets/asset_textures/majorTexture.png");
			
			Texture2D majorTextureAtlas = (Texture2D)AssetDatabase.LoadMainAssetAtPath("Assets/asset_textures/majorTexture.png");
			
			//Texture2D majorTextureAtlas = new Texture2D(512,512);
			
			//using (BinaryReader reader = new BinaryReader(File.Open(Application.dataPath +"/asset_textures/majorTexture.png", FileMode.Open)))
			//{
		//majorTextureAtlas.LoadImage(reader.ReadBytes((int)reader.BaseStream.Length));
		//	}	
			
			// change the texture on the atlas mat here.
		
			
		
			
			(target as InGamePythonInterpreter4).atlastMat.SetTexture("_MainTex", majorTextureAtlas);
			
			AssetDatabase.SaveAssets();
			
			
			
		}
		
		if (GUILayout.Button("Save_Level"))
			{
			
			
			(target as InGamePythonInterpreter4).save("file.txt");
			
			}
		
		
		
		}

	}
