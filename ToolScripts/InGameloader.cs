using UnityEngine;
using System.Collections.Generic;
using System;
using IronPython;	
using System.Runtime.Serialization.Formatters.Binary;	
using System.IO;	
	




/// <summary>
/// A simple python behaviour that demonstrates how code can be ran in the runtime engine
/// </summary>
public class InGameloader : MonoBehaviour 
{	
	public levelobject curlevel;
	public int iterations;
	public int xcenter;
	public int ycenter;
	public int xsd;
	public int ysd;

	public List<GameObject> tiles = new List<GameObject>();
	
	
	
	public GameObject go;
	
	public GameObject model1;
	public GameObject model2;
	public GameObject model3;
	public GameObject model4;
	
	public List<GameObject> modelarray = new List<GameObject>();
	
	
	
	public int[,] table;
	public int[,] heighttable;
	
	public void parseLevel(int[,]table,int [,]heighttable)
	{
	for (int i = 0;i < 200; i++){
					for (int j = 0;j < 200; j++){
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
	// Use this for initialization
	void Start () 
    {
		
		
		modelarray.Add(model1);
		modelarray.Add(model2);
		modelarray.Add(model3);
		modelarray.Add(model4);

	}


    void OnGUI()
    {
		
		
        
		
		
		 
		
		 if (GUI.Button(new Rect(300, 270, 80, 40), "Load"))
        {
			
			
            string fileName = "file.txt";
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = new FileStream(fileName,FileMode.Open,FileAccess.Read);
			levelobject loadedlevel = (levelobject)bf.Deserialize(fs);
			fs.Close();
			
			table = loadedlevel.table;
			heighttable = loadedlevel.heighttable; 
			
			
			foreach (GameObject tile in tiles)
				{
				Destroy(tile);
				}
				tiles.Clear();
				
				parseLevel(table,heighttable);
			}
		
		
		
		
		
		
        
    }

}
