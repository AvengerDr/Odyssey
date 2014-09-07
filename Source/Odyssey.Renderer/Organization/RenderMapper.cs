using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Odyssey.Graphics.Models;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos;

namespace Odyssey.Organization
{
    public class RenderMapper : IEnumerable<RenderableInstruction>
    {
        private readonly List<Tuple<Technique, Model, IEntity>> tuples;
        private List<RenderableInstruction> instructions;


        public RenderMapper()
        {
            tuples = new List<Tuple<Technique, Model, IEntity>>();
            instructions = new List<RenderableInstruction>();
        }

        public void Clear()
        {
            instructions.Clear();
        }

        public void AddEntity(Technique technique, Model model, IEntity entity)
        {
            Contract.Requires<ArgumentNullException>(technique != null, "effect");
            Contract.Requires<ArgumentNullException>(model != null, "model");
            Contract.Requires<ArgumentNullException>(entity != null, "entity");
            tuples.Add(new Tuple<Technique, Model, IEntity>(technique, model, entity));
        }

        public void Group()
        {
            var effectGroup = from t in tuples
                group t by new {Effect = t.Item1, Model = t.Item2}
                into gEffectModelEntities
                select new
                {
                    Effect = gEffectModelEntities.Key.Effect, 
                    Model = gEffectModelEntities.Key.Model,
                    Entities = (from t in gEffectModelEntities
                               select t.Item3).ToList()
                };

            instructions = effectGroup.Select(v => new RenderableInstruction(v.Effect, v.Model, v.Entities)).ToList();
            tuples.Clear();
        }

        public IEnumerator<RenderableInstruction> GetEnumerator()
        {
            return instructions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}