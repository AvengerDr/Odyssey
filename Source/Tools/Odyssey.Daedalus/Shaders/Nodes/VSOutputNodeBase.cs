﻿using Odyssey.Daedalus.Shaders.Structs;
using Odyssey.Engine;
using Odyssey.Graphics.Shaders;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.Text;
using Odyssey.Serialization;

namespace Odyssey.Daedalus.Shaders.Nodes
{
    [DataContract]
    public abstract class VSOutputNodeBase : NodeBase
    {
        [DataMember]
        [SupportedType(Type.Float4)]
        public INode Position { get; set; }
       
        [DataMember]
        [SupportedType(Type.Struct)]
        public override IVariable Output { get; set; }

        public override IEnumerable<INode> DescendantNodes
        {
            get
            {
                yield return Position;
                foreach (var node in Position.DescendantNodes)
                    yield return node;
            }
        }

        protected VSOutputNodeBase()
        {
            IsVerbose = true;
        }

        public override void Validate(TechniqueKey key)
        {
            Contract.Requires<NullReferenceException>(Output != null);
            base.Validate(key);
        }

        public override string Operation(ref int indentation)
        {
            Struct vsOutput = (Struct)Output;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("\t{0} {1};", vsOutput.Type, vsOutput.Name));
            sb.AppendLine(string.Format("\t{0} = {1};", vsOutput[Param.SemanticVariables.Position], Position.Access()));
            //sb.AppendLine(string.Format("\t{0} = {1};", vsOutput[Param.SemanticVariables.WorldPosition], WorldPosition.Access()));
            sb.AppendLine(string.Format("\treturn {0};", vsOutput.Name));

            return sb.ToString();
        }

        public override string Access()
        {
            return string.Empty;
        }

        protected override void RegisterNodes()
        {
            AddNode("Position", Position);
        }
    }
}
