using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.WebApi.Models
{
    public class CreateOrderApiRequest
    {
        public Guid Id { get; set; }

        public IDictionary<string, decimal> Items { get; set; }
    }
}
