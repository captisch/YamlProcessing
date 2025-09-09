using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YamlProcessing.Models;

public partial class ConfigItem : ObservableObject
{
    [ObservableProperty]
    public string? name;
    
    [ObservableProperty]
    public string? defaultValue;
    
    [ObservableProperty]
    private string? value;
    
    [ObservableProperty]
    public string? type;
    
    [ObservableProperty]
    public string? description;
    
    [ObservableProperty]
    public string? access;
    
    public ObservableCollection<string?> options = new();
}