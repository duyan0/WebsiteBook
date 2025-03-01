using BanSach.Models;
using System.Linq;
using System.Web.Http;

namespace BanSach.API
{
    [RoutePrefix("api/KhachHangs")]
    public class KhachHangsApiController : ApiController
    {
        private readonly db_Book db = new db_Book();

        public KhachHangsApiController()
        {
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;
        }

        // GET: api/KhachHangs/GetAllCustomers
        [HttpGet]
        [Route("GetAllCustomers")]
        public IHttpActionResult GetAllCustomers()
        {
            var customers = db.KhachHang.ToList();
            return Ok(customers);
        }

        // GET: api/KhachHangs/GetCustomerById/5
        [HttpGet]
        [Route("GetCustomerById/{id}")]
        public IHttpActionResult GetCustomerById(int id)
        {
            var customer = db.KhachHang.Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        // POST: api/KhachHangs/AddCustomer
        [HttpPost]
        [Route("AddCustomer")]
        public IHttpActionResult AddCustomer(KhachHang khachHang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.KhachHang.Add(khachHang);
            db.SaveChanges();

            return Ok("Thêm khách hàng thành công.");
        }

        // PUT: api/KhachHangs/UpdateCustomer/5
        [HttpPut]
        [Route("UpdateCustomer/{id}")]
        public IHttpActionResult UpdateCustomer(int id, KhachHang khachHang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCustomer = db.KhachHang.Find(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            existingCustomer.TenKH = khachHang.TenKH;
            existingCustomer.SoDT = khachHang.SoDT;
            existingCustomer.Email = khachHang.Email;
            db.Entry(existingCustomer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Ok("Cập nhật khách hàng thành công.");
        }

        // DELETE: api/KhachHangs/DeleteCustomer/5
        [HttpDelete]
        [Route("DeleteCustomer/{id}")]
        public IHttpActionResult DeleteCustomer(int id)
        {
            var customer = db.KhachHang.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.KhachHang.Remove(customer);
            db.SaveChanges();

            return Ok("Xóa khách hàng thành công.");
        }
    }
}
