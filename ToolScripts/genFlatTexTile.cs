using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;	

public class genFlatTexTile : MonoBehaviour {
	
	public Texture2D textureAtlas;
	
	private Collider collider;
	
	public List<Texture2D> textures = new List<Texture2D>();
	
	public Rect[] atlasUvs;
	
	public void Createpreviewfolder()
	{
		string path = Application.dataPath + "/asset_textures";
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
	
	
	
	
	
		
		
		
	
	public void updateColors (Color[] colors, Vector3 rayorigin,Vector2 rayPixPos, Vector3 direction, Texture2D currentSideTex, float xmin, float xmax, float ymin, float ymax)
	{
						RaycastHit hit = new RaycastHit();
							if(Physics.Raycast(rayorigin,direction,out hit))
							
								{
								
								Vector2 texcoord = hit.textureCoord;
								Texture2D hittexture = hit.collider.renderer.material.mainTexture as Texture2D;
								//Debug.Log(hit.collider.gameObject);
								//Debug.Log(texcoord);
								
								//null reference errors from cubes with no maintexture, only color.
								texcoord.x *= hittexture.width; 
								texcoord.y *= hittexture.height;
								float colliderwidth = Mathf.Abs(xmax - xmin);
								float colliderheight = Mathf.Abs(ymax - ymin);
								
								//linear transformations for x and y coords
								int newy = (int)(((rayPixPos.y - ymin) * currentSideTex.height) / colliderheight) + 0;
								int newx = (int)(((rayPixPos.x - xmin) * currentSideTex.width) / colliderwidth) + 0;
								
									
								Color color = hittexture.GetPixel((int)texcoord.x,(int)texcoord.y);
								//Debug.Log( new Vector2((int)(32*(x/colliderwidth) ,(int)(32*(colliderheight))));
								int newindex = newx + (newy * currentSideTex.width);
								
								
								
								colors[newindex] = color;		
			    				
								//Debug.Log(color);		
								texcoord = Vector2.zero;	
								}	
		
		
	}
		
	
	public void iterateColliderBounds(Collider collider, float resolution, Vector3 direction) 
	
	{
		float xmin = 0;
		float xmax = 0;
		float ymin = 0;
		float ymax = 0;		
		Vector3 rayorigin = Vector3.zero;
		Vector2 rayPixPos;
		// if the direction is facing forward or back
		if (direction.z != 0){
			
			// if the direction is facing forward then set bounds
			if (direction.z > 0) {
				xmin = collider.bounds.min.x;
				xmax = collider.bounds.max.x;
				ymin = collider.bounds.min.y;
				ymax = collider.bounds.max.y;
							}
			else {
				// if the direction is facing backward then set bounds switched
//				xmin = collider.bounds.max.x;
//				xmax = collider.bounds.min.x;
//				ymin = collider.bounds.max.y;
//				ymax = collider.bounds.min.y;
				
				
				xmin = collider.bounds.min.x;
				xmax = collider.bounds.max.x;
				ymin = collider.bounds.min.y;
				ymax = collider.bounds.max.y;
				
				
				 }
		}
		
		if (direction.y != 0){
			// if the direction is up or down then set bounds
			if (direction.y > 0) {
				xmin = collider.bounds.min.x;
				xmax = collider.bounds.max.x;
				ymin = collider.bounds.min.z;
				ymax = collider.bounds.max.z;
							}
			else {
				
				xmin = collider.bounds.min.x;
				xmax = collider.bounds.max.x;
				ymin = collider.bounds.min.z;
				ymax = collider.bounds.max.z;
					
				 }	
				
			}
		if (direction.x != 0){
			// if the direction is left or right then set bounds
			if (direction.x > 0) {
				xmin = collider.bounds.min.z;
				xmax = collider.bounds.max.z;
				ymin = collider.bounds.min.y;
				ymax = collider.bounds.max.y;
							}
			else {

				xmin = collider.bounds.min.z;
				xmax = collider.bounds.max.z;
				ymin = collider.bounds.min.y;
				ymax = collider.bounds.max.y;
					
				 }		
				}
			
			
			
		int height =(int) Mathf.Abs(ymax-ymin)*(int)resolution;
		int width = (int) Mathf.Abs(xmax-xmin)*(int)resolution;		
		
					
		Texture2D currentSideTex = new Texture2D(width,height);
		Color[] colors = currentSideTex.GetPixels();		
		
		
		for (float x = xmin; x < xmax; x+=(1/resolution))
					{
					
				for (float y = ymin; y < ymax; y+=(1/resolution))
						
						{	
				
							if (direction.z != 0)
								{
							
								
								 rayorigin = new Vector3(x,y,collider.bounds.center.z) - direction;
								
				
								}
							else if (direction.y != 0)
								{
							
								
								 rayorigin = new Vector3(x,collider.bounds.center.y,y) - direction;
								
				
								}
							
							else if (direction.x != 0)
								{
							
								
								 rayorigin = new Vector3(collider.bounds.center.x,y,x) - direction;
								
				
								}
						// this causes an error where the origin is too small, smaller than minimum...i think
						//	possible to use another variable that holds the unmodified vector
						rayPixPos = new Vector2(x,y);
						
						
						//we now update colors for each pixel, not sure about the ray orgin calcs above though
						updateColors(colors,rayorigin,rayPixPos,direction,currentSideTex,xmin,xmax,ymin,ymax);
					
							
						// after the colors are updated we probably need to update the textures 
							
						
						
								
							}
			
							
				
						}
		
				currentSideTex.SetPixels(colors);
				currentSideTex.Apply();	
				textures.Add(currentSideTex);		
					
			
					}
		
	
			
	public void genTileTextures () {
	
		
		
	Debug.Log("starting gentile tex");		
		
	Createpreviewfolder();	
	
	textureAtlas = new Texture2D(512,512);
		
	if ((!File.Exists(Application.dataPath +"/asset_textures/" +this.name)) || (!File.Exists(Application.dataPath +"/asset_textures/" +this.name +"_atlas")))
		{
		
	Vector3 direction;	
		
	if (null == this.gameObject.GetComponent<Collider>())
		{	
		if (null == this.gameObject.GetComponentInChildren<Collider>())
			{
			Debug.Log("breakout of gen tile texs, MISSING colliders");	
			return;
			}	
		else
			{
			 collider =  this.gameObject.GetComponentInChildren<Collider>();
		
			}
		
		}
		
	else
		{
			 collider =  this.gameObject.GetComponent<Collider>();
		}
			
		
		
	this.gameObject.transform.localPosition += new Vector3(100,100,100);	
		
	
		//need to call this 6 times with 6 directions


	for (int x = -1; x <= 1; x += 2){ 	
	
				direction = new Vector3(x,0,0);
		iterateColliderBounds(collider,32,direction);
		}
	for (int y = -1; y <= 1; y += 2){
					direction = new Vector3(0,y,0);
		iterateColliderBounds(collider,32,direction);
			}
	for (int z = -1; z <= 1; z += 2){
					direction = new Vector3(0,0,z);
		iterateColliderBounds(collider,32,direction);
				}			
		
	
	Debug.Log("attemtping to build atlas and return coords");
			
	atlasUvs = textureAtlas.PackTextures(textures.ToArray(),0);
	RectSerializer[] atlasUvs_serialize = new RectSerializer[atlasUvs.Length];
	
			
	for (int i = 0; i < atlasUvs.Length; i++)
			{
			Rect uv = atlasUvs[i];
			
			RectSerializer newrect = new RectSerializer();
			newrect.Fill(uv);
			atlasUvs_serialize[i] = newrect;	
				
			}
			
	string fileName = Application.dataPath + "/asset_textures/"+this.name + "_atlas";		
			
	BinaryFormatter bf = new BinaryFormatter();
	FileStream fs = new FileStream(fileName,FileMode.Create,FileAccess.Write);
	bf.Serialize(fs,atlasUvs_serialize);		
	fs.Close();				
	
			
	byte[] texturetosave = textureAtlas.EncodeToPNG();
	File.WriteAllBytes(Application.dataPath + "/asset_textures/"+this.name ,texturetosave);		
			
			
	this.gameObject.transform.localPosition += new Vector3(-100,-100,-100);
		
		}
		
	else{
			
		
		
		 using (BinaryReader reader = new BinaryReader(File.Open(Application.dataPath +"/asset_textures/" + this.name, FileMode.Open)))
			{
			textureAtlas.LoadImage(reader.ReadBytes((int)reader.BaseStream.Length));
			}	
		
			string fileName = Application.dataPath + "/asset_textures/"+this.name + "_atlas";
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
			// from the atlas anyway...
			
			
		}	
		
		
	
	}
	
	
}
