#region License

// Copyright © 2013-2014 Avengers UTD - Adalberto L. Simeone
//
// The Odyssey Engine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License Version 3 as published by
// the Free Software Foundation.
//
// The Odyssey Engine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details at http://gplv3.fsf.org/

#endregion License

#region Using Directives

using Odyssey.Content;
using Odyssey.Engine;
using SharpDX;
using SharpDX.DirectWrite;

#endregion Using Directives

namespace Odyssey.UserInterface.Style
{
    public static class Extensions
    {
        //public static TextFormat ToTextFormat(this TextDescription tDesc, IServiceRegistry services)
        //{
        //    var deviceService = services.GetService<IDirect2DService>();
        //    var content = services.GetService<IAssetProvider>();
        //    TextFormat textFormat;
        //    if (content.Contains(tDesc.FontFamily))
        //        textFormat = new TextFormat(deviceService.Direct2DDevice, tDesc.FontFamily, services.GetService<IStyleService>().FontCollection,
        //            (SharpDX.DirectWrite.FontWeight)tDesc.FontWeight, (SharpDX.DirectWrite.FontStyle)tDesc.FontStyle,FontStretch.Normal, tDesc.Size);
        //    else textFormat = new TextFormat(deviceService.Direct2DDevice, tDesc.FontFamily, (SharpDX.DirectWrite.FontWeight) tDesc.FontWeight,
        //        (SharpDX.DirectWrite.FontStyle) tDesc.FontStyle, tDesc.Size);

        //    textFormat.TextAlignment = (SharpDX.DirectWrite.TextAlignment) tDesc.TextAlignment;
        //    textFormat.ParagraphAlignment = (SharpDX.DirectWrite.ParagraphAlignment) tDesc.ParagraphAlignment;
        //    return textFormat;
        //}
    }
}