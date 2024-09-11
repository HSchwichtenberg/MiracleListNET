using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
#nullable enable

namespace ITVisions.Logging;

/// <summary>
/// Protokollierung mit einer beliebigen Methode, die eine Zeichenkette erwartet
/// </summary>
public sealed class UniversalLoggerProvider : ILoggerProvider
{
 private readonly IDisposable? _onChangeToken;
 private UniversalLoggerConfiguration _currentConfig;
 private readonly ConcurrentDictionary<string, UniversalLogger> _loggers =
     new(StringComparer.OrdinalIgnoreCase);

 public Action<string> LogTo { get; }

 public UniversalLoggerProvider(Action<string> logTo)
 {
  LogTo = logTo;
 }

 public ILogger CreateLogger(string categoryName) =>
    _loggers.GetOrAdd(categoryName, name => new UniversalLogger(name, () => new UniversalLoggerConfiguration(), LogTo));

 private UniversalLoggerConfiguration GetCurrentConfig() => _currentConfig;

 public void Dispose()
 {
  _loggers.Clear();
  _onChangeToken?.Dispose();
 }
}


public sealed class UniversalLoggerConfiguration
{
 public int EventId { get; set; }

 public Dictionary<LogLevel, ConsoleColor> LogLevelToColorMap { get; set; } = new()
 {
  [LogLevel.Information] = ConsoleColor.Green
 };
}

public sealed class UniversalLogger : ILogger
{
 private readonly string _name;
 private readonly Func<UniversalLoggerConfiguration> _getCurrentConfig;
 public Action<string> LogTo { get; }

 public UniversalLogger(
     string name,
     Func<UniversalLoggerConfiguration> getCurrentConfig, Action<string> logTo) =>
     (_name, _getCurrentConfig, LogTo) = (name, getCurrentConfig, logTo);

 public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

 public bool IsEnabled(LogLevel logLevel) =>
     _getCurrentConfig().LogLevelToColorMap.ContainsKey(logLevel);

 public void Log<TState>(
     LogLevel logLevel,
     EventId eventId,
     TState state,
     Exception? exception,
     Func<TState, Exception?, string> formatter)
 {
  if (!IsEnabled(logLevel))
  {
   return;
  }

  UniversalLoggerConfiguration config = _getCurrentConfig();
  if (config.EventId == 0 || config.EventId == eventId.Id)
  {

   LogTo($"[{eventId.Id,2}: {logLevel,-12}] {_name} -  {formatter(state, exception)}");

  }
 }
}