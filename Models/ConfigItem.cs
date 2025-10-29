using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace YamlProcessing.Models;

enum ConfigItemTypes
{
    String,
    Integer,
    Boolean,
    Float,
    Enum
};

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
    public bool? access;
    
    public ObservableCollection<string?> options { get; set; } = new();
    
    partial void OnDefaultValueChanged(string? oldValue, string? newValue)
    {
        if (string.IsNullOrWhiteSpace(Value) && !string.IsNullOrWhiteSpace(newValue))
        {
            Value = newValue;
        }
    }

    partial void OnValueChanged(string? oldValue, string? newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue) && !string.IsNullOrWhiteSpace(DefaultValue))
        {
            // Nur setzen, wenn es wirklich leer ist; verhindert Endlosschleifen
            if (!string.Equals(oldValue, DefaultValue))
                Value = DefaultValue;
        }
    }
    
    partial void OnLostFocus(string? oldValue, string? newValue)

    [RelayCommand]
    private void IsEntryValid(ConfigItem item)
    {
        switch (item.Type)
        {
            case "OpenString":
                if (string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(item.DefaultValue))
                {
                    item.Value = item.DefaultValue;
                }
                break;
            case "RestrictedString":
                break;
            case "Integer":
                break;
            case "Boolean":
                break;
            case "Float":
                break;
            case "Enum":
                break;
            default:
                break;
        }
    }

}

    