using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ReactivePlayer.UI.WPF.Controls
{
    public class PathButton : Button
    {
        #region Data

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data),
            typeof(Geometry),
            typeof(PathButton),
            new PropertyMetadata(new PropertyChangedCallback(OnDataChanged)));

        public Geometry Data
        {
            get { return (Geometry)this.GetValue(DataProperty); }
            set { this.SetValue(DataProperty, value); }
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdatePath(d as PathButton, (Path p) => p.Data = e.NewValue as Geometry);
        }

        #endregion

        #region Brush

        public static readonly DependencyProperty PathBrushProperty = DependencyProperty.Register(
            nameof(PathBrush),
            typeof(Brush),
            typeof(PathButton),
            new PropertyMetadata(new PropertyChangedCallback(OnPathBrushChanged)));

        public Brush PathBrush
        {
            get { return (Brush)this.GetValue(PathBrushProperty); }
            set { this.SetValue(PathBrushProperty, value); }
        }

        private static void OnPathBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdatePath(d as PathButton, (Path p) => p.Fill = e.NewValue as Brush);
        }

        #endregion

        private static void UpdatePath(PathButton pathButton, Action<Path> pathUpdater)
        {
            var path = pathButton.Content as Path;

            if (path == null)
            {
                pathButton.Content = new Path()
                {
                    Data = pathButton.Data,
                    Fill = pathButton.PathBrush,
                    Stretch = Stretch.Uniform
                };
            }

            pathUpdater(path);
        }
    }
}