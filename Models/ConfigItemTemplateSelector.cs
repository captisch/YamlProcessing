using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using YamlProcessing.Models;

namespace YamlProcessing.Templates;

public sealed class ConfigItemTemplateSelector : IDataTemplate
{
    public IDataTemplate? OpenStringTemplate { get; set; }
    public IDataTemplate? BooleanTemplate { get; set; }
    public IDataTemplate? RestrictedStringTemplate { get; set; }
    public IDataTemplate? IntegerTemplate { get; set; }
    public IDataTemplate? FilePathTemplate { get; set; }
    public IDataTemplate? DefaultTemplate { get; set; }

    public Control? Build(object? param)
    {
        if (param is not ConfigItem item)
            return DefaultTemplate?.Build(param);

        var type = item.Type?.Trim();

        return type?.Equals("OpenString", StringComparison.OrdinalIgnoreCase) == true ? OpenStringTemplate?.Build(param)
            : type?.Equals("Boolean", StringComparison.OrdinalIgnoreCase) == true ? BooleanTemplate?.Build(param)
            : type?.Equals("RestrictedString", StringComparison.OrdinalIgnoreCase) == true ? RestrictedStringTemplate?.Build(param)
            : type?.Equals("Integer", StringComparison.OrdinalIgnoreCase) == true ? IntegerTemplate?.Build(param)
            :type?.Equals("FilePath", StringComparison.OrdinalIgnoreCase) == true ? FilePathTemplate?.Build(param)
            : DefaultTemplate?.Build(param);
    }

    public bool Match(object? data) => data is ConfigItem;
}
