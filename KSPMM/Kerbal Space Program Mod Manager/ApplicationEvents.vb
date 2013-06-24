Namespace My

    Partial Friend Class MyApplication
        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup

            AddHandler AppDomain.CurrentDomain.AssemblyResolve, New System.ResolveEventHandler(AddressOf CurrentDomain_AssemblyResolve)

        End Sub

        Private Function CurrentDomain_AssemblyResolve(ByVal sender As System.Object, ByVal e As System.ResolveEventArgs) As System.Reflection.Assembly

            ' TODO: If you have more than one embedded assembly you need to check which  
            ' one to load. 

            ' Load embedded assemblies  
            Dim stream As System.IO.Stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Kerbal_Space_Program_Mod_Manager.Ionic.Zip.dll")
            Dim raw(stream.Length) As Byte
            stream.Read(raw, 0, stream.Length)
            Return System.Reflection.Assembly.Load(raw)

        End Function

    End Class

End Namespace


