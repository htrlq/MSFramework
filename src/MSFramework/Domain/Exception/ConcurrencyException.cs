using System;

namespace MSFramework.Domain.Exception
{
	public class ConcurrencyException : MSFrameworkException
	{
		public ConcurrencyException(Guid id)
			: base($"A different version than expected was found in aggregate {id}")
		{
		}
	}
}