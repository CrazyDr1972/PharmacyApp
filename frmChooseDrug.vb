Imports Pharmacy.GlobalFunctions
Imports Pharmacy.GlobalVariables
Imports System.Data.SqlClient
Imports System.Windows.Forms.Timer
Imports System.Threading.Timer
Imports System.Globalization
Imports System.Threading


Public Class frmChooseDrug

    Private Sub GetDrugsList()

        Dim myTot As Integer

        stringDTG = "SELECT distinct [AP_DESCRIPTION] " & _
                "FROM [dbo].[APOTIKH] " & _
                "WHERE AP_DESCRIPTION like '%" & txtSearchDrugsByName.Text & "%' " & _
                "ORDER BY [AP_DESCRIPTION]"

        myTot = FillDatagrid(dgvDrugsNew, bsDrugsNew, {"Προιόν"}, {300}, {"0"}, {})

        grpDrugDescription.Text = "Σύνολο προίοντων: " & myTot



    End Sub

    Private Sub GetMorfesList()

        Dim myTot As Integer
        Dim Description As String = ""

        Try
            Description = dgvDrugsNew.SelectedRows(0).Cells(0).Value
        Catch ex As Exception
        End Try

        stringDTG = "SELECT [AP_MORFI], [AP_CODE], [AP_ID] " & _
              "FROM [dbo].[APOTIKH] " & _
              "WHERE AP_DESCRIPTION = '" & Description & "' " & _
              "ORDER BY [AP_MORFI]"

        myTot = FillDatagrid(dgvMorfesNew, bsMorfesNew, {"Μορφές"}, {220}, {"0"}, {"AP_CODE", "AP_ID"})

    End Sub



    Private Sub GetOtherDetails()


        Dim SQLSTRING As String = "SELECT APOTIKH_BARCODES.BRAP_AP_BARCODE " & _
                                   "FROM APOTIKH left JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID " & _
                                   "WHERE APOTIKH.AP_DESCRIPTION = '" & dgvDrugsNew.SelectedRows(0).Cells(0).Value & "' AND APOTIKH.AP_MORFI = '" & dgvMorfesNew.SelectedRows(0).Cells(0).Value & "' "

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionString)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(SQLSTRING, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then
                    Do While myReader.Read()

                        ' Aν είμαστε σε φάση ανταλλαγής ΑΠΟ
                        If AreExchangedDrugsFrom = True Then

                            frmCustomers.dgvExchangeFrom2.Rows(rowIndex).Cells(0).Value = myReader("BRAP_AP_BARCODE")

                            'αλλιως
                        Else
                            frmCustomers.dgvExchangeTo2.Rows(rowIndex).Cells(0).Value = myReader("BRAP_AP_BARCODE")
                        End If


                    Loop
                Else
                End If

            End Using
        End Using


        SQLSTRING = "SELECT APOTIKH.AP_TIMH_XON " & _
                                 "FROM APOTIKH " & _
                                 "WHERE APOTIKH.AP_DESCRIPTION = '" & dgvDrugsNew.SelectedRows(0).Cells(0).Value & "' AND APOTIKH.AP_MORFI = '" & dgvMorfesNew.SelectedRows(0).Cells(0).Value & "' "

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionString)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(SQLSTRING, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then
                    Do While myReader.Read()

                        ' Aν είμαστε σε φάση ανταλλαγής ΑΠΟ
                        If AreExchangedDrugsFrom = True Then
                            frmCustomers.dgvExchangeFrom2.Rows(rowIndex).Cells(4).Value = GetEffectiveDrugXondr(Convert.ToString(grpDrugDetails.Text), CType(myReader("AP_TIMH_XON"), Decimal))

                            'αλλιως
                        Else

                            frmCustomers.dgvExchangeTo2.Rows(rowIndex).Cells(4).Value = GetEffectiveDrugXondr(Convert.ToString(grpDrugDetails.Text), CType(myReader("AP_TIMH_XON"), Decimal))

                        End If
                    Loop
                Else
                End If

            End Using
        End Using

    End Sub


    Private Sub frmChooseDrug_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        GetDrugsList()

        GetMorfesList()

        txtSearchDrugsByName.Select()

    End Sub

    







    Private Sub dgvMorfesNew_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvMorfesNew.CellClick
        'Αν επιλέγουμε φάρμακα ΠΡΟΣ
        If AreExchangedDrugsFrom = True Then

            'Εμφανίζει στο DatagridView
            frmCustomers.dgvExchangeFrom2.Rows(rowIndex).Cells(1).Value = dgvDrugsNew.SelectedRows(0).Cells(0).Value.ToString ' όνομα προιόντος
            frmCustomers.dgvExchangeFrom2.Rows(rowIndex).Cells(2).Value = dgvMorfesNew.SelectedRows(0).Cells(0).Value.ToString ' μορφή προιόντος
            frmCustomers.dgvExchangeFrom2.Rows(rowIndex).Cells(10).Value = dgvMorfesNew.SelectedRows(0).Cells(2).Value.ToString ' DrugId

            GetOtherDetails() ' Barcode & Χονδρική

            'Θέτει τον κέρσορα στά Τεμάχια και πειρμένει καταχώρηση
            frmCustomers.dgvExchangeFrom2.CurrentCell = frmCustomers.dgvExchangeFrom2.Item(3, rowIndex)
            frmCustomers.dgvExchangeFrom2.BeginEdit(True)

            ' Κλείνει τηνπαρούσα form
            Me.Close()

            ' ΑΛΛΙΩΣ
        Else

            'Εμφανίζει στο DatagridView
            frmCustomers.dgvExchangeTo2.Rows(rowIndex).Cells(1).Value = dgvDrugsNew.SelectedRows(0).Cells(0).Value.ToString ' όνομα προιόντος
            frmCustomers.dgvExchangeTo2.Rows(rowIndex).Cells(2).Value = dgvMorfesNew.SelectedRows(0).Cells(0).Value.ToString ' μορφή προιόντος
            frmCustomers.dgvExchangeTo2.Rows(rowIndex).Cells(10).Value = dgvMorfesNew.SelectedRows(0).Cells(2).Value.ToString ' DrugId

            GetOtherDetails() ' Barcode & Χονδρική

            'Θέτει τον κέρσορα στά Τεμάχια και πειρμένει καταχώρηση
            frmCustomers.dgvExchangeTo2.CurrentCell = frmCustomers.dgvExchangeTo2.Item(3, rowIndex)
            frmCustomers.dgvExchangeTo2.BeginEdit(True)

            ' Κλείνει τηνπαρούσα form
            Me.Close()
        End If


    End Sub

    Private Sub dgvDrugsNew_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDrugsNew.CellClick

        GetMorfesList()

    End Sub


    'Private Sub GetDetailsFromBarcode(ByVal barcode As String, destinationDatagrid As DataGridView)

    '    Dim stringSQL As String = "SELECT APOTIKH_BARCODES.BRAP_AP_BARCODE, APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.AP_TIMH_XON, APOTIKH.AP_ID " & _
    '                                "FROM APOTIKH left JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID " & _
    '                                "WHERE APOTIKH_BARCODES.BRAP_AP_BARCODE= @barcode"
    '    Dim rowIndex As Integer = frmCustomers.dgvExchangeFrom2.CurrentCell.RowIndex

    '    Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionString)
    '        con.Open()

    '        Dim cmd As New SqlCommand(stringSQL, con)
    '        cmd.Parameters.AddWithValue("@barcode", barcode)

    '        'Ορισμός ExecuteReader 
    '        Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

    '        If myReader.HasRows Then
    '            Do While myReader.Read()

    '                'Εμφανίζει στο Datagrid τις αντίστοιχες τιμές
    '                destinationDatagrid.Rows(rowIndex).Cells(0).Value = myReader(0) ' barcode
    '                destinationDatagrid.Rows(rowIndex).Cells(1).Value = myReader(1) ' όνομα
    '                destinationDatagrid.Rows(rowIndex).Cells(2).Value = myReader(2) ' μορφή
    '                destinationDatagrid.Rows(rowIndex).Cells(4).Value = myReader(3) ' χονδρική
    '                destinationDatagrid.Rows(rowIndex).Cells(10).Value = myReader(4) ' DrugId
    '            Loop
    '        Else
    '            MsgBox("To barcode '" & barcode & "' δεν υπάρχει στο αρχείο μου")
    '        End If


    '        'Θέτει τον κέρσορα στά Τεμάχια και πειρμένει καταχώρηση
    '        destinationDatagrid.CurrentCell = destinationDatagrid.Item(3, rowIndex)
    '        destinationDatagrid.BeginEdit(True)


    '    End Using

    'End Sub


    Private Sub GetDetailsFromBarcode(ByVal barcode As String, ByVal sourceDatagrid As DataGridView)

        Dim stringSQL As String = "SELECT APOTIKH_BARCODES.BRAP_AP_BARCODE, APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.AP_TIMH_XON, APOTIKH.AP_ID, apotikh.ap_code " & _
                                    "FROM APOTIKH left JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID " & _
                                    "WHERE APOTIKH_BARCODES.BRAP_AP_BARCODE= @barcode"
        Dim rowIndex As Integer = sourceDatagrid.CurrentCell.RowIndex

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            Dim cmd As New SqlCommand(stringSQL, con)
            cmd.Parameters.AddWithValue("@barcode", barcode)

            'Ορισμός ExecuteReader 
            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then
                Do While myReader.Read()

                    'Εμφανίζει στα τις αντίστοιχες τιμές
                    txtAP_Description.Text = myReader(1) ' όνομα
                    txtAP_Morfi.Text = myReader(2) ' μορφή
                    txtAP_Xondriki.Text = myReader(3) ' χονδρική
                    grpDrugDetails.Text = myReader(5) ' DrugId (ap_code)

                    'Εμφανίζει στο Datagrid τις αντίστοιχες τιμές
                    sourceDatagrid.Rows(rowIndex).Cells(0).Value = myReader(0) ' barcode
                    sourceDatagrid.Rows(rowIndex).Cells(1).Value = myReader(1) ' όνομα
                    sourceDatagrid.Rows(rowIndex).Cells(2).Value = myReader(2) ' μορφή
                    sourceDatagrid.Rows(rowIndex).Cells(4).Value = myReader(3) ' χονδρική
                    sourceDatagrid.Rows(rowIndex).Cells(10).Value = myReader(5) ' DrugId (ap_code)
                Loop
            Else
                MsgBox("To barcode '" & barcode & "' δεν υπάρχει στο αρχείο μου")
            End If

            If frmCustomers.chkWithExpir.Checked = False Then

                'Θέτει τον κέρσορα στά Τεμάχια και πειρμένει καταχώρηση
                sourceDatagrid.CurrentCell = sourceDatagrid.Item(3, rowIndex)
                sourceDatagrid.BeginEdit(True)

            ElseIf frmCustomers.chkWithExpir.Checked = True Then

                'frmExpirationsAddNew.Show()

            End If



        End Using

    End Sub


    Private Sub tmrExchanges_Tick(sender As Object, e As EventArgs) Handles tmrExchanges.Tick
        'Αν επιλέγουμε φάρμακα ΑΠΟ
        If AreExchangedDrugsFrom = True Then

            GetDetailsFromBarcode(txtSearchDrugsByBarcode.Text, frmCustomers.dgvExchangeFrom2)

            tmrExchanges.Enabled = False
            Me.Close()

            'Αν επιλέγουμε φάρμακα ΠΡΟΣ
        Else
            GetDetailsFromBarcode(txtSearchDrugsByBarcode.Text, frmCustomers.dgvExchangeTo2)

            tmrExchanges.Enabled = False
            Me.Close()
        End If


    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click

        If AreExchangedDrugsFrom = True Then
            frmCustomers.dgvExchangeFrom2.Rows(rowIndex).Cells(10).Value = 0 ' DrugId

            'Θέτει τον κέρσορα στά Τεμάχια και πειρμένει καταχώρηση
            frmCustomers.dgvExchangeFrom2.CurrentCell = frmCustomers.dgvExchangeFrom2.Item(1, rowIndex)
            frmCustomers.dgvExchangeFrom2.BeginEdit(True)
        Else
            frmCustomers.dgvExchangeTo2.Rows(rowIndex).Cells(10).Value = 0 ' DrugId

            'Θέτει τον κέρσορα στά Τεμάχια και πειρμένει καταχώρηση
            frmCustomers.dgvExchangeTo2.CurrentCell = frmCustomers.dgvExchangeTo2.Item(1, rowIndex)
            frmCustomers.dgvExchangeTo2.BeginEdit(True)
        End If

        Me.Close()

    End Sub



    Private Sub txtSearchDrugsByName_TextChanged(sender As Object, e As EventArgs) Handles txtSearchDrugsByName.TextChanged

        ' τον απενεργοποιεί
        tmrExchanges.Enabled = False

        If String.IsNullOrWhiteSpace(txtSearchDrugsByName.Text) Then
            GetDrugsList()
            GetMorfesList()
        End If

    End Sub

    Private Sub txtSearchDrugsByName_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearchDrugsByName.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            tmrExchanges.Enabled = False
            GetDrugsList()
            GetMorfesList()
        End If
    End Sub

    Private Sub txtSearchDrugsByBarcode_TextChanged(sender As Object, e As EventArgs) Handles txtSearchDrugsByBarcode.TextChanged

        ' δεν κάνει αυτόματη αναζήτηση σε κάθε χαρακτήρα
        tmrExchanges.Enabled = False

    End Sub

    Private Sub txtSearchDrugsByBarcode_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearchDrugsByBarcode.KeyDown
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Tab Then
            e.SuppressKeyPress = True
            tmrExchanges.Enabled = False
            tmrExchanges_Tick(tmrExchanges, EventArgs.Empty)
        End If
    End Sub

    Private Sub chkBarcodeManually_CheckedChanged(sender As Object, e As EventArgs) Handles chkBarcodeManually.CheckedChanged
        If chkBarcodeManually.Checked = True Then
            ' τον απενεργοποιεί
            tmrExchanges.Enabled = False
        Else
            ' ενεργοποιεί τον timer
            tmrExchanges.Enabled = True
        End If
    End Sub

    Private Sub dgvMorfesNew_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvMorfesNew.CellContentClick

    End Sub
End Class
