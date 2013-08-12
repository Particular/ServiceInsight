using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Caliburn.PresentationFramework.Screens;
using Particular.ServiceInsight.Desktop.MessageHeaders.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    public class HeaderInfoPropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyInfo _property;
        private readonly Type _owner;

        public HeaderInfoPropertyDescriptor(PropertyInfo property, Type owner)
            : base(property.Name, new Attribute[0])
        {
            _owner = owner;
            _property = property;
        }

        protected override AttributeCollection CreateAttributeCollection()
        {
            var attribs = new List<Attribute>();
            var attributes = _property.GetCustomAttributes(false).OfType<Attribute>().ToArray();
            var editor = attributes.OfType<EditorAttribute>().FirstOrDefault();
            var isScreen = typeof (IScreen).IsAssignableFrom(PropertyType);

            if (attributes.Length > 0)
            {
                attribs.AddRange(attributes);
            }

            if (isScreen)
            {
                attribs.Add(new ExpandableObjectAttribute());
            }
            
            if (editor == null && isScreen)
            {
                attribs.Add(new EditorAttribute(typeof(EmptyTypeEditor), typeof(EmptyTypeEditor)));
            }
            else if(editor == null)
            {
                attribs.Add(new EditorAttribute(typeof(SelectableTextBoxEditor), typeof(SelectableTextBoxEditor)));
            }

            return new AttributeCollection(attribs.ToArray());
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object GetValue(object component)
        {
            return component;
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return _owner; }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return _property.PropertyType; }
        }

        public override string DisplayName
        {
            get
            {
                return _property.Name;
            }
        }
    }
}