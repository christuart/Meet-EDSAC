using UnityEngine;
using System.Collections;

public class AspectRatioFixer : MonoBehaviour {

	public float lastUpdate = 0f;

	// Use this for initialization
	void Awake () 
	{
		// set the desired aspect ratio (the values in this example are
		// hard-coded for 16:9, but you could make them into public
		// variables instead so you can set them at design time)
		float targetaspect = 5.700f / 2.070f;
		
		// determine the game window's current aspect ratio
		float windowaspect = (float)Screen.width / (float)Screen.height;
		
		// current viewport height should be scaled by this amount
		float scaleheight = windowaspect / targetaspect;
		
		// obtain camera component so we can modify its viewport
		Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
		foreach (Camera camera in cameras) {
			if (!camera.gameObject.CompareTag("ClearScreenCamera")) {
				// if scaled height is less than current height, add letterbox
				if (scaleheight < 1.0f)
				{  
					Rect rect = camera.rect;
					
					rect.width = 1.0f;
					rect.height = scaleheight;
					rect.x = 0;
					rect.y = (1.0f - scaleheight) / 2.0f;
					
					camera.rect = rect;
				}
				else // add pillarbox
				{
					float scalewidth = 1.0f / scaleheight;
					
					Rect rect = camera.rect;
					
					rect.width = scalewidth;
					rect.height = 1.0f;
					rect.x = (1.0f - scalewidth) / 2.0f;
					rect.y = 0;
					
					camera.rect = rect;
				}
			}
		}
		Destroy (this);
	}

	void OnDrawGizmos() {
		if (Time.realtimeSinceStartup < lastUpdate) {
			
			lastUpdate = Time.realtimeSinceStartup;

		} else if (Time.realtimeSinceStartup-lastUpdate > .10f) {

			lastUpdate = Time.realtimeSinceStartup;

			// set the desired aspect ratio (the values in this example are
			// hard-coded for 16:9, but you could make them into public
			// variables instead so you can set them at design time)
			float targetaspect = 5.700f / 2.070f;
			
			// determine the game window's current aspect ratio
			Camera.main.rect = new Rect(0f,0f,1f,1f);
			Vector2 screenSize = Camera.main.ViewportToScreenPoint(new Vector2(1f,1f));
			float windowaspect = (float)screenSize.x / (float)screenSize.y;
			
			// current viewport height should be scaled by this amount
			float scaleheight = windowaspect / targetaspect;
			
			// obtain camera component so we can modify its viewport
			Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
			foreach (Camera camera in cameras) {
				if (!camera.gameObject.CompareTag("ClearScreenCamera")) {
					// if scaled height is less than current height, add letterbox
					if (scaleheight < 1.0f)
					{  
						Rect rect = camera.rect;
						
						rect.width = 1.0f;
						rect.height = scaleheight;
						rect.x = 0;
						rect.y = (1.0f - scaleheight) / 2.0f;
						
						camera.rect = rect;
					}
					else // add pillarbox
					{
						float scalewidth = 1.0f / scaleheight;
						
						Rect rect = camera.rect;
						
						rect.width = scalewidth;
						rect.height = 1.0f;
						rect.x = (1.0f - scalewidth) / 2.0f;
						rect.y = 0;
						
						camera.rect = rect;
					}
				}
			}

		}

	}

}
