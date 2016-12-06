using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Poco.Core
{
    // Based on http://blog.flimflan.com/ASimpleColorConsoleTraceListener.html
    public class ColorConsoleTraceListener : ConsoleTraceListener
    {
        Dictionary<TraceEventType, ConsoleColor> eventColor = new Dictionary<TraceEventType, ConsoleColor>()
        { 
            { TraceEventType.Verbose,       ConsoleColor.DarkGray },
            { TraceEventType.Information,   ConsoleColor.Gray },
            { TraceEventType.Warning,       ConsoleColor.Yellow },
            { TraceEventType.Error,         ConsoleColor.Red },
            { TraceEventType.Critical,      ConsoleColor.DarkRed },
            { TraceEventType.Start,         ConsoleColor.DarkCyan },
            { TraceEventType.Stop,          ConsoleColor.DarkCyan },  
        };

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            TraceEvent(eventCache, source, eventType, id, "{0}", message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = getEventColor(eventType, originalColor);
            base.TraceEvent(eventCache, source, eventType, id, format, args);
            Console.ForegroundColor = originalColor;
        }

        ConsoleColor getEventColor(TraceEventType eventType, ConsoleColor defaultColor)
        {
            return eventColor.ContainsKey(eventType) ? eventColor[eventType] : defaultColor;
        }
    }
}
