using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]

public class roomsimple 
{
	
	
	
	public List<Vector2Serializer> walls = new List<Vector2Serializer>();
	public List<Vector2Serializer> floortiles = new List<Vector2Serializer>();
	
	//public float xpos;
	//public float ypos;
	public float width;
	public float height;
	public Vector2Serializer center;
	public float xpos;
	public float ypos;
	
	
	public roomsimple(/*float Xpos, float Ypos,*/ float Width, float Height, Vector2 Center, List<Vector2> Walls, List<Vector2> Floortiles, float Xpos, float Ypos)
		{
			//xpos = Xpos;
			//ypos = Ypos;
			width = Width;
			height = Height;
			
			xpos = Xpos;
			ypos = Ypos;
		
			center.Fill(Center);
			
			
			foreach(Vector2 v2 in Walls){
		  Vector2Serializer tempwall = new Vector2Serializer();
				tempwall.Fill(v2);
			walls.Add(tempwall);
		}
					
			foreach(Vector2 v2 in Floortiles){
		  Vector2Serializer tempfloor = new Vector2Serializer();
				tempfloor.Fill(v2);
			floortiles.Add(tempfloor);
		
		}
	}
	// Use this for initialization
	void Start ()
	{
	
	}
	
	
}

