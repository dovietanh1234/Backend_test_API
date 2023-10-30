using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_API.Entities;
using WEB_API.Models.Orders_model;
using WEB_API.Services.Orders;

namespace WEB_API.Controllers.Order
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly WebApiClass3Context _context;

        public OrdersController(WebApiClass3Context context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("payment")]
        public async Task<IActionResult> Order(OrderModel request)
        {

            
                // cần thêm cart -> xem lại user in DB để chạy:
                HandleOrder new_user = new HandleOrder(_context, request.userId, request.ShipAddress, request.cityShipId, request.tel, request.paymentMethodId);
            await new_user.HandleToOrder();
            return Ok( "payment success" );
           

           
        }   



    }
}
