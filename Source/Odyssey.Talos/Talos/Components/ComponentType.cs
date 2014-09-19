using System.Diagnostics.Contracts;

namespace Odyssey.Talos.Components
{
    public sealed class ComponentType
    {
        static long nextBit = 1;
        static long nextId = 0;

        public long KeyPart { get; private set; }
        public long Id { get; private set; }

        public ComponentType()
        {
            Contract.Ensures(nextId < 64, "Maximum number of components reached.");
            this.Id = nextId;
            this.KeyPart = nextBit;

            nextId++;
            nextBit <<= 1;
        }

    }
}
