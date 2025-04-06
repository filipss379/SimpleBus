using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBus
{
    public class RegisteredConsumersByMessageType
    {
        public RegisteredConsumersByMessageType(Type[] consumersTypes)
        {
            ConsumersByMessageType = new Dictionary<Type, List<Type>>();
            foreach (var consumer in consumersTypes)
            {
                var messageTypes = consumer
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
                    .Select(i => i.GetGenericArguments()[0]);

                foreach (var messageType in messageTypes)
                {
                    if (!ConsumersByMessageType.ContainsKey(messageType))
                    {
                        ConsumersByMessageType[messageType] = new List<Type>();
                    }
                    ConsumersByMessageType[messageType].Add(consumer);
                }
            }
        }
        
        public Dictionary<Type, List<Type>> ConsumersByMessageType { get; }

        public Type[] GetMessagesTypes() => ConsumersByMessageType.Keys.ToArray();
    }
}