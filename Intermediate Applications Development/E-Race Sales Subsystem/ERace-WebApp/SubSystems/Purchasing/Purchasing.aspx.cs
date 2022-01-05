using System;
using ERace_WebApp.Security;
using ERaceSystem.BLL;
using ERaceSystem.BLL.Purchasing;
using ERaceSystem.ViewModels;
using ERaceSystem.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERaceSystem.ViewModels.Purchasing;

namespace ERace_WebApp.SubSystems.Purchasing
{
    public partial class Purchasing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("Director") || User.IsInRole("Office Manager"))
                {
                    SecurityController ssysmgr = new SecurityController();
                    int? employeeid = ssysmgr.GetCurrentUserEmployeeId(User.Identity.Name);
                    int id = employeeid ?? default(int);
                    EmployeeItem info = new EmployeeItem();
                    MessageUserControl.TryRun(() =>
                    {
                        EmployeeController esysmgr = new EmployeeController();
                        info = esysmgr.Employee_FindByID(id);                   
                        if (info == null)
                        {
                            LoggedUser.Text = "Visitor/Unauthorized user";
                            throw new Exception("Logged employee cannot be found on file ");
                        }
                        else
                        {
                            LoggedUser.Text = info.FirstName + " " + info.LastName;
                        }
                    });
                }            
                else
                    {
                        Response.Redirect("~/SubSystems/Purchasing/AccessDenied.aspx");
                    }
                }
            else
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }
        protected void Select_Click(object sender, EventArgs e)
        {
            SecurityController ssysmgr = new SecurityController();
            int? employeeid = ssysmgr.GetCurrentUserEmployeeId(User.Identity.Name);
            int id = employeeid ?? default(int);
            EmployeeItem info = new EmployeeItem();
            MessageUserControl.TryRun(() =>
            {
                EmployeeController esysmgr = new EmployeeController();
                 info = esysmgr.Employee_FindByID(id);
            });       
            VendorController vendorController = new VendorController();
            VendorInfo vendorinfo = vendorController.List_VendorInfo(int.Parse(VendorNameDDL.SelectedValue));
            //Vendor vendorinfo = VendorController.List_VendorInfo(int.Parse(VendorNameDDL.SelectedValue));
            //VendorInfo vendorinfo = null;    
            VendorInformation.Text = vendorinfo.Name + " - " + vendorinfo.Contact + " - " + vendorinfo.Phone;
            InventoryController sysmgr = new InventoryController();
            List<InventoryList> datainfo = new List<InventoryList>();
            List<InventoryList> filteredDatainfo = new List<InventoryList>();
            MessageUserControl.TryRun(() =>
            {
                datainfo = sysmgr.GetVendorInventory(int.Parse(VendorNameDDL.SelectedValue));
                List<int> empty = new List<int>();
                for (int index = 0; index < 4; index++)
                {
                    if (datainfo[index].ProductList.Count() > 0)
                    {
                        filteredDatainfo.Add(datainfo[index]);
                    }
                }
                if (filteredDatainfo.Count() == 0)
                {
                    RepeaterInventory.DataSource = "";
                    RepeaterInventory.DataBind();
                    throw new Exception("The vendor you selected has no vendorcatalog");
                }
                RepeaterInventory.DataSource = filteredDatainfo;
                RepeaterInventory.DataBind();
                OrderController OrderContoller = new OrderController();
                OrderList order = new OrderList();
                order = OrderContoller.GetVendorOrder(int.Parse(VendorNameDDL.SelectedValue), info.EmployeeID);
                ListView_order.DataSource = order.ItemList;
                ListView_order.DataBind();
                foreach (var item in ListView_order.Items)
                {
                    double UnitSize = double.Parse((item.FindControl("UnitSizeLabel") as Label).Text);
                    double UnitCost = double.Parse((item.FindControl("UnitCostLabel") as TextBox).Text);
                    double Qty = double.Parse((item.FindControl("OrderQtyLabel") as TextBox).Text);

                    (item.FindControl("PerItemCostLabel") as Label).Text = (UnitCost / UnitSize).ToString("0.00");
                    (item.FindControl("ExtendedCostLabel") as Label).Text = (Qty * UnitCost).ToString("0.00");
                }
                Subtotal.Text = order.SubTotal.ToString("0.00");
                Tax.Text = order.TaxGST.ToString("0.00");
                Total.Text = (order.SubTotal + order.TaxGST).ToString("0.00");
                Comments.Text = order.Comment;
                vendorValidator.Text = VendorNameDDL.SelectedValue;
                VendorNameDDL.Enabled = false;
                Select.Enabled = false;
            }, "Success", "Display vendor and inventory Details");
        }
        protected void TracksSelectionList_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            SecurityController ssysmgr = new SecurityController();
            int? employeeid = ssysmgr.GetCurrentUserEmployeeId(User.Identity.Name);
            int id = employeeid ?? default(int);
            EmployeeItem info = new EmployeeItem();
            MessageUserControl.TryRun(() =>
            {
                EmployeeController esysmgr = new EmployeeController();
                info = esysmgr.Employee_FindByID(id);
            });
            MessageUserControl.TryRun(() =>
            {
                List<OrderItemList> SelectionList = new List<OrderItemList>();
                foreach (ListViewItem item in ListView_order.Items)
                {
                    OrderItemList oldItems = new OrderItemList();
                    oldItems.OrderDetailID = item.DataItemIndex;
                    // try it out
                    oldItems.Product = (item.FindControl("ProductLabel") as Label).Text;
                    if (!int.TryParse((item.FindControl("OrderQtyLabel") as TextBox).Text, out int test))
                    {
                        throw new Exception(oldItems.Product + ": Quantity can only be greater than 0");
                    }
                    else if (int.Parse((item.FindControl("OrderQtyLabel") as TextBox).Text) <= 0)
                    {
                        throw new Exception(oldItems.Product + ": Quantity can only be greater than 0");
                    }
                    else
                    {
                        oldItems.OrderQty = int.Parse((item.FindControl("OrderQtyLabel") as TextBox).Text);
                    }
                    oldItems.UnitSize = int.Parse((item.FindControl("UnitSizeLabel") as Label).Text);
                    if (!decimal.TryParse((item.FindControl("UnitCostLabel") as TextBox).Text, out decimal tesst))
                    {
                        throw new Exception(oldItems.Product + ": Unit Cost can only be greater than 0");
                    }
                    else if (decimal.Parse((item.FindControl("UnitCostLabel") as TextBox).Text) <= 0)
                    {
                        throw new Exception(oldItems.Product + ": Unit Cost can only be greater than 0");
                    }
                    else
                    {
                        oldItems.UnitCost = decimal.Parse((item.FindControl("UnitCostLabel") as TextBox).Text);
                    }
                    SelectionList.Add(oldItems);
                }

                OrderController orderController = new OrderController();
                int productid = int.Parse(e.CommandArgument.ToString());
                OrderItemList newItem = new OrderItemList();
                newItem.OrderDetailID = ListView_order.Items.Count();
                int errorcount = 0;
                newItem.Product = orderController.GetProductList(productid, int.Parse(vendorValidator.Text)).ItemName;
                newItem.OrderQty = 1;
                foreach (var item in ListView_order.Items)
                {
                    if (newItem.Product == (item.FindControl("ProductLabel") as Label).Text)
                    {
                        int quantity = int.Parse((item.FindControl("OrderQtyLabel") as TextBox).Text) + 1;
                        (item.FindControl("OrderQtyLabel") as TextBox).Text = quantity.ToString();
                        errorcount += 1;
                    }
                }
                newItem.UnitSize = orderController.GetProductList(productid, int.Parse(vendorValidator.Text)).UnitSize;
                newItem.UnitCost = orderController.GetProductList(productid, int.Parse(vendorValidator.Text)).UnitCost;
                if (errorcount == 0)
                {
                    SelectionList.Add(newItem);
                    ListView_order.DataSource = SelectionList;
                    ListView_order.DataBind();
                }

            });
        }
        protected void PlaceOrder_Click(object sender, EventArgs e) 
        { 
        
        }
        protected void Save_Click(object sender, EventArgs e)
        {
            decimal subtotal = 0;
            SecurityController ssysmgr = new SecurityController();
            int? employeeid = ssysmgr.GetCurrentUserEmployeeId(User.Identity.Name);
            int id = employeeid ?? default(int);
            EmployeeItem info = new EmployeeItem();
            MessageUserControl.TryRun(() =>
            {
                EmployeeController esysmgr = new EmployeeController();
                info = esysmgr.Employee_FindByID(id);
            });
            MessageUserControl.TryRun(() => {
                List<OrderItemList> SelectionList = new List<OrderItemList>();
                OrderController orderController = new OrderController();
                foreach (ListViewItem item in ListView_order.Items)
                {
                    OrderItemList oldItems = new OrderItemList();
                    oldItems.OrderDetailID = null;
                    oldItems.Product = (item.FindControl("ProductLabel") as Label).Text;

                    if (!int.TryParse((item.FindControl("OrderQtyLabel") as TextBox).Text, out int test))
                    {
                        throw new Exception(oldItems.Product + ": Quantity can only be  greater than 0");
                    }
                    else if (int.Parse((item.FindControl("OrderQtyLabel") as TextBox).Text) <= 0)
                    {
                        throw new Exception(oldItems.Product + ": Quantity can only be  greater than 0");
                    }
                    else
                    {
                        oldItems.OrderQty = int.Parse((item.FindControl("OrderQtyLabel") as TextBox).Text);
                    }
                    oldItems.UnitSize = int.Parse((item.FindControl("UnitSizeLabel") as Label).Text);

                    if (!decimal.TryParse((item.FindControl("UnitCostLabel") as TextBox).Text, out decimal tesst))
                    {
                        throw new Exception(oldItems.Product + ": Unit Cost can only be  greater than 0");
                    }
                    else if (decimal.Parse((item.FindControl("UnitCostLabel") as TextBox).Text) <= 0)
                    {
                        throw new Exception(oldItems.Product + ": Unit Cost can only be  greater than 0");
                    }
                    else
                    {
                        oldItems.UnitCost = decimal.Parse((item.FindControl("UnitCostLabel") as TextBox).Text);
                    }

                    SelectionList.Add(oldItems);
                    subtotal += (oldItems.UnitCost * oldItems.OrderQty);
                }
                OrderList AnewOrder = new OrderList();
                AnewOrder.ItemList = SelectionList;
                AnewOrder.Comment = Comments.Text;
                AnewOrder.SubTotal = subtotal;

                orderController.UpdateOrder(int.Parse(vendorValidator.Text), AnewOrder);
                VendorNameDDL.Enabled = true;
                Select.Enabled = true;
            }, "Success", "Order saved.");
        }
        protected void Delete_Click(object sender, EventArgs e)
        {
            MessageUserControl.TryRun(() => {
                OrderController sysmgrUser = new OrderController();
                sysmgrUser.RemoveOrder(int.Parse(vendorValidator.Text));
                List<OrderItemList> refreshitem = new List<OrderItemList>();
                List<ProductList> refreshList = new List<ProductList>();
                ListView_order.DataSource = refreshitem;
                ListView_order.DataBind();
                RepeaterInventory.DataSource = refreshList;
                RepeaterInventory.DataBind();
                vendorValidator.Text = "";
                Subtotal.Text = 0.ToString();
                Tax.Text = 0.ToString();
                Total.Text = 0.ToString();
                VendorNameDDL.Enabled = true;
                Select.Enabled = true;
            }, "Success", "Order deleted");
        }        
        protected void Cancel_Click(object sender, EventArgs e)
        {
            SecurityController ssysmgr = new SecurityController();
            int? employeeid = ssysmgr.GetCurrentUserEmployeeId(User.Identity.Name);
            int id = employeeid ?? default(int);
            EmployeeItem info = new EmployeeItem();
            MessageUserControl.TryRun(() =>
            {
                EmployeeController esysmgr = new EmployeeController();
                info = esysmgr.Employee_FindByID(id);
            });
            VendorController vendorController = new VendorController();
            VendorInfo vendorinfo = vendorController.List_VendorInfo(int.Parse(VendorNameDDL.SelectedValue));
            //Vendor vendorinfo = VendorController.List_VendorInfo(int.Parse(VendorNameDDL.SelectedValue));
            //VendorInfo vendorinfo = null;    
            VendorInformation.Text = vendorinfo.Name + " - " + vendorinfo.Contact + " - " + vendorinfo.Phone;
            InventoryController sysmgr = new InventoryController();
            List<InventoryList> datainfo = new List<InventoryList>();
            List<InventoryList> filteredDatainfo = new List<InventoryList>();
            MessageUserControl.TryRun(() =>
            {
                datainfo = sysmgr.GetVendorInventory(int.Parse(vendorValidator.Text));
                List<int> empty = new List<int>();
                for (int index = 0; index < 4; index++)
                {
                    if (datainfo[index].ProductList.Count() > 0)
                    {
                        filteredDatainfo.Add(datainfo[index]);
                    }
                }
                if (filteredDatainfo.Count() == 0)
                {
                    RepeaterInventory.DataSource = "";
                    RepeaterInventory.DataBind();
                    ListView_order.DataSource = null;
                    ListView_order.DataBind();
                    throw new Exception("The vendor you selected has no vendorcatalog");
                }
                RepeaterInventory.DataSource = filteredDatainfo;
                RepeaterInventory.DataBind();
                OrderController OrderContoller = new OrderController();
                OrderList OrderList = new OrderList();
                OrderList = OrderContoller.GetVendorOrder(int.Parse(vendorValidator.Text), info.EmployeeID);
                ListView_order.DataSource = OrderList.ItemList;
                ListView_order.DataBind();
                Subtotal.Text = OrderList.SubTotal.ToString("0.00");
                Tax.Text = OrderList.TaxGST.ToString("0.00");
                Total.Text = (OrderList.SubTotal + OrderList.TaxGST).ToString("0.00");
                Comments.Text = OrderList.Comment;
                VendorNameDDL.Enabled = true;
                Select.Enabled = true;
            }, "Cancel", "All changes are gone.");
            }
        protected void Refresh_Click(object sender, EventArgs e) 
        { 
        
        }
    }
}