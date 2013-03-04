using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class RectSerializer
{
public float x;
public float y;
public float Width;
public float Height;		
	
public void Fill(Rect rect2)
{
x = rect2.x;
y = rect2.y;
Width = rect2.width;
Height = rect2.height;	
}

public Rect Rect2
{ get { return new Rect(x, y,Width,Height); } }
}
