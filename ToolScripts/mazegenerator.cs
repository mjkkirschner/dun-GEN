using UnityEngine;
using System.Collections.Generic;
using System;

public class mazegenerator : MonoBehaviour
{
	public GameObject go;
	public List<room> rooms = new List<room>();
	public  block[,] quadtable;
	
	
	public mazegenerator ()
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
		 room newroom = new room(20,20,6,4);
		 rooms.Add(newroom);
		 foreach (Vector2 wall in newroom.walls)
					{
					quadtable[(int)wall.x,(int)wall.y] = new block(19,19,wall.x*20,wall.y*20,"wall");
					}
		
		foreach (Vector2 floor in newroom.floortiles)
					{
					quadtable[(int)floor.x,(int)floor.y] = new block(19,19,floor.x*20,floor.y*20,"floor");
					}
		
	}
	public void generate() {	
	for(int i = 0; i < 100; i++) 
	{
			Debug.Log("restarting function");
		Vector2 roomtotest = new Vector2(UnityEngine.Random.Range(2,10),UnityEngine.Random.Range(2,5)); 
		
		int roomindex = UnityEngine.Random.Range(0,rooms.Count-1);
		room currentroom = rooms[roomindex];	
		int wallindex = UnityEngine.Random.Range(0,currentroom.walls.Count-1);
		Debug.Log(roomindex);
		Debug.Log(currentroom.walls.Count-1);
	
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
						
					block o = quadtable[j,k];
					if (o != null) {	
					if (o.type == "earth")
						Debug.Log( "earth");
					else{
						Debug.Log( "something is here");
						die = true;
						}
				}
			}			
		}
	}
		if (die == false){
			room newroom3 = new room(scanstart.x,scanstart.y,roomtotest.x,roomtotest.y);
			rooms.Add(newroom3);
			
				
				
			foreach (Vector2 walli in newroom3.walls) {	
					Debug.Log(new Vector2(walli.x,walli.y));
			  quadtable[(int)walli.x,(int)walli.y] = new block(19,19,walli.x*20,walli.y*20,"wall");
				}
			foreach (Vector2 floori in newroom3.floortiles)	{
					
			  quadtable[(int)floori.x,(int)floori.y] = new block(19,19,floori.x*20,floori.y*20,"floor");
				}
			quadtable[(int)walltotest.x,(int)walltotest.y] = new block(19,19,walltotest.x*20,walltotest.y*20,"floor");    
			quadtable[(int)walltotest.x + (int)doorcheck.x,(int)walltotest.y+(int)doorcheck.y] = new block(19,19,((int)walltotest.x+doorcheck.x)*20,((int)walltotest.y+doorcheck.y)*20,"floor");
			
					
			}
		}
		
	}
	// Use this for initialization
	void Start ()
	{
	
		
		//call the generate function
		generate();
		block[,] table = this.GetComponent<mazegenerator>().quadtable;
		for (int i = 0;i < 100; i++){
			for (int j = 0;j < 100; j++){
				if (table[i,j] != null){
				
				Debug.Log(table[i,j].type);
				if (table[i,j].type == "wall"){
					Instantiate(go,new Vector3(i,j,0f),Quaternion.identity);
				
					}
				}
			}
		}
	}
}