#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

// Properties you can use from C# code
sampler2D tex1;
sampler2D tex2;

// Required attributes of the input vertices
struct VertexShaderInput
{
    float3 Position : POSITION0;
    float4 Normals : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

// Semantics for output of vertex shader / input of pixel shader
struct VertexShaderOutput
{
    float4 Position : SV_Position;
    float3 Normal : TEXCOORD2;
    float2 TexCoord : TEXCOORD0;
    float3 Pos : TEXCOORD1;
};

// Actual shaders
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Pos = input.Position;
    output.Normal = normalize(input.Normals);
    output.TexCoord = input.TexCoord;
    
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{   
    float4 color1 = tex2D(tex1, input.TexCoord);
    float4 color2 = tex2D(tex2, input.TexCoord);
    
    return float4(1.0,1.0,0.8,0.8);
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