using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#region Additional namespaces
using ERaceSystem.DAL;
using ERaceSystem.Entities;
using System.ComponentModel;
using ERaceSystem.ViewModels;
using ERaceSystem.ViewModels.Receiving;
using DMIT2018Common.UserControls;

#endregion
namespace ERaceSystem.BLL
{
    [DataObject]
    public class PurchaseOrderController
    {
        [DataObjectMethod(DataObjectMethodType.Select,false)]
        public List<SelectionList>GetActivePurchaseOrderList()
        {
            using (var context = new ERaceSystemContext())
            {
                var result = from x in context.Orders
                             where x.Closed == false && x.OrderNumber != null && x.OrderDate != null
                             orderby x.OrderNumber
                             select new SelectionList
                             {
                                 ValueId = (int)x.OrderID,
                                 DisplayText = x.OrderNumber + " - " + x.Vendor.Name
                             };
                return result.ToList();
            }

        }

        public VendorDetails GetVendorDetails(int OrderId)
        {
            using (var context = new ERaceSystemContext())
            {
                var result = from x in context.Orders
                             where x.OrderID == OrderId
                             select new VendorDetails
                             {
                                 VendorID = x.Vendor.VendorID,
                                 Name = x.Vendor.Name,
                                 Address = x.Vendor.Address + " " + x.Vendor.City,
                                 Phone = x.Vendor.Phone,
                                 Contact = x.Vendor.Contact
                             };
                return result.FirstOrDefault();
            }
        }

        public List<PurchaseOrderDetail> GetPurchaseOrderDetails(int OrderId)
        {
            using (var context = new ERaceSystemContext())
            {

                var result = from x in context.OrderDetails
                             where x.OrderID == OrderId
                             select new PurchaseOrderDetail
                             {
                                 OrderDetailID = x.OrderDetailID,
                                 ItemDescription = x.Product.ItemName,
                                 OrderedUnits = x.OrderUnitSize > 1 ? x.Quantity + " x case of " + x.OrderUnitSize : x.Quantity + " ea",
                                 QtyOrdered = x.Quantity * x.OrderUnitSize,
                                 Unit = x.OrderUnitSize > 1 ? " x case of " + x.OrderUnitSize : " ea",
                                 QtyOutstanding = ((x.Quantity * x.OrderUnitSize -(from y in context.ReceiveOrderItems
                                                                                  where y.OrderDetailID == x.OrderDetailID
                                                                                  select y.ItemQuantity).Sum())==null?(x.Quantity*x.OrderUnitSize): (x.Quantity * x.OrderUnitSize - (from y in context.ReceiveOrderItems
                                                                                                                                                                                     where y.OrderDetailID == x.OrderDetailID
                                                                                                                                                                                     select y.ItemQuantity).Sum()))<0?0: (x.Quantity * x.OrderUnitSize - (from y in context.ReceiveOrderItems
                                                                                                                                                                                                                                                          where y.OrderDetailID == x.OrderDetailID
                                                                                                                                                                                                                                                          select y.ItemQuantity).Sum()) == null ? (x.Quantity * x.OrderUnitSize) : (x.Quantity * x.OrderUnitSize - (from y in context.ReceiveOrderItems
                                                                                                                                                                                                                                                                                                                                                                    where y.OrderDetailID == x.OrderDetailID
                                                                                                                                                                                                                                                                                                                                                                    select y.ItemQuantity).Sum()),
                                 UnitReceived = null,
                                 UnitRejected = null,
                                 Reason = "",
                                 QtySalvaged=null
                             };
                return result.ToList();
            }
        }

        private List<string> errors = new List<string>();
        public void ForceCloseOrder(int OrderId, string Reason, List<ProductInventory> items)
        {
            using (var context = new ERaceSystemContext())
            {
                if (Reason == "")
                {
                    errors.Add("You must provide a reason");
                }
                var exist = (from x in context.Orders
                             where x.OrderID == OrderId && x.Closed == false && x.OrderNumber != null && x.OrderDate != null
                             select x).FirstOrDefault();
                if (exist == null)
                {
                    errors.Add("Order does not exist or already closed");
                }
                foreach(ProductInventory item in items)
                {
                    int productid = (from x in context.OrderDetails
                                     where x.OrderDetailID == item.OrderDetailID
                                     select x.ProductID).FirstOrDefault();
                    var real = (from x in context.Products
                                where x.ProductID == productid
                                select x).FirstOrDefault();
                    if (real == null)
                    {
                        errors.Add("Invalid item");
                    }
                }
                if (errors.Count() > 0)
                {
                    
                    throw new BusinessRuleException("your transaction contains following errors: ", errors);
                }
                else
                {
                    Order purchaseOrder = (from x in context.Orders
                                           where x.OrderID == OrderId
                                           select x).FirstOrDefault();
                    purchaseOrder.Closed = true;
                    purchaseOrder.Comment = Reason;
                    context.Entry(purchaseOrder).State = System.Data.Entity.EntityState.Modified;
                    List < UnOrderedItem > exists = (from x in context.UnOrderedItems
                                                     where x.OrderID == OrderId
                                                     select x).ToList();
                    if (exists != null)
                    {
                        foreach (UnOrderedItem item in exists)
                        {
                            context.UnOrderedItems.Remove(item);
                        }
                    }
                    foreach (ProductInventory item in items)
                    {
                        int productid = (from x in context.OrderDetails
                                         where x.OrderDetailID == item.OrderDetailID
                                         select x.ProductID).FirstOrDefault();
                        var real = (from x in context.Products
                                    where x.ProductID == productid
                                    select x).FirstOrDefault();
                        if (real != null)
                        {
                            real.QuantityOnOrder = real.QuantityOnOrder - item.QtyOutstanding;
                        }
                        context.Entry(real).State = System.Data.Entity.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
        }

        public void DeleteUnorderedItem(int ItemId)
        {
            using (var context = new ERaceSystemContext())
            {

                UnOrderedItem exists = (from x in context.UnOrderedItems
                                              where x.ItemID == ItemId
                                              select x).FirstOrDefault();
                if (exists != null)
                {
                    context.UnOrderedItems.Remove(exists);
                }
                //commit
                context.SaveChanges();
            }
        }

        public void DeleteAllUnorderedItem(int orderid)
        {
            using (var context = new ERaceSystemContext())
            {

                List<UnOrderedItem> exists = (from x in context.UnOrderedItems
                                            where x.OrderID==orderid
                                        select x).ToList();
                foreach(UnOrderedItem item in exists)
                {
                    context.UnOrderedItems.Remove(item);
                }
                //commit
                context.SaveChanges();
            }
        }

        int index = 0;
        public void ReceiveOrder(int orderid, int employeeid, List<ItemReceived>received, List<ItemReturned> returns, List<ItemReturned> rejects,int unclosedRow)
        {
            using (var context = new ERaceSystemContext())
            {
                int employeeCategory = (from x in context.Employees
                                        where x.EmployeeID == employeeid
                                        select x.PositionID).FirstOrDefault();
                if(employeeCategory!=7 && employeeCategory != 9)
                {
                    errors.Add("You are not allowed to receive the shipment");
                }
                else
                {
                    bool close = (from x in context.Orders
                                  where x.OrderID == orderid
                                  select x.Closed).FirstOrDefault();
                    if (close == true)
                    {
                        errors.Add("Order is already closed. You cannot make anymore modification");
                    }
                    foreach (ItemReceived item in received)
                    {
                        int unitsize = (from x in context.OrderDetails
                                        where x.OrderDetailID == item.OrderDetailID
                                        select x.OrderUnitSize).FirstOrDefault();
                        if(item.QtyOutstanding<=0 && item.QtySalvaged > 0)
                        {
                            errors.Add("There is no product outstanding");
                        }
                        if(item.QtySalvaged>0 && item.UnitRejected == 0)
                        {
                            errors.Add("You cannot salvage products without any rejection");
                        }
                        int QtyReceived = item.UnitReceived * unitsize + item.QtySalvaged;
                        if (QtyReceived - item.QtyOutstanding > unitsize)
                        {
                            errors.Add("You receive too much products");
                        }
                        var exists = (from x in context.OrderDetails
                                      where x.OrderDetailID == item.OrderDetailID
                                      select x).FirstOrDefault();
                        if (exists == null)
                        {
                            errors.Add("You cannot receive item not on original order");
                        }
                        if (item.UnitRejected < 0)
                        {
                            errors.Add("Unit reject cannot be negative");
                        }
                        if (item.UnitReceived < 0)
                        {
                            errors.Add("Unit received cannot be negative");
                        }
                        if (item.QtySalvaged < 0)
                        {
                            errors.Add("Unit salvaged cannot be negative");
                        }
                    }
                    foreach (ItemReturned item in returns)
                    {
                        if (item.ItemQuantity <= 0)
                        {
                            errors.Add("Return quantity cannot be 0 or negative");
                        }
                    }
                    foreach (ItemReturned item in rejects)
                    {
                        if (item.ItemUnit <= 0)
                        {
                            errors.Add("Rejected unit must be bigger than 0");
                        }
                        if (item.QtySalvaged < 0)
                        {
                            errors.Add("Quantity salvaged cannot be less than 0");
                        }
                        int unitsize = (from x in context.OrderDetails
                                        where x.OrderDetailID == item.OrderDetailID
                                        select x.OrderUnitSize).FirstOrDefault();
                        if(unitsize==1&& item.QtySalvaged > 0)
                        {
                            errors.Add("You cannot salvaged item if is not bulk");
                        }
                        if (item.ItemUnit > 0 && item.Comment == "")
                        {
                            errors.Add("You must provide a rejection reason");
                        }

                    }
                    if (errors.Count() > 0)
                    {
                        throw new BusinessRuleException("your transaction contains following errors: ", errors);
                    }
                    else
                    {
                        ReceiveOrder table = new ReceiveOrder();
                        table.EmployeeID = employeeid;
                        table.OrderID = orderid;
                        table.ReceiveDate = DateTime.Now;
                        context.ReceiveOrders.Add(table);
                        int receivedOrderID = (from x in context.ReceiveOrders
                                               where x.OrderID == orderid && x.EmployeeID == employeeid
                                               orderby x.ReceiveDate descending
                                               select x.ReceiveOrderID).FirstOrDefault();
                        foreach (ItemReceived item in received)
                        {
                            ReceiveOrderItem row = new ReceiveOrderItem();
                            row.ReceiveOrderID = receivedOrderID;
                            row.OrderDetailID = item.OrderDetailID;
                            int Unitsize = (from x in context.OrderDetails
                                            where x.OrderDetailID == item.OrderDetailID
                                            select x.OrderUnitSize).FirstOrDefault();
                            row.ItemQuantity = item.UnitReceived * Unitsize + item.QtySalvaged;
                            if (item.QtyOutstanding - row.ItemQuantity <= 0)
                            {
                                index++;
                            }
                            context.ReceiveOrderItems.Add(row);
                            int productid = (from x in context.OrderDetails
                                             where x.OrderDetailID == item.OrderDetailID
                                             select x.ProductID).FirstOrDefault();
                            var product = (from x in context.Products
                                           where x.ProductID == productid
                                           select x).FirstOrDefault();
                            product.QuantityOnHand = product.QuantityOnHand + row.ItemQuantity;
                            product.QuantityOnOrder = (product.QuantityOnOrder - row.ItemQuantity) <= 0 ? 0 : (product.QuantityOnOrder - row.ItemQuantity);
                            context.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        }

                        foreach (ItemReturned item in returns)
                        {
                            ReturnOrderItem turnback = new ReturnOrderItem();
                            turnback.ReceiveOrderID = receivedOrderID;
                            turnback.OrderDetailID = null;
                            turnback.UnOrderedItem = item.UnOrderedItem;
                            turnback.ItemQuantity = (int)item.ItemQuantity;
                            turnback.Comment = item.Comment;
                            turnback.VendorProductID = item.VendorProductID;
                            context.ReturnOrderItems.Add(turnback);
                        }
                        foreach (ItemReturned item in rejects)
                        {
                            ReturnOrderItem turnback = new ReturnOrderItem();
                            turnback.ReceiveOrderID = receivedOrderID;
                            turnback.OrderDetailID = item.OrderDetailID;
                            turnback.UnOrderedItem = null;
                            int unitsize = (from x in context.OrderDetails
                                            where x.OrderDetailID == item.OrderDetailID
                                            select x.OrderUnitSize).FirstOrDefault();
                            turnback.ItemQuantity = (int)item.ItemUnit * unitsize - (int)item.QtySalvaged;
                            turnback.Comment = item.Comment;
                            turnback.VendorProductID = null;
                            context.ReturnOrderItems.Add(turnback);
                        }
                        if (index == unclosedRow)
                        {
                            //close the order
                            Order purchaseOrder = (from x in context.Orders
                                                   where x.OrderID == orderid
                                                   select x).FirstOrDefault();
                            purchaseOrder.Closed = true;
                            context.Entry(purchaseOrder).State = System.Data.Entity.EntityState.Modified;
                        }
                        //delete the unordereditem
                        List<UnOrderedItem> exists = (from x in context.UnOrderedItems
                                                      where x.OrderID == orderid
                                                      select x).ToList();
                        if (exists != null)
                        {
                            foreach (UnOrderedItem item in exists)
                            {
                                context.UnOrderedItems.Remove(item);
                            }
                        }
                    }
                    }
                
                
                  //commit
                  context.SaveChanges();
            }
            
        }
        public void InsertUnorderedItem(UnorderedItem item)
        {
            using (var context = new ERaceSystemContext())
            {
                var exists = (from x in context.Orders
                             where x.OrderID == item.OrderID && x.Closed == false
                             select x).FirstOrDefault();
                if (exists==null)
                {
                    errors.Add("The current order does not exist.");
                }
                if (item.ItemName == "")
                {
                    errors.Add("Item Name is required");
                }
                if(item.VendorProductID=="")
                {
                    errors.Add("Vendor Product ID is required");
                }
                if (item.Quantity <= 0)
                {
                    errors.Add("Quantity is required and cannot be negative");
                }
                if (errors.Count > 0)
                {
                    throw new BusinessRuleException("your transaction contains following errors: ", errors);
                }
                else
                {
                    UnOrderedItem temp = new UnOrderedItem();
                    temp.ItemName = item.ItemName;
                    temp.VendorProductID = item.VendorProductID;
                    temp.Quantity = item.Quantity;
                    temp.OrderID = item.OrderID;
                    context.UnOrderedItems.Add(temp);
                    //commit
                    context.SaveChanges();
                }
            }
        }
        public List<UnorderedItem> GetUnorderedItem(int OrderId)
        {
            using (var context = new ERaceSystemContext())
            {
                var result = from x in context.UnOrderedItems
                             where x.OrderID == OrderId
                             select new UnorderedItem
                             {
                                 ItemID=x.ItemID,
                                 ItemName=x.ItemName,
                                 VendorProductID=x.VendorProductID,
                                 Quantity=x.Quantity
                             };
                return result.ToList();
            }
        }
    }
}
