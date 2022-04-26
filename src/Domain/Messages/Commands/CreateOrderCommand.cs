using Core.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messages.Commands
{
    public class CreateOrderCommand : IRequest
    {
        public Guid Id { get; set; }

        public IDictionary<string, decimal> Items { get; set; }
    }
}
