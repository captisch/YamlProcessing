using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YamlProcessing.Models;

public partial class ExternalModule : ObservableObject
{
    [ObservableProperty] private string? source;
    
    [ObservableProperty] private string? moduleName;

    [ObservableProperty] private string? instanceName;
    
    private ObservableCollection<port> ports { get; set; } = new();
}

public partial class port : ObservableObject
{
    [ObservableProperty]
    private string? name;
    
    [ObservableProperty]
    private string? direction;
    
    [ObservableProperty]
    private int? size;
}