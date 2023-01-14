<%@ WebHandler Language="VB" Class="data" %>

Imports System
Imports System.Web

Public Class data : Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        ' Write your handler implementation here.
        Dim applicationKey As String = GetQuery(context, "application")
        Dim baseKey As String = GetQuery(context, "base")
        Dim controlKey As String = GetQuery(context, "control")
        Dim attributeKey As String = GetQuery(context, "attribute")
        Dim responseMode As String = GetQuery(context, "mode")
        Dim value As String = GetQuery(context, "value")

        Dim di As New IO.DirectoryInfo(TB.SystemMain.DocumentPath.Replace("/", "\") & "database\")
        If applicationKey Is Nothing Then Throw New ArgumentNullException("applicationKey")
        If baseKey Is Nothing Then Throw New ArgumentNullException("base")
        If controlKey Is Nothing Then Throw New ArgumentNullException("control")
        If attributeKey Is Nothing Then Throw New ArgumentNullException("attribute")
        If responseMode Is Nothing Then responseMode = "text"

        context.Response.ContentType = "text/plain"

        context.Response.Headers.Add("Access-Control-Allow-Origin", "*")
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept")
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE")

        Dim fi As New IO.FileInfo(IO.Path.Combine(di.FullName, applicationKey, baseKey, controlKey, attributeKey & ".txt"))

        If value IsNot Nothing Then
            Try
                value = TB.String.FromBase64String(value)
            Catch ex As Exception
            End Try
            TB.FileSystem.CreateDirectories(fi.Directory)
            My.Computer.FileSystem.WriteAllText(fi.FullName, value, False, System.Text.Encoding.Unicode)
            fi.Refresh()
        End If

        If fi.Exists Then
            Dim fileContent = My.Computer.FileSystem.ReadAllText(fi.FullName, System.Text.Encoding.Unicode)
            If responseMode = "base64" Then
                fileContent = TB.String.ToBase64String(fileContent)
            End If
            context.Response.Write(fileContent)
        Else
            context.Response.Write("")
            context.Response.End()
        End If
    End Sub

    Private Function GetQuery(ByVal context As HttpContext, ByVal key As String) As String
        Dim ret As String = ""
        Try
            ret = context.Request.Item(key)
        Catch ex As Exception

        End Try
        Return ret
    End Function

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class