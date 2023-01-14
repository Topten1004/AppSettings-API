
Imports System.Net

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.lblError.Text = ""
    End Sub

    Protected Sub btnRead_Click(sender As Object, e As EventArgs) Handles btnRead.Click
        Submit(Me.txtApplication.Text, Me.txtBaseKey.Text, Me.txtControlKey.Text, Me.txtAttributeKey.Text, Nothing)
    End Sub

    Protected Sub btnWrite_Click(sender As Object, e As EventArgs) Handles btnWrite.Click
        Dim value = Me.txtValue.Text
        value = value.Replace(Chr(0), String.Empty)
        Submit(Me.txtApplication.Text, Me.txtBaseKey.Text, Me.txtControlKey.Text, Me.txtAttributeKey.Text, value)
    End Sub

    Private Sub Submit(application As String, baseKey As String, controlKey As String, attributeKey As String, valueWriteDecoded As String)
        Dim documentPath = Request.Url.GetLeftPart(UriPartial.Authority) & VirtualPathUtility.ToAbsolute("~/") ' TB.SystemMain.DocumentPath.Replace("/", "\") & "database\"
        'Dim di As New IO.DirectoryInfo(documentPath)
        Dim url = $"{documentPath}data.ashx?application={application}&base={baseKey}&control={controlKey}&attribute={attributeKey}&"
        If valueWriteDecoded IsNot Nothing Then
            Dim valueEncoded = TB.String.ToBase64String(valueWriteDecoded)
            url = url & $"value={valueEncoded}"
        End If
        Me.btnLink.PostBackUrl = url
        Me.btnLink.Text = url

        Dim wc As New WebClient
        Dim uri As New Uri(url, UriKind.Absolute)
        Dim valueReadDecoded = wc.DownloadString(uri)
        Try
            'Dim valueReadDecoded = valueReadEncoded 'TB.String.FromBase64String(valueReadEncoded)
            Me.txtValue.Text = valueReadDecoded
        Catch ex As Exception
            Me.lblError.Text = ex.Message
        End Try

    End Sub
End Class
