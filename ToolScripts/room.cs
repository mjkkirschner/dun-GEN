using UnityEngine;
using System.Collections.Generic;
using System;

public class room 
{
	
	
	
	public List<Vector2> walls = new List<Vector2>(); 
	public List<Vector2> floortiles = new List<Vector2>();
	
	public float xpos;
	public float ypos;
	public float width;
	public float height;
	public Vector2 center;
	
	public room(float Xpos, float Ypos, float Width, float Height)
		{
			xpos = Xpos;
			ypos = Ypos;
			width = Width;
			height = Height;
			center = new Vector2((xpos-.5f)+.5f*width,(ypos-.5f)+.5f*height);
		
		  	for(float i = xpos; i < xpos+width; i++)
				{
			 		for(float j = ypos; j < ypos+height; j++)
						{	
						
						floortiles.Add(new Vector2(i,j));
				
						}
				}
		
			for(float i = xpos-1; i < xpos+width+1; i++)
				{
			 		if ((i != xpos-1) && (i !=xpos+width))
						{
						walls.Add(new Vector2(i,ypos-1));		
						walls.Add(new Vector2(i,ypos+height));
						}
					else
						{	
					for(float j = ypos-1; j < ypos+height+1; j++)
						{	
				
						walls.Add(new Vector2(i,j));
				
						}
				}
		}
		
		
		
		
		
		
		
		}
	// Use this for initialization
	void Start ()
	{
	
	}
	
	
}

