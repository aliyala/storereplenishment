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
        public IEnumerable<Order> ProduceOrders(ProductData productData)
        {
            return _orderService.ProduceOrders(productData.Products, productData.BatchSizes, productData.ProductBatchSizes,
                productData.BatchQuantities, productData.BatchSizeSelection);
        }
    }
}
