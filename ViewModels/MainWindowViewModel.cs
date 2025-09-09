using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using YamlDotNet.RepresentationModel;
using YamlProcessing.Models;

namespace YamlProcessing.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<ConfigItem> items { get; set; } = new();
    
    public MainWindowViewModel()
    {
        Test();
    }
    
    public void Test()
    {
        string configFilePath = "C:\\Users\\Schröer\\RiderProjects\\YamlProcessing\\Assets\\configFile.yaml";
        
        var yml = System.IO.File.ReadAllText(configFilePath);
        
        var deserializer = new YamlDotNet.Serialization.Deserializer();

        items = deserializer.Deserialize<ObservableCollection<ConfigItem>>(yml);

        foreach (var item in items)
        {
            Console.WriteLine($"Name: {item.Name}, Description: {item.Description}");
            foreach (var option in item.options)
            {
                Console.WriteLine($"   {option}");
            }
        }
    }
}