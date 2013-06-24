<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WindowSize1
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.MaximizedTextB = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.PYBox = New System.Windows.Forms.TextBox()
        Me.PXBox = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.YTBox = New System.Windows.Forms.TextBox()
        Me.XTBox = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox1.Controls.Add(Me.Button1)
        Me.GroupBox1.Controls.Add(Me.MaximizedTextB)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.PYBox)
        Me.GroupBox1.Controls.Add(Me.PXBox)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.YTBox)
        Me.GroupBox1.Controls.Add(Me.XTBox)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(98, 131)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Size"
        '
        'MaximizedTextB
        '
        Me.MaximizedTextB.BackColor = System.Drawing.SystemColors.Control
        Me.MaximizedTextB.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.MaximizedTextB.ForeColor = System.Drawing.SystemColors.WindowText
        Me.MaximizedTextB.Location = New System.Drawing.Point(30, 95)
        Me.MaximizedTextB.Name = "MaximizedTextB"
        Me.MaximizedTextB.ReadOnly = True
        Me.MaximizedTextB.Size = New System.Drawing.Size(63, 13)
        Me.MaximizedTextB.TabIndex = 10
        Me.MaximizedTextB.Text = "False"
        Me.MaximizedTextB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 82)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(59, 13)
        Me.Label5.TabIndex = 9
        Me.Label5.Text = "Maximized:"
        '
        'PYBox
        '
        Me.PYBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.PYBox.Location = New System.Drawing.Point(29, 61)
        Me.PYBox.Name = "PYBox"
        Me.PYBox.ReadOnly = True
        Me.PYBox.Size = New System.Drawing.Size(63, 13)
        Me.PYBox.TabIndex = 8
        Me.PYBox.Text = "450"
        '
        'PXBox
        '
        Me.PXBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.PXBox.Location = New System.Drawing.Point(29, 48)
        Me.PXBox.Name = "PXBox"
        Me.PXBox.ReadOnly = True
        Me.PXBox.Size = New System.Drawing.Size(63, 13)
        Me.PXBox.TabIndex = 7
        Me.PXBox.Text = "671"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 61)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(23, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "pY:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 48)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(23, 13)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "pX:"
        '
        'YTBox
        '
        Me.YTBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.YTBox.Location = New System.Drawing.Point(29, 29)
        Me.YTBox.Name = "YTBox"
        Me.YTBox.ReadOnly = True
        Me.YTBox.Size = New System.Drawing.Size(63, 13)
        Me.YTBox.TabIndex = 4
        Me.YTBox.Text = "450"
        '
        'XTBox
        '
        Me.XTBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.XTBox.Location = New System.Drawing.Point(29, 16)
        Me.XTBox.Name = "XTBox"
        Me.XTBox.ReadOnly = True
        Me.XTBox.Size = New System.Drawing.Size(63, 13)
        Me.XTBox.TabIndex = 3
        Me.XTBox.Text = "671"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 29)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(17, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Y:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(17, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "X:"
        '
        'Button1
        '
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.Button1.Location = New System.Drawing.Point(9, 110)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 17)
        Me.Button1.TabIndex = 11
        Me.Button1.Text = "Reset"
        Me.Button1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button1.UseVisualStyleBackColor = True
        '
        'WindowSize1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "WindowSize1"
        Me.Size = New System.Drawing.Size(104, 151)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents YTBox As System.Windows.Forms.TextBox
    Friend WithEvents XTBox As System.Windows.Forms.TextBox
    Friend WithEvents PYBox As System.Windows.Forms.TextBox
    Friend WithEvents PXBox As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents MaximizedTextB As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button

End Class
