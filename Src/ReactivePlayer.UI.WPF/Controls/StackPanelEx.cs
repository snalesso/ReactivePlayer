using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ReactivePlayer.UI.WPF.Controls
{
    // StackPanel source code: https://referencesource.microsoft.com/#PresentationFramework/src/Framework/System/Windows/Controls/Stack.cs,9c87817eddd701e2
    public class StackPanelEx : StackPanel
    {
        public double SpacingValue
        {
            get => (double)this.GetValue(SpacingValueProperty);
            set => this.SetValue(SpacingValueProperty, value);
        }

        public static DependencyProperty SpacingValueProperty = DependencyProperty.Register(
            nameof(SpacingValue),
            typeof(double),
            typeof(StackPanelEx),
            new UIPropertyMetadata(0d, SpacingValueChangedCallback));

        private static void SpacingValueChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is StackPanelEx stackPanelEx))
                return;

            if (stackPanelEx.IsLoaded)
            {
                stackPanelEx.UpdateChildrensSpacing();
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (this.VisualChildrenCount > 1)
                this.UpdateChildrensSpacing();

            //if ((visualAdded is FrameworkElement feChild))
            //{
            //    feChild.Margin = new Thickness(this.SpacingValue, 0, 0, 0);
            //}
        }

        private void UpdateChildrensSpacing()
        {
            var feChildren = new List<FrameworkElement>();
            foreach (var child in this.Children)
            {
                if (child is FrameworkElement feChild)
                    feChildren.Add(feChild);
            }

            for (int i = 0; i < feChildren.Count; i++)
            {
                var feChild = feChildren[i];

                if (i == 0)
                {
                    feChild.Margin = new Thickness(0);
                }
                else
                {
                    feChild.Margin = new Thickness(this.SpacingValue, 0, 0, 0);
                }
            }

            //if (Debugger.IsAttached)
            //    this.UpdateLayout();
        }
    }
}