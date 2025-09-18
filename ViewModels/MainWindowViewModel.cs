using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization.NamingConventions;
using YamlProcessing.Models;

namespace YamlProcessing.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<ConfigItem> items { get; set; } = new();
    
    public MainWindowViewModel()
    {
        Load();
    }
    
    public void Load()
    {
        // Pfad zum YAML-File in Assets-Ordner
        // Build-Eigenschaft der Datei auf "Content" und "Copy if newer" setzen !
        string configFilePath = Path.Combine(AppContext.BaseDirectory, "Assets", "configFile.yaml"); 
        
        var yml = System.IO.File.ReadAllText(configFilePath);
        
        var deserializer = new YamlDotNet.Serialization.Deserializer();

        items = deserializer.Deserialize<ObservableCollection<ConfigItem>>(yml);
        
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(item.DefaultValue))
                item.Value = item.DefaultValue;
        }


        foreach (var item in items)
        {
            Console.WriteLine($"Name: {item.Name}, Description: {item.Description}, Access: {item.Access}");
            foreach (var option in item.options)
            {
                Console.WriteLine($"   {option}");
            }
        }
    }
    
    [RelayCommand]
    private Task Save()
    {
        string saveFilePath = Path.Combine(AppContext.BaseDirectory, "Assets", "configFile_output.yaml");

        var yml = "";

        foreach (var item in items)
        {
            yml +=  item.Name + ": " + item.Value + "\n";
        }
        
        /*
        var serializer = new YamlDotNet.Serialization.SerializerBuilder()
            .IgnoreFields()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var yml = serializer.Serialize(items);
        */
        
        
        System.IO.File.WriteAllText(saveFilePath, yml);
        Console.WriteLine($"Saving to {saveFilePath}");
        return Task.CompletedTask;;
    }
    
    [RelayCommand]
    public async Task ChooseOutputDirectory(ConfigItem item)
    {
        if (item is null)
            return;

        var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var window = lifetime?.MainWindow;
        if (window?.StorageProvider is null)
            return;

        var result = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Ordner auswählen",
            AllowMultiple = false
        });

        var folder = result?.FirstOrDefault();
        if (folder is not null)
        {
            var path = folder.TryGetLocalPath() ?? folder.Path.LocalPath;
            item.Value = path;
        }
    }

}