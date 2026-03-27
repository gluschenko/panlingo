using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Panlingo.Samples.ViewModels;

namespace Panlingo.Samples;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
