using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace NXA.SC.Caas.Shared.Utils
{
    internal static class ApplicationLogging
    {
        internal static ILoggerFactory? LoggerFactory { get; set; } = new LoggerFactory();
        internal static ILogger? CreateLogger<T>() => LoggerFactory?.CreateLogger<T>();
        internal static ILogger? CreateLogger(string categoryName) => LoggerFactory?.CreateLogger(categoryName);
    }
}
