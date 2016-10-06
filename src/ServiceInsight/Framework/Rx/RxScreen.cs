namespace ServiceInsight.Framework.Rx
{
    using System;
    using System.Linq.Expressions;
    using System.Windows;
    using Caliburn.Micro;
    using Pirac;

    public class RxScreen : ViewModelBase, IScreen, IViewAware
    {
        public string DisplayName { get; set; }

        public bool IsNotifying
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<ActivationEventArgs> Activated;

        public event EventHandler<DeactivationEventArgs> AttemptingDeactivation;

        public event EventHandler<DeactivationEventArgs> Deactivated;

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;

        [Obsolete("Old CM Method")]
        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            NotifyOfPropertyChange(memberExpression.Member.Name);
        }

        [Obsolete("Old CM Method")]
        public void NotifyOfPropertyChange(string propertyName)
        {
            OnPropertyChanging(propertyName, null);
            OnPropertyChanged(propertyName, null, null);
        }

        protected override void OnActivate(bool wasInitialized)
        {
            base.OnActivate(wasInitialized);
            OnActivate();
            Activated?.Invoke(this, new ActivationEventArgs());
        }

        [Obsolete("Old CM Method")]
        protected virtual void OnViewAttached(object view, object context)
        {
            base.OnViewAttached((FrameworkElement)view);
        }

        [Obsolete("Old CM Method")]
        protected virtual void OnViewLoaded(object view)
        {
            base.OnViewLoaded((FrameworkElement)view);
        }

        [Obsolete("Old CM Method")]
        protected virtual void OnActivate()
        {
        }

        [Obsolete("Old CM Method")]
        public void CanClose(Action<bool> callback)
        {
            callback(CanClose(base.GetView()));
        }

        [Obsolete("Old CM Method")]
        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void AttachView(object view, object context = null)
        {
            OnViewAttached(view, context);
            ((IHaveView)this).AttachView((FrameworkElement)view);
        }

        public object GetView(object context = null)
        {
            return base.GetView();
        }
    }
}