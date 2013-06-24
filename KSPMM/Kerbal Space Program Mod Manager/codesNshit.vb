Imports System.Windows.Forms
Imports System.IO
Imports Microsoft.Win32

Module codesNshit

    Public Function GetFileNameFromURL(ByVal URL As String) As String
        Try
            URL = URL.Replace("%20", " ")
            Return URL.Substring(URL.LastIndexOf("/") + 1)
        Catch ex As Exception
            Return URL
        End Try
    End Function

    Public Sub WriteValueToFile(ByVal FileToWrite As String, ByVal ValueName As String, ByVal ValueToStore As String, _
                                Optional ByVal Append As Boolean = True)
        ValueToStore = ValueToStore.Replace("=", "$€1")
        My.Computer.FileSystem.WriteAllText(FileToWrite, ValueName.ToLower & "=" & ValueToStore & vbNewLine, Append)

    End Sub

    Public Function ReadValueFromFile(ByVal FileToRead As String, Optional ByVal ValueToRead As String = "all", _
                                     Optional ByVal IsInteger As Boolean = False)

        Dim lsb As New ListBox

        ' Open the file to read from.
        Dim readText() As String = File.ReadAllLines(FileToRead)
        Dim s As String
        For Each s In readText
            lsb.Items.Add(s)
        Next

        'Split and sort all the files'
        Dim All As String = ""
        For Each tx As String In lsb.Items
            Dim key As String
            Dim value As String
            Dim Split(1) As String

            Split = tx.Split("=")

            If Split(1) = "" Or Split(1) = "%No Value%" Then
                If IsInteger = True Then
                    Dim st As String = 0
                    Split(1) = st
                Else
                    Dim st As String = "%No Value%"
                    Split(1) = st
                End If
            End If

            key = Split(0)
            value = Split(1)

            value = value.Replace("$£1", "=")

            If ValueToRead.ToLower = "all" Then
                All = All & value & vbNewLine
            Else
                If key.ToLower = ValueToRead.ToLower Then
                    Return value
                    End
                End If
            End If
        Next
        If ValueToRead.ToLower = "all" Then
            Return All
            End
        End If
        If IsInteger = True Then
            Return 0
            End
        End If
        Return "%Not found%"
    End Function

    Public Function checkKSPMMFiletype()

        Try
            If My.Computer.Registry.CurrentUser.OpenSubKey("Software\Classes\.kspmm") IsNot Nothing Then

                If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\Classes\KSPMM\shell\open\command", "", "") = """" & Application.ExecutablePath & """ ""%1""" Then

                    Return True

                End If

            End If
        Finally
            My.Computer.Registry.ClassesRoot.Close()
        End Try

        Return False
    End Function
    Sub InstallFileType()
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

        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Classes\KSPMM\", "URL Protocol", "")

    End Sub
End Module