Imports Microsoft.Win32

Public Class Settings

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Sub debug() Handles Button2.Click
        MsgBox(checkKSPMMFiletype)
    End Sub

    Sub Opened() Handles Me.Load
        CheckBox1.Checked = checkKSPMMFiletype()
    End Sub

    Sub InstallKSPMMFiletype() Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            Dim newKey As RegistryKey
            newKey = My.Computer.Registry.CurrentUser.CreateSubKey("Software\Classes\.kspmm")

            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Classes\.kspmm", "", "KSPMM")

            Dim newKey1 As RegistryKey
            newKey1 = My.Computer.Registry.CurrentUser.CreateSubKey("Software\Classes\KSPMM")

            Dim newKey2 As RegistryKey
            newKey2 = My.Computer.Registry.CurrentUser.CreateSubKey("Software\Classes\KSPMM\DefaultIcon")

            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Classes\KSPMM\DefaultIcon", "", """" & Application.ExecutablePath & """,1")

            Dim newKey3 As RegistryKey
            newKey3 = My.Computer.Registry.CurrentUser.CreateSubKey("Software\Classes\KSPMM\shell\open\command")

            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Classes\KSPMM\shell\open\command", "", """" & Application.ExecutablePath & """ ""%1""")

        Else

            Using key As RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey("Software\Classes")
                key.DeleteSubKey(".kspmm")
                key.DeleteSubKey("KSPMM\shell")
            End Using


        End If
    End Sub
End Class