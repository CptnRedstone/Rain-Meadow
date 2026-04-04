using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RainMeadow
{
    public class InvalidProgrammerException : InvalidOperationException
    {
        public InvalidProgrammerException(string message) : base(message + " you goof") { }
    }

    public partial class RainMeadow
    {
        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Message = 2,
            Warn = 3,
            Error = 4,
            Fatal = 5,
        }
        private static string TrimCaller(string callerFile) { return (callerFile = callerFile.Substring(Mathf.Max(callerFile.LastIndexOf(Path.DirectorySeparatorChar), callerFile.LastIndexOf(Path.AltDirectorySeparatorChar)) + 1)).Substring(0, callerFile.LastIndexOf('.')); }
        private static string LogTime() { return ((int)(Time.time * 1000)).ToString(); }
        private static string LogDOT() { return DateTime.Now.ToUniversalTime().TimeOfDay.ToString().Substring(0, 8); }
        // Note: we use Logger.Info because Bepinex ships with logging level info by default. We are promoting our debugs to infos here so they get through.
        public static void DebugMe([CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            if (RainMeadow.rainMeadowOptions.CurrentLogLevel.Value <= LogLevel.Debug)
                instance.Logger.LogInfo($"{LogDOT()}|{LogTime()}|{TrimCaller(callerFile)}.{callerName}");
        }
        /// <summary> Used for engine-level logging events. </summary>
        public static void LogDebug(object data, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            if (RainMeadow.rainMeadowOptions.CurrentLogLevel.Value <= LogLevel.Debug)
                instance.Logger.LogDebug($"{LogDOT()}|{LogTime()}|{TrimCaller(callerFile)}.{callerName}:{data}");
        }
        /// <summary> Used for method calls, and developer insight. </summary>
        public static void LogInfo(object data, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            if (RainMeadow.rainMeadowOptions.CurrentLogLevel.Value <= LogLevel.Info)
                instance.Logger.LogInfo($"{LogDOT()}|{LogTime()}|{TrimCaller(callerFile)}.{callerName}:{data}");
        }
        /// <summary> Used for player insight. </summary>
        public static void LogMessage(object data, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            if (RainMeadow.rainMeadowOptions.CurrentLogLevel.Value <= LogLevel.Message)
                instance.Logger.LogMessage($"{LogDOT()}|{LogTime()}|{TrimCaller(callerFile)}.{callerName}:{data}");
        }
        /// <summary> Used for failsafes or potential faults. </summary>
        public static void LogWarning(object data, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            if (RainMeadow.rainMeadowOptions.CurrentLogLevel.Value <= LogLevel.Warn)
                instance.Logger.LogWarning($"{LogDOT()}|{LogTime()}|{TrimCaller(callerFile)}.{callerName}:{data}");
        }
        /// <summary> Used for errors idk what you're expecting. </summary>
        public static void LogError(object data, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            if (RainMeadow.rainMeadowOptions.CurrentLogLevel.Value <= LogLevel.Error)

                instance.Logger.LogError($"{LogDOT()}|{LogTime()}|{TrimCaller(callerFile)}.{callerName}:{data}");
        }
        /// <summary> Used for uncaught, game-breaking exceptions. </summary>
        public static void LogFatal(object data, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            if (RainMeadow.rainMeadowOptions.CurrentLogLevel.Value <= LogLevel.Fatal)

                instance.Logger.LogFatal($"{LogDOT()}|{LogTime()}|{TrimCaller(callerFile)}.{callerName}:{data}");
        }

        [Conditional("TRACING")]
        public static void Stacktrace()
        {
            var stacktrace = Environment.StackTrace;
            stacktrace = stacktrace.Substring(stacktrace.IndexOf('\n') + 1);
            stacktrace = stacktrace.Substring(stacktrace.IndexOf('\n'));
            instance.Logger.LogInfo(stacktrace);
        }

        [Conditional("TRACING")]
        public static void Dump(object data, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            var dump = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = ShallowJsonDump.customResolver,
                Converters = new List<JsonConverter>() { new ShallowJsonDump() }

            });
            instance.Logger.LogInfo($"{LogDOT()}|{LogTime()}|{TrimCaller(callerFile)}.{callerName}:{dump}");
        }

        // tracing stays on for one net-frame after pressing L
        public static bool tracing;
        // this better captures the caller member info for delegates/lambdas at the cost of using the stackframe
        [Conditional("TRACING")]
        public static void Trace(object data, [CallerFilePath] string callerFile = "")
        {
            if (tracing)
            {
                instance.Logger.LogInfo($"{LogDOT()}|{LogTime()}|{TrimCaller(callerFile)}.{new StackFrame(1, false).GetMethod()}:{data}");
            }
        }
    }
}
