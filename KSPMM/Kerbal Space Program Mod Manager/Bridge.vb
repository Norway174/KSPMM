Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Net
Imports System.ComponentModel


Public Class Bridge
    Sub DownloadsVisible() Handles ToolStripButtonDownloads.Click, Me.Load
        If SplitContainer1.Panel1Collapsed = True Then
            SplitContainer1.Panel1Collapsed = False
        Else
            SplitContainer1.Panel1Collapsed = True
        End If
    End Sub

    Private Sub CloseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub ExitAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitAllToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub WebBrowser1_Navigating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs) Handles WebBrowser1.Navigating

        If e.Url.ToString.EndsWith(".zip") Then
            e.Cancel = True
            addDownload(e.Url)
        Else
            ToolStripSpringTextBox.Text = e.Url.ToString
        End If
    End Sub

    Dim fileDL As String
    Sub addDownload(ByVal file)

        fileDL = Main.RootMODS & GetFileNameFromURL(file.ToString)

        fileDL = fileDL.Replace(".zip", ".kspmm")

        Dim WC As WebClient = New WebClient
        AddHandler WC.DownloadProgressChanged, AddressOf WC_ProgressChanged
        AddHandler WC.DownloadFileCompleted, AddressOf WC_DownloadComplete
        AddHandler Cancellbl.Click, AddressOf WC.CancelAsync

        WC.DownloadFileAsync(file, fileDL)
        Label2.Text = "Downloading: " & GetFileNameFromURL(file.ToString.Replace(".zip", ""))
        MovableLabel1.Visible = True
        GroupBox1.Visible = True
    End Sub

    Private Sub WC_ProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)

        Dim bytesIn As Double = Double.Parse(e.BytesReceived.ToString())
        Dim totalBytes As Double = Double.Parse(e.TotalBytesToReceive.ToString())
        Dim percentage As Double = bytesIn / totalBytes * 100

        ProgressBar1.Value = Int32.Parse(Math.Truncate(percentage).ToString())
    End Sub

    Private Sub WC_DownloadComplete(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)
        MovableLabel1.Visible = False
        GroupBox1.Visible = False
        ProgressBar1.Value = 0

        If e.Cancelled = True Then
            My.Computer.FileSystem.DeleteFile(fileDL)
        ElseIf e.Error IsNot Nothing Then
            MsgBox("Error: " & e.Error.ToString)
            My.Computer.FileSystem.DeleteFile(fileDL)
        Else
            Main.LoadMods()
        End If

        fileDL = ""
    End Sub


    Sub Navigated() Handles WebBrowser1.DocumentCompleted
        TabPage1.Text = WebBrowser1.DocumentTitle

        Dim theElementCollection As HtmlElementCollection
        theElementCollection = WebBrowser1.Document.GetElementsByTagName("input")

        For Each curElement As HtmlElement In theElementCollection
            If curElement.OuterHtml.Contains("red_btn") Then
                curElement.SetAttribute("InnerText", "Download with KSPMM!")
            End If
        Next

    End Sub

    Private Sub ToolStripSpringTextBox_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ToolStripSpringTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            WebBrowser1.Navigate(ToolStripSpringTextBox.Text)
        End If
    End Sub

    Sub SearchGoogleTextBox(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ToolStripTextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            WebBrowser1.Navigate("https://www.google.com/search?q=" & ToolStripTextBox1.Text)
        End If
    End Sub

    Private Sub ToolStripLabel1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripLabel1.Click
        WebBrowser1.Navigate("https://www.google.com")
    End Sub

    Sub BeginLoading() Handles WebBrowser1.Navigated
        StatusStrip1.Visible = True
    End Sub
    Private Sub Loading(ByVal sender As Object, ByVal e As Windows.Forms.WebBrowserProgressChangedEventArgs) Handles WebBrowser1.ProgressChanged
        Try
            TLoading.Text = WebBrowser1.Url.ToString
            TProgressBar1.Maximum = e.MaximumProgress
            TProgressBar1.Value = e.CurrentProgress
        Catch ex As Exception
            'Nothing
        End Try
    End Sub
    Sub Done() Handles WebBrowser1.DocumentCompleted
        StatusStrip1.Visible = False
    End Sub

    Private Sub ToolStripButton7_Click(sender As Object, e As EventArgs) Handles ToolStripButton7.Click, Me.Load
        WebBrowser1.Navigate("http://kerbalspaceport.com/")
    End Sub

    Private Sub ToolStripButton4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton4.Click
        WebBrowser1.Navigate("http://kerbalspaceprogram.com/forum/index.php")
    End Sub

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        WebBrowser1.GoHome()
    End Sub

    Private Sub ToolStripButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton2.Click
        WebBrowser1.GoBack()
    End Sub

    Private Sub ToolStripButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton3.Click
        WebBrowser1.GoForward()
    End Sub

    Private Sub ToolStripButton5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton5.Click
        WebBrowser1.Refresh()
    End Sub

End Class


Public Class ToolStripSpringTextBox
    Inherits ToolStripTextBox

    Public Overrides Function GetPreferredSize( _
        ByVal constrainingSize As Size) As Size

        ' Use the default size if the text box is on the overflow menu
        ' or is on a vertical ToolStrip.
        If IsOnOverflow Or Owner.Orientation = Orientation.Vertical Then
            Return DefaultSize
        End If

        ' Declare a variable to store the total available width as 
        ' it is calculated, starting with the display width of the 
        ' owning ToolStrip.
        Dim width As Int32 = Owner.DisplayRectangle.Width

        ' Subtract the width of the overflow button if it is displayed. 
        If Owner.OverflowButton.Visible Then
            width = width - Owner.OverflowButton.Width - _
                Owner.OverflowButton.Margin.Horizontal()
        End If

        ' Declare a variable to maintain a count of ToolStripSpringTextBox 
        ' items currently displayed in the owning ToolStrip. 
        Dim springBoxCount As Int32 = 0

        For Each item As ToolStripItem In Owner.Items

            ' Ignore items on the overflow menu.
            If item.IsOnOverflow Then Continue For

            If TypeOf item Is ToolStripSpringTextBox Then
                ' For ToolStripSpringTextBox items, increment the count and 
                ' subtract the margin width from the total available width.
                springBoxCount += 1
                width -= item.Margin.Horizontal
            Else
                ' For all other items, subtract the full width from the total
                ' available width.
                width = width - item.Width - item.Margin.Horizontal
            End If
        Next

        ' If there are multiple ToolStripSpringTextBox items in the owning
        ' ToolStrip, divide the total available width between them. 
        If springBoxCount > 1 Then width = CInt(width / springBoxCount)

        ' If the available width is less than the default width, use the
        ' default width, forcing one or more items onto the overflow menu.
        If width < DefaultSize.Width Then width = DefaultSize.Width

        ' Retrieve the preferred size from the base class, but change the
        ' width to the calculated width. 
        Dim preferredSize As Size = MyBase.GetPreferredSize(constrainingSize)
        preferredSize.Width = width
        Return preferredSize

    End Function
End Class


'A HUGE thanks to Dave Kreskowiak over at CodeProject.com, without his tutorial. This would have not been possible!
'Check out his tutorial here, http://www.codeproject.com/Articles/14770/Create-your-Own-Runtime-Movable-Windows-Forms-Cont
'Which is where the credits for this Class should go to.

<ToolboxBitmap(GetType(MovableLabel), "MovableLabel.bmp"), _
Description("A .NET Label control, movable at runtime.")> _
Public Class MovableLabel
    Inherits System.Windows.Forms.Panel

    ' Event names to be used as keys for the parent Labels Events collection
    Private Const EVENT_STAYWITHINPARENTBOUNDSCHANGED As String = "StayWithinParentBoundsEvent"

    ' Holds the mouse position relative to the inside of our control when the mouse button goes down.
    Private m_CursorOffset As Point
    ' Used by the MoveMove event handler to show that the setup to move the control has completed.
    Private m_Moving As Boolean
    ' Used to store the current cursor shape when we start to move the control.
    Private m_CurrentCursor As Cursor
    ' Used to specify if our control should stay with the visible bounds of our parent container.
    Private m_StayWithinParent As Boolean

    <Bindable(True), DefaultValue(True), Category("Behavior"), _
    Description("Gets or sets whether the control will stay within its parent container's visible bounds."), _
    DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    Public Property StayWithinParentBounds() As Boolean
        Get
            Return m_StayWithinParent
        End Get

        Set(ByVal value As Boolean)
            If value <> m_StayWithinParent Then
                m_StayWithinParent = value
                MoveControlWithinBounds()
                Me.OnStayWithinParentBoundsChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    Public Custom Event StayWithinParentBoundsChanged As EventHandler
        AddHandler(ByVal value As EventHandler)
            MyBase.Events.AddHandler(MovableLabel.EVENT_STAYWITHINPARENTBOUNDSCHANGED, value)
        End AddHandler

        RemoveHandler(ByVal value As EventHandler)
            MyBase.Events.RemoveHandler(MovableLabel.EVENT_STAYWITHINPARENTBOUNDSCHANGED, value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
            CType(MyBase.Events(MovableLabel.EVENT_STAYWITHINPARENTBOUNDSCHANGED), EventHandler).Invoke(sender, e)
        End RaiseEvent
    End Event

    Protected Overridable Sub OnStayWithinParentBoundsChanged(ByVal e As EventArgs)
        Dim newHandler As EventHandler = TryCast(MyBase.Events(EVENT_STAYWITHINPARENTBOUNDSCHANGED), EventHandler)
        If Not newHandler Is Nothing Then
            newHandler.Invoke(Me, e)
        End If
    End Sub

    Private Sub MovableLabel_HandleDestroyed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.HandleDestroyed
        Try
            RemoveHandler Me.Parent.Resize, AddressOf Parent_Resize
        Catch ex As NullReferenceException
        End Try
    End Sub

    Private Sub MovableLabel_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LocationChanged
        If StayWithinParentBounds Then
            MoveControlWithinBounds()
        End If
    End Sub

    Private Sub MovableLabel_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            m_CursorOffset = e.Location
            m_CurrentCursor = MyBase.Cursor
            MyBase.Cursor = Cursors.SizeAll
            m_Moving = True
        End If
    End Sub

    Private Sub MovableLabel_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        If m_Moving Then
            Dim clientPosition As Point = MyBase.Parent.PointToClient(System.Windows.Forms.Cursor.Position)
            Dim adjustedLocation As New Point(clientPosition.X - m_CursorOffset.X, clientPosition.Y - m_CursorOffset.Y)

            If StayWithinParentBounds Then
                MoveControlWithinBounds(adjustedLocation)
            Else
                MyBase.Location = adjustedLocation
            End If
        End If
    End Sub

    Private Sub MovableLabel_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseUp
        m_Moving = False
        MyBase.Cursor = m_CurrentCursor
    End Sub

    Private Sub MovableLabel_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ParentChanged
        Try
            AddHandler Me.Parent.Resize, AddressOf Parent_Resize
        Catch ex As NullReferenceException
        End Try
    End Sub

    Private Sub Parent_Resize(ByVal sender As Object, ByVal e As System.EventArgs)
        If StayWithinParentBounds Then
            MoveControlWithinBounds()
        End If
    End Sub

    Private Overloads Sub MoveControlWithinBounds()
        If MyBase.IsHandleCreated Then
            With MyBase.Location
                MoveControlWithinBounds(New Point(.X, .Y))
            End With
        End If
    End Sub

    Private Overloads Sub MoveControlWithinBounds(ByVal location As Point)
        If MyBase.IsHandleCreated Then
            Dim x As Integer = location.X
            Dim y As Integer = location.Y

            With MyBase.Parent.ClientRectangle
                If x > .Right - MyBase.Width Then
                    x = .Right - MyBase.Width
                End If

                If y > .Bottom - MyBase.Height Then
                    y = .Bottom - MyBase.Height
                End If

                If x < .Left Then
                    x = .Left
                End If

                If y < .Top Then
                    y = .Top
                End If
            End With
            MyBase.Location = New Point(x, y)
        End If
    End Sub

End Class