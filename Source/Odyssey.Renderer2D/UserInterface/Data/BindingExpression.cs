#region Using Directives

using Odyssey.Utilities.Reflection;
using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reflection;

#endregion Using Directives

namespace Odyssey.UserInterface.Data
{
    public class BindingExpression
    {
        private readonly Binding binding;
        private readonly object target;
        private readonly PropertyInfo targetProperty;
        private PropertyInfo sourceProperty;
        private object value;

        public BindingExpression(Binding binding, object target, string targetProperty)
        {
            Contract.Requires<ArgumentNullException>(binding != null, "binding");
            Contract.Requires<ArgumentNullException>(target != null, "target");
            var targetType = target.GetType();
            if (!ReflectionHelper.ContainsProperty(targetType, targetProperty))
                throw new InvalidOperationException(string.Format("Target object '{0}' does not contain a '{1}' property",
                    targetType.Name,
                    targetProperty));
            this.binding = binding;
            this.target = target;
            this.targetProperty = targetType.GetRuntimeProperty(targetProperty);
        }

        internal event EventHandler<BindingValueChangedEventArgs> ValueChanged;

        public Binding SourceBinding
        {
            get { return binding; }
        }

        public string SourcePropertyName
        {
            get { return sourceProperty.Name; }
        }

        public string TargetPropertyName
        {
            get { return targetProperty.Name; }
        }

        internal object Value
        {
            get { return value; }
            set
            {
                ChangeValue(value, true);
                Dirty();
            }
        }

        private object SourceValue
        {
            get { return sourceProperty != null ? sourceProperty.GetValue(binding.Source) : binding.Source.ToString(); }
        }

        private object TargetValue
        {
            get { return targetProperty.GetValue(target); }
        }

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(binding.Path))
                sourceProperty = binding.Source.GetType().GetRuntimeProperty(binding.Path);
            Value = SourceValue;

            var iNotifyPropertyChanged = binding.Source as INotifyPropertyChanged;
            
            if (iNotifyPropertyChanged != null)
                iNotifyPropertyChanged.PropertyChanged += UpdateFromSource;
        }

        public void UpdateTarget()
        {
            targetProperty.SetValue(target, value);
        }

        internal void ChangeValue(object newValue, bool notify)
        {
            object oldValue = value;

            value = newValue;
            if (notify && ValueChanged != null)
                ValueChanged(this, new BindingValueChangedEventArgs(oldValue, newValue));
        }

        private void Dirty()
        {
            if (binding.UpdateSourceTrigger == UpdateSourceTrigger.PropertyChanged)
                UpdateTarget();
        }

        private void UpdateFromSource(object sender, PropertyChangedEventArgs e)
        {
            if (!string.Equals(e.PropertyName, binding.Path))
                Value = SourceValue;
        }
    }
}