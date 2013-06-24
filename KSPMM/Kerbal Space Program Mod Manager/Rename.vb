Imports Ionic.Zip
Imports System.IO

Public Class Rename
    Private _modLocation As String

    Public Property [modLocation]() As String
        Get
            Return _modLocation
        End Get
        Set(ByVal Value As String)
            _modLocation = Value
        End Set
    End Property

    Dim oldModName As String
    Dim oldInfoName As String
    Dim newModName As String
    Dim newInfoName As String


    Sub LoadName() Handles Me.Load

        Dim rnd As String = Path.GetRandomFileName
        rnd = rnd.Substring(0, rnd.LastIndexOf("."))
        Dim tempfolder As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\KSPMods\" & rnd & "\"

        My.Computer.FileSystem.CreateDirectory(tempfolder)

        Dim ZipToUnpack As String = "C1P3SML.zip"
        Dim UnpackDirectory As String = tempfolder
        Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
            Dim e As ZipEntry
            ' here, we extract every entry, but we could extract conditionally,
            ' based on entry name, size, date, checkbox status, etc.   
            For Each e In zip1
                If e.FileName.ToLower.EndsWith("info.txt") Then
                    e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)

                End If
            Next
        End Using
    End Sub

End Class