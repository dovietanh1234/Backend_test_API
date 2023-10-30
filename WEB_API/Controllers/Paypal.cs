using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System.Collections.Generic;
using System.Threading.Tasks;
using WEB_API.Common.Dependency_injection;
using Microsoft.AspNetCore.Http;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;
using WEB_API.Entities;
using WEB_API.Models.Product;

namespace WEB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Paypal : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly SaboxPaypal _saboxPaypal;
        private readonly HttpResponse _response;
        private readonly WebApiClass3Context _context;

        public Paypal(IConfiguration configuration, WebApiClass3Context context)
        {
            _configuration = configuration;
            _response = null;
            _context = context;
            _saboxPaypal = new SaboxPaypal(_configuration);
        }

        [HttpPost]
        [Route("payment")]
        public async Task<IActionResult> payment(int UserId)
        {
            // Construct a request object and set desired parameters
            // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders

            var data = _context.Products.Where(p => p.Price < 1000).ToList();
            var user = _context.User2s.Where(u => u.Id == UserId).OrderByDescending(u => u.Id).FirstOrDefault();

            var quantity = 2;
            var item_total = 0.0;
            var shipping_cost = 10;

            // tạo list chứa các sản phẩm trong cart:
            List<Item> cart_new = new List<Item>();

            foreach (var product in data)
            {
                item_total += (double)(product.Price) * quantity;
                cart_new.Add(new Item()
                {
                    Name = product.Name,
                    UnitAmount = new Money()
                    {
                        CurrencyCode = "USD",
                        Value = product.Price.ToString()
                    },
                    Quantity = quantity.ToString()
                });
            }



            var order = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = "USD",
                            Value = (item_total + (item_total*0.01) + shipping_cost ).ToString(),
                            AmountBreakdown = new AmountBreakdown()
                            {
                                ItemTotal = new Money()
                                {
                                    CurrencyCode = "USD",
                                    Value = item_total.ToString()
                                },
                                TaxTotal = new Money()
                                {
                                    CurrencyCode = "USD",
                                    Value = (item_total*0.01).ToString()
                                },
                                Shipping = new Money()
                                {
                                    CurrencyCode = "USD",
                                    Value = shipping_cost.ToString()
                                }
                            }
                        },
                        Items = cart_new,
                        Payee = new Payee(){
                            Email = "conbonha2k@gmail.com"
                        },
                        ShippingDetail = new ShippingDetail()
                        {
                            Name = new Name()
                            {
                                FullName = user.Name
                            },
                            AddressPortable = new AddressPortable()
                            {
                                AddressLine1 = user.Address,
                                AddressLine2 = user.Address,
                                AdminArea1 = user.Address,
                                AdminArea2 = user.Address,
                                PostalCode = "000084",
                                CountryCode = "VN"
                            }
                        },
                        InvoiceId = RandomString()
                    }
                },
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl = "https://localhost:7290/api/Paypal/success",
                    CancelUrl = "https://localhost:7290/api/Paypal/cancel"
                }
            };

            // Call API with your client and get a response for your call:
            // client: _saboxPaypal.client();
            // response: _response
            try
            {
            var request = new OrdersCreateRequest();

            request.Prefer("return=representation");
            request.RequestBody(order);

            // ĐANG LỖI ĐOẠN NÀY
            var response = await _saboxPaypal.client().Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                return BadRequest();
            }

            PayPalCheckoutSdk.Orders.Order result = response.Result<PayPalCheckoutSdk.Orders.Order>();
                return Ok(new
                {
                    link1 = result.Links[0].Href,
                    link2 = result.Links[1].Href,
                    link3 = result.Links[2].Href
                });

            }catch(Exception ex)
            {
                return BadRequest( ex.Message );
            }
              
        }

        [HttpGet]
        [Route("success")]
        public async Task<IActionResult> payment_success( string PayerID, string token)
        {
            // tạo yêu cầu xác nhận thanh toán:
            var request = new OrdersCaptureRequest(token);
            request.RequestBody(new OrderActionRequest());

            // gửi yêu cầu và nhận phản hồi:
            var response = await _saboxPaypal.client().Execute(request);

           // if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{
                // xác nhận thanh toán thất bại:
              //  return BadRequest("payment fail");
            //}
            // xoá cart -> gửi mail client -> có hoá đơn lịch sử ...


            return Ok( "payment success" );
        }

        [HttpGet]
        [Route("cancel")]
        public IActionResult payment_cancel()
        {

            return Forbid("fail to payment");
        }

        [HttpPost]
        [Route("payment_cash")]
        public IActionResult payment_cash(int UserId)
        {
            var data = _context.Products.Where(p => p.Price < 1000).ToList();
            var user = _context.User2s.Where(u => u.Id == UserId).OrderByDescending(u => u.Id).FirstOrDefault();

            var quantity = 2;
            var item_total = 0.0;

            // tạo list chứa các sản phẩm trong cart:
            List<Cart2> cart_new = new List<Cart2>();

            foreach (var product in data)
            {
                item_total += (double)(product.Price) * quantity;
                cart_new.Add(new Cart2()
                {
                    Name = product.Name,
                    UnitAmount = new Money()
                    {
                        CurrencyCode = "USD",
                        Value = product.Price.ToString()
                    },
                    Quantity = quantity.ToString()
                });
            }



            return Ok( new
            {
                data1 = user.Name,
                data2 = cart_new,
                data3 = item_total
            } );
        }



        private static string RandomString(int length = 6)
        {
            Random random = new Random();

            string Character = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(Character, length).Select(s => s[random.Next(s.Length)]).ToArray());

        }

        




    }
}


/*
 HttpResponse: Bạn có thể sử dụng lớp này để thiết lập các thuộc 
tính như loại nội dung, mã trạng thái, thông điệp lý do,
tiêu đề và nội dung của phản hồi HTTP. Bạn cũng có thể sử dụng 
các phương thức của lớp này để xóa, đệm, gửi hoặc chuyển hướng phản hồi HTTP
 */