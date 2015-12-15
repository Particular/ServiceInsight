namespace ServiceInsight.Framework.Rx
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using Caliburn.Micro;

    public partial class RxConductor<T>
    {
        public partial class Collection
        {
            public class AllActive : RxConductorBase<T>
            {
                readonly BindableCollection<T> items = new BindableCollection<T>();
                readonly bool openPublicItems;

                public AllActive(bool openPublicItems)
                    : this()
                {
                    this.openPublicItems = openPublicItems;
                }

                public AllActive()
                {
                    items.CollectionChanged += (s, e) =>
                    {
                        switch (e.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                e.NewItems.OfType<IChild>().Apply(x => x.Parent = this);
                                break;

                            case NotifyCollectionChangedAction.Remove:
                                e.OldItems.OfType<IChild>().Apply(x => x.Parent = null);
                                break;

                            case NotifyCollectionChangedAction.Replace:
                                e.NewItems.OfType<IChild>().Apply(x => x.Parent = this);
                                e.OldItems.OfType<IChild>().Apply(x => x.Parent = null);
                                break;

                            case NotifyCollectionChangedAction.Reset:
                                items.OfType<IChild>().Apply(x => x.Parent = this);
                                break;
                        }
                    };
                }

                public IObservableCollection<T> Items
                {
                    get { return items; }
                }

                protected override void OnActivate()
                {
                    items.OfType<IActivate>().Apply(x => x.Activate());
                }

                protected override void OnDeactivate(bool close)
                {
                    items.OfType<IDeactivate>().Apply(x => x.Deactivate(close));
                    if (close)
                    {
                        items.Clear();
                    }
                }

                public override void CanClose(Action<bool> callback)
                {
                    CloseStrategy.Execute(items, (canClose, closable) =>
                    {
                        if (!canClose && closable.Any())
                        {
                            closable.OfType<IDeactivate>().Apply(x => x.Deactivate(true));
                            items.RemoveRange(closable);
                        }

                        callback(canClose);
                    });
                }

                protected override void OnInitialize()
                {
                    if (openPublicItems)
                    {
                        GetType().GetProperties()
                            .Where(x => x.Name != "Parent" && typeof(T).IsAssignableFrom(x.PropertyType))
                            .Select(x => x.GetValue(this, null))
                            .Cast<T>()
                            .Apply(ActivateItem);
                    }
                }

                public override void ActivateItem(T item)
                {
                    if (item == null)
                    {
                        return;
                    }

                    item = EnsureItem(item);

                    if (IsActive)
                    {
                        ScreenExtensions.TryActivate(item);
                    }

                    OnActivationProcessed(item, true);
                }

                public override void DeactivateItem(T item, bool close)
                {
                    if (item == null)
                    {
                        return;
                    }

                    if (close)
                    {
                        CloseStrategy.Execute(new[] { item }, (canClose, closable) =>
                        {
                            if (canClose)
                                CloseItemCore(item);
                        });
                    }
                    else
                    {
                        ScreenExtensions.TryDeactivate(item, false);
                    }
                }

                public override IEnumerable<T> GetChildren()
                {
                    return items;
                }

                void CloseItemCore(T item)
                {
                    ScreenExtensions.TryDeactivate(item, true);
                    items.Remove(item);
                }

                protected override T EnsureItem(T newItem)
                {
                    var index = items.IndexOf(newItem);

                    if (index == -1)
                    {
                        items.Add(newItem);
                    }
                    else
                    {
                        newItem = items[index];
                    }

                    return base.EnsureItem(newItem);
                }
            }
        }
    }
}