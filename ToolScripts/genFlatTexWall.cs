using UnityEngine;
using System.Collections;

public class genFlatTexWall : MonoBehaviour {
	
	private LayerMask layermask; 
	public Texture2D walltexture;
		
	// Use this for initialization
	public void genWallTextures () {
	
	if (null == this.gameObject.GetComponent<Collider>())
		{	
		return;
		}
		
	this.gameObject.transform.localPosition += new Vector3(0,100,0);	
		
	Collider collider =  this.gameObject.GetComponent<Collider>();
	// we need the texture to be bigger than the collider by some factor, so each block is not a pixel	
	int height =(int) collider.bounds.size.y*32;
	int width = (int) collider.bounds.size.x*32;		
	walltexture = new Texture2D(width,height);		
		
		
	//iterate the collider from smallest to largest edge
		
	for (float x = collider.bounds.min.x; x < collider.bounds.max.x; x+=.03125f)
		{
			for (float y = collider.bounds.min.y; y < collider.bounds.max.y; y+=.03125f)
			{	
				
				//Debug.Log(new Vector2(x,y));
				Vector3 rayorigin = new Vector3(x,y,collider.bounds.min.z) - new Vector3(0,0,1);
				RaycastHit hit = new RaycastHit();
				if(Physics.Raycast(rayorigin,new Vector3(0,0,1.0f),out hit))//,Mathf.Infinity,layermask))
				
					{
					
					Vector2 texcoord = hit.textureCoord;
					Texture2D hittexture = hit.collider.renderer.material.mainTexture as Texture2D;
					//Debug.Log(hit.collider.gameObject);
					//Debug.Log(texcoord);
					
					//null reference errors from cubes with no maintexture, only color.
					texcoord.x *= hittexture.width; 
					texcoord.y *= hittexture.height;
					float colliderwidth = collider.bounds.size.x;
					float colliderheight = collider.bounds.size.y;
					
					int newy = (int)(((y - collider.bounds.min.y) * walltexture.height) / colliderheight) + 0;
					int newx = (int)(((x - collider.bounds.min.x) * walltexture.width) / colliderwidth) + 0;
					
						
					Color color = hittexture.GetPixel((int)texcoord.x,(int)texcoord.y);
					//Debug.Log( new Vector2((int)(32*(x/colliderwidth) ,(int)(32*(colliderheight))));
					walltexture.SetPixel(newx,newy,color);		
    				walltexture.Apply();
					//Debug.Log(color);		
					texcoord = Vector2.zero;	
					}

		}
		
		
			
	// this is the texture coordinate of where the ray hit, we can then sample the correct texture...
	//why are we doing this, why not just read pixels of all the textures... because we will get the wrong
	// sides, this lets us get the flat sides and rebuild textures with non flat uvs.		
	//because we know what the mesh is and what texture it uses we can now look up the pixel at this coordinate
	// and save this to a new texture the size of the wall...		
		}
	this.gameObject.transform.localPosition += new Vector3(0,-100,0);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void Start (){
	
	layermask = GameObject.Find("Interpreter").GetComponent<InGamePythonInterpreter4>().layermask;
		
	}
	
	
}
