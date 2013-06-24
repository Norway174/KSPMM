Imports System.Windows.Forms
Imports Microsoft.Win32

Public Class WelcomeScreen
    Sub loadpage() Handles Me.Shown
        WebBrowser1.Navigate("http://kspmm.norway174.com/BehindTheScenes/welcomePage/")
    End Sub

    Dim hasloaded = 1
    Sub updatepage() Handles WebBrowser1.DocumentCompleted
        If hasloaded = True Then
            hasloaded = False
            Exit Sub
        ElseIf hasloaded = 1 Then
            hasloaded = False
        Else
            hasloaded = True
        End If


        Dim txt As String = WebBrowser1.DocumentText

        txt = txt.Replace("<kspmm: version>", My.Application.Info.Version.ToString())

        WebBrowser1.DocumentText = txt
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Sub DontShowAgain() Handles CheckBox1.CheckedChanged
        CheckIfKeyExists()
        If CheckBox1.Checked = True Then
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Norway174\KSPMM", "StartUpWindow", True)
        Else
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Norway174\KSPMM", "StartUpWindow", False)
        End If
    End Sub

    Sub CheckIfKeyExists()
        Dim newKey As RegistryKey
        newKey = My.Computer.Registry.CurrentUser.CreateSubKey("Norway174\KSPMM")
    End Sub

    Sub SetCheck() Handles Me.Load
        Try
            If My.Computer.Registry.CurrentUser.OpenSubKey("Norway174\KSPMM") IsNot Nothing Then
                Dim keyValue As Object
                keyValue = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Norway174\KSPMM", "StartUpWindow", "True")
                CheckBox1.Checked = keyValue
            End If
        Finally
            My.Computer.Registry.CurrentUser.Close()
        End Try

    End Sub

End Class
