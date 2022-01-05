<%@ Page Title="Returns/Refunds" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReturnRefunds.aspx.cs" Inherits="ERace_WebApp.SubSystems.Sales.ReturnRefunds" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">
        <div class="jumbotron">
            <h1>Refunds/ Returns </h1>
        </div>
        <div class="col-2">
            <asp:Label ID="Label7" runat="server" Text="User: "></asp:Label>&nbsp;
            <asp:Label ID="LoggedUser" runat="server"></asp:Label>
        </div>
    </div>
    <div class="row">
        <div class="offset-1">
            <%--Add MessageUserControl--%>
                <uc1:MessageUserControl runat="server" id="MessageUserControl" />
        </div>
    </div>
    <div class="row">
        <asp:Label ID="InvoiceArg" runat="server" visible="false"></asp:Label>
        <asp:TextBox ID="OriginalInvoice" runat="server" TextMode="Number" 
            ToolTip="Original Invoice #"></asp:TextBox>&nbsp;&nbsp;

        <asp:LinkButton ID="LookupInvoice" runat="server" CssClass="btn" 
            OnClick="LookupInvoice_Click">Lookup Invoice</asp:LinkButton>&nbsp;&nbsp;
        <asp:LinkButton ID="Clear" runat="server"  CssClass="btn" 
            OnClick="Clear_Click">Clear</asp:LinkButton>&nbsp;&nbsp;
    </div>

    <div class="col-sm-10">
        <asp:GridView ID="RefundDetailsGV" runat="server" AutoGenerateColumns="False"
            CssClass="table table-striped" GridLines="none" BorderStyle="None"
            CellPadding="4" ForeColor="#333333" DataSourceID="RefundListODS">
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
                            <asp:Label runat="server" ID ="QuantityBought" TextMode="Number" Text='<%# Eval("Quantity") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Price">
                        <ItemTemplate>
                            <asp:Label ID="Price" runat="server" Text='<%# string.Format("{0:0.00}",Eval("Price")) %>'></asp:Label>&nbsp;&nbsp;
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Amount">
                        <ItemTemplate>
                            <asp:Label ID="Amount" runat="server" Text='<%# string.Format("{0:0.00}",Eval("Amount")) %>'></asp:Label>&nbsp;&nbsp;
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Restock">
                        <ItemTemplate>
                            <asp:CheckBox ID="RestockCheckBox" runat="server" />&nbsp;&nbsp;
                            <asp:Label ID="RestockCharge" runat="server" Text='<%# string.Format("{0:0.00}",Eval("RestockCharge")) %>'></asp:Label>&nbsp;&nbsp;
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Reason">
                        <ItemTemplate>
                            <asp:CheckBox ID="RefundCheckBox" runat="server" />&nbsp;&nbsp;
                            <asp:TextBox ID="RestockCharge" runat="server" Text='<%# Eval("Reason") %>'></asp:TextBox>&nbsp;&nbsp;
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                    <EmptyDataTemplate>
                    No Products selected for Refunds.
                </EmptyDataTemplate>
            </asp:GridView>        
        </div>
    <div class="col-2">
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
                <asp:Label ID="Label8" runat="server" Text="Refund Total:   "></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="RefundTotal" runat="server" ></asp:Label>
            </div>        
        </div> 
     <div class="row">
           <%-- <asp:Button ID="RefundButton" runat="server" Text="Refund: Cash/Debit" 
                CssClass="btn btn-secondary btn-xs"
                style="color:antiquewhite; background-color:darkgreen; font-size:1em;" OnClick="RefundButton_Click" />--%>
        </div>
          <div class="row">              
              <asp:Label ID="Label1" runat="server" Text ="Refund Invoice #:   "></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="RefundInvoiceID" runat="server"></asp:Label>
          </div>

        
    </div>
     
                   
                </div>





    <asp:ObjectDataSource ID="RefundListODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="Invoicedetails_FindById" TypeName="ERaceSystem.BLL.StoreRefundController">
        <SelectParameters>
            <asp:ControlParameter ControlID="InvoiceArg" PropertyName="Text" Name="invoiceid" Type="Int32"></asp:ControlParameter>
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
                <br />
                <br />
                <br />
    <div class="row">
        <div class="col-med-2">
            <a class="btn btn-default" href="InStoreSales.aspx">In-Store Sales Page &raquo;</a>
       
            <a class="btn btn-default" href="Default.aspx">Default Page &raquo;</a>
        </div>
    </div>
   
</asp:Content>
