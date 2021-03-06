using System;
using System.Collections.Generic;

namespace MSFramework.Domain.Event
{
	public interface IEventHandlerTypeStore
	{
		bool Add(Type eventType, Type handlerType);
		IEnumerable<Type> GetHandlerTypes(Type eventType);
	}
}