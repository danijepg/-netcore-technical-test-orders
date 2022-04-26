using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.Messages.Commands;
using Core.Messages.Events;

namespace Core.Persistence
{
    public interface IOrderAggregateRootRepository
    {
        Task Insert(CreateOrderCommand model);
    }
}
