using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

[CustomEditor (typeof( InGamePythonInterpreter4))]
public class InGamePythonInterpreter4_editor : Editor
{
	
	
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		
		
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
		
		
		
	}

}