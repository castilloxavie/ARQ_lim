using System;
using Domain.Interfaces;

namespace Infrastructure.Logging;

public class ConsoleLogger : IAppLogger
{
    public void Log(string message)
    {
        Console.WriteLine("[LOG] " + DateTime.Now + " - " + message);
    }
}