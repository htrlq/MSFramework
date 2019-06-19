﻿using System;
using MediatR;
using MSFramework.AspNetCore;
using Ordering.Domain.AggregateRoot;

namespace Ordering.Application.Command
{
	public class ChangeOrderAddressCommand : IRequest<ApiResult>
	{
		public Address NewAddress { get; set; }
		
		public Guid OrderId { get; set; }
    }
}