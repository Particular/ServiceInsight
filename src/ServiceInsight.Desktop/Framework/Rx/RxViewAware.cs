namespace Particular.ServiceInsight.Desktop.Framework.Rx
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Caliburn.Micro;
    using ReactiveUI;

    public class RxViewAware : ReactiveObject, IViewAware
    {
        bool cacheViews;

        static readonly DependencyProperty PreviouslyAttachedProperty = DependencyProperty.RegisterAttached(
            "PreviouslyAttached",
            typeof(bool),
            typeof(RxViewAware),
            null
            );

        public static bool CacheViewsByDefault = true;

        protected readonly Dictionary<object, object> Views = new Dictionary<object, object>();

        public RxViewAware()
            : this(CacheViewsByDefault) { }

        public RxViewAware(bool cacheViews)
        {
            CacheViews = cacheViews;
        }

        public event EventHandler<ViewAttachedEventArgs> ViewAttached = delegate { };

        [PropertyChanged.DoNotNotify]
        protected bool CacheViews
        {
            get { return cacheViews; }
            set
            {
                cacheViews = value;
                if (!cacheViews)
                    Views.Clear();
            }
        }

        void IViewAware.AttachView(object view, object context)
        {
            if (CacheViews)
            {
                Views[context ?? View.DefaultContext] = view;
            }

            var nonGeneratedView = View.GetFirstNonGeneratedView(view);

            var element = nonGeneratedView as FrameworkElement;
            if (element != null && !(bool)element.GetValue(PreviouslyAttachedProperty))
            {
                element.SetValue(PreviouslyAttachedProperty, true);
                //View.ExecuteOnLoad(element, (s, e) => OnViewLoaded(s));
            }

            OnViewAttached(nonGeneratedView, context);
            ViewAttached(this, new ViewAttachedEventArgs { View = nonGeneratedView, Context = context });
        }

        // ReSharper disable UnusedParameter.Global
        protected virtual void OnViewAttached(object view, object context)
        // ReSharper restore UnusedParameter.Global
        {
        }

        //protected virtual void OnViewLoaded(object view)
        //{
        //}

        public virtual object GetView(object context = null)
        {
            object view;
            Views.TryGetValue(context ?? View.DefaultContext, out view);
            return view;
        }
    }
}