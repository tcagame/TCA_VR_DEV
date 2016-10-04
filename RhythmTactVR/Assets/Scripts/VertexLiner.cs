using UnityEngine;
using System.Collections;

[ RequireComponent( typeof( MeshRenderer ) ) ]
public class VertexLiner : MonoBehaviour {

	private Material _material;

    void Awake( ) {
        _material = GetComponent< Renderer >( ).material;
		_material.EnableKeyword( "_ALPHABLEND_ON" );

		string materialTag = "RenderType";
		Renderer rend = GetComponent<Renderer>();
        string result = rend.material.GetTag(materialTag, true, "Nothing");
        if (result == "Nothing")
            Debug.LogError(materialTag + " not found in " + rend.material.shader.name);
        else
            Debug.Log("Tag found!, its value: " + result);
    }
    

	// Use this for initialization
	void Start( ) {
	    
	}
	
	// Update is called once per frame
	void Update () {
		_material.SetColor( "_Color", new Color( 1, 0, 0, 0.5f ) );
	}
}
