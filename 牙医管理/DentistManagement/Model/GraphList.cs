using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DentistManagement.Model
{
    [StyleTypedPropertyAttribute(Property = "ItemContainerStyle", StyleTargetType = typeof(GraphListItem))]

    [TemplatePart(Name = "Graph", Type = typeof(Shape))]
    public class GraphList : ItemsControl, INotifyPropertyChanged
    {
        static GraphList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphList), new FrameworkPropertyMetadata(typeof(GraphList)));
        }

        public override void OnApplyTemplate()
        {
            Graph = Template.FindName("Graph", this) as Shape;
            base.OnApplyTemplate();
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            var c = new GraphListItem();
            c.PositionChanged += Container_PositionChanged;
            return c;
        }

        Shape Graph;
        void ShowLine()
        {
            var p = Graph;

            if (p is Polyline)
            {
                var pl = p as Polyline;

                if (HasItems)
                {
                    PointCollection pc = new PointCollection();
                    foreach (var item in this.Items)
                    {
                        var c = this.ItemContainerGenerator.ContainerFromItem(item) as GraphListItem;
                        if (c.IsDragging)
                        {
                            XX = c.X;
                            YY = c.Y;
                        }

                        pc.Add(new Point(c.X + 5, c.Y + 5));
                    }
                    pl.Points = pc;
                }
                else
                {
                    pl.Points = null;
                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (item is GraphListItem)
            {
                return true;
            }

            return base.IsItemItsOwnContainerOverride(item);
        }
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is GraphListItem)
            {
                var c = element as GraphListItem;
                c.PositionChanged -= Container_PositionChanged;
                ShowLine();
            }
            base.ClearContainerForItemOverride(element, item);
        }

        void Container_PositionChanged(object sender, EventArgs e)
        {
            ShowLine();
        }
        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
        }
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            base.OnItemsChanged(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static readonly DependencyProperty XXProperty =
          DependencyProperty.Register("XX", typeof(double), typeof(GraphList));

        public double XX
        {
            get { return (double)GetValue(XXProperty); }
            set
            {
                SetValue(XXProperty, value);

            }
        }

        public static readonly DependencyProperty YYProperty =
         DependencyProperty.Register("YY", typeof(double), typeof(GraphList));

        public double YY
        {
            get { return (double)GetValue(YYProperty); }
            set
            {
                SetValue(YYProperty, value);

            }
        }

    }
    [TemplateVisualState(GroupName = "SelectState", Name = "Selected")]
    [TemplateVisualState(GroupName = "SelectState", Name = "UnSelected")]
    public class GraphListItem : ContentControl, INotifyPropertyChanged
    {
        static GraphListItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphListItem), new FrameworkPropertyMetadata(typeof(GraphListItem)));
        }
        public GraphListItem()
        {

        }

        public override void OnApplyTemplate()
        {
            this.RenderTransform = tm;
            base.OnApplyTemplate();
        }

        public event EventHandler PositionChanged;

        protected virtual void OnPositionChanged()
        {
            if (PositionChanged != null)
            {
                this.Dispatcher.BeginInvoke(PositionChanged, this, EventArgs.Empty);
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(GraphListItem), new PropertyMetadata(false));

        public bool IsDragging
        {
            get
            {
                return (bool)GetValue(IsDraggingProperty.DependencyProperty);
            }
        }

        public static readonly DependencyPropertyKey IsDraggingProperty = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(GraphListItem), new FrameworkPropertyMetadata(false));

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {

            offsetpoint = e.GetPosition(this);
            this.CaptureMouse();
            this.SetValue(IsDraggingProperty, true);
            base.OnMouseLeftButtonDown(e);

        }
        Point offsetpoint;
        TranslateTransform tm = new TranslateTransform();
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsDragging)
            {
                var p = e.GetPosition(this.VisualParent as UIElement);
                //tm.X =;
                //tm.Y =;
                this.X = p.X - offsetpoint.X;
                this.Y = p.Y - offsetpoint.Y;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.SetValue(IsDraggingProperty, false);
            this.ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }



        public double X
        {
            get { return (double)GetValue(XProperty); }
            set
            {
                SetValue(XProperty, value);
                NotifyPropertyChange("X");
            }
        }


        // Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(GraphListItem), new PropertyMetadata(OnPropertyChanged));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set
            {
                SetValue(YProperty, value);
                NotifyPropertyChange("Y");
            }
        }

        // Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(GraphListItem), new PropertyMetadata(OnPropertyChanged));

        static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GraphListItem item = sender as GraphListItem;
            switch (args.Property.Name)
            {
                case "X":
                    item.tm.X = (double)args.NewValue;
                    item.OnPositionChanged();
                    break;
                case "Y":
                    item.tm.Y = (double)args.NewValue;
                    item.OnPositionChanged();
                    break;
                default:
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
