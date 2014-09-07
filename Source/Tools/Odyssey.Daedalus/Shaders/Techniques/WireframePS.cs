#region Using Directives

using Odyssey.Daedalus.Shaders.Methods;
using Odyssey.Daedalus.Shaders.Nodes;
using Odyssey.Daedalus.Shaders.Nodes.Functions;
using Odyssey.Daedalus.Shaders.Nodes.Math;
using Odyssey.Daedalus.Shaders.Nodes.Operators;
using Odyssey.Daedalus.Shaders.Structs;
using Odyssey.Engine;
using Odyssey.Graphics.Effects;
using Odyssey.Graphics.Shaders;
using ConstantBuffer = Odyssey.Daedalus.Shaders.Structs.ConstantBuffer;

#endregion Using Directives

namespace Odyssey.Daedalus.Shaders.Techniques
{
    public class WireframePS : Shader
    {
        public WireframePS()
        {
            Name = "WireframePS";
            Type = ShaderType.Pixel;
            KeyPart = new TechniqueKey(ps: PixelShaderFlags.Diffuse, sm: ShaderModel.SM_4_0_Level_9_3);
            FeatureLevel = FeatureLevel.PS_4_0_Level_9_3;
            EnableSeparators = true;
            var inputStruct = WireframeVS.VertexPositionTextureIntensityBarycentricOut;
            inputStruct.Name = "input";
            InputStruct = inputStruct;
            OutputStruct = Struct.PixelShaderOutput;
            Structs.ConstantBuffer cbInstance = ConstantBuffer.CBMaterial;

            Struct material = (Struct) cbInstance[Param.Struct.Material];
            Add(cbInstance);

            IVariable position = InputStruct[Param.SemanticVariables.ObjectPosition];
            IVariable diffuse = material[Param.Material.Diffuse];
            IVariable specular = material[Param.Material.Specular];
            IVariable intensity = InputStruct[Param.SemanticVariables.Intensity];
            IVariable texture = InputStruct[Param.SemanticVariables.Texture];
            IVariable barycentric = InputStruct[Param.SemanticVariables.Barycentric];

            //2
            UnaryFunctionNode fWidth = new UnaryFunctionNode
            {
                Input1 = new ReferenceNode() {Value = barycentric},
                Function = HlslIntrinsics.Fwidth,
                Output = new Vector() {Type = Shaders.Type.Float3, Name = "d"},
                IsVerbose = true
            };

            //TrinaryFunctionNode smoothstep = new TrinaryFunctionNode()
            //{
            //    Input1 = new ConstantNode() {Value = new[] {0f, 0f, 0f}},
            //    Input2 = new MultiplyNode() {Input1 = new ScalarNode() {Value = 1.5f}, Input2 = fWidth},
            //    Input3 = new ReferenceNode() {Value = barycentric},
            //    Function = HLSLIntrinsics.Smoothstep,
            //    IsVerbose = true,
            //    Output = new Vector() {  Type = Shaders.Type.Float3, Name = "a3"}
            //};

            TrinaryFunctionNode smoothstep = new TrinaryFunctionNode()
            {
                Input1 = fWidth,
                Input2 = new MultiplyNode() {Input1 = new ScalarNode() {Value = 3f}, Input2 = fWidth},
                Input3 = new ReferenceNode() {Value = barycentric},
                Function = HlslIntrinsics.Smoothstep,
                IsVerbose = true,
                Output = new Vector() {Type = Shaders.Type.Float3, Name = "a3"}
            };

            BinaryFunctionNode minDistanceXY = new BinaryFunctionNode()
            {
                Input1 = new SwizzleNode
                {
                    Input = smoothstep,
                    Swizzle = new[] {Swizzle.X}
                },
                Input2 = new SwizzleNode
                {
                    Input = smoothstep,
                    Swizzle = new[] {Swizzle.Y}
                },
                Function = HlslIntrinsics.Min,
            };
            BinaryFunctionNode edgeIntensity = new BinaryFunctionNode()
            {
                Input1 = minDistanceXY,
                Input2 = new SwizzleNode
                {
                    Input = smoothstep,
                    Swizzle = new[] {Swizzle.Z}
                },
                Function = HlslIntrinsics.Min,
                Output = new Vector() {Type = Shaders.Type.Float, Name = "minDistance"},
                IsVerbose = true
            };

            MultiplyNode objectDiffuse = new MultiplyNode()
            {
                Input1 = new ReferenceNode() {Value = diffuse},
                Input2 = new CastNode
                {
                    Input = new SwizzleNode
                    {
                        Input = new ReferenceNode {Value = intensity},
                        Swizzle = new[] {Swizzle.X, Swizzle.Y, Swizzle.Z, Swizzle.Null}
                    },
                    Mask = new[] {"0", "0", "0", "1"},
                    Output = new Vector() {Type = Shaders.Type.Float4, Name = "intensity4"}
                },
                Output = new Vector() {Type = Shaders.Type.Float4, Name = "diffuse"},
                IsVerbose = true
            };

            TrinaryFunctionNode finalColor = new TrinaryFunctionNode()
            {
                Input1 = new ReferenceNode() {Value = specular},
                Input2 = objectDiffuse,
                Input3 = edgeIntensity,
                Function = HlslIntrinsics.Lerp
            };

            Result = new PSOutputNode
            {
                FinalColor = finalColor,
                Output = OutputStruct
            };
        }

        public static Structs.ConstantBuffer CBPerFrame
        {
            get
            {
                Structs.ConstantBuffer cbFrame = new Structs.ConstantBuffer
                {
                    Name = Param.ConstantBuffer.PerFrame,
                    UpdateType = UpdateType.SceneFrame
                };
                cbFrame.Add(Matrix.CameraView);
                cbFrame.Add(Matrix.CameraProjection);
                cbFrame.Add(Vector.LightDirection);
                return cbFrame;
            }
        }

        public static Structs.ConstantBuffer CBPerInstance
        {
            get
            {
                Structs.ConstantBuffer cbInstance = ConstantBuffer.CBPerInstance;
                return cbInstance;
            }
        }
    }
}