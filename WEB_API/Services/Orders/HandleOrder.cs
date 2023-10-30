using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Orders;
using WEB_API.Entities;

namespace WEB_API.Services.Orders
{
    public class HandleOrder
    {

        private readonly WebApiClass3Context _context;

        private int _userId;
        private string _shipAddress;
        private int _cityShipId ;
        private string _tel;
        private int _paymentMethodId;

        // userId     ShipAddress    cityShipId    tel    paymentMethodId

        public HandleOrder( WebApiClass3Context context,  int userId, string ShipAddress, int cityShipId,string tel ,int paymentMethodId) { 
            _context = context;
            _userId = userId;
            _shipAddress = ShipAddress;
            _cityShipId = cityShipId;
            _tel = tel;
            _paymentMethodId = paymentMethodId;
        }

        // LỖI ĐANG SỬ DỤNG QUÁ NHIỀU ĐỐI TƯỢNG _CONTEXT TRONG CÙNG MỘT THỜI ĐIỂM: 168 179 46 
       
        public async Task<int> HandleToOrder()
        {
            // sử dụng tạo một invoice id cho user:
            string invoice_id = await RandomString();

            // lấy ra phương thức thanh toán để xem người dùng thanh toán bằng phương thức nào?
            var payment_method = _context.MethodPayments.FirstOrDefault(p => p.Id == _paymentMethodId);

            if (payment_method == null)
            {
                return 4000;
            }

            // tính toán lấy ra tổng giá trị của sản phẩm trong cart:
            int price_cart = await compute_total_price();

           

            if (payment_method.Id == 1)
            {
                // XỬ LÝ THANH TOÁN OFFLINE:
                // * để cho bước này ngắn gọn thì mình sẽ tạo một method cùng làm 1 hành động nhưng truyền vào các tham số khác nhau để nó xử lý cho gọn:

                
                //await Task.WhenAll(CreateTblOrder(price_cart, invoice_id, 3, 2), createOrderDetail());

                //CREATE INVOICE:
                await CreateInvoice(invoice_id, price_cart, 2);
                // CREATE ORDER:
                await CreateTblOrder(price_cart, invoice_id, 1, 2);
                // CREATE ORDER DETAIL:
                await CreateOrderDetail();
                
                


            }
            else
            {
                // XỬ LÝ THANH TOÁN ONLINE:

                //CREATE INVOICE;
               await CreateInvoice(invoice_id, price_cart, 3);
                //CREATE ORDER:
                await CreateTblOrder(price_cart, invoice_id, 3, 2);
                // CREATE ORDER DETAIL:
              await CreateOrderDetail();

                
              //  await Task.WhenAll(CreateInvoice(invoice_id, price_cart, 3), CreateTblOrder(price_cart, invoice_id, 3, 2), CreateOrderDetail());




            }



            return 0002;
        }

        // CREATE ORDER:
        private async Task CreateTblOrder(int price_cart, string InvoiceId, int status_id, int ship_id) {
            var Order = new Entities.Order()
            {
                UserId = _userId,
                CreatedAt = DateTime.Now,
                GrandTotal = price_cart,
                ShippingAddress = _shipAddress,
                Tel = _tel,
                InvoiceId = InvoiceId,
                StatusId = status_id,
                ShipingId = ship_id,
                IdCityShip = _cityShipId,
                PaymentMethodId = _paymentMethodId

            };

            await _context.Orders.AddAsync(Order);
            await _context.SaveChangesAsync();
        }

        // CREATE ORDER DETAIL:
        private async Task CreateOrderDetail()
        {
            // vì là hàm này ko cần phải lưu nhanh chóng vội vàng nên ta đặt nó ko cần async
            var carts = await _context.Carts.Where( c => c.UserId == _userId ).ToListAsync();
            var order_id = await _context.Orders.Where( o => o.UserId == _userId ).OrderByDescending(o => o.Id).FirstOrDefaultAsync();
            if (order_id == null)
            {
                return;
            }

            foreach ( var c in carts )
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == c.ProductId);
                // XỬ LÝ NẾU SẢN PHẨM .IsActive()
               await _context.OrderProducts.AddAsync(new OrderProduct()
                {
                    ProductId = c.ProductId,
                    OrderId = order_id.Id,
                    BuyQty = c.BuyQty,
                    Price = product.Price
                });
            }

            _context.Carts.RemoveRange(carts);
            await _context.SaveChangesAsync();
        }


        //CREATE INVOICE:
        private async Task CreateInvoice(string InvoiceNo, int total_price, int status_id)
        {
            var CityShip = await _context.CityShippings.FirstOrDefaultAsync(c => c.Id == _cityShipId);
            var method_p = await _context.MethodPayments.FirstOrDefaultAsync( p => p.Id == _paymentMethodId);
            var status_p = await _context.Statuses.FirstOrDefaultAsync( s => s.Id == status_id);

            var Invoice = new Invoice()
            {
                InvoiceNo = InvoiceNo,
                CreatedAt = DateTime.Now,
                TotalMoney = price_invoice(total_price, (int)(CityShip.PriceShipping)),
                PaymentMethod = method_p.Name,
                City = CityShip.Name,
                Status = status_p.Name
            };

           await _context.Invoices.AddAsync(Invoice);
           await _context.SaveChangesAsync();
            return;
        }

        private int price_invoice(int total_price,int price_ship)
        {
            return total_price += (int)(total_price * 0.01) + price_ship;
        }


        /*
        private async Task<int> compute_total_price()
        {
            var cart_user = await _context.Carts.Where(c => c.UserId == _userId).ToListAsync();
            // khai báo một biến để chứa tổng tiền:
            int a = 0;
            var tasks = new List<Task>();
            foreach (var cart in cart_user)
            {
                var task = Task.Run(async () =>
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cart.ProductId);
                    // xử lý nếu sản phẩm bị tắt và ko lấy được: if product.isActive == false thì bỏ qua ko đưa vào đơn hàng:
                    int value = (int)(product.Price * cart.BuyQty);
                    Interlocked.Add(ref a, value);

                    // lưu giá trị vào biến total_price bằng phương pháp: "Interlocked.Add()"
                });
                // thêm task vừa xử lý vào list tasks
                tasks.Add(task);
            }
            // chờ cho các list tasks xử lý hoàn thành:
            await Task.WhenAll(tasks);
            return a;
        }
        // vì thằng _context là là một trường non-static -> để sử dụng nó phải bỏ static
        */


        // hàm trên sai vì nó đang bị xung đột DBContext:
        private async Task<int> compute_total_price()
        {
            var cart_user = await _context.Carts.Where(c => c.UserId == _userId).ToListAsync();
            // khai báo một biến để chứa tổng tiền:
            int a = 0;
            foreach (var cart in cart_user)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cart.ProductId);
                // xử lý nếu sản phẩm bị tắt và ko lấy được: if product.isActive == false thì bỏ qua ko đưa vào đơn hàng: 
                
                //ép kiểu cho dễ
                int value = (int)(product.Price * cart.BuyQty);
                a += value;

            }

            return a;

        }
        /*
         RÚT KINH NGHIỆM:
         bạn đang sử dụng Task.Run để tạo ra các luồng mới để truy vấn dữ liệu từ cơ sở dữ liệu. Điều này có thể gây ra lỗi
        khi các luồng này cùng truy cập đến cùng một đối tượng _context. Bạn không cần thiết phải sử dụng Task.Run trong 
        trường hợp này, vì bạn đã sử dụng các phương thức bất đồng bộ của DbContext như FindAsync và ToListAsync. 
         */







        /*
                 private static string RandomString(int length = 6)
               {
                   Random random = new Random();
                   string Character = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                   return new string(Enumerable.Repeat(Character, length).Select(s => s[random.Next(s.Length)]).ToArray());
               }

               ÁP DỤNG KIỂU ASYNC AWAIT CHO METHOD NÀY:
                */
        private static async Task<string> RandomString(int length = 6)
        {
            Random random = new Random();
            string Character = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // trong trường hợp này vì làm một công việc duy nhất nên ta chỉ tạo 1 task -> nếu như sử dụng vòng for or cái gì khác lặp lại thì ta cần sử dụng list task để xử lý nhé!
            // trong một Task.Run() => ta luôn đưa vào một callback để xử lý vấn đề bên trong nó! -> giống nhưu promise new Promise ( (resolve, reject) => { // handle; resolve( result ); } )
            Task<string> task = Task.Run(() => new string(Enumerable.Repeat(Character, length).Select(s => s[random.Next(s.Length)]).ToArray()));

            string result  = await task;
            return result;

        }

    }
}


/*
 
mã 4000: ko có đối tượng
mã 0002: thành công
_ private async Task funcA(){} là trả về void 
_   var task = Task.Run(() => {  });
nếu như trong xử lý trong task mà có chứa ví dụ: firstOrDefaultAsync() thì: ask.Run( async () => { // xử lý await })
_ Interlocked.Add() là gì: là method in c# | cách sd: dùng để cộng 2 số nguyên, THAY THẾ số nguyên ĐẦU TIỀN = TỔNG CỦA 2 SỐ ... 
ví dụ: int a = 2 -> Interlocked.Add( ref a, 10 )  => a = 12;
 */
