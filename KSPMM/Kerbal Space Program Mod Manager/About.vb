Public NotInheritable Class About


    Dim DonateUrl As String = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=willyodden%40hotmail%2ecom&lc=NO&item_name=Kerbal%20Mod%20Manager%20Donate&item_number=KSPMM%2dDonation&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted"

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        Process.Start(DonateUrl)
    End Sub

    Sub HideAbout() Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub About_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub VisitHomePage(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("http://kspmm.norway174.com/")
    End Sub
    Private Sub VisitForumPage(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Process.Start("http://forum.kerbalspaceprogram.com/showthread.php/13155")
    End Sub
End Class
