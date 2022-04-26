using Core.Messages.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Models
{
    public class OrderAggregateRoot
    {
        public Guid Id { get; set; }

        public IDictionary<string, decimal> Items { get; set; }

        public List<CreateOrderCommand> Events { get; set; }

        public OrderAggregateRoot(CreateOrderCommand command)
        {
            this.Events = new List<CreateOrderCommand>();

            this.Apply(command);
        }

        private void Apply(CreateOrderCommand command)
        {
            this.Events.Add(command);

            this.Id = command.Id;
            this.Items = command.Items;
        }
    }
}
