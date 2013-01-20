using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class levelobject
{
	
	
public int[,] table {get;set;}
public int[,] heighttable{get;set;}
public List<roomsimple> roomslist{get;set;}
	
public levelobject(int[,] Table, int[,] Heighttable, List<roomsimple> Roomslist)
{
    table = Table;
    heighttable = Heighttable;
	roomslist = Roomslist;	
}

		

}

