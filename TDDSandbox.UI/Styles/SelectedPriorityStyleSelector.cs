using System.Windows;
using System.Windows.Controls;

namespace PyxisPrepAshp.Styles
{
    public class SelectedPriorityStyleSelector : StyleSelector
    {
        public Style Selected { get; set; }
        public Style Default { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var textBlock = item as TextBlock;
            return Selected;
        }
    }
}
