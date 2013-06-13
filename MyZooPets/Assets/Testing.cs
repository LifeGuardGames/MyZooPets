using UnityEngine;
using System.Collections;

public class Testing : MonoBehaviour {
        private LTRect beautyTileRect;
        public Texture2D beauty;

        private float w;
    private float h;

	// Use this for initialization
	void Start () {
        w = Screen.width;
        h = Screen.height;
	           beautyTileRect = new LTRect(0.0f,0.0f,1.0f,0.33f );

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        Rect staticRect = new Rect(0.76f*w, 0.53f*h, 0.2f*w, 0.14f*h);
        Debug.Log(beautyTileRect.rect.y);
        if(GUI.Button( staticRect, "Flip Tile")){
            LeanTween.move( beautyTileRect, new Vector2( 0f, beautyTileRect.rect.y + 0.33f ), 1.0f);
        }
        // LeanTween.move( beautyTileRect, new Vector2( 0f, beautyTileRect.rect.y + 0.33f ), 1.0f, new object[]{});
        GUI.DrawTextureWithTexCoords( new Rect(0.8f*w, 0.5f*h - beauty.height*0.5f, beauty.width*0.5f, beauty.height*0.2f), beauty, beautyTileRect.rect);
    }
}
