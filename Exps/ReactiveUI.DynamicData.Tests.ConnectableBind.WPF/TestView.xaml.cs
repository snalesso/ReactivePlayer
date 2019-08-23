using System;
using System.Windows;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF
{
    /// <summary>
    /// Interaction logic for TestView.xaml
    /// </summary>
    public partial class TestView : Window
    {
        public TestView()
        {
            this.InitializeComponent();
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            (this.DataContext as TestViewModel).Connect();
        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            (this.DataContext as TestViewModel).Disconnect();
        }

        private void GC_Collect(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }
    }
}