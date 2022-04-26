using Core.Messages.Commands;
using Infrastructure.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace TechnicalTest.Controllers
{
    [ApiController]
    [Route("order")]
    public class OrdersController : ControllerBase
    {
        private IMediator Mediator { get; }

        public OrdersController(IMediator mediator)
        {
            this.Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public async Task<IActionResult> Create (CreateOrderApiRequest request)
        {
            CreateOrderCommand command = new CreateOrderCommand
            {
                Id = request.Id,
                Items = request.Items
            }; 

            try
            {
                await Mediator.Send(command);

                return this.StatusCode(201);
            }
            catch(Exception)
            {
                return this.UnprocessableEntity();
            }
        }
    }
}
