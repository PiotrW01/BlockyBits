#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

// Properties you can use from C# code
float4x4 World;
float4x4 View;
float4x4 Projection;
float3 GridPos;
//Texture2D tex;
sampler2D tex : register(s0);
SamplerState Sampler : register(s1);

cbuffer Time : register(b1)
{
    float Time;
}

cbuffer LightBuffer : register(b2)
{
    float3 LightDirection; // Direction of the light (normalized)
    float4 LightColor; // Color of the light (RGBA)
    float4 AmbientColor; // Ambient color
};

// Required attributes of the input vertices
struct VertexShaderInput
{
    float3 Position : POSITION0;
    float4 Normals : NORMAL0;
    float4 ScreenPos : TEXCOORD0;
};

// Semantics for output of vertex shader / input of pixel shader
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 ScreenPos : TEXCOORD0;
};

float Noise(float2 coord)
{
    float n = sin(dot(coord, float2(12.9898, 78.233))) * 43758.5453;
    return frac(n); // Use the fractional part to get a value between 0 and 1
}

// Actual shaders
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    float offset = sin((GridPos.x + input.Position.x, GridPos.z + input.Position.z) * Time / 2.0);
    input.Position.y -= abs(offset) * 0.00001;
    
    float4 worldPosition = mul(float4(input.Position.xyz, 1.0), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.Normal = normalize(input.Normals);
    output.ScreenPos = output.Position;
    
    
    
    
    return output;
}

float2 RotateUV(float2 uv, float angle)
{
    float s = sin(angle);
    float c = cos(angle);
    
    float2x2 rotationMatrix = float2x2(c, -s, s, c);
    return mul(rotationMatrix, uv - 0.5) + 0.5;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{   
    float2 pixelCoords = input.ScreenPos.yx / input.ScreenPos.w; // Perspective divide
    pixelCoords = pixelCoords * 0.5 + 0.5;
    pixelCoords = RotateUV(pixelCoords, radians(-90));
    //float4 noiseColor = tex2D(noise, pixelCoords + 0.05 * Time);
    //pixelCoords += 0.2 * noiseColor.rr;
    pixelCoords = clamp(pixelCoords, 0.0, 1.0);
    //float4 behind = tex.Sample(Sampler, pixelCoords);
    // Final color is a combination of ambient and diffuse lighting
    float4 water = float4(0.0, 0.0, 0.8, 0.6);
    float4 behind = tex2D(tex, pixelCoords);
    behind.g = 0.0;
    return behind;
    //return lerp(tex2D(tex, pixelCoords), water, 0.6);
}

// Technique and passes within the technique
technique ColorEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}