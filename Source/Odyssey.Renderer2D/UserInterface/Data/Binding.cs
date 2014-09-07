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

namespace Odyssey.UserInterface.Data
{
    public class Binding
    {
        public Binding() : this(string.Empty, string.Empty)
        {
        }

        public Binding(string targetElement, string path)
        {
            Path = path;
            TargetElement = targetElement;
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }

        public string Path { get; set; }

        public object Source { get; set; }

        public string TargetElement { get; set; }

        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
    }
}