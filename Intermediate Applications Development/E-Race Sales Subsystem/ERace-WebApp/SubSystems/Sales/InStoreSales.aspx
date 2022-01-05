<%@ Page Title="InStore Sales" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InStoreSales.aspx.cs" Inherits="ERace_WebApp.SubSystems.Sales.InStoreSales" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-10">
            <h1>InStore Sales</h1>
        </div>
        <div class="col-2">
            <asp:Label ID="Label7" runat="server" Text="User: "></asp:Label>&nbsp;
            <asp:Label ID="LoggedUser" runat="server"></asp:Label>
        </div>
    </div>
    
    <div class="row">
        <div class="col">
            <h3>Purchases</h3>

            <div class="row">
                <div class="offset-1">
                    <%--Add MessageUserControl--%>
                    <uc1:MessageUserControl runat="server" id="MessageUserControl" />
                </div>
            </div>
        </div>
        <div class="row">
            <div class="row">
                            <asp:Label ID="Label2" runat="server" Text="New Invoice #:   "></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label ID="InvoiceID" runat="server" Font-Bold="false" Font-Underline="false"></asp:Label>
                        </div>
            <div class="row">
                <asp:DropDownList ID="CategoryDDL" runat="server" DataSourceID="CategoryODS" DataTextField="DisplayText" DataValueField="ValueId"
                     CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="CategoryDDL_SelectedIndexChanged" Width="300px">
                    <asp:ListItem Value="0">[Select a Category]</asp:ListItem>
                </asp:DropDownList>            
                <asp:DropDownList ID="ProductDDL" runat="server" AutoPostBack="True" DataSourceID="ProductODS" DataTextField="DisplayText" DataValueField="ValueId"
                     CssClass="form-control" AppendDataBoundItems="false" Width="300px" OnSelectedIndexChanged="ProductDDL_SelectedIndexChanged">
                    <asp:ListItem Value="0">[Select a Product]</asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="ProductArg" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="QtyArg" runat="server" Visible="false"></asp:Label>

                <asp:Label ID="Label1" runat="server" Text="Quantity:   "></asp:Label>
                <asp:TextBox ID="Quantity" runat="server" TextMode="Number" PlaceHolder="Qty"> </asp:TextBox>
                <asp:LinkButton ID="AddButton" runat="server"  OnClick="AddButton_Click"  CssClass="btn btn-primary btn-xs">
                        <i class="fa fa-plus" style="color:antiquewhite; font-size:1em;">   Add Product</i>&nbsp;
                 </asp:LinkButton>
            <div class="row">
                <asp:GridView ID="InvoiceDetailGV" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-striped" GridLines="none" BorderStyle="None"
                    CellPadding="4" ForeColor="#333333">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775"></AlternatingRowStyle>

                    <Columns>
                        <asp:TemplateField HeaderText="Product">
                            <ItemTemplate>
                                <asp:Label ID="ProductID" runat="server" Text='<%# Eval("ProductID") %>' Visible="false"></asp:Label>
                                <asp:Label ID="Product" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>&nbsp;&nbsp;
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Quantity">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID ="QuantityBought" TextMode="Number" Text='<%# Eval("Quantity") %>'></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Price">
                            <ItemTemplate>
                                 <asp:LinkButton ID="Refresh" runat="server" 
                                     CssClass="btn" OnCommand="Refresh_Command" CommandArgument='<%# Eval("ProductID") %>'>
                                     <i class="fa fa-refresh" style="color:forestgreen; font-size:1.25em;"></i>
                                </asp:LinkButton>&nbsp;&nbsp;
                                <asp:Label ID="Price" runat="server" Text='<%# string.Format("{0:0.00}",Eval("Price")) %>'></asp:Label>&nbsp;&nbsp;
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Amount">
                            <ItemTemplate>
                                <asp:Label ID="Amount" runat="server" Text='<%# string.Format("{0:0.00}",Eval("Amount")) %>'></asp:Label>&nbsp;&nbsp;
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:LinkButton ID="RemoveProduct" runat="server"
                                                CssClass="btn" OnCommand="RemoveProduct_Command" CommandArgument='<%# Eval("ProductID") %>'>
                                    <i class="fa fa-times" style="color:red; font-size:1.25em;"></i>&nbsp;                                   
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                     <EmptyDataTemplate>
                        No Products selected for Sale
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
                <div class="row">
                    <div class="col">
                        <div class="row">
                            <asp:Label ID="Label3" runat="server" Text="SubTotal:   "></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label ID="Subtotal" runat="server" ></asp:Label>
                        </div>
                    
                        <div class="row">
                            <asp:Label ID="Label5" runat="server" Text="Tax:   "></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label ID="Tax" runat="server" ></asp:Label>
                        </div>
                    
                        <div class="row">
                            <asp:Label ID="Label8" runat="server" Text="Total:   "></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label ID="Total" runat="server" ></asp:Label>
                        </div>        
                    </div> 
                    <div class="row">
                        <asp:Button ID="PayButton" runat="server" Text="Payment: Cash/Debit" 
                            CssClass="btn btn-secondary btn-xs"
                            style="color:antiquewhite; background-color:darkgreen; font-size:1em;" OnClick="PayButton_Click" />
                        <asp:LinkButton ID="Clear" runat="server"  CssClass="btn" OnClick="Clear_Click">
                             <i class="fas fa-shopping-cart" style="color:teal; font-size:1em;"></i>&nbsp;&nbsp;
                            Clear Cart</asp:LinkButton>               
                    </div>
                </div>
               
           


    <%--    ODS Controls--%> 
            <asp:ObjectDataSource ID="CategoryODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="ListCategories" TypeName="ERaceSystem.BLL.CategoryController"></asp:ObjectDataSource>
            <asp:ObjectDataSource ID="ProductODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="ProductsByCategory" TypeName="ERaceSystem.BLL.ProductController">
                <SelectParameters>
                    <asp:ControlParameter ControlID="CategoryDDL" PropertyName="SelectedValue" Name="categoryid" Type="Int32"></asp:ControlParameter>
                </SelectParameters>
            </asp:ObjectDataSource>
                <br />
                <br />
                <br />
                <br />

             <div class="row">
                <div class="col-med-2">
                    <a class="btn btn-default" href="ReturnRefunds.aspx">Returns/Refunds Page &raquo;</a>
       
                    <a class="btn btn-default" href="Default.aspx">Default Page &raquo;</a>
                </div>
            </div>
           
        </div>
    </div>
    </div>
</asp:Content>
