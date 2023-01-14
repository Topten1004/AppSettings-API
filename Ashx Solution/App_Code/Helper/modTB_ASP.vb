'Imports System.Runtime.InteropServices
'Imports System.Text
'Version 1.0.0.1
Option Strict Off
Imports System.Drawing
Imports Microsoft.VisualBasic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System


Public Module PageExtension




    ''' <summary>
    ''' Search rekursiv for the HTML control
    ''' </summary>
    ''' <param name="control">the parent control to search in</param>
    ''' <param name="id">the name of the control</param>
    ''' <returns>the html element like a div</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension> _
    Public Function FindHtmlControlByIdInControl(control As Control, id As String) As HtmlControl
        For Each childControl As Control In control.Controls
            If childControl.ID IsNot Nothing AndAlso childControl.ID.Equals(id, StringComparison.OrdinalIgnoreCase) AndAlso TypeOf childControl Is HtmlControl Then
                Return DirectCast(childControl, HtmlControl)
            End If

            If childControl.HasControls() Then
                Dim result As HtmlControl = FindHtmlControlByIdInControl(childControl, id)
                If result IsNot Nothing Then
                    Return result
                End If
            End If
        Next

        Return Nothing
    End Function


    ''' <summary>
    ''' Gets the ID of the post back control.
    ''' 
    ''' See: http://geekswithblogs.net/mahesh/archive/2006/06/27/83264.aspx
    ''' </summary>
    ''' <param name = "page">The page.</param>
    ''' <returns></returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function GetPostBackControlId(page As Page) As String
        If Not page.IsPostBack Then
            Return String.Empty
        End If

        Dim control As Control = Nothing
        ' first we will check the "__EVENTTARGET" because if post back made by the controls
        ' which used "_doPostBack" function also available in Request.Form collection.
        Dim controlName As String = page.Request.Params("__EVENTTARGET")
        If Not String.IsNullOrEmpty(controlName) Then
            control = page.FindControl(controlName)
        Else
            ' if __EVENTTARGET is null, the control is a button type and we need to
            ' iterate over the form collection to find it

            ' ReSharper disable TooWideLocalVariableScope
            Dim controlId As String
            Dim foundControl As Control
            ' ReSharper restore TooWideLocalVariableScope

            For Each ctl As String In page.Request.Form
                ' handle ImageButton they having an additional "quasi-property" 
                ' in their Id which identifies mouse x and y coordinates
                If ctl.EndsWith(".x") OrElse ctl.EndsWith(".y") Then
                    controlId = ctl.Substring(0, ctl.Length - 2)
                    foundControl = page.FindControl(controlId)
                Else
                    foundControl = page.FindControl(ctl)
                End If

                If Not (TypeOf foundControl Is Button OrElse TypeOf foundControl Is ImageButton) Then
                    Continue For
                End If

                control = foundControl
                Exit For
            Next
        End If

        Return If(control Is Nothing, String.Empty, control.ID)
    End Function


    ''' <summary>
    ''' Focus any control on page.
    ''' </summary>
    ''' <param name = "page">The page.</param>
    <System.Runtime.CompilerServices.Extension> _
    Public Sub FocusControl(page As Page, ByVal ClientId As String)
        Dim clientScript As ClientScriptManager = page.ClientScript

        clientScript.RegisterClientScriptBlock(page.GetType, "CtrlFocus", (Convert.ToString("<script> " & vbCr & vbLf & vbCr & vbLf & "          function ScrollView()" & vbCr & vbLf & vbCr & vbLf & "          {" & vbCr & vbLf & "             var el = document.getElementById('") & ClientId) + "')" & vbCr & vbLf & "             if (el != null)" & vbCr & vbLf & "             {        " & vbCr & vbLf & "                el.scrollIntoView();" & vbCr & vbLf & "                el.focus();" & vbCr & vbLf & "             }" & vbCr & vbLf & "          }" & vbCr & vbLf & vbCr & vbLf & "          window.onload = ScrollView;" & vbCr & vbLf & vbCr & vbLf & "          </script>")
    End Sub



End Module

Namespace TB
    Namespace [Enumerations]
        Public Module modEnumeration
            Public Enum InitializeEnum As Byte
                NotInitialized = 0
                Initialized = 1
                IsInitializing = 2
            End Enum

        End Module
    End Namespace


    Namespace [String]
        Public Module modString

            ''' <summary>
            ''' Compresses the string.
            ''' </summary>
            ''' <param name="text">The text.</param>
            ''' <returns></returns>
            Public Function CompressData(text As String) As String
                Dim buffer__1 As Byte() = System.Text.Encoding.UTF8.GetBytes(text)
                Return CompressDataToBase64(buffer__1)
            End Function


            ''' <summary>
            ''' Compresses the string.
            ''' </summary>
            ''' <param name="data">The data.</param>
            ''' <returns></returns>
            Public Function CompressData(data() As Byte) As Byte()
                Dim memoryStream = New IO.MemoryStream()
                Using gZipStream = New IO.Compression.GZipStream(memoryStream, IO.Compression.CompressionMode.Compress, True)
                    gZipStream.Write(data, 0, data.Length)
                End Using

                memoryStream.Position = 0

                Dim compressedData = New Byte(memoryStream.Length - 1) {}
                memoryStream.Read(compressedData, 0, compressedData.Length)

                Dim gZipBuffer = New Byte(compressedData.Length + 3) {}
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length)
                Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, gZipBuffer, 0, 4)
                Return gZipBuffer
            End Function

            ''' <summary>
            ''' Compresses the string.
            ''' </summary>
            ''' <param name="data">The data.</param>
            ''' <returns></returns>
            Public Function CompressDataToBase64(data() As Byte) As String
                Dim gZipBuffer As Byte() = CompressData(data)
                Return Convert.ToBase64String(gZipBuffer)
            End Function


            ''' <summary>
            ''' Decompresses the string.
            ''' </summary>
            ''' <param name="compressedText">The compressed text.</param>
            ''' <returns></returns>
            Public Function DecompressString(compressedText As String) As String
                Dim Data As Byte() = DecompressData(compressedText)
                Return System.Text.Encoding.UTF8.GetString(Data)
            End Function

            ''' <summary>
            ''' Decompresses the string.
            ''' </summary>
            ''' <param name="gZipBuffer">The compressed text.</param>
            ''' <returns></returns>
            Public Function DecompressData(gZipBuffer As Byte()) As Byte()
                Using memoryStream = New IO.MemoryStream()
                    Dim dataLength As Integer = BitConverter.ToInt32(gZipBuffer, 0)
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4)

                    Dim buffer = New Byte(dataLength - 1) {}

                    memoryStream.Position = 0
                    Using gZipStream = New IO.Compression.GZipStream(memoryStream, IO.Compression.CompressionMode.Decompress)
                        gZipStream.Read(buffer, 0, buffer.Length)
                    End Using

                    Return buffer
                End Using
            End Function

            ''' <summary>
            ''' Decompresses the string.
            ''' </summary>
            ''' <param name="compressedText">The compressed text.</param>
            ''' <returns></returns>
            Public Function DecompressData(compressedText As String) As Byte()
                Dim gZipBuffer As Byte() = Convert.FromBase64String(compressedText)
                Return DecompressData(gZipBuffer)
            End Function


            ''' <summary>
            ''' Returns a random string from minlenght character until maximum length caracter.
            ''' </summary>
            ''' <param name="LengthMin">Minimum length of text.</param>
            ''' <param name="LengthMax">Maxmimum length of text.</param>
            ''' <returns>random string like lorem ipsum website</returns>
            Public Function LoremIpsum(ByVal LengthMin As Integer, ByVal LengthMax As Integer) As String
                If LengthMax <= 0 Then Return String.Empty
                Dim W As String = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec dui sapien, dignissim id lobortis eget, euismod ac odio. Praesent vitae mauris magna, sed malesuada tortor. Integer varius interdum hendrerit. Integer malesuada interdum turpis vel dictum. Nulla facilisi. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Sed non mi lectus. Integer nunc augue, egestas ac ultrices fermentum, ultricies tempus urna. Nunc consectetur, odio ac malesuada porta, erat arcu scelerisque nulla, ac ullamcorper odio justo sed eros. Phasellus eu ante at tellus dignissim consectetur. Sed a elementum enim. Pellentesque dapibus congue felis at viverra.<newline><newline>Donec sollicitudin hendrerit nisl, et pretium nunc aliquam eu. Integer varius risus et turpis iaculis consequat id in dui. Morbi ac leo id urna viverra viverra nec et urna. Suspendisse ullamcorper ullamcorper velit a dictum. Morbi eu urna non nunc scelerisque varius. Nunc quis magna quis nulla placerat tincidunt. Aenean ultricies turpis in nisl ornare eleifend. Nunc aliquam rutrum sem, a mattis nibh tincidunt eget. Aliquam eget dapibus leo. Ut vehicula sollicitudin vestibulum. Etiam suscipit enim a tortor sodales quis pellentesque sem interdum. Etiam fermentum, nibh vitae facilisis ultrices, metus eros blandit velit, at faucibus tortor ligula nec turpis.<newline><newline>Sed ultricies blandit ipsum eu viverra. Etiam at dolor in massa mollis tristique non ut augue. Nullam non leo nunc, eu malesuada felis. Praesent non adipiscing lorem. Aliquam enim mauris, vulputate sit amet consequat a, laoreet sit amet est. Integer ultrices pharetra nisl vitae dictum. Mauris tortor eros, consequat at ultricies ac, ultrices eu tortor.<newline><newline>Proin aliquam dictum metus et ultrices. Aliquam consectetur convallis fermentum. In suscipit adipiscing metus iaculis scelerisque. Phasellus non blandit urna. Donec eu orci mi, vitae pulvinar dolor. Morbi rutrum tincidunt enim, quis dapibus urna porta et. Vivamus vitae lacus nunc. Etiam volutpat tincidunt massa sit amet pharetra. Nulla fermentum congue fermentum. Donec porta, augue ornare posuere placerat, ante mauris cursus purus, ac sodales nibh risus varius velit. Maecenas sem sem, dapibus id egestas ac, fermentum sit amet nisl. Cras at tortor erat, quis egestas est. Nullam et elit ut nisi aliquam volutpat. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Praesent hendrerit feugiat consequat. Cras semper, mauris vitae accumsan posuere, purus nibh volutpat velit, non pretium dolor velit in neque.<newline><newline>Maecenas sit amet mauris adipiscing dui molestie pretium. Donec molestie sollicitudin porttitor. Donec in elit lorem, vel tempus nisi. Nulla bibendum vestibulum lorem, id interdum tortor dictum sit amet. Vivamus ut nisi turpis, eget ullamcorper lorem. Integer accumsan, metus nec malesuada facilisis, turpis mi tempus magna, vitae luctus enim nisi eu nibh. Proin erat quam, porttitor a porttitor condimentum, adipiscing ut lorem. Suspendisse euismod nulla non risus ultricies in suscipit eros pharetra. Sed turpis justo, hendrerit id ultrices a, sollicitudin at libero.<newline><newline>Nullam ac nisl ac tortor pulvinar laoreet id eget nunc. Proin venenatis molestie erat, eget iaculis magna suscipit vel. Donec fringilla convallis ipsum sed facilisis. Integer nec sapien lacus, quis sollicitudin eros. Curabitur malesuada, urna aliquam consequat eleifend, magna velit convallis nunc, et feugiat mi quam a lectus. Sed eu urna non quam fermentum auctor. Proin dignissim auctor nisi a volutpat. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vivamus at risus sed ante consectetur mollis. Fusce sit amet erat massa, ac mattis nisl. Quisque tempor vehicula nunc sed feugiat. Etiam ac arcu vitae tellus egestas pharetra quis ut justo. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Etiam quis felis eget urna laoreet pellentesque. Maecenas dictum tempor dolor sit amet mattis.<newline><newline>Sed scelerisque lectus et eros vehicula et viverra arcu lobortis. Aenean nec risus a quam eleifend pellentesque. Vivamus ultrices porttitor risus, id lobortis est semper vel. Vestibulum iaculis dolor eu urna tincidunt gravida vehicula nulla pulvinar. Sed in odio id nisi laoreet lobortis id eu tortor. Nulla eu magna at magna condimentum aliquam at vitae neque. Nulla facilisi. Aliquam consectetur pretium ligula a eleifend. Proin bibendum sem id odio scelerisque cursus. Donec lobortis, dui auctor pharetra cursus, ante eros aliquam velit, eget mollis purus leo vitae ipsum. Aenean pulvinar consequat urna sed scelerisque. Sed vitae massa in mauris posuere porta. Curabitur laoreet, est id ultrices cursus, dui ante imperdiet dui, et ultrices ante orci vitae urna. Maecenas gravida magna sit amet nunc accumsan varius. Ut fermentum, ante dictum dapibus tristique, nisl enim volutpat justo, suscipit interdum nisl velit nec elit. Nulla facilisi.<newline><newline>Mauris nec enim eu ligula condimentum commodo tristique ornare lectus. Quisque feugiat felis ut mi imperdiet et feugiat sem vestibulum. Ut commodo nunc nec turpis lobortis id tempus nisi consectetur. Duis sit amet arcu libero. Aliquam sodales neque eu lacus viverra rhoncus. Etiam tempus tempor dui, sed dapibus neque sagittis nec. Duis scelerisque dapibus pharetra. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.<newline><newline>Sed gravida vehicula nulla, et pharetra neque porttitor eu. Aenean feugiat massa nec dui rutrum id gravida sem tincidunt. Vestibulum porta hendrerit elementum. Phasellus imperdiet vestibulum risus, at semper ligula sodales vel. Pellentesque pharetra pulvinar gravida. Cras vulputate mauris at arcu lobortis quis dapibus ante ultricies. Aenean elementum, erat eget pulvinar varius, leo arcu rutrum libero, vel lacinia est velit et tortor. Donec vestibulum semper varius. Vivamus ultricies dignissim leo eu bibendum. Sed vitae porttitor purus.<newline><newline>Quisque et ornare mauris. Duis pretium erat pulvinar nunc placerat at pretium nisi pharetra. Proin eget nibh tellus. Mauris ut porta dolor. Nunc id lobortis dolor. Pellentesque placerat sagittis nunc, non pharetra justo rhoncus vel. Integer in nisi id purus rutrum facilisis vel a massa. Nulla facilisi. Fusce scelerisque, mauris vel blandit feugiat, turpis turpis iaculis elit, vitae interdum erat nisl eget nulla.<newline><newline>Pellentesque ut posuere leo. Ut aliquam lacus eu lacus ultrices condimentum. Sed bibendum cursus libero ac pretium. Cras at felis vel augue imperdiet rutrum nec in sem. Suspendisse diam diam, hendrerit id ultrices vitae, vestibulum nec elit. Praesent dapibus ornare velit, quis bibendum elit ultricies sit amet. Nam porttitor ipsum sit amet dolor malesuada aliquam. Vestibulum sodales nisl non mauris aliquam elementum. Etiam tristique, velit id facilisis eleifend, augue libero pulvinar orci, volutpat faucibus magna risus eget mauris. Integer vel est a justo tincidunt bibendum.<newline><newline>Vestibulum non est ut enim accumsan adipiscing. Sed sollicitudin sagittis sem ac gravida. Fusce sed arcu tellus, at viverra lectus. Aenean eget volutpat arcu. In at luctus libero. Sed dapibus nunc a leo rutrum tempus. Pellentesque iaculis bibendum nisl non gravida. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.<newline><newline>In dapibus tincidunt tellus in dapibus. Ut nec lectus nec eros scelerisque imperdiet at eget libero. Duis egestas eleifend vulputate. Ut congue gravida consectetur. Proin id leo sapien, in dignissim orci. Maecenas dolor nulla, auctor nec iaculis a, venenatis id sem. Donec dignissim lacus non quam sagittis bibendum porttitor faucibus ligula. Aenean elementum libero et neque pellentesque sed vestibulum tortor viverra. Phasellus nisi justo, dapibus eu ornare a, pulvinar non turpis. Aliquam pellentesque diam ut eros dapibus nec suscipit nunc egestas. Suspendisse velit nibh, posuere sed suscipit a, posuere et nisi. Pellentesque ac nisl a nunc euismod commodo eget sed diam. Ut convallis, augue congue tempus eleifend, nisi ipsum eleifend est, a auctor dolor diam sed massa. Cras non velit massa.<newline><newline>Nullam ut turpis vitae quam interdum mollis quis non nibh. Maecenas massa mi, faucibus quis tincidunt at, euismod eget nibh. Maecenas aliquam porta mauris eget imperdiet. Duis ultrices eros suscipit elit accumsan consectetur. Sed vitae ipsum elit, in egestas mi. Praesent arcu nulla, aliquet interdum condimentum nec, venenatis volutpat dui. Vestibulum vitae nulla urna. Suspendisse potenti. Sed luctus fringilla ipsum, eu dapibus enim fermentum sit amet. Cras ullamcorper erat ac ante tincidunt eget porta sapien pulvinar. Fusce tincidunt sodales orci, nec lacinia nunc blandit eu.<newline><newline>Maecenas vitae metus justo, vitae accumsan tortor. Nunc blandit ultricies enim nec porta. Proin facilisis quam vitae sapien ultricies eu iaculis diam feugiat. Nullam vel nisl vitae magna eleifend accumsan in vitae lorem. Praesent in suscipit sapien. Sed metus ligula, ultricies viverra bibendum non, pulvinar ut velit. Phasellus congue rutrum nisl et placerat. Pellentesque rhoncus pharetra urna ut pulvinar. Sed egestas nulla quis leo consequat nec congue urna condimentum. Ut vitae purus elit. Sed eu massa nisl, quis sollicitudin velit. Suspendisse potenti. Aliquam tempor neque vitae velit tincidunt varius. In erat nibh, dignissim id bibendum faucibus, egestas in sem. Curabitur ligula est, facilisis in vehicula porttitor, sodales ullamcorper quam.<newline><newline>Integer iaculis dapibus erat, ultricies imperdiet justo tristique eget. In hac habitasse platea dictumst. Integer mauris mauris, dictum gravida mattis nec, mollis id lacus. Etiam ut magna turpis. Sed mi turpis, varius sit amet condimentum quis, tincidunt aliquet felis. Sed velit enim, laoreet nec aliquet ut, laoreet nec felis. Nulla et nisl eu quam tincidunt volutpat. Morbi condimentum malesuada tellus, eu sagittis leo rutrum ut. Phasellus vulputate dolor aliquet enim molestie a gravida urna consequat. Mauris quis fringilla augue. Ut vel nibh et elit lobortis lobortis sed at est. Aenean ante nisi, cursus vel lacinia non, venenatis quis tortor. Fusce dictum dolor ac neque blandit eu mattis ligula tempus. Nunc commodo mi non sem porttitor non mattis elit aliquet. Mauris lacinia velit non turpis eleifend id imperdiet dolor porta.<newline><newline>Nam leo ipsum, pharetra a blandit a, ultrices id tellus. Duis dignissim dui tortor. Suspendisse molestie tempus felis non aliquam. Donec iaculis libero in lectus cursus vitae laoreet magna sollicitudin. Quisque vitae ligula tortor, sit amet vestibulum lectus. In enim sem, consectetur a laoreet a, convallis ac justo. Aliquam vel sem consectetur enim elementum sodales nec vulputate lorem. Mauris magna quam, condimentum vitae scelerisque ut, viverra a tortor. Aliquam iaculis pellentesque sem id molestie. "
                W = W.Replace("<newline>", vbNewLine)
                Dim Multi As Integer = Global.System.Math.Floor(LengthMax / W.Length)
                Dim Res As String = ""
                For i As Integer = 0 To Multi
                    Res &= W
                Next
                Dim Length As Integer = (Rnd() * (LengthMax - LengthMin)) + LengthMin
                Dim StartIndex As Integer = (Rnd() * (Res.Length - Length))
                If StartIndex > 0 Then
                    Res = Res.Substring(StartIndex, Length)
                Else
                    Res = Res.Substring(0, Length)
                End If
                If Res.Contains(".") Then Res = Res.Substring(Res.LastIndexOf(".") + 1)
                Res = Res.Trim

                Return Res
            End Function

            Public Function TrimStart(ByVal Source As String, ByVal TextToTrim As String, ByVal IgnoreCase As Boolean) As String
                If IgnoreCase = True Then
                    If Source.ToLower.StartsWith(TextToTrim.ToLower) Then
                        Return Source.Substring(TextToTrim.Length)
                    Else
                        Return Source
                    End If
                Else
                    If Source.StartsWith(TextToTrim) Then
                        Return Source.Substring(TextToTrim.Length)
                    Else
                        Return Source
                    End If
                End If
            End Function

            Public Function TrimEnd(ByVal Source As String, ByVal TextToTrim As String, ByVal IgnoreCase As Boolean) As String
                If IgnoreCase = True Then
                    If Source.ToLower.EndsWith(TextToTrim.ToLower) Then
                        Return Source.Substring(0, Source.Length - TextToTrim.Length)
                    Else
                        Return Source
                    End If
                Else
                    If Source.EndsWith(TextToTrim) Then
                        Return Source.Substring(0, Source.Length - TextToTrim.Length)
                    Else
                        Return Source
                    End If
                End If
            End Function


            ''' <summary>
            ''' Returns a list of email addresses which are in the source string
            ''' </summary>
            ''' <param name="Src">any string with email addresses</param>
            ''' <returns>a list of email addresses</returns>
            ''' <remarks></remarks>
            Public Function ExtractEmails(ByVal Src As String) As String()
                '(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])
                Dim PatternEmail As String = "(?:[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[A-Za-z0-9-]*[A-Za-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])"


                Dim L As New List(Of String)
                'Dim pattern As String = "^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"
                Dim emailAddressMatch As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(Src, PatternEmail)
                For Each M As System.Text.RegularExpressions.Match In emailAddressMatch.Captures
                    L.Add(M.Value)

                Next

                Return L.ToArray
            End Function


            ''' <summary>
            ''' Returns a list of url's which are in the source string
            ''' </summary>
            ''' <param name="Src">any string with url inside</param>
            ''' <returns>a list of url hyperlinks</returns>
            Public Function ExtractUrls(ByVal Src As String) As String()
                Dim L As New List(Of String)
                Dim linkParser As New Regex("\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled Or RegexOptions.IgnoreCase)
                ' Dim rawString As String = "house home go www.monstermmorpg.com nice hospital http://www.monstermmorpg.com this is incorrect url http://www.monstermmorpg.commerged continue"
                For Each m As Match In linkParser.Matches(Src)
                    L.Add(m.Value)
                Next
                Return L.ToArray
            End Function


            ''' <summary>
            ''' Check if the string is an email address
            ''' </summary>
            ''' <param name="Src">an email address</param>
            ''' <returns>true if the string is an email address</returns>
            ''' <remarks></remarks>
            Public Function IsEmail(ByVal Src As String) As Boolean
                Dim W As String = Src
                Dim L As New List(Of String)
                L.AddRange(ExtractEmails(Src))
                If L.Count = 0 Then Return False
                If L(0) <> W Then Return False
                Return True
            End Function



            ''' <summary>
            ''' Returns the Object as explicit declared type by reading the value from a string.
            ''' </summary>
            ''' <param name="T"></param>
            Public Function ConvertFromString(ByVal T As Type, ByVal Value As String) As Object
                Return System.ComponentModel.TypeDescriptor.GetConverter(T).ConvertFromString(Value)
            End Function


            ''' <summary>
            ''' Reverses the string. example Hello goes to olleH
            ''' </summary>
            ''' <param name="Src"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function Reverse(ByVal Src As String) As String
                Dim ret As String = ""
                If String.IsNullOrEmpty(Src) = False Then
                    Dim Arr() As Char = Src.ToCharArray
                    Array.Reverse(Arr)
                    ret = New String(Arr)
                End If
                Return ret
            End Function

            ''' <summary>
            ''' Returns the String when splitting it by any character or string declared in SplitCharacter
            ''' </summary>
            ''' <param name="SplitCharacter"></param>
            ''' <param name="BaseString"></param>
            ''' <param name="Index"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function SplitString(ByVal SplitCharacter As String, ByVal BaseString As String, ByVal Index As Integer) As String
                If BaseString.Split(SplitCharacter.ToCharArray).Length - 1 < Index Then Return ""
                Return BaseString.Split(SplitCharacter.ToCharArray)(Index)
            End Function

            ''' <summary>
            ''' Returns the String when splitting it by "semikolon" (";")
            ''' </summary>
            ''' <param name="BaseString"></param>
            ''' <param name="Index"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function SplitString(ByVal BaseString As String, ByVal Index As Integer) As String
                If BaseString.Split(";".ToCharArray).Length - 1 < Index Then Return ""
                Return BaseString.Split(";".ToCharArray)(Index)
            End Function

            ''' <summary>
            ''' Returns the Index of a searched string. Can ignore the results while the ignore count value is lower then the founded result count.
            ''' </summary>
            ''' <param name="MainString"></param>
            ''' <param name="SearchForString"></param>
            ''' <param name="IgnoreFoundCount"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function IndexOf(ByVal MainString As String, ByVal SearchForString As String, ByVal IgnoreFoundCount As Integer) As Integer
                Dim StartIndex As Integer
                Dim Result As Integer = -1
                Dim IgnoreFoundIndex As Integer = 0

                Do
                    Result = MainString.IndexOf(SearchForString, StartIndex)
                    If Result < 0 Then Exit Do
                    IgnoreFoundIndex += 1
                    If IgnoreFoundIndex > IgnoreFoundCount Then Exit Do
                    StartIndex = Result + 1
                Loop
                Return Result
            End Function


            ''' <summary>
            ''' Returns TRUE if the result is any alpha string. 
            ''' Ex. "198" = FALSE, "ABC" = TRUE
            ''' </summary>
            ''' <param name="InString"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function IsAlpha(ByVal InString As String) As Boolean
                If InString Is Nothing Then Return False
                If Not System.Text.RegularExpressions.Regex.IsMatch(InString, "^[a-zA-Z]") Then
                    Return False
                Else
                    Return True
                End If
            End Function

            ''' <summary>
            ''' Returns TRUE if the result is any numeric string. 
            ''' Ex. "198" = TRUE, "ABC" = FALSE
            ''' </summary>
            ''' <param name="InString"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function IsNumeric(ByVal InString As String) As Boolean
                If InString Is Nothing Then Return False
                If Not System.Text.RegularExpressions.Regex.IsMatch(InString, "\d+") Then
                    Return False
                Else
                    Return True
                End If
            End Function



            ''' <summary>
            ''' Extract a String from a Base string by using the start-pattern string and end-pattern string.
            ''' </summary>
            ''' <param name="BaseString"></param>
            ''' <param name="StartIndex"></param>
            ''' <param name="StartString"></param>
            ''' <param name="EndString"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function ExtractStringFromString(ByVal BaseString As String, ByRef StartIndex As Integer, ByVal StartString As String, ByVal EndString As String) As String
                Return ExtractStringFromString(BaseString, StartIndex, StartString, EndString, False)
            End Function

            ''' <summary>
            ''' Extract a String from a Base string by using the start-pattern string and end-pattern string.
            ''' </summary>
            ''' <param name="BaseString"></param>
            ''' <param name="StartIndex"></param>
            ''' <param name="StartString"></param>
            ''' <param name="EndString"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function ExtractStringFromString(ByVal BaseString As String, ByRef StartIndex As Integer, ByVal StartString As String, ByVal EndString As String, ByVal IgnoreCaseInKeyWords As Boolean) As String
                Dim W As String = ""
                If BaseString Is Nothing Then BaseString = String.Empty
                Dim i, j As Integer

                If StartIndex < 0 Then Return W
                If StartIndex > BaseString.Length Then Return W
                If IgnoreCaseInKeyWords = True Then
                    i = BaseString.ToLower.IndexOf(StartString.ToLower, StartIndex)
                Else
                    i = BaseString.IndexOf(StartString, StartIndex)
                End If
                If i < StartIndex Then StartIndex = i : Return W
                'If i = StartIndex Then StartIndex = i : Return W
                j = i + StartString.Length + 0 ' +1 'Change since I saw, if no char is available between StartString and EndString Keyword, we need +0 instead of +1
                If j > BaseString.Length - 1 Then j = BaseString.Length - 1

                If IgnoreCaseInKeyWords = True Then
                    j = BaseString.ToLower.IndexOf(EndString.ToLower, j)
                Else
                    j = BaseString.IndexOf(EndString, j)
                End If

                If j < i Then
                    'Neu: StartIndex wird auf das Ende gesetzt, wenn kein EndTag vorhanden ist
                    j = BaseString.Length
                    If j <= i Then Return W
                End If
                i = i + StartString.Length
                'If j < i Then Return W

                If j < i Then
                    StartIndex = -1 'BaseString.Length
                    Return W
                End If
                W = BaseString.Substring(i, j - i)
                StartIndex = i

                Return W

NothingToDo:
                StartIndex = -1
                Return W
            End Function


            ''' <summary>
            ''' Returns the BaseString as many times as the counter declares.
            ''' </summary>
            ''' <param name="Count"></param>
            ''' <param name="BaseString"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function StringDub(ByVal Count As Integer, ByVal BaseString As String) As String
                Dim Res As String = ""
                For i As Integer = 1 To Count
                    Res = Res & BaseString
                Next
                Return Res
            End Function



            ''' <summary>
            ''' Returns the string first first Charater as uper case character. Other characters are not touched.
            ''' </summary>
            ''' <param name="Word"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function FirstLetterUpperCase(ByVal Word As String) As String
                If Word Is Nothing Then Return ""
                Return String.Format("{0}{1} ", Word(0).ToString().ToUpper(), Word.Substring(1))
            End Function


            ''' <summary>
            ''' Convert any readable word into a Base64 String
            ''' Ex: "Hello world it's 4 you." 
            ''' goes to "R0lGODlhZAAyALMAAIiIiLu7u2ZmZiIiIszMzO7u7hERETMzM0RERN3d3aqqqlVVVZmZmXd3dwAAAP////yH5BAAAAAAALAAAAABkADIAAATh8MlJq7046827"
            ''' </summary>
            ''' <param name="ReadableWord"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function ToBase64String(ByVal ReadableWord As String) As String
                Return Global.System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(ReadableWord))
            End Function

            ''' <summary>
            ''' Convert any original Bytearray into a Base64 String using the defined encoder
            ''' Ex: "Hello world it's 4 you." 
            ''' goes to "R0lGODlhZAAyALMAAIiIiLu7u2ZmZiIiIszMzO7u7hERETMzM0RERN3d3aqqqlVVVZmZmXd3dwAAAP////yH5BAAAAAAALAAAAABkADIAAATh8MlJq7046827"
            ''' </summary>
            Public Function ToBase64String(ByVal ReadableWord As String, ByVal enc As System.Text.Encoding) As String
                Return Global.System.Convert.ToBase64String(enc.GetBytes(ReadableWord))
            End Function

            ''' <summary>
            ''' Convert any Base64 String back into the readable word
            ''' Ex: "R0lGODlhZAAyALMAAIiIiLu7u2ZmZiIiIszMzO7u7hERETMzM0RERN3d3aqqqlVVVZmZmXd3dwAAAP////yH5BAAAAAAALAAAAABkADIAAATh8MlJq7046827"
            ''' goes to: "Hello world it's 4 you."
            ''' </summary>
            Public Function FromBase64String(ByVal Base64Word As String) As String
                Dim bytes = Global.System.Convert.FromBase64String(Base64Word)
                Return System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length) ', 0, Base64Word.Length hinzugef¸gt aus Kompatibilit‰tsgr¸nden zur modTB f¸r Mobile
            End Function

            ''' <summary>
            ''' Convert any Base64 String back into the original ByteArray by using the defined encoder
            ''' </summary>
            Public Function FromBase64String(ByVal Base64Word As String, ByVal enc As System.Text.Encoding) As String
                Return enc.GetString(Global.System.Convert.FromBase64String(Base64Word), 0, Base64Word.Length)
            End Function

            ''' <summary>
            ''' decode a string by using TB-methods (ex. W832nlmk goes to HALLO)
            ''' Optionflag = 0 --> TB + MD5 Encoder, 
            ''' Optionflag = 1 --> TB, 
            ''' Optionflag = 2 --> MD5 Encoder, 
            ''' Optionflag = 3 --> TB-Alphanumeric, 
            ''' </summary>
            Public Function TB_Decode(ByVal Wort As String, Optional ByVal OptionFlag As Integer = 0) As String
                'Optionflag = 0 --> TB + MD5 Encoder
                'Optionflag = 1 --> TB 
                'Optionflag = 2 --> MD5 Encoder
                'Optionflag = 3 --> TB-Alphanumeric

                Dim i As Integer
                Dim a As Integer
                Dim W As Char
                Dim Result As String = ""

                If String.IsNullOrEmpty(Wort) = False Then

                    If OptionFlag = 0 Or OptionFlag = 2 Then
                        Wort = DecodeFromMD5(Wort)
                        Result = Wort
                    End If


                    If OptionFlag = 0 Or OptionFlag = 1 Then
                        Result = ""
                        For i = 1 To Wort.Length
                            W = Global.Microsoft.VisualBasic.Strings.Mid(Wort, i, 1).ToCharArray()(0)
                            a = Asc(W)
                            a = a - 15
                            If a < 1 Then a = a + 255
                            W = Chr(a)
                            Result = Result & W
                        Next
                    End If


                    If OptionFlag = 3 Then
                        Result = Result.Replace(Chr(13), "[13]")
                        Result = Result.Replace(Chr(9), "[9]")
                        Result = Result.Replace(Chr(10), "[10]")
                        Result = Result.Replace(Chr(27), "[27]")
                        Result = Result.Replace(Chr(0), "[0]")
                    End If
                End If

                Return Result
            End Function


            ''' <summary>
            ''' encode a string by using MD5 methode
            ''' </summary>
            Private Function EncodeToMD5(ByVal Wort As String) As String
                Dim Result As String
                Dim Password As String = "jack.01"


                Dim rd As New System.Security.Cryptography.RijndaelManaged

                Dim md5 As New System.Security.Cryptography.MD5CryptoServiceProvider
                Dim key() As Byte = md5.ComputeHash(Global.System.Text.Encoding.UTF8.GetBytes(Password))

                md5.Clear()
                rd.Key = key
                rd.GenerateIV()

                Dim iv() As Byte = rd.IV
                Dim ms As New IO.MemoryStream

                ms.Write(iv, 0, iv.Length)

                Dim cs As New System.Security.Cryptography.CryptoStream(ms, rd.CreateEncryptor, System.Security.Cryptography.CryptoStreamMode.Write)
                Dim data() As Byte = Global.System.Text.Encoding.UTF8.GetBytes(Wort)

                cs.Write(data, 0, data.Length)
                cs.FlushFinalBlock()

                Dim encdata() As Byte = ms.ToArray()
                Result = Global.System.Convert.ToBase64String(encdata)
                cs.Close()
                rd.Clear()

                Return Result
            End Function

            ''' <summary>
            ''' decode a string by using MD5 methode
            ''' </summary>
            Private Function DecodeFromMD5(ByVal Wort As String) As String
                Dim Result As String = ""
                Dim Password As String = "jack.01"

                Dim rd As New System.Security.Cryptography.RijndaelManaged
                Dim rijndaelIvLength As Integer = 16
                Dim md5 As New System.Security.Cryptography.MD5CryptoServiceProvider
                Dim key() As Byte = md5.ComputeHash(Global.System.Text.Encoding.UTF8.GetBytes(Password))

                md5.Clear()

                Try
                    Dim encdata() As Byte = Global.System.Convert.FromBase64String(Wort)
                    Dim ms As New IO.MemoryStream(encdata)
                    Dim iv(15) As Byte

                    ms.Read(iv, 0, rijndaelIvLength)
                    rd.IV = iv
                    rd.Key = key

                    Dim cs As New System.Security.Cryptography.CryptoStream(ms, rd.CreateDecryptor, System.Security.Cryptography.CryptoStreamMode.Read)

                    Dim data(CInt(ms.Length - rijndaelIvLength)) As Byte
                    Dim i As Integer = cs.Read(data, 0, data.Length)

                    Result = Global.System.Text.Encoding.UTF8.GetString(data, 0, i)
                    cs.Close()
                    rd.Clear()

                Catch ex As Exception
                    System.Diagnostics.Debug.WriteLine("MD5 Exception Occured:" & ex.Message)
                    'Throw New Exception("MD5 Exception Occured:" & ex.Message, ex)
                End Try

                Return Result

            End Function


            Public Enum TrimToCharsEnum As Integer
                FileNameSimple = 0
                FileNameExtendedChars = 1
                Alpha = 2
                Numeric = 3
            End Enum

            ''' <summary>
            ''' trim a string by checking the string from a list of good charaters
            ''' used characters are: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.-_ "
            ''' </summary>
            Public Function TrimToChars(ByVal OriginalText As String) As String
                Dim GoodChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.-_ " 'change to allow " " for files

                Return TrimToChars(OriginalText, GoodChars.ToCharArray)
            End Function

            Public Function TrimToChars(ByVal OriginalText As String, ByVal Style As TrimToCharsEnum) As String
                Dim GoodChars As String = ""

                Select Case Style
                    Case TrimToCharsEnum.FileNameSimple
                        GoodChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_ "
                    Case TrimToCharsEnum.FileNameExtendedChars
                        GoodChars = " !#$%&'()+,-./0123456789;=@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{}~ÄÅÇÉÑÖÜáàâäãåçéèêëí""ïñóòôöõúùûü†°¢£§•¶ß®©™´¨≠ÆØ∞±≤≥¥µ∂∑∏π∫ªºΩæø¿¡¬√ƒ≈∆«»… ÀÃÕŒœ–—“”‘’÷◊ÿŸ⁄€‹›ﬁﬂ‡·‚„‰ÂÊÁËÈÍÎÏÌÓÔÒÚÛÙıˆ˜¯˘˙˚¸˝˛ˇ"
                    Case TrimToCharsEnum.Alpha
                        GoodChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_ ¿¡¬√ƒ≈∆«»… ÀÃÕŒœ–—“”‘’÷◊ÿŸ⁄€‹›ﬁﬂ‡·‚„‰ÂÊÁËÈÍÎÏÌÓÔÒÚÛÙıˆ˜¯˘˙˚¸"
                    Case TrimToCharsEnum.Numeric
                        GoodChars = "0123456789-,.+"
                End Select


                Return TrimToChars(OriginalText, GoodChars.ToCharArray)
            End Function

            ''' <summary>
            ''' trim a string by checking the string from a list of good charaters
            ''' </summary>
            Public Function TrimToChars(ByVal OriginalText As String, ByVal GoodChars As String) As String
                Return TrimToChars(OriginalText, GoodChars.ToCharArray)
            End Function


            ''' <summary>
            ''' trim a string by checking the string from a list of good charaters
            ''' </summary>
            Public Function TrimToChars(ByVal OriginalText As String, ByVal GoodChars() As Char) As String
                Dim i As Integer
                Dim Res As String = ""

                If String.IsNullOrEmpty(OriginalText) = False Then
                    For i = 1 To OriginalText.Length
                        If InStr(GoodChars, Global.Microsoft.VisualBasic.Strings.Mid(OriginalText, i, 1)) > 0 Then Res = Res & Global.Microsoft.VisualBasic.Strings.Mid(OriginalText, i, 1)
                    Next
                End If

                Return Res
            End Function


            Public Function ToHex(ByVal data() As Byte, ByVal Separator As String) As String
                Dim res As String = ""
                If Separator Is Nothing Then Separator = ""
                Dim W As String

                If data IsNot Nothing Then
                    If Separator.Length > 0 Then
                        res = BitConverter.ToString(data).Replace("-", Separator)
                    Else

                        For i As Integer = 0 To data.Length - 1
                            W = data(i).ToString("X")
                            'If Separator.Length = 0 Then 'trim to 3 characters like "000"
                            Do
                                If W.Length >= 2 Then Exit Do
                                W = "0" & W
                            Loop
                            'End If
                            res &= W & Separator
                        Next
                        'Next
                    End If
                End If

                res = res.TrimEnd(Separator.ToCharArray)
                Return res
            End Function

            'Public Function ByteToHexBitFiddle(ByVal bytes As Byte()) As String
            '    Dim c As Char() = New Char(bytes.Length * 2 - 1) {}
            '    Dim b As Integer
            '    For i As Integer = 0 To bytes.Length - 1
            '        b = bytes(i) >> 4
            '        c(i * 2) = ChrW(55 + b + (((b - 10) >> 31) And -7))
            '        b = bytes(i) And &HF
            '        c(i * 2 + 1) = ChrW(55 + b + (((b - 10) >> 31) And -7))
            '    Next
            '    Return New String(c)
            'End Function





            Public Function FromHex(ByVal value As String, ByVal Separator As String) As Byte()
                Dim res As New List(Of Byte)

                If Separator Is Nothing OrElse Separator.Length = 0 Then
                    'just count all values by 3
                    For i As Integer = 0 To value.Length - 1 Step 2
                        Dim B As Byte
                        Try
                            B = System.Convert.ToByte(value.Substring(i, 2), 16)
                            res.Add(B)
                        Catch ex As Exception

                        End Try
                        'If Byte.TryParse("&H" & value.Substring(i, 3), B) = True Then
                        '    res.Add(B)
                        'End If
                    Next

                Else
                    'use separator to parse
                    For Each W As String In value.Split(Separator.ToCharArray)
                        Dim B As Byte
                        Try
                            If String.IsNullOrEmpty(W) = False Then
                                B = System.Convert.ToByte(W, 16)
                                res.Add(B)
                            End If
                        Catch ex As Exception

                        End Try
                        'bad mistake. found January 10th 2013. convertion to Byte does not take
                        'the Hex values. Old version:
                        'If Byte.TryParse(W, B) = True Then
                        '    B = Convert.ToByte(B, 16)
                        '    res.Add(B)
                        'End If
                    Next
                End If




                Return res.ToArray
            End Function


            ''' <summary>
            ''' Returns an MD5 hash value. This is non-reversable
            ''' </summary>
            ''' <param name="value">any string</param>
            ''' <returns>a value which has always a length of 32 chars</returns>
            Public Function MD5Hash(ByVal value As String) As String
                If String.IsNullOrEmpty(value) = True Then Return String.Empty
                Dim B() As Byte = System.Text.Encoding.Unicode.GetBytes(value)
                Return MD5Hash(B)
            End Function


            ''' <summary>
            ''' Returns an MD5 hash value. This is non-reversable
            ''' </summary>
            ''' <param name="value">any string</param>
            ''' <returns>a value which has always a length of 32 chars</returns>
            Public Function MD5Hash(ByVal value() As Byte) As String
                If value Is Nothing Then Return String.Empty
                Dim Md5 As New System.Security.Cryptography.MD5CryptoServiceProvider
                Dim result As String = System.BitConverter.ToString(Md5.ComputeHash(value))
                result = result.Replace("-", "") 'remove "-" from hex data
                Return result
            End Function



            ''' <summary>
            ''' encode a string by using TB-methods (ex. HALLO goes to W832nlmk)
            ''' Optionflag = 0 --> TB + MD5 Encoder, 
            ''' Optionflag = 1 --> TB, 
            ''' Optionflag = 2 --> MD5 Encoder, 
            ''' Optionflag = 3 --> TB-Alphanumeric
            ''' </summary>
            Public Function TB_Encode(ByVal Wort As String, Optional ByVal OptionFlag As Integer = 0) As String
                'Optionflag = 0 --> TB + MD5 Encoder
                'Optionflag = 1 --> TB 
                'Optionflag = 2 --> MD5 Encoder
                'Optionflag = 3 --> TB-Alphanumeric

                Dim i As Integer
                Dim a As Integer
                Dim W As Char
                Dim Result As String = ""


                If OptionFlag = 3 Then
                    Result = Wort
                    Result = Result.Replace(Chr(13), "[13]")
                    Result = Result.Replace(Chr(9), "[9]")
                    Result = Result.Replace(Chr(10), "[10]")
                    Result = Result.Replace(Chr(27), "[27]")
                    Result = Result.Replace(Chr(0), "[0]")
                    Wort = Result
                End If


                If OptionFlag = 0 Or OptionFlag = 1 Then
                    Result = ""
                    For i = 1 To Wort.Length
                        W = Global.Microsoft.VisualBasic.Strings.Mid(Wort, i, 1).ToCharArray()(0)
                        a = Asc(W)
                        a = a + 15
                        If a > 255 Then a = a - 255
                        W = Chr(a)
                        Result = Result & W
                    Next
                    Wort = Result
                End If


                If OptionFlag = 0 Or OptionFlag = 2 Then
                    If OptionFlag = 2 Then 'fault since long time, missed Result=Wort, 26.1.2012, ti
                        Result = Wort
                    End If
                    Result = EncodeToMD5(Result)
                End If

                Return Result
            End Function
        End Module
    End Namespace

    Public NotInheritable Class Wow
        Private Sub New()
        End Sub
        Public Shared ReadOnly Property Is64BitProcess() As Boolean
            Get
                Return IntPtr.Size = 8
            End Get
        End Property

        Public Shared ReadOnly Property Is64BitOperatingSystem() As Boolean
            Get
                ' Clearly if this is a 64-bit process we must be on a 64-bit OS.
                If Is64BitProcess Then
                    Return True
                End If
                ' Ok, so we are a 32-bit process, but is the OS 64-bit?
                ' If we are running under Wow64 than the OS is 64-bit.
                Dim isWow64 As Boolean
                Return ModuleContainsFunction("kernel32.dll", "IsWow64Process") AndAlso IsWow64Process(GetCurrentProcess(), isWow64) AndAlso isWow64
            End Get
        End Property

        Private Shared Function ModuleContainsFunction(moduleName As String, methodName As String) As Boolean
            Dim hModule As IntPtr = GetModuleHandle(moduleName)
            If hModule <> IntPtr.Zero Then
                Return GetProcAddress(hModule, methodName) <> IntPtr.Zero
            End If
            Return False
        End Function

        <Runtime.InteropServices.DllImport("kernel32.dll", SetLastError:=True)> _
        Private Shared Function IsWow64Process(hProcess As IntPtr, <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> ByRef isWow64 As Boolean) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
        End Function
        <Runtime.InteropServices.DllImport("kernel32.dll", CharSet:=Runtime.InteropServices.CharSet.Auto, SetLastError:=True)> _
        Private Shared Function GetCurrentProcess() As IntPtr
        End Function
        <Runtime.InteropServices.DllImport("kernel32.dll", CharSet:=Runtime.InteropServices.CharSet.Auto)> _
        Private Shared Function GetModuleHandle(moduleName As String) As IntPtr
        End Function
        <Runtime.InteropServices.DllImport("kernel32.dll", CharSet:=Runtime.InteropServices.CharSet.Ansi, SetLastError:=True)> _
        Private Shared Function GetProcAddress(hModule As IntPtr, methodName As String) As IntPtr
        End Function


    End Class


    Namespace SystemMain
        Public Module modSystem

            <Runtime.InteropServices.DllImport("user32.dll", SetLastError:=True, CharSet:=Runtime.InteropServices.CharSet.Auto)> _
            Private Function FindWindow( _
                 ByVal lpClassName As String, _
                 ByVal lpWindowName As String) As IntPtr
            End Function

            <Runtime.InteropServices.DllImport("user32.dll")> _
            Private Function SetForegroundWindow(ByVal hWnd As IntPtr) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
            End Function



            Public Function BringWindowOnTop(ByVal MainWindowTitle As String) As Boolean
                Dim ret As Boolean = False
                Try
                    Dim Ptr As IntPtr = FindWindow(Nothing, MainWindowTitle)
                    If Ptr <> IntPtr.Zero Then ret = BringWindowOnTop(Ptr)
                Catch ex As Exception

                End Try

                Return ret
            End Function


            Public Function BringWindowOnTop(ByVal MainWindowHandle As IntPtr) As Boolean
                Dim ret As Boolean = False
                Try
                    ret = SetForegroundWindow(MainWindowHandle)
                Catch ex As Exception

                End Try
                Return ret
            End Function



             

            Public Function ExecutingApplicationPath() As String
                Return ExecutingApplicationPath(True)
            End Function

            Public Function ExecutingApplicationPath(ByVal UseBaseStartUpAssembly As Boolean) As String
                Dim W As String = ""
                Try
                    Dim Asm As System.Reflection.Assembly
                    If UseBaseStartUpAssembly Then
                        Asm = System.Reflection.Assembly.GetEntryAssembly
                    Else
                        Asm = System.Reflection.Assembly.GetCallingAssembly
                    End If
                    Dim U As New Uri(Asm.CodeBase)
                    W = U.LocalPath
                Catch ex As Exception
                    ex = ex
                End Try

                Return W
            End Function

            ''' <summary>
            ''' Check if the processor uses 32 or 64Bit architecture.
            ''' </summary>
            ''' <returns>The default return FALSE is for 32Bit x86 CPU system. If TRUE, the system is 64Bit.</returns>
            ''' <remarks>If operating system only supports 32bit but 64bit is really included, this function returns 32Bit as answer</remarks>
            Public Function Is64BitHardware() As Boolean
                Return Wow.Is64BitOperatingSystem
            End Function



            ''' <summary>
            ''' Check if the application uses 32 or 64Bit architecture.
            ''' </summary>
            ''' <returns>The default return FALSE is for 32Bit x86 CPU system. If TRUE, the system is 64Bit.</returns>
            ''' <remarks></remarks>
            Public Function Is64BitSoftware() As Boolean
                Return Wow.Is64BitProcess
                'Dim Ret As Boolean = False
                'Dim s As Integer = System.Runtime.InteropServices.Marshal.SizeOf(New IntPtr(0))
                'If s = 4 Then
                '    '32Bit
                '    Ret = False
                'ElseIf s = 8 Then
                '    '64Bit
                '    Ret = True
                'Else
                '    'something new
                '    Ret = False
                'End If
                'Return Ret
            End Function


            ''' <summary>
            ''' Returns TRUE if Visual Studio is running in design mode. Mostly used for User controls to not calculate the usercontrol while designing.
            ''' </summary>
            ''' <returns>True if design mode is active</returns>
            ''' <param name="Control">A control like a form to check if it is in design mode.</param>
            ''' <remarks>Use "Me" as control</remarks>
            Public Function IsInDesignMode(ByVal Control As System.ComponentModel.Component) As Boolean
                'WPF:             If System.ComponentModel.DesignerProperties.GetIsInDesignMode(Me) = True Then Exit For
                'Return Me.DesignMode '<-- use this
                If Control IsNot Nothing AndAlso Control.Site IsNot Nothing Then Return Control.Site.DesignMode
                Return False 'dummy
            End Function

            
 
            



            ''' <summary>
            ''' Read some informations about the current installed and running language settings. 
            ''' Ex: Name = es-ES, Native name = espaÒol (EspaÒa), LCID = 3082, ThreeLetterWindowsLanguageName = ESN
            ''' </summary>
            ''' <returns>dictionary with keywords: CompareInfo,DisplayName,EnglishName,
            ''' IsNeutralCulture,IsReadOnly,LCID,Name,NativeName,Parent,
            ''' TextInfo,ThreeLetterISOLanguageName,ThreeLetterWindowsLanguageName,TwoLetterISOLanguageName</returns>
            ''' <remarks></remarks>
            Public Function ReadCultureInfo() As Dictionary(Of String, Object)
                'This code produces the following output.
                '
                'PROPERTY                         INTERNATIONAL            TRADITIONAL
                'CompareInfo                      CompareInfo - 3082       CompareInfo - 1034
                'DisplayName                      Spanish (Spain)          Spanish (Spain)
                'EnglishName                      Spanish (Spain)          Spanish (Spain)
                'IsNeutralCulture                 False                    False
                'IsReadOnly                       False                    False
                'LCID                             3082                     1034
                'Name                             es-ES                    es-ES
                'NativeName                       espaÒol (EspaÒa)         espaÒol (EspaÒa)
                'Parent                           es                       es
                'TextInfo                         TextInfo - 3082          TextInfo - 1034
                'ThreeLetterISOLanguageName       spa                      spa
                'ThreeLetterWindowsLanguageName   ESN                      ESN
                'TwoLetterISOLanguageName         es                       es




                'Dim NFI As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
                'Return NFI.NumberDecimalSeparator

                ' Creates and initializes the CultureInfo which uses the international sort.
                'Dim myCIintl As New System.Globalization.CultureInfo("es-ES", False)
                Dim myCIintl As Globalization.CultureInfo = Globalization.CultureInfo.CurrentCulture

                '' Creates and initializes the CultureInfo which uses the traditional sort.
                'Dim myCItrad As New Globalization.CultureInfo(&H40A, False)

                Dim di As New Dictionary(Of String, Object)

                '' Displays the properties of each culture.
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "PROPERTY", "INTERNATIONAL", "TRADITIONAL")
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "CompareInfo", myCIintl.CompareInfo, myCItrad.CompareInfo)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "DisplayName", myCIintl.DisplayName, myCItrad.DisplayName)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "EnglishName", myCIintl.EnglishName, myCItrad.EnglishName)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "IsNeutralCulture", myCIintl.IsNeutralCulture, myCItrad.IsNeutralCulture)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "IsReadOnly", myCIintl.IsReadOnly, myCItrad.IsReadOnly)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "LCID", myCIintl.LCID, myCItrad.LCID)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "Name", myCIintl.Name, myCItrad.Name)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "NativeName", myCIintl.NativeName, myCItrad.NativeName)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "Parent", myCIintl.Parent, myCItrad.Parent)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "TextInfo", myCIintl.TextInfo, myCItrad.TextInfo)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "ThreeLetterISOLanguageName", myCIintl.ThreeLetterISOLanguageName, myCItrad.ThreeLetterISOLanguageName)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "ThreeLetterWindowsLanguageName", myCIintl.ThreeLetterWindowsLanguageName, myCItrad.ThreeLetterWindowsLanguageName)
                'Console.WriteLine("{0,-33}{1,-25}{2,-25}", "TwoLetterISOLanguageName", myCIintl.TwoLetterISOLanguageName, myCItrad.TwoLetterISOLanguageName)
                'Console.WriteLine()


                di.Add("CompareInfo", myCIintl.CompareInfo)
                di.Add("DisplayName", myCIintl.DisplayName)
                di.Add("EnglishName", myCIintl.EnglishName)
                di.Add("IsNeutralCulture", myCIintl.IsNeutralCulture)
                di.Add("IsReadOnly", myCIintl.IsReadOnly)
                di.Add("LCID", myCIintl.LCID)
                di.Add("Name", myCIintl.Name)
                di.Add("NativeName", myCIintl.NativeName)
                di.Add("Parent", myCIintl.Parent)
                di.Add("TextInfo", myCIintl.TextInfo)
                di.Add("ThreeLetterISOLanguageName", myCIintl.ThreeLetterISOLanguageName)
                di.Add("ThreeLetterWindowsLanguageName", myCIintl.ThreeLetterWindowsLanguageName)
                di.Add("TwoLetterISOLanguageName", myCIintl.TwoLetterISOLanguageName)

                Return di
            End Function








            ''' <summary>
            ''' MAIN sub!
            ''' Returns the dictionary of the applications commandline arguments
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            '''   Example:
            ''' 
            '''   Private Sub LoadCommandLineArgs()
            '''    On Error Resume Next
            '''        Dim Value As String = ""
            '''        Dim di As New Dictionary(Of String, String)
            '''    di = TB.SystemMain.GetCommandLineArgs()
            ''' 
            '''    If di.TryGetValue("child", Value) = True AndAlso FlightTimer.Helper.myVal(Value) = 1 Then Call KillInstances()
            '''    If di.TryGetValue("topmost", Value) = True AndAlso FlightTimer.Helper.myVal(Value) = 1 Then Call AlwaysOnTopToolStripMenuItem_Click(Nothing, Nothing)
            '''    If di.TryGetValue("wndleft", Value) = True Then Me.Left = FlightTimer.Helper.myVal(Value)
            '''    If di.TryGetValue("wndtop", Value) = True Then Me.Top = FlightTimer.Helper.myVal(Value)
            '''   End Sub
            ''' </remarks>
            Public Function GetCommandLineArgs() As Dictionary(Of String, String)
                Return GetCommandLineArgs(System.Environment.GetCommandLineArgs)
            End Function

            Public Function GetCommandLineArgs(ByVal SetKeyToLowerCase As Boolean) As Dictionary(Of String, String)
                Return GetCommandLineArgs(SetKeyToLowerCase, False)
            End Function


            Public Function GetCommandLineArgs(ByVal SetKeyToLowerCase As Boolean, ByVal RemoveStarterCharsInKey As Boolean) As Dictionary(Of String, String)
                'turn GetCommandLineArgs(System.Environment.GetCommandLineArgs)
                '
                Dim diBase As Dictionary(Of String, String) = GetCommandLineArgs(System.Environment.GetCommandLineArgs)
                Dim di As New Dictionary(Of String, String)
                For Each W As String In diBase.Keys
                    Dim Key As String = W
                    If RemoveStarterCharsInKey = True Then
                        Key = Key.TrimStart("-")
                        Key = Key.TrimStart("/")
                    End If
                    If SetKeyToLowerCase Then Key = Key.ToLower
                    di.Add(Key, diBase(W))
                Next
                Return di
                '
            End Function

            ''' <summary>
            ''' Returns the dictionary of the commandline arguments
            ''' </summary>
            ''' <param name="Data"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetCommandLineArgs(ByVal Data() As String) As Dictionary(Of String, String)
                Dim di As New Dictionary(Of String, String)

                Dim i As Integer
                Dim Key As String
                Dim Value As String

                If Data Is Nothing Then Data = System.Environment.GetCommandLineArgs 'Neu, weil die ‹berladung sonst nicht greift
                For Each W As String In Data
                    i = W.IndexOf("=")
                    If i > -1 Then
                        Key = W.Substring(0, i)
                        Value = W.Substring(i + 1)
                    Else
                        Key = W
                        Value = ""
                    End If
                    TB.Controls.AddToDic(di, Key, Value)
                Next

                Return di
            End Function






            ''' <summary>
            ''' Returns the path for "My documents"
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function DocumentPathAllUsers() As String
                Return DocumentPathAllUsers(Nothing)
            End Function

 

            Public _lastDocumentPath As String = ""
            ''' <summary>
            ''' Returns the path for "My documents"
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function DocumentPath() As String
                Dim W As String = ""
                Try
                    If HttpContext.Current Is Nothing Then
                        W = _lastDocumentPath
                    Else
                        Dim di As New IO.DirectoryInfo(HttpContext.Current.Server.MapPath("~/database"))
                        W = di.FullName
                        If W.EndsWith("\") = False Then W &= "\"
                        If _lastDocumentPath <> W Then
                            _lastDocumentPath = W
                        End If
                    End If
                Catch ex As Exception
                    W = _lastDocumentPath
                End Try

                Return W
            End Function

            Public Function AppPath(ByVal UnusedPlaceholder As String) As String
                Return AppPath()
            End Function


            Public _lastAppPath As String = ""
            Public Function AppPath() As String
                Dim W As String = ""
                Try
                    If HttpContext.Current Is Nothing Then
                        W = _lastAppPath
                    Else
                        Dim di As New IO.DirectoryInfo(HttpContext.Current.Server.MapPath("~/"))
                        W = di.FullName
                        If W.EndsWith("\") = False Then W &= "\"
                        If _lastAppPath <> W Then
                            _lastAppPath = W
                        End If
                    End If
                Catch ex As Exception
                    W = _lastAppPath
                End Try
                Return W
            End Function
 
          

        End Module
    End Namespace











    Namespace FileSystem



        'Namespace Shell32_TBE
        '    <Runtime.InteropServices.ComImport()> _
        '    <Runtime.InteropServices.Guid("00020400-0000-0000-C000-000000000046")> _
        '    <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)> _
        '    Public Interface IDispatch
        '        Function GetTypeInfoCount() As UInteger
        '        Function GetTypeInfo(ByVal iTInfo As UInteger, ByVal lcid As UInteger) As IDispatch
        '        Sub GetIDsOfNames( _
        '            ByVal riid As UInteger, _
        '            <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.LPWStr)> _
        '            ByVal rgszNames As String, _
        '            ByVal cNames As UInteger, _
        '            ByVal lcid As UInteger, _
        '            ByRef rgDispId As UInteger)

        '        Sub Invoke(ByVal dispIdMember As UInteger, ByVal riid As UInteger, ByVal lcid As UInteger, _
        '            ByVal wFlags As Short, ByVal pDispParams As UInteger, ByRef pVarResult As Object, _
        '            ByRef pExcepInfo As UInteger, ByRef puArgErr As UInteger)
        '    End Interface

        '    <Runtime.InteropServices.ComImport()> _
        '    <Runtime.InteropServices.Guid("D8F015C0-C278-11CE-A49E-444553540000")> _
        '    <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
        '    <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
        '    Public Interface IShellDispatch
        '        ReadOnly Property [Application]() As IDispatch
        '        ReadOnly Property Parent() As IDispatch 'IDispatch
        '        Function ShellNameSpace(ByVal vDir As Object) As Folder
        '        Function BrowseForFolder(ByVal hWnd As Integer, ByVal Title As String, ByVal Otions As Integer, ByVal RootFolder As Object) As Folder
        '        Function Windows() As IDispatch 'IDispatch
        '        Sub Open(ByVal vDir As Object)
        '        Sub Explore(ByVal vDir As Object)
        '        Sub MinimizeAll()
        '        Sub UndoMinimizeALL()
        '        Sub FileRun()
        '        Sub CascadeWindows()
        '        Sub TileVertically()
        '        Sub TileHorizontally()
        '        Sub ShutdownWindows()
        '        Sub Suspend()
        '        Sub EjectPC()
        '        Sub SetTime()
        '        Sub TrayProperties()
        '        Sub Help()
        '        Sub FindFiles()
        '        Sub FindComputer()
        '        Sub RefreshMenu()
        '        Sub ControlPanelItem(ByVal bstrDir As String)
        '    End Interface

        '    <Runtime.InteropServices.ComImport()> _
        '    <Runtime.InteropServices.Guid("BBCBDE60-C3FF-11CE-8350-444553540000")> _
        '    <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
        '    <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
        '    Public Interface Folder
        '        ReadOnly Property Title() As String
        '        ReadOnly Property Application() As IDispatch
        '        ReadOnly Property Parent() As IDispatch
        '        ReadOnly Property ParentFolder() As Folder ' Shell32.Folder

        '        Function Items() As Shell32_TBE.FolderItems ' Shell32.FolderItems

        '        Function ParseName( _
        '            <Runtime.InteropServices.In()> _
        '            <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.BStr)> _
        '            ByVal bName As String) As Shell32_TBE.FolderItem ' Shell32.FolderItem

        '        Sub NewFolder( _
        '            <Runtime.InteropServices.In()> _
        '            <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.BStr)> _
        '            ByVal bName As String, _
        '            <Runtime.InteropServices.In()> _
        '            Optional ByVal vOptions As Object = Nothing _
        '            )

        '        Sub MoveHere( _
        '            <Runtime.InteropServices.In()> _
        '            ByVal vItem As Object, _
        '            <Runtime.InteropServices.In()> _
        '            Optional ByVal vOptions As Object = Nothing)


        '        Sub CopyHere( _
        '            <Runtime.InteropServices.In()> _
        '            ByVal vItem As Object, _
        '            <Runtime.InteropServices.In()> _
        '            Optional ByVal vOptions As Object = Nothing)

        '        Function GetDetailsOf( _
        '            <Runtime.InteropServices.In()> _
        '            ByVal vItem As Object, _
        '            <Runtime.InteropServices.In()> _
        '            ByVal iColumn As Integer _
        '        ) As String

        '    End Interface

        '    <Runtime.InteropServices.Guid("744129E0-CBE5-11CE-8350-444553540000")> _
        '    <Runtime.InteropServices.ComImport()> _
        '    <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
        '    <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
        '    Public Interface FolderItems
        '        'Inherits IEnumerable
        '        ReadOnly Property Count() As Integer
        '        ReadOnly Property Application() As IDispatch
        '        ReadOnly Property Parent() As IDispatch
        '        Function Item(ByVal index As Object) As FolderItem

        '        <Runtime.InteropServices.DispId(-4)> _
        '        Function _NewEnum() As IDispatch ' IEnumVARIANT

        '    End Interface


        '    <Runtime.InteropServices.ComImport()> _
        '    <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
        '    <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
        '    <Runtime.InteropServices.Guid("FAC32C80-CBE4-11CE-8350-444553540000")> _
        '    Public Interface FolderItem
        '        ReadOnly Property Application() As IDispatch
        '        ReadOnly Property Parent() As IDispatch
        '        Property Name() As String
        '        ReadOnly Property Path() As String
        '        ReadOnly Property GetLink() As IDispatch
        '        ReadOnly Property GetFolder() As IDispatch
        '        ReadOnly Property IsLink() As Boolean
        '        ReadOnly Property IsFolder() As Boolean
        '        ReadOnly Property IsFileSystem() As Boolean
        '        ReadOnly Property IsBrowsable() As Boolean
        '        Property ModifyDate() As Date
        '        ReadOnly Property Size() As Integer
        '        ReadOnly Property Type() As String
        '        Function Verbs() As FolderItemVerbs
        '        Sub InvokeVerb(ByVal vVerb As Object)
        '    End Interface


        '    <Runtime.InteropServices.ComImport()> _
        '    <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
        '    <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
        '    <Runtime.InteropServices.Guid("1F8352C0-50B0-11CF-960C-0080C7F4EE85")> _
        '    Public Interface FolderItemVerbs
        '        'Inherits IEnumerable
        '        ReadOnly Property Count() As Integer
        '        ReadOnly Property Application() As IDispatch
        '        ReadOnly Property Parent() As IDispatch
        '        Function Item(ByVal index As Object) As FolderItemVerb

        '        <Runtime.InteropServices.DispId(-4)> _
        '        Function _NewEnum() As IDispatch ' IEnumVARIANT
        '    End Interface


        '    <Runtime.InteropServices.ComImport()> _
        '    <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
        '    <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
        '    <Runtime.InteropServices.Guid("08EC3E00-50B0-11CF-960C-0080C7F4EE85")> _
        '    Public Interface FolderItemVerb
        '        ReadOnly Property Application() As IDispatch
        '        ReadOnly Property Parent() As IDispatch
        '        ReadOnly Property Name() As String
        '        Sub DoIt()
        '    End Interface

        'End Namespace

        Namespace Shell32
            '' Hier werden die Schnittstellen von Klassen der Com-Library "MS Shell Controls and Automation"
            '' implementiert, mit denen ein Kopiervorgang durchgef¸hrt werden kann. Dieser Kopiervorgang
            '' akzeptiert auch Zip-Dateien, n‰mlich als "zip-komprimierte Ordner".

            '' Die Information ¸ber Typen, Attribute und Reihenfolge der Schnittstellenmember sind in
            '' C:\Program Files\Microsoft Visual Studio 8\VC\PlatformSDK\Include\shldisp.idl dokumentiert.
            '' Wer diese Doku in Augenschein nehmen will, kann, falls er shldisp.idl nicht findet, das
            '' Visual Studio 2008 SDK 1.1 downloaden: 
            ''http://www.microsoft.com/downloads/details.aspx?displaylang=en&FamilyID=59ec6ec3-4273-48a3-ba25-dc925a45584d


            '<System.Runtime.InteropServices.Guid("00020400-0000-0000-c000-000000000046")> _
            '<System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)> _
            'Public Interface IDispatch
            '    <System.Runtime.InteropServices.PreserveSig()> _
            '    Function GetTypeInfoCount() As Integer
            '    Function GetTypeInfo( _
            '        <System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)> ByVal iTInfo As Integer, _
            '        <System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)> ByVal lcid As Integer _
            '        ) As System.Runtime.InteropServices.ComTypes.ITypeInfo
            '    <System.Runtime.InteropServices.PreserveSig()> _
            '    Function GetIDsOfNames(ByRef riid As Guid, ByVal rgsNames As String(), _
            '        <System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)> ByVal cNames As Integer, _
            '        <System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)> ByVal lcid As Integer, _
            '        ByVal rgDispId As Integer()) As Integer
            '    <System.Runtime.InteropServices.PreserveSig()> _
            '    Function Invoke(ByVal dispIdMember As Integer, ByRef riid As Guid, _
            '        <System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)> ByVal lcid As Integer, _
            '        <System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)> ByVal dwFlags As Integer, _
            '        ByRef pDispParams As System.Runtime.InteropServices.ComTypes.DISPPARAMS, _
            '        <System.Runtime.InteropServices.Out()> ByVal pVarResult As Object(), _
            '        ByRef pExcepInfo As System.Runtime.InteropServices.ComTypes.EXCEPINFO, _
            '        <System.Runtime.InteropServices.Out()> ByVal pArgErr As IntPtr()) As Integer
            'End Interface

            '<System.Runtime.InteropServices.ComImport()> _
            '<System.Runtime.InteropServices.Guid("D8F015C0-C278-11CE-A49E-444553540000")> _
            '<System.Runtime.InteropServices.TypeLibType(System.Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            '<System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            'Public Interface IShellDispatch
            '    ReadOnly Property Application() As IDispatch
            '    ReadOnly Property Parent() As IDispatch
            '    Function [Namespace](ByVal vDir As Object) As Folder
            '    Function BrowseForFolder(ByVal hWnd As Integer, ByVal Title As String, ByVal Otions As Integer, ByVal RootFolder As Object) As Folder
            '    Function Windows() As IDispatch 'IDispatch
            '    Sub Open(ByVal vDir As Object)
            '    Sub Explore(ByVal vDir As Object)
            '    Sub MinimizeAll()
            '    Sub UndoMinimizeALL()
            '    Sub FileRun()
            '    Sub CascadeWindows()
            '    Sub TileVertically()
            '    Sub TileHorizontally()
            '    Sub ShutdownWindows()
            '    Sub Suspend()
            '    Sub EjectPC()
            '    Sub SetTime()
            '    Sub TrayProperties()
            '    Sub Help()
            '    Sub FindFiles()
            '    Sub FindComputer()
            '    Sub RefreshMenu()
            '    Sub ControlPanelItem(ByVal bstrDir As String)
            'End Interface

            '<System.Runtime.InteropServices.ComImport()> _
            '<System.Runtime.InteropServices.Guid("BBCBDE60-C3FF-11CE-8350-444553540000")> _
            '<System.Runtime.InteropServices.TypeLibType(System.Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            '<System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            'Public Interface Folder
            '    ReadOnly Property Title() As String
            '    ReadOnly Property Application() As IDispatch
            '    ReadOnly Property Parent() As IDispatch
            '    ReadOnly Property ParentFolder() As Folder ' Shell32.Folder

            '    Function Items() As FolderItems ' Shell32.FolderItems
            '    Function ParseName(ByVal bName As String) As FolderItem ' Shell32.FolderItem
            '    Sub NewFolder(ByVal bName As String, Optional ByVal vOptions As Object = Nothing)
            '    Sub MoveHere(ByVal vItem As Object, Optional ByVal vOptions As Object = Nothing)
            '    Sub CopyHere(ByVal vItem As Object, Optional ByVal vOptions As Object = Nothing)
            '    Function GetDetailsOf(ByVal vItem As Object, ByVal iColumn As Integer) As String

            'End Interface

            '<System.Runtime.InteropServices.Guid("744129E0-CBE5-11CE-8350-444553540000")> _
            '<System.Runtime.InteropServices.ComImport()> _
            '<System.Runtime.InteropServices.TypeLibType(System.Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            '<System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            'Public Interface FolderItems
            '    'Inherits IEnumerable
            '    ReadOnly Property Count() As Integer
            '    ReadOnly Property Application() As IDispatch
            '    ReadOnly Property Parent() As IDispatch
            '    Function Item(ByVal index As Object) As FolderItem

            '    <System.Runtime.InteropServices.DispId(-4)> _
            '    <System.Runtime.InteropServices.TypeLibFunc(System.Runtime.InteropServices.TypeLibFuncFlags.FRestricted Or System.Runtime.InteropServices.TypeLibFuncFlags.FHidden)> _
            '    Function _NewEnum() As System.Runtime.InteropServices.ComTypes.IEnumVARIANT

            'End Interface

            '<System.Runtime.InteropServices.ComImport()> _
            '<System.Runtime.InteropServices.TypeLibType(System.Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            '<System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            '<System.Runtime.InteropServices.Guid("FAC32C80-CBE4-11CE-8350-444553540000")> _
            'Public Interface FolderItem
            '    ReadOnly Property Application() As IDispatch
            '    ReadOnly Property Parent() As IDispatch
            '    Property Name() As String
            '    ReadOnly Property Path() As String
            '    ReadOnly Property GetLink() As IDispatch
            '    ReadOnly Property GetFolder() As IDispatch
            '    ReadOnly Property IsLink() As Boolean
            '    ReadOnly Property IsFolder() As Boolean
            '    ReadOnly Property IsFileSystem() As Boolean
            '    ReadOnly Property IsBrowsable() As Boolean
            '    Property ModifyDate() As Date
            '    ReadOnly Property Size() As Integer
            '    ReadOnly Property Type() As String
            '    Function Verbs() As FolderItemVerbs
            '    Sub InvokeVerb(ByVal vVerb As Object)
            'End Interface

            '<System.Runtime.InteropServices.ComImport()> _
            '<System.Runtime.InteropServices.TypeLibType(System.Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            '<System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            '<System.Runtime.InteropServices.Guid("1F8352C0-50B0-11CF-960C-0080C7F4EE85")> _
            'Public Interface FolderItemVerbs
            '    'Inherits IEnumerable
            '    ReadOnly Property Count() As Integer
            '    ReadOnly Property Application() As IDispatch
            '    ReadOnly Property Parent() As IDispatch
            '    Function Item(ByVal index As Object) As FolderItemVerb

            '    <System.Runtime.InteropServices.DispId(-4)> _
            '    <System.Runtime.InteropServices.TypeLibFunc(System.Runtime.InteropServices.TypeLibFuncFlags.FRestricted Or System.Runtime.InteropServices.TypeLibFuncFlags.FHidden)> _
            '    Function _NewEnum() As System.Runtime.InteropServices.ComTypes.IEnumVARIANT
            'End Interface

            '<System.Runtime.InteropServices.ComImport()> _
            '<System.Runtime.InteropServices.TypeLibType(System.Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            '<System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            '<System.Runtime.InteropServices.Guid("08EC3E00-50B0-11CF-960C-0080C7F4EE85")> _
            'Public Interface FolderItemVerb
            '    ReadOnly Property Application() As IDispatch
            '    ReadOnly Property Parent() As IDispatch
            '    ReadOnly Property Name() As String
            '    Sub DoIt()
            'End Interface










            '' Hier eine radikal abgespeckte Version der Com-Interfaces, die ganz speziell nur die zum
            '' Zippen notwendigen Teile der  Schnittstellen-Definitionen von Shell32 implementiert.
            '' Einige Member sind als "DummiXY" implementiert, und dienen nur als Platzhalter, damit die
            '' Member "NameSpace()", "CopyHere()" und "Items()" in der VTable richtig addressiert werden.
            '' Weitere Member wurden weggelassen.

            '''' <summary>
            '''' Hier als "DummiXX" implementierte Member sind nicht zur Verwendung vorgesehen.
            '''' </summary>
            '<System.Runtime.InteropServices.ComImport()> _
            '<System.Runtime.InteropServices.Guid("D8F015C0-C278-11CE-A49E-444553540000")> _
            '<System.Runtime.InteropServices.TypeLibType(System.Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            '<System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            'Public Interface IShellDispatch
            '    ReadOnly Property Dummi1() As Object
            '    ReadOnly Property Dummi2() As Object
            '    Function [NameSpace](ByVal vDir As Object) As ShellFolder
            'End Interface

            ''' <summary>
            ''' Hier als "DummiXX" implementierte Member sind nicht zur Verwendung vorgesehen.
            ''' </summary>
            <System.Runtime.InteropServices.ComImport()> _
            <System.Runtime.InteropServices.Guid("BBCBDE60-C3FF-11CE-8350-444553540000")> _
            <System.Runtime.InteropServices.TypeLibType(System.Runtime.InteropServices.TypeLibTypeFlags.FDispatchable Or System.Runtime.InteropServices.TypeLibTypeFlags.FDual)> _
            <System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIDispatch)> _
            Public Interface ShellFolder
                <System.Runtime.InteropServices.DispId(1610743809)> _
                ReadOnly Property Dummi1() As Object
                <System.Runtime.InteropServices.DispId(1610743810)> _
                ReadOnly Property Dummi2() As Object
                <System.Runtime.InteropServices.DispId(1610743811)> _
                ReadOnly Property ParentFolder() As ShellFolder
                <System.Runtime.InteropServices.DispId(0)> _
                ReadOnly Property Title() As String
                <System.Runtime.InteropServices.DispId(1610743816)> _
                Sub CopyHere( _
                      <System.Runtime.InteropServices.In()> _
                      ByVal vItem As Object, _
                      <System.Runtime.InteropServices.In()> _
                      Optional ByVal vOptions As Object = Nothing)
                <System.Runtime.InteropServices.DispId(1610743817)> _
                Function Dummi3(ByVal vItem As Object, ByVal iColumn As Integer) As String
                <System.Runtime.InteropServices.DispId(1610743815)> _
                Sub MoveHere(ByVal vItem As Object, ByVal vOptions As Object)
                <System.Runtime.InteropServices.DispId(1610743814)> _
                Sub Dummi4(ByVal bName As String, ByVal vOptions As Object)
                <System.Runtime.InteropServices.DispId(1610743812)> _
                Function Items() As Object 'ist nicht der korrekte Datentyp, aber f¸r CopyHere() reichts
            End Interface






            <Runtime.InteropServices.Guid("00020400-0000-0000-c000-000000000046")> _
            <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)> _
            Public Interface IDispatch
                <Runtime.InteropServices.PreserveSig()> _
                Function GetTypeInfoCount() As Integer
                Function GetTypeInfo( _
                    <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.U4)> ByVal iTInfo As Integer, _
                    <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.U4)> ByVal lcid As Integer _
                    ) As Runtime.InteropServices.ComTypes.ITypeInfo
                <Runtime.InteropServices.PreserveSig()> _
                Function GetIDsOfNames(ByRef riid As Guid, ByVal rgsNames As String(), _
                    <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.U4)> ByVal cNames As Integer, _
                    <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.U4)> ByVal lcid As Integer, _
                    ByVal rgDispId As Integer()) As Integer
                <Runtime.InteropServices.PreserveSig()> _
                Function Invoke(ByVal dispIdMember As Integer, ByRef riid As Guid, _
                    <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.U4)> ByVal lcid As Integer, _
                    <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.U4)> ByVal dwFlags As Integer, _
                    ByRef pDispParams As Runtime.InteropServices.ComTypes.DISPPARAMS, _
                    <Runtime.InteropServices.Out()> ByVal pVarResult As Object(), _
                    ByRef pExcepInfo As Runtime.InteropServices.ComTypes.EXCEPINFO, _
                    <Runtime.InteropServices.Out()> ByVal pArgErr As IntPtr()) As Integer
            End Interface

            <Runtime.InteropServices.ComImport()> _
            <Runtime.InteropServices.Guid("D8F015C0-C278-11CE-A49E-444553540000")> _
            <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            Public Interface IShellDispatch
                ReadOnly Property Application() As IDispatch
                ReadOnly Property Parent() As IDispatch
                Function [Namespace](ByVal vDir As Object) As Folder
                Function BrowseForFolder(ByVal hWnd As Integer, ByVal Title As String, ByVal Otions As Integer, ByVal RootFolder As Object) As Folder
                Function Windows() As IDispatch 'IDispatch
                Sub Open(ByVal vDir As Object)
                Sub Explore(ByVal vDir As Object)
                Sub MinimizeAll()
                Sub UndoMinimizeALL()
                Sub FileRun()
                Sub CascadeWindows()
                Sub TileVertically()
                Sub TileHorizontally()
                Sub ShutdownWindows()
                Sub Suspend()
                Sub EjectPC()
                Sub SetTime()
                Sub TrayProperties()
                Sub Help()
                Sub FindFiles()
                Sub FindComputer()
                Sub RefreshMenu()
                Sub ControlPanelItem(ByVal bstrDir As String)
            End Interface

            <Runtime.InteropServices.ComImport()> _
            <Runtime.InteropServices.Guid("BBCBDE60-C3FF-11CE-8350-444553540000")> _
            <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            Public Interface Folder
                ReadOnly Property Title() As String
                ReadOnly Property Application() As IDispatch
                ReadOnly Property Parent() As IDispatch
                ReadOnly Property ParentFolder() As Folder ' Shell32.Folder

                Function Items() As FolderItems ' Shell32.FolderItems
                Function ParseName(ByVal bName As String) As FolderItem ' Shell32.FolderItem
                Sub NewFolder(ByVal bName As String, Optional ByVal vOptions As Object = Nothing)
                Sub MoveHere(ByVal vItem As Object, Optional ByVal vOptions As Object = Nothing)
                Sub CopyHere(ByVal vItem As Object, Optional ByVal vOptions As Object = Nothing)
                Function GetDetailsOf(ByVal vItem As Object, ByVal iColumn As Integer) As String

            End Interface

            <Runtime.InteropServices.Guid("744129E0-CBE5-11CE-8350-444553540000")> _
            <Runtime.InteropServices.ComImport()> _
            <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            Public Interface FolderItems
                'Inherits IEnumerable
                ReadOnly Property Count() As Integer
                ReadOnly Property Application() As IDispatch
                ReadOnly Property Parent() As IDispatch
                Function Item(ByVal index As Object) As FolderItem

                <Runtime.InteropServices.DispId(-4)> _
                <Runtime.InteropServices.TypeLibFunc(Runtime.InteropServices.TypeLibFuncFlags.FRestricted Or Runtime.InteropServices.TypeLibFuncFlags.FHidden)> _
                Function _NewEnum() As Runtime.InteropServices.ComTypes.IEnumVARIANT

            End Interface

            <Runtime.InteropServices.ComImport()> _
            <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            <Runtime.InteropServices.Guid("FAC32C80-CBE4-11CE-8350-444553540000")> _
            Public Interface FolderItem
                ReadOnly Property Application() As IDispatch
                ReadOnly Property Parent() As IDispatch
                Property Name() As String
                ReadOnly Property Path() As String
                ReadOnly Property GetLink() As IDispatch
                ReadOnly Property GetFolder() As IDispatch
                ReadOnly Property IsLink() As Boolean
                ReadOnly Property IsFolder() As Boolean
                ReadOnly Property IsFileSystem() As Boolean
                ReadOnly Property IsBrowsable() As Boolean
                Property ModifyDate() As Date
                ReadOnly Property Size() As Integer
                ReadOnly Property Type() As String
                Function Verbs() As FolderItemVerbs
                Sub InvokeVerb(ByVal vVerb As Object)
            End Interface

            <Runtime.InteropServices.ComImport()> _
            <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            <Runtime.InteropServices.Guid("1F8352C0-50B0-11CF-960C-0080C7F4EE85")> _
            Public Interface FolderItemVerbs
                'Inherits IEnumerable
                ReadOnly Property Count() As Integer
                ReadOnly Property Application() As IDispatch
                ReadOnly Property Parent() As IDispatch
                Function Item(ByVal index As Object) As FolderItemVerb

                <Runtime.InteropServices.DispId(-4)> _
                <Runtime.InteropServices.TypeLibFunc(Runtime.InteropServices.TypeLibFuncFlags.FRestricted Or Runtime.InteropServices.TypeLibFuncFlags.FHidden)> _
                Function _NewEnum() As Runtime.InteropServices.ComTypes.IEnumVARIANT
            End Interface

            <Runtime.InteropServices.ComImport()> _
            <Runtime.InteropServices.TypeLibType(Runtime.InteropServices.TypeLibTypeFlags.FOleAutomation)> _
            <Runtime.InteropServices.InterfaceType(Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)> _
            <Runtime.InteropServices.Guid("08EC3E00-50B0-11CF-960C-0080C7F4EE85")> _
            Public Interface FolderItemVerb
                ReadOnly Property Application() As IDispatch
                ReadOnly Property Parent() As IDispatch
                ReadOnly Property Name() As String
                Sub DoIt()
            End Interface
        End Namespace


        Public Module modFileSystem

            Public Function IsDirectory(ByVal Path As String) As Boolean
                Dim ret As Boolean = False
                'Try
                'GetAggributes returns "Directory" on file. It is not working
                '    'first check if path exists
                '    Dim Attr As System.IO.FileAttributes
                '    Attr = System.IO.File.GetAttributes(Path)
                '    ret = (Attr = IO.FileAttributes.Directory)

                'Catch ex As Exception
                'it does not exist. 
                'lets try to determine if the file is any directory
                If Path.EndsWith("\") Then
                    'now we are 100% sure it is an directory
                    ret = True
                Else
                    'at this state we are not sure what it is
                    'check for file extension
                    Dim LastPart As String = Path.Split("\")(Path.Split("\").Length - 1)
                    If IO.Path.HasExtension(Path) = True Then
                        ret = False
                        'we are nearly sure it is an file now
                        Dim Ext As String = IO.Path.GetExtension(Path)
                        If LastPart.Length >= 5 And Ext.Length >= 3 Then
                            'lets say it is an file with extension
                            ret = False
                        Else
                            'any directory with point in the directory
                            'example "db.1\"
                            ret = True
                        End If
                    Else
                        'we are nearly 99% sure it is an directory now
                        'on Windows System, if no extension is available we 
                        'assume that is an directory.
                        ret = True
                    End If
                End If

                'End Try
                Return ret
            End Function

            ''' <summary>
            ''' Returns the icon of an exe file or dll. 
            ''' Set the Imagesize to this size you need the bitmap back. Default = 16px * 16px
            ''' </summary>
            ''' <param name="fi"></param>
            ''' <param name="ImageSize"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function ImageFromAssociatedFile(ByVal fi As IO.FileInfo, ByVal ImageSize As System.Drawing.Size) As System.Drawing.Bitmap
                Dim bm As New System.Drawing.Bitmap(ImageSize.Width, ImageSize.Height)
                Try
                    bm = ImageFromAssociatedFile(fi).GetThumbnailImage(ImageSize.Width, ImageSize.Height, Nothing, IntPtr.Zero)

                Catch ex As Exception
                    Using gr As Graphics = Graphics.FromImage(bm)
                        gr.Clear(Color.Gray)
                        gr.DrawString(fi.Name, New Font("Arial", 6, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Black, 0, 5)
                    End Using
                End Try
                Return bm
            End Function

            ''' <summary>
            ''' Returns the icon of an exe file or dll. 
            ''' Set the Imagesize to this size you need the bitmap back. Default = 16px * 16px
            ''' </summary>
            ''' <param name="fi"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function ImageFromAssociatedFile(ByVal fi As IO.FileInfo) As System.Drawing.Bitmap
                Dim bm As System.Drawing.Bitmap '(ImageSize.Width, ImageSize.Height)
                Try
                    bm = CType(System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName).ToBitmap(), System.Drawing.Bitmap)

                Catch ex As Exception
                    bm = New System.Drawing.Bitmap(64, 64)
                    Using gr As Graphics = Graphics.FromImage(bm)
                        gr.Clear(Color.Gray)
                        gr.DrawString(fi.Name, New Font("Arial", 6, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Black, 0, 5)
                    End Using
                End Try
                Return bm
            End Function
 
 
 

             

            'find all file in a specified directory and all subdirectories
            Public Function FindFilesRecursive(ByVal BaseDirectory As IO.DirectoryInfo) As List(Of IO.FileInfo)
                Return FindFilesRecursive(BaseDirectory, Nothing)
            End Function


            'find all file in a specified directory and all subdirectories
            Public Function FindFilesRecursive(ByVal BaseDirectory As IO.DirectoryInfo, ByVal Pattern As String) As List(Of IO.FileInfo)
                Dim LRes As New List(Of IO.FileInfo)
                If Pattern Is Nothing Then Pattern = String.Empty
                For Each fi As IO.FileInfo In BaseDirectory.GetFiles(Pattern)
                    LRes.Add(fi)
                Next

                For Each di As IO.DirectoryInfo In BaseDirectory.GetDirectories
                    Dim LR2 As List(Of IO.FileInfo) = FindFilesRecursive(di, Pattern)
                    LRes.AddRange(LR2.ToArray)
                Next

                Return LRes
            End Function

 

            '      		[DllImport("kernel32")]
            'private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);
            '[DllImport("kernel32")]
            'private static extern int GetPrivateProfileString(string section,string key,string def,StringBuilder retVal,int size,string filePath);


            ''' <summary>
            ''' Returns a trimmed path
            ''' For example: C:\Myfolder\MySubfolder\Myfile.txt goes to C:\...\Myfile.txt
            ''' </summary>
            Public Function TrimPath(ByVal fi As IO.FileInfo, ByVal fnt As Font, ByVal PixelWidth As Integer) As String
                If fnt Is Nothing Then fnt = New System.Drawing.Font("Arial", 9)
                Dim PixelW As Integer
                Dim Res As String = ""
                Dim StartW As String
                Dim bm As New Bitmap(1, 1)
                Dim L As New List(Of String)
                Call DirectoryToList(fi.Directory, L)
                Dim AllPrint As Boolean = False

                StartW = fi.Directory.Name & "\" & fi.Name
                Using gr As Graphics = Graphics.FromImage(bm)
                    PixelW = gr.MeasureString(StartW, fnt).ToSize.Width
                    Res = ""
                    Dim i As Integer = 0
                    Do
                        If i > L.Count - 1 - 1 Then
                            AllPrint = True
                            Exit Do
                        End If
                        Dim W As String = L(i)
                        If L(i).EndsWith("\") = False Then W = W & "\"
                        PixelW += gr.MeasureString(W, fnt).ToSize.Width
                        If PixelW > PixelWidth Then Exit Do
                        Res = Res & W
                        i += 1
                    Loop

                    If AllPrint = False Then
                        Res &= "...\"
                    End If

                    Res &= StartW

                    Return Res
                End Using
            End Function

            ''' <summary>
            ''' Returns a trimmed path
            ''' For example: C:\Myfolder\MySubfolder\Myfile.txt goes to C:\...\Myfile.txt
            ''' </summary>
            Public Function TrimPath(ByVal di As IO.DirectoryInfo, ByVal fnt As Font, ByVal PixelWidth As Integer) As String
                If fnt Is Nothing Then fnt = New System.Drawing.Font("Arial", 9)
                Dim PixelW As Integer
                Dim Res As String = ""
                Dim StartW As String
                Dim bm As New Bitmap(1, 1)
                Dim L As New List(Of String)
                Call DirectoryToList(di.Parent, L)
                Dim AllPrint As Boolean = False

                StartW = di.Name & "\"
                Using gr As Graphics = Graphics.FromImage(bm)
                    PixelW = gr.MeasureString(StartW, fnt).ToSize.Width
                    Res = ""
                    Dim i As Integer = 0
                    Do
                        If i > L.Count - 1 - 0 Then 'new, not -1 again but -0
                            AllPrint = True
                            Exit Do
                        End If
                        Dim W As String = L(i)
                        If L(i).EndsWith("\") = False Then W = W & "\"
                        PixelW += gr.MeasureString(W, fnt).ToSize.Width
                        If PixelW > PixelWidth Then Exit Do
                        Res = Res & W
                        i += 1
                    Loop

                    If AllPrint = False Then
                        Res &= "...\"
                    End If

                    Res &= StartW

                    Return Res
                End Using
            End Function

            Private Sub DirectoryToList(ByVal Dir As IO.DirectoryInfo, ByRef L As List(Of String))
                If Dir IsNot Nothing Then
                    L.Insert(0, Dir.Name)
                    Call DirectoryToList(Dir.Parent, L)
                End If
            End Sub


            ''' <summary>
            ''' Does make the same thing like "System.Io.Path.Combine" 
            ''' But if the filename starts with an "\" the microsoft version makes an fault ignores the BaseDirectory. 
            ''' This function will have an eye on it.
            ''' </summary>
            ''' <param name="BaseDirectory"></param>
            ''' <param name="FileOrFolderName"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function Combine(ByVal BaseDirectory As String, ByVal FileOrFolderName As String) As String
                If FileOrFolderName Is Nothing OrElse BaseDirectory Is Nothing Then Return String.Empty
                If FileOrFolderName.StartsWith("\") Then Return BaseDirectory & FileOrFolderName

                Return IO.Path.Combine(BaseDirectory, FileOrFolderName)
            End Function

            ''' <summary>
            ''' Switches the file name of two files.
            ''' For example: 
            ''' A = C:\Test1.txt (content = "12345")
            ''' B = D:\Temp\Hello World.doc (content = a word file)
            ''' After switch
            ''' A = D:\Temp\Hello World.doc (content = "12345")
            ''' B = C:\Test1.txt (content = a word file)
            ''' </summary>
            ''' <param name="fi1">file to switch name with fi2</param>
            ''' <param name="fi2">file to switch name with fi1</param>
            ''' <remarks>Can be used to change the order of files in a list.</remarks>
            Public Sub SwitchFileNames(ByVal fi1 As IO.FileInfo, ByVal fi2 As IO.FileInfo)
                Dim fiBak As New IO.FileInfo(fi1.FullName & ".bak")

                IO.File.Move(fi1.FullName, fiBak.FullName)
                IO.File.Move(fi2.FullName, fi1.FullName)
                IO.File.Move(fiBak.FullName, fi2.FullName)
            End Sub


#Region "Zip/Unzip"


            '''' <summary>
            '''' Extract a zip file into the same folder
            '''' </summary>
            '''' <returns>True if all worked perfectly</returns>
            '''' <remarks>http://msdn.microsoft.com/en-us/library/ms723207(VS.85).aspx</remarks>
            'Public Function UnZip(ByVal fiSrcZipFile As IO.FileInfo, ByVal AskForOverWrite As Boolean) As Boolean
            '    '0:     Default. No options specified. 
            '    '4:     Do not display a progress dialog box. 
            '    '8:     Rename the target file if a file exists at the target location with the same name. 
            '    '16:    Click "Yes to All" in any dialog box displayed. 
            '    '64:    Preserve undo information, if possible. 
            '    '128:   Perform the operation only if a wildcard file name (*.*) is specified. 
            '    '256:   Display a progress dialog box but do not show the file names. 
            '    '512:   Do not confirm the creation of a new directory if the operation requires one to be created. 
            '    '1024:  Do not display a user interface if an error occurs. 
            '    '4096:  Disable recursion. 
            '    '8192:  Do not copy connected files as a group. Only copy the specified files. 


            '    'UnZip mit Shell32

            '    'Referenz auf Shell32.dll nˆtig in Windows\System32 
            '    Dim objShell As TB.FileSystem.Shell32.IShellDispatch
            '    Dim Gut As Boolean = True 'returns true if all worked perfectly

            '    'LateBinding  
            '    objShell = CreateObject("Shell.Application")

            '    'Entpackerobjekt erstellen
            '    With objShell 'CreateObject("Shell.Application") 'CreateObject("Shell.Application")

            '        'Achtung, ohne "Shell32.Folder" wird das private Interface "Shell32Folder" verwendet
            '        Dim S32DstFolder As TB.FileSystem.Shell32.Folder 'Enth‰lt den Inhalt der ZipDatei
            '        Dim S32ZipFile As TB.FileSystem.Shell32.Folder 'Die ZipDatei  

            '        S32DstFolder = .ShellNameSpace(fiSrcZipFile.DirectoryName)
            '        S32ZipFile = .ShellNameSpace(fiSrcZipFile.FullName)

            '        If AskForOverWrite = False Then
            '            Dim i As Integer
            '            Dim Item As TB.FileSystem.Shell32.FolderItem
            '            Dim fi As IO.FileInfo
            '            For i = 0 To S32ZipFile.Items.Count - 1
            '                Item = S32ZipFile.Items.Item(i)
            '                'Now we can raise any event with a information about the 
            '                'file size and files count. Perhaps for any progressbar.
            '                'RaiseEvent ZipProgress(cSng(i / S32ZipFile.Items.Count * 100), Item.Size)
            '                If Item.IsFolder = False Then 'Do not delete folders (perhaps folders are updated with other files)
            '                    fi = New IO.FileInfo(IO.Path.Combine(fiSrcZipFile.DirectoryName, Item.Name))
            '                    If fi.Exists = True Then
            '                        Try
            '                            'Delete the file manually the get any
            '                            'exception if necessary. Because in this case,
            '                            'some file are not from zip file because they 
            '                            'will be leave protected in old version.
            '                            fi.Delete()
            '                        Catch ex As Exception
            '                            Gut = False
            '                        End Try
            '                    End If
            '                End If
            '                S32DstFolder.CopyHere(Item, (4 Or 16))

            '            Next
            '        Else
            '            'Use system file dialog.
            '            'If a file or folder exists you can choose
            '            'between "overwriting" or "not overwriting" the file.
            '            S32DstFolder.CopyHere(S32ZipFile.Items, Nothing)
            '        End If

            '    End With

            '    Return Gut
            'End Function



            'Public Sub ZipFile(ByVal Sources As List(Of IO.FileInfo), ByVal DstZipFile As IO.FileInfo)
            '    DstZipFile.Refresh()
            '    If DstZipFile.Exists = True Then
            '        Try
            '            My.Computer.FileSystem.DeleteFile(DstZipFile.FullName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
            '        Catch ex As Exception
            '            Throw New Exception("Access error in TB.modFileSystem.ZipFile. DstZipFile Could not be deleted: " & DstZipFile.FullName)
            '        End Try

            '    End If
            '    CreateEmptyZip(DstZipFile)
            '    Try
            '        Dim objShell As TB.FileSystem.Shell32_TBE.IShellDispatch
            '        Dim Gut As Boolean = True 'returns true if all worked perfectly
            '        Dim i As Integer

            '        'LateBinding  
            '        objShell = CreateObject("Shell.Application")

            '        'Entpackerobjekt erstellen
            '        'With objShell 'CreateObject("Shell.Application") 'CreateObject("Shell.Application")

            '        Dim S32SrcFolder As TB.FileSystem.Shell32_TBE.Folder 'Enth‰lt den Inhalt der ZipDatei
            '        Dim S32ZipFile As TB.FileSystem.Shell32_TBE.Folder 'Die ZipDatei  

            '        S32ZipFile = objShell.ShellNameSpace(DstZipFile.FullName)

            '        'With CreateObject("Shell.Application")
            '        '.NameSpace("c:\testzip.zip").CopyHere("c:\test.txt")
            '        Dim fi As IO.FileInfo
            '        For Each fi In Sources
            '            S32SrcFolder = objShell.ShellNameSpace(fi.DirectoryName)
            '            ' .NameSpace("c:\testzip.zip").CopyHere .NameSpace(FolderName).items 'use this line if we want to zip all items in a folder into our zip file
            '            'Call objShell.Namespace(DstZipFile.FullName).CopyHere(objShell.Namespace(fi.FullName).items()) 'use this line if we want to zip all items in a folder into our zip file
            '            'For i = 0 To S32SrcFolder.Items.Count - 1
            '            '    If S32SrcFolder.Items.Item(i).Name = fi.Name Then
            '            '        'S32ZipFile.Items.Item(i)
            '            'S32SrcFolder.CopyHere(S32ZipFile.Items) 'geht nicht
            '            'S32ZipFile.CopyHere(S32SrcFolder.Items) 'es geht was. aber kein inhalt im zip file
            '            'S32ZipFile.CopyHere(S32SrcFolder.Items)'1:1 abgeschrieben vom Beispiel aus VB6

            '            '        'S32ZipFile.CopyHere(S32ZipFile.Items.Item(i))'Geht, hat allerdings keinen Effekt im Zip
            '            '    End If
            '            'Next
            '        Next

            '        'End With
            '    Catch ex As Exception

            '    End Try
            '    ' All done!
            'End Sub


            'Private Sub CreateEmptyZip(ByVal fiZipFile As IO.FileInfo)
            '    Dim strZIPHeader As String

            '    'strZIPHeader = Chr$(80) & Chr$(75) & Chr$(5) & Chr$(6) & String(18, 0) ' header required to convince Windows shell that this is really a zip file
            '    strZIPHeader = Chr(80) & Chr(75) & Chr(5) & Chr(6)
            '    strZIPHeader = strZIPHeader & Microsoft.VisualBasic.Strings.StrDup(18, Chr(0)) ' String( 18, 0) ' header required to convince Windows shell that this is really a zip file

            '    If fiZipFile.Exists = True Then fiZipFile.Delete()
            '    Call My.Computer.FileSystem.WriteAllText(fiZipFile.FullName, strZIPHeader, False)
            'End Sub


            Public Sub ZipFile(ByVal FolderName As String, Optional ByVal ZipFileName As String = Nothing)
                'If FolderName.EndsWith("\") = False Then FolderName = FolderName & "\"
                FolderName = IO.Path.GetFullPath(FolderName)
                If ZipFileName Is Nothing Then
                    ZipFileName = FolderName & ".zip"
                Else
                    ZipFileName = IO.Path.GetFullPath(ZipFileName)
                End If
                If IO.File.Exists(ZipFileName) Then IO.File.Delete(ZipFileName)
                'leeres ZipFile erzeugen
                Dim emptyZipData = New Byte() {80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                Using fs = New IO.FileStream(ZipFileName, IO.FileMode.CreateNew)
                    fs.Write(emptyZipData, 0, emptyZipData.Length)
                End Using
                ShellCopy(FolderName, ZipFileName)
            End Sub

            Public Sub UnZip(ByVal ZipFileName As String, Optional ByVal FolderName As String = Nothing)
                ZipFileName = IO.Path.GetFullPath(ZipFileName)
                If FolderName Is Nothing Then
                    Dim i = ZipFileName.LastIndexOf("."c)
                    FolderName = ZipFileName.Substring(0, i)
                Else
                    FolderName = IO.Path.GetFullPath(FolderName)
                End If
                Try 'try weil Delete evtl. keine berechtigung hat
                    If IO.Directory.Exists(FolderName) Then IO.Directory.Delete(FolderName, True)
                Catch ex As Exception

                End Try
                Try
                    IO.Directory.CreateDirectory(FolderName)
                Catch ex As Exception

                End Try
                ShellCopy(ZipFileName, FolderName)
            End Sub

            Private Sub ShellCopy(ByVal source As String, ByVal destination As String)
                ' F¸r die Shell sind Zip-Dateien einfach Datei-Ordner
                ' Zippen/UnZippen ist also ein simpler Kopier-Vorgang von einen in den anderen ShellFolder
                Dim _shell As Shell32.IShellDispatch
                _shell = DirectCast(CreateObject("Shell.Application"), Shell32.IShellDispatch)
                Dim dst = _shell.Namespace(destination)
                Dim src = _shell.Namespace(source)
                Dim itms = src.Items
                dst.CopyHere(itms)
                'Com-Objekte sind speziell aufzur‰umen - beachte auch die Reihenfolge
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(itms)
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(src)
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(dst)
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_shell)
            End Sub

#End Region

            ''' <summary>
            ''' Return the path and file name by replacing standard file pattern
            ''' </summary>
            ''' <param name="FileName">Dim A As String = "%ProgramFiles%Test\1234\MyFile.txt"</param>
            ''' <returns>D:\Program Files\Test\1234\MyFile.txt</returns>
            ''' <remarks></remarks>
            Public Function GetFileByKeyWordReplacer(ByVal FileName As String) As String
                If FileName Is Nothing Then Return ""
                FileName = FileName.Replace("%CurrentDirectory%", TB.SystemMain.AppPath)
                'FileName = FileName.Replace("%UserDocumentDirectory%", TB.SystemMain.DocumentPath)
                FileName = FileName.Replace("%UserDocumentDirectory%", New IO.DirectoryInfo(TB.SystemMain.DocumentPath).Parent.FullName & "\")
                FileName = FileName.Replace("%UserAllDocumentDirectory%", New IO.DirectoryInfo(TB.SystemMain.DocumentPathAllUsers).Parent.FullName & "\")
                If FileName.Contains("%CPU%") Then
                    If TB.SystemMain.Is64BitHardware Then
                        FileName = FileName.Replace("%CPU%", "x64")
                    Else
                        FileName = FileName.Replace("%CPU%", "x86")
                    End If
                End If
                'FileName = FileName.Replace("%UserAllDocumentDirectory%", TB.SystemMain.DocumentPathAll)
                Dim regex As System.Text.RegularExpressions.Regex
                regex = New System.Text.RegularExpressions.Regex("%{1}(?'EnvName'\w{1,32})%{1}")
                Dim found As System.Text.RegularExpressions.MatchCollection = regex.Matches(FileName)
                For Each aMatch As System.Text.RegularExpressions.Match In found
                    'Console.WriteLine(aMatch.ToString)
                    'Console.WriteLine(Environment.GetEnvironmentVariable(aMatch.ToString.Replace("%", "")))
                    FileName = FileName.Replace(aMatch.ToString, Environment.GetEnvironmentVariable(aMatch.ToString.Replace("%", "")) & "\")
                Next

                Return FileName
            End Function




            <Runtime.InteropServices.DllImport("kernel32.dll")> _
            Private Function GetVolumeInformation(ByVal lpRootPathName As String, ByVal lpVolumeNameBuffer As System.Text.StringBuilder, ByVal nVolumeNameSize As Integer, ByRef lpVolumeSerialNumber As Integer, ByRef lpMaximumComponentLength As Integer, ByRef lpFileSystemFlags As Integer, ByVal lpFileSystemNameBuffer As System.Text.StringBuilder, ByVal nFileSystemNameSize As Integer) As Integer
            End Function

            Public Enum ShortCutEnum As Byte
                Desktop = 0
                SameDirectory = 1
                StartMenu = 2
            End Enum
            Private LastDriveSerial As Integer = -1


            Public Function GetDriveSerial(ByVal strRootPath As String) As Integer
                If LastDriveSerial > -1 Then Return LastDriveSerial
                Dim sbVolumeName As System.Text.StringBuilder = New System.Text.StringBuilder(256)
                Dim sbFileSystemName As System.Text.StringBuilder = New System.Text.StringBuilder(256)
                Dim nVolSerial As Integer = 0
                Dim nMaxCompLength As Integer = 0
                Dim nFSFlags As Integer = 0
                Dim bResult As Integer = 0
                Try
                    bResult = GetVolumeInformation(strRootPath, sbVolumeName, 256, nVolSerial, nMaxCompLength, nFSFlags, sbFileSystemName, 256)
                Catch ex As Exception
                End Try
                LastDriveSerial = nVolSerial
                If bResult <> 0 Then
                    Return nVolSerial
                Else
                    Return 0
                End If
            End Function

#Region "off topic"
            'Public NotInheritable Class Win32Wrapper
            'Public Const DRIVE_UNKNOWN = &H0
            'Public Const DRIVE_NO_ROOT_DIR = &H1
            'Public Const DRIVE_REMOVABLE = &H2
            'Public Const DRIVE_FIXED = &H3
            'Public Const DRIVE_REMOTE = &H4
            'Public Const DRIVE_CDROM = &H5
            'Public Const DRIVE_RAMDISK = &H6


            '<Runtime.InteropServices.DllImport("kernel32.dll")> _
            '    Public Shared Function GetDriveType(ByVal lpRootPathName As String) As System.UInt32
            'End Function






            'Public Shared Function GetVolumeName(ByVal strRootPath As String) As String
            '    Dim sbVolumeName As System.Text.StringBuilder = New System.Text.StringBuilder(256)
            '    Dim sbFileSystemName As System.Text.StringBuilder = New System.Text.StringBuilder(256)
            '    Dim nVolSerial As Integer = 0
            '    Dim nMaxCompLength As Integer = 0
            '    Dim nFSFlags As Integer = 0
            '    Dim bResult As Boolean = GetVolumeInformation(strRootPath, sbVolumeName, 256, nVolSerial, nMaxCompLength, nFSFlags, sbFileSystemName, 256)
            '    If bResult Then
            '        Return sbVolumeName.ToString
            '    Else
            '        Return ""
            '    End If
            'End Function



            'Public Shared Function GetInformations(ByVal strRootPath As String) As String()
            '    Dim sbVolumeName As System.Text.StringBuilder = New System.Text.StringBuilder(256)
            '    Dim sbFileSystemName As System.Text.StringBuilder = New System.Text.StringBuilder(256)
            '    Dim nVolSerial As Integer = 0
            '    Dim nMaxCompLength As Integer = 0
            '    Dim nFSFlags As Integer = 0
            '    Dim result As String() = {"", "", "", "", ""}
            '    Dim bResult As Boolean = GetVolumeInformation(strRootPath, sbVolumeName, 256, nVolSerial, nMaxCompLength, nFSFlags, sbFileSystemName, 256)
            '    If bResult Then
            '        result(0) = sbVolumeName.ToString
            '        result(1) = sbFileSystemName.ToString
            '        result(2) = nVolSerial.ToString
            '        result(3) = nMaxCompLength.ToString
            '        result(4) = nFSFlags.ToString
            '    End If
            '    Return result
            'End Function
            'End Class

#End Region

          





            <Runtime.InteropServices.DllImport("kernel32.dll", CharSet:=Runtime.InteropServices.CharSet.Auto, SetLastError:=True)> _
            Private Function GetShortPathName(<Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.LPTStr)> path As String, <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.LPTStr)> shortPath As System.Text.StringBuilder, shortPathLength As Integer) As Integer
            End Function



            Public Function CreateShortCutFile(ByVal fiSrcFile As IO.FileInfo, ByVal Title As String, ByVal Destination As ShortCutEnum, ByVal Description As String, ByVal ForAllUsersShared As Boolean) As IO.FileInfo
                Dim fiShortCut As IO.FileInfo = Nothing

                Dim objShell As Object
                Dim objShortcut As Object
                'Dim TargetPath As String
                'Dim Test As IWshRuntimeLibrary.IWshShortcut
                'Test.Arguments = ""
                'Test.Description = ""
                'Test.FullName = ""
                'Test.Hotkey = ""
                'Test.IconLocation = ""
                'Test.RelativePath = ""
                'Test.TargetPath = ""
                'Test.WindowStyle = 0
                'Test.WorkingDirectory = ""

                If fiSrcFile Is Nothing Then Return Nothing

                If Title Is Nothing Then
                    Title = fiSrcFile.Name.Substring(0, fiSrcFile.Name.Substring(0).Length - fiSrcFile.Extension.Length)
                End If

                Try

                    objShell = CreateObject("Wscript.Shell") ' CreateObject("wshom.ocx")
                    'objShell = CreateObject("Wscript.Network") ' CreateObject("wshom.ocx")
                    'initialize the object WshShell 
                    'objShell = New WshShell

                    If Destination = ShortCutEnum.SameDirectory Then
                        fiShortCut = New IO.FileInfo(fiSrcFile.FullName & ".lnk")
                    ElseIf Destination = ShortCutEnum.Desktop Then
                        Dim diDesktop As IO.DirectoryInfo
                        diDesktop = New IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))
                        If ForAllUsersShared Then
                            diDesktop = New IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory))
                        End If
                        fiShortCut = New IO.FileInfo(IO.Path.Combine(diDesktop.FullName, Title & ".lnk"))
                    Else
                        'Dim diStartMenu As New IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu))
                        Dim diStartMenu As New IO.DirectoryInfo(WindowsEnvironmentStartMenu)
                        If ForAllUsersShared Then
                            diStartMenu = New IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu))
                        End If
                        fiShortCut = New IO.FileInfo(IO.Path.Combine(diStartMenu.FullName, Title & ".lnk"))
                    End If

                    'initialize the object WshShortcut 
                    'the complete name of the .lnk file, include full path plus the .LNK file extension 
                    objShortcut = objShell.CreateShortcut(fiShortCut.FullName)

                    Dim sb As New System.Text.StringBuilder(255)

                    Call GetShortPathName(fiSrcFile.FullName, sb, sb.Capacity)
                    Dim DosFileName As String = sb.ToString
                    'TargetPath = objShell.ScriptFullName
                    'the file to be called by the .lnk file, ej. "c:\windows\calc.exe" 
                    objShortcut.TargetPath = DosFileName ' fiSrcFile.FullName
                    objShortcut.Description = Description
                    objShortcut.WorkingDirectory = DosFileName.Substring(0, DosFileName.LastIndexOf("\")) 'fiSrcFile.Directory.FullName ' TB.SystemMain.AppPath
                    'objShortcut.IconLocation = New Uri(System.Reflection.Assembly.GetExecutingAssembly.CodeBase).LocalPath ' New IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly.CodeBase).FullName ' W‰re die eigene DLL
                    objShortcut.IconLocation = DosFileName & ", 0" 'fiSrcFile.FullName.Replace("\", "\\").Replace("\\\\", "\\") & ", 0" 'New Uri(System.Reflection.Assembly.GetEntryAssembly.CodeBase).LocalPath



                    '(optional) := any command line supported by the file indicated in txtTarget.Text 
                    'objShortcut.Arguments = xxxx 

                    '(optional) : = a valid icon file : = To use the same icon of the target file, do not use the next line. 
                    'objShortcut.IconLocation = xxxx 

                    'Save the .lnk 
                    objShortcut.Save()
                    'Return fiShortCut
                Catch ex As Exception
                    fiShortCut = Nothing
                End Try

                Return fiShortCut
            End Function

            Public Function WindowsEnvironmentStartMenu() As String
                Dim Ret As String = "C:\ProgramData\Microsoft\Windows\Start Menu"
                Dim CommonStartMenuRegistry As Global.Microsoft.Win32.RegistryKey 'Key for shell folders
                CommonStartMenuRegistry = Global.Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders", False)

                Dim V As Object = CommonStartMenuRegistry.GetValue("Common Start Menu")
                If V IsNot Nothing AndAlso TypeOf V Is String Then
                    Ret = V.ToString
                End If

                Return Ret
            End Function

            ''' <summary>
            ''' Create all folders and sub folders. It works rekursive.
            ''' Returns True if all seems to be correct.
            ''' </summary>
            ''' <param name="Directory"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function CreateDirectories(ByVal Directory As IO.DirectoryInfo) As Boolean
                Try
                    If Directory.Parent IsNot Nothing Then
                        If IO.Directory.Exists(Directory.Parent.FullName) = False Then Call CreateDirectories(Directory.Parent)
                    End If
                    If IO.Directory.Exists(Directory.FullName) = False Then Directory.Create()
                Catch ex As Exception
                    Return False
                End Try
                Return True
            End Function


            ''' <summary>
            ''' Delete the whole Directory with all his content files.
            ''' If any error occours, the procedure will proceed with
            ''' the next file.
            ''' </summary>
            ''' <param name="Directory"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function DeleteDirectory(ByVal Directory As IO.DirectoryInfo) As Boolean
                Dim fi As IO.FileInfo
                Dim di As IO.DirectoryInfo
                Dim Gut As Boolean = True

                Directory.Refresh() 'Get the real state of the directory, not any cached
                For Each fi In Directory.GetFiles
                    Try
                        fi.Delete()
                    Catch ex As Exception
                        Gut = False
                    End Try
                Next

                For Each di In Directory.GetDirectories
                    Gut = Gut And DeleteDirectory(di)
                Next

                Try
                    Directory.Delete()
                Catch ex As Exception
                    Gut = False
                End Try

                Return Gut
            End Function


            ''' <summary>
            ''' Returns the MD5 hash string of any string
            ''' </summary>
            ''' <param name="Wort"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetMD5Hash(ByVal Wort As String) As String
                Return GetMD5Hash(Wort, System.Text.Encoding.GetEncoding("iso-8859-1"))
            End Function

            ''' <summary>
            ''' Returns the MD5 hash string of any file
            ''' </summary>
            ''' <param name="fInfo"></param>
            ''' <returns></returns>
            ''' <remarks>New (30.9.2009), with manually reading the file with filestream for compatibility with Mobile</remarks>
            Public Function GetMD5Hash(ByVal fInfo As IO.FileInfo) As String
                Dim Data() As Byte
                Dim BR As New IO.FileStream(fInfo.FullName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
                ReDim Data(BR.Length - 1)
                BR.Read(Data, 0, Data.Length)
                BR.Close()
                Return GetMD5Hash(Data)
            End Function

            ''' <summary>
            ''' Returns the MD5 hash string of any bytearray
            ''' </summary>
            ''' <param name="Data"></param>
            ''' <returns></returns>
            ''' <remarks>New (30.9.2009), with setting the encoding length for compatibility with Mobile</remarks>
            Public Function GetMD5Hash(ByVal Data() As Byte) As String
                Dim Encoding As System.Text.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1")
                Dim HashData As Byte() = New System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(Data)
                Return Encoding.GetString(HashData, 0, HashData.Length)
            End Function

            ''' <summary>
            ''' Returns the MD5 hash string of any string by using a specified encoder.
            ''' Default = System.Text.Encoding.GetEncoding("iso-8859-1")
            ''' </summary>
            ''' <param name="Wort"></param>
            ''' <param name="Encoding"></param>
            ''' <returns></returns>
            ''' <remarks>New (30.9.2009), with setting the encoding length for compatibility with Mobile</remarks>
            Public Function GetMD5Hash(ByVal Wort As String, ByVal Encoding As System.Text.Encoding) As String
                Dim StringData As Byte() = Encoding.GetBytes(Wort)
                Dim HashData As Byte() = New System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(StringData)

                Return Encoding.GetString(HashData, 0, HashData.Length)
            End Function



            ''' <summary>
            ''' Returns the Bytearray of a file
            ''' </summary>
            ''' <param name="FileName"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function LoadFileAsByteArray(ByVal FileName As String) As Byte()
                Dim fs As IO.FileStream = New IO.FileStream(FileName, IO.FileMode.Open)
                Dim reader As IO.BinaryReader = New IO.BinaryReader(fs)
                Dim Data As Byte() = reader.ReadBytes(fs.Length)
                reader.Close()
                fs.Close()
                Return Data
            End Function



            ''' <summary>
            ''' load a string from a file
            ''' </summary>
            Public Function LoadStringFromFile(ByVal DateiName As String, ByVal Enc As System.Text.Encoding) As String
                Dim Data As String = ""
                Dim srIn As IO.StreamReader

                If IO.File.Exists(DateiName) = False Then Return Data
                srIn = New IO.StreamReader(DateiName, Enc)
                Data = srIn.ReadToEnd()
                srIn.Close()


                Return Data
            End Function



            ''' <summary>
            ''' load a string from a file
            ''' </summary>
            Public Function LoadStringFromFile(ByVal DateiName As String) As String
                Return LoadStringFromFile(DateiName, Global.System.Text.Encoding.UTF8)
            End Function

            ''' <summary>
            ''' save a string in a file.
            ''' If all seems to be ok the function returns True.
            ''' </summary>
            Public Function SaveFile(ByVal Dateiname As String, ByVal NewData As String, Optional ByVal OverWrite As Boolean = True) As Boolean
                Return SaveFile(Dateiname, NewData, OverWrite, Global.System.Text.Encoding.Unicode)
            End Function

            ''' <summary>
            ''' save a string in a file.
            ''' If all seems to be ok the function returns True.
            ''' </summary>
            Public Function SaveFile(ByVal Dateiname As String, ByVal NewData As String, ByVal OverWrite As Boolean, ByVal Enc As Global.System.Text.Encoding) As Boolean
                If Dateiname = "" Then Return False
                If OverWrite = False Then
                    If IO.File.Exists(Dateiname) = True Then
                        If MsgBox("File already exists. Overwrite file?" & vbNewLine & Dateiname, MsgBoxStyle.YesNo) = MsgBoxResult.No Then Return False
                    End If
                End If

                Dim srOut As IO.StreamWriter

                Try
                    srOut = New IO.StreamWriter(Dateiname, False, Enc)
                    srOut.Write(NewData)
                    srOut.Close()
                Catch ex As Exception
                    Return False
                End Try

                Return True

            End Function

            ''' <summary>
            ''' save the object to a file by using different ways.
            ''' </summary>
            Public Function SaveFile(ByVal DateiName As String, ByRef NewData As Object, Optional ByVal OverWrite As Boolean = True) As Boolean
                Dim ByteData() As Byte
                Dim strData As String = ""
                Dim ptr As IntPtr
                Dim ObjectSize As Integer = -1


                If DateiName = "" Then Return False
                If OverWrite = False Then
                    If IO.File.Exists(DateiName) = True Then
                        If MsgBox("File already exists. Overwrite file?" & vbNewLine & DateiName, MsgBoxStyle.YesNo) = MsgBoxResult.No Then Return False
                    End If
                End If

                If NewData Is Nothing Then
                    IO.File.Delete(DateiName)
                Else

                    On Error Resume Next
                    If ObjectSize = -1 Then ObjectSize = Len(NewData)
                    If ObjectSize = -1 Then ObjectSize = Global.System.Runtime.InteropServices.Marshal.SizeOf(NewData)
                    If ObjectSize = -1 Then ObjectSize = Global.System.Runtime.InteropServices.Marshal.SizeOf(NewData(0)) * CType(NewData, Array).Length 'Array, 'UBound(NewData), New: 21.6. 2010, clsDigiCamNet
                    If ObjectSize = -1 Then ObjectSize = CType(NewData, Array).Length 'UBound(NewData)
                    If ObjectSize = -1 Then 'Memory Stream
                        Dim ArrData() As Byte
                        Dim S As Global.System.IO.UnmanagedMemoryStream
                        S = CType(NewData, IO.UnmanagedMemoryStream)
                        ObjectSize = CInt(S.Length) 'Fehlerquelle
                        ReDim ArrData(ObjectSize - 1)
                        S.Read(ArrData, 0, ObjectSize)
                        NewData = Nothing
                        NewData = ArrData
                    End If
                    On Error GoTo 0


                    If ObjectSize > -1 Then
                        'Dim gc As System.Runtime.InteropServices.GCHandle = System.Runtime.InteropServices.GCHandle.Alloc(NewData, Runtime.InteropServices.GCHandleType.Pinned)
                        'ptr = gc.AddrOfPinnedObject.ToInt32
                        ''ptr = Marshal.AllocHGlobal(Len(NewData))
                        ReDim ByteData(ObjectSize - 1)
                        'Marshal.Copy(ptr, ByteData, 0, ByteData.Length)
                        'gc.Free()
                        ptr = Global.System.Runtime.InteropServices.Marshal.AllocCoTaskMem(ObjectSize)
                        Global.System.Runtime.InteropServices.Marshal.Copy(NewData, 0, ptr, ObjectSize)
                        Global.System.Runtime.InteropServices.Marshal.Copy(ptr, ByteData, 0, ObjectSize)

                        If IO.File.Exists(DateiName) = True Then IO.File.Delete(DateiName)
                        Dim fs As IO.FileStream = New IO.FileStream(DateiName, IO.FileMode.Create)
                        Dim w As IO.BinaryWriter = New IO.BinaryWriter(fs)
                        w.Write(ByteData)
                        w.Close()
                        fs.Close()

                    End If
                End If
            End Function


            ''' <summary>
            ''' save the object data to a file by using serialisation
            ''' </summary>
            Public Function SaveFile2(ByVal DateiName As String, ByRef Data As Object, Optional ByVal OverWrite As Boolean = True) As Boolean
                If DateiName = "" Then Return False
                If OverWrite = False Then
                    If IO.File.Exists(DateiName) = True Then
                        If MsgBox("File already exists. Overwrite file?" & vbNewLine & DateiName, MsgBoxStyle.YesNo) = MsgBoxResult.No Then Return False
                    End If
                End If

                If Data Is Nothing Then
                    Kill(DateiName)
                Else
                    Dim fs As IO.FileStream = New IO.FileStream(DateiName, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
                    Dim BF As New Runtime.Serialization.Formatters.Binary.BinaryFormatter

                    BF.Serialize(fs, Data)
                    fs.Close()


                    '*** Visual Basic 6.0 ***
                    'Dim N As Integer

                    'N = FreeFile()
                    'FileOpen(N, DateiName, OpenMode.Output)
                    'Print(N, Data)
                    'FileClose(N)
                End If

                Return True
            End Function


            ''' <summary>
            ''' check if the file is in use by another program or if it is locked.
            ''' </summary>
            Public Function FileInUse(ByVal sFilename As String) As Boolean

                Try
                    Dim FS As New Global.System.IO.FileStream(sFilename, IO.FileMode.Open, IO.FileAccess.ReadWrite, IO.FileShare.None)
                    FS.Close()
                    Return False
                Catch ex As Exception
                    Return True
                End Try

            End Function



            ''' <summary>
            ''' returns a file name with full path included. Automatically applies the folder if neccessary and the extension.
            ''' </summary>
            Public Function ConvertToFileName(ByVal SettingName_or_SettingFileName As String, ByVal Folder As String, Optional ByVal Extension As String = ".ini") As String
                Dim D As String
                If Folder Is Nothing Then Folder = Environment.CurrentDirectory

                'Dateierweiterung anf¸gen bei Bedarf
                D = SettingName_or_SettingFileName
                If Global.Microsoft.VisualBasic.Strings.Right(Global.Microsoft.VisualBasic.Strings.LCase(D), 1 + Extension.Length) <> "." & Global.Microsoft.VisualBasic.Strings.LCase(Extension) Then D = D & "." & Extension


                'Pr¸fen ob die Datei bereits einen Pfad hat oder nicht.
                Dim Datei As New IO.FileInfo(IO.Path.Combine(Folder, D)) 'Auf Pfad pr¸fen

                Return Datei.FullName
            End Function
        End Module
    End Namespace



    Namespace CopyMemory
        Public Module modCopyMemory
#Region "CopyMemory Functions"

            Private Function StringToStruct(ByVal InString As String, ByVal InType As Type) As Object

                Dim objEncode As Global.System.Text.Encoding
                Dim bytes() As Byte
                Dim oOutput As Object

                objEncode = New Global.System.Text.ASCIIEncoding
                bytes = objEncode.GetBytes(InString)

                oOutput = ByteToStruct(bytes, InType)

                Return oOutput

            End Function


            Private Function StructToString(ByVal Struct As Object) As String

                Dim objEncode As Global.System.Text.Encoding
                Dim bytes() As Byte
                Dim sOutput As String

                bytes = StructToByte(Struct)

                objEncode = New Global.System.Text.ASCIIEncoding
                sOutput = objEncode.GetString(bytes)
                sOutput = sOutput.Replace(Chr(0), Chr(32))

                Return sOutput

            End Function


            Private Function StructToByte(ByVal Struct As Object) As Byte()

                Dim iStructSize As Integer = Global.System.Runtime.InteropServices.Marshal.SizeOf(Struct)

                Dim buffer As IntPtr = Global.System.Runtime.InteropServices.Marshal.AllocHGlobal(iStructSize)

                Global.System.Runtime.InteropServices.Marshal.StructureToPtr(Struct, buffer, False)

                Dim btData(iStructSize - 1) As Byte

                Global.System.Runtime.InteropServices.Marshal.Copy(buffer, btData, 0, iStructSize)
                Global.System.Runtime.InteropServices.Marshal.FreeHGlobal(buffer)

                Return btData

            End Function

            Private Function ByteToStruct(ByVal btData As Byte(), ByVal StructureType As Type, Optional ByVal ibyteIndex As Integer = 0) As Object

                Dim iStructSize As Integer = Global.System.Runtime.InteropServices.Marshal.SizeOf(StructureType)

                If (iStructSize > btData.Length) Then Return Nothing

                Dim buffer As IntPtr = Global.System.Runtime.InteropServices.Marshal.AllocHGlobal(iStructSize)

                Global.System.Runtime.InteropServices.Marshal.Copy(btData, ibyteIndex, buffer, iStructSize)

                Dim retStruct As Object = Global.System.Runtime.InteropServices.Marshal.PtrToStructure(buffer, StructureType)

                Global.System.Runtime.InteropServices.Marshal.FreeHGlobal(buffer)

                Return retStruct

            End Function
#End Region
        End Module
    End Namespace

    Namespace Web
        Public Module modWeb

            <System.Runtime.CompilerServices.Extension> _
            Public Function AddArgument(builder As UriBuilder, key As String, value As String) As UriBuilder
                Diagnostics.Contracts.Contract.Requires(builder IsNot Nothing)
                Diagnostics.Contracts.Contract.Requires(key IsNot Nothing)
                Diagnostics.Contracts.Contract.Requires(value IsNot Nothing)

                Dim query = builder.Query

                If query.Length > 0 Then
                    query = query.Substring(1) & "&"
                End If

                query &= Uri.EscapeDataString(key) & "=" & Uri.EscapeDataString(value)

                builder.Query = query

                Return builder
            End Function


            Public Function UrlWithoutQuery(ByVal Url As String) As String
                Dim uri As New Uri(Url)
                Return UrlWithoutQuery(uri)
            End Function

            Public Function UrlWithoutQuery(ByVal Url As Uri) As String
                Dim path As String = String.Format("{0}{1}{2}{3}", Url.Scheme, Uri.SchemeDelimiter, Url.Authority, Url.AbsolutePath)
                'If Url.IsDefaultPort = False Then
                '    path = String.Format("{0}{1}{2}{3}", Url.Scheme, Uri.SchemeDelimiter, Url.Authority, Url.AbsolutePath)
                'End If
                Return path
            End Function


            Private LastMacAddress As String = Nothing
            Private LastProxies As List(Of String) = Nothing


            ''' <summary>
            ''' Get all open listener ports on this computer
            ''' </summary>
            ''' <returns>80;1024;8080;49000;49001;</returns>
            Public Function GetOpenPorts() As String
                'Dim PortStartIndex As Integer = 1000
                'Dim PortEndIndex As Integer = 2000
                Dim properties As Net.NetworkInformation.IPGlobalProperties = Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties()
                Dim tcpEndPoints As Net.IPEndPoint() = properties.GetActiveTcpListeners()

                Dim usedPorts As List(Of Integer) = tcpEndPoints.Select(Function(p) p.Port).ToList
                Dim unusedPort As Integer = 0

                Dim W As String = ""
                For Each i As Integer In usedPorts
                    If (";" & W).Contains(";" & i.ToString("0") & ";") = False Then
                        W &= i.ToString("0") & ";"
                    End If
                Next
                Return W
            End Function


            '''' <summary>
            '''' Resolvers the specified DNS name.
            '''' </summary>
            '''' <param name="DnsName">Name of the DNS.</param>
            '''' <returns></returns>
            'Public Function DnsToIpResolver(ByVal DnsName As String) As String()
            '    'Dim IHE As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(DnsName)
            '    Dim IHE As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(DnsName)
            '    Dim ret As String() = New String(IHE.AddressList.Length - 1) {}
            '    For i As Integer = 0 To IHE.AddressList.Length - 1
            '        ret(i) = IHE.AddressList(i).ToString()
            '    Next
            '    Return ret
            'End Function




            ''' <summary>
            ''' Get a list of proxie servers. Download the list from Proxy4free.com
            ''' </summary>
            ''' <returns>a list of proxie data</returns>
            ''' <remarks></remarks>
            Public Function GetProxyNames() As List(Of String)
                If LastProxies IsNot Nothing AndAlso LastProxies.Count > 0 Then Return LastProxies
                Dim Pattern As String = "<a (.+?)</a>" '"<td>(.+?)</td>"
                Dim xregex As New System.Text.RegularExpressions.Regex(Pattern)
                Dim proxies As New List(Of String)
                Dim client As New Net.WebClient()
                Dim WebContent As String

                WebContent = client.DownloadString("http://www.proxy4free.com/list/webproxy_rating1.html") 'http://www.proxy4free.com/page1.html")

                ''Clean the list
                'For Each W As String In WebContent
                For Each Matchx As System.Text.RegularExpressions.Match In xregex.Matches(WebContent)
                    If Matchx.Groups(0).Value.Contains(".") And _
                    Matchx.Groups(0).Value.Contains("http") And _
                    Matchx.Groups(0).Value.Contains("href") Then
                        Dim Res As String = Matchx.Groups(1).Value
                        Res = TB.String.ExtractStringFromString(Res, 0, "href=" & Chr(34), Chr(34))
                        Res = Res.TrimEnd("/".ToCharArray)
                        proxies.Add(Res)
                    End If
                Next
                'Next

                LastProxies = proxies
                Return proxies
            End Function

            Public Function GetProxyIps() As List(Of String)
                Dim LIp As New List(Of String)

                For Each W As String In GetProxyNames()
                    Try

                        W = IpAddress(W)
                        If String.IsNullOrEmpty(W) = False Then LIp.Add(W)
                    Catch ex As Exception
                    End Try

                Next
                Return LIp
            End Function




            ''' <summary>
            ''' Returns the first MAC address in the system
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetMacAddress() As String
                Dim i As Integer
                If LastMacAddress IsNot Nothing Then Return LastMacAddress
                Dim Res As String

                Dim ComputerProperties As Net.NetworkInformation.IPGlobalProperties = Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties()
                Dim NInterface As Net.NetworkInformation.NetworkInterface() = Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                'Console.WriteLine(Interface information for {0}.{1} , computerProperties.HostName, computerProperties.DomainName)
                If NInterface Is Nothing OrElse NInterface.Length <= 0 Then
                    'Console.WriteLine("No network interfaces found.")
                    Return ""
                End If

                'Console.WriteLine("Number of interfaces .................... : {0}", nics.Length)
                For Each Adapter As Net.NetworkInformation.NetworkInterface In NInterface
                    Dim Properties As Net.NetworkInformation.IPInterfaceProperties = Adapter.GetIPProperties()
                    ' .GetIPInterfaceProperties();
                    'Console.WriteLine()
                    'Console.WriteLine(adapter.Description)
                    '  Console.WriteLine([String].Empty.PadLeft(adapter.Description.Length, =c))
                    '  Console.WriteLine( Interface type .......................... : {0}, adapter.NetworkInterfaceType)
                    '  Console.Write( Physical address ........................ : )
                    Dim MacAddress As Net.NetworkInformation.PhysicalAddress = Adapter.GetPhysicalAddress()
                    Dim B As Byte() = MacAddress.GetAddressBytes()
                    Res = ""
                    'Res = System.Text.Encoding.UTF8.GetString(B)
                    For i = 0 To B.Length - 1
                        Res &= B(i).ToString("X") 'In Hex umwandeln
                    Next
                    If String.IsNullOrEmpty(Res.Trim) = False Then
                        LastMacAddress = Res
                        Return Res
                    End If
                    'For i As Integer = 0 To B.Length - 1
                    '    ' Display the physical address in hexadecimal.
                    '    'Console.Write({0}, bytes(i).ToString(X2))
                    '    ' Insert a hyphen after each byte, unless we are at the end of the
                    '    ' address.
                    '    If i <> B.Length - 1 Then
                    '        'Console.Write(-)
                    '    End If
                    'Next
                    'Console.WriteLine()
                Next
                Return ""
            End Function


            ''' <summary>
            ''' Liefert eine Url zur¸ck im Html Format
            ''' </summary>
            ''' <param name="s"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Function HtmlUrlEncode(ByVal s As String) As String
                Dim sb As New System.Text.StringBuilder(s)
                'Return sb.Replace("&", "&amp;").Replace("<", "&lt;").Replace("""", "&quot;").Replace("'", "&#39;").ToString
                'neu mit ˆ,‰,¸, ﬂ
                Return sb.Replace("&", "&amp;").Replace("<", "&lt;").Replace("""", "&quot;").Replace("'", "&#39;").Replace(" ", "%20").Replace("‰", "&auml;").Replace("¸", "&uuml;").Replace("ˆ", "&ouml;").Replace("ﬂ", "&szlig;").Replace("ƒ", "&Auml;").Replace("‹", "&Uuml;").Replace("÷", "&Ouml;").ToString
            End Function

            ''' <summary>
            ''' Liefert eine Url zur¸ck ohne Html Format
            ''' </summary>
            ''' <param name="s"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Function HtmlUrlDecode(ByVal s As String) As String
                Dim sb As New System.Text.StringBuilder(s)
                Return sb.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&quot;", """").Replace("&#39;", "'").Replace("%20", " ").Replace("&auml;", "‰").Replace("&uuml;", "¸").Replace("&ouml;", "ˆ").Replace("&szlig;", "ﬂ").Replace("&Auml;", "ƒ").Replace("&Uuml;", "‹").Replace("&Ouml;", "÷").ToString
            End Function



            Public Function ReadBrowser() As String
                Return TB.Web.ReadWebSite("http://www.fscan.com/static/browser.aspx")
            End Function

            ''' <summary>
            ''' Read the IP of from this computer connection from the internet. Reads the IP from tb-electronics server. 
            ''' Can store the External IP with the computer name on the server when StoreIpOnServer flag is on true
            ''' </summary>
            Public Function ReadExternalIp(ByVal StoreIpOnServer As Boolean, ByVal QueryText As String) As String
                If StoreIpOnServer = True Then
                    Return TB.Web.ReadWebSite("http://www.tb-electronics.ch/static/site/ip.aspx?autorefresh=1&password=" & My.Computer.Name & "&q=" & QueryText)
                Else
                    Return TB.Web.ReadWebSite("http://www.tb-electronics.ch/static/site/ip.aspx?autorefresh=0&password=" & My.Computer.Name & "&q=" & QueryText)
                End If
            End Function

            ''' <summary>
            ''' Read the IP of from this computer connection from the internet. Reads the IP from tb-electronics server
            ''' </summary>
            Public Function ReadExternalIp() As String
                Return ReadExternalIp(False, "")
                'Return TB.Web.ReadWebSite("http://www.fscan.com/static/ip.aspx")
            End Function


            Public Function ReadFileFromFtp(ByVal Url As String, ByVal UserName As String, ByVal Password As String, ByVal enc As System.Text.Encoding) As String
                Dim Data() As Byte
                Dim Res As String

                Data = ReadFileFromFtp(Url, UserName, Password)
                If Data Is Nothing Then Return ""
                Res = enc.GetString(Data)
                'Res = System.Text.Encoding.UTF8.GetString(Data)
                'If Res.Contains(Chr(0)) Then Res = Res.Substring(0, Res.IndexOf(Chr(0)))
                Return Res
            End Function


            Public Function ReadFileFromFtp(ByVal Url As String, ByVal UserName As String, ByVal Password As String) As Byte()
                'Dim Url As String = "ftp://www.goldengel.ch/Version.txt"
                Dim SourceStream As System.IO.Stream = Nothing
                Dim Response As Net.WebResponse = Nothing
                Dim MyFtpWebRequest As Net.FtpWebRequest

                Try

                    'Create the request and credentials
                    MyFtpWebRequest = Net.WebRequest.Create(New Uri(Url)) '"ftp://myftpserver/dir1/dir2/test.htmf"
                    MyFtpWebRequest.Credentials = New Net.NetworkCredential(UserName, Password)

                    'Get response from stream
                    Response = MyFtpWebRequest.GetResponse()

                    SourceStream = Response.GetResponseStream()

                    Dim Buffer(4096) As Byte, BlockSize As Integer
                    'Memory stream to store data   
                    Dim TempStream As New IO.MemoryStream
                    Do
                        BlockSize = SourceStream.Read(Buffer, 0, 4096)
                        If BlockSize > 0 Then TempStream.Write(Buffer, 0, BlockSize)
                    Loop While BlockSize > 0

                    'return the document binary data   
                    Return TempStream.ToArray()

                Catch ex As Exception
                    System.Diagnostics.Debug.WriteLine("modTb:modWeb:ReadFileFromFtp:" & ex.Message & " url=" & Url)
                Finally
                    Try
                        If SourceStream IsNot Nothing Then SourceStream.Close()
                    Catch ex As Exception
                    End Try
                    Try
                        If Response IsNot Nothing Then Response.Close()
                    Catch ex As Exception
                    End Try
                End Try

                Return Nothing
            End Function

            Public Function ReadWebSite(ByVal Url As String) As String
                Return ReadWebSite(Url, System.Text.Encoding.UTF8)
            End Function

            Public Function ReadWebSite(ByVal Url As String, ByVal enc As System.Text.Encoding) As String
                Dim Data() As Byte
                Dim Res As String
                '("iso-8859-1", "utf-8", "utf-16", "us-ascii", "IBM437", "ibm850")

                Data = ReadFileFromWebSite(Url)
                If Data Is Nothing Then Return ""
                Res = enc.GetString(Data) ' System.Text.Encoding.UTF8.GetString(Data)
                If Res.Contains(Chr(0)) Then Res = Res.Substring(0, Res.IndexOf(Chr(0)))
                Return Res
            End Function

            Public Function ReadFileFromWebSite(ByVal FileName As String, ByVal Content As String) As Byte()


                Dim Str As String = FileName
                If FileName.Contains("&") Or FileName.Contains("?") Then 'Its a weblink
                    'filename has bad characters. because it could come from 
                    'the ReadUrl function, we will no catch the queries and write
                    'these data into the file.
                    If FileName.Contains("\") Then Str = FileName.Substring(0, FileName.LastIndexOf("\"))
                    Str = Str.TrimEnd("\") & "\"

                    '"D:\Temp\TB-RemoteDesktop\ReadFileFromHardDisk.txt"
                    FileName = IO.Path.Combine(Str, "ReadFileFromHardDisk.txt")
                End If



                'Dim XmlWriter As New Xml.XmlTextWriter(FileName, System.Text.Encoding.UTF8)

                'XmlWriter.WriteStartDocument()
                'XmlWriter.WriteStartElement("MAIN")
                'For Each W As String In Content.Split("&")
                '    If String.IsNullOrEmpty(W) = False Then
                '        If W.Contains("=") Then
                '            XmlWriter.WriteStartElement(W.Split("=")(0))
                '            XmlWriter.WriteStartAttribute(W.Split("=")(1))
                '            XmlWriter.WriteEndElement()
                '        Else
                '            XmlWriter.WriteStartAttribute(W)
                '        End If
                '    End If
                'Next
                'XmlWriter.WriteEndElement()
                'XmlWriter.WriteEndDocument()
                'XmlWriter.Flush()
                'XmlWriter.Close()


                'Dim sb As New System.Text.StringBuilder
                'Dim XmlReader As New Xml.XmlTextReader(FileName)
                'Do
                '    If XmlReader.Read = False Then Exit Do
                '    Try

                '        sb.AppendLine(XmlReader.Name & "=" & XmlReader.Value)
                '    Catch ex As Exception

                '        Stop
                '    End Try
                'Loop

                Dim OldContent As String = ""
                If IO.File.Exists(FileName) = True Then OldContent = My.Computer.FileSystem.ReadAllText(FileName, System.Text.Encoding.Unicode)
                My.Computer.FileSystem.WriteAllText(FileName, Content, False, System.Text.Encoding.Unicode)

                Return System.Text.Encoding.ASCII.GetBytes(OldContent & Content)
                'Return System.Text.Encoding.ASCII.GetBytes(sb.ToString)
            End Function


            Public Function ReadFileWithoutLock(ByVal FileName As String, Optional ByVal BufferSize As Integer = 4096) As Byte()
                Dim Buffer(BufferSize) As Byte
                Dim BlockSize As Integer
                Dim FileStream As New IO.FileStream(FileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)

                'Memory stream to store data   
                Dim TempStream As New IO.MemoryStream
                Do
                    BlockSize = FileStream.Read(Buffer, 0, BufferSize)
                    If BlockSize > 0 Then TempStream.Write(Buffer, 0, BlockSize)
                Loop While BlockSize > 0

                'return the document binary data   
                Return TempStream.ToArray()
            End Function


            Public Function ReadFileFromWebSite(ByVal URL As String) As Byte()


                If URL.StartsWith(Uri.UriSchemeFile) OrElse (URL.Contains("/") = False AndAlso URL.Contains("\") = True) Then
                    'File from local HDD
                    Dim FileName As String
                    FileName = New Uri(URL).LocalPath
                    Return ReadFileFromWebSite(FileName, URL)
                End If

                Dim Req As Net.HttpWebRequest
                Dim SourceStream As System.IO.Stream = Nothing
                Dim Response As Net.HttpWebResponse = Nothing

                Try
                    'Ignore bad https certificates - expired, untrusted, bad name, etc.   
                    'Net.ServicePointManager.CertificatePolicy = New Net.MyAcceptCertificatePolicy

                    'create a web request to the URL   
                    Req = Net.HttpWebRequest.Create(URL)

                    'Grrrrr.... HttpWebRequest does not know rfc   
                    'you cannot use http://username:password@server:port/uri   
                    'Set username and password if required   
                    'If Len(UserName) > 0 Then
                    '    Req.Credentials = New Net.NetworkCredential("root", "mmanasek")
                    'End If

                    'get a response from web site   
                    Response = Req.GetResponse()

                    'Source stream with requested document   
                    SourceStream = Response.GetResponseStream()

                    'SourceStream has no ReadAll, so we must read data block-by-block   
                    'Temporary Buffer and block size   
                    Dim Buffer(4096) As Byte
                    Dim BlockSize As Integer

                    'Memory stream to store data   
                    Dim TempStream As New IO.MemoryStream
                    Do
                        BlockSize = SourceStream.Read(Buffer, 0, 4096)
                        If BlockSize > 0 Then TempStream.Write(Buffer, 0, BlockSize)
                    Loop While BlockSize > 0

                    'return the document binary data   
                    Return TempStream.ToArray()
                Catch ex As Exception
                    System.Diagnostics.Debug.WriteLine("modTb:modWeb:ReadFileFromWebSite:" & ex.Message & " url=" & URL)
                Finally
                    'grrr... Using is great, but the command is not in VB.Net   
                    Try
                        If SourceStream IsNot Nothing Then SourceStream.Close()
                    Catch ex As Exception
                    End Try
                    Try
                        If Response IsNot Nothing Then Response.Close()
                    Catch ex As Exception

                    End Try
                End Try

                Return Nothing
            End Function




            Public Function ReadFileFromWebSite_Old(ByVal Url As String) As Byte()
                Dim Data As New List(Of Char)

                Try
                    Dim BufferSize As Integer = 1024
                    Dim Bytes() As Char
                    Dim MyWebRequest As Net.WebRequest = Net.WebRequest.Create(Url)
                    Dim MyWebResponse As Net.WebResponse = MyWebRequest.GetResponse
                    Dim MyStream As New IO.StreamReader(MyWebResponse.GetResponseStream, System.Text.Encoding.UTF8)
                    Do
                        If MyStream.EndOfStream = True Then Exit Do
                        ReDim Bytes(Global.System.Math.Min(MyStream.Peek, BufferSize - 1))
                        MyStream.Read(Bytes, 0, Bytes.Length)
                        Data.AddRange(Bytes) 'System.Text.Encoding.UTF8.GetString()
                    Loop
                    MyWebRequest = Nothing
                    MyStream.Dispose()
                    MyStream = Nothing
                Catch ex As Exception
                    System.Diagnostics.Debug.WriteLine("error in modTB:ReadWebSite: " & ex.Message & " url=" & Url)
                End Try

                Dim enc As System.Text.Encoding = System.Text.Encoding.Default
                Return enc.GetBytes(Data.ToArray)
            End Function

            ''' <summary>
            ''' returns the Ip adress of an host. Like www.goldengel.ch return 69.1.293.49
            ''' </summary>
            Public Function IpAddress(ByVal HostName As String) As String
                Try
                    Dim LIps As New List(Of Net.IPAddress)
                    Dim Res As String = ""

                    HostName = HostName.ToLower
                    HostName = HostName.Replace("http://", "")
                    HostName = HostName.Replace("https://", "")
                    If HostName.Contains("/") Then
                        HostName = HostName.Substring(0, HostName.IndexOf("/", 1)) 'www.goldengel.ch/temp/index.aspx > www.goldengel.ch
                    End If

                    LIps.AddRange(Net.Dns.GetHostAddresses(HostName))
                    For i As Integer = 0 To LIps.Count - 1
                        If LIps(i).AddressFamily = Net.Sockets.AddressFamily.InterNetwork Then
                            If LIps(i).ToString.Contains(":") = False Then
                                Res = LIps(i).ToString()
                                Exit For
                            End If
                        End If
                    Next
                    Return Res
                Catch ex As Exception
                    Return "127.0.0.1"
                End Try
            End Function

            ''' <summary>
            ''' returns the Ip adress of the local computer
            ''' </summary>
            ''' <returns>127.0.0.1 or local ip in network like 192.168.1.2</returns>
            ''' <remarks></remarks>
            Public Function IpAddress() As String
                Return IpAddress(Net.Dns.GetHostName)
            End Function

        End Module
    End Namespace



    Namespace Math
        Public Module modMath

            ''' <summary>
            ''' Create Bits of a Byte as string
            ''' </summary>
            ''' <param name="value">value as byte like 255</param>
            ''' <returns>the value as Bit representation like 11111111</returns>
            Public Function ByteToBitString(ByVal value As Byte) As String
                Dim S As String = System.Convert.ToString(value, 2)
                Dim Res As String = "00000000".Substring(S.Length)
                Res &= S

                Return Res
            End Function


            ''' <summary>
            ''' return Byte from string
            ''' </summary>
            ''' <param name="value">for example 11111111</param>
            ''' <returns>for example 255</returns>
            Public Function BitStringToByte(ByVal value As String) As Byte
                'Throw New System.Exception("BitStringToByte not implemented")
                Dim L As New List(Of Byte)
                For i As Integer = 0 To value.Length - 1 Step 8
                    If L.Count > 0 Then Exit For
                    Dim B As Byte = System.Convert.ToByte(value.Substring(i, 8))
                    L.Add(B)
                Next
                If L.Count = 0 Then
                    L.Add(0)
                End If
                Return L(0)
            End Function

            ''' <summary>
            ''' Convert any Hex string into byte string
            ''' </summary>
            ''' <param name="HexValue">example: FF010203AE</param>
            ''' <returns>92109</returns>
            Public Function HexToByte(ByVal HexValue As String) As Byte()
                If String.IsNullOrEmpty(HexValue) = True Then HexValue = ""
                If HexValue.Contains("-") Then
                    Return HexToByte(HexValue, "-")
                ElseIf HexValue.Contains(",") Then
                    Return HexToByte(HexValue, ",")
                Else
                    Return HexToByte(HexValue, "")
                End If

            End Function


            Public Function ValueToHex(ByVal value As Long, ByVal Separator As String) As String
                Dim intBytes As Byte() = BitConverter.GetBytes(value)
                If BitConverter.IsLittleEndian = True Then
                    Array.Reverse(intBytes)
                End If
                Return ByteToHex(intBytes, Separator)
            End Function

            Public Function ValueToHex(ByVal value As UInt16, ByVal Separator As String) As String
                Dim intBytes As Byte() = BitConverter.GetBytes(value)
                If BitConverter.IsLittleEndian = True Then
                    Array.Reverse(intBytes)
                End If
                Return ByteToHex(intBytes, Separator)
            End Function


            Public Function ValueToHex(ByVal value As Int16, ByVal Separator As String) As String
                Dim intBytes As Byte() = BitConverter.GetBytes(value)
                If BitConverter.IsLittleEndian = True Then
                    Array.Reverse(intBytes)
                End If
                Return ByteToHex(intBytes, Separator)
            End Function

            Public Function ValueToHex(ByVal value As Int32, ByVal Separator As String) As String
                Dim intBytes As Byte() = BitConverter.GetBytes(value)
                If BitConverter.IsLittleEndian = True Then
                    Array.Reverse(intBytes)
                End If
                Return ByteToHex(intBytes, Separator)
            End Function

            ''' <summary>
            ''' Convert any Hex string into byte string
            ''' </summary>
            ''' <param name="HexValue">example: FF010203AE</param>
            ''' <param name="Separator">a separator between chars. if none is set, every HEX value will be assumed as 2 chars item</param>
            ''' <returns>92109</returns>
            ''' <remarks>Hexvalue needs to be a character count which could be divided by 2</remarks>
            Public Function HexToByte(ByVal HexValue As String, ByVal Separator As String) As Byte()
                Dim L As New List(Of Byte)

                If String.IsNullOrEmpty(HexValue) = True Then HexValue = ""
                If HexValue.Length = 1 Then HexValue = "0" & HexValue 'if user did not submit a zero before the value, we will fix that. Only works for single Hex values.

                If String.IsNullOrEmpty(Separator) = True Then
                    For i As Integer = 0 To HexValue.Length - 1 Step 2
                        Dim iByte As Integer = System.Convert.ToInt32(HexValue.Substring(i, 2), 16)
                        Dim bByte As Byte = System.Convert.ToByte(iByte)
                        L.Add(bByte)
                    Next
                Else
                    If HexValue.EndsWith(Separator) Then HexValue = HexValue.TrimEnd(Separator)
                    For Each W As String In HexValue.Split(Separator)
                        Dim iByte As Integer = System.Convert.ToInt32(W, 16)
                        Dim bByte As Byte = System.Convert.ToByte(iByte)
                        L.Add(bByte)
                    Next
                End If


                Return L.ToArray
            End Function

            ''' <summary>
            ''' Convert any byte array into HEX string
            ''' </summary>
            ''' <param name="data">byte array</param>
            ''' <returns>example: FF-01-02-AE</returns>
            Public Function ByteToHex(ByVal data() As Byte) As String
                Return ByteToHex(data, "-")
            End Function


            ''' <summary>
            ''' Convert any byte array into HEX string
            ''' </summary>
            ''' <param name="data">byte array</param>
            ''' <param name="separator">a separator can be empty string or "-" for example</param>
            ''' <returns>example: FF-01-02-AE</returns>
            Public Function ByteToHex(ByVal data() As Byte, ByVal separator As String) As String
                If data Is Nothing Then Return ""

                Dim res As String = ""
                For Each B As Byte In data
                    Dim W As String = B.ToString("X")
                    If String.IsNullOrEmpty(separator) = True AndAlso W.Length = 1 Then W = "0" & W
                    res &= W & separator
                Next

                res = res.TrimEnd(separator)

                Return res
            End Function


            ''' <summary>
            ''' Returns a value of 'something' when only percent value is known.
            ''' </summary>
            ''' <param name="valuePercent">the percent value from 0% to 100% </param>
            ''' <param name="Min">the minimum value of then range of the value. For an byte it is 0.</param>
            ''' <param name="Max">the maximum value of the range of the value. For an byte it is 255.</param>
            ''' <returns>a value of 'something'. For an byte it could be from 0 to 255.</returns>
            ''' <remarks></remarks>
            Public Function FromPercent(ByVal valuePercent As Single, ByVal Min As Single, ByVal Max As Single) As Single
                Dim res As Single
                res = (Max - Min) / 100 * valuePercent
                res += Min
                Return res
            End Function

            ''' <summary>
            ''' Returns a value as percent of a single value
            ''' </summary>
            ''' <param name="value">the value of 'something'. For example an byte, converted to single which goes from 0 to 255.</param>
            ''' <param name="Min">the minimum value of then range of the value. For an byte it is 0.</param>
            ''' <param name="Max">the maximum value of the range of the value. For an byte it is 255.</param>
            ''' <returns>a value between 0% and 100%</returns>
            ''' <remarks></remarks>
            Public Function ToPercent(ByVal value As Single, ByVal Min As Single, ByVal Max As Single) As Single
                Dim res As Single
                res = (value - Min) / (Max - Min)
                If Single.IsInfinity(res) Then res = 0
                res = res * 100
                Return res
            End Function

            ''' <summary>
            ''' Convert objects into TRUE / FALSE values
            ''' </summary>
            ''' <param name="Src"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function BooleanConverter(ByVal Src As Object) As Boolean
                If Src Is Nothing Then Return False
                Dim W As String = ";yes;on;1;1;y;enabled;ein;true;"
                Dim SrcW As String
                SrcW = System.ComponentModel.TypeDescriptor.GetConverter(Src.GetType).ConvertToString(Src)

                If W.Contains(";" & SrcW.ToLower & ";") Then
                    Return True
                Else
                    Return False
                End If
            End Function


#Region "PixToPos and PosToPix"
            Public Function PixToPos(ByVal R As RectangleF, ByVal MapSize As RectangleF, ByVal ImgSize As SizeF) As RectangleF
                Dim P1, P2 As PointF
                P1.X = R.X
                P2.X = R.Right
                P1.Y = R.Y
                P2.Y = R.Bottom
                P1 = PixToPos(P1, MapSize, ImgSize)
                P2 = PixToPos(P2, MapSize, ImgSize)
                Return RectangleF.FromLTRB(P1.X, P1.Y, P2.X, P2.Y) 'Neu ab V1.0.0.11 mit Rectangle >F<
            End Function


            Public Function PosToPix(ByVal pos As PointF, ByVal MapSize As RectangleF, ByVal ImgSize As Size) As PointF
                Dim R As New RectangleF(0, 0, ImgSize.Width, ImgSize.Height)
                Return PosToPix(pos, MapSize, R)
            End Function


            Public Function PosToPix(ByVal R As RectangleF, ByVal MapSize As RectangleF, ByVal ImgSize As SizeF) As RectangleF
                Return PosToPix(R, MapSize, New RectangleF(0, 0, ImgSize.Width, ImgSize.Height))
            End Function


            Public Function PosToPix(ByVal R As RectangleF, ByVal MapSize As RectangleF, ByVal ImgRect As RectangleF) As RectangleF
                Dim P1, P2 As PointF
                P1.X = R.X
                P2.X = R.Right
                P1.Y = R.Y
                P2.Y = R.Bottom
                P1 = PosToPix(P1, MapSize, ImgRect)
                P2 = PosToPix(P2, MapSize, ImgRect)
                Dim X1 As Single = P1.X
                Dim Y1 As Single = P1.Y
                Dim X2 As Single = P2.X
                Dim Y2 As Single = P2.Y

                'If X1 > X2 Then TB.Math.Swap(X1, X2)
                'If Y1 > Y2 Then TB.Math.Swap(Y1, Y2)

                Return Rectangle.FromLTRB(X1, Y1, X2, Y2)
            End Function


            Public Function PosToPix(ByVal pos As PointF, ByVal MapSize As RectangleF, ByVal ImgRect As RectangleF) As PointF
                If MapSize.Width = 0 Or MapSize.Height = 0 Then Return New PointF(1, 1)
                If Single.IsInfinity(pos.X) = True Then Return New PointF(1, 1)
                If Single.IsInfinity(pos.Y) = True Then Return New PointF(1, 1)
                If Single.IsNaN(pos.X) = True Then Return New PointF(1, 1)
                If Single.IsNaN(pos.Y) = True Then Return New PointF(1, 1)

                Dim X As Single
                Dim Y As Single

                X = pos.X - MapSize.Left
                Y = pos.Y - MapSize.Top

                X = X / MapSize.Width * ImgRect.Width
                Y = Y / MapSize.Height * ImgRect.Height

                'Y = ImgRect.Height - Y       'Y in the picture is inverted
                Return New PointF(X, Y)
            End Function

            Public Function PixToPos(ByVal PixelPos As PointF, ByVal MapSize As RectangleF, ByVal ImgSize As SizeF) As PointF
                Return PixToPos(PixelPos, MapSize, New RectangleF(0, 0, ImgSize.Width, ImgSize.Height))
            End Function

            Public Function PixToPos(ByVal PixelPos As PointF, ByVal MapSize As RectangleF, ByVal ImgRect As RectangleF) As PointF
                If MapSize.Width = 0 Or MapSize.Height = 0 Then Return New PointF(1, 1)
                If Single.IsInfinity(PixelPos.X) = True Then Return New PointF(1, 1)
                If Single.IsInfinity(PixelPos.Y) = True Then Return New PointF(1, 1)
                If Single.IsNaN(PixelPos.X) = True Then Return New PointF(1, 1)
                If Single.IsNaN(PixelPos.Y) = True Then Return New PointF(1, 1)

                Dim X As Single
                Dim Y As Single

                X = PixelPos.X
                Y = PixelPos.Y                          'Nicht invertiert, weil PosToPix bereits invertiert wird
                'Y = ImgRect.Height - PixelPos.Y        'Y in the picture is inverted

                X = X / ImgRect.Width * MapSize.Width
                Y = Y / ImgRect.Height * MapSize.Height

                X = X + MapSize.Left
                Y = Y + MapSize.Top


                Return New PointF(X, Y)
            End Function



#End Region


            ''' <summary>
            ''' Returns the positive Rectangle from a rectangle. 
            ''' That means if the Height or Width is below zero it zwitches the Points.
            ''' </summary>
#Region "Function RectToPositiveRect"
            Public Function RectToPositiveRect(ByVal X1 As Single, ByVal Y1 As Single, ByVal X2 As Single, ByVal Y2 As Single) As System.Drawing.RectangleF
                Return RectToPositiveRect(System.Drawing.RectangleF.FromLTRB(X1, Y1, X2, Y2))
            End Function


            Public Function RectToPositiveRect(ByVal sX1 As String, ByVal sY1 As String, ByVal sX2 As String, ByVal sY2 As String) As System.Drawing.RectangleF
                Dim X1, Y1, X2, Y2 As Single
                X1 = TB.Math.myVal(sX1)
                Y1 = TB.Math.myVal(sY1)
                X2 = TB.Math.myVal(sX2)
                Y2 = TB.Math.myVal(sY2)
                Return RectToPositiveRect(System.Drawing.RectangleF.FromLTRB(X1, Y1, X2, Y2))
            End Function


            Public Function RectToPositiveRect(ByVal R As System.Drawing.RectangleF) As System.Drawing.RectangleF
                Dim X1, Y1, X2, Y2 As Single
                X1 = R.X
                Y1 = R.Y
                X2 = R.Right
                Y2 = R.Bottom
                If X1 > X2 Then TB.Math.Swap(X1, X2)
                If Y1 > Y2 Then TB.Math.Swap(Y1, Y2)
                Return System.Drawing.RectangleF.FromLTRB(X1, Y1, X2, Y2)
            End Function


            Public Function RectToPositiveRect(ByVal R As System.Drawing.Rectangle) As System.Drawing.Rectangle
                Dim X1, Y1, X2, Y2 As Integer
                X1 = R.X
                Y1 = R.Y
                X2 = R.Right
                Y2 = R.Bottom
                If X1 > X2 Then TB.Math.Swap(X1, X2)
                If Y1 > Y2 Then TB.Math.Swap(Y1, Y2)
                Return System.Drawing.Rectangle.FromLTRB(X1, Y1, X2, Y2)
            End Function
#End Region



            ''' <summary>
            ''' Inflates the rectangle by a value.
            ''' Supports negative rectangles
            ''' </summary>
            Public Function InflateRect(ByVal SrcRect As RectangleF, ByVal Width As Single, ByVal Height As Single) As RectangleF
                If SrcRect.Left < SrcRect.Right Then
                    SrcRect.X -= Width
                    SrcRect.Width += (Width * 2)
                Else
                    SrcRect.X += Width
                    SrcRect.Width -= (Width * 2)
                End If
                If SrcRect.Top < SrcRect.Bottom Then
                    SrcRect.Y -= Height
                    SrcRect.Height += (Height * 2)
                Else
                    SrcRect.Y += Height
                    SrcRect.Height -= (Height * 2)
                End If
                Return SrcRect
            End Function


            ''' <summary>
            ''' Determines if any Rectangle is in the source rectangle.
            ''' The difference between the .Net Rectangle.InsectsWith and 
            ''' this function is only, that this function can handle 
            ''' negative values too.
            ''' </summary>
            Public Function InsectsWith(ByVal RectangleSrc As Rectangle, ByVal Rectangle2 As Rectangle) As Boolean
                Return TB.Math.RectToPositiveRect(RectangleSrc).IntersectsWith(TB.Math.RectToPositiveRect(Rectangle2))
                'If TB.Math.ContainsPoint(RectangleSrc, New Point(Rectangle2.Left, Rectangle2.Top)) = True Then Return True
                'If TB.Math.ContainsPoint(RectangleSrc, New Point(Rectangle2.Right, Rectangle2.Top)) = True Then Return True
                'If TB.Math.ContainsPoint(RectangleSrc, New Point(Rectangle2.Left, Rectangle2.Bottom)) = True Then Return True
                'If TB.Math.ContainsPoint(RectangleSrc, New Point(Rectangle2.Right, Rectangle2.Bottom)) = True Then Return True
                'Return False
            End Function

            ''' <summary>
            ''' Determines if any RectangleF is in the source rectangleF.
            ''' The difference between the .Net RectangleF.InsectsWith and 
            ''' this function is only, that this function can handle 
            ''' negative values too.
            ''' </summary>
            Public Function InsectsWith(ByVal RectangleSrc As RectangleF, ByVal Rectangle2 As RectangleF) As Boolean
                Return TB.Math.RectToPositiveRect(RectangleSrc).IntersectsWith(TB.Math.RectToPositiveRect(Rectangle2))
                'If TB.Math.ContainsPoint(RectangleSrc, New PointF(Rectangle2.Left, Rectangle2.Top)) = True Then Return True
                'If TB.Math.ContainsPoint(RectangleSrc, New PointF(Rectangle2.Right, Rectangle2.Top)) = True Then Return True
                'If TB.Math.ContainsPoint(RectangleSrc, New PointF(Rectangle2.Left, Rectangle2.Bottom)) = True Then Return True
                'If TB.Math.ContainsPoint(RectangleSrc, New PointF(Rectangle2.Right, Rectangle2.Bottom)) = True Then Return True
                'Return False
            End Function

            ''' <summary>
            ''' Determines if the point is in the rectangle.
            ''' The difference between the .Net Rectangle.Contains and 
            ''' this function is only, that this function can handle 
            ''' negative values too.
            ''' </summary>
            Public Function ContainsPoint(ByVal RectangleSrc As Rectangle, ByVal Pnt As Point) As Boolean
                If Pnt.X < Global.System.Math.Min(RectangleSrc.Left, RectangleSrc.Right) Then Return False
                If Pnt.Y < Global.System.Math.Min(RectangleSrc.Top, RectangleSrc.Bottom) Then Return False
                If Pnt.X > Global.System.Math.Max(RectangleSrc.Left, RectangleSrc.Right) Then Return False
                If Pnt.Y > Global.System.Math.Max(RectangleSrc.Top, RectangleSrc.Bottom) Then Return False
                Return True
            End Function

            ''' <summary>
            ''' Determines if the point is in the rectangle.
            ''' The difference between the .Net RectangleF.Contains and 
            ''' this function is only, that this function can handle 
            ''' negative values too.
            ''' </summary>
            Public Function ContainsPoint(ByVal RectangleSrc As RectangleF, ByVal Pnt As PointF) As Boolean
                If Pnt.X < Global.System.Math.Min(RectangleSrc.Left, RectangleSrc.Right) Then Return False
                If Pnt.Y < Global.System.Math.Min(RectangleSrc.Top, RectangleSrc.Bottom) Then Return False
                If Pnt.X > Global.System.Math.Max(RectangleSrc.Left, RectangleSrc.Right) Then Return False
                If Pnt.Y > Global.System.Math.Max(RectangleSrc.Top, RectangleSrc.Bottom) Then Return False
                Return True
            End Function


            Public Function Average(ByVal values As List(Of Single)) As Single
                Dim res As Single = 0

                Try
                    If values Is Nothing OrElse values.Count = 0 Then Return res
                    If values.Count = 1 Then Return values(0)

                    res = values(0)
                    For i As Integer = 2 To values.Count
                        res = (res * ((i - 1) / i)) + (values(i - 1) * (1 / i))
                    Next
                Catch ex As Exception

                End Try

                Return res
            End Function


            '''<summary>The following class represents simple functionality of the trapezoid.</summary>
            Class MathTrapezoidClass
                'http://msdn.microsoft.com/en-us/library/system.math.asin.aspx
                Private m_longBase As Double
                Private m_shortBase As Double
                Private m_leftLeg As Double
                Private m_rightLeg As Double

                Public Sub New(ByVal longbase As Double, ByVal shortbase As Double, ByVal leftLeg As Double, ByVal rightLeg As Double)
                    m_longBase = System.Math.Abs(longbase)
                    m_shortBase = System.Math.Abs(shortbase)
                    m_leftLeg = System.Math.Abs(leftLeg)
                    m_rightLeg = System.Math.Abs(rightLeg)
                End Sub

                Private Function GetRightSmallBase() As Double
                    GetRightSmallBase = (System.Math.Pow(m_rightLeg, 2) - System.Math.Pow(m_leftLeg, 2) + System.Math.Pow(m_longBase, 2) + System.Math.Pow(m_shortBase, 2) - 2 * m_shortBase * m_longBase) / (2 * (m_longBase - m_shortBase))
                End Function

                Public Function GetHeight() As Double
                    Dim x As Double = GetRightSmallBase()
                    GetHeight = System.Math.Sqrt(System.Math.Pow(m_rightLeg, 2) - System.Math.Pow(x, 2))
                End Function

                Public Function GetSquare() As Double
                    GetSquare = GetHeight() * m_longBase / 2
                End Function

                Public Function GetLeftBaseRadianAngle() As Double
                    Dim sinX As Double = GetHeight() / m_leftLeg
                    GetLeftBaseRadianAngle = System.Math.Round(System.Math.Asin(sinX), 2)
                End Function

                Public Function GetRightBaseRadianAngle() As Double
                    Dim x As Double = GetRightSmallBase()
                    Dim cosX As Double = (System.Math.Pow(m_rightLeg, 2) + System.Math.Pow(x, 2) - System.Math.Pow(GetHeight(), 2)) / (2 * x * m_rightLeg)
                    GetRightBaseRadianAngle = System.Math.Round(System.Math.Acos(cosX), 2)
                End Function

                Public Function GetLeftBaseDegreeAngle() As Double
                    Dim x As Double = GetLeftBaseRadianAngle() * 180 / System.Math.PI
                    GetLeftBaseDegreeAngle = System.Math.Round(x, 2)
                End Function

                Public Function GetRightBaseDegreeAngle() As Double
                    Dim x As Double = GetRightBaseRadianAngle() * 180 / System.Math.PI
                    GetRightBaseDegreeAngle = System.Math.Round(x, 2)
                End Function

                'Public Shared Sub Main()
                '    Dim trpz As MathTrapezoidClass = New MathTrapezoidClass(20, 10, 8, 6)
                '    Console.WriteLine("The trapezoid's bases are 20.0 and 10.0, the trapezoid's legs are 8.0 and 6.0")
                '    Dim h As Double = trpz.GetHeight()
                '    Console.WriteLine("Trapezoid height is: " + h.ToString())
                '    Dim dxR As Double = trpz.GetLeftBaseRadianAngle()
                '    Console.WriteLine("Trapezoid left base angle is: " + dxR.ToString() + " Radians")
                '    Dim dyR As Double = trpz.GetRightBaseRadianAngle()
                '    Console.WriteLine("Trapezoid right base angle is: " + dyR.ToString() + " Radians")
                '    Dim dxD As Double = trpz.GetLeftBaseDegreeAngle()
                '    Console.WriteLine("Trapezoid left base angle is: " + dxD.ToString() + " Degrees")
                '    Dim dyD As Double = trpz.GetRightBaseDegreeAngle()
                '    Console.WriteLine("Trapezoid left base angle is: " + dyD.ToString() + " Degrees")
                'End Sub

            End Class



            ''' <summary>Calculates the angle of any vector in reference to the X-Axis</summary>
            ''' <remarks>Returns angle in Radians. To Calculate degrees use: Angle * 180 / System.Math.PI</remarks>
            Public Function GetAngle(ByVal ptFrom As System.Drawing.PointF, ByVal ptTo As System.Drawing.PointF) As Double
                ptTo -= New System.Drawing.SizeF(ptFrom) 'ptTo in Relation zu ptFrom setzen

                With ptTo
                    Dim Amount As Double = System.Math.Sqrt((.X * .X) + (.Y * .Y))
                    If Amount > 0 Then           'Nulldivision vermeiden
                        If .X > 0 Then
                            Return System.Math.Asin(.Y / Amount)
                        Else
                            Return System.Math.PI - System.Math.Asin(.Y / Amount)
                        End If
                    End If
                End With
                Return 0
            End Function


            'Public Function PointOfTriangle(ByVal Pt1 As System.Drawing.PointF, ByVal Pt2 As System.Drawing.PointF) As System.Drawing.PointF

            '    Dim dSquare As Double = (Pt1.X - Pt2.X) ^ 2 + (Pt1.Y - Pt2.Y) ^ 2
            '    Dim Length As Double = Global.System.Math.Sqrt(dSquare)

            '    Dim X As Double = Pt1.X + Length
            '    Dim Y As Double = Pt2.Y + Length

            '    Return New PointF(X, Y)
            'End Function

            ''' <summary>
            ''' Returns a point with calculated angel distance from another point
            ''' </summary>
            ''' <param name="Alpha">Point from 0 to 360 degrees</param>
            ''' <param name="Radius">the radius from one to the other point</param>
            ''' <param name="ptFrom">start point. could be PointF.Empty</param>
            Public Function AngleToPoint(ByVal Alpha As Double, ByVal Radius As Single, ByVal ptFrom As System.Drawing.PointF) As System.Drawing.PointF
                Dim X, Y As Single
                Dim P As System.Drawing.PointF

                Alpha = Alpha * Global.System.Math.PI / 180 'From degrees to Bogenmass
                'X = Global.System.Math.Sin(Alpha * Radius)
                'Y = Global.System.Math.Cos(Alpha * Radius)
                X = Global.System.Math.Sin(Alpha) * Radius
                Y = Global.System.Math.Cos(Alpha) * Radius


                P = New System.Drawing.PointF(ptFrom.X + X, ptFrom.Y + Y)
                Return P
            End Function




            ''' <summary>
            ''' Returns the String of the decimal point.
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function DecimalPoint() As String
                'Dim i As Single = 1.1
                'Dim W As String = i.ToString
                'Return W.Substring(1, 1)
                Return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator

                'Dim NFI As System.Globalization.NumberFormatInfo = _
                '    System.Globalization.CultureInfo.CurrentCulture.NumberFormat
                'Return NFI.NumberDecimalSeparator

            End Function



            ''' <summary>
            ''' Returns the single value of a string.
            ''' Removes any non-numerics before trying to calculating the value.
            ''' </summary>
            ''' <param name="SourceString"></param>
            ''' <param name="RemoveLetters"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function myVal(ByVal SourceString As String, ByVal RemoveLetters As Boolean) As Double
                Dim GoodStrings As String = "0123456789.,-+"
                Dim i As Integer


                If SourceString Is Nothing Then Return 0
                If RemoveLetters = True Then
                    If SourceString.Contains("E+") Or SourceString.Contains("E-") Then GoodStrings &= "E"

                    i = 0
                    Do
                        If i > SourceString.Length - 1 Then Exit Do
                        If GoodStrings.Contains(SourceString.Substring(i, 1)) = False Then
                            SourceString = SourceString.Substring(0, i) & SourceString.Substring(i + 1)
                            i -= 1
                        End If
                        i += 1
                    Loop
                End If

                Return myVal(SourceString)
            End Function


            ''' <summary>
            ''' Returns the double value of a string.
            ''' </summary>
            ''' <param name="SourceString"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function myVal(ByVal SourceString As String) As Double
                Try
                    'another way:
                    'Result = Double.Parse(SourceString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture.NumberFormat) ' Culture.NumberFormat)
                    If SourceString Is Nothing Then Return 0
                    If SourceString.Length = 0 Then Return 0
                    SourceString = SourceString.Replace(".", DecimalPoint)
                    SourceString = SourceString.Replace(",", DecimalPoint)
                    If SourceString = "-" Then SourceString = "0"
                    If SourceString.ToLower.Contains("e") Then
                        Try
                            Return Double.Parse(SourceString, System.Globalization.NumberStyles.Float)
                        Catch ex As Exception

                            Return Global.System.Convert.ToDecimal(SourceString)
                        End Try
                    Else
                        Return Global.System.Convert.ToDecimal(SourceString)
                    End If
                Catch ex As Exception
                    Return 0
                End Try
            End Function

            ''' <summary>
            ''' check the Ascii values of two string
            ''' </summary>
            Public Function IsLarger(ByVal V1 As String, ByVal V2 As String) As Boolean
                Dim rCntMax As Integer
                Dim rCnt As Integer

                rCntMax = V1.Length
                If V2.Length < rCntMax Then rCntMax = V2.Length
                For rCnt = 1 To rCntMax

                    If Global.Microsoft.VisualBasic.Strings.Asc(Global.Microsoft.VisualBasic.Strings.Mid(V1, rCnt, 1)) > Global.Microsoft.VisualBasic.Strings.Asc(Global.Microsoft.VisualBasic.Strings.Mid(V2, rCnt, 1)) Then
                        Return True 'Zeichen ist Grˆsser:
                        Exit For
                    Else 'Wenn das Zeichen gleich gross oder kleiner ist:
                        If Global.Microsoft.VisualBasic.Strings.Asc(Global.Microsoft.VisualBasic.Strings.Mid(V1, rCnt, 1)) < Global.Microsoft.VisualBasic.Strings.Asc(Global.Microsoft.VisualBasic.Strings.Mid(V2, rCnt, 1)) Then Exit For
                    End If
                Next
                Return False
            End Function


            ''' <summary>
            ''' swap one object with the other
            ''' </summary>
            Public Sub Swap(ByRef V1 As DateTime, ByRef V2 As DateTime)
                Dim V3 As DateTime

                V3 = V1
                V1 = V2
                V2 = V3
            End Sub

            ''' <summary>
            ''' swap one object with the other
            ''' </summary>
            Public Sub Swap(ByRef V1 As Integer, ByRef V2 As Integer)
                Dim V3 As Integer

                V3 = V1
                V1 = V2
                V2 = V3
            End Sub

            ''' <summary>
            ''' swap one object with the other
            ''' </summary>
            Public Sub Swap(ByRef V1 As Object, ByRef V2 As Object)
                Dim V3 As Object

                V3 = V1
                V1 = V2
                V2 = V3
            End Sub

            ''' <summary>
            ''' swap one object with the other
            ''' </summary>
            Public Sub Swap(ByRef V1() As Byte, ByRef V2() As Byte)
                Dim V3() As Byte

                V3 = V1
                V1 = V2
                V2 = V3
            End Sub

            ''' <summary>
            ''' sort any array by numbers
            ''' </summary>
            Public Sub BubbleSort(ByRef myArray() As Integer, ByVal SortKeyArray() As Double, Optional ByVal ASC As Boolean = True)
                Dim i, j As Integer

                ReDim Preserve myArray(UBound(SortKeyArray)) 'stellt sicher, dass die Dimension gleichgross ist

                For i = 0 To UBound(SortKeyArray)
                    For j = i + 1 To UBound(SortKeyArray)
                        If ASC = True Then
                            If SortKeyArray(i) > SortKeyArray(j) Then
                                Call Swap(myArray(i), myArray(j))
                                Call Swap(SortKeyArray(i), SortKeyArray(j))
                            End If
                        Else
                            If SortKeyArray(i) < SortKeyArray(j) Then
                                Call Swap(myArray(i), myArray(j))
                                Call Swap(SortKeyArray(i), SortKeyArray(j))
                            End If
                        End If
                    Next
                Next
            End Sub

            ''' <summary>
            ''' sort any array by characters
            ''' </summary>
            Public Sub BubbleSort(ByRef myArray() As Integer, ByVal SortKeyArray() As String, Optional ByVal ASC As Boolean = True)
                Dim i, j As Integer

                ReDim Preserve myArray(UBound(SortKeyArray)) 'stellt sicher, dass die Dimension gleichgross ist

                For i = 0 To UBound(SortKeyArray)
                    For j = i + 1 To UBound(SortKeyArray)
                        If ASC = True Then
                            If IsLarger(SortKeyArray(i), SortKeyArray(j)) = True Then
                                Call Swap(myArray(i), myArray(j))
                                Call Swap(SortKeyArray(i), SortKeyArray(j))
                            End If
                        Else
                            If IsLarger(SortKeyArray(i), SortKeyArray(j)) = False Then
                                Call Swap(myArray(i), myArray(j))
                                Call Swap(SortKeyArray(i), SortKeyArray(j))
                            End If
                        End If
                    Next
                Next
            End Sub

            ''' <summary>
            ''' sort any array by date
            ''' </summary>
            Public Sub BubbleSort(ByRef myArray() As Integer, ByVal SortKeyArray() As Date, Optional ByVal ASC As Boolean = True)
                Dim i, j As Integer

                ReDim Preserve myArray(UBound(SortKeyArray)) 'stellt sicher, dass die Dimension gleichgross ist

                For i = 0 To UBound(SortKeyArray)
                    For j = i + 1 To UBound(SortKeyArray)
                        If ASC = True Then
                            If SortKeyArray(i) > SortKeyArray(j) Then
                                Call Swap(myArray(i), myArray(j))
                                Call Swap(SortKeyArray(i), SortKeyArray(j))
                            End If
                        Else
                            If SortKeyArray(i) < SortKeyArray(j) Then
                                Call Swap(myArray(i), myArray(j))
                                Call Swap(SortKeyArray(i), SortKeyArray(j))
                            End If
                        End If
                    Next
                Next
            End Sub


            Public Function BytesToKbMbGb(ByVal ByteSize As Long) As String
                Return BytesToKbMbGb(CULng(ByteSize))
            End Function


            Public Function BytesToKbMbGb(ByVal ByteSize As ULong) As String
                Dim suffix() As String = {"B", "KB", "MB", "GB", "TB", "PB", "EB"}
                Dim run As Integer = 0
                Dim fdSize As Double = ByteSize
                Do While (fdSize >= 1024)
                    fdSize /= 1024
                    run += 1
                Loop

                If Global.System.Math.Truncate(fdSize) = fdSize Then
                    Return String.Format("{0:0}", fdSize) & suffix(run)
                Else
                    Return String.Format("{0:0.0}", fdSize) & suffix(run)
                End If
            End Function



            ''' <summary>
            ''' convert a single value in Hz into a frequency string
            ''' </summary>
            Public Function HzToKhzMhz(ByVal Hertz As Single, ByVal AllowRound As Boolean) As String
                Return HzToKhzMhz(Hertz, 0, AllowRound)
            End Function

            ''' <summary>
            ''' convert a single value in Hz into a frequency string
            ''' </summary>
            Public Function HzToKhzMhz(ByVal Hertz As Single) As String
                Return HzToKhzMhz(Hertz, 1, True)
            End Function

            Public Function HzToKhzMhz(ByVal Hertz As Single, ByVal Digits As Integer, ByVal AllowRound As Boolean) As String

                If Single.IsInfinity(Hertz) Then Hertz = 0
                If Single.IsNaN(Hertz) Then Hertz = 0
                Dim Suffix() As String = {"Hz", "kHz", "MHz", "GHz", "THz", "PHz", "EHz"}
                Dim W As String = ""
                Dim Potenz As Integer = 0
                Dim Hz As Single = Hertz
                Dim Divider As Integer = 1000
                Do
                    If Hz < Divider Then Exit Do
                    If AllowRound = False AndAlso GetDecimals(Hz / Divider) > 0 Then Exit Do
                    Hz = Global.System.Math.Floor(Hz / Divider)
                    Potenz += 1
                Loop


                'If AllowRound = False Then 'Der Wert darf nicht vom Ursprungswert abweichen
                '    If Hz * (Divider ^ Potenz) <> Hertz Then Hz = Hertz : Potenz = 0
                'End If
                ''W = Format(Math.Round(v, Digits), "0") & W
                Try
                    If Potenz = 0 And GetDecimals(Hz) > 0 And AllowRound = False Then
                        W = Hz.ToString & Suffix(Potenz)
                    Else
                        W = System.String.Format("{0:0." & TB.String.StringDub(Digits, "0") & "}" & Suffix(Potenz), Hz)
                    End If
                Catch ex As Exception
                    W = Hertz & "Hz"
                End Try
                Return W
            End Function


            Public Function GetDecimals(ByVal V As Single) As Single
                If Single.IsInfinity(V) Then V = 0
                If Single.IsNaN(V) Then V = 0
                Dim i As Integer = Global.System.Math.Truncate(V)
                Return V - i
            End Function

            Public Function GetDecimals(ByVal V As Double) As Double
                If Double.IsInfinity(V) Then V = 0
                If Double.IsNaN(V) Then V = 0
                Dim i As Integer = Global.System.Math.Truncate(V)
                Return V - i
            End Function

            ''' <summary>
            ''' convert a frequency string into a single value
            ''' </summary>
            Public Function kHzMhzToHz(ByVal ValueString As String) As Single
                Dim i As Int16
                Dim v As Single = 0
                Dim W As String = ""

                If ValueString.Contains(DecimalPoint) = False Then
                    ValueString = ValueString.Replace(",", DecimalPoint).Replace(".", DecimalPoint)
                End If


                For i = ValueString.Length To 1 Step -1
                    If IsNumeric(Mid(ValueString, i, 1)) = True Then
                        W = Mid(ValueString, i + 1)
                        'v = Val(Strings.Left(ValueString, i))
                        v = TB.Math.myVal(Global.Microsoft.VisualBasic.Strings.Left(ValueString, i))
                        Exit For
                    End If
                Next

                Select Case W.ToLower
                    Case "" : v = v
                    Case "hz" : v = v
                    Case "k" : v = v * 1000
                    Case "khz" : v = v * 1000
                    Case "mhz" : v = v * 1000000
                    Case "m" : v = v * 1000000
                    Case "ghz" : v = v * 1000000000
                    Case "g" : v = v * 1000000000
                    Case Else
                End Select

                Return v
            End Function



            ''' <summary>
            ''' Trim value between min and max value
            ''' </summary>
            Public Function myTrim(ByVal Val As Integer, ByVal Min As Byte, ByVal Max As Byte) As Byte
                Return Global.System.Convert.ToByte(Global.System.Math.Max(Global.System.Math.Min(Val, Max), Min))
            End Function

            ''' <summary>
            ''' Trim value between min and max value
            ''' </summary>
            Public Function myTrim(ByVal Val As Integer, ByVal Min As Integer, ByVal Max As Integer) As Integer
                Return Global.System.Math.Max(Global.System.Math.Min(Val, Max), Min)
            End Function

            ''' <summary>
            ''' Trim value between min and max value
            ''' </summary>
            Public Function myTrim(ByVal Val As Long, ByVal Min As Long, ByVal Max As Long) As Long
                If Single.IsNaN(Val) Then Val = Min
                If Single.IsInfinity(Val) Then Val = Min
                Return Global.System.Math.Max(Global.System.Math.Min(Val, Max), Min)
            End Function

            ''' <summary>
            ''' Trim value between min and max value
            ''' </summary>
            Public Function myTrim(ByVal Val As Single, ByVal Min As Single, ByVal Max As Single) As Single
                If Single.IsNaN(Val) Then Val = Min
                If Single.IsInfinity(Val) Then Val = Min
                Return Global.System.Math.Max(Global.System.Math.Min(Val, Max), Min)
            End Function

            ''' <summary>
            ''' Trim value between min and max value
            ''' </summary>
            Public Function myTrim(ByVal Val As Double, ByVal Min As Double, ByVal Max As Double) As Double
                If Double.IsNaN(Val) Then Val = Min
                If Double.IsInfinity(Val) Then Val = Min
                Return Global.System.Math.Max(Global.System.Math.Min(Val, Max), Min)
            End Function
        End Module
    End Namespace


    Namespace Controls
        Public Module modControls

            


            ''' <summary>
            ''' Try to convert standard types with their values into an regular string. 
            ''' For example "New Point(3,3)" goes to "3;3" instead of the original "{X=3,Y=3}".
            ''' This function is used with the StringToType function.
            ''' </summary>
            ''' <param name="Src">Any object except classes and structures.</param>
            ''' <returns>the value of the object as string. 
            ''' For example for the object "New RectangleF(3.4,2.1,8.0,10.0)" it returns "3.4000000;2.10000000;8.00000000;10.00000000"</returns>
            ''' <remarks>Supports. String, Boolean, Byte, Integer, Long, Single, Double, Point, PointF, Rectangle, RectangleF, Size, SizeF, IntPtr, Char, DrawingGraphics, ListOf 
            ''' Anderes ‰hnliches Vorgehen auch auf: http://www.west-wind.com/WebLog/posts/980.aspx</remarks>
            <MethodImpl(MethodImplOptions.NoOptimization)> _
            Public Function TypeToString(ByVal Src As Object) As String
                Dim res As String = String.Empty
                If Src Is Nothing Then Return String.Empty

                If Src.GetType.IsEnum = True Then
                    Src = Global.System.Convert.ChangeType(Src, System.Enum.GetUnderlyingType(Src.GetType))
                End If

                Dim TypeName As String = Src.GetType.Name
                If Src.GetType.IsGenericType AndAlso Src.GetType.GetGenericTypeDefinition() Is GetType(Nullable(Of )) Then
                    TypeName = Src.GetType.GetGenericArguments()(0).Name
                End If

                Select Case TypeName
                    'standards
                    Case GetType(String).Name
                        res = CType(Src, String)
                    Case GetType(Boolean).Name
                        If CType(Src, Boolean) = True Then
                            res = "1"
                        Else
                            res = "0"
                        End If
                    Case GetType(Byte).Name
                        res = CType(Src, Byte).ToString("0")
                    Case GetType(Integer).Name
                        res = CType(Src, Integer).ToString("0")
                    Case GetType(Long).Name
                        res = CType(Src, Long).ToString("0")
                    Case GetType(Single).Name
                        res = CType(Src, Single).ToString("r").Replace(TB.Math.DecimalPoint, ".") 'new r will fix the exponential issue and will print all values with decimal without exponential
                        'res = res.Trim(".0".ToCharArray)
                        If res.Length = 0 Then res = "0"
                    Case GetType(Double).Name
                        res = CType(Src, Double).ToString("r").Replace(TB.Math.DecimalPoint, ".") 'new r will fix the exponential issue and will print all values with decimal without exponential
                        'res = res.Trim(".0".ToCharArray)
                    Case GetType(System.Drawing.Point).Name
                        Dim Pt As System.Drawing.Point = CType(Src, System.Drawing.Point)
                        res = String.Format("{0:0};{1:0}", Pt.X, Pt.Y)
                    Case GetType(System.Drawing.PointF).Name
                        Dim pt As System.Drawing.PointF = CType(Src, System.Drawing.PointF)
                        Dim X As Single = pt.X
                        Dim Y As Single = pt.Y
                        res = TB.Controls.TypeToString(X) & ";" & TB.Controls.TypeToString(Y)
                        'res = String.Format("{0:0.00000000};{1:0.00000000}", pt.X, pt.Y)
                    Case GetType(System.Drawing.Rectangle).Name
                        Dim R As System.Drawing.Rectangle = CType(Src, System.Drawing.Rectangle)
                        res = String.Format("{0:0};{1:0};{2:0};{3:0};", R.X, R.Y, R.Width, R.Height)
                    Case GetType(System.Drawing.RectangleF).Name
                        Dim R As System.Drawing.RectangleF = CType(Src, System.Drawing.RectangleF)
                        res = String.Format("{0:0.00000000};{1:0.00000000};{2:0.000000000};{3:0.00000000};", R.X, R.Y, R.Width, R.Height)
                    Case GetType(System.Drawing.Size).Name
                        Dim S As System.Drawing.Size = CType(Src, System.Drawing.Size)
                        res = String.Format("{0:0};{1:0}", S.Width, S.Height)
                    Case GetType(System.Drawing.SizeF).Name
                        Dim S As System.Drawing.SizeF = CType(Src, System.Drawing.SizeF)
                        res = String.Format("{0:0.00000000};{1:0.00000000}", S.Width, S.Height)
                    Case GetType(IntPtr).Name
                        res = CType(Src, IntPtr).ToInt64.ToString("0")
                    Case GetType(System.Drawing.Color).Name
                        res = CType(Src, System.Drawing.Color).ToArgb.ToString("0")
                    Case GetType(DateTime).Name
                        res = CType(Src, DateTime).Ticks.ToString("0")
                    Case GetType(TimeSpan).Name
                        res = CType(Src, TimeSpan).Ticks.ToString("0")
                    Case GetType(IO.FileInfo).Name
                        res = CType(Src, IO.FileInfo).FullName
                    Case GetType(IO.DirectoryInfo).Name
                        res = CType(Src, IO.DirectoryInfo).FullName


                        'specials
                    Case GetType(Char).Name 'Char
                        res = Global.Microsoft.VisualBasic.Strings.AscW(CType(Src, Char)).ToString("0")
                    Case GetType(Drawing2D.GraphicsPath).Name 'Drawing.GraphicsPath
                        Dim L As New List(Of String)
                        For Each P As System.Drawing.PointF In CType(Src, Drawing2D.GraphicsPath).PathPoints
                            L.Add(TypeToString(P))
                        Next
                        res = String.Join(";", L.ToArray)



                    Case Else

                        'Interfaces and collections
                        'If TypeOf Src Is System.Collections.ICollection Then
                        '    Dim L As System.Collections.ObjectModel.Collection(Of Object)
                        '    L = CType(Src, System.Collections.ICollection)

                        '    For Each O As Object In L
                        '        res = res & TypeToString(O) & "|"
                        '    Next

                        If TypeOf Src Is System.Collections.IList Then
                            'Dim DstTmp() As Object =
                            Dim IListTmp As System.Collections.IList = CType(Src, System.Collections.IList)
                            Dim IListTmpItems(IListTmp.Count - 1) As Object
                            IListTmp.CopyTo(IListTmpItems, 0)
                            'Dim L As List(Of Object)
                            'L = CType(Src, System.Collections.IList)

                            For Each O As Object In IListTmpItems
                                res = res & TypeToString(O) & "|"
                            Next



                        ElseIf TypeOf Src Is System.Collections.IDictionary Then

                            'ToDo: support of Dictionary
                            'not supported
                            If TypeOf Src Is Dictionary(Of String, Object) Then
                                Dim dic As Dictionary(Of String, Object) = CType(Src, Dictionary(Of String, Object))
                                For Each K As String In dic.Keys
                                    Dim O As Object = dic(K)
                                    res = res & K & "|" & TypeToString(O) & "|"
                                Next

                            Else
                                res = Src.ToString
                            End If


                        ElseIf TypeOf Src Is Pen Then
                            Dim P As Pen = CType(Src, Pen)
                            res = TypeToString(P.Color) & ";" & TypeToString(P.Width) & ";" & TypeToString(CInt(P.DashStyle))

                        ElseIf TypeOf Src Is SolidBrush Then
                            res = TypeToString(CType(Src, SolidBrush).Color)


                        Else
                            res = Src.ToString

                        End If


                End Select

                Return res
            End Function




            ''' <summary>
            ''' Optionally, but not for all situations working, use this type instead of a reference object
            ''' </summary>
            ''' <param name="Src"></param>
            ''' <param name="DestinationType"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function StringToType(ByVal Src As String, ByVal DestinationType As Type) As Object
                Return StringToType(Src, Nothing, DestinationType)
            End Function


            ''' <summary>
            ''' Try to convert values from the TypeToString function into their object values. 
            ''' For example the string "3;3" can be converted into "New Point(3,3)".
            ''' This function is used with the StringToType function.
            ''' </summary>
            ''' <param name="Src">A string with information about the object value.</param>
            ''' <param name="DstObj">The object with already declared type to write the data in.</param>
            ''' <returns>The same object like the DstObj is. This is just to stay more comfortable by using a call like "MyList.Add(StringToType("9,4",New Point(3,3))"
            ''' For example for the string "3.4000000;2.10000000;8.00000000;10.00000000" can be converted back to an rectangle object "New RectangleF(3.4,2.1,8.0,10.0)"</returns>
            ''' <remarks>Supports. String, Boolean, Byte, Integer, Long, Single, Double, Point, PointF, Rectangle, RectangleF, Size, SizeF, IntPtr, Char, DrawingGraphics, ListOf </remarks>
            Public Function StringToType(ByVal Src As String, ByRef DstObj As Object) As Object
                If DstObj Is Nothing Then Return Nothing
                Dim DstType As Type = DstObj.GetType
                Return StringToType(Src, DstObj, DstType)
            End Function



            'Public Function StringToType(ByVal Src As String, ByVal T As Type) As Object
            '    Dim O As Object=CType( Object  t)
            'End Function


            ''' <summary>
            ''' Try to convert values from the TypeToString function into their object values. 
            ''' For example the string "3;3" can be converted into "New Point(3,3)".
            ''' This function is used with the StringToType function.
            ''' </summary>
            ''' <param name="Src">A string with information about the object value.</param>
            ''' <param name="DstObj">The object with already declared type to write the data in.</param>
            ''' <returns>The same object like the DstObj is. This is just to stay more comfortable by using a call like "MyList.Add(StringToType("9,4",New Point(3,3))"
            ''' For example for the string "3.4000000;2.10000000;8.00000000;10.00000000" can be converted back to an rectangle object "New RectangleF(3.4,2.1,8.0,10.0)"</returns>
            ''' <remarks>Supports. String, Boolean, Byte, Integer, Long, Single, Double, Point, PointF, Rectangle, RectangleF, Size, SizeF, IntPtr, Char, DrawingGraphics, ListOf </remarks>
            <MethodImpl(MethodImplOptions.NoOptimization)> _
            Private Function StringToType(ByVal Src As String, ByRef DstObj As Object, ByVal DestinationType As Type) As Object
                Dim Gut As Boolean = False
                If Src Is Nothing Then Return Nothing
                If DstObj Is Nothing Then DstObj = New Object
                Dim RemChar As Boolean = True

                Dim TypeName As String = DestinationType.Name
                If DestinationType.IsGenericType AndAlso DestinationType.GetGenericTypeDefinition() Is GetType(Nullable(Of )) Then
                    TypeName = DestinationType.GetGenericArguments()(0).Name
                End If

                Select Case TypeName
                    'standards
                    Case GetType(String).Name
                        DstObj = CType(Src, String) 'ok
                    Case GetType(Boolean).Name
                        DstObj = TB.Math.BooleanConverter(Src) ' Convert.ToBoolean(Src) '0/1 ok
                    Case GetType(Byte).Name
                        DstObj = Byte.Parse(Global.System.Math.Floor(TB.Math.myVal(Src, False)) Mod (CInt(Byte.MaxValue) + 1)) ' CType(Src, Byte).ToString("0")
                    Case GetType(Integer).Name
                        'DstObj = Integer.Parse(Src) 'CType(Src, Integer).ToString("0")
                        ' DstObj = Integer.Parse(CStr(Global.System.Math.Floor(TB.Math.myVal(Src, False)) Mod (CLng(Integer.MaxValue) + 1))) ' CType(Src, Byte).ToString("0")
                        DstObj = CInt(TB.Math.myTrim(TB.Math.myVal(Src, False), Integer.MinValue, Integer.MaxValue))
                    Case GetType(Long).Name
                        ' DstObj = Long.Parse(CStr(Global.System.Math.Floor(TB.Math.myVal(Src, False)) Mod (CSng(Long.MaxValue) + 1))) ' CType(Src, Long).ToString("0")
                        ' DstObj = CLng(TB.Math.myTrim(TB.Math.myVal(Src, False), Long.MinValue, Long.MaxValue))
                        Dim lValue As Long = 0
                        Long.TryParse(Src, lValue)
                        DstObj = lValue
                    Case GetType(Single).Name
                        'DstObj = Single.Parse(CStr(TB.Math.myVal(Src, False) Mod (CDbl(Single.MaxValue) + 1))) 'CType(Src, Single).ToString.Replace(TB.Math.DecimalPoint, ".")
                        DstObj = TB.Math.myTrim(TB.Math.myVal(Src, False), Single.MinValue, Single.MaxValue)
                    Case GetType(Double).Name
                        'DstObj = Double.Parse(CStr(TB.Math.myVal(Src, False) Mod (CDbl(Double.MaxValue) + 0))) '0 da kein grˆsserer type mˆglich und nˆtig. CType(Src, Double).ToString.Replace(TB.Math.DecimalPoint, ".")
                        'DstObj = Double.Parse(CStr(TB.Math.myTrim(TB.Math.myVal(Src, False), Double.MinValue, Double.MaxValue))) ' Mod (CDbl(Double.MaxValue) + 0))) '0 da kein grˆsserer type mˆglich und nˆtig. CType(Src, Double).ToString.Replace(TB.Math.DecimalPoint, ".")
                        DstObj = TB.Math.myTrim(TB.Math.myVal(Src, False), Double.MinValue, Double.MaxValue)
                    Case GetType(Point).Name
                        If Src.Contains(";") Then DstObj = New Point(TB.Math.myVal(Src.Split(";")(0), RemChar), TB.Math.myVal(Src.Split(";")(1), RemChar))
                        'DstObj = String.Format("{0:0};{1:0}", Pt.X, Pt.Y)
                    Case GetType(PointF).Name
                        If Src.Split(";").Length >= 2 Then
                            DstObj = New PointF(TB.Math.myVal(Src.Split(";")(0), RemChar), TB.Math.myVal(Src.Split(";")(1), RemChar))
                        ElseIf Src.Split(",").Length >= 2 Then
                            '{Width=640, Height=480}
                            DstObj = New PointF(TB.Math.myVal(Src.Split(",")(0), RemChar), TB.Math.myVal(Src.Split(",")(1), RemChar))
                        End If
                        'DstObj = String.Format("{0:0.00000000};{1:0.00000000}", pt.X, pt.Y)
                    Case GetType(Rectangle).Name
                        If Src.Split(";").Length >= 4 Then
                            DstObj = New Rectangle(TB.Math.myVal(Src.Split(";")(0), RemChar), TB.Math.myVal(Src.Split(";")(1), RemChar), TB.Math.myVal(Src.Split(";")(2), RemChar), TB.Math.myVal(Src.Split(";")(3), RemChar))
                        ElseIf Src.Split(",").Length >= 4 Then
                            DstObj = New Rectangle(TB.Math.myVal(Src.Split(",")(0), RemChar), TB.Math.myVal(Src.Split(",")(1), RemChar), TB.Math.myVal(Src.Split(",")(2), RemChar), TB.Math.myVal(Src.Split(",")(3), RemChar))
                        End If
                        'DstObj = String.Format("{0:0};{1:0};{2:0};{3:0};", R.X, R.Y, R.Width, R.Height)
                    Case GetType(RectangleF).Name
                        If Src.Split(";").Length >= 4 Then
                            'incedeible fault by using false index
                            DstObj = New RectangleF(TB.Math.myVal(Src.Split(";")(0), RemChar), TB.Math.myVal(Src.Split(";")(1), RemChar), TB.Math.myVal(Src.Split(";")(2), RemChar), TB.Math.myVal(Src.Split(";")(3), RemChar))
                        ElseIf Src.Split(",").Length >= 4 Then
                            DstObj = New RectangleF(TB.Math.myVal(Src.Split(",")(0), RemChar), TB.Math.myVal(Src.Split(",")(1), RemChar), TB.Math.myVal(Src.Split(",")(2), RemChar), TB.Math.myVal(Src.Split(",")(3), RemChar))
                        End If
                        'DstObj = String.Format("{0:0.00000000};{1:0.00000000};{2:0.000000000};{3:0.00000000};", R.X, R.Y, R.Width, R.Height)
                    Case GetType(Size).Name
                        'If Src.Contains(";") Then DstObj = New Size(TB.Math.myVal(Src.Split(";")(0)), TB.Math.myVal(Src.Split(";")(1)))
                        If Src.Split(";").Length >= 2 Then
                            DstObj = New Size(TB.Math.myVal(Src.Split(";")(0), RemChar), TB.Math.myVal(Src.Split(";")(1), RemChar))
                        ElseIf Src.Split(",").Length >= 2 Then
                            '{Width=640, Height=480}
                            DstObj = New Size(TB.Math.myVal(Src.Split(",")(0), RemChar), TB.Math.myVal(Src.Split(",")(1), RemChar))
                        End If
                        'DstObj = String.Format("{0:0};{1:0}", S.Width, S.Height)
                    Case GetType(SizeF).Name
                        'If Src.Contains(";") Then DstObj = New SizeF(TB.Math.myVal(Src.Split(";")(0)), TB.Math.myVal(Src.Split(";")(1)))
                        If Src.Split(";").Length >= 2 Then
                            DstObj = New SizeF(TB.Math.myVal(Src.Split(";")(0), RemChar), TB.Math.myVal(Src.Split(";")(1), RemChar))
                        ElseIf Src.Split(",").Length >= 2 Then
                            '{Width=640, Height=480}
                            DstObj = New SizeF(TB.Math.myVal(Src.Split(",")(0), RemChar), TB.Math.myVal(Src.Split(",")(1), RemChar))
                        End If
                        'DstObj = String.Format("{0:0.00000000};{1:0.00000000}", S.Width, S.Height)
                    Case GetType(IntPtr).Name
                        'DstObj = CType(Src, IntPtr).ToInt64.ToString("0")
                        DstObj = New IntPtr(CInt(TB.Math.myVal(Src, False) Mod (CLng(Integer.MaxValue) + 1))) 'CType(Src, Integer).ToString("0")
                    Case GetType(Color).Name
                        'DstObj = CType(Src, Color).ToArgb.ToString("0")
                        If Src.Contains(";") Then
                            DstObj = Color.FromArgb(CInt(TB.Math.myVal(Src.Split(";")(0), True)), CInt(TB.Math.myVal(Src.Split(";")(1), True)), CInt(TB.Math.myVal(Src.Split(";")(2), True)), CInt(TB.Math.myVal(Src.Split(";")(3), True)))
                        ElseIf Src.Contains(",") Then
                            DstObj = Color.FromArgb(CInt(TB.Math.myVal(Src.Split(",")(0), True)), CInt(TB.Math.myVal(Src.Split(",")(1), True)), CInt(TB.Math.myVal(Src.Split(",")(2), True)), CInt(TB.Math.myVal(Src.Split(",")(3), True)))
                        Else
                            'default
                            DstObj = Color.FromArgb(CInt(TB.Math.myVal(Src, False) Mod (CLng(Integer.MaxValue) + 1)))
                        End If
                    Case GetType(DateTime).Name
                        Dim lValue As Long = 0
                        If Long.TryParse(Src, lValue) Then
                            DstObj = New DateTime(lValue)
                        Else
                            DstObj = New DateTime(0)
                        End If
                        ' DstObj = New DateTime(TB.Math.myVal(Src, True))
                    Case GetType(TimeSpan).Name
                        Dim lValue As Long = 0
                        If Long.TryParse(Src, lValue) Then
                            DstObj = New TimeSpan(lValue)
                        Else
                            DstObj = New TimeSpan(0)
                        End If
                        '  DstObj = New TimeSpan(TB.Math.myVal(Src, True))

                        'specials
                    Case GetType(Char).Name 'Char
                        'DstObj = Strings.AscW(CType(Src, Char)).ToString("0")
                        DstObj = Global.Microsoft.VisualBasic.Strings.ChrW(CInt(TB.Math.myVal(Src, False) Mod (CLng(Integer.MaxValue) + 1))) 'integer
                    Case GetType(Drawing2D.GraphicsPath).Name 'Drawing.GraphicsPath
                        'Dim L As New List(Of String)
                        'For Each P As PointF In CType(Src, Drawing2D.GraphicsPath).PathPoints
                        '    L.Add(TypeToString(P))
                        'Next
                        'DstObj = String.Join(";", L.ToArray)
                        Dim L As New List(Of PointF)
                        Dim P As PointF
                        Dim i As Integer
                        For i = 0 To Src.Split(";").Length - 1 Step 2
                            P = StringToType(Src.Split(";")(i) & ";" & Src.Split(";")(i + 1), P)
                            L.Add(P)
                        Next

                        Dim GP As New Drawing2D.GraphicsPath
                        GP.AddLines(L.ToArray)
                        DstObj = GP

                    Case GetType(IO.FileInfo).Name
                        DstObj = New IO.FileInfo(Src)
                    Case GetType(IO.DirectoryInfo).Name
                        DstObj = New IO.DirectoryInfo(Src)

                    Case Else
                        'Interfaces
                        If TypeOf DstObj Is System.Collections.IList Then
                            Dim T As Type
                            T = System.ComponentModel.TypeDescriptor.GetReflectionType(DstObj)


                            If TypeOf DstObj Is List(Of Integer) Then
                                Dim L As New List(Of Integer)
                                For Each W As String In Src.Split("|")
                                    Dim value As Integer
                                    StringToType(W, value)
                                    L.Add(value)
                                Next
                            End If

                            If TypeOf DstObj Is List(Of Point) Then
                                Dim L As New List(Of Point)
                                For Each W As String In Src.Split("|")
                                    Dim value As Point
                                    StringToType(W, value)
                                    L.Add(value)
                                Next
                            End If

                            If TypeOf DstObj Is List(Of PointF) Then
                                Dim L As New List(Of PointF)
                                For Each W As String In Src.Split("|")
                                    Dim value As PointF
                                    StringToType(W, value)
                                    L.Add(value)
                                Next
                            End If


                            If TypeOf DstObj Is List(Of Single) Then
                                Dim L As New List(Of Single)
                                For Each W As String In Src.Split("|")
                                    Dim value As Single
                                    StringToType(W, value)
                                    L.Add(value)
                                Next
                            End If


                            If TypeOf DstObj Is List(Of Double) Then
                                Dim L As New List(Of Double)
                                For Each W As String In Src.Split("|")
                                    Dim value As Double
                                    StringToType(W, value)
                                    L.Add(value)
                                Next
                            End If


                            If TypeOf DstObj Is List(Of Rectangle) Then
                                Dim L As New List(Of Rectangle)
                                For Each W As String In Src.Split("|")
                                    Dim value As Rectangle
                                    StringToType(W, value)
                                    L.Add(value)
                                Next
                            End If


                            If TypeOf DstObj Is List(Of RectangleF) Then
                                Dim L As New List(Of RectangleF)
                                For Each W As String In Src.Split("|")
                                    Dim value As RectangleF
                                    StringToType(W, value)
                                    L.Add(value)
                                Next
                            End If



                            If TypeOf DstObj Is List(Of Size) Then
                                Dim L As New List(Of Size)
                                For Each W As String In Src.Split("|")
                                    Dim value As Size
                                    StringToType(W, value)
                                    L.Add(value)
                                Next
                            End If


                            If TypeOf DstObj Is List(Of SizeF) Then
                                Dim L As New List(Of SizeF)
                                For Each W As String In Src.Split("|")
                                    Dim value As SizeF
                                    StringToType(W, value)
                                    L.Add(value)
                                Next
                            End If

                            ''Dim L As New List(Of System.Collections.IList)

                            'For Each W As String In Src.Split(";")
                            '    Dim Obj As Object
                            '    System.ComponentModel.TypeDescriptor.GetConverter(Obj.GetType).ConvertFrom(Obj)
                            '    StringToType(W, Obj)
                            '    L.Add(Obj)
                            'Next
                            'L = CType(Src, System.Collections.IList)
                            'For Each O As Object In L
                            '    DstObj = DstObj & TypeToString(O) & ";"
                            'Next

                        ElseIf TypeOf DstObj Is System.Collections.IDictionary Then
                            'ToDo: support of Dictionary
                            'not supported
                            If TypeOf DstObj Is Dictionary(Of String, Object) Then
                                Dim dic As New Dictionary(Of String, Object)
                                If String.IsNullOrEmpty(Src) = False Then
                                    For dIndex As Integer = 0 To Src.Split("|").Length - 1 Step 2
                                        Dim Key As String = Src.Split("|")(dIndex)
                                        Dim sObj As String = Src.Split("|")(dIndex + 1)
                                        Dim Obj As Object = Nothing
                                        Call StringToType(sObj, Obj)
                                        If dic.ContainsKey(Key) = False Then
                                            dic.Add(Key, Obj)
                                        End If
                                    Next
                                End If

                                DstObj = dic
                            Else
                                DstObj = Src.ToString
                            End If

                        ElseIf TypeOf DstObj Is Pen Then
                            Dim P As New Pen(Color.Empty)
                            Dim i As Integer
                            P.Color = StringToType(Src.Split(";")(0), i)
                            P.Width = StringToType(Src.Split(";")(1), i)
                            P.DashStyle = CType(StringToType(Src.Split(";")(2), i), Drawing2D.DashStyle)
                            DstObj = P
                            'res = TypeToString(P.Color) & ";" & TypeToString(P.Width) & ";" & TypeToString(CInt(P.DashStyle))

                        ElseIf TypeOf DstObj Is SolidBrush Then
                            Dim B As New SolidBrush(Color.Empty)
                            Dim i As Integer
                            'B.Color = CType(StringToType(Src, i), Color)
                            B = New SolidBrush(Color.FromArgb(CType(StringToType(Src, i), Integer)))


                        ElseIf DstObj.GetType.IsEnum = True Then
                            DstObj = Global.System.Convert.ChangeType(DstObj, System.Enum.GetUnderlyingType(DstObj.GetType))
                            DstObj = StringToType(Src, DstObj)
                        Else

                            'enumerator

                            DstObj = Nothing 'Src.ToString

                        End If


                End Select

                Return DstObj
            End Function



            ''' <summary>
            ''' F¸gt einem Dictionary ein Objekt zu mit R¸cksicht auf ein bereits vorhandenes Objekt in der Liste.
            ''' </summary>
            Public Sub AddToDic(ByVal di As Dictionary(Of String, Object), ByVal Key As String, ByVal Value As Object)
                If di Is Nothing Then di = New Dictionary(Of String, Object)
                If di.ContainsKey(Key) = False Then
                    di.Add(Key, Value)
                Else
                    di(Key) = Value
                End If
            End Sub


            ''' <summary>
            ''' F¸gt einem Dictionary ein Objekt zu mit R¸cksicht auf ein bereits vorhandenes Objekt in der Liste.
            ''' </summary>
            Public Sub AddToDic(ByVal di As Dictionary(Of String, Byte()), ByVal Key As String, ByVal Value() As Byte)
                If di Is Nothing Then di = New Dictionary(Of String, Byte())
                If di.ContainsKey(Key) = False Then
                    di.Add(Key, Value)
                Else
                    di(Key) = Value
                End If
            End Sub


            ''' <summary>
            ''' F¸gt einem Dictionary ein Objekt zu mit R¸cksicht auf ein bereits vorhandenes Objekt in der Liste.
            ''' </summary>
            Public Sub AddToDic(ByVal di As Dictionary(Of String, String), ByVal Key As String, ByVal Value As String)
                If di Is Nothing Then di = New Dictionary(Of String, String)
                If di.ContainsKey(Key) = False Then
                    di.Add(Key, Value)
                Else
                    di(Key) = Value
                End If
            End Sub

           



            ''' <summary>
            ''' Create a copy from a source object
            ''' </summary>
            ''' <param name="SrcObject"></param>
            ''' <param name="DstObject"></param>
            ''' <remarks></remarks>
            Public Sub CloneObject(ByVal SrcObject As Object, ByRef DstObject As Object)
                Dim mS As New IO.MemoryStream
                Dim B As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                B.Serialize(mS, SrcObject)
                mS.Seek(0, IO.SeekOrigin.Begin)
                DstObject = B.Deserialize(mS)
                mS.Close()
            End Sub


 

 

        End Module

    End Namespace


End Namespace
