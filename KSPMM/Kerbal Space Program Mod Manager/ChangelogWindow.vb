Public Class ChangelogWindow

    Sub Loading() Handles Me.Shown, Button2.Click
        TextBox1.Text = "Loading..."

        GetLog()
    End Sub

    Sub GetLog()
        Try
            Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(Main.Changelog)
            Dim response As System.Net.HttpWebResponse = request.GetResponse()

            Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())

            Dim sourcecode As String = sr.ReadToEnd()

            'sourcecode only seems to contain LF's not CRLF so doesn't display multiline properly
            'Do this in case the file reversts for some reason
            sourcecode = sourcecode.Replace(vbCrLf, vbCr)
            'now replace all the CRs with crlf soe the text box looks right
            TextBox1.Text = sourcecode.Replace(vbLf, vbCrLf)

        Catch ex As Exception
            TextBox1.Text = "Uanble to connect. Try again." & vbNewLine & vbNewLine & _
            "Error message; " & ex.ToString
        End Try

    End Sub

    Sub CloseLog() Handles Button1.Click
        Me.Close()
    End Sub
End Class