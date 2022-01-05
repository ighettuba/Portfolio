using ERace_WebApp.Security;
using ERaceSystem.BLL;
using ERaceSystem.ViewModels;
using ERaceSystem.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERace_WebApp.SubSystems.Sales
{
    public partial class ReturnRefunds : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //test our security
            //are you logged in?
            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("Clerk"))
                {

                    //obtain the CustomerId on the security User record
                    SecurityController ssysmgr = new SecurityController();
                    int? employeeid = ssysmgr.GetCurrentUserEmployeeId(User.Identity.Name);

                    //need to convert the int? to an int for the call to the CustomerController method
                    //int custid = customerid == null ? default(int) : int.Parse(customerid.ToString());
                    int emplid = employeeid ?? default(int);

                    MessageUserControl.TryRun(() =>
                    {
                        EmployeeController csysmgr = new EmployeeController();
                        EmployeeItem item = csysmgr.Employee_FindByID(emplid);
                        if (item == null)
                        {
                            LoggedUser.Text = "Unknown";
                            throw new Exception("Logged employee cannot be found on file ");
                        }
                        else
                        {
                            LoggedUser.Text = item.LastName + ", " + item.FirstName;
                        }
                    });
                }
                else
                {
                    Response.Redirect("~/SubSystems/Sales/AccessDenied.aspx");
                }
            }
            else
            {
                Response.Redirect("~/Account/Login.aspx");
            }

        }

        protected void Clear_Click(object sender, EventArgs e)
        {
            RefundDetailsGV.DataSource = null;
            RefundDetailsGV.DataBind();
            InvoiceArg.Text = null;

            Subtotal.Text = "0.00";
            Tax.Text = null;
            RefundTotal.Text = null;
        }

        protected void LookupInvoice_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(OriginalInvoice.Text))
            {
                MessageUserControl.ShowInfo("Invoice Search",
                   "You must enter an invoice number to begin refund process.");
            }
            else
            {
                InvoiceArg.Text = OriginalInvoice.Text;
                MessageUserControl.TryRun(() =>
                {
                    StoreRefundController controller = new StoreRefundController();
                    RefundInvoice invoice = new RefundInvoice();
                    List<RefundItem> detaillist = null;

                    invoice = controller.Invoice_FindById(int.Parse(InvoiceArg.Text));

                    if (invoice.InvoiceID==null)
                    {
                        MessageUserControl.ShowInfo("Invoice Search",
                   "Must enter an invoice number.");
                    }
                    if (invoice.InvoiceID <=0)
                    {
                        MessageUserControl.ShowInfo("Invoice Search",
                   "Must enter valid invoice number.");
                    }
                    else
                    {
                        foreach(RefundItem item in invoice.Items)
                        {
                            RefundItem refundItem = new RefundItem();
                            refundItem.RefundID = 0;
                            refundItem.InvoiceID = item.InvoiceID;
                            refundItem.ProductID = item.ProductID;
                            refundItem.ItemName = item.ItemName;
                            refundItem.Quantity = item.Quantity;
                            refundItem.Price = item.Price;
                            refundItem.Amount = item.Amount;
                            refundItem.RestockCharge = item.RestockCharge;
                            refundItem.Reason = "";
                            detaillist.Add(refundItem);
                        }
                        RefundDetailsGV.DataSource = detaillist;
                        RefundDetailsGV.DataBind();
                    }
                },"Invoice Found","Details of selected invoice are attatched");
            }
        }

        protected void CalculateTotals()
        {
            Subtotal.Text = "0.00";
            Tax.Text = null;
            RefundTotal.Text = null;

            CheckBox refundSelected = null;
            for (int index = 0; index < RefundDetailsGV.Rows.Count; index++)
            {
                refundSelected = RefundDetailsGV.Rows[index].FindControl("RestockCheckBox") as CheckBox;
                if (refundSelected.Checked)
                {
                    Subtotal.Text = (double.Parse(Subtotal.Text) + double.Parse((RefundDetailsGV.Rows[index].FindControl("Amount") as Label).Text)).ToString();
                }             
            }
            Subtotal.Text = string.Format("{0:0.00}", decimal.Parse(Subtotal.Text));
            Tax.Text = string.Format("{0:0.00}", ((decimal.Parse(Subtotal.Text) * (decimal)0.05)));
            RefundTotal.Text = string.Format("{0:0.00}", (decimal.Parse(Subtotal.Text) + decimal.Parse(Tax.Text)));
        }

        protected void RefundButton_Click(object sender, EventArgs e)
        {
            CalculateTotals();
            //Validate Items in the GV

            //process Refund transaction

        }
    }
    
}