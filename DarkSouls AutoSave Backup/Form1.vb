Public Class Form1
    Dim dir As String = "C:\"
    Dim sw As IO.StreamWriter
    Dim sr As IO.StreamReader
    Dim config As DataTable
    Dim lines As Array



    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If IO.File.Exists(System.Windows.Forms.Application.StartupPath + "Ven.txt") Then
            sr = New IO.StreamReader(System.Windows.Forms.Application.StartupPath + "Ven.txt")

            Try
                lines = sr.ReadLine.Split(vbTab)
                dir = lines(1)
                PopulateStatus()
            Catch EmptyConfigFile As Exception
                PopulateStatus()

            End Try
            sr.Close()

        Else
            sw = New IO.StreamWriter(System.Windows.Forms.Application.StartupPath + "Ven.txt")
            Call ChangePath()
        End If




    End Sub

    Public Function ChangePath()
        Dim Bool As Boolean = False

        If MessageBox.Show("Please select the location of the save game files", "Set Location", MessageBoxButtons.OKCancel) = Windows.Forms.DialogResult.OK Then
            fbdSelectDir.ShowDialog()

            Do While Bool = False
                If IO.File.Exists(fbdSelectDir.SelectedPath + "DRAKS00005.sl2") = False Then
                    MessageBox.Show("Dark Souls save game file does not exist in location. Please select a valid save game location", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    fbdSelectDir.ShowDialog()
                Else : Bool = True
                End If
            Loop


            dir = fbdSelectDir.SelectedPath


            PopulateStatus()
            sw.WriteLine("Path:" + vbTab + dir)
            sw.Flush()
            sw.Close()
        Else
            sw.Flush()
            sw.Close()
        End If

    End Function

    Public Function PopulateStatus()
        lblStatus.Text = "Path Selected: " + dir
    End Function

    Private Sub lblStatus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblStatus.Click
       

        sw = New IO.StreamWriter(System.Windows.Forms.Application.StartupPath + "Ven.txt")
        ChangePath()
    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click
        If Label1.Text = "STOPPED ..." Then
            Label1.Text = "RUNNING ..."
            Timer1.Enabled = True
            Timer1_Tick(sender, e)
        Else : Label1.Text = "STOPPED ..."
            Timer1.Enabled = False
        End If

    End Sub

    Public Function RunBackup()

        For i As Integer = 1 To 20
            If IO.Directory.Exists(dir + CStr(Format(Today, "yyyy-MM-dd")) + " " + CStr(i)) = True Then
            Else
                IO.Directory.CreateDirectory(dir + CStr(Format(Today, "yyyy-MM-dd")) + " " + CStr(i))
                IO.File.Copy(dir + "DRAKS00005.sl2", dir + CStr(Format(Today, "yyyy-MM-dd")) + " " + CStr(i) + "\DRAKS00005.sl2")
                Exit For
            End If

        Next

    End Function

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        RunBackup()
    End Sub
End Class
