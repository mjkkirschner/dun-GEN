using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
//using IronPython;	
using System.Runtime.Serialization.Formatters.Binary;	
using System.IO;	
	




/// <summary>
/// The Manager that interfaces with the unity editor, sets all data, runs algorithm, and builds the level after parsing data
/// would benefit from being made more modular, all parsing for instance could happen within another object
/// </summary>
public class InGamePythonInterpreter4 : MonoBehaviour 
{	
	
	public int oldResolution;
	public int texResolution = 32;
	public bool atlasDirty = false; 
	public Material atlastMat;
	
	public GameObject generator;
	public levelobject curlevel;
	public int iterations = 10;
	public int xcenter;
	public int ycenter;
	public int xsd;
	public int ysd;
	
	public int wallheight;
	
	public bool doormaxalways;
	public bool doorminalways;
	public bool doorrandomize;
	public int desiredDoorWidth;
	
	public bool flatlevel;
	public int heightmin;
	public int heightmax;
	
	//public LayerMask layermask;

	public List<GameObject> roomparents = new List<GameObject>();
	
	
	public List<GameObject> tiles = new List<GameObject>();
	
	
	public List<GameObject> modelarray = new List<GameObject>();
	public List<GameObject> modelpreviewlist = new List<GameObject>();
	
	private List<roomsimple> crooms = new List<roomsimple>();
	public List<Vector3> wallmasterlist = new List<Vector3>();
	private List<room> rooms = new List<room>();
	
	
	// we cannot serialize these because they are 2d arrays, we could instead store this as a binary asset and get it back
	private int[,] table;
	public int[,] heighttable;
	//private TextAsset testcode;
   // private string m_pyCode;
   // private string m_pyOutput;
   // private PythonEnvironment m_pyEnv;
	 
//    private const string INITIALIZATION_CODE =
//@"
//import clr
//clr.AddReference('UnityEngine')
//import UnityEngine
//";
	
	
//	public void parseLevel(int[,]table,int [,]heighttable)
//	{
//	for (int i = 0;i < 200; i++){
//					for (int j = 0;j < 200; j++){
//				
//						// build the walls
//						if (table[i,j] == 3){
//							int height = heighttable[i,j];
//							for (int k = 0; k < height; k++){
//								Vector3 rot = new Vector3(UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f));
//								rot.x = Mathf.Round(rot.x / 90) * 90;
//								rot.y = Mathf.Round(rot.y / 90) * 90;
//								rot.z = Mathf.Round(rot.z / 90) * 90;
//								
//							GameObject currentgo = Instantiate(modelarray[UnityEngine.Random.Range(0,modelarray.Count)],new Vector3(i,k,j),Quaternion.identity) as GameObject;
//							currentgo.transform.Rotate(rot.x,rot.y,rot.z);
//							tiles.Add(currentgo);
//								
//							}
//						}
//						// build the floor tiles.
//						if (table[i,j] == 2){
//							int height = heighttable[i,j];
//							for (int k = height-2; k < height-1; k++){
//							GameObject currentgo = Instantiate(model4,new Vector3(i,k,j),Quaternion.identity) as GameObject;
//								tiles.Add(currentgo);	
//	
//					}
//	
//				}
//			}
//	
//			
//		}	
//		
//		
//	}	
//	public void parseroom(List<object> roomslist, int[,] heighttable ){
//		
//		
//	foreach (System.Object room in roomslist){
//			
//			GameObject roomcenter = new GameObject();
//			
//			IList<object>walls = m_pyEnv.m_pythonEngine.Operations.GetMember<IList<object>>(room,"walls");
//				foreach (System.Object vector2 in walls){
//					float posx = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"x");
//					float posy = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"y");
//					Vector2 position = new Vector2(posx,posy);
//					//Debug.Log(position);
//					if ((posx >= 0.0) && (posy >= 0.0)){ 
//						
//					int height = heighttable[(int)posx,(int)posy];
//							for (int k = 0; k < height; k++){
//								Vector3 rot = new Vector3(UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f));
//								rot.x = Mathf.Round(rot.x / 90) * 90;
//								rot.y = Mathf.Round(rot.y / 90) * 90;
//								rot.z = Mathf.Round(rot.z / 90) * 90;
//							
//							if (wallmasterlist.Contains(new Vector3(posx,k,posy))){
//							
//							break;
//								}
//							wallmasterlist.Add(new Vector3(posx,k,posy));
//							GameObject currentgo = Instantiate(modelarray[UnityEngine.Random.Range(0,modelarray.Count)],new Vector3(posx,k,posy),Quaternion.identity) as GameObject;
//							currentgo.transform.Rotate(rot.x,rot.y,rot.z);
//							tiles.Add(currentgo);
//							currentgo.transform.parent = roomcenter.transform;
//						
//						
//						}
//					}
//				}
//			
//		IList<object>floors = m_pyEnv.m_pythonEngine.Operations.GetMember<IList<object>>(room,"floortiles");
//			foreach (System.Object vector2 in floors){
//					float posx = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"x");
//					float posy = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"y");
//					Vector2 position = new Vector2(posx,posy);
//					if ((posx >= 0.0) && (posy >= 0.0)){
//					
//						int height = heighttable[(int)posx,(int)posy];
//							for (int k = height-2; k < height-1; k++){
//							GameObject currentgo = Instantiate(model4,new Vector3(posx,k,posy),Quaternion.identity) as GameObject;
//								tiles.Add(currentgo);	
//						currentgo.transform.parent = roomcenter.transform;
//					
//					}
//			
//				}	
//		
//		
//		
//			}
//	
//		}
//}

	
	
	
public void genMajorAtlas () {
	
	Rect [] atlasUvs; 	
	// we need to grab a list of textures from the preview folder	
	List<Texture2D> textures = new List<Texture2D>(); 	
	
	string[] filePaths = Directory.GetFiles(Application.dataPath +"/asset_textures/", "*.png"); 	
	
		foreach(string filename in filePaths){
	 		
			Texture2D texture = new Texture2D(100,100);
			
			using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
			{
			texture.LoadImage(reader.ReadBytes((int)reader.BaseStream.Length));
			}	
			textures.Add(texture);
			
		}
		
		// we should now have an array filled with all textures.. we'll take this and pass it to the pack function
			
	Debug.Log("starting major atlas generation ");		
		
	//Createpreviewfolder();	
	
	Texture2D textureAtlas = new Texture2D(4096,4096);
		
	if ((!File.Exists(Application.dataPath +"/asset_textures/majorTexture")) || (!File.Exists(Application.dataPath +"/asset_textures/majorTexture"  +"_atlas")) || (atlasDirty == true ))
		{
			
	// if the atlas is not there recreate it from all the textures	
	
	Debug.Log("attemtping to build major atlas and return coords");
			
	atlasUvs = textureAtlas.PackTextures(textures.ToArray(),0);
	RectSerializer[] atlasUvs_serialize = new RectSerializer[atlasUvs.Length];
	
			
	for (int i = 0; i < atlasUvs.Length; i++)
			{
			Rect uv = atlasUvs[i];
			
			RectSerializer newrect = new RectSerializer();
			newrect.Fill(uv);
			atlasUvs_serialize[i] = newrect;	
				
			}
			
	string fileName = Application.dataPath + "/asset_textures/majorTexture" + "_atlas";		
			
	BinaryFormatter bf = new BinaryFormatter();
	FileStream fs = new FileStream(fileName,FileMode.Create,FileAccess.Write);
	bf.Serialize(fs,atlasUvs_serialize);		
	fs.Close();				
	
	// we just saved the atlas of the major atlas  out		
			
	byte[] texturetosave = textureAtlas.EncodeToPNG();
	File.WriteAllBytes(Application.dataPath + "/asset_textures/majorTexture" ,texturetosave);		
			
			
		
		}
		
	else{
		// if they both already exist then load it and the atlas data	
		// but  check that the lengths are the same, if they are not then throw this atlas away and recall this function
		
		 using (BinaryReader reader = new BinaryReader(File.Open(Application.dataPath +"/asset_textures/majorTexture", FileMode.Open)))
			{
			textureAtlas.LoadImage(reader.ReadBytes((int)reader.BaseStream.Length));
			}	
		
			string fileName = Application.dataPath + "/asset_textures/majorTexture" + "_atlas";
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = new FileStream(fileName,FileMode.Open,FileAccess.Read);
			RectSerializer[] atlasUvs_serialize = (RectSerializer[])bf.Deserialize(fs);
			
			atlasUvs = new Rect[atlasUvs_serialize.Length];
			for (int i = 0; i < atlasUvs_serialize.Length; i++)
			{
				RectSerializer uv = atlasUvs_serialize[i];
				atlasUvs[i] = uv.Rect2;
			}
			
			fs.Close();
			
			//we can repopulate the texture array for each model here if we like, the problem is if we already have the atlas saved out then we don't
			//regenerate the textures so we never add them to the textures array, theres really no reason, but might be later, we can extract them
			// from the atlas anyway..
			
			// this isnt a particulary great check, what if I got rid of 2 textures and then added two textures?
			if (filePaths.Length != atlasUvs.Length)
			{
			
				File.Delete(Application.dataPath +"/asset_textures/majorTexture");
				File.Delete(fileName);	
			}
			
			
		}	
		
	
		
		//at this point the atlas should be set full of rectangles and the textures all generated.
//		
//		faceDictionary.Clear();
//		rotationDictionary.Clear();
//		
//		// we may want to make this conditional upon the current rotation, so if we are rotated a certain way then we add 
//		// the top face to whatever is actually on top, we can also just do this at the time we get the textures
//		
//		faceDictionary.Add("right",atlasUvs[0]);
//		faceDictionary.Add("left",atlasUvs[1]);	
//		faceDictionary.Add("top",atlasUvs[2]);	
//		faceDictionary.Add("bottom",atlasUvs[3]);
//		faceDictionary.Add("back",atlasUvs[4]);
//		faceDictionary.Add("front",atlasUvs[5]);
//		

			
		atlasDirty = false;
	
	}	
	
	
	
	
	
public void iterateColliderSides (GameObject wall)
	{
		
		int widthSegments;
		int lengthSegments;	
		float width;
		float length;
		GameObject plane;
		BoxCollider collider;
		
	foreach (Transform wallCluster in wall.transform) 
		{
			
			collider = (wallCluster.collider as BoxCollider);
			
			
			width = collider.size.x;
			length = collider.size.z;
	 		widthSegments = Mathf.RoundToInt(collider.size.x);	
			lengthSegments = Mathf.RoundToInt(collider.size.z);	
			
			plane = genLowPolyMesh("Top", widthSegments,lengthSegments,width,length,wallCluster.gameObject);
			plane.transform.position = collider.center + new Vector3(0, collider.size.y/2, 0);
			
			
			width = collider.size.x;
			length = collider.size.z;
			widthSegments = Mathf.RoundToInt(collider.size.x);	
			lengthSegments = Mathf.RoundToInt(collider.size.z);	
			
			plane = genLowPolyMesh("Bottom", widthSegments,lengthSegments,width,length,wallCluster.gameObject);
			plane.transform.position = collider.center - new Vector3(0, collider.size.y/2, 0);
			
			
			
			width = collider.size.x;
			length = collider.size.y;	
			widthSegments = Mathf.RoundToInt((collider).size.x);	
			lengthSegments = Mathf.RoundToInt((collider).size.y);	
			
			plane = genLowPolyMesh("Front", widthSegments,lengthSegments,width,length,wallCluster.gameObject);
			plane.transform.position = collider.center - new Vector3(0,0, collider.size.z/2);
			
			
			width = collider.size.x;
			length = collider.size.y;	
			widthSegments = Mathf.RoundToInt((collider).size.x);	
			lengthSegments = Mathf.RoundToInt((collider).size.y);	
			
			plane = genLowPolyMesh("Back", widthSegments,lengthSegments,width,length,wallCluster.gameObject);
			plane.transform.position = collider.center + new Vector3(0,0, collider.size.z/2);
			
			
			
			width = collider.size.z;
			length = collider.size.y;	
			widthSegments = Mathf.RoundToInt((collider).size.z);	
			lengthSegments = Mathf.RoundToInt((collider).size.y);	
			
			plane = genLowPolyMesh("Right", widthSegments,lengthSegments,width,length,wallCluster.gameObject);
			plane.transform.position = collider.center +  new Vector3(collider.size.x/2,0,0);
			
			
			width = collider.size.z;
			length = collider.size.y;	
			widthSegments = Mathf.RoundToInt((collider).size.z);	
			lengthSegments = Mathf.RoundToInt((collider).size.y);	
			
			plane = genLowPolyMesh("Left", widthSegments,lengthSegments,width,length,wallCluster.gameObject);
			plane.transform.position = collider.center -  new Vector3(collider.size.x/2,0,0);
			
			
			}
		
	}
	
	
	public Vector3 vertexCentroid (List<Vector3> currentVerts, GameObject ParentCluster)
	{
		Vector3 vertexCentroid = new Vector3(0,0,0);
			
		GameObject tempParent = new GameObject("temp offset");
		tempParent.transform.position = ParentCluster.GetComponent<BoxCollider>().center;	
			
		
		
		
		foreach (Vector3 vertex in currentVerts)
		{
		Vector3 vertexmoved = tempParent.transform.TransformPoint(vertex);
		vertexCentroid += vertexmoved;
			
		}
			DestroyImmediate(tempParent);
			vertexCentroid = (vertexCentroid/currentVerts.Count);
			return vertexCentroid;
	}
	
	public GameObject genLowPolyMesh (String colSide,int widthSegments,int lengthSegments,float width,float length,GameObject ParentCluster)
	{
		
			
			List<GameObject> tile_children = new List<GameObject>();
			
			Dictionary<Vector3,string> tilemap = ParentCluster.GetComponent<clusterComponet>().tilemap;
			GameObject plane = new GameObject();	
	
			 MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
       		 plane.AddComponent(typeof(MeshRenderer));
			
			foreach (Transform child in ParentCluster.transform)
		{
		
			tile_children.Add(child.gameObject);
		}
			
			
		
			Mesh m = new Mesh();
            m.name = plane.name;
 			
		
			//float width = widthSegments;
			//float length = lengthSegments;
			
            int hCount2 = widthSegments+1;
            int vCount2 = lengthSegments+1;
            int numTriangles = widthSegments * lengthSegments * 6;
            //int numVertices = hCount2 * vCount2;
		
			int numVertices = (widthSegments*lengthSegments) * 4;
 			
            Vector3[] vertices = new Vector3[numVertices];
            Vector2[] uvs = new Vector2[numVertices];
            int[] triangles = new int[numTriangles];
 
            int index = 0;
            float uvFactorX = 1.0f/widthSegments;
            float uvFactorY = 1.0f/lengthSegments;
            float scaleX = width/widthSegments;
            float scaleY = length/lengthSegments;
			int uvindex  = 0;
            for (float y = 0.0f; y < lengthSegments; y++)
            {	
                for (float x = 0.0f; x < widthSegments; x++)
                {	
				
					
					
                    if (colSide == "Top")
                    {	
						// testing out new generation method//
                        vertices[index] = new Vector3(x*scaleX - width/2f, 0.0f, y*scaleY - length/2f);
						vertices[index+1] = new Vector3((x+1)*scaleX - width/2f, 0.0f, y*scaleY - length/2f);	
						vertices[index+2] = new Vector3(x*scaleX - width/2f, 0.0f, (y+1)*scaleY - length/2f);
						vertices[index+3] = new Vector3((x+1)*scaleX - width/2f, 0.0f, (y+1)*scaleY - length/2f);
					
						
					}
				
					else if (colSide == "Bottom")
                    {
						
                        vertices[(numVertices-1)-index] = new Vector3(x*scaleX - width/2f, 0.0f, y*scaleY - length/2f);
                   		vertices[(numVertices-1)-(index+1)] = new Vector3((x+1)*scaleX - width/2f, 0.0f, y*scaleY - length/2f);
						vertices[(numVertices-1)-(index+2)] = new Vector3(x*scaleX - width/2f, 0.0f, (y+1)*scaleY - length/2f);
						vertices[(numVertices-1)-(index+3)] = new Vector3((x+1)*scaleX - width/2f, 0.0f, (y+1)*scaleY - length/2f);
					
				
					}
				
				
				
				
                    else if (colSide == "Front")
                    {
                        vertices[index] = new Vector3(x*scaleX - width/2f, y*scaleY - length/2f, 0.0f);
                    	vertices[index+1] = new Vector3((x+1)*scaleX - width/2f, y*scaleY - length/2f, 0.0f);
						vertices[index+2] = new Vector3(x*scaleX - width/2f, (y+1)*scaleY - length/2f, 0.0f);
						vertices[index+3] = new Vector3((x+1)*scaleX - width/2f, (y+1)*scaleY - length/2f, 0.0f);
				
					}
					else if (colSide == "Back")
                    {
                        vertices[(numVertices-1)-index] = new Vector3(x*scaleX - width/2f, y*scaleY - length/2f, 0.0f);
                    	vertices[(numVertices-1)-(index+1)] = new Vector3((x+1)*scaleX - width/2f, y*scaleY - length/2f, 0.0f);
						vertices[(numVertices-1)-(index+2)] = new Vector3(x*scaleX - width/2f, (y+1)*scaleY - length/2f, 0.0f);
						vertices[(numVertices-1)-(index+3)] = new Vector3((x+1)*scaleX - width/2f, (y+1)*scaleY - length/2f, 0.0f);
								
					}
				
				
				
				
				
					else if (colSide == "Right")
					{
						vertices[index] = new Vector3(0.0f, y*scaleY - length/2f, x*scaleX - width/2f);
						vertices[index+1] = new Vector3(0.0f, (y)*scaleY - length/2f, (x+1)*scaleX - width/2f);
						vertices[index+2] = new Vector3(0.0f, (y+1)*scaleY - length/2f, (x)*scaleX - width/2f);
						vertices[index+3] = new Vector3(0.0f, (y+1)*scaleY - length/2f, (x+1)*scaleX - width/2f);
					
					
					
					
					
					}
                    else if (colSide == "Left")
					
				{
						vertices[(numVertices-1)-index] = new Vector3(0.0f, y*scaleY - length/2f, x*scaleX - width/2f);
						vertices[(numVertices-1)-(index+1)] = new Vector3(0.0f, (y)*scaleY - length/2f, (x+1)*scaleX - width/2f);
						vertices[(numVertices-1)-(index+2)] = new Vector3(0.0f, (y+1)*scaleY - length/2f, (x)*scaleX - width/2f);	
						vertices[(numVertices-1)-(index+3)] = new Vector3(0.0f, (y+1)*scaleY - length/2f, (x+1)*scaleX - width/2f);
					
				
				
				
					}
                    
					
					
					index += 4;
					
                }
            }
 		
		

		
		// create a new list to hold current verts
		
		List<Vector3> currentVerts = new List<Vector3>();
		
		if ((colSide == "Left") || (colSide == "Bottom") || (colSide == "Back"))
		{ 	
			
			
			index = numTriangles-1;
			//uvindex = numVertices-1;
			
			for (int y = 0; y < lengthSegments; y++)
            {
                for (int x = 0; x < widthSegments; x++)
                {
					
 					triangles[index]   = ((y*4)     * (hCount2-1)) + x*4;
                    triangles[index-1] = ((y*4) * (hCount2-1))+2 + x*4;
                    triangles[index-2] = ((y*4)     * (hCount2-1)) + x*4 + 1;
 
                    triangles[index-3] = ((y*4) * (hCount2-1))+2 + x*4;
                    triangles[index-4] = ((y*4) * (hCount2-1))+2 + x*4 + 1;
                    triangles[index-5] = ((y*4)     * (hCount2-1)) + x*4 + 1;
                    
					

					
					
					currentVerts.Add(vertices[triangles[index]]);
					currentVerts.Add(vertices[triangles[index-1]]);
					currentVerts.Add(vertices[triangles[index-2]]);
					currentVerts.Add(vertices[triangles[index-3]]);
					currentVerts.Add(vertices[triangles[index-4]]);
					currentVerts.Add(vertices[triangles[index-5]]);
					
					
					uvs[(((y*4)     * (hCount2-1)) + x*4)] = new Vector2(0,0);
					uvs[(((y*4) * (hCount2-1))+2 + x*4)] = new Vector2(1,0);
					uvs[(((y*4)     * (hCount2-1)) + x*4 + 1)] = new Vector2(0,1);
					uvs[(((y*4) * (hCount2-1))+2 + x*4 + 1)] = new Vector2(1,1);

					index -= 6;
					
					
					//here we calculate the centroid of the quad and find the closest block
					
					Vector3 centroid = vertexCentroid(currentVerts,ParentCluster);
					
					GameObject closestGameObject = tile_children
						.OrderBy(go => Vector3.Distance(go.transform.position, centroid))
							.FirstOrDefault();
					
						Debug.Log(closestGameObject);
					
					
					
					
				}
            }
 	
		}
		
		else{
			index = 0;
		 for (int y = 0; y < lengthSegments; y++)
            {
                for (int x = 0; x < widthSegments; x++)
                {
				
                    triangles[index]   = ((y*4)     * (hCount2-1)) + x*4;
                    triangles[index+1] = ((y*4) * (hCount2-1))+2 + x*4;
                    triangles[index+2] = ((y*4)     * (hCount2-1)) + x*4 + 1;
 
                    triangles[index+3] = ((y*4) * (hCount2-1))+2 + x*4;
                    triangles[index+4] = ((y*4) * (hCount2-1))+2 + x*4 + 1;
                    triangles[index+5] = ((y*4)     * (hCount2-1)) + x*4 + 1;
                    
					
			
					currentVerts.Add(vertices[triangles[index]]);
					currentVerts.Add(vertices[triangles[index+1]]);
					currentVerts.Add(vertices[triangles[index+2]]);
					currentVerts.Add(vertices[triangles[index+3]]);
					currentVerts.Add(vertices[triangles[index+4]]);
					currentVerts.Add(vertices[triangles[index+5]]);
					
					
					
					index += 6;
                
				
					Vector3 centroid = vertexCentroid(currentVerts,ParentCluster);
					
					
					
					GameObject closestGameObject = tile_children
						.OrderBy(go => Vector3.Distance(go.transform.position, centroid))
							.FirstOrDefault();
					
					Debug.Log(closestGameObject);
					
				
				}
            }
		}
			
		
			
            m.vertices = vertices;
            m.uv = uvs;
            m.triangles = triangles;
            m.RecalculateNormals();
 
            //AssetDatabase.CreateAsset(m, "Assets/Editor/" + planeAssetName);
            //AssetDatabase.SaveAssets();
        
 		meshFilter.renderer.sharedMaterial = atlastMat;
        meshFilter.sharedMesh = m;
        m.RecalculateBounds();
		return plane;
}	

	
public void clusterTiles_findCuts (GameObject roomcenter, string wallside)
		
	{
		
	GameObject parent = roomcenter.transform.FindChild(wallside).gameObject;	
	
	List<GameObject> clustersAddToWall = new List<GameObject>();
		
	
		// for each cluster inside of the wall
	foreach (Transform cluster in parent.transform)
		{
			
		List<GameObject> clustertiles = new List<GameObject>();	
			// for each child of each cluster
			foreach ( Transform tile in cluster)
			{
  		
			clustertiles.Add(tile.gameObject);
				
			
			}
			
			
			int maxRowSize = 0 ;
			
			foreach ( GameObject tile in clustertiles)
			{
  			
			
						// select all the tiles with same y value
			List<GameObject> neighborsSameY = clustertiles.Where(o => Mathf.Abs(o.transform.position.y - tile.transform.position.y) < .3).ToList(); 
				
			if (neighborsSameY.Count > maxRowSize)
				{
					maxRowSize = neighborsSameY.Count;
					
				}
				//this is a first loop where we calculate the largest row
				
			}
			
			
			
			
			foreach (GameObject tile in clustertiles)
			{
  			
			
						// select all the tiles with same y value
			List<GameObject> neighborsSameY2 = clustertiles.Where(o => Mathf.Abs(o.transform.position.y - tile.transform.position.y) < .3).ToList(); 
				 
			if (neighborsSameY2.Count < maxRowSize)
				{
				
					
				GameObject newcluster = new GameObject("cluster_");
				tile.transform.parent = newcluster.transform;
				
				clustersAddToWall.Add(newcluster);	
				
					
				}	
				
			
			
			}
			
	
		}		
		
		foreach (GameObject floatingCluster in clustersAddToWall)
			{
		floatingCluster.transform.parent = roomcenter.transform.FindChild(wallside).transform;
			}
		
	}
	
	
	
	
public void clusterTiles(GameObject roomcenter, string wallside)
	{
		// BUGS IN THIS METHOD, clusters are subdividing too easily... WHAT THE HELL IS GOING ON! how do debug this, maybe do it visually
	
		
		
	// new list of wall tiles for this wall	
	List<GameObject> currentWallTiles = new List<GameObject>();	
	
	// the first cluster	
	GameObject cluster = new GameObject("cluster one");
	
		
		
	//parent  = the wall 	
	GameObject parent = roomcenter.transform.FindChild(wallside).gameObject;	
	
	// add all children of the wall to the list of walltiles....	
	foreach (Transform child in parent.transform)
		{
  		currentWallTiles.Add(child.gameObject);
		}	
		
	if (currentWallTiles.Count == 0)
		{
			DestroyImmediate(cluster);
			return;
		
		
		}
	
	// stick the cluster into the the wall AFTER adding the tiles of that wall to a list, so the cluster is not counted as a tile
	cluster.transform.parent = roomcenter.transform.FindChild(wallside).transform;	
		
	//randomly select a tile from the list, bug here...
	
	
	GameObject currentTile = currentWallTiles[UnityEngine.Random.Range(0,currentWallTiles.Count-1)];
	currentWallTiles.Remove(currentTile);
	//place the randomly selected first tile into the first cluster
	currentTile.transform.parent = cluster.transform;
	
	
		//while the list contains more than one tile
		while (currentWallTiles.Count > 0)
		
		{
			float minDistance = 10000;
			GameObject closest = null;
			//check each gameobject in the list of wall tiles and the current tile ( which is first the first tile, then it's the previous closest tile
				foreach (GameObject tileToCluster in currentWallTiles)
					{
						foreach (Transform clusterTile in cluster.transform){
					float distance = Vector3.Distance(clusterTile.position,tileToCluster.transform.position);
					if (distance <= minDistance)
							{
							minDistance = distance;
							closest = tileToCluster;
							}
						}
        			}
			
			if (null != closest){
			
			if (minDistance <= 1.0)
				{	
				closest.transform.parent = cluster.transform;
				
				
				}
			else
				{
				cluster = new GameObject("cluster_");
				cluster.transform.parent = roomcenter.transform.FindChild(wallside).transform;
				closest.transform.parent = cluster.transform;
				}
			
				currentWallTiles.Remove(closest);

				//currentTile = closest;
			}
			
			
		
		}
		
		
		
	
	}
	
	
public void SortXYZ (GameObject roomcenter,string wallside) 	
	{
	// get all the cluters in the wall	
	foreach (Transform cluster in roomcenter.transform.FindChild(wallside).transform)
		{
			//get each child from cluster and add to a list of children
		List<GameObject> children = new List<GameObject>();
		foreach (Transform child in cluster)
			{
				
				// add each child to a list
				children.Add(child.gameObject);
				
				
				
			}	
			// sort this list by x,y,z
			List<GameObject> sortedChildren  = children.OrderBy (child => child.transform.position.x)
															.ThenBy(child => child.transform.position.y)
																	.ThenBy(child => child.transform.position.z).ToList();
			
			
			
			// get all tiles in relation to the bottom left corner... ***TEST IF THIS WORKS FOR ALL WALLS, not just top***
			cluster.gameObject.AddComponent<clusterComponet>();
			cluster.GetComponent<clusterComponet>().tilemap.Clear();
			
			foreach (GameObject sortedchild in sortedChildren)
			{
				
				Vector3 key = sortedchild.transform.localPosition - sortedChildren[0].transform.localPosition;
				
				cluster.gameObject.GetComponent<clusterComponet>().tilemap.Add(key, sortedchild.name.Remove(sortedchild.name.Length-7) + "|" + (sortedchild.transform.localEulerAngles.ToString()));
			}
			
			
			foreach(KeyValuePair<Vector3,String> entry in cluster.gameObject.GetComponent<clusterComponet>().tilemap)
			{
    			Debug.Log(entry.Value);
				// do something with entry.Value or entry.Key
			}
				
						
			
		
		
		}
		 
  
		
	
	
	//List<Order> SortedList = objListOrder.OrderBy(o=>o.OrderDate).ToList();
	
	
	}
	
	
public void createEncapCollider (GameObject roomcenter, string wallside) 
	{
//get colliders in each room's wallside		
	
	
if (roomcenter.transform.FindChild(wallside).gameObject.GetComponent<Collider>())
		{
	DestroyImmediate(roomcenter.transform.FindChild(wallside).GetComponent<Collider>());
		Debug.Log("destroy" +  wallside + "collider");
		}
		
// want to change this so we look for clusters inside and the children within these
		
foreach (Transform cluster in roomcenter.transform.FindChild(wallside).transform)
		{
		
		//get all the children's colliders inside the cluster		
		Collider[] colliders = cluster.GetComponentsInChildren<Collider>();
		//create a new center vector for each room if the there are children of the topwall
		if (colliders.Length > 0)
				{
				Vector3 center = new Vector3(0,0,0);
				//iterate the colliders in all children and calculate their average center		
				// I think this is wrong as we're getting the centroid, not the center of the bounds, so asymetric objects have offcenter centers
				foreach (Collider col in colliders)
						{
									
						center = center + col.gameObject.transform.position;	
						}		
						
						center = center / (colliders.Length);
						
						
				//create a new bounds and INSTEAD of centering it, just set it equal to the bounds of the first object...	
				Bounds totalBounds = colliders[0].bounds;
				
				//iterate all the children colliders encapsulate them with the bounds object just created
				
				foreach (Collider col in colliders)
						{
							
						totalBounds.Encapsulate(col.bounds);
						
							
						}		
				//add a box collider to each top wall object	
				cluster.gameObject.AddComponent<BoxCollider>();		
				//get that collider we just added		
				BoxCollider collider =(BoxCollider)cluster.gameObject.GetComponent<Collider>();
				// set the center = to the center of the bounds
				collider.center = totalBounds.center;
				// set the size =  to the size of the bounds		
				collider.size = totalBounds.size;			
				}	
				
			//now that the collider is sized correctly and centered we can generate the texture of the wall		
			//roomcenter.transform.FindChild("topwall").GetComponent<genFlatTexWall>().genWallTextures();
		}
	}
	
	public int wallcheck(float posx,float posy,roomsimple room){
		if (posx == room.xpos-1){
		//Debug.Log ("left");
		return 0; 	
		
		}
		if (posx == room.width+room.xpos){
			//Debug.Log ("right");
			return 1;
		}
		if (posy == room.height+room.ypos){
			//Debug.Log ("top");
			return 2;
		}
		if (posy == room.ypos-1){
			//Debug.Log ("bottom");
			return 3;
		}
		
		return 4;
		//Debug.Log("something has gone wrong");
		
		
	}
	
	public bool edgecheck(float posx,float posy,roomsimple room){
		if ((posx == room.xpos-1) || (posy== room.ypos-1)){
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
	
		
			//use the parent parameter as the parent of the room, it also holds the RoomComponent later
						
			GameObject roomcenter = parent;
			roomcenter.transform.position = Vector3.zero;
			// add each tile made to the tile list
			tiles.Add(roomcenter);
			foreach (Vector2Serializer wall in room.walls){
					
					float posx = wall.x;
					float posy = wall.y;
					// only look at tiles above 0,0	
					if ((posx >= 0.0) && (posy >= 0.0)){ 
					// use the height to create walls of the correct height	
					int height = heighttable[(int)posx,(int)posy];
							for (int k = 0; k < height+(wallheight); k++){
								Vector3 rot = new Vector3(UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f));
								rot.x = Mathf.Round(rot.x / 90) * 90;
								rot.y = Mathf.Round(rot.y / 90) * 90;
								rot.z = Mathf.Round(rot.z / 90) * 90;
							
							if (wallmasterlist.Contains(new Vector3(posx,k,posy))){
							//Debug.Log(new Vector3(posx,k,posy));
							//Debug.Log("already in wallmasterlist");
							continue;
								}
							wallmasterlist.Add(new Vector3(posx,k,posy));
							GameObject currentgo = Instantiate(modelarray[UnityEngine.Random.Range(0,modelarray.Count)],new Vector3(posx,k,posy),Quaternion.identity) as GameObject;
							currentgo.transform.Rotate(rot.x,rot.y,rot.z);
							tiles.Add(currentgo);
							
					
					
							//here is where we check the x and z and determine what parent this tile belongs to
								if (wallcheck(posx,posy,room) == 0) {							
							
							currentgo.transform.parent = roomcenter.transform.FindChild("leftwall").transform;
					}
							else if (wallcheck(posx,posy,room) == 1) {							
							
							currentgo.transform.parent = roomcenter.transform.FindChild("rightwall").transform;
					}
							
							else if (wallcheck(posx,posy,room) == 2) {							
							
							currentgo.transform.parent = roomcenter.transform.FindChild("topwall").transform;
					}
							
							else if (wallcheck(posx,posy,room) == 3) {							
							
							currentgo.transform.parent = roomcenter.transform.FindChild("bottomwall").transform;
					}
					
					
					
					
							//I think the wrong positions are added to the walltobuild because of the offset roomcenter
						
					
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
							
					Vector3 rot = new Vector3(UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f),UnityEngine.Random.Range(0.0f,360.0f));
								rot.x = Mathf.Round(rot.x / 90) * 90;
								rot.y = Mathf.Round(rot.y / 90) * 90;
								rot.z = Mathf.Round(rot.z / 90) * 90;
					
					
							GameObject currentgo = Instantiate(modelarray[3],new Vector3(posx,k,posy),Quaternion.identity) as GameObject;
							tiles.Add(currentgo);	
							currentgo.transform.parent = roomcenter.transform.FindChild("floor").transform;
							currentgo.transform.Rotate(rot.x,rot.y,rot.z);
						}
						
						else{	
					
						
					
						int height = heighttable[(int)posx,(int)posy];
							for (int k = height-3; k < height-1; k++){
								GameObject currentgo = Instantiate(modelarray[3],new Vector3(posx,k,posy),Quaternion.identity) as GameObject;
								tiles.Add(currentgo);	
								currentgo.transform.parent = roomcenter.transform.FindChild("floor").transform;
						
					}
			
				}	
		
		
		
			}
	
		}

		
	//this creates sub clusters that group the walls 

	clusterTiles(roomcenter,"topwall");
	clusterTiles(roomcenter,"bottomwall");
	clusterTiles(roomcenter,"leftwall");
	clusterTiles(roomcenter,"rightwall");
	clusterTiles(roomcenter,"floor");	
	
		
		
	clusterTiles_findCuts(roomcenter,"topwall");
	clusterTiles_findCuts(roomcenter,"bottomwall");
	clusterTiles_findCuts(roomcenter,"leftwall");
	clusterTiles_findCuts(roomcenter,"rightwall");
	clusterTiles_findCuts(roomcenter,"floor");		
		
		
		
	//this creates colliders around entire walls	
		
	createEncapCollider(roomcenter,"topwall");
	createEncapCollider(roomcenter,"bottomwall");
	createEncapCollider(roomcenter,"leftwall");
	createEncapCollider(roomcenter,"rightwall");
	createEncapCollider(roomcenter,"floor");	
	
		
		
		
	// this creates a dictionary of the tiles relative to the bottom left corner of the wall	
	// it may be necessary to do this before generating the tiles, then inside of the generation method we can grab the correct tile.
	// lets test that first...... this method needs to be tested for working with more than just the topwall!!!!!	
	SortXYZ(roomcenter,"topwall");		
	SortXYZ(roomcenter,"bottomwall");
	SortXYZ(roomcenter,"leftwall");	
	SortXYZ(roomcenter,"rightwall");
	
		
		
		
	// this iterates each side of each wall and then creates low poly planes on each side.	
		
	iterateColliderSides(roomcenter.transform.FindChild("topwall").gameObject);		
	iterateColliderSides(roomcenter.transform.FindChild("bottomwall").gameObject);		
	iterateColliderSides(roomcenter.transform.FindChild("leftwall").gameObject);		
	iterateColliderSides(roomcenter.transform.FindChild("rightwall").gameObject);		
	//iterateColliderSides(roomcenter.transform.FindChild("floor").gameObject);		
	//temporarily turn off the floor since we are not sorting it and creating lookup texture dictionaries for it yet... and cluster comps etc.
		
	// guessing we need to do the atlas creation after all the room parsing and building because
	// the other texture creation is happening as we iterate each room	
	
	
		
	}
	
	
	
	
	public void parseroom2(roomsimple room, int[,] heighttable, List<GameObject> modelarray ){
	
		
		//create a new centerobject and the 4 walls and floor subparents
		GameObject newroomcenter = new GameObject("Room");
		roomparents.Add(newroomcenter);
		
		GameObject leftwall = new GameObject("leftwall");
		GameObject rightwall = new GameObject("rightwall");
		GameObject topwall = new GameObject("topwall");
		GameObject bottomwall = new GameObject("bottomwall");					
		GameObject floor = new GameObject("floor");
		
		leftwall.transform.parent = newroomcenter.transform;
		
		rightwall.transform.parent = newroomcenter.transform;
		
		bottomwall.transform.parent = newroomcenter.transform;
		
		topwall.transform.parent = newroomcenter.transform;
		//topwall.layer = 2;
		//topwall.AddComponent<genFlatTexWall>();
		
		floor.transform.parent = newroomcenter.transform;
		
		
		newroomcenter.AddComponent<Roomcomponent>().room = room;
		newroomcenter.GetComponent<Roomcomponent>().load_models();
		
		parseroom2(room,heighttable,modelarray,newroomcenter);	
	
			
		
	}	
		
	public void combineSubMeshCheck(GameObject wall)
	{
		int vertcount = 0;
		List<GameObject> childtiles = new List<GameObject>();
		// iterate each tile of the parent wall
	foreach (Transform child in wall.transform)
			
		{	childtiles.Add(child.gameObject);
			//get all children mesh filters
			MeshFilter[] meshes = child.gameObject.GetComponentsInChildren<MeshFilter>();
			foreach (MeshFilter meshfilter in meshes)
			{	//count the vertexes that we would combine together
				vertcount += meshfilter.sharedMesh.vertexCount;
				// if theres too many verts for a combined mesh we need to create a second parent(topwall2 for example)
					
			}
			
			
		
		}
		
		if (vertcount > 64000)
				{
				
				GameObject subparent = new GameObject("subparent");
				//subparent.AddComponent<CombineChildren>();	
				// this funkiness places the subparent under the roomobject rather then under the wall parent
				subparent.transform.parent = wall.transform.parent.transform;
				// now we need to move half the children to the new parent
				for (int i = 0; i < childtiles.Count/2; i++)
						{
						
						childtiles[i].transform.parent = subparent.transform; 
					
					
						}
				}	
		
	}
	
	
	
	public void loadlevel()
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
				DestroyImmediate(tile);
				}
				tiles.Clear();
				wallmasterlist.Clear();
				roomparents.Clear();
				//Debug.Log(m_pyOutput);
				
				foreach (roomsimple room in crooms){
					parseroom2(room,heighttable,modelarray);
				}
			}
	public void loadlevelData()
		{
            string fileName = "auto_iteration_save.txt";
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = new FileStream(fileName,FileMode.Open,FileAccess.Read);
			levelobject loadedlevel = (levelobject)bf.Deserialize(fs);
			fs.Close();
			
			table = loadedlevel.table;
			heighttable = loadedlevel.heighttable; 
			crooms = loadedlevel.roomslist;
			
			}
	
	// Use this for initialization
	void Start () 
    {
		
		

//		testcode  = Resources.Load("mazegen1_roomslist") as TextAsset;
//        m_pyCode = testcode.text;
//		m_pyEnv = new PythonEnvironment();
//        m_pyEnv.RunCommand(INITIALIZATION_CODE);
//        m_pyOutput = string.Empty;
		 
			//m_pyEnv.RunCommand(m_pyCode);	
		
			//Action generate = m_pyEnv.m_scriptScope.GetVariable<Action>("genmaze");
		
            //get a delegate to the python function
            //Func<int, bool> IsOdd = m_pyEnv.m_scriptScope.GetVariable<Func<int, bool>>("isodd");
			
			//Func<int, bool> IsZero = m_pyEnv.m_scriptScope.GetVariable<Func<int, bool>>("equalzero");
			
	
	}


   // void OnGUI()
    //{
		
		
        //m_pyCode = GUI.TextArea(new Rect(50, 50, 600, 200), m_pyCode);
      //  if (GUI.Button(new Rect(50, 270, 80, 40), "Run"))
        public void Run ()
		{	
			
			roomparents.Clear();
			rooms.Clear();
			crooms.Clear();
			
			
//            m_pyOutput = string.Empty;
//           	m_pyEnv.ExposeVariable("iterations",iterations);
//			m_pyEnv.ExposeVariable("xcenter",xcenter);
//			m_pyEnv.ExposeVariable("ycenter",ycenter);
//			m_pyEnv.ExposeVariable("xsd",xsd);
//			m_pyEnv.ExposeVariable("ysd",ysd);
//			
//			m_pyEnv.ExposeVariable("doormaxalways",doormaxalways);
//			m_pyEnv.ExposeVariable("doorminalways",doorminalways);
//			m_pyEnv.ExposeVariable("doorrandomize",doorrandomize);			
//			m_pyEnv.ExposeVariable("desireddoorwidth",desiredDoorWidth);
//			
//			m_pyEnv.ExposeVariable("flatlevel",flatlevel);
//			m_pyEnv.ExposeVariable("heightmax",heightmax);
//			m_pyEnv.ExposeVariable("heightmin",heightmin);
			
			
			
			
			
			
			//PythonEnvironment.CommandResult result = m_pyEnv.RunCommand(m_pyCode);
			
			//if theres no generator object create one
			if (GameObject.Find("Generator") == null)
				{
			
			generator = new GameObject("Generator");
			generator.AddComponent<mazegenerator_python_standin>();
			
				}
			else
			{
			
			DestroyImmediate(generator);
			generator = new GameObject("Generator");
			generator.AddComponent<mazegenerator_python_standin>();
			
			}	
			//run the generation
			
			generator.GetComponent<mazegenerator_python_standin>().generate(iterations,xcenter,ycenter,xsd,ysd,doormaxalways,doorminalways,doorrandomize,desiredDoorWidth,flatlevel,heightmin,heightmax);
		

			table =(int[,]) generator.GetComponent<mazegenerator_python_standin>().convertedQuadTable;
			heighttable = (int[,]) generator.GetComponent<mazegenerator_python_standin>().heightsTable;
			
			
			//table =(int[,])  m_pyEnv.m_scriptScope.GetVariable("convertarr");
			//heighttable = (int[,]) m_pyEnv.m_scriptScope.GetVariable("convertarrheights");
			
			// here we are going to take all the room objects from python and turn them into c# objects that can be serialized
//			IList<object> orgrooms = (IList<object>) m_pyEnv.m_scriptScope.GetVariable("rooms");
//			
//			foreach (object element in orgrooms){
//				rooms.Add((object)element);	
//			}
		
			rooms = generator.GetComponent<mazegenerator_python_standin>().rooms; 		
		
		
			// the next few sections iterate all rooms from python and create roomsimple objets from them (so they can be serialzied)
//			foreach (System.Object room in rooms){	
//				
//				
//				int xpos = m_pyEnv.m_pythonEngine.Operations.GetMember<int>(room,"xpos");
//				int ypos = m_pyEnv.m_pythonEngine.Operations.GetMember<int>(room,"ypos");
//				
//				int width = m_pyEnv.m_pythonEngine.Operations.GetMember<int>(room,"width");
//				int height = m_pyEnv.m_pythonEngine.Operations.GetMember<int>(room,"height");
//				System.Object center = m_pyEnv.m_pythonEngine.Operations.GetMember<System.Object>(room,"center"); 			
//					float centerposx = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(center,"x");
//					float centerposy = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(center,"y");
//				Vector2 centerpos = new Vector2	(centerposx,centerposy);
//					
//					
//					
//				IList<object>walls = m_pyEnv.m_pythonEngine.Operations.GetMember<IList<object>>(room,"walls");
//	
//				List<Vector2> currentwalllist = new List<Vector2>();
//				List<Vector2> currentfloortilelist = new List<Vector2>();	
//					foreach (System.Object vector2 in walls){
//					float posx = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"x");
//					float posy = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"y");
//					Vector2 position = new Vector2(posx,posy);	
//					currentwalllist.Add(position);
//				
//				
//			}
//						
//			IList<object>floors = m_pyEnv.m_pythonEngine.Operations.GetMember<IList<object>>(room,"floortiles");
//			foreach (System.Object vector2 in floors){
//					float posx = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"x");
//					float posy = m_pyEnv.m_pythonEngine.Operations.GetMember<float>(vector2,"y");
//					Vector2 position = new Vector2(posx,posy);
//					currentfloortilelist.Add(position);
//				
//								
//			}	
//			// finally create the serializble room object from all the data we got from python and add it to the room list (crooms)
//			roomsimple currentroom = new roomsimple(width,height,centerpos,currentwalllist,currentfloortilelist,xpos,ypos);
//				
//				crooms.Add(currentroom);
//				
//		}	
		
		
		foreach (room _room in rooms)
			{
			roomsimple currentroom = new roomsimple(_room.width,_room.height,_room.center,_room.walls,_room.floortiles,_room.xpos,_room.ypos);
			crooms.Add(currentroom);
			}	
				
			// we should check if generate returned something(maybe a bool on completion)	
           // if (!string.IsNullOrEmpty(result.output))
            //{
                //m_pyOutput =  result.output;
				foreach (GameObject tile in tiles)
				{
				DestroyImmediate(tile);
				}
				tiles.Clear();
				wallmasterlist.Clear();
				//Debug.Log(m_pyOutput);
				
				foreach (roomsimple room in crooms)
				{
					parseroom2(room,heighttable,modelarray);
				}
							
				genMajorAtlas();
		
				 curlevel = new levelobject(table,heighttable,crooms);	
				 save("auto_iteration_save.txt");		
			}
							
            //if (result.exception != null)
            //{
              //  m_pyOutput += "Python exception : " + result.exception.Message;
			//	Debug.Log(m_pyOutput);
				
            //}
		
		
		 public void save(string filename)
        {
            string fileName = filename;
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = new FileStream(fileName,FileMode.Create,FileAccess.Write);
			bf.Serialize(fs,curlevel);
			fs.Close();
			
			// we'll want to create a level object that contains multiple arrays, serialzing those, then cast back to that object
			// and reparse from it.
			
			
		}
		
	
	
	
	
		public void load()
        {
			
			loadlevel();
			
			
			
			
			
		}
		
		
		
		
        
    }

