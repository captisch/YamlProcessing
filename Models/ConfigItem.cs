using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private string? _value;

    public string? Value
    {
        get => _value;
        set
        {
            if (value == _value) return;
            
            string? previousValue = _value;

            if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(DefaultValue))
            {
                value = DefaultValue;
            }
            else value = value.Trim();
            
            _value = value;
            OnPropertyChanged();
        }
    }
    
    [ObservableProperty]
    public string? name;
    
    [ObservableProperty]
    public string? defaultValue;
    
    /*
    [ObservableProperty]
    private string? value;
    */
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

    /*
    partial void OnValueChanged(string? oldValue, string? newValue)
    {
        Console.WriteLine($"Changed from \"{oldValue}\" to \"{newValue}\"");
        if (string.IsNullOrWhiteSpace(newValue) && !string.IsNullOrWhiteSpace(DefaultValue))
        {
            
            Value = DefaultValue;
        }
        else if (char.IsWhiteSpace(newValue[0]) || char.IsWhiteSpace(newValue[^1]))
        {
            Value = newValue.Trim();
        }
    }
    */

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

    