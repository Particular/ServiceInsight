using System;
using System.Linq;

namespace Particular.ServiceInsight.Desktop.Framework.Attributes
{
    internal interface IAttachment
    {
        void AttachTo(object obj);
    }

    public abstract class Attachment<T> : IAttachment
    {
        protected T instance;

        protected abstract void OnAttach();

        void IAttachment.AttachTo(object obj)
        {
            instance = (T)obj;
            OnAttach();
        }
    }
}