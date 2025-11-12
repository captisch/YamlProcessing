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
    
    public ObservableCollection<ExternalModule> externalModules { get; set; } = new();
    
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

    private string CreateOutputConfigFile()
    {
        var yml = "";

        foreach (var item in items)
        {
            yml +=  item.Name + ": " + item.Value + "\n";
        }
        
        yml += "\nExternal_Modules: {\n";

        foreach (var (index, module) in externalModules.Index())
        {
            yml += $"\t\"Ext_mod_{index}\":{{\n";
            var source = module.LinkSource ? module.Source : "None";
            yml += $"\t\t\"source\": \"{source}\",\n";
            yml += $"\t\t\"module_name\": \"{module.Module.Name}\",\n";
            yml += $"\t\t\"instance_name\": \"{module.Instance}\",\n";
            yml += $"\t\t\"ports\": {{\n";
            foreach (var (index_port, port) in module.Module.Ports.Index())
            {
                yml += $"\t\t\t\"port{index_port}\": {{\n";
                yml += $"\t\t\t\t\"name\": \"{port.Name}\",\n";
                var direction = port.Direction.ToString().ToLower() == "input" ? "in" : 
                    port.Direction.ToString().ToLower() == "output" ? "out" :
                    port.Direction.ToString().ToLower();
                yml += $"\t\t\t\t\"direction\": \"{direction}\",\n";
                yml += $"\t\t\t\t\"size\": {port.Width}\n";
                yml += $"\t\t\t}},\n";
            }
            yml += "\t\t},\n";
            yml += "\t},\n";
        }
        yml += "}\n";
        
        return yml;
    }
    
    private ConfigItem? FindItemByName(string name)
    {
        name = name.ToLower();
        return items.FirstOrDefault(item => item.Name == name);
    }

    [RelayCommand]
    private Task Save()
    {
        var rootPath = Path.GetPathRoot(AppContext.BaseDirectory);

        var nameItem = FindItemByName("Name");
        if (nameItem is null)
            return Task.CompletedTask;

        string saveFilePath = Path.Combine(rootPath, "fentwumsGUI", "systembuilder", $"configFile_{nameItem.Value}.yaml");

        if (!Directory.Exists(Path.GetDirectoryName(saveFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath));
        }

        var yml = CreateOutputConfigFile();
        
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
    
    [RelayCommand]
    private async Task AddExternalModule()
    {
        var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var window = lifetime?.MainWindow;
        if (window?.StorageProvider is null)
            return;

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Moduledatei auswählen",
            AllowMultiple = true,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Verilog-Dateien"){ Patterns = new[] {"*.v"} }
            }.ToList()
            
        });
        var verilogParser = new VerilogParser();
        foreach (var file in files)
        {
            var verilogPath = file.TryGetLocalPath() ?? file.Path.LocalPath;
            #if DEBUG
            Console.WriteLine(verilogPath);
            #endif
            
            var modules = verilogParser.ReadVerilog(verilogPath);

            if (modules.Count > 0)
            {
                foreach (var module in modules)
                {
                    externalModules.Add(new ExternalModule
                    {
                        Module = module,
                        Source = verilogPath,
                        Instance = $"Ext_Mod_{externalModules.Count}",
                    });
                }
            }
        }
    }

    
}