using System.Xml.Schema;

namespace WEB_API.Models.Common
{
	public class PaginationList<T> : List<T>
	{
		/* class định nghĩa cho bất kỳ kiểu dữ liệu nào -> vì thế ta cần sử dụng generic annonymous <T> */

		// thuộc tính thứ nhất tôi đang đứng ở trang nào: PageIndex
		public int PageIndex { get; set; } // what is the page we are standing?
		public int totalPage { get; set; } // how many pages

		// constructor's parameter: 1. list items return "items", 2. total amount records "count" & PageIndex, PageSize
		public PaginationList( List<T> items, int count, int PageIndex, int pageSize ) {
			PageIndex = PageIndex;
			// số nguyên chia số nguyên nó thành số nguyên -> ép kiểu 1 trong 2 đứa về double 
			totalPage = (int)Math.Ceiling(count / (double)pageSize); // total amount pages ex: 20 elements / 3 elements on a page = 7 pages 
			AddRange( items );
			// AddRange(items) thêm nhiều phần tử vào danh sách một lần, thay vì phải dùng vòng lặp và gọi phương thức Add từng phần tử
			// vì class này là một mảng dữ liệu rồi!

			// how do we caculate totalPage -> we will take: 
			/*
 totalPage sẽ làm những gì? ta sẽ phải lấy hàm count tổng số -> totalPage sẽ bằng totalPage Nhưng mà ta có chuyền cái biến totalPage vào đâu?
nếu ko có ta phải định nghĩa Hàm "count" ... Quan trọng nhất là cái item mình trả về (ta sẽ trả về một cái list => vì vậy tôi SẼ DÙNG: " AddRange() ")
_ Cái totalPage của mình dùng để chia lấy dư: count / pageSize
_ làm tròn lên tổng số pageSize 

			AddRange() -> dùng để thêm các phần tử của một tập hợp vào cuối của một danh sách
			-> 
			ví dụ: 
			List<int> list1 = new List<int>() { 1, 2, 3, 4, 5 };
			int[] array1 = new int[] { 6, 7, 8, 9, 10 };
			list1.AddRange(array1);
			
			 */
		}



		// we will check from original list i will take items list like the same list:
		public static PaginationList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
		{
			// hàm nay sẽ lấy danh sách vừa đủ trả về cho client
			var count = source.Count(); // source's length
			var items = source.Skip( (pageIndex - 1)*pageSize ).Take(pageSize).ToList();

			return new PaginationList<T>( items, count, pageIndex, pageSize );

		}
	}
}
