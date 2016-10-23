namespace ServiceInsight.Framework.Rx
{
    using System;
    using System.Linq.Expressions;
    using System.Windows;
    using Pirac;

    public class RxScreen : ViewModelBase
    {
        public string DisplayName { get; set; }

        protected override void OnViewAttached(FrameworkElement view)
        {
            base.OnViewAttached(view);
            OnViewAttached(view, null);
        }

        [Obsolete("Old CM Method")]
        protected void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
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
        protected void NotifyOfPropertyChange(string propertyName)
        {
            OnPropertyChanging(propertyName, null);
            OnPropertyChanged(propertyName, null, null);
        }

        protected override void OnViewLoaded(FrameworkElement view)
        {
            base.OnViewLoaded(view);
            OnViewLoaded((object)view);
        }

        protected override void OnActivate(bool wasInitialized)
        {
            base.OnActivate(wasInitialized);
            OnActivate();
        }

        [Obsolete("Old CM Method")]
        protected virtual void OnViewAttached(object view, object context)
        {
        }

        [Obsolete("Old CM Method")]
        protected virtual void OnViewLoaded(object view)
        {
        }

        [Obsolete("Old CM Method")]
        protected virtual void OnActivate()
        {
        }
    }
}