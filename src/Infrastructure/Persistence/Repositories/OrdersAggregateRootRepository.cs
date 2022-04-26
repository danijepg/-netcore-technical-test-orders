using Core.Messages.Events;
using Core.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using Core.Messages.Commands;

namespace Infrastructure.Persistence.Repositories
{
    // This should not exist
    public class OrdersAggregateRootRepository : IOrderAggregateRootRepository
    {
        private IDbConnection Connection { get; }

        public OrdersAggregateRootRepository(IDbConnection connection)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task Insert(CreateOrderCommand command)
        {
            string sql = "INSERT INTO Orders (event_id, order_id, payload) VALUES (@OrderId, @Payload)";

            await Connection.ExecuteAsync(sql, new { OrderId = command.Id, Payload = command });
        }
    }
}
