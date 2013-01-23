using UnityEngine;
using System.Collections;

public class genFlatTex : MonoBehaviour {
	
	
	public GameObject model; 
		
	// Use this for initialization
	void Start () {
		
	Vector3 rayorigin = model.transform.localPosition - new Vector3(0,0,1);	
	RaycastHit hit;
	
		if(Physics.Raycast(rayorigin,Vector3.forward, out hit))
		
		{
	Vector2 texcoord = hit.textureCoord;	
	Debug.Log(texcoord);
	// this is the texture coordinate of where the ray hit, we can then sample the correct texture...
	//why are we doing this, why not just read pixels of all the textures... because we will get the wrong
	// sides, this lets us get the flat sides and rebuild textures with non flat uvs.		
	//because we know what the mesh is and what texture it uses we can now look up the pixel at this coordinate
	// and save this to a new texture the size of the wall...		
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
