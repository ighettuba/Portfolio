<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ERace_WebApp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Team B</h1>
        <asp:Image ID="Image1" runat="server" src="logo.png" Height="191px" Width="218px"/>
        <p class="lead">Welcome to Team B Project. We are Wanlun, Jing and Ian.</p>
        <p>This is the E-Race Web Application:</p>
        <br />
        <br />
        <p><a href="https://dmit-2018.github.io/eRace/" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Ian J Ghettuba</h2>
            <p><b>Sales Subsystem</b></p>
            <p>Setup/Security Responsibilities</p>
            <ul>
                <li>Completed ReadMe file</li>
                <li>Created VS solution</li>
                <li>Created subfolders</li>
                <li>Edited Default Page</li>
                <li>Created BusinessRuleException class</li>
                <li>Added Web Application Security</li>

            </ul>
            <p>Known bug list</p>
            <ul>
                <li>Was not able to make Refunds Page functional</li>
                <li>Refunds Page not tested</li>
            </ul>
            <a class="btn btn-default" href="SubSystems/Sales/Default.aspx">Sales Page &raquo;</a>
        </div>
        <div class="col-md-4">
            <h2>Jing Di</h2>
            <p><b>Purchasing Subsystem</b></p>
            <p>Setup/Security Responsibilities</p>
            <ul>
                <li>Edit gitignore</li>
                <li>to be filled in</li>
                <li>to be filled in</li>
            </ul>
            <p>Known bug list</p>
            <ul>
                <li>to be filled in</li>
                <li>to be filled in</li>
                <li>to be filled in</li>
            </ul>
            <a class="btn btn-default" href="SubSystems/Purchasing/Default.aspx">Purchasing Page &raquo;</a>
        </div>
        <div class="col-md-4">
            <h2>Wanlun Xue</h2>
            <p><b>Receiving subsystem</b></p>
            <p>Setup/Security Responsibilities</p>
            <ul>
                <li>Set up webpage for subfolders</li>
                <li>Create Team Logo</li>
                <li>Edit Default page, Contact pages</li>
                <li>Create About page with connectionstring and login info</li>
                <li>Navigation on Site Master</li>
                <li>Reverse Engineering database</li>
            </ul>
            <p>Known bug list</p>
            <ul>
                <li>Nothing so far</li>
            </ul>
            <a class="btn btn-default" href="SubSystems/Receiving/DefaultReceiving.aspx">Receiving Page &raquo;</a>

        </div>
</div>       

</asp:Content>
