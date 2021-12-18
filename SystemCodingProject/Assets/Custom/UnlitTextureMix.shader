Shader "Simple colored lighting"
{
    // a single color property
    Properties 
    {
        _Color ("Main Color", Color) = (1,0.5,0.5,1)
    }
    // define one subshader
    SubShader
    {
        // a single pass in our subshader
        Pass
        {
        // use fixed function per-vertex lighting
            Material
            {
                Diffuse [_Color]
            }
            Lighting On
        }
    }
}
