using System;
using Avalonia.Controls;

namespace YamlProcessing.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Console.WriteLine(this);
    }
}