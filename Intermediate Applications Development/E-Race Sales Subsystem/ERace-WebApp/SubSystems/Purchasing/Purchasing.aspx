<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Purchasing.aspx.cs" Inherits="ERace_WebApp.SubSystems.Purchasing.Purchasing" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-10">
            <h2>Receiving</h2>
        </div>
        <div class="col-2">
            <asp:Label ID="Label2" runat="server" Text="User: "></asp:Label>&nbsp;
            <asp:Label ID="LoggedUser" runat="server"></asp:Label>
        </div>
    </div>
    <br />
    <div class="row">
        <uc1:MessageUserControl runat="server" id="MessageUserControl" />
    </div>

    <div class="row">
        <asp:Label ID="Label1" runat="server" Text="Vendor:"></asp:Label>
        <asp:DropDownList ID="VendorNameDDL" runat="server" 
            DataSourceID="VendorNameDDLODS" 
            DataTextField="DisplayText" 
            DataValueField="ValueId"
            AppendDataBoundItems="true">
            <asp:ListItem Value="0" Text="Select a Vendor"></asp:ListItem>
        </asp:DropDownList>&nbsp;&nbsp;
        <asp:Button ID="Select" runat="server" Text="Select" OnClick="Select_Click" />&nbsp;&nbsp;
        <asp:Button ID="PlaceOrder" runat="server" Text="Place Order" OnClick="PlaceOrder_Click" />&nbsp;&nbsp;
        <asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click" />&nbsp;&nbsp;
        <asp:Button ID="Delete" runat="server" Text="Delete" OnClick="Delete_Click" />&nbsp;&nbsp;
        <asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click" />
        <br />
        <asp:Label ID="VendorInformation" runat="server" Text="Vendor Name - Contact - Phone"></asp:Label>
        <asp:TextBox ID="Comments" runat="server" Height="60px" Width="300px"></asp:TextBox>
        <br />
        <asp:Label ID="SubtotalText" runat="server" Text="Subtotal"></asp:Label>
        <asp:TextBox ID="Subtotal" runat="server" Enabled="false"></asp:TextBox>
        <asp:Label ID="TaxText" runat="server" Text="Tax"></asp:Label>
        <asp:TextBox ID="Tax" runat="server" Enabled="false"></asp:TextBox>
        <asp:Label ID="TotalText" runat="server" Text="Total"></asp:Label>
        <asp:TextBox ID="Total" runat="server" Enabled="false"></asp:TextBox>
        <br />
        <asp:ListView ID="ListView_order" runat="server" AutoPostBack="true">
            <AlternatingItemTemplate>
                <tr style="background-color: #FFFFFF; color: #284775;">
                    <td>

                        <asp:LinkButton ID="RemovefromList" runat="server" OnCommand="Delete_Item"
                            CssClass="btn" CommandArgument='<%# Eval("OrderDetailID") %>'><a href="{CCB6EC49-C91B-4D9D-90E9-E1EFD459C57C}|ERaceSystem\ERaceSystem.csproj|c:\dj\7788\1%20edu\1%20nait\term3\dmit2018\6%20github\a03-team-b-project-dw\a03-team-b-project-erace-webapp\eracesystem\bll\purchasing\vendorcontroller.cs">{CCB6EC49-C91B-4D9D-90E9-E1EFD459C57C}|ERaceSystem\ERaceSystem.csproj|c:\dj\7788\1 edu\1 nait\term3\dmit2018\6 github\a03-team-b-project-dw\a03-team-b-project-erace-webapp\eracesystem\bll\purchasing\vendorcontroller.cs</a>
                            <span aria-hidden="true" class="glyphicon glyphicon-remove">&nbsp;</span>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:Label Text='<%# Eval("Product") %>' runat="server" ID="ProductLabel" /></td>
                    <td>
                        <asp:TextBox Text='<%# Eval("OrderQty") %>' runat="server" ID="OrderQtyLabel" Width="50px"></asp:TextBox></td>
                    <td>
                        <asp:Label Text='<%# Eval("UnitSize") %>' runat="server" ID="UnitSizeLabel" /></td>
                    <td>
                        <asp:TextBox Text='<%# string.Format("{0:0.00}", Eval("UnitCost")) %>' runat="server" ID="UnitCostLabel" Width="100px"></asp:TextBox></td>
                    <td>
                        <asp:LinkButton ID="Refresh" runat="server"
                            CssClass="btn" OnClick="Refresh_Click"> 
                            <span aria-hidden="true" class="glyphicon glyphicon-refresh">&nbsp;</span>
                        </asp:LinkButton>
                        <asp:Label runat="server" ID="PerItemCostLabel" /></td>
                    <td>
                        <asp:Label runat="server" ID="ExtendedCostLabel" /></td>
                </tr>
            </AlternatingItemTemplate>
            <EmptyDataTemplate>
                <table runat="server" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px;">
                    <tr>
                        <td>No prodct selected yet, please select an item from the Inventory.</td>
                    </tr>
                </table>
            </EmptyDataTemplate>
            <ItemTemplate>
                <tr style="background-color: #E0FFFF; color: #333333;">
                    <td>
                        <asp:LinkButton ID="RemovefromList" runat="server" OnCommand="Delete_Item"
                            CssClass="btn" CommandArgument='<%# Eval("OrderDetailID") %>'>
                            <span aria-hidden="true" class="glyphicon glyphicon-remove">&nbsp;</span>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:Label Text='<%# Eval("Product") %>' runat="server" ID="ProductLabel" /></td>
                    <td>
                        <asp:TextBox Text='<%# Eval("OrderQty") %>' runat="server" ID="OrderQtyLabel" Width="50px"></asp:TextBox></td>
                    <td>
                        <asp:Label Text='<%# Eval("UnitSize")%>' runat="server" ID="UnitSizeLabel" /></td>
                    <td>
                        <asp:TextBox Text='<%# string.Format("{0:0.00}", Eval("UnitCost")) %>' runat="server" ID="UnitCostLabel" Width="100px"></asp:TextBox></td>
                    <td>
                        <asp:LinkButton ID="Refresh" runat="server"
                            CssClass="btn" OnClick="Refresh_Click"> 
                            <span aria-hidden="true" class="glyphicon glyphicon-refresh">&nbsp;</span>
                        </asp:LinkButton>
                        <asp:Label runat="server" ID="PerItemCostLabel" /></td>
                    <td>
                        <asp:Label runat="server" ID="ExtendedCostLabel" /></td>
                </tr>
            </ItemTemplate>
            <LayoutTemplate>
                <table runat="server">
                    <tr runat="server">
                        <td runat="server">
                            <table runat="server" id="itemPlaceholderContainer" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px; font-family: Verdana, Arial, Helvetica, sans-serif;" border="1">
                                <tr runat="server" style="background-color: #E0FFFF; color: #333333;">
                                    <th runat="server"></th>
                                    <th runat="server">Product</th>
                                    <th runat="server">Order Qty</th>
                                    <th runat="server">Unit Size</th>
                                    <th runat="server">Unit Cost</th>
                                    <th runat="server">Per-Item Cost</th>
                                    <th runat="server">Extended Cost</th>
                                </tr>
                                <tr runat="server" id="itemPlaceholder"></tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </LayoutTemplate>

        </asp:ListView>
        <br />
        <asp:Repeater ID="RepeaterInventory" runat="server" ItemType="ERaceSystem.ViewModels.Purchasing.InventoryList">
            <HeaderTemplate>
                <h3>Inventory</h3>
            </HeaderTemplate>
            <ItemTemplate>
                <h5><strong><%# Item.Description %></strong></h5>
                <asp:ListView ID="ListView_Inventory" DataSource="<%# Item.ProductList %>" runat="server" OnItemCommand="TracksSelectionList_ItemCommand">
                    <AlternatingItemTemplate>
                        <tr style="background-color: #FFFFFF; color: #284775;">
                            <td>
                                <asp:LinkButton ID="AddtoPlaylist" runat="server"
                                    CssClass="btn" CommandArgument='<%# Eval("ProductID") %>'>
                                <span aria-hidden="true" class="glyphicon glyphicon-plus">&nbsp;</span>
                                </asp:LinkButton>
                            </td>
                            <td>
                                <asp:Label Text='<%# Eval("ItemName") %>' runat="server" ID="ItemNameLabel" /></td>
                            <td>
                                <asp:Label Text='<%# Eval("ReOrderLevel") %>' runat="server" ID="ReOrderLevelLabel" /></td>
                            <td>
                                <asp:Label Text='<%# Eval("QuantityOnHand") %>' runat="server" ID="QuantityOnHandLabel" /></td>
                            <td>
                                <asp:Label Text='<%# Eval("QuantityOnOrder") %>' runat="server" ID="QuantityOnOrderlabel" /></td>
                            <td>
                                <asp:Label Text='<%# Eval("Size") %>' runat="server" ID="SizeLabel" /></td>
                        </tr>
                    </AlternatingItemTemplate>
                    <EmptyDataTemplate>
                        <table runat="server" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px;">
                            <tr>
                                <td>No data was returned.</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <ItemTemplate>
                        <tr style="background-color: #E0FFFF; color: #333333;">
                            <td>
                                <asp:LinkButton ID="AddtoPlaylist" runat="server"
                                    CssClass="btn" CommandArgument='<%# Eval("ProductID") %>'>
                <span aria-hidden="true" class="glyphicon glyphicon-plus">&nbsp;</span>
                                </asp:LinkButton>
                            </td>
                            <td>
                                <asp:Label Text='<%# Eval("ItemName") %>' runat="server" ID="ItemNameLabel" /></td>
                            <td>
                                <asp:Label Text='<%# Eval("ReOrderLevel") %>' runat="server" ID="ReOrderLevelLabel" /></td>
                            <td>
                                <asp:Label Text='<%# Eval("QuantityOnHand") %>' runat="server" ID="QuantityOnHandLabel" /></td>
                            <td>
                                <asp:Label Text='<%# Eval("QuantityOnOrder") %>' runat="server" ID="QuantityOnOrderlabel" /></td>
                            <td>
                                <asp:Label Text='<%# Eval("Size") %>' runat="server" ID="SizeLabel" /></td>

                        </tr>
                    </ItemTemplate>
                    <LayoutTemplate>
                        <table runat="server">
                            <tr runat="server">
                                <td runat="server">
                                    <table runat="server" id="itemPlaceholderContainer" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px; font-family: Verdana, Arial, Helvetica, sans-serif;" border="1">
                                        <tr runat="server" style="background-color: #E0FFFF; color: #333333;">
                                            <th runat="server"></th>
                                            <th runat="server">Product</th>
                                            <th runat="server">ReOrder</th>
                                            <th runat="server">In Stock</th>
                                            <th runat="server">On Order</th>
                                            <th runat="server">Size</th>
                                        </tr>
                                        <tr runat="server" id="itemPlaceholder"></tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </LayoutTemplate>
                </asp:ListView>
            </ItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Label Text="" runat="server" ID="vendorValidator" Visible="false" />
    </div>
    <asp:ObjectDataSource ID="VendorNameDDLODS" runat="server" 
        OldValuesParameterFormatString="original_{0}" 
        SelectMethod="List_VendorNames" 
        TypeName="ERaceSystem.BLL.Purchasing.VendorController">
    </asp:ObjectDataSource>
</asp:Content>

