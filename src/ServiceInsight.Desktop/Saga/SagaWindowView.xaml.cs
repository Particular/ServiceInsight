namespace Particular.ServiceInsight.Desktop.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public partial class SagaWindowView : ISagaWindowView
    {
        public SagaWindowView()
        {
            InitializeComponent();
            DataContextChanged += SagaWindowView_DataContextChanged;
        }

        void SagaWindowView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dataContext = (e.NewValue as ISagaWindowViewModel);
            if (dataContext != null)
            {
                dataContext.PropertyChanged += SagaWindowView_PropertyChanged;
            }
        }

        public void Initialize()
        {
        }

        bool refreshVisual;

        void SagaWindowView_LayoutUpdated(object sender, EventArgs e)
        {
            if (refreshVisual)
            {
                RefreshAll();
                refreshVisual = false;
            }
        }

        void SagaWindowView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowEndpoints" || e.PropertyName == "Data" || e.PropertyName == "ShowMessageData")
            {
                refreshVisual = true;
            }
        }

        void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshAll();
        }

        void RefreshAll()
        {
            var steps = (ItemsControl)FindName("Steps");
            for (var i = 0; i < steps.Items.Count; i++)
            {
                var stepsContainer = steps.ItemContainerGenerator.ContainerFromIndex(i);
                if (stepsContainer != null && VisualTreeHelper.GetChildrenCount(stepsContainer) > 0)
                {
                    var item = (StackPanel)(((Grid)VisualTreeHelper.GetChild(stepsContainer, 0))).Children[0];
                    DrawLines(item, ((ISagaWindowViewModel)DataContext).ShowEndpoints);
                }
            }
        }

        void DrawLines(StackPanel panel, bool showEndpoints)
        {
            if (panel == null) return;
            
            var endpointHeight = showEndpoints ? -14 : 0;
            var caption = (FrameworkElement)panel.FindName("StepName");
            var captionPosition = GetPosition("StepName", panel);
            var message = (FrameworkElement)panel.FindName("InitialMessage");
            var messagePosition = GetPosition("InitialMessage", panel);
            var messageData = ((FrameworkElement)((ContentPresenter)message).ContentTemplate.FindName("MessageDataPanel", message));

            var messageDataHeight = messageData.Visibility == Visibility.Visible ? messageData.ActualHeight : 0;

            var parent = panel.Parent as Grid;
            RemoveExistingLines(parent);

            AddLine(new Point(messagePosition.X, message.ActualHeight + endpointHeight - messageDataHeight), new Point(captionPosition.X + caption.ActualWidth, message.ActualHeight + endpointHeight - messageDataHeight), parent);

            var stepName = (Panel)panel.FindName("StepName");
            var icon = stepName.Children.Cast<FrameworkElement>().OfType<ContentControl>().FirstOrDefault(c => c.Visibility == Visibility.Visible);
            var iconPosition = icon.TransformToAncestor(panel).Transform(new Point(0, 0));
            var lastPoint = new Point(iconPosition.X + icon.ActualWidth / 2, message.ActualHeight + endpointHeight - messageDataHeight);
            var timeoutPoint = new Point(0, 0);

            var timeoutMessages = (ItemsControl)panel.FindName("TimeoutMessages");
            for (var i = 0; i < timeoutMessages.Items.Count; i++)
            {
                var timeout = VisualTreeHelper.GetChild(timeoutMessages.ItemContainerGenerator.ContainerFromIndex(i), 0) as Panel;
                var timeoutMessage = timeout.FindName("TimeoutMessage") as FrameworkElement;
                var timeoutMessagePosition = timeoutMessage.TransformToAncestor(panel).Transform(new Point(0, 0));

                AddTimeoutVerticalLine(timeout, panel, parent, ref lastPoint, ref timeoutPoint, i);
                AddLine(timeoutPoint, new Point(timeoutPoint.X, timeoutPoint.Y + 12), parent);
                AddLine(new Point(timeoutPoint.X, timeoutPoint.Y + 12), new Point(timeoutMessagePosition.X + timeoutMessage.ActualWidth / 2, timeoutPoint.Y + 12), parent);
                AddLine(new Point(timeoutMessagePosition.X + timeoutMessage.ActualWidth / 2, timeoutPoint.Y + 12), new Point(timeoutMessagePosition.X + timeoutMessage.ActualWidth / 2, timeoutMessagePosition.Y + (endpointHeight + 14)), parent);
                AddArrow(parent, new Point(timeoutMessagePosition.X + timeoutMessage.ActualWidth / 2, timeoutMessagePosition.Y + (endpointHeight + 14)));
            }

            var sagaMessagesPosition = GetPosition("SagaMessages", panel);
            var sagaMessages = (ItemsControl)panel.FindName("SagaMessages");
            if (sagaMessages.Items.Count > 0)
            {
                AddLine(new Point(captionPosition.X + caption.ActualWidth, message.ActualHeight + endpointHeight - messageDataHeight), new Point(sagaMessagesPosition.X + (sagaMessages.ActualWidth / 2), message.ActualHeight + endpointHeight - messageDataHeight), parent);
                AddLine(new Point(sagaMessagesPosition.X + (sagaMessages.ActualWidth / 2), message.ActualHeight + endpointHeight - messageDataHeight), new Point(sagaMessagesPosition.X + (sagaMessages.ActualWidth / 2), sagaMessagesPosition.Y + (endpointHeight + 14)), parent);
                AddArrow(parent, new Point(sagaMessagesPosition.X + (sagaMessages.ActualWidth / 2), sagaMessagesPosition.Y + (endpointHeight + 14)));
            }
        }

        void AddTimeoutVerticalLine(Panel timeout, StackPanel panel, Grid parent, ref Point lastPoint, ref Point timeoutPoint, int i)
        {
            var timeoutIcon = timeout.FindName("TimeoutIcon") as FrameworkElement;
            var timeoutIconPosition = timeoutIcon.TransformToAncestor(panel).Transform(new Point(0, 0));
            timeoutPoint = new Point(timeoutIconPosition.X + timeoutIcon.ActualWidth / 2, timeoutIconPosition.Y);

            AddLine(lastPoint, timeoutPoint, parent);
            timeoutPoint.Y += timeoutIcon.ActualHeight;
            lastPoint = timeoutPoint;
        }

        void AddLine(Point origin, Point destination, Panel rootCanvas)
        {
            var relationshipPath = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Visibility = Visibility.Visible
            };

            var geometry = new StreamGeometry
            {
                FillRule = FillRule.EvenOdd
            };

            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(new Point(origin.X, origin.Y), false, false);

                var midPointX = Math.Abs(origin.X - destination.X) / 2;

                if (origin.X > destination.X)
                {
                    ctx.LineTo(new Point(origin.X - midPointX, origin.Y), true, true);
                    ctx.LineTo(new Point(origin.X - midPointX, destination.Y), true, true);
                }
                else
                {
                    ctx.LineTo(new Point(origin.X + midPointX, origin.Y), true, true);
                    ctx.LineTo(new Point(origin.X + midPointX, destination.Y), true, true);
                }

                ctx.LineTo(new Point(destination.X, destination.Y), true, true);
            }

            geometry.Freeze();

            relationshipPath.Data = geometry;
            rootCanvas.Children.Add(relationshipPath);
        }

        void AddArrow(Panel rootCanvas, Point arrowPoint)
        {
            var size = 10.0;

            var relationshipPath = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Visibility = Visibility.Visible,
                Fill = Brushes.Black
            };

            var geometry = new StreamGeometry
            {
                FillRule = FillRule.EvenOdd
            };

            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(new Point(arrowPoint.X, arrowPoint.Y), true, true);

                ctx.LineTo(new Point(arrowPoint.X - size / 2, arrowPoint.Y - size), true, true);
                ctx.LineTo(new Point(arrowPoint.X + size / 2, arrowPoint.Y - size), true, true);
            }

            geometry.Freeze();

            relationshipPath.Data = geometry;
            rootCanvas.Children.Add(relationshipPath);
        }

        static void RemoveExistingLines(Panel rootCanvas)
        {
            var children = rootCanvas.Children;
            var paths = new List<FrameworkElement>();
            foreach (var line in children.Cast<FrameworkElement>().OfType<Path>())
            {
                paths.Add(line);
            }

            foreach (var path in paths)
            {
                children.Remove(path);
            }
        }

        Point GetPosition(string name, Panel panel)
        {
            return ((UIElement)panel.FindName(name))
                .TransformToAncestor(panel)
                .Transform(new Point(0, 0));
        }

        void RootGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var model = DataContext as ISagaWindowViewModel;
            var message = ((FrameworkElement)sender).DataContext as SagaMessage;
            SetSelected(model, message.MessageId);
        }

        void SetSelected(ISagaWindowViewModel model, Guid id)
        {
            foreach (var step in model.Data.Changes)
            {
                SetSelected(step.InitiatingMessage, id);
                SetSelected(step.OutgoingMessages, id);
            }
        }

        void SetSelected(IEnumerable<SagaMessage> messages, Guid id)
        {
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    SetSelected(message, id);
                }
            }
        }

        void SetSelected(SagaMessage message, Guid id)
        {
            message.IsSelected = message.MessageId == id;
        }

        void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            //var model = DataContext as ISagaWindowViewModel;
            var message = ((Hyperlink)e.Source).DataContext as SagaTimeoutMessage;
            if (message != null)
            {
                var steps = (ItemsControl)FindName("Steps");
                for (var i = 0; i < steps.Items.Count; i++)
                {
                    var update = steps.Items[i] as SagaUpdate;
                    if (update != null && update.InitiatingMessage.MessageId == message.MessageId)
                    {
                        ScrollIntoView(steps, i);
                    }
                }
            }
        }

        static void ScrollIntoView(ItemsControl steps, int i)
        {
            var stepsContainer = steps.ItemContainerGenerator.ContainerFromIndex(i);
            if (stepsContainer != null && VisualTreeHelper.GetChildrenCount(stepsContainer) > 0)
            {
                var item = (StackPanel)(((Grid)VisualTreeHelper.GetChild(stepsContainer, 0))).Children[0];
                item.BringIntoView();
            }
        }
    }

    public interface ISagaWindowView
    {
    }
}
