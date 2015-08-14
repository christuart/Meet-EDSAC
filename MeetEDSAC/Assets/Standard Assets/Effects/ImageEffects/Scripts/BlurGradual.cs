using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.ImageEffects;


[RequireComponent (typeof(Camera))]
[AddComponentMenu ("Image Effects/Blur/Blur (Gradual)")]
public class BlurGradual : PostEffectsBase
{
	
	[Range(0, 2)]
	public int downsample = 1;
	
	public enum BlurType {
		StandardGauss = 0,
		SgxGauss = 1,
	}
	
	[Range(0.01f, 10.0f)]
	public float blurStartSize = 0.01f;
	[Range(0.01f, 10.0f)]
	public float blurFinalSize = 3.0f;
	[Range(0.0f, 1.0f)]
	public float blurSlide = .25f;
	private bool blurOn = false;
	private bool blurSliding = false;

	public float blurSize = 0.01f;
	private bool blurSet = false;
	
	[Range(1, 4)]
	public int blurIterations = 1;
	
	public BlurType blurType= BlurType.StandardGauss;
	
	public Shader blurShader = null;
	private Material blurMaterial = null;

	public override bool CheckResources () {
		CheckSupport (false);
		
		blurMaterial = CheckShaderAndCreateMaterial (blurShader, blurMaterial);
		
		if (!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}
	
	public void OnDisable () {
		if (blurMaterial)
			DestroyImmediate (blurMaterial);
	}

	public void SetBlur(bool _blurOn) {
		blurOn = _blurOn;
		blurSliding = true;
	}
	
	public void OnRenderImage (RenderTexture source, RenderTexture destination) {
		if (!blurSet) {
			blurSize = blurStartSize;
			blurSet = true;
		}
		if (blurSliding) {
			if (blurOn) {
				if (blurFinalSize - blurSize < 0.02f * (blurFinalSize-blurStartSize)) {
					blurSize = blurFinalSize;
					blurSliding = false;
				} else {
					blurSize = Mathf.Lerp(blurSize,blurFinalSize,blurSlide);
				}
			} else {
				if (blurSize - blurStartSize < 0.02f * (blurFinalSize-blurStartSize)) {
					blurSize = blurStartSize;
					blurSliding = false;
				} else {
					blurSize = Mathf.Lerp(blurSize,blurStartSize,0.65f);
				}
			}
		}
		if (blurSize <= 0.0f) {
			Graphics.Blit (source, destination);
				return;
		}
		if (CheckResources() == false) {
			Graphics.Blit (source, destination);
			return;
		}
			
		float widthMod = 1.0f / (1.0f * (1<<downsample));
		
		blurMaterial.SetVector ("_Parameter", new Vector4 (-blurSize * widthMod, blurSize * widthMod, 0.0f, 0.0f));
		source.filterMode = FilterMode.Bilinear;
		
		int rtW = source.width >> downsample;
		int rtH = source.height >> downsample;
		
		// downsample
		RenderTexture rt = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
		
		rt.filterMode = FilterMode.Bilinear;
		Graphics.Blit (source, rt, blurMaterial, 0);
		
		var passOffs= blurType == BlurType.StandardGauss ? 0 : 2;
		
		for(int i = 0; i < blurIterations; i++) {
			float iterationOffs = (i*1.0f);
			blurMaterial.SetVector ("_Parameter", new Vector4 (-blurSize * widthMod + iterationOffs, blurSize * widthMod - iterationOffs, 0.0f, 0.0f));
			
			// vertical blur
			RenderTexture rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt, rt2, blurMaterial, 1 + passOffs);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;
			
			// horizontal blur
			rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt, rt2, blurMaterial, 2 + passOffs);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;
		}
		
		Graphics.Blit (rt, destination);
		
		RenderTexture.ReleaseTemporary (rt);
	}
}
