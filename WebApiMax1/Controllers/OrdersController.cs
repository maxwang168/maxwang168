using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using WebApiMax1.Models;

namespace WebApiMax1.Controllers
{
    /*
    WebApiConfig 類別可能需要其他變更以新增此控制器的路由，請將這些陳述式合併到 WebApiConfig 類別的 Register 方法。注意 OData URL 有區分大小寫。

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using WebApiMax1.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Orders>("Orders");
    builder.EntitySet<Order_Details>("Order_Details"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

    /// <summary>
    /// http://localhost:27135/odata/Orders?$select=OrderID,CustomerID
    /// http://localhost:27135/odata/Orders?$sortby=OrderID
    /// </summary>
    public class OrdersController : ODataController
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: odata/Orders
        [EnableQuery]
        public IQueryable<Orders> GetOrders()
        {
            return db.Orders;
        }

        // GET: odata/Orders(5)
        [EnableQuery]
        public SingleResult<Orders> GetOrders([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(orders => orders.OrderID == key));
        }

        // PUT: odata/Orders(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Orders> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Orders orders = db.Orders.Find(key);
            if (orders == null)
            {
                return NotFound();
            }

            patch.Put(orders);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdersExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(orders);
        }

        // POST: odata/Orders
        public IHttpActionResult Post(Orders orders)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(orders);
            db.SaveChanges();

            return Created(orders);
        }

        // PATCH: odata/Orders(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Orders> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Orders orders = db.Orders.Find(key);
            if (orders == null)
            {
                return NotFound();
            }

            patch.Patch(orders);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdersExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(orders);
        }

        // DELETE: odata/Orders(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Orders orders = db.Orders.Find(key);
            if (orders == null)
            {
                return NotFound();
            }

            db.Orders.Remove(orders);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Orders(5)/Order_Details
        [EnableQuery]
        public IQueryable<Order_Details> GetOrder_Details([FromODataUri] int key)
        {
            return db.Orders.Where(m => m.OrderID == key).SelectMany(m => m.Order_Details);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrdersExists(int key)
        {
            return db.Orders.Count(e => e.OrderID == key) > 0;
        }
    }
}
