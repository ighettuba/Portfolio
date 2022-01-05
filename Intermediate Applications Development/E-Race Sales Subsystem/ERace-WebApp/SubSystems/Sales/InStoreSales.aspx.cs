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
    public partial class InStoreSales : System.Web.UI.Page
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

        protected void AddButton_Click(object sender, EventArgs e)
        {
            List<InvoiceItem> invoicelist = new List<InvoiceItem>();
            InvoiceItem item =null;
            //Check if category selection was made
            if (int.Parse(CategoryDDL.SelectedValue) == 0)
            {
                MessageUserControl.ShowInfo("Category ", "Category not selected for associated products to list");
            }
            else
            {
                //Check if category selection was made
                if (ProductDDL.Items.Count == 0)
                {
                    MessageUserControl.ShowInfo("Product ", "Category selected has no products");
                }
                else
                {
                            //collect ProductID
                            ProductArg.Text = ProductDDL.SelectedValue;
                    //Check if quantity was entered
                    //this will blow up if no qty is entered. test for null qty
                    if(int.Parse(Quantity.Text) <= 0)
                    {
                        MessageUserControl.ShowInfo("Product Quantity ", "Please enter quantity amount greater than zero");
                    }
                    else if (Quantity.Text == null)
                    {
                        MessageUserControl.ShowInfo("Product Quantity ", "Please enter quantity in the available field");
                    }
                    else
                    {     
                        //Collect Quantity
                        QtyArg.Text = Quantity.Text;   
                        
                        MessageUserControl.TryRun(() => 
                        {

                            ProductController sysmgr = new ProductController();
                            //locate product to add
                            ProductItem info = sysmgr.Product_FindById(int.Parse(ProductDDL.SelectedValue));
                            //check if exists
                            if (info == null)                            {
                                MessageUserControl.ShowInfo("Product Find Error.", "No information on this Product");
                            }
                            else
                            {
                                 item = new InvoiceItem()
                                {
                                    ProductID = info.ProductID,
                                    ItemName = info.ItemName,
                                    Price = info.ItemPrice,
                                    Quantity = int.Parse(QtyArg.Text),
                                    Amount = info.ItemPrice * int.Parse(QtyArg.Text)
                                };

                                //is GV populated?
                                if (InvoiceDetailGV.Rows.Count == 0)
                                {               
                                    invoicelist.Add(item);
                                }
                                if(InvoiceDetailGV.Rows.Count>0)
                                {
                                    foreach (GridViewRow row in InvoiceDetailGV.Rows)
                                    {
                                        InvoiceItem existing = new InvoiceItem();
                                    
                                        //check if item is already on the GV
                                        if (ProductArg.Text.Equals((row.FindControl("ProductId") as Label).Text))
                                        {
                                            //adjust item on GV with new input
                                            existing.ProductID = int.Parse((row.FindControl("ProductId") as Label).Text);
                                            existing.ItemName = (row.FindControl("Product") as Label).Text;
                                            existing.Price = decimal.Parse((row.FindControl("Price") as Label).Text);
                                            existing.Quantity = (int.Parse((row.FindControl("QuantityBought") as TextBox).Text) + int.Parse(QtyArg.Text));
                                            existing.Amount = existing.Quantity * existing.Price;

                                            //nullify item to add to gv
                                            item = null;
                                        }
                                        else
                                        {
                                            //if item is not on the GV
                                            existing.ProductID = int.Parse((row.FindControl("ProductId") as Label).Text);
                                            existing.ItemName = (row.FindControl("Product") as Label).Text;
                                            existing.Price = decimal.Parse((row.FindControl("Price") as Label).Text);
                                            existing.Quantity = int.Parse((row.FindControl("QuantityBought") as TextBox).Text);
                                            existing.Amount = existing.Quantity * existing.Price;
                                        }
                                        //add  existing invoice items from GV to list
                                        invoicelist.Add(existing);                    
                                    }   
                                    //add new item to list
                                    if (item != null)
                                    {
                                        invoicelist.Add(item);               
                                    }
                                }
                            }
                        },"Success","Product has been added to the list");
                        //refresh GV
                        InvoiceDetailGV.DataSource = invoicelist;
                        InvoiceDetailGV.DataBind();

                        //Tabulate Subtotal, Tax and Total
                        CalculateTotals(InvoiceDetailGV);

                        ProductArg.Text = null;
                        QtyArg.Text = null;
                    }
                }
            }
        }

        protected void CategoryDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProductDDL.DataBind();
        }

        protected void RemoveProduct_Command(object sender, CommandEventArgs e)
        {
            if (InvoiceDetailGV.Rows.Count == 0)
            {
                MessageUserControl.ShowInfo("Item Update", "You must have a invoice items desplayed to select one for deletion.");
            }
            else
            {
                int deleterow = int.Parse(e.CommandArgument.ToString());
                InvoiceItem deleteitem = new InvoiceItem();
                MessageUserControl.TryRun(() =>
                {
                    List<InvoiceItem> invoicelist = new List<InvoiceItem>();
                    for (int index = 0; index < InvoiceDetailGV.Rows.Count; index++)
                    {
                        InvoiceItem updateitem = new InvoiceItem();
                        //load GV to list
                        updateitem.ProductID = int.Parse((InvoiceDetailGV.Rows[index].FindControl("ProductId") as Label).Text);
                        updateitem.ItemName = (InvoiceDetailGV.Rows[index].FindControl("Product") as Label).Text;
                        updateitem.Price = decimal.Parse((InvoiceDetailGV.Rows[index].FindControl("Price") as Label).Text);
                        updateitem.Quantity = int.Parse((InvoiceDetailGV.Rows[index].FindControl("QuantityBought") as TextBox).Text);
                        updateitem.Amount = updateitem.Quantity * updateitem.Price;
                        //check if item is to be deleted
                        if (updateitem.ProductID == deleterow)
                        {
                            //colect item to remove
                            deleteitem = updateitem;
                        }
                        //add all existing invoice items from GV to list
                        invoicelist.Add(updateitem);
                    }
                    //remove item from list
                    invoicelist.Remove(deleteitem);
                    //refresh GV
                    InvoiceDetailGV.DataSource = invoicelist;
                    InvoiceDetailGV.DataBind();


                    //Tabulate Subtotal, Tax and Total
                    CalculateTotals(InvoiceDetailGV);


                }, "Success", "Invoice item have been removed");
            }
        }

        protected void Refresh_Command(object sender, CommandEventArgs e)
        {
            if (InvoiceDetailGV.Rows.Count == 0)
            {
                MessageUserControl.ShowInfo("Item Update","You must have invoice items displayed to refresh sale quantities.");
            }
            else
            {
                MessageUserControl.TryRun(() =>
                {
                    List<InvoiceItem> invoicelist = new List<InvoiceItem>();
                    //walk throug GV
                    for (int index = 0; index < InvoiceDetailGV.Rows.Count; index++)
                    {
                        InvoiceItem updateitem = new InvoiceItem();
                        //adjust item on GV with new Quantity
                        updateitem.ProductID = int.Parse((InvoiceDetailGV.Rows[index].FindControl("ProductId") as Label).Text);
                        updateitem.ItemName = (InvoiceDetailGV.Rows[index].FindControl("Product") as Label).Text;
                        updateitem.Price = decimal.Parse((InvoiceDetailGV.Rows[index].FindControl("Price") as Label).Text);
                        updateitem.Quantity = int.Parse((InvoiceDetailGV.Rows[index].FindControl("QuantityBought") as TextBox).Text);
                        updateitem.Amount = updateitem.Quantity * updateitem.Price;

                        //add  existing invoice items  to list
                        invoicelist.Add(updateitem);
                    }
                    //refresh GV
                    InvoiceDetailGV.DataSource = invoicelist;
                    InvoiceDetailGV.DataBind();

                    //Tabulate New Subtotal, Tax and Total
                    CalculateTotals(InvoiceDetailGV);
                }, "Success", "Invoice items have been refreshed");
            }
        }

        protected void CalculateTotals(GridView invoiceitems)
        {
            Subtotal.Text = "0.00";
            Tax.Text = null;
            Total.Text = null;
            for (int index = 0; index < invoiceitems.Rows.Count; index++)
            {
                Subtotal.Text = (double.Parse(Subtotal.Text) + double.Parse((InvoiceDetailGV.Rows[index].FindControl("Amount") as Label).Text)).ToString();
            }
            Subtotal.Text = string.Format("{0:0.00}", decimal.Parse(Subtotal.Text));
            Tax.Text = string.Format("{0:0.00}", ((decimal.Parse(Subtotal.Text) *(decimal)0.05)));
            Total.Text = string.Format("{0:0.00}", (decimal.Parse(Subtotal.Text) + decimal.Parse(Tax.Text)));
        }

        protected void Clear_Click(object sender, EventArgs e)
        {
                //delete invoice items
                InvoiceDetailGV.DataSource = null;
                InvoiceDetailGV.DataBind();

                //delete invoice totals
                Subtotal.Text = "0.00";
                Tax.Text = "0.00";
                Total.Text = "0.00";

         
        }

        protected void PayButton_Click(object sender, EventArgs e)
        {
            //check if there are any items in the GV
            if (InvoiceDetailGV.Rows.Count == 0)
            {
                MessageUserControl.ShowInfo("Save Invoice Error", "Invoice must have items in list to save");
            }
            else
            {
                List<InvoiceItem> invoice = new List<InvoiceItem>();
                foreach(GridViewRow item in InvoiceDetailGV.Rows)
                {
                    InvoiceItem updateitem = new InvoiceItem();
                    //adjust item on GV with new Quantity
                    updateitem.ProductID = int.Parse((item.FindControl("ProductId") as Label).Text);
                    updateitem.ItemName = (item.FindControl("Product") as Label).Text;
                    updateitem.Price = decimal.Parse((item.FindControl("Price") as Label).Text);
                    updateitem.Quantity = int.Parse((item.FindControl("QuantityBought") as TextBox).Text);
                    updateitem.Amount = updateitem.Quantity * updateitem.Price;

                    //add  existing invoice items  to list
                    invoice.Add(updateitem);
                }

                //Send Info to BLL for deletion
                MessageUserControl.TryRun(() =>
                {
                    int newInvoiceID;
                    InvoiceController sysmgr = new InvoiceController();
                    newInvoiceID= sysmgr.Add_DetailsToInvoice(invoice, Subtotal.Text, Tax.Text, Total.Text);

                    InvoiceID.Text = newInvoiceID.ToString();
                },"Success","New invoice has been created in the system");
            }
        }
        protected void ProductDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}