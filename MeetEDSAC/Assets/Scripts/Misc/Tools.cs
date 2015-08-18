using UnityEngine;
using System.Collections;

public abstract class Tools : MonoBehaviour {
	
	public static float LerpPlus(float from, float to, float rate, float threshold, float finalRate, ref bool inProgress) {
		if (Mathf.Abs(to - from) > threshold) {
			inProgress = true;
			return Mathf.Lerp (from, to, rate);
		} else {
			if (Mathf.Abs(to - from) > finalRate) {
				inProgress = true;
				return from + Mathf.Sign (to - from) * finalRate;
			} else {
				inProgress = false;
				return to;
			}
		}
	}
	
	public static int IntPow(int f, int p) {
		if (p < 0) 
			return -1;
		int r = 1;
		while (p > 0) {
			r *= f;
			p--;
		}
		return r;
	}
	
	public static Color HexToColour(string hex)
	{
		hex = hex.Replace ("0x", "");//in case the string is formatted 0xFFFFFF
		hex = hex.Replace ("#", "");//in case the string is formatted #FFFFFF
		byte a = 255;//assume fully visible unless specified in hex
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		//Only use alpha if the string has enough characters
		if(hex.Length == 8){
			a = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		}
		return new Color32(r,g,b,a);
	}
	
	public static Vector4 ViewportFromBounds(Vector3 min, Vector3 max, Camera mainCam) {
		
		float minXFound;
		float minYFound;
		float maxXFound;
		float maxYFound;
		
		Vector3 dX = new Vector3(max.x - min.x,0f,0f);
		Vector3 dY = new Vector3(0f,max.y - min.y,0f);
		Vector3 dZ = new Vector3(0f,0f,max.z - min.z);
		
		minXFound = Mathf.Min(new float[] {
			mainCam.WorldToViewportPoint(min).x,
			mainCam.WorldToViewportPoint(min+dX).x,
			mainCam.WorldToViewportPoint(min+dX+dY).x,
			mainCam.WorldToViewportPoint(min+dY).x,
			mainCam.WorldToViewportPoint(min+dZ).x,
			mainCam.WorldToViewportPoint(min+dZ+dX).x,
			mainCam.WorldToViewportPoint(min+dZ+dX+dY).x,
			mainCam.WorldToViewportPoint(min+dZ+dY).x });
		minYFound = Mathf.Min(new float[] {
			mainCam.WorldToViewportPoint(min).y,
			mainCam.WorldToViewportPoint(min+dX).y,
			mainCam.WorldToViewportPoint(min+dX+dY).y,
			mainCam.WorldToViewportPoint(min+dY).y,
			mainCam.WorldToViewportPoint(min+dZ).y,
			mainCam.WorldToViewportPoint(min+dZ+dX).y,
			mainCam.WorldToViewportPoint(min+dZ+dX+dY).y,
			mainCam.WorldToViewportPoint(min+dZ+dY).y });
		maxXFound = Mathf.Max(new float[] {
			mainCam.WorldToViewportPoint(min).x,
			mainCam.WorldToViewportPoint(min+dX).x,
			mainCam.WorldToViewportPoint(min+dX+dY).x,
			mainCam.WorldToViewportPoint(min+dY).x,
			mainCam.WorldToViewportPoint(min+dZ).x,
			mainCam.WorldToViewportPoint(min+dZ+dX).x,
			mainCam.WorldToViewportPoint(min+dZ+dX+dY).x,
			mainCam.WorldToViewportPoint(min+dZ+dY).x });
		maxYFound = Mathf.Max(new float[] {
			mainCam.WorldToViewportPoint(min).y,
			mainCam.WorldToViewportPoint(min+dX).y,
			mainCam.WorldToViewportPoint(min+dX+dY).y,
			mainCam.WorldToViewportPoint(min+dY).y,
			mainCam.WorldToViewportPoint(min+dZ).y,
			mainCam.WorldToViewportPoint(min+dZ+dX).y,
			mainCam.WorldToViewportPoint(min+dZ+dX+dY).y,
			mainCam.WorldToViewportPoint(min+dZ+dY).y });
		
		return new Vector4(minXFound,minYFound,maxXFound,maxYFound);
	}
	public static float MinDistanceFromBounds(Vector3 min, Vector3 max, Camera mainCam) {
		
		Vector3 dX = new Vector3(max.x - min.x,0f,0f);
		Vector3 dY = new Vector3(0f,max.y - min.y,0f);
		Vector3 dZ = new Vector3(0f,0f,max.z - min.z);
		Vector3 minusPos = -mainCam.transform.position;
		
		return Mathf.Min(new float[] {
			(minusPos+min).magnitude,
			(minusPos+min+dX).magnitude,
			(minusPos+min+dX+dY).magnitude,
			(minusPos+min+dY).magnitude,
			(minusPos+min+dZ).magnitude,
			(minusPos+min+dZ+dX).magnitude,
			(minusPos+min+dZ+dX+dY).magnitude,
			(minusPos+min+dZ+dY).magnitude });
	}
	public static Vector2 ViewportToCameraPoint(Camera cam, Vector2 viewportPoint) {
		return (Vector2)cam.ViewportToScreenPoint(viewportPoint) - new Vector2(cam.rect.x * Screen.width, cam.rect.y * Screen.height);
	}
}
