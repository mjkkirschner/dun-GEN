using UnityEngine;
using System.Collections.Generic;
using System;
using IronPython;	
using System.Runtime.Serialization.Formatters.Binary;	
using System.IO;	
	




/// <summary>
/// A simple python behaviour that demonstrates how code can be ran in the runtime engine
/// </summary>
public class InGamePythonInterpreter4 : MonoBehaviour 
{	
	public levelobject curlevel;
	public int iterations;
	public int xcenter;
	public int ycenter;
	public int xsd;
	public int ysd;
	public int wallheight;
	
	
	public List<GameObject> tiles = new List<GameObject>();
	
	
	
	
	public GameObject model1;
	public GameObject model2;
	public GameObject model3;
	public GameObject model4;
	
	public List<GameObject> modelarray = new List<GameObject>();
	
	
	private List<roomsimple> crooms = new List<roomsimple>();
	public List<Vector3> wallmasterlist = new List<Vector3>();
	private List<object> rooms = new List<object>();
	
	private int[,] table;
	public int[,] heighttable;
	private TextAsset testcode;
    private string m_pyCode;
    private string m_pyOutput;
    private PythonEnvironment m_pyEnv;
	 
    private const string INITIALIZATION_CODE =
@"
import clr
clr.AddReference('UnityEngine')
import UnityEngine
";
	
	
	public void parseLevel(int[,]table,int [,]heighttable)
	{
	for (int i = 0;i < 200; i++){
					for (int j = 0;j < 200; j++){
				
						// build the walls
						if (table[i,j] == 3){
							int height = heighttable[i,j];
							for (int k = 0; k < height; k++){
								Vector3 rot = new Vector3(UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f));
								rot.x = Mathf.Round(rot.x / 90) * 90;
								rot.y = Mathf.Round(rot.y / 90) * 90;
								rot.z = Mathf.Round(rot.z / 90) * 90;
								
							GameObject currentgo = Instantiate(modelarray[UnityEngine.Random.Range(0,modelarray.Count)],new Vector3(i,k,j),Quaternion.identity) as GameObject;
							currentgo.transform.Rotate(rot.x,rot.y,rot.z);
							tiles.Add(currentgo);
								
							}
						}
						// build the floor tiles.
						if (table[i,j] == 2){
							int height = heighttable[i,j];
							for (int k = height-2; k < height-1; k++){
							GameObject currentgo = Instantiate(model4,new Vector3(i,k,j),Quaternion.identity) as GameObject;
								tiles.Add(currentgo);	
	
					}
	
				}
			}
	
			
		}	
		
		
	}	
	public void parseroom(List<object> roomslist, int[,] heighttable ){
		
		
	foreach (System.Object room in roomslist){
			
			GameObject roomcenter = new GameObject();
			
			IList<object>walls = m_pyEnv.m_pythonEngine.Operations.GetMember<IList<object>>(room,"walls");
				foreach (System.Object vector2 in walls){
					float posx = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"x");
					float posy = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"y");
					Vector2 position = new Vector2(posx,posy);
					Debug.Log(position);
					if ((posx >= 0.0) && (posy >= 0.0)){ 
						
					int height = heighttable[(int)posx,(int)posy];
							for (int k = 0; k < height; k++){
								Vector3 rot = new Vector3(UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f));
								rot.x = Mathf.Round(rot.x / 90) * 90;
								rot.y = Mathf.Round(rot.y / 90) * 90;
								rot.z = Mathf.Round(rot.z / 90) * 90;
							
							if (wallmasterlist.Contains(new Vector3(posx,k,posy))){
							
							break;
								}
							wallmasterlist.Add(new Vector3(posx,k,posy));
							GameObject currentgo = Instantiate(modelarray[UnityEngine.Random.Range(0,modelarray.Count)],new Vector3(posx,k,posy),Quaternion.identity) as GameObject;
							currentgo.transform.Rotate(rot.x,rot.y,rot.z);
							tiles.Add(currentgo);
							currentgo.transform.parent = roomcenter.transform;
						
						
						}
					}
				}
			
		IList<object>floors = m_pyEnv.m_pythonEngine.Operations.GetMember<IList<object>>(room,"floortiles");
			foreach (System.Object vector2 in floors){
					float posx = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"x");
					float posy = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"y");
					Vector2 position = new Vector2(posx,posy);
					if ((posx >= 0.0) && (posy >= 0.0)){
					
						int height = heighttable[(int)posx,(int)posy];
							for (int k = height-2; k < height-1; k++){
							GameObject currentgo = Instantiate(model4,new Vector3(posx,k,posy),Quaternion.identity) as GameObject;
								tiles.Add(currentgo);	
						currentgo.transform.parent = roomcenter.transform;
					
					}
			
				}	
		
		
		
			}
	
		}
}
	
	public bool edgecheck(float posx,float posy,roomsimple room){
		if ((posx == room.xpos) || (posy== room.ypos)){
		//Debug.Log ("edge");
		return true; 	
		
		}
		if ((posx == room.width+room.xpos) || (posy == room.height+room.ypos)){
			//Debug.Log ("edge");
			return true;
		}
		return false;
		
	}
		
	public void parseroom2(roomsimple room, int[,] heighttable, List<GameObject> modelarray, GameObject parent ){
	
		
		
			
			GameObject roomcenter = parent;
			tiles.Add(roomcenter);
			foreach (Vector2Serializer wall in room.walls){
					
					float posx = wall.x;
					float posy = wall.y;
						
					if ((posx >= 0.0) && (posy >= 0.0)){ 
						
					int height = heighttable[(int)posx,(int)posy];
							for (int k = 0; k < height+(wallheight); k++){
								Vector3 rot = new Vector3(UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f));
								rot.x = Mathf.Round(rot.x / 90) * 90;
								rot.y = Mathf.Round(rot.y / 90) * 90;
								rot.z = Mathf.Round(rot.z / 90) * 90;
							
							if (wallmasterlist.Contains(new Vector3(posx,k,posy))){
							
							break;
								}
							wallmasterlist.Add(new Vector3(posx,k,posy));
							GameObject currentgo = Instantiate(modelarray[UnityEngine.Random.Range(0,modelarray.Count)],new Vector3(posx,k,posy),Quaternion.identity) as GameObject;
							currentgo.transform.Rotate(rot.x,rot.y,rot.z);
							tiles.Add(currentgo);
							currentgo.transform.parent = roomcenter.transform;
							
							//grab the roomcomponent of the current room/parent object, and store the actual walls that we build here.
							// these are the blocks we WILL clear from the wallmasterlist when we regen the room
							roomcenter.GetComponent<Roomcomponent>().wallstobuild.Add(currentgo.transform.localPosition);
						
						}
					}
				}
			
		
			foreach (Vector2Serializer floor in room.floortiles){
					float posx = floor.x;
					float posy = floor.y;
					
					if ((posx >= 0.0) && (posy >= 0.0)){
						if (!edgecheck(posx,posy,room)){
							int height = heighttable[(int)posx,(int)posy];
							int k = height-2;
							GameObject currentgo = Instantiate(model4,new Vector3(posx,k,posy),Quaternion.identity) as GameObject;
							tiles.Add(currentgo);	
							currentgo.transform.parent = roomcenter.transform;
						}
						
						else{	
					
						
					
						int height = heighttable[(int)posx,(int)posy];
							for (int k = height-3; k < height-1; k++){
								GameObject currentgo = Instantiate(model4,new Vector3(posx,k,posy),Quaternion.identity) as GameObject;
								tiles.Add(currentgo);	
								currentgo.transform.parent = roomcenter.transform;
						
					}
			
				}	
		
		
		
			}
	
		}
	}
	
	
	
	
	public void parseroom2(roomsimple room, int[,] heighttable, List<GameObject> modelarray ){
	
		
	
		GameObject newroomcenter = new GameObject();
		newroomcenter.AddComponent<Roomcomponent>().room = room;
		parseroom2(room,heighttable,modelarray,newroomcenter);	
	
			
		
	}	
		
	
	
	
	
	
	
	
	// Use this for initialization
	void Start () 
    {
		
		
		modelarray.Add(model1);
		modelarray.Add(model2);
		modelarray.Add(model3);
		modelarray.Add(model4);
		
		testcode  = Resources.Load("mazegen1_roomslist") as TextAsset;
        m_pyCode = testcode.text;
		m_pyEnv = new PythonEnvironment();
        m_pyEnv.RunCommand(INITIALIZATION_CODE);
        m_pyOutput = string.Empty;
		 
			//m_pyEnv.RunCommand(m_pyCode);	
		
			//Action generate = m_pyEnv.m_scriptScope.GetVariable<Action>("genmaze");
		
            //get a delegate to the python function
            //Func<int, bool> IsOdd = m_pyEnv.m_scriptScope.GetVariable<Func<int, bool>>("isodd");
			
			//Func<int, bool> IsZero = m_pyEnv.m_scriptScope.GetVariable<Func<int, bool>>("equalzero");
			
	
	}


    void OnGUI()
    {
		
		
        m_pyCode = GUI.TextArea(new Rect(50, 50, 600, 200), m_pyCode);
        if (GUI.Button(new Rect(50, 270, 80, 40), "Run"))
        {	
			
			
			rooms.Clear();
			crooms.Clear();
			
			
            m_pyOutput = string.Empty;
           	m_pyEnv.ExposeVariable("iterations",iterations);
			m_pyEnv.ExposeVariable("xcenter",xcenter);
			m_pyEnv.ExposeVariable("ycenter",ycenter);
			m_pyEnv.ExposeVariable("xsd",xsd);
			m_pyEnv.ExposeVariable("ysd",ysd);
			
			PythonEnvironment.CommandResult result = m_pyEnv.RunCommand(m_pyCode);
			
			
			table =(int[,])  m_pyEnv.m_scriptScope.GetVariable("convertarr");
			heighttable = (int[,]) m_pyEnv.m_scriptScope.GetVariable("convertarrheights");
			
			// here we are going to take all the room objects from python and turn them into c# objects that can be serialized
			IList<object> orgrooms = (IList<object>) m_pyEnv.m_scriptScope.GetVariable("rooms");
			
			foreach (object element in orgrooms){
				rooms.Add((object)element);	
			}
			// the next few sections iterate all rooms from python and create roomsimple objets from them (so they can be serialzied)
			foreach (System.Object room in rooms){	
				
				
				int xpos = m_pyEnv.m_pythonEngine.Operations.GetMember<int>(room,"xpos");
				int ypos = m_pyEnv.m_pythonEngine.Operations.GetMember<int>(room,"ypos");
				
				int width = m_pyEnv.m_pythonEngine.Operations.GetMember<int>(room,"width");
				int height = m_pyEnv.m_pythonEngine.Operations.GetMember<int>(room,"height");
				System.Object center = m_pyEnv.m_pythonEngine.Operations.GetMember<System.Object>(room,"center"); 			
					float centerposx = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(center,"x");
					float centerposy = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(center,"y");
				Vector2 centerpos = new Vector2	(centerposx,centerposy);
					
					
					
				IList<object>walls = m_pyEnv.m_pythonEngine.Operations.GetMember<IList<object>>(room,"walls");
	
				List<Vector2> currentwalllist = new List<Vector2>();
				List<Vector2> currentfloortilelist = new List<Vector2>();	
					foreach (System.Object vector2 in walls){
					float posx = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"x");
					float posy = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"y");
					Vector2 position = new Vector2(posx,posy);	
					currentwalllist.Add(position);
				
				
			}
						
			IList<object>floors = m_pyEnv.m_pythonEngine.Operations.GetMember<IList<object>>(room,"floortiles");
			foreach (System.Object vector2 in floors){
					float posx = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"x");
					float posy = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"y");
					Vector2 position = new Vector2(posx,posy);
					currentfloortilelist.Add(position);
				
								
			}	
			// finally create the serializble room object from all the data we got from python and add it to the room list (crooms)
			roomsimple currentroom = new roomsimple(width,height,centerpos,currentwalllist,currentfloortilelist,xpos,ypos);
				
				crooms.Add(currentroom);
				
		}	
				
				
				
            if (!string.IsNullOrEmpty(result.output))
            {
                m_pyOutput =  result.output;
				foreach (GameObject tile in tiles)
				{
				Destroy(tile);
				}
				tiles.Clear();
				wallmasterlist.Clear();
				Debug.Log(m_pyOutput);
				
				foreach (roomsimple room in crooms){
					parseroom2(room,heighttable,modelarray);
				}
					
			 curlevel = new levelobject(table,heighttable,crooms);	
						
			}
							
            if (result.exception != null)
            {
                m_pyOutput += "Python exception : " + result.exception.Message;
				Debug.Log(m_pyOutput);
				
            }
		}
		
		
		 if (GUI.Button(new Rect(200, 270, 80, 40), "Save"))
        {
            string fileName = "file.txt";
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = new FileStream(fileName,FileMode.Create,FileAccess.Write);
			bf.Serialize(fs,curlevel);
			fs.Close();
			
			// we'll want to create a level object that contains multiple arrays, serialzing those, then cast back to that object
			// and reparse from it.
			
			
		}
		
		 if (GUI.Button(new Rect(300, 270, 80, 40), "Load"))
        {
			
			
            string fileName = "file.txt";
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = new FileStream(fileName,FileMode.Open,FileAccess.Read);
			levelobject loadedlevel = (levelobject)bf.Deserialize(fs);
			fs.Close();
			
			table = loadedlevel.table;
			heighttable = loadedlevel.heighttable; 
			crooms = loadedlevel.roomslist;
			
			foreach (GameObject tile in tiles)
				{
				Destroy(tile);
				}
				tiles.Clear();
				wallmasterlist.Clear();
				Debug.Log(m_pyOutput);
				
				foreach (roomsimple room in crooms){
					parseroom2(room,heighttable,modelarray);
				}
			
			
			
			
			
			
		}
		
		
		
		
		
		
        
        GUI.TextArea(new Rect(50, 330, 600, 300), m_pyOutput);
    }

}
