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
using ERaceSystem.ViewModels.Sales;

#endregion

namespace ERaceSystem.BLL
{
    [DataObject]
    public class StoreRefundController
    {
        public RefundInvoice Invoice_FindById(int invoiceid)
        {           

            using (var context = new ERaceSystemContext())
            {
                RefundInvoice invoice = new RefundInvoice();
                invoice = (from inv in context.Invoices
                           where inv.InvoiceID == invoiceid
                           select new RefundInvoice
                           {
                               InvoiceID = inv.InvoiceID,
                               InvoiceDate = inv.InvoiceDate,
                               EmployeeID = inv.EmployeeID,
                               SubTotal = inv.SubTotal,
                               GST = inv.GST,
                               Total = inv.Total
                           }).FirstOrDefault();
                
                return invoice;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<RefundItem> Invoicedetails_FindById(int invoiceid)
        {

            using (var context = new ERaceSystemContext())
            {
                List<RefundItem> invoicedetails = null;

              
                RefundItem detail = new RefundItem();

                invoicedetails = (from det in context.InvoiceDetails
                                  where det.InvoiceID == invoiceid
                                  select new RefundItem
                                  {
                                      InvoiceID = det.InvoiceID,
                                      ProductID = det.ProductID,
                                      ItemName = det.Product.ItemName,
                                      Quantity = det.Quantity,
                                      Price = det.Price,
                                      Reason = "",
                                      RestockCharge = det.Product.ReStockCharge
                                  }).ToList();

                foreach (RefundItem item in invoicedetails)
                {
                    detail = new RefundItem();
                    detail.InvoiceID = item.InvoiceID;
                    detail.ProductID = item.ProductID;
                    detail.ItemName = item.ItemName;
                    detail.Quantity = item.Quantity;
                    detail.Price = item.Price;
                    detail.Reason = "";
                    detail.RestockCharge = item.RestockCharge;
                    detail.Amount = item.Price * item.Quantity;
                    invoicedetails.Add(detail);
                }
                return invoicedetails;
            }
        }
    }
}
