Public Class WindowSize1

    Public Event ResetClick As EventHandler

    Public Property XValue() As String
        Get
            Return Me.XTBox.Text
        End Get
        Set(ByVal value As String)
            Me.XTBox.Text = value
        End Set
    End Property

    Public Property YValue() As String
        Get
            Return Me.YTBox.Text
        End Get
        Set(ByVal value As String)
            Me.YTBox.Text = value
        End Set
    End Property

    Public Property XLocation() As String
        Get
            Return Me.PXBox.Text
        End Get
        Set(ByVal value As String)
            Me.PXBox.Text = value
        End Set
    End Property

    Public Property YLocation() As String
        Get
            Return Me.PYBox.Text
        End Get
        Set(ByVal value As String)
            Me.PYBox.Text = value
        End Set
    End Property

    Public Property Maximized() As Boolean
        Get
            Return Me.MaximizedTextB.Text
        End Get
        Set(ByVal value As Boolean)
            Me.MaximizedTextB.Text = value
        End Set
    End Property

    Sub MaximizedGotFucus() Handles MaximizedTextB.Click, MaximizedTextB.GotFocus
        MaximizedTextB.SelectAll()
    End Sub

    
    Dim Loading As Boolean = False
    Private Sub CheckCh() Handles Button1.Click
        If Loading = True Then
            Dim We As System.EventArgs
            We = EventArgs.Empty
            RaiseEvent ResetClick(Button1, We)
        End If
        Loading = False
    End Sub
    Sub Loader() Handles Me.Load
        Loading = True
    End Sub
End Class
