using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YamlProcessing.Models;

public partial class ExternalModule : ObservableObject
{
    [ObservableProperty] private string? source;
    
    [ObservableProperty] private Module? module;

    [ObservableProperty] private string? instance;
    
    [ObservableProperty] private bool giveSource;
}