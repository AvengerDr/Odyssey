using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Odyssey.UserInterface.Data
{
    public class BindingCollection : IEnumerable<KeyValuePair<string,Binding>>
    {
        private readonly Dictionary<string, Binding> bindings;

        internal BindingCollection()
        {
            bindings = new Dictionary<string, Binding>();
        }

        [Pure]
        public bool ContainsBinding(string property)
        {
            return bindings.ContainsKey(property);
        }

        public void Add(string property, Binding binding)
        {
            bindings.Add(property, binding);
        }

        #region IEnumerable<Binding>
        public IEnumerator<KeyValuePair<string, Binding>> GetEnumerator()
        {
            return bindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

    }
}
