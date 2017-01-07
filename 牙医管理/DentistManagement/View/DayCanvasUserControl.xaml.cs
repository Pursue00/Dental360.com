using System;
using System.Collections.Generic;
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
using System.Windows.Controls.Primitives;
using DentistManagement.ViewModel;
using DentistManagement.Model;

namespace DentistManagement.View
{
    /// <summary>
    /// DayCanvasUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class DayCanvasUserControl : UserControl
    {
        public DayCanvasUserControl()
        {
            InitializeComponent();
            //this.DataContext = new MainWindowViewModel();
        }

        private const int MINIMAL_SIZE = 10;   //拖拽时最小有效距离
        /// <summary>
        /// 拖拽改变大小事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReSize_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            switch (thumb.VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    if (this.ActualHeight + e.VerticalChange > MINIMAL_SIZE)
                    {
                        this.Height += e.VerticalChange;
                    }
                    break;
                case VerticalAlignment.Top:
                    if (this.ActualHeight - e.VerticalChange > MINIMAL_SIZE)
                    {
                        this.Height -= e.VerticalChange;
                        Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
                    }
                    break;
            }
            switch (thumb.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    if (this.ActualWidth - e.HorizontalChange > MINIMAL_SIZE)
                    {
                        this.Width -= e.HorizontalChange;
                        Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
                    }
                    break;
                case HorizontalAlignment.Right:
                    if (this.ActualWidth + e.HorizontalChange > MINIMAL_SIZE)
                    {
                        this.Width += e.HorizontalChange;
                    }
                    break;
            }
            //lbTime.Content = string.Format("h:{0},v:{1}", e.HorizontalChange, e.VerticalChange);
           //lbTimeLeft.Content = e.HorizontalChange + "-" + e.VerticalChange;
            e.Handled = true;
            //MessageBox.Show(a);
        }
    }
}
