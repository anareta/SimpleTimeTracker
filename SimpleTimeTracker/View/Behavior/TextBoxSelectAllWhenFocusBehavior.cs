using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleTimeTracker.View.Behavior
{
    public class TextBoxSelectAllWhenFocusBehavior : Behavior<TextBox>
    {

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.PreviewMouseDown += AssociatedObject_GotFocus;
        }


        private void AssociatedObject_GotFocus(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.Dispatcher.BeginInvoke(() =>
            {
                this.AssociatedObject.SelectAll();
            });
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.PreviewMouseDown -= AssociatedObject_GotFocus;
        }
    }
}
