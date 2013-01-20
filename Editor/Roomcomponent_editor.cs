using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

[CustomEditor (typeof( Roomcomponent))]
public class Roomcomponent_editor : Editor
{
	
	
	
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		// foreach texture that exists for the room create a button using each.
		foreach (Texture2D btntex2 in (target as Roomcomponent) .previewslist){  
		 if (GUILayout.Button(btntex2)){
            Debug.Log("Clicked the button with an image");
			// logic will need to go here to select new tiles
			}

		}
	
		 if (GUILayout.Button("RegenRoom")){
			//regen the room
			// find the main generator object that holds the parse methods and lists of objects ( this is like a static class or manager)
			// we may need to replace this with the maze generator c# object and update it with the parse methods we've written
          InGamePythonInterpreter4 interperter =  GameObject.Find("Interpreter").GetComponent<InGamePythonInterpreter4>();
			
			// for each child of the room parent we destroy the child and remove those destroyed tiles from the tiles list
			// then clear the entire wallmasterlist...this causes a bug that we create new blocks at the interface where we should not
			// what to do is this, each room should hold a second list of walls, either just the walls we really want, or a subtraction list
			foreach (Transform tile in (target as Roomcomponent).transform)
				{
				Destroy(tile.gameObject);
				interperter.tiles.Remove(tile.gameObject);
				interperter.wallmasterlist = interperter.wallmasterlist.Except((target as  Roomcomponent).wallstobuild).ToList();
				}
			
			
			interperter.parseroom2((target as Roomcomponent).room,interperter.heighttable,(target as Roomcomponent).modelarray,(target as Roomcomponent).gameObject);
			
			
			
			
			}

	
	}
}