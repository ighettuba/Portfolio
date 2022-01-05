<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="ERace_WebApp.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Some extras about the project</h2>
    <br />
    <h3>WebConnectionString Details</h3>
    <br />
    <i>You can copy the code below to your WebConnectionStrings.config file to connect to database, make sure to remove extra space</i>
    <br />
    <hr />
    <br />
    <p>
        < ?xml version="1.0"?>
    </p>
    <p>
    < connectionStrings>
    </p>
    <p>
    < add name="eRaceDB"
       connectionString="Data Source=.; Initial Catalog=eRace; Integrated Security=true;"
       providerName="System.Data.SqlClient"/>
    </p>
    <p>
    < add name="DefaultConnection" 
       connectionString="Data Source=.;Initial Catalog=eRace;Integrated Security=True"
       providerName="System.Data.SqlClient" />
    </p>
    <p>
    < /connectionStrings>
    </p>
    
    <br />
    <hr />
    <br />
    <h3>Security login settings</h3>
    <br />
    <ul>
        <li>Role: Administrator</li>
        <li>User Name: Webmaster</li>
        <li>Password: Pa$$w0rd</li>
    </ul>
    <ul>
        <li>Role: Food Service</li>
        <li>User Name: MarlaKreeg</li>
        <li>Password: Pa$$w0rd2018</li>
    </ul>
    <ul>
        <li>Role: Clerk</li>
        <li>User Name: MarceauSerat</li>
        <li>Password: Pa$$w0rd2018</li>
    </ul>
    <ul>
        <li>Role: Office Manager </li>
        <li>User Name: KevinBeecham</li>
        <li>Password: Pa$$w0rd2018</li>
    </ul>
    <ul>
        <li>Role: Director</li>
        <li>User Name: IshratImsri</li>
        <li>Password: Pa$$w0rd2018</li>
    </ul>
     <ul>
        <li>Role: Racing Coordinator</li>
        <li>User Name: MartaThargg</li>
        <li>Password: Pa$$w0rd2018</li>
    </ul>

</asp:Content>
