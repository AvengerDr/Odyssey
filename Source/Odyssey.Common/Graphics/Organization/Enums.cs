using System;

namespace Odyssey.Graphics.Organization
{

    public enum CommandType
    {
        Undefined,
        Render,
        RasterizerStateChange,
        BlendStateChange,
        DepthStencilStateChange,
        PostProcessing,
        PlayAnimation,
        Render2D,
    }

    public enum PreferredRasterizerState
    {
        CullBack,
        Wireframe
    }

    public enum PreferredBlendState
    {
        Opaque,
        Additive,
        /// <summary>
        /// SourceBlend = SourceAlpha/1, DestBlend = InvSourceAlpha/1, Op = Add
        /// </summary>
        AlphaBlend 
    }

    public enum PreferredDepthStencilState
    {
        Enabled,
        EnabledComparisonLessEqual,
        None,
    }

    [Flags]
    public enum InstanceSemantic
    {
        None = 0,
        World = 1,
        Color = 2,
    }

}
