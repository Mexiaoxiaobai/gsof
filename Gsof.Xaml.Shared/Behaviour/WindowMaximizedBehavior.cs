using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace Gsof.Xaml.Behaviours
{
    public class WindowMaximizedBehavior : Behavior<ButtonBase>
    {
        private Window _window;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += OnButtonClick;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Click -= OnButtonClick;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (_window == null)
            {
                _window = Window.GetWindow(AssociatedObject);
            }

            if (_window == null)
            {
                return;
            }

            var isMax = _window.WindowState == WindowState.Maximized;

            if (AssociatedObject is ToggleButton)
            {
                var tb = (ToggleButton)AssociatedObject;
                tb.IsChecked = isMax;
            }

            _window.WindowState = isMax ? WindowState.Normal : WindowState.Maximized;
        }
    }
}