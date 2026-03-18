
Imports Pharmacy.GlobalFunctions
Imports Pharmacy.GlobalVariables
Imports System.Data.SqlClient


Public Class frmExpirDrugsList



    ' ****************************************************************************************************************************
    ' **********    ΔΙΑΧΕΙΡΗΣΗ ΑΡΧΕΙΟΥ ΛΗΓΜΕΝΩΝ ΦΑΡΜΑΚΩΝ    ********************************************************************************
    ' ****************************************************************************************************************************


    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        ' Καθορίζει το ενημερωτικό flashing label
        timerLabel = lblCustomerEditMessage

        'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
        If btnEdit.Text = "Edit" Then

            ChangeControlsCustomerEdit(True)

            'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
        ElseIf btnEdit.Text = "Cancel" Then

            ChangeControlsCustomerEdit(False)

        End If

    End Sub



    Private Sub ChangeControlsCustomerEdit(ByVal selector As Boolean)
        ' Selector: True -> Edit
        '           False -> Cancel, Save

        ' Γεμίζει το DatagridView 
        stringDTG = "SELECT Drugs.Name, DateDiff(month, getdate(), concat(convert(varchar(4),ExpYear), '-',convert(varchar(2),ExpMonth), '-01')) as Duration, " & _
                                            "concat(convert(varchar(2),ExpMonth),'-',convert(varchar(4),ExpYear)) as ExpDate3, Expirations.Id " & _
                                    "FROM Drugs INNER JOIN Expirations ON Drugs.Id = Expirations.DrugId " & _
                                    "WHERE DateDiff(month, getdate(), concat(convert(varchar(4),ExpYear), '-',convert(varchar(2),ExpMonth), '-01')) < " & months & " " & _
                                    "ORDER BY Duration, Drugs.name"

        grbCustomerEdit.Text = "Σύνολο: " & FillDatagrid(dgvCustomers, bsCustomersEdit, {"Όνομα", "Λήξη", "Λήξη"}, {200, 50, 70}, {"0", "##-####", "0"}, {"Id", "Duration"})

        ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
        EditDatagrid({btnSave, btnEdit, btnDelete}, dgvCustomers, selector)

        ' Δεν επιτρέπει να προστεθούν νέα αρχεία
        'dgvCustomers.AllowUserToAddRows = False


        'Δεν αφήνει να αλλαχτούν τα ονόματα των φαρμάκων και οι Ημέρες από σήμερα
        dgvCustomers.Columns(2).ReadOnly = True
        dgvCustomers.Columns(0).ReadOnly = True

        ' Ενεργοποίηση ενημερωτικού flashing label
        timerLabel.Visible = selector
        tmrFlashLabel.Enabled = selector



    End Sub



    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
        ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
        UpdateDatagrid({btnSave, btnEdit, btnDelete}, dgvCustomers)

        ' Τροποποιεί τα Controls της τρέχουσας form
        ChangeControlsCustomerEdit(False)

    End Sub



    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        ' Βρίσκει το Id του ληγμένου φαρμάκου
        Dim Id As Integer = dgvCustomers.Rows(dgvCustomers.SelectedRows(0).Index).Cells(3).Value

        ' Επιβεβαίωση της διαγραφής
        If MessageBox.Show("Θέλετε να διαγράψετε την εγγραφή # " & Id & " ?", "Delete", MessageBoxButtons.YesNo) = DialogResult.Yes Then

            ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
            dgvCustomers.Rows.Remove(dgvCustomers.Rows(dgvCustomers.SelectedCells.Item(0).RowIndex))

            ' Διαγραφή από το SQL Database
            Dim sqlString As String = "DELETE FROM Expirations " & _
                                     "WHERE Id = '" & Id & "'"
            DeleteRecordById(sqlString)

        End If

        ' Τροποποιεί τα Controls της τρέχουσας form
        ChangeControlsCustomerEdit(False)

    End Sub




    ' Αναλαμβάνει να αρχίσει να αναβοσβήνει ένα label ενημέρωσης
    Private Sub FlashingLabel(ByVal oLabel As Label, ByVal interval As Integer)

        If oLabel.ForeColor = SystemColors.ControlText Then
            oLabel.ForeColor = Color.Red
        Else
            oLabel.ForeColor = SystemColors.ControlText
        End If

    End Sub

    ' Κάθε φορά που ο Timer κάνει tick τρέχει η ρουτίνα που αναβοσβήνει το label
    Private Sub tmrFlashLabel_Tick(sender As Object, e As EventArgs) Handles tmrFlashLabel.Tick
        FlashingLabel(timerLabel, 500)
    End Sub

    'Private Sub frmCustomersEdit_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
    '    frmCustomers.BringToFront()
    '    frmCustomers.grpCustomers.Enabled = True
    '    frmCustomers.grpDebts.Enabled = True
    '    frmCustomers.grpHairDies.Enabled = True
    '    frmCustomers.MinimizeBox = True
    'End Sub

    ' Με το που φορτώνεται η φορμα κάνει τα εξής...
    Private Sub frmCustomersEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Καθορίζει το ενημερωτικό flashing label
        timerLabel = lblCustomerEditMessage

        ' Τίτλος φόρμα
        Me.Text = "Φάρμακα που λήγουν σε " & months & " μήνες"

        'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
        If btnEdit.Text = "Edit" Then

            ChangeControlsCustomerEdit(False)

            'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
        ElseIf btnEdit.Text = "Cancel" Then

            ChangeControlsCustomerEdit(True)

        End If
    End Sub


    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
        frmCustomers.BringToFront()

    End Sub


    Private Sub dgvCustomers_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvCustomers.CellValidating
        Dim headerText As String = dgvCustomers.Columns(e.ColumnIndex).HeaderText

        If headerText.Equals("Ημερ. λήξης") Then
            Dim dt As DateTime

            ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι ημερομηνία
            If e.FormattedValue.ToString <> String.Empty AndAlso Not DateTime.TryParse(e.FormattedValue.ToString, dt) Then
                MessageBox.Show("Λάθος καταχώρηση ημερομηνίας", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                e.Cancel = True

            End If

        End If

    End Sub



    Private Sub dgvDebts_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCustomers.CellEndEdit
        Try
            ' Clear the row error in case the user presses ESC.   
            dgvCustomers.Rows(e.RowIndex).ErrorText = String.Empty
        Catch ex As Exception
        End Try

    End Sub

End Class