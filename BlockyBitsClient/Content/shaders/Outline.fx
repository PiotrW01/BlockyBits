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
    float3 Pos : POSITION1;
};




// Actual shaders
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Pos = input.Position;
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
    float min = 0.05;
    float max = 1.0 - 0.05;
    float x = input.Pos.x;
    float y = input.Pos.y;
    float z = input.Pos.z;
        // Check if the point is inside the unit cube
        // Check if the point is on the edges (not on the faces)
        bool is_edge = false;

        // Check along the x axis
        if (min < x && x < max &&
            (y <= min || y >= 1.0 - min) &&
            (z <= min || z >= 1.0 - min))
        {
            is_edge = true;
        }
        
        // Check along the y axis
        if (min < y && y < max &&
            (x <= min || x >= 1.0 - min) &&
            (z <= min || z >= 1.0 - min))
        {
            is_edge = true;
        }
        
        // Check along the z axis
        if (min < z && z < max &&
            (x <= min || x >= 1.0 - min) &&
            (y <= min || y >= 1.0 - min))
        {
            is_edge = true;
        }
    
    
        if (is_edge)
        {
            return float4(1.0, 1.0, 1.0, 1.0);
        }
        else
        {
            discard;
        }
    return float4(1.0, 1.0, 1.0, 1.0);
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