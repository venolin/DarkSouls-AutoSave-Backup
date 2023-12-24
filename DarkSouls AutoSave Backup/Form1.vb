Imports System.Runtime.InteropServices
Imports System.Media

Public Class Form1
    Dim dir As String = "C:\"
    Dim sw As IO.StreamWriter
    Dim sr As IO.StreamReader
    Dim config As DataTable
    Dim lines As Array
    Dim logLineCount As Integer

    Private mediaPlayer As New SoundPlayer(System.Windows.Forms.Application.StartupPath + "\Resources\Sounds\Manual save completed.wav")

    ' Import the necessary Windows API functions
    <DllImport("user32.dll")>
    Private Shared Function RegisterHotKey(ByVal hWnd As IntPtr, ByVal id As Integer, ByVal fsModifiers As UInteger, ByVal vk As UInteger) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function UnregisterHotKey(ByVal hWnd As IntPtr, ByVal id As Integer) As Boolean
    End Function

    ' Define the key modifiers
    Private Const MOD_CTRLALT As UInteger = &H3 ' Ctrl + Alt

    ' Define the key code for 'S'
    Private Const VK_S As UInteger = &H53

    ' Define a unique identifier for the hotkey
    Private Const HOTKEY_ID As Integer = 1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Set the form title to include the publish version
        Me.Text = "Game AutoSave Backup - Version " & GetPublishVersion()

        ''Set the BackColor of the label to white with 50% transparency
        'Label2.BackColor = Color.FromArgb(64, Color.White)

        ' Register the hotkey when the form loads
        RegisterHotKey(Me.Handle, HOTKEY_ID, MOD_CTRLALT, VK_S)

        If IO.File.Exists(System.Windows.Forms.Application.StartupPath + "\Ven.txt") Then
            sr = New IO.StreamReader(System.Windows.Forms.Application.StartupPath + "\Ven.txt")

            Try
                lines = sr.ReadLine.Split(vbTab)
                dir = lines(1)
                PopulateStatus()
            Catch EmptyConfigFile As Exception
                PopulateStatus()

            End Try
            sr.Close()

        Else
            sw = New IO.StreamWriter(System.Windows.Forms.Application.StartupPath + "\Ven.txt")
            Call ChangePath()
        End If

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Unregister the hotkey when the form is closing
        UnregisterHotKey(Me.Handle, HOTKEY_ID)
    End Sub

    Public Function ChangePath()
        Dim Bool As Boolean = False

        If MessageBox.Show("Please select the location of the save game files", "Set Location", MessageBoxButtons.OKCancel) = Windows.Forms.DialogResult.OK Then
            fbdSelectDir.ShowDialog()

            If fbdSelectDir.ShowDialog.OK Then
                dir = fbdSelectDir.SelectedPath
            Else
            End If

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


        sw = New IO.StreamWriter(System.Windows.Forms.Application.StartupPath + "\Ven.txt")
        ChangePath()
    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click
        ToggleProcess()
    End Sub

    Private Sub ToggleProcess()
        If Label1.Text = "STOPPED ..." Then
            Label1.Text = "RUNNING ..."
            Timer1.Enabled = True
            Label2.Text = Label2.Text & vbNewLine & "Running ... " & Now()
            Timer1_Tick(Me, EventArgs.Empty)
        Else
            Label1.Text = "STOPPED ..."
            Timer1.Enabled = False
            Label2.Text = Label2.Text & vbNewLine & "Stopped ... " & Now()
        End If
    End Sub

    Public Function RunBackup(Message As String)

        My.Computer.FileSystem.CopyDirectory(dir, System.Windows.Forms.Application.StartupPath + "\" + CStr(Format(Now, "yyyy-MM-dd HH.mm.ss")) + Message)
        Label2.Text = Label2.Text + vbNewLine + "Backup ... " + Now() + Message

    End Function

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        RunBackup("")
    End Sub

    Public Function OutputEcho(Info As String) As String
        logLineCount += 1
        Return vbNewLine + Info + Now()

    End Function

    Public Function DisplayCleaner()
        If logLineCount >= 12 Then

        End If
    End Function

    Protected Overrides Sub WndProc(ByRef m As Message)
        ' Handle the hotkey message
        If m.Msg = &H312 AndAlso m.WParam.ToInt32() = HOTKEY_ID Then
            ' Hotkey pressed, handle the event
            RunBackup(" Saved manually")
            mediaPlayer.Play()
        End If

        MyBase.WndProc(m)
    End Sub

    Private Function GetPublishVersion() As String
        ' Retrieve the publish version from the application manifest
        Return My.Application.Info.Version.ToString()
    End Function
End Class
