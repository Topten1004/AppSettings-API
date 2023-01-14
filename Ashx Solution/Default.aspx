<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Application" Width="150px"></asp:Label>
            <asp:TextBox ID="txtApplication" runat="server"></asp:TextBox><br />

            <asp:Label ID="Label2" runat="server" Text="Base" Width="150px"></asp:Label>
            <asp:TextBox ID="txtBaseKey" runat="server"></asp:TextBox><br />

            <asp:Label ID="Label3" runat="server" Text="Control" Width="150px"></asp:Label>
            <asp:TextBox ID="txtControlKey" runat="server"></asp:TextBox><br />

            <asp:Label ID="Label4" runat="server" Text="Attribute" Width="150px"></asp:Label>
            <asp:TextBox ID="txtAttributeKey" runat="server"></asp:TextBox><br />

            <asp:Label ID="Label5" runat="server" Text="Value" Width="150px"></asp:Label>
            <asp:TextBox ID="txtValue" runat="server"></asp:TextBox><br />

        </div>
        <asp:Button ID="btnRead" runat="server" Text="Read" />
        <asp:Button ID="btnWrite" runat="server" Text="Write" />
        
        <p>
            <br />
            <asp:LinkButton ID="btnLink" runat="server"></asp:LinkButton>
        </p>
        <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
    </form>
</body>
</html>
