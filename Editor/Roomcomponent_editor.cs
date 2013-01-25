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
		for (int index = 0; index< (target as Roomcomponent).modelarray.Count;index++){
		GameObject model =  (target as Roomcomponent).modelarray[index];
		(target as Roomcomponent).modelarray[index] = (GameObject)EditorGUILayout.ObjectField(model, typeof(GameObject),true);
		}
		
		if (GUILayout.Button("Add Tile"))
		{
		(target as Roomcomponent).modelarray.Add((target as Roomcomponent).modelarray[0]);
		(target as Roomcomponent).Start();
		Repaint();		
		}
		
		if (GUILayout.Button("Remove Tile"))
		{
		(target as Roomcomponent).modelarray.RemoveAt((target as Roomcomponent).modelarray.Count-1);
		(target as Roomcomponent).Start();
		Repaint();		
		}
		
		
		
		if (GUI.changed)
		{
		(target as Roomcomponent).Start();
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
		if (GUILayout.Button("RegenRoom")){
			//regen the room
			// find the main generator object that holds the parse methods and lists of objects ( this is like a static class or manager)
			// we may need to replace this with the maze generator c# object and update it with the parse methods we've written
          InGamePythonInterpreter4 interperter =  GameObject.Find("Interpreter").GetComponent<InGamePythonInterpreter4>();
			
			// for each child of the room parent we destroy the child and remove those destroyed tiles from the tiles list
			// then clear the entire wallmasterlist...this causes a bug that we create new blocks at the interface where we should not
			// what to do is this, each room should hold a second list of walls, either just the walls we really want, or a subtraction list
			foreach (Transform wall in (target as Roomcomponent).transform)
				{
					foreach(Transform tile in wall.transform)
						{
					// need to search deeper now that we have the wall parents here we are not removing the correct tiles from the list.
					Destroy(tile.gameObject);
					interperter.tiles.Remove(tile.gameObject);
						}	
				}
				
			interperter.wallmasterlist = interperter.wallmasterlist.Except((target as  Roomcomponent).wallstobuild).ToList();
			(target as  Roomcomponent).wallstobuild.Clear();
			interperter.parseroom2((target as Roomcomponent).room,interperter.heighttable,(target as Roomcomponent).modelarray,(target as Roomcomponent).gameObject);
			
			
			
			
			}

	
	}
}