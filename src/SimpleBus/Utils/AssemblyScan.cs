using System;
using System.Linq;
using System.Reflection;

namespace SimpleBus.Utils
{
    public static class AssemblyScan
    {
        public static Type[] FindConsumers(this Assembly assembly)
        {
            var consumers = assembly
                .GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)))
                .ToArray();

            return consumers;
        }
    }
}   