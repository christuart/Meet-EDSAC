Shader "Custom/InvisibleMask" {
  SubShader {
    // draw after all opaque objects (queue = 2001):
    Tags { "Queue"="Geometry+1" }
    Pass {
      Blend One One // keep the image behind it
      
    }
  } 
}