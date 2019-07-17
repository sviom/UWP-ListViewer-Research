using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ListScrollResearch.CustomControls
{
    public class ShowDpPropertyButton : Button
    {
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(DateTimeOffset), typeof(ShowDpPropertyButton), new PropertyMetadata(DateTimeOffset.Now));
        public DateTimeOffset SelectedDate
        {
            get { return (DateTimeOffset)GetValue(SelectedDateProperty); }
            set
            {
                SetValue(SelectedDateProperty, value);
            }
        }
    }
}
