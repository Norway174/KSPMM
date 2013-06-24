Imports System.IO
Imports Ionic.Zip
Imports System.Threading
Imports System.Net
Imports System.Text
Imports System
Imports System.Net.Sockets
Imports Microsoft.Win32
Imports System.Collections.ObjectModel
Imports Microsoft.VisualBasic.ApplicationServices

Public Class Main

#Region "Declarations"

    Public Root As String = Application.StartupPath
    Public RootMODS As String = Root & "/--MODS--/"
    Public RootInstalled As String = RootMODS & "installed/"
    Public RootGameData As String = Root & "/GameData/"
    Public Key As String = "Norway174\KSPMM"

    Public ReadOnly CheckForUpdates As String = _
        "http://kspmm.norway174.com/BehindTheScenes/Versions/VersionControl.php?print"
    Public ReadOnly SessionsPage As String = _
        "http://kspmm.norway174.com/BehindTheScenes/Sessions/Sessions.php"
    Public ReadOnly ChatPage As String = _
        "http://kspmm.norway174.com/BehindTheScenes/Chat/technical.php"

    Public ReadOnly Changelog As String = _
        "http://kspmm.norway174.com/changelog/changelog.txt"

    Public UpdateEnabled = True
    Public EnableSessions = True
    Public EnableChat = True

#End Region

#Region "Drag and drop function"
    Private Sub Panel1_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles DataGridView1.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim filePaths As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
            For Each fileLoc As String In filePaths
                AddNewFile(fileLoc)
            Next
        End If

    End Sub

    Private Sub Panel1_DragOver(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles DataGridView1.DragOver

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim filePaths As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
            Dim FilePth As String = Path.GetExtension(filePaths(0))
            If FilePth.ToLower = ".zip".ToLower Or FilePth.ToLower = ".KSPMM".ToLower Then
                e.Effect = DragDropEffects.Move
            End If
        End If

    End Sub
#End Region

#Region "SetStatus(Status)"
    Sub MeLoaded() Handles Me.Shown
        SetStatus("Kerbal Mod Manager Initialized! Loaded " & DataGridView1.RowCount.ToString & " mods.")
    End Sub

    Sub SetStatus(ByVal Status As String, Optional ByVal miliseconds As Integer = 5000)

        'Is the current operation one that affects multiple mods
        If blnMultiModActionInProgress Then
            Status = MultiModMessage(intMultiMod_Total, intMultiMod_Current) & Status
        End If

        StatusLabel.Text = Status
        If miliseconds = 0 Then
            Application.DoEvents()
        Else
            wait(miliseconds)
            StatusLabel.Text = ""
        End If
    End Sub
    Private Sub wait(ByVal interval As Integer)
        Dim sw As New Stopwatch
        sw.Start()
        Do While sw.ElapsedMilliseconds < interval
            ' Allows UI to remain responsive
            Application.DoEvents()
        Loop
        sw.Stop()
    End Sub

    'Function to generate a prefix for status mesage when multimods are underway
    Private Function MultiModMessage(TotalMods As Integer, CurrentMod As Integer) As String
        Dim strReturn As String = ""

        If TotalMods > 1 Then
            strReturn = String.Format("Applying Multiple Mods {0} of {1}-", CurrentMod, TotalMods)
        End If
        Return strReturn
    End Function

#End Region

#Region "Search"
    Private Sub ToolStripButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton2.Click
        ToolStripTextBox1.Text = "Search!"
        ToolStripTextBox1.ForeColor = Color.Gray
        DataGridView1.DataSource = DataSet1.Tables("Mods")
    End Sub

    Private Sub SearchBox_TextChanged() Handles ToolStripTextBox1.TextChanged
        If ToolStripTextBox1.Text = "Search!" And ToolStripTextBox1.ForeColor = Color.Gray Then
            DataGridView1.DataSource = DataSet1.Tables("Mods")
        Else
            DataGridView1.DataSource = DataSet1.Tables("Mods").Select("Title LIKE '*" & ToolStripTextBox1.Text & "*'")
        End If
    End Sub

    Private Sub SearchBoxEnter() Handles ToolStripTextBox1.Enter
        If SearchBoxEmpty() = False Then
            ToolStripTextBox1.Text = ""
            ToolStripTextBox1.ForeColor = Color.Black
        End If
    End Sub

    Private Sub SearchBoxLeave() Handles ToolStripTextBox1.Leave
        If SearchBoxEmpty() = True Then Exit Sub
        ToolStripTextBox1.Text = "Search!"
        ToolStripTextBox1.ForeColor = Color.Gray
        SearchBox_TextChanged()
    End Sub

    Function SearchBoxEmpty()
        If ToolStripTextBox1.Text <> "" And ToolStripTextBox1.ForeColor <> Color.Gray Then
            Return True
        Else
            Return False
        End If
    End Function

#End Region

#Region "Div. Importance"

    Sub AddToList(ByVal title, ByVal version, ByVal enabled, Optional ByVal location1 = "")
        Dim newModsRow As DataSet1.modsRow
        newModsRow = DataSet1.mods.NewmodsRow()

        newModsRow.title = title
        newModsRow.version = version
        newModsRow.Enabled = enabled
        newModsRow.location = location1

        DataSet1.mods.Rows.Add(newModsRow)

    End Sub

    Sub CheckKSPexe() Handles Me.Load
        Dim fileExists As Boolean
        fileExists = My.Computer.FileSystem.FileExists(Root & "\KSP.exe")
        ToolStripButton1.Enabled = fileExists
        LaunchKSPToolStripMenuItem.Enabled = fileExists
        If fileExists = False Then
            ToolStripButton1.ToolTipText = "Could not find KSP.exe"
            LaunchKSPToolStripMenuItem.ToolTipText = "Could not find KSP.exe"
        End If

        If checkKSPMMFiletype() = False Then
            If Dialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                InstallFileType()
            End If
        End If

        Try
            If My.Computer.Registry.CurrentUser.GetValue(Key, "FMKey", Nothing) IsNot Nothing Then
                Dim Regkey As RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey(Key, True)
                Regkey.DeleteValue("FMKey")
                MsgBox("Key Deleted")
                Regkey.Close()
            End If
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "Mod Trackers/Selectors"

    Sub UpdateModscount() Handles DataGridView1.RowsAdded, DataGridView1.RowsRemoved, Me.Load
        Modscounter.Text = DataGridView1.RowCount.ToString & " # Mods"
    End Sub

    Private Sub DataGridView1_CellMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DataGridView1.CellMouseDown
        If e.RowIndex = -1 Then Exit Sub
        If e.Button = MouseButtons.Right Then
            DataGridView1.CurrentCell = DataGridView1(e.ColumnIndex, e.RowIndex)
        End If
    End Sub

    Sub SelectionChanged() Handles DataGridView1.SelectionChanged, DataGridView1.CurrentCellChanged, Me.Load
        If DataGridView1.RowCount <= 0 Then
            FastEnableBtn.Text = "Enable"
            FastEnableBtn.Enabled = False
            EnableDisableBtn.Text = "Enable"
            EnableDisableBtn.Enabled = False
            FastDeleteBtn.Enabled = False
            Deletebtn.Enabled = False
            Exit Sub
        Else
            FastEnableBtn.Enabled = True
            EnableDisableBtn.Enabled = True
        End If
        If DataGridView1.SelectedRows.Count <= 0 Then
            Exit Sub
        End If

        Dim Enabled1 As String = CType(DataGridView1.Item(3, DataGridView1.SelectedRows.Item(0).Index).Value, String)
        If Enabled1 = "Enabled" Then
            FastEnableBtn.Text = "Disable"
            EnableDisableBtn.Text = "Disable"
            FastDeleteBtn.Enabled = False
            Deletebtn.Enabled = False
        ElseIf Enabled1 = "Download" Then
            FastEnableBtn.Text = "Download"
            EnableDisableBtn.Text = "Download"
            FastDeleteBtn.Enabled = False
            Deletebtn.Enabled = False
        Else
            FastEnableBtn.Text = "Enable"
            EnableDisableBtn.Text = "Enable"
            FastDeleteBtn.Enabled = True
            Deletebtn.Enabled = True
        End If

        If Enabled1 = "Stock" Then
            FastEnableBtn.Enabled = False
            EnableDisableBtn.Enabled = False
            FastDeleteBtn.Enabled = True
            Deletebtn.Enabled = True
        Else
            FastEnableBtn.Enabled = True
            EnableDisableBtn.Enabled = True
        End If

    End Sub

    Private Sub ContextMenuStrip1_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles DataGridView1.DataMemberChanged
        If DataGridView1.SelectedCells.Count = 0 Then
            EnableDisableBtn.Enabled = False
            Deletebtn.Enabled = False
            FastDeleteBtn.Enabled = False
            FastEnableBtn.Enabled = False
        Else
            EnableDisableBtn.Enabled = True
            Deletebtn.Enabled = True
            FastDeleteBtn.Enabled = True
            FastEnableBtn.Enabled = True
        End If
    End Sub

#End Region

#Region "Load Mods (Core)"

    Sub LoadMods() Handles Me.Load, ReloadMods.Click
        If downloadingFile = True Then Exit Sub

        Dim folderExists As Boolean
        folderExists = My.Computer.FileSystem.DirectoryExists(RootMODS)
        'Dim folderExists2 As Boolean
        'folderExists2 = My.Computer.FileSystem.DirectoryExists(RootInstalled)
        If folderExists = False Then
            If (MsgBox("Mods folder does not exist, do you wish to create it?" & vbNewLine & _
                       "This is required for the program to run.", MsgBoxStyle.YesNo) = MsgBoxResult.Yes) Then
                My.Computer.FileSystem.CreateDirectory(RootMODS)
                'My.Computer.FileSystem.CreateDirectory(RootInstalled)
            Else
                Application.Exit()
            End If
        End If



        AddToList("*DUMMY*", "*DUMMY*", "*DUMMY*", "*DUMMY*")


        Dim selected As Integer = 0
        If DataGridView1.RowCount >= 1 Then
            selected = DataGridView1.CurrentRow.Index
        End If


        DataSet1.Clear()


        '*****LOAD THE FEATURED MOD AT THE TOP OF THE LIST******
        'BackgroundWorker1_DoWork()
        'If FMInstalled = False Then
        '    Dim TrueFMName As String = GetFileNameFromURL(FeaturedURL)
        '    TrueFMName = TrueFMName.Replace(".zip", "")
        '    TrueFMName = TrueFMName.Replace("kspmm", "")
        '    AddToList(TrueFMName & " (" & FeaturedName & ")", FeaturedVersion, "Download", FeaturedURL)
        'End If


        For Each folder In My.Computer.FileSystem.GetDirectories(RootGameData)
            Dim existZip As Boolean = False
            existZip = My.Computer.FileSystem.FileExists(RootMODS & Path.GetFileName(folder) & ".zip")
            Dim existKspmm As Boolean = False
            existKspmm = My.Computer.FileSystem.FileExists(RootMODS & Path.GetFileName(folder) & ".kspmm")

            If existZip = False And existKspmm = False Then
                If folder.ToLower.EndsWith("squad") Then
                    AddToList("Squad's Default Stock Parts", "KSP Version", "Stock", folder)
                Else
                    AddToList(Path.GetFileName(folder), "Unknown", "Enabled", folder)
                End If

            End If

        Next

        If My.Computer.FileSystem.GetFiles(RootMODS).Count = 0 Then Exit Sub

        Dim rnd As String = Path.GetRandomFileName
        rnd = rnd.Substring(0, rnd.LastIndexOf("."))
        Dim tempfolder As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\KSPMods\" & rnd & "\"

        My.Computer.FileSystem.CreateDirectory(tempfolder)

        Dim files As ReadOnlyCollection(Of String)

        files = My.Computer.FileSystem.GetFiles(RootMODS, FileIO.SearchOption.SearchTopLevelOnly, "*.*")

        For Each fle In files

            Dim Filename As String = Path.GetFileNameWithoutExtension(fle)
            Dim Filetype As String = Path.GetExtension(fle)
            Dim version As String = "Unknown"
            Dim Enabled1 As String = IsInstalled(Filename)

            Dim InfoFound As Boolean = False
            Dim Accepted As Boolean = False

            If Filetype.ToLower = ".zip" Or Filetype.ToLower = ".kspmm" Then
                Dim ZipToUnpack As String = Path.GetFullPath(fle)
                Dim UnpackDirectory As String = tempfolder
                Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
                    Dim e As ZipEntry
                    ' here, we extract every entry, but we could extract conditionally,
                    ' based on entry name, size, date, checkbox status, etc.   
                    For Each e In zip1

                        If e.FileName.ToLower.EndsWith("info.txt") Then
                            e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)
                            Debug.Print("Found Info")
                            InfoFound = True
                        End If

                    Next
                End Using
                Accepted = True
            ElseIf Filetype.ToLower = ".rar" Then
                version = "RAR"
                'Decides where or not to include the RAR files in the list.
                Accepted = False
            End If

            If InfoFound = True Then
                Dim Infotxtloc = ""
                For Each inf In My.Computer.FileSystem.GetFiles(tempfolder, FileIO.SearchOption.SearchAllSubDirectories)
                    If Path.GetFileName(inf).ToLower = "info.txt" Then
                        Infotxtloc = Path.GetFullPath(inf)
                        Exit For
                    End If
                Next
                If Infotxtloc IsNot "" Then
                    version = ReadValueFromFile(Infotxtloc, "Version")
                    Dim TrueFilename = ReadValueFromFile(Infotxtloc, "Title")
                    If TrueFilename = "%No Value%" Or TrueFilename = "%Not found%" Then
                    Else
                        If Filename.EndsWith("_%&OUTDATED%&") Then
                            Filename = TrueFilename & " (Outdated)"
                        Else
                            Filename = TrueFilename
                        End If
                    End If
                End If
            End If
            If Accepted = True Then
                Filename = Filename.Replace("_%&OUTDATED%&", " (Outdated!)")
                AddToList(Filename, version, Enabled1, Path.GetFullPath(fle))
            End If
        Next



        DataGridView1.Focus()
        Try
            Me.DataGridView1.CurrentCell = Me.DataGridView1(0, selected)
        Catch ex As Exception
            Me.DataGridView1.CurrentCell = Me.DataGridView1(0, selected - 1)
        End Try

        My.Computer.FileSystem.DeleteDirectory(tempfolder, FileIO.DeleteDirectoryOption.DeleteAllContents)


        Dim int As Integer = 0
        For Each lne As DataGridViewRow In DataGridView1.Rows
            Dim Enabled1 As String = CType(lne.DataGridView.Item(3, DataGridView1.Rows.Item(int).Index).Value, String)
            int = int + 1

            Debug.Print(Enabled1)
            If Enabled1 = "Enabled" Then
                lne.DefaultCellStyle.BackColor = Color.LimeGreen
            ElseIf Enabled1 = "Stock" Then
                lne.DefaultCellStyle.BackColor = Color.LightBlue
            Else
                'lne.DefaultCellStyle.BackColor = Color.PaleVioletRed
            End If
        Next

    End Sub

    Function IsInstalled(Filename)
        Dim fileExists As Boolean
        fileExists = My.Computer.FileSystem.FileExists(RootInstalled & Filename & ".txt")
        Dim fileExists2 As Boolean
        fileExists2 = My.Computer.FileSystem.DirectoryExists(RootGameData & Filename)
        If fileExists = True Or fileExists2 = True Then
            Return "Enabled"
            Exit Function
        Else
            Return "Disabled"
        End If

    End Function

#End Region

#Region "Mod adding & Enabling/disabling"

    Sub AddNewFile(ByVal FileLoc As String)
        If File.Exists(FileLoc) Then

            Dim FilePth As String = Path.GetFullPath(FileLoc)
            Dim Filenm As String = Path.GetFileName(FileLoc)

            Dim fileExists As Boolean = False
            fileExists = My.Computer.FileSystem.FileExists(RootMODS & Filenm)
            Dim OutdatedFileExists As Boolean = My.Computer.FileSystem.FileExists(RootMODS & Filenm & "_%&OUTDATED%&")
            If fileExists = True Then
                If OutdatedFileExists = True Then My.Computer.FileSystem.DeleteFile(RootMODS & Filenm & "_%&OUTDATED%&")
                My.Computer.FileSystem.RenameFile(RootMODS & Filenm, _
                                                  Path.GetFileNameWithoutExtension(RootMODS & Filenm) & _
                                                  "_%&OUTDATED%&" & Path.GetExtension(FilePth))

                My.Computer.FileSystem.RenameDirectory(RootGameData & Path.GetFileNameWithoutExtension(FilePth), _
                                                       Path.GetFileNameWithoutExtension(FilePth) & "_%&OUTDATED%&")

            End If
            My.Computer.FileSystem.CopyFile(FilePth, RootMODS & Filenm, True)
            DataSet1.Clear()
            LoadMods()
        End If
    End Sub

    Sub EnableDisableMod() Handles DataGridView1.CellMouseDoubleClick, EnableDisableBtn.Click, FastEnableBtn.Click
        Dim ModLocToEnable As String = CType(DataGridView1.Item(2, DataGridView1.SelectedRows.Item(0).Index).Value, String)
        Dim Enabled1 As String = CType(DataGridView1.Item(3, DataGridView1.SelectedRows.Item(0).Index).Value, String)

        If Enabled1 = "Enabled" Then
            SetStatus("Disabling mod!", 0)
            DeleteInstalledMod(ModLocToEnable)
        ElseIf Enabled1 = "Download" Then
            DownloadFile(ModLocToEnable)
        ElseIf Enabled1 = "Stock" Then
            'Nothing
        Else
            ExtractFiles(ModLocToEnable)
        End If


    End Sub

#End Region

#Region "Unpackaging mod (Core)"

    Sub ExtractFiles(ByVal ModZipFile)
        CheckForIllegalCrossThreadCalls = False
        Dim rnd As String = Path.GetRandomFileName
        rnd = rnd.Substring(0, rnd.LastIndexOf("."))
        Dim tempfolder As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\KSPMods\" & rnd & "\"
        My.Computer.FileSystem.CreateDirectory(tempfolder)

        Dim virtualKSP As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\KSPVIRTUAL\" & rnd & "\"
        My.Computer.FileSystem.CreateDirectory(virtualKSP)

        Dim DateToday As String = Date.Now
        Dim FilesAddedTextFile As String = "[Generated on: " & DateToday & "]"
        Dim Modname As String = Path.GetFileNameWithoutExtension(ModZipFile)
        If Me.InvokeRequired = False Then
            ProgressBar12.Visible = True
            Me.Enabled = False
        End If
        Dim filesint As Integer = 1

        If ModZipFile.ToString.ToLower.EndsWith(".rar") Then

            'Temporarily added here for now.
            Me.Enabled = True
            Exit Sub

            UnRar(tempfolder, Path.GetFullPath(ModZipFile))
        Else
            'Extracting the files to the temp folder
            Using zip2 As ZipFile = ZipFile.Read(ModZipFile)
                'AddHandler zip2.ExtractProgress, AddressOf MyExtractProgress
                Dim e As ZipEntry
                ' here, we extract every entry, but we could extract conditionally,
                ' based on entry name, size, date, checkbox status, etc.  
                Application.DoEvents()

                For Each e In zip2
                    If e.FileName = "info.txt" Then Continue For
                    SetStatus("Packing out: " & e.FileName, 0)
                    Application.DoEvents()
                    e.Extract(tempfolder, ExtractExistingFileAction.OverwriteSilently)
                    filesint = filesint + 1
                    ProgressBar12.Maximum = zip2.Count + 1
                    ProgressBar12.Value = filesint
                    Application.DoEvents()
                Next
            End Using
        End If


        'Reading the folders & sorting which folder goes where
        Dim foldersF As New ListBox
        SetStatus("Reading TempFolder", 0)
        For Each fld In My.Computer.FileSystem.GetDirectories(tempfolder, FileIO.SearchOption.SearchAllSubDirectories)
            fld = fld.Replace(tempfolder, "\")

            If fld.Contains("\_") = True Then
                'Nothing
            ElseIf fld.EndsWith("textures") = True Or fld.EndsWith("texture") Then
                'Nothing
            ElseIf fld.EndsWith("mesh") = True Then
                'Nothing
            Else
                foldersF.Items.Add(fld)
            End If
        Next

        SetStatus("Scanning for parts", 0)
        'Dim ExisitingParts(foldersF.Items.Count - 1) As String
        Dim int As Integer = 0
        For Each I As String In foldersF.Items

            If IsPart(tempfolder & I) = True Then

                Dim Part As String = I
                Dim endnam As Integer = Part.LastIndexOf("\")
                If endnam - -1 Then Part = Part.Substring(endnam) Else Part = "\" & Part
                Part = virtualKSP & "\Parts" & Part
                I = tempfolder & I
                'If My.Computer.FileSystem.DirectoryExists(Part) = True Then 'Check if part exists!
                '    ExisitingParts(int) = Part
                '    int = int + 1
                '    Continue For
                'End If
                My.Computer.FileSystem.CopyDirectory(I, Part, True)
                'Part = Part.Replace(Root, "[Root]")
                'FilesAddedTextFile = FilesAddedTextFile & vbNewLine & Part
                int = int + 1
            End If
        Next

        'For Each itn In ExisitingParts
        '    'MsgBox(itn)
        'Next

        SetStatus("Scanning for Plugins", 0)
        Dim files As ReadOnlyCollection(Of String)
        files = My.Computer.FileSystem.GetFiles(tempfolder, FileIO.SearchOption.SearchAllSubDirectories, "*.dll")
        For Each dll In files
            dll = dll.Replace(tempfolder, "\")

            If dll.Contains("\_") = True Then
                'Nothing
            Else
                Dim Plugin As String = dll
                Dim endnam As Integer = Plugin.LastIndexOf("\")
                If endnam - -1 Then Plugin = Plugin.Substring(endnam) Else Plugin = "\" & Plugin
                Plugin = virtualKSP & "\Plugins" & Plugin
                dll = tempfolder & dll
                My.Computer.FileSystem.CopyFile(dll, Plugin, True)
                'Plugin = Plugin.Replace(Root, "[Root]")
                'FilesAddedTextFile = FilesAddedTextFile & vbNewLine & Plugin
            End If
        Next

        SetStatus("Reading .Craft-files", 0)
        Dim ships As ReadOnlyCollection(Of String)
        ships = My.Computer.FileSystem.GetFiles(tempfolder, FileIO.SearchOption.SearchAllSubDirectories, "*.craft")
        For Each craf In ships
            craf = craf.Replace(tempfolder, "\")

            If craf.Contains("\_") = False Then
                Dim CRType As String = ReadCraftType(tempfolder & craf)
                Dim Ship As String = Root & "\Ships\" & CRType & "\" & Path.GetFileName(craf)
                craf = tempfolder & craf
                My.Computer.FileSystem.CopyFile(craf, Ship, True)
                'Ship = Ship.Replace(Root, "[Root]")
                'FilesAddedTextFile = FilesAddedTextFile & vbNewLine & Ship
            End If
        Next


        FindFolders("PluginData", tempfolder, virtualKSP)
        FindFolders("Interior", tempfolder, virtualKSP)
        FindFolders("Resources", tempfolder, virtualKSP)
        FindFolders("Internals", tempfolder, virtualKSP)
        FindFolders("Sounds", tempfolder, virtualKSP)
        FindFolders("Flags", tempfolder, virtualKSP)
        FindFolders("Props", tempfolder, virtualKSP)
        FindFolders("Spaces", tempfolder, virtualKSP)
        'Add more folders here if KSP needs it.

        'Add More steps here to scan VirtualKSP folder & move files to the main KSP folder.
        SetStatus("Scanning Virtual KSP Folder", 0)
        Dim Searchdir As ReadOnlyCollection(Of String)
        Searchdir = My.Computer.FileSystem.GetDirectories(virtualKSP, FileIO.SearchOption.SearchTopLevelOnly, "*.*")

        For Each cdir In Searchdir
            If Path.GetFileName(cdir).ToLower = "parts" Then 'For parts
                For Each prt In My.Computer.FileSystem.GetFiles(cdir, FileIO.SearchOption.SearchAllSubDirectories, "part.cfg")

                    'Update part cfg's if they're not made for KSP 0.20
                    Dim orginalcfg As String = My.Computer.FileSystem.ReadAllText(prt)
                    If Not RegularExpressions.Regex.IsMatch(orginalcfg, "(//.*\n|\s)*part(//.*\n|\s)*{.*}(//.*\s|\n)*", RegularExpressions.RegexOptions.IgnoreCase + RegularExpressions.RegexOptions.Singleline) Then
                        My.Computer.FileSystem.WriteAllText(prt, "PART" & vbNewLine & "{" & vbNewLine & orginalcfg & vbNewLine & "}", False)
                    End If

                Next
            Else 'Everything else

            End If
        Next

        SetStatus("Applying mod to KSP! Please be patient, this may take a while.", 0)
        My.Computer.FileSystem.CopyDirectory(virtualKSP, RootGameData & Path.GetFileNameWithoutExtension(ModZipFile))

        SetStatus("Generating an installation log", 0)

        If Me.InvokeRequired = False Then
            ProgressBar12.Visible = False
            Me.Enabled = True
        End If
        'My.Computer.FileSystem.WriteAllText(RootInstalled & Modname & ".txt", FilesAddedTextFile, False)
        My.Computer.FileSystem.DeleteDirectory(tempfolder, FileIO.DeleteDirectoryOption.DeleteAllContents)
        LoadMods()

        'If we are doing multimods, then skip the pause on completion until the last one
        If (Not blnMultiModActionInProgress) OrElse (intMultiMod_Current = intMultiMod_Total) Then
            SetStatus("Completed!")
        Else
            SetStatus("Completed!", 0)
        End If

    End Sub

    Sub FindFolders(ByVal fold, tempfolder, virtualKSP, Optional ByVal type = "*")
        SetStatus("Searching for " & fold, 0)

        Dim FoldersToFind() As String = {fold}
        Dim PluginDataFld As ReadOnlyCollection(Of String)
        PluginDataFld = My.Computer.FileSystem.GetDirectories(tempfolder, FileIO.SearchOption.SearchAllSubDirectories, FoldersToFind)

        For Each fld In PluginDataFld

            Dim PluginData As ReadOnlyCollection(Of String)
            PluginData = My.Computer.FileSystem.GetFiles(fld, FileIO.SearchOption.SearchAllSubDirectories, "*." & type)
            For Each dll In PluginData
                dll = dll.Replace(tempfolder, "\")

                If dll.Contains("\_") = True Then
                    'Nothing
                Else
                    If fold = "Sounds" And dll.Contains("Part") Then
                        Continue For
                    End If
                    Dim Plugin As String = dll
                    Dim endnam As Integer = Plugin.Replace(tempfolder, "").IndexOf("\" & fold)
                    If endnam - -1 Then Plugin = Plugin.Substring(endnam) Else Plugin = "\" & Plugin
                    Plugin = virtualKSP & Plugin
                    dll = tempfolder & dll
                    My.Computer.FileSystem.CopyFile(dll, Plugin, True)
                    'Plugin = Plugin.Replace(Root, "[Root]")
                    'FilesAddedTextFile = FilesAddedTextFile & vbNewLine & Plugin
                End If
            Next

        Next
    End Sub

    Function IsPart(ByVal FolderToCheck As String)
        Dim fileExists As Boolean
        fileExists = My.Computer.FileSystem.FileExists(FolderToCheck & "\part.cfg")
        If fileExists = True Then
            Return True
            End
        End If
        Return False
    End Function

    Function ReadCraftType(ByVal CraftFile)
        Dim CraftFileTxt As String = File.ReadAllText(CraftFile)
        Dim location1 As Integer = CraftFileTxt.IndexOf("type = ")

        CraftFileTxt = CraftFileTxt.Substring(location1 + 7, 3)
        If CraftFileTxt = "SPH" Then
            Return "SPH"
        Else
            Return "VAB"
        End If
    End Function
    Function ReadPartCat(ByVal PartFile)
        Dim CatFileTxt As String = File.ReadAllText(PartFile)
        Dim location1 As Integer = CatFileTxt.IndexOf("category = ")
        Dim location2 As Integer = CatFileTxt.IndexOf(vbNewLine & "subcategory")

        CatFileTxt = CatFileTxt.Substring(location1 + "category = ".Count, location1 - location2)

        Return CatFileTxt

    End Function

    'DOES NOT WORK!!! IT'S NOT IN USE ANYWHERE!
    'Extract RAR's
    'Credits to: http://www.kodyaz.com/articles/how-to-unrar-rar-files-using-vb.net-extract-code.aspx
    Private Sub UnRar(ByVal WorkingDirectory As String, ByVal filepath As String)

        ' Microsoft.Win32 and System.Diagnostics namespaces are imported

        Dim objRegKey As RegistryKey
        objRegKey = Registry.ClassesRoot.OpenSubKey("WinRAR\Shell\Open\Command")
        ' Windows 7 Registry entry for WinRAR Open Command

        Dim obj As Object = objRegKey.GetValue("")

        Dim objRarPath As String = obj.ToString()
        objRarPath = objRarPath.Substring(1, objRarPath.Length - 7)

        objRegKey.Close()

        Dim objArguments As String
        ' in the following format
        ' " X G:\Downloads\samplefile.rar G:\Downloads\sampleextractfolder\"
        objArguments = " X " & " " & filepath & " " + " " + WorkingDirectory

        MsgBox(objArguments)

        Dim objStartInfo As New ProcessStartInfo()
        ' Set the UseShellExecute property of StartInfo object to FALSE
        ' Otherwise the we can get the following error message
        ' The Process object must have the UseShellExecute property set to false in order to use environment variables.
        objStartInfo.UseShellExecute = False
        objStartInfo.FileName = objRarPath
        objStartInfo.Arguments = objArguments
        objStartInfo.WindowStyle = ProcessWindowStyle.Hidden
        objStartInfo.WorkingDirectory = WorkingDirectory & "\"

        Dim objProcess As New Process()
        objProcess.StartInfo = objStartInfo
        objProcess.Start()

    End Sub

#End Region

#Region "Removing mod & uinstalling mod"

    Sub DeleteInstalledMod(ByVal ModToDelete)

        If My.Computer.FileSystem.DirectoryExists(RootGameData & Path.GetFileNameWithoutExtension(ModToDelete)) = True Then
            SetStatus("Deleting mod", 0)

            My.Computer.FileSystem.DeleteDirectory(RootGameData & Path.GetFileNameWithoutExtension(ModToDelete), _
                                                   FileIO.DeleteDirectoryOption.DeleteAllContents)

        Else
            SetStatus("Reading installation file", 0)

            Dim ListOfFilesToDelete As New ListBox
            Dim InstallFile = RootInstalled & Path.GetFileNameWithoutExtension(ModToDelete) & ".txt"

            Dim readText() As String = File.ReadAllLines(InstallFile)
            Dim s As String
            For Each s In readText
                ListOfFilesToDelete.Items.Add(s)
            Next

            For Each itm As String In ListOfFilesToDelete.Items
                If itm.StartsWith("[Generated on:") = True And itm.EndsWith("]") = True Then Continue For
                SetStatus("Deleting: " & itm.Replace("[Root]", ""), 0)
                itm = itm.Replace("[Root]", Root)
                Try
                    If itm.StartsWith(Root & "\Parts\") = True Then
                        My.Computer.FileSystem.DeleteDirectory(itm, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    Else
                        My.Computer.FileSystem.DeleteFile(itm)
                    End If
                Catch ex As Exception
                    SetStatus("File not found! Moving on...", 0)
                End Try
            Next
            My.Computer.FileSystem.DeleteFile(InstallFile)

        End If

        LoadMods()

        'If we are doing multimods, then skip the pause on completion until the last one
        If (Not blnMultiModActionInProgress) OrElse (intMultiMod_Current = intMultiMod_Total) Then
            SetStatus("Mod deleted!")
        Else
            SetStatus("Mod deleted!", 0)
        End If

    End Sub

    Private Sub DeleteMod() Handles Deletebtn.Click, FastDeleteBtn.Click
        Dim ModLocToEnable As String = CType(DataGridView1.Item(2, DataGridView1.SelectedRows.Item(0).Index).Value, String)
        Dim Enabled1 As String = CType(DataGridView1.Item(3, DataGridView1.SelectedRows.Item(0).Index).Value, String)
        If (MsgBox("Are you sure you want to delete this mod?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes) Then
            If Enabled1 = "Enabled" Then
                DeleteInstalledMod(ModLocToEnable)
            End If

            If Path.GetFileNameWithoutExtension(ModLocToEnable) = "Squad" Then

                Dim response = InputBox("You are trying to delete Stock parts, I highly suggest againts that. KSPMM will NOT keep any backup. Or held resposible for any damages. Please enter 1234 in the box to delete, otherwise press cancel.", "Deleting Stock Parts?", "")
                If response = "1234" Then
                    My.Computer.FileSystem.DeleteDirectory(ModLocToEnable, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    SetStatus("Deleted Stock Parts.")
                Else
                    SetStatus("Did not delete Stock Parts. Either you pressed cancel, or you entered the wrong code.")
                End If

            Else
                My.Computer.FileSystem.DeleteFile(ModLocToEnable)
            End If

            LoadMods()
        End If
    End Sub

#End Region

#Region "Start-Up Args"

    Sub CrossThread() Handles Me.Load
        CheckForIllegalCrossThreadCalls = False
        AddHandler My.Application.StartupNextInstance, AddressOf CheckArgs
    End Sub

    Public Event StartupNextInstance As StartupNextInstanceEventHandler

    Sub CheckArgs(ByVal sender As Object, _
                  ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs) _
              Handles Me.StartupNextInstance
        If e.CommandLine.Count > 0 Then
            RunArg(e.CommandLine(0))
        End If
    End Sub
    Sub CheckArgsFirst() Handles Me.Load
        If My.Application.CommandLineArgs.Count > 0 Then
            RunArg(My.Application.CommandLineArgs(0))
        End If
    End Sub

    Sub RunArg(ByVal e As String)
        Dim Arg As String = e

        'If it's a file
        If My.Computer.FileSystem.FileExists(Arg) = True Then
            If Path.GetExtension(Arg).ToLower = ".rar" Or Path.GetExtension(Arg).ToLower = ".kspmm" Or _
                Path.GetExtension(Arg).ToLower = ".zip" Then
                AddNewFile(Arg)
                SetStatus("Added file from: " & Path.GetFileNameWithoutExtension(Arg))
            Else
                SetStatus("Unsupported file type")
                ShowMessage("The file type you tried to add is not supported by KSPMM.")
            End If

            'If it's a url
        ElseIf Arg.ToLower.StartsWith("kspmm://") = True Then
            'If it's a file
            If Arg.ToLower.EndsWith(".zip") = True Or Arg.ToLower.EndsWith(".kspmm") = True Then
                Dim Args As String = Arg.Replace("kspmm://", "")
                If Args.StartsWith("http://") Or Args.StartsWith("https://") Then
                    DownloadFile(Args)
                Else
                    DownloadFile("http://" & Args)
                End If
            ElseIf Arg.ToLower.Contains("kerbalspaceport.com/") Then
                Bridge.Show()
                Dim Args As String = Arg.Replace("kspmm://", "http://")
                Bridge.WebBrowser1.Navigate(Args)
                Bridge.TopMost = True
                Application.DoEvents()
                Bridge.TopMost = False
            Else
                SetStatus("Unknown file url")
                ShowMessage("The URL is not a direct link to a supported file type.")
            End If
        Else
            SetStatus("Unknown command")
        End If
    End Sub
#End Region

#Region "Adding & Downloading new files"


    Sub AddNewFileDialog() Handles FileToolStripMenuItem.Click, OpenToolStripMenuItem1.Click
        OpenFileDialog1.ShowDialog()

        Dim Files() As String = OpenFileDialog1.FileNames
        For Each File In Files
            AddNewFile(File)
        Next
    End Sub

    Sub AddFileFromURL() Handles LinkToolStripMenuItem.Click, URLToolStripMenuItem.Click
        Dim defaulturlstring As String = "http://"
        If My.Computer.Clipboard.ContainsText = True Then
            If My.Computer.Clipboard.GetText.EndsWith(".zip") Or My.Computer.Clipboard.GetText.EndsWith(".kspmm") Then
                defaulturlstring = My.Computer.Clipboard.GetText
            End If
        End If
        Dim url As String = InputBox("Enter the direct link to the file you wish to download", "Mod Manager Download File", defaulturlstring)
        If url.StartsWith("http://") Or url.StartsWith("https://") Or url.StartsWith("kspmm://") Then
            If url.EndsWith(".zip") Or url.EndsWith(".kspmm") Then
                DownloadFile(url)
            Else
                MsgBox("Only .zip and .KSPMM files supported. Sorry!", MsgBoxStyle.Information)
            End If

        ElseIf url = "" Then
            'Nothing
        Else
            MsgBox("Invalid url!", MsgBoxStyle.Exclamation)
        End If


    End Sub

    Dim downloadingFile As Boolean = False

    Sub DownloadFile(ByVal urlToFile)
        downloadingFile = True
        Dim client As WebClient = New WebClient
        AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler client.DownloadFileCompleted, AddressOf client_DownloadCompleted
        client.DownloadFileAsync(New Uri(urlToFile), RootMODS & GetFileNameFromURL(urlToFile))

        SetStatus("Downloading from: " & urlToFile, 0)
        ProgressBar12.Visible = True
        ProgressBar12.Maximum = 100
    End Sub
    Private Sub client_ProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        SetStatus("Dowloading: " & e.ProgressPercentage & "%", 0)
        ProgressBar12.Value = e.ProgressPercentage
    End Sub
    Private Sub client_DownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        SetStatus("Download Complete")
        ProgressBar12.Visible = False
        downloadingFile = False
        wait(2)
        LoadMods()
    End Sub


#End Region

#Region "Auto-Update Handlers"

    Dim CanUpdate As Boolean = False
    Sub CheckVersion() Handles Me.Shown, CheckAgainToolStripMenuItem.Click

        VersionNumberInfo.Text = "Installed: " & My.Application.Info.Version.ToString
        NewestVersionWeb.Text = "Current: Retrieving information..."
        UpdateButton.Text = "Update..."
        UpdateButton.Enabled = False

        If UpdateEnabled = False Then
            NewestVersionWeb.Text = "Offline mode"
            CheckAgainToolStripMenuItem.Enabled = False
            Exit Sub
        End If

        If BackgroundWorker1.IsBusy = True Then BackgroundWorker1.CancelAsync()
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub BackgroundWorker1_DoWork() Handles BackgroundWorker1.DoWork
        Try
            CanUpdate = False
            Dim FileURL As String = CheckForUpdates

            Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(FileURL)
            Dim response As System.Net.HttpWebResponse = request.GetResponse()

            Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())

            Dim sourcecode As String = sr.ReadToEnd()

            My.Computer.FileSystem.WriteAllText(Root & "\Temp.txt", sourcecode, False)


            Dim Version = ReadValueFromFile(Root & "\Temp.txt", "Version")
            Dim url = ReadValueFromFile(Root & "\Temp.txt", "Url")

            Dim Locat As String = Root & "\Temp.txt"

            NewestVersionWeb.Text = "Current: " & Version
            NewestVersionWeb.Tag = url

            If My.Application.Info.Version.ToString = Version Then
                UpdateButton.Text = "No update avalible!"
                UpdateStrip.Visible = False
            Else
                UpdateButton.Enabled = True
                CanUpdate = True
                UpdateStrip.Visible = True
            End If

            My.Computer.FileSystem.DeleteFile(Root & "\Temp.txt")

        Catch ex As Exception
            NewestVersionWeb.Text = "Current: Please try again later."
            CanUpdate = False
        End Try
    End Sub

    Sub DeleteOldVersion() Handles Me.Shown
        Dim mainExe As String = System.IO.Path.GetFileName(Application.ExecutablePath)
        Dim oldExeLoc As String = System.IO.Path.GetDirectoryName(mainExe)
        Dim oldExe As String = oldExeLoc & "KSPMM.Old"

        If My.Computer.FileSystem.FileExists(oldExe) Then My.Computer.FileSystem.DeleteFile(oldExe)
    End Sub

    Private Sub UpdateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpdateButton.Click, UpdateStripUpdatenowBtn.Click
        Dim UpdateUrl As String

        Dim Testurl As String = "http://localhost/Downloads/Kerbal%20Space%20Program%20Mod%20Manager.exe" 'This was just for local testing/debugging
        UpdateUrl = NewestVersionWeb.Tag

        Dim UpdW As New Updater
        UpdW.PassedText = UpdateUrl
        UpdW.Show()
    End Sub

    Sub ShowChangelog() Handles ChangelogToolStripMenuItem1.Click, UpdateStripChangelogBtn.Click
        ChangelogWindow.ShowDialog()
    End Sub

#Region "UpdateStrip"
    Sub DismissUpdate() Handles UpdateStripDismissBtn.Click
        UpdateStrip.Visible = False
    End Sub
#End Region

#End Region

#Region "Simple Context Menu Items"
    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click, LaunchKSPToolStripMenuItem.Click
        Process.Start(Root & "\KSP.exe")
    End Sub

    Private Sub BridgeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BridgeToolStripMenuItem.Click, ToolStripButton3.Click
        Bridge.Show()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click, ExitToolStripMenuItem1.Click
        Application.Exit()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click, AboutToolStripMenuItem1.Click
        About.Show()
        About.Focus()
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingsToolStripMenuItem.Click
        Settings.ShowDialog()
    End Sub

    Private Sub OpenFolder() Handles OpenFolderBtn.Click
        Process.Start(Root)
    End Sub

#End Region

#Region "Key Presses Shortcuts"

    Private Sub Form1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown, DataGridView1.KeyDown
        If e.Control = True And e.KeyCode = Keys.F Then
            MesgBox.Focus()
        ElseIf e.Control = True And e.KeyCode = Keys.Space Then
            Dim fileExists As Boolean
            fileExists = My.Computer.FileSystem.FileExists(Root & "/KSP.exe")
            If fileExists = False Then SetStatus("Could Not locate KSP.exe!", 1000) _
                Else Process.Start(Root & "/KSP.exe")
        ElseIf e.KeyCode = Keys.Enter Then
            EnableDisableMod()
        ElseIf e.KeyCode = Keys.Delete Then
            DeleteMod()
        ElseIf e.KeyCode = Keys.Alt And e.KeyCode = Keys.R Then
            ResetSize()
            ShowMessage("Size Reset!")
            MsgBox("Worked?")
        End If
    End Sub

#End Region

#Region "Screen Settings"

    Sub UpdateValuesResezing() Handles Me.Resize, Me.LocationChanged
        If WSMaxed() = True Or Me.WindowState = FormWindowState.Minimized Then
            WSBox.Maximized = True
        Else
            WSBox.Maximized = False
        End If

        If WSBox.Maximized = False Then
            WSBox.XValue = Me.Size.Width
            WSBox.YValue = Me.Size.Height

            WSBox.XLocation = Me.Location.X
            WSBox.YLocation = Me.Location.Y
        End If

        If CanSave = True Then
            Dim newKey As RegistryKey
            newKey = My.Computer.Registry.CurrentUser.CreateSubKey(Key)
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\" & Key, "XY", WSBox.XValue & ":" & WSBox.YValue)
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\" & Key, "pXY", WSBox.XLocation & ":" & WSBox.YLocation)
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\" & Key, "Full", WSMaxed)
        End If
    End Sub

    Function WSMaxed()
        If Me.WindowState = FormWindowState.Maximized Then
            Return True
        Else
            Return False
        End If
    End Function

    Dim LoadingSetting As Boolean = False
    Dim CanSave As Boolean = False
    Sub LoadSettings() Handles Me.Load
        Dim exists As Boolean = False
        Try
            If My.Computer.Registry.CurrentUser.OpenSubKey(Key) IsNot Nothing Then
                exists = True
            End If
        Finally
            My.Computer.Registry.CurrentUser.Close()
        End Try

        If exists = True Then
            LoadingSetting = True


            Dim keyValueFull As Boolean
            keyValueFull = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\" & Key, "Full", False)

            Dim keyValueXY As String
            keyValueXY = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\" & Key, "XY", "671:450")

            Dim keyValuepXY As String
            keyValuepXY = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\" & Key, "pXY", "100:100")

            Dim XY() As String
            XY = keyValueXY.Split(":")
            Dim pXY() As String
            pXY = keyValuepXY.Split(":")

            Dim screenWidth As Integer = Screen.PrimaryScreen.Bounds.Width
            Dim screenHeight As Integer = Screen.PrimaryScreen.Bounds.Height

            If XY(0) >= screenWidth Or XY(1) >= screenHeight Or pXY(0) >= screenWidth Or pXY(1) >= screenHeight Then
                LoadingSetting = False
                CanSave = True
                Exit Sub
            End If

            Me.Width = XY(0)
            Me.Height = XY(1)
            Dim p As New Point(pXY(0), pXY(1))
            Me.Location = p

            If keyValueFull = True Then
                Me.WindowState = FormWindowState.Maximized
            End If
            LoadingSetting = False
            CanSave = True
        End If
    End Sub

    Private Sub ResetSize() Handles WSBox.ResetClick, ResetSizeToolStripMenuItem.Click
        Dim DefX As Integer = 671
        Dim DefY As Integer = 450
        Dim DefP As New Point(100, 100)
        Me.Width = DefX
        Me.Height = DefY
        Me.Location = DefP
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub SizeSettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SizeSettingsToolStripMenuItem.Click
        WSBox.Visible = SizeSettingsToolStripMenuItem.Checked
    End Sub

#End Region

#Region "Welcome Screen"
    Private Sub WelcomeScreenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WelcomeScreenToolStripMenuItem.Click
        WelcomeScreen.ShowDialog()
    End Sub

    Sub ShowWelcome() Handles Me.Shown

        Dim keyValue As Object
        keyValue = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\" & Key, "StartUpWindow", "False")

        If keyValue = False Or keyValue = "" Then
            WelcomeScreen.ShowDialog()
            Exit Sub
        End If
    End Sub

#End Region

#Region "Enable/Disable All mods"

    'Flag to let us know we are doing multiple mods and where we are up to
    Private blnMultiModActionInProgress As Boolean = False
    Private intMultiMod_Total As Integer = 1
    Private intMultiMod_Current As Integer = 1

    Sub EnableAllMods() Handles EnableAllToolStripMenuItem.Click, EnableAllFast.Click
        'Set the flag to say we are starting
        blnMultiModActionInProgress = True
        intMultiMod_Total = DataGridView1.RowCount
        'enable seems to start at 1 and disable at 0 - weird tried this with 0
        'For i = 1 To DataGridView1.RowCount - 1
        For i = 0 To DataGridView1.RowCount - 1
            intMultiMod_Current = i + 1
            Me.DataGridView1.CurrentCell = Me.DataGridView1(0, i)
            Dim ModLocToEnable As String = CType(DataGridView1.Item(2, DataGridView1.SelectedRows.Item(0).Index).Value, String)
            Dim Enabled1 As String = CType(DataGridView1.Item(3, DataGridView1.SelectedRows.Item(0).Index).Value, String)

            If Enabled1 = "Enabled" Or Enabled1 = "Download" Or Enabled1 = "Stock" Then
                'Nothing
            Else
                'Don't Multithread this - was causing me issues
                ExtractFiles(ModLocToEnable)
                'Threading.ThreadPool.QueueUserWorkItem(AddressOf ExtractFiles, ModLocToEnable)	

            End If
        Next
        'Set the flag to say we are finished
        blnMultiModActionInProgress = False
    End Sub

    Sub DisableAllMods() Handles DisableAllToolStripMenuItem.Click, DisableAllFast.Click
        'Set the flag to say we are starting
        blnMultiModActionInProgress = True
        intMultiMod_Total = DataGridView1.RowCount
        For i = 0 To DataGridView1.RowCount - 1
            intMultiMod_Current = i + 1
            Me.DataGridView1.CurrentCell = Me.DataGridView1(0, i)
            Dim ModLocToEnable As String = CType(DataGridView1.Item(2, DataGridView1.SelectedRows.Item(0).Index).Value, String)
            Dim Enabled1 As String = CType(DataGridView1.Item(3, DataGridView1.SelectedRows.Item(0).Index).Value, String)


            'Changed this to be "disabled"
            'If Enabled1 = "Disable" Or Enabled1 = "Download" Then
            If Enabled1 = "Disabled" Or Enabled1 = "Download" Or Enabled1 = "Stock" Then
                'Nothing
            Else

                'Don't Multithread this - was causing me issues
                DeleteInstalledMod(ModLocToEnable)
                'Threading.ThreadPool.QueueUserWorkItem(AddressOf DeleteInstalledMod, ModLocToEnable)
            End If
        Next
        blnMultiModActionInProgress = False
    End Sub

#End Region

#Region "Display message until user input"
    Sub ShowMessage(ByVal Message As String)
        MessageLabel.Text = Message
        StatusStrip2.Visible = True
    End Sub

    Sub DismissMessage() Handles DismissLabel.Click
        StatusStrip2.Visible = False
    End Sub

#End Region

#Region "Sessions Counter"
    Sub LoadPg() Handles Me.Shown, ToolSSessions.DoubleClick
        If EnableSessions = False Then
            ToolSSessions.Text = "Offline mode"
            Exit Sub
        End If
        ToolSSessions.Text = "Refreshing..."
        Application.DoEvents()
        Try

            Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(SessionsPage)
            Dim response As System.Net.HttpWebResponse = request.GetResponse()

            Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())

            Dim sourcecode As String = sr.ReadToEnd()

            Dim Num As String = sourcecode.Replace(vbNewLine, "")
            Dim s As String = ""
            If Num <> 1 Then s = "s"

            ToolSSessions.Text = Num & "user" & s & " online  "
        Catch ex As Exception
            ToolSSessions.Text = "Connection failed"
        End Try

    End Sub

    Private Sub UserOnlineRefresh_Tick(sender As Object, e As EventArgs) Handles UserOnlineRefresh.Tick
        LoadPg()
    End Sub
#End Region

#Region "Chat"

    Sub RequestChat()
        SH_Loading.Visible = True

        Try
            DataSet1.Chat.Clear()

            Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(ChatPage)
            Dim response As System.Net.HttpWebResponse = request.GetResponse()

            Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())

            Dim sourcecode As String = sr.ReadToEnd()

            SH_Title.Text = GetTagContents(sourcecode, "<title>", "</title>")(0)

            Dim Chat As List(Of String) = GetTagContents(sourcecode, "<msgbody>", "<msgbody />")

            For Each itm In Chat
                Dim nck As List(Of String) = GetTagContents(itm, "<nick>", "<nick />")
                Dim msg As List(Of String) = GetTagContents(itm, "<msg>", "<msg />")
                Dim time As List(Of String) = GetTagContents(itm, "<time>", "<time />")
                addChat(msg(0), nck(0), time(0))
            Next
        Catch ex As Exception
            SH_Title.Text = "Connection error"
        End Try

        SH_Loading.Visible = False
    End Sub

    Sub ChatVisibility() Handles SHChat.Click, Me.Load
        SplitContainer1.Panel2Collapsed = Not SHChat.Checked

        If EnableChat = False Then
            SHChat.Enabled = False
            Exit Sub
        End If
        If SplitContainer1.Panel2Collapsed = False Then RequestChat()
        'If ListBox1.Items.Count - 1 > 0 Then ListBox1.SelectedIndex = ListBox1.Items.Count - 1
        'If DataGridView3.RowCount - 1 > 0 Then DataGridView3.FirstDisplayedScrollingRowIndex = DataGridView3.RowCount - 1
    End Sub

    Sub UpdateSendBtn() Handles MesgBox.TextChanged, NickBox.TextChanged, MesgBox.LostFocus, NickBox.LostFocus
        If MesgBoxEmpty() = False Then
            SendBtn.Enabled = False
        ElseIf MesgBox.ForeColor = Color.Gray Or MesgBox.Text = "Message (Max 1000 chars)" Then
            SendBtn.Enabled = False
        ElseIf NickBoxEmpty() = False Then
            SendBtn.Enabled = False
        ElseIf NickBox.ForeColor = Color.Gray Or NickBox.Text = "Nick (Max 32 Chars)" Then
            SendBtn.Enabled = False
        Else
            SendBtn.Enabled = True
        End If
    End Sub

    Sub SendMsg() Handles SendBtn.Click

        addChat(MesgBox.Text, NickBox.Text, "Now")

        SendBtn.Enabled = False
        Dim Nick As String = NickBox.Text
        Dim Msg As String = MesgBox.Text
        MesgBox.Text = ""
        MesgBox.Focus()
        Dim msgpage As String = _
            ChatPage & "?nick=" & Nick & "&text=" & Msg
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(msgpage)
        Dim response As System.Net.HttpWebResponse = request.GetResponse()

    End Sub

    Sub Reload() Handles SH_ReloadBtn.Click
        RequestChat()
        'If DataGridView3.RowCount - 1 > 0 Then DataGridView3.FirstDisplayedScrollingRowIndex = DataGridView3.RowCount - 1
    End Sub

    Sub addChat(ByVal msg As String, ByVal nick As String, ByVal time As String)

        Dim newChatRow As DataSet1.ChatRow
        newChatRow = DataSet1.Chat.NewChatRow()

        newChatRow.nick = nick
        newChatRow.msg = msg
        newChatRow.timestmp = time

        DataSet1.Chat.Rows.Add(newChatRow)

    End Sub

#Region "MessageBox"

    Private Sub MsgBoxEnter() Handles MesgBox.Enter
        If MesgBoxEmpty() = False Then
            MesgBox.Text = ""
            MesgBox.ForeColor = Color.Black
        End If
    End Sub

    Private Sub MesgBoxLeave() Handles MesgBox.Leave
        If MesgBoxEmpty() = True Then Exit Sub
        MesgBox.Text = "Message (Max 1000 chars)"
        MesgBox.ForeColor = Color.Gray
    End Sub

    Function MesgBoxEmpty()
        If MesgBox.Text <> "" And MesgBox.ForeColor <> Color.Gray Then
            Return True
        Else
            Return False
        End If
    End Function
#End Region

#Region "NickBox"

    Private Sub NickBoxEnter() Handles NickBox.Enter
        If NickBoxEmpty() = False Then
            NickBox.Text = ""
            NickBox.ForeColor = Color.Black
        End If
    End Sub

    Private Sub NickBoxLeave() Handles NickBox.Leave
        If NickBoxEmpty() = True Then Exit Sub
        NickBox.Text = "Nick (Max 32 Chars)"
        NickBox.ForeColor = Color.Gray
    End Sub

    Function NickBoxEmpty()
        If NickBox.Text <> "" And NickBox.ForeColor <> Color.Gray Then
            Return True
        Else
            Return False
        End If
    End Function
#End Region

    'Thanks to http://www.vbforums.com/showthread.php?574237-Get-the-Text-inbetween-two-words-(such-as-HTML-Tags)-without-RegEx for this sub
    Private Function GetTagContents(ByVal Source As String, ByVal startTag As String, ByVal endTag As String) As List(Of String)
        Dim StringsFound As New List(Of String)
        Dim Index As Integer = Source.IndexOf(startTag) + startTag.Length

        While Index <> startTag.Length - 1
            StringsFound.Add(Source.Substring(Index, Source.IndexOf(endTag, Index) - Index))
            Index = Source.IndexOf(startTag, Index) + startTag.Length
        End While

        Return StringsFound
    End Function


#End Region

    Sub debugger() Handles BtnDebugger.Click
        Overwrite_Mod.ShowDialog()
    End Sub

    Private Sub ResetSize(sender As Object, e As EventArgs) Handles WSBox.ResetClick, ResetSizeToolStripMenuItem.Click

    End Sub
End Class
