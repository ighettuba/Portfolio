using System;
using ERace_WebApp.Security;
using ERaceSystem.BLL;
using ERaceSystem.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERaceSystem.ViewModels.Receiving;


namespace ERace_WebApp.SubSystems.Receiving
{
    public partial class DefaultReceiving : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //is the user logged in
            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("Clerk") || User.IsInRole("Food Service"))
                {
                    SecurityController ssysmgr = new SecurityController();
                    int? employeeid = ssysmgr.GetCurrentUserEmployeeId(User.Identity.Name);
                    int id = employeeid ?? default(int);
                    MessageUserControl.TryRun(() =>
                    {
                        EmployeeController csysmgr = new EmployeeController();
                        EmployeeItem item = csysmgr.Employee_FindByID(id);
                        if (item == null)
                        {
                            LoggedUser.Text = "Visitor/Unauthorized user";
                            throw new Exception("Logged employee cannot be found on file ");
                        }
                        else
                        {
                            LoggedUser.Text = item.LastName + ", " + item.FirstName;
                            ReceiveShipment.Enabled = false;
                            ReceiveShipment.Visible = false;
                            ForceClose.Visible = false;
                            ForceCloseReason.Visible = false;
                            UnorderedTable.Visible = false;    
                            if (!Page.IsPostBack)
                            {
                                PurchaseOrderDropDownList.DataBind();
                                PurchaseOrderDropDownList.Items.Insert(0, new ListItem("Select a PO", "-1"));
                                PurchaseOrderDropDownList.SelectedIndex = -1;
                            }
                            else
                            {
                                int index = PurchaseOrderDropDownList.SelectedIndex;
                                PurchaseOrderDropDownList.DataBind();
                                PurchaseOrderDropDownList.Items.Insert(0, new ListItem("Select a PO", "-1"));
                                PurchaseOrderDropDownList.SelectedIndex = index;
                            }
                        }
                    });
                }
                else
                {
                    Response.Redirect("~/SubSystems/Receiving/AccessDenied.aspx");
                    ReceiveShipment.Enabled = false;
                    ForceClose.Visible = false;
                    ForceCloseReason.Visible = false;
                    UnorderedTable.Visible = false;
                }
            }
            else
            {
                Response.Redirect("~/Account/Login.aspx");
            }

        }

        protected void Open_Click(object sender, EventArgs e)
        {
            var controller = new PurchaseOrderController();
            if (int.Parse(PurchaseOrderDropDownList.SelectedValue) == -1)
            {
                VendorName.Text = "";
                VendorAddress.Text = "";
                VendorContact.Text ="";
                PhoneNumber.Text = "";
                PurchaseOrderDisplay.DataSource = null;
                PurchaseOrderDisplay.DataBind();
                PurchaseOrderDisplay.Visible = false; 
                ReceiveShipment.Visible = false;
                ReceiveShipment.Enabled = false;
                ForceClose.Visible = false;
                ForceCloseReason.Visible = false;
                UnorderedTable.Visible = false;
            }
            else
            {
                MessageUserControl.TryRun(() =>
                {
                    ReceiveShipment.Enabled = true;
                    VendorDetails vendorDetails = controller.GetVendorDetails(int.Parse(PurchaseOrderDropDownList.SelectedValue));
                    VendorName.Text = vendorDetails.Name;
                    VendorAddress.Text = vendorDetails.Address;
                    VendorContact.Text = vendorDetails.Contact;
                    string area = vendorDetails.Phone.Substring(0, 3);
                    string major = vendorDetails.Phone.Substring(3, 3);
                    string minor = vendorDetails.Phone.Substring(6);
                    PhoneNumber.Text = string.Format("{0}-{1}-{2}",area,major,minor);
                    List<PurchaseOrderDetail> info = controller.GetPurchaseOrderDetails(int.Parse(PurchaseOrderDropDownList.SelectedValue));
                    PurchaseOrderDisplay.DataSource = info;
                    PurchaseOrderDisplay.DataBind();
                    PurchaseOrderDisplay.Visible = true;
                    ForceClose.Visible = true;
                    ForceCloseReason.Visible = true;
                    ReceiveShipment.Visible = true;
                    foreach (GridViewRow row in PurchaseOrderDisplay.Rows)
                    {
                        int QtyOutstanding = int.Parse((row.FindControl("QtyOutstanding") as Label).Text);
                        if (QtyOutstanding <= 0)
                        {
                            (row.FindControl("UnitReceived") as TextBox).Visible = false;
                            (row.FindControl("Unit") as Label).Visible = false;
                        }
                    }
                   //controller.DeleteUnorderedItem(int.Parse(PurchaseOrderDropDownList.SelectedValue));
                    UnorderedTable.Visible = true;
                    controller.DeleteAllUnorderedItem(int.Parse(PurchaseOrderDropDownList.SelectedValue));
                    List<UnorderedItem> items= controller.GetUnorderedItem(int.Parse(PurchaseOrderDropDownList.SelectedValue));
                    if (items.Count==0)
                    {
                        List<UnorderedItem> dummyrow = new List<UnorderedItem>();
                        UnorderedItem dummy = new UnorderedItem();
                        dummyrow.Add(dummy);
                        UnorderedTable.DataSource = dummyrow;
                        UnorderedTable.DataBind();
                        UnorderedTable.Rows[0].Visible = false;
                        PurchaseOrderDisplay.Visible = true;
                    }
                    else
                    {
                        UnorderedTable.DataSource = items;
                        UnorderedTable.DataBind();
                        PurchaseOrderDisplay.Visible = true;
                    }                
                }, "Open the Purchase Order", "Display Purchase Order Details");
            }
         

        }

        protected void UnorderedTable_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //add button
            if (e.CommandArgument.Equals("New"))
            {
                UnorderedItem item = new UnorderedItem();
                item.ItemName = (UnorderedTable.FooterRow.FindControl("ItemNameFooter") as TextBox).Text;
                item.VendorProductID = (UnorderedTable.FooterRow.FindControl("VendorProductIDFooter") as TextBox).Text;
                item.Quantity = (UnorderedTable.FooterRow.FindControl("QuantityFooter") as TextBox).Text==""?0:int.Parse((UnorderedTable.FooterRow.FindControl("QuantityFooter") as TextBox).Text);
                item.OrderID = (int.Parse(PurchaseOrderDropDownList.SelectedValue));
                if (item.ItemName == "")
                {
                    MessageUserControl.ShowInfo("add new item", "Item name must be provided");
                }
                if (item.VendorProductID == "")
                {
                    MessageUserControl.ShowInfo("add new item", "Vendor ID must be provided");
                }
                if (item.Quantity <= 0)
                {
                    MessageUserControl.ShowInfo("add new item", "Quantity must be provided");
                }
                MessageUserControl.TryRun(() =>
                {
                    var controll = new PurchaseOrderController();
                    controll.InsertUnorderedItem(item);
                    
                },"Add New Unordered Item","Add Successful");
                var controller = new PurchaseOrderController();
                List<UnorderedItem> items = controller.GetUnorderedItem(int.Parse(PurchaseOrderDropDownList.SelectedValue));
                if (items.Count == 0)
                {
                    List<UnorderedItem> dummyrow = new List<UnorderedItem>();
                    UnorderedItem dummy = new UnorderedItem();
                    dummyrow.Add(dummy);
                    UnorderedTable.DataSource = dummyrow;
                    UnorderedTable.DataBind();
                    UnorderedTable.Rows[0].Visible = false;
                    UnorderedTable.Visible = true;
                    PurchaseOrderDisplay.Visible = true;
                }
                else
                {
                    UnorderedTable.DataSource = items;
                    UnorderedTable.DataBind();
                    UnorderedTable.Visible = true;
                    PurchaseOrderDisplay.Visible = true;
                }
                ForceClose.Visible = true;
                ForceCloseReason.Visible = true;
                PurchaseOrderDisplay.Visible = true;
                ReceiveShipment.Enabled = true;
                ReceiveShipment.Visible = true;
            }
        }

        protected void UnorderedTable_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
                 int ItemID = int.Parse((UnorderedTable.Rows[e.RowIndex].FindControl("ItemID") as Label).Text);
                if (ItemID==0)
                {
                    MessageUserControl.ShowInfo("Remove item", "Invalid item");
                }
                else
                {
                    MessageUserControl.TryRun(() =>
                    {
                        var controller = new PurchaseOrderController();
                        controller.DeleteUnorderedItem(ItemID);
                        UnorderedTable.Visible = true;
                        List<UnorderedItem>items= controller.GetUnorderedItem(int.Parse(PurchaseOrderDropDownList.SelectedValue));
                        ForceClose.Visible = true;
                        ForceCloseReason.Visible = true;
                        ReceiveShipment.Enabled = true;
                        if (items.Count == 0)
                        {
                            List<UnorderedItem> dummyrow = new List<UnorderedItem>();
                            UnorderedItem dummy = new UnorderedItem();
                            dummyrow.Add(dummy);
                            UnorderedTable.DataSource = dummyrow;
                            UnorderedTable.DataBind();
                            UnorderedTable.Rows[0].Visible = false;
                        }
                        else
                        {
                            UnorderedTable.DataSource = items;
                            UnorderedTable.DataBind();
                        }
                    }, "Remove Unordered Item", "Remove Successful");
                ReceiveShipment.Visible = true;
                }
            }

        protected void ForceClose_Click(object sender, EventArgs e)
        {
            VendorAddress.Text = "";
            VendorContact.Text = "";
            VendorName.Text = "";
            PhoneNumber.Text = "";
            UnorderedTable.Visible = false;
            PurchaseOrderDisplay.Visible = false;
            string reason = ForceCloseReason.Text;
            int OrderID = int.Parse(PurchaseOrderDropDownList.SelectedValue);
            List<ProductInventory> items = new List<ProductInventory>();
            foreach(GridViewRow row in PurchaseOrderDisplay.Rows)
            {
                ProductInventory item = new ProductInventory();
                item.OrderDetailID = int.Parse((row.FindControl("OrderDetailID") as Label).Text);
                item.QtyOutstanding = int.Parse((row.FindControl("QtyOutstanding") as Label).Text);
                items.Add(item);
            }
            MessageUserControl.TryRun(() =>
            {
                var controller = new PurchaseOrderController();
                controller.ForceCloseOrder(OrderID, reason, items);
            }, "Force Close", "Successful close the order");
            var tempcontroller = new PurchaseOrderController();
            PurchaseOrderDropDownList.SelectedIndex = -1;
            PurchaseOrderDropDownList.DataBind();
            PurchaseOrderDropDownList.Items.Insert(0, new ListItem("Select a PO", "-1"));
        }

        protected void ReceiveShipment_Click(object sender, EventArgs e)
        {
            int unclosedRow = 0;
            int orderId= int.Parse(PurchaseOrderDropDownList.SelectedValue);
            SecurityController ssysmgr = new SecurityController();
            int? employeeid = ssysmgr.GetCurrentUserEmployeeId(User.Identity.Name);
            int id = employeeid ?? default(int);
            List<ItemReceived> received = new List<ItemReceived>();
            foreach (GridViewRow row in PurchaseOrderDisplay.Rows)
            {
                if(int.Parse((row.FindControl("QtyOutstanding") as Label).Text) > 0)
                {
                    unclosedRow++;
                }
                if((row.FindControl("UnitReceived") as TextBox).Text != "" || (row.FindControl("QtySalvaged") as TextBox).Text != "")
                {
                    ItemReceived item = new ItemReceived();
                    item.OrderDetailID = int.Parse((row.FindControl("OrderDetailID") as Label).Text);
                    item.QtyOrdered = int.Parse((row.FindControl("QtyOrdered") as Label).Text);
                    item.QtyOutstanding = int.Parse((row.FindControl("QtyOutstanding") as Label).Text);
                    item.UnitReceived = (row.FindControl("UnitReceived") as TextBox).Text == "" ? 0 : int.Parse((row.FindControl("UnitReceived") as TextBox).Text);
                    item.QtySalvaged = (row.FindControl("QtySalvaged") as TextBox).Text == "" ? 0 : int.Parse((row.FindControl("QtySalvaged") as TextBox).Text);
                    item.UnitRejected= (row.FindControl("UnitRejected") as TextBox).Text == "" ? 0 : int.Parse((row.FindControl("UnitRejected") as TextBox).Text);
                    received.Add(item);
                }    
            }
            List<ItemReturned> returns = new List<ItemReturned>();
            foreach(GridViewRow row in UnorderedTable.Rows)
            {
                ItemReturned returnItem = new ItemReturned();
                returnItem.OrderDetailID = null;
                returnItem.UnOrderedItem = (row.FindControl("ItemName") as Label).Text;
                returnItem.VendorProductID = (row.FindControl("VendorProductID") as Label).Text;
                returnItem.ItemQuantity = int.Parse((row.FindControl("Quantity") as Label).Text);
                returnItem.Comment = "Not on original order";
                if(returnItem.ItemQuantity!=0 && returnItem.VendorProductID != null && returnItem.UnOrderedItem!="")
                {
                    returns.Add(returnItem);
                }
            }
            List<ItemReturned> rejects = new List<ItemReturned>();
            foreach(GridViewRow row in PurchaseOrderDisplay.Rows)
            {
                if((row.FindControl("UnitRejected") as TextBox).Text != "")
                {
                    ItemReturned reject = new ItemReturned();
                    reject.OrderDetailID = int.Parse((row.FindControl("OrderDetailID") as Label).Text);
                    reject.UnOrderedItem = null;
                    reject.ItemUnit = (row.FindControl("UnitRejected") as TextBox).Text == "" ? 0 : int.Parse((row.FindControl("UnitRejected") as TextBox).Text);
                    reject.QtySalvaged = (row.FindControl("QtySalvaged") as TextBox).Text == "" ? 0 : int.Parse((row.FindControl("QtySalvaged") as TextBox).Text);
                    reject.Comment = (row.FindControl("Reason") as TextBox).Text;
                    reject.VendorProductID = null;
                    rejects.Add(reject);
                }
            }
            MessageUserControl.TryRun(() =>
            {
                var controller = new PurchaseOrderController();
                controller.ReceiveOrder(orderId, id, received, returns, rejects,unclosedRow);
            }, "Receive Order", "Successful receive the order");
            var tempcontroller = new PurchaseOrderController();
            PurchaseOrderDropDownList.SelectedIndex = -1;
            PurchaseOrderDropDownList.DataBind();
            PurchaseOrderDropDownList.Items.Insert(0, new ListItem("Select a PO", "-1"));
            VendorAddress.Text = "";
            VendorContact.Text = "";
            VendorName.Text = "";
            PhoneNumber.Text = "";
            List<PurchaseOrderDetail> info = tempcontroller.GetPurchaseOrderDetails(int.Parse(PurchaseOrderDropDownList.SelectedValue));
            PurchaseOrderDisplay.DataSource = info;
            PurchaseOrderDisplay.DataBind();
            List<UnorderedItem> items = tempcontroller.GetUnorderedItem(int.Parse(PurchaseOrderDropDownList.SelectedValue));
            ForceClose.Visible = false;
            ForceCloseReason.Visible = false;
            ReceiveShipment.Visible = false;
            if (items.Count == 0)
            {
                List<UnorderedItem> dummyrow = new List<UnorderedItem>();
                UnorderedItem dummy = new UnorderedItem();
                dummyrow.Add(dummy);
                UnorderedTable.DataSource = dummyrow;
                UnorderedTable.DataBind();
                UnorderedTable.Rows[0].Visible = false;
            }
            else
            {
                UnorderedTable.DataSource = items;
                UnorderedTable.DataBind();
            }
            UnorderedTable.Visible = false;
            PurchaseOrderDisplay.Visible = false;
        }
    }
}