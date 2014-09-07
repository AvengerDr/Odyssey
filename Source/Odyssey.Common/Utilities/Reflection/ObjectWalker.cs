using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Odyssey.Utilities.Reflection
{
    public class ObjectWalker
    {
        internal delegate object ReadMethod();

        internal delegate void WriteMethod(object value);
        private object root;

        private object objectValue;
        private readonly LinkedList<WalkerInstruction> instructions;

        public string Name
        {
            get { return root.GetType().Name; }
        }

        public MemberInfo CurrentMember { get; private set; }

        public ObjectWalker()
        {
        }

        public ObjectWalker(object obj, string path) : this(obj)
        {
            FollowPath(path);
        }

        public object Root { get { return root; } }

        internal LinkedListNode<WalkerInstruction> LastInstruction { get { return instructions.Last; } }
        internal LinkedListNode<WalkerInstruction> FirstInstruction { get { return instructions.First; } }

        public ObjectWalker(object obj)
        {
            Contract.Requires<ArgumentNullException>(obj != null, "obj");
            instructions = new LinkedList<WalkerInstruction>();
            Reset(obj);
        }

        void Reset(object obj)
        {
            CurrentMember = null;
            root = obj;
            objectValue = root;
            instructions.Clear();
        }

        public void SetTarget(object obj, string path)
        {
            Reset(obj);
            FollowPath(path);
        }

        bool ShouldAdvance(MemberInfo member)
        {
            if (member != null)
            {
                CurrentMember = member;
                return true;
            }
            else return false;
        }

        void FollowPath(string propertyPath)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(propertyPath), "Path cannot be null");
            foreach (string subPath in propertyPath.Split('.'))
            {
                string expression = subPath;
                string arrayName;

                int index;
                if (Text.Text.IsExpressionArray(subPath, out arrayName, out index))
                {
                    expression = arrayName;
                }

                if (!AdvanceTo(expression, index))
                    throw new InvalidOperationException(string.Format("Type '{0}' does not contain a property or field '{1}'", Name, expression));

            }
        }


        bool AdvanceTo(string memberName, int index = -1)
        {
            return AdvanceToProperty(memberName, index) || AdvanceToField(memberName, index);
        }

        bool AdvanceToProperty(string propertyName, int index = -1)
        {
            var member = ReflectionHelper.GetProperty(objectValue.GetType(), propertyName);
            bool result = ShouldAdvance(member);
            if (result)
            {
                var instruction = new WalkerInstruction()
                {
                    Object = objectValue,
                    Property = member,
                    Field = null,
                    ReadMethod = ReadPropertyValue,
                    WriteMethod = WritePropertyValue,
                    Index = index
                };

                instructions.AddLast(instruction);
                
                UpdateValue();

                if (instruction.Index >= 0 && instruction.Property.GetIndexParameters().Length == 0)
                {
                    AdvanceToProperty("Item", index);
                }
            }
            return result;
        }

        bool AdvanceToField(string fieldName, int index = -1)
        {
            var member = ReflectionHelper.GetField(objectValue.GetType(), fieldName);
            bool result = ShouldAdvance(member);
            if (result)
            {
                var instruction = new WalkerInstruction()
                {
                    Object = objectValue,
                    Property = null,
                    Field = member,
                    ReadMethod = ReadFieldValue,
                    WriteMethod = WriteFieldValue,
                    Index = index
                };

                instructions.AddLast(instruction);

                UpdateValue();
            }
            return result;
        }

        object ReadPropertyValue()
        {
            var instruction = instructions.Last.Value;
            return instruction.Index >= 0 && instruction.Property.GetIndexParameters().Length > 0
                ? instruction.Property.GetValue(instruction.Object, new object[] { instruction.Index })
                : instruction.Property.GetValue(instruction.Object);
        }

        void WritePropertyValue(object value)
        {
            var instruction = instructions.Last.Value;
            WritePropertyValue(instruction.Property, instruction.Object, value, instruction.Index);
        }

        static void WritePropertyValue(PropertyInfo property, object parent, object value, int index = -1)
        {
            if (index >= 0 && property.GetIndexParameters().Length > 0)
                property.SetValue(parent, value, new object[index]);
            else
                property.SetValue(parent, value);
        }

        static void WriteFieldValue(FieldInfo field, object parent, object value, int index = -1)
        {
            if (index >= 0)
            {
                Array array = (Array) field.GetValue(parent);
                array.SetValue(value, index);
            }
            else
                field.SetValue(parent, value);
        }

        void WriteFieldValue(object value)
        {
            var lastInstruction = instructions.Last.Value;
            lastInstruction.Field.SetValue(lastInstruction.Object, value);

            if (lastInstruction.Field.DeclaringType.GetTypeInfo().IsValueType)
            {
                var previousInstruction = instructions.Last.Previous.Value;
                if (previousInstruction.IsProperty)
                {
                    WritePropertyValue(previousInstruction.Property, previousInstruction.Object, value, previousInstruction.Index);
                }
                else
                {
                    WriteFieldValue(previousInstruction.Field, previousInstruction.Object, value, previousInstruction.Index);
                }
            }
        }

        object ReadFieldValue()
        {
            var instruction = instructions.Last.Value;
            var value = instruction.Field.GetValue(instruction);

            if (instruction.Index>= 0)
            {
                Array array = (Array) value;
                value = array.GetValue(new[] { instruction.Index });
            }
            return value;
        }

        object Read()
        {
            return instructions.Last.Value.ReadMethod();
        }

        void UpdateValue()
        {
            objectValue = Read();
        }

        public TValue ReadValue<TValue>()
        {
            return (TValue) Read();
        }

        public void WriteValue<TValue>(TValue value)
        {
            instructions.Last.Value.WriteMethod(value);
        }

        internal struct WalkerInstruction
        {
            public bool IsProperty { get { return Property != null; }}
            public ReadMethod ReadMethod;
            public WriteMethod WriteMethod;
            public PropertyInfo Property;
            public FieldInfo Field;
            public object Object;
            public int Index;
        }
    }
}
