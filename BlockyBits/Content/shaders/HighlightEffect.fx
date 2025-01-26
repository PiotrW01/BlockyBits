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
float4 Color;
float3 GridPos;
sampler2D original : register(s0);
sampler2D overlay : register(s1);
float2 texCoord : TEXCOORD0;

// Required attributes of the input vertices
struct VertexShaderInput
{
    float3 Position : POSITION;
    float4 Normals : NORMAL0;
};

// Semantics for output of vertex shader / input of pixel shader
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 ScreenPos : TEXCOORD0;
    float3 Pos : TEXCOORD1;
};

// Actual shaders
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Pos = input.Position;
    input.Position += input.Normals * 0.05;
    float4 worldPosition = mul(float4(input.Position.xyz, 1.0), World);
    float4 viewPosition = mul(worldPosition, View);

    
    output.Position = mul(viewPosition, Projection);
    output.ScreenPos = mul(viewPosition, Projection);
    
    
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{   
    // Compute normalized screen coordinates (NDC space)
    float2 pixelCoords = input.ScreenPos.xy / input.ScreenPos.w; // Perspective divide
    pixelCoords = pixelCoords * 0.5 + 0.5; // Map from [-1,1] to [0,1]
    if (input.Pos.x < GridPos.x || input.Pos.x > GridPos.x + 1 || input.Pos.y < GridPos.y || input.Pos.y > GridPos.y + 1 ||
       input.Pos.z < GridPos.z || input.Pos.z > GridPos.z + 1)
        return tex2D(original, texCoord);
    
    float4 originalColor = tex2D(original, texCoord);
    float4 overlayColor = tex2D(overlay, texCoord);
    
    //return lerp(originalColor, overlayColor, float4(0.5,0.5,0.5,0.5)); // Debug: Return screen UV coordinates as color
    return float4(0.0,0.0,0.0,1.0);
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