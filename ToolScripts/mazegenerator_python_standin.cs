using UnityEngine;
using System.Collections.Generic;
using System;


public class mazegenerator_python_standin : MonoBehaviour
{
	
	public List<room> rooms = new List<room>();
	public  block[,] quadtable;
	public 	int[,] heightsTable;
	public int[,] convertedQuadTable;
	private int doorwidth;
	
	
	System.Random randomgen = new System.Random(); //reuse this if you are generating many
	
	public int gaussint(int mean, int stdDev)
	{ 
		double u1 =randomgen.NextDouble(); //these are uniform(0,1) random doubles
		double u2 = randomgen.NextDouble();
		double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
		int randNormal = (int)(mean + stdDev * randStdNormal); //random normal(mean,stdDev^2)	
		randNormal = Math.Max(randNormal,2);
		return randNormal;
	}	
	
	public bool cornercheck (room room, Vector2 walltotest)
	{
	
		if((walltotest.x == room.xpos-1) && (walltotest.y == room.ypos-1))
		{
			return true;
			
		}
		
		if((walltotest.x == room.xpos-1) && (walltotest.y == room.ypos + room.height))
		{
			return true;
			
		}
		
		
		if((walltotest.x == room.xpos + room.width) && (walltotest.y == room.ypos + room.height))
		{
			return true;
			
		}
		
		
		if((walltotest.x == room.xpos + room.width) && (walltotest.y == room.ypos-1))
		{
			return true;
			
		}
		
		return false;
		
	}
	
	public mazegenerator_python_standin ()
		{
			quadtable = new block [100,100];
		
		
		  	for(int i = 0; i < 2000; i+=20)
				{
			 		for(int j = 0; j < 2000; j+=20)
						{	
						
						block currentsquare = new block(19,19,i,j,"earth");
						
						quadtable[i/20,j/20] = currentsquare;
						
						}
				}
		
			heightsTable = new int[100,100];
				
			for(int i = 0; i < 2000; i+=20)
				{
			 		for(int j = 0; j < 2000; j+=20)
						{	
						
						int currentheight = 1;
						
						heightsTable[i/20,j/20] = currentheight;
						
						}
				}
			
		
		
		 room newroom = new room(20,20,6,4);
		 rooms.Add(newroom);
		 foreach (Vector2 wall in newroom.walls)
					{
					quadtable[(int)wall.x,(int)wall.y] = new block(19,19,wall.x*20,wall.y*20,"wall");
					heightsTable[(int)wall.x,(int)wall.y] = newroom.zheight;
					}
		
		foreach (Vector2 floor in newroom.floortiles)
					{
					quadtable[(int)floor.x,(int)floor.y] = new block(19,19,floor.x*20,floor.y*20,"floor");
					heightsTable[(int)floor.x,(int)floor.y] = newroom.zheight;
					}
		
	}
	public void generate(int iterations, int xcenter, int ycenter, int xsd, int ysd, bool doormaxalways, bool doorminalways, bool doorrandomize, int desiredDoorWidth, bool flatlevel, int heightmin, int heightmax) {	
	for(int i = 0; i < iterations; i++) 
	{
		
		Debug.Log("restarting function");
		Vector2 roomtotest = new Vector2(gaussint(xcenter,xsd),gaussint(ycenter,ysd)); 
		
		int roomindex = UnityEngine.Random.Range(0,rooms.Count-1);
		room currentroom = rooms[roomindex];	
		int wallindex = UnityEngine.Random.Range(0,currentroom.walls.Count-1);
		//Debug.Log(roomindex);
		//Debug.Log(currentroom.walls.Count-1);
	
		Vector2 walltotest = currentroom.walls[wallindex];
		
		bool top = false;
		bool bottom = false;
		bool left = false;
		bool right = false;
		
		if (quadtable[(int)walltotest.x,(int)walltotest.y+1].type == "earth")
			top = true;
		if (quadtable[(int)walltotest.x+1,(int)walltotest.y].type == "earth")
			right = true;
		if (quadtable[(int)walltotest.x-1,(int)walltotest.y].type == "earth")
			left = true;
		if (quadtable[(int)walltotest.x,(int)walltotest.y-1].type == "earth")
			bottom = true;
		if (Convert.ToInt32(top)+Convert.ToInt32(right)+Convert.ToInt32(left)+Convert.ToInt32(bottom) > 1){
			Debug.Log("Corner Selected, Breaking Out");
			continue;
			}
		if (Convert.ToInt32(top)+Convert.ToInt32(right)+Convert.ToInt32(left)+Convert.ToInt32(bottom) < 1){
			Debug.Log("Corner Selected, Breaking Out");
			continue;
			}
		
			
		Vector2 scandir = walltotest - currentroom.center;
		Vector2 scanend = new Vector2 (0,0);
		Vector2	scanstart = new Vector2 (0,0);
		Vector2 doorcheck= new Vector2 (0,0);		

		//all this ugliness is because of how I am iterating from start to end and the inclusion rules of python range function
		if (top == true){
			 scanend =  walltotest + new Vector2((int)(Math.Floor(roomtotest.x/2)+1),1*roomtotest.y+2);
			 scanstart = new Vector2(walltotest.x - (int)(Math.Floor(roomtotest.x/2)),walltotest.y+1);
			 doorcheck = new Vector2(0,-1);
			//Debug.Log( "top");
			}
		else if (bottom == true){
			 scanend = new Vector2(walltotest.x + (int)(Math.Floor(roomtotest.x/2)+1),walltotest.y);
			 scanstart = walltotest + new Vector2(-1*(int)(Math.Floor(roomtotest.x/2)),-1*(roomtotest.y));
			 doorcheck = new Vector2(0,1);
			//Debug.Log( "bottom");
			}
		
		else if (left == true) {

			 scanstart = walltotest + new Vector2(-1*roomtotest.x,-1*(int)(Math.Floor(roomtotest.y/2)));
			 scanend = new Vector2(walltotest.x,walltotest.y + (int)(Math.Floor(roomtotest.y/2))+1);
			 doorcheck = new Vector2(1,0);
			//Debug.Log( "left");
			}
		
		else if (right == true){
			 scanend= (walltotest + new Vector2(1*(roomtotest.x+2),1*(int)(Math.Floor(roomtotest.y/2))+1));
			 scanstart = new Vector2(walltotest.x + 1,walltotest.y -1*(int)(Math.Floor(roomtotest.y/2)));
			 doorcheck = new Vector2(-1,0);
			//Debug.Log( "right");	
			}
		
		
			
		bool die = false;
		for(int j =(int) scanstart.x; j < (int)scanend.x; j++) {
			if (die != true)
							{
				for(int k = (int)scanstart.y; k < (int)scanend.y; k++) {	
					//Debug.Log(new Vector2(j,k));
					if ((j >=2) && (k >=2))
						{	
						block o = quadtable[j,k];
						if (o != null) {	
						if (o.type == "earth")
							Debug.Log( "earth");
						else{
							//Debug.Log( "something is here");
							die = true;
							}
						}		
					}
					else {
						die = true;
						Debug.Log ("Outside of Level Borders");	
						}
						
						
			}			
		}
	}
		if (die == false){
			room newroom3 = new room(scanstart.x,scanstart.y,roomtotest.x,roomtotest.y);
			currentroom.neighbors.Add(newroom3);
			newroom3.neighbors.Add(currentroom);	
			
			List<Vector2> overlapped = new List<Vector2>();  	
			
			foreach (Vector2 wall in currentroom.walls)
				{
				if (newroom3.walls.Contains(wall))
					{
					overlapped.Add(wall);
					}
				}
			
			overlapped.RemoveAll(wall => cornercheck(newroom3,wall));
			overlapped.RemoveAll(wall => cornercheck(currentroom,wall));
		
//			foreach (Vector2 wall in overlapped)
//				{
//				if (cornercheck(newroom3,wall))
//					{
//					overlapped.Remove(wall);
//					}
//				}	
//			foreach (Vector2 wall in overlapped)
//				{
//				if (cornercheck(currentroom,wall))
//					{
//					overlapped.Remove(wall);
//					}
//				}		
			
			if(flatlevel)	
				{
				 newroom3.zheight = currentroom.zheight;
					
				}
			else 
				{
				newroom3.zheight = currentroom.zheight + UnityEngine.Random.Range(heightmin,heightmax);
					
				}
				
				
			rooms.Add(newroom3);
			
			
				
			foreach (Vector2 walli in newroom3.walls) 
			{	
					//Debug.Log(new Vector2(walli.x,walli.y));
			 		 quadtable[(int)walli.x,(int)walli.y] = new block(19,19,walli.x*20,walli.y*20,"wall");
					if (heightsTable[(int)walli.x,(int)walli.y] > newroom3.zheight)
							{
							heightsTable[(int)walli.x,(int)walli.y] = heightsTable[(int)walli.x,(int)walli.y];
							}
					else
						{	
					
						heightsTable[(int)walli.x,(int)walli.y] = newroom3.zheight;
						}
			
			
			}
			
				
			foreach (Vector2 floori in newroom3.floortiles)	{
					
			 	quadtable[(int)floori.x,(int)floori.y] = new block(19,19,floori.x*20,floori.y*20,"floor");
				
			 	if (heightsTable[(int)floori.x,(int)floori.y] > newroom3.zheight)
							{
							heightsTable[(int)floori.x,(int)floori.y] = heightsTable[(int)floori.x,(int)floori.y];
							}
				else
							{	
					
							heightsTable[(int)floori.x,(int)floori.y] = newroom3.zheight;
							}
			
				
				}
			
			quadtable[(int)walltotest.x,(int)walltotest.y] = new block(19,19,walltotest.x*20,walltotest.y*20,"floor");    
			quadtable[(int)walltotest.x + (int)doorcheck.x,(int)walltotest.y+(int)doorcheck.y] = new block(19,19,((int)walltotest.x+doorcheck.x)*20,((int)walltotest.y+doorcheck.y)*20,"floor");
			
			
			room lastroom = rooms[rooms.Count-1];	
			
				
			if (doormaxalways)
				{
				doorwidth = overlapped.Count;
				
				
				}
			else if (doorminalways)
				{
				doorwidth = 1;	
				}
			
			else if (doorrandomize)
				{
				if (overlapped.Count > 1)
					{
					doorwidth = UnityEngine.Random.Range(1,overlapped.Count);	
					}
				else {
					doorwidth = 1;	
					 } 
				}
			else
				{
				doorwidth = Math.Min(desiredDoorWidth,overlapped.Count);	
				}
				
				
				for (int doorindex = 0; doorindex < doorwidth; doorindex++)
				{
				Vector2 walltodelete = overlapped[doorindex];
				if (currentroom.walls.Contains(walltodelete))
					{
					currentroom.walls.Remove(walltodelete);
					currentroom.floortiles.Add(walltodelete);	//guessing gid rid of this to fix double floor bug
					}
			
				if (lastroom.walls.Contains(walltodelete))
					{
					lastroom.walls.Remove(walltodelete);
					lastroom.floortiles.Add(walltodelete);	//guessing gid rid of this to fix double floor bug
					}
					
					
					
				}
				
				
				if(lastroom.walls.Contains(new Vector2(walltotest.x + doorcheck.x, walltotest.y + doorcheck.y)))
					{
					lastroom.walls.Remove(new Vector2(walltotest.x + doorcheck.x, walltotest.y + doorcheck.y));
					lastroom.floortiles.Remove(new Vector2(walltotest.x + doorcheck.x, walltotest.y + doorcheck.y));
					}
				
				if(currentroom.walls.Contains(new Vector2(walltotest.x + doorcheck.x, walltotest.y + doorcheck.y)))
					{
					currentroom.walls.Remove(new Vector2(walltotest.x + doorcheck.x, walltotest.y + doorcheck.y));
					currentroom.floortiles.Remove(new Vector2(walltotest.x + doorcheck.x, walltotest.y + doorcheck.y));
					}
			}
		}
		
	convertedQuadTable = new int[2000,2000]; 
	foreach (block currentblock in quadtable) 
		{
		if (currentblock.type == "earth")
			{
//			Debug.Log(currentblock.xpos);
//			Debug.Log(currentblock.ypos);	
			convertedQuadTable[(int)currentblock.xpos,(int)currentblock.ypos] = 1;
			}
		else if (currentblock.type == "floor")
			{
			convertedQuadTable[(int)currentblock.xpos,(int)currentblock.ypos] = 2;
			}	
		else if (currentblock.type == "wall")
			{
			convertedQuadTable[(int)currentblock.xpos,(int)currentblock.ypos] = 3;
			}		
			
		else
			{
			convertedQuadTable[(int)currentblock.xpos,(int)currentblock.ypos] = 0;
			}			
			
			
		}
	
	
	}
	
				
			
		
	
}