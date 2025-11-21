using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YamlProcessing.Models;

public class Module
{
    public string Name { get; set; }
    public List<Port>? Ports { get; set; }
    public string? Logic { get; set; }
    
    public List<ModuleParameter> Parameters { get; set; } = new();
}

public enum PortTypes{
    none,
    wire,
    reg
}

public enum PortDirections{
    none,
    input,
    output,
    inout,
}

public partial class Port : ObservableObject
{
    [ObservableProperty] private string? name;
    [ObservableProperty] private int width;
    [ObservableProperty] private PortTypes type;
    [ObservableProperty] private PortDirections direction;
    [ObservableProperty] private bool routeToTopmodule = true;
}

public class VerilogParser
{
    public List<Module> ReadVerilog(string verilogFile)
    {
        var modules = new List<Module>();
        if (!System.IO.File.Exists(verilogFile)) return modules;
        if (string.IsNullOrWhiteSpace(verilogFile)) return modules;
        if (!verilogFile.EndsWith(".v")) return modules;
        
        
        string verilogText = System.IO.File.ReadAllText(verilogFile);
        
        var patternSingleLineComment = @"//.*";
        var patternMultiLineComment = @"/\*[\s\S]*?\*/";
        
        var verilogTextNoComments = Regex.Replace(
            (Regex.Replace (verilogText, patternMultiLineComment, "")),
            patternSingleLineComment, "");

        var patternModule =
            @"(?<module>module\s+(?<modulename>\w+)\s*\((?<portlist>[\s\S]+?)\)\s*;(?<logic>[\s\S]*?)endmodule)";

        var matchModule = Regex.Matches(verilogTextNoComments, patternModule);

        foreach (Match match in matchModule)
        {
            var modulename = match.Groups["modulename"].Value;
            var modulelogic = match.Groups["logic"].Value;
            var moduleports = new List<Port>();

            var portlist = match.Groups["portlist"].Value;
            var ports = portlist.Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p));

            PortDirections lastPortDirection = PortDirections.none;
            PortTypes lastPortType = PortTypes.none;
            int lastPortWidth = 1;

            foreach (var port in ports)
            {
                var portBracketsReplaced = port.Replace('[', ' ').Replace(']', ' ');

                var elements = portBracketsReplaced.Split(' ')
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrWhiteSpace(p));

                foreach (var element in elements)
                {
                    if (Enum.TryParse(element, out PortDirections portDirection))
                    {
                        lastPortDirection = portDirection;
                        lastPortType = PortTypes.wire;
                        lastPortWidth = 1;
                    }
                    else if (Enum.TryParse(element, out PortTypes portType))
                    {
                        lastPortType = portType;
                        lastPortWidth = 1;
                    }
                    else if (char.IsDigit(element[0]))
                    {
                        var width = element.Split(':')
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .Select(int.Parse)
                            .First();
                        lastPortWidth = width + 1;
                    }
                    else
                    {
                        moduleports.Add(new Port
                        {
                            Direction = lastPortDirection,
                            Type = lastPortType,
                            Width = lastPortWidth,
                            Name = element
                        });
                    }
                }
            }
            
            modules.Add(new Module
            {
                Name = modulename,
                Ports = moduleports,
                Logic = modulelogic
            });
        }
        return modules;
    }
    
}