using Core.Messages.Commands;
using Core.Persistence;
using Light.GuardClauses;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Handlers.Commands
{
    public class OrderCommandHandler : IRequestHandler<CreateOrderCommand>
    {
        private IBus BusClient { get; }

        public OrderCommandHandler(
            IOrderAggregateRootRepository repo,
            IBus bus)
        {
            var  _ = repo.MustNotBeDefault();
            this.BusClient = bus.MustNotBeDefault(nameof(bus));
        }

        public Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // TODO: atomically instantiate a new OrderAggregateRoot, persist it and send OrderCreatedEvent
            

            return Unit.Task;
        }
    }
}
