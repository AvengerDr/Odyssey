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

namespace Odyssey.Daedalus.Shaders.Techniques
{
    public class WireframeVS : Shader
    {
        public WireframeVS()
        {
            Name = "WireframeVS";
            Type = ShaderType.Vertex;
            KeyPart = new TechniqueKey(vs: VertexShaderFlags.Position | VertexShaderFlags.Normal | VertexShaderFlags.TextureUV | VertexShaderFlags.Barycentric);
            FeatureLevel = FeatureLevel.VS_4_0_Level_9_1;
            EnableSeparators = true;
            InputStruct = VertexPositionNormalBarycentric;
            OutputStruct = VertexPositionTextureIntensityBarycentricOut;

            ConstantBuffer cbFrame = CBPerFrame;
            ConstantBuffer cbInstance = CBPerInstance;
            Add(cbFrame);
            Add(cbInstance);

            IVariable position = InputStruct[Param.SemanticVariables.ObjectPosition];
            IVariable lightDirection = cbFrame[Param.Vectors.LightDirection];
            IVariable normal = InputStruct[Param.SemanticVariables.Normal];
            IVariable texture = InputStruct[Param.SemanticVariables.Texture];
            IVariable barycentric = InputStruct[Param.SemanticVariables.Barycentric];

            CastNode nV3toV4 = new CastNode
            {
                Input = new SwizzleNode { Input = new ReferenceNode { Value = position }, Swizzle = new[] { Swizzle.X, Swizzle.Y, Swizzle.Z, Swizzle.Null} },
                Output = new Vector { Type = Shaders.Type.Float4, Name = "vPosition" },
                Mask = new[] { "0", "0", "0", "1" },
                IsVerbose = true
            };

            MatrixMultiplyNode mulPosWVP = new MatrixMultiplyNode
            {
                Input1 = nV3toV4,
                Input2 = MatrixMultiplyNode.WorldViewProjection,
                Output = new Vector() { Type = Shaders.Type.Float4, Name = "vClipPosition"},
                IsVerbose = true
            };

            MultiplyNode perspectiveCorrection = new MultiplyNode()
            {
                Input1 = new ReferenceNode() {Value = barycentric},
                Input2 = new SwizzleNode()
                    {
                        Input = mulPosWVP,
                        Swizzle = new Swizzle[] {Swizzle.W,}
                    }
            };

            BinaryFunctionNode dotLightNormal = new BinaryFunctionNode
            {
                Input1 = new ReferenceNode { Value = lightDirection },
                Input2 = new ReferenceNode { Value = normal },
                Function = HlslIntrinsics.Dot
            };

            BinaryFunctionNode maxIntensity = new BinaryFunctionNode
            {
                Input1 = dotLightNormal,
                Input2 = new ScalarNode() { Value = 0.3f },
                Function = HlslIntrinsics.Max
            };

            CustomOutputNode outputNode = new CustomOutputNode() { Output = OutputStruct };
            outputNode.RegisterField(Vector.ClipPosition, mulPosWVP, Shaders.Type.Float4);
            outputNode.RegisterField(Param.SemanticVariables.Intensity, Semantics.Intensity, maxIntensity, Shaders.Type.Float3);
            outputNode.RegisterField(Vector.TextureUV, new ReferenceNode { Value = texture }, Shaders.Type.Float2);
            outputNode.RegisterField(Param.SemanticVariables.Barycentric, Semantics.Barycentric, perspectiveCorrection, Shaders.Type.Float3);

            Result = outputNode;
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
                Structs.ConstantBuffer cbInstance = Structs.ConstantBuffer.CBPerInstance;
                return cbInstance;
            }
        }

        public static Struct VertexPositionNormalBarycentric
        {
            get
            {
                Struct vpt = new Struct
                {
                    Name = "input",
                    CustomType = CustomType.VSIn,
                };
                vpt.Add(Vector.ObjectPosition);
                vpt.Add(Vector.Normal);
                vpt.Add(Vector.TextureUV);
                vpt.Add(Vector.Barycentric);
                return vpt;
            }
        }

        public static Struct VertexPositionTextureIntensityBarycentricOut
        {
            get
            {
                Struct output = new Struct
                {
                    CustomType = CustomType.VSOut,
                    Name = "output",
                };
                output.Add(Vector.ClipPosition);
                output.Add(Vector.Intensity);
                output.Add(Vector.TextureUV);
                output.Add(Vector.Barycentric);
                return output;
            }
        }
    }

}