using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ReactivePlayer.UI.WPF.Controls
{
    public class GridLengthSwitcher : Control
    {
        public static readonly DependencyProperty ToggleSourceProperty = DependencyProperty.Register(
            nameof(ToggleSource),
            typeof(bool),
            typeof(GridLengthSwitcher),
            new PropertyMetadata(true, ToggleSourceValueChangedCallback));
        public bool ToggleSource
        {
            get { return Convert.ToBoolean(this.GetValue(ToggleSourceProperty)); }
            set { this.SetValue(ToggleSourceProperty, value); }
        }
        private static void ToggleSourceValueChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register(
            nameof(TrueValueProperty),
            typeof(GridLength),
            typeof(GridLengthSwitcher));
        public GridLength TrueValue
        {
            get { return (GridLength)(this.GetValue(TrueValueProperty)); }
            set { this.SetValue(TrueValueProperty, value); }
        }
        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(
            nameof(FalseValueProperty),
            typeof(GridLength),
            typeof(GridLengthSwitcher));
        public GridLength FalseValue
        {
            get { return (GridLength)(this.GetValue(FalseValueProperty)); }
            set { this.SetValue(FalseValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(ValueProperty),
            typeof(GridLength),
            typeof(GridLengthSwitcher));
        public GridLength Value
        {
            get { return (GridLength)(this.GetValue(ValueProperty)); }
            private set { this.SetValue(ValueProperty, value); }
        }
    }
}