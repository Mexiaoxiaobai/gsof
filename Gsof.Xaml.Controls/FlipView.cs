﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Gsof.Xaml.Extensions;

namespace Gsof.Xaml.Controls
{
    /// <summary>
    /// FilpView
    /// </summary>
    [TemplatePart(Name = "PART_PREVIOUSITEM")]
    [TemplatePart(Name = "PART_CURRENTITEM")]
    [TemplatePart(Name = "PART_NEXTITEM")]
    public class FlipView : Selector
    {
        #region Private Fields
        private ContentPresenter _currentItem;
        private ContentPresenter _previousItem;
        private ContentPresenter _nextItem;
        private FrameworkElement _partRoot;
        private FrameworkElement _partContainer;

        private double _fromValue;
        private double _elasticFactor = 1.0;

        #endregion

        #region DependencyProperty

        public double ChangeSelectedOffset
        {
            get { return (double)GetValue(ChangeSelectedOffsetProperty); }
            set { SetValue(ChangeSelectedOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeSelectedOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeSelectedOffsetProperty =
            DependencyProperty.Register("ChangeSelectedOffset", typeof(double), typeof(FlipView), new PropertyMetadata(50d));

        /// <summary>
        /// 前一项
        /// </summary>
        public object PreviousItem
        {
            get { return (object)GetValue(PreviousItemProperty); }
            set { SetValue(PreviousItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviousItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousItemProperty =
            DependencyProperty.Register("PreviousItem", typeof(object), typeof(FlipView), new PropertyMetadata(null));

        /// <summary>
        /// 下一项
        /// </summary>
        public object NextItem
        {
            get { return (object)GetValue(NextItemProperty); }
            set { SetValue(NextItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextItemProperty =
            DependencyProperty.Register("NextItem", typeof(object), typeof(FlipView), new PropertyMetadata(null));





        #endregion


        #region Constructor

        static FlipView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipView), new FrameworkPropertyMetadata(typeof(FlipView)));
            //SelectedIndexProperty.OverrideMetadata(typeof(FlipView), new FrameworkPropertyMetadata(-1, OnSelectedIndexChanged));
        }

        public FlipView()
        {
            this.CommandBindings.Add(new CommandBinding(NextCommand, this.OnNextExecuted, this.OnNextCanExecute));
            this.CommandBindings.Add(new CommandBinding(PreviousCommand, this.OnPreviousExecuted, this.OnPreviousCanExecute));
        }

        #endregion

        #region Private methods

        private void OnRootManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this._fromValue = e.TotalManipulation.Translation.X;
            if (this._fromValue > 0)
            {
                if (this.SelectedIndex > 0)
                {
                    this.SelectedIndex -= 1;
                }
            }
            else
            {
                if (this.SelectedIndex < this.Items.Count - 1)
                {
                    this.SelectedIndex += 1;
                }
            }

            if (this._elasticFactor < 1)
            {
                this.RunSlideAnimation(0, ((MatrixTransform)this._partRoot.RenderTransform).Matrix.OffsetX);
            }
            this._elasticFactor = 1.0;
        }

        private void OnRootManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!(this._partRoot.RenderTransform is MatrixTransform))
            {
                this._partRoot.RenderTransform = new MatrixTransform();
            }

            Matrix matrix = ((MatrixTransform)this._partRoot.RenderTransform).Matrix;
            var delta = e.DeltaManipulation;

            if ((this.SelectedIndex == 0 && delta.Translation.X > 0 && this._elasticFactor > 0)
                || (this.SelectedIndex == this.Items.Count - 1 && delta.Translation.X < 0 && this._elasticFactor > 0))
            {
                this._elasticFactor -= 0.05;
            }

            matrix.Translate(delta.Translation.X * this._elasticFactor, 0);
            this._partRoot.RenderTransform = new MatrixTransform(matrix);

            e.Handled = true;
        }

        private void OnRootManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this._partContainer;
            e.Handled = true;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RefreshViewPort(this.SelectedIndex);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.SelectedIndex > -1)
            {
                this.RefreshViewPort(this.SelectedIndex);
            }
        }
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FlipView;

            if (control == null)
            {
                return;
            }

            control.OnSelectedIndexChanged(e);
        }

        private void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!this.EnsureTemplateParts())
            {
                return;
            }

            if ((int)e.NewValue >= 0 && (int)e.NewValue < this.Items.Count)
            {
                double toValue = (int)e.OldValue < (int)e.NewValue ? -this.ActualWidth : this.ActualWidth;
                this.RunSlideAnimation(toValue, this._fromValue);
            }
        }

        private void RefreshViewPort(int selectedIndex)
        {
            if (!this.EnsureTemplateParts())
            {
                return;
            }

            Canvas.SetLeft(this._previousItem, -this.ActualWidth);
            Canvas.SetLeft(this._nextItem, this.ActualWidth);
            this._partRoot.RenderTransform = new TranslateTransform();

            //var currentItem = this.GetItemAt(selectedIndex);
            var nextItem = this.GetItemAt(selectedIndex + 1);
            var previousItem = this.GetItemAt(selectedIndex - 1);

            //this._currentItem.Content = currentItem;
            //this._nextItem.Content = nextItem;
            //this._previousItem.Content = previousItem;

            //SelectedItem = currentItem;
            PreviousItem = previousItem;
            NextItem = nextItem;
        }

        public void RunSlideAnimation(double toValue, double p_fromValue = 0)
        {
            if (!(this._partRoot.RenderTransform is TranslateTransform))
            {
                this._partRoot.RenderTransform = new TranslateTransform();
            }

            _partRoot.CreateRenderTransform();

            var story = AnimationFactory.Instance.GetAnimation(this._partRoot, toValue, p_fromValue);
            story.Completed += (s, e) => this.RefreshViewPort(this.SelectedIndex);
            story.Begin();
        }

        private object GetItemAt(int index)
        {
            if (index < 0 || index >= this.Items.Count)
            {
                return null;
            }

            return this.Items[index];
        }

        private bool EnsureTemplateParts()
        {
            return this._currentItem != null &&
                this._nextItem != null &&
                this._previousItem != null &&
                this._partRoot != null;
        }

        private void OnPreviousCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectedIndex > 0;
        }

        private void OnPreviousExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectedIndex -= 1;
        }

        private void OnNextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectedIndex < (this.Items.Count - 1);
        }

        private void OnNextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectedIndex += 1;
        }
        #endregion

        #region Commands

        public static RoutedUICommand NextCommand = new RoutedUICommand("Next", "Next", typeof(FlipView));
        public static RoutedUICommand PreviousCommand = new RoutedUICommand("Previous", "Previous", typeof(FlipView));

        #endregion

        #region Override methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._previousItem = this.GetTemplateChild("PART_PREVIOUSITEM") as ContentPresenter;
            this._nextItem = this.GetTemplateChild("PART_NEXTITEM") as ContentPresenter;
            this._currentItem = this.GetTemplateChild("PART_CURRENTITEM") as ContentPresenter;
            this._partRoot = this.GetTemplateChild("PART_Root") as FrameworkElement;
            this._partContainer = this.GetTemplateChild("PART_Container") as FrameworkElement;

            this.Loaded += this.OnLoaded;
            this.SizeChanged += this.OnSizeChanged;


            this._partRoot.ManipulationStarting += this.OnRootManipulationStarting;
            this._partRoot.ManipulationDelta += this.OnRootManipulationDelta;
            this._partRoot.ManipulationCompleted += this.OnRootManipulationCompleted;

            this._partRoot.MouseLeftButtonDown += OnRootMouseLeftButtonDown;
            this._partRoot.MouseLeftButtonUp += OnRootMouseLeftButtonUp;

            this._partRoot.MouseMove += OnRootMouseMove;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlipViewItem();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            RefreshViewPort(this.SelectedIndex);
        }

        #endregion

        #region MouseEvent

        private bool _isLeftButtonDown;
        private Point? _startPoint;

        private void OnRootMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed
                || !_isLeftButtonDown
                || _startPoint == null)
            {
                return;
            }

            if (!(this._partRoot.RenderTransform is MatrixTransform))
            {
                this._partRoot.RenderTransform = new MatrixTransform();
            }

            var cPoint = e.GetPosition(this);
            var x = cPoint.X - _startPoint.Value.X;

            if ((this.SelectedIndex == 0 && x > 0 && this._elasticFactor > 0)
                || (this.SelectedIndex == this.Items.Count - 1 && x < 0 && this._elasticFactor > 0))
            {
                this._elasticFactor -= 0.05;
            }

            var tx = this._elasticFactor * x;
            if (Math.Abs(tx) >= this.ActualWidth)
            {
                tx = (tx < 0 ? -1 : 1) * this.ActualWidth;
            }

            var matrix = new Matrix();
            matrix.Translate(tx, 0);
            this._partRoot.RenderTransform = new MatrixTransform(matrix);

            if (x < 0)
            {
                _nextItem.Opacity = 0.5;
            }
            else
            {
                _previousItem.Opacity = 0.5;
            }

            e.Handled = true;
        }

        private void OnRootMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isLeftButtonDown = true;
            _startPoint = e.GetPosition(this);
            _partRoot.CaptureMouse();
            e.Handled = true;
        }

        private void OnRootMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var cPoint = e.GetPosition(this);

            if (_startPoint != null)
            {
                this._fromValue = cPoint.X - _startPoint.Value.X;
            }

            if (Math.Abs(this._fromValue) > ChangeSelectedOffset)
            {
                if (this._fromValue > 0)
                {
                    if (this.SelectedIndex > 0)
                    {
                        this.SelectedIndex -= 1;
                    }
                }
                else
                {
                    if (this.SelectedIndex < this.Items.Count - 1)
                    {
                        this.SelectedIndex += 1;
                    }
                }

                if (this._elasticFactor < 1)
                {
                    if (this._partRoot.RenderTransform is MatrixTransform)
                    {
                        this.RunSlideAnimation(0, ((MatrixTransform)this._partRoot.RenderTransform).Matrix.OffsetX);
                    }
                }
                this._elasticFactor = 1.0;
            }
            else
            {
                if (this._partRoot.RenderTransform is MatrixTransform)
                {
                    this.RunSlideAnimation(0, ((MatrixTransform)this._partRoot.RenderTransform).Matrix.OffsetX);
                }
            }

            _startPoint = null;
            _isLeftButtonDown = false;
            _partRoot.ReleaseMouseCapture();

        }

        #endregion






    }
}