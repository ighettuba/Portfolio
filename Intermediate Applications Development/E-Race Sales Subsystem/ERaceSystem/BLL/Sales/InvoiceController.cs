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
using DMIT2018Common.UserControls;

#endregion
namespace ERaceSystem.BLL
{
    [DataObject]
    public class InvoiceController
    {
        public int Add_DetailsToInvoice(List<InvoiceItem> invoice, string subtotal, string tax, string total)
        {
            using (var context = new ERaceSystemContext())
            {
                int newInvoiceID = 0;
                List<string> errors = new List<string>();
                Invoice newinvoice = null;
                List<InvoiceDetail> newdetails = new List<InvoiceDetail>();
                InvoiceDetail itemdetail = null;

                //check invoice items were passed from web page
                if (invoice == null)
                {
                    errors.Add("There are no items selected for sale");
                }
                else
                {
                    //check calculated monetary values aclulated on web page are passed
                    if (subtotal.Length + tax.Length + total.Length== 0)
                    {
                        errors.Add("Items selected have can not be calculated. Please speak to a manager");
                    }
                    else
                    {
                        //TRX
                        //create instance of invoice
                        newinvoice = new Invoice();
                        newinvoice.InvoiceDate = DateTime.Now;
                        newinvoice.SubTotal = decimal.Parse(subtotal);
                        newinvoice.Total = decimal.Parse(total);
                        newinvoice.GST = decimal.Parse(tax);
                        newinvoice.EmployeeID = 20;     //James Calder

                        //create Invoice
                        context.Invoices.Add(newinvoice);
                        //load invoice details into list
                        foreach (InvoiceItem item in invoice)
                        {
                            itemdetail = new InvoiceDetail();
                            itemdetail.Price = item.Price;
                            itemdetail.ProductID = item.ProductID;
                            itemdetail.Quantity = item.Quantity;
                          
                            //Adding item to details list
                            newinvoice.InvoiceDetails.Add(itemdetail);
                        }                      

                        if (newinvoice == null)
                        {
                            errors.Add("Unable to create new invoice.");
                        }
                        else
                        {
                            //Grab invoice ID
                            newInvoiceID = newinvoice.InvoiceID;
                            //Adjust product inventory
                            foreach (InvoiceDetail item in newinvoice.InvoiceDetails)
                            {
                              
                                var existingProduct = (from prod in context.Products
                                                      where prod.ProductID == item.ProductID
                                                      select prod).FirstOrDefault();
                                //does product exist?
                                if (existingProduct == null)
                                {
                                    errors.Add("Product does not exist in the database");
                                }
                                else
                                {
                                                                  
                                    existingProduct.QuantityOnHand = existingProduct.QuantityOnHand - item.Quantity;
                                    //Update product record
                                    context.Entry(existingProduct).Property(nameof(item)).IsModified = true;

                                }                              
                            }
                        }

                        if (errors.Count > 0)
                        {
                            throw new BusinessRuleException("Create invoice error", errors);
                        }
                        else
                        {
                            context.SaveChanges();                           
                        }
                    }
                }               
                return newInvoiceID;
            }

        }
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


    }
}