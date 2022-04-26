using Core.Domain.Models;
using Core.Messages.Commands;
using Core.Messages.Events;
using Core.Persistence;
using Light.GuardClauses;
using MassTransit;
using MediatR;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Handlers.Commands
{
    public class OrderCommandHandler : IRequestHandler<CreateOrderCommand>
    {
        private IBus BusClient { get; }

        private IOrderAggregateRootRepository Repository { get; }

        //private IWritingContext<Order> orders; 

        public OrderCommandHandler(
            IOrderAggregateRootRepository repo,
            IBus bus)
        {
            this.Repository = repo.MustNotBeDefault();
            this.BusClient = bus.MustNotBeDefault(nameof(bus));
        }

        public Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            OrderAggregateRoot aggregate = new OrderAggregateRoot(request);
            //orders.save(aggregate)

            Repository.Insert(request);

            var @event = new OrderCreatedEvent
            {
                Id = request.Id,
                Correlation = request.Id,
                Items = request.Items,
            };

            //Transactions are obviously wrong at this point. In case an exception is thrown during the creation/publishing of the event
            //the entire handling (inclusing the storing of the aggregate) should be reverted via rollback. There is nothing to do here anyway
            //The transaccion should be handled across the thread. This requires a more complex database pooling that the one presented
            //here
            this.BusClient.Publish(@event);
            
            return Unit.Task;
        }
    }
}
