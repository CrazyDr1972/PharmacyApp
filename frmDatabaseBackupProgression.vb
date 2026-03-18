Imports Pharmacy.GlobalFunctions
Imports Pharmacy.GlobalVariables
Imports System.Data.SqlClient

Public Class frmDatabaseBackupProgression



    Private Sub BackupDatabaseFiles()


        Dim sql As String = ""
        Dim folderDB As String = frmCustomers.txtSourceDB.Text
        Dim folderVSFiles As String = frmCustomers.txtSourceFolderVS.Text
        Dim folderUSB As String = frmCustomers.txtDestinationDrive.Text
        Dim InitTime As DateTime = Now()
        Dim TimeDiff As Integer = 0
        Dim lstIndex As Integer = 0


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)

            ' Βγάζει τα database offline
            lstMessage.Items.Add("-----------------------------")
            lstMessage.Items.Add("Setting databases offline... ")
            lstIndex += 1
            lstMessage.Refresh()

            con.Open()
            sql = "ALTER DATABASE [" & strDB1 & "] SET Offline WITH ROLLBACK IMMEDIATE " & _
                "ALTER DATABASE [" & strDB2 & "] SET Offline WITH ROLLBACK IMMEDIATE "
            Dim cmd As New SqlCommand(sql, con)
            cmd.ExecuteNonQuery()

            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()

            ' Αν δεν υπάρχει το directory τότε το δημιουργεί
            Try
                If (Not System.IO.Directory.Exists(folderUSB)) Then
                    lstMessage.Items.Add("Target dir... Creating... ")
                    lstIndex += 1
                    lstMessage.Refresh()
                    System.IO.Directory.CreateDirectory(folderUSB)
                    lstMessage.Items(lstIndex) &= "OK"
                    lstMessage.Refresh()
                Else
                    lstMessage.Items.Add("Target dir... Deleting... ")
                    lstIndex += 1
                    lstMessage.Refresh()
                    My.Computer.FileSystem.DeleteDirectory(folderUSB, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    lstMessage.Items(lstIndex) &= "Recreating... "
                    lstMessage.Refresh()
                    My.Computer.FileSystem.CreateDirectory(folderUSB)
                    lstMessage.Items(lstIndex) &= "OK"
                    lstMessage.Refresh()
                End If
            Catch ex As Exception
                lstMessage.Items.Add("!!! Error !!! ")
                lstIndex += 1
                lstMessage.Refresh()
            End Try


            ' Αντιγράφει τα database
            lstMessage.Items.Add("Copying databases to usb... ")
            lstIndex += 1
            lstMessage.Refresh()

            Try
                lstMessage.Items.Add("   " & frmCustomers.txtDB1.Text & ".mdf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.CopyFile(folderDB & "\" & frmCustomers.txtDB1.Text & ".mdf", folderUSB & "\" & frmCustomers.txtDB1.Text & ".mdf", FileIO.UIOption.OnlyErrorDialogs)
                VerifyDB(frmCustomers.txtDB1.Text & ".mdf", lstIndex)

                lstMessage.Items.Add("   " & frmCustomers.txtDB1.Text & ".ldf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.CopyFile(folderDB & "\" & frmCustomers.txtDB1.Text & ".ldf", folderUSB & "\" & frmCustomers.txtDB1.Text & ".ldf", FileIO.UIOption.OnlyErrorDialogs)
                VerifyDB(frmCustomers.txtDB1.Text & ".ldf", lstIndex)

                lstMessage.Items.Add("   " & frmCustomers.txtDB2.Text & ".mdf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.CopyFile(folderDB & "\" & frmCustomers.txtDB2.Text & ".mdf", folderUSB & "\" & frmCustomers.txtDB2.Text & ".mdf", FileIO.UIOption.OnlyErrorDialogs)
                VerifyDB(frmCustomers.txtDB2.Text & ".mdf", lstIndex)

                lstMessage.Items.Add("   " & frmCustomers.txtDB2.Text & "_log.ldf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.CopyFile(folderDB & "\" & frmCustomers.txtDB2.Text & "_log.ldf", folderUSB & "\" & frmCustomers.txtDB2.Text & "_log.ldf", FileIO.UIOption.AllDialogs)
                VerifyDB(frmCustomers.txtDB2.Text & "_log.ldf", lstIndex)

            Catch ex As Exception
                lstMessage.Items.Add("!!! Error !!! ")
                lstMessage.Refresh()
            End Try

            ' Βγάζει τα database online
            lstMessage.Items.Add("Setting databases online... ")
            lstIndex += 1
            lstMessage.Refresh()

            sql = "ALTER DATABASE [" & strDB1 & "] SET Online " & _
                 "ALTER DATABASE [" & strDB2 & "] SET Online"
            cmd = New SqlCommand(sql, con)
            cmd.ExecuteNonQuery()

            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()

            TimeDiff = DateDiff(DateInterval.Second, InitTime, Now())

            lstMessage.Items.Add("BackUp of Databases to USB completed in " & (TimeDiff \ 60) & ":" & ((TimeDiff / 60) - (TimeDiff \ 60)) * 60)
            lstMessage.Items.Add("-------------------------------------")
            lstIndex += 2
            lstMessage.Refresh()

        End Using

        DatabaseBackupTaken = True

    End Sub

    Private Sub VerifyDB(ByVal db As String, ByVal lstIndex As Double)
        Dim infoReaderUSB, infoReaderHD As System.IO.FileInfo
        Dim folderDB As String = frmCustomers.txtSourceDB.Text
        Dim folderVSFiles As String = frmCustomers.txtSourceFolderVS.Text
        Dim folderUSB As String = frmCustomers.txtDestinationDrive.Text

        Try
            infoReaderHD = My.Computer.FileSystem.GetFileInfo(folderDB & "\" & db)
            infoReaderUSB = My.Computer.FileSystem.GetFileInfo(folderUSB & "\" & db)
            If infoReaderHD.Length = infoReaderUSB.Length Then
                lstMessage.Items(lstIndex) &= "Verified..." & infoReaderHD.Length & " bytes"
                lstMessage.Refresh()
            End If
        Catch ex As Exception
            lstMessage.Items(lstIndex) &= "Verification Failed !"
            lstMessage.Refresh()
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        BackupDatabaseFiles()
    End Sub


    Private Sub frmDatabaseBackupProgression_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Enabled = True
    End Sub

    Private Sub Timer1_Tick_1(sender As Object, e As EventArgs) Handles Timer1.Tick
        If DatabaseBackupTaken = False Then
            frmCustomers.Enabled = False
            BackupDatabaseFiles()
        Else
            frmCustomers.Enabled = True
            Me.Close()

        End If
    End Sub
End Class