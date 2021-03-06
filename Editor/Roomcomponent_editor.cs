using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

[CustomEditor (typeof( Roomcomponent))]
public class Roomcomponent_editor : Editor


{
	
	Vector2 ScrollPosition = new Vector2(0,0);
	
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		
		

		EditorGUILayout.BeginVertical();
		//first blank space
		
		for (int index = 0; index< (target as Roomcomponent).modelarray.Count;index++){
		GameObject model =  (target as Roomcomponent).modelarray[index];
		(target as Roomcomponent).modelarray[index] = (GameObject)EditorGUILayout.ObjectField(model, typeof(GameObject),true);
		}
		
		if (GUILayout.Button("Add Tile"))
		{
		(target as Roomcomponent).modelarray.Add((target as Roomcomponent).modelarray[0]);
		(target as Roomcomponent).load_models();
		Repaint();		
		}
		
		if (GUILayout.Button("Remove Tile"))
		{
		(target as Roomcomponent).modelarray.RemoveAt((target as Roomcomponent).modelarray.Count-1);
		(target as Roomcomponent).load_models();
		Repaint();		
		}
		
		
		
		if (GUI.changed)
		{
		(target as Roomcomponent).load_models();
		Repaint();	
		}
		
		
		
		EditorGUILayout.EndVertical();
		
		

		
		Rect rect = EditorGUILayout.BeginVertical();
		ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
		// foreach texture that exists for the room create a button using each.
		foreach (Texture2D btntex2 in (target as Roomcomponent) .previewslist)
		{  
		
			
		 if (GUILayout.Button(btntex2)){
            Debug.Log("Select These Tiles In This Room...etc");
			
			
			
			
			
		
			
			//logic will need to go here to select new tiles
			}
		}
			
			
			
			
		
		
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		if (GUILayout.Button("RegenRoom"))
			{
			//regen the room
			// find the main generator object that holds the parse methods and lists of objects ( this is like a static class or manager)
			// we may need to replace this with the maze generator c# object and update it with the parse methods we've written
          InGamePythonInterpreter4 interperter =  GameObject.Find("Interpreter").GetComponent<InGamePythonInterpreter4>();
			interperter.loadlevelData();
			// for each child of the room parent we destroy the child and remove those destroyed tiles from the tiles list
			// then clear the entire wallmasterlist...this causes a bug that we create new blocks at the interface where we should not
			// what to do is this, each room should hold a second list of walls, either just the walls we really want, or a subtraction list
			List<Transform> tempchildren = new List<Transform>();
			foreach (Transform wall in (target as Roomcomponent).transform)
				{
				
					
					foreach(Transform tile in wall.transform)
						{
					// need to search deeper now that we have the wall parents here we are not removing the correct tiles from the list.
					//we also need to delete all the combined meshes on the objects...(from the combinations)...THIS
					//IS A BAD WAY TO DO IT, we should instead have a button to call combine on all objects so we can play with the level
					//until this point...
					tempchildren.Add(tile);
					
							}
						}	
				
			
				
				foreach(Transform tile in tempchildren)
					{
					if (tile)
						{	
						interperter.tiles.Remove(tile.gameObject);
						GameObject.DestroyImmediate(tile.gameObject);
						
						
						}	
		
			
				}
				
			 //remove the wallstobuild from the wallmasterlist so that we recreate them on the next parse pass.	
			interperter.wallmasterlist = interperter.wallmasterlist.Except((target as  Roomcomponent).wallstobuild).ToList();
			(target as  Roomcomponent).wallstobuild.Clear();
			
			// need to reset the rotation of roomcomp.gamobject to 0,0,0
			
			(target as Roomcomponent).gameObject.transform.eulerAngles = new Vector3(0,0,0);
				
			// after restarting unity room.walls is being lost.
			interperter.parseroom2((target as Roomcomponent).room,interperter.heighttable,(target as Roomcomponent).modelarray,(target as Roomcomponent).gameObject);
			interperter.genMajorAtlas();
			
			
			
			
		
		}
				
	}	
	
}	