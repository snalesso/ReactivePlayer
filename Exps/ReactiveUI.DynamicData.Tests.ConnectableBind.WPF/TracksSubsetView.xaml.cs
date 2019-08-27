using System;
using System.Windows;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF
{
    /// <summary>
    /// Interaction logic for TracksSubsetView.xaml
    /// </summary>
    public partial class TracksSubsetView : Window
    {
        public TracksSubsetView()
        {
            this.InitializeComponent();
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            (this.DataContext as TracksSubsetViewModel).Connect();
        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            (this.DataContext as TracksSubsetViewModel).Disconnect();
        }

        private void GC_Collect(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }
    }
}