#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

// Properties you can use from C# code
float4x4 World;
float4x4 View;
float4x4 Projection;
float3 GridPos;
sampler2D tex;
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

// Actual shaders
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    float offset = sin(Time + GridPos.x + GridPos.z + input.Position.x + input.Position.z) / 2;
    input.Position.y -= abs(offset);
    
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
    pixelCoords = clamp(pixelCoords, 0.0, 1.0);
    //pixelCoords.xy = pixelCoords.yx;
    // Normalize the normal
    float3 normal = normalize(input.Normal);
    // Calculate the diffuse lighting component (Lambertian reflection)
    float diffuse = max(dot(normal, -LightDirection), 0.0f); // Dot product with light direction (lightDirection is already normalized)

    // Calculate the final color by combining ambient light, diffuse light, and the light's color
    float4 diffuseColor = LightColor * diffuse;
    float4 ambientColor = AmbientColor * float4(0.0,0.0,1.0,0.7);
    // Final color is a combination of ambient and diffuse lighting
    float4 water = float4(0.0, 0.0, 0.8, 0.6);
    float4 behind = tex2D(tex, RotateUV(pixelCoords, radians(-90)));
    behind.gb = float2(0.0, 0.0);
    return lerp(behind, water, 0.2);
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