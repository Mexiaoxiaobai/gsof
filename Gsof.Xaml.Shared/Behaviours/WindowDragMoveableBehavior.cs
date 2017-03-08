using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using Gsof.Xaml.Extensions;

namespace Gsof.Xaml.Shared.Behaviours
{
    public class WindowDragMoveableBehavior : Behavior<FrameworkElement>
    {
        public bool AllowHandler
        {
            get { return (bool)GetValue(AllowHandlerProperty); }
            set { SetValue(AllowHandlerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowHandler.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowHandlerProperty =
            DependencyProperty.Register("AllowHandler", typeof(bool), typeof(WindowDragMoveableBehavior), new PropertyMetadata(false));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), AllowHandler);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown));
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnWindowDragMove();
        }

        private void OnWindowDragMove()
        {
            var window = AssociatedObject.GetWindow();
            if (window == null)
            {
                return;
            }

            window.DragMove();
        }
    }
}
