using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoreReplanishment.Application;
using StoreReplenishment.Domain;
using StoreReplenishmentApi.Dto;

namespace StoreReplenishmentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }
        
        [HttpPost]
        public IEnumerable<Order> ProduceOrder(OrderRequest orderRequest)
        {  
            return _orderService.ProduceOrder(orderRequest.Products, orderRequest.BatchSizes, orderRequest.ProductBatchSizes, 
                orderRequest.BatchQuantities, orderRequest.BatchSizeSelection);
        }
    }
}
