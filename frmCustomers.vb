Imports Pharmacy.GlobalFunctions
Imports Pharmacy.GlobalVariables
Imports System.Data.SqlClient
Imports System.Windows.Forms.Timer
Imports System.Threading.Timer
Imports System.Globalization
Imports System.Threading
Imports System.IO
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.Configuration
Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Math
Imports System.Runtime.Remoting.Messaging
Imports System.Security.Principal
Imports System.Diagnostics
Imports System.Text.RegularExpressions




Public Class frmCustomers

    ' Timer “debounce” 4″ για αναζήτηση μετά την πληκτρολόγηση
    Private WithEvents tmrCellLookup As New System.Windows.Forms.Timer() With {.Interval = 30}

    ' Κρατάμε θέση κελιού & παλιά τιμή για restore αν δεν βρεθεί αποτέλεσμα
    Private _pendingRow As Integer = -1
    Private _pendingCol As Integer = -1
    Private _oldCellValue As Object = Nothing
    ' --- Νέες μεταβλητές module επιπέδου ---
    Private _lastObservedValue As String = Nothing
    Private _stableCount As Integer = 0   ' πόσα συνεχόμενα ticks η τιμή μένει ίδια
    ' πάνω-πάνω στο frmCustomers
    Private _suppressDebtsRowLeave As Boolean = False
    Private _debounceSnap As String = ""
    Private _suppressSelectionChanged As Boolean = False
    Private _isRebinding As Boolean = False
    Private _suppressSearchTextChanged As Boolean = False
    Private WithEvents tmrSel As New System.Windows.Forms.Timer() With {.Interval = 250}
    Private _pendingCustomerId As Integer = -1
    ' ======= [frmCustomers] Paradrugs search – cloned from frmChooseParadrugFromCatalog =======
    Private WithEvents tmrTextboxEntry As New System.Windows.Forms.Timer() With {.Interval = 300}
    Private lastKeyWasAlphaNumeric As Boolean = False
    Private _loadingChooseFromCatalog As Boolean = False   ' guard όπως στο choose-form
    Private barcodeType As String = ""                     ' "name" / "barcode" / "qrcode"
    Private _pricesCellOldValue As String = ""
    Private _pricesCellOldRow As Integer = -1
    Private _pricesCellOldColumn As Integer = -1
    Private _suppressDrugQrCellEvents As Boolean = False
    ' module-level
    Private _editSw As New Stopwatch()
    Private _enterPressedThisEdit As Boolean = False
    Private Const STABLE_TICKS As Integer = 12           ' ~360ms με Interval 30ms
    Private Const MAX_EDIT_MS_FOR_SCANNER As Integer = 600
    Private _suppressCellValueChanged As Boolean = False
    Private _forceLastAfterBind As Boolean = False
    ' Καταστολή επιβεβαίωσης μέσα στην UpdateCustomer3 όταν έχουμε ήδη επιβεβαιώσει στο RowLeave
    Private _confirmingFromRowLeave As Boolean = False
    ' Κορυφή του frmCustomers:
    Private _suppressDebtsValidation As Boolean = False







    Private Sub frmCustomer_Load(sender As Object, e As EventArgs) Handles Me.Load

        Thread.CurrentThread.CurrentCulture = New CultureInfo("el-GR", False)

        SetPCTerminal()

        'Κρατάει αντίγραφα των database σε κάθε έναρξη προγράμματος
        BackUpDatabaseAtStarting()

        'MsgBox(GetDatabaseStatus("Pharmacy2013C"))

        'Βάζει στα αντίστοιχα textbox τις τιμές
        txtSourceDB.Text = strDBFolder
        txtConnectionString.Text = connectionstring
        txtPCName.Text = My.Computer.Name
        txtSourceFarmnet_mdf.Text = strDB1_Source

        ' dgvDebts_ScanHint
        lblScanHint.Text = "Πληκτρολόγηση: πάτα Enter για εισαγωγή"
        lblScanHint.Font = New Font(Me.Font, FontStyle.Bold)
        lblScanHint.ForeColor = Color.DarkOrange
        lblScanHint.BackColor = Color.Transparent

        FillComboBox(cbExchangers, GetDistinctContentsDBField(
                  "SELECT ExchangerName FROM PharmacyCustomFiles.dbo.ExchangerList ORDER BY ExchangerName", "ExchangerName"), {})

        'Υπολογίζει την χρηματική διαφορά
        DisplayBalancePerPharmacist()

        WireUpGridEvents()
        dgvCustomers.AutoGenerateColumns = False
        dgvCustomers.EnableHeadersVisualStyles = False

        ' === Exchanges grids: allow multi deletion ===
        ApplyReadOnlyOnExchangeGrids()

        SetupGridsForMultiDelete()

        ' Φροντίζει η τιμή να είναι στη μέση
        ' Αν η στήλη λέγεται "Ποσό"
        If dgvDebtsList.Columns.Contains("Ποσό") Then
            dgvDebtsList.Columns("Ποσό").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Else
            ' Εναλλακτικά βάσει HeaderText
            For Each col As DataGridViewColumn In dgvDebtsList.Columns
                If String.Equals(col.HeaderText?.Trim(), "Ποσό", StringComparison.OrdinalIgnoreCase) Then
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    Exit For
                End If
            Next
        End If


        EnableDoubleBuffer(dgvCustomers)
        EnableDoubleBuffer(dgvDebtsList)
        EnableDoubleBuffer(dgvDrugsOnLoan)
        EnableDoubleBuffer(dgvPrescriptions)
        EnableDoubleBuffer(dgvHairdiesList)

        FillComboBox(cboMyPharmacist, GetDistinctContentsDBField(
                   "SELECT DISTINCT FromWho FROM PharmacyCustomFiles.dbo.ExchangesMaster", "FromWho"), {})

        'Εμφανίζει τις τιμές των παραφαρμάκων
        DisplayDrugsOrParadrugs()
        GetExpirationsList()

        GetAgoresOrSoldList()
        DisplayAgoresSoldTotals()
        UpdateDataTameia()
        GetPhonesList()

        GetExchangesList("given")
        GetExchangesList("taken")
        DisplayExchangesBalance()

        CalculatePreviousTotalBalance()

        DisplayLastUpdate()

        ' Βρίσκει τους πελάτες που αντιστοιχούν στις επιλογές μας και τους γράφει στο ListBox
        GetCustomersList()

        UpdateStartDateExchanges("get")

        'Eλέγχει αν το Pharmacy2013 χρειάζεται ενημέρωση
        'CheckNeedForUpdatingPharmacy2013()

        'Εμφανίζει τους SQL server
        txtSQLServer_Pharmakon.Text = PharmakonServer
        txtSQLServer_Pharmacy2013.Text = Pharmacy2013Server

        tmrCellLookup.Enabled = False

        If Not IsProcessElevated() Then
            lblAdminInfo.Visible = False
        Else
            lblAdminInfo.Visible = True
        End If

        Me.Text = "Διαχείρηση φαρμακείου! " & Version

        tmrTextboxEntry.Stop()
        tmrTextboxEntry.Interval = 300

        FixHeadersLook(dgvCustomers)
        FixHeadersLook(dgvDebtsList)


    End Sub



    Private Sub txtSearchPricesParadrugs_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearchPricesParadrugs.KeyDown
        lastKeyWasAlphaNumeric = IsAlphaNumericKey(e)

        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Tab Then
            e.SuppressKeyPress = True   ' μην αλλάζει focus / μην κάνει ding
            tmrTextboxEntry.Stop()
            ProcessScannedInput(txtSearchPricesParadrugs.Text)
        End If
    End Sub


    ' Καλείται όταν θεωρούμε ότι το scan/πληκτρολόγηση ολοκληρώθηκε
    Private Sub ProcessScannedInput(ByVal input As String)
        _loadingChooseFromCatalog = True

        Dim found As Integer, qr As String
        Dim expDate As String = ""

        ' 1) Barcode;
        If IsNumeric(input) AndAlso input.Length <= 20 Then
            barcodeType = "barcode"
            rbByBarcode.Checked = True : rbByQRcode.Checked = False : rbByName.Checked = False
        Else
            ' 2) QRCode;
            qr = GetQRFromScannedCode(input)   ' υπάρχει ήδη στο frmCustomers
            If Not String.IsNullOrEmpty(qr) AndAlso IsNumeric(qr) Then
                barcodeType = "qrcode"
                expDate = GetFirstYYMM(input)
                rbByBarcode.Checked = False : rbByQRcode.Checked = True : rbByName.Checked = False
                txtSearchPricesParadrugs.Text = qr   ' αντικατάσταση με τον πραγματικό product_code
            Else
                ' 3) Όνομα (όχι καθαρά αριθμητικό ή μεγάλα alpha-num strings)
                If Not IsNumeric(input) Then
                    barcodeType = "name"
                    rbByBarcode.Checked = False : rbByQRcode.Checked = False : rbByName.Checked = True
                Else
                    ClearPricesGrid()
                    _loadingChooseFromCatalog = False
                    Exit Sub
                End If
            End If
        End If

        ' === Τρέχουμε το αντίστοιχο query, ανά tab (Drugs/Paradrugs) ===
        If rbParadrugs.Checked Then
            ' PRICES PARADRUGS → χρησιμοποιεί την ήδη υπάρχουσα συνάρτηση
            found = GetPriceParaDrugs(barcodeType)      ' γεμίζει dgvPricesParadrugs & rtxtPricesParadrugs
            If found = 0 Then
                rbDrugs.Checked = True
            End If
        End If
        If rbDrugs.Checked Then
            ' DRUGS από APOTIKH (αν θες μόνο παραφάρμακα, άφησέ το rbParadrugs always on)
            found = GetDrugs(barcodeType)               ' ήδη υπάρχει στο frmCustomers
            If found = 0 AndAlso barcodeType = "barcode" AndAlso Not String.IsNullOrEmpty(qr) AndAlso IsNumeric(qr) Then
                barcodeType = "qrcode" : rbByQRcode.Checked = True : rbByBarcode.Checked = False : rbByName.Checked = False
                expDate = GetFirstYYMM(input)
                txtSearchPricesParadrugs.Text = qr
                found = GetDrugs("qrcode")
            End If
            If found = 0 AndAlso barcodeType <> "name" Then
                barcodeType = "name" : rbByName.Checked = True : rbByBarcode.Checked = False : rbByQRcode.Checked = False
                found = GetDrugs("name")
            End If
        End If

        If found > 0 Then
            If Not String.IsNullOrEmpty(expDate) Then
                UpsertExpirationFromYYMM(expDate)
            End If
            ' Refresh του grid
            GetExpirationsList()
        End If

        ' Focus back + επιλογή κειμένου για νέο scan
        If rbByName.Checked = False Then
            txtSearchPricesParadrugs.SelectAll()
        End If
        txtSearchPricesParadrugs.Focus()
        barcodeType = ""
        _loadingChooseFromCatalog = False
    End Sub

    Private Function IsAlphaNumericKey(e As KeyEventArgs) As Boolean
        Dim k = e.KeyCode
        ' αλφαριθμητικά + Space + Back/Delete θεωρούνται ανθρώπινη πληκτρολόγηση
        Return (k >= Keys.D0 AndAlso k <= Keys.Z) OrElse (k >= Keys.NumPad0 AndAlso k <= Keys.NumPad9) _
           OrElse k = Keys.Space OrElse k = Keys.Back OrElse k = Keys.Delete
    End Function

    Private Sub ClearPricesGrid()
        If dgvPricesParadrugs Is Nothing Then Exit Sub
        Dim dt As New DataTable()
        dgvPricesParadrugs.DataSource = dt
        rtxtPricesParadrugs.Clear()
    End Sub



    Private Sub txtSearchPricesParadrugs_TextChanged(sender As Object, e As EventArgs) Handles txtSearchPricesParadrugs.TextChanged
        If _loadingChooseFromCatalog Then Exit Sub

        ' προαιρετικό UX: αν φαίνεται ανθρώπινη πληκτρολόγηση → γύρνα σε Name
        If lastKeyWasAlphaNumeric Then
            rbByName.Checked = True : rbByBarcode.Checked = False : rbByQRcode.Checked = False
        End If

        ' Debounce: κάθε αλλαγή restart τον timer, και κρατάμε snapshot στο Tag
        tmrTextboxEntry.Stop()
        tmrTextboxEntry.Tag = txtSearchPricesParadrugs.Text
        tmrTextboxEntry.Start()
    End Sub

    Private Sub tmrTextboxEntry_Tick(sender As Object, e As EventArgs) Handles tmrTextboxEntry.Tick
        tmrTextboxEntry.Stop()
        Dim snapshot As String = TryCast(tmrTextboxEntry.Tag, String)
        ProcessScannedInput(If(snapshot, txtSearchPricesParadrugs.Text))
    End Sub



    Private Sub DeleteSelectedDebts()
        ' Σίγαση auto-save από RowLeave όσο κάνουμε delete/refresh
        _suppressDebtsRowLeave = True
        Try
            ' Κλείσε τυχόν edits ώστε να μην μείνουν «ημι-τιμές»
            dgvDebtsList.EndEdit()
            If bsDebts IsNot Nothing Then
                Try : bsDebts.EndEdit() : Catch : End Try
            End If

            ' Κάνε το πραγματικό delete
            DeleteSelectedRowsGeneric(dgvDebtsList, 3, "Debts", "Διαγραφή (Χρέη)")

            ' Αφαίρεσε το CurrentCell για να μην αλλάξει γραμμή/σώζει κατά λάθος
            Try
                dgvDebtsList.CurrentCell = Nothing
            Catch
            End Try

            ' Ξαναφόρτωσε καθαρά
            _forceLastAfterBind = True
            GetDebtsAndHairDiesList()

            ' Ενημερώσεις οθόνης
            DisplaySums_Debts()
            DisplayTotalDebtPerCustomer()
            DisplayLastUpdate()

            ' Καθάρισε επιλογές για να μην υπάρξει νέο RowLeave αμέσως μετά
            Try
                dgvDebtsList.ClearSelection()
            Catch
            End Try
        Finally
            _suppressDebtsRowLeave = False
            _enterPressedThisEdit = False
            _suppressCellValueChanged = False
        End Try
    End Sub


    Sub FixHeadersLook(dgv As DataGridView)
        dgv.EnableHeadersVisualStyles = False

        Dim ch = dgv.ColumnHeadersDefaultCellStyle
        If ch.BackColor = Color.Empty Then ch.BackColor = SystemColors.Control
        If ch.ForeColor = Color.Empty Then ch.ForeColor = SystemColors.ControlText

        ' Κάνε τα selection colors ίδια με τα normal,
        ' ώστε να μην φαίνεται “επιλεγμένος” ο header.
        ch.SelectionBackColor = ch.BackColor
        ch.SelectionForeColor = ch.ForeColor
    End Sub

    ' Πλήκτρο Delete στο grid
    Private Sub dgvDebtsList_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvDebtsList.KeyDown
        If e.KeyCode = Keys.Delete Then
            DeleteSelectedDebts()
            e.Handled = True
        End If
    End Sub

    Private Sub DeleteSelectedHairdies()
        DeleteSelectedRowsGeneric(dgvHairdiesList, 2, "HairDies", "Διαγραφή (Βαφές)")
        _forceLastAfterBind = True
        GetDebtsAndHairDiesList()
        DisplayLastUpdate()
    End Sub


    Private Sub dgvHairdiesList_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvHairdiesList.KeyDown
        If e.KeyCode = Keys.Delete Then
            DeleteSelectedHairdies()
            e.Handled = True
        End If
    End Sub


    Private Sub DeleteSelectedDrugsOnLoan()
        DeleteSelectedRowsGeneric(dgvDrugsOnLoan, 6, "DrugsOnLoan", "Διαγραφή (Δανεικά φάρμακα)")
        GetDrugsOnLoanList()
        ActivateDatagridDrugsOnLoan(dgvDrugsOnLoan.RowCount > 1)
        DisplaySums_DrugsOnLoan()
        DisplayLastUpdate()
    End Sub



    Private Sub dgvDrugsOnLoan_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvDrugsOnLoan.KeyDown
        If e.KeyCode = Keys.Delete Then
            DeleteSelectedDrugsOnLoan()
            e.Handled = True
        End If
    End Sub

    Private Sub DeleteSelectedPrescriptions()
        DeleteSelectedRowsGeneric(dgvPrescriptions, 6, "Prescriptions", "Διαγραφή (Συνταγές)")
        GetPrescriptionsList()
        DisplayLastUpdate()
    End Sub


    Private Sub dgvPrescriptions_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvPrescriptions.KeyDown
        If e.KeyCode = Keys.Delete Then
            DeleteSelectedPrescriptions()
            e.Handled = True
        End If
    End Sub


    Private Sub DeleteSelectedRowsGeneric(dgv As DataGridView, idColumnIndex As Integer, tableName As String, Optional confirmTitle As String = "Διαγραφή")
        ' Ολοκλήρωσε τυχόν edit για να «γραφτούν» οι τιμές
        dgv.EndEdit()

        ' 1) Μάζεψε τα επιλεγμένα rows ή, αν δεν υπάρχουν, πάρε το CurrentRow (fallback)
        Dim selected As New List(Of DataGridViewRow)
        For Each r As DataGridViewRow In dgv.SelectedRows
            selected.Add(r)
        Next
        If selected.Count = 0 AndAlso dgv.CurrentRow IsNot Nothing AndAlso Not dgv.CurrentRow.IsNewRow Then
            selected.Add(dgv.CurrentRow)
        End If
        If selected.Count = 0 Then Exit Sub

        ' 2) Επιβεβαίωση
        If MessageBox.Show($"Θέλεις σίγουρα να διαγράψεις {selected.Count} εγγραφ{If(selected.Count = 1, "ή", "ές")} ;",
                   confirmTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
            Exit Sub
        End If

        ' 3) Μαζεψε τα Ids (αγνόησε νέες γραμμές χωρίς Id) + σβήσε τοπικά όσες είναι «νέες»
        Dim ids As New List(Of Integer)
        For Each r As DataGridViewRow In selected
            If Not r.IsNewRow Then
                Dim v = r.Cells(idColumnIndex).Value
                Dim id As Integer
                If v IsNot Nothing AndAlso v IsNot DBNull.Value AndAlso Integer.TryParse(v.ToString(), id) Then
                    ids.Add(id)
                End If
            End If
        Next

        ' --- ΜΟΝΑΔΙΚΗ ΔΙΟΡΘΩΣΗ: ΜΗΝ αγγίζεις ποτέ IsNewRow ---
        For Each r As DataGridViewRow In selected
            If r.IsNewRow Then Continue For ' ← αποφεύγει το "Uncommitted new row cannot be deleted"
            Dim v = r.Cells(idColumnIndex).Value
            If v Is Nothing OrElse v Is DBNull.Value Then
                Try : dgv.Rows.Remove(r) : Catch : End Try
            End If
        Next
        ' -----------------------------------------------------

        If ids.Count = 0 Then Exit Sub

        ' 4) Διαγραφή στη ΒΔ
        Using con As New SqlClient.SqlConnection(connectionstring)
            con.Open()
            Dim sql As String = $"DELETE FROM PharmacyCustomFiles.dbo.{tableName} WHERE Id = @Id"
            Using tr = con.BeginTransaction()
                Using cmd As New SqlClient.SqlCommand(sql, con, tr)
                    cmd.Parameters.Add("@Id", SqlDbType.Int)
                    For Each id In ids
                        cmd.Parameters("@Id").Value = id
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
        End Using
    End Sub





    ' --- Όταν ξεκινά το edit, κρατάμε την παλιά τιμή ---
    Private Sub dgvDebtsList_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) _
    Handles dgvDebtsList.CellBeginEdit
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            If dgvDebtsList.Columns(e.ColumnIndex).HeaderText = "Περιγραφή" Then
                _enterPressedThisEdit = False
                _editSw.Reset()
                _editSw.Start()
            End If
        End If
    End Sub


    ' === Unified editing for dgvDebtsList (ποσό & περιγραφή) ===

    ' ΕΝΑ textbox ref για ό,τι κελί επεξεργαζόμαστε τώρα
    Private _editingTB As TextBox = Nothing

    ' Βοηθητικό: αναγνωρίζει τις «χρηματικές» στήλες του dgvDebtsList
    Private Function IsMoneyColumn(col As DataGridViewColumn) As Boolean
        If col Is Nothing Then Return False
        ' Στο debts grid η στήλη ποσού έχει HeaderText "Ποσό"
        ' (Αν στο δικό σου grid έχεις και άλλη "Λιανική", άστην επίσης ως money)
        Return col.HeaderText.Equals("Ποσό", StringComparison.CurrentCultureIgnoreCase) _
        OrElse col.HeaderText.Equals("Λιανική", StringComparison.CurrentCultureIgnoreCase)
    End Function

    ' Unified: ανάλογα με τη στήλη, δένουμε τους σωστούς handlers
    ' Unified: ανάλογα με τη στήλη, δένουμε τους σωστούς handlers
    Private Sub dgvDebtsList_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) _
Handles dgvDebtsList.EditingControlShowing

        If dgvDebtsList.CurrentCell Is Nothing Then Exit Sub

        ' Καθάρισε ΠΑΝΤΑ τυχόν παλιούς handlers από το προηγούμενο control
        If _editingTB IsNot Nothing Then
            RemoveHandler _editingTB.KeyPress, AddressOf MoneyTextBox_KeyPress
            RemoveHandler _editingTB.KeyDown, AddressOf DescTextBox_KeyDown
        End If

        _editingTB = TryCast(e.Control, TextBox)
        If _editingTB Is Nothing Then Exit Sub

        Dim col = dgvDebtsList.Columns(dgvDebtsList.CurrentCell.ColumnIndex)

        ' === ΝΕΟ: κεντράρισμα κατά την πληκτρολόγηση ΜΟΝΟ για τη «Λιανική» ===
        If col IsNot Nothing AndAlso col.HeaderText.Equals("Ποσό", StringComparison.CurrentCultureIgnoreCase) Then
            _editingTB.TextAlign = HorizontalAlignment.Center
        Else
            ' (προαιρετική επαναφορά για τις υπόλοιπες στήλες)
            _editingTB.TextAlign = HorizontalAlignment.Left
        End If
        ' =====================================================================

        If IsMoneyColumn(col) Then
            ' ΠΟΣΟ: επιτρέπουμε -, δεκαδικό, ψηφία
            AddHandler _editingTB.KeyPress, AddressOf MoneyTextBox_KeyPress
        ElseIf col IsNot Nothing AndAlso col.HeaderText.Equals("Περιγραφή", StringComparison.CurrentCultureIgnoreCase) Then
            ' ΠΕΡΙΓΡΑΦΗ: το δικό σου Enter-behavior
            AddHandler _editingTB.KeyDown, AddressOf DescTextBox_KeyDown
        End If
    End Sub


    ' Είσοδος για χρηματικά κελιά: -, δεκαδικό, ψηφία μόνο
    Private Sub MoneyTextBox_KeyPress(sender As Object, e As KeyPressEventArgs)
        Dim tb = DirectCast(sender, TextBox)
        Dim decSep = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator

        ' Επιτρέπουμε control keys / ψηφία
        If Char.IsControl(e.KeyChar) OrElse Char.IsDigit(e.KeyChar) Then Return

        Dim ch = e.KeyChar.ToString()

        ' Δεκαδικό διαχωριστικό: μόνο μία φορά
        If ch = decSep Then
            If tb.Text.Contains(decSep) Then e.Handled = True
            Return
        End If

        ' Μείον: μόνο ως 1ος χαρακτήρας και μόνο μία φορά
        If ch = "-" Then
            Dim hasMinus = tb.Text.StartsWith("-")
            Dim caretAtStart = (tb.SelectionStart = 0)
            If hasMinus OrElse Not caretAtStart Then e.Handled = True
            Return
        End If

        ' Οτιδήποτε άλλο: κόψ’ το
        e.Handled = True
    End Sub

    ' Τελικό validation/normalization για χρηματικές στήλες (χωρίς €)
    Private Sub dgvDebtsList_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) _
    Handles dgvDebtsList.CellValidating

        If _suppressDebtsValidation Then Exit Sub

        ' Αν δεν είμαστε σε πραγματικό edit του τρέχοντος κελιού, μην κάνεις τίποτα
        If dgvDebtsList.CurrentCell Is Nothing _
       OrElse Not dgvDebtsList.IsCurrentCellInEditMode _
       OrElse dgvDebtsList.CurrentRow Is Nothing _
       OrElse dgvDebtsList.CurrentRow.IsNewRow Then Exit Sub

        Dim col = dgvDebtsList.Columns(e.ColumnIndex)
        If Not IsMoneyColumn(col) Then
            If e.ColumnIndex = 0 OrElse e.ColumnIndex = 1 Then
                If Not ValidateDebts_DateAndAmount(e.RowIndex) Then
                    e.Cancel = True
                End If
            End If
            Exit Sub
        End If

        Dim txt = (If(e.FormattedValue, "")).ToString().Trim()
        If txt = "" OrElse txt = "-" Then Return

        Dim value As Decimal
        If Not Decimal.TryParse(txt, Globalization.NumberStyles.Number Or Globalization.NumberStyles.AllowLeadingSign,
                            Globalization.CultureInfo.CurrentCulture, value) Then
            MessageBox.Show("Δώσε έγκυρη αριθμητική τιμή (π.χ. -12,34).", "Έλεγχος ποσού", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            e.Cancel = True
            Return
        End If

        ' Όριο ποσού
        If Math.Abs(value) > 10000D Then
            MessageBox.Show("Το ποσό δεν μπορεί να υπερβαίνει τα 10.000,00 €.",
                    "Σφάλμα ποσού", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            e.Cancel = True
            Return
        End If


        ' Κανονικοποίηση εμφάνισης
        dgvDebtsList.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = value
        dgvDebtsList.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.Format = "0.00"
        dgvDebtsList.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    End Sub



    ' Σίγαση format exceptions (π.χ. όταν ο χρήστης έχει πληκτρολογήσει μόνο "-")
    Private Sub dgvDebtsList_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) _
    Handles dgvDebtsList.DataError
        e.ThrowException = False
    End Sub


    Private Sub dgvDebtsList_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) _
    Handles dgvDebtsList.CellEndEdit
        lblScanHint.Visible = False
        _editSw.Stop()
    End Sub


    Private Sub DescTextBox_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            _enterPressedThisEdit = True
            lblScanHint.Visible = False
            ' Προαιρετικά: μην κάνει “ding”/άλλα navigation. Θα γίνει commit από τον δικό σου κώδικα.
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub


    Private Sub SetPCTerminal()

        txtDB1.Text = strDB1
        txtDB2.Text = strDB2
        txtSourceDB.Text = strDBFolder

        Me.Text = "Διαχείρηση Φαρμακείου  " & Version

        If My.Computer.Name = "CRAZYDR" Then
            rbWhereSpiti.Checked = True
            txtSourceFarmnetDB.Text = strCSAfolder_Home
            FarNetFolder = strCSAfolder_Home
            Pharmacy2013Folder = strDBFolder_Home
            PharmakonServer = PharmaconServer_Home
            Pharmacy2013Server = Pharmacy2013Server_Home
        ElseIf My.Computer.Name = "DESKTOP-T7HMABG" Then
            rbWhereFarm1.Checked = True
            FarNetFolder = strCSAfolder_Farm
            Pharmacy2013Folder = strDBFolder_Farm
            txtSourceFarmnetDB.Text = strCSAfolder_Farm
            PharmakonServer = PharmaconServer_Farm
            Pharmacy2013Server = Pharmacy2013Server_Farm
        ElseIf My.Computer.Name = "DESKTOP-B3F3GNN" Then
            rbWhereFarm2.Checked = True
        ElseIf My.Computer.Name = "LAPTOP-4AJPEK4U" Then
            rbWhereLaptop.Checked = True
        ElseIf My.Computer.Name = "NIKOYLA-PC" Then
            rbWhereNikoyla.Checked = True
        ElseIf My.Computer.Name = "SALONI-PC" Then
            rbWhereSaloni.Checked = True
        End If

        lblPCName.Text = My.Computer.Name
    End Sub


    'Private Sub DisplayDataOnComboboxes(ByVal oComboBox As String)
    '    Select Case oComboBox
    '        Case "cboExchangeFromDate"
    '            FillComboBox(CType(cboExchangeFromDate, ComboBox), GetDistinctContentsDBField( _
    '                                        "SELECT DISTINCT format([DeliveryDate],'dddd, dd-MM-yyy', 'el-gr') as MyDate, DeliveryDate " & _
    '                                        "FROM ExpirationsNew " & _
    '                                        "WHERE FromWho = '" & dgvExchangesTotal.Rows(dgvExchangesTotal.SelectedRows(0).Index).Cells(0).Value.ToString & "' AND DeliveryDate is not null " & _
    '                                        "ORDER BY DeliveryDate", "MyDate"), {"Όλες"})


    '    End Select


    'End Sub





    ' ****************************************************************************************************************************
    ' **********    ΔΙΑΧΕΙΡΗΣΗ ΠΕΛΑΤΟΛΟΓΙΟΥ    ***********************************************************************************
    ' ****************************************************************************************************************************





    'Private Sub GetCustomers()
    '    'Μεταβλητες
    '    Dim sqlString As String = "SELECT Name, Id From PharmacyCustomFiles.dbo.Customers " & _
    '                              "WHERE Customers.Name like '%" & txtSearchCustomers.Text.ToString & "%' " & _
    '                              "ORDER BY Name"

    '    ' Με βάση το μέρος ονόματος του πελάτη (textbox) και τις επιλογές μας στα RadioButton,
    '    ' βρίσκει όλους τους πελάτες..
    '    If rbAll.Checked = True Then

    '        sqlString = "SELECT * From PharmacyCustomFiles.dbo.Customers " & _
    '                    "WHERE Customers.Name like '%" & txtSearchCustomers.Text.ToString & "%'  " & _
    '                    "ORDER BY Name"

    '        ' ή μονο τους πελάτες με χρέη..
    '    ElseIf rbDebts.Checked = True Then

    '        sqlString = "SELECT Customers.name, Customers.id, sum(Debts.Ammount) as SumDebts  " & _
    '                    "FROM Customers INNER JOIN " & _
    '                        "Debts ON Customers.Id = Debts.CustomerId " & _
    '                    "WHERE Customers.Name like '%" & txtSearchCustomers.Text.ToString & "%' " & _
    '                    "GROUP BY  customers.name, customers.id " & _
    '                    "HAVING sum(Debts.Ammount) > 0"

    '        ' ή μονο τους πελάτες με βαφές..
    '    ElseIf rbHairDies.Checked = True Then

    '        sqlString = "SELECT  distinct Customers.Name, Customers.id " & _
    '                    "FROM Customers INNER JOIN HairDies ON Customers.Id = HairDies.CustomerId " & _
    '                    "WHERE Customers.Name like '%" & txtSearchCustomers.Text.ToString & "%' AND " & _
    '                        "HairDies.HairDieDescription Is Not null " & _
    '                    "ORDER BY Name"
    '    End If

    '    ' Γεμίζει το lstCustomers με τους πελάτες που αντιστοιχούν στις επιλογές μας και 
    '    ' κρατάει στην μεταβλητή sumCustomers τον συνολικό αριθμό πελατών
    '    Dim sumCustomers As Integer = FillListBox(sqlString, lstCustomers, "Name")

    '    ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Select Case sumCustomers
    '        Case 0
    '            rtxtMessage.Text = "Δεν βρέθηκαν πελάτες"

    '            ' Αν δεν υπάρχουν πελάτες δεν μπορείς να καταχωρήσεις Χρέη & Βαφές
    '            btnEdit.Enabled = False
    '            btnEditHairDie.Enabled = False

    '        Case 1
    '            rtxtMessage.Text = "Βρέθηκε 1 πελάτης"

    '            ' Επανεργοποίηση πλήκτρων για να καταχωρήσεις Χρέη & Βαφές
    '            btnEdit.Enabled = True
    '            btnEditHairDie.Enabled = True

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtMessage, {"1"})

    '        Case Is > 1
    '            rtxtMessage.Text = "Βρέθηκαν " & sumCustomers.ToString & " πελάτες"

    '            ' Επανεργοποίηση πλήκτρων για να καταχωρήσεις Χρέη & Βαφές
    '            btnEdit.Enabled = True
    '            btnEditHairDie.Enabled = True

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtMessage, {sumCustomers.ToString})

    '    End Select

    '    ' και ανάλογα με το αν έχουμε επιλέξει να βρίσκει τα Χρέη ή τις βαφές
    '    If rbDebts.Checked = True Then
    '        'Υπολογίζει το συνολικό χρέος
    '        sqlString = "SELECT * " & _
    '                 "FROM Customers INNER JOIN Debts ON Customers.Id = Debts.CustomerId " & _
    '                 "WHERE Customers.Name like '%" & txtSearchCustomers.Text.ToString & "%' "
    '        Dim TotalDebt As String = String.Format("{0:#,##0.00 €}", CalculateSums(sqlString, "Ammount"))
    '        ' και το συμπληρώνει στο RichTextBox
    '        rtxtMessage.Text &= " με χρέη " & vbCrLf & "που ανέρχονται στα " & TotalDebt

    '        If sumCustomers > 0 Then

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών και το συνολικό χρέος
    '            HightlightInRichTextBox(rtxtMessage, {sumCustomers.ToString, TotalDebt})

    '        End If

    '    ElseIf rbHairDies.Checked = True Then
    '        rtxtMessage.Text &= " με βαφές"

    '        If sumCustomers > 0 Then
    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtMessage, {sumCustomers.ToString})
    '        End If

    '    End If

    'End Sub



    'Private Sub txtSearchCustomers_TextChanged(sender As Object, e As EventArgs)

    '    ' Βρίσκει τους πελάτες που αντιστοιχούν στις επιλογές μας και τους γράφει στο ListBox
    '    GetCustomers()

    'End Sub



    'Private Sub DisplayDebtsAndHairDies()

    '    ' Βρίσκει το Id της επιλογής μας από το lstCustomers
    '    Dim lstCustomersIndex As String = ""
    '    Try
    '        lstCustomersIndex = lstCustomers.SelectedValue.ToString
    '    Catch ex As Exception
    '    End Try


    '    ' Εμφανίζει το όνομα του πελάτη στο μεγάλο κεντρικό Label
    '    lblSelectedCustomerName.Text = lstCustomers.Text


    '    'Ελέγχει ότι το ListBox με τους πελάτες έχει ήδη κάποιο πελάτη επιλεγμένο και ...
    '    If lstCustomersIndex <> "System.Data.DataRowView" Then

    '        ' Γεμίζει το DatagridView dgvDebts με τα χρωστούμενα του επιλεγμένου πελάτη
    '        stringDTG = "Select Date, Ammount, DebtDescription, Id, CustomerId From PharmacyCustomFiles.dbo.Debts WHERE CustomerId = '" & lstCustomers.SelectedValue.ToString & "' ORDER BY Date DESC"
    '        FillDatagrid(dgvDebts, bsDebts, {"Ημερομηνία", "Ποσό", "Περιγραφή"}, {80, 60, 220}, {"dd-MM-yyyy", "c", "0"}, {"Id", "CustomerId"})

    '        ' Αναγράφει το συνολικό χρέος στον τίτλο του GroupBox
    '        grpDebts.Text = "Σύνολο συναλλαγών: " & String.Format("{0:#,##0.00 €}", CalculateSums(stringDTG, "Ammount"))

    '        ' Γεμίζει το dgvHairDies με τις εγγραφές που αντιστοιχούν στις βαφές του επιλεγμένου πελάτη
    '        stringDTG = "Select Date, HairDieDescription, Id, CustomerId From PharmacyCustomFiles.dbo.HairDies WHERE CustomerId = '" & lstCustomersIndex & "' ORDER BY Id DESC"
    '        FillDatagrid(dgvHairDies, bsHairDies, {"Ημερομηνία", "Κωδικός"}, {80, 150}, {"dd-MM-yyyy", "0"}, {"Id", "CustomerId"})

    '        ' Alignment των στοιχείων των Column
    '        dgvDebts.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        dgvDebts.Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

    '        ' Alignment των HeaderText των Column
    '        dgvDebts.Columns(0).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        dgvDebts.Columns(1).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter


    '    End If
    'End Sub

    Public Sub GetDrugsOnLoanList()

        ' Βρίσκει το Id της επιλογής μας από το lstCustomers
        Dim lstCustomersIndex As Integer = 0
        Try
            lstCustomersIndex = dgvCustomers.SelectedRows(0).Cells(1).Value

        Catch ex As Exception
            lstCustomersIndex = 0
            Exit Sub
        End Try

        ' Γεμίζει το DatagridView dgvDebts με τα χρωστούμενα του επιλεγμένου πελάτη
        stringDTG = "Select Name,Price, DateIn,Barcode1, Barcode2, DateOut, CustomerId, Id From PharmacyCustomFiles.dbo.DrugsOnLoan " &
                    "WHERE CustomerId = '" & lstCustomersIndex.ToString & "' AND DateOut is null " &
                    "ORDER BY DateIn, Id"
        stringDTG_DrugsOnLoan = stringDTG
        DisplayCustomDatagrid_DrugsOnLoan(bsDrugsOnLoan, dgvDrugsOnLoan)

        ' Αναγράφει το συνολικό χρέος στον τίτλο του GroupBox
        Try
            DisplaySums_DrugsOnLoan()
        Catch ex As Exception
        End Try


        'Πάει στο τελευταίο record του datagrid
        Try
            Me.dgvDrugsOnLoan.FirstDisplayedScrollingRowIndex = Me.dgvDrugsOnLoan.RowCount - 1
        Catch ex As Exception
        End Try

        Try
            ' Alignment των στοιχείων των Column
            dgvDrugsOnLoan.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            dgvDrugsOnLoan.Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            ' Alignment των HeaderText των Column
            dgvDrugsOnLoan.Columns(0).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            dgvDrugsOnLoan.Columns(1).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
        Catch ex As Exception
        End Try

    End Sub


    Public Shared Sub GetPrescriptionsList()

        Dim totPrescriptions As Integer = 0

        ' Βρίσκει το Id της επιλογής μας από το lstCustomers
        Dim lstCustomersIndex As Integer = 0

        Try
            lstCustomersIndex = frmCustomers.dgvCustomers.SelectedRows(0).Cells(1).Value

        Catch ex As Exception
            lstCustomersIndex = 0
            Exit Sub
        End Try

        ' Γεμίζει το DatagridView dgvDebts με τα χρωστούμενα του επιλεγμένου πελάτη
        If frmCustomers.chkSelectAll.Checked = False Then
            stringDTG = "SELECT [Ektelesis],[InitDate],[EndDate],[Barcode], [ProcessedDate],[Drug1], [Drug2], [Drug3],[Drug4], [Drug5], [Drug6],[Drug7], [Drug8], [Drug9], [Drug10], " &
                            "[Drug11], [Drug12], [Drug13],[Drug14], [Drug15], [Drug16],[Drug17], [Drug18], [Drug19], [Drug20], [CustomerId], [Id], [Analosima], [Notes] " &
                        "FROM [PharmacyCustomFiles].[dbo].[Prescriptions] " &
                        "WHERE CustomerId = '" & lstCustomersIndex.ToString & "' AND ProcessedDate IS NULL " &
                        "ORDER BY InitDate, Ektelesis, Id"
        Else
            stringDTG = "SELECT [Ektelesis],[InitDate],[EndDate],[Barcode], [ProcessedDate],[Drug1], [Drug2], [Drug3],[Drug4], [Drug5], [Drug6],[Drug7], [Drug8], [Drug9], [Drug10], " &
                            "[Drug11], [Drug12], [Drug13],[Drug14], [Drug15], [Drug16],[Drug17], [Drug18], [Drug19], [Drug20], [CustomerId], [Id], [Analosima], [Notes] " &
                      "FROM [PharmacyCustomFiles].[dbo].[Prescriptions] " &
                      "WHERE CustomerId = '" & lstCustomersIndex.ToString & "' " &
                      "ORDER BY InitDate, Ektelesis, Id"
        End If

        'stringDTG_DrugsOnLoan = stringDTG
        totPrescriptions = frmCustomers.DisplayCustomDatagrid_Prescriptions(bsPrescriptions, frmCustomers.dgvPrescriptions)

        Try
            frmCustomers.grpPrescriptions.Text = "#" & frmCustomers.dgvCustomers.SelectedRows(0).Cells(1).Value.ToString & "   " & frmCustomers.dgvCustomers.SelectedRows(0).Cells(0).Value.ToString

            If totPrescriptions = 0 Then
                frmCustomers.lblTotPrescriptions.Text = "Καμιά συνταγή"
            ElseIf totPrescriptions = 1 Then
                frmCustomers.lblTotPrescriptions.Text = "1 συνταγή"
            ElseIf totPrescriptions > 1 Then
                frmCustomers.lblTotPrescriptions.Text = totPrescriptions & " συνταγές"
            End If

        Catch ex As Exception
            frmCustomers.grpPrescriptions.Text = ""
        End Try

        'Πάει στο τελευταίο record του datagrid
        'Try
        '    frmCustomers.dgvPrescriptions.FirstDisplayedScrollingRowIndex = frmCustomers.dgvPrescriptions.RowCount - 1
        'Catch ex As Exception
        'End Try

        Try
            ' Alignment των στοιχείων των Column
            frmCustomers.dgvPrescriptions.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            frmCustomers.dgvPrescriptions.Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            frmCustomers.dgvPrescriptions.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            ' Alignment των HeaderText των Column
            frmCustomers.dgvPrescriptions.Columns(0).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            frmCustomers.dgvPrescriptions.Columns(1).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            frmCustomers.dgvPrescriptions.Columns(2).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
        Catch ex As Exception
        End Try

    End Sub


    ' Αντικατάστησε ΟΛΗ τη ρουτίνα με αυτή την έκδοση (ελάχιστες αλλαγές για ασφαλές rebind)
    Private Sub GetDebtsAndHairDiesList()

        ' 1) Πάρε το Id πελάτη με ασφάλεια
        Dim custId As Integer
        Try
            custId = CInt(dgvCustomers.SelectedRows(0).Cells(1).Value)
        Catch
            Exit Sub
        End Try

        ' 2) Φέρε Debts + HairDies σε ένα DataSet (ίδιο όπως πριν)
        Dim ds As New DataSet()
        Try
            Using con As New SqlClient.SqlConnection(connectionstring)
                Using cmd As New SqlClient.SqlCommand("
                SELECT [Date], [Ammount], [DebtDescription], [Id], [CustomerId]
                FROM PharmacyCustomFiles.dbo.Debts
                WHERE CustomerId = @cid
                ORDER BY [Date], [Id];

                SELECT [Date], [HairDieDescription], [Id], [CustomerId]
                FROM PharmacyCustomFiles.dbo.HairDies
                WHERE CustomerId = @cid
                ORDER BY [Date], [Id];", con)

                    cmd.Parameters.Add("@cid", SqlDbType.Int).Value = custId
                    Using da As New SqlClient.SqlDataAdapter(cmd)
                        con.Open()
                        da.Fill(ds)
                    End Using
                End Using
            End Using
        Catch
            ' Αν κάτι πάει στραβά, μην κρασάρει το UI
            Return
        End Try

        Dim dtDebts As DataTable = If(ds.Tables.Count > 0, ds.Tables(0), New DataTable())
        Dim dtHair As DataTable = If(ds.Tables.Count > 1, ds.Tables(1), New DataTable())

        ' 3) Ασφαλές rebind: σίγαση validation + τερματισμός edits ΠΡΙΝ αλλάξουμε DataSource
        Dim prevCV_Debts As Boolean = dgvDebtsList.CausesValidation
        Dim prevCV_Hair As Boolean = dgvHairdiesList.CausesValidation

        _suppressDebtsValidation = True   ' <-- χρησιμοποιείται ήδη στο CellValidating
        dgvDebtsList.CausesValidation = False
        dgvHairdiesList.CausesValidation = False

        dgvDebtsList.SuspendLayout()
        dgvHairdiesList.SuspendLayout()

        Try
            ' --- Κλείσε τυχόν edits / validation re-entrancy ---
            Try : dgvDebtsList.EndEdit(DataGridViewDataErrorContexts.Commit) : Catch : End Try
            Try : dgvDebtsList.CommitEdit(DataGridViewDataErrorContexts.Commit) : Catch : End Try
            Try
                If bsDebts IsNot Nothing Then
                    bsDebts.EndEdit()
                End If
            Catch
            End Try
            ' CurrencyManager (προαιρετικό)
            Try
                If dgvDebtsList.DataSource IsNot Nothing Then
                    Dim cm = TryCast(Me.BindingContext(dgvDebtsList.DataSource), CurrencyManager)
                    cm?.EndCurrentEdit()
                End If
            Catch
            End Try
            ' Καθάρισε current cell πριν το rebind για να μην γίνει set_CurrentCell εν μέσω commit
            Try : dgvDebtsList.CurrentCell = Nothing : Catch : End Try

            Try : dgvHairdiesList.EndEdit(DataGridViewDataErrorContexts.Commit) : Catch : End Try
            Try : dgvHairdiesList.CommitEdit(DataGridViewDataErrorContexts.Commit) : Catch : End Try
            Try : dgvHairdiesList.CurrentCell = Nothing : Catch : End Try
            ' ----------------------------------------------------

            ' --- Debts grid (ίδιο setup όπως πριν) ---
            dgvDebtsList.AutoGenerateColumns = False
            If dgvDebtsList.Columns.Count = 0 Then
                Dim cDate2 As New DataGridViewTextBoxColumn With {
                .DataPropertyName = "Date",
                .HeaderText = "Ημερομηνία",
                .Width = 80,
                .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "dd-MM-yyyy", .Alignment = DataGridViewContentAlignment.MiddleCenter}
            }
                Dim cAmt As New DataGridViewTextBoxColumn With {
                .DataPropertyName = "Ammount",
                .HeaderText = "Ποσό",
                .Width = 80,
                .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "N2", .Alignment = DataGridViewContentAlignment.MiddleCenter}
            }
                Dim cDesc As New DataGridViewTextBoxColumn With {
                .DataPropertyName = "DebtDescription",
                .HeaderText = "Περιγραφή",
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            }
                Dim cId As New DataGridViewTextBoxColumn With {.DataPropertyName = "Id", .Visible = False}
                Dim cCust As New DataGridViewTextBoxColumn With {.DataPropertyName = "CustomerId", .Visible = False}
                dgvDebtsList.Columns.AddRange({cDate2, cAmt, cDesc, cId, cCust})

                dgvDebtsList.Columns(0).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                dgvDebtsList.Columns(1).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            End If

            ' ΚΑΘΑΡΙΣΕ πρώτα, ΜΕΤΑ δώσε το νέο DataSource (αποφεύγει το exception)
            dgvDebtsList.DataSource = Nothing
            dgvDebtsList.DataSource = dtDebts   ' <— γραμμή ~1042 που έσκαγε

            ' --- Hair dyes grid (ίδιο setup όπως πριν) ---
            dgvHairdiesList.AutoGenerateColumns = False
            If dgvHairdiesList.Columns.Count = 0 Then
                Dim cDate2 As New DataGridViewTextBoxColumn With {
                .DataPropertyName = "Date",
                .HeaderText = "Ημερομηνία",
                .Width = 80,
                .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "dd-MM-yyyy", .Alignment = DataGridViewContentAlignment.MiddleCenter}
            }
                Dim cCode As New DataGridViewTextBoxColumn With {
                .DataPropertyName = "HairDieDescription",
                .HeaderText = "Κωδικός βαφής",
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            }
                Dim cId2 As New DataGridViewTextBoxColumn With {.DataPropertyName = "Id", .Visible = False}
                Dim cCust2 As New DataGridViewTextBoxColumn With {.DataPropertyName = "CustomerId", .Visible = False}
                dgvHairdiesList.Columns.AddRange({cDate2, cCode, cId2, cCust2})

                dgvHairdiesList.Columns(0).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            End If

            dgvHairdiesList.DataSource = Nothing
            dgvHairdiesList.DataSource = dtHair

        Finally
            dgvDebtsList.ResumeLayout()
            dgvHairdiesList.ResumeLayout()

            ' Επαναφορά flags
            dgvDebtsList.CausesValidation = prevCV_Debts
            dgvHairdiesList.CausesValidation = prevCV_Hair
            _suppressDebtsValidation = False
        End Try

        ' 4) Συμβατότητα με stringDTG / stringDTG_Debts (όπως πριν)
        stringDTG = "Select Date, Ammount, DebtDescription, Id, CustomerId From PharmacyCustomFiles.dbo.Debts WHERE CustomerId = '" & custId.ToString & "' ORDER BY Date, Id"
        stringDTG_Debts = stringDTG
        stringDTG = "Select Date, HairDieDescription, Id, CustomerId From PharmacyCustomFiles.dbo.HairDies WHERE CustomerId = '" & custId.ToString & "' ORDER BY Date, Id"

        ' 5) Υπολόγισε σύνολο (ίδιο)
        Dim total As Decimal = 0D
        For Each r As DataRow In dtDebts.Rows
            If r.RowState <> DataRowState.Deleted AndAlso Not IsDBNull(r("Ammount")) Then
                total += CDec(r("Ammount"))
            End If
        Next
        lblTotalCustomerDebt.Text = total.ToString("c")
        DisplayTotalDebtPerCustomer()

        ' 6) Scroll/select στο τέλος αν ζητήθηκε (ίδιο)

        If _forceLastAfterBind AndAlso dgvDebtsList.Rows.Cast(Of DataGridViewRow)().Any(Function(r) Not r.IsNewRow) Then
            _forceLastAfterBind = False
            Me.BeginInvoke(Sub() SafeScrollToLastRow(dgvDebtsList))
        End If
    End Sub




    Private Sub GetDebtsAndHairDiesList_old()

        ' Βρίσκει το Id της επιλογής μας από το lstCustomers
        Dim lstCustomersIndex As Integer = 0
        Try
            lstCustomersIndex = dgvCustomers.SelectedRows(0).Cells(1).Value

        Catch ex As Exception
            lstCustomersIndex = 0
            Exit Sub
        End Try

        ' Γεμίζει το DatagridView dgvDebts με τα χρωστούμενα του επιλεγμένου πελάτη
        stringDTG = "Select Date, Ammount, DebtDescription, Id, CustomerId From PharmacyCustomFiles.dbo.Debts WHERE CustomerId = '" & lstCustomersIndex.ToString & "' ORDER BY Date, Id"
        stringDTG_Debts = stringDTG
        DisplayCustomDatagrid_Debts(bsDebts, dgvDebtsList)

        ' Αναγράφει το συνολικό χρέος στον τίτλο του GroupBox
        DisplaySums_Debts()

        'Πάει στο τελευταίο record του datagrid
        Try
            Me.dgvDebtsList.FirstDisplayedScrollingRowIndex = Me.dgvDebtsList.RowCount - 1
        Catch ex As Exception
        End Try

        ' Γεμίζει το dgvHairDies με τις εγγραφές που αντιστοιχούν στις βαφές του επιλεγμένου πελάτη
        stringDTG = "Select Date, HairDieDescription, Id, CustomerId From PharmacyCustomFiles.dbo.HairDies WHERE CustomerId = '" & lstCustomersIndex.ToString & "' ORDER BY Date, Id"
        FillDatagrid(dgvHairdiesList, bsHairDies, {"Ημερομηνία", "Κωδικός βαφής"}, {80, 130}, {"dd-MM-yyyy", "0"}, {"Id", "CustomerId"})

        Try
            ' Alignment των στοιχείων των Column
            dgvDebtsList.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            dgvDebtsList.Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            ' Alignment των HeaderText των Column
            dgvDebtsList.Columns(0).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            dgvDebtsList.Columns(1).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
        Catch ex As Exception
        End Try

        'Πάει στο τελευταίο record του datagrid
        Try
            Me.dgvHairdiesList.FirstDisplayedScrollingRowIndex = Me.dgvHairdiesList.RowCount - 1
        Catch ex As Exception
        End Try

        Try
            grpCustHairDies.Text = "#" & dgvCustomers.SelectedRows(0).Cells(1).Value.ToString & "  " & dgvCustomers.SelectedRows(0).Cells(0).Value.ToString
        Catch ex As Exception
            grpCustHairDies.Text = ""
        End Try

    End Sub

    Private Sub DisplaySums_Debts()
        ' Αναγράφει το συνολικό χρέος στον τίτλο του GroupBox
        lblTotalCustomerDebt.Text = String.Format("{0:#,##0.00 €}", CalculateSums(stringDTG_Debts, "Ammount"))
        If CalculateSums(stringDTG_Debts, "Ammount") > 0 Then
            lblTotalDebtLabel.Text = "ΧΡΕH απο "
        Else
            lblTotalDebtLabel.Text = "ΟΦΕΙΛΕΣ απο "
        End If

        Dim tot As Integer = dgvDebtsList.Rows.Count - 1

        Select Case tot
            Case 1
                lblTotalDebtLabel.Text &= tot & " συναλλαγή: "
            Case Is > 1
                lblTotalDebtLabel.Text &= tot & " συναλλαγές: "
        End Select

        Try
            grpCustDebts.Text = "#" & dgvCustomers.SelectedRows(0).Cells(1).Value.ToString & "  " & dgvCustomers.SelectedRows(0).Cells(0).Value.ToString
        Catch ex As Exception
            grpCustDebts.Text = ""
        End Try
    End Sub


    Private Sub DisplaySums_DrugsOnLoan()
        ' Αναγράφει το συνολικό χρέος στον τίτλο του GroupBox
        lblSumDrugsOnLoan.Text = String.Format("{0:#,##0.00 €}", CalculateSums(stringDTG_DrugsOnLoan, "Price"))
        lblSumDrugsOnLoanLabel.Text = "ΧΡΕΗ από "
        Dim tot As Integer = CalculateTotCount(stringDTG_DrugsOnLoan)
        Select Case tot
            Case 1
                lblSumDrugsOnLoanLabel.Text &= tot & " φάρμακo:"
                'lblTotDrugsOnLoan.Text = CalculateTotCount(stringDTG_DrugsOnLoan, "Price") & " φάρμακo"
            Case Is > 1
                lblSumDrugsOnLoanLabel.Text &= tot & " φάρμακα:"
                'lblTotDrugsOnLoan.Text = CalculateTotCount(stringDTG_DrugsOnLoan, "Price") & " φάρμακα"

        End Select

        Try
            grpDrugsOnLoan.Text = "#" & dgvCustomers.SelectedRows(0).Cells(1).Value.ToString & "  " & dgvCustomers.SelectedRows(0).Cells(0).Value.ToString
        Catch ex As Exception
            grpDrugsOnLoan.Text = ""
        End Try
    End Sub


    'Private Sub lstCustomers_SelectedIndexChanged(sender As Object, e As EventArgs)

    '    ' Εμφανίζει τα χρέη και τις βαφές στα αντίστοιχα datagrid
    '    DisplayDebtsAndHairDies()

    'End Sub












    Private Sub btnCustomersEdit_Click(sender As Object, e As EventArgs)

        ' ΠΡΩΤΑ απενεργοποιεί τo TabControl
        tbcMain.Enabled = False
        ' και εξαφανίζει τo Minimize button
        Me.MinimizeBox = False

        'ΜΕΤΑ εμφανίζει την frm CustomersEdit
        frmCustomersEdit.Show()
        ' και τη φέρνει μπροστά
        frmCustomersEdit.BringToFront()

    End Sub



    'Private Sub rbDebts_CheckedChanged(sender As Object, e As EventArgs)

    '    ' Βρίσκει τους πελάτες που αντιστοιχούν στις επιλογές μας και τους γράφει στο ListBox
    '    GetCustomers()

    'End Sub



    'Private Sub rbHairDies_CheckedChanged(sender As Object, e As EventArgs)

    '    ' Βρίσκει τους πελάτες που αντιστοιχούν στις επιλογές μας και τους γράφει στο ListBox
    '    GetCustomers()

    'End Sub



    ' ****************************************************************************************************************************
    ' **********    ΔΙΑΧΕΙΡΗΣΗ ΧΡΕΩΝ    ******************************************************************************************
    ' ****************************************************************************************************************************


    'Private Sub btnEdit_Click(sender As Object, e As EventArgs)
    '    ' Καθορίζει το ενημερωτικό flashing label
    '    timerLabel = lblDebtMessage

    '    'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
    '    If btnEdit.Text = "Edit" Then

    '        ChangeControlsDebts(True)

    '        'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
    '    ElseIf btnEdit.Text = "Cancel" Then

    '        ChangeControlsDebts(False)

    '    End If

    'End Sub



    'Private Sub ChangeControlsDebts(ByVal selector As Boolean)
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save

    '    ' Γεμίζει το DatagridView dgvDebts με τα χρωστούμενα του επιλεγμένου πελάτη
    '    stringDTG = "Select Date, Ammount, DebtDescription, Id, CustomerId From PharmacyCustomFiles.dbo.Debts WHERE CustomerId = '" & lstCustomers.SelectedValue.ToString & "' ORDER BY Date DESC"
    '    FillDatagrid(dgvDebts, bsDebts, {"Ημερομηνία", "Ποσό", "Περιγραφή"}, {80, 60, 220}, {"dd-MM-yyyy", "c", "0"}, {"Id", "CustomerId"})

    '    'Τροποποίηση των άλλων GroupBox
    '    grpCustomers.Enabled = Not selector
    '    grpHairDies.Enabled = Not selector

    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnSave, btnEdit, btnDelete}, dgvDebts, selector)

    '    ' Ενεργοποίηση ενημερωτικού flashing label
    '    timerLabel.Visible = selector
    '    tmrFlashLabel.Enabled = selector
    'End Sub



    'Private Sub btnSave_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
    '    ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    UpdateDatagrid({btnSave, btnEdit, btnDelete}, dgvDebts, lstCustomers.SelectedValue, "CustomerId")

    '    ChangeControlsDebts(False)

    '    ' Αναγράφει το συνολικό χρέος στον τίτλο του GroupBox
    '    grpDebts.Text = "Σύνολο συναλλαγών: " & String.Format("{0:#,##0.00 €}", CalculateSums(stringDTG, "Ammount"))

    'End Sub



    'Private Sub btnDelete_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Delete των δεδομένων του DataGrid 
    '    ' μαζί με τροποποίηση των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    DeleteDatagrid(dgvDebts)

    '    ChangeControlsDebts(False)

    'End Sub





    'Private Sub dgvDebts_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDebts.CellContentClick

    '    ' Δοκιμές !!!
    '    txtId.Text = dgvDebts.CurrentCell.Value.ToString
    '    txtId2.Text = dgvDebts.Item("Id", dgvDebts.CurrentRow.Index).Value.ToString

    'End Sub



    'Private Sub dgvDebts_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvDebts.CellMouseClick
    '    'lblDebtSum.Text = dgvDebts.SelectedRows(0).Index.ToString
    '    ' Δοκιμές !!!
    '    txtId.Text = dgvDebts.CurrentCell.Value.ToString
    '    txtId2.Text = dgvDebts.Item("Id", dgvDebts.CurrentRow.Index).Value.ToString
    'End Sub




    'Private Sub dgvDebts_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs)
    '    Dim headerText As String = dgvDebts.Columns(e.ColumnIndex).HeaderText

    '    ' Ελέγχει αν βρισκόμαστε στο πεδίο Ημερομηνία
    '    If headerText.Equals("Ημερομηνία") Then
    '        Dim dt As DateTime

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι ημερομηνία
    '        If e.FormattedValue.ToString <> String.Empty AndAlso Not DateTime.TryParse(e.FormattedValue.ToString, dt) AndAlso btnEdit.Text = "Cancel" Then
    '            MessageBox.Show("Λάθος καταχώρηση ημερομηνίας", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True

    '        End If
    '        ' Ελέγχει αν βρισκόμαστε σε χρηματικό πεδίο
    '    ElseIf headerText.Equals("Ποσό") Then
    '        Dim dc As Decimal

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι δεκαδική
    '        If e.FormattedValue.ToString <> String.Empty AndAlso Decimal.TryParse(e.FormattedValue.ToString, dc) = False AndAlso btnEdit.Text = "Cancel" Then
    '            MessageBox.Show("Δεν καταχωρήσατε σωστή χρηματική τιμή", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True

    '        End If

    '    End If

    'End Sub



    'Private Sub dgvDebts_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
    '    Try
    '        ' Clear the row error in case the user presses ESC.   
    '        dgvDebts.Rows(e.RowIndex).ErrorText = String.Empty
    '    Catch ex As Exception
    '    End Try

    'End Sub


    '' ****************************************************************************************************************************
    '' **********    ΔΙΑΧΕΙΡΗΣΗ ΒΑΦΩΝ    ******************************************************************************************
    '' ****************************************************************************************************************************



    'Private Sub btnEditHairDie_Click(sender As Object, e As EventArgs)
    '    ' Καθορίζει το ενημερωτικό flashing label
    '    timerLabel = lblHairDieMessage

    '    'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
    '    If btnEditHairDie.Text = "Edit" Then

    '        ChangeControlsHairDies(True)

    '        'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
    '    ElseIf btnEditHairDie.Text = "Cancel" Then

    '        ChangeControlsHairDies(False)

    '    End If
    'End Sub



    'Private Sub ChangeControlsHairDies(ByVal selector As Boolean)
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save

    '    ' Γεμίζει το dgvHairDies με τις εγγραφές που αντιστοιχούν στις βαφές του επιλεγμένου πελάτη
    '    stringDTG = "Select Date, HairDieDescription, Id, CustomerId From PharmacyCustomFiles.dbo.HairDies WHERE CustomerId = '" & lstCustomers.SelectedValue.ToString & "' ORDER BY Id DESC"
    '    FillDatagrid(dgvHairDies, bsHairDies, {"Ημερομηνία", "Κωδικός"}, {80, 150}, {"dd-MM-yyyy", "0"}, {"Id", "CustomerId"})

    '    'Τροποποίηση των άλλων GroupBox
    '    grpCustomers.Enabled = Not selector
    '    grpDebts.Enabled = Not selector

    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnSaveHairDie, btnEditHairDie, btnDeleteHairDie}, dgvHairDies, selector)

    '    ' Ενεργοποίηση ενημερωτικού flashing label
    '    timerLabel.Visible = selector
    '    tmrFlashLabel.Enabled = selector
    'End Sub


    'Private Sub btnSaveHairDie_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
    '    ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    UpdateDatagrid({btnSaveHairDie, btnEditHairDie, btnDeleteHairDie}, dgvHairDies, lstCustomers.SelectedValue, "CustomerId")

    '    ChangeControlsHairDies(False)
    'End Sub



    'Private Sub btnDeleteHairDie_Click(sender As Object, e As EventArgs)
    '    ' Ξεκινάει την διαδικασία Delete των δεδομένων του DataGrid 
    '    ' μαζί με τροποποίηση των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    DeleteDatagrid(dgvHairDies)

    '    ChangeControlsHairDies(False)
    'End Sub


    '' Ελέγχει αν η ημερομηνία είοναι σωστή και αν δεν υπάρχει βάζει τη σημερινή
    'Private Sub dgvHairDies_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs)

    '    Dim headerText As String = dgvHairDies.Columns(e.ColumnIndex).HeaderText

    '    ' Ελέγχει αν βρισκόμαστε στο πεδίο Ημερομηνία
    '    If headerText.Equals("Ημερομηνία") Then
    '        Dim dt As DateTime

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι ημερομηνία
    '        If e.FormattedValue.ToString <> String.Empty AndAlso Not DateTime.TryParse(e.FormattedValue.ToString, dt) AndAlso btnEditHairDie.Text = "Cancel" Then
    '            MessageBox.Show("Λάθος καταχώρηση ημερομηνίας", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True

    '        End If

    '    End If
    'End Sub



    'Private Sub dgvHairDies_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
    '    Try
    '        ' Clear the row error in case the user presses ESC.   
    '        dgvDebts.Rows(e.RowIndex).ErrorText = String.Empty
    '    Catch ex As Exception

    '    End Try

    'End Sub





    ' ****************************************************************************************************************************
    ' **********    ΔΙΑΦΟΡΕΣ ΑΛΛΕΣ ΡΟΥΤΙΝΕΣ    ***********************************************************************************
    ' ****************************************************************************************************************************



    ' Αναλαμβάνει να αρχίσει να αναβοσβήνει ένα label ενημέρωσης
    Private Sub FlashingLabel(ByVal oLabel As Label, ByVal interval As Integer)

        'If oLabel.ForeColor = oLabel.BackColor Then
        '    oLabel.ForeColor = SystemColors.ControlText
        'Else
        '    oLabel.ForeColor = oLabel.BackColor
        'End If

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




























    ' *******************************************************************
    '********************************************************************
    '                TAB 2 
    '********************************************************************
    '********************************************************************





    '' ****************************************************************************************************************************
    '' **********    ΔΙΑΧΕΙΡΗΣΗ ΑΡΧΕΙΟΥ ΦΑΡΜΑΚΩΝ    *******************************************************************************
    '' ****************************************************************************************************************************


    'Private Sub GetDrugs()

    '    ' Με βάση το μέρος ονόματος του πελάτη (textbox) και τις επιλογές μας στo ComboBox
    '    ' βρίσκει όλους τους πελάτες..
    '    If cboSearchCategory.Text = "Όλα" Then
    '        stringDTG = "SELECT  Name, Id From PharmacyCustomFiles.dbo.Drugs " & _
    '                               "WHERE Drugs.Name like '%" & txtSearchDrugs.Text.ToString & "%' " & _
    '                               "ORDER BY Name, Id"
    '    Else
    '        stringDTG = "SELECT  Name, Id From PharmacyCustomFiles.dbo.Drugs " & _
    '                               "WHERE Drugs.Name like '%" & txtSearchDrugs.Text.ToString & "%' AND " & _
    '                                        "Drugs.Category = '" & cboSearchCategory.Text & "' " & _
    '                                "ORDER BY Name, Id"
    '    End If

    '    ' Γεμίζει το lstCustomers με τους πελάτες που αντιστοιχούν στις επιλογές μας και 
    '    ' κρατάει στην μεταβλητή sumCustomers τον συνολικό αριθμό πελατών
    '    Dim sumDrugs As Integer = FillDatagrid(dgvDrugs, bsDrugs, {"Όνομα"}, {400}, {"0"}, {"Id"})

    '    ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Select Case sumDrugs
    '        Case 0

    '            rtxtDrugsMessage.Text = "Δεν βρέθηκαν προιόντα"

    '        Case 1

    '            rtxtDrugsMessage.Text = "Βρέθηκε 1 προιόντα"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtDrugsMessage, {"1"})

    '        Case Is > 1

    '            rtxtDrugsMessage.Text = "Βρέθηκαν " & sumDrugs.ToString & " προιόντα"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtDrugsMessage, {sumDrugs.ToString})

    '    End Select

    'End Sub


    Private Function GetPriceParaDrugs(ByVal mode As String) As Integer

        ' Με βάση το μέρος ονόματος (textbox) 
        ' βρίσκει όλα τα παραφάρμακα και τις τιμές τους..
        If mode = "name" Then
            Try
                stringDTG = "SELECT Id, isnull(Name, '') as Name2, isnull(Xondr,'') as Xondr2, isnull(Lian,'') as Lian2, isnull(Notes,'') as Notes2, " &
                                           "isnull(AP_Code,'') as AP_Code2,  isnull(AP_ID,'') as AP_ID2, isnull(Barcode,'') as Barcode2 " &
                                       "FROM PharmacyCustomFiles.dbo.PricesParaDrugs " &
                                       "WHERE Name like '%" & txtSearchPricesParadrugs.Text.ToString & "%' " &
                                       "ORDER BY Name, Id"
            Catch ex As Exception

            End Try


        ElseIf mode = "barcode" Then

            stringDTG = "SELECT Id, isnull(Name, '') as Name2, isnull(Xondr,'') as Xondr2, isnull(Lian,'') as Lian2, isnull(Notes,'') as Notes2, " &
                            "isnull(AP_Code,'') as AP_Code2,  isnull(AP_ID,'') as AP_ID2, isnull(Barcode,'') as Barcode2 " &
                        "FROM PharmacyCustomFiles.dbo.PricesParaDrugs " &
                        "WHERE Barcode like '%" & txtSearchPricesParadrugs.Text.ToString & "%' " &
                        "ORDER BY Name, Id"

        End If


        Dim sumDrugs As Integer = DisplayCustomDatagrid_Paradrugs()
        sumDrugs = dgvPricesParadrugs.Rows.Count - 1

        ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
        ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
        Select Case sumDrugs
            Case 0

                rtxtPricesParadrugs.Text = "Δεν βρέθηκαν παραφάρμακα"

            Case 1

                rtxtPricesParadrugs.Text = "Βρέθηκε 1 παραφάρμακο"

                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtPricesParadrugs, {"1"})

            Case Is > 1

                rtxtPricesParadrugs.Text = "Βρέθηκαν " & sumDrugs.ToString("###,###") & " παραφάρμακα"

                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtPricesParadrugs, {sumDrugs.ToString("###,###")})

        End Select

        Return sumDrugs

    End Function


    Private Function GetDrugs(ByVal mode As String) As Integer

        Dim str2find, qr As String

        EnsureDrugQrCodeOverridesTable()

        str2find = txtSearchPricesParadrugs.Text



        ' Με βάση το μέρος ονόματος (textbox) 
        ' βρίσκει όλα τα παραφάρμακα και τις τιμές τους..
        If mode = "name" Then
            'stringDTG = "SELECT dbo.APOTIKH.AP_ID, dbo.APOTIKH.AP_CODE, dbo.APOTIKH.AP_DESCRIPTION, dbo.APOTIKH.AP_MORFI, dbo.APOTIKH.AP_NARKWTIKO, " &
            '                               "dbo.APOTIKH.AP_NOSOKOMEIAKO, dbo.APOTIKH.AP_KTHNIATRIKO, dbo.APOTIKH.AP_ELLEICH, dbo.APOTIKH.AP_APOSYRSH, dbo.APOTIKH.AP_LISTA, " &
            '                               "dbo.APOTIKH.AP_IFET, dbo.APOTIKH_BARCODES.BRAP_AP_BARCODE, dbo.APOTIKH.AP_TIMH_XON, dbo.APOTIKH.AP_TIMH_LIAN " &
            '                           "FROM dbo.APOTIKH LEFT JOIN dbo.APOTIKH ON dbo.APOTIKH.AP_ID = dbo.APOTIKH_BARCODES.BRAP_AP_ID " &
            '                           "WHERE AP_DESCRIPTION like '%" & txtSearchPricesParadrugs.Text.ToString & "%' " &
            '                           "ORDER BY AP_DESCRIPTION, AP_MORFI"
            Try
                stringDTG = "SELECT dbo.APOTIKH.AP_ID, dbo.APOTIKH.AP_CODE, dbo.APOTIKH.AP_DESCRIPTION, dbo.APOTIKH.AP_MORFI, dbo.APOTIKH.AP_NARKWTIKO, 
                                           dbo.APOTIKH.AP_NOSOKOMEIAKO, dbo.APOTIKH.AP_KTHNIATRIKO, dbo.APOTIKH.AP_ELLEICH, dbo.APOTIKH.AP_APOSYRSH, dbo.APOTIKH.AP_LISTA, 
                                           dbo.APOTIKH.AP_IFET, dbo.APOTIKH_BARCODES.BRAP_AP_BARCODE, 
                                           ISNULL(NULLIF(QRO.QRCode, ''), dbo.APOTIKH_QRCODES.APQ_PRODUCT_CODE) AS APQ_PRODUCT_CODE, 
                                           CASE WHEN NULLIF(QRO.QRCode, '') IS NULL THEN 0 ELSE 1 END AS APQ_IS_CUSTOM_QR,
                                           dbo.APOTIKH.AP_TIMH_XON, dbo.APOTIKH.AP_TIMH_LIAN 
                             FROM dbo.APOTIKH 
							 LEFT JOIN dbo.APOTIKH_BARCODES ON dbo.APOTIKH.AP_ID = dbo.APOTIKH_BARCODES.BRAP_AP_ID 
						     LEFT JOIN dbo.APOTIKH_QRCODES ON dbo.APOTIKH.AP_ID = dbo.APOTIKH_QRCODES.APQ_AP_ID 
                             LEFT JOIN PharmacyCustomFiles.dbo.DrugQrCodeOverrides AS QRO ON dbo.APOTIKH.AP_ID = QRO.AP_ID
                             WHERE AP_DESCRIPTION like '%" & str2find & "%'  
                             ORDER BY AP_DESCRIPTION, AP_MORFI"



            Catch ex As Exception

            End Try


        ElseIf mode = "barcode" Then

            stringDTG = "SELECT dbo.APOTIKH.AP_ID, dbo.APOTIKH.AP_CODE, dbo.APOTIKH.AP_DESCRIPTION, dbo.APOTIKH.AP_MORFI, dbo.APOTIKH.AP_NARKWTIKO, " &
                            "dbo.APOTIKH.AP_NOSOKOMEIAKO, dbo.APOTIKH.AP_KTHNIATRIKO, dbo.APOTIKH.AP_ELLEICH, dbo.APOTIKH.AP_APOSYRSH, dbo.APOTIKH.AP_LISTA, " &
                            "dbo.APOTIKH.AP_IFET, dbo.APOTIKH_BARCODES.BRAP_AP_BARCODE, dbo.APOTIKH.AP_TIMH_XON, dbo.APOTIKH.AP_TIMH_LIAN " &
                        "FROM dbo.APOTIKH INNER JOIN dbo.APOTIKH_BARCODES ON dbo.APOTIKH.AP_ID = dbo.APOTIKH_BARCODES.BRAP_AP_ID " &
                        "WHERE dbo.APOTIKH_BARCODES.BRAP_AP_BARCODE like '%" & str2find & "%' " &
                         "ORDER BY AP_DESCRIPTION, AP_MORFI"

        ElseIf mode = "qrcode" Then

            'qr = GetQRFromScannedCode(str2find)


            stringDTG = "SELECT dbo.APOTIKH.AP_ID, dbo.APOTIKH.AP_CODE, dbo.APOTIKH.AP_DESCRIPTION, dbo.APOTIKH.AP_MORFI, dbo.APOTIKH.AP_NARKWTIKO, " &
                            "dbo.APOTIKH.AP_NOSOKOMEIAKO, dbo.APOTIKH.AP_KTHNIATRIKO, dbo.APOTIKH.AP_ELLEICH, dbo.APOTIKH.AP_APOSYRSH, dbo.APOTIKH.AP_LISTA, " &
                            "dbo.APOTIKH.AP_IFET, ISNULL(NULLIF(QRO.QRCode, ''), dbo.APOTIKH_QRCODES.APQ_PRODUCT_CODE) AS APQ_PRODUCT_CODE, CASE WHEN NULLIF(QRO.QRCode, '') IS NULL THEN 0 ELSE 1 END AS APQ_IS_CUSTOM_QR, dbo.APOTIKH.AP_TIMH_XON, dbo.APOTIKH.AP_TIMH_LIAN " &
                        "FROM dbo.APOTIKH " &
                        "LEFT JOIN dbo.APOTIKH_QRCODES ON dbo.APOTIKH.AP_ID = dbo.APOTIKH_QRCODES.APQ_AP_ID " &
                        "LEFT JOIN PharmacyCustomFiles.dbo.DrugQrCodeOverrides AS QRO ON dbo.APOTIKH.AP_ID = QRO.AP_ID " &
                        "WHERE ISNULL(NULLIF(QRO.QRCode, ''), dbo.APOTIKH_QRCODES.APQ_PRODUCT_CODE) like '%" & str2find & "%' " &
                         "ORDER BY AP_DESCRIPTION, AP_MORFI"

        End If


        Dim sumDrugs As Integer = DisplayCustomDatagrid_Drugs()

        ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
        ' ανάλογα με τον αριθμό των φαρμάκων που βρέθηκαν
        Select Case sumDrugs
            Case 0
                rtxtPricesParadrugs.Text = "Δεν βρέθηκαν προιόντα"
            Case 1
                rtxtPricesParadrugs.Text = "Βρέθηκε 1 προιον"
            Case Is > 1
                rtxtPricesParadrugs.Text = "Βρέθηκαν " & sumDrugs.ToString("###,###") & " προιόντα"
        End Select

        Dim TotKorres As Integer = CalculateTotCount("SELECT * FROM APOTIKH WHERE  AP_DESCRIPTION like '%" & txtSearchPricesParadrugs.Text.ToString & "%'AND " &
                                                     "(AP_DESCRIPTION like '%KORRES%' or AP_DESCRIPTION like '%ΚΟΡΡΕΣ%' or AP_CODE like '%KOR')")
        If TotKorres > 0 Then
            rtxtPricesParadrugs.Text &= ", KORRES (" & TotKorres.ToString("###,###") & ")"
        End If

        ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
        If sumDrugs = 1 Then
            HightlightInRichTextBox(rtxtPricesParadrugs, {"1"})
        ElseIf sumDrugs > 1 Then
            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
            HightlightInRichTextBox(rtxtPricesParadrugs, {sumDrugs.ToString("###,###")})
        End If

        If TotKorres > 1 Then
            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
            HightlightInRichTextBox(rtxtPricesParadrugs, {TotKorres.ToString("###,###")})
        End If

        Return sumDrugs

    End Function


    Private Sub GetTameiaAskedList()

        ' Με βάση το μέρος ονόματος (textbox) 
        ' βρίσκει όλα τα παραφάρμακα και τις τιμές τους..

        If cboTameia.Text = "ΟΛΑ" Then
            stringDTG = "SELECT MyDate, Description,isnull(AmountAsked,0) as AmountAsked2, isnull(AmountGiven,0) as AmountGiven2, isnull(Difference,0) as Difference2, " &
                       "isnull(DifferPercent,0) as DifferPercent2, isnull(PercentagePaid,0) as PercentagePaid2, isnull(IRS, 0) as IRS2, Id " &
                       "FROM PharmacyCustomFiles.dbo.TameiaAsked " &
                       "WHERE Description like '%" & txtSearchTameia.Text.ToString & "%' " &
                       "ORDER BY MyDate, Id"

            stringDTG2 = "SELECT MyDate, Description,isnull(AmountAsked,0) as AmountAsked2, isnull(AmountGiven,0) as AmountGiven2, isnull(Difference,0) as Difference2, " &
                            "isnull(DifferPercent,0) as DifferPercent2, isnull(PercentagePaid,0) as PercentagePaid2, isnull(IRS, 0) as IRS2, Id " &
                            "FROM PharmacyCustomFiles.dbo.TameiaAsked " &
                            "WHERE Description like '%" & txtSearchTameia.Text.ToString & "%' AND IRS=1"

        Else
            stringDTG = "SELECT MyDate, Description,isnull(AmountAsked,0) as AmountAsked2, isnull(AmountGiven,0) as AmountGiven2, isnull(Difference,0) as Difference2, " &
                                   "isnull(DifferPercent,0) as DifferPercent2, isnull(PercentagePaid,0) as PercentagePaid2, isnull(IRS, 0) as IRS2, Id " &
                                   "FROM PharmacyCustomFiles.dbo.TameiaAsked " &
                                   "WHERE Description like '%" & txtSearchTameia.Text.ToString & "%' AND " &
                                    "Description like '%" & cboTameia.Text.ToString & "%' " &
                                   "ORDER BY MyDate, Id"

            stringDTG2 = "SELECT MyDate, Description,isnull(AmountAsked,0) as AmountAsked2, isnull(AmountGiven,0) as AmountGiven2, isnull(Difference,0) as Difference2, " &
                            "isnull(DifferPercent,0) as DifferPercent2, isnull(PercentagePaid,0) as PercentagePaid2, isnull(IRS, 0) as IRS2, Id " &
                            "FROM PharmacyCustomFiles.dbo.TameiaAsked " &
                            "WHERE Description like '%" & txtSearchTameia.Text.ToString & "%' AND IRS=1 AND " &
                            "Description like '%" & cboTameia.Text.ToString & "%' "

        End If




        'Dim sumDrugs As Integer = FillDatagrid(dgvTameiaAsked, bsTameiaAsked, {"Ημερομ", "Αιτιολογία", "Αιτούμ.Ποσό", "Εκκαθ.Ποσό", _
        '                            "Πόσο Κρατ.", "% Κρατ.", "% Πληρωμ.", "Βιβλίο"}, {60, 250, 70, 70, 70, 70, 50, 50}, _
        '                            {"", "", "F2", "F2", "", "", "", ""}, {"Id"})

        Dim totalExchanges As Integer = DisplayCustomDatagrid_TameiaAsked()


        DisplaySumsOnRichTextbox_TameiaAsked()




    End Sub

    Private Sub GetPhonesList()
        Dim total As Integer = 0

        ' Με βάση το μέρος ονόματος (textbox) 
        ' βρίσκει όλα τα παραφάρμακα και τις τιμές τους..

        Select Case cboPhoneCatalog.Text
            Case "Η ΑΤΖΕΝΤΑ ΜΟΥ"
                dgvPhones.ScrollBars = ScrollBars.Vertical
                dgvPhones.ReadOnly = False

                stringDTG = "SELECT * " &
                                       "FROM PharmacyCustomFiles.dbo.Phonebook " &
                                       "WHERE Fullname like '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PhoneNumber1 LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PhoneNumber2 LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PhoneNumber3 LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PhoneNumber4 LIKE '%" & txtSearchPhones.Text.ToString & "%' " &
                                       "ORDER BY Fullname"

                total = DisplayCustomDatagrid_Phones()

            Case "ΑΣΘΕΝΕΙΣ - PHARM"
                dgvPhones.Columns.Clear()
                dgvPhones.AutoGenerateColumns = True
                dgvPhones.ScrollBars = ScrollBars.Both
                dgvPhones.ReadOnly = True

                stringDTG = "Select [PE_ID], [PE_MHTRWO],[PE_EPWNYMIA],[PE_ONOMA],[PE_ODOS],[PE_PERIOXH],[PE_POLH], " &
                                "[PE_POST],[PE_PHONE],[PE_KINHTO],[PE_FAX] ,[PE_EMAIL],[PE_STOX_TEL_DATE] ,[PE_BIRTH_YEAR],[PE_AMKA] " &
                            "FROM [Pharmacy2013C].[dbo].[PELATES] " &
                            "WHERE PE_EPWNYMIA like '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PE_ONOMA LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PE_ODOS LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PE_PERIOXH LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PE_POLH LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PE_PHONE LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PE_KINHTO LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PE_BIRTH_YEAR LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "PE_AMKA LIKE '%" & txtSearchPhones.Text.ToString & "%' " &
                            "order BY [PE_EPWNYMIA], PE_ONOMA"

                total = FillDatagrid(dgvPhones, bsPhones, {"Id", "Αρ.Μητρώου", "Επώνυμο", "Όνομα", "Οδός", "Περιοχή", "Πόλη", "ΤΚ", "Σταθερό", "Κινητό", "Φαξ", "eMail", "Ημερ.Εγγραφής", "ΈτοςΓέννησης", "ΑΜΚΑ"},
                                                          {50, 50, 120, 100, 150, 100, 100, 80, 80, 80, 50, 50, 50, 50, 80}, {"", "", "", "", "", "", "", "", "", "", "", "", "", "", ""}, {"PE_ID", "PE_FAX", "PE_EMAIL", "PE_STOX_TEL_DATE", "PE_POST"})

            Case "ΙΑΤΡΟΙ"
                dgvPhones.Columns.Clear()
                dgvPhones.AutoGenerateColumns = True
                dgvPhones.ScrollBars = ScrollBars.Vertical
                dgvPhones.ReadOnly = True

                stringDTG = "SELECT [DC_EPWNYMO], [DC_ONOMA], [DC_AMKA], [DC_AMETAA]  " &
                            "FROM [Pharmacy2013C].[dbo].[DOCTORS] " &
                            "WHERE DC_EPWNYMO like '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "DC_ONOMA LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "DC_AMKA LIKE '%" & txtSearchPhones.Text.ToString & "%' OR " &
                                           "DC_AMETAA LIKE '%" & txtSearchPhones.Text.ToString & "%' " &
                            "order BY [DC_EPWNYMO], DC_ONOMA"

                total = FillDatagrid(dgvPhones, bsPhones, {"Επώνυμο", "Όνομα", "ΑΜΚΑ", "Αρ.Μητρώου"},
                                                          {250, 250, 120, 120}, {"", "", "", ""}, {})

        End Select

        Select Case total
            Case 0
                rtxtPhones.Text = "Δεν βρέθηκαν καταχωρήσεις"
            Case 1

                rtxtPhones.Text = "Βρέθηκε 1 καταχώρηση"
                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtPhones, {"1"})

            Case Is > 1

                rtxtPhones.Text = "Βρέθηκαν " & total.ToString & " καταχωρήσεις"
                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtPhones, {total.ToString})
        End Select

    End Sub

    Private Sub DisplaySumsOnRichTextbox_TameiaAsked()

        If cboTameia.Text = "ΟΛΑ" Then
            stringDTG = "SELECT MyDate, Description,isnull(AmountAsked,0) as AmountAsked2, isnull(AmountGiven,0) as AmountGiven2, isnull(Difference,0) as Difference2, " &
                       "isnull(DifferPercent,0) as DifferPercent2, isnull(PercentagePaid,0) as PercentagePaid2, isnull(IRS, 0) as IRS2, Id " &
                       "FROM PharmacyCustomFiles.dbo.TameiaAsked " &
                       "WHERE Description like '%" & txtSearchTameia.Text.ToString & "%' " &
                       "ORDER BY MyDate, Id"

            stringDTG2 = "SELECT MyDate, Description,isnull(AmountAsked,0) as AmountAsked2, isnull(AmountGiven,0) as AmountGiven2, isnull(Difference,0) as Difference2, " &
                            "isnull(DifferPercent,0) as DifferPercent2, isnull(PercentagePaid,0) as PercentagePaid2, isnull(IRS, 0) as IRS2, Id " &
                            "FROM PharmacyCustomFiles.dbo.TameiaAsked " &
                            "WHERE Description like '%" & txtSearchTameia.Text.ToString & "%' AND IRS=1"

        Else
            stringDTG = "SELECT MyDate, Description,isnull(AmountAsked,0) as AmountAsked2, isnull(AmountGiven,0) as AmountGiven2, isnull(Difference,0) as Difference2, " &
                                   "isnull(DifferPercent,0) as DifferPercent2, isnull(PercentagePaid,0) as PercentagePaid2, isnull(IRS, 0) as IRS2, Id " &
                                   "FROM PharmacyCustomFiles.dbo.TameiaAsked " &
                                   "WHERE Description like '%" & txtSearchTameia.Text.ToString & "%' AND " &
                                    "Description like '%" & cboTameia.Text.ToString & "%' " &
                                   "ORDER BY MyDate, Id"

            stringDTG2 = "SELECT MyDate, Description,isnull(AmountAsked,0) as AmountAsked2, isnull(AmountGiven,0) as AmountGiven2, isnull(Difference,0) as Difference2, " &
                            "isnull(DifferPercent,0) as DifferPercent2, isnull(PercentagePaid,0) as PercentagePaid2, isnull(IRS, 0) as IRS2, Id " &
                            "FROM PharmacyCustomFiles.dbo.TameiaAsked " &
                            "WHERE Description like '%" & txtSearchTameia.Text.ToString & "%' AND IRS=1 AND " &
                            "Description like '%" & cboTameia.Text.ToString & "%' "

        End If

        Dim totalExchanges As Integer = 0
        Try
            totalExchanges = dgvTameiaAsked.Rows.Count - 1
        Catch ex As Exception

        End Try

        Dim sumAsked As Decimal = CalculateSums(stringDTG, "AmountAsked2")
        Dim sumKept As Decimal = CalculateSums(stringDTG2, "Difference2")
        Dim sumDifference As Decimal = sumAsked - CalculateSums(stringDTG, "AmountGiven2") - sumKept

        ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
        ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
        Select Case totalExchanges
            Case 0

                rtxtTameiaAsked.Text = "Δεν βρέθηκαν συναλλαγές"

            Case 1

                rtxtTameiaAsked.Text = "Βρέθηκε 1 συναλλαγή"

                rtxtTameiaAsked.Text &= ", ύψους " & sumAsked.ToString("c") & ". Οι κρατήσεις ανέρχονται σε " & sumKept.ToString("c")
                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtTameiaAsked, {"1", sumAsked.ToString("c"), sumKept.ToString("c")})
                rtxtTameiaAsked2.Text = "Το συνολικό υπολειπόμενο χρωστούμενο ποσό είναι " & sumDifference.ToString("c")
                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtTameiaAsked2, {sumDifference.ToString("c")})

            Case Is > 1

                rtxtTameiaAsked.Text = "Βρέθηκαν " & totalExchanges.ToString & " συναλλαγές"

                rtxtTameiaAsked.Text &= ", ύψους  " & sumAsked.ToString("c") & ".  Οι κρατήσεις ανέρχονται σε " & sumKept.ToString("c")
                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtTameiaAsked, {totalExchanges.ToString, sumAsked.ToString("c"), sumKept.ToString("c")})
                rtxtTameiaAsked2.Text = "Το συνολικό υπολειπόμενο χρωστούμενο ποσό είναι " & sumDifference.ToString("c")
                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtTameiaAsked2, {sumDifference.ToString("c")})

        End Select
    End Sub



    Private Sub GetTameiaGivenList()

        ' Με βάση το μέρος ονόματος (textbox) 
        ' βρίσκει όλα τα παραφάρμακα και τις τιμές τους..
        Dim TameiaAskedId As Integer = 0


        Try
            TameiaAskedId = dgvTameiaAsked.SelectedRows(0).Cells(8).Value
        Catch ex As Exception
            Exit Sub
        End Try

        stringDTG = "SELECT MyDate, Description, isnull(AmountPaid,0) as AmountPaid2, isnull(PercTotalPaid,0) as PercTotalPaid2, isnull(DifferenceToTotal,0) as DifferenceToTotal2, Id, TameiaAskedId " &
                    "FROM PharmacyCustomFiles.dbo.TameiaPaid " &
                     "WHERE TameiaAskedId =" & TameiaAskedId & " " &
                    "ORDER BY MyDate, Id"

        Dim sumPayments As Integer = DisplayCustomDatagrid_TameiaGiven()

        CalculateTotalTameiaPaidPerTransaction()

    End Sub


    Private Sub CalculateTotalTameiaPaidPerTransaction()
        Dim sumAmount As Decimal = 0
        Try
            sumAmount = CType(CalculateSums(stringDTG, "AmountPaid2"), Decimal)
        Catch ex As Exception
            sumAmount = 0
        End Try

        If sumAmount > 0 Then
            dgvTameiaAsked.SelectedRows(0).Cells(3).Value = sumAmount
        Else

        End If
    End Sub

    'Private Function CalculateTotalPaidPerInvoice() As Decimal
    '    ' Με βάση το μέρος ονόματος (textbox) 
    '    ' βρίσκει όλα τα παραφάρμακα και τις τιμές τους..
    '    Dim TameiaAskedId As Integer = 0
    '    Try
    '        TameiaAskedId = dgvTameiaAsked.SelectedRows(0).Cells(8).Value
    '    Catch ex As Exception
    '        Return 0
    '    End Try

    '    stringDTG = "SELECT sum(AmountPaid) " & _
    '              "FROM PharmacyCustomFiles.dbo.TameiaPaid " & _
    '               "WHERE TameiaAskedId =" & TameiaAskedId

    'End Function



    Private Sub GetCustomersList()

        Dim sumCustomers As Integer = 0

        ' If rbSearchAll.Checked = True Then
        If cboSearchCustomers.Text = "Όλους" Then
            Try
                stringDTG = "SELECT Name, Id From PharmacyCustomFiles.dbo.Customers " &
                                               "WHERE Name like '%" & txtSearchCustomer.Text.ToString & "%' " &
                                               "ORDER BY Name"

                sumCustomers = FillDatagrid(dgvCustomers, bsCustomers, {"Όνοματεπώνυμο"}, {222}, {""}, {"Id"}, "", False)
            Catch ex As Exception

            End Try


            ' ή μονο τους πελάτες με χρέη..
            'ElseIf rbSearchDebts.Checked = True Then
        ElseIf cboSearchCustomers.Text = "Χρέη από συναλλαγές" Then

            stringDTG = "SELECT PharmacyCustomFiles.dbo.Customers.name, PharmacyCustomFiles.dbo.Customers.id, sum(PharmacyCustomFiles.dbo.Debts.Ammount) as SumDebts " &
                       "FROM PharmacyCustomFiles.dbo.Customers INNER JOIN " &
                           "PharmacyCustomFiles.dbo.Debts ON PharmacyCustomFiles.dbo.Customers.Id = PharmacyCustomFiles.dbo.Debts.CustomerId " &
                       "WHERE PharmacyCustomFiles.dbo.Customers.Name like '%" & txtSearchCustomer.Text.ToString & "%' " &
                       "GROUP BY  PharmacyCustomFiles.dbo.customers.name, PharmacyCustomFiles.dbo.customers.id " &
                       "HAVING sum(PharmacyCustomFiles.dbo.Debts.Ammount) > '0.001' " &
                       "ORDER BY PharmacyCustomFiles.dbo.customers.name"

            sumCustomers = FillDatagrid(dgvCustomers, bsCustomers, {"Όνοματεπώνυμο", "", "Χρέος"}, {160, 30, 80}, {"", " ", "F2"}, {"Id"}, "", False)

        ElseIf cboSearchCustomers.Text = "Οφειλές σε πελάτες" Then

            stringDTG = "SELECT PharmacyCustomFiles.dbo.Customers.name, PharmacyCustomFiles.dbo.Customers.id, sum(PharmacyCustomFiles.dbo.Debts.Ammount) as SumDebts " &
                       "FROM PharmacyCustomFiles.dbo.Customers INNER JOIN " &
                           "PharmacyCustomFiles.dbo.Debts ON PharmacyCustomFiles.dbo.Customers.Id = PharmacyCustomFiles.dbo.Debts.CustomerId " &
                       "WHERE PharmacyCustomFiles.dbo.Customers.Name like '%" & txtSearchCustomer.Text.ToString & "%' " &
                       "GROUP BY  PharmacyCustomFiles.dbo.customers.name, PharmacyCustomFiles.dbo.customers.id " &
                       "HAVING sum(PharmacyCustomFiles.dbo.Debts.Ammount) < '-0.001' " &
                       "ORDER BY PharmacyCustomFiles.dbo.customers.name"

            sumCustomers = FillDatagrid(dgvCustomers, bsCustomers, {"Όνοματεπώνυμο", "", "Οφειλή"}, {160, 30, 80}, {"", " ", "F2"}, {"Id"}, "", False)

        ElseIf cboSearchCustomers.Text = "Χρέη από φάρμακα" Then

            stringDTG = "SELECT PharmacyCustomFiles.dbo.Customers.name, PharmacyCustomFiles.dbo.Customers.id, sum(PharmacyCustomFiles.dbo.DrugsOnLoan.Price) as SumDebts " &
                       "FROM PharmacyCustomFiles.dbo.Customers INNER JOIN " &
                           "PharmacyCustomFiles.dbo.DrugsOnLoan ON PharmacyCustomFiles.dbo.Customers.Id = PharmacyCustomFiles.dbo.DrugsOnLoan.CustomerId " &
                       "WHERE PharmacyCustomFiles.dbo.Customers.Name like '%" & txtSearchCustomer.Text.ToString & "%' " &
                       "GROUP BY  PharmacyCustomFiles.dbo.customers.name, PharmacyCustomFiles.dbo.customers.id " &
                       "HAVING sum(PharmacyCustomFiles.dbo.DrugsOnLoan.Price) > '0.001' " &
                       "ORDER BY PharmacyCustomFiles.dbo.customers.name"

            sumCustomers = FillDatagrid(dgvCustomers, bsCustomers, {"Όνοματεπώνυμο", "", "Φάρμακα"}, {160, 30, 80}, {"", " ", "F2"}, {"Id"}, "", False)

        ElseIf cboSearchCustomers.Text = "Συνταγές προς εκτέλεση" Then

            stringDTG = "SELECT PharmacyCustomFiles.dbo.Customers.name, PharmacyCustomFiles.dbo.Customers.id " &
                       "FROM PharmacyCustomFiles.dbo.Customers INNER JOIN " &
                           "PharmacyCustomFiles.dbo.Prescriptions ON PharmacyCustomFiles.dbo.Customers.Id = PharmacyCustomFiles.dbo.Prescriptions.CustomerId " &
                       "WHERE PharmacyCustomFiles.dbo.Customers.Name like '%" & txtSearchCustomer.Text.ToString & "%' AND PharmacyCustomFiles.dbo.Prescriptions.ProcessedDate IS NULL " &
                        "GROUP BY  PharmacyCustomFiles.dbo.customers.name, PharmacyCustomFiles.dbo.customers.id " &
                        "ORDER BY PharmacyCustomFiles.dbo.customers.name"

            sumCustomers = FillDatagrid(dgvCustomers, bsCustomers, {"Όνοματεπώνυμο", ""}, {232, 30}, {"", " "}, {"Id"}, "", False)

        ElseIf cboSearchCustomers.Text = "Βαφές" Then

            stringDTG = "SELECT  distinct PharmacyCustomFiles.dbo.Customers.Name, PharmacyCustomFiles.dbo.Customers.id " &
                        "FROM PharmacyCustomFiles.dbo.Customers INNER JOIN PharmacyCustomFiles.dbo.HairDies ON PharmacyCustomFiles.dbo.Customers.Id = PharmacyCustomFiles.dbo.HairDies.CustomerId " &
                        "WHERE PharmacyCustomFiles.dbo.Customers.Name like '%" & txtSearchCustomer.Text.ToString & "%' AND " &
                            "PharmacyCustomFiles.dbo.HairDies.HairDieDescription Is Not null " &
                        "ORDER BY Name"

            sumCustomers = FillDatagrid(dgvCustomers, bsCustomers, {"Όνοματεπώνυμο"}, {232}, {""}, {"Id"}, "", False)
        End If

        ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
        ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
        DisplaySumsCustomers()

    End Sub







    Private Sub DisplaySumsCustomers()
        Dim sumCustomers As Integer
        Dim SumDebts As Decimal = 0

        If cboSearchCustomers.Text = "Χρέη από συναλλαγές" Or cboSearchCustomers.Text = "Χρέη από φάρμακα" Or cboSearchCustomers.Text = "Οφειλές σε πελάτες" Then
            For i As Integer = 0 To dgvCustomers.Rows.Count - 1
                SumDebts += Convert.ToDecimal(dgvCustomers.Rows(i).Cells(2).Value)
            Next
        End If

        Try
            grpCustDebts.Text = "#" & dgvCustomers.SelectedRows(0).Cells(1).Value.ToString & "  " & dgvCustomers.SelectedRows(0).Cells(0).Value.ToString
        Catch ex As Exception
            grpCustDebts.Text = ""
        End Try

        Try
            sumCustomers = dgvCustomers.RowCount - 1
        Catch ex As Exception
        End Try


        Select Case sumCustomers
            Case 0

                rtxtCustomersMessage.Text = "Δεν βρέθηκαν πελάτες"
                WhenNoCustomers(True)

            Case 1

                If cboSearchCustomers.Text = "Χρέη από συναλλαγές" Then
                    rtxtCustomersMessage.Text = "Βρέθηκε 1 πελάτης, με χρέη ύψους " & SumDebts.ToString("###,###.##") & " €"
                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtCustomersMessage, {"1", SumDebts.ToString("###,###.##")})
                ElseIf cboSearchCustomers.Text = "Οφειλές σε πελάτες" Then
                    rtxtCustomersMessage.Text = "Βρέθηκε 1 πελάτης, στον οποίο οφείλω συνολικά " & SumDebts.ToString("###,###.##") & " €"
                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtCustomersMessage, {sumCustomers.ToString, SumDebts.ToString("###,###.##")})
                ElseIf cboSearchCustomers.Text = "Χρέη από φάρμακα" Then
                    rtxtCustomersMessage.Text = "Βρέθηκε 1 πελάτης, με φάρμακα ύψους " & SumDebts.ToString("###,###.##") & " €"
                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtCustomersMessage, {"1", SumDebts.ToString("###,###.##")})
                Else
                    rtxtCustomersMessage.Text = "Βρέθηκε 1 πελάτης"
                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtCustomersMessage, {"1"})
                End If

                WhenNoCustomers(False)

            Case Is > 1
                If cboSearchCustomers.Text = "Χρέη από συναλλαγές" Then
                    rtxtCustomersMessage.Text = "Βρέθηκαν " & sumCustomers.ToString & " πελάτες, με συνολικά χρέη ύψους " & SumDebts.ToString("###,###.##") & " €"
                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtCustomersMessage, {sumCustomers.ToString, SumDebts.ToString("###,###.##")})
                ElseIf cboSearchCustomers.Text = "Οφειλές σε πελάτες" Then
                    rtxtCustomersMessage.Text = "Βρέθηκαν " & sumCustomers.ToString & " πελάτες, στους οποίους οφείλω συνολικά " & SumDebts.ToString("###,###.##") & " €"
                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtCustomersMessage, {sumCustomers.ToString, SumDebts.ToString("###,###.##")})
                ElseIf cboSearchCustomers.Text = "Χρέη από φάρμακα" Then
                    rtxtCustomersMessage.Text = "Βρέθηκαν " & sumCustomers.ToString & " πελάτες, με φάρμακα ύψους " & SumDebts.ToString("###,###.##") & " €"
                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtCustomersMessage, {sumCustomers.ToString, SumDebts.ToString("###,###.##")})
                Else
                    rtxtCustomersMessage.Text = "Βρέθηκαν " & sumCustomers.ToString & " πελάτες"
                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtCustomersMessage, {sumCustomers.ToString})
                End If

                WhenNoCustomers(False)

        End Select
    End Sub

    Private Sub WhenNoCustomers(ByVal mode As Boolean)
        grpCustDebts.Visible = Not mode
        grpPrescriptions.Visible = Not mode
        grpDrugsOnLoan.Visible = Not mode
        grpCustHairDies.Visible = Not mode
        txtNoCustomers.Visible = mode

    End Sub


    Private Sub GetAgoresOrSoldList()

        If cbAgoresOrSold.Text = "Έσοδα (Πωλήσεις)" Then

            stringDTG = "SELECT * From PharmacyCustomFiles.dbo.FarmSold " &
                                         "WHERE Description like '%" & txtAgoresSoldSearch.Text.ToString & "%' " &
                                         "ORDER By MyDate"

            FillDatagrid(dgvAgoresSold, bsAgoresSold, {"Id", "Ημερομ", "Παραστ", "Περιγραφή", "Χονδρ.13%",
                  "Χονδρ.24%", "Χονδρ.6%", "Λιαν.13%", "Λιαν.24%", "Λιαν.6%", "ΦΠΑ Εσόδων"}, {3, 65, 50, 150, 70, 70, 70, 70, 70, 70, 60},
                  {"", "", "", "", "F2", "F2", "F2", "F2", "F2", "F2", "F2"}, {"Id", "ZItems", "ZReceipts"})


        ElseIf cbAgoresOrSold.Text = "Έξοδα (Δαπάνες)" Then

            stringDTG = "SELECT * From PharmacyCustomFiles.dbo.FarmAgores " &
                                        "WHERE Description like '%" & txtAgoresSoldSearch.Text.ToString & "%' " &
                                         "ORDER By MyDate"

            FillDatagrid(dgvAgoresSold, bsAgoresSold, {"Id", "Ημερομ", "Παραστ", "Περιγραφή", "Αγορές με 13%",
                               "Αγορές με 24%", "Αγορές με 6%", "Δαπ. Χωρίς Έκπτωση", "Δαπ. Με Έκπτωση", "ΦΠΑ αγορών", "ΦΠΑ Δαπανών"}, {3, 65, 50, 150, 70, 70, 70, 70, 70, 70, 60},
                               {"", "", "", "", "F2", "F2", "F2", "F2", "F2", "F2", "F2"}, {"Id"})
        End If

        'Πάει στο τελευταίο record του datagrid
        Try
            Me.dgvAgoresSold.FirstDisplayedScrollingRowIndex = Me.dgvAgoresSold.RowCount - 1
        Catch ex As Exception
        End Try




    End Sub


    Private Sub GetExchangesGivenToList()

        ' Με βάση το μέρος ονόματος (textbox) 
        ' βρίσκει όλα τα παραφάρμακα και τις τιμές τους..

        stringDTG = "SELECT Id, DrugName, Qnt, Xondr,  RP, AP_Code, MyDate From PharmacyCustomFiles.dbo.Exchanges " &
                                     "WHERE Exch ='" & cbExchangers.Text & "' AND FromTo=0 " &
                                     "ORDER BY Id"

        ' Γεμίζει το lstCustomers με τους πελάτες που αντιστοιχούν στις επιλογές μας και 
        ' κρατάει στην μεταβλητή sumCustomers τον συνολικό αριθμό πελατών
        Dim sumDrugs As Integer = FillDatagrid(dgvGivenTo, bsExchangesGivenTo, {"Id", "Όνομα", "ΦΠΑ", "Ποσ", "Χονδρική", "RP", "Κωδικός"}, {3, 264, 35, 35, 55, 20, 80, 20}, {"", "", "", "F2", "", ""}, {"Id", "AP_Code", "MyDate"})        ' Dim sumDrugs As Integer = FillDatagrid(dgvPricesParadrugs, bsPricesParadrugs, {}, {}, {}, {})

        ' Γράφει μια ενημερωση στο Label κάτω από το datagrid
        ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
        Dim mySqlString = "SELECT Qnt, Xondr From PharmacyCustomFiles.dbo.Exchanges " &
                         "WHERE Exch ='" & cbExchangers.Text & "' AND FromTo=0 "
        Dim totalItems As Integer = CalculateSums(mySqlString, "Qnt")
        Dim totalSum As Decimal = CalculateSums(mySqlString, "Xondr")
        Dim FromWho As String = cbExchangers.Text

        InPerPharmacist = totalSum

        Select Case totalItems
            Case 0
                rtxtGivenTo.Text = "Δεν πήραμε προιόντα από [ " & FromWho & "]"


            Case 1
                rtxtGivenTo.Text = "Πήραμε 1 προιόν, συνολικού κόστους " & totalSum.ToString("###,###.## €")

                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtGivenTo, {"1", totalSum.ToString})

            Case Is > 1
                rtxtGivenTo.Text = "Πήραμε " & totalItems.ToString("###,###") & " προιόντα, συνολικού κόστους " & totalSum.ToString("###,###.## € ")

                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtGivenTo, {totalItems.ToString("###,###"), totalSum.ToString("###,###.##")})

        End Select

        'Πάει στο τελευταίο record του datagrid
        SafeScrollToLastRow(dgvGivenTo)

    End Sub


    Private Sub GetExchangesList(ByVal mode As String)
        Dim FromDate, ToDate As String

        FromDate = dtpFromDate.Value
        ToDate = dtpToDate.Value

        If mode = "given" Then
            stringDTG = "SELECT Id, DrugName, FPA, Qnt, Xondr,  RP, AP_Code, MyDate From PharmacyCustomFiles.dbo.Exchanges " &
                                    "WHERE Exch ='" & cbExchangers.Text & "' AND FromTo=0 AND " &
                                        "Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') <=0 AND " &
                                        "Datediff(day, MyDate, '" & CType(ToDate, Date).ToString("yyyy-MM-dd") & "') >=0 " &
                                    "ORDER BY MyDate, Id"

            DisplayCustomDatagrid_Exchanges(bsExchangesGivenTo, dgvGivenTo)

        ElseIf mode = "taken" Then
            stringDTG = "SELECT Id, DrugName, FPA, Qnt, Xondr,  RP, AP_Code, MyDate From PharmacyCustomFiles.dbo.Exchanges " &
                                        "WHERE Exch ='" & cbExchangers.Text & "' AND FromTo=1 AND " &
                                            "Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') <=0 AND " &
                                            "Datediff(day, MyDate, '" & CType(ToDate, Date).ToString("yyyy-MM-dd") & "') >=0 " &
                                        "ORDER BY MyDate, Id"

            DisplayCustomDatagrid_Exchanges(bsExchangesTakenFrom, dgvTakenFrom)

        End If


        ' Γράφει μια ενημερωση στο Label κάτω από το datagrid
        ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
        'Dim mySqlString = "SELECT Qnt, Xondr From PharmacyCustomFiles.dbo.Exchanges " & _
        '                 "WHERE Exch ='" & cbExchangers.Text & "' AND FromTo=0 "
        Dim totalItems As Integer = CalculateSums(stringDTG, "Qnt")
        Dim totalSum As Decimal = CalculateSums(stringDTG, "Xondr")
        Dim FromWho As String = cbExchangers.Text


        If mode = "given" Then

            OutPerPharmacist = totalSum

            Select Case totalItems
                Case 0
                    rtxtGivenTo.Text = "Δεν δώσαμε προιόντα από [ " & FromWho & "]"


                Case 1
                    rtxtGivenTo.Text = "Δώσαμε 1 προιόν, συνολικού κέρδους " & totalSum.ToString("###,###.00 €")

                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBoxGreen(rtxtGivenTo, {"1", totalSum.ToString})

                Case Is > 1
                    rtxtGivenTo.Text = "Δώσαμε " & totalItems.ToString("###,###") & " προιόντα, συνολικού κέρδους " & totalSum.ToString("###,###.00 € ")

                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBoxGreen(rtxtGivenTo, {totalItems.ToString("###,###"), totalSum.ToString("###,###.00")})

            End Select

            'Πάει στο τελευταίο record του datagrid
            SafeScrollToLastRow(dgvGivenTo)


        ElseIf mode = "taken" Then

            InPerPharmacist = totalSum

            Select Case totalItems
                Case 0
                    rtxtTakenFrom.Text = "Δεν πήραμε προιόντα από [ " & FromWho & "]"


                Case 1
                    rtxtTakenFrom.Text = "Πήραμε 1 προιόν, συνολικού κόστους " & totalSum.ToString("###,###.00 €")

                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtTakenFrom, {"1", totalSum.ToString})

                Case Is > 1
                    rtxtTakenFrom.Text = "Πήραμε " & totalItems.ToString("###,###") & " προιόντα, συνολικού κόστους " & totalSum.ToString("###,###.00 € ")

                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtTakenFrom, {totalItems.ToString("###,###"), totalSum.ToString("###,###.00")})

            End Select

            'Πάει στο τελευταίο record του datagrid
            SafeScrollToLastRow(dgvTakenFrom)

        End If

        ApplyReadOnlyOnExchangeGrids() ' <— πρόσθεσέ το εδώ

    End Sub

    ' --- frmCustomers.vb ---
    Private Sub SafeScrollToLastRow(dgv As DataGridView)
        If dgv Is Nothing OrElse dgv.RowCount <= 0 Then Exit Sub

        ' 1) Βρες τελευταίο ΟΡΑΤΟ row που ΔΕΝ είναι NewRow
        Dim lastVisible As Integer = dgv.Rows.GetLastRow(DataGridViewElementStates.Visible)
        If lastVisible < 0 Then Exit Sub

        Dim targetRow As Integer = -1
        For i As Integer = lastVisible To 0 Step -1
            If dgv.Rows(i).Visible AndAlso Not dgv.Rows(i).IsNewRow Then
                targetRow = i : Exit For
            End If
        Next

        ' Αν υπάρχουν μόνο NewRow/καθόλου γραμμές → καθάρισε επιλογή & scroll μόνο
        If targetRow = -1 Then
            Try
                _suppressSelectionChanged = True
                dgv.ClearSelection()
                Try : dgv.FirstDisplayedScrollingRowIndex = lastVisible : Catch : End Try
            Finally
                _suppressSelectionChanged = False
            End Try
            Exit Sub
        End If

        ' 2) Διάλεξε πρώτη ορατή και μη ReadOnly στήλη (για πιθανό CurrentCell)
        Dim firstVisibleCol As Integer = -1
        For Each col As DataGridViewColumn In dgv.Columns
            If col.Visible AndAlso Not col.ReadOnly Then
                firstVisibleCol = col.Index : Exit For
            End If
        Next

        ' 3) Scroll ώστε να φαίνεται η targetRow
        Try : dgv.FirstDisplayedScrollingRowIndex = targetRow : Catch : End Try

        ' 4) Κλείσε τυχόν edit ΠΡΙΝ πειράξεις επιλογές/CurrentCell
        Dim prevCV As Boolean = dgv.CausesValidation
        Dim prevEditMode As DataGridViewEditMode = dgv.EditMode
        dgv.CausesValidation = False
        dgv.EditMode = DataGridViewEditMode.EditProgrammatically
        Try
            If dgv.IsCurrentCellInEditMode Then
                Try
                    If Not dgv.EndEdit(DataGridViewDataErrorContexts.Commit) Then
                        dgv.CancelEdit()
                    End If
                Catch
                    Try : dgv.CancelEdit() : Catch : End Try
                End Try
            End If

            ' 5) Μόνο οπτική επιλογή (ασφαλές)
            _suppressSelectionChanged = True
            dgv.ClearSelection()
            dgv.Rows(targetRow).Selected = True
            _suppressSelectionChanged = False
        Finally
            dgv.CausesValidation = prevCV
            dgv.EditMode = prevEditMode
        End Try

        ' 6) Προσπάθησε να ορίσεις CurrentCell ΜΟΝΟ αν είναι απολύτως ασφαλές
        If firstVisibleCol >= 0 AndAlso dgv.IsHandleCreated AndAlso Not dgv.IsDisposed Then
            dgv.BeginInvoke(Sub()
                                If dgv.IsDisposed Then Exit Sub
                                If targetRow < 0 OrElse targetRow >= dgv.RowCount Then Exit Sub
                                If firstVisibleCol < 0 OrElse firstVisibleCol >= dgv.Columns.Count Then Exit Sub
                                If Not dgv.Columns(firstVisibleCol).Visible OrElse dgv.Columns(firstVisibleCol).ReadOnly Then Exit Sub
                                If Not dgv.Rows(targetRow).Visible OrElse dgv.Rows(targetRow).IsNewRow Then Exit Sub

                                ' Αν υπάρχει ενεργό edit εδώ, μην επιχειρήσεις CurrentCell – απλώς άφησε το Selected
                                If dgv.IsCurrentCellInEditMode Then Exit Sub

                                    ' Επιπλέον ασπίδα: αν το προηγούμενο κελί δεν μπόρεσε να κάνει commit (λόγω validation),
                                    ' μην πετάξεις exception – δοκίμασε σε περιβάλλον χωρίς validation και πιάσ’ το αθόρυβα.
                                    Dim restoreCV = dgv.CausesValidation
                                    Dim restoreEdit = dgv.EditMode
                                    dgv.CausesValidation = False
                                    dgv.EditMode = DataGridViewEditMode.EditProgrammatically

                                    ' Προαιρετική σίγαση του ειδικού validation στα χρέη
                                    Dim restoreDebtValid As Boolean = _suppressDebtsValidation
                                    If Object.ReferenceEquals(dgv, dgvDebtsList) Then _suppressDebtsValidation = True

                                    Try
                                        ' Αν είναι ήδη αυτό το κελί, μη ξαναγράψεις
                                        Dim cur = dgv.CurrentCell
                                        If cur Is Nothing OrElse cur.RowIndex <> targetRow OrElse cur.ColumnIndex <> firstVisibleCol Then
                                            dgv.CurrentCell = dgv.Rows(targetRow).Cells(firstVisibleCol)
                                        End If
                                        dgv.Rows(targetRow).Selected = True
                                    Catch
                                        ' Αν αποτύχει, κράτα μόνο οπτική επιλογή — αυτό αποφεύγει το InvalidOperationException
                                    Finally
                                        _suppressDebtsValidation = restoreDebtValid
                                        dgv.CausesValidation = restoreCV
                                        dgv.EditMode = restoreEdit
                                    End Try
        End Sub)
        End If
    End Sub








    Private Sub UpdateExchangesTotalAndSums()

        Dim stringGiven As String = "SELECT Id, DrugName, Qnt, Xondr,  RP, AP_Code, MyDate From PharmacyCustomFiles.dbo.Exchanges " &
                                    "WHERE Exch ='" & cbExchangers.Text & "' AND FromTo=0 AND Datediff(day, MyDate, '" & dtpFromDate.Value.ToString("yyyy-MM-dd") & "') <=0 "

        Dim stringTaken As String = "SELECT Id, DrugName, Qnt, Xondr,  RP, AP_Code, MyDate From PharmacyCustomFiles.dbo.Exchanges " &
                                    "WHERE Exch ='" & cbExchangers.Text & "' AND FromTo=1 AND Datediff(day, MyDate, '" & dtpFromDate.Value.ToString("yyyy-MM-dd") & "') <=0 "

        ' Γράφει μια ενημερωση στο Label κάτω από το datagrid
        ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν

        Dim totalItemsGiven As Integer = CalculateSums(stringGiven, "Qnt"), totalItemsTaken As Integer = CalculateSums(stringTaken, "Qnt")
        Dim totalSumGiven As Decimal = CalculateSums(stringGiven, "Xondr"), totalSumTaken As Decimal = CalculateSums(stringTaken, "Xondr")

        Dim FromWho As String = cbExchangers.Text

        OutPerPharmacist = totalSumGiven
        InPerPharmacist = totalSumTaken

        Select Case totalItemsGiven
            Case 0
                rtxtGivenTo.Text = "Δεν δώσαμε προιόντα σε [ " & FromWho & "]"


            Case 1
                rtxtGivenTo.Text = "Δώσαμε 1 προιόν, συνολικού κέρδους " & totalSumGiven.ToString("###,###.00 €")

                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBoxGreen(rtxtGivenTo, {"1", totalSumGiven.ToString})

            Case Is > 1
                rtxtGivenTo.Text = "Δώσαμε " & totalItemsGiven.ToString("###,###") & " προιόντα, συνολικού κέρδους " & totalSumGiven.ToString("###,###.00 € ")

                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBoxGreen(rtxtGivenTo, {totalItemsGiven.ToString("###,###"), totalSumGiven.ToString("###,###.00")})

        End Select

        Select Case totalItemsTaken
            Case 0
                rtxtTakenFrom.Text = "Δεν πήραμε προιόντα από [ " & FromWho & "]"


            Case 1
                rtxtTakenFrom.Text = "Πήραμε 1 προιόν, συνολικού κέρδους " & totalSumTaken.ToString("###,###.00 €")

                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtGivenTo, {"1", totalSumTaken.ToString})

            Case Is > 1
                rtxtTakenFrom.Text = "Πήραμε " & totalItemsTaken.ToString("###,###") & " προιόντα, συνολικού κέρδους " & totalSumTaken.ToString("###,###.00 € ")

                ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                HightlightInRichTextBox(rtxtTakenFrom, {totalItemsTaken.ToString("###,###"), totalSumTaken.ToString("###,###.00")})

        End Select

        CalculatePreviousTotalBalance()
        DisplayExchangesBalance()

    End Sub






    Private Sub CalculateNewLianikiSelectedDrug()

        ' Δήλωση μεταβλητών
        Dim xondrPrice As Decimal = 0
        Dim fpa As Decimal = 0
        Dim profit As Decimal = 0
        Dim result As Decimal

        ' Παίρνει την χονδρική τιμή του επιλεγμένου φαρμάκου
        Try
            xondrPrice = CType(dgvPricesParadrugs.Rows(dgvPricesParadrugs.SelectedRows(0).Index).Cells(1).Value, Decimal)
        Catch ex As Exception
            xondrPrice = 0
        End Try

        fpa = (CType(cboFPA_Paradrugs.Text, Decimal) / 100) + 1
        profit = (CType(txtProfit_Paradrugs.Text, Integer) / 100) + 1

        ' Υπολογίζει την λιανική του φαρμάκου και την εμφανίζει στο textbox
        result = (fpa * profit * xondrPrice)
        txtTotalPrice_Paradrugs.Text = result.ToString("###,###.##")

    End Sub


    'Private Sub txtSearchDrugs_TextChanged_1(sender As Object, e As EventArgs)
    '    ' Βρίσκει τα προιόντα που αντιστοιχούν στις επιλογές μας και τους γράφει στο ListBox
    '    'GetDrugs()

    '    ' Αναγράφει τιμές και λήξεις για το επιλεγμένο προιόν
    '    'DisplayPricesAndExpirations()
    'End Sub



    'Private Sub cboSearchCategory_SelectedIndexChanged(sender As Object, e As EventArgs)

    '    ' Βρίσκει τα προιόντα που αντιστοιχούν στις επιλογές μας και τους γράφει στο ListBox
    '    'GetDrugs()

    '    ' Αναγράφει τιμές και λήξεις για το επιλεγμένο προιόν
    '    'DisplayPricesAndExpirations()

    'End Sub




    'Private Sub DisplayPricesAndExpirations()

    '    ' Δήλωση μεταβλητής 
    '    Dim drugName As String = ""
    '    Dim drugId As Integer = 0

    '    Try
    '        ' Βρίσκει το Name και το Id της επιλογής μας από το dgvDrugs
    '        drugName = dgvDrugs.CurrentCell.Value.ToString
    '        drugId = dgvDrugs.Rows(dgvDrugs.SelectedRows(0).Index).Cells("Id").Value
    '    Catch ex As Exception
    '    End Try

    '    ' Εμφανίζει το όνομα και το Id του φαρμάκου στο μεγάλο κεντρικό Label
    '    lblSelectedDrugName.Text = drugName
    '    grpDrugName.Text = "# " & drugId.ToString


    '    'Ελέγχει ότι το Datagrid με τα φάρμακα έχει ήδη κάποιο φάρμακο επιλεγμένο και ...
    '    'If drugName <> "" Then

    '    ' Γεμίζει το DatagridView με τις τιμές του επιλεγμένου φαρμάκου 
    '    stringDTG = "Select Price2, Price1, Notes2, Name, Id, DrugId From PharmacyCustomFiles.dbo.Prices WHERE DrugId = '" & drugId & "' AND (Price1> 0 OR Price2 > 0) "
    '    FillDatagrid(dgvPrices, bsPrices, {"Λιανική", "Χονδρική", "Παρατηρήσεις"}, {80, 80, 297}, {"c", "c", "0"}, {"Id", "DrugId", "Name"})

    '    ' Γεμίζει το DatagridView dgvExpirations με τα ημερομηνίες λήξης του επιλεγμένου φαρμάκου
    '    stringDTG = "Select ExpMonth, ExpYear, Id, DrugId, Name From PharmacyCustomFiles.dbo.Expirations WHERE DrugId = '" & drugId & "' ORDER BY ExpYear,ExpMonth"
    '    FillDatagrid(dgvExpirations, bsExpirations, {"Μήνας", "Έτος"}, {85, 85}, {"##", "####"}, {"Id", "DrugId", "Name"})

    '    For t = 0 To 1
    '        ' Alignment των στοιχείων των Column
    '        dgvPrices.Columns(t).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        dgvExpirations.Columns(t).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        ' Alignment των HeaderText των Column
    '        dgvPrices.Columns(t).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        dgvExpirations.Columns(t).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '    Next t

    '    ' Βρίσκει και αναγράφει τις λήξεις φαρμάκων για το επιλεγμένο χρονικό διάστημα
    '    months = 2
    '    DisplayIncomingExpirations(months)
    'End Sub




    'Private Sub dgvDrugs_CellClick(sender As Object, e As DataGridViewCellEventArgs)

    '    ' Βρίσκει τα προιόντα που αντιστοιχούν στις επιλογές μας και τους γράφει στο ListBox
    '    GetDrugs()

    '    ' Αναγράφει τιμές και λήξεις για το επιλεγμένο προιόν
    '    DisplayPricesAndExpirations()

    '    'Ξαναγράφει τους τίτλους στα 2 groupbox
    '    GrpPrices.Text = "Τιμές και Παρατηρήσεις"
    '    grpExpirations.Text = "Λήξεις Φαρμάκων"

    'End Sub





    'Private Sub dgvDrugs_KeyDown(sender As Object, e As KeyEventArgs)

    '    ' Βρίσκει τα προιόντα που αντιστοιχούν στις επιλογές μας και τους γράφει στο ListBox
    '    GetDrugs()

    '    ' Αναγράφει τιμές και λήξεις για το επιλεγμένο προιόν
    '    DisplayPricesAndExpirations()

    'End Sub




    ' ****************************************************************************************************************************
    ' **********    ΔΙΑΧΕΙΡΗΣΗ ΤΙΜΩΝ   *******************************************************************************************
    ' ****************************************************************************************************************************



    'Private Sub ChangeControlsPrices(ByVal selector As Boolean)
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save

    '    ' Βρίσκει το Name της επιλογής μας από το dgvDrugs
    '    Dim drugName As String = dgvDrugs.CurrentCell.Value.ToString
    '    Dim drugId As Integer = dgvDrugs.Rows(dgvDrugs.SelectedRows(0).Index).Cells("Id").Value

    '    ' Γεμίζει το DatagridView dgvDebts με τα χρωστούμενα του επιλεγμένου πελάτη
    '    stringDTG = "Select Price2, Price1, Notes2, Name, Id, DrugId From PharmacyCustomFiles.dbo.Prices WHERE DrugId = '" & drugId & "' AND (Price1> 0 OR Price2 > 0) "
    '    FillDatagrid(dgvPrices, bsPrices, {"Λιανική", "Χονδρική", "Παρατηρήσεις"}, {80, 80, 297}, {"c", "c", "0"}, {"Id", "DrugId", "Name"})

    '    'Τροποποίηση των άλλων GroupBox
    '    grpDrugName.Enabled = Not selector
    '    grpExpirations.Enabled = Not selector

    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnSavePrices, btnEditPrices, btnDeletePrices}, dgvPrices, selector)

    '    ' Ενεργοποίηση ενημερωτικού flashing label
    '    timerLabel.Visible = selector
    '    tmrFlashLabel.Enabled = selector

    'End Sub




    'Private Sub btnEditPrices_Click(sender As Object, e As EventArgs)

    '    ' Καθορίζει το ενημερωτικό flashing label
    '    timerLabel = lblPricesMessage

    '    'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
    '    If btnEditPrices.Text = "Edit" Then

    '        ChangeControlsPrices(True)

    '        'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
    '    ElseIf btnEditPrices.Text = "Cancel" Then

    '        ChangeControlsPrices(False)

    '    End If

    'End Sub




    'Private Sub btnSavePrices_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
    '    ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    UpdateDatagrid({btnSavePrices, btnEditPrices, btnDeletePrices}, dgvPrices, dgvDrugs.Rows(dgvDrugs.SelectedRows(0).Index).Cells("Id").Value, "DrugId")

    '    ChangeControlsPrices(False)
    'End Sub




    'Private Sub btnDeletePrices_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Delete των δεδομένων του DataGrid 
    '    DeleteDatagrid(dgvPrices)

    '    ' Tροποποίηση των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    ChangeControlsPrices(False)

    'End Sub


    '' Ελέγχει αν έχουν καταχωρηθεί τιμές αριθμητικές στα πεδία τιμών
    'Private Sub dgvPrices_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs)

    '    'If btnEditPrices.Text = "Cancel" Then

    '    Dim headerText As String = dgvPrices.Columns(e.ColumnIndex).HeaderText
    '    '    Dim dc As Decimal

    '    '    ' Ελέγχει αν βρισκόμαστε σε χρηματικό πεδίο
    '    '    If headerText.Equals("Λιανική") Or headerText.Equals("Χονδρική") Then

    '    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι δεκαδική
    '    '        If e.FormattedValue.ToString <> String.Empty And e.FormattedValue.ToString.EndsWith(" €", StringComparison.CurrentCultureIgnoreCase) = False AndAlso Decimal.TryParse(e.FormattedValue.ToString, dc) = False AndAlso btnEditPrices.Text = "Cancel" Then
    '    '            MessageBox.Show("Δεν καταχωρήσατε σωστή χρηματική τιμή", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '    '            e.Cancel = True

    '    '        End If

    '    '    End If
    '    'End If

    '    'Ελέγχει αν βρισκόμαστε σε χρηματικό πεδίο
    '    If headerText.Equals("Λιανική") Then
    '        Try
    '            dgvPrices.Rows(e.RowIndex).Cells("Price1").Value = CType(e.FormattedValue, Decimal) * 2
    '        Catch ex As Exception
    '        End Try

    '    End If

    'End Sub



    'Private Sub dgvPrices_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
    '    Try
    '        ' Clear the row error in case the user presses ESC.   
    '        dgvPrices.Rows(e.RowIndex).ErrorText = String.Empty
    '    Catch ex As Exception

    '    End Try
    'End Sub




    ' ****************************************************************************************************************************
    ' **********    ΔΙΑΧΕΙΡΗΣΗ ΗΜΕΡΟΜΗΝΙΩΝ ΛΗΞΗΣ ΦΑΡΜΑΚΩΝ   **********************************************************************
    ' ****************************************************************************************************************************



    'Private Sub ChangeControlsExpDate(ByVal selector As Boolean)
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save

    '    ' Βρίσκει το Name της επιλογής μας από το dgvDrugs
    '    Dim drugName As String = dgvDrugs.CurrentCell.Value.ToString
    '    Dim drugId As Integer = dgvDrugs.Rows(dgvDrugs.SelectedRows(0).Index).Cells("Id").Value

    '    ' Γεμίζει το DatagridView dgvExpirations με τα ημερομηνίες λήξης του επιλεγμένου φαρμάκου
    '    stringDTG = "Select ExpMonth, ExpYear, Id, DrugId, Name From PharmacyCustomFiles.dbo.Expirations WHERE DrugId = '" & drugId & "' ORDER BY ExpYear,ExpMonth"
    '    FillDatagrid(dgvExpirations, bsExpirations, {"Μήνας", "Έτος"}, {85, 85}, {"##", "####"}, {"Id", "DrugId", "Name"})

    '    'Τροποποίηση των άλλων GroupBox
    '    grpDrugName.Enabled = Not selector
    '    GrpPrices.Enabled = Not selector

    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnSaveExp, btnEditExp, btnDeleteExp}, dgvExpirations, selector)

    '    ' Ενεργοποίηση ενημερωτικού flashing label
    '    timerLabel.Visible = selector
    '    tmrFlashLabel.Enabled = selector

    'End Sub




    'Private Sub btnEditExp_Click(sender As Object, e As EventArgs)
    '    ' Καθορίζει το ενημερωτικό flashing label
    '    timerLabel = lblExpirationsMessage

    '    'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
    '    If btnEditExp.Text = "Edit" Then

    '        ChangeControlsExpDate(True)

    '        'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
    '    ElseIf btnEditExp.Text = "Cancel" Then

    '        ChangeControlsExpDate(False)

    '    End If
    'End Sub




    'Private Sub btnSaveExp_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
    '    ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    UpdateDatagrid({btnSaveExp, btnEditExp, btnDeleteExp}, dgvExpirations, dgvDrugs.Rows(dgvDrugs.SelectedRows(0).Index).Cells("Id").Value, "DrugId")

    '    ChangeControlsExpDate(False)

    'End Sub



    'Private Sub btnDeleteExp_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Delete των δεδομένων του DataGrid 
    '    ' μαζί με τροποποίηση των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    DeleteDatagrid(dgvExpirations)

    '    ChangeControlsExpDate(False)

    'End Sub





    '' Ελέγχει αν η ημερομηνία είοναι σωστή 
    'Private Sub dgvExpirations_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs)

    '    Dim headerText As String = dgvExpirations.Columns(e.ColumnIndex).HeaderText

    '    ' Ελέγχει αν βρισκόμαστε στο πεδίο Μήνας
    '    If headerText.Equals("Μήνας") Then
    '        Dim int As Integer

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι Μήνας
    '        If (e.FormattedValue.ToString <> String.Empty AndAlso Not Integer.TryParse(e.FormattedValue.ToString, int) AndAlso btnEditExp.Text = "Cancel") Then
    '            MessageBox.Show("Το '" & e.FormattedValue.ToString & "' δεν αντιστοιχεί σε μήνα", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True
    '        End If

    '        Try
    '            If CType(e.FormattedValue, Integer) > 12 Or CType(e.FormattedValue, Integer) < 0 Then
    '                MessageBox.Show("Το '" & e.FormattedValue.ToString & "' δεν αντιστοιχεί σε μήνα", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '                e.Cancel = True
    '            End If
    '        Catch ex As Exception
    '        End Try

    '        ' Ελέγχει αν βρισκόμαστε στο πεδίο Έτος
    '    ElseIf headerText.Equals("Έτος") Then
    '        Dim int As Integer

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι Έτος
    '        If e.FormattedValue.ToString <> String.Empty AndAlso Not Integer.TryParse(e.FormattedValue.ToString, int) AndAlso btnEditExp.Text = "Cancel" Then

    '            MessageBox.Show("Το '" & e.FormattedValue.ToString & "' δεν αντιστοιχεί σε έγκυρο έτος", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True

    '        End If
    '        Try
    '            If CType(e.FormattedValue, Integer) < Year(Today) Then
    '                MessageBox.Show("Το '" & e.FormattedValue.ToString & "' δεν αντιστοιχεί σε έγκυρο έτος", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '                e.Cancel = True
    '            End If
    '        Catch ex As Exception
    '        End Try
    '    End If
    'End Sub



    Private Function CheckIfValidExpDate(value As String) As Boolean
        Dim firstPart As String = ""
        Dim secondPart As String = ""
        Dim separator() As String = {"/", "-"}
        Dim dt As DateTime

        Try
            For t = 0 To separator.Length - 1
                If value.IndexOf(separator(t)) <> 0 Then
                    firstPart = value.Substring(0, value.IndexOf(separator(t)))
                    secondPart = value.Substring((value.IndexOf(separator(t)) + 1), (value.Length - value.IndexOf(separator(t)) - 1))
                End If
            Next
        Catch ex As Exception

        End Try

        If firstPart = "" Or secondPart = "" Then Return False

        Dim myDate As DateTime = "1/" & firstPart & "/" & secondPart

        If DateTime.TryParse(myDate, dt) Then
            Return True
        End If

        Return False
        'MsgBox(value & ": / has index " & value.IndexOf("/"))
        'MsgBox(value.Substring(0, value.IndexOf("/")))
        'MsgBox(value.Substring((value.IndexOf("/") + 1), (value.Length - value.IndexOf("/") - 1)))
        ''MsgBox(firstPart & " - " & secondPart)
    End Function


    'Private Sub dgvExpirations_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
    '    Try
    '        ' Clear the row error in case the user presses ESC.   
    '        dgvExpirations.Rows(e.RowIndex).ErrorText = String.Empty
    '    Catch ex As Exception

    '    End Try
    'End Sub




    'Private Sub DisplayIncomingExpirations(ByVal months As Integer)

    '    Dim sqlString As String = "SELECT Drugs.Name, DateDiff(month, getdate(), concat(convert(varchar(4),ExpYear), '-',convert(varchar(2),ExpMonth), '-01')) as Duration, " & _
    '                                        "concat(convert(varchar(2),ExpMonth),'-',convert(varchar(4),ExpYear)) as ExpDate3 " & _
    '                                "FROM Drugs INNER JOIN Expirations ON Drugs.Id = Expirations.DrugId " & _
    '                                "WHERE DateDiff(month, getdate(), concat(convert(varchar(4),ExpYear), '-',convert(varchar(2),ExpMonth), '-01')) < " & months & " "

    '    ' Γράφει μια ενημερωση στο Label κάτω από το dgvExpirations
    '    ' ανάλογα με τον αριθμό των φαρμάκων που λήγουν σύντομα
    '    Select Case GetIncomingExpirations(sqlString).Length
    '        Case 0
    '            rtxtExpirMessage.Text = "Δεν βρέθηκαν προιόντα που λήγουν μέσα στoυς επόμενους " & months & " μήνες"

    '            'Εξαφανίζει την ερώτηση για να παρουσιάσει την λίστα των ληγμένων φαρμάκων
    '            rtxtQuestion.Visible = False
    '            btnDisplayExpirDrugsList.Visible = False

    '        Case 1
    '            rtxtExpirMessage.Text = "Βρέθηκε 1 προιόν που λήγει μέσα στoυς επόμενους " & months & " μήνες"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtExpirMessage, {"1"})

    '        Case Is > 1
    '            Dim numExpirDrugs As Integer = GetIncomingExpirations(sqlString).Length
    '            rtxtExpirMessage.Text = "Βρέθηκαν " & numExpirDrugs & " προιόντα που λήγουν μέσα στoυς επόμενους " & months & " μήνες"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtExpirMessage, {numExpirDrugs.ToString})

    '            'Εμφανίζει την ερώτηση για να παρουσιάσει την λίστα των ληγμένων φαρμάκων
    '            rtxtQuestion.Visible = True
    '            btnDisplayExpirDrugsList.Visible = True
    '    End Select

    'End Sub



    Private Function GetIncomingExpirations(ByVal sqlString As String) As Array
        Dim Expirations() As String
        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlString, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                Dim t As Integer = 0

                If myReader.HasRows Then

                    Do While myReader.Read()

                        ReDim Preserve Expirations(t)
                        Expirations(t) = myReader("Name")
                        t += 1
                    Loop
                Else

                End If

                ReDim Preserve Expirations(t)
                Return Expirations
            End Using
        End Using

    End Function




    Private Sub btnDisplayExpirDrugsList_Click(sender As Object, e As EventArgs)
        frmExpirDrugsList.Show()
    End Sub


    Private Sub btnEditDrugsNew_Click(sender As Object, e As EventArgs)

        ' ΠΡΩΤΑ απενεργοποιείται το TabControl
        tbcMain.Enabled = False

        ' META εμφανίζεται το frm με την λίστα των φαρμάκων προς διόρθωση
        frmDrugListEdit.Show()

    End Sub





    ' *******************************************************************
    '********************************************************************
    '                TAB 3 
    '********************************************************************
    '********************************************************************





    ' ****************************************************************************************************************************
    ' **********    ΔΙΑΧΕΙΡΗΣΗ ΛΙΣΤΑΣ Ζ    *******************************************************************************
    ' ****************************************************************************************************************************


    'Private Sub GetZList()

    '    'Μεταβλητές 
    '    Dim MonthToSearch As String = ""
    '    Dim YearToSearch As String = ""


    '    'Δημιουργία SQL string
    '    If cboZMonth.Text = "Όλοι οι μήνες" And cboZYear.Text = "Όλα τα έτη" Then

    '        MonthToSearch = "LIKE '%%'"
    '        YearToSearch = "LIKE '%%'"

    '    ElseIf cboZMonth.Text = "Όλοι οι μήνες" And cboZYear.Text <> "Όλα τα έτη" Then

    '        MonthToSearch = "LIKE '%%'"
    '        YearToSearch = "= '" & cboZYear.Text & "'"

    '    ElseIf cboZMonth.Text <> "Όλοι οι μήνες" And cboZYear.Text <> "Όλα τα έτη" Then

    '        MonthToSearch = "= '" & GetNumericMonthFromName(cboZMonth.Text) & "'"
    '        YearToSearch = "= '" & cboZYear.Text & "'"
    '    Else

    '        MonthToSearch = "LIKE '%%'"
    '        YearToSearch = "LIKE '%%'"

    '    End If

    '    stringDTG = "SELECT ZId, ZDate, Drugs650Total, Drugs230Total, Drugs130Total, Items, Receipts, Id From PharmacyCustomFiles.dbo.Z_List " & _
    '        "WHERE YEAR(ZDate) " & YearToSearch & " AND MONTH(ZDate) " & MonthToSearch & " " & _
    '                           "ORDER BY ZDate"

    '    ' Γεμίζει το dgv με τα Z και 
    '    ' κρατάει στην μεταβλητή sumΖ τον συνολικό αριθμό τους
    '    Dim sumZ As Integer = FillDatagrid(dgvZlist, bsZList, {"#", "Ημερομηνία", "6%", "23%", "13%", "Τεμ", "Αποδ"},
    '                        {30, 80, 65, 65, 65, 35, 35}, {"0", "dd-MM-yyyy", "c", "c", "c", "0", "0"}, {"Id"})
    '    'Dim sumZ As Integer = FillDatagrid(dgvZlist, bsZList, {"#", "Ημερομηνία", "Εισπρ 6%", "Εισπρ 23%", "Εισπρ 13%", "Τεμάχια", "Αποδείξεις"},
    '    '{50, 80, 95, 90, 90, 80, 80}, {"0", "dd-MM-yyyy", "c", "c", "c", "0", "0"}, {"Id"})

    '    ' Φτιάχνει τα alignment για όλα τα πεδία
    '    For t = 0 To 6
    '        dgvZlist.Columns(t).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        dgvZlist.Columns(t).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '    Next

    '    ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Select Case sumZ
    '        Case 0

    '            rtxtZlistMessage.Text = "H λίστα των Ζ δεν περιέχει εγγραφές"

    '        Case 1

    '            rtxtZlistMessage.Text = "H λίστα των Ζ περιέχει 1 εγγραφή"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtZlistMessage, {"1"})

    '        Case Is > 1

    '            rtxtZlistMessage.Text = "H λίστα των Ζ περιέχει " & sumZ.ToString & " εγγραφές"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtZlistMessage, {sumZ.ToString})

    '    End Select

    '    ' Select last row
    '    dgvZlist.MultiSelect = False
    '    dgvZlist.ClearSelection()
    '    dgvZlist.Rows(dgvZlist.Rows.GetLastRow(0)).Cells(0).Selected = True

    'End Sub


    'Private Sub DisplayDetailsCurrentSelectionZ()
    '    ' 0 -> 6.5%, 
    '    ' 1 -> 13%
    '    ' 2 -> 26%

    '    'Ορισμός μεταβλητών
    '    Dim total650, total130, total230 As Decimal
    '    Dim dateZ As Date, id As Integer, receipts As Integer, items As Integer
    '    Dim taxTotal650, taxTotal130, taxTotal230 As Decimal
    '    Dim tax650, tax130, tax230 As Decimal


    '    'Παίρνει τις τιμές ανα κατηγορία από το CurrentRow
    '    Try
    '        total650 = dgvZlist.Item("Drugs650Total", dgvZlist.CurrentRow.Index).Value()
    '        total130 = dgvZlist.Item("Drugs130Total", dgvZlist.CurrentRow.Index).Value()
    '        total230 = dgvZlist.Item("Drugs230Total", dgvZlist.CurrentRow.Index).Value()

    '        dateZ = dgvZlist.Item("ZDate", dgvZlist.CurrentRow.Index).Value()
    '        id = dgvZlist.Item("ZId", dgvZlist.CurrentRow.Index).Value()
    '        receipts = dgvZlist.Item("Receipts", dgvZlist.CurrentRow.Index).Value()
    '        items = CType(dgvZlist.Item("Items", dgvZlist.CurrentRow.Index).Value(), Integer)
    '    Catch ex As Exception
    '    End Try

    '    'Υπολογίζει τα φορολογητέα ποσά
    '    taxTotal650 = total650 / (1.065)
    '    taxTotal130 = total130 / (1.13)
    '    taxTotal230 = total230 / (1.23)

    '    'Υπολογίζει το ΦΠΑ
    '    tax650 = taxTotal650 * (0.065)
    '    tax130 = taxTotal130 * (0.13)
    '    tax230 = taxTotal230 * (0.23)

    '    ' Αναγράφει τα ΦΠΑ στα αντίστοιχα label
    '    If (total650 + total130 + total230) <> 0 Then

    '        lblA1.Text = total650.ToString("#,##0.00 €")
    '        lblA2.Text = taxTotal650.ToString("#,##0.00 €")
    '        lblA3.Text = tax650.ToString("#,##0.00 €")
    '        lblB1.Text = total130.ToString("#,##0.00 €")
    '        lblB2.Text = taxTotal130.ToString("#,##0.00 €")
    '        lblB3.Text = tax130.ToString("#,##0.00 €")
    '        lblc1.Text = total650.ToString("#,##0.00 €")
    '        lblC2.Text = taxTotal130.ToString("#,##0.00 €")
    '        lblC3.Text = tax230.ToString("#,##0.00 €")

    '        lblTotal1.Text = (total650 + total130 + total230).ToString("#,##0.00 €")
    '        lblTotal2.Text = (taxTotal650 + taxTotal130 + taxTotal230).ToString("#,##0.00 €")
    '        lblTotal3.Text = (tax650 + tax130 + tax230).ToString("#,##0.00 €")

    '        lblItems.Text = items.ToString("###,###,###")
    '        lblReceipts.Text = receipts.ToString("###,###")
    '        lblDateSelection.Text = dateZ.ToString("dddd dd-MM-yyyy")

    '    End If

    'End Sub




    'Private Sub btnEditZList_Click(sender As Object, e As EventArgs)
    '    ' Καθορίζει το ενημερωτικό flashing label
    '    timerLabel = lblZListMessage

    '    'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
    '    If btnEditZList.Text = "Edit" Then

    '        ChangeControlsZListEdit(True)

    '        'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
    '    ElseIf btnEditZList.Text = "Cancel" Then

    '        ChangeControlsZListEdit(False)

    '    End If

    'End Sub



    'Private Sub ChangeControlsZListEdit(ByVal selector As Boolean)
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save

    '    ' Γεμίζει το DatagridView dgvDebts με τα χρωστούμενα του επιλεγμένου πελάτη
    '    stringDTG = "SELECT ZId, ZDate, Drugs650Total, Drugs230Total, Drugs130Total, Items, Receipts, Id From PharmacyCustomFiles.dbo.Z_List " & _
    '                "ORDER BY ZDate"
    '    FillDatagrid(dgvZlist, bsZList, {"#", "Ημερομηνία", "6%", "23%", "13%", "Τεμ", "Αποδ"},
    '                        {30, 80, 65, 65, 65, 35, 35}, {"0", "dd-MM-yyyy", "c", "c", "c", "0", "0"}, {"Id"})

    '    ' Φτιάχνει τα alignment για όλα τα πεδία
    '    For t = 0 To 5
    '        dgvZlist.Columns(t).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        dgvZlist.Columns(t).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '    Next

    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnSaveZList, btnEditZList, btnDeleteZList}, dgvZlist, selector)

    '    ' Ενεργοποίηση ενημερωτικού flashing label
    '    timerLabel.Visible = selector
    '    tmrFlashLabel.Enabled = selector

    'End Sub



    'Private Sub btnSaveZList_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
    '    ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    UpdateDatagrid({btnSaveZList, btnEditZList, btnDeleteZList}, dgvZlist)

    '    ChangeControlsZListEdit(False)

    '    ' ανανέωσε τη λίστα των Ζ
    '    GetZList()

    'End Sub



    'Private Sub btnDeleteZList_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Delete των δεδομένων του DataGrid 
    '    ' μαζί με τροποποίηση των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    DeleteDatagrid(dgvZlist)


    '    ' Τροποποιεί τα Controls της τρέχουσας form
    '    ChangeControlsZListEdit(False)

    '    ' ανανέωσε τη λίστα των Ζ
    '    GetZList()

    'End Sub



    'Private Sub dgvZlist_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
    '    Try
    '        ' Clear the row error in case the user presses ESC.   
    '        dgvZlist.Rows(e.RowIndex).ErrorText = String.Empty
    '    Catch ex As Exception

    '    End Try
    'End Sub

    'Private Sub dgvZlist_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs)

    '    ' Κρατάει στη μνήμη τη παλιά και τη νέα τιμή του κελιού
    '    Dim oldValue = dgvZlist(e.ColumnIndex, e.RowIndex).Value.ToString
    '    Dim newValue = e.FormattedValue
    '    Dim headerText As String = dgvZlist.Columns(e.ColumnIndex).HeaderText

    '    ' Ελέγχει αν βρισκόμαστε στο πεδίο Ημερομηνία
    '    If headerText.Equals("Ημερομηνία") Then
    '        Dim dt As DateTime

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι ημερομηνία
    '        If e.FormattedValue.ToString <> String.Empty AndAlso Not DateTime.TryParse(e.FormattedValue.ToString, dt) AndAlso btnEditZList.Text = "Cancel" Then
    '            MessageBox.Show("Το '" & e.FormattedValue & "' δεν είναι ημερομηνία!", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

    '            'Εμποδίζει την έξοδο από το κελί
    '            e.Cancel = True
    '            Exit Sub

    '        End If

    '    End If

    '    ' Ελέγχει αν βρισκόμαστε σε χρηματικό πεδίο
    '    If headerText.Equals("Σύνολο 6%") Or headerText.Equals("Σύνολο 23%") Or headerText.Equals("Σύνολο 13%") AndAlso btnEditZList.Text = "Cancel" Then
    '        Dim dc As Decimal

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι δεκαδική
    '        If e.FormattedValue.ToString <> String.Empty AndAlso Decimal.TryParse(e.FormattedValue.ToString, dc) = False Then

    '            'Μύνημα λάθους
    '            MessageBox.Show("Το '" & e.FormattedValue & "' δεν είναι χρηματικό ποσό !", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

    '            'Εμποδίζει την έξοδο από το κελί
    '            e.Cancel = True
    '        End If


    '        ' Αν το πεδίο αποκτήσει τιμή που είναι αρνητική
    '        If CType(e.FormattedValue, Decimal) < 0 Then

    '            'Μύνημα λάθους
    '            MessageBox.Show("Το ποσό δεν μπορεί να είναι αρνητικό !", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            'Εμποδίζει την έξοδο από το κελί
    '            e.Cancel = True

    '        End If

    '    End If

    '    ' Ελέγχει αν βρισκόμαστε σε αριθμητικό πεδίο
    '    If headerText.Equals("#") Or headerText.Equals("Τεμάχια") Then
    '        Dim int As Integer
    '        Dim mySQL As String = "Select * From PharmacyCustomFiles.dbo.Z_List WHERE ZId ='" & newValue & "'"

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι δεκαδική
    '        If e.FormattedValue.ToString <> String.Empty AndAlso Integer.TryParse(e.FormattedValue.ToString, int) = False AndAlso btnEditZList.Text = "Cancel" Then

    '            'Μύνημα λάθους
    '            MessageBox.Show("Το '" & e.FormattedValue & "' δεν είναι ακέραιος αριθμός!", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

    '            'Εμποδίζει την έξοδο από το κελί
    '            e.Cancel = True

    '        End If

    '        'Αν η παλιά τιμή είναι διαφορετική από τη νέα ΚΑΙ η νέα τιμή υπάρχει ήδη στο database
    '        If headerText.Equals("#") AndAlso oldValue <> newValue AndAlso IsItAllreadyThere(mySQL) = True AndAlso btnEditZList.Text = "Cancel" Then

    '            'Μύνημα λάθους
    '            MessageBox.Show("Υπάρχει ήδη Ζ με # " & newValue & " !", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

    '            'Εμποδίζει την έξοδο από το κελί
    '            e.Cancel = True

    '        End If

    '    End If

    'End Sub


    'Private Sub dgvZlist_CellClick(sender As Object, e As DataGridViewCellEventArgs)
    '    DisplayDetailsCurrentSelectionZ()
    '    CalculateZTotalsForPeriod()
    'End Sub



    ''Υπολογίζει τα σύνολα Ζ για τη συγκεκριμένη περίοδο
    'Private Sub CalculateZTotalsForPeriod()
    '    ' Δήλωση μεταβλητών
    '    Dim sqlString As String = ""
    '    Dim items, receipts As Integer
    '    Dim total650, total130, total230 As Decimal
    '    Dim taxTotal650, taxTotal130, taxTotal230 As Decimal
    '    Dim tax650, tax130, tax230 As Decimal
    '    Dim MonthToSearch As String = ""
    '    Dim YearToSearch As String = ""

    '    'Δημιουργία SQL string
    '    If cboZMonth.Text = "Όλοι οι μήνες" And cboZYear.Text = "Όλα τα έτη" Then

    '        MonthToSearch = "LIKE '%%'"
    '        YearToSearch = "LIKE '%%'"

    '        lblDateZSelectionTotal.Text = "Σύνολα όλων των ετών"

    '    ElseIf cboZMonth.Text = "Όλοι οι μήνες" And cboZYear.Text <> "Όλα τα έτη" Then

    '        MonthToSearch = "LIKE '%%'"
    '        YearToSearch = "= '" & cboZYear.Text & "'"

    '        lblDateZSelectionTotal.Text = "Σύνολα έτους " & cboZYear.Text

    '    ElseIf cboZMonth.Text <> "Όλοι οι μήνες" And cboZYear.Text <> "Όλα τα έτη" Then

    '        MonthToSearch = "= '" & GetNumericMonthFromName(cboZMonth.Text) & "'"
    '        YearToSearch = "= '" & cboZYear.Text & "'"

    '        lblDateZSelectionTotal.Text = "Σύνολα για " & GetNumericMonthFromName(cboZMonth.Text) & "/" & cboZYear.Text

    '    Else

    '        MonthToSearch = "LIKE '%%'"
    '        YearToSearch = "LIKE '%%'"

    '        lblDateZSelectionTotal.Text = "Σύνολα όλων των ετών"

    '    End If

    '    sqlString = "SELECT * From PharmacyCustomFiles.dbo.Z_List " & _
    '                "WHERE YEAR(ZDate) " & YearToSearch & " AND MONTH(ZDate) " & MonthToSearch & " "


    '    'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
    '    Using con As New SqlClient.SqlConnection(connectionString)

    '        'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
    '        Using cmd As New SqlClient.SqlCommand(sqlString, con)

    '            ' Ανοίγει την σύνδεση
    '            con.Open()

    '            'Ορισμός ExecuteReader 
    '            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

    '            If myReader.HasRows Then
    '                Do While myReader.Read()

    '                    'Υπολογίζει τα συνολικά ποσά
    '                    total650 += myReader("Drugs650Total")
    '                    total130 += myReader("Drugs130Total")
    '                    total230 += myReader("Drugs230Total")

    '                    'Υπολογίζει τα τεμάχια και τις αποδείξεις
    '                    items += myReader("Items")
    '                    receipts += myReader("Receipts")

    '                Loop
    '            End If

    '        End Using

    '        'Υπολογίζει τα φορολογητέα ποσά
    '        taxTotal650 = total650 / (1.065)
    '        taxTotal130 = total130 / (1.13)
    '        taxTotal230 = total230 / (1.23)

    '        'Υπολογίζει το ΦΠΑ
    '        tax650 = taxTotal650 * (0.065)
    '        tax130 = taxTotal130 * (0.13)
    '        tax230 = taxTotal230 * (0.23)

    '        ' Αναγράφει τα ΦΠΑ στα αντίστοιχα label
    '        If (total650 + total130 + total230) <> 0 Then

    '            lblA1Total.Text = total650.ToString("#,##0.00 €")
    '            lblA2Total.Text = taxTotal650.ToString("#,##0.00 €")
    '            lblA3Total.Text = tax650.ToString("#,##0.00 €")
    '            lblB1Total.Text = total130.ToString("#,##0.00 €")
    '            lblB2Total.Text = taxTotal130.ToString("#,##0.00 €")
    '            lblB3Total.Text = tax130.ToString("#,##0.00 €")
    '            lblC1Total.Text = total230.ToString("#,##0.00 €")
    '            lblC2Total.Text = taxTotal230.ToString("#,##0.00 €")
    '            lblC3Total.Text = tax230.ToString("#,##0.00 €")

    '            lblTotal1Total.Text = (total650 + total130 + total230).ToString("#,##0.00 €")
    '            lblTotal2Total.Text = (taxTotal650 + taxTotal130 + taxTotal230).ToString("#,##0.00 €")
    '            lblTotal3Total.Text = (tax650 + tax130 + tax230).ToString("#,##0.00 €")

    '            lblItemsTotal.Text = items.ToString("###,###,###")
    '            lblReceiptsTotal.Text = receipts.ToString("###,###,###")

    '        End If

    '    End Using



    'End Sub

    '' Κάθε φορά που κινούμαστε στο datagrid με τα arrows Up & Down...
    'Private Sub dgvZlist_KeyDown(sender As Object, e As KeyEventArgs)

    '    DisplayDetailsCurrentSelectionZ()
    '    CalculateZTotalsForPeriod()
    'End Sub

    'Private Sub cboZYear_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    GetZList()
    '    CalculateZTotalsForPeriod()
    'End Sub


    'Private Sub dgvPrices_CellClick(sender As Object, e As DataGridViewCellEventArgs)
    '    GrpPrices.Text = "Name:" & dgvPrices.SelectedRows(0).Cells("Name").Value
    'End Sub


    'Private Sub dgvExpirations_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)
    '    grpExpirations.Text = "Name:" & dgvExpirations.SelectedRows(0).Cells("Name").Value
    'End Sub

    'Private Sub btnTest_Click(sender As Object, e As EventArgs)
    '    If CheckIfValidExpDate(txtTest.Text) Then
    '        MsgBox(txtTest.Text & "= OK")
    '    Else
    '        MsgBox(txtTest.Text & "= FAILED")
    '    End If
    'End Sub



    'Private Sub cboZMonth_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    GetZList()
    '    CalculateZTotalsForPeriod()
    'End Sub




    ' *******************************************************************
    '********************************************************************
    '                TAB 4 
    '********************************************************************
    '********************************************************************





    ' ****************************************************************************************************************************
    ' **********    ΔΙΑΧΕΙΡΗΣΗ ΑΡΧΕΙΟΥ ΠΑΡΑΓΓΕΛΙΩΝ    *******************************************************************************
    ' ****************************************************************************************************************************


    'Private Sub GetDeliveryList()

    '    ' Με βάση το μέρος ονόματος του πελάτη (textbox) και τις επιλογές μας στo ComboBox
    '    ' βρίσκει όλους τους πελάτες..

    '    stringDTG = "SELECT DeliveryCode, DeliveryDate, Id From PharmacyCustomFiles.dbo.DeliveryList " & _
    '                "WHERE DeliveryCode like '%" & txtDeliveriesList.Text & "%' " & _
    '                "OR DeliveryDate like '%" & txtDeliveriesList.Text & "%'" & _
    '                "ORDER BY DeliveryDate DESC"


    '    ' Γεμίζει το lstCustomers με τους πελάτες που αντιστοιχούν στις επιλογές μας και 
    '    ' κρατάει στην μεταβλητή sumCustomers τον συνολικό αριθμό πελατών
    '    Dim sumDeliveries As Integer = FillDatagrid(dgvDeliveriesList, bsDeliveriesList, {"Κωδικός", "Ημερομηνία"}, {68, 80}, {"0", "d"}, {"Id"})

    '    ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Select Case sumDeliveries
    '        Case 0

    '            rtxtDeliveriesList.Text = "Δεν βρέθηκαν παραγγελίες"

    '        Case 1

    '            rtxtDeliveriesList.Text = "Βρέθηκε 1 παραγγελία"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtDeliveriesList, {"1"})

    '        Case Is > 1

    '            rtxtDeliveriesList.Text = "Βρέθηκαν " & sumDeliveries.ToString & " παραγγελίες"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtDeliveriesList, {sumDeliveries.ToString})

    '    End Select

    'End Sub




    'Private Sub txtDeliveriesList_TextChanged(sender As Object, e As EventArgs)
    '    GetDeliveryList()
    'End Sub




    'Private Sub btnEditDeliveriesList_Click(sender As Object, e As EventArgs)
    '    ' Καθορίζει το ενημερωτικό flashing label
    '    timerLabel = lblDeliveriesListMessage

    '    'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
    '    If btnEditDeliveriesList.Text = "Edit" Then

    '        ChangeControlsDeliveriesList(True)

    '        'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
    '    ElseIf btnEditDeliveriesList.Text = "Cancel" Then

    '        ChangeControlsDeliveriesList(False)

    '    End If

    'End Sub



    'Private Function ChangeControlsDeliveriesList(ByVal selector As Boolean) As Integer
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save

    '    Dim No As Integer = 0

    '    ' Γεμίζει το DatagridView dgvDebts με τα χρωστούμενα του επιλεγμένου πελάτη
    '    stringDTG = "SELECT DeliveryCode, DeliveryDate, Id From PharmacyCustomFiles.dbo.DeliveryList " & _
    '               "WHERE DeliveryCode like '%" & txtDeliveriesList.Text & "%' " & _
    '               "OR DeliveryDate like '%" & txtDeliveriesList.Text & "%' " & _
    '               "ORDER BY DeliveryDate DESC"
    '    No = FillDatagrid(dgvDeliveriesList, bsDeliveriesList, {"Κωδικός", "Ημερομηνία"}, {68, 80}, {"0", "d"}, {"Id"})

    '    'Τροποποίηση των άλλων GroupBox της σελίδας
    '    grpDrugsPerDelivery.Enabled = Not selector

    '    'Τροποποίηση των άλλων tab pages
    '    tbpCustomers.Enabled = Not selector
    '    tbpDrugs.Enabled = Not selector
    '    tbpZList.Enabled = Not selector

    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnSaveDeliveriesList, btnEditDeliveriesList, btnDeleteDeliveriesList}, dgvDeliveriesList, selector)

    '    ' Ενεργοποίηση ενημερωτικού flashing label
    '    timerLabel.Visible = selector
    '    tmrFlashLabel.Enabled = selector

    '    Return No

    'End Function



    'Private Sub btnSaveDeliveriesList_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
    '    ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    UpdateDatagrid({btnSaveDeliveriesList, btnEditDeliveriesList, btnDeleteDeliveriesList}, dgvDeliveriesList)

    '    ChangeControlsDeliveriesList(False)

    '    dgvDeliveriesList.Refresh()

    'End Sub



    'Private Sub btnDeleteDeliveriesList_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Delete των δεδομένων του DataGrid 
    '    ' μαζί με τροποποίηση των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    DeleteDatagrid(dgvDeliveriesList)

    '    ChangeControlsDeliveriesList(False)

    'End Sub





    'Private Sub dgvDeliveriesList_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs)
    '    Dim headerText As String = dgvDeliveriesList.Columns(e.ColumnIndex).HeaderText
    '    Dim oldValue = dgvDeliveriesList(e.ColumnIndex, e.RowIndex).Value.ToString
    '    Dim newValue = e.FormattedValue.ToString

    '    ' Ελέγχει αν βρισκόμαστε στο πεδίο Ημερομηνία
    '    If headerText.Equals("Ημερομηνία") Then
    '        Dim dt As DateTime

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι ημερομηνία
    '        If newValue <> String.Empty AndAlso Not DateTime.TryParse(newValue, dt) AndAlso btnEditDeliveriesList.Text = "Cancel" Then
    '            MessageBox.Show("Λάθος καταχώρηση ημερομηνίας", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True

    '        End If
    '        ' Ελέγχει αν βρισκόμαστε υπάρχει ήδη ο κωδικός
    '    ElseIf headerText.Equals("Κωδικός") Then
    '        'Dim val As Integer = Asc("β")
    '        'Dim val2 As Integer = Asc("b")
    '        'Dim val3 As Integer = Asc("B")
    '        'Dim val4 As Integer = Asc("Β")

    '        If newValue <> String.Empty AndAlso oldValue <> newValue AndAlso Asc(newValue.Substring(0, 1)) > 122 _
    '          AndAlso btnEditDeliveriesList.Text = "Cancel" Then
    '            'Μύνημα λάθους
    '            MessageBox.Show("O # " & newValue & " δεν είναι έγκυρος!" & vbCrLf & "Μήπως περιέχει ελληνικά γράμματα;", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True
    '        End If
    '        If newValue <> String.Empty AndAlso oldValue <> newValue AndAlso IsItAllreadyThere("SELECT * FROM PharmacyCustomFiles.dbo.DeliveryList WHERE DeliveryCode LIKE '%" & newValue.Substring(1, newValue.Length - 1) & "'") = True _
    '         AndAlso btnEditDeliveriesList.Text = "Cancel" Then
    '            'Μύνημα λάθους
    '            MessageBox.Show("Υπάρχει ήδη o κωδικός # " & newValue & " !", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True
    '        End If
    '    End If
    '    'MsgBox("old: " & oldValue & " - New: " & newValue)
    'End Sub



    'Private Sub dgvDeliveriesList_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
    '    Try
    '        ' Clear the row error in case the user presses ESC.   
    '        dgvDeliveriesList.Rows(e.RowIndex).ErrorText = String.Empty
    '    Catch ex As Exception
    '    End Try

    'End Sub


    'Private Function DisplayDrugsPerDeliveryWithControls(ByVal oDataGridView As DataGridView, ByVal oBindingSource As BindingSource, _
    '                              ByVal columnName() As String, ByVal columnWidth() As Integer, ByVal columnFormat() _
    '                              As String) As Integer

    '    'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
    '    con = New SqlConnection(connectionString)

    '    'που την ανοίγει εδώ
    '    con.Open()

    '    'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
    '    cmdDTG = New SqlCommand(stringDTG, con)
    '    daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
    '    cbDTG = New SqlCommandBuilder(daDTG)

    '    dsDTG = New DataSet

    '    'Αδειάζει το datagridView
    '    oDataGridView.Columns.Clear()

    '    ' και ο SqlDataAdapter γεμίζει το Dataset
    '    daDTG.Fill(dsDTG, "DTG")
    '    ' το οποίο γεμίζει το datatable
    '    dtDTG = dsDTG.Tables("DTG")

    '    'Καθορίζει το source του BindingSource ως το Datatable
    '    oBindingSource.DataSource = dtDTG

    '    'Κλείνει την σύνδεση
    '    con.Close()

    '    If rbDeliveryBarcode.Checked = True Then

    '        'Καθορίζει το  source του DataGrid ως το BindingSource
    '        oDataGridView.DataSource = oBindingSource

    '        ' Εναλλαγή του χρωματισμού των rows
    '        oDataGridView.RowsDefaultCellStyle.BackColor = Color.Bisque
    '        oDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

    '    ElseIf rbDeliveryName.Checked = True Then

    '        'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
    '        oDataGridView.AutoGenerateColumns = False

    '        'Καθορίζει το  source του DataGrid ως το BindingSource
    '        oDataGridView.DataSource = oBindingSource


    '        '"SELECT apotikh.AP_DESCRIPTION, APOTIKH.AP_MORFI, count(APOTIKH.AP_DESCRIPTION) as TotalCount, APOTIKH_BARCODES.BRAP_AP_BARCODE, ExpirationsNew.DeliveryCode, " & _
    '        '               "ExpirationsNew.DeliveryDate, ExpirationsNew.DrugId " &

    '        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
    '        Dim Description As New DataGridViewTextBoxColumn
    '        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
    '        Description.DataPropertyName = "AP_DESCRIPTION"

    '        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
    '        Dim Morfi As New DataGridViewTextBoxColumn
    '        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
    '        Morfi.DataPropertyName = "AP_MORFI"

    '        'Όρίζει το 3ο πεδίο του Datagrid σαν textbox
    '        Dim TotCount As New DataGridViewTextBoxColumn
    '        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
    '        TotCount.DataPropertyName = "AP_MORFI"



    '        'Όρίζει το 1ο πεδίο του Datagrid σαν combobox
    '        Dim Combo As New DataGridViewComboBoxColumn
    '        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
    '        Combo.DataPropertyName = "AP_DESCRIPTION"

    '        'Παίρνει όλες τις πιθανές τιμές του Category και τις προσθέτει σαν επιλογές στο combobox
    '        For t = 0 To DrugList.Length - 1
    '            If Not (DrugList(t) Is Nothing) Then Combo.Items.Add(DrugList(t))
    '        Next

    '        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
    '        Dim Id As New DataGridViewTextBoxColumn
    '        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
    '        Id.DataPropertyName = "Id"

    '        'Όρίζει το 3ο πεδίο του Datagrid σαν textbox
    '        Dim DeliveryId As New DataGridViewTextBoxColumn
    '        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
    '        Id.DataPropertyName = "DeliveryId"

    '        'Εμφανίζει τα columns του Datagrid
    '        oDataGridView.Columns.Add(Combo)
    '        oDataGridView.Columns.Add(Id)
    '        oDataGridView.Columns.Add(DeliveryId)

    '        ' Εναλλαγή του χρωματισμού των rows
    '        oDataGridView.RowsDefaultCellStyle.BackColor = Color.Bisque
    '        oDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

    '        'Εξαφανίζει τo πεδίο "Id"
    '        oDataGridView.Columns(1).Visible = False
    '        oDataGridView.Columns(2).Visible = False

    '        ' Μορφοποιεί τα columns
    '        For t = 0 To columnName.Length - 1

    '            oDataGridView.Columns(t).HeaderText = columnName(t) 'Βάζει τίτλο σε κάθε Column

    '            oDataGridView.Columns(t).Width = columnWidth(t) ' Αλλάζει το φάρδος του κάθε Column

    '            oDataGridView.Columns(t).DefaultCellStyle.Format = columnFormat(t)  ' Formatαρισμα των στοιχείων
    '        Next

    '    End If

    '    'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
    '    Return dsDTG.Tables(0).Rows.Count

    'End Function


    'Private Sub LoadDatagridWithDrugsPerDelivery()

    '    ' Βρίσκει το Id της επιλογής μας από το dgvDeliveriesList
    '    Dim DeliveryId As Integer = dgvDeliveriesList.Rows(dgvDeliveriesList.SelectedRows(0).Index).Cells("Id").Value

    '    If rbDeliveryBarcode.Checked = True Then

    '        'DrugList = GetDistinctContentsDBField("SELECT APOTIKH_BARCODES.BRAP_AP_BARCODE, concat(APOTIKH.AP_DESCRIPTION, ' ', APOTIKH.AP_MORFI) as Name,  APOTIKH.AP_CODE " & _
    '        '      "FROM APOTIKH INNER JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID " & _
    '        '      "ORDER BY APOTIKH.AP_DESCRIPTION", "Name")

    '        ' Γεμίζει το DatagridView 
    '        'stringDTG = "SELECT APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, count(APOTIKH.AP_DESCRIPTION) as TotalCount " & _
    '        '            "FROM APOTIKH INNER JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID INNER JOIN ExpirationsNew ON APOTIKH.AP_ID = ExpirationsNew.DrugId " & _
    '        '            "WHERE ExpirationsNew.DeliveryCode = '" & DeliveryId & "' " & _
    '        '            "GROUP BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, TotalCount"
    '        stringDTG = "SELECT apotikh.AP_DESCRIPTION, APOTIKH.AP_MORFI, count(APOTIKH.AP_DESCRIPTION) as TotalCount, APOTIKH_BARCODES.BRAP_AP_BARCODE, ExpirationsNew.DeliveryCode, " & _
    '                       "ExpirationsNew.DeliveryDate, ExpirationsNew.DrugId " & _
    '                    "FROM APOTIKH INNER JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID INNER JOIN ExpirationsNew ON APOTIKH.AP_CODE = ExpirationsNew.DrugId " & _
    '                    "WHERE ExpirationsNew.DeliveryCode = 'B245418' " & _
    '                    "GROUP BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH_BARCODES.BRAP_AP_BARCODE, ExpirationsNew.DeliveryCode, ExpirationsNew.DeliveryDate,  ExpirationsNew.DrugId"

    '        grpDrugsPerDelivery.Text = "Σύνολο προίοντων: " & DisplayDrugsPerDeliveryWithControls(dgvDrugsPerDelivery, bsDrugsPerDelivery, {"Προιόν", "Μορφή", "Τεμάχια"}, {250, 250, 100}, {"0", "0", "0"})

    '    ElseIf rbDeliveryName.Checked = True Then

    '        'DrugList = GetDistinctContentsDBField("SELECT distinct Name from Drugs ORDER by Name", "Name")

    '        '' Γεμίζει το DatagridView 
    '        'stringDTG = "SELECT DrugName, Id, DeliveryId FROM DrugsPerDelivery WHERE DeliveryId LIKE '%" & DeliveryId.ToString & "%' ORDER BY DrugName"
    '        'grpDrugsPerDelivery.Text = "Σύνολο προίοντων: " & DisplayDrugsPerDeliveryWithControls(dgvDrugsPerDelivery, bsDrugsPerDelivery, {"Φαρμακο"}, {250}, {"0"})

    '    End If
    'End Sub


    'Private Sub LoadDatagridWithDrugsPerDeliveryNew()

    '    ' Βρίσκει το Id της επιλογής μας από το dgvDeliveriesList
    '    Dim DeliveryId As Integer = dgvDeliveriesList.Rows(dgvDeliveriesList.SelectedRows(0).Index).Cells("Id").Value

    '    If rbDeliveryBarcode.Checked = True Then

    '        'DrugList = GetDistinctContentsDBField("SELECT APOTIKH_BARCODES.BRAP_AP_BARCODE, concat(APOTIKH.AP_DESCRIPTION, ' ', APOTIKH.AP_MORFI) as Name,  APOTIKH.AP_CODE " & _
    '        '      "FROM APOTIKH INNER JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID " & _
    '        '      "ORDER BY APOTIKH.AP_DESCRIPTION", "Name")

    '        ' Γεμίζει το DatagridView 
    '        'stringDTG = "SELECT APOTIKH_BARCODES.BRAP_AP_BARCODE, APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI,  APOTIKH.AP_CODE " & _
    '        '            "FROM APOTIKH INNER JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID "
    '        stringDTG = "SELECT Barcode, DrugName,DrugType, Id, DeliveryId FROM DrugsPerDeliveryNew "

    '        grpDrugsPerDelivery.Text = "Σύνολο προίοντων: " & DisplayDrugsPerDeliveryWithControls(dgvDrugsPerDelivery, bsDrugsPerDelivery, {"Barcode", "Όνομα", "Μορφή"}, {100, 250, 250}, {"0", "0", "0"})

    '    ElseIf rbDeliveryName.Checked = True Then

    '        DrugList = GetDistinctContentsDBField("SELECT distinct Name from Drugs ORDER by Name", "Name")

    '        ' Γεμίζει το DatagridView 
    '        stringDTG = "SELECT DrugName, Id, DeliveryId FROM DrugsPerDelivery WHERE DeliveryId LIKE '%" & DeliveryId.ToString & "%' ORDER BY DrugName"
    '        grpDrugsPerDelivery.Text = "Σύνολο προίοντων: " & DisplayDrugsPerDeliveryWithControls(dgvDrugsPerDelivery, bsDrugsPerDelivery, {"Φαρμακο"}, {250}, {"0"})

    '    End If
    'End Sub






    'Private Sub btnEditDrugsPerDelivery_Click(sender As Object, e As EventArgs)
    '    ' Καθορίζει το ενημερωτικό flashing label
    '    timerLabel = lblDrugsPerDeliveryMessage

    '    'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
    '    If btnEditDrugsPerDelivery.Text = "Edit" Then

    '        ChangeControlsDrugsPerDeliveryEdit(True)

    '        'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
    '    ElseIf btnEditDrugsPerDelivery.Text = "Cancel" Then

    '        ChangeControlsDrugsPerDeliveryEdit(False)

    '    End If

    'End Sub



    'Private Sub ChangeControlsDrugsPerDeliveryEdit(ByVal selector As Boolean)
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save

    '    ' Γεμίζει το DatagridView 
    '    LoadDatagridWithDrugsPerDelivery()

    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnSaveDrugsPerDelivery, btnEditDrugsPerDelivery, btnDeleteDrugsPerDelivery}, dgvDrugsPerDelivery, selector)

    '    ' Ενεργοποίηση ενημερωτικού flashing label
    '    timerLabel.Visible = selector
    '    tmrFlashLabel.Enabled = selector

    'End Sub



    'Private Sub btnSaveDrugsPerDelivery_Click(sender As Object, e As EventArgs)

    '    ' Βρίσκει το Id της επιλογής μας από το dgvDeliveriesList
    '    Dim DeliveryId As Integer = dgvDeliveriesList.Rows(dgvDeliveriesList.SelectedRows(0).Index).Cells("Id").Value

    '    ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
    '    ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    UpdateDatagrid({btnSaveDrugsPerDelivery, btnEditDrugsPerDelivery, btnDeleteDrugsPerDelivery}, dgvDrugsPerDelivery, DeliveryId, "DeliveryId")

    '    ChangeControlsDrugsPerDeliveryEdit(False)

    'End Sub



    'Private Sub btnDeleteDrugsPerDelivery_Click(sender As Object, e As EventArgs)

    '    ' Ξεκινάει την διαδικασία Delete των δεδομένων του DataGrid 
    '    ' μαζί με τροποποίηση των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    DeleteDatagrid(dgvDrugsPerDelivery)

    '    ' Τροποποιεί τα Controls της τρέχουσας form
    '    ChangeControlsDrugsPerDeliveryEdit(False)

    'End Sub


    'Private Sub dgvDeliveriesList_CellClick(sender As Object, e As DataGridViewCellEventArgs)
    '    LoadDatagridWithDrugsPerDelivery()
    'End Sub



    Private Sub dgvDrugsPerDelivery_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs)
        ' Φτιάχνει τα χαραχτηριστικά του ComboBox του datagrid DrugsPerDelivery
        If TypeOf e.Control Is DataGridViewComboBoxEditingControl Then
            DirectCast(e.Control, ComboBox).DropDownStyle = ComboBoxStyle.DropDown
            DirectCast(e.Control, ComboBox).AutoCompleteSource = AutoCompleteSource.ListItems
            DirectCast(e.Control, ComboBox).AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        End If
    End Sub


    'Private Sub GetDeliveryBasedOnDrug()

    '    ' Με βάση το προιόν βρίσκει την παραγγελία

    '    stringDTG = "SELECT DeliveryList.DeliveryCode, DeliveryList.DeliveryDate " & _
    '                "FROM DeliveryList INNER JOIN " & _
    '                     "DrugsPerDelivery ON DeliveryList.Id = DrugsPerDelivery.DeliveryId INNER JOIN " & _
    '                     "Drugs ON DrugsPerDelivery.DrugName = Drugs.Name " & _
    '                "WHERE (Drugs.Name LIKE '%" & cboDrugReturns.Text & "%') " & _
    '                "ORDER BY DeliveryDate DESC"


    '    ' Γεμίζει το datagrid με στοιχεία που αντιστοιχούν στις επιλογές μας και 
    '    ' κρατάει στην μεταβλητή sum τον συνολικό αριθμό 
    '    Dim sum As Integer = FillDatagrid(dgvDrugReturns, bsDrugReturns, {"Κωδικός", "Ημερομηνία"}, {60, 120}, {"0", "d"})

    '    ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Select Case sum
    '        Case 0

    '            rtxtDrugReturns.Text = "Δεν βρέθηκαν παραγγελίες"

    '        Case 1

    '            rtxtDrugReturns.Text = "Βρέθηκε 1 παραγγελία"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtDrugReturns, {"1"})

    '        Case Is > 1

    '            rtxtDrugReturns.Text = "Βρέθηκαν " & sum.ToString & " παραγγελίες"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtDrugReturns, {sum.ToString})

    '    End Select

    'End Sub

    'Private Sub cboDrugReturns_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    GetDeliveryBasedOnDrug()
    'End Sub



    ' *********************************************************************************************************
    ' ******************* ΦΑΡΜΑΚΑ NEW *****************************************************************
    ' *****************************************************************************************************



    'Private Sub GetDrugsNew()

    '    ' Με βάση το μέρος ονόματος του πελάτη (textbox) και τις επιλογές μας στo ComboBox
    '    ' βρίσκει όλους τους πελάτες..
    '    If rbBarcode.Checked = True Then

    '        stringDTG = "SELECT APOTIKH_BARCODES.BRAP_AP_BARCODE, APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.AP_TIMH_LIAN, APOTIKH.AP_TIMH_XON, APOTIKH.ap_code " & _
    '                    "FROM APOTIKH LEFT JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID " & _
    '                               "WHERE APOTIKH_BARCODES.BRAP_AP_BARCODE = '" & txtSearchDrugsNew.Text & "' " & _
    '                                "ORDER BY APOTIKH.AP_DESCRIPTION"

    '    ElseIf rbName.Checked = True Then
    '        stringDTG = "SELECT APOTIKH_BARCODES.BRAP_AP_BARCODE, APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.AP_TIMH_LIAN, APOTIKH.AP_TIMH_XON, APOTIKH.ap_code " & _
    '                    "FROM APOTIKH LEFT JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID " & _
    '                    "WHERE APOTIKH.AP_DESCRIPTION like '%" & txtSearchDrugsNew.Text & "%' " & _
    '                    "ORDER BY APOTIKH.AP_DESCRIPTION"
    '    End If

    '    ' Γεμίζει το lstCustomers με τους πελάτες που αντιστοιχούν στις επιλογές μας και 
    '    ' κρατάει στην μεταβλητή sumCustomers τον συνολικό αριθμό πελατών
    '    Dim sumDrugs As Integer = 0
    '    Try
    '        sumDrugs = FillDatagrid(dgvDrugsNew, bsDrugsNew, {"Barcode", "Όνομα", "Μορφή", "Λιανική", "Χονδρική"}, {200, 200, 100, 60, 80}, {"0", "0", "0", "c", "c"}, {"ap_code", "BRAP_AP_BARCODE"})

    '    Catch ex As Exception
    '    End Try

    '    ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Select Case sumDrugs
    '        Case 0

    '            rtxtDrugsNewMessage.Text = "Δεν βρέθηκαν προιόντα"

    '        Case 1

    '            rtxtDrugsNewMessage.Text = "Βρέθηκε 1 προιόν"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtDrugsNewMessage, {"1"})

    '        Case Is > 1

    '            rtxtDrugsNewMessage.Text = "Βρέθηκαν " & sumDrugs.ToString & " προιόντα"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtDrugsNewMessage, {sumDrugs.ToString})

    '    End Select


    'End Sub

    'Private Sub txtSearchDrugsNew_LostFocus(sender As Object, e As EventArgs)
    '    ' Αν φύγουμε από τη καταχώρηση barcode σταματάει ο Timer
    '    tmrPeriodic.Enabled = False
    'End Sub


    'Private Sub txtSearchDrug_TextChanged(sender As Object, e As EventArgs)

    '    'Αν είμαστε σε ΠΑΡΑΛΑΒΗ ή ΑΝΤΑΛΛΑΓΗ ΑΠΟ..
    '    If rbDelivery.Checked = True Or (rbExchanges.Checked = True And rbExchangeFrom.Checked = True) Then

    '        ' και δεν είμαστε σε manual καταχώρηση barcode
    '        If chkManualBarcode.Checked = False Then
    '            ' ενεργοποιεί τον timer
    '            tmrPeriodic.Enabled = True

    '            ' αλλιώς
    '        ElseIf chkManualBarcode.Checked = False Then
    '            ' τον απενεργοποιεί
    '            tmrPeriodic.Enabled = False

    '        End If

    '        'Αν είμαστε σε ΑΝΤΑΛΛΑΓΗ ΠΡΟΣ..
    '    ElseIf rbExchanges.Checked = True And rbExchangeTo.Checked = True Then

    '        ' και δεν είμαστε σε manual καταχώρηση barcode
    '        If chkManualBarcode.Checked = False Then

    '            ' ενεργοποιεί τον timer
    '            tmrPeriodic.Enabled = True

    '            ' αλλιώς
    '        ElseIf chkManualBarcode.Checked = False Then

    '            ' τον απενεργοποιεί
    '            tmrPeriodic.Enabled = False

    '        End If

    '        ' Αν είμαστε σε καταχώρηση με όνομα προιόντος
    '        If rbName.Checked = True Then
    '            ' Εμφανίζει το πλήκτρο "Ανταλλαγή"
    '            btnExchange.Visible = True

    '            ' Αναζητάει τη ανανεωμένη λίστα προιόντων
    '            GetDrugsNew()

    '        ElseIf rbBarcode.Checked = True Then
    '            btnExchange.Visible = False
    '        End If

    '        'Αν είμαστε σε AΡXEIO
    '    ElseIf rbDrugInfo.Checked = True Then
    '        ' Αναζητάει τη ανανεωμένη λίστα προιόντων
    '        GetDrugsNew()

    '        'DisplayExpirationsNew()

    '    End If
    'End Sub



    '' Εαν επιλέξουμε αναζήτηση προιόντος με όνομα
    'Private Sub rbName_CheckedChanged(sender As Object, e As EventArgs)

    '    If rbName.Checked = True Then
    '        If rbDelivery.Checked = True Then


    '        ElseIf rbExchanges.Checked = True And rbExchangeFrom.Checked = True Then
    '            btnExchange.Visible = True

    '            GetDrugsNew() ' βρίσκει την λίστα προιόντων
    '            'DisplayExpirationsNew() ' αναγράφει τις λήξεις

    '        ElseIf rbExchanges.Checked = True And rbExchangeTo.Checked = True Then

    '            btnExchange.Visible = True
    '            GetDrugsNew() ' βρίσκει την λίστα προιόντων

    '        End If
    '        txtSearchDrugsNew.Focus()

    '    ElseIf rbBarcode.Checked = True Then
    '        txtSearchDrugsNew.Focus()
    '        btnExchange.Visible = False
    '    End If

    'End Sub



    '' Εαν επιλέξουμε αναζήτηση προιόντος με barcode
    'Private Sub rbBarcode_CheckedChanged(sender As Object, e As EventArgs)

    '    If rbBarcode.Checked = True Then
    '        GetDrugsNew() ' βρίσκει την λίστα προιόντων
    '        'DisplayExpirationsNew() ' αναγράφει τις λήξεις

    '        txtSearchDrugsNew.Focus()

    '        chkManualBarcode.Enabled = True  ' ενεργοποιεί το manual barcode checkbox
    '    Else
    '        chkManualBarcode.Enabled = False ' απενεργοποιεί το manual barcode checkbox
    '    End If


    'End Sub



    '' Εαν επιλέξουμε κάποιο row από το datagrid
    'Private Sub dgvDrugsNew_CellClick(sender As Object, e As DataGridViewCellEventArgs)

    '    'DisplayExpirationsNew() ' αναγράφει τις λήξεις


    '    'CalculateNewLianikiSelectedDrug()

    'End Sub




    ' ****************************************************************************************************************************
    ' **********
    ' **********    ΔΙΑΧΕΙΡΗΣΗ ΗΜΕΡΟΜΗΝΙΩΝ ΛΗΞΗΣ ΦΑΡΜΑΚΩΝ (NEW !!!!!!)  **********************************************************************
    ' ****************************************************************************************************************************



    'Private Sub ChangeControlsExpDateNew(ByVal selector As Boolean)
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save

    '    ' Βρίσκει το Id της επιλογής μας από το dgvDrugs
    '    Dim drugId As Integer = dgvDrugsNew.Rows(dgvDrugsNew.SelectedRows(0).Index).Cells("ap_code").Value

    '    ' Γεμίζει το DatagridView dgvExpirations με τα ημερομηνίες λήξης του επιλεγμένου φαρμάκου
    '    stringDTG = "Select ExpMonth, ExpYear,BarCode, Id, DrugId, DeliveryCode, DeliveryDate, Quantity, FromWho From ExpirationsNew " & _
    '                "WHERE DrugId = '" & drugId & "' ORDER BY ExpYear,ExpMonth"
    '    FillDatagrid(dgvExpirationsNew, bsExpirationsNew, {"Μήνας", "Έτος", "Barcode"}, {75, 75, 105}, {"0", "##", "####"}, _
    '                 {"Id", "DrugId", "DeliveryCode", "DeliveryDate", "Quantity"})

    '    'Τροποποίηση των άλλων GroupBox
    '    'grpDrugName.Enabled = Not selector
    '    'GrpPrices.Enabled = Not selector

    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnSaveExpNew, btnEditExpNew, btnDeleteExpNew}, dgvExpirationsNew, selector, 1)

    '    ' Ενεργοποίηση ενημερωτικού flashing label
    '    Try
    '        timerLabel.Visible = selector
    '        tmrFlashLabel.Enabled = selector
    '    Catch ex As Exception
    '    End Try


    'End Sub




    'Private Sub btnEditExpNew_Click(sender As Object, e As EventArgs) Handles btnEditExpNew.Click
    '    ' Καθορίζει το ενημερωτικό flashing label
    '    timerLabel = lblExpirationsMessage

    '    'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
    '    If btnEditExpNew.Text = "Edit" Then

    '        ChangeControlsExpDateNew(True)

    '        'Αν το πληκτρο Edit έχει ήδη πατηθεί (-> CANCEL)... 
    '    ElseIf btnEditExpNew.Text = "Cancel" Then

    '        ChangeControlsExpDateNew(False)

    '        'lastRowExpNew = dgvExpirationsNew.SelectedCells(0).RowIndex

    '    End If
    'End Sub




    'Private Sub btnSaveExpNew_Click(sender As Object, e As EventArgs) Handles btnSaveExpNew.Click

    '    ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
    '    ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    UpdateDatagrid({btnSaveExpNew, btnEditExpNew, btnDeleteExpNew}, dgvExpirationsNew, dgvDrugsNew.Rows(dgvDrugsNew.SelectedRows(0).Index).Cells("ap_code").Value, "DrugId")

    '    ChangeControlsExpDateNew(False)

    '    'Focus στο textbook που εισάγεται το barcode και επιλογή του
    '    txtSearchDrugsNew.Focus()
    '    txtSearchDrugsNew.SelectionStart = 0
    '    txtSearchDrugsNew.SelectionLength = txtSearchDrugsNew.TextLength



    'End Sub



    'Private Sub btnDeleteExpNew_Click(sender As Object, e As EventArgs) Handles btnDeleteExpNew.Click

    '    ' Ξεκινάει την διαδικασία Delete των δεδομένων του DataGrid 
    '    ' μαζί με τροποποίηση των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    DeleteDatagrid(dgvExpirationsNew)

    '    ChangeControlsExpDateNew(False)

    'End Sub





    '' Ελέγχει αν η ημερομηνία είοναι σωστή 
    'Private Sub dgvExpirationsNew_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvExpirationsNew.CellValidating

    '    Dim headerText As String = dgvExpirationsNew.Columns(e.ColumnIndex).HeaderText

    '    ' Ελέγχει αν βρισκόμαστε στο πεδίο Μήνας
    '    If headerText.Equals("Μήνας") Then
    '        Dim int As Integer

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι Μήνας
    '        If (e.FormattedValue.ToString <> String.Empty AndAlso Not Integer.TryParse(e.FormattedValue.ToString, int) AndAlso btnEditExpNew.Text = "Cancel") Then
    '            MessageBox.Show("Το '" & e.FormattedValue.ToString & "' δεν αντιστοιχεί σε μήνα", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True
    '        End If

    '        Try
    '            If (CType(e.FormattedValue, Integer) > 12 Or CType(e.FormattedValue, Integer) < 0) Then
    '                MessageBox.Show("Το '" & e.FormattedValue.ToString & "' δεν αντιστοιχεί σε μήνα", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '                e.Cancel = True
    '            End If
    '        Catch ex As Exception
    '        End Try

    '        ' Μόλις φύγει απο το πεδίο γράφει τον αριθμό και την ημερομηνία παραγγελίας 
    '        If rbDelivery.Checked = True And txtDeliveryCode.Text <> "" Then
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(5).Value = txtDeliveryCode.Text
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(6).Value = dtpDeliveryExchangeDate.Value
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(8).Value = cboDeliveryFromWho.Text

    '        ElseIf rbExchanges.Checked = True And rbExchangeFrom.Checked = True And cboExchanges.Text <> "" Then
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(8).Value = cboExchanges.Text
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(6).Value = dtpDeliveryExchangeDate.Value
    '        End If

    '        'Κρατάει την προηγούμενη τιμή του μήνα
    '        Try
    '            expMonth = CType(e.FormattedValue, Integer)
    '        Catch ex As Exception
    '        End Try

    '        ' Eάν δεν έχουμε βάλει barcode στη λήξη 
    '        If dgvExpirationsNew.Rows(e.RowIndex).Cells(2).Value.ToString = "" Then
    '            ' βάζει σαν ποσότητα όσο έχει στο textbox
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(7).Value = CType(txtQuantity.Text, Integer)

    '            ' αλλιως
    '        Else
    '            ' βάζει σαν ποσότητα 1
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(7).Value = 1
    '        End If

    '        ' Ελέγχει αν βρισκόμαστε στο πεδίο Έτος
    '    ElseIf headerText.Equals("Έτος") Then
    '        Dim int As Integer

    '        ' Αν το πεδίο αποκτήσει τιμή που ΔΕΝ είναι Έτος
    '        If e.FormattedValue.ToString <> String.Empty AndAlso Not Integer.TryParse(e.FormattedValue.ToString, int) AndAlso btnEditExpNew.Text = "Cancel" Then

    '            MessageBox.Show("Το '" & e.FormattedValue.ToString & "' δεν αντιστοιχεί σε έγκυρο έτος", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True

    '        End If
    '        Try
    '            If CType(e.FormattedValue, Integer) < Year(Today) Then
    '                MessageBox.Show("Το '" & e.FormattedValue.ToString & "' δεν αντιστοιχεί σε έγκυρο έτος", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '                e.Cancel = True
    '            End If
    '        Catch ex As Exception
    '        End Try

    '        ' Μόλις φύγει απο το πεδίο γράφει τον αριθμό παραγγελίας
    '        If rbDelivery.Checked = True And txtDeliveryCode.Text <> "" Then
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(5).Value = txtDeliveryCode.Text
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(6).Value = dtpDeliveryExchangeDate.Value
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(8).Value = cboDeliveryFromWho.Text

    '        ElseIf rbExchanges.Checked = True And rbExchangeFrom.Checked = True And cboExchanges.Text <> "" Then
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(8).Value = cboExchanges.Text
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(6).Value = dtpDeliveryExchangeDate.Value
    '        End If

    '        'Κρατάει την προηγούμενη τιμή του έτους
    '        Try
    '            expYear = CType(e.FormattedValue, Integer)
    '        Catch ex As Exception
    '        End Try

    '        ' Eάν δεν έχουμε βάλει barcode στη λήξη 
    '        If dgvExpirationsNew.Rows(e.RowIndex).Cells(2).Value.ToString = "" Then
    '            ' βάζει σαν ποσότητα όσο έχει στο textbox
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(7).Value = CType(txtQuantity.Text, Integer)

    '            ' αλλιως
    '        Else
    '            ' βάζει σαν ποσότητα 1
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(7).Value = 1
    '        End If

    '        ' Ελέγχει αν βρισκόμαστε στο πεδίο Barcode
    '    ElseIf headerText.Equals("Barcode") Then

    '        If e.FormattedValue.ToString <> String.Empty AndAlso btnEditExpNew.Text = "Cancel" Then
    '            If expMonth <> 0 And expYear <> 0 Then
    '                dgvExpirationsNew.Rows(e.RowIndex).Cells(0).Value = expMonth.ToString
    '                dgvExpirationsNew.Rows(e.RowIndex).Cells(1).Value = expYear.ToString
    '                ' βάζει σαν ποσότητα 1
    '                dgvExpirationsNew.Rows(e.RowIndex).Cells(7).Value = 1
    '            End If
    '        ElseIf e.FormattedValue.ToString = String.Empty AndAlso btnEditExpNew.Text = "Cancel" Then
    '            ' βάζει σαν ποσότητα όσο έχει στο textbox
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(7).Value = CType(txtQuantity.Text, Integer)
    '        End If

    '        ' Μόλις φύγει απο το πεδίο γράφει τον αριθμό παραγγελίας
    '        If rbDelivery.Checked = True And txtDeliveryCode.Text <> "" Then
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(5).Value = txtDeliveryCode.Text
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(6).Value = dtpDeliveryExchangeDate.Value
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(8).Value = cboDeliveryFromWho.Text

    '        ElseIf rbExchanges.Checked = True And rbExchangeFrom.Checked = True And cboExchanges.Text <> "" Then
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(8).Value = cboExchanges.Text
    '            dgvExpirationsNew.Rows(e.RowIndex).Cells(6).Value = dtpDeliveryExchangeDate.Value
    '        End If

    '    End If
    'End Sub




    'Private Sub dgvExpirationsNew_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvExpirationsNew.CellEndEdit
    '    Try
    '        ' Clear the row error in case the user presses ESC.   
    '        dgvExpirationsNew.Rows(e.RowIndex).ErrorText = String.Empty
    '    Catch ex As Exception

    '    End Try
    'End Sub




    'Private Sub DisplayIncomingExpirationsNew(ByVal months As Integer)

    '    Dim sqlString As String = "SELECT Drugs.Name, DateDiff(month, getdate(), concat(convert(varchar(4),ExpYear), '-',convert(varchar(2),ExpMonth), '-01')) as Duration, " & _
    '                                        "concat(convert(varchar(2),ExpMonth),'-',convert(varchar(4),ExpYear)) as ExpDate3 " & _
    '                                "FROM Drugs INNER JOIN Expirations ON Drugs.Id = Expirations.DrugId " & _
    '                                "WHERE DateDiff(month, getdate(), concat(convert(varchar(4),ExpYear), '-',convert(varchar(2),ExpMonth), '-01')) < " & months & " "

    '    ' Γράφει μια ενημερωση στο Label κάτω από το dgvExpirations
    '    ' ανάλογα με τον αριθμό των φαρμάκων που λήγουν σύντομα
    '    Select Case GetIncomingExpirations(sqlString).Length
    '        Case 0
    '            rtxtExpirMessage.Text = "Δεν βρέθηκαν προιόντα που λήγουν μέσα στoυς επόμενους " & months & " μήνες"

    '            'Εξαφανίζει την ερώτηση για να παρουσιάσει την λίστα των ληγμένων φαρμάκων
    '            rtxtQuestion.Visible = False
    '            btnDisplayExpirDrugsList.Visible = False

    '        Case 1
    '            rtxtExpirMessage.Text = "Βρέθηκε 1 προιόν που λήγει μέσα στoυς επόμενους " & months & " μήνες"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtExpirMessage, {"1"})

    '        Case Is > 1
    '            Dim numExpirDrugs As Integer = GetIncomingExpirations(sqlString).Length
    '            rtxtExpirMessage.Text = "Βρέθηκαν " & numExpirDrugs & " προιόντα που λήγουν μέσα στoυς επόμενους " & months & " μήνες"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtExpirMessage, {numExpirDrugs.ToString})

    '            'Εμφανίζει την ερώτηση για να παρουσιάσει την λίστα των ληγμένων φαρμάκων
    '            rtxtQuestion.Visible = True
    '            btnDisplayExpirDrugsList.Visible = True
    '    End Select

    'End Sub



    Private Function GetIncomingExpirationsNew(ByVal sqlString As String) As Array
        Dim Expirations() As String
        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlString, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                Dim t As Integer = 0

                If myReader.HasRows Then

                    Do While myReader.Read()

                        ReDim Preserve Expirations(t)
                        Expirations(t) = myReader("Name")
                        t += 1
                    Loop
                Else

                End If

                ReDim Preserve Expirations(t)
                Return Expirations
            End Using
        End Using

    End Function




    Private Sub btnDisplayExpirDrugsListNew_Click(sender As Object, e As EventArgs)
        frmExpirDrugsList.Show()
    End Sub


    Private Sub btnEditDrugs_Click(sender As Object, e As EventArgs)

        ' ΠΡΩΤΑ απενεργοποιείται το TabControl
        tbcMain.Enabled = False

        ' META εμφανίζεται το frm με την λίστα των φαρμάκων προς διόρθωση
        frmDrugListEdit.Show()

    End Sub












    'End Sub

    Public Shared Sub DisplayLastUpdate()

        With frmCustomers
            If .tbcMain.SelectedIndex = 0 Then

                .lblLastUpdateExchanges.Text = GetInfoDB("PharmacyCustomFiles", "Exchanges", "modified")

            ElseIf .tbcMain.SelectedIndex = 1 Then

                .lblLastUpdateCustomers.Text = GetInfoDB("PharmacyCustomFiles", "Customers", "modified")
                .lblLastUpdateDebts.Text = GetInfoDB("PharmacyCustomFiles", "Debts", "modified")
                .lblLastUpdateHairDies.Text = GetInfoDB("PharmacyCustomFiles", "HairDies", "modified")
                .lblLastUpdateDrugsOnLoan.Text = GetInfoDB("PharmacyCustomFiles", "DrugsOnLoan", "modified")
                .lblLastUpdatePrescriptions.Text = GetInfoDB("PharmacyCustomFiles", "Prescriptions", "modified")

            ElseIf .tbcMain.SelectedIndex = 2 Then

                .lblLastUpdateParadrugs.Text = GetInfoDB("PharmacyCustomFiles", "PricesParadrugs", "modified")

            ElseIf .tbcMain.SelectedIndex = 3 Then

                .lblLastUpdatePhones.Text = GetInfoDB("PharmacyCustomFiles", "Phonebook", "modified")

            End If
        End With

    End Sub



    Private Sub tbcMain_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tbcMain.SelectedIndexChanged

        ' Ανανεώνει την ώρα τελευταίας ανανέωσης
        DisplayLastUpdate()

        If tbcMain.SelectedIndex <> 1 Then
            Try
                frmPrescriptionInfo.Close()
            Catch ex As Exception
            End Try
        End If

        If tbcMain.SelectedIndex = 0 Then

            cbExchangers.Text = "Λίντα"

        ElseIf tbcMain.SelectedIndex = 1 Then
            GetCustomersList()

        ElseIf tbcMain.SelectedIndex = 2 Then

            txtSearchPricesParadrugs.Focus()
            ' Ανανεώνει την ώρα τελευταίας ανανέωσης
            DisplayLastUpdate()
            DisplayDrugsOrParadrugs()
            GetExpirationsList()

        ElseIf tbcMain.SelectedIndex = 3 Then
            GetPhonesList()

        ElseIf tbcMain.SelectedIndex = 4 Then

            SetPCTerminal()

            lblLastBuilded.Text = LastModifiedDate(My.Computer.FileSystem.CurrentDirectory & "\Pharmacy.exe")

            lblLastUpdatedDB1.Text = "Created on " & CreatedDate("C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\Pharmacy2013C.MDF")

            lblLastUpdated.Text = Format(GetLastUpdateFarmnet(), "dd-MM-yyyy, HH:mm")

            'lblLastMod_DB2.Text = LastModifiedDate("C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\Pharmacy2013C.LDF")
            'MsgBox(My.Computer.FileSystem.CurrentDirectory & "\Pharmacy.exe")

        End If
    End Sub





    'Private Function MoveRecordIntoDrugsSoldOrGiven(ByVal barcode As String) As Boolean


    '    Dim sqlInsert As String = ""



    '    'Δήλωση για την αντιγραφή όλου του row με το επιλεγμένο barcode από το
    '    ' τις λήξεις στα πουλημένα
    '    sqlInsert = "INSERT INTO DrugsSoldOrExchanged (DrugId, ExpMonth, ExpYear, Barcode, DeliveryCode, DeliveryDate, Quantity, ToWho) " & _
    '                                    "(SELECT DrugId, ExpMonth, ExpYear, Barcode, DeliveryCode, DeliveryDate, Quantity, FromWho " & _
    '                                    "FROM ExpirationsNew " & _
    '                                    "WHERE Barcode = '" & barcode & "')"

    '    'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
    '    Using con As New SqlClient.SqlConnection(connectionstring)

    '        'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
    '        Using cmd As New SqlClient.SqlCommand(sqlInsert, con)

    '            ' Ανοίγει την σύνδεση
    '            con.Open()

    '            'Ορισμός ExecuteReader 
    '            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

    '            If myReader.RecordsAffected = 0 Then
    '                Return False
    '            End If

    '        End Using

    '    End Using


    '    'Δήλωση για την αντικατάσταση του FromWho (= ποιος έδωσε το προιον στο φαρμακειο) με το 
    '    ' ToWho (=σε ποιον δώσαμε τελικά το προιον) 
    '    sqlInsert = "UPDATE DrugsSoldOrExchanged " & _
    '                "SET ToWho = '" & cboExchanges.Text & "', " & _
    '                 "SoldOrExchangedDate = '" & dtpDeliveryExchangeDate.Value & "' " & _
    '                "WHERE Barcode = '" & barcode & "' "

    '    'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
    '    Using con As New SqlClient.SqlConnection(connectionstring)


    '        'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
    '        Using cmd As New SqlClient.SqlCommand(sqlInsert, con)

    '            ' Ανοίγει την σύνδεση
    '            con.Open()

    '            'Ορισμός ExecuteReader 
    '            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

    '            If myReader.RecordsAffected = 0 Then
    '                Return False
    '            End If

    '        End Using

    '    End Using

    '    Return True

    'End Function



    'Private Function InsertNamedRecordIntoDrugsSoldOrGiven() As Boolean


    '    Dim sqlInsert As String = ""
    '    Dim drugId = dgvDrugsNew.Rows(dgvDrugsNew.SelectedRows(0).Index).Cells(5).Value
    '    Dim quantity As Integer = CType(txtQuantity.Text, Integer)
    '    Dim toWho As String = cboExchanges.Text
    '    Dim exchangedDate As Date = dtpDeliveryExchangeDate.Value



    '    'Δήλωση για την αντιγραφή όλου του row με το επιλεγμένο barcode από το
    '    ' τις λήξεις στα πουλημένα
    '    sqlInsert = "INSERT INTO DrugsSoldOrExchanged (DrugId, Quantity, ToWho, SoldOrExchangedDate) " & _
    '                                    "VALUES ('" & drugId.ToString & "', '" & quantity.ToString & "', '" & toWho & "', '" & exchangedDate.ToString("yyyy-MM-dd") & "')"

    '    'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
    '    Using con As New SqlClient.SqlConnection(connectionstring)

    '        'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
    '        Using cmd As New SqlClient.SqlCommand(sqlInsert, con)

    '            ' Ανοίγει την σύνδεση
    '            con.Open()

    '            'Ορισμός ExecuteReader 
    '            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

    '            If myReader.RecordsAffected = 0 Then
    '                Return False
    '            End If

    '        End Using

    '    End Using

    '    Return True

    'End Function



    'Private Function InsertNamedRecordIntoExpirationsNew() As Boolean


    '    Dim sqlInsert As String = ""
    '    Dim drugId = dgvDrugsNew.Rows(dgvDrugsNew.SelectedRows(0).Index).Cells(5).Value
    '    Dim quantity As Integer = CType(txtQuantity.Text, Integer)
    '    Dim toWho As String = cboExchanges.Text
    '    Dim exchangedDate As Date = dtpDeliveryExchangeDate.Value


    '    'Δήλωση για την αντιγραφή όλου του row με το επιλεγμένο barcode από το
    '    ' τις λήξεις στα πουλημένα
    '    sqlInsert = "INSERT INTO ExpirationsNew (DrugId, Quantity, FromWho, DeliveryDate) " & _
    '                                    "VALUES ('" & drugId.ToString & "', '" & quantity.ToString & "', '" & toWho & "', '" & exchangedDate.ToString("yyyy-MM-dd") & "')"

    '    'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
    '    Using con As New SqlClient.SqlConnection(connectionstring)

    '        'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
    '        Using cmd As New SqlClient.SqlCommand(sqlInsert, con)

    '            ' Ανοίγει την σύνδεση
    '            con.Open()

    '            'Ορισμός ExecuteReader 
    '            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

    '            If myReader.RecordsAffected = 0 Then
    '                Return False
    '            End If

    '        End Using

    '    End Using



    '    Return True

    'End Function



    Private Function DeleteRecordFromExpirations(ByVal barcode As String) As Boolean

        'Δήλωση για την αντιγραφή του προιόντος με το επιλεγμένο barcode από το
        ' τις λήξεις στα πουλημένα
        Dim sqlInsert As String = "DELETE FROM PharmacyCustomFiles.dbo.[ExpirationsNew] " &
                                  "WHERE Barcode= '" & barcode & "'"

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlInsert, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows = False Then
                    Return False
                End If

            End Using
        End Using

        Return True

    End Function


    Private Function DeleteRecordFromExpirations(ByVal id As Integer) As Boolean

        'Δήλωση για την αντιγραφή του προιόντος με το επιλεγμένο barcode από το
        ' τις λήξεις στα πουλημένα
        Dim sqlInsert As String = "DELETE FROM PharmacyCustomFiles.dbo.[ExpirationsNew] " &
                                  "WHERE Id= '" & id & "'"

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlInsert, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows = False Then
                    Return False
                End If

            End Using
        End Using

        Return True

    End Function





    'Private Sub rbDelivery_CheckedChanged(sender As Object, e As EventArgs)
    '    ' Ενεργοποίηση πλήκτρων και groupbox ανάλογα με το αν..

    '    ' είμαστε σε φάση ΠΑΡΑΛΑΒΗΣ
    '    If rbDelivery.Checked = True Then
    '        dtpDeliveryExchangeDate.Enabled = True
    '        flpDeliveryCode.Visible = True
    '        flpExchanges.Enabled = False

    '        ' εμφανίζει στο Datagrid των λήξεων τις λήξεις
    '        ' DisplayExpirationsNew()

    '        ' είμαστε σε φάση ANTAΛΛΑΓΗΣ
    '    ElseIf rbExchanges.Checked Then
    '        dtpDeliveryExchangeDate.Enabled = True
    '        flpDeliveryCode.Visible = False
    '        flpExchanges.Enabled = True

    '        ' είμαστε σε φάση ΑΡΧΕΙΟΥ
    '    ElseIf rbDrugInfo.Checked Then
    '        dtpDeliveryExchangeDate.Enabled = False
    '        flpDeliveryCode.Visible = False
    '        flpExchanges.Enabled = False

    '        ' εμφανίζει στο Datagrid των λήξεων τις λήξεις
    '        'DisplayExpirationsNew()

    '    End If

    'End Sub


    Private Sub cboCalcLian_FPA_SelectedIndexChanged(sender As Object, e As EventArgs)
        'CalculateNewLianikiSelectedDrug()
    End Sub

    Private Sub txtCalcLian_Profit_TextChanged(sender As Object, e As EventArgs)
        'CalculateNewLianikiSelectedDrug()
    End Sub


    'Private Sub rbExchanges_CheckedChanged(sender As Object, e As EventArgs)
    '    If rbExchanges.Checked = True Then
    '        flpExchanges.Visible = True
    '        cboExchanges.Visible = True

    '    Else
    '        flpExchanges.Visible = False
    '        cboExchanges.Visible = False

    '    End If
    'End Sub

    Private Sub dgvDrugsNew_KeyUp(sender As Object, e As KeyEventArgs)
        'DisplayExpirationsNew() ' αναγράφει τις λήξεις

        'CalculateNewLianikiSelectedDrug()
    End Sub




    'Private Sub GetExchangesTotal()
    '    If rbExchangeTotalApotheka.Checked = True Then
    '        stringDTG = "SELECT [DeliveryCode], count(quantity) as totalquantity " & _
    '           "FROM [Pharmacy].[dbo].[ExpirationsNew] " & _
    '           "WHERE FromWho = 'ΣΥΝΦΑ' " & _
    '           "GROUP BY [DeliveryCode]"

    '    ElseIf rbExchangeTotalPharmacist.Checked = True Then
    '        stringDTG = "SELECT [FromWho], count(quantity) as totalquantity " & _
    '          "FROM [Pharmacy].[dbo].[ExpirationsNew] " & _
    '          "WHERE FromWho <> 'ΣΥΝΦΑ' " & _
    '          "GROUP BY [FromWho]"

    '    End If


    '    ' Γεμίζει το Datagrid που αντιστοιχούν στις επιλογές μας και 
    '    ' κρατάει στην μεταβλητή sumDrugs τον συνολικό αριθμό πελατών
    '    Dim sumDrugs As Integer = 0
    '    Try
    '        sumDrugs = FillDatagrid(dgvExchangesTotal2, bsExchangesTotal, {"Κωδικός", "Τμχ"}, {100, 40}, {"0", "0"}, {})

    '    Catch ex As Exception
    '    End Try

    '    ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Select Case sumDrugs
    '        Case 0

    '            rtxtExchangesTotal.Text = "Δεν βρέθηκαν συναλλαγές"

    '        Case 1

    '            rtxtExchangesTotal.Text = "Βρέθηκε 1 συναλλαγή"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtExchangesTotal, rtxtExchangesTotal.Text, {"1"})

    '        Case Is > 1

    '            rtxtExchangesTotal.Text = "Βρέθηκαν " & sumDrugs.ToString & " συναλλαγές"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtExchangesTotal, rtxtExchangesTotal.Text, {sumDrugs.ToString})

    '    End Select


    'End Sub




    'Private Sub DisplayExchangesFrom()

    '    ' Δήλωση μεταβλητής 
    '    Dim exchangeFrom As String = ""
    '    Dim FromWho As String = ""
    '    Dim dateString As String = ""

    '    'If cboExchangeFromDate.Text = "Όλες" Then
    '    '    dateString = ") "
    '    'ElseIf cboExchangeFromDate.Text = "Τρέχων μήνας" Then
    '    '    dateString = " AND Month(deliveryDate) = month(getdate())) "
    '    'ElseIf cboExchangeFromDate.Text = "Χθες" Then
    '    '    dateString = " AND DateDiff(Day, getdate(), deliveryDate) = -1) "

    '    'ElseIf cboExchangeFromDate.Text = "Σήμερα" Then
    '    '    dateString = " AND DateDiff(Day, getdate(), deliveryDate) = 0) "
    '    'End If


    '    Try

    '        ' Βρίσκει το Id της επιλογής μας από το dgvDrugs
    '        'exchangeFrom = dgvExchangesTotal.Rows(dgvExchangesTotal.SelectedRows(0).Index).Cells(0).Value
    '        exchangeFrom = cboExchangeFromPharmacist.Text

    '    Catch ex As Exception
    '    End Try

    '    If rbExchangeTotalApotheka.Checked = True Then

    '        stringDTG = "SELECT APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, SUM(ExpirationsNew.Quantity) AS totalSum, " & _
    '                                                              "(SUM(ExpirationsNew.Quantity) * APOTIKH.[AP_TIMH_XON]) as totalSum2 " & _
    '                                                      "FROM APOTIKH INNER JOIN ExpirationsNew ON APOTIKH.AP_CODE = ExpirationsNew.DrugId " & _
    '                                                      "WHERE DeliveryCode = '" & exchangeFrom.ToString & "') " & _
    '                                                      "GROUP BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.[AP_TIMH_XON], APOTIKH.AP_CODE "

    '        FromWho = "ΣΥΝΦΑ"

    '    ElseIf rbExchangeTotalPharmacist.Checked = True Then


    '        stringDTG = "SELECT APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, SUM(ExpirationsNew.Quantity) AS totalSum, " & _
    '                                                            "(SUM(ExpirationsNew.Quantity) * APOTIKH.[AP_TIMH_XON]) as totalSum2 " & _
    '                                                    "FROM APOTIKH INNER JOIN ExpirationsNew ON APOTIKH.AP_CODE = ExpirationsNew.DrugId " & _
    '                                                    "WHERE (FromWho = '" & exchangeFrom.ToString & "' " & dateString & _
    '                                                    "GROUP BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.[AP_TIMH_XON], APOTIKH.AP_CODE "

    '        'stringDTG = "SELECT APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, SUM(ExpirationsNew.Quantity) AS totalSum, " & _
    '        '                                                  "(SUM(ExpirationsNew.Quantity) * APOTIKH.[AP_TIMH_XON]) as totalSum2, ExpirationsNew.Id  " & _
    '        '                                          "FROM APOTIKH INNER JOIN ExpirationsNew ON APOTIKH.AP_CODE = ExpirationsNew.DrugId " & _
    '        '                                          "WHERE FromWho = '" & exchangeFrom.ToString & "' AND DeliveryDate = '" & CType(cboExchangeFromDate.Text, Date).ToString("yyyy-MM-dd") & "' " & _
    '        '                                          "GROUP BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.[AP_TIMH_XON], APOTIKH.AP_CODE, ExpirationsNew.Id "

    '        FromWho = exchangeFrom.ToString

    '    End If


    '    ' Γεμίζει το DatagridView dgvExpirations με τα ημερομηνίες λήξης του επιλεγμένου φαρμάκου

    '    FillDatagrid(dgvExchangesFrom, bsExchangesFrom, {"Όνομα", "Μορφή", "Τμχ", "Χονδρική"}, {100, 93, 28, 73}, {"0", "##", "###", "c"}, _
    '                 {"Id"})

    '    Try
    '        For t = 0 To 2
    '            ' Alignment των στοιχείων των Column
    '            dgvExchangesFrom.Columns(t).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '            ' Alignment των HeaderText των Column
    '            dgvExchangesFrom.Columns(t).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        Next t
    '    Catch ex As Exception

    '    End Try


    '    ' Γράφει μια ενημερωση στο Label κάτω από το datagrid
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Dim SumExchangesItems As Decimal = CalculateSums(stringDTG, "totalSum")
    '    Dim SumExchangesPrices As Decimal = CalculateSums(stringDTG, "totalSum2")

    '    InPerPharmacist = SumExchangesPrices

    '    Select Case SumExchangesItems
    '        Case 0
    '            rtxtExchangesFrom.Text = "Δεν πήραμε προιόντα από [ " & FromWho & "]"


    '        Case 1
    '            rtxtExchangesFrom.Text = "Πήραμε 1 προιόν από [ " & FromWho & " ] " & vbCrLf & _
    '                                    " συνολικού κόστους " & SumExchangesPrices.ToString("###,###.## €")

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtExchangesFrom, {"1", SumExchangesPrices.ToString})

    '        Case Is > 1
    '            rtxtExchangesFrom.Text = "Πήραμε " & SumExchangesItems.ToString & " προιόντα από [ " & FromWho & " ]" & vbCrLf & _
    '                                    " συνολικού κόστους " & SumExchangesPrices.ToString("###,###.## €")

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtExchangesFrom, {SumExchangesItems.ToString, SumExchangesPrices.ToString})

    '    End Select

    'End Sub




    Private Sub rbExchangeTotalApotheka_CheckedChanged(sender As Object, e As EventArgs)
        ''GetExchangesTotal()
        'DisplayExchangesFrom()
        'DisplayExchangesTo(dgvExchangesTo, rtxtExchangesTo)
    End Sub

    'Private Sub rbExchangeTotalPharmacist_CheckedChanged(sender As Object, e As EventArgs)
    '    'GetExchangesTotal()
    '    DisplayExchangesFrom()
    'End Sub

    Private Sub cboExchangeFromDate_SelectedIndexChanged(sender As Object, e As EventArgs)

        '' Εμφανίζει τις ΑΝΤΑΛΛΑΓΕΣ ΑΠΟ
        'DisplayExchangesFrom()

        '' Εμφανίζει τις ΑΝΤΑΛΛΑΓΕΣ ΠΡΟΣ
        'DisplayExchangesTo(dgvExchangesTo, rtxtExchangesTo, cboExchanges.Text)

        'Υπολογίζει την χρηματική διαφορά
        DisplayBalancePerPharmacist()
    End Sub


    Private Sub DisplayBalancePerPharmacist()
        BalancePerPharmacist = OutPerPharmacist - InPerPharmacist

        If BalancePerPharmacist > 0 Then
            txtBalance.ForeColor = Color.Red
        ElseIf BalancePerPharmacist < 0 Then
            txtBalance.ForeColor = Color.Red
        Else
            txtBalance.ForeColor = Color.Black
        End If

        txtBalance.Text = Math.Abs(BalancePerPharmacist).ToString("###,###.00 €")
    End Sub

    Private Sub DisplayExchangesBalance()
        Dim PreviousBalance As Decimal = 0
        Dim CurrentBalance As Decimal = 0
        Try
            PreviousBalance = CType(lblPreviousBalance.Text, Decimal)
        Catch ex As Exception
        End Try

        If lblPreviousBalance.ForeColor = Color.Red Then
            PreviousBalance *= -1
        End If

        BalancePerPharmacist = OutPerPharmacist - InPerPharmacist + PreviousBalance
        CurrentBalance = OutPerPharmacist - InPerPharmacist

        If BalancePerPharmacist > 0 Then
            lblExchangesBalance2.ForeColor = Color.Green
        ElseIf BalancePerPharmacist < 0 Then
            lblExchangesBalance2.ForeColor = Color.Red
        Else
            lblExchangesBalance2.ForeColor = Color.Black
        End If

        If CurrentBalance > 0 Then
            lblCurrentBalance.ForeColor = Color.Green
        ElseIf CurrentBalance < 0 Then
            lblCurrentBalance.ForeColor = Color.Red
        Else
            lblCurrentBalance.ForeColor = Color.Black
        End If

        lblExchangesBalance2.Text = Math.Abs(BalancePerPharmacist).ToString("###,###.00 €")
        lblCurrentBalance.Text = Math.Abs(OutPerPharmacist - InPerPharmacist).ToString("###,###.00 €")
    End Sub

    'Private Sub rbExchangeTo_CheckedChanged(sender As Object, e As EventArgs)

    '    ' Αν είμαστε σε ΑΝΤΑΛΛΑΓΕΣ ΠΡΟΣ..
    '    If rbExchangeTo.Checked = True Then

    '        '' εμφανίζει στο Datagrid των λήξεων τα προιόντα που έχουμε δωσει στον φαρμακοποιό ανταλλαγής
    '        'DisplayExchangesTo(dgvExpirationsNew, rtxtExpirationsNewInfo, cboExchanges.Text)
    '        'grpDrugNameNew.Text = ""

    '        'αλλιώς
    '    Else
    '        ' εμφανίζει στο Datagrid των λήξεων τις λήξεις
    '        'DisplayExpirationsNew()

    '    End If

    '    '' Ενεργοποιεί την μεταφορά του προιόντος από τις λήξεις στα δοσμένα
    '    'ExchangeDrugsWithOthers()

    'End Sub

    'Private Sub cboExchanges_SelectedIndexChanged(sender As Object, e As EventArgs)

    '    ' Αν είμαστε σε ΑΝΤΑΛΛΑΓΕΣ ΠΡΟΣ..
    '    If rbExchangeTo.Checked = True Then

    '        '' εμφανίζει στο Datagrid των λήξεων τα προιόντα που έχουμε δωσει στον φαρμακοποιό ανταλλαγής
    '        'DisplayExchangesTo(dgvExpirationsNew, rtxtExpirationsNewInfo, cboExchanges.Text)
    '        ''DisplayExchangesTo(dgvExchangesTo, rtxtExchangesTo, cboExchanges.Text)

    '        'αλλιώς
    '    Else
    '        ' εμφανίζει στο Datagrid των λήξεων τις λήξεις
    '        'DisplayExpirationsNew()

    '    End If

    '    ''Βάζει και στο άλλο combo στις ΑΝΤΑΛΛΑΓΕΣ την ίδια τιμή
    '    'cboExchangeFromPharmacist.Text = cboExchanges.Text

    '    ' Ενεργοποιεί την μεταφορά του προιόντος από τις λήξεις στα δοσμένα
    '    ExchangeDrugsWithOthers()

    'End Sub

    'Private Sub ExchangeDrugsWithOthers()

    '    ' Είμαστε σε ΑΝΤΑΛΛΑΓΗ και
    '    If rbExchangeTo.Checked = True Then

    '        ' χρησιμοποιούμε BARCODE 
    '        If rbBarcode.Checked = True Then

    '            'και έχουμε ήδη καταχωρήσει κάτι
    '            If txtSearchDrugsNew.Text <> "" Then

    '                'Έλεγχος barcode
    '                Dim id As Integer = IsItAllreadyThere("SELECT * FROM PharmacyCustomFiles.dbo.ExpirationsNew WHERE Barcode = '" & txtSearchDrugsNew.Text & "' ", "Id")
    '                If id = 0 Then  ' δεν υπάρχει τέτοιο barcode στο Φαρμακείο
    '                    If IsItAllreadyThere("SELECT * FROM APOTIKH_BARCODES WHERE BRAP_AP_BARCODE = '" & txtSearchDrugsNew.Text & "' ") <> True Then ' δεν υπάρχει ούτε σαν barcode φαρμάκου
    '                        MsgBox("Δεν υπάρχει κανένα προιόν με barcode # " & txtSearchDrugsNew.Text & " στo αρχείο ΣΥΝΦΑ")
    '                        Exit Sub
    '                    Else ' Υπάρχει σαν barcode φαρμάκου
    '                        MsgBox("Πρέπει να καταχωρήσετε το barcode που αντιστοιχεί στο κουπόνι " & vbCrLf & _
    '                                                 "της συσκευασίας, όχι εκείνο που αντιστοιχεί στο προιόν")
    '                        Exit Sub
    '                    End If
    '                Else

    '                    ' Αντιγράφει το αρχείο στο database πωλήσεων - ανταλλαγών
    '                    'MoveRecordIntoDrugsSoldOrGiven(txtSearchDrugsNew.Text)

    '                    'Σβήνει το αρχείο από το database λήξεων
    '                    DeleteRecordFromExpirations(id)

    '                    '' ενημερώνει το datagrid
    '                    'DisplayExchangesTo(dgvExpirationsNew, rtxtExpirationsNewInfo)

    '                End If

    '            End If


    '        ElseIf rbName.Checked = True Then

    '            'και έχουμε ήδη καταχωρήσει κάτι
    '            If txtSearchDrugsNew.Text <> "" Then




    '            End If

    '        End If

    '    End If



    'End Sub

    'Private Sub DisplayExchangesTo(ByVal oDatagrid As DataGridView, ByVal oRichTextBox As RichTextBox, Optional ByVal ToWhoByName As String = "")
    '    ' ToWhoByName: KENO -> Ψάχνει τον φαρμακοποιό ανταλλαγής από το datagrid
    '    '              "???" -> Tον δίνουμε εμείς

    '    ' Δήλωση μεταβλητής 
    '    Dim FromWho As String = ""
    '    Dim dateString As String = ""

    '    If cboExchangeFromDate.Text = "Όλες" Then
    '        dateString = ") "
    '    ElseIf cboExchangeFromDate.Text = "Τρέχων μήνας" Then
    '        dateString = " AND Month(deliveryDate) = month(getdate())) "
    '    ElseIf cboExchangeFromDate.Text = "Χθες" Then
    '        dateString = " AND DateDiff(Day, getdate(), deliveryDate) = -1) "

    '    ElseIf cboExchangeFromDate.Text = "Σήμερα" Then
    '        dateString = " AND DateDiff(Day, getdate(), deliveryDate) = 0) "
    '    End If

    '    Try
    '        If ToWhoByName = "" Then
    '            ' Βρίσκει το φαρμακοποιό ανταλλαγής ΄που επιλέξαμε στο datagrid
    '            FromWho = dgvExchangesTotal2.Rows(dgvExchangesTotal2.SelectedRows(0).Index).Cells(0).Value
    '        Else
    '            ' χρησιμοποιεί αυτόν που του δώσαμε εμείς
    '            FromWho = ToWhoByName
    '        End If


    '    Catch ex As Exception
    '    End Try


    '    If rbExchangeTotalPharmacist.Checked = True Then

    '        'stringDTG = "SELECT APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, SUM(ExpirationsNew.Quantity) AS totalSum, " & _
    '        '                                                  "(SUM(ExpirationsNew.Quantity) * APOTIKH.[AP_TIMH_XON]) as totalSum2, ExpirationsNew.Id  " & _
    '        '                                          "FROM APOTIKH INNER JOIN ExpirationsNew ON APOTIKH.AP_CODE = ExpirationsNew.DrugId " & _
    '        '                                          "WHERE (FromWho = '" & FromWho & "' " & dateString & _
    '        '                                          "GROUP BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.[AP_TIMH_XON], APOTIKH.AP_CODE, ExpirationsNew.Id "

    '        'If cboExchangeFromDate.Text = "Όλες" Then
    '        stringDTG = "SELECT APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, SUM(DrugsSoldOrExchanged.Quantity) AS totalSum, " & _
    '                                                            "(SUM(DrugsSoldOrExchanged.Quantity) * APOTIKH.[AP_TIMH_XON]) as totalSum2 " & _
    '                                                    "FROM APOTIKH INNER JOIN DrugsSoldOrExchanged ON APOTIKH.AP_CODE = DrugsSoldOrExchanged.DrugId " & _
    '                                                    "WHERE (ToWho = '" & FromWho & "' " & dateString & _
    '                                                    "GROUP BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.[AP_TIMH_XON], APOTIKH.AP_CODE"
    '        'Else
    '        '    stringDTG = "SELECT APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, SUM(DrugsSoldOrExchanged.Quantity) AS totalSum, " & _
    '        '                                                      "(SUM(DrugsSoldOrExchanged.Quantity) * APOTIKH.[AP_TIMH_XON]) as totalSum2 " & _
    '        '                                              "FROM APOTIKH INNER JOIN DrugsSoldOrExchanged ON APOTIKH.AP_CODE = DrugsSoldOrExchanged.DrugId " & _
    '        '                                              "WHERE ToWho = '" & FromWho & "' AND DeliveryDate = '" & CType(cboExchangeFromDate.Text, Date).ToString("yyyy-MM-dd") & "' " & _
    '        '                                              "GROUP BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.[AP_TIMH_XON], APOTIKH.AP_CODE"
    '        'End If

    '    End If


    '    ' Γεμίζει το DatagridView dgvExpirations με τα ημερομηνίες λήξης του επιλεγμένου φαρμάκου

    '    FillDatagrid(oDatagrid, bsExchangesTo, {"Όνομα", "Μορφή", "Τμχ", "Χονδρική"}, {100, 93, 28, 73}, {"0", "##", "###", "c"}, _
    '                 {})

    '    Try
    '        For t = 0 To 2
    '            ' Alignment των στοιχείων των Column
    '            oDatagrid.Columns(t).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '            ' Alignment των HeaderText των Column
    '            oDatagrid.Columns(t).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        Next t
    '    Catch ex As Exception

    '    End Try


    '    ' Γράφει μια ενημερωση στο Label κάτω από το datagrid
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Dim SumExchangesItems As Decimal = CalculateSums(stringDTG, "totalSum")
    '    Dim SumExchangesPrices As Decimal = CalculateSums(stringDTG, "totalSum2")

    '    OutPerPharmacist = SumExchangesPrices

    '    Select Case SumExchangesItems
    '        Case 0
    '            oRichTextBox.Text = "Δεν δώσαμε προιόντα σε [ " & FromWho & "]"


    '        Case 1
    '            oRichTextBox.Text = "Δώσαμε 1 προιόν σε [ " & FromWho & " ] " & vbCrLf & _
    '                                    " συνολικού κέρδους " & SumExchangesPrices.ToString("###,###.## €")

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBoxGreen(oRichTextBox, {"1", SumExchangesPrices.ToString})

    '        Case Is > 1
    '            oRichTextBox.Text = "Δώσαμε " & SumExchangesItems.ToString & " προιόντα σε [ " & FromWho & " ]" & vbCrLf & _
    '                                    " συνολικού κέρδους " & SumExchangesPrices.ToString("###,###.## €")

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBoxGreen(oRichTextBox, {SumExchangesItems.ToString, SumExchangesPrices.ToString})

    '    End Select

    'End Sub



    'Private Sub btnDeleteExchangesFrom_Click(sender As Object, e As EventArgs)

    '    ' Μεταβλητές
    '    Dim id As Integer = 0
    '    Dim selectedRow As Integer = dgvExchangesFrom.SelectedCells.Item(0).RowIndex

    '    Try
    '        id = dgvExchangesFrom.Rows(dgvExchangesFrom.SelectedRows(0).Index).Cells("Id").Value
    '    Catch ex As Exception
    '    End Try


    '    ' Επιβεβαίωση της διαγραφής
    '    If MessageBox.Show("Do you want to delete row # " & id & " ?", "Delete", MessageBoxButtons.YesNo) = DialogResult.Yes Then

    '        ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
    '        'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

    '        ' Σβήνει το row από το Database
    '        DeleteRecordFromExpirations(id)

    '        DisplayExchangesFrom()

    '    End If


    'End Sub


    'Private Sub txtQuantity_TextChanged(sender As Object, e As EventArgs) Handles txtQuantity.TextChanged
    '    Dim selectedRow As Integer

    '    ' Αν είμαστε σε ΑΝΤΑΛΛΑΓΕΣ ΑΠΟ
    '    If rbExchanges.Checked = True And rbExchangeFrom.Checked = True Then

    '        Try
    '            '' Βρίσκει το selected row του datagrid
    '            'selectedRow = dgvExchangesFrom.SelectedCells.Item(0).RowIndex
    '        Catch ex As Exception
    '        End Try

    '        Try
    '            ' Eάν δεν έχουμε βάλει barcode στη λήξη 
    '            If dgvExpirationsNew.Rows(selectedRow).Cells(2).Value.ToString = "" Then
    '                ' βάζει σαν ποσότητα όσο έχει στο textbox
    '                dgvExpirationsNew.Rows(selectedRow).Cells(7).Value = CType(txtQuantity.Text, Integer)

    '                ' αλλιως
    '            Else
    '                ' βάζει σαν ποσότητα 1
    '                dgvExpirationsNew.Rows(selectedRow).Cells(7).Value = 1
    '            End If
    '        Catch ex As Exception
    '        End Try
    '    End If


    'End Sub



    'Private Sub btnExchange_Click(sender As Object, e As EventArgs)

    '    If rbExchanges.Checked = True And rbExchangeTo.Checked = True Then

    '        'Καταχωρεί τη μεταφορά
    '        InsertNamedRecordIntoDrugsSoldOrGiven()

    '        '' ενημερώνει το datagrid
    '        'DisplayExchangesTo(dgvExpirationsNew, rtxtExpirationsNewInfo, cboExchanges.Text)

    '    ElseIf rbExchanges.Checked = True And rbExchangeFrom.Checked = True Then

    '        'Καταχωρεί τη μεταφορά
    '        InsertNamedRecordIntoExpirationsNew()

    '        ' ενημερώνει το datagrid
    '        'DisplayExpirationsNew()

    '        'Focus στο textbook που εισάγεται το barcode και επιλογή του
    '        txtSearchDrugsNew.Focus()
    '        txtSearchDrugsNew.SelectionStart = 0
    '        txtSearchDrugsNew.SelectionLength = txtSearchDrugsNew.TextLength
    '    End If




    'End Sub


    Private Sub cboExchangeFromPharmacist_SelectedIndexChanged(sender As Object, e As EventArgs)
        ''Βάζει και στο άλλο combo στα φάρμακα την ίδια τιμή
        'cboExchanges.Text = cboExchangeFromPharmacist.Text

        '' Εμφανίζει τις ΑΝΤΑΛΛΑΓΕΣ ΑΠΟ
        'DisplayExchangesFrom()

        '' Εμφανίζει τις ΑΝΤΑΛΛΑΓΕΣ ΠΡΟΣ
        'DisplayExchangesTo(dgvExchangesTo, rtxtExchangesTo, cboExchanges.Text)

        'Υπολογίζει την χρηματική διαφορά
        DisplayBalancePerPharmacist()

    End Sub



    'Private Sub rbDrugInfo_CheckedChanged(sender As Object, e As EventArgs)

    '    If rbDrugInfo.Checked = True Then

    '        ' εμφανίζει στο Datagrid των λήξεων τις λήξεις
    '        'DisplayExpirationsNew()

    '    End If
    'End Sub


    'Private Sub GetExchangesFrom()


    '    If cboSearchCategory.Text = "Όλα" Then
    '        stringDTG = "SELECT  Name, Id From PharmacyCustomFiles.dbo.Drugs " & _
    '                               "WHERE Drugs.Name like '%" & txtSearchDrugs.Text.ToString & "%' " & _
    '                               "ORDER BY Name, Id"
    '    Else
    '        stringDTG = "SELECT  Name, Id From PharmacyCustomFiles.dbo.Drugs " & _
    '                               "WHERE Drugs.Name like '%" & txtSearchDrugs.Text.ToString & "%' AND " & _
    '                                        "Drugs.Category = '" & cboSearchCategory.Text & "' " & _
    '                                "ORDER BY Name, Id"
    '    End If

    '    ' Γεμίζει το lstCustomers με τους πελάτες που αντιστοιχούν στις επιλογές μας και 
    '    ' κρατάει στην μεταβλητή sumCustomers τον συνολικό αριθμό πελατών
    '    Dim sumDrugs As Integer = FillDatagrid(dgvDrugs, bsDrugs, {"Όνομα"}, {400}, {"0"}, {"Id"})

    '    ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Select Case sumDrugs
    '        Case 0

    '            rtxtDrugsMessage.Text = "Δεν βρέθηκαν προιόντα"

    '        Case 1

    '            rtxtDrugsMessage.Text = "Βρέθηκε 1 προιόντα"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtDrugsMessage, {"1"})

    '        Case Is > 1

    '            rtxtDrugsMessage.Text = "Βρέθηκαν " & sumDrugs.ToString & " προιόντα"

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtDrugsMessage, {sumDrugs.ToString})

    '    End Select

    'End Sub





    Private Function DisplayExchangesWithControls(ByVal oDataGridView As DataGridView, ByVal oBindingSource As BindingSource,
                                  ByVal columnName() As String, ByVal columnWidth() As Integer, ByVal columnFormat() _
                                  As String) As Integer
        'Μεταβλητές
        Dim totalItems As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(stringDTG, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)

        dsDTG = New DataSet

        'Αδειάζει το datagridView
        oDataGridView.Columns.Clear()

        ' και ο SqlDataAdapter γεμίζει το Dataset
        daDTG.Fill(dsDTG, "DTG")
        ' το οποίο γεμίζει το datatable
        dtDTG = dsDTG.Tables("DTG")

        'Καθορίζει το source του BindingSource ως το Datatable
        oBindingSource.DataSource = dtDTG

        'Κλείνει την σύνδεση
        con.Close()

        ' Εναλλαγή του χρωματισμού των rows
        oDataGridView.RowsDefaultCellStyle.BackColor = Color.Bisque
        oDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

        'ElseIf rbDeliveryName.Checked = True Then

        'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
        oDataGridView.AutoGenerateColumns = False

        'Καθορίζει το  source του DataGrid ως το BindingSource
        oDataGridView.DataSource = oBindingSource


        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim Barcode As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        Barcode.DataPropertyName = "BRAP_AP_BARCODE"

        ''Όρίζει το 2ο πεδίο του Datagrid σαν combobox
        'Dim cboDescription As New DataGridViewComboBoxColumn
        '' και του δίνει τη τιμή του αντίστοιχου πεδίου
        'cboDescription.DataPropertyName = "AP_DESCRIPTION"
        ''cboDescription.DataSource = dtDrugsList
        ''cboDescription.DisplayMember = "AP_DESCRIPTION"

        ''Παίρνει όλες τις πιθανές τιμές του array και τις προσθέτει σαν επιλογές στο combobox
        'For t = 0 To DrugListNew.Length - 1
        '    If Not (DrugListNew(t) Is Nothing) Then cboDescription.Items.Add(DrugListNew(t))
        'Next

        'Όρίζει το 2ο πεδίο του Datagrid σαν TextBox
        Dim Description As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        Description.DataPropertyName = "AP_DESCRIPTION"
        'cboDescription.DataSource = dtDrugsList
        'cboDescription.DisplayMember = "AP_DESCRIPTION"

        'Όρίζει το 3ο πεδίο του Datagrid σαν combobox
        Dim cboMorfi As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        cboMorfi.DataPropertyName = "AP_MORFI"

        'Όρίζει το 4ο πεδίο του Datagrid σαν textbox
        Dim Quantity As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        Quantity.DataPropertyName = "Quantity"

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim PriceXondr As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        PriceXondr.DataPropertyName = "AP_TIMH_XON"

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim PriceTot As New DataGridViewTextBoxColumn
        '' και του δίνει τη τιμή του αντίστοιχου πεδίου
        PriceTot.DataPropertyName = "Total"

        'Όρίζει το 6ο πεδίο του Datagrid σαν textbox
        Dim DrugDate As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        DrugDate.DataPropertyName = "DrugDate"

        'Όρίζει το 7ο πεδίο του Datagrid σαν textbox
        Dim FromWho As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        FromWho.DataPropertyName = "FromWho"

        'Όρίζει το 8ο πεδίο του Datagrid σαν textbox
        Dim ToWho As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        ToWho.DataPropertyName = "ToWho"

        'Όρίζει το 9ο πεδίο του Datagrid σαν textbox
        Dim Id As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        Id.DataPropertyName = "Id"

        'Όρίζει το 10ο πεδίο του Datagrid σαν textbox
        Dim DrugId As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        DrugId.DataPropertyName = "DrugId"

        'Εμφανίζει τα columns του Datagrid
        oDataGridView.Columns.Add(Barcode)
        'oDataGridView.Columns.Add(cboDescription)
        oDataGridView.Columns.Add(Description)
        oDataGridView.Columns.Add(cboMorfi)
        oDataGridView.Columns.Add(Quantity)
        oDataGridView.Columns.Add(PriceXondr)
        oDataGridView.Columns.Add(PriceTot)
        oDataGridView.Columns.Add(DrugDate)
        oDataGridView.Columns.Add(FromWho)
        oDataGridView.Columns.Add(ToWho)
        oDataGridView.Columns.Add(Id)
        oDataGridView.Columns.Add(DrugId)


        ' Εναλλαγή του χρωματισμού των rows
        oDataGridView.RowsDefaultCellStyle.BackColor = Color.Bisque
        oDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

        'Εξαφανίζει τα πεδία
        oDataGridView.Columns(6).Visible = False
        oDataGridView.Columns(7).Visible = False
        oDataGridView.Columns(8).Visible = False
        oDataGridView.Columns(9).Visible = False
        'oDataGridView.Columns(10).Visible = False

        ' Μορφοποιεί τα columns
        Try
            For t = 0 To columnName.Length - 1

                oDataGridView.Columns(t).HeaderText = columnName(t) 'Βάζει τίτλο σε κάθε Column

                oDataGridView.Columns(t).Width = columnWidth(t) ' Αλλάζει το φάρδος του κάθε Column

                oDataGridView.Columns(t).DefaultCellStyle.Format = columnFormat(t)  ' Formatαρισμα των στοιχείων
            Next
        Catch ex As Exception
        End Try


        For i As Integer = 0 To oDataGridView.Rows.Count - 1

            Try
                'Ενημερώνει τις Χονδρικές βασιζόμενος στα Τεμάχια και το Σύνολο
                oDataGridView.Rows(i).Cells(4).Value = oDataGridView.Rows(i).Cells(5).Value / oDataGridView.Rows(i).Cells(3).Value

                'Γράφει το συνολικό αριθμό τεμαχίων σαν τίτλο
                totalItems += oDataGridView.Rows(i).Cells(3).Value
            Catch ex As Exception
            End Try


        Next

        'Πηγαίνει στην τελευταία σειρά
        oDataGridView.MultiSelect = False
        oDataGridView.ClearSelection()

        Try
            oDataGridView.Rows(oDataGridView.Rows.Count - 1).Cells(0).Selected = True
        Catch ex As Exception
        End Try



        'Επιστρέφει τον συνολικό αριθμό τεμαχίων 
        Return totalItems


    End Function



    'Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
    '    GetExchangedDrugsFrom()
    'End Sub

    Private Sub dgvExchangeFrom2_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvExchangeFrom2.CellEndEdit
        Try
            ' Clear the row error in case the user presses ESC.   
            dgvExchangeFrom2.Rows(e.RowIndex).ErrorText = String.Empty
        Catch ex As Exception
        End Try
    End Sub

    'Private Sub dgvExchangeFrom2_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles _
    '            dgvExchangeFrom2.CellValidating

    '    If dgvExchangeFrom2.ReadOnly = False Then

    '        'Δήλωση μεταβλητών
    '        Dim headerText As String = dgvExchangeFrom2.Columns(e.ColumnIndex).HeaderText
    '        Dim int As Integer
    '        Dim dbl As Double


    '        'Αν είμαστε στο πεδίο "Barcode"
    '        If headerText.Equals("Barcode") Then
    '            Try
    '                ' και η καταχωρημένη τιμή που ΔΕΝ είναι ακέραιος αριθμός
    '                If Not Integer.TryParse(e.FormattedValue.ToString, dbl) _
    '                        AndAlso btnEdit.Text = "Cancel" Then
    '                    MessageBox.Show("To '" & e.FormattedValue.ToString & "' δεν είναι ακέραιος αριθμός!", _
    '                                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '                    e.Cancel = True
    '                Else

    '                    'GetDetailsFromBarcode(e.FormattedValue.ToString, dgvExchangeFrom2)
    '                End If
    '            Catch ex As Exception
    '            End Try

    '            'Αν είμαστε στο πεδίο "Τεμάχια"
    '        ElseIf headerText.Equals("Τμχ") Then
    '            Try
    '                ' και η καταχωρημένη τιμή που ΔΕΝ είναι ακέραιος αριθμός
    '                If Not Integer.TryParse(e.FormattedValue.ToString, int) _
    '                        AndAlso btnEdit.Text = "Cancel" Then
    '                    MessageBox.Show("To '" & e.FormattedValue.ToString & "' δεν είναι ακέραιος αριθμός!", _
    '                                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '                    e.Cancel = True
    '                Else
    '                    'Αν έχει καταχωρηθεί η Χονδρική 
    '                    If dgvExchangeFrom2.Rows(e.RowIndex).Cells(4).Value <> 0 Then

    '                        'Υπολογίζει και γράφει το Σύνολο  
    '                        dgvExchangeFrom2.Rows(e.RowIndex).Cells(5).Value = CType(e.FormattedValue, Integer) * CType(dgvExchangeFrom2.Rows(e.RowIndex).Cells(4).Value, Decimal)

    '                    End If

    '                End If
    '            Catch ex As Exception
    '            End Try

    '            'Αν είμαστε στο πεδίο "Χονδρική"
    '        ElseIf headerText.Equals("Χονδρική") Then
    '            Try
    '                ' και η καταχωρημένη τιμή που ΔΕΝ είναι χρηματική
    '                If e.FormattedValue.ToString <> String.Empty AndAlso Not Decimal.TryParse(e.FormattedValue.ToString.Substring(0, e.FormattedValue.ToString.Length - 1), int) AndAlso btnEdit.Text = "Cancel" Then
    '                    MessageBox.Show("To '" & e.FormattedValue.ToString & "' δεν είναι έγκυρη τιμή!", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '                    e.Cancel = True
    '                Else

    '                    'Αν έχουν καταχωρηθεί τα Τεμάχια 
    '                    If dgvExchangeFrom2.Rows(e.RowIndex).Cells(3).Value <> 0 Then

    '                        'Υπολογίζει και γράφει το Σύνολο  
    '                        dgvExchangeFrom2.Rows(e.RowIndex).Cells(5).Value = CType(e.FormattedValue, Decimal) * CType(dgvExchangeFrom2.Rows(e.RowIndex).Cells(3).Value, Integer)

    '                    End If
    '                End If
    '            Catch ex As Exception
    '            End Try

    '        ElseIf headerText.Equals("Σύνολο") Then
    '            Try
    '                ' Αν έχουν καταχωρηθεί τα Τεμάχια 
    '                If dgvExchangeFrom2.Rows(e.RowIndex).Cells(3).Value <> 0 Then

    '                    ' Υπολογίζει την Χονδρική
    '                    dgvExchangeFrom2.Rows(e.RowIndex).Cells(4).Value = CType(e.FormattedValue, Decimal) / CType(dgvExchangeFrom2.Rows(e.RowIndex).Cells(3).Value, Integer)

    '                End If
    '            Catch ex As Exception
    '            End Try
    '        End If


    '        '' Αν δεν έχουμε βάλει τεμάχια βάζει αυτόματα το 1
    '        'Try
    '        '    If dgvExchangeFrom2.Rows(e.RowIndex).Cells(3).Value = "" Then
    '        '        quantityExp = 1
    '        '    Else
    '        '        quantityExp = CType(dgvExchangeFrom2.Rows(e.RowIndex).Cells(3).Value, Integer)
    '        '    End If
    '        'Catch ex As Exception
    '        'End Try


    '    End If
    'End Sub


    'Private Sub dgvExchangeFrom2_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles dgvExchangeFrom2.EditingControlShowing


    '    If TypeOf e.Control Is System.Windows.Forms.ComboBox Then
    '        With DirectCast(e.Control, System.Windows.Forms.ComboBox)
    '            'Set the dropdown style as you like
    '            .DropDownStyle = ComboBoxStyle.DropDown
    '            .AutoCompleteMode = AutoCompleteMode.Suggest
    '            .AutoCompleteSource = AutoCompleteSource.ListItems

    '        End With
    '    ElseIf TypeOf e.Control Is System.Windows.Forms.TextBox Then
    '        With DirectCast(e.Control, System.Windows.Forms.TextBox)

    '        End With
    '    Else
    '        'The type is either TextBoxColumn/ImageColumn etc..
    '    End If

    '    Dim TextBoxControl As DataGridViewTextBoxEditingControl = TryCast(e.Control, DataGridViewTextBoxEditingControl)
    '    If TextBoxControl IsNot Nothing Then
    '        AddHandler TextBoxControl.KeyDown, AddressOf ExchangeBarcodeKeypress
    '    End If
    'End Sub





    'Private Sub btnEditExchangeFrom_Click(sender As Object, e As EventArgs) Handles btnEditExchangeFrom.Click

    '    'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
    '    If btnEditExchangeFrom.Text = "Edit" Then

    '        ChangeControlsbtnEditExchangeFrom2(True)

    '        'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
    '    ElseIf btnEditExchangeFrom.Text = "Cancel" Then

    '        ChangeControlsbtnEditExchangeFrom2(False)

    '    End If

    'End Sub


    'Private Sub GetExchangedDrugsFrom()

    '    Dim dateString As String = ""
    '    Dim FromWho As String = cboMyPharmacist.Text

    '    If cboIntervall.Text = "Όλες" Then
    '        dateString = ") "
    '    ElseIf cboIntervall.Text = "Τρέχων μήνας" Then
    '        dateString = " AND Month(DrugDate) = month(getdate())) "
    '    ElseIf cboIntervall.Text = "Χθες" Then
    '        dateString = " AND DateDiff(Day, getdate(), DrugDate) = -1) "

    '    ElseIf cboIntervall.Text = "Σήμερα" Then
    '        dateString = " AND DateDiff(Day, getdate(), DrugDate) = 0) "
    '    End If

    '    stringDTG = "SELECT ExchangesMaster.DrugId, ExchangesMaster.Id, APOTIKH_BARCODES.BRAP_AP_BARCODE, APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.AP_TIMH_XON, " & _
    '                        "ExchangesMaster.Quantity, ExchangesMaster.DrugDate, ExchangesMaster.FromWho, ExchangesMaster.Total " & _
    '                "FROM APOTIKH left JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID left JOIN " & _
    '                        "ExchangesMaster ON APOTIKH.AP_ID = ExchangesMaster.DrugID " & _
    '                "WHERE (FromWho = '" & FromWho & "' " & dateString & _
    '                " ORDER BY ExchangesMaster.Id"

    '    Dim tot As Integer = DisplayExchangesWithControls(dgvExchangeFrom2, bsExchangeFrom2, _
    '                           {"Barcode", "Προιόν", "Μορφή", "Τμχ", "Χονδρική", "Σύνολο"}, {100, 240, 200, 50, 60, 60}, {"0", "0", "0", "0", "c", "c"})

    '    grpExchangeFrom2.Text = "Σύνολο προίοντων: " & tot.ToString


    '    ' Γράφει μια ενημερωση στο Label κάτω από το datagrid
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Dim totalItems As Integer = CalculateSums(stringDTG, "Quantity")
    '    Dim totalSum As Decimal = CalculateSums(stringDTG, "Total")

    '    InPerPharmacist = totalSum

    '    Select Case totalItems
    '        Case 0
    '            rtxtExchangeFrom2.Text = "Δεν πήραμε προιόντα από [ " & FromWho & "]"


    '        Case 1
    '            rtxtExchangeFrom2.Text = "Πήραμε 1 προιόν από [ " & FromWho & " ], συνολικού κόστους " & totalSum.ToString("###,###.## €")

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtExchangeFrom2, {"1", totalSum.ToString})

    '        Case Is > 1
    '            rtxtExchangeFrom2.Text = "Πήραμε " & totalItems.ToString & " προιόντα από [ " & FromWho & " ], συνολικού κόστους " & totalSum.ToString("###,###.## € ")

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBox(rtxtExchangeFrom2, {totalItems.ToString, totalSum.ToString("###,###.##")})

    '    End Select

    'End Sub



    'Private Sub GetExchangedDrugsTo()

    '    Dim dateString As String = ""
    '    Dim ToWho As String = cboMyPharmacist.Text

    '    If cboIntervall.Text = "Όλες" Then
    '        dateString = ") "
    '    ElseIf cboIntervall.Text = "Τρέχων μήνας" Then
    '        dateString = " AND Month(DrugDate) = month(getdate())) "
    '    ElseIf cboIntervall.Text = "Χθες" Then
    '        dateString = " AND DateDiff(Day, getdate(), DrugDate) = -1) "

    '    ElseIf cboIntervall.Text = "Σήμερα" Then
    '        dateString = " AND DateDiff(Day, getdate(), DrugDate) = 0) "
    '    End If

    '    stringDTG = "SELECT ExchangesMaster.DrugId, ExchangesMaster.Id, APOTIKH_BARCODES.BRAP_AP_BARCODE, APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.AP_TIMH_XON, " & _
    '                        "ExchangesMaster.Quantity, ExchangesMaster.DrugDate, ExchangesMaster.FromWho, ExchangesMaster.Total " & _
    '                "FROM APOTIKH left JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID left JOIN " & _
    '                        "ExchangesMaster ON APOTIKH.AP_ID = ExchangesMaster.DrugID " & _
    '                "WHERE (ToWho = '" & ToWho & "' " & dateString & _
    '                " ORDER BY ExchangesMaster.Id"

    '    Dim tot As Integer = DisplayExchangesWithControls(dgvExchangeTo2, bsExchangeTo2, _
    '                           {"Barcode", "Προιόν", "Μορφή", "Τμχ", "Χονδρική", "Σύνολο"}, {100, 240, 200, 50, 60, 60}, {"0", "0", "0", "0", "c", "c"})

    '    grpExchangeTo2.Text = "Σύνολο προίοντων: " & tot.ToString


    '    ' Γράφει μια ενημερωση στο Label κάτω από το datagrid
    '    ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
    '    Dim totalItems As Integer = CalculateSums(stringDTG, "Quantity")
    '    Dim totalSum As Decimal = CalculateSums(stringDTG, "Total")

    '    OutPerPharmacist = totalSum

    '    Select Case totalItems
    '        Case 0
    '            rtxtExchangeTo2.Text = "Δεν δώσαμε προιόντα σε [ " & ToWho & "]"


    '        Case 1
    '            rtxtExchangeTo2.Text = "Δώσαμε 1 προιόν σε [ " & ToWho & " ], συνολικού κέρδους " & totalSum.ToString("###,###.## €")

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBoxGreen(rtxtExchangeTo2, {"1", totalSum.ToString})

    '        Case Is > 1
    '            rtxtExchangeTo2.Text = "Δώσαμε " & totalItems.ToString & " προιόντα σε [ " & ToWho & " ], συνολικού κέρδους " & totalSum.ToString("###,###.## € ")

    '            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
    '            HightlightInRichTextBoxGreen(rtxtExchangeTo2, {totalItems.ToString, totalSum.ToString("###,###.##")})

    '    End Select

    'End Sub


    'Private Sub UpdateExchangesFrom2()
    '    Dim insertData As String = ""

    '    Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionString)
    '        con.Open()
    '        For i As Integer = 0 To dgvExchangeFrom2.Rows.Count - 1

    '            If CheckIfRecordChanged_ExchangesFrom(i) = True Then

    '                'MsgBox(" Line " & i.ToString & " exists!")
    '                insertData = "UPDATE ExchangesMaster " & _
    '                              "SET [DrugID] = @DrugID, [Quantity] =  @Quantity, [DrugDate] = @DrugDate, [FromWho] = @FromWho, [ToWho]= @ToWho, [Total] = @Total " & _
    '                              "WHERE Id = @Id"
    '            ElseIf CType(dgvExchangeFrom2.Rows(i).Cells(10).Value, Integer) > 0 Or dgvExchangeFrom2.Rows(i).Cells(1).Value <> "" Then

    '                insertData = "INSERT INTO ExchangesMaster " & _
    '                                                        "([DrugID] ,[Quantity] ,[DrugDate] ,[FromWho] ,[ToWho] ,[Total]) " & _
    '                                                        "VALUES (@DrugID, @Quantity, @DrugDate, @FromWho, @ToWho, @Total)"
    '            Else
    '                Exit Sub

    '            End If

    '            Dim cmd As New SqlCommand(insertData, con)
    '            cmd.Parameters.AddWithValue("@Quantity", If(dgvExchangeFrom2.Rows(i).Cells(3).Value, DBNull.Value))
    '            cmd.Parameters.AddWithValue("@Total", If(dgvExchangeFrom2.Rows(i).Cells(5).Value, DBNull.Value))
    '            cmd.Parameters.AddWithValue("@DrugID", If(dgvExchangeFrom2.Rows(i).Cells(10).Value, DBNull.Value))
    '            cmd.Parameters.AddWithValue("@DrugDate", dtpExchangesNew.Value)
    '            cmd.Parameters.AddWithValue("@FromWho", If(cboMyPharmacist.Text, DBNull.Value))
    '            cmd.Parameters.AddWithValue("@ToWho", If(dgvExchangeFrom2.Rows(i).Cells(8).Value, DBNull.Value))
    '            cmd.Parameters.AddWithValue("@Id", If(dgvExchangeFrom2.Rows(i).Cells(9).Value, DBNull.Value))

    '            cmd.ExecuteNonQuery()
    '        Next
    '    End Using

    'End Sub




    Private Sub UpdateExchangesTo2()
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()
            For i As Integer = 0 To dgvExchangeTo2.Rows.Count - 1

                If CheckIfRecordChanged_ExchangesTo(i) = True Then

                    'MsgBox(" Line " & i.ToString & " exists!")
                    insertData = "UPDATE ExchangesMaster " &
                                  "SET [DrugID] = @DrugID, [Quantity] =  @Quantity, [DrugDate] = @DrugDate, [FromWho] = @FromWho, [ToWho]= @ToWho, [Total] = @Total " &
                                  "WHERE Id = @Id"
                ElseIf CType(dgvExchangeTo2.Rows(i).Cells(10).Value, Integer) > 0 Then

                    insertData = "INSERT INTO ExchangesMaster " &
                                                            "([DrugID] ,[Quantity] ,[DrugDate] ,[FromWho] ,[ToWho] ,[Total]) " &
                                                            "VALUES (@DrugID, @Quantity, @DrugDate, @FromWho, @ToWho, @Total)"
                Else
                    Exit Sub

                End If

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Quantity", If(dgvExchangeTo2.Rows(i).Cells(3).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@Total", If(dgvExchangeTo2.Rows(i).Cells(5).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@DrugID", If(dgvExchangeTo2.Rows(i).Cells(10).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@DrugDate", dtpExchangesNew.Value)
                cmd.Parameters.AddWithValue("@FromWho", If(dgvExchangeTo2.Rows(i).Cells(7).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@ToWho", If(cboMyPharmacist.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Id", If(dgvExchangeTo2.Rows(i).Cells(9).Value, DBNull.Value))

                cmd.ExecuteNonQuery()
            Next
        End Using

    End Sub


    Private Sub Add2ExchangerList()
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "INSERT INTO PharmacyCustomFiles.dbo.ExchangerList " &
                        "([ExchangerName]) " &
                        "VALUES (@Name)"

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Name", cbExchangers.Text)

            cmd.ExecuteNonQuery()

        End Using

    End Sub


    Private Sub DeleteFromExchangerList()
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.[ExchangerList] " &
                                "WHERE ExchangerName= '" & cbExchangers.Text & "'"

            Dim cmd As New SqlCommand(insertData, con)

            cmd.ExecuteNonQuery()

        End Using

    End Sub




    Private Sub UpdatePricesParadrugs()
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()
            For i As Integer = 0 To dgvPricesParadrugs.Rows.Count - 1
                ChangedOrExists = CheckIfRecordChangedOrExists_PricesParadrugs(i)

                If ChangedOrExists = "Changed" Then

                    insertData = "UPDATE PharmacyCustomFiles.dbo.PricesParadrugs " &
                                  "SET [Name] = @Name, [Xondr] = @Xondr, [Lian] =  @Lian, [Notes] = @Notes, [AP_Code]= @AP_Code " &
                                  "WHERE Id = @Id"

                ElseIf ChangedOrExists = "NewRow" Then

                    insertData = "INSERT INTO PharmacyCustomFiles.dbo.PricesParadrugs " &
                                                            "([Name] ,[Xondr] ,[Lian] ,[Notes] ,[AP_Code]) " &
                                                            "VALUES (@Name, @Xondr, @Lian, @Notes, @AP_Code)"

                    'ElseIf ChangedOrExists = "NotChanged" Then

                    '    Exit Sub

                End If

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Name", If(dgvPricesParadrugs.Rows(i).Cells(1).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@Xondr", If(dgvPricesParadrugs.Rows(i).Cells(2).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@Lian", If(dgvPricesParadrugs.Rows(i).Cells(3).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@Notes", If(dgvPricesParadrugs.Rows(i).Cells(4).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@AP_Code", If(dgvPricesParadrugs.Rows(i).Cells(5).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@Id", If(dgvPricesParadrugs.Rows(i).Cells(0).Value, DBNull.Value))

                cmd.ExecuteNonQuery()

            Next

        End Using

    End Sub




    Private Sub UpdateCustomer2(ByVal i As Integer)
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""


        ' Κρατάει στη μνήμη τη παλιά και τη νέα τιμή του κελιού
        Dim oldValue As String = ""
        Dim newValue As String = ""

        Try
            oldValue = dgvCustomers.Rows(i).Cells(0).Value.ToString
            newValue = dgvCustomers.Rows(i).Cells(0).EditedFormattedValue
        Catch ex As Exception

        End Try


        Dim mySQL As String = "Select Name, Id From PharmacyCustomFiles.dbo.Customers WHERE Name='" & newValue & "'"

        ''Αν η παλιά τιμή είναι διαφορετική από τη νέα ΚΑΙ η νέα τιμή υπάρχει ήδη στο database
        'If oldValue <> newValue AndAlso IsItAllreadyThere(mySQL) = True Then
        '    'Μύνημα λάθους
        '    MessageBox.Show("Το όνομα " & newValue & " υπάρχει ήδη !", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        '    'Ξαμαγράφει τη παλιά τιμή
        '    dgvCustomers.Rows(i).Cells(0).Value = oldValue
        '    dgvCustomers.RefreshEdit()

        '    'Εμποδίζει την έξοδο από το κελί
        '    dgvCustomers.CurrentCell = dgvCustomers.Rows(i).Cells(0)
        '    dgvCustomers.BeginEdit(True)

        '    Exit Sub

        'End If

        If oldValue = newValue Then
            Exit Sub
        End If


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists = CheckIfRecordChangedOrExists_Customers(i)

            If ChangedOrExists = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.Customers " &
                              "SET [Name] = @Name " &
                              "WHERE Id = @Id"

            ElseIf ChangedOrExists = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.Customers " &
                            "([Name]) VALUES (@Name)"

            End If

            If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

                Dim cmd As New SqlCommand(insertData, con)

                If IsDBNull(dgvCustomers.Rows(i).Cells(0).EditedFormattedValue) = True Or dgvCustomers.Rows(i).Cells(0).EditedFormattedValue = "" Then

                Else

                    cmd.Parameters.AddWithValue("@Name", If(dgvCustomers.Rows(i).Cells(0).EditedFormattedValue, DBNull.Value))

                    If ChangedOrExists = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvCustomers.Rows(i).Cells(1).Value, DBNull.Value))

                    cmd.ExecuteNonQuery()

                    ' Ανανεώνει τη λίστα των πελατων, ξαναεπιλέγοντας τον τελευταίο πελάτη
                    Try
                        txtSearchCustomer.Text = dgvCustomers.Rows(i).Cells(0).EditedFormattedValue
                    Catch ex As Exception
                    End Try

                    'Dim SelectedCustomer As String = dgvCustomers.Rows(i).Cells(0).EditedFormattedValue
                    'GetCustomersList()
                    'Dim rowIndex As Integer = SearchDatagrid(dgvCustomers, SelectedCustomer)
                    'dgvCustomers.CurrentCell = dgvCustomers.Rows(rowIndex).Cells(0)

                    'Ανανεώνει το Last Update
                    DisplayLastUpdate()

                End If

            ElseIf ChangedOrExists = "Error" Then

            End If

        End Using

    End Sub



    Private Sub UpdateCustomer(ByVal i As Integer)
        Dim insertData As String = ""
        Dim mySQL As String = ""
        Dim ChangedOrExists As String = ""
        Dim oldValue As String = ""
        Dim newValue As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists = CheckIfRecordChangedOrExists_Customers(i)

            If ChangedOrExists = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.Customers " &
                              "SET [Name] = @Name " &
                              "WHERE Id = @Id"

            ElseIf ChangedOrExists = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.Customers " &
                            "([Name]) VALUES (@Name)"

            End If

            If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

                Dim cmd As New SqlCommand(insertData, con)

                If IsDBNull(dgvCustomers.Rows(i).Cells(0).EditedFormattedValue) = True Or dgvCustomers.Rows(i).Cells(0).EditedFormattedValue = "" Then
                Else

                    cmd.Parameters.AddWithValue("@Name", If(dgvCustomers.Rows(i).Cells(0).EditedFormattedValue, DBNull.Value))

                    If ChangedOrExists = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvCustomers.Rows(i).Cells(1).Value, DBNull.Value))

                    cmd.ExecuteNonQuery()


                    ' Ανανεώνει τη λίστα των πελατων, ξαναεπιλέγοντας τον τελευταίο πελάτη
                    txtSearchCustomer.Text = If(dgvCustomers.Rows(i).Cells(0).EditedFormattedValue, DBNull.Value)

                    GetCustomersList()

                    'Ανανεώνει το Last Update
                    DisplayLastUpdate()

                End If


            ElseIf ChangedOrExists = "Error" Then

            End If

        End Using

    End Sub



    Private Sub UpdateDebts2(ByVal i As Integer)
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrNew_Exchanges = CheckIfRecordChangedOrExists_Debts(i)

            If ChangedOrNew_Exchanges = "Error" Then
            ElseIf ChangedOrNew_Exchanges = "NotChanged" Then
                Exit Sub
            End If


            If ChangedOrNew_Exchanges = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.debts " &
                              "SET [CustomerId] = @CustomerId, [Date] = @Date, [DebtDescription] =@DebtDescription, [Ammount]= @Ammount " &
                              "WHERE Id = @Id"
            ElseIf ChangedOrNew_Exchanges = "NewRow" Then
                insertData = "INSERT INTO PharmacyCustomFiles.dbo.debts " &
                            "([CustomerId], [Date], [DebtDescription], [Ammount]) VALUES (@CustomerId, @Date, @DebtDescription, @Ammount)"
            End If

            Dim cmd As New SqlCommand(insertData, con)

            ' Αν δεν έχουμε καταχωρήσει όνομα φαρμάκου..
            If IsDBNull(dgvDebtsList.Rows(i).Cells(0).Value) = True Or IsDate(dgvDebtsList.Rows(i).Cells(0).Value) = False Then
                ' ή τιμή ..
            ElseIf IsDBNull(dgvDebtsList.Rows(i).Cells(1).Value) = True Or dgvDebtsList.Rows(i).Cells(1).Value = 0 Then
                ' δεν κάνει τίποτα

                ' Αλλιώς ->
            Else
                ' Περνάει τις παραμέτρους του SQL 
                If ChangedOrNew_Exchanges = "NewRow" Then

                    cmd.Parameters.AddWithValue("@CustomerId", If(dgvCustomers.SelectedRows(0).Cells(1).Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Date", If(dgvDebtsList.Rows(i).Cells(0).EditedFormattedValue = "", DBNull.Value, CType(dgvDebtsList.Rows(i).Cells(0).EditedFormattedValue, Date)))
                    cmd.Parameters.AddWithValue("@DebtDescription", If(dgvDebtsList.Rows(i).Cells(2).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Ammount", If(dgvDebtsList.Rows(i).Cells(1).EditedFormattedValue = "", 0, CType(dgvDebtsList.Rows(i).Cells(1).EditedFormattedValue, Decimal)))

                ElseIf ChangedOrNew_Exchanges = "Changed" Then

                    cmd.Parameters.AddWithValue("@CustomerId", If(dgvCustomers.SelectedRows(0).Cells(1).Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Date", If(dgvDebtsList.Rows(i).Cells(0).EditedFormattedValue = "", DBNull.Value, CType(dgvDebtsList.Rows(i).Cells(0).EditedFormattedValue, Date)))
                    cmd.Parameters.AddWithValue("@DebtDescription", If(dgvDebtsList.Rows(i).Cells(2).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Ammount", If(dgvDebtsList.Rows(i).Cells(1).EditedFormattedValue = "", 0, CType(dgvDebtsList.Rows(i).Cells(1).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@Id", If(dgvDebtsList.Rows(i).Cells(3).Value, DBNull.Value))

                End If

                ' Σώζει τις αλλαγές
                cmd.ExecuteNonQuery()

                ' Ανανεώνει τα σύνολα και τις τιμές
                'UpdateExchangesTotalAndSums()

                ' Αν προσθέσαμε μια νέα έγγραφή ανανεώνει το datagrid
                If ChangedOrNew_Exchanges = "NewRow" Then
                    GetCustomersList()
                End If

            End If

        End Using

    End Sub




    Private Sub UpdateDebts(ByVal i As Integer, ByVal column As Integer)
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists = CheckIfRecordChangedOrExists_Debts(i)

            If ChangedOrExists = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.debts " &
                              "SET [CustomerId] = @CustomerId, [Date] = @Date, [DebtDescription] =@DebtDescription, [Ammount]= @Ammount " &
                              "WHERE Id = @Id"

            ElseIf ChangedOrExists = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.debts " &
                            "([CustomerId], [Date], [DebtDescription], [Ammount]) VALUES (@CustomerId, @Date, @DebtDescription, @Ammount)"

            End If

            If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

                Dim cmd As New SqlCommand(insertData, con)

                If IsDBNull(dgvDebtsList.Rows(i).Cells(1).Value) = True Or dgvDebtsList.Rows(i).Cells(1).Value = 0 Then

                Else

                    cmd.Parameters.AddWithValue("@CustomerId", If(dgvCustomers.SelectedRows(0).Cells(1).Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Date", If(dgvDebtsList.Rows(i).Cells(0).EditedFormattedValue = "", DBNull.Value, CType(dgvDebtsList.Rows(i).Cells(0).EditedFormattedValue, Date)))
                    cmd.Parameters.AddWithValue("@DebtDescription", If(dgvDebtsList.Rows(i).Cells(2).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Ammount", If(dgvDebtsList.Rows(i).Cells(1).EditedFormattedValue = "", 0, CType(dgvDebtsList.Rows(i).Cells(1).EditedFormattedValue, Decimal)))

                    If ChangedOrExists = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvDebtsList.Rows(i).Cells(3).Value, DBNull.Value))

                    cmd.ExecuteNonQuery()

                    lblNewRecord_Debts.Text = "ΕΓΓΡΑΦΗ"

                    'Ανανεώνει το Last Update
                    DisplayLastUpdate()

                    'Αν κάνουμε update στην τιμή
                    'If column = 1 Then
                    '    Dim SelectedCustomer As String = dgvCustomers.SelectedRows(0).Cells(0).Value
                    '    ' Ανανεώνει τη λίστα των πελατων
                    '    GetCustomersList()
                    '    Dim rowIndex As Integer = SearchDatagrid(dgvCustomers, SelectedCustomer)
                    '    dgvCustomers.CurrentCell = dgvCustomers.Rows(rowIndex).Cells(0)

                    'End If



                End If

            ElseIf ChangedOrExists = "Error" Then

            End If

        End Using

    End Sub



    Private Sub UpdateDrugsOnLoan(ByVal i As Integer, ByVal column As Integer)
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""

        Try
            Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
                con.Open()

                ChangedOrExists = CheckIfRecordChangedOrExists_DrugsOnLoan(i)

                If ChangedOrExists = "Changed" Then

                    insertData = "UPDATE PharmacyCustomFiles.dbo.DrugsOnLoan " &
                                  "SET [Name] = @Name, [Price] = @Price, [DateIn] =@DateIn, [Barcode1]= @Barcode1, [Barcode2] = @Barcode2, [DateOut] = @DateOut, [CustomerId] =@CustomerId " &
                                  "WHERE Id = @Id"

                ElseIf ChangedOrExists = "NewRow" Then

                    insertData = "INSERT INTO PharmacyCustomFiles.dbo.DrugsOnLoan " &
                                "([Name], [Price], [DateIn], [Barcode1], [Barcode2], [DateOut], [CustomerId]) VALUES (@Name, @Price, @DateIn, @Barcode1, @Barcode2, @DateOut, @CustomerId)"

                End If

                If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

                    Dim cmd As New SqlCommand(insertData, con)

                    If IsDBNull(dgvDrugsOnLoan.Rows(i).Cells(1).Value) = True Or dgvDrugsOnLoan.Rows(i).Cells(1).Value = "" Or
                        IsDBNull(dgvDrugsOnLoan.Rows(i).Cells(2).Value) = True Or dgvDrugsOnLoan.Rows(i).Cells(2).Value = 0 Or
                        IsDBNull(dgvDrugsOnLoan.Rows(i).Cells(3).Value) = True Or
                        IsDBNull(dgvDrugsOnLoan.Rows(i).Cells(4).Value) = True Or
                        IsDBNull(dgvDrugsOnLoan.Rows(i).Cells(7).Value) = True Then

                        'MsgBox("NoWrite")

                    Else
                        cmd.Parameters.AddWithValue("@DateIn", If(dgvDrugsOnLoan.Rows(i).Cells(0).EditedFormattedValue = "", DBNull.Value, CType(dgvDrugsOnLoan.Rows(i).Cells(0).EditedFormattedValue, Date)))
                        cmd.Parameters.AddWithValue("@Name", If(dgvDrugsOnLoan.Rows(i).Cells(1).Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@Price", If(dgvDrugsOnLoan.Rows(i).Cells(2).EditedFormattedValue = "", 0, CType(dgvDrugsOnLoan.Rows(i).Cells(2).EditedFormattedValue, Decimal)))
                        cmd.Parameters.AddWithValue("@Barcode1", If(dgvDrugsOnLoan.Rows(i).Cells(3).EditedFormattedValue, DBNull.Value))
                        cmd.Parameters.AddWithValue("@Barcode2", If(dgvDrugsOnLoan.Rows(i).Cells(4).EditedFormattedValue, DBNull.Value))
                        cmd.Parameters.AddWithValue("@DateOut", If(dgvDrugsOnLoan.Rows(i).Cells(5).EditedFormattedValue = "", DBNull.Value, CType(dgvDrugsOnLoan.Rows(i).Cells(5).EditedFormattedValue, Date)))
                        cmd.Parameters.AddWithValue("@CustomerId", If(dgvCustomers.SelectedRows(0).Cells(1).Value, DBNull.Value))

                        If ChangedOrExists = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvDrugsOnLoan.Rows(i).Cells(6).Value, DBNull.Value))

                        cmd.ExecuteNonQuery()

                        'Ανανεώνει το Last Update
                        DisplayLastUpdate()

                        'Αν κάνουμε update στην τιμή
                        'If column = 7 Then

                        ' Ανανεώνει τη λίστα των πελατων, ξαναεπιλέγοντας τον τελευταίο πελάτη
                        Dim SelectedCustomer As String = dgvCustomers.SelectedRows(0).Cells(0).Value
                        GetCustomersList()
                        Dim rowIndex As Integer = SearchDatagrid(dgvCustomers, SelectedCustomer)
                        dgvCustomers.CurrentCell = dgvCustomers.Rows(rowIndex).Cells(0)

                        ' Αναγράφει το συνολικό χρέος στον τίτλο του GroupBox
                        DisplaySums_DrugsOnLoan()

                    End If

                ElseIf ChangedOrExists = "Error" Then

                End If

            End Using

        Catch ex As Exception
            MsgBox(ex.Message & "OK")
        End Try



    End Sub


    ' --- DIRECT INSERT, χωρίς εξάρτηση από το dgvDrugsOnLoan ---
    Public Sub InsertDrugOnLoanDirect(drugName As String,
                                  price As Decimal,
                                  barcode1 As String,
                                  barcode2 As String,
                                  Optional dateIn As Date? = Nothing,
                                  Optional dateOut As Date? = Nothing)

        Dim sql As String = "INSERT INTO PharmacyCustomFiles.dbo.DrugsOnLoan " &
                        "([Name],[Price],[DateIn],[Barcode1],[Barcode2],[DateOut],[CustomerId]) " &
                        "VALUES (@Name,@Price,@DateIn,@Barcode1,@Barcode2,@DateOut,@CustomerId)"

        Using con As New SqlClient.SqlConnection(connectionstring)
            con.Open()
            Using cmd As New SqlClient.SqlCommand(sql, con)
                Dim din As Object = If(dateIn.HasValue, CType(dateIn.Value, Date), CType(Date.Today, Date))
                Dim dout As Object = If(dateOut.HasValue, CType(dateOut.Value, Date), DBNull.Value)
                Dim custId As Object = If(dgvCustomers.SelectedRows.Count > 0,
                                      dgvCustomers.SelectedRows(0).Cells(1).Value,
                                      DBNull.Value)

                cmd.Parameters.AddWithValue("@Name", If(String.IsNullOrWhiteSpace(drugName), DBNull.Value, drugName))
                cmd.Parameters.AddWithValue("@Price", price)
                cmd.Parameters.AddWithValue("@DateIn", din)
                cmd.Parameters.AddWithValue("@Barcode1", If(String.IsNullOrWhiteSpace(barcode1), DBNull.Value, barcode1))
                cmd.Parameters.AddWithValue("@Barcode2", If(String.IsNullOrWhiteSpace(barcode2), DBNull.Value, barcode2))
                cmd.Parameters.AddWithValue("@DateOut", dout)
                cmd.Parameters.AddWithValue("@CustomerId", custId)

                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' Ελαφρύ refresh όπως ήδη κάνεις αλλού
        GetDrugsOnLoanList()
        DisplaySums_DrugsOnLoan()
        DisplayTotalDebtPerCustomer()
        DisplayLastUpdate()
    End Sub



    Private Sub btnAddDrug_Click(sender As Object, e As EventArgs) Handles btnAddDrug.Click
        Me.Enabled = False
        ' Καταχώρηση χρήσης scanner για φάρμακα σε πελάτη
        UsingBarcodeForm = "AddDrugOnLoan"
        ' Άνοιγμα του frmAddDrugOnLoan
        frmAddDrugOnLoan.Show()
    End Sub



    'Private Sub UpdatePrescriptions(ByVal i As Integer, ByVal column As Integer)
    '    Dim insertData As String = ""
    '    Dim ChangedOrExists As String = ""


    '    Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
    '        con.Open()

    '        ChangedOrExists = CheckIfRecordChangedOrExists_Prescriptions(i)

    '        If ChangedOrExists = "Changed" Then

    '            insertData = "UPDATE PharmacyCustomFiles.dbo.Prescriptions " & _
    '                          "SET [Ektelesis] = @Ektelesis, [InitDate] = @InitDate, [EndDate] = @EndDate, [Barcode] =@Barcode, " & _
    '                                "[ProcessedDate]= @ProcessedDate, [CustomerId] = @CustomerId, [Drug1] = @Drug1, [Drug2] = @Drug2, [Drug3] = @Drug3 " & _
    '                          "WHERE Id = @Id"

    '        ElseIf ChangedOrExists = "NewRow" Then

    '            insertData = "INSERT INTO PharmacyCustomFiles.dbo.Prescriptions " & _
    '                        "([Ektelesis],[InitDate], [EndDate], [Barcode], [ProcessedDate], [CustomerId], [Drug1], [Drug2], [Drug3]) " & _
    '                        "VALUES (@Ektelesis, @InitDate, @EndDate, @Barcode, @ProcessedDate, @CustomerId, @Drug1, @Drug2, @Drug3)"

    '        End If

    '        If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

    '            Dim cmd As New SqlCommand(insertData, con)

    '            If IsDBNull(dgvPrescriptions.Rows(i).Cells(0).Value) = True Then   ' Ektelesis
    '            ElseIf IsDBNull(dgvPrescriptions.Rows(i).Cells(1).Value) = True Then ' InitDate
    '            ElseIf IsDBNull(dgvPrescriptions.Rows(i).Cells(2).Value) = True Then 'EndDate
    '            ElseIf IsDBNull(dgvPrescriptions.Rows(i).Cells(3).Value) = True Then 'Barcode
    '            ElseIf IsDBNull(dgvPrescriptions.Rows(i).Cells(5).Value) = True Then 'CustomerId
    '            ElseIf IsDBNull(dgvPrescriptions.Rows(i).Cells(7).Value) = True Then 'Drug1 
    '            ElseIf IsDBNull(dgvPrescriptions.Rows(i).Cells(8).Value) = True Then 'Drug2 
    '            ElseIf IsDBNull(dgvPrescriptions.Rows(i).Cells(9).Value) = True Then 'Drug3 

    '            Else
    '                cmd.Parameters.AddWithValue("@Ektelesis", If(dgvPrescriptions.Rows(i).Cells(0).Value, DBNull.Value))
    '                cmd.Parameters.AddWithValue("@InitDate", If(dgvPrescriptions.Rows(i).Cells(1).Value, DBNull.Value))
    '                cmd.Parameters.AddWithValue("@EndDate", If(dgvPrescriptions.Rows(i).Cells(2).Value, DBNull.Value))
    '                cmd.Parameters.AddWithValue("@Barcode", If(dgvPrescriptions.Rows(i).Cells(3).Value, DBNull.Value))
    '                cmd.Parameters.AddWithValue("@ProcessedDate", If(dgvPrescriptions.Rows(i).Cells(4).Value, DBNull.Value))
    '                cmd.Parameters.AddWithValue("@CustomerId", If(dgvPrescriptions.Rows(i).Cells(5).Value, DBNull.Value))
    '                cmd.Parameters.AddWithValue("@Drug1", If(dgvPrescriptions.Rows(i).Cells(7).Value, DBNull.Value))
    '                cmd.Parameters.AddWithValue("@Drug2", If(dgvPrescriptions.Rows(i).Cells(8).Value, DBNull.Value))
    '                cmd.Parameters.AddWithValue("@Drug3", If(dgvPrescriptions.Rows(i).Cells(9).Value, DBNull.Value))

    '                If ChangedOrExists = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvPrescriptions.Rows(i).Cells(6).Value, DBNull.Value))


    '                cmd.ExecuteNonQuery()

    '                'Ανανεώνει το Last Update
    '                DisplayLastUpdate()

    '                ' Ανανεώνει τη λίστα των πελατων, ξαναεπιλέγοντας τον τελευταίο πελάτη
    '                Dim SelectedCustomer As String = dgvCustomers.SelectedRows(0).Cells(0).Value
    '                GetCustomersList()
    '                Dim rowIndex As Integer = SearchDatagrid(dgvCustomers, SelectedCustomer)
    '                dgvCustomers.CurrentCell = dgvCustomers.Rows(rowIndex).Cells(0)


    '            End If

    '        ElseIf ChangedOrExists = "Error" Then

    '        End If

    '    End Using

    'End Sub



    Private Function SearchDatagrid(ByVal datagrid As DataGridView, ByVal key As String) As Integer
        Dim rowIndex As Integer = -1

        For Each row As DataGridViewRow In datagrid.Rows
            If row.Cells(0).Value.ToString().Equals(key) Then
                rowIndex = row.Index
                Exit For
            End If
        Next
        Return rowIndex
    End Function


    Private Sub PayWholeDebt()
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "INSERT INTO PharmacyCustomFiles.dbo.debts " &
                            "([CustomerId], [Date], [DebtDescription], [Ammount]) VALUES (@CustomerId, @Date, @DebtDescription, @Ammount)"

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@CustomerId", If(dgvCustomers.SelectedRows(0).Cells(1).Value, DBNull.Value))
            cmd.Parameters.AddWithValue("@Date", Today())
            cmd.Parameters.AddWithValue("@DebtDescription", "--- ΑΠΟΠΛΗΡΩΜΗ ---")
            cmd.Parameters.AddWithValue("@Ammount", CType(lblTotalCustomerDebt.Text, Decimal) * -1)

            cmd.ExecuteNonQuery()

        End Using

    End Sub


    Private Sub UpdateHairDies(ByVal i As Integer)
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists = CheckIfRecordChangedOrExists_Hairdies(i)

            If ChangedOrExists = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.Hairdies " &
                              "SET [CustomerId] = @CustomerId, [Date] = @Date, [HairDieDescription] =@HairDieDescription " &
                              "WHERE Id = @Id"

            ElseIf ChangedOrExists = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.Hairdies " &
                            "([CustomerId], [Date], [HairDieDescription]) VALUES (@CustomerId, @Date, @HairDieDescription)"

            End If

            If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

                Dim cmd As New SqlCommand(insertData, con)

                If (IsDBNull(dgvHairdiesList.Rows(i).Cells(1).EditedFormattedValue) = True Or dgvHairdiesList.Rows(i).Cells(1).EditedFormattedValue = "") And
                    (IsDBNull(dgvHairdiesList.Rows(i).Cells(0).EditedFormattedValue) = True Or dgvHairdiesList.Rows(i).Cells(0).EditedFormattedValue = "") Then

                Else

                    cmd.Parameters.AddWithValue("@CustomerId", If(dgvCustomers.SelectedRows(0).Cells(1).Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Date", If(dgvHairdiesList.Rows(i).Cells(0).EditedFormattedValue = "", DBNull.Value, CType(dgvHairdiesList.Rows(i).Cells(0).EditedFormattedValue, Date)))
                    cmd.Parameters.AddWithValue("@HairDieDescription", If(dgvHairdiesList.Rows(i).Cells(1).EditedFormattedValue, DBNull.Value))

                    If ChangedOrExists = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvHairdiesList.Rows(i).Cells(2).Value, DBNull.Value))

                    cmd.ExecuteNonQuery()

                    'Ανανεώνει το Last Update date
                    DisplayLastUpdate()

                    'Βρίσκει το Ιd του τελευταίου record και το βάζει στην αντίστοιχη θέση του Datagrid
                    'έτσι ώστε να μην βλέπει την τελευταία εγγραφή σαν νέα και να μην την ξαναγράφει !!!
                    dgvHairdiesList.Rows(i).Cells(2).Value = GetLastRecordId("hairdies")

                End If

            ElseIf ChangedOrExists = "Error" Then

            End If

        End Using

    End Sub



    Private Sub UpdateSold(ByVal i As Integer)
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists = CheckIfRecordChangedOrExists_FarmSold(i)

            If ChangedOrExists = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.FarmSold " &
                              "SET [MyDate] = @MyDate, [CodeReceipt] = @CodeReceipt, [Description] =  @Description, [SoldXondr130] = @SoldXondr130, " &
                                "[SoldXondr230] = @SoldXondr230, [SoldXondr650] = @SoldXondr650, [SoldLian130] = @SoldLian130, [SoldLian230] = @SoldLian230, " &
                                "[SoldLian650] = @SoldLian650, [InFPA] = @InFPA " &
                              "WHERE Id = @Id"

            ElseIf ChangedOrExists = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.FarmSold " &
                                "([MyDate], [CodeReceipt], [Description], [SoldXondr130], [SoldXondr230], [SoldXondr650], " &
                                "[SoldLian130], [SoldLian230], [SoldLian650], [InFPA]) " &
                            "VALUES (@MyDate, @CodeReceipt, @Description, @SoldXondr130, @SoldXondr230, @SoldXondr650, " &
                                "@SoldLian130, @SoldLian230, @SoldLian650,@InFPA)"

            End If

            If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

                Dim cmd As New SqlCommand(insertData, con)

                If IsDBNull(dgvAgoresSold.Rows(i).Cells(3).EditedFormattedValue) = True Or dgvAgoresSold.Rows(i).Cells(3).EditedFormattedValue = "" Then

                Else
                    cmd.Parameters.AddWithValue("@MyDate", If(dgvAgoresSold.Rows(i).Cells(1).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(1).EditedFormattedValue, Date)))
                    cmd.Parameters.AddWithValue("@CodeReceipt", If(dgvAgoresSold.Rows(i).Cells(2).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Description", If(dgvAgoresSold.Rows(i).Cells(3).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@SoldXondr130", If(dgvAgoresSold.Rows(i).Cells(4).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(4).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@SoldXondr230", If(dgvAgoresSold.Rows(i).Cells(5).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(5).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@SoldXondr650", If(dgvAgoresSold.Rows(i).Cells(6).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(6).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@SoldLian130", If(dgvAgoresSold.Rows(i).Cells(7).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(7).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@SoldLian230", If(dgvAgoresSold.Rows(i).Cells(8).EditedFormattedValue = "",
                                               DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(8).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@SoldLian650", If(dgvAgoresSold.Rows(i).Cells(9).EditedFormattedValue = "",
                                               DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(9).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@InFPA", If(dgvAgoresSold.Rows(i).Cells(10).EditedFormattedValue = "",
                                               DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(10).EditedFormattedValue, Decimal)))

                    If ChangedOrExists = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvAgoresSold.Rows(i).Cells(0).Value, DBNull.Value))

                    cmd.ExecuteNonQuery()

                End If

            ElseIf ChangedOrExists = "Error" Then

            End If

        End Using

    End Sub



    Private Sub UpdateAgores(ByVal i As Integer)
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists = CheckIfRecordChangedOrExists_FarmAgores(i)

            If ChangedOrExists = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.FarmAgores " &
                              "SET [MyDate] = @MyDate, [CodeReceipt] = @CodeReceipt, [Description] =  @Description, [Agores130] = @Agores130, " &
                                "[Agores230] = @Agores230, [Agores650] = @Agores650, [DapNoDisc] = @DapNoDisc, [DapWithDisc] = @DapWithDisc, " &
                                "[AgoresFPA] = @AgoresFPA, [DapFPA] = @DapFPA " &
                              "WHERE Id = @Id"

            ElseIf ChangedOrExists = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.FarmAgores " &
                                "([MyDate], [CodeReceipt], [Description], [Agores130], [Agores230], [Agores650], " &
                                "[DapNoDisc], [DapWithDisc], [AgoresFPA], [DapFPA]) " &
                            "VALUES (@MyDate, @CodeReceipt, @Description, @Agores130, @Agores230, @Agores650, " &
                                "@DapNoDisc, @DapWithDisc, @AgoresFPA,@DapFPA)"

            End If

            If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

                Dim cmd As New SqlCommand(insertData, con)

                If IsDBNull(dgvAgoresSold.Rows(i).Cells(3).EditedFormattedValue) = True Or dgvAgoresSold.Rows(i).Cells(3).EditedFormattedValue = "" Then

                Else
                    cmd.Parameters.AddWithValue("@MyDate", If(dgvAgoresSold.Rows(i).Cells(1).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(1).EditedFormattedValue, Date)))
                    cmd.Parameters.AddWithValue("@CodeReceipt", If(dgvAgoresSold.Rows(i).Cells(2).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Description", If(dgvAgoresSold.Rows(i).Cells(3).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Agores130", If(dgvAgoresSold.Rows(i).Cells(4).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(4).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@Agores230", If(dgvAgoresSold.Rows(i).Cells(5).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(5).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@Agores650", If(dgvAgoresSold.Rows(i).Cells(6).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(6).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@DapNoDisc", If(dgvAgoresSold.Rows(i).Cells(7).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(7).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@DapWithDisc", If(dgvAgoresSold.Rows(i).Cells(8).EditedFormattedValue = "",
                                               DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(8).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@AgoresFPA", If(dgvAgoresSold.Rows(i).Cells(9).EditedFormattedValue = "",
                                               DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(9).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@DapFPA", If(dgvAgoresSold.Rows(i).Cells(10).EditedFormattedValue = "",
                                               DBNull.Value, CType(dgvAgoresSold.Rows(i).Cells(10).EditedFormattedValue, Decimal)))

                    If ChangedOrExists = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvAgoresSold.Rows(i).Cells(0).Value, DBNull.Value))

                    cmd.ExecuteNonQuery()

                End If

            ElseIf ChangedOrExists = "Error" Then

            End If

        End Using

    End Sub


    'Private Sub UpdateExchangesPerRow(ByVal i As Integer)
    '    Dim insertData As String = ""

    '    Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
    '        con.Open()

    '        'If ChangedOrNew_Exchanges <> "NewRow" Then
    '        ChangedOrNew_Exchanges = CheckIfRecordChanged_Exchanges(i)
    '        MsgBox(ChangedOrNew_Exchanges)
    '        'End If



    '        If ChangedOrNew_Exchanges = "Changed" Then
    '            insertData = "UPDATE PharmacyCustomFiles.dbo.Exchanges " & _
    '                          "SET [DrugName] = @DrugName, [Qnt] =  @Qnt,[Xondr] = @Xondr, [RP] =  @RP, [MyDate] = @MyDate, [AP_Code]= @AP_Code " & _
    '                          "WHERE Id = @Id"
    '        ElseIf ChangedOrNew_Exchanges = "NewRow" Then
    '            insertData = "INSERT INTO PharmacyCustomFiles.dbo.Exchanges " & _
    '                                                    "([DrugName] ,[Xondr] ,[Qnt] ,[RP] ,[AP_Code], [MyDate], [Exch], [FromTo]) " & _
    '                                                    "VALUES (@DrugName, @Xondr, @Qnt, @RP, @AP_Code, @MyDate, @Exch, @FromTo)"
    '        End If

    '        If ChangedOrNew_Exchanges = "Changed" Or ChangedOrNew_Exchanges = "NewRow" Then

    '            Dim cmd As New SqlCommand(insertData, con)

    '            If ExchangesGivenOrTaken = "given" Then
    '                If IsDBNull(dgvGivenTo.Rows(i).Cells(1).Value) = True Or dgvGivenTo.Rows(i).Cells(1).Value = "" Then
    '                ElseIf IsDBNull(dgvGivenTo.Rows(i).Cells(3).Value) = True Or dgvGivenTo.Rows(i).Cells(3).Value = 0 Then
    '                Else
    '                    If ChangedOrNew_Exchanges = "NewRow" Then
    '                        MsgBox("EFV= " & dgvGivenTo.Rows(i).Cells(1).EditedFormattedValue & vbCrLf & "V= " & dgvGivenTo.Rows(i).Cells(1).Value)
    '                        cmd.Parameters.AddWithValue("@DrugName", If(dgvGivenTo.Rows(i).Cells(1).Value, DBNull.Value))
    '                        cmd.Parameters.AddWithValue("@Xondr", If(dgvGivenTo.Rows(i).Cells(3).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(3).EditedFormattedValue, Decimal)))
    '                        cmd.Parameters.AddWithValue("@Qnt", If(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue, Integer)))
    '                        cmd.Parameters.AddWithValue("@RP", If(dgvGivenTo.Rows(i).Cells(4).EditedFormattedValue = "", DBNull.Value, dgvGivenTo.Rows(i).Cells(4).EditedFormattedValue))
    '                        cmd.Parameters.AddWithValue("@MyDate", If(dgvGivenTo.Rows(i).Cells(6).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(6).EditedFormattedValue, DateTime)))
    '                        cmd.Parameters.AddWithValue("@AP_Code", If(dgvGivenTo.Rows(i).Cells(5).EditedFormattedValue = "", 0, CType(dgvGivenTo.Rows(i).Cells(5).EditedFormattedValue, Integer)))
    '                        cmd.Parameters.AddWithValue("@Exch", cbExchangers.Text)
    '                        cmd.Parameters.AddWithValue("@FromTo", 0)

    '                    ElseIf ChangedOrNew_Exchanges = "Changed" Then
    '                        cmd.Parameters.AddWithValue("@DrugName", If(dgvGivenTo.Rows(i).Cells(1).EditedFormattedValue, DBNull.Value))
    '                        cmd.Parameters.AddWithValue("@Xondr", If(dgvGivenTo.Rows(i).Cells(3).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(3).EditedFormattedValue, Decimal)))
    '                        cmd.Parameters.AddWithValue("@Qnt", If(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue, Integer)))
    '                        cmd.Parameters.AddWithValue("@RP", If(dgvGivenTo.Rows(i).Cells(4).EditedFormattedValue = "", DBNull.Value, dgvGivenTo.Rows(i).Cells(4).EditedFormattedValue))
    '                        cmd.Parameters.AddWithValue("@MyDate", If(dgvGivenTo.Rows(i).Cells(6).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(6).EditedFormattedValue, DateTime)))
    '                        cmd.Parameters.AddWithValue("@AP_Code", If(dgvGivenTo.Rows(i).Cells(5).EditedFormattedValue = "", 0, CType(dgvGivenTo.Rows(i).Cells(5).EditedFormattedValue, Integer)))
    '                        cmd.Parameters.AddWithValue("@Exch", cbExchangers.Text)
    '                        cmd.Parameters.AddWithValue("@FromTo", 0)
    '                        cmd.Parameters.AddWithValue("@Id", If(dgvGivenTo.Rows(i).Cells(0).Value, DBNull.Value))
    '                    End If

    '                    cmd.ExecuteNonQuery()

    '                End If

    '            ElseIf ExchangesGivenOrTaken = "taken" Then
    '                If IsDBNull(dgvTakenFrom.Rows(i).Cells(1).EditedFormattedValue) = True Or dgvTakenFrom.Rows(i).Cells(1).EditedFormattedValue = "" Then
    '                Else
    '                    cmd.Parameters.AddWithValue("@DrugName", If(dgvTakenFrom.Rows(i).Cells(1).EditedFormattedValue, DBNull.Value))
    '                    cmd.Parameters.AddWithValue("@Xondr", If(dgvTakenFrom.Rows(i).Cells(3).EditedFormattedValue = "", DBNull.Value, CType(dgvTakenFrom.Rows(i).Cells(3).EditedFormattedValue, Decimal)))
    '                    cmd.Parameters.AddWithValue("@Qnt", If(dgvTakenFrom.Rows(i).Cells(2).EditedFormattedValue = "", DBNull.Value, CType(dgvTakenFrom.Rows(i).Cells(2).EditedFormattedValue, Integer)))
    '                    cmd.Parameters.AddWithValue("@RP", If(dgvTakenFrom.Rows(i).Cells(4).EditedFormattedValue = "", DBNull.Value, dgvTakenFrom.Rows(i).Cells(4).EditedFormattedValue))
    '                    cmd.Parameters.AddWithValue("@MyDate", If(dgvTakenFrom.Rows(i).Cells(6).EditedFormattedValue = "", DBNull.Value, CType(dgvTakenFrom.Rows(i).Cells(6).EditedFormattedValue, DateTime)))
    '                    cmd.Parameters.AddWithValue("@AP_Code", If(dgvTakenFrom.Rows(i).Cells(5).EditedFormattedValue = "", 0, CType(dgvTakenFrom.Rows(i).Cells(5).EditedFormattedValue, Integer)))
    '                    cmd.Parameters.AddWithValue("@Exch", cbExchangers.Text)
    '                    cmd.Parameters.AddWithValue("@FromTo", 1)

    '                    If ChangedOrNew_Exchanges = "Changed" Then
    '                        cmd.Parameters.AddWithValue("@Id", If(dgvTakenFrom.Rows(i).Cells(0).Value, DBNull.Value))
    '                    End If

    '                    cmd.ExecuteNonQuery()

    '                End If
    '            End If

    '            UpdateExchangesTotalAndSums()

    '        ElseIf ChangedOrNew_Exchanges = "Error" Then

    '        End If

    '    End Using

    '    If ChangedOrNew_Exchanges = "NewRow" Then
    '        GetExchangesList("given")
    '        GetExchangesList("taken")
    '    End If

    'End Sub

    Private Sub UpdateParadrug(ByVal i As Integer, Optional ByVal mode As String = "")
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""
        Dim DrugName As String = dgvPricesParadrugs.Rows(i).Cells(0).EditedFormattedValue

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()
            If mode = "" Then
                ChangedOrNew_Paradrugs = CheckIfRecordChangedOrExists_Paradrugs(i)
            ElseIf mode = "noControl" Then
                ChangedOrNew_Paradrugs = "Changed"
            End If

            If ChangedOrNew_Paradrugs = "Error" Then
            ElseIf ChangedOrNew_Paradrugs = "NotChanged" Then
                Exit Sub
            End If

            If ChangedOrNew_Paradrugs = "Changed" Then
                insertData = "UPDATE PharmacyCustomFiles.dbo.PricesParadrugs " &
                              "SET [Name] = @Name, [Xondr] = @Xondr, [Lian] =  @Lian, [Notes] = @Notes, [AP_Code]= @AP_Code, [AP_ID]= @AP_ID, [Barcode]= @Barcode " &
                              "WHERE Id = @Id"
            ElseIf ChangedOrNew_Paradrugs = "NewRow" Then

                If CheckIfParadrugExists(DrugName) = False Then
                    insertData = "INSERT INTO PharmacyCustomFiles.dbo.PricesParadrugs " &
                                "([Name] ,[Xondr] ,[Lian] ,[Notes] ,[AP_Code], [AP_ID], [Barcode]) " &
                                "VALUES (@Name, @Xondr, @Lian, @Notes, @AP_Code, @AP_ID, @Barcode )"

                Else
                    MsgBox("Υπάρχει ήδη ένα προιον με το όνομα (" & DrugName & ") - ΚΑΤΑΧΩΡΗΣΗ ΑΔΥΝΑΤΗ ")
                    Exit Sub
                End If
            End If

            Dim cmd As New SqlCommand(insertData, con)

            ' Περνάει τις παραμέτρους του SQL 
            If ChangedOrNew_Paradrugs = "NewRow" Then

                cmd.Parameters.AddWithValue("@Name", If(DrugName, DBNull.Value))
                cmd.Parameters.AddWithValue("@Xondr", If(dgvPricesParadrugs.Rows(i).Cells(1).EditedFormattedValue = "", DBNull.Value, CType(dgvPricesParadrugs.Rows(i).Cells(1).EditedFormattedValue, Decimal)))
                cmd.Parameters.AddWithValue("@Lian", If(dgvPricesParadrugs.Rows(i).Cells(2).EditedFormattedValue = "", DBNull.Value, CType(dgvPricesParadrugs.Rows(i).Cells(2).EditedFormattedValue, Decimal)))
                cmd.Parameters.AddWithValue("@Notes", If(dgvPricesParadrugs.Rows(i).Cells(3).EditedFormattedValue, DBNull.Value))
                cmd.Parameters.AddWithValue("@AP_Code", If(dgvPricesParadrugs.Rows(i).Cells(4).EditedFormattedValue = "", 0, CType(dgvPricesParadrugs.Rows(i).Cells(4).EditedFormattedValue, Integer)))
                cmd.Parameters.AddWithValue("@AP_ID", If(dgvPricesParadrugs.Rows(i).Cells(6).EditedFormattedValue = "", 0, CType(dgvPricesParadrugs.Rows(i).Cells(6).EditedFormattedValue, Integer)))
                cmd.Parameters.AddWithValue("@Barcode", If(dgvPricesParadrugs.Rows(i).Cells(7).EditedFormattedValue, DBNull.Value))

            ElseIf ChangedOrNew_Paradrugs = "Changed" Then
                cmd.Parameters.AddWithValue("@Name", If(DrugName, DBNull.Value))
                cmd.Parameters.AddWithValue("@Xondr", If(dgvPricesParadrugs.Rows(i).Cells(1).EditedFormattedValue = "", DBNull.Value, CType(dgvPricesParadrugs.Rows(i).Cells(1).EditedFormattedValue, Decimal)))
                cmd.Parameters.AddWithValue("@Lian", If(dgvPricesParadrugs.Rows(i).Cells(2).EditedFormattedValue = "", DBNull.Value, CType(dgvPricesParadrugs.Rows(i).Cells(2).EditedFormattedValue, Decimal)))
                cmd.Parameters.AddWithValue("@Notes", If(dgvPricesParadrugs.Rows(i).Cells(3).EditedFormattedValue, DBNull.Value))
                cmd.Parameters.AddWithValue("@AP_Code", If(dgvPricesParadrugs.Rows(i).Cells(4).EditedFormattedValue = "", 0, CType(dgvPricesParadrugs.Rows(i).Cells(4).EditedFormattedValue, Integer)))
                cmd.Parameters.AddWithValue("@AP_ID", If(dgvPricesParadrugs.Rows(i).Cells(6).EditedFormattedValue = "", 0, CType(dgvPricesParadrugs.Rows(i).Cells(6).EditedFormattedValue, Integer)))
                cmd.Parameters.AddWithValue("@Barcode", If(dgvPricesParadrugs.Rows(i).Cells(7).EditedFormattedValue, DBNull.Value))
                cmd.Parameters.AddWithValue("@Id", If(dgvPricesParadrugs.Rows(i).Cells(5).Value, DBNull.Value))

            End If

            If dgvPricesParadrugs.Rows(i).Cells(0).EditedFormattedValue = "" Or
                CType(dgvPricesParadrugs.Rows(i).Cells(1).EditedFormattedValue, Decimal) = 0 Or
                CType(dgvPricesParadrugs.Rows(i).Cells(2).EditedFormattedValue, Decimal) = 0 Then

            Else
                cmd.ExecuteNonQuery()

                lblNewRecordAdded.Text = "ΕΓΓΡΑΦΗ"

                'Ανανεώνει το Last Update
                DisplayLastUpdate()

                dirty = False

                If ChangedOrNew_Paradrugs = "NewRow" Then
                    DatagridEdited = "PricesParadrug"
                    NewRowName = DrugName
                    tmrRerunDatagridV.Enabled = True
                End If
            End If

        End Using

    End Sub


    Private Sub UpdateExpirationList(ByVal i As Integer, ByVal c As Integer, Optional ByVal mode As String = "")
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""
        Dim month, myYear As Integer
        Dim category, name As String

        If rbParadrugs.Checked = True Then
            category = "ΠΑΡΑΦΑΡΜΑΚΑ"
        Else
            category = "ΦΑΡΜΑΚΑ"
        End If

        Try
            month = dgvExpirations.Rows(i).Cells(0).EditedFormattedValue
            myYear = dgvExpirations.Rows(i).Cells(1).EditedFormattedValue
            name = dgvPricesParadrugs.SelectedRows(0).Cells(0).Value
        Catch ex As Exception
            Exit Sub
        End Try

        If IsNumeric(month) = True And month > 0 And month < 13 And IsNumeric(myYear) = True Then
            If CType(myYear, Integer) > Year(Now()) + 25 Then
                MsgBox("Το έτος (" & myYear & ") δεν μπορεί να είναι μεγαλύτερο από " & (Year(Now()) + 25))
                Exit Sub
            ElseIf CType(myYear, Integer) < Year(Now()) Then
                MsgBox("Το έτος (" & myYear & ") δεν μπορεί να είναι μικρότερο από " & Year(Now()))
                Exit Sub
            End If
        Else
            Exit Sub
        End If

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()
            If mode = "" Then
                ChangedOrNew_Paradrugs = CheckIfRecordChangedOrExists_Expirations(i)
            ElseIf mode = "noControl" Then
                ChangedOrNew_Paradrugs = "Changed"
            End If



            If ChangedOrNew_Paradrugs = "Error" Then
            ElseIf ChangedOrNew_Paradrugs = "NotChanged" Then
                Exit Sub
            End If

            If ChangedOrNew_Paradrugs = "Changed" Then
                insertData = "UPDATE PharmacyCustomFiles.dbo.Expirations " &
                              "SET [Month] = @Month, [Year] = @Year, [ProductName] =  @ProductName, [Category] =@Category, [AP_ID] = @AP_ID, [AP_CODE]= @AP_CODE, [ParadrugId]= @ParadrugId " &
                              "WHERE Id = @Id"
            ElseIf ChangedOrNew_Paradrugs = "NewRow" Then
                If CheckIfExpirationExists() = False Then
                    insertData = "INSERT INTO PharmacyCustomFiles.dbo.Expirations " &
                                                                            "([Month] ,[Year] ,[ProductName] ,[Category], [AP_ID] ,[AP_CODE], [ParadrugId]) " &
                                                                            "VALUES (@Month, @Year, @ProductName,@Category, @AP_ID, @AP_CODE, @ParadrugId)"
                Else
                    Exit Sub
                End If

            End If



            Dim cmd As New SqlCommand(insertData, con)

            ' Περνάει τις παραμέτρους του SQL 
            cmd.Parameters.AddWithValue("@Month", month)
            cmd.Parameters.AddWithValue("@Year", myYear)
            cmd.Parameters.AddWithValue("@Category", category)

            If rbParadrugs.Checked = True Then
                cmd.Parameters.AddWithValue("@ProductName", name)
                cmd.Parameters.AddWithValue("@AP_ID", If(dgvPricesParadrugs.SelectedRows(0).Cells(6).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@AP_CODE", If(dgvPricesParadrugs.SelectedRows(0).Cells(4).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@ParadrugId", If(dgvPricesParadrugs.SelectedRows(0).Cells(5).Value, DBNull.Value))
            ElseIf rbDrugs.Checked = True Then
                cmd.Parameters.AddWithValue("@ProductName", dgvPricesParadrugs.SelectedRows(0).Cells(0).Value & " (" & dgvPricesParadrugs.SelectedRows(0).Cells(1).Value & ")")
                cmd.Parameters.AddWithValue("@AP_ID", If(dgvPricesParadrugs.SelectedRows(0).Cells(5).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@AP_CODE", If(dgvPricesParadrugs.SelectedRows(0).Cells(4).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@ParadrugId", If(dgvPricesParadrugs.SelectedRows(0).Cells(5).Value, DBNull.Value))
            End If


            If ChangedOrNew_Paradrugs = "NewRow" Then

            ElseIf ChangedOrNew_Paradrugs = "Changed" Then

                cmd.Parameters.AddWithValue("@Id", If(dgvExpirations.Rows(i).Cells(2).Value, DBNull.Value))

            End If
            cmd.ExecuteNonQuery()

            lblNewRecord_Exp.Text = "ΕΓΓΡΑΦΗ"

            'Ανανεώνει το Last Update
            'DisplayLastUpdate()

            ' Αν προσθέσαμε μια νέα έγγραφή ανανεώνει το datagrid
            'If ChangedOrNew_Paradrugs = "NewRow" Then
            'GetExpirationsList()
            'txtSearchPricesParadrugs.Text = dgvPricesParadrugs.Rows(i).Cells(0).Value
            'End If

            'Αν είμαστε στο Έτος και περνάμε barcode
            If rbByBarcode.Checked = True AndAlso c = 1 Then
                'MsgBox("OK")
                txtSearchPricesParadrugs.SelectAll()
                txtSearchPricesParadrugs.Focus()
            End If

            'End If

        End Using

    End Sub



    ' ΚΥΡΙΑ ΡΟΥΤΙΝΑ: Upsert Expiration με βάση yyMM (χωρίς rowIndex)
    Public Sub UpsertExpirationFromYYMM(ByVal yyMM As String)
        If String.IsNullOrWhiteSpace(yyMM) OrElse yyMM.Length <> 4 Then Exit Sub

        Dim yy As Integer, mm As Integer
        If Not Integer.TryParse(yyMM.Substring(0, 2), yy) Then Exit Sub
        If Not Integer.TryParse(yyMM.Substring(2, 2), mm) Then Exit Sub

        Dim yearFull As Integer = 2000 + yy
        If mm < 1 OrElse mm > 12 Then Exit Sub

        ' Προαιρετικοί φραγμοί (ίδιο ύφος με τα υπόλοιπα):
        If yearFull > Year(Now()) + 25 Then
            MsgBox("Το έτος (" & yearFull & ") δεν μπορεί να είναι μεγαλύτερο από " & (Year(Now()) + 25))
            Exit Sub
        ElseIf yearFull < Year(Now()) Then
            MsgBox("Το έτος (" & yearFull & ") δεν μπορεί να είναι μικρότερο από " & Year(Now()))
            Exit Sub
        End If

        ' Πρέπει να υπάρχει επιλεγμένη γραμμή στο dgvPricesParadrugs
        If dgvPricesParadrugs Is Nothing OrElse dgvPricesParadrugs.SelectedRows.Count = 0 Then Exit Sub

        Dim row = dgvPricesParadrugs.SelectedRows(0)

        ' ---- Προσαρμόζεις εδώ αν οι στήλες σου έχουν άλλα index ----
        Dim productName As Object = SafeCell(row, 0) ' ProductName
        Dim apCode As Object = SafeCell(row, 4)      ' AP_CODE
        Dim paradrugId As Object = SafeCell(row, 5)  ' ParadrugId
        Dim apId As Object = SafeCell(row, 5)        ' AP_ID
        ' ------------------------------------------------------------

        Dim category As String = If(rbParadrugs IsNot Nothing AndAlso rbParadrugs.Checked, "ΠΑΡΑΦΑΡΜΑΚΑ", "ΦΑΡΜΑΚΑ")

        ' Αν δεν έχεις ParadrugId (DBNull/Nothing), δεν μπορεί να γίνει μοναδικό upsert
        If paradrugId Is Nothing OrElse paradrugId Is DBNull.Value OrElse Not IsNumeric(paradrugId) Then
            MsgBox("Δεν βρέθηκε έγκυρο ParadrugId για ενημέρωση λήξης.")
            Exit Sub
        End If

        Dim sql As String =
        "IF EXISTS (SELECT 1 FROM [PharmacyCustomFiles].[dbo].[Expirations] " &
        "           WHERE ParadrugId=@ParadrugId AND [Month]=@Month AND [Year]=@Year) " &
        "BEGIN " &
        "   UPDATE [PharmacyCustomFiles].[dbo].[Expirations] " &
        "   SET [ProductName]=@ProductName, [Category]=@Category, [AP_ID]=@AP_ID, [AP_CODE]=@AP_CODE " &
        "   WHERE ParadrugId=@ParadrugId AND [Month]=@Month AND [Year]=@Year; " &
        "END " &
        "ELSE " &
        "BEGIN " &
        "   INSERT INTO [PharmacyCustomFiles].[dbo].[Expirations] " &
        "   ([Month],[Year],[ProductName],[Category],[AP_ID],[AP_CODE],[ParadrugId]) " &
        "   VALUES (@Month,@Year,@ProductName,@Category,@AP_ID,@AP_CODE,@ParadrugId); " &
        "END;"

        Using con As New SqlConnection(connectionstring)
            con.Open()
            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@Month", mm)
                cmd.Parameters.AddWithValue("@Year", yearFull)
                cmd.Parameters.AddWithValue("@ProductName", IfNullToDb(productName))
                cmd.Parameters.AddWithValue("@Category", category)
                cmd.Parameters.AddWithValue("@AP_ID", IfNullToDb(apId))
                cmd.Parameters.AddWithValue("@AP_CODE", IfNullToDb(apCode))
                cmd.Parameters.AddWithValue("@ParadrugId", Convert.ToInt32(paradrugId))
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ' --------- Helpers ---------

    ' Ασφαλής ανάγνωση κελιού: επιστρέφει Nothing αν λείπει η στήλη/τιμή
    Private Function SafeCell(ByVal row As DataGridViewRow, ByVal cellIndex As Integer) As Object
        Try
            Dim val = row.Cells(cellIndex).Value
            If val Is Nothing Then Return Nothing
            If TypeOf val Is String AndAlso String.IsNullOrWhiteSpace(CStr(val)) Then Return Nothing
            Return val
        Catch
            Return Nothing
        End Try
    End Function

    ' Μετατρέπει Nothing/"" σε DBNull, αλλιώς γυρνάει την τιμή όπως είναι
    Private Function IfNullToDb(ByVal value As Object) As Object
        If value Is Nothing Then Return DBNull.Value
        If TypeOf value Is String AndAlso String.IsNullOrWhiteSpace(CStr(value)) Then Return DBNull.Value
        Return value
    End Function



    Private Sub UpdateFPAOnExchanges()
        Dim insertData As String = ""
        lblFPAInfo.Text = ""

        ' Περνάει όλα τα ΦΠΑ από το Pharmacy2013C στις Ανταλλαγές
        lblFPAInfo.Text = "APOT-->EXCH"
        insertData = "UPDATE [PharmacyCustomFiles].[dbo].[Exchanges] " &
"SET FPA = T2.AP_FP_ID " &
                    "FROM Pharmacy2013C.dbo.APOTIKH as T2 " &
                    "WHERE Exchanges.AP_Code = T2.AP_CODE AND Exchanges.FPA Is NULL"
        ExecuteSQLTransact(insertData)

        ' Αντικαθιστά τα 1,2,3 με τους συντελεστές ΦΠΑ
        ' ΦΠΑ 6%
        lblFPAInfo.Text = "ΦΠΑ 6%"
        insertData = "UPDATE [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "SET FPA = 6 " &
                    "WHERE FPA = 3.0"
        ExecuteSQLTransact(insertData)

        ' ΦΠΑ 13%
        lblFPAInfo.Text = "ΦΠΑ 13%"
        insertData = "UPDATE [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "SET FPA = 13 " &
                    "WHERE FPA = 1.0"
        ExecuteSQLTransact(insertData)

        ' ΦΠΑ 24%
        lblFPAInfo.Text = "ΦΠΑ 24%"
        insertData = "UPDATE [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "SET FPA = 24 " &
                    "WHERE FPA = 2.0"
        ExecuteSQLTransact(insertData)

        ' ΦΠΑ 6% όπου δεν υπάρχει κωδικός φαρμάκου (παλιές εισαγωγές 2013-2014)
        lblFPAInfo.Text = "ΦΠΑ 6% on null"
        insertData = "UPDATE [PharmacyCustomFiles].[dbo].[Exchanges] " &
                   "SET FPA = 6 " &
                   "WHERE FPA is NULL"
        ExecuteSQLTransact(insertData)

        ' ΦΠΑ 6% όπου παλιά ήταν 6,5
        lblFPAInfo.Text = "ΦΠΑ 6,5% --> 6%"
        insertData = "UPDATE [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "SET FPA = 6 " &
                    "WHERE FPA = 6.5"
        ExecuteSQLTransact(insertData)

        lblFPAInfo.Text = ""


    End Sub


    Private Sub UpdateExchanges_Given(ByVal i As Integer)
        Dim insertData As String = ""
        Dim DrugFPA As Integer = 0


        'Dim qnt As Integer = 0

        '' Αν το quantity είναι <1000 τότε περνάει αλλιώς μπαίνει το 1
        'If IsNumeric(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue) = True Then
        '    If CType(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue, Double) < 1000 Then
        '        Qnt = CType(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue, Double)
        '    Else
        '        Qnt = 1
        '        dgvGivenTo.Rows(i).Cells(2).Value = 1
        '    End If
        'Else
        '    Qnt = 1
        '    dgvGivenTo.Rows(i).Cells(2).Value = 1
        'End If


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrNew_Exchanges = CheckIfRecordChanged_Exchanges(i)

            If ChangedOrNew_Exchanges = "Error" Then
            ElseIf ChangedOrNew_Exchanges = "NotChanged" Then
                Exit Sub
            End If
            'MsgBox(ChangedOrNew_Exchanges)


            If ChangedOrNew_Exchanges = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.Exchanges " &
                             "SET [DrugName] = @DrugName, [Qnt] =  @Qnt,[Xondr] = @Xondr, [RP] =  @RP, [FPA] =  @FPA  " &
                             "WHERE Id = @Id"
            ElseIf ChangedOrNew_Exchanges = "NewRow" Then
                insertData = "INSERT INTO PharmacyCustomFiles.dbo.Exchanges " &
                                                        "([DrugName] ,[Xondr] ,[Qnt] ,[RP] ,[AP_Code], [MyDate], [Exch], [FromTo], [FPA]) " &
                                                        "VALUES (@DrugName, @Xondr, @Qnt, @RP, @AP_Code, @MyDate, @Exch, @FromTo, @FPA)"
            End If

            Dim cmd As New SqlCommand(insertData, con)

            ' Αν δεν έχουμε καταχωρήσει όνομα φαρμάκου..
            If IsDBNull(dgvGivenTo.Rows(i).Cells(1).Value) = True Or dgvGivenTo.Rows(i).Cells(1).Value = "" Then
                ' ή τιμή ..
            ElseIf IsDBNull(dgvGivenTo.Rows(i).Cells(4).Value) = True Or dgvGivenTo.Rows(i).Cells(4).Value = 0 Then
                ' δεν κάνει τίποτα

                ' Αλλιώς ->
            Else
                ' Περνάει τις παραμέτρους του SQL 
                If ChangedOrNew_Exchanges = "NewRow" Then

                    cmd.Parameters.AddWithValue("@DrugName", If(dgvGivenTo.Rows(i).Cells(1).Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Xondr", If(dgvGivenTo.Rows(i).Cells(4).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(4).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@Qnt", qnt)
                    cmd.Parameters.AddWithValue("@RP", DBNull.Value)
                    cmd.Parameters.AddWithValue("@MyDate", If(dgvGivenTo.Rows(i).Cells(7).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(7).EditedFormattedValue, DateTime)))
                    cmd.Parameters.AddWithValue("@AP_Code", If(dgvGivenTo.Rows(i).Cells(6).EditedFormattedValue = "", 0, CType(dgvGivenTo.Rows(i).Cells(6).EditedFormattedValue, String)))
                    cmd.Parameters.AddWithValue("@Exch", cbExchangers.Text)
                    cmd.Parameters.AddWithValue("@FromTo", 0)
                    cmd.Parameters.AddWithValue("@FPA", If(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue, Decimal)))

                ElseIf ChangedOrNew_Exchanges = "Changed" Then

                    ' MsgBox("EFV= " & dgvGivenTo.Rows(i).Cells(1).EditedFormattedValue & vbCrLf & "V= " & dgvGivenTo.Rows(i).Cells(1).Value)

                    cmd.Parameters.AddWithValue("@DrugName", If(dgvGivenTo.Rows(i).Cells(1).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Xondr", If(dgvGivenTo.Rows(i).Cells(4).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(4).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@Qnt", CType(dgvGivenTo.Rows(i).Cells(3).EditedFormattedValue, Integer))
                    cmd.Parameters.AddWithValue("@RP", dgvGivenTo.Rows(i).Cells(5).Value)
                    cmd.Parameters.AddWithValue("@FPA", If(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue = "", DBNull.Value, CType(dgvGivenTo.Rows(i).Cells(2).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@Id", If(dgvGivenTo.Rows(i).Cells(0).Value, DBNull.Value))
                End If

                ' Σώζει τις αλλαγές
                Try
                    cmd.ExecuteNonQuery()
                Catch ex As Exception
                    If ex.Message.Contains("String or binary data would be truncated.") Then
                    Else
                        MsgBox("ERROR (UpdateExchanges_Given) !!!" & vbCrLf & ex.Message)
                    End If
                End Try


                ' Ανανεώνει τα σύνολα και τις τιμές
                UpdateExchangesTotalAndSums()

                'Ανανεώνει το Last Update
                DisplayLastUpdate()

                ' Αν προσθέσαμε μια νέα έγγραφή ανανεώνει το datagrid
                If ChangedOrNew_Exchanges = "NewRow" Then
                    GetExchangesList("given")
                End If
                'GetExchangesList("given")

                DisplayFPAPerCurrentIntervall()

            End If

        End Using

    End Sub

    Private Sub dgvExpirations_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvExpirations.CellClick

        If CurrentRowHasId() = 0 Then
            HideExpirationDatagrid(True)
        End If

    End Sub


    Private Sub dgvExpirations_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvExpirations.CellValidating


        'Δήλωση μεταβλητών
        Dim headerText As String = dgvExpirations.Columns(e.ColumnIndex).HeaderText
        Dim int As Integer

        If e.FormattedValue.ToString = "" Then Exit Sub

        'Αν είμαστε στο πεδίο "Μήνας"
        If headerText.Equals("Μήνας") Then
            Try
                ' και η καταχωρημένη τιμή που ΔΕΝ είναι ακέραιος αριθμός
                If Not Integer.TryParse(e.FormattedValue.ToString, int) Then
                    MessageBox.Show("To '" & e.FormattedValue.ToString & "' δεν είναι ακέραιος αριθμός!",
                                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    e.Cancel = True
                Else
                    'Αν δεν είναι μήνας 
                    If CType(e.FormattedValue.ToString, Integer) > 12 Or CType(e.FormattedValue.ToString, Integer) < 1 Then
                        MessageBox.Show("Δεν καταχωρήσατε έγκυρο μήνα (1-12)!",
                                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        e.Cancel = True
                    End If

                End If
            Catch ex As Exception
            End Try
        End If

        'Αν είμαστε στο πεδίο "Έτος"
        If headerText.Equals("Έτος") Then
            Try
                ' και η καταχωρημένη τιμή που ΔΕΝ είναι ακέραιος αριθμός
                If Not Integer.TryParse(e.FormattedValue.ToString, int) Then
                    MessageBox.Show("To '" & e.FormattedValue.ToString & "' δεν είναι ακέραιος αριθμός!",
                                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    e.Cancel = True
                    'Else
                    '    'Αν δεν είναι έγκυρο έτος 
                    '    If CType(e.FormattedValue.ToString, Integer) > 2050 Or CType(e.FormattedValue.ToString, Integer) < 2013 Then
                    '        MessageBox.Show("Δεν καταχωρήσατε έγκυρο έτος (2013-2050)!", _
                    '                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    '        e.Cancel = True
                    '    End If

                End If
            Catch ex As Exception
            End Try

        End If

    End Sub





    Private Sub dgvTakenFrom_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvTakenFrom.CellValidating


        'Δήλωση μεταβλητών
        Dim headerText As String = dgvTakenFrom.Columns(e.ColumnIndex).HeaderText
        Dim int As Integer

        'Αν είμαστε στο πεδίο "Τεμάχια"
        If headerText.Equals("Ποσ") Then
            Try
                ' και η καταχωρημένη τιμή που ΔΕΝ είναι ακέραιος αριθμός
                If Not Integer.TryParse(e.FormattedValue.ToString, int) Then
                    MessageBox.Show("To '" & e.FormattedValue.ToString & "' δεν είναι ακέραιος αριθμός!",
                                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    e.Cancel = True
                Else

                    'Αν η υπόσοτητα υπερβαίνει το 999 
                    If CType(e.FormattedValue.ToString, Integer) > 999 Then
                        MessageBox.Show("H ποσότητα είναι πολύ μεγάλη!",
                                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        e.Cancel = True
                    Else
                        Qnt = CType(e.FormattedValue.ToString, Integer)
                    End If

                End If
            Catch ex As Exception
            End Try

        End If

    End Sub



    Private Sub dgvGivenTo_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvGivenTo.CellValidating


        'Δήλωση μεταβλητών
        Dim headerText As String = dgvGivenTo.Columns(e.ColumnIndex).HeaderText
        Dim int As Integer

        'Αν είμαστε στο πεδίο "Τεμάχια"
        If headerText.Equals("Ποσ") Then
            Try
                ' και η καταχωρημένη τιμή που ΔΕΝ είναι ακέραιος αριθμός
                If Not Integer.TryParse(e.FormattedValue.ToString, int) Then
                    MessageBox.Show("To '" & e.FormattedValue.ToString & "' δεν είναι ακέραιος αριθμός!",
                                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    e.Cancel = True
                Else

                    'Αν η υπόσοτητα υπερβαίνει το 999 
                    If CType(e.FormattedValue.ToString, Integer) > 999 Then
                        MessageBox.Show("H ποσότητα είναι πολύ μεγάλη!",
                                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        e.Cancel = True
                    Else
                        Qnt = CType(e.FormattedValue.ToString, Integer)
                    End If

                End If
            Catch ex As Exception
            End Try

        End If

    End Sub



    Private Sub UpdateExchanges_Taken(ByVal i As Integer)
        Dim insertData As String = ""
        'Dim qnt As Integer = 0
        'Try
        '    '' Αν το quantity είναι <1000 τότε περνάει αλλιώς μπαίνει το 1
        '    If IsNumeric(dgvTakenFrom.Rows(i).Cells(2).EditedFormattedValue) = True Then
        '        If CType(dgvTakenFrom.Rows(i).Cells(2).EditedFormattedValue, Double) < 1000 Then
        '            Qnt = CType(dgvTakenFrom.Rows(i).Cells(2).EditedFormattedValue, Double)
        '        Else
        '            Qnt = 1
        '            dgvTakenFrom.Rows(i).Cells(2).Value = 1
        '        End If
        '    Else
        '        Qnt = 1
        '        dgvTakenFrom.Rows(i).Cells(2).Value = 1
        '    End If
        'Catch ex As Exception
        '    MsgBox("Λάθος με την καταχώρηση δεδομένων!")
        'End Try


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrNew_Exchanges = CheckIfRecordChanged_Exchanges(i)

            If ChangedOrNew_Exchanges = "Error" Then
            ElseIf ChangedOrNew_Exchanges = "NotChanged" Then
                Exit Sub
            End If
            'MsgBox(ChangedOrNew_Exchanges)

            If ChangedOrNew_Exchanges = "Changed" Then
                insertData = "UPDATE PharmacyCustomFiles.dbo.Exchanges " &
                             "SET [DrugName] = @DrugName, [Qnt] =  @Qnt, [Xondr] = @Xondr, [RP] =  @RP, [FPA] =  @FPA " &
                             "WHERE Id = @Id"
            ElseIf ChangedOrNew_Exchanges = "NewRow" Then
                insertData = "INSERT INTO PharmacyCustomFiles.dbo.Exchanges " &
                                                        "([DrugName] ,[Xondr] ,[Qnt] ,[RP] ,[AP_Code], [MyDate], [Exch], [FromTo], [FPA]) " &
                                                        "VALUES (@DrugName, @Xondr, @Qnt, @RP, @AP_Code, @MyDate, @Exch, @FromTo, @FPA)"
            End If

            Dim cmd As New SqlCommand(insertData, con)

            ' Αν δεν έχουμε καταχωρήσει όνομα φαρμάκου..
            If IsDBNull(dgvTakenFrom.Rows(i).Cells(1).Value) = True Or dgvTakenFrom.Rows(i).Cells(1).Value = "" Then
                ' ή τιμή ..
            ElseIf IsDBNull(dgvTakenFrom.Rows(i).Cells(4).Value) = True Or dgvTakenFrom.Rows(i).Cells(4).Value = 0 Then
                ' δεν κάνει τίποτα

                ' Αλλιώς ->
            Else
                ' Περνάει τις παραμέτρους του SQL 
                If ChangedOrNew_Exchanges = "NewRow" Then

                    cmd.Parameters.AddWithValue("@DrugName", If(dgvTakenFrom.Rows(i).Cells(1).Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Xondr", If(dgvTakenFrom.Rows(i).Cells(4).EditedFormattedValue = "", DBNull.Value, CType(dgvTakenFrom.Rows(i).Cells(4).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@FPA", If(dgvTakenFrom.Rows(i).Cells(2).EditedFormattedValue = "", 0, CType(dgvTakenFrom.Rows(i).Cells(2).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@Qnt", Qnt)
                    cmd.Parameters.AddWithValue("@RP", DBNull.Value)
                    cmd.Parameters.AddWithValue("@MyDate", If(dgvTakenFrom.Rows(i).Cells(7).EditedFormattedValue = "", DBNull.Value, CType(dgvTakenFrom.Rows(i).Cells(7).EditedFormattedValue, DateTime)))
                    cmd.Parameters.AddWithValue("@AP_Code", If(dgvTakenFrom.Rows(i).Cells(6).EditedFormattedValue = "", 0, CType(dgvTakenFrom.Rows(i).Cells(6).EditedFormattedValue, String)))
                    cmd.Parameters.AddWithValue("@Exch", cbExchangers.Text)
                    cmd.Parameters.AddWithValue("@FromTo", 1)

                ElseIf ChangedOrNew_Exchanges = "Changed" Then

                    ' MsgBox("EFV= " & dgvGivenTo.Rows(i).Cells(1).EditedFormattedValue & vbCrLf & "V= " & dgvGivenTo.Rows(i).Cells(1).Value)

                    cmd.Parameters.AddWithValue("@DrugName", If(dgvTakenFrom.Rows(i).Cells(1).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@FPA", If(dgvTakenFrom.Rows(i).Cells(2).EditedFormattedValue = "", 0, CType(dgvTakenFrom.Rows(i).Cells(2).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@Xondr", If(dgvTakenFrom.Rows(i).Cells(4).EditedFormattedValue = "", DBNull.Value, CType(dgvTakenFrom.Rows(i).Cells(4).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@Qnt", CType(dgvTakenFrom.Rows(i).Cells(3).EditedFormattedValue, Integer))
                    cmd.Parameters.AddWithValue("@RP", dgvTakenFrom.Rows(i).Cells(5).Value)
                    cmd.Parameters.AddWithValue("@Id", If(dgvTakenFrom.Rows(i).Cells(0).Value, DBNull.Value))
                End If

                ' Σώζει τις αλλαγές
                Try
                    cmd.ExecuteNonQuery()
                Catch ex As Exception
                    If ex.Message.Contains("String or binary data would be truncated.") Then
                    Else
                        MsgBox("ERROR (UpdateExchanges_Given) !!!" & vbCrLf & ex.Message & vbCrLf & "RP= '" & dgvTakenFrom.Rows(i).Cells(5).Value & "'" & vbCrLf & "Drugname= '" & dgvTakenFrom.Rows(i).Cells(1).Value & "'")
                    End If

                End Try


                ' Ανανεώνει τα σύνολα και τις τιμές
                UpdateExchangesTotalAndSums()

                'Ανανεώνει το Last Update
                DisplayLastUpdate()

                ' Αν προσθέσαμε μια νέα έγγραφή ανανεώνει το datagrid
                'GetExchangesList("taken")
                If ChangedOrNew_Exchanges = "NewRow" Then
                    GetExchangesList("taken")
                End If

            End If

        End Using

    End Sub

    Private Sub dgvPricesParadrugs_DataError(ByVal sender As Object, ByVal e As DataGridViewDataErrorEventArgs)

        ' If the data source raises an exception when a cell value is  
        ' commited, display an error message. 
        If e.Exception IsNot Nothing AndAlso
            e.Context = DataGridViewDataErrorContexts.Commit Then

            MessageBox.Show("CustomerID value must be unique.")

        End If

    End Sub



    Private Sub DeleteExchangesFrom2()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvExchangeFrom2.Rows(dgvExchangeFrom2.SelectedRows(0).Index).Cells(9).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.ExchangesMaster " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Do you want to delete row # " & id & " ?", "Delete", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using


    End Sub

    Private Sub DeleteExchangesMulti(ByVal mode As String)
        Dim dgv As DataGridView = If(mode = "given", dgvGivenTo, dgvTakenFrom)

        ' Κλείσε τυχόν edit για να “γράφουν” σωστά οι τιμές
        dgv.EndEdit()

        ' Μάζεψε όλα τα επιλεγμένα Id
        Dim ids As New List(Of Integer)
        For Each r As DataGridViewRow In dgv.SelectedRows
            If Not r.IsNewRow Then
                Dim v = r.Cells(0).Value
                If v IsNot Nothing AndAlso Not IsDBNull(v) Then
                    ids.Add(CInt(v))
                End If
            End If
        Next

        ' Αν ο χρήστης έχει μόνο τρέχουσα γραμμή επιλεγμένη (χωρίς SelectedRows), πάρε κι αυτήν
        If ids.Count = 0 AndAlso dgv.CurrentRow IsNot Nothing AndAlso Not dgv.CurrentRow.IsNewRow Then
            Dim v = dgv.CurrentRow.Cells(0).Value
            If v IsNot Nothing AndAlso Not IsDBNull(v) Then ids.Add(CInt(v))
        End If

        If ids.Count = 0 Then
            MessageBox.Show("Δεν βρέθηκαν έγκυρες επιλεγμένες εγγραφές για διαγραφή.", "Διαγραφή", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("Θέλετε να διαγράψετε " & ids.Count & " επιλεγμένες εγγραφές;", "Διαγραφή", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Using con As New SqlClient.SqlConnection(connectionstring)
            con.Open()
            Dim tr = con.BeginTransaction()
            Try
                Using cmd As New SqlClient.SqlCommand("DELETE FROM PharmacyCustomFiles.dbo.Exchanges WHERE Id = @Id", con, tr)
                    cmd.Parameters.Add("@Id", SqlDbType.Int)
                    For Each id In ids
                        cmd.Parameters("@Id").Value = id
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            Catch ex As Exception
                Try : tr.Rollback() : Catch : End Try
                MessageBox.Show("Σφάλμα στη διαγραφή εγγραφών: " & ex.Message, "Σφάλμα", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub


    Private Sub DeleteExchangesTo2()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvExchangeTo2.Rows(dgvExchangeTo2.SelectedRows(0).Index).Cells(9).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.ExchangesMaster " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Do you want to delete row # " & id & " ?", "Delete", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using


    End Sub


    Private Sub DeletePricesParadrugs()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvPricesParadrugs.Rows(dgvPricesParadrugs.SelectedRows(0).Index).Cells(5).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.PricesParadrugs " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using


    End Sub


    Private Sub DeleteSuppliers()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvAgoresSold.Rows(dgvAgoresSold.SelectedRows(0).Index).Cells(0).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            If cbAgoresOrSold.Text = "Έσοδα (Πωλήσεις)" Then
                insertData = "DELETE FROM PharmacyCustomFiles.dbo.FarmSold " &
                                        "WHERE Id = @Id"
            ElseIf cbAgoresOrSold.Text = "Έξοδα (Δαπάνες)" Then
                insertData = "DELETE FROM PharmacyCustomFiles.dbo.FarmAgores " &
                                       "WHERE Id = @Id"
            End If


            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using


    End Sub


    Private Sub DeleteExchanges(ByVal mode As String)
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            If mode = "given" Then
                id = dgvGivenTo.Rows(dgvGivenTo.SelectedRows(0).Index).Cells(0).Value
            ElseIf mode = "taken" Then
                id = dgvTakenFrom.Rows(dgvTakenFrom.SelectedRows(0).Index).Cells(0).Value
            End If

        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.Exchanges " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using

    End Sub

    Private Sub DeleteTameiaGiven()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvTameiaGiven.SelectedRows(0).Cells(5).Value
        Catch ex As Exception

        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.TameiaPaid " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using

    End Sub

    Private Sub DeleteCustomers()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvCustomers.Rows(dgvCustomers.SelectedRows(0).Index).Cells(1).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.Customers " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using

    End Sub


    Private Sub DeleteDebts()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvDebtsList.Rows(dgvDebtsList.SelectedRows(0).Index).Cells(3).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.Debts " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()

                GetCustomersList()

            End If
        End Using

    End Sub

    Private Sub DeleteDrugsOnLoan()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvDrugsOnLoan.Rows(dgvDrugsOnLoan.SelectedRows(0).Index).Cells(6).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.DrugsOnLoan " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()

                'GetCustomersList()

            End If
        End Using

    End Sub


    Private Sub DeletePrescriptions()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvPrescriptions.Rows(dgvPrescriptions.SelectedRows(0).Index).Cells(6).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.Prescriptions " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()

            End If
        End Using

    End Sub


    Private Sub DeleteTameiaAsked()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvTameiaAsked.Rows(dgvTameiaAsked.SelectedRows(0).Index).Cells(8).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.TameiaAsked " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using

    End Sub



    Private Sub DeletePhones()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvPhones.Rows(dgvPhones.SelectedRows(0).Index).Cells(5).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.Phonebook " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using

    End Sub


    Private Sub DeleteHairdies()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvHairdiesList.Rows(dgvHairdiesList.SelectedRows(0).Index).Cells(2).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.Hairdies " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη καταχώρηση # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using

    End Sub


    'Private Sub BackupDatabaseFiles()
    '    Dim sql As String = ""
    '    Dim folderDB As String = txtSourceDB.Text
    '    Dim folderVSFiles As String = txtSourceFolderVS.Text
    '    Dim folderUSB As String = txtDestinationDrive.Text

    '    Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)

    '        ' Βγάζει τα database offline
    '        lstMessage.Items.Clear()
    '        lstMessage.Items.Add("Setting databases offline..")
    '        lstMessage.Refresh()

    '        con.Open()
    '        sql = "ALTER DATABASE [" & strDB1 & "] SET Offline WITH ROLLBACK IMMEDIATE " & _
    '            "ALTER DATABASE [" & strDB2 & "] SET Offline WITH ROLLBACK IMMEDIATE "
    '        Dim cmd As New SqlCommand(sql, con)
    '        cmd.ExecuteNonQuery()
    '        'con.Close()

    '        ' Αν δεν υπάρχει το directory τότε το δημιουργεί
    '        Try
    '            If (Not System.IO.Directory.Exists(folderUSB)) Then
    '                lstMessage.Items.Add("Creating target directory on usb..")
    '                lstMessage.Refresh()
    '                System.IO.Directory.CreateDirectory(folderUSB)
    '            Else
    '                lstMessage.Items.Add("Target directory exists..")
    '                lstMessage.Refresh()
    '            End If
    '        Catch ex As Exception
    '            lstMessage.Items.Add("!!! Error !!! ")
    '            lstMessage.Refresh()
    '        End Try


    '        ' Αντιγράφει τα database
    '        lstMessage.Items.Add("Copying databases to usb..")
    '        lstMessage.Refresh()
    '        Try
    '            System.IO.File.Copy(folderDB & "\" & txtDB1.Text & ".mdf", folderUSB & "\" & txtDB1.Text & ".mdf", True)
    '            System.IO.File.Copy(folderDB & "\" & txtDB1.Text & ".ldf", folderUSB & "\" & txtDB1.Text & ".ldf", True)
    '            System.IO.File.Copy(folderDB & "\" & txtDB2.Text & ".mdf", folderUSB & "\" & txtDB2.Text & ".mdf", True)
    '            System.IO.File.Copy(folderDB & "\" & txtDB2.Text & "_log.ldf", folderUSB & "\" & txtDB2.Text & "_log.ldf", True)
    '        Catch ex As Exception
    '            lstMessage.Items.Add("!!! Error !!! ")
    '            lstMessage.Refresh()
    '        End Try

    '        ' Αντιγράφει τα folder με τα αρχεία του VS
    '        lstMessage.Items.Add("Copying VS files to usb..")
    '        lstMessage.Refresh()
    '        Try
    '            My.Computer.FileSystem.CopyDirectory(folderVSFiles, folderUSB & "\Pharmacy", True)
    '        Catch ex As Exception
    '            lstMessage.Items.Add("!!! Error !!! ")
    '            lstMessage.Refresh()
    '        End Try

    '        ' Βγάζει τα database online
    '        lstMessage.Items.Add("Setting databases online..")
    '        lstMessage.Refresh()
    '        'con.Open()
    '        sql = "ALTER DATABASE [" & strDB1 & "] SET Online " & _
    '             "ALTER DATABASE [" & strDB2 & "] SET Online"
    '        cmd = New SqlCommand(sql, con)
    '        cmd.ExecuteNonQuery()
    '        'con.Close()

    '        lstMessage.Items.Add("OK")
    '        lstMessage.Items.Add("-------------")
    '        lstMessage.Refresh()

    '    End Using


    'End Sub

    Private Sub BackUpDatabaseAtStarting()
        If My.Computer.Name = "FARMAKEIO" Or My.Computer.Name = "DESKTOP-T7HMABG" Then
            txtDestinationDrive.Text = "G:\PharmacyBackup\" & Today.Day.ToString & "-" & Today.Month.ToString & "-" & Today.Year.ToString
            If Directory.Exists(txtDestinationDrive.Text) Then
                If MsgBox("Έχω ήδη κρατήσει αντίγραφο των Database για σήμερα, θέλετε να ξαναπάρω;", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                    ' Παίρνει αντίγραφο των Database
                    frmDatabaseBackupProgression.Show()
                    BackupDatabaseFiles()
                Else
                    Exit Sub
                End If
            Else
                If MsgBox("Daily backup of databases, please press OK to proceed", MsgBoxStyle.OkOnly) = MsgBoxResult.Ok Then
                    ' Παίρνει αντίγραφο των Database
                    frmDatabaseBackupProgression.Show()
                    BackupDatabaseFiles()
                End If
            End If

        End If

    End Sub





    Private Sub BackupDatabaseFiles()
        Dim sql As String = ""
        Dim folderDB As String = txtSourceDB.Text
        Dim folderVSFiles As String = txtSourceFolderVS.Text
        Dim folderUSB As String = txtDestinationDrive.Text
        Dim InitTime As DateTime = Now()
        Dim TimeDiff As Integer = 0

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)

            ' Βγάζει τα database offline
            lstMessage.Items.Add("Setting databases offline... ")
            lstIndex += 1
            lstMessage.Refresh()

            con.Open()
            sql = "ALTER DATABASE [" & strDB1 & "] SET Offline WITH ROLLBACK IMMEDIATE " &
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
                lstMessage.Items.Add("   " & txtDB1.Text & ".mdf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.CopyFile(folderDB & "\" & txtDB1.Text & ".mdf", folderUSB & "\" & txtDB1.Text & ".mdf", FileIO.UIOption.OnlyErrorDialogs)
                VerifyDB(txtDB1.Text & ".mdf", lstIndex)

                lstMessage.Items.Add("   " & txtDB1.Text & ".ldf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.CopyFile(folderDB & "\" & txtDB1.Text & ".ldf", folderUSB & "\" & txtDB1.Text & ".ldf", FileIO.UIOption.OnlyErrorDialogs)
                VerifyDB(txtDB1.Text & ".ldf", lstIndex)

                lstMessage.Items.Add("   " & txtDB2.Text & ".mdf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.CopyFile(folderDB & "\" & txtDB2.Text & ".mdf", folderUSB & "\" & txtDB2.Text & ".mdf", FileIO.UIOption.OnlyErrorDialogs)
                VerifyDB(txtDB2.Text & ".mdf", lstIndex)

                lstMessage.Items.Add("   " & txtDB2.Text & "_log.ldf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.CopyFile(folderDB & "\" & txtDB2.Text & "_log.ldf", folderUSB & "\" & txtDB2.Text & "_log.ldf", FileIO.UIOption.AllDialogs)
                VerifyDB(txtDB2.Text & "_log.ldf", lstIndex)

            Catch ex As Exception
                lstMessage.Items.Add("!!! Error !!! ")
                lstMessage.Refresh()
            End Try

            ' Βγάζει τα database online
            lstMessage.Items.Add("Setting databases online... ")
            lstIndex += 1
            lstMessage.Refresh()

            sql = "ALTER DATABASE [" & strDB1 & "] SET Online " &
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


    End Sub


    Private Sub RestoreDatabaseFiles()
        Dim sql As String = ""
        Dim folderDB As String = txtSourceDB.Text
        Dim folderVSFiles As String = txtSourceFolderVS.Text
        Dim folderUSB As String = txtDestinationDrive.Text
        Dim InitTime As DateTime = Now()
        Dim TimeDiff As Integer = 0

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)

            ' Βγάζει τα database offline
            lstMessage.Items.Add("Setting databases offline... ")
            lstIndex += 1
            lstMessage.Refresh()

            con.Open()
            sql = "ALTER DATABASE [" & strDB1 & "] SET Offline WITH ROLLBACK IMMEDIATE " &
                "ALTER DATABASE [" & strDB2 & "] SET Offline WITH ROLLBACK IMMEDIATE "
            Dim cmd As New SqlCommand(sql, con)
            cmd.ExecuteNonQuery()

            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()

            '' Αν δεν υπάρχει το directory τότε το δημιουργεί
            'Try
            '    If (Not System.IO.Directory.Exists(folderUSB)) Then
            '        lstMessage.Items.Add("Target dir... Creating... ")
            '        lstIndex += 1
            '        lstMessage.Refresh()
            '        System.IO.Directory.CreateDirectory(folderUSB)
            '        lstMessage.Items(lstIndex) &= "OK"
            '        lstMessage.Refresh()
            '    Else
            '        lstMessage.Items.Add("Target dir... Deleting... ")
            '        lstIndex += 1
            '        lstMessage.Refresh()
            '        My.Computer.FileSystem.DeleteDirectory(folderUSB, FileIO.DeleteDirectoryOption.DeleteAllContents)
            '        lstMessage.Items(lstIndex) &= "Recreating... "
            '        lstMessage.Refresh()
            '        My.Computer.FileSystem.CreateDirectory(folderUSB)
            '        lstMessage.Items(lstIndex) &= "OK"
            '        lstMessage.Refresh()
            '    End If
            'Catch ex As Exception
            '    lstMessage.Items.Add("!!! Error !!! ")
            '    lstIndex += 1
            '    lstMessage.Refresh()
            'End Try


            ' Αντιγράφει τα database
            lstMessage.Items.Add("Copying databases to HD... ")
            lstIndex += 1
            lstMessage.Refresh()

            Try
                lstMessage.Items.Add("   " & txtDB1.Text & ".mdf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.DeleteFile(folderDB & "\" & txtDB1.Text & ".mdf")
                My.Computer.FileSystem.CopyFile(folderUSB & "\" & txtDB1.Text & ".mdf", folderDB & "\" & txtDB1.Text & ".mdf", FileIO.UIOption.AllDialogs)
                VerifyDB(txtDB1.Text & ".mdf", lstIndex)

                lstMessage.Items.Add("   " & txtDB1.Text & ".ldf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.DeleteFile(folderDB & "\" & txtDB1.Text & ".ldf")
                My.Computer.FileSystem.CopyFile(folderUSB & "\" & txtDB1.Text & ".ldf", folderDB & "\" & txtDB1.Text & ".ldf", FileIO.UIOption.AllDialogs)
                VerifyDB(txtDB1.Text & ".ldf", lstIndex)

                lstMessage.Items.Add("   " & txtDB2.Text & ".mdf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.DeleteFile(folderDB & "\" & txtDB2.Text & ".mdf")
                My.Computer.FileSystem.CopyFile(folderUSB & "\" & txtDB2.Text & ".mdf", folderDB & "\" & txtDB2.Text & ".mdf", FileIO.UIOption.AllDialogs)
                VerifyDB(txtDB2.Text & ".mdf", lstIndex)

                lstMessage.Items.Add("   " & txtDB2.Text & "_log.ldf... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.DeleteFile(folderDB & "\" & txtDB2.Text & "_log.ldf")
                My.Computer.FileSystem.CopyFile(folderUSB & "\" & txtDB2.Text & "_log.ldf", folderDB & "\" & txtDB2.Text & "_log.ldf", FileIO.UIOption.AllDialogs)
                VerifyDB(txtDB2.Text & "_log.ldf", lstIndex)

            Catch ex As Exception
                lstMessage.Items.Add("!!! Error !!! ")
                lstMessage.Refresh()
            End Try

            ' Βγάζει τα database online
            lstMessage.Items.Add("Setting databases online... ")
            lstIndex += 1
            lstMessage.Refresh()

            sql = "ALTER DATABASE [" & strDB1 & "] SET Online " &
                 "ALTER DATABASE [" & strDB2 & "] SET Online"
            cmd = New SqlCommand(sql, con)
            cmd.ExecuteNonQuery()

            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()

            TimeDiff = DateDiff(DateInterval.Second, InitTime, Now())

            lstMessage.Items.Add("Restore of Databases to HD completed in " & (TimeDiff \ 60) & ":" & ((TimeDiff / 60) - (TimeDiff \ 60)) * 60)
            lstMessage.Items.Add("-------------------------------------")
            lstIndex += 2
            lstMessage.Refresh()

        End Using


    End Sub


    Private Sub VerifyDB(ByVal db As String, ByVal lstIndex As Double)
        Dim infoReaderUSB, infoReaderHD As System.IO.FileInfo
        Dim folderDB As String = txtSourceDB.Text
        Dim folderVSFiles As String = txtSourceFolderVS.Text
        Dim folderUSB As String = txtDestinationDrive.Text

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





    Private Sub BackupVisualBasicFiles()
        Dim sql As String = ""
        Dim folderDB As String = txtSourceDB.Text
        Dim folderVSFiles As String = txtSourceFolderVS.Text
        Dim folderUSB As String = txtDestinationDrive.Text
        Dim InitTime As DateTime = Now()

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)

            'Αν δεν υπάρχει το directory τότε το δημιουργεί
            Try
                If (Not System.IO.Directory.Exists(folderUSB & "/Pharmacy")) Then
                    lstMessage.Items.Add("Target dir..Creating..")
                    lstIndex += 1
                    lstMessage.Refresh()
                    System.IO.Directory.CreateDirectory(folderUSB & "/Pharmacy")
                    lstMessage.Items(lstIndex) &= "OK"
                    lstMessage.Refresh()
                Else
                    lstMessage.Items.Add("Target dir..Deleting..")
                    lstIndex += 1
                    lstMessage.Refresh()
                    My.Computer.FileSystem.DeleteDirectory(folderUSB & "/Pharmacy", FileIO.DeleteDirectoryOption.DeleteAllContents)
                    lstMessage.Items(lstIndex) &= "Recreating.."
                    lstMessage.Refresh()
                    My.Computer.FileSystem.CreateDirectory(folderUSB & "/Pharmacy")
                    lstMessage.Items(lstIndex) &= "OK"
                    lstMessage.Refresh()
                End If
            Catch ex As Exception
                lstMessage.Items.Add("!!! Error !!! ")
                lstIndex += 1
                lstMessage.Refresh()
            End Try

            ' Αντιγράφει τα folder με τα αρχεία του VS
            lstMessage.Items.Add("Copying Visual Basic files to usb..")
            lstIndex += 1
            lstMessage.Refresh()

            Try
                My.Computer.FileSystem.CopyDirectory(folderVSFiles, folderUSB & "/Pharmacy", FileIO.UIOption.AllDialogs)
                lstMessage.Items(lstIndex) &= "OK"
                lstMessage.Refresh()
            Catch ex As Exception
                lstMessage.Items.Add("!!! Error !!! ")
                lstIndex += 1
                lstMessage.Refresh()
            End Try

            Dim TimeDiff As Integer = DateDiff(DateInterval.Second, InitTime, Now())

            lstMessage.Items.Add("BackUp of Visual Basic dir to USB completed in " & (TimeDiff \ 60) & ":" & ((TimeDiff / 60) - (TimeDiff \ 60)) * 60)
            lstMessage.Items.Add("-------------------------------------")
            lstIndex += 2
            lstMessage.Refresh()

        End Using


    End Sub

    Private Sub RestoreVisualBasicFiles()
        Dim sql As String = ""
        Dim folderDB As String = txtSourceDB.Text
        Dim folderVSFiles As String = txtSourceFolderVS.Text
        Dim folderUSB As String = txtDestinationDrive.Text
        Dim InitTime As DateTime = Now()

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)

            'Διαγράφει το folder Debug από το USB
            Try
                lstMessage.Items.Add("Deleting Debug dir on USB... ")
                lstIndex += 1
                lstMessage.Refresh()
                My.Computer.FileSystem.DeleteDirectory(folderUSB & "/Pharmacy/Pharmacy/bin/Debug", FileIO.DeleteDirectoryOption.DeleteAllContents)
                lstMessage.Items(lstIndex) &= "OK"
                lstMessage.Refresh()
            Catch ex As Exception
                lstMessage.Items(lstIndex) &= "Failed!"
                lstMessage.Refresh()
            End Try

            ''Σβήνει το προηγούμενο backup folder από το HD
            'Try
            '    lstMessage.Items.Add("Deleting old backup dir... ")
            '    lstIndex += 1
            '    lstMessage.Refresh()
            '    My.Computer.FileSystem.DeleteDirectory(folderVSFiles & "_OLD", FileIO.DeleteDirectoryOption.DeleteAllContents)
            '    lstMessage.Items(lstIndex) &= "OK"
            '    lstMessage.Refresh()
            'Catch ex As Exception
            '    lstMessage.Items(lstIndex) &= "Failed!"
            '    lstMessage.Refresh()
            'End Try

            '' Κρατάει τα προηγούμενα αρχεία σαν backup στο HD
            'Try
            '    lstMessage.Items.Add("Renaming folder as backup... ")
            '    lstIndex += 1
            '    lstMessage.Refresh()
            '    My.Computer.FileSystem.RenameDirectory(folderVSFiles, "Pharmacy_OLD")
            '    lstMessage.Items(lstIndex) &= "OK"
            '    lstMessage.Refresh()
            'Catch ex As Exception
            '    lstMessage.Items(lstIndex) &= "Failed!"
            '    lstMessage.Refresh()
            'End Try

            ' Αντιγράφει τα folder με τα αρχεία του VS
            lstMessage.Items.Add("Copying Visual Basic files from USB to HD..")
            lstIndex += 1
            lstMessage.Refresh()

            Try
                My.Computer.FileSystem.CopyDirectory(folderUSB & "/Pharmacy", folderVSFiles, FileIO.UIOption.AllDialogs)
                lstMessage.Items(lstIndex) &= "OK"
                lstMessage.Refresh()
            Catch ex As Exception
                lstMessage.Items.Add("!!! Error !!! ")
                lstIndex += 1
                lstMessage.Refresh()
            End Try

            Dim TimeDiff As Integer = DateDiff(DateInterval.Second, InitTime, Now())

            lstMessage.Items.Add("BackUp of Visual Basic dir to USB completed in " & (TimeDiff \ 60) & ":" & ((TimeDiff / 60) - (TimeDiff \ 60)) * 60)
            lstMessage.Items.Add("-------------------------------------")
            lstIndex += 2
            lstMessage.Refresh()

        End Using


    End Sub


    Private Function ComparePharmacy2013VsFarnet() As String
        Dim infoReaderNew1, infoReaderOld1, infoReaderNew2, infoReaderOld2 As System.IO.FileInfo
        Dim pharmacy2013Folder As String = txtSourceDB.Text
        Dim farnet2013Folder As String = txtSourceFarmnetDB.Text
        Dim result As String = ""

        Try
            infoReaderOld1 = My.Computer.FileSystem.GetFileInfo(pharmacy2013Folder & "\" & txtDB1.Text & ".mdf")
            infoReaderNew1 = My.Computer.FileSystem.GetFileInfo(farnet2013Folder & "\Farnet_2013.MDF")

            infoReaderOld2 = My.Computer.FileSystem.GetFileInfo(pharmacy2013Folder & "\" & txtDB1.Text & ".ldf")
            infoReaderNew2 = My.Computer.FileSystem.GetFileInfo(farnet2013Folder & "\Farnet_2013_log.LDF")

            If (infoReaderOld1.Length = infoReaderNew1.Length) And (infoReaderOld2.Length = infoReaderNew2.Length) Then
                result = "No"
                'lstMessage.Items(lstIndex) &= "Verified..." & infoReaderHD.Length & " bytes"
                'lstMessage.Refresh()
            Else
                result = "MDF different by " & (infoReaderNew1.Length - infoReaderOld1.Length) & ", LDF different by " & (infoReaderNew2.Length - infoReaderOld2.Length)
            End If
        Catch ex As Exception
            'lstMessage.Items(lstIndex) &= "Verification Failed !"
            'lstMessage.Refresh()
        End Try
        Return result
    End Function



    Private Sub UpdatePharmacy2013C()
        Dim sql As String = ""
        Dim pharmacy2013Folder As String = txtSourceDB.Text
        Dim farnet2013Folder As String = txtSourceFarmnetDB.Text
        Dim InitTime As DateTime = Now()
        'Dim PharmaconServer As String = "MSSQL$CSASQL"
        'Dim PharmaconServer As String = "SQLEXpress"

        'Καθαρίζει το listbox
        lstMessage.Items.Clear()

        'Σταματάει το SQLServer του Pharmacon
        'StopService(PharmaconServer)
        'lstMessage.Items.Add("-------------------------------------")
        'lstIndex += 1
        'lstMessage.Refresh()


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)

            ' Βγάζει τα database offline
            lstMessage.Items.Add("Setting " & strDB1 & " offline... ")
            lstIndex += 1
            lstMessage.Refresh()

            con.Open()
            sql = "ALTER DATABASE [" & strDB1 & "] SET Offline WITH ROLLBACK IMMEDIATE "
            Dim cmd As New SqlCommand(sql, con)
            cmd.ExecuteNonQuery()

            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()

            ' Σύγκριση μεγεθών database
            'lstMessage.Items.Add("Comparing databases... ")
            'lstIndex += 1
            'lstMessage.Refresh()
            'Try
            '    My.Computer.FileSystem.CopyFile(farnet2013Folder & "\Farnet_2013.MDF", pharmacy2013Folder & "\" & txtDB1.Text & ".mdf", FileIO.UIOption.AllDialogs)
            '    My.Computer.FileSystem.CopyFile(farnet2013Folder & "\Farnet_2013_log.LDF", pharmacy2013Folder & "\" & txtDB1.Text & ".ldf", FileIO.UIOption.AllDialogs)
            'Catch ex As Exception
            '    lstMessage.Items.Add("!!! Error !!!")
            '    lstIndex += 1
            '    lstMessage.Refresh()
            'End Try
            'lstMessage.Items(lstIndex) &= "OK"
            'lstMessage.Refresh()


            ' Αντιγράφει τα database
            lstMessage.Items.Add("Updating " & strDB1 & "... ")
            lstIndex += 1
            lstMessage.Refresh()

            Try
                'My.Computer.FileSystem.CopyFile(farnet2013Folder & "\Farnet_2013.MDF", pharmacy2013Folder & "\" & txtDB1.Text & ".mdf", FileIO.UIOption.AllDialogs)
                'My.Computer.FileSystem.CopyFile(farnet2013Folder & "\Farnet_2013_log.LDF", pharmacy2013Folder & "\" & txtDB1.Text & ".ldf", FileIO.UIOption.AllDialogs)
                My.Computer.FileSystem.CopyFile(farnet2013Folder & "\" & txtSourceFarmnet_mdf.Text & ".MDF", pharmacy2013Folder & "\" & txtDB1.Text & ".mdf", FileIO.UIOption.AllDialogs)
                My.Computer.FileSystem.CopyFile(farnet2013Folder & "\" & txtSourceFarmnet_mdf.Text & "_log.LDF", pharmacy2013Folder & "\" & txtDB1.Text & ".ldf", FileIO.UIOption.AllDialogs)
            Catch ex As Exception
                lstMessage.Items.Add("!!! Error !!!")
                lstIndex += 1
                lstMessage.Refresh()
            End Try
            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()

            ' Βγάζει τα database online
            lstMessage.Items.Add("Setting " & strDB1 & " online... ")
            lstIndex += 1
            lstMessage.Refresh()

            sql = "ALTER DATABASE [" & strDB1 & "] SET Online "
            cmd = New SqlCommand(sql, con)
            cmd.ExecuteNonQuery()

            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()

            Dim TimeDiff As Integer = DateDiff(DateInterval.Second, InitTime, Now())

            lstMessage.Items.Add("Update of " & strDB1 & " completed in " & (TimeDiff \ 60) & ":" & ((TimeDiff / 60) - (TimeDiff \ 60)) * 60)
            lstMessage.Items.Add("-------------------------------------")
            lstIndex += 2
            lstMessage.Refresh()

        End Using

        'Ξαναρχίζει το SQLServer του Pharmacon
        'StartService(PharmaconServer)

    End Sub


    Private Sub UpdatePharmacy2025()

        Dim InitTime As DateTime = Now()
        'Dim PharmaconServer As String = "MSSQL$CSASQL"
        'Dim PharmaconServer As String = "SQLEXpress"

        'Καθαρίζει το listbox
        lstMessage.Items.Clear()

        'Σταματάει το CASSQLServer του Pharmacon
        lstMessage.Items.Add("-------------------------------------")
        lstIndex += 1
        lstMessage.Refresh()
        StopService(PharmakonServer)

        'Σταματάει το SQLServer του προγράμματος μου
        StopService(Pharmacy2013Server)

        ' Αντιγράφει τα database
        lstMessage.Items.Add("Updating " & strDB1 & "... ")
        lstIndex += 1
        lstMessage.Refresh()

        ' Αντιγράφει το νεό DB του Pharmakon
        Try
            My.Computer.FileSystem.CopyFile(FarNetFolder & txtSourceFarmnet_mdf.Text & ".MDF", Pharmacy2013Folder & txtDB1.Text & ".mdf", FileIO.UIOption.OnlyErrorDialogs)
            My.Computer.FileSystem.CopyFile(FarNetFolder & txtSourceFarmnet_mdf.Text & "_log.LDF", Pharmacy2013Folder & txtDB1.Text & ".ldf", FileIO.UIOption.OnlyErrorDialogs)
            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()
        Catch ex As Exception
            lstMessage.Items.Add("!!! Error !!!")
            MsgBox(ex.Message)
            lstIndex += 1
            lstMessage.Refresh()
        End Try

        'Ξεκινά το CASSQLServer του Pharmacon
        StartService(PharmakonServer)

        'Ξεκινά το SQLServer του προγράμματος μου
        StartService(Pharmacy2013Server)

        Dim TimeDiff As Integer = DateDiff(DateInterval.Second, InitTime, Now())

        lstMessage.Items.Add("Update of " & strDB1 & " completed in " & ((TimeDiff / 60) - (TimeDiff \ 60)) * 60 & " seconds.")
        lstMessage.Items.Add("-------------------------------------")
        lstIndex += 2
        lstMessage.Refresh()


    End Sub

    Private Sub UpdatePharmacy2025_TRYOUT()

        Dim InitTime As DateTime = Now()
        'Dim PharmaconServer As String = "MSSQL$CSASQL"
        'Dim PharmaconServer As String = "SQLEXpress"
        Dim folderPath As String

        If My.Computer.Name = "CRAZYDR" Then
            folderPath = "D:\Pharmacy Update\"
        ElseIf My.Computer.Name = "DESKTOP-T7HMABG" Then
            folderPath = "G:\Pharmacy Update\"
        End If



        'Καθαρίζει το listbox
        lstMessage.Items.Clear()
        lstIndex = -1

        'Σταματάει το CASSQLServer του Pharmacon
        lstMessage.Items.Add("-------------------------------------")
        lstIndex += 1
        lstMessage.Refresh()
        StopService(PharmakonServer)

        'Σταματάει το SQLServer του προγράμματος μου
        StopService(Pharmacy2013Server)

        'Αν δεν υπάρχει το δοκιμαστικό folder το δημιουργεί
        If Not System.IO.Directory.Exists(folderPath) Then
            System.IO.Directory.CreateDirectory(folderPath)
            lstMessage.Items.Add("Δημιουργία του folder " & folderPath & ". ")
        Else
            lstMessage.Items.Add("To folder " & folderPath & " υπάρχει ήδη. ")
        End If
        lstIndex += 1
        lstMessage.Refresh()

        ' Σβήνει τα περιεχόμενα του δοκιμαστικού folder
        lstMessage.Items.Add("Εκκαθάριση των αρχείων του folder " & folderPath & ". ")
        For Each deleteFile In Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
            File.Delete(deleteFile)
        Next
        lstIndex += 1
        lstMessage.Refresh()

        ' Αντιγράφει τα database
        lstMessage.Items.Add("Updating... ")
        lstIndex += 1
        lstMessage.Refresh()

        ' Αντιγράφει το νεό DB του Pharmakon
        Try
            My.Computer.FileSystem.CopyFile(FarNetFolder & txtSourceFarmnet_mdf.Text & ".MDF", folderPath & txtSourceFarmnet_mdf.Text & ".mdf", FileIO.UIOption.AllDialogs)
            My.Computer.FileSystem.CopyFile(FarNetFolder & txtSourceFarmnet_mdf.Text & "_log.LDF", folderPath & txtSourceFarmnet_mdf.Text & ".ldf", FileIO.UIOption.AllDialogs)
            lstMessage.Items(lstIndex) &= ".. Pharmakon OK.."
            lstMessage.Refresh()
        Catch ex As Exception
            lstMessage.Items.Add("!!! Error !!!")
            MsgBox(ex.Message)
            lstIndex += 1
            lstMessage.Refresh()
        End Try

        ' Αντιγράφει το νεό DB του Pharmacy2013
        Try
            My.Computer.FileSystem.CopyFile(Pharmacy2013Folder & txtDB1.Text & ".MDF", folderPath & txtDB1.Text & ".mdf", FileIO.UIOption.AllDialogs)
            My.Computer.FileSystem.CopyFile(Pharmacy2013Folder & txtDB1.Text & ".LDF", folderPath & txtDB1.Text & ".ldf", FileIO.UIOption.AllDialogs)
            lstMessage.Items(lstIndex) &= "..Pharmacy2013 OK.."
            lstMessage.Refresh()
        Catch ex As Exception
            lstMessage.Items.Add("!!! Error !!!")
            MsgBox(ex.Message)
            lstIndex += 1
            lstMessage.Refresh()
        End Try

        'Ξεκινά το CASSQLServer του Pharmacon
        StartService(PharmakonServer)

        'Ξεκινά το SQLServer του προγράμματος μου
        StartService(Pharmacy2013Server)

        Dim TimeDiff As Integer = DateDiff(DateInterval.Second, InitTime, Now())

        lstMessage.Items.Add("Update of " & strDB1 & " completed in " & ((TimeDiff / 60) - (TimeDiff \ 60)) * 60 & " seconds.")
        lstMessage.Items.Add("-------------------------------------")
        lstIndex += 2
        lstMessage.Refresh()


    End Sub

    Private Sub UpdatePharmacy2013C_old()
        Dim sql As String = ""
        Dim pharmacy2013Folder As String = txtSourceDB.Text
        Dim farnet2013Folder As String = txtSourceFarmnetDB.Text
        Dim InitTime As DateTime = Now()
        'Dim PharmaconServer As String = "MSSQL$CSASQL"
        Dim PharmaconServer As String = "SQLEXpress"

        'Καθαρίζει το listbox
        lstMessage.Items.Clear()

        'Σταματάει το SQLServer του Pharmacon
        StopService(PharmaconServer)
        lstMessage.Items.Add("-------------------------------------")
        lstIndex += 1
        lstMessage.Refresh()


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)

            ' Βγάζει τα database offline
            lstMessage.Items.Add("Setting " & strDB1 & " offline... ")
            lstIndex += 1
            lstMessage.Refresh()

            con.Open()
            sql = "ALTER DATABASE [" & strDB1 & "] Set Offline With ROLLBACK IMMEDIATE "
            Dim cmd As New SqlCommand(sql, con)
            cmd.ExecuteNonQuery()

            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()

            ' Σύγκριση μεγεθών database
            lstMessage.Items.Add("Comparing databases... ")
            lstIndex += 1
            lstMessage.Refresh()
            'Try
            '    My.Computer.FileSystem.CopyFile(farnet2013Folder & "\Farnet_2013.MDF", pharmacy2013Folder & "\" & txtDB1.Text & ".mdf", FileIO.UIOption.AllDialogs)
            '    My.Computer.FileSystem.CopyFile(farnet2013Folder & "\Farnet_2013_log.LDF", pharmacy2013Folder & "\" & txtDB1.Text & ".ldf", FileIO.UIOption.AllDialogs)
            'Catch ex As Exception
            '    lstMessage.Items.Add("!!! Error !!!")
            '    lstIndex += 1
            '    lstMessage.Refresh()
            'End Try
            'lstMessage.Items(lstIndex) &= "OK"
            'lstMessage.Refresh()


            ' Αντιγράφει τα database
            lstMessage.Items.Add("Updating " & strDB1 & "... ")
            lstIndex += 1
            lstMessage.Refresh()

            Try
                'My.Computer.FileSystem.CopyFile(farnet2013Folder & "\Farnet_2013.MDF", pharmacy2013Folder & "\" & txtDB1.Text & ".mdf", FileIO.UIOption.AllDialogs)
                'My.Computer.FileSystem.CopyFile(farnet2013Folder & "\Farnet_2013_log.LDF", pharmacy2013Folder & "\" & txtDB1.Text & ".ldf", FileIO.UIOption.AllDialogs)
                My.Computer.FileSystem.CopyFile(farnet2013Folder & "\" & strDB1_Source & ".MDF", pharmacy2013Folder & "\" & txtDB1.Text & ".mdf", FileIO.UIOption.AllDialogs)
                My.Computer.FileSystem.CopyFile(farnet2013Folder & "\" & strDB1_Source & "_log.LDF", pharmacy2013Folder & "\" & txtDB1.Text & ".ldf", FileIO.UIOption.AllDialogs)
            Catch ex As Exception
                lstMessage.Items.Add("!!! Error !!!")
                lstIndex += 1
                lstMessage.Refresh()
            End Try
            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()

            ' Βγάζει τα database online
            lstMessage.Items.Add("Setting " & strDB1 & " online... ")
            lstIndex += 1
            lstMessage.Refresh()

            sql = "ALTER DATABASE [" & strDB1 & "] Set Online "
            cmd = New SqlCommand(sql, con)
            cmd.ExecuteNonQuery()

            lstMessage.Items(lstIndex) &= "OK"
            lstMessage.Refresh()

            Dim TimeDiff As Integer = DateDiff(DateInterval.Second, InitTime, Now())

            lstMessage.Items.Add("Update Of " & strDB1 & " completed In " & (TimeDiff \ 60) & ":" & ((TimeDiff / 60) - (TimeDiff \ 60)) * 60)
            lstMessage.Items.Add("-------------------------------------")
            lstIndex += 2
            lstMessage.Refresh()

        End Using

        'Ξαναρχίζει το SQLServer του Pharmacon
        StartService(PharmaconServer)

    End Sub

    'Private Sub RestoreDatabaseFiles()
    '    Dim sql As String = ""
    '    Dim folderDB As String = txtSourceDB.Text
    '    Dim folderVSFiles As String = txtSourceFolderVS.Text
    '    Dim folderUSB As String = txtDestinationDrive.Text

    '    Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)

    '        ' Βγάζει τα database offline
    '        lstMessage.Items.Clear()
    '        lstMessage.Items.Add("Setting databases offline..")
    '        lstMessage.Refresh()

    '        con.Open()
    '        sql = "ALTER DATABASE [" & strDB1 & "] SET Offline WITH ROLLBACK IMMEDIATE " & _
    '            "ALTER DATABASE [" & strDB2 & "] SET Offline WITH ROLLBACK IMMEDIATE "
    '        Dim cmd As New SqlCommand(sql, con)
    '        cmd.ExecuteNonQuery()
    '        'con.Close()

    '        ' Αντιγράφει τα database
    '        lstMessage.Items.Add("Copying databases to server..")
    '        lstMessage.Refresh()
    '        Try
    '            System.IO.File.Copy(folderUSB & "\" & txtDB1.Text & ".mdf", folderDB & "\" & txtDB1.Text & ".mdf", True)
    '            System.IO.File.Copy(folderUSB & "\" & txtDB1.Text & ".ldf", folderDB & "\" & txtDB1.Text & ".ldf", True)
    '            System.IO.File.Copy(folderUSB & "\" & txtDB2.Text & ".mdf", folderDB & "\" & txtDB2.Text & ".mdf", True)
    '            System.IO.File.Copy(folderUSB & "\" & txtDB2.Text & "_log.ldf", folderDB & "\" & txtDB2.Text & "_log.ldf", True)
    '        Catch ex As Exception
    '            lstMessage.Items.Add("!!! Error !!! ")
    '            lstMessage.Refresh()
    '        End Try

    '        ' Διαγράφει το folder debug
    '        lstMessage.Items.Add("Deleting Debug folder from usb..")
    '        lstMessage.Refresh()
    '        Try
    '            System.IO.Directory.Delete(txtDestinationDrive.Text & "\Pharmacy\Pharmacy\bin\Debug", True)
    '        Catch ex As Exception
    '            lstMessage.Items.Add("No directory Debug to erase!")
    '            lstMessage.Refresh()
    '        End Try

    '        ' Αντιγράφει τα folder με τα αρχεία του VS
    '        lstMessage.Items.Add("Copying VS files to HD..")
    '        lstMessage.Refresh()
    '        Try
    '            My.Computer.FileSystem.CopyDirectory(folderUSB & "\Pharmacy", folderVSFiles, True)
    '        Catch ex As Exception
    '            lstMessage.Items.Add("!!! Error !!! ")
    '            lstMessage.Refresh()
    '        End Try

    '        ' Βγάζει τα database online
    '        lstMessage.Items.Add("Setting databases online..")
    '        lstMessage.Refresh()
    '        'con.Open()
    '        sql = "ALTER DATABASE [" & strDB1 & "] SET Online " & _
    '            "ALTER DATABASE [" & strDB2 & "] SET Online"
    '        cmd = New SqlCommand(sql, con)
    '        cmd.ExecuteNonQuery()
    '        'con.Close()

    '        lstMessage.Items.Add("OK")
    '        lstMessage.Items.Add("-------------")
    '        lstMessage.Refresh()

    '    End Using

    'End Sub


    'Private Sub RestoreDatabaseFiles()
    '    Dim sql As String = ""
    '    Dim folderDB As String = txtSourceDB.Text
    '    Dim folderVSFiles As String = txtSourceFolderVS.Text
    '    Dim folderUSB As String = txtDestinationDrive.Text
    '    Dim infoReaderUSB, infoReaderSource As System.IO.FileInfo

    '    Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)

    '        ' Βγάζει τα database offline
    '        lstMessage.Items.Clear()
    '        lstMessage.Items.Add("Setting databases offline..")
    '        lstMessage.Refresh()

    '        con.Open()
    '        sql = "ALTER DATABASE [" & strDB1 & "] SET Offline WITH ROLLBACK IMMEDIATE " & _
    '            "ALTER DATABASE [" & strDB2 & "] SET Offline WITH ROLLBACK IMMEDIATE "
    '        Dim cmd As New SqlCommand(sql, con)
    '        cmd.ExecuteNonQuery()
    '        'con.Close()

    '        ' Αντιγράφει τα database
    '        lstMessage.Items.Add("Copying databases to server..")
    '        lstMessage.Refresh()
    '        Try
    '            System.IO.File.Copy(folderUSB & "\" & txtDB1.Text & ".mdf", folderDB & "\" & txtDB1.Text & ".mdf", True)
    '            System.IO.File.Copy(folderUSB & "\" & txtDB1.Text & ".ldf", folderDB & "\" & txtDB1.Text & ".ldf", True)
    '            System.IO.File.Copy(folderUSB & "\" & txtDB2.Text & ".mdf", folderDB & "\" & txtDB2.Text & ".mdf", True)
    '            System.IO.File.Copy(folderUSB & "\" & txtDB2.Text & "_log.ldf", folderDB & "\" & txtDB2.Text & "_log.ldf", True)
    '        Catch ex As Exception
    '            lstMessage.Items.Add("!!! Error !!! ")
    '            lstMessage.Refresh()
    '        End Try

    '        ' Verification !!
    '        Try
    '            infoReaderSource = My.Computer.FileSystem.GetFileInfo(folderDB & "\" & txtDB1.Text & ".mdf")
    '            infoReaderUSB = My.Computer.FileSystem.GetFileInfo(folderUSB & "\" & txtDB1.Text & ".mdf")
    '            If infoReaderSource.Length = infoReaderUSB.Length Then
    '                lstMessage.Items.Add("Verified.." & txtDB1.Text & ".mdf  (" & infoReaderSource.Length & " bytes)")
    '                lstMessage.Refresh()
    '            End If
    '        Catch ex As Exception
    '            lstMessage.Items.Add("Verified Failed !" & txtDB1.Text & ".mdf")
    '            lstMessage.Refresh()
    '        End Try

    '        Try
    '            infoReaderSource = My.Computer.FileSystem.GetFileInfo(folderDB & "\" & txtDB1.Text & ".ldf")
    '            infoReaderUSB = My.Computer.FileSystem.GetFileInfo(folderUSB & "\" & txtDB1.Text & ".ldf")
    '            If infoReaderSource.Length = infoReaderUSB.Length Then
    '                lstMessage.Items.Add("Verified.." & txtDB1.Text & ".ldf  (" & infoReaderSource.Length & " bytes)")
    '                lstMessage.Refresh()
    '            End If
    '        Catch ex As Exception
    '            lstMessage.Items.Add("Verified Failed !" & txtDB1.Text & ".ldf")
    '            lstMessage.Refresh()
    '        End Try

    '        Try
    '            infoReaderSource = My.Computer.FileSystem.GetFileInfo(folderDB & "\" & txtDB2.Text & ".mdf")
    '            infoReaderUSB = My.Computer.FileSystem.GetFileInfo(folderUSB & "\" & txtDB2.Text & ".mdf")
    '            If infoReaderSource.Length = infoReaderUSB.Length Then
    '                lstMessage.Items.Add("Verified.." & txtDB2.Text & ".mdf  (" & infoReaderSource.Length & " bytes)")
    '                lstMessage.Refresh()
    '            End If
    '        Catch ex As Exception
    '            lstMessage.Items.Add("Verified Failed !" & txtDB2.Text & ".mdf")
    '            lstMessage.Refresh()
    '        End Try

    '        Try
    '            infoReaderSource = My.Computer.FileSystem.GetFileInfo(folderDB & "\" & txtDB2.Text & "_log.ldf")
    '            infoReaderUSB = My.Computer.FileSystem.GetFileInfo(folderUSB & "\" & txtDB2.Text & "_log.ldf")
    '            If infoReaderSource.Length = infoReaderUSB.Length Then
    '                lstMessage.Items.Add("Verified.." & txtDB2.Text & "_log.ldf  (" & infoReaderSource.Length & " bytes)")
    '                lstMessage.Refresh()
    '            End If
    '        Catch ex As Exception
    '            lstMessage.Items.Add("Verified Failed !" & txtDB2.Text & "_log.ldf")
    '            lstMessage.Refresh()
    '        End Try


    '        ' Βγάζει τα database online
    '        lstMessage.Items.Add("Setting databases online..")
    '        lstMessage.Refresh()
    '        'con.Open()
    '        sql = "ALTER DATABASE [" & strDB1 & "] SET Online " & _
    '            "ALTER DATABASE [" & strDB2 & "] SET Online"
    '        cmd = New SqlCommand(sql, con)
    '        cmd.ExecuteNonQuery()
    '        'con.Close()

    '        lstMessage.Items.Add("OK")
    '        lstMessage.Items.Add("-------------")
    '        lstMessage.Refresh()

    '    End Using

    'End Sub





    Private Function CheckIfRecordChanged_ExchangesFrom(ByVal index As Integer) As Boolean
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT * FROM  PharmacyCustomFiles.dbo.ExchangesMaster WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvExchangeFrom2.Rows(index).Cells(9).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    ' Aν έχει αλλάξει DrugId, Quantity ή Total
                    If myReader("DrugId") <> dgvExchangeFrom2.Rows(index).Cells(10).Value Or
                        CType(myReader("Quantity"), Decimal) <> dgvExchangeFrom2.Rows(index).Cells(3).Value Or
                        myReader("Total") <> dgvExchangeFrom2.Rows(index).Cells(5).Value Then

                        Return True
                    End If
                Loop

                Return True
            End If

            Return False

        End Using

    End Function



    Private Function CheckIfRecordChanged_ExchangesTo(ByVal index As Integer) As Boolean
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT * FROM  PharmacyCustomFiles.dbo.ExchangesMaster WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvExchangeTo2.Rows(index).Cells(9).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    ' Aν έχει αλλάξει DrugId, Quantity ή Total
                    If myReader("DrugId") <> dgvExchangeTo2.Rows(index).Cells(10).Value Or
                        CType(myReader("Quantity"), Decimal) <> dgvExchangeTo2.Rows(index).Cells(3).Value Or
                        myReader("Total") <> dgvExchangeTo2.Rows(index).Cells(5).Value Then

                        Return True
                    End If
                Loop

                Return True
            End If

            Return False

        End Using

    End Function



    Private Function CheckIfRecordChangedOrExists_PricesParadrugs(ByVal index As Integer) As String
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT Id, isnull(Name,''), isnull(Xondr,'0'), isnull(Lian,'0'), isnull(Notes,''), isnull(AP_Code,'0') FROM  PharmacyCustomFiles.dbo.PricesParadrugs WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            Dim myId As Integer = dgvPricesParadrugs.Rows(0).Cells(5).Value

            'MsgBox(index & " -> " & dgvPricesParadrugs.Rows(index).Cells(1).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(2).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(3).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(4).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(5).Value & "-")
            cmd.Parameters.AddWithValue("@Id", myId)

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    ' Aν έχει αλλάξει κάτι
                    If myReader(1) <> dgvPricesParadrugs.Rows(index).Cells(0).EditedFormattedValue Or
                        CType(myReader(2), Decimal) <> CType(dgvPricesParadrugs.Rows(index).Cells(1).EditedFormattedValue, Decimal) Or
                        CType(myReader(3), Decimal) <> CType(dgvPricesParadrugs.Rows(index).Cells(2).EditedFormattedValue, Decimal) Or
                       myReader(4) <> dgvPricesParadrugs.Rows(index).Cells(3).EditedFormattedValue Or
                       CType(myReader(5), Integer) <> dgvPricesParadrugs.Rows(index).Cells(4).EditedFormattedValue Then

                        Return "Changed"
                    End If
                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function


    Private Function CheckIfRecordChangedOrExists_Customers(ByVal index As Integer) As String
        Dim insertData As String = ""


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT isnull(Name,''), isnull(Id,0) FROM  PharmacyCustomFiles.dbo.Customers WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvCustomers.Rows(index).Cells(1).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    ' Aν έχει αλλάξει κάτι
                    'MsgBox(myReader(0).ToString & " - " & dgvCustomers.Rows(index).Cells(0).EditedFormattedValue)
                    If myReader(0) <> dgvCustomers.Rows(index).Cells(0).EditedFormattedValue Then

                        Return "Changed"
                    End If
                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function



    Private Function CheckIfRecordChangedOrExists_Debts(ByVal index As Integer) As String
        Dim insertData As String = ""

        'MsgBox(GetLastDebtRecord())


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT isnull(Date,''), isnull([Ammount],0), isnull(DebtDescription,'') FROM  PharmacyCustomFiles.dbo.Debts WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvDebtsList.Rows(index).Cells(3).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    ' Aν έχει αλλάξει κάτι
                    If dgvDebtsList.Rows(index).Cells(0).EditedFormattedValue <> "" AndAlso CType(dgvDebtsList.Rows(index).Cells(0).EditedFormattedValue, Date) <> myReader(0) Then
                        Return "Changed"
                    ElseIf dgvDebtsList.Rows(index).Cells(1).EditedFormattedValue <> "" AndAlso CType(dgvDebtsList.Rows(index).Cells(1).EditedFormattedValue, Decimal) <> myReader(1) Then
                        Return "Changed"
                    ElseIf dgvDebtsList.Rows(index).Cells(2).EditedFormattedValue <> "" AndAlso dgvDebtsList.Rows(index).Cells(2).EditedFormattedValue <> myReader(2) Then
                        Return "Changed"
                    End If

                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function



    Private Function CheckIfRecordChangedOrExists_DrugsOnLoan(ByVal index As Integer) As String
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "Select isnull(Name,''), isnull(Price,0), isnull(DateIn,''), isnull(Barcode1,''), isnull(Barcode2,''), isnull(DateOut,''), isnull(CustomerId,0) " &
                        "FROM PharmacyCustomFiles.dbo.DrugsOnLoan " &
                        "WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvDrugsOnLoan.Rows(index).Cells(6).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    ' Aν έχει αλλάξει κάτι
                    If dgvDrugsOnLoan.Rows(index).Cells(0).EditedFormattedValue <> "" AndAlso CType(dgvDrugsOnLoan.Rows(index).Cells(0).EditedFormattedValue, Date) <> myReader(2) Then
                        Return "Changed"
                    ElseIf dgvDrugsOnLoan.Rows(index).Cells(1).EditedFormattedValue <> "" AndAlso dgvDrugsOnLoan.Rows(index).Cells(1).EditedFormattedValue <> myReader(0) Then
                        Return "Changed"
                    ElseIf dgvDrugsOnLoan.Rows(index).Cells(3).EditedFormattedValue <> "" AndAlso dgvDrugsOnLoan.Rows(index).Cells(3).EditedFormattedValue <> myReader(3) Then
                        Return "Changed"
                    ElseIf dgvDrugsOnLoan.Rows(index).Cells(4).EditedFormattedValue <> "" AndAlso dgvDrugsOnLoan.Rows(index).Cells(4).EditedFormattedValue <> myReader(4) Then
                        Return "Changed"
                    ElseIf dgvDrugsOnLoan.Rows(index).Cells(2).EditedFormattedValue <> "" AndAlso CType(dgvDrugsOnLoan.Rows(index).Cells(2).EditedFormattedValue, Decimal) <> myReader(1) Then
                        Return "Changed"
                    ElseIf dgvDrugsOnLoan.Rows(index).Cells(5).EditedFormattedValue <> "" AndAlso CType(dgvDrugsOnLoan.Rows(index).Cells(5).EditedFormattedValue, Date) <> myReader(5) Then
                        Return "Changed"
                    End If

                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function


    Private Function CheckIfRecordChangedOrExists_Prescriptions(ByVal index As Integer) As String
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            'insertData = "Select isnull(Ektelesis,''), isnull(InitDate,''), isnull(EndDate,''), isnull(Barcode,''), isnull(ProcessedDate,''), isnull(CustomerId,0), isnull(Drug1,''), isnull(Drug2,''), isnull(Drug3,'') " & _
            '            "FROM PharmacyCustomFiles.dbo.Prescriptions " & _
            '            "WHERE Id = @Id "
            insertData = "Select Ektelesis, InitDate, EndDate, Barcode, ISNULL(ProcessedDate,''), ISNULL(CustomerId,-1), Drug1, Drug2, Drug3 " &
                        "FROM PharmacyCustomFiles.dbo.Prescriptions " &
                        "WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvPrescriptions.Rows(index).Cells(6).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()
                    'MsgBox(myReader(1) & " " & dgvPrescriptions.Rows(index).Cells(1).Value)
                    ' Aν έχει αλλάξει κάτι
                    If dgvPrescriptions.Rows(index).Cells(0).Value <> "" AndAlso dgvPrescriptions.Rows(index).Cells(0).Value <> myReader(0) Then  ' Ektelesis
                        Return "Changed"

                    ElseIf IsDBNull(dgvPrescriptions.Rows(index).Cells(1).Value) = False AndAlso dgvPrescriptions.Rows(index).Cells(1).Value <> myReader(1) Then 'InitDate
                        Return "Changed"
                    ElseIf IsDBNull(dgvPrescriptions.Rows(index).Cells(2).Value) = False AndAlso dgvPrescriptions.Rows(index).Cells(2).Value <> myReader(2) Then 'EndDate
                        Return "Changed"
                    ElseIf dgvPrescriptions.Rows(index).Cells(3).Value <> "" AndAlso dgvPrescriptions.Rows(index).Cells(3).Value <> myReader(3) Then  'Barcode
                        Return "Changed"
                    ElseIf IsDBNull(dgvPrescriptions.Rows(index).Cells(4).Value) = False AndAlso dgvPrescriptions.Rows(index).Cells(4).Value <> myReader(4) Then 'ProcessedDate
                        Return "Changed"
                    ElseIf IsDBNull(dgvPrescriptions.Rows(index).Cells(5).Value) AndAlso CType(dgvPrescriptions.Rows(index).Cells(5).Value, Decimal) <> myReader(5) Then  'CustomerId
                        Return "Changed"
                    ElseIf dgvPrescriptions.Rows(index).Cells(7).Value <> "" AndAlso dgvPrescriptions.Rows(index).Cells(7).Value <> myReader(7) Then  'Drug1
                        Return "Changed"
                    ElseIf dgvPrescriptions.Rows(index).Cells(8).Value <> "" AndAlso dgvPrescriptions.Rows(index).Cells(8).Value <> myReader(8) Then  'Drug2
                        Return "Changed"
                    ElseIf dgvPrescriptions.Rows(index).Cells(9).Value <> "" AndAlso dgvPrescriptions.Rows(index).Cells(9).Value <> myReader(9) Then  'Drug3
                        Return "Changed"
                    End If

                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function



    Private Function GetLastRecordId(ByVal mode As String) As String
        Dim insertData As String = ""


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            If mode = "debts" Then
                insertData = "SELECT * FROM PharmacyCustomFiles.dbo.Debts " &
                                      "WHERE  Id = IDENT_CURRENT('PharmacyCustomFiles.dbo.Debts')"
            ElseIf mode = "hairdies" Then
                insertData = "SELECT * FROM PharmacyCustomFiles.dbo.Hairdies " &
                                   "WHERE  Id = IDENT_CURRENT('PharmacyCustomFiles.dbo.Hairdies')"
            End If

            Dim cmd As New SqlCommand(insertData, con)

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            Do While myReader.Read()

                Return myReader(0)
            Loop
        End Using

        Return ""

    End Function

    Private Function CheckIfRecordChangedOrExists_Hairdies(ByVal index As Integer) As String
        Dim insertData As String = ""


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT isnull(Date,''), isnull(HairDieDescription,'') FROM  PharmacyCustomFiles.dbo.Hairdies WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvHairdiesList.Rows(index).Cells(2).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    ' Aν έχει αλλάξει κάτι
                    If dgvHairdiesList.Rows(index).Cells(0).EditedFormattedValue <> "" AndAlso CType(dgvHairdiesList.Rows(index).Cells(0).EditedFormattedValue, Date) <> myReader(0) Then
                        Return "Changed"
                    ElseIf dgvHairdiesList.Rows(index).Cells(1).EditedFormattedValue <> "" AndAlso dgvHairdiesList.Rows(index).Cells(1).EditedFormattedValue <> myReader(1) Then
                        Return "Changed"
                    End If

                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function


    Private Function CheckIfRecordChangedOrExists_FarmSold(ByVal index As Integer) As String
        Dim insertData As String = ""
        Dim NumericFields(10) As Decimal



        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT isnull(Id,0), isnull(MyDate,''), isnull(CodeReceipt,0), isnull(Description,''), " &
                              "isnull(SoldXondr130,0), isnull(SoldXondr230,0), isnull(SoldXondr650, 0), " &
                              "isnull(SoldLian130, 0), isnull(SoldLian230, 0), isnull(SoldLian650, 0), isnull(inFPA, 0) " &
                         "FROM PharmacyCustomFiles.dbo.FarmSold " &
                         "WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvAgoresSold.Rows(index).Cells(0).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()


                    For t = 4 To 10
                        If dgvAgoresSold.Rows(index).Cells(t).EditedFormattedValue = "" Then
                            NumericFields(t) = 0
                        Else
                            NumericFields(t) = CType(dgvAgoresSold.Rows(index).Cells(t).EditedFormattedValue, Decimal)
                        End If
                    Next

                    If myReader(1) <> CType(dgvAgoresSold.Rows(index).Cells(1).EditedFormattedValue, Date) Or
                       myReader(2) <> dgvAgoresSold.Rows(index).Cells(2).EditedFormattedValue Or
                       myReader(3) <> dgvAgoresSold.Rows(index).Cells(3).EditedFormattedValue Or
                      myReader(4) <> NumericFields(4) Or
                       myReader(5) <> NumericFields(5) Or
                      myReader(6) <> NumericFields(6) Or
                       myReader(7) <> NumericFields(7) Or
                       myReader(8) <> NumericFields(8) Or
                       myReader(9) <> NumericFields(9) Or
                      myReader(10) <> NumericFields(10) Then

                        Return "Changed"

                    End If
                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function



    Private Function CheckIfRecordChangedOrExists_TameiaAsked(ByVal index As Integer) As String
        Dim insertData As String = ""
        Dim NumericFields(10) As Decimal
        Dim str1 As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()
            insertData = "SELECT isnull(MyDate,''), isnull(Description,''), " &
                              "isnull(AmountAsked,0), isnull(AmountGiven,0), isnull(Difference, 0), " &
                              "isnull(DifferPercent, 0), isnull(PercentagePaid, 0), isnull(IRS, 0), isnull(Id,0) " &
                         "FROM PharmacyCustomFiles.dbo.TameiaAsked " &
                         "WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvTameiaAsked.Rows(index).Cells(8).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    For t = 2 To 6

                        str1 = dgvTameiaAsked.Rows(index).Cells(t).EditedFormattedValue

                        If str1 = "" Then
                            NumericFields(t) = 0
                        Else
                            If t = 5 Or t = 6 Then
                                NumericFields(t) = DePercent(str1)
                            Else
                                NumericFields(t) = CType(str1, Decimal)
                            End If
                        End If

                    Next

                    If myReader(0) <> CType(dgvTameiaAsked.Rows(index).Cells(0).EditedFormattedValue, Date) Or
                       myReader(1) <> dgvTameiaAsked.Rows(index).Cells(1).EditedFormattedValue Or
                      myReader(2) <> NumericFields(2) Or
                       myReader(3) <> NumericFields(3) Or
                      myReader(4) <> NumericFields(4) Or
                       myReader(5) <> NumericFields(5) Or
                        myReader(6) <> NumericFields(6) Or
                       myReader(7) <> dgvTameiaAsked.Rows(index).Cells(7).EditedFormattedValue Then

                        Return "Changed"

                    End If
                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function


    Private Function CheckIfRecordChangedOrExists_Phones(ByVal index As Integer) As String
        Dim insertData As String = ""
        Dim NumericFields(10) As Decimal
        Dim str1 As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()
            insertData = "SELECT isnull(Fullname,''), isnull(PhoneNumber1,''), " &
                              "isnull(PhoneNumber2,''), isnull(PhoneNumber3,''), isnull(PhoneNumber4, '') " &
                         "FROM PharmacyCustomFiles.dbo.Phonebook " &
                         "WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvPhones.Rows(index).Cells(5).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    If myReader(0) <> dgvPhones.Rows(index).Cells(0).EditedFormattedValue Or
                        myReader(1) <> dgvPhones.Rows(index).Cells(1).EditedFormattedValue Or
                        myReader(2) <> dgvPhones.Rows(index).Cells(2).EditedFormattedValue Or
                        myReader(3) <> dgvPhones.Rows(index).Cells(3).EditedFormattedValue Or
                        myReader(4) <> dgvPhones.Rows(index).Cells(4).EditedFormattedValue Then

                        Return "Changed"

                    End If
                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function


    Private Function CheckIfRecordChangedOrExists_Paradrugs(ByVal index As Integer) As String
        Dim insertData As String = ""
        Dim NumericFields(10) As Decimal
        Dim str1 As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT isnull(Name,''), isnull(Xondr,'0'), isnull(Lian,'0'), isnull(Notes,''), " &
                            "isnull(AP_Code,'0'), isnull(AP_ID,'0'), isnull(Barcode,'') " &
                        "FROM  PharmacyCustomFiles.dbo.PricesParadrugs " &
                        "WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvPricesParadrugs.Rows(index).Cells(5).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    If myReader(0) <> dgvPricesParadrugs.Rows(index).Cells(0).EditedFormattedValue Or
                        myReader(1) <> CType(dgvPricesParadrugs.Rows(index).Cells(1).EditedFormattedValue, Decimal) Or
                        myReader(2) <> CType(dgvPricesParadrugs.Rows(index).Cells(2).EditedFormattedValue, Decimal) Or
                        myReader(3) <> dgvPricesParadrugs.Rows(index).Cells(3).EditedFormattedValue Or
                        myReader(4) <> dgvPricesParadrugs.Rows(index).Cells(4).EditedFormattedValue Or
                        myReader(5) <> dgvPricesParadrugs.Rows(index).Cells(5).EditedFormattedValue Or
                        myReader(6) <> dgvPricesParadrugs.Rows(index).Cells(6).EditedFormattedValue Then

                        Return "Changed"

                    End If
                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function



    Private Function CheckIfRecordChangedOrExists_Expirations(ByVal index As Integer) As String
        Dim insertData As String = ""
        Dim NumericFields(10) As Decimal
        Dim str1 As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT isnull(Month,0), isnull(Year,'0') " &
                        "FROM  PharmacyCustomFiles.dbo.Expirations " &
                        "WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvExpirations.Rows(index).Cells(2).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()
                    Try
                        If myReader(0) <> CType(dgvExpirations.Rows(index).Cells(0).EditedFormattedValue, Integer) Or
                                                myReader(1) <> CType(dgvExpirations.Rows(index).Cells(1).EditedFormattedValue, Integer) Then

                            Return "Changed"

                        End If
                    Catch ex As Exception

                    End Try

                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function




    Private Function CheckIfRecordChangedOrExists_TameiaGiven(ByVal index As Integer) As String
        Dim insertData As String = ""
        Dim NumericFields(10) As Decimal
        Dim str1 As String = ""
        Dim MyDate As Date, Description As String, AmountPaid As Decimal

        '"VALUES (@MyDate, @Description, @AmountPaid, @PercTotalPaid, @DifferenceToTotal, @CompletedPayment, " & _
        '                        "@TameiaAskedId)"

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()
            insertData = "SELECT isnull(MyDate,''), isnull(Description,''), " &
                              "isnull(AmountPaid,0) " &
                         "FROM PharmacyCustomFiles.dbo.TameiaPaid " &
                         "WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvTameiaGiven.Rows(index).Cells(5).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()
                    Try
                        MyDate = CType(dgvTameiaGiven.Rows(index).Cells(0).EditedFormattedValue, Date)
                    Catch ex As Exception
                        MyDate = Today
                    End Try

                    Try
                        AmountPaid = CType(dgvTameiaGiven.Rows(index).Cells(2).EditedFormattedValue, Decimal)
                    Catch ex As Exception
                        AmountPaid = 0
                    End Try

                    Description = dgvTameiaGiven.Rows(index).Cells(1).EditedFormattedValue

                    'MsgBox("MR=" & myReader(3) & " - DGV=" & dgvTameiaGiven.Rows(index).Cells(5).EditedFormattedValue)

                    If myReader(0) <> MyDate Or
                       myReader(1) <> Description Or
                      myReader(2) <> AmountPaid Then

                        Return "Changed"

                    End If
                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function



    Private Function DePercent(ByVal var As String) As Decimal
        Dim varDec As Decimal
        Try
            varDec = CType(var.Substring(0, var.Length - 2), Decimal)
        Catch ex As Exception

        End Try
        ' MsgBox("vardec= " & varDec)

        Return varDec

    End Function


    Private Function CheckIfRecordChangedOrExists_FarmAgores(ByVal index As Integer) As String
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT isnull(Id,0), isnull(MyDate,''), isnull(CodeReceipt,0), isnull(Description,''), " &
                              "isnull(Agores130,0), isnull(Agores230,0), isnull(Agores650, 0), " &
                              "isnull(DapNoDisc, 0), isnull(DapWithDisc, 0), isnull(AgoresFPA, 0), isnull(DapFPA, 0) " &
                         "FROM PharmacyCustomFiles.dbo.FarmAgores " &
                         "WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", If(dgvAgoresSold.Rows(index).Cells(0).Value, DBNull.Value))

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    Dim NumericFields(10) As Decimal
                    For t = 4 To 10
                        If dgvAgoresSold.Rows(index).Cells(t).EditedFormattedValue = "" Then
                            NumericFields(t) = 0
                        Else
                            NumericFields(t) = CType(dgvAgoresSold.Rows(index).Cells(t).EditedFormattedValue, Decimal)
                        End If
                    Next

                    If myReader(1) <> CType(dgvAgoresSold.Rows(index).Cells(1).EditedFormattedValue, Date) Or
                       myReader(2) <> dgvAgoresSold.Rows(index).Cells(2).EditedFormattedValue Or
                       myReader(3) <> dgvAgoresSold.Rows(index).Cells(3).EditedFormattedValue Or
                      myReader(4) <> NumericFields(4) Or
                       myReader(5) <> NumericFields(5) Or
                      myReader(6) <> NumericFields(6) Or
                       myReader(7) <> NumericFields(7) Or
                       myReader(8) <> NumericFields(8) Or
                       myReader(9) <> NumericFields(9) Or
                      myReader(10) <> NumericFields(10) Then

                        Return "Changed"

                    End If
                Loop

                Return "NotChanged"
            End If

            Return "NewRow"

        End Using

    End Function


    Private Function CheckIfRecordChanged_Exchanges(ByVal index As Integer) As String
        Dim insertData As String = ""
        Dim FPA As Decimal = 0

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT isnull(Id,0), isnull(DrugName,''), isnull(FPA,0), isnull(Qnt,0), isnull(Xondr,0), isnull(Rp,0) FROM  PharmacyCustomFiles.dbo.Exchanges WHERE Id = @Id "

            Dim cmd As New SqlCommand(insertData, con)

            If ExchangesGivenOrTaken = "given" Then
                cmd.Parameters.AddWithValue("@Id", If(dgvGivenTo.Rows(index).Cells(0).Value, DBNull.Value))
            ElseIf ExchangesGivenOrTaken = "taken" Then
                cmd.Parameters.AddWithValue("@Id", If(dgvTakenFrom.Rows(index).Cells(0).Value, DBNull.Value))
            End If


            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then

                Do While myReader.Read()

                    Dim DrugName As String = "", Qnt As Integer = 0, Xondr As Decimal = 0, RP As String = ""

                    If ExchangesGivenOrTaken = "given" Then
                        If dgvGivenTo.Rows(index).Cells(1).EditedFormattedValue <> "" Then ' Drugname
                            DrugName = dgvGivenTo.Rows(index).Cells(1).EditedFormattedValue
                        End If
                        If dgvGivenTo.Rows(index).Cells(2).EditedFormattedValue <> "" Then ' FPA
                            FPA = CType(dgvGivenTo.Rows(index).Cells(2).EditedFormattedValue, Decimal)
                        End If
                        If dgvGivenTo.Rows(index).Cells(3).EditedFormattedValue <> "" Then ' Quantity
                            Qnt = CType(dgvGivenTo.Rows(index).Cells(3).EditedFormattedValue, Integer)
                        End If
                        If dgvGivenTo.Rows(index).Cells(4).EditedFormattedValue <> "" Then ' Xondriki
                            Xondr = CType(dgvGivenTo.Rows(index).Cells(4).EditedFormattedValue, Decimal)
                        End If
                        If dgvGivenTo.Rows(index).Cells(5).Value Is DBNull.Value Then ' RP
                        Else
                            'RP = dgvGivenTo.Rows(index).Cells(5).EditedFormattedValue
                            RP = dgvGivenTo.Rows(index).Cells(5).Value

                        End If

                    ElseIf ExchangesGivenOrTaken = "taken" Then
                        If dgvTakenFrom.Rows(index).Cells(1).EditedFormattedValue <> "" Then ' Drugname
                            DrugName = dgvTakenFrom.Rows(index).Cells(1).EditedFormattedValue
                        End If
                        If dgvTakenFrom.Rows(index).Cells(2).EditedFormattedValue <> "" Then ' FPA
                            FPA = CType(dgvTakenFrom.Rows(index).Cells(2).EditedFormattedValue, Decimal)
                        End If
                        If dgvTakenFrom.Rows(index).Cells(3).EditedFormattedValue <> "" Then ' Quantity
                            Try
                                Qnt = CType(dgvTakenFrom.Rows(index).Cells(3).EditedFormattedValue, Integer)
                            Catch ex As Exception
                                Qnt = 1
                            End Try
                        End If
                        If dgvTakenFrom.Rows(index).Cells(4).EditedFormattedValue <> "" Then ' Xondriki
                            Xondr = CType(dgvTakenFrom.Rows(index).Cells(4).EditedFormattedValue, Decimal)
                        End If
                        If dgvTakenFrom.Rows(index).Cells(5).Value Is DBNull.Value Then ' RP
                        Else
                            'RP = dgvTakenFrom.Rows(index).Cells(5).EditedFormattedValue
                            RP = dgvTakenFrom.Rows(index).Cells(5).Value
                        End If
                    End If


                    ' Aν έχει αλλάξει κάτι
                    If myReader(1) <> DrugName Or
                        CType(myReader(2), Decimal) <> FPA Or
                        CType(myReader(3), Integer) <> Qnt Or
                        CType(myReader(4), Decimal) <> Xondr Or
                        myReader(5) <> RP Then

                        Return "Changed"
                    Else
                        Return "NotChanged"
                    End If
                Loop
            Else
                Return "NewRow"

            End If

        End Using
        Return "Error"
    End Function


    'Private Sub ChangeControlsbtnEditExchangeFrom(ByVal selector As Boolean)
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save


    '    ' Γεμίζει το DatagridView 
    '    GetExchangesFrom()

    '    Try
    '        ' Φτιάχνει τα alignment για όλα τα πεδία
    '        For t = 0 To 5
    '            dgvExchangeFrom2.Columns(t).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '            dgvExchangeFrom2.Columns(t).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        Next
    '    Catch ex As Exception
    '    End Try


    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnEditExchangeFrom, btnSaveExchangeFrom, btnDeleteExchangeFrom}, dgvExchangeFrom2, selector)

    '    '' Ενεργοποίηση ενημερωτικού flashing label
    '    'timerLabel.Visible = selector
    '    'tmrFlashLabel.Enabled = selector

    'End Sub



    'Private Sub ChangeControlsbtnEditExchangeFrom2(ByVal selector As Boolean)
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save


    '    ' Γεμίζει το DatagridView 
    '    GetExchangedDrugsFrom()

    '    Try
    '        ' Φτιάχνει τα alignment για όλα τα πεδία
    '        For t = 0 To 5
    '            dgvExchangeFrom2.Columns(t).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '            dgvExchangeFrom2.Columns(t).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        Next
    '    Catch ex As Exception
    '    End Try


    '    'Ενεργοποιεί το Datetime picker
    '    dtpExchangesNew.Enabled = selector

    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnSaveExchangeFrom, btnEditExchangeFrom, btnDeleteExchangeFrom}, dgvExchangeFrom2, selector)

    '    '' Ενεργοποίηση ενημερωτικού flashing label
    '    'timerLabel.Visible = selector
    '    'tmrFlashLabel.Enabled = selector

    'End Sub



    'Private Sub ChangeControlsbtnEditExchangeTo2(ByVal selector As Boolean)
    '    ' Selector: True -> Edit
    '    '           False -> Cancel, Save


    '    ' Γεμίζει το DatagridView 
    '    GetExchangedDrugsTo()

    '    Try
    '        ' Φτιάχνει τα alignment για όλα τα πεδία
    '        For t = 0 To 5
    '            dgvExchangeFrom2.Columns(t).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '            dgvExchangeFrom2.Columns(t).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '        Next
    '    Catch ex As Exception
    '    End Try


    '    'Ενεργοποιεί το Datetime picker
    '    dtpExchangesNew.Enabled = selector

    '    ' Τροποποίηση κουμπιών κλπ ΕΝΤΟΣ του GroupBox που περιέχει το DataGrid μας
    '    EditDatagrid({btnSaveExchangeTo, btnEditExchangeTo, btnDeleteExchangeTo}, dgvExchangeTo2, selector)


    'End Sub





    'Private Sub btnSaveExchangeFrom_Click(sender As Object, e As EventArgs) Handles btnSaveExchangeFrom.Click

    '    ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
    '    UpdateExchangesFrom2()

    '    ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    ChangeControlsbtnEditExchangeFrom2(False)

    '    ' ανανεώνει τη λίστα
    '    GetExchangedDrugsFrom()

    'End Sub



    'Private Sub btnDeleteExchangeFrom_Click(sender As Object, e As EventArgs) Handles btnDeleteExchangeFrom.Click

    '    ' Ξεκινάει την διαδικασία Delete των δεδομένων του DataGrid 
    '    ' μαζί με τροποποίηση των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    DeleteExchangesFrom2()

    '    ' Τροποποιεί τα Controls της τρέχουσας form
    '    ChangeControlsbtnEditExchangeFrom2(False)

    '    ' ανανέωσε τη λίστα των Ζ
    '    GetExchangedDrugsFrom()

    'End Sub

    'Private Sub dgvExchangeFrom2_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvExchangeFrom2.CellClick

    '    If dgvExchangeFrom2.ReadOnly = False Then

    '        Dim cell As DataGridViewTextBoxCell = CType(dgvExchangeFrom2.Rows(e.RowIndex).Cells(e.ColumnIndex), DataGridViewTextBoxCell)

    '        ' Αν είμαστε στο ΟΝΟΜΑ...
    '        If e.ColumnIndex = 1 Then

    '            'Κρατάει τις συντεταγμένες του cell
    '            rowIndex = e.RowIndex
    '            columnIndex = e.ColumnIndex

    '            ' Ελέγχει αν είμαστε σε ανταλλαγή ΑΠΟ ή ΠΡΟΣ
    '            AreExchangedDrugsFrom = True

    '            ' Ανοίγει το form για να επιλέξουμε προιόν
    '            OpenChooseDrugDialog(frmChooseDrug.tbpDescription)

    '            ' Αν είμαστε στο BARCODE...
    '        ElseIf e.ColumnIndex = 0 Then

    '            'Κρατάει τις συντεταγμένες του cell
    '            rowIndex = e.RowIndex
    '            columnIndex = e.ColumnIndex

    '            ' Ελέγχει αν είμαστε σε ανταλλαγή ΑΠΟ ή ΠΡΟΣ
    '            AreExchangedDrugsFrom = True


    '            ' Ανοίγει το form για να επιλέξουμε προιόν
    '            OpenChooseDrugDialog(frmChooseDrug.tbpBarcode)


    '        Else

    '            ' Απενενεργοποιεί τον timer
    '            tmrExchanges.Enabled = False


    '        End If
    '    End If
    'End Sub


    'Private Sub OpenChooseDrugDialog(ByVal TabPage As TabPage)

    '    ' Ανοίγει το form για να επιλέξουμε προιόν
    '    frmChooseDrug.Show()

    '    frmChooseDrug.tbcChooseDrug.SelectedTab = TabPage

    '    Dim textBox As TextBox = Nothing

    '    If TabPage.Text = "Barcode" Then
    '        textBox = frmChooseDrug.txtSearchDrugsByBarcode
    '    ElseIf TabPage.Text = "Όνομα" Then
    '        textBox = frmChooseDrug.txtSearchDrugsByName
    '    End If

    '    'Focus στο textbook που εισάγεται το barcode και επιλογή του
    '    textBox.Focus()
    '    textBox.SelectionStart = 0
    '    textBox.SelectionLength = txtSearchDrugsNew.TextLength
    'End Sub


    'Private Sub GetDetailsFromBarcode(ByVal barcode As String, destinationDatagrid As DataGridView)

    '    Dim stringSQL As String = "SELECT APOTIKH_BARCODES.BRAP_AP_BARCODE, APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, APOTIKH.AP_TIMH_XON " & _
    '                                "FROM APOTIKH INNER JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID " & _
    '                                "WHERE APOTIKH_BARCODES.BRAP_AP_BARCODE= @barcode"
    '    Dim rowIndex As String = dgvExchangeFrom2.CurrentCell.RowIndex

    '    Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionString)
    '        con.Open()

    '        Dim cmd As New SqlCommand(stringSQL, con)
    '        cmd.Parameters.AddWithValue("@barcode", barcode)

    '        'Ορισμός ExecuteReader 
    '        Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

    '        If myReader.HasRows Then
    '            Do While myReader.Read()

    '                'Εμφανίζει στο Datagrid τις αντίστοιχες τιμές
    '                destinationDatagrid.Rows(rowIndex).Cells(1).Value = myReader(1) ' όνομα
    '                destinationDatagrid.Rows(rowIndex).Cells(2).Value = myReader(2) ' μορφή
    '                destinationDatagrid.Rows(rowIndex).Cells(4).Value = myReader(3) ' χονδρική

    '            Loop
    '        Else
    '            MsgBox("To barcode '" & barcode & "' δεν υπάρχει στο αρχείο μου")
    '        End If

    '    End Using

    'End Sub






    Private Sub dgvExchangeFrom2_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvExchangeFrom2.RowLeave
        'Δήλωση μεταβλητών
        Dim headerText As String = dgvExchangeFrom2.Columns(e.ColumnIndex).HeaderText


        'Αν έχουν καταχωρηθεί η Χονδρική τα Τεμάχια 

        Try
            If dgvExchangeFrom2.Rows(rowIndex).Cells(4).Value <> 0 Then

                If dgvExchangeFrom2.Rows(rowIndex).Cells(3).Value <> 0 Then

                    'Υπολογίζει και γράφει το Σύνολο  
                    dgvExchangeFrom2.Rows(rowIndex).Cells(5).Value = CType(dgvExchangeFrom2.Rows(rowIndex).Cells(3).Value, Integer) * CType(dgvExchangeFrom2.Rows(rowIndex).Cells(4).Value, Decimal)

                Else

                End If
            End If
        Catch ex As Exception
        End Try



    End Sub



    'Private Sub cboMyPharmacist_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboMyPharmacist.SelectedIndexChanged

    '    ' Εμφανίζει τις ΑΝΤΑΛΛΑΓΕΣ ΑΠΟ
    '    GetExchangedDrugsFrom()

    '    ' Εμφανίζει τις ΑΝΤΑΛΛΑΓΕΣ ΠΡΟΣ
    '    GetExchangedDrugsTo()

    '    'Υπολογίζει την χρηματική διαφορά
    '    DisplayBalancePerPharmacist()

    'End Sub

    'Private Sub cboIntervall_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboIntervall.SelectedIndexChanged
    '    ' Εμφανίζει τις ΑΝΤΑΛΛΑΓΕΣ ΑΠΟ
    '    GetExchangedDrugsFrom()

    '    ' Εμφανίζει τις ΑΝΤΑΛΛΑΓΕΣ ΠΡΟΣ
    '    GetExchangedDrugsTo()

    '    'Υπολογίζει την χρηματική διαφορά
    '    DisplayBalancePerPharmacist()

    'End Sub

    'Private Sub btnSaveExchangeTo_Click(sender As Object, e As EventArgs) Handles btnSaveExchangeTo.Click

    '    ' Ξεκινάει την διαδικασία Updating των δεδομένων του DataGrid 
    '    UpdateExchangesTo2()

    '    ' μαζί με των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    ChangeControlsbtnEditExchangeTo2(False)

    '    ' ανανεώνει τη λίστα
    '    GetExchangedDrugsTo()
    'End Sub

    'Private Sub btnEditExchangeTo_Click(sender As Object, e As EventArgs) Handles btnEditExchangeTo.Click
    '    'Αν το πληκτρο Edit δεν έχει πατηθεί ακόμα (->EDIT)... 
    '    If btnEditExchangeTo.Text = "Edit" Then

    '        ChangeControlsbtnEditExchangeTo2(True)

    '        'Αν το πληκτρο Edit έχει ήδη πατηθεί ακόμα (-> CANCEL)... 
    '    ElseIf btnEditExchangeTo.Text = "Cancel" Then

    '        ChangeControlsbtnEditExchangeTo2(False)

    '    End If
    'End Sub

    'Private Sub btnDeleteExchangeTo_Click(sender As Object, e As EventArgs) Handles btnDeleteExchangeTo.Click
    '    ' Ξεκινάει την διαδικασία Delete των δεδομένων του DataGrid 
    '    ' μαζί με τροποποίηση των κουμπιών που περιέχονται στο GroupBox του DataGrid
    '    DeleteExchangesTo2()

    '    ' Τροποποιεί τα Controls της τρέχουσας form
    '    ChangeControlsbtnEditExchangeTo2(False)

    '    ' ανανέωσε τη λίστα 
    '    GetExchangedDrugsTo()
    'End Sub

    'Private Sub dgvExchangeTo2_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvExchangeTo2.CellClick


    '    If dgvExchangeTo2.ReadOnly = False Then

    '        Dim cell As DataGridViewTextBoxCell = CType(dgvExchangeTo2.Rows(e.RowIndex).Cells(e.ColumnIndex), DataGridViewTextBoxCell)

    '        ' Αν είμαστε στο ΟΝΟΜΑ...
    '        If e.ColumnIndex = 1 Then

    '            'Κρατάει τις συντεταγμένες του cell
    '            rowIndex = e.RowIndex
    '            columnIndex = e.ColumnIndex

    '            ' Ελέγχει αν είμαστε σε ανταλλαγή ΑΠΟ ή ΠΡΟΣ
    '            AreExchangedDrugsFrom = False


    '            ' Ανοίγει το form για να επιλέξουμε προιόν
    '            OpenChooseDrugDialog(frmChooseDrug.tbpDescription)

    '            ' Αν είμαστε στο BARCODE...
    '        ElseIf e.ColumnIndex = 0 Then

    '            'Κρατάει τις συντεταγμένες του cell
    '            rowIndex = e.RowIndex
    '            columnIndex = e.ColumnIndex

    '            ' Ελέγχει αν είμαστε σε ανταλλαγή ΑΠΟ ή ΠΡΟΣ
    '            AreExchangedDrugsFrom = False

    '            ' Ανοίγει το form για να επιλέξουμε προιόν
    '            OpenChooseDrugDialog(frmChooseDrug.tbpBarcode)

    '        ElseIf e.ColumnIndex = 10 Then
    '            MsgBox(dgvExchangeTo2.SelectedRows(0).Cells(e.ColumnIndex).Value.ToString)
    '        Else

    '            ' Απενενεργοποιεί τον timer
    '            tmrExchanges.Enabled = False

    '        End If
    '    End If
    'End Sub


    'Private Sub dgvExchangeTo2_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvExchangeTo2.CellValidating


    '    If dgvExchangeTo2.ReadOnly = False Then

    '        'Δήλωση μεταβλητών
    '        Dim headerText As String = dgvExchangeTo2.Columns(e.ColumnIndex).HeaderText
    '        Dim int As Integer

    '        'Αν είμαστε στο πεδίο "Τεμάχια"
    '        If headerText.Equals("Τμχ") Then
    '            Try
    '                ' και η καταχωρημένη τιμή που ΔΕΝ είναι ακέραιος αριθμός
    '                If Not Integer.TryParse(e.FormattedValue.ToString, int) _
    '                        AndAlso btnEdit.Text = "Cancel" Then
    '                    MessageBox.Show("To '" & e.FormattedValue.ToString & "' δεν είναι ακέραιος αριθμός!", _
    '                                    "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '                    e.Cancel = True
    '                Else
    '                    'Αν έχει καταχωρηθεί η Χονδρική 
    '                    If dgvExchangeTo2.Rows(e.RowIndex).Cells(4).Value <> 0 Then

    '                        'Υπολογίζει και γράφει το Σύνολο  
    '                        dgvExchangeTo2.Rows(e.RowIndex).Cells(5).Value = CType(e.FormattedValue, Integer) * CType(dgvExchangeTo2.Rows(e.RowIndex).Cells(4).Value, Decimal)

    '                    End If

    '                    'Me.Hide()
    '                    'frmExpirationsAddNew.Show()

    '                End If
    '            Catch ex As Exception
    '            End Try

    '            'Αν είμαστε στο πεδίο "Χονδρική"
    '        ElseIf headerText.Equals("Χονδρική") Then
    '            Try
    '                ' και η καταχωρημένη τιμή που ΔΕΝ είναι χρηματική
    '                If e.FormattedValue.ToString <> String.Empty AndAlso Not Decimal.TryParse(e.FormattedValue.ToString.Substring(0, e.FormattedValue.ToString.Length - 1), int) AndAlso btnEdit.Text = "Cancel" Then
    '                    MessageBox.Show("To '" & e.FormattedValue.ToString & "' δεν είναι έγκυρη τιμή!", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '                    e.Cancel = True
    '                Else

    '                    ''Αν έχουν καταχωρηθεί τα Τεμάχια 
    '                    'If dgvExchangeTo2.Rows(e.RowIndex).Cells(3).Value <> 0 Then

    '                    '    'Υπολογίζει και γράφει το Σύνολο  
    '                    '    dgvExchangeTo2.Rows(e.RowIndex).Cells(5).Value = CType(e.FormattedValue, Decimal) * CType(dgvExchangeTo2.Rows(e.RowIndex).Cells(3).Value, Integer)

    '                End If
    '            Catch ex As Exception
    '            End Try

    '        ElseIf headerText.Equals("Σύνολο") Then
    '            Try
    '                ' Αν έχουν καταχωρηθεί τα Τεμάχια 
    '                If dgvExchangeTo2.Rows(e.RowIndex).Cells(3).Value <> 0 And e.FormattedValue <> dgvExchangeTo2.Rows(e.RowIndex).Cells(5).Value Then

    '                    ' Υπολογίζει την Χονδρική
    '                    dgvExchangeTo2.Rows(e.RowIndex).Cells(4).Value = CType(e.FormattedValue, Decimal) / CType(dgvExchangeTo2.Rows(e.RowIndex).Cells(3).Value, Integer)

    '                End If
    '            Catch ex As Exception
    '            End Try
    '        End If

    '    End If

    'End Sub



    Private Sub btnEditDrugsList_Click(sender As Object, e As EventArgs)
        ' ΠΡΩΤΑ απενεργοποιείται το TabControl
        tbcMain.Enabled = False

        ' META εμφανίζεται το frm με την λίστα των φαρμάκων προς διόρθωση
        frmDrugListEdit.Show()
    End Sub

    Private Sub btnEditDrugList2_Click(sender As Object, e As EventArgs) Handles btnEditDrugList2.Click
        ' ΠΡΩΤΑ απενεργοποιείται το TabControl
        tbcMain.Enabled = False

        ' META εμφανίζεται το frm με την λίστα των φαρμάκων προς διόρθωση
        frmDrugListEdit.Show()
    End Sub


    Private Sub IsBarcodeEnteredWhole()

    End Sub


    'Private Sub DisplayDrugsOrParadrugs(Optional mode As String = "")
    '    Dim str2find, qr As String

    '    str2find = txtSearchPricesParadrugs.Text
    '    qr = GetQRFromScannedCode(txtSearchPricesParadrugs.Text)


    '    If rbParadrugs.Checked = True Then
    '        dgvPricesParadrugs.ReadOnly = False
    '        If IsNumeric(str2find) = True Then
    '            rbByBarcode.Checked = True
    '        Else
    '            rbByName.Checked = True
    '        End If

    '        If rbByName.Checked = True Then
    '            GetPriceParaDrugs("name")

    '        ElseIf rbByBarcode.Checked = True Then

    '            GetPriceParaDrugs("barcode")

    '        End If
    '    ElseIf rbDrugs.Checked = True Then
    '        dgvPricesParadrugs.ReadOnly = True
    '        If IsNumeric(str2find) = True Then
    '            rbByBarcode.Checked = True
    '        ElseIf IsNumeric(qr) = True Then
    '            rbByBarcode.Checked = True
    '            txtSearchPricesParadrugs.Text = qr
    '        Else
    '            rbByName.Checked = True
    '        End If

    '        If rbByName.Checked = True Then
    '            GetDrugs("name")

    '        ElseIf rbByBarcode.Checked = True Then
    '            Dim found As Integer

    '            ' Πρώτα ψάχνει ανάμεσα στα Barcodes
    '            barcodeType = "barcode"
    '            rbByBarcode.Checked = True
    '            rbByQRcode.Checked = False
    '            found = GetDrugs("barcode")
    '            ' Αν δεν βρει τίποτα..
    '            If found = 0 Then
    '                ' ψάχνει και ανάμεσα στα QRcodes
    '                barcodeType = "qrcode"
    '                rbByBarcode.Checked = False
    '                rbByQRcode.Checked = True
    '                found = GetDrugs("qrcode")

    '            End If

    '        End If
    '    End If
    'End Sub

    Private Sub DisplayDrugsOrParadrugs(Optional mode As String = "")
        Dim str2find As String, found As Integer

        str2find = txtSearchPricesParadrugs.Text

        ' Ψάχνει πρώτα ανάμεσα στα φάρμακα
        If GetDrugs(barcodeType) = 0 Then
            GetPriceParaDrugs(barcodeType) ' και μετά στα παραφάρμακα
        End If

        'If rbParadrugs.Checked = True Then
        '    If GetPriceParaDrugs(barcodeType) = 0 Then
        '        GetDrugs(barcodeType)
        '    End If
        'ElseIf rbDrugs.Checked = True Then
        '    If GetDrugs(barcodeType) = 0 Then
        '        GetPriceParaDrugs(barcodeType)
        '    End If
        'End If
    End Sub

    Public Shared Function GetPercentFromDrug(ByVal Id As String) As String
        Dim Result, myValue As String

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            Dim SQLstring As String = "SELECT AP_FP_ID FROM APOTIKH WHERE  APOTIKH.AP_Code ='" & Id & "' "

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(SQLstring, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then
                    Do While myReader.Read()
                        myValue = myReader(0)
                    Loop
                End If

            End Using
        End Using

        Select Case myValue
            Case 1
                Result = "13"
            Case 2
                Result = "24"
            Case 3
                Result = "6"
            Case Else
                Result = "0"
        End Select

        Return Result
    End Function


    Private Sub dgvPricesParadrugs_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPricesParadrugs.CellClick

        ' Αλλάζει το ΦΠΑ στον υπολογισμό της Λιανικής ανάλογα με το επιλεγμένο φάρμακο
        Try
            If dgvPricesParadrugs.SelectedRows(0).Cells(4).Value <> 0 Then
                Dim fpa As Integer = 0

                SelectedDetailsApCode = dgvPricesParadrugs.SelectedRows(0).Cells(4).Value

                fpa = CType(frmParadrugSelectedDetails.GetOtherDetails("SELECT AP_FP_ID FROM APOTIKH "), Integer)

                Select Case fpa
                    Case 1
                        cboFPA_Paradrugs.Text = "13"
                    Case 2
                        cboFPA_Paradrugs.Text = "24"
                    Case 3
                        cboFPA_Paradrugs.Text = "6"
                End Select
            End If
        Catch ex As Exception
        End Try


        GetExpirationsList()

        If rbParadrugs.Checked = True Then

            CalculateNewLianikiSelectedDrug()

            ' Αν είμαστε στο AP_Code...
            If e.ColumnIndex = 4 Then

                'UpdateParadrugsOnLeave()

                'Try
                '    'Εαν δεν έχει Id = Νέα εγγραφή μη περασμένη
                '    If IsDBNull(dgvPricesParadrugs.SelectedRows(0).Cells(5).Value) = True Then
                '        txtSearchPricesParadrugs.Text = dgvPricesParadrugs.SelectedRows(0).Cells(0).Value
                '        'MsgBox(dgvPricesParadrugs.SelectedRows(0).Cells(5).Value & "-")
                '    End If
                'Catch ex As Exception

                'End Try

                'Κρατάει τις συντεταγμένες του cell
                rowIndex = e.RowIndex
                columnIndex = e.ColumnIndex

                If IsDBNull(dgvPricesParadrugs.SelectedRows(0).Cells(e.ColumnIndex).Value) Then

                    'MsgBox(dgvPricesParadrugs.SelectedRows(0).Cells(e.ColumnIndex).EditedFormattedValue & " " & dgvPricesParadrugs.SelectedRows(0).Cells(e.ColumnIndex).Value)

                    OpenDrugSelectionFromCatalogForm("newParadrug")

                    'Αν δεν έχουμε αντιστοιχήσει Κωδικό στο επιλεγμένο παραφάρμακο
                ElseIf dgvPricesParadrugs.SelectedRows(0).Cells(e.ColumnIndex).Value = 0 Then

                    'txtSearchPricesParadrugs.Text = dgvPricesParadrugs.SelectedRows(0).Cells(0).Value

                    OpenDrugSelectionFromCatalogForm("oldParadrug")

                ElseIf dgvPricesParadrugs.SelectedRows(0).Cells(e.ColumnIndex).Value > 0 Then

                    SelectedDetailsApCode = dgvPricesParadrugs.SelectedRows(0).Cells(4).Value
                    SelectedDetailsDrugName = dgvPricesParadrugs.SelectedRows(0).Cells(0).Value
                    OpenParadrugSelectionDetails()

                End If

            End If

            'dgvPricesParadrugs.Focus()

        ElseIf rbDrugs.Checked = True Then
            ' Αν είμαστε στο Όνομα...
            If e.ColumnIndex = 0 Then

                Me.Enabled = False

                'Κρατάει τις συντεταγμένες του cell
                rowIndex = e.RowIndex
                columnIndex = e.ColumnIndex

                SelectedDetailsApCode = dgvPricesParadrugs.SelectedRows(0).Cells(4).Value
                SelectedDetailsDrugName = dgvPricesParadrugs.SelectedRows(0).Cells(0).Value
                OpenParadrugSelectionDetails()

            End If
        End If


    End Sub


    Private Sub GetExpirationsList()
        Dim myTot As Integer

        Dim ParadrugId As Integer = 0
        Dim ParadrugName As String = ""
        Dim AP_ID As Integer = 0
        Dim AP_CODE As Integer = 0

        If rbParadrugs.Checked = True Then

            Try
                ParadrugId = dgvPricesParadrugs.SelectedRows(0).Cells(5).Value
                AP_ID = dgvPricesParadrugs.SelectedRows(0).Cells(6).Value
                AP_CODE = dgvPricesParadrugs.SelectedRows(0).Cells(4).Value
            Catch ex As Exception
                ParadrugId = 0
            End Try

            Try
                ParadrugName = dgvPricesParadrugs.SelectedRows(0).Cells(0).Value
            Catch ex As Exception
            End Try



            If ParadrugId > 0 Then
                lblDirtyState_Exp.Text = ""
                txtRowChanged.Text = "(" & ParadrugId & ") " & ParadrugName
                stringDTG = "SELECT Month, Year, Id, ProductName " &
                                              "FROM PharmacyCustomFiles.[dbo].[Expirations] " &
                                              "WHERE (ParadrugId = '" & ParadrugId & "') AND (Month is not null) AND (Year is not null) " &
                                                     "AND Category = 'ΠΑΡΑΦΑΡΜΑΚΑ' " &
                                              "ORDER BY Year, Month"

            ElseIf ParadrugId = 0 And ParadrugName <> "" Then
                lblDirtyState_Exp.Text = "No Paradrug ID!"
                txtRowChanged.Text = ParadrugName
                stringDTG = "SELECT Month, Year, Id, ProductName " &
                                             "FROM PharmacyCustomFiles.[dbo].[Expirations] " &
                                             "WHERE (ProductName = '" & ParadrugName & "') AND (Month is not null) AND (Year is not null) " &
                                             "AND Category = 'ΠΑΡΑΦΑΡΜΑΚΑ' " &
                                             "ORDER BY Year, Month"
            Else
                Exit Sub
            End If



        ElseIf rbDrugs.Checked = True Then
            Try
                'ParadrugId = dgvPricesParadrugs.SelectedRows(0).Cells(5).Value
                AP_ID = dgvPricesParadrugs.SelectedRows(0).Cells(5).Value
                AP_CODE = dgvPricesParadrugs.SelectedRows(0).Cells(4).Value
            Catch ex As Exception
            End Try

            stringDTG = "SELECT Month, Year, Id " &
                               "FROM PharmacyCustomFiles.[dbo].[Expirations] " &
                               "WHERE (AP_ID = '" & AP_ID & "' AND AP_CODE = '" & AP_CODE & "') AND (Month is not null) AND (Year is not null) " &
                                    "AND Category = 'ΦΑΡΜΑΚΑ' " &
                                "ORDER BY Year, Month"

        End If

        myTot = FillDatagrid(dgvExpirations, bsExpDates2, {"Μήνας", "Έτος"}, {50, 50}, {"", ""}, {"Id", "ProductName"})

        If myTot = 0 Then
            HideExpirationDatagrid(True)
        Else
            HideExpirationDatagrid(False)
        End If


    End Sub


    Private Sub HideExpirationDatagrid(ByVal mode As Boolean)
        dgvExpirations.Visible = Not mode
        txtNoExpirations.Visible = mode
        btnDeleteExpiration.Visible = Not mode

        'Πάει στο τελευταίο record του datagrid
        'Me.dgvExpirations.FirstDisplayedScrollingRowIndex = Me.dgvTakenFrom.RowCount - 1
        If mode = False And rbByBarcode.Checked = True And chkManualBarcode.Checked = False Then
            Try
                dgvExpirations.Focus()
                dgvExpirations.CurrentCell = dgvExpirations(0, dgvExpirations.RowCount - 1)
                dgvExpirations.BeginEdit(True)
            Catch ex As Exception
            End Try
        End If




    End Sub


    'Private Sub dgvPricesParadrugs_DefaultValuesNeeded(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowEventArgs) Handles dgvPricesParadrugs.DefaultValuesNeeded

    '    With e.Row
    '        .Cells(5).Value = 0
    '    End With

    'End Sub

    Private Sub OpenDrugSelectionFromCatalogForm(ByVal mode As String)
        Dim textBox As TextBox = Nothing
        Dim str As String = ""

        'Περνάει στην αναζήτηση τη πρώτη λέξη της ονομασίας του επιλεγμένου παραφάρμακου
        Try
            Select Case mode
                Case "newParadrug"
                    str = dgvPricesParadrugs.SelectedRows(0).Cells(0).EditedFormattedValue
                Case "newExchanges"
                    If IsDBNull(dgvGivenTo.SelectedRows(0).Cells(0).EditedFormattedValue) = False Then
                        str = ""
                    Else
                        str = dgvGivenTo.SelectedRows(0).Cells(0).EditedFormattedValue
                    End If
                Case "newExchangesTaken"
                    If IsDBNull(dgvTakenFrom.SelectedRows(0).Cells(0).EditedFormattedValue) = False Then
                        str = ""
                    Else
                        str = dgvTakenFrom.SelectedRows(0).Cells(0).EditedFormattedValue
                    End If

                Case "oldParadrug"
                    str = dgvPricesParadrugs.SelectedRows(0).Cells(0).Value
                Case "oldExchanges"
                    str = dgvGivenTo.SelectedRows(0).Cells(0).Value
                Case "oldExchangesTaken"
                    str = dgvTakenFrom.SelectedRows(0).Cells(0).Value
            End Select
        Catch ex As Exception
            Exit Sub
            Me.Enabled = True
        End Try

        ' Ανοίγει το form για να επιλέξουμε προιόν
        frmChooseParadrugFromCatalog.Show()

        textBox = frmChooseParadrugFromCatalog.txtSearchParaDrugsByName
        Try
            textBox.Text = str.Substring(0, str.IndexOf(" "))
        Catch ex As Exception
            textBox.Text = str
        End Try

        'Focus στο textbook που εισάγεται το barcode και επιλογή του
        textBox.Focus()
        textBox.SelectionStart = 0
        textBox.SelectionLength = textBox.TextLength

    End Sub

    Private Sub OpenExpirationPairingForm()
        Dim textBox As TextBox = Nothing
        Dim str As String = ""

        ' Ανοίγει το form για να επιλέξουμε προιόν
        frmExpirationsList.Show()

        'Περνάει στην αναζήτηση τη πρώτη λέξη της ονομασίας του επιλεγμένου παραφάρμακου
        'str = dgvPricesParadrugs.SelectedRows(0).Cells(0).EditedFormattedValue
        str = dgvPricesParadrugs.SelectedRows(0).Cells(0).Value

        textBox = frmExpirationsList.txtSearchExpirations
        Try
            textBox.Text = str.Substring(0, str.IndexOf(" "))
        Catch ex As Exception
            textBox.Text = str
        End Try

        'Focus στο textbook που εισάγεται το barcode και επιλογή του
        textBox.Focus()
        textBox.SelectionStart = 0
        textBox.SelectionLength = textBox.TextLength

    End Sub

    Private Sub OpenParadrugSelectionDetails()

        ' Ανοίγει το form για να επιλέξουμε προιόν
        frmParadrugSelectedDetails.Show()

    End Sub

    'Private Sub dgvPricesParadrugs_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPricesParadrugs.RowLeave

    '    UpdatePriceSpecificParadrug(e.RowIndex)

    'End Sub

    Private Sub btnDeletePriceParadrugs_Click(sender As Object, e As EventArgs) Handles btnDeletePriceParadrugs.Click
        Dim ExpRows As Integer = CheckIfProductHasExpirations()
        If ExpRows = 0 Then
            DeletePricesParadrugs()
            GetPriceParaDrugs("name")
            GetExpirationsList()
        Else
            MsgBox("ΑΔΥΝΑΤΗ ΔΙΑΓΡΑΦΗ - Το προιόν έχει ακόμα λήξεις (" & ExpRows & ")")
        End If

    End Sub





    Private Sub btnOpenFolderDB_Click(sender As Object, e As EventArgs) Handles btnOpenFolderDB.Click
        Dim FolderPath As String = txtSourceDB.Text
        Process.Start("explorer.exe", FolderPath)
    End Sub


    Private Sub btnOpenFolderVS_Click(sender As Object, e As EventArgs) Handles btnOpenFolderVS.Click
        Dim FolderPath As String = txtSourceFolderVS.Text
        Process.Start("explorer.exe", FolderPath)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles btnOpenDestinationFolder.Click
        Dim FolderPath As String = txtDestinationDrive.Text
        Process.Start("explorer.exe", FolderPath)
    End Sub




    Private Sub rbWhereFarm1_CheckedChanged(sender As Object, e As EventArgs) Handles rbWhereFarm1.CheckedChanged
        txtSourceFolderVS.Text = "???"
        txtDestinationDrive.Text = "???"
        'If txtDB1.Text <> "" Then
        '    btnUpdatePharmacy2013C.Text = "Update " & txtDB1.Text
        'Else
        '    btnUpdatePharmacy2013C.Text = "Update Pharmacy2013C"
        'End If
        btnBackupRestore.Enabled = True
        btnUpdatePharmacy2013C.Enabled = True
        btnCoppyAppStation1.Enabled = False

    End Sub

    Private Sub rbWhereSpiti_CheckedChanged(sender As Object, e As EventArgs) Handles rbWhereSpiti.CheckedChanged
        txtSourceFolderVS.Text = "F:\Documents & Projects\Visual Studio 2012 - LEARNING\Pharmacy"
        txtDestinationDrive.Text = "D:\PharmacyBackup"
        btnBackupRestore.Enabled = True
        btnUpdatePharmacy2013C.Enabled = True
        btnCoppyAppStation1.Enabled = False
    End Sub


    Private Sub rbPC2Usb_CheckedChanged(sender As Object, e As EventArgs) Handles rbPC2Usb.CheckedChanged
        If rbEverything.Checked = True Then
            btnBackupRestore.Text = "Backup Everything"
        ElseIf rbOnlyDatabases.Checked = True Then
            btnBackupRestore.Text = "Backup Only Databases"
        ElseIf rbOnlyVisualBasic.Checked = True Then
            btnBackupRestore.Text = "Backup Only Visual Basic Files"
        End If

    End Sub

    Private Sub rbUsb2PC_CheckedChanged(sender As Object, e As EventArgs) Handles rbUsb2PC.CheckedChanged
        If rbEverything.Checked = True Then
            btnBackupRestore.Text = "Restore Everything"
        ElseIf rbOnlyDatabases.Checked = True Then
            btnBackupRestore.Text = "Restore Only Databases"
        ElseIf rbOnlyVisualBasic.Checked = True Then
            btnBackupRestore.Text = "Restore Only Visual Basic Files"
        End If

    End Sub


    Private Sub cbExchangers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbExchangers.SelectedIndexChanged

        GetExchangesList("given")
        GetExchangesList("taken")
        CalculatePreviousTotalBalance()
        DisplayExchangesBalance()
        DisplayFPAPerCurrentIntervall()

    End Sub

    Private Sub dgvGivenTo_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvGivenTo.CellClick
        If e.ColumnIndex = 1 And chkAutoInsertName.Checked = True Then

            ExchangesGivenOrTaken = "given"

            'Κρατάει τις συντεταγμένες του cell
            rowIndex = e.RowIndex
            columnIndex = e.ColumnIndex

            'Αν πρόκειται για καταχώρηση προιόντος σε νέα ανταλλαγή
            If IsDBNull(dgvGivenTo.SelectedRows(0).Cells(e.ColumnIndex).Value) = True Then

                Me.Enabled = False

                OpenDrugSelectionFromCatalogForm("newExchanges")

            End If

        ElseIf e.ColumnIndex = 5 Then

            ExchangesGivenOrTaken = "given"

            Me.Enabled = False

            ' Ανοίγει το form για να επιλέξουμε προιόν
            frmNarkoticWithSpecialExchange.Show()

        End If
    End Sub

    Private Function GetXondrSelectedDrug(ByVal ApCode As String) As Decimal
        Dim Xondr As Decimal = GetEffectiveDrugXondr(ApCode)
        SelectedDetailsXondr = Xondr
        Return Xondr
    End Function

    Private Sub PersistExchangeXondrOverride(ByVal dgv As DataGridView, ByVal rowIndex As Integer)
        Try
            If rowIndex < 0 OrElse rowIndex >= dgv.Rows.Count Then Exit Sub
            If dgv.Rows(rowIndex).IsNewRow Then Exit Sub

            Dim apCode As String = Convert.ToString(dgv.Rows(rowIndex).Cells(6).Value).Trim()
            If String.IsNullOrWhiteSpace(apCode) Then Exit Sub

            Dim drugName As String = Convert.ToString(dgv.Rows(rowIndex).Cells(1).Value).Trim()
            Dim qnt As Integer = 0
            Dim totalXondr As Decimal = 0D

            Integer.TryParse(Convert.ToString(dgv.Rows(rowIndex).Cells(3).Value), qnt)
            Decimal.TryParse(Convert.ToString(dgv.Rows(rowIndex).Cells(4).Value), totalXondr)

            If qnt <= 0 OrElse totalXondr < 0D Then Exit Sub

            Dim unitXondr As Decimal = Decimal.Round(totalXondr / qnt, 4, MidpointRounding.AwayFromZero)
            SaveDrugXondrOverride(apCode, drugName, unitXondr)
        Catch ex As Exception
        End Try
    End Sub

    'Private Sub dgvGivenTo_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvGivenTo.CellValidating

    '    Dim headerText As String = dgvGivenTo.Columns(e.ColumnIndex).HeaderText

    '    ' Ελέγχει αν βρισκόμαστε στο πεδίο Ποσότητα
    '    If headerText.Equals("Ποσ") Then
    '        Dim intg As Integer
    '        Dim xondr As Decimal = 0

    '        If IsDBNull(dgvGivenTo.SelectedRows(0).Cells(5).Value) = True Then
    '            Exit Sub
    '        Else
    '            xondr = GetXondrSelectedDrug(dgvGivenTo.SelectedRows(0).Cells(5).Value)
    '        End If


    '        If e.FormattedValue.ToString <> String.Empty AndAlso Integer.TryParse(e.FormattedValue.ToString, intg) = False Then
    '            MessageBox.Show("Λάθος καταχώρηση ποσότητας", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            e.Cancel = True
    '        ElseIf e.FormattedValue.ToString <> String.Empty And chkAutoInsertName.Checked = True Then
    '            dgvGivenTo.SelectedRows(0).Cells(3).Value = xondr * CType(e.FormattedValue, Integer)
    '        End If

    '    End If

    'End Sub

    Private Sub dgvGivenTo_DefaultValuesNeeded(sender As Object, e As DataGridViewRowEventArgs) Handles dgvGivenTo.DefaultValuesNeeded

        'Σώνει την θέση στο Datagrid του νέου row
        rowIndex = e.Row.Index

        'Ορίζει τις default τιμές για τα νέα Row του Datagrid
        With e.Row
            .Cells(3).Value = "1" ' Quantity
            .Cells(4).Value = "0" ' Xondr
            .Cells(7).Value = Now() ' ημερομηνία και ώρα
        End With

    End Sub


    Private Sub dgvTakenFrom_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTakenFrom.CellEndEdit
        'MsgBox("EDV=" & dgvTakenFrom.Rows(e.RowIndex).Cells(2).EditedFormattedValue & " - V= " & dgvTakenFrom.Rows(e.RowIndex).Cells(2).Value)
        'MsgBox("EDV=" & dgvTakenFrom.Rows(e.RowIndex).Cells(1).EditedFormattedValue & " - V= " & dgvTakenFrom.Rows(e.RowIndex).Cells(1).Value)

        ' Αν πιέσουμε πάνω σε ποσότητα επαναπροσδιορίζει την τιμή
        If e.ColumnIndex = 3 Then
            RecalculateTotalXondr_TakenFrom(e.RowIndex, dgvTakenFrom.Rows(e.RowIndex).Cells(4).Value)
        End If


    End Sub


    Private Sub btnDeleteGivenTo_Click(sender As Object, e As EventArgs) Handles btnDeleteGivenTo.Click
        DeleteExchangesMulti("given")
        GetExchangesList("given")
        DisplayExchangesBalance()
        DisplayFPAPerCurrentIntervall()

    End Sub

    Private Sub flpExchanges_AutoSizeChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub dgvGivenTo_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvGivenTo.CellContentClick
        If e.ColumnIndex = 1 And chkAutoInsertName.Checked = True Then

            ExchangesGivenOrTaken = "given"

            Me.Enabled = False

            'Κρατάει τις συντεταγμένες του cell
            rowIndex = e.RowIndex
            columnIndex = e.ColumnIndex

            'Αν έχουμε ήδη καταχωρήσει όνομα προιόντος αλλά χωρίς κωδικό
            If IsDBNull(dgvGivenTo.SelectedRows(0).Cells(e.ColumnIndex).Value) = False And (IsDBNull(dgvGivenTo.SelectedRows(0).Cells(6).Value) = True OrElse dgvGivenTo.SelectedRows(0).Cells(6).Value = "") Then

                OpenDrugSelectionFromCatalogForm("oldExchanges")

            Else

                SelectedDetailsApCode = dgvGivenTo.SelectedRows(0).Cells(6).Value
                SelectedDetailsDrugName = dgvGivenTo.SelectedRows(0).Cells(1).Value
                OpenParadrugSelectionDetails()

                dgvGivenTo.SelectedRows(0).Cells(4).Value = dgvGivenTo.SelectedRows(0).Cells(3).Value * SelectedDetailsXondr

            End If

        End If

    End Sub

    Private Sub ApplyReadOnlyOnExchangeGrids()
        For Each dgv As DataGridView In New DataGridView() {dgvGivenTo, dgvTakenFrom}
            dgv.ReadOnly = False
            dgv.EditMode = DataGridViewEditMode.EditOnEnter
            dgv.AllowUserToAddRows = False                        ' χωρίς new-row
            dgv.AllowUserToDeleteRows = False                     ' χωρίς delete από πληκτρολόγιο
            dgv.MultiSelect = True                                ' κρατάμε multi-select για μαζικές διαγραφές με το κουμπί
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect

            If dgv.Columns.Count > 0 Then
                For Each col As DataGridViewColumn In dgv.Columns
                    col.ReadOnly = True
                Next

                If dgv.Columns.Count > 4 Then
                    dgv.Columns(4).ReadOnly = False
                    dgv.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            End If
        Next
    End Sub

    Private Sub btnDeleteTakenFrom_Click(sender As Object, e As EventArgs) Handles btnDeleteTakenFrom.Click

        DeleteExchangesMulti("taken")
        GetExchangesList("taken")
        DisplayExchangesBalance()
        DisplayFPAPerCurrentIntervall()

    End Sub

    Private Sub dgvTakenFrom_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTakenFrom.CellClick
        If e.ColumnIndex = 1 And chkAutoInsertName.Checked = True Then

            ExchangesGivenOrTaken = "taken"

            'Κρατάει τις συντεταγμένες του cell
            rowIndex = e.RowIndex
            columnIndex = e.ColumnIndex

            'Αν πρόκειται για καταχώρηση προιόντος σε νέα ανταλλαγή
            If IsDBNull(dgvTakenFrom.SelectedRows(0).Cells(e.ColumnIndex).Value) = True Then

                Me.Enabled = False

                OpenDrugSelectionFromCatalogForm("newExchangesTaken")

            End If

        ElseIf e.ColumnIndex = 5 Then

            ExchangesGivenOrTaken = "taken"
            Me.Enabled = False

            ' Ανοίγει το form για να επιλέξουμε προιόν
            frmNarkoticWithSpecialExchange.Show()

        End If

    End Sub

    Private Sub dgvTakenFrom_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTakenFrom.CellContentClick
        If e.ColumnIndex = 1 And chkAutoInsertName.Checked = True Then

            ExchangesGivenOrTaken = "taken"

            'Κρατάει τις συντεταγμένες του cell
            rowIndex = e.RowIndex
            columnIndex = e.ColumnIndex

            'Αν έχουμε ήδη καταχωρήσει όνομα προιόντος αλλά χωρίς κωδικό
            If IsDBNull(dgvTakenFrom.SelectedRows(0).Cells(e.ColumnIndex).Value) = False And (IsDBNull(dgvTakenFrom.SelectedRows(0).Cells(6).Value) = True OrElse dgvTakenFrom.SelectedRows(0).Cells(6).Value = "") Then

                Me.Enabled = False

                OpenDrugSelectionFromCatalogForm("oldExchangesTaken")

            Else

                SelectedDetailsApCode = dgvTakenFrom.SelectedRows(0).Cells(6).Value
                SelectedDetailsDrugName = dgvTakenFrom.SelectedRows(0).Cells(1).Value
                OpenParadrugSelectionDetails()

                dgvTakenFrom.SelectedRows(0).Cells(4).Value = dgvTakenFrom.SelectedRows(0).Cells(3).Value * SelectedDetailsXondr

            End If

        End If

    End Sub

    Private Sub dgvTakenFrom_DefaultValuesNeeded(sender As Object, e As DataGridViewRowEventArgs) Handles dgvTakenFrom.DefaultValuesNeeded
        'Ορίζει deafault τιμές 
        With e.Row
            .Cells(3).Value = "1" ' Quantity
            .Cells(4).Value = "0" ' Xondr
            .Cells(7).Value = Now() ' ημερομηνία και ώρα
        End With
    End Sub


    Private Sub btnExchangesBalancePerPharmacist_Click(sender As Object, e As EventArgs) Handles btnExchangesBalancePerPharmacist.Click
        Me.Enabled = False
        frmExchangesTotalsPerPharmacist.Show()
    End Sub

    Private Sub dgvAgoresSold_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvAgoresSold.CellClick
        If cbAgoresOrSold.Text = "Έξοδα (Δαπάνες)" And e.ColumnIndex = 3 Then

            'Κρατάει τις συντεταγμένες του cell
            rowIndex = e.RowIndex
            columnIndex = e.ColumnIndex

            Me.Enabled = False
            frmAddAgoresToList.Show()

        End If
    End Sub

    Private Sub dgvAgoresSold_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvAgoresSold.CellContentClick

    End Sub

    Private Sub dgvAgoresSold_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgvAgoresSold.CellEnter
        If cbAgoresOrSold.Text = "Έξοδα (Δαπάνες)" And e.ColumnIndex = 3 Then

            'Κρατάει τις συντεταγμένες του cell
            rowIndex = e.RowIndex
            columnIndex = e.ColumnIndex

            Me.Enabled = False
            frmAddAgoresToList.Show()

        End If
    End Sub

    Private Sub dgvAgoresSold_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvAgoresSold.RowLeave
        If cbAgoresOrSold.Text = "Έσοδα (Πωλήσεις)" Then
            UpdateSold(e.RowIndex)
        ElseIf cbAgoresOrSold.Text = "Έξοδα (Δαπάνες)" Then
            UpdateAgores(e.RowIndex)
        End If

    End Sub

    Private Sub cbAgoresOrSold_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbAgoresOrSold.SelectedIndexChanged
        GetAgoresOrSoldList()
        DisplayAgoresSoldTotals()
    End Sub

    Private Sub txtAgoresSoldSearch_TextChanged(sender As Object, e As EventArgs) Handles txtAgoresSoldSearch.TextChanged
        GetAgoresOrSoldList()
        DisplayAgoresSoldTotals()

    End Sub

    Private Sub btnAgoresSoldDeleteRecord_Click(sender As Object, e As EventArgs) Handles btnAgoresSoldDeleteRecord.Click
        DeleteSuppliers()
        GetAgoresOrSoldList()
        DisplayAgoresSoldTotals()
    End Sub


    Private Sub DisplayAgoresSoldTotals()
        Dim sqlString As String = ""

        If cbAgoresOrSold.Text = "Έσοδα (Πωλήσεις)" Then

            sqlString = "SELECT * From PharmacyCustomFiles.dbo.FarmSold " &
                                         "WHERE Description like '%" & txtAgoresSoldSearch.Text.ToString & "%' " &
                                         "ORDER By MyDate"

            ' Γράφει μια ενημερωση στο Label κάτω από το datagrid
            ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν

            Dim totalRecords As Integer = CalculateRecords(sqlString)
            Dim totalXondr130 As Decimal = CalculateSums(sqlString, "SoldXondr130")
            Dim totalXondr230 As Decimal = CalculateSums(sqlString, "SoldXondr230")
            Dim totalXondr650 As Decimal = CalculateSums(sqlString, "SoldXondr650")
            Dim totalLian130 As Decimal = CalculateSums(sqlString, "SoldLian130")
            Dim totalLian230 As Decimal = CalculateSums(sqlString, "SoldLian230")
            Dim totalLian650 As Decimal = CalculateSums(sqlString, "SoldLian650")
            Dim totalinFPA As Decimal = CalculateSums(sqlString, "InFPA")

            Dim total As Decimal = totalXondr130 + totalXondr230 + totalXondr650 + totalLian130 + totalLian230 + totalLian650
            Dim grandtotal As Decimal = total + totalinFPA

            Select Case totalRecords
                Case 0
                    rtxtAgoresSoldMessage.Text = "Δεν υπάρχουν εγγραφές!"
                Case 1
                    rtxtAgoresSoldMessage.Text = "Βλέπετε 1 εγγραφή       Καθαρά " & total.ToString("###,###.## €       ") & "ΦΠΑ " &
                                                    totalinFPA.ToString("###,###.## €       ") & "Σύνολο " & grandtotal.ToString("###,###.## €") & " "
                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBoxGreen(rtxtAgoresSoldMessage, {"1", total.ToString("###,###.##"), totalinFPA.ToString("###,###.##"), grandtotal.ToString("###,###.##")})
                Case Is > 1
                    rtxtAgoresSoldMessage.Text = "Βλέπετε " & totalRecords.ToString & " εγγραφές       Καθαρά " & total.ToString("###,###.## €       ") &
                                                 "   ΦΠΑ " & totalinFPA.ToString("###,###.## €       ") & "Σύνολο " & grandtotal.ToString("###,###.#0 €") & " "
                    HightlightInRichTextBoxGreen(rtxtAgoresSoldMessage, {totalRecords.ToString, total.ToString("###,###.##"), totalinFPA.ToString("###,###.##"), grandtotal.ToString("###,###.#0")})
            End Select

        ElseIf cbAgoresOrSold.Text = "Έξοδα (Δαπάνες)" Then

            sqlString = "SELECT * From PharmacyCustomFiles.dbo.FarmAgores " &
                                        "WHERE Description like '%" & txtAgoresSoldSearch.Text.ToString & "%' " &
                                        "ORDER By MyDate"

            ' Γράφει μια ενημερωση στο Label κάτω από το datagrid
            ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν

            Dim totalRecords As Integer = CalculateRecords(sqlString)
            Dim totalAgores130 As Decimal = CalculateSums(sqlString, "Agores130")
            Dim totalAgores230 As Decimal = CalculateSums(sqlString, "Agores230")
            Dim totalAgores650 As Decimal = CalculateSums(sqlString, "Agores650")
            Dim totalDapNoDisc As Decimal = CalculateSums(sqlString, "DapNoDisc")
            Dim totalDapWithDisc As Decimal = CalculateSums(sqlString, "DapWithDisc")
            Dim totalAgoresFPA As Decimal = CalculateSums(sqlString, "AgoresFPA")
            Dim totalDapFPA As Decimal = CalculateSums(sqlString, "DapFPA")

            Dim total As Decimal = totalAgores130 + totalAgores230 + totalAgores650 + totalDapNoDisc + totalDapWithDisc
            Dim totalFPA = totalAgoresFPA + totalDapFPA
            Dim grandtotal As Decimal = total + totalFPA


            Select Case totalRecords
                Case 0
                    rtxtAgoresSoldMessage.Text = "Δεν υπάρχουν εγγραφές!"
                Case 1
                    rtxtAgoresSoldMessage.Text = "Βλέπετε 1 εγγραφή       Καθαρά " & total.ToString("###,###.## €       ") & "ΦΠΑ " &
                                                    totalFPA.ToString("###,###.## €") & "       Σύνολο " & grandtotal.ToString("###,###.## €") & " "
                    ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
                    HightlightInRichTextBox(rtxtAgoresSoldMessage, {"1", total.ToString("###,###.##"), totalFPA.ToString("###,###.##"), grandtotal.ToString("###,###.##")})
                Case Is > 1
                    rtxtAgoresSoldMessage.Text = "Βλέπετε " & totalRecords.ToString & " εγγραφές       Καθαρά " & total.ToString("###,###.## €       ") & "ΦΠΑ " &
                                                    totalFPA.ToString("###,###.## €") & "       Σύνολο " & grandtotal.ToString("###,###.## €") & " "
                    HightlightInRichTextBox(rtxtAgoresSoldMessage, {totalRecords.ToString, total.ToString("###,###.##"), totalFPA.ToString("###,###.##"), grandtotal.ToString("###,###.#0")})
            End Select
        End If


    End Sub



    ' Debounce 350ms για την αναζήτηση πελατών
    Private Sub txtSearchCustomer_TextChanged(sender As Object, e As EventArgs) Handles txtSearchCustomer.TextChanged
        If _suppressSearchTextChanged Then Return  ' δεν κάνουμε αναζήτηση όταν αλλάζουμε το Text προγραμματικά
        tmrSearchCustomers.Stop()
        _debounceSnap = txtSearchCustomer.Text   ' snapshot για να αγνοούμε «μπαγιάτικα» ticks
        tmrSearchCustomers.Interval = 350        ' 250–400 ms είναι καλή τιμή
        tmrSearchCustomers.Start()
    End Sub





    Private Sub DisplayLabelIfCustomerWithoutDebtsOrHairdies()
        Dim TotalCustomerDebt As Decimal = CType(lblTotalCustomerDebt.Text.Substring(0, Len(lblTotalCustomerDebt.Text) - 1), Decimal)

        'If lblTotalCustomerDebt.Text = "0.00 €" Then
        '    TotalCustomerDebt = 0
        'Else
        '    TotalCustomerDebt = CType(lblTotalCustomerDebt.Text, Decimal)
        'End If
        'TotalCustomerDebt = lblTotalCustomerDebt.Text.Substring(0, Len(lblTotalCustomerDebt.Text) - 1)


        If TotalCustomerDebt >= 0 Then
            lblTotalCustomerDebt.ForeColor = Color.Black
        Else
            lblTotalCustomerDebt.ForeColor = Color.Crimson
        End If


        If TotalCustomerDebt = 0 Then
            Try
                If dgvDebtsList.SelectedRows(0).Cells(1).Value = 0 Then
                    ActivateDatagridDebts(True)
                Else
                    ActivateDatagridDebts(False)
                End If
            Catch ex As Exception
                ActivateDatagridDebts(False)
            End Try

        ElseIf TotalCustomerDebt <> 0 Then
            ActivateDatagridDebts(True)
        End If


        Try
            If dgvHairdiesList.RowCount = 1 Then
                Try
                    If IsDBNull(dgvHairdiesList.SelectedRows(0).Cells(1).Value) = True Then
                        ActivateDatagridHairdies(True)
                    Else
                        ActivateDatagridHairdies(False)
                    End If
                Catch ex As Exception
                    ActivateDatagridHairdies(False)
                End Try

            ElseIf dgvHairdiesList.RowCount > 1 Then
                ActivateDatagridHairdies(True)
            End If
        Catch ex As Exception
        End Try

        'MsgBox(dgvDrugsOnLoan.RowCount)

        Try
            ActivateDatagridDrugsOnLoan(dgvDrugsOnLoan.RowCount > 0)
        Catch ex As Exception
        End Try

        Try
            If dgvPrescriptions.RowCount = 1 Then
                Try
                    If IsDBNull(dgvPrescriptions.SelectedRows(0).Cells(1).Value) = True Or IsDBNull(dgvPrescriptions.SelectedRows(0).Cells(2).Value) = True Then
                        ActivateDatagridPrescriptions(True)
                    Else
                        ActivateDatagridPrescriptions(False)
                    End If
                Catch ex As Exception
                    ActivateDatagridPrescriptions(False)
                End Try
            ElseIf dgvPrescriptions.RowCount > 1 Then
                ActivateDatagridPrescriptions(True)
            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Sub ActivateDatagridHairdies(ByVal mode As Boolean)

        lblCustWithNoHairdies.Visible = Not mode
        dgvHairdiesList.Visible = mode
        btnDeleteHairdies.Visible = mode
        lblLastUpdateHairDies.Visible = mode

    End Sub

    Private Sub ActivateDatagridDrugsOnLoan(ByVal mode As Boolean)

        lblCustWithoutDrugsOnLoan.Visible = Not mode
        dgvDrugsOnLoan.Visible = mode
        btnDeleteDrugOnLoan.Visible = mode
        lblSumDrugsOnLoan.Visible = mode
        lblSumDrugsOnLoanLabel.Visible = mode
        lblLastUpdateDrugsOnLoan.Visible = mode
        btnAddDrug.Visible = mode

    End Sub


    ' --- MINI WRAPPER για άμεσο save της επιλεγμένης γραμμής Δανεικών Φαρμάκων ---
    Public Sub ForceSaveSelectedDrugOnLoan()
        If dgvDrugsOnLoan.SelectedRows.Count = 0 Then Exit Sub
        Dim r As Integer = dgvDrugsOnLoan.SelectedRows(0).Index
        ' Κλείνουμε τυχόν edit, για να «γράψουν» οι τιμές
        dgvDrugsOnLoan.EndEdit()
        ' Καλούμε την υπάρχουσα ρουτίνα αποθήκευσης
        UpdateDrugsOnLoan(r, 4)
        DisplaySums_DrugsOnLoan()
        DisplayTotalDebtPerCustomer()
        DisplayLastUpdate()
        ActivateDatagridDrugsOnLoan(True)

    End Sub


    Private Sub ActivateDatagridPrescriptions(ByVal mode As Boolean)

        lblCustWithPrescriptions.Visible = Not mode
        dgvPrescriptions.Visible = mode
        btnDeletePrescriptions.Visible = mode
        lblLastUpdatePrescriptions.Visible = mode
        chkSelectAll.Visible = mode
        lblTotPrescriptions.Visible = mode

    End Sub


    Private Sub ActivateDatagridDebts(ByVal mode As Boolean)

        lblCustWithNoDebts.Visible = Not mode
        dgvDebtsList.Visible = mode
        btnPayDebts.Visible = mode
        btnAddDebt.Visible = mode
        btnDeleteDebts.Visible = mode
        btnPrintDebtsList.Visible = mode
        lblTotalDebtLabel.Visible = mode
        lblTotalCustomerDebt.Visible = mode
        lblLastUpdateDebts.Visible = mode

    End Sub


    Private Sub lblCustWithNoDebts_Click(sender As Object, e As EventArgs) Handles lblCustWithNoDebts.Click

        ActivateDatagridDebts(True)

    End Sub

    Private Sub lblCustWithNoHairdies_Click(sender As Object, e As EventArgs) Handles lblCustWithNoHairdies.Click

        ActivateDatagridHairdies(True)

    End Sub

    'Private Sub dgvCustomers_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvCustomers.CellValidating

    '    ' Κρατάει στη μνήμη τη παλιά και τη νέα τιμή του κελιού
    '    Dim oldValue = dgvCustomers(e.ColumnIndex, e.RowIndex).Value.ToString
    '    Dim newValue = e.FormattedValue

    '    ' Ελέγχει αν βρισκόμαστε στο πεδίο Όνομα
    '    Dim headerText As String = dgvCustomers.Columns(e.ColumnIndex).HeaderText
    '    If headerText.Equals("Όνοματεπώνυμο") Then
    '        Dim mySQL As String = "Select Name, Id From PharmacyCustomFiles.dbo.Customers WHERE Name='" & newValue & "'"

    '        'Αν η παλιά τιμή είναι διαφορετική από τη νέα ΚΑΙ η νέα τιμή υπάρχει ήδη στο database
    '        If oldValue <> newValue AndAlso IsItAllreadyThere(mySQL) = True Then
    '            'Μύνημα λάθους
    '            MessageBox.Show("Το όνομα " & newValue & " υπάρχει ήδη !", "Επιβεβαίωση στοιχείων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '            'Ξαμαγράφει τη παλιά τιμή
    '            dgvCustomers(e.ColumnIndex, e.RowIndex).Value = oldValue
    '            ' Ανανεώνει το Datagrid για να φανεί πάλι η παλιά τιμή
    '            dgvCustomers.RefreshEdit()
    '            'Εμποδίζει την έξοδο από το κελί
    '            e.Cancel = True
    '        End If

    '    End If

    'End Sub




    Private Sub btnDeleteCustomer_Click(sender As Object, e As EventArgs) Handles btnDeleteCustomer.Click
        'Ελέγχει αν ο πελάτης έχει χρέη ή βαφές
        Select Case CheckIfCustomerHasRecords()

            Case True
                MessageBox.Show("Ο επιλεγμένος πελάτης έχει καταχωρημένες εγγραφές." &
                                vbCrLf & "Η διαγραφή του δεν επιτρέπεται.", "Αδυναμία διαγραφής",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Case False

                DeleteCustomers()
                GetCustomersList()
                _forceLastAfterBind = True
                GetDebtsAndHairDiesList()
                DisplayLabelIfCustomerWithoutDebtsOrHairdies()

        End Select
    End Sub

    Private Function CheckIfCustomerHasRecords() As Boolean

        ' Ορισμός μεταβλητών
        Dim sqlstring(2) As String
        Dim myReader As SqlDataReader

        ' Ορίζει την sqlString για τα Χρέη
        sqlstring(0) = "SELECT distinct PharmacyCustomFiles.dbo.Customers.Name, PharmacyCustomFiles.dbo.Customers.id " &
                    "FROM PharmacyCustomFiles.dbo.Customers INNER JOIN PharmacyCustomFiles.dbo.Debts ON PharmacyCustomFiles.dbo.Customers.Id = PharmacyCustomFiles.dbo.Debts.CustomerId " &
                    "WHERE PharmacyCustomFiles.dbo.Customers.Id = '" & dgvCustomers.Item("Id", dgvCustomers.CurrentRow.Index).Value &
                    "' AND PharmacyCustomFiles.dbo.Debts.Ammount > 0"

        ' και εκείνη για τις βαφές..
        sqlstring(1) = "SELECT  distinct PharmacyCustomFiles.dbo.Customers.Name, PharmacyCustomFiles.dbo.Customers.id " &
                    "FROM PharmacyCustomFiles.dbo.Customers INNER JOIN PharmacyCustomFiles.dbo.HairDies ON PharmacyCustomFiles.dbo.Customers.Id = PharmacyCustomFiles.dbo.HairDies.CustomerId " &
                    "WHERE PharmacyCustomFiles.dbo.Customers.Id = '" & dgvCustomers.Item("Id", dgvCustomers.CurrentRow.Index).Value &
                    "' AND PharmacyCustomFiles.dbo.HairDies.HairDieDescription Is Not null"

        ' και εκείνη για τα φάρμακα..
        sqlstring(2) = "SELECT  distinct PharmacyCustomFiles.dbo.Customers.Name, PharmacyCustomFiles.dbo.Customers.id " &
                    "FROM PharmacyCustomFiles.dbo.Customers INNER JOIN PharmacyCustomFiles.dbo.DrugsOnLoan ON PharmacyCustomFiles.dbo.Customers.Id = PharmacyCustomFiles.dbo.DrugsOnLoan.CustomerId " &
                    "WHERE PharmacyCustomFiles.dbo.Customers.Id = '" & dgvCustomers.Item("Id", dgvCustomers.CurrentRow.Index).Value &
                    "' AND PharmacyCustomFiles.dbo.DrugsOnLoan.Name Is Not null"

        ' Ψάχνει πρώτα τα Χρέη (0) και μετά τις Βαφές(1)
        For t = 0 To 2
            'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
            Using con As New SqlConnection(connectionstring)

                'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
                Using cmd As New SqlCommand(sqlstring(t), con)

                    ' Ανοίγει την σύνδεση
                    con.Open()

                    'Ορισμός ExecuteReader 
                    myReader = cmd.ExecuteReader()

                    ' Αν υπάρχουν records
                    If myReader.HasRows Then
                        Return True  ' επιστρέφει την τιμή True (και βγαίνει από το function)
                    End If

                End Using
            End Using
        Next t

        Return False  ' Αν ΔΕΝ υπάρχουν records επιστρέφει την τιμή False

    End Function



    Private Function CheckIfProductHasExpirations() As Integer

        ' Ορισμός μεταβλητών
        Dim sqlstring As String
        Dim myReader As SqlDataReader
        Dim paradrugId As Integer = dgvPricesParadrugs.SelectedRows(0).Cells(5).Value

        ' Ορίζει την sqlString
        sqlstring = "SELECT count(*) " &
                    "FROM [PharmacyCustomFiles].[dbo].[Expirations] " &
                    "WHERE ParadrugId = '" & paradrugId & "'"

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlCommand(sqlstring, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                myReader = cmd.ExecuteReader()

                ' Αν υπάρχουν records
                'If myReader.HasRows Then
                myReader.Read()

                Return myReader(0)  ' επιστρέφει την τιμή t (και βγαίνει από το function)
                'End If

            End Using
        End Using

        'Return False  ' Αν ΔΕΝ υπάρχουν records επιστρέφει την τιμή False

    End Function



    Private Function CheckIfExchangerHasRecords() As Integer

        ' Ορισμός μεταβλητών
        Dim sqlstring As String
        Dim myReader As SqlDataReader
        Dim ExchangerName As String = cbExchangers.Text

        ' Ορίζει την sqlString
        sqlstring = "SELECT count(*) " &
                    "FROM [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "WHERE [Exch] = '" & ExchangerName & "'"

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlCommand(sqlstring, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                myReader = cmd.ExecuteReader()

                ' Αν υπάρχουν records
                'If myReader.HasRows Then
                myReader.Read()

                Return myReader(0)  ' επιστρέφει την τιμή t (και βγαίνει από το function)
                'End If

            End Using
        End Using

        'Return False  ' Αν ΔΕΝ υπάρχουν records επιστρέφει την τιμή False

    End Function


    Private Function CheckIfParadrugExists(ByVal name As String) As Boolean
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT * " &
                        "FROM [PharmacyCustomFiles].[dbo].[Expirations]" &
                        "WHERE ProductName = @Name "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Name", dgvPricesParadrugs.SelectedRows(0).Cells(0).Value)

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then
                Return True
            End If

            Return False

        End Using

    End Function



    Private Function CheckIfExpirationExists() As Boolean
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT * " &
                        "FROM [PharmacyCustomFiles].[dbo].[Expirations]" &
                        "WHERE ParadrugId = @Id AND Month = @month AND Year = @year "

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Id", dgvPricesParadrugs.SelectedRows(0).Cells(5).Value)
            cmd.Parameters.AddWithValue("@month", dgvExpirations.SelectedRows(0).Cells(0).EditedFormattedValue)
            cmd.Parameters.AddWithValue("@year", dgvExpirations.SelectedRows(0).Cells(1).EditedFormattedValue)

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then
                Return True
            End If

            Return False

        End Using

    End Function


    Private Function CheckIfExchangerExists() As Boolean
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "SELECT * " &
                        "FROM [PharmacyCustomFiles].[dbo].[ExchangerList] " &
                        "WHERE ExchangerName = @Name"

            Dim cmd As New SqlCommand(insertData, con)

            cmd.Parameters.AddWithValue("@Name", cbExchangers.Text)

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then
                Return True
            End If

            Return False

        End Using

    End Function




    Private Sub dgvDebtsList_DefaultValuesNeeded(sender As Object, e As DataGridViewRowEventArgs) Handles dgvDebtsList.DefaultValuesNeeded

        'Ορίζει default τιμές 
        With e.Row
            .Cells(0).Value = Today() ' Date
            .Cells(1).Value = 0 ' Ποσό

        End With

    End Sub


    Private Sub dgvHairdiesList_DefaultValuesNeeded(sender As Object, e As DataGridViewRowEventArgs) Handles dgvHairdiesList.DefaultValuesNeeded
        'Ορίζει default τιμές 
        With e.Row
            .Cells(0).Value = Today() ' Date

        End With
    End Sub

    Private Sub btnDeleteDebts_Click(sender As Object, e As EventArgs) Handles btnDeleteDebts.Click
        DeleteSelectedDebts()
        _forceLastAfterBind = True
        GetDebtsAndHairDiesList()
        DisplayLabelIfCustomerWithoutDebtsOrHairdies()
        ' Ανανεώνει τη λίστα των πελατων, ξαναεπιλέγοντας τον τελευταίο πελάτη
        Dim SelectedCustomer As String = dgvCustomers.SelectedRows(0).Cells(0).Value
        GetCustomersList()
        Dim rowIndex As Integer = SearchDatagrid(dgvCustomers, SelectedCustomer)
        dgvCustomers.CurrentCell = dgvCustomers.Rows(rowIndex).Cells(0)
        DisplayTotalDebtPerCustomer()
    End Sub

    Private Sub btnDeleteHairdies_Click(sender As Object, e As EventArgs) Handles btnDeleteHairdies.Click
        DeleteSelectedHairdies()
        _forceLastAfterBind = True
        GetDebtsAndHairDiesList()
        DisplayLabelIfCustomerWithoutDebtsOrHairdies()
    End Sub

    Private Sub btnPayDebts_Click(sender As Object, e As EventArgs) Handles btnPayDebts.Click
        ' 1) Πάρε το οφειλόμενο ποσό (αν δεν είναι αριθμός ή ≤0, βγες)
        Dim totalDue As Decimal
        If Not Decimal.TryParse(lblTotalCustomerDebt.Text, Globalization.NumberStyles.Any,
                                Globalization.CultureInfo.CurrentCulture, totalDue) _
           OrElse totalDue <= 0D Then
            MessageBox.Show("Δεν υπάρχει οφειλή για αποπληρωμή.", "Πληρωμή", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' 2) Άνοιξε το modal για να ζητήσεις ποσό (default = όλο το ποσό)
        Using f As New frmPayDebt(totalDue)
            If f.ShowDialog(Me) = DialogResult.OK Then
                Dim pay As Decimal = f.PaidAmount ' Ελεγμένο: 0 < pay ≤ totalDue
                ' 3) Πέρασε την πληρωμή (αρνητική εγγραφή στο debts)
                InsertDebtPayment(pay)

                ' 4) Ελαφρύ refresh UI
                _forceLastAfterBind = True
                GetDebtsAndHairDiesList()
                DisplayLabelIfCustomerWithoutDebtsOrHairdies()
                DisplayTotalDebtPerCustomer()
            End If
        End Using
    End Sub

    Private Sub InsertDebtPayment(amount As Decimal)
        Dim insertData As String =
        "INSERT INTO PharmacyCustomFiles.dbo.debts " &
        "([CustomerId], [Date], [DebtDescription], [Ammount]) " &
        "VALUES (@CustomerId, @Date, @DebtDescription, @Ammount)"

        Using con As New SqlClient.SqlConnection(connectionstring)
            con.Open()
            Using cmd As New SqlClient.SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@CustomerId", If(dgvCustomers.SelectedRows(0).Cells(1).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@Date", Today())
                cmd.Parameters.AddWithValue("@DebtDescription", "--- ΑΠΟΠΛΗΡΩΜΗ ---")
                cmd.Parameters.AddWithValue("@Ammount", -amount) ' αρνητικό: πληρωμή
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub



    Private Sub txtSearchTameia_TextChanged(sender As Object, e As EventArgs) Handles txtSearchTameia.TextChanged

        UpdateDataTameia()

    End Sub


    Private Sub UpdateDataTameia()
        GetTameiaAskedList()
        GetTameiaGivenList()

        'Πάει στο τελευταίο record του datagrid
        Try
            Me.dgvTameiaAsked.FirstDisplayedScrollingRowIndex = Me.dgvTameiaAsked.RowCount - 1
        Catch ex As Exception
        End Try
    End Sub


    Private Function DisplayCustomDatagrid_TameiaAsked() As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(stringDTG, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)
        dsDTG = New DataSet

        ' και ο SqlDataAdapter γεμίζει το Dataset
        daDTG.Fill(dsDTG, "DTG")
        ' το οποίο γεμίζει το datatable
        dtDTG = dsDTG.Tables("DTG")

        'Καθορίζει το source του BindingSource ως το Datatable
        bsTameiaAsked.DataSource = dtDTG

        'Κλείνει την σύνδεση
        con.Close()


        With dgvTameiaAsked

            'Αδειάζει το datagridView
            .Columns.Clear()

            'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
            .AutoGenerateColumns = False

            'Καθορίζει το  source του DataGrid ως το BindingSource
            .DataSource = bsTameiaAsked

        End With

        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim MyDate As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        MyDate.DataPropertyName = "MyDate"
        ' Formatting..
        MyDate.HeaderText = "Ημερ/νία"
        MyDate.Width = 70
        MyDate.DefaultCellStyle.Format = ""

        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
        Dim Description As New DataGridViewTextBoxColumn
        With Description
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Description"
            ' Formatting..
            .HeaderText = "Αιτιολογία"
            .Width = 120
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 3ο πεδίο του Datagrid σαν textbox
        Dim AmountAsked As New DataGridViewTextBoxColumn
        With AmountAsked
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AmountAsked2"
            ' Formatting..
            .HeaderText = "Αιτούμενο ποσό"
            .Width = 70
            .DefaultCellStyle.Format = "F2"
        End With

        'Όρίζει το 4ο πεδίο του Datagrid σαν textbox
        Dim AmountGiven As New DataGridViewTextBoxColumn
        With AmountGiven
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AmountGiven2"
            ' Formatting..
            .HeaderText = "Καταβληθέν ποσό"
            .Width = 70
            .DefaultCellStyle.Format = "F2"
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Difference As New DataGridViewTextBoxColumn
        With Difference
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Difference2"
            ' Formatting..
            .HeaderText = "Κρατήσεις (ποσό)"
            .Width = 70
            .DefaultCellStyle.Format = "F2"
        End With


        'Όρίζει το 6ο πεδίο του Datagrid σαν textbox
        Dim DifferPercent As New DataGridViewTextBoxColumn
        With DifferPercent
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "DifferPercent2"
            ' Formatting..
            .HeaderText = "Κρατήσεις (%)"
            .Width = 70
            .DefaultCellStyle.Format = "##0.0"

        End With


        'Όρίζει το 7ο πεδίο του Datagrid σαν textbox
        Dim PercentagePaid As New DataGridViewTextBoxColumn
        With PercentagePaid
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "PercentagePaid2"
            ' Formatting..
            .HeaderText = "Εξόφληση (%)"
            .Width = 70
            .DefaultCellStyle.Format = "##0.0"
        End With


        'Όρίζει το 8ο πεδίο του Datagrid σαν textbox
        Dim IRS As New DataGridViewCheckBoxColumn
        With IRS
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "IRS2"
            ' Formatting..
            .HeaderText = "ΟΚ"
            .Width = 30
            .DefaultCellStyle.Format = ""
        End With


        'Όρίζει το 9ο πεδίο του Datagrid σαν textbox
        Dim Id As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        With Id
            .DataPropertyName = "Id"
            .HeaderText = "Id"
            .Width = 25
            .DefaultCellStyle.Format = ""
        End With




        ''Όρίζει το 1ο πεδίο του Datagrid σαν combobox
        'Dim Combo As New DataGridViewComboBoxColumn
        '' και του δίνει τη τιμή του αντίστοιχου πεδίου
        'Combo.DataPropertyName = "AP_DESCRIPTION"
        ''Παίρνει όλες τις πιθανές τιμές του Category και τις προσθέτει σαν επιλογές στο combobox
        'For t = 0 To DrugList.Length - 1
        '    If Not (DrugList(t) Is Nothing) Then Combo.Items.Add(DrugList(t))
        'Next


        With dgvTameiaAsked

            'Εμφανίζει τα columns του Datagrid
            .Columns.Add(MyDate)
            .Columns.Add(Description)
            .Columns.Add(AmountAsked)
            .Columns.Add(AmountGiven)
            .Columns.Add(Difference)
            .Columns.Add(DifferPercent)
            .Columns.Add(PercentagePaid)
            .Columns.Add(IRS)
            .Columns.Add(Id)

            ' Εναλλαγή του χρωματισμού των rows
            .RowsDefaultCellStyle.BackColor = Color.Bisque
            .AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

            'Εξαφανίζει τo πεδίο "Id"
            '.Columns(8).Visible = False
        End With


        '' Μορφοποιεί τα columns

        'For t = 0 To columnName.Length - 1

        '    oDataGridView.Columns(t).HeaderText = columnName(t) 'Βάζει τίτλο σε κάθε Column

        '    oDataGridView.Columns(t).Width = columnWidth(t) ' Αλλάζει το φάρδος του κάθε Column

        '    oDataGridView.Columns(t).DefaultCellStyle.Format = columnFormat(t)  ' Formatαρισμα των στοιχείων
        'Next

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dsDTG.Tables(0).Rows.Count

    End Function




    Private Function DisplayCustomDatagrid_Debts(ByVal oBinding As BindingSource, ByVal oDatagrid As DataGridView) As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(stringDTG, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)
        dsDTG = New DataSet

        ' και ο SqlDataAdapter γεμίζει το Dataset
        daDTG.Fill(dsDTG, "DTG")
        ' το οποίο γεμίζει το datatable
        dtDTG = dsDTG.Tables("DTG")

        'Καθορίζει το source του BindingSource ως το Datatable
        oBinding.DataSource = dtDTG

        'Κλείνει την σύνδεση
        con.Close()


        With oDatagrid

            'Αδειάζει το datagridView
            .Columns.Clear()

            'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
            .AutoGenerateColumns = False

            'Καθορίζει το  source του DataGrid ως το BindingSource
            .DataSource = oBinding

        End With


        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim DebtDate As New DataGridViewTextBoxColumn
        With DebtDate
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Date"
            ' Formatting..
            .HeaderText = "Ημερομηνία"
            .Width = 70
            .DefaultCellStyle.Format = "dd-MM-yyyy"
        End With

        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
        Dim Ammount As New DataGridViewTextBoxColumn
        With Ammount
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Ammount"
            ' Formatting..
            .HeaderText = "Ποσό"
            .Width = 50
            .DefaultCellStyle.Format = "F2"
        End With

        'Όρίζει το 3ο πεδίο του Datagrid σαν textbox
        Dim DebtDescription As New DataGridViewTextBoxColumn
        With DebtDescription
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "DebtDescription"
            ' Formatting..
            .HeaderText = "Περιγραφή"
            .Width = 230
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 4ο πεδίο του Datagrid σαν textbox
        Dim Id As New DataGridViewTextBoxColumn
        With Id
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Id"
            ' Formatting..
            .HeaderText = "Id"
            .Width = 40
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim CustomerId As New DataGridViewTextBoxColumn
        With CustomerId
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "CustomerId"
            ' Formatting..
            .HeaderText = "CustomerId"
            .Width = 40
            .DefaultCellStyle.Format = ""
        End With

        With oDatagrid

            'Εμφανίζει τα columns του Datagrid
            .Columns.Add(DebtDate)
            .Columns.Add(Ammount)
            .Columns.Add(DebtDescription)
            .Columns.Add(Id)
            .Columns.Add(CustomerId)

            ' Εναλλαγή του χρωματισμού των rows
            .RowsDefaultCellStyle.BackColor = Color.Bisque
            .AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

            'Εξαφανίζει τα πεδία
            .Columns(3).Visible = False
            .Columns(4).Visible = False
        End With

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dsDTG.Tables(0).Rows.Count

    End Function



    Private Function DisplayCustomDatagrid_DrugsOnLoan(ByVal oBinding As BindingSource, ByVal oDatagrid As DataGridView) As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(stringDTG, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)
        dsDTG = New DataSet

        ' και ο SqlDataAdapter γεμίζει το Dataset
        daDTG.Fill(dsDTG, "DTG")
        ' το οποίο γεμίζει το datatable
        dtDTG = dsDTG.Tables("DTG")

        'Καθορίζει το source του BindingSource ως το Datatable
        oBinding.DataSource = dtDTG

        'Κλείνει την σύνδεση
        con.Close()


        With oDatagrid

            'Αδειάζει το datagridView
            .Columns.Clear()

            'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
            .AutoGenerateColumns = False

            'Καθορίζει το  source του DataGrid ως το BindingSource
            .DataSource = oBinding

        End With


        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim DateIn As New DataGridViewTextBoxColumn
        With DateIn
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "DateIn"
            ' Formatting..
            .HeaderText = "Δώθηκε"
            .Width = 70
            .DefaultCellStyle.Format = "dd-MM-yyyy"
        End With

        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
        Dim Price As New DataGridViewTextBoxColumn
        With Price
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Price"
            ' Formatting..
            .HeaderText = "Ποσό"
            .Width = 80
            .DefaultCellStyle.Format = "F2"
        End With

        'Όρίζει το 3ο πεδίο του Datagrid σαν textbox
        Dim Name As New DataGridViewTextBoxColumn
        With Name
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Name"
            ' Formatting..
            .HeaderText = "Περιγραφή"
            .Width = 200
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 4ο πεδίο του Datagrid σαν textbox
        Dim Barcode1 As New DataGridViewTextBoxColumn
        With Barcode1
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Barcode1"
            ' Formatting..
            .HeaderText = "Barcode1"
            .Width = 80
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 4ο πεδίο του Datagrid σαν textbox
        Dim Barcode2 As New DataGridViewTextBoxColumn
        With Barcode2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Barcode2"
            ' Formatting..
            .HeaderText = "Barcode2"
            .Width = 80
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 4ο πεδίο του Datagrid σαν textbox
        Dim DateOut As New DataGridViewTextBoxColumn
        With DateOut
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Επιστροφή"
            ' Formatting..
            .HeaderText = "Ημερομηνία"
            .Width = 70
            .DefaultCellStyle.Format = "dd-MM-yyyy"
        End With

        'Όρίζει το 4ο πεδίο του Datagrid σαν textbox
        Dim Id As New DataGridViewTextBoxColumn
        With Id
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Id"
            ' Formatting..
            .HeaderText = "Id"
            .Width = 40
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim CustomerId As New DataGridViewTextBoxColumn
        With CustomerId
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "CustomerId"
            ' Formatting..
            .HeaderText = "CustomerId"
            .Width = 40
            .DefaultCellStyle.Format = ""
        End With

        With oDatagrid

            'Εμφανίζει τα columns του Datagrid
            .Columns.Add(DateIn)
            .Columns.Add(Name)
            .Columns.Add(Price)
            .Columns.Add(Barcode1)
            .Columns.Add(Barcode2)
            .Columns.Add(DateOut)
            .Columns.Add(Id)
            .Columns.Add(CustomerId)

            ' Εναλλαγή του χρωματισμού των rows
            .RowsDefaultCellStyle.BackColor = Color.Bisque
            .AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

            'Εξαφανίζει τα πεδία
            '.Columns(3).Visible = False
            '.Columns(4).Visible = False
        End With

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dsDTG.Tables(0).Rows.Count

    End Function


    Private Function DisplayCustomDatagrid_Prescriptions(ByVal oBinding As BindingSource, ByVal oDatagrid As DataGridView) As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(stringDTG, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)
        dsDTG = New DataSet

        ' και ο SqlDataAdapter γεμίζει το Dataset
        daDTG.Fill(dsDTG, "DTG")
        ' το οποίο γεμίζει το datatable
        dtDTG = dsDTG.Tables("DTG")

        'Καθορίζει το source του BindingSource ως το Datatable
        oBinding.DataSource = dtDTG

        'Κλείνει την σύνδεση
        con.Close()


        With oDatagrid

            'Αδειάζει το datagridView
            .Columns.Clear()

            'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
            .AutoGenerateColumns = False

            'Καθορίζει το  source του DataGrid ως το BindingSource
            .DataSource = oBinding

        End With

        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim Ektelesis As New DataGridViewTextBoxColumn
        With Ektelesis
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Ektelesis"
            ' Formatting..
            .HeaderText = "Έκτέλεση"
            .Width = 66
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim InitDate As New DataGridViewTextBoxColumn
        With InitDate
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "InitDate"
            ' Formatting..
            .HeaderText = "Έναρξη"
            .Width = 69
            .DefaultCellStyle.Format = "dd-MM-yyyy"
        End With

        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
        Dim EndDate As New DataGridViewTextBoxColumn
        With EndDate
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "EndDate"
            ' Formatting..
            .HeaderText = "Λήξη"
            .Width = 69
            .DefaultCellStyle.Format = "dd-MM-yyyy"
        End With

        ''Όρίζει το 3ο πεδίο του Datagrid σαν textbox
        Dim Barcode As New DataGridViewTextBoxColumn
        With Barcode
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Barcode"
            ' Formatting..
            .HeaderText = "Barcode"
            .Width = 77
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 4ο πεδίο του Datagrid σαν textbox
        Dim ProcessedDate As New DataGridViewTextBoxColumn
        With ProcessedDate
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "ProcessedDate"
            ' Formatting..
            .HeaderText = "Εκτελέστηκε"
            .Width = 70
            .DefaultCellStyle.Format = "dd-MM-yyyy"
        End With


        'Όρίζει το 4ο πεδίο του Datagrid σαν textbox
        Dim Id As New DataGridViewTextBoxColumn
        With Id
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Id"
            ' Formatting..
            .HeaderText = "Id"
            .Width = 40
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim CustomerId As New DataGridViewTextBoxColumn
        With CustomerId
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "CustomerId"
            ' Formatting..
            .HeaderText = "CustomerId"
            .Width = 40
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug1 As New DataGridViewTextBoxColumn
        With Drug1
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug1"
            ' Formatting..
            .HeaderText = "Δραστική 1"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug2 As New DataGridViewTextBoxColumn
        With Drug2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug2"
            ' Formatting..
            .HeaderText = "Δραστική 2"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug3 As New DataGridViewTextBoxColumn
        With Drug3
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug3"
            ' Formatting..
            .HeaderText = "Δραστική 3"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug4 As New DataGridViewTextBoxColumn
        With Drug4
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug4"
            ' Formatting..
            .HeaderText = "Δραστική 4"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug5 As New DataGridViewTextBoxColumn
        With Drug5
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug5"
            ' Formatting..
            .HeaderText = "Δραστική 5"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug6 As New DataGridViewTextBoxColumn
        With Drug6
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug6"
            ' Formatting..
            .HeaderText = "Δραστική 6"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug7 As New DataGridViewTextBoxColumn
        With Drug7
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug7"
            ' Formatting..
            .HeaderText = "Δραστική 7"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug8 As New DataGridViewTextBoxColumn
        With Drug8
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug8"
            ' Formatting..
            .HeaderText = "Δραστική 8"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug9 As New DataGridViewTextBoxColumn
        With Drug9
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug9"
            ' Formatting..
            .HeaderText = "Δραστική 9"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug10 As New DataGridViewTextBoxColumn
        With Drug10
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug10"
            ' Formatting..
            .HeaderText = "Δραστική 10"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug11 As New DataGridViewTextBoxColumn
        With Drug11
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug11"
            ' Formatting..
            .HeaderText = "Δραστική 11"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug12 As New DataGridViewTextBoxColumn
        With Drug12
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug12"
            ' Formatting..
            .HeaderText = "Δραστική 12"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug13 As New DataGridViewTextBoxColumn
        With Drug13
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug13"
            ' Formatting..
            .HeaderText = "Δραστική 13"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug14 As New DataGridViewTextBoxColumn
        With Drug14
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug14"
            ' Formatting..
            .HeaderText = "Δραστική 14"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug15 As New DataGridViewTextBoxColumn
        With Drug15
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug15"
            ' Formatting..
            .HeaderText = "Δραστική 15"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug16 As New DataGridViewTextBoxColumn
        With Drug16
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug16"
            ' Formatting..
            .HeaderText = "Δραστική 16"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug17 As New DataGridViewTextBoxColumn
        With Drug17
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug17"
            ' Formatting..
            .HeaderText = "Δραστική 17"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug18 As New DataGridViewTextBoxColumn
        With Drug18
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug18"
            ' Formatting..
            .HeaderText = "Δραστική 18"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug19 As New DataGridViewTextBoxColumn
        With Drug19
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug19"
            ' Formatting..
            .HeaderText = "Δραστική 19"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Drug20 As New DataGridViewTextBoxColumn
        With Drug20
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Drug20"
            ' Formatting..
            .HeaderText = "Δραστική 20"
            .Width = 100
            .DefaultCellStyle.Format = ""
        End With


        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Analosima As New DataGridViewCheckBoxColumn
        With Analosima
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Analosima"
            ' Formatting..
            .HeaderText = "Αναλώσιμο"
            .Width = 30
            .DefaultCellStyle.Format = ""
        End With


        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim Notes As New DataGridViewTextBoxColumn
        With Notes
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Notes"
            ' Formatting..
            .HeaderText = "Παρατηρήσεις"
            .Width = 230
            .DefaultCellStyle.Format = ""
        End With

        With oDatagrid

            'Εμφανίζει τα columns του Datagrid
            .Columns.Add(Ektelesis)
            .Columns.Add(InitDate)
            .Columns.Add(EndDate)
            .Columns.Add(Barcode)
            .Columns.Add(ProcessedDate)
            .Columns.Add(CustomerId)
            .Columns.Add(Id)
            .Columns.Add(Drug1)
            .Columns.Add(Drug2)
            .Columns.Add(Drug3)
            .Columns.Add(Drug4)
            .Columns.Add(Drug5)
            .Columns.Add(Drug6)
            .Columns.Add(Drug7)
            .Columns.Add(Drug8)
            .Columns.Add(Drug9)
            .Columns.Add(Drug10)
            .Columns.Add(Drug11)
            .Columns.Add(Drug12)
            .Columns.Add(Drug13)
            .Columns.Add(Drug14)
            .Columns.Add(Drug15)
            .Columns.Add(Drug16)
            .Columns.Add(Drug17)
            .Columns.Add(Drug18)
            .Columns.Add(Drug19)
            .Columns.Add(Drug20)
            .Columns.Add(Analosima)
            .Columns.Add(Notes)

            ' Εναλλαγή του χρωματισμού των rows
            .RowsDefaultCellStyle.BackColor = Color.Bisque
            .AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

            'Εξαφανίζει τα πεδία
            '.Columns(5).Visible = False
            '.Columns(6).Visible = False
        End With

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dsDTG.Tables(0).Rows.Count

    End Function


    Private Function DisplayCustomDatagrid_Exchanges(ByVal oBinding As BindingSource, ByVal oDatagrid As DataGridView) As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(stringDTG, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)
        dsDTG = New DataSet

        ' και ο SqlDataAdapter γεμίζει το Dataset
        daDTG.Fill(dsDTG, "DTG")
        ' το οποίο γεμίζει το datatable
        dtDTG = dsDTG.Tables("DTG")

        'Καθορίζει το source του BindingSource ως το Datatable
        oBinding.DataSource = dtDTG

        'Κλείνει την σύνδεση
        con.Close()


        With oDatagrid

            'Αδειάζει το datagridView
            .Columns.Clear()

            'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
            .AutoGenerateColumns = False

            'Καθορίζει το  source του DataGrid ως το BindingSource
            .DataSource = oBinding

        End With


        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim Id As New DataGridViewTextBoxColumn
        With Id
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Id"
            ' Formatting..
            .HeaderText = "Id"
            .Width = 30
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
        Dim DrugName As New DataGridViewTextBoxColumn
        With DrugName
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "DrugName"
            ' Formatting..
            .HeaderText = "Προιόν"
            .Width = 260
            .DefaultCellStyle.Format = ""
        End With

        Dim Qnt As New DataGridViewTextBoxColumn
        With Qnt
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Qnt"
            ' Formatting..
            .HeaderText = "Ποσ"
            .Width = 30
            .DefaultCellStyle.Format = ""
        End With

        Dim Xondr As New DataGridViewTextBoxColumn
        With Xondr
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Xondr"
            ' Formatting..
            .HeaderText = "Χονδρική"
            .Width = 55
            .DefaultCellStyle.Format = "F2"
        End With

        Dim RP As New DataGridViewTextBoxColumn
        With RP
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "RP"
            ' Formatting..
            .HeaderText = "RP"
            .Width = 35
            .DefaultCellStyle.Format = ""
        End With

        Dim AP_Code As New DataGridViewTextBoxColumn
        With AP_Code
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AP_Code"
            ' Formatting..
            .HeaderText = "Κωδικός"
            .Width = 80
            .DefaultCellStyle.Format = ""
        End With

        Dim MyDate As New DataGridViewTextBoxColumn
        With MyDate
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "MyDate"
            ' Formatting..
            .HeaderText = "MyDate"
            .Width = 80
            .DefaultCellStyle.Format = ""
        End With

        Dim FPA As New DataGridViewTextBoxColumn
        With FPA
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "FPA"
            ' Formatting..
            .HeaderText = "ΦΠΑ"
            .Width = 35
            .DefaultCellStyle.Format = ""
        End With


        With oDatagrid

            'Εμφανίζει τα columns του Datagrid
            .Columns.Add(Id)
            .Columns.Add(DrugName)
            .Columns.Add(FPA)
            .Columns.Add(Qnt)
            .Columns.Add(Xondr)
            .Columns.Add(RP)
            .Columns.Add(AP_Code)
            .Columns.Add(MyDate)

            ' Εναλλαγή του χρωματισμού των rows
            .RowsDefaultCellStyle.BackColor = Color.Bisque
            .AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

            'Εξαφανίζει τo πεδίο "Id"
            .Columns(0).Visible = False
            .Columns(6).Visible = False
            .Columns(7).Visible = False
        End With

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dsDTG.Tables(0).Rows.Count

    End Function




    Private Function DisplayCustomDatagrid_Paradrugs() As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(stringDTG, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)
        dsDTG = New DataSet

        ' και ο SqlDataAdapter γεμίζει το Dataset
        daDTG.Fill(dsDTG, "DTG-Prdg")
        ' το οποίο γεμίζει το datatable
        dtDTG = dsDTG.Tables("DTG-Prdg")

        'Καθορίζει το source του BindingSource ως το Datatable
        bsPricesParadrugs.DataSource = dtDTG

        'Κλείνει την σύνδεση
        con.Close()


        With dgvPricesParadrugs

            'Αδειάζει το datagridView
            .Columns.Clear()

            'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
            .AutoGenerateColumns = False

            'Καθορίζει το  source του DataGrid ως το BindingSource
            .DataSource = bsPricesParadrugs

        End With

        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim Name2 As New DataGridViewTextBoxColumn
        With Name2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Name2"
            ' Formatting..
            .HeaderText = "Όνομα"
            .Width = 260
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
        Dim Xondr2 As New DataGridViewTextBoxColumn
        With Xondr2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Xondr2"
            ' Formatting..
            .HeaderText = "Χονδρική"
            .Width = 60
            .DefaultCellStyle.Format = "F2"
        End With

        Dim Lian2 As New DataGridViewTextBoxColumn
        With Lian2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Lian2"
            ' Formatting..
            .HeaderText = "Λιανική"
            .Width = 60
            .DefaultCellStyle.Format = "F2"
        End With

        Dim Notes2 As New DataGridViewTextBoxColumn
        With Notes2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Notes2"
            ' Formatting..
            .HeaderText = "Σημειώσεις"
            .Width = 120
            .DefaultCellStyle.Format = ""
        End With

        Dim AP_Code2 As New DataGridViewTextBoxColumn
        With AP_Code2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AP_Code2"
            ' Formatting..
            .HeaderText = "Κωδικός"
            .Width = 70
            .DefaultCellStyle.Format = ""
        End With

        Dim AP_ID2 As New DataGridViewTextBoxColumn
        With AP_ID2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AP_ID2"
            ' Formatting..
            .HeaderText = "Κωδικός"
            .Width = 70
            .DefaultCellStyle.Format = ""
        End With

        Dim Barcode2 As New DataGridViewTextBoxColumn
        With Barcode2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Barcode2"
            ' Formatting..
            .HeaderText = "Barcode"
            .Width = 90
            .DefaultCellStyle.Format = ""
        End With

        Dim Id As New DataGridViewTextBoxColumn
        With Id
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Id"
            ' Formatting..
            .HeaderText = "Id"
            .Width = 50
            .DefaultCellStyle.Format = ""
        End With


        With dgvPricesParadrugs
            'Εμφανίζει τα columns του Datagrid
            .Columns.Add(Name2)
            .Columns.Add(Xondr2)
            .Columns.Add(Lian2)
            .Columns.Add(Notes2)
            .Columns.Add(AP_Code2)
            .Columns.Add(Id)
            .Columns.Add(AP_ID2)
            .Columns.Add(Barcode2)

            ' Εναλλαγή του χρωματισμού των rows
            .RowsDefaultCellStyle.BackColor = Color.Bisque
            .AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

            'Εξαφανίζει τo πεδίο "Id"
            .Columns(5).Visible = False
            .Columns(6).Visible = False
        End With

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dsDTG.Tables("DTG-Prdg").Rows.Count

    End Function



    Private Function DisplayCustomDatagrid_Drugs() As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(stringDTG, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)
        dsDTG = New DataSet

        ' και ο SqlDataAdapter γεμίζει το Dataset
        daDTG.Fill(dsDTG, "DTG")
        ' το οποίο γεμίζει το datatable
        dtDTG = dsDTG.Tables("DTG")

        'Καθορίζει το source του BindingSource ως το Datatable
        bsDrugs2.DataSource = dtDTG

        'Κλείνει την σύνδεση
        con.Close()


        With dgvPricesParadrugs

            'Αδειάζει το datagridView
            .Columns.Clear()

            'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
            .AutoGenerateColumns = False

            'Καθορίζει το  source του DataGrid ως το BindingSource
            .DataSource = bsDrugs2

        End With

        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim Name2 As New DataGridViewTextBoxColumn
        With Name2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AP_DESCRIPTION"
            ' Formatting..
            .HeaderText = "Όνομα"
            .Width = 240
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim Morfi2 As New DataGridViewTextBoxColumn
        With Morfi2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AP_MORFI"
            ' Formatting..
            .HeaderText = "Μορφή"
            .Width = 180
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
        Dim Xondr2 As New DataGridViewTextBoxColumn
        With Xondr2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AP_TIMH_XON"
            ' Formatting..
            .HeaderText = "Χονδρική"
            .Width = 60
            .DefaultCellStyle.Format = "F2"
        End With

        Dim Lian2 As New DataGridViewTextBoxColumn
        With Lian2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AP_TIMH_LIAN"
            ' Formatting..
            .HeaderText = "Λιανική"
            .Width = 60
            .DefaultCellStyle.Format = "F2"
        End With

        Dim AP_Code2 As New DataGridViewTextBoxColumn
        With AP_Code2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AP_CODE"
            ' Formatting..
            .HeaderText = "AP_Code"
            .Width = 70
            .DefaultCellStyle.Format = ""
        End With

        Dim AP_ID2 As New DataGridViewTextBoxColumn
        With AP_ID2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AP_ID"
            ' Formatting..
            .HeaderText = "AP_ID"
            .Width = 70
            .DefaultCellStyle.Format = ""
        End With

        'Dim Barcode2 As New DataGridViewTextBoxColumn
        'With Barcode2
        '    If barcodeType = "barcode" Then
        '        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        '        .DataPropertyName = "BRAP_AP_BARCODE"
        '        ' Formatting..
        '        .HeaderText = "Barcode"
        '        .Width = 90
        '        .DefaultCellStyle.Format = ""
        '    ElseIf barcodeType = "qrcode" Then
        '        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        '        .DataPropertyName = "APQ_PRODUCT_CODE"
        '        ' Formatting..
        '        .HeaderText = "QRcode"
        '        .Width = 90
        '        .DefaultCellStyle.Format = ""
        '    End If
        'End With

        Dim Barcode2 As New DataGridViewTextBoxColumn
        Dim Qrcode2 As New DataGridViewTextBoxColumn
        Dim QrcodeCustom2 As New DataGridViewTextBoxColumn
        If rbByName.Checked = True Then
            With Barcode2
                ' και του δίνει τη τιμή του αντίστοιχου πεδίου
                .Name = "colDrugBarcode"
                .DataPropertyName = "BRAP_AP_BARCODE"
                ' Formatting..
                .HeaderText = "Barcode"
                .Width = 90
                .DefaultCellStyle.Format = ""
                .ReadOnly = True
            End With

            With Qrcode2
                ' και του δίνει τη τιμή του αντίστοιχου πεδίου
                .Name = "colDrugQRCode"
                .DataPropertyName = "APQ_PRODUCT_CODE"
                ' Formatting..
                .HeaderText = "QRcode"
                .Width = 100
                .DefaultCellStyle.Format = ""
                .ReadOnly = False
            End With

            With QrcodeCustom2
                .Name = "colDrugQRCodeCustom"
                .DataPropertyName = "APQ_IS_CUSTOM_QR"
                .HeaderText = "IsCustomQr"
                .Visible = False
            End With
        Else
            If barcodeType = "barcode" Then
                With Barcode2
                    ' και του δίνει τη τιμή του αντίστοιχου πεδίου
                    .Name = "colDrugBarcode"
                    .DataPropertyName = "BRAP_AP_BARCODE"
                    ' Formatting..
                    .HeaderText = "Barcode"
                    .Width = 90
                    .DefaultCellStyle.Format = ""
                    .ReadOnly = True
                End With
            ElseIf barcodeType = "qrcode" Then
                With Barcode2
                    ' και του δίνει τη τιμή του αντίστοιχου πεδίου
                    .Name = "colDrugQRCode"
                    .DataPropertyName = "APQ_PRODUCT_CODE"
                    ' Formatting..
                    .HeaderText = "QRcode"
                    .Width = 100
                    .DefaultCellStyle.Format = ""
                    .ReadOnly = False
                End With

                With QrcodeCustom2
                    .Name = "colDrugQRCodeCustom"
                    .DataPropertyName = "APQ_IS_CUSTOM_QR"
                    .HeaderText = "IsCustomQr"
                    .Visible = False
                End With
            End If
        End If


        'Εμφανίζει τα columns του Datagrid
        With dgvPricesParadrugs
            If rbByName.Checked = True Then
                .Columns.Add(Name2)
                .Columns.Add(Morfi2)
                .Columns.Add(Xondr2)
                .Columns.Add(Lian2)
                .Columns.Add(AP_Code2)
                .Columns.Add(AP_ID2)
                .Columns.Add(Barcode2)
                .Columns.Add(Qrcode2)
                .Columns.Add(QrcodeCustom2)
            Else
                .Columns.Add(Name2)
                .Columns.Add(Morfi2)
                .Columns.Add(Xondr2)
                .Columns.Add(Lian2)
                .Columns.Add(AP_Code2)
                .Columns.Add(AP_ID2)
                .Columns.Add(Barcode2)
                If barcodeType = "qrcode" Then
                    .Columns.Add(QrcodeCustom2)
                End If
            End If

            ' Εναλλαγή του χρωματισμού των rows
            .RowsDefaultCellStyle.BackColor = Color.Bisque
            .AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

            .ReadOnly = False
            For Each col As DataGridViewColumn In .Columns
                col.ReadOnly = True
            Next
            If .Columns.Contains("colDrugQRCode") Then
                .Columns("colDrugQRCode").ReadOnly = False
            End If
            If .Columns.Contains("colDrugQRCodeCustom") Then
                .Columns("colDrugQRCodeCustom").Visible = False
            End If

            'Εξαφανίζει τo πεδίο "Id"
            .Columns(4).Visible = False
            .Columns(5).Visible = False
        End With

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dsDTG.Tables(0).Rows.Count

    End Function



    Private Function DisplayCustomDatagrid_Phones() As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(stringDTG, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)
        dsDTG = New DataSet

        ' και ο SqlDataAdapter γεμίζει το Dataset
        daDTG.Fill(dsDTG, "DTG")
        ' το οποίο γεμίζει το datatable
        dtDTG = dsDTG.Tables("DTG")

        'Καθορίζει το source του BindingSource ως το Datatable
        bsPhones.DataSource = dtDTG

        'Κλείνει την σύνδεση
        con.Close()


        With dgvPhones

            'Αδειάζει το datagridView
            .Columns.Clear()

            'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
            .AutoGenerateColumns = False

            'Καθορίζει το  source του DataGrid ως το BindingSource
            .DataSource = bsPhones

        End With

        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim Fullname As New DataGridViewTextBoxColumn
        With Fullname
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Fullname"
            ' Formatting..
            .HeaderText = "Ονοματεπώνυμο"
            .Width = 273
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
        Dim PhoneNumber1 As New DataGridViewTextBoxColumn
        With PhoneNumber1
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "PhoneNumber1"
            ' Formatting..
            .HeaderText = "Τηλέφωνο 1"
            .Width = 120
            .DefaultCellStyle.Format = ""
        End With

        Dim PhoneNumber2 As New DataGridViewTextBoxColumn
        With PhoneNumber2
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "PhoneNumber2"
            ' Formatting..
            .HeaderText = "Τηλέφωνο 2"
            .Width = 120
            .DefaultCellStyle.Format = ""
        End With

        Dim PhoneNumber3 As New DataGridViewTextBoxColumn
        With PhoneNumber3
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "PhoneNumber3"
            ' Formatting..
            .HeaderText = "Τηλέφωνο 3"
            .Width = 120
            .DefaultCellStyle.Format = ""
        End With

        Dim PhoneNumber4 As New DataGridViewTextBoxColumn
        With PhoneNumber4
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "PhoneNumber4"
            ' Formatting..
            .HeaderText = "Τηλέφωνο 4"
            .Width = 120
            .DefaultCellStyle.Format = ""
        End With

        Dim Id As New DataGridViewTextBoxColumn
        With Id
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Id"
            ' Formatting..
            .HeaderText = "Id"
            .Width = 50
            .DefaultCellStyle.Format = ""
        End With

        With dgvPhones

            'Εμφανίζει τα columns του Datagrid
            .Columns.Add(Fullname)
            .Columns.Add(PhoneNumber1)
            .Columns.Add(PhoneNumber2)
            .Columns.Add(PhoneNumber3)
            .Columns.Add(PhoneNumber4)
            .Columns.Add(Id)

            ' Εναλλαγή του χρωματισμού των rows
            .RowsDefaultCellStyle.BackColor = Color.Bisque
            .AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

            'Εξαφανίζει τo πεδίο "Id"
            .Columns(5).Visible = False
        End With

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dsDTG.Tables(0).Rows.Count

    End Function


    Private Function DisplayCustomDatagrid_TameiaGiven() As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(stringDTG, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)

        dsDTG = New DataSet

        ' και ο SqlDataAdapter γεμίζει το Dataset
        daDTG.Fill(dsDTG, "DTG")
        ' το οποίο γεμίζει το datatable
        dtDTG = dsDTG.Tables("DTG")

        'Καθορίζει το source του BindingSource ως το Datatable
        bsTameiaGiven.DataSource = dtDTG

        With dgvTameiaGiven

            'Αδειάζει το datagridView
            .Columns.Clear()

            'Εμποδίσει το Datagrid να εμφανίσει αυτόματα τα Columns
            .AutoGenerateColumns = False

            'Καθορίζει το  source του DataGrid ως το BindingSource
            .DataSource = bsTameiaGiven

        End With

        'Κλείνει την σύνδεση
        con.Close()

        'Όρίζει το 1ο πεδίο του Datagrid σαν textbox
        Dim MyDate As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        MyDate.DataPropertyName = "MyDate"
        ' Formatting..
        MyDate.HeaderText = "Ημερομηνία"
        MyDate.Width = 80
        MyDate.DefaultCellStyle.Format = ""

        'Όρίζει το 2ο πεδίο του Datagrid σαν textbox
        Dim Description As New DataGridViewTextBoxColumn
        With Description
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "Description"
            ' Formatting..
            .HeaderText = "Αιτιολογία"
            .Width = 250
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 3ο πεδίο του Datagrid σαν textbox
        Dim AmountPaid As New DataGridViewTextBoxColumn
        With AmountPaid
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "AmountPaid2"
            ' Formatting..
            .HeaderText = "Καταβληθέν ποσό"
            .Width = 70
            .DefaultCellStyle.Format = "F2"
        End With

        'Όρίζει το 4ο πεδίο του Datagrid σαν textbox
        Dim PercTotalPaid As New DataGridViewTextBoxColumn
        With PercTotalPaid
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "PercTotalPaid2"
            ' Formatting..
            .HeaderText = "% επί συνόλου"
            .Width = 70
            .DefaultCellStyle.Format = ""
        End With

        'Όρίζει το 5ο πεδίο του Datagrid σαν textbox
        Dim DifferenceToTotal As New DataGridViewTextBoxColumn
        With DifferenceToTotal
            ' και του δίνει τη τιμή του αντίστοιχου πεδίου
            .DataPropertyName = "DifferenceToTotal2"
            ' Formatting..
            .HeaderText = "Υπόλοιπο"
            .Width = 70
            .DefaultCellStyle.Format = "F2"
        End With

        'Όρίζει το 7ο πεδίο του Datagrid σαν textbox
        Dim Id As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        With Id
            .DataPropertyName = "Id"
            .HeaderText = "Id"
            .Width = 50
        End With


        'Όρίζει το 8ο πεδίο του Datagrid σαν textbox
        Dim TameiaAskedId As New DataGridViewTextBoxColumn
        ' και του δίνει τη τιμή του αντίστοιχου πεδίου
        With TameiaAskedId
            .DataPropertyName = "TameiaAskedId"
            .HeaderText = "TameiaAskedId"
            .Width = 50
        End With

        ''Όρίζει το 1ο πεδίο του Datagrid σαν combobox
        'Dim Combo As New DataGridViewComboBoxColumn
        '' και του δίνει τη τιμή του αντίστοιχου πεδίου
        'Combo.DataPropertyName = "AP_DESCRIPTION"
        ''Παίρνει όλες τις πιθανές τιμές του Category και τις προσθέτει σαν επιλογές στο combobox
        'For t = 0 To DrugList.Length - 1
        '    If Not (DrugList(t) Is Nothing) Then Combo.Items.Add(DrugList(t))
        'Next

        With dgvTameiaGiven

            'Εμφανίζει τα columns του Datagrid
            .Columns.Add(MyDate)
            .Columns.Add(Description)
            .Columns.Add(AmountPaid)
            .Columns.Add(PercTotalPaid)
            .Columns.Add(DifferenceToTotal)
            .Columns.Add(Id)
            .Columns.Add(TameiaAskedId)


            ' Εναλλαγή του χρωματισμού των rows
            .RowsDefaultCellStyle.BackColor = Color.Bisque
            .AlternatingRowsDefaultCellStyle.BackColor = Color.Beige

            'Εξαφανίζει τo πεδίο "Id"
            '.Columns(5).Visible = False
            '.Columns(6).Visible = False

        End With


        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dsDTG.Tables(0).Rows.Count

    End Function


    Private Sub UpdateTameiaAsked(ByVal i As Integer)
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""
        'Dim myvar1 As String = dgvTameiaAsked.Rows(i).Cells(5).EditedFormattedValue
        'Dim myvar2 As Decimal = dgvTameiaAsked.Rows(i).Cells(5).Value

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists = CheckIfRecordChangedOrExists_TameiaAsked(i)

            If ChangedOrExists = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.TameiaAsked " &
                                "SET [MyDate] = @MyDate, [Description] =  @Description, [AmountAsked] = @AmountAsked, [AmountGiven] = @AmountGiven,  " &
                                "[Difference] = @Difference, [DifferPercent] = @DifferPercent, [PercentagePaid] = @PercentagePaid, [IRS] = @IRS " &
                                "WHERE Id = @Id"

            ElseIf ChangedOrExists = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.TameiaAsked " &
                                "([MyDate], [Description], [AmountAsked], [AmountGiven], [Difference], [DifferPercent]," &
                                "[PercentagePaid], [IRS]) " &
                            "VALUES (@MyDate, @Description, @AmountAsked, @AmountGiven, @Difference, @DifferPercent," &
                                "@PercentagePaid, @IRS)"

            End If

            If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

                Dim cmd As New SqlCommand(insertData, con)

                If IsDBNull(dgvTameiaAsked.Rows(i).Cells(0).Value) = True Then
                    'MsgBox("Καταχωρήστε έγκυρη ημερομηνία!")
                ElseIf IsDBNull(dgvTameiaAsked.Rows(i).Cells(1).Value) = True Then
                    ' MsgBox("Καταχωρήστε έγκυρη περιγραφή!")
                ElseIf dgvTameiaAsked.Rows(i).Cells(2).Value = 0 Then
                    ' MsgBox("Καταχωρήστε έγκυρο ποσό!")
                Else

                    'Dim DifferPercent As Decimal = 0

                    'If dgvTameiaAsked.Rows(i).Cells(5).EditedFormattedValue <> "" Then
                    '    DifferPercent = CType(dgvTameiaAsked.Rows(i).Cells(5).EditedFormattedValue.Substring(0, dgvTameiaAsked.Rows(i).Cells(5).EditedFormattedValue.Length - 2), Decimal)
                    '    DifferPercent = DifferPercent * 100
                    'End If

                    'MsgBox("EFV5=" & dgvTameiaAsked.Rows(i).Cells(5).EditedFormattedValue & " - V5=" & dgvTameiaAsked.Rows(i).Cells(5).Value & " - DP-EFV5=" & DePercent(dgvTameiaAsked.Rows(i).Cells(5).EditedFormattedValue))

                    cmd.Parameters.AddWithValue("@MyDate", If(dgvTameiaAsked.Rows(i).Cells(0).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvTameiaAsked.Rows(i).Cells(0).EditedFormattedValue, Date)))
                    cmd.Parameters.AddWithValue("@Description", If(dgvTameiaAsked.Rows(i).Cells(1).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@AmountAsked", If(dgvTameiaAsked.Rows(i).Cells(2).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvTameiaAsked.Rows(i).Cells(2).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@AmountGiven", If(dgvTameiaAsked.Rows(i).Cells(3).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvTameiaAsked.Rows(i).Cells(3).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@Difference", If(dgvTameiaAsked.Rows(i).Cells(4).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvTameiaAsked.Rows(i).Cells(4).EditedFormattedValue, Decimal)))
                    'cmd.Parameters.AddWithValue("@DifferPercent", If(dgvTameiaAsked.Rows(i).Cells(5).EditedFormattedValue = "", _
                    '                            DBNull.Value, DePercent(dgvTameiaAsked.Rows(i).Cells(5).EditedFormattedValue)))
                    cmd.Parameters.AddWithValue("@DifferPercent", dgvTameiaAsked.Rows(i).Cells(5).Value)
                    cmd.Parameters.AddWithValue("@PercentagePaid", dgvTameiaAsked.Rows(i).Cells(6).Value)
                    cmd.Parameters.AddWithValue("@IRS", dgvTameiaAsked.Rows(i).Cells(7).EditedFormattedValue)

                    If ChangedOrExists = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvTameiaAsked.Rows(i).Cells(8).Value, DBNull.Value))

                    cmd.ExecuteNonQuery()

                End If

            ElseIf ChangedOrExists = "Error" Then

            End If

        End Using

    End Sub


    Private Sub UpdatePhones(ByVal i As Integer)
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists_Phones = CheckIfRecordChangedOrExists_Phones(i)

            If ChangedOrExists_Phones = "NotChanged" Then
                Exit Sub
            ElseIf ChangedOrExists_Phones = "Error" Then

            End If

            If ChangedOrExists_Phones = "Changed" Then

                ' Επιβεβαίωση της τροποποίησης
                If MessageBox.Show("Θέλετε να τροποποιήσετε τα στοιχεία;", "Τροποποίηση", MessageBoxButtons.YesNo) = DialogResult.No Then
                    Exit Sub
                End If

                insertData = "UPDATE PharmacyCustomFiles.dbo.Phonebook " &
                                "SET [Fullname] = @Fullname, [PhoneNumber1] =  @PhoneNumber1, [PhoneNumber2] = @PhoneNumber2, [PhoneNumber3] = @PhoneNumber3,  " &
                                "[PhoneNumber4] = @PhoneNumber4 " &
                                "WHERE Id = @Id"

            ElseIf ChangedOrExists_Phones = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.Phonebook " &
                                "([Fullname], [PhoneNumber1], [PhoneNumber2], [PhoneNumber3], [PhoneNumber4]) " &
                                "VALUES (@Fullname, @PhoneNumber1, @PhoneNumber2, @PhoneNumber3, @PhoneNumber4)"

            End If


            Dim cmd As New SqlCommand(insertData, con)

            If IsDBNull(dgvPhones.Rows(i).Cells(0).Value) = True Then

            Else

                cmd.Parameters.AddWithValue("@Fullname", dgvPhones.Rows(i).Cells(0).EditedFormattedValue)
                cmd.Parameters.AddWithValue("@PhoneNumber1", dgvPhones.Rows(i).Cells(1).EditedFormattedValue)

                cmd.Parameters.AddWithValue("@PhoneNumber2", dgvPhones.Rows(i).Cells(2).EditedFormattedValue)

                cmd.Parameters.AddWithValue("@PhoneNumber3", dgvPhones.Rows(i).Cells(3).EditedFormattedValue)

                cmd.Parameters.AddWithValue("@PhoneNumber4", dgvPhones.Rows(i).Cells(4).EditedFormattedValue)

                If ChangedOrExists_Phones = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvPhones.Rows(i).Cells(5).Value, DBNull.Value))

                cmd.ExecuteNonQuery()

                DisplayLastUpdate()

                txtSearchPhones.Text = dgvPhones.Rows(i).Cells(0).EditedFormattedValue

            End If

        End Using


    End Sub


    Private Sub UpdateParadrugs(ByVal i As Integer)
        Dim insertData As String = ""

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists_Phones = CheckIfRecordChangedOrExists_Phones(i)
            If ChangedOrExists_Phones = "NotChanged" Then
                Exit Sub
            ElseIf ChangedOrExists_Phones = "Error" Then

            End If

            If ChangedOrExists_Phones = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.Phonebook " &
                                "SET [Fullname] = @Fullname, [PhoneNumber1] =  @PhoneNumber1, [PhoneNumber2] = @PhoneNumber2, [PhoneNumber3] = @PhoneNumber3,  " &
                                "[PhoneNumber4] = @PhoneNumber4 " &
                                "WHERE Id = @Id"

            ElseIf ChangedOrExists_Phones = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.Phonebook " &
                                "([Fullname], [PhoneNumber1], [PhoneNumber2], [PhoneNumber3], [PhoneNumber4]) " &
                                "VALUES (@Fullname, @PhoneNumber1, @PhoneNumber2, @PhoneNumber3, @PhoneNumber4)"

            End If


            Dim cmd As New SqlCommand(insertData, con)

            If IsDBNull(dgvPhones.Rows(i).Cells(0).Value) = True Then

            Else

                cmd.Parameters.AddWithValue("@Fullname", dgvPhones.Rows(i).Cells(0).EditedFormattedValue)
                cmd.Parameters.AddWithValue("@PhoneNumber1", dgvPhones.Rows(i).Cells(1).EditedFormattedValue)

                cmd.Parameters.AddWithValue("@PhoneNumber2", dgvPhones.Rows(i).Cells(2).EditedFormattedValue)

                cmd.Parameters.AddWithValue("@PhoneNumber3", dgvPhones.Rows(i).Cells(3).EditedFormattedValue)

                cmd.Parameters.AddWithValue("@PhoneNumber4", dgvPhones.Rows(i).Cells(4).EditedFormattedValue)

                If ChangedOrExists_Phones = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvPhones.Rows(i).Cells(5).Value, DBNull.Value))

                cmd.ExecuteNonQuery()

                txtSearchPhones.Text = dgvPhones.Rows(i).Cells(0).EditedFormattedValue

            End If

        End Using


    End Sub

    Private Function CreationDate(ByVal path As String) As String
        Dim fileCreatedDate As DateTime = File.GetCreationTime(path)

        Return fileCreatedDate
    End Function

    Private Function LastModifiedDate(ByVal path As String) As String
        Dim fileCreatedDate As DateTime = File.GetLastWriteTime(path)

        Return Format(fileCreatedDate, "dd-MM-yyyy, HH:mm")
    End Function

    Private Function CreatedDate(ByVal path As String) As String
        Dim fileCreatedDate As DateTime = File.GetCreationTime(path)

        Return Format(fileCreatedDate, "dd-MM-yyyy, HH:mm")
    End Function




    Private Sub UpdateTameiaGiven(ByVal i As Integer)
        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""

        'Dim myvar1 As String = dgvTameiaAsked.Rows(i).Cells(5).EditedFormattedValue
        'Dim myvar2 As Decimal = dgvTameiaAsked.Rows(i).Cells(5).Value


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists = CheckIfRecordChangedOrExists_TameiaGiven(i)

            If ChangedOrExists = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.TameiaPaid " &
                                "SET [MyDate] = @MyDate, [Description] =  @Description, [AmountPaid] = @AmountPaid, [PercTotalPaid] = @PercTotalPaid,  " &
                                "[DifferenceToTotal] = @DifferenceToTotal, [TameiaAskedId] = @TameiaAskedId " &
                                "WHERE Id = @Id"

            ElseIf ChangedOrExists = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.TameiaPaid " &
                                "([MyDate], [Description], [AmountPaid], [PercTotalPaid], [DifferenceToTotal], " &
                                "[TameiaAskedId]) " &
                            "VALUES (@MyDate, @Description, @AmountPaid, @PercTotalPaid, @DifferenceToTotal, " &
                                "@TameiaAskedId)"

            End If

            If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

                Dim cmd As New SqlCommand(insertData, con)

                If IsDBNull(dgvTameiaGiven.Rows(i).Cells(0).Value) = True Then
                    'MsgBox("Καταχωρήστε έγκυρη ημερομηνία!")
                ElseIf IsDBNull(dgvTameiaGiven.Rows(i).Cells(1).Value) = True Then
                    ' MsgBox("Καταχωρήστε έγκυρη περιγραφή!")
                ElseIf dgvTameiaGiven.Rows(i).Cells(2).Value = 0 Then
                    ' MsgBox("Καταχωρήστε έγκυρο ποσό!")
                Else

                    'MsgBox("EFV5=" & dgvTameiaGiven.Rows(i).Cells(1).EditedFormattedValue & " - V5=" & dgvTameiaGiven.Rows(i).Cells(1).Value & " - DP-EFV5=" & dgvTameiaGiven.Rows(i).Cells(5).EditedFormattedValue)


                    'Dim CompletedPayment As Boolean
                    'MsgBox("cv=" & dgvTameiaGiven.Rows(i).Cells(5).EditedFormattedValue)

                    cmd.Parameters.AddWithValue("@MyDate", If(dgvTameiaGiven.Rows(i).Cells(0).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvTameiaGiven.Rows(i).Cells(0).EditedFormattedValue, Date)))
                    cmd.Parameters.AddWithValue("@Description", If(dgvTameiaGiven.Rows(i).Cells(1).EditedFormattedValue, DBNull.Value))
                    cmd.Parameters.AddWithValue("@AmountPaid", If(dgvTameiaGiven.Rows(i).Cells(2).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvTameiaGiven.Rows(i).Cells(2).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@PercTotalPaid", If(dgvTameiaGiven.Rows(i).Cells(3).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvTameiaGiven.Rows(i).Cells(3).EditedFormattedValue, Decimal)))
                    cmd.Parameters.AddWithValue("@DifferenceToTotal", If(dgvTameiaGiven.Rows(i).Cells(4).EditedFormattedValue = "",
                                                DBNull.Value, CType(dgvTameiaGiven.Rows(i).Cells(4).EditedFormattedValue, Decimal)))

                    cmd.Parameters.AddWithValue("@TameiaAskedId", dgvTameiaAsked.SelectedRows(0).Cells(8).Value)

                    If ChangedOrExists = "Changed" Then cmd.Parameters.AddWithValue("@Id", If(dgvTameiaGiven.Rows(i).Cells(5).Value, DBNull.Value))



                    cmd.ExecuteNonQuery()

                End If

            ElseIf ChangedOrExists = "error" Then
                Exit Sub
            End If

        End Using

    End Sub



    Private Sub dgvTameiaAsked_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTameiaAsked.CellClick
        GetTameiaGivenList()
    End Sub


    Private Sub CalculateDiffAndPercPerRow(ByVal i As Integer)
        'MsgBox(dgvTameiaAsked.Rows(i).Cells(3).EditedFormattedValue & " - " & dgvTameiaAsked.Rows(i).Cells(3).Value)

        Dim amountAsked As Decimal = 0
        Dim amountGiven As Decimal = 0


        Try
            amountAsked = dgvTameiaAsked.Rows(i).Cells(2).Value
            amountGiven = dgvTameiaAsked.Rows(i).Cells(3).Value

        Catch ex As Exception
            Exit Sub
        End Try

        If IsNumeric(amountAsked) = True And amountAsked > 0 Then
            If IsNumeric(amountGiven) = True Then

                dgvTameiaAsked.Rows(i).Cells(4).Value = amountAsked - amountGiven ' Διαφορά τιμής μεταξυ αίτησης & εκκαθάρισης
                dgvTameiaAsked.Rows(i).Cells(5).Value = (((amountAsked - amountGiven) / amountAsked) * 100).ToString("###.0") ' % διαφοράς επί αιτούμενης τιμής
                dgvTameiaAsked.Rows(i).Cells(6).Value = ((amountGiven / amountAsked) * 100).ToString("###.0")

                If ((amountGiven / amountAsked) * 100) > 97 Then
                    dgvTameiaAsked.Rows(i).Cells(7).Value = True
                Else
                    dgvTameiaAsked.Rows(i).Cells(7).Value = False
                End If

            End If
        End If
    End Sub


    Private Function CalculateDiffAndPercPerRow_TameiaGiven(ByVal i As Integer) As Boolean
        'MsgBox(dgvTameiaAsked.Rows(i).Cells(3).EditedFormattedValue & " - " & dgvTameiaAsked.Rows(i).Cells(3).Value)

        Dim amountPaid As Decimal = 0
        Dim amountTotal As Decimal = 0
        Dim amountPermitted As Decimal = 0
        Dim amountEntered As Decimal
        Dim id As Integer = 0



        Try
            id = dgvTameiaAsked.SelectedRows(0).Cells(8).Value
        Catch ex As Exception
        End Try

        Dim STR As String = "SELECT AmountPaid " &
                       "FROM PharmacyCustomFiles.dbo.TameiaPaid " &
                      "WHERE TameiaAskedId = '" & id & "'"

        Try
            amountTotal = dgvTameiaAsked.SelectedRows(0).Cells(2).Value
            amountEntered = dgvTameiaGiven.SelectedRows(0).Cells(2).EditedFormattedValue
            amountPaid = CalculateSums(STR, "AmountPaid")
            'MsgBox(amountPaid)
            amountPermitted = amountTotal - amountPaid

        Catch ex As Exception
            Return False
        End Try

        Try
            id = dgvTameiaGiven.SelectedRows(0).Cells(5).Value
        Catch ex As Exception
            id = 0
        End Try

        If IsNumeric(amountTotal) = True And amountTotal > 0 Then
            'If id > 0 Then amountPermitted
            If amountEntered + amountPaid <= amountTotal Then

                dgvTameiaGiven.Rows(i).Cells(4).Value = amountPermitted - amountEntered ' Διαφορά τιμής μεταξυ αίτησης & εκκαθάρισης
                dgvTameiaGiven.Rows(i).Cells(3).Value = ((amountEntered / amountTotal) * 100).ToString("###.0") ' % διαφοράς επί αιτούμενης τιμής
            Else
                MsgBox("Δεν μπορείτε να καταχωρήσετε περισσότερα από " & amountPermitted.ToString("c") & " ευρώ !")
                Return False
            End If
        End If
        Return True
    End Function


    Private Sub dgvTameiaAsked_DefaultValuesNeeded(sender As Object, e As DataGridViewRowEventArgs) Handles dgvTameiaAsked.DefaultValuesNeeded
        'Ορίζει default τιμές 
        With e.Row
            .Cells(0).Value = Today() ' Date
            .Cells(2).Value = 0
            .Cells(3).Value = 0

        End With
    End Sub


    Private Sub dgvTameiaGiven_DefaultValuesNeeded(sender As Object, e As DataGridViewRowEventArgs) Handles dgvTameiaGiven.DefaultValuesNeeded
        Dim myName As String = ""
        Dim myDate As Date

        Try
            myName = dgvTameiaAsked.SelectedRows(0).Cells(1).Value
            myDate = dgvTameiaAsked.SelectedRows(0).Cells(0).Value
        Catch ex As Exception
        End Try

        'Ορίζει default τιμές 
        With e.Row
            .Cells(0).Value = Today() ' Date
            .Cells(1).Value = myName & " " & Month(myDate) & "/" & Year(myDate)
            .Cells(2).Value = 0
            .Cells(3).Value = 0
            .Cells(4).Value = 0

            Try
                .Cells(6).Value = dgvTameiaAsked.SelectedRows(0).Cells(8).Value
            Catch ex As Exception
            End Try


        End With
    End Sub


    Private Sub btnDeleteTameiaAsked_Click(sender As Object, e As EventArgs) Handles btnDeleteTameiaAsked.Click
        DeleteTameiaAsked()
        GetTameiaAskedList()
    End Sub

    Private Function DisplayCustomDatagrid_TameiaAsked(dataGridView As DataGridView, bsTameiaAsked As BindingSource) As Integer
        Throw New NotImplementedException
    End Function

    Private Sub dgvGivenTo_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvGivenTo.CellEndEdit

        ' Αν πιέσουμε πάνω σε ποσότητα επαναπροσδιορίζει την τιμή
        If e.ColumnIndex = 3 Then
            RecalculateTotalXondr_GivenTo(e.RowIndex, dgvGivenTo.Rows(e.RowIndex).Cells(4).Value)
        End If

    End Sub

    Private Sub RecalculateTotalXondr_TakenFrom(ByVal rowIndex As Integer, ByVal InitialValue As Decimal)
        Dim ap_Code As Integer = 0
        Dim Xondr As Decimal = 1
        Dim NewValue As Decimal = 0
        Dim ProductName As String = dgvTakenFrom.Rows(rowIndex).Cells(1).EditedFormattedValue


        If dgvTakenFrom.Rows(rowIndex).Cells(6).EditedFormattedValue <> "" Then
            ap_Code = CType(dgvTakenFrom.Rows(rowIndex).Cells(6).EditedFormattedValue, Integer)
        Else
            ap_Code = 0
        End If

        If ap_Code > 0 And chkAutoInsertName.Checked = True Then
            NewValue = GetXondrSelectedDrug(ap_Code) * Qnt
            If NewValue <> InitialValue Then
                If MsgBox("To προιόν '" & ProductName & "' έχει διαφορετική τιμή από εκείνη του Pharmakon, από " & Format(NewValue / Qnt, "C") & " έχει γίνει " & Format(InitialValue / Qnt, "C") & ". " & vbCrLf & "Να προχωρήσω σε αποκατάσταση της αρχικής τιμής;", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                    dgvTakenFrom.Rows(rowIndex).Cells(4).Value = NewValue
                End If
            End If


        ElseIf ap_Code = 0 And chkAutoInsertName.Checked = True Then
            If IsDBNull(dgvTakenFrom.Rows(rowIndex).Cells(1).Value) = True Or IsDBNull(dgvTakenFrom.Rows(rowIndex).Cells(1).EditedFormattedValue) = True Then
                If dgvTakenFrom.Rows(rowIndex).Cells(4).Value <> 0 Then
                    MsgBox("Αντιστοιχήστε ένα κωδικό φαρμάκου στη καταχώρηση σας!!")
                End If
            End If
        ElseIf chkAutoInsertName.Checked = False Then

        End If
    End Sub


    Private Sub RecalculateTotalXondr_GivenTo(ByVal rowIndex As Integer, ByVal InitialValue As Decimal)
        Dim ap_Code As Integer = 0
        Dim Xondr As Decimal = 1
        Dim NewValue As Decimal = 0
        Dim ProductName As String = dgvGivenTo.Rows(rowIndex).Cells(1).EditedFormattedValue

        Try
            If dgvGivenTo.Rows(rowIndex).Cells(6).EditedFormattedValue <> "" Then
                ap_Code = CType(dgvGivenTo.Rows(rowIndex).Cells(6).EditedFormattedValue, Integer)
            End If
        Catch ex As Exception
        End Try

        'End If

        If ap_Code > 0 And chkAutoInsertName.Checked = True Then
            NewValue = GetXondrSelectedDrug(ap_Code) * Qnt
            If NewValue <> InitialValue Then
                If MsgBox("To προιόν '" & ProductName & "' έχει διαφορετική τιμή από εκείνη του Pharmakon, από " & Format(NewValue / Qnt, "C") & " έχει γίνει " & Format(InitialValue / Qnt, "C") & ". " & vbCrLf & "Να προχωρήσω σε αποκατάσταση της αρχικής τιμής;", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                    dgvGivenTo.Rows(rowIndex).Cells(4).Value = NewValue
                End If
            End If


        ElseIf ap_Code = 0 And chkAutoInsertName.Checked = True Then
            Try
                If IsDBNull(dgvGivenTo.Rows(rowIndex).Cells(1).Value) = True Or IsDBNull(dgvGivenTo.Rows(rowIndex).Cells(1).EditedFormattedValue = True) Then
                    If dgvGivenTo.Rows(rowIndex).Cells(4).Value <> 0 Then
                        MsgBox("Αντιστοιχήστε ένα κωδικό φαρμάκου στη καταχώρηση σας!!")
                    End If

                End If
            Catch ex As Exception
            End Try

        ElseIf chkAutoInsertName.Checked = False Then
            'MsgBox(dgvGivenTo.Rows(rowIndex).Cells(3).Value & " - EFV: " & dgvGivenTo.Rows(rowIndex).Cells(3).EditedFormattedValue)
            'dgvGivenTo.Rows(rowIndex).Cells(3).Value = GetXondrSelectedDrug(ap_Code) * qnt
        End If

        'dgvGivenTo.CurrentCell = dgvGivenTo.SelectedRows(0).Cells(1)

    End Sub





    Private Sub DisplayNewRowExchanges(ByVal oDatagrid As DataGrid)

    End Sub

    Private Sub dgvTakenFrom_KeyPress(sender As Object, e As KeyPressEventArgs) Handles dgvTakenFrom.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Return) And dgvTakenFrom.CurrentCell.RowIndex = dgvTakenFrom.Rows.Count - 1 Then
            'MessageBox.Show("Enter key pressed on last row (" & dgvGivenTo.CurrentCell.RowIndex & "-" & (dgvGivenTo.Rows.Count - 1) & ")")
            If IsDBNull(dgvTakenFrom.SelectedRows(0).Cells(1).Value) = False Then
                bsExchangesTakenFrom.AddNew()
                dgvTakenFrom.SelectedRows(0).Cells(3).Value = 1 ' Qnt
                dgvTakenFrom.SelectedRows(0).Cells(4).Value = 0 ' Xondr

                If chkAutoInsertName.Checked = False Then
                    dgvTakenFrom.CurrentCell = dgvTakenFrom.SelectedRows(0).Cells(1)
                    dgvTakenFrom.BeginEdit(True)
                End If

            End If

        End If
    End Sub



    Private Sub dgvTameiaAsked_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTameiaAsked.CellValueChanged

        If e.ColumnIndex = 0 Or e.ColumnIndex = 1 Then
            UpdateTameiaAsked(e.RowIndex)
        ElseIf e.ColumnIndex = 2 Then
            'MsgBox(dgvTameiaAsked.Rows(e.RowIndex).Cells(e.ColumnIndex).EditedFormattedValue & " - " & dgvTameiaAsked.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)
            CalculateDiffAndPercPerRow(e.RowIndex)
            UpdateTameiaAsked(e.RowIndex)
            DisplaySumsOnRichTextbox_TameiaAsked()
        End If
    End Sub

    Private Sub dgvTameiaGiven_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTameiaGiven.CellContentClick

    End Sub

    Private Sub dgvTameiaGiven_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTameiaGiven.CellValueChanged

        If e.ColumnIndex = 0 Or e.ColumnIndex = 1 Then

            UpdateTameiaGiven(e.RowIndex)

        ElseIf e.ColumnIndex = 2 Then
            'MsgBox(dgvTameiaAsked.Rows(e.RowIndex).Cells(e.ColumnIndex).EditedFormattedValue & " - " & dgvTameiaAsked.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)

            If CalculateDiffAndPercPerRow_TameiaGiven(e.RowIndex) = True Then
                UpdateTameiaGiven(e.RowIndex)

                Dim TARowIndex As Integer = 0
                Try
                    TARowIndex = dgvTameiaAsked.CurrentCell.RowIndex
                Catch ex As Exception
                End Try

                CalculateTotalTameiaPaidPerTransaction()

                CalculateDiffAndPercPerRow(TARowIndex)

                UpdateTameiaAsked(TARowIndex)

                'GetTameiaAskedList()

                DisplaySumsOnRichTextbox_TameiaAsked()

            End If

        End If
    End Sub

    'Private Sub dgvGivenTo_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvGivenTo.CellValueChanged
    '    ExchangesGivenOrTaken = "given"
    '    UpdateExchangesPerRow(e.RowIndex)
    'End Sub

    'Private Sub dgvTakenFrom_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTakenFrom.CellValueChanged
    '    ExchangesGivenOrTaken = "taken"
    '    UpdateExchangesPerRow(e.RowIndex)
    'End Sub

    'Private Sub dgvTakenFrom_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTakenFrom.RowLeave
    '    ExchangesGivenOrTaken = "taken"
    '    UpdateExchangesPerRow(e.RowIndex)
    '    If ChangedOrNew_Exchanges = "NewRow" Then
    '        GetExchangesList("taken")
    '    End If

    'End Sub

    'Private Sub dgvGivenTo_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvGivenTo.RowLeave
    '    ExchangesGivenOrTaken = "given"
    '    UpdateExchangesPerRow(e.RowIndex)
    '    If ChangedOrNew_Exchanges = "NewRow" Then
    '        GetExchangesList("given")
    '    End If
    'End Sub


    Private Sub btnDeleteTameiaGiven_Click(sender As Object, e As EventArgs) Handles btnDeleteTameiaGiven.Click
        DeleteTameiaGiven()
        GetTameiaGivenList()
        CalculateTotalTameiaPaidPerTransaction()

        Try
            CalculateDiffAndPercPerRow(dgvTameiaAsked.CurrentCell.RowIndex)
            UpdateTameiaAsked(dgvTameiaAsked.CurrentCell.RowIndex)
        Catch ex As Exception
        End Try

    End Sub

    Private Sub rbWhereLaptop_CheckedChanged(sender As Object, e As EventArgs) Handles rbWhereLaptop.CheckedChanged
        txtSourceFolderVS.Text = "C:\Pharmacy\VBNET Development"
        txtDestinationDrive.Text = "C:\Pharmacy\MyPharmacy Files"
        btnBackupRestore.Enabled = True
        btnUpdatePharmacy2013C.Enabled = True
        btnCoppyAppStation1.Enabled = False
    End Sub

    Private Sub dgvTameiaAsked_LostFocus(sender As Object, e As EventArgs) Handles dgvTameiaAsked.LostFocus

    End Sub

    Private Sub dgvTameiaAsked_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTameiaAsked.RowLeave
        Try
            dgvTameiaAsked.CurrentCell = dgvTameiaAsked.SelectedRows(0).Cells(0)
        Catch ex As Exception
        End Try

    End Sub

    Private Sub txtSearchPhones_TextChanged(sender As Object, e As EventArgs) Handles txtSearchPhones.TextChanged
        GetPhonesList()
    End Sub


    Private Sub cboTameia_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTameia.SelectedIndexChanged
        UpdateDataTameia()
    End Sub


    Private Sub btnDeletePhones_Click(sender As Object, e As EventArgs) Handles btnDeletePhones.Click
        DeletePhones()
        GetPhonesList()
    End Sub

    Private Sub dgvPhones_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPhones.CellContentClick

    End Sub

    Private Sub dgvPhones_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPhones.CellValueChanged
        Select Case cboPhoneCatalog.Text
            Case "Η ΑΤΖΕΝΤΑ ΜΟΥ"

                UpdatePhones(e.RowIndex)
                If ChangedOrExists_Phones = "NewRow" Then
                    GetPhonesList()
                End If

            Case "ΑΣΘΕΝΕΙΣ - PHARM"

            Case "ΙΑΤΡΟΙ"

        End Select

    End Sub

    Private Sub dgvGivenTo_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvGivenTo.CellValueChanged
        If e.RowIndex < 0 Then Exit Sub
        If e.ColumnIndex = 4 Then PersistExchangeXondrOverride(dgvGivenTo, e.RowIndex)

        'Όποτε αλλάζει η τιμή του cell και..

        'είμαστε σε manual καταχώρηση..
        If chkAutoInsertName.Checked = False Then

            ExchangesGivenOrTaken = "given"
            UpdateExchanges_Given(e.RowIndex)

            ' Αν είμαστε σε αυτόματη καταχώρηση..
        ElseIf chkAutoInsertName.Checked = True Then

            ' Αν το Όνομα είναι κενο ->
            If IsDBNull(dgvGivenTo.Rows(e.RowIndex).Cells(1).Value) = True Then
                ' -> τίποτα

                ' Αν το Όνομα ΔΕΝ είναι κένο ->
            Else
                ' Αν το Όνομα και η Χονδρική έχουν κάποια τιμή ->
                If dgvGivenTo.Rows(e.RowIndex).Cells(1).Value <> "" And
                                       dgvGivenTo.Rows(e.RowIndex).Cells(4).Value > 0 Then
                    '///// MsgBox("Whole Row Updated!" & vbCrLf & "Name: " & dgvGivenTo.Rows(e.RowIndex).Cells(1).Value & ", Xondriki: " & dgvGivenTo.Rows(e.RowIndex).Cells(3).Value)
                    ' -> σώζει τις αλλαγές
                    ExchangesGivenOrTaken = "given"
                    UpdateExchanges_Given(e.RowIndex)

                End If
            End If
        End If



    End Sub



    '  If e.ColumnIndex = 0 Or e.ColumnIndex = 1 Then
    '        UpdateTameiaAsked(e.RowIndex)
    '    ElseIf e.ColumnIndex = 2 Then
    ''MsgBox(dgvTameiaAsked.Rows(e.RowIndex).Cells(e.ColumnIndex).EditedFormattedValue & " - " & dgvTameiaAsked.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)
    '        CalculateDiffAndPercPerRow(e.RowIndex)
    '        UpdateTameiaAsked(e.RowIndex)
    '        DisplaySumsOnRichTextbox_TameiaAsked()
    '    End If

    'Private Sub dgvGivenTo_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgvGivenTo.CellEnter
    '    If dgvGivenTo.CurrentRow.IsNewRow() = True And chkAutoInsertName.Checked = False Then
    '        rowIndex = dgvGivenTo.CurrentRow.Index
    '        'MsgBox("New Line: " & rowIndex)
    '        ChangedOrNew_Exchanges = "NewRow"

    '    End If
    'End Sub

    'Private Sub dgvGivenTo_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvGivenTo.RowLeave


    '    'είμαστε σε manual καταχώρηση..
    '    If chkAutoInsertName.Checked = False Then


    '        ' Αν είμαστε σε αυτόματη καταχώρηση..
    '    ElseIf chkAutoInsertName.Checked = True Then
    '        Try
    '            ' εάν το Όνομα είναι κενό ->
    '            If dgvGivenTo.Rows(rowIndex).Cells(1).Value Is Nothing Then
    '                'δεν κάνει τίποτα

    '                'αλλιώς ->
    '            Else
    '                ' σώνει τις αλλαγές
    '                ExchangesGivenOrTaken = "given"
    '                UpdateExchangesPerRow(e.RowIndex)
    '            End If
    '        Catch ex As Exception
    '        End Try
    '    End If

    'End Sub



    Private Sub dgvTakenFrom_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTakenFrom.CellValueChanged
        If e.RowIndex < 0 Then Exit Sub
        If e.ColumnIndex = 4 Then PersistExchangeXondrOverride(dgvTakenFrom, e.RowIndex)

        'Όποτε αλλάζει η τιμή του cell και..

        'είμαστε σε manual καταχώρηση..
        If chkAutoInsertName.Checked = False Then

            ExchangesGivenOrTaken = "taken"
            UpdateExchanges_Taken(e.RowIndex)

            ' Αν είμαστε σε αυτόματη καταχώρηση..
        ElseIf chkAutoInsertName.Checked = True Then

            ' Αν το Όνομα είναι κενο ->
            If IsDBNull(dgvTakenFrom.Rows(e.RowIndex).Cells(1).Value) = True Then
                ' -> τίποτα

                ' Αν το Όνομα ΔΕΝ είναι κένο ->
            Else
                ' Αν το Όνομα και η Χονδρική έχουν κάποια τιμή ->
                If dgvTakenFrom.Rows(e.RowIndex).Cells(1).Value <> "" And
                                       dgvTakenFrom.Rows(e.RowIndex).Cells(4).Value > 0 Then
                    '///// MsgBox("Whole Row Updated!" & vbCrLf & "Name: " & dgvGivenTo.Rows(e.RowIndex).Cells(1).Value & ", Xondriki: " & dgvGivenTo.Rows(e.RowIndex).Cells(3).Value)
                    ' -> σώζει τις αλλαγές
                    ExchangesGivenOrTaken = "taken"
                    UpdateExchanges_Taken(e.RowIndex)

                End If
            End If
        End If
    End Sub


    Private Sub dgvPricesParadrugs_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPricesParadrugs.CellEnter

        If e.ColumnIndex = 2 Then
            If rbParadrugs.Checked = True Then
                CalculateNewLianikiSelectedDrug()
            ElseIf rbDrugs.Checked = True Then

            End If
        End If


    End Sub

    Private Function IsDrugQrCodeColumn(ByVal column As DataGridViewColumn) As Boolean
        If column Is Nothing Then Return False
        Return column.Name = "colDrugQRCode" OrElse column.HeaderText = "QRcode"
    End Function

    Private Function NormalizeQrCodeInput(ByVal input As String) As String
        Dim value As String = If(input, String.Empty).Trim()
        If value = "" Then Return ""

        If value.Length > 15 Then
            Dim extracted As String = GlobalFunctions.GetQRFromScannedCode(value)
            If Not String.IsNullOrWhiteSpace(extracted) Then
                Return extracted.Trim()
            End If
        End If

        Return value
    End Function

    Private Sub ApplyDrugQrCodeVisuals()
        If dgvPricesParadrugs Is Nothing OrElse dgvPricesParadrugs.Columns.Count = 0 Then Exit Sub
        If Not dgvPricesParadrugs.Columns.Contains("colDrugQRCode") Then Exit Sub
        If Not dgvPricesParadrugs.Columns.Contains("colDrugQRCodeCustom") Then Exit Sub

        For Each row As DataGridViewRow In dgvPricesParadrugs.Rows
            If row.IsNewRow Then Continue For

            Dim qrCell = row.Cells("colDrugQRCode")
            Dim flagCell = row.Cells("colDrugQRCodeCustom")
            Dim isCustom As Boolean = False

            If flagCell.Value IsNot Nothing AndAlso Not IsDBNull(flagCell.Value) Then
                Boolean.TryParse(flagCell.Value.ToString(), isCustom)
                If Not isCustom Then isCustom = (flagCell.Value.ToString() = "1")
            End If

            If isCustom Then
                qrCell.Style.BackColor = Color.LightGoldenrodYellow
                qrCell.Style.SelectionBackColor = Color.Goldenrod
                qrCell.Style.Font = New Font(dgvPricesParadrugs.Font, FontStyle.Bold)
                qrCell.ToolTipText = "Custom override από PharmacyCustomFiles"
            Else
                qrCell.Style.BackColor = dgvPricesParadrugs.DefaultCellStyle.BackColor
                qrCell.Style.SelectionBackColor = dgvPricesParadrugs.DefaultCellStyle.SelectionBackColor
                qrCell.Style.Font = New Font(dgvPricesParadrugs.Font, FontStyle.Regular)
                qrCell.ToolTipText = "QRcode από Pharmacy2013C"
            End If
        Next
    End Sub

    Private Sub dgvPricesParadrugs_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgvPricesParadrugs.CellBeginEdit
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        If rbDrugs.Checked AndAlso IsDrugQrCodeColumn(dgvPricesParadrugs.Columns(e.ColumnIndex)) Then
            Dim currentValue = dgvPricesParadrugs.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
            If currentValue Is Nothing OrElse IsDBNull(currentValue) Then
                _pricesCellOldValue = ""
            Else
                _pricesCellOldValue = currentValue.ToString()
            End If
            _pricesCellOldRow = e.RowIndex
            _pricesCellOldColumn = e.ColumnIndex
        End If
    End Sub

    Private Sub dgvPricesParadrugs_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPricesParadrugs.CellEndEdit
        If _suppressDrugQrCellEvents Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        If Not rbDrugs.Checked Then Exit Sub
        If Not IsDrugQrCodeColumn(dgvPricesParadrugs.Columns(e.ColumnIndex)) Then Exit Sub

        Dim row = dgvPricesParadrugs.Rows(e.RowIndex)
        Dim rawNewQr = row.Cells(e.ColumnIndex).Value
        Dim scannedValue As String = If(rawNewQr Is Nothing OrElse IsDBNull(rawNewQr), String.Empty, rawNewQr.ToString()).Trim()
        Dim newQr As String = NormalizeQrCodeInput(scannedValue)
        Dim oldQr As String = If(_pricesCellOldValue, String.Empty).Trim()

        If String.Equals(newQr, oldQr, StringComparison.Ordinal) Then Exit Sub

        Dim apId As Long
        Dim rawApCode = row.Cells(4).Value
        Dim rawDrugName = row.Cells(0).Value
        Dim apCode As String = If(rawApCode Is Nothing OrElse IsDBNull(rawApCode), String.Empty, rawApCode.ToString()).Trim()
        Dim drugName As String = If(rawDrugName Is Nothing OrElse IsDBNull(rawDrugName), String.Empty, rawDrugName.ToString()).Trim()

        If Not Long.TryParse(If(row.Cells(5).Value, String.Empty).ToString(), apId) OrElse apId <= 0 Then
            MessageBox.Show("Δεν βρέθηκε έγκυρο AP_ID για το φάρμακο.", "QRcode", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            _suppressDrugQrCellEvents = True
            row.Cells(e.ColumnIndex).Value = oldQr
            _suppressDrugQrCellEvents = False
            Exit Sub
        End If

        Dim prompt As String
        If newQr = "" Then
            prompt = "Να διαγραφεί το custom QRcode για το φάρμακο '" & drugName & "';" & vbCrLf &
                     "Αν συνεχίσεις, θα χρησιμοποιείται ξανά το QRcode της Pharmacy2013C."
        Else
            prompt = "Να αποθηκευτεί το νέο QRcode για το φάρμακο '" & drugName & "';" & vbCrLf &
                     oldQr & "  ->  " & newQr
            If scannedValue <> "" AndAlso scannedValue <> newQr Then
                prompt &= vbCrLf & vbCrLf & "Το scan μετατράπηκε αυτόματα από raw QR σε product code."
            End If
        End If

        If MessageBox.Show(prompt, "Επιβεβαίωση QRcode", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            _suppressDrugQrCellEvents = True
            row.Cells(e.ColumnIndex).Value = oldQr
            _suppressDrugQrCellEvents = False
            Exit Sub
        End If

        Try
            SaveDrugQrCodeOverride(apId, apCode, drugName, newQr)
        Catch ex As Exception
            _suppressDrugQrCellEvents = True
            row.Cells(e.ColumnIndex).Value = oldQr
            _suppressDrugQrCellEvents = False
            MessageBox.Show("Αποτυχία αποθήκευσης QRcode: " & ex.Message, "QRcode", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try

        Dim effectiveQr As String = If(GetEffectiveDrugQrCode(apId), String.Empty)
        Dim hasCustom As Boolean = HasDrugQrCodeOverride(apId)

        _suppressDrugQrCellEvents = True
        row.Cells(e.ColumnIndex).Value = effectiveQr
        If dgvPricesParadrugs.Columns.Contains("colDrugQRCodeCustom") Then
            row.Cells("colDrugQRCodeCustom").Value = If(hasCustom, 1, 0)
        End If
        _suppressDrugQrCellEvents = False

        _pricesCellOldValue = effectiveQr
        ApplyDrugQrCodeVisuals()
    End Sub


    Private Sub dgvPricesParadrugs_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPricesParadrugs.CellValueChanged
        If rbParadrugs.Checked = True Then
            If e.ColumnIndex = 0 Then
            ElseIf e.ColumnIndex = 1 Then
            ElseIf e.ColumnIndex = 2 Then
                Try
                    CalculateNewLianikiSelectedDrug()
                Catch ex As Exception
                End Try
            ElseIf e.ColumnIndex = 3 Then
            ElseIf e.ColumnIndex = 4 Then
            ElseIf e.ColumnIndex = 6 Then
            End If

            ParadrugCurrentRowChanged = True
            '' Αν το Όνομα είναι κενο ->
            'If IsDBNull(dgvPricesParadrugs.Rows(e.RowIndex).Cells(0).Value) = True Then
            '    ' -> τίποτα

            '    ' Αν το Όνομα ΔΕΝ είναι κένο ->
            'Else
            '    ' Αν το Όνομα και η ΛΙανική-Χονδρική έχουν κάποια τιμή ->
            '    If dgvPricesParadrugs.Rows(e.RowIndex).Cells(0).Value <> "" And _
            '        dgvPricesParadrugs.Rows(e.RowIndex).Cells(1).Value > 0 And _
            '        dgvPricesParadrugs.Rows(e.RowIndex).Cells(2).Value > 0 Then
            '        '///// MsgBox("Whole Row Updated!" & vbCrLf & "Name: " & dgvGivenTo.Rows(e.RowIndex).Cells(1).Value & ", Xondriki: " & dgvGivenTo.Rows(e.RowIndex).Cells(3).Value)

            '        ' -> σώζει τις αλλαγές (εκτός αν αλλάζουμε το Ap-Code)
            '        If e.ColumnIndex <> 4 And e.ColumnIndex <> 5 Then
            '            'MsgBox("Barcode = |" & dgvPricesParadrugs.Rows(e.RowIndex).Cells(7).Value)
            'UpdateParadrug(e.RowIndex)
            '        End If

            '    End If
            'End If
        ElseIf rbDrugs.Checked = True Then

        End If

    End Sub


    'Private Sub dgvPricesParadrugs_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPricesParadrugs.CellValueChanged
    '    If rbParadrugs.Checked = True Then
    '        If e.ColumnIndex = 0 Then

    '        ElseIf e.ColumnIndex = 1 Then

    '        ElseIf e.ColumnIndex = 2 Then
    '            Try
    '                CalculateNewLianikiSelectedDrug()
    '            Catch ex As Exception
    '            End Try

    '        End If

    '        ' Αν το Όνομα είναι κενο ->
    '        If IsDBNull(dgvPricesParadrugs.Rows(e.RowIndex).Cells(0).Value) = True Then
    '            ' -> τίποτα

    '            ' Αν το Όνομα ΔΕΝ είναι κένο ->
    '        Else
    '            ' Αν το Όνομα και η ΛΙανική-Χονδρική έχουν κάποια τιμή ->
    '            If dgvPricesParadrugs.Rows(e.RowIndex).Cells(0).Value <> "" And _
    '                dgvPricesParadrugs.Rows(e.RowIndex).Cells(1).Value > 0 And _
    '                dgvPricesParadrugs.Rows(e.RowIndex).Cells(2).Value > 0 Then
    '                '///// MsgBox("Whole Row Updated!" & vbCrLf & "Name: " & dgvGivenTo.Rows(e.RowIndex).Cells(1).Value & ", Xondriki: " & dgvGivenTo.Rows(e.RowIndex).Cells(3).Value)

    '                ' -> σώζει τις αλλαγές (εκτός αν αλλάζουμε το Ap-Code)
    '                If e.ColumnIndex <> 4 And e.ColumnIndex <> 5 Then
    '                    'MsgBox("Barcode = |" & dgvPricesParadrugs.Rows(e.RowIndex).Cells(7).Value)
    '                    UpdateParadrug(e.RowIndex)
    '                End If

    '            End If
    '        End If
    '    ElseIf rbDrugs.Checked = True Then

    '    End If

    'End Sub

    Private Sub dgvPricesParadrugs_DefaultValuesNeeded(sender As Object, e As DataGridViewRowEventArgs) Handles dgvPricesParadrugs.DefaultValuesNeeded

        If rbParadrugs.Checked = True Then
            'Σώνει την θέση στο Datagrid του νέου row
            rowIndex = e.Row.Index

            'Ορίζει τις default τιμές για τα νέα Row του Datagrid
            With e.Row
                .Cells(1).Value = "0" ' Xondriki
                .Cells(2).Value = "0" ' LIaniki
                .Cells(4).Value = 0 ' AP_Code
            End With
        ElseIf rbDrugs.Checked = True Then

        End If

    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs)
        Dim index As Integer = dgvPricesParadrugs.CurrentRow.Index
        MsgBox(dgvPricesParadrugs.Rows(index).Cells(0).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(2).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(3).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(4).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(5).Value & "-")
        MsgBox(dgvPricesParadrugs.Rows(3).Cells(5).Value)
    End Sub

    Private Sub Button3_Click_2(sender As Object, e As EventArgs) Handles Button3.Click
        txtSearchPricesParadrugs.Text = ""
        txtSearchPricesParadrugs.Focus()

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        txtSearchPhones.Text = ""
        txtSearchPhones.Focus()
    End Sub

    Private Sub cboFPA_Paradrugs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFPA_Paradrugs.SelectedIndexChanged
        Try
            CalculateNewLianikiSelectedDrug()
        Catch ex As Exception
        End Try

    End Sub

    Private Sub txtProfit_Paradrugs_TextChanged(sender As Object, e As EventArgs) Handles txtProfit_Paradrugs.TextChanged
        CalculateNewLianikiSelectedDrug()
    End Sub


    Private Sub btnClearSearch_Click(sender As Object, e As EventArgs) Handles btnClearSearch.Click
        txtSearchCustomer.Text = ""
        txtSearchCustomer.Focus()

    End Sub


    Private Function CalculatePreviousTotalBalance() As Decimal
        Dim sql As String
        Dim Total, TotalGiven, TotalTaken As Decimal
        Dim FromDate, ToDate As String

        FromDate = dtpFromDate.Value
        ToDate = dtpToDate.Value

        sql = "SELECT Id, DrugName, Qnt, Xondr,  RP, AP_Code, MyDate From PharmacyCustomFiles.dbo.Exchanges " &
                                    "WHERE Exch ='" & cbExchangers.Text & "' AND FromTo=0 AND " &
                                        "Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') >0 "

        TotalGiven = CalculateSums(sql, "Xondr")
        lblPreviousBalanceGivenTo.Text = Math.Abs(TotalGiven).ToString("###,###.00 €")
        lblPreviousBalanceGivenTo.ForeColor = Color.Green


        sql = "SELECT Id, DrugName, Qnt, Xondr,  RP, AP_Code, MyDate From PharmacyCustomFiles.dbo.Exchanges " &
                                   "WHERE Exch ='" & cbExchangers.Text & "' AND FromTo=1 AND " &
                                        "Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') >0 "

        TotalTaken = CalculateSums(sql, "Xondr")
        lblPreviousBalanceTakenFrom.Text = Math.Abs(TotalTaken).ToString("###,###.00 €")
        lblPreviousBalanceTakenFrom.ForeColor = Color.Red


        Total = TotalGiven - TotalTaken
        lblPreviousBalance.Text = Math.Abs(Total).ToString("###,###.00")

        With lblPreviousBalance
            Select Case Total
                Case Is > 0
                    .ForeColor = Color.Green
                Case Is < 0
                    .ForeColor = Color.Red
                Case Is = 0
                    .ForeColor = Color.Black
            End Select
        End With
        Return Total

    End Function


    Private Sub cboPhoneCatalog_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPhoneCatalog.SelectedIndexChanged
        GetPhonesList()
        'If cboPhoneCatalog.Text = "ΙΑΤΡΟΙ" Then
        '    dgvPhones.ScrollBars = ScrollBars.Both
        'Else
        '    dgvPhones.ScrollBars = ScrollBars.Vertical
        'End If
    End Sub


    ' --- Όταν αλλάξει το κελί, ξεκινά ο timer (interval = 1000 ms τώρα) ---
    Private Sub dgvDebtsList_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) _
    Handles dgvDebtsList.CellValueChanged

        If _suppressCellValueChanged Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        ' Τρέχουμε debounce ΜΟΝΟ για τη στήλη "Περιγραφή"
        If dgvDebtsList.Columns(e.ColumnIndex).HeaderText = "Περιγραφή" Then
            _pendingRow = e.RowIndex
            _pendingCol = e.ColumnIndex
            _lastObservedValue = Nothing
            _stableCount = 0

            _suppressDebtsRowLeave = True   ' ← ΜΗΝ κάνεις save στο RowLeave όσο περιμένουμε το lookup

            tmrCellLookup.Interval = 30
            tmrCellLookup.Start()
        End If
    End Sub


    ' --- Αν χρησιμοποιείς edit κατά κελί, σιγουρέψου ότι το CellValueChanged θα “πυροδοτείται”: ---
    Private Sub dgvDebtsList_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) _
    Handles dgvDebtsList.CurrentCellDirtyStateChanged

        If dgvDebtsList.IsCurrentCellDirty Then
            dgvDebtsList.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub tmrCellLookup_Tick(sender As Object, e As EventArgs) Handles tmrCellLookup.Tick
        If _pendingRow < 0 OrElse _pendingCol < 0 Then Exit Sub

        ' --- Inline state για «περιμένω λιανική» ---
        Static waitingForRetail As Boolean = False
        Static pendingRecipeValue As String = Nothing

        Dim cell = dgvDebtsList.Rows(_pendingRow).Cells(_pendingCol)
        Dim currentValue As String = If(cell.Value, String.Empty).ToString().Trim()

        ' === Αν περιμένω να ολοκληρωθεί η Λιανική από προηγούμενη προσπάθεια ===
        If waitingForRetail Then
            Dim priceCol As Integer = Math.Max(0, _pendingCol - 1)
            Dim priceCell = dgvDebtsList.Rows(_pendingRow).Cells(priceCol)
            If priceCell Is Nothing Then
                waitingForRetail = False
                pendingRecipeValue = Nothing
                _suppressDebtsRowLeave = False
                Return
            End If

            ' ΜΗΝ αφήσεις RowLeave να σώσει ενδιάμεσα
            _suppressDebtsRowLeave = True

            ' Αν ο χρήστης ακόμη πληκτρολογεί, μείνε στη Λιανική
            If priceCell.IsInEditMode Then
                Try
                    dgvDebtsList.CurrentCell = priceCell
                    dgvDebtsList.BeginEdit(True)
                Catch
                End Try
                Return
            End If

            ' Έχει ολοκληρωθεί το edit → πάρε την committed τιμή
            Dim rawRetail As Object = priceCell.Value
            Dim retailValue As Decimal, hasRetail As Boolean = False
            If rawRetail IsNot Nothing Then
                Dim s = rawRetail.ToString().Trim()
                If s.Length > 0 Then
                    hasRetail = Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.CurrentCulture, retailValue)
                    If Not hasRetail Then hasRetail = Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, retailValue)
                    If hasRetail AndAlso retailValue < 0D Then hasRetail = False
                End If
            End If

            If hasRetail AndAlso Not String.IsNullOrEmpty(pendingRecipeValue) Then
                tmrCellLookup.Stop()
                _suppressCellValueChanged = True
                cell.Value = $"Συνταγή ({pendingRecipeValue})"
                SaveDebtRow(_pendingRow)
                _suppressCellValueChanged = False
                _enterPressedThisEdit = False
                waitingForRetail = False
                pendingRecipeValue = Nothing
                _suppressDebtsRowLeave = False ' <-- επιτέλους αφήνουμε ξανά RowLeave
                Exit Sub
            Else
                ' Ακόμη άκυρη τιμή → μείνε στη Λιανική
                Try
                    dgvDebtsList.CurrentCell = priceCell
                    dgvDebtsList.BeginEdit(True)
                Catch
                End Try
                Return
            End If
        End If
        ' --------------------------------------------------------------------

        ' --- Debounce σταθερότητας τιμής ---
        If _lastObservedValue Is Nothing Then
            _lastObservedValue = currentValue
            _stableCount = 1
            Exit Sub
        End If

        If currentValue = _lastObservedValue Then
            _stableCount += 1
        Else
            _lastObservedValue = currentValue
            _stableCount = 1
        End If

        If _stableCount < STABLE_TICKS Then Exit Sub

        tmrCellLookup.Stop()

        ' --- Scanner vs Keyboard + απαίτηση Enter ---
        Dim elapsed As Long = If(_editSw IsNot Nothing, _editSw.ElapsedMilliseconds, Long.MaxValue)
        Dim isScannerFast As Boolean = (elapsed <= MAX_EDIT_MS_FOR_SCANNER)

        If Not (isScannerFast OrElse _enterPressedThisEdit) Then
            lblScanHint.Text = "Πληκτρολόγηση: πάτα Enter για εισαγωγή"
            lblScanHint.Visible = True
            _suppressDebtsRowLeave = False
            Return
        End If

        lblScanHint.Visible = False

        ' --- Heuristics περιεχομένου ---
        Dim isBarcode As Boolean = currentValue.All(AddressOf Char.IsDigit)
        Dim isQRCodeCandidate As Boolean = (Not HasWhitespace(currentValue)) ' QR: όχι κενά

        If currentValue.Length < 12 OrElse (Not isBarcode AndAlso Not isQRCodeCandidate) Then
            _suppressDebtsRowLeave = False
            Return
        End If

        ' === Lookup QR/Barcode ===
        Dim fullDesc As String = Nothing
        If currentValue.Length > 15 Then
            Dim qr = currentValue.Substring(2, 14)
            fullDesc = LookupFullDescriptionByQRCode(qr)
        Else
            fullDesc = LookupFullDescriptionByBarcode(currentValue)
        End If

        If fullDesc IsNot Nothing Then
            _suppressCellValueChanged = True

            cell.Value = fullDesc

            ' λιανική από scan (αν υπάρχει)
            Dim priceCol As Integer = Math.Max(0, _pendingCol - 1)
            Dim rawScanned As String = If(_lastObservedValue, "")
            Dim retail As Decimal? = GetRetailFromScanned(rawScanned)

            If retail.HasValue Then
                dgvDebtsList.Rows(_pendingRow).Cells(priceCol).Style.Format = "0.00"
                dgvDebtsList.Rows(_pendingRow).Cells(priceCol).Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                dgvDebtsList.Rows(_pendingRow).Cells(priceCol).Value = retail.Value
            Else
                dgvDebtsList.Rows(_pendingRow).Cells(priceCol).Value = Nothing
            End If

            SaveDebtRow(_pendingRow)

            _suppressCellValueChanged = False
            _enterPressedThisEdit = False
            _suppressDebtsRowLeave = False
            Exit Sub
        End If

        ' === ΜΟΝΟ ΑΡΙΘΜΟΙ → "Συνταγή (...)" με απαίτηση προ-συμπληρωμένης Λιανικής ===
        If currentValue.All(AddressOf Char.IsDigit) Then
            Dim priceCol As Integer = Math.Max(0, _pendingCol - 1)
            Dim priceCell = dgvDebtsList.Rows(_pendingRow).Cells(priceCol)

            ' Από εδώ και μέχρι να τελειώσει η διαδικασία, ΜΗΝ αφήνεις RowLeave να σώσει κάτι
            _suppressDebtsRowLeave = True

            ' Αν η Λιανική είναι σε edit, περίμενε να τελειώσει
            If priceCell IsNot Nothing AndAlso priceCell.IsInEditMode Then
                Try
                    dgvDebtsList.CurrentCell = priceCell
                    dgvDebtsList.BeginEdit(True)
                Catch
                End Try
                waitingForRetail = True
                pendingRecipeValue = currentValue
                tmrCellLookup.Start()
                _enterPressedThisEdit = False
                Return
            End If

            ' Διαβάζουμε ΜΟΝΟ committed τιμή (Value), όχι EditedFormattedValue
            Dim rawRetail As Object = If(priceCell IsNot Nothing, priceCell.Value, Nothing)
            Dim retailValue As Decimal, hasRetail As Boolean = False
            If rawRetail IsNot Nothing Then
                Dim s = rawRetail.ToString().Trim()
                If s.Length > 0 Then
                    hasRetail = Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.CurrentCulture, retailValue)
                    If Not hasRetail Then hasRetail = Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, retailValue)
                    If hasRetail AndAlso retailValue < 0D Then hasRetail = False
                End If
            End If

            If Not hasRetail Then
                MessageBox.Show("Συμπλήρωσε πρώτα Λιανική τιμή.", "Έλλειψη Λιανικής", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Try
                    dgvDebtsList.CurrentCell = priceCell
                    dgvDebtsList.BeginEdit(True)
                Catch
                End Try
                waitingForRetail = True
                pendingRecipeValue = currentValue
                tmrCellLookup.Start()
                _enterPressedThisEdit = False
                Return
            End If

            ' Έχουμε committed λιανική → προχώρα μία και καλή
            _suppressCellValueChanged = True
            cell.Value = $"Συνταγή ({currentValue})"
            SaveDebtRow(_pendingRow)
            _suppressCellValueChanged = False
            _enterPressedThisEdit = False
            _suppressDebtsRowLeave = False ' <-- επαναφορά μετά το οριστικό save
            Exit Sub
        End If

        _suppressDebtsRowLeave = False
    End Sub








    ' ΕΝΙΑΙΑ ρουτίνα καταχώρησης της τρέχουσας γραμμής χρεών
    Private Sub SaveDebtRow(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= dgvDebtsList.Rows.Count Then Exit Sub

        If String.IsNullOrWhiteSpace(CStr(dgvDebtsList.Rows(rowIndex).Cells(2).EditedFormattedValue)) _
   AndAlso String.IsNullOrWhiteSpace(CStr(dgvDebtsList.Rows(rowIndex).Cells(1).EditedFormattedValue)) _
   AndAlso String.IsNullOrWhiteSpace(CStr(dgvDebtsList.Rows(rowIndex).Cells(0).EditedFormattedValue)) Then
            Exit Sub
        End If

        ' Ολοκλήρωσε edits σε κελί/row/binding
        dgvDebtsList.EndEdit()
        Try
            If bsDebts IsNot Nothing Then bsDebts.EndEdit()
        Catch
        End Try

        ' Αποθήκευση χωρίς έλεγχο IsCurrentRowDirty — το UpdateDebts
        ' θα τρέξει μόνο αν όντως υπάρχει αλλαγή/νέα εγγραφή.
        UpdateDebts(rowIndex, _pendingCol)
        DisplaySums_Debts()
        DisplayTotalDebtPerCustomer()
    End Sub



    ' --- Λιανική τιμή από QR-ProductCode ---
    Private Function LookupRetailByQRCode(productCode As String) As Decimal?
        EnsureDrugQrCodeOverridesTable()

        Const sql As String =
        "SELECT TOP (1) A.AP_TIMH_LIAN " &
        "FROM dbo.APOTIKH AS A " &
        "LEFT JOIN dbo.APOTIKH_QRCODES AS Q ON Q.APQ_AP_ID = A.AP_ID " &
        "LEFT JOIN PharmacyCustomFiles.dbo.DrugQrCodeOverrides AS QO ON QO.AP_ID = A.AP_ID " &
        "WHERE ISNULL(NULLIF(QO.QRCode, ''), Q.APQ_PRODUCT_CODE) = @code " &
        "ORDER BY A.AP_ID;"
        Using con As New SqlConnection(connectionstring)
            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@code", productCode)
                con.Open()
                Dim obj = cmd.ExecuteScalar()
                If obj Is DBNull.Value OrElse obj Is Nothing Then Return Nothing
                Return Convert.ToDecimal(obj)
            End Using
        End Using
    End Function

    ' --- Λιανική τιμή από barcode ---
    Private Function LookupRetailByBarcode(barcode As String) As Decimal?
        Const sql As String =
        "SELECT TOP (1) A.AP_TIMH_LIAN " &
        "FROM dbo.APOTIKH AS A " &
        "INNER JOIN dbo.APOTIKH_BARCODES AS B ON B.BRAP_AP_ID = A.AP_ID " &
        "WHERE B.BRAP_AP_BARCODE = @barcode " &
        "ORDER BY A.AP_ID;"
        Using con As New SqlConnection(connectionstring)
            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@barcode", barcode)
                con.Open()
                Dim obj = cmd.ExecuteScalar()
                If obj Is DBNull.Value OrElse obj Is Nothing Then Return Nothing
                Return Convert.ToDecimal(obj)
            End Using
        End Using
    End Function

    ' --- Wrapper: δέχεται το raw σκανάρισμα (όπως ήδη κάνεις στο tmrCellLookup_Tick) ---
    Private Function GetRetailFromScanned(scanned As String) As Decimal?
        If String.IsNullOrWhiteSpace(scanned) Then Return Nothing
        scanned = scanned.Trim()

        ' ίδιο heuristic με της Περιγραφής: QR αν είναι “μεγάλο” string
        If scanned.Length > 15 Then
            ' Στο δικό σου code κάνεις Substring(2,14) για το QR product code (GS1).
            ' Χρησιμοποιούμε ακριβώς το ίδιο για συνέπεια.
            Dim qrProductCode As String
            If scanned.Length >= 16 Then
                qrProductCode = scanned.Substring(2, 14)
            Else
                Return Nothing
            End If
            Return LookupRetailByQRCode(qrProductCode)
        Else
            ' barcode (π.χ. ΕΑΝ-13)
            Return LookupRetailByBarcode(scanned)
        End If
    End Function


    ' --- Το SQL lookup: AP_DESCRIPTION + AP_MORFI από APOTIKH, via APOTIKH_QRCODES ---
    Private Function LookupFullDescriptionByQRCode(productCode As String) As String
        ' Γύρισε Nothing αν δεν βρεθεί ΑΚΡΙΒΩΣ μία εγγραφή
        EnsureDrugQrCodeOverridesTable()

        Const sql As String =
        "SELECT TOP (2) ISNULL(A.AP_DESCRIPTION,'') + ' ' + ISNULL(A.AP_MORFI,'') AS FullDescription " &
        "FROM dbo.APOTIKH A " &
        "LEFT JOIN dbo.APOTIKH_QRCODES Q ON A.AP_ID = Q.APQ_AP_ID " &
        "LEFT JOIN PharmacyCustomFiles.dbo.DrugQrCodeOverrides AS QO ON QO.AP_ID = A.AP_ID " &
        "WHERE ISNULL(NULLIF(QO.QRCode, ''), Q.APQ_PRODUCT_CODE) = @code;"

        Try
            Using con As New SqlClient.SqlConnection(connectionstring)
                Using cmd As New SqlClient.SqlCommand(sql, con)
                    cmd.Parameters.Add("@code", SqlDbType.NVarChar, 100).Value = productCode
                    con.Open()
                    Using rdr = cmd.ExecuteReader()
                        Dim results As New List(Of String)
                        While rdr.Read()
                            results.Add(rdr.GetString(0))
                        End While
                        ' Αν βρέθηκε ακριβώς 1 εγγραφή, επιστρέφει το string, αλλιώς Nothing
                        If results.Count = 1 Then
                            Return results(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch
            ' Σε σφάλμα DB κράτα την παλιά τιμή (επιστρέφουμε Nothing)
            Return Nothing
        End Try
    End Function

    ' --- Το SQL lookup: AP_DESCRIPTION + AP_MORFI από APOTIKH, via APOTIKH_QRCODES ---
    Private Function LookupFullDescriptionByBarcode(productCode As String) As String
        ' Γύρισε Nothing αν δεν βρεθεί ΑΚΡΙΒΩΣ μία εγγραφή
        Const sql As String =
        "SELECT TOP (2) ISNULL(A.AP_DESCRIPTION,'') + ' ' + ISNULL(A.AP_MORFI,'') AS FullDescription " &
        "FROM dbo.APOTIKH_BARCODES Q " &
        "JOIN dbo.APOTIKH A ON A.AP_ID = Q.BRAP_AP_ID " &
        "WHERE Q.BRAP_AP_BARCODE = @code;"

        Try
            Using con As New SqlClient.SqlConnection(connectionstring)
                Using cmd As New SqlClient.SqlCommand(sql, con)
                    cmd.Parameters.Add("@code", SqlDbType.NVarChar, 100).Value = productCode
                    con.Open()
                    Using rdr = cmd.ExecuteReader()
                        Dim results As New List(Of String)
                        While rdr.Read()
                            results.Add(rdr.GetString(0))
                        End While
                        ' Αν βρέθηκε ακριβώς 1 εγγραφή, επιστρέφει το string, αλλιώς Nothing
                        If results.Count = 1 Then
                            Return results(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch
            ' Σε σφάλμα DB κράτα την παλιά τιμή (επιστρέφουμε Nothing)
            Return Nothing
        End Try
    End Function



    Private Sub UpdatePharmacy2013BasedFarmnet()
        If My.Computer.Name = "DESKTOP-T7HMABG" Or My.Computer.Name = "LAPTOP-4AJPEK4U" Or My.Computer.Name = "CRAZYDR" Then

            'MsgBox("Έχετε κλείσει το CSA Farmakon και από τα 2 κομπιούτερ ;", MsgBoxStyle.Exclamation, "Updating " & txtDB1.Text)
            MsgBox("Δεν χρειάζεται να κλείσετε το Pharmakon και το προγραμμα του φαρμακείου ΑΛΛΑ μην τα χρησιμοποιήσετε κατά την διάρκεια του Updating !", MsgBoxStyle.Exclamation, "Updating " & txtDB1.Text)

            lstMessage.Items.Clear()
            lstIndex = -1

            UpdatePharmacy2025()
        Else
            MsgBox("Αυτή η διαδικασία γίνεται μόνο στο Φαρμακείο, από το κεντρικό pc!", MsgBoxStyle.Critical, "Λάθος...")
        End If
    End Sub


    Private Sub btnUpdatePharmacy2013C_Click(sender As Object, e As EventArgs) Handles btnUpdatePharmacy2013C.Click

        If Not IsProcessElevated() Then
            If MessageBox.Show("Απαιτούνται δικαιώματα διαχειριστή. Συνέχεια;", "UAC", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                RelaunchAsAdmin()
                Return
            End If
        Else
            UpdatePharmacy2013BasedFarmnet()
        End If


    End Sub

    Private Sub rbEverything_CheckedChanged(sender As Object, e As EventArgs) Handles rbEverything.CheckedChanged
        If rbPC2Usb.Checked = True Then
            btnBackupRestore.Text = "Backup Everything"
        ElseIf rbUsb2PC.Checked = True Then
            btnBackupRestore.Text = "Restore Everything"
        End If

    End Sub

    Private Sub rbOnlyDatabases_CheckedChanged(sender As Object, e As EventArgs) Handles rbOnlyDatabases.CheckedChanged
        If rbPC2Usb.Checked = True Then
            btnBackupRestore.Text = "Backup Only Databases"
        ElseIf rbUsb2PC.Checked = True Then
            btnBackupRestore.Text = "Restore Only Databases"
        End If

    End Sub

    Private Sub rbOnlyVisualBasic_CheckedChanged(sender As Object, e As EventArgs) Handles rbOnlyVisualBasic.CheckedChanged
        If rbPC2Usb.Checked = True Then
            btnBackupRestore.Text = "Backup Only Visual Basic Files"
        ElseIf rbUsb2PC.Checked = True Then
            btnBackupRestore.Text = "Restore Only Visual Basic Files"
        End If

    End Sub

    Private Sub btnBackupRestore_Click(sender As Object, e As EventArgs) Handles btnBackupRestore.Click
        lstMessage.Items.Clear()
        lstIndex = -1

        If rbPC2Usb.Checked = True Then
            If rbEverything.Checked = True Then
                If MessageBox.Show("Θέλετε να κάνετε BACKUP όλων των αρχείων ?", "Backup Everything", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    BackupDatabaseFiles()
                    BackupVisualBasicFiles()
                End If
            ElseIf rbOnlyDatabases.Checked = True Then
                If MessageBox.Show("Θέλετε να κάνετε BACKUP μόνο των Database ?", "Backup Only Databases", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    BackupDatabaseFiles()
                End If
            ElseIf rbOnlyVisualBasic.Checked = True Then
                If MessageBox.Show("Θέλετε να κάνετε BACKUP μόνο των αρχείων Visual Basic ?", "Backup Only Visual Basic Files", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    BackupVisualBasicFiles()
                End If
            End If


        ElseIf rbUsb2PC.Checked = True Then
            If rbEverything.Checked = True Then
                If MessageBox.Show("Θέλετε να κάνετε RESTORE όλων των αρχείων ?", "Restore Everything", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    RestoreDatabaseFiles()
                    RestoreVisualBasicFiles()
                End If
            ElseIf rbOnlyDatabases.Checked = True Then
                If MessageBox.Show("Θέλετε να κάνετε RESTORE μόνο των Database ?", "Restore Only Databases", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    RestoreDatabaseFiles()
                End If
            ElseIf rbOnlyVisualBasic.Checked = True Then
                If MessageBox.Show("Θέλετε να κάνετε RESTORE μόνο των αρχείων Visual Basic ?", "Restore Only Visual Basic Files", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    RestoreVisualBasicFiles()
                End If
            End If

        End If

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub


    Private Sub dgvDrugsOnLoan_DefaultValuesNeeded(sender As Object, e As DataGridViewRowEventArgs) Handles dgvDrugsOnLoan.DefaultValuesNeeded

        'Ορίζει default τιμές 
        With e.Row
            .Cells(0).Value = Today() ' Date
            .Cells(2).Value = 0 ' Ποσό

        End With
    End Sub



    Private Sub dgvDrugsOnLoan_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDrugsOnLoan.CellValueChanged
        UpdateDrugsOnLoan(e.RowIndex, e.ColumnIndex)
        DisplaySums_Debts()
        DisplayTotalDebtPerCustomer()

    End Sub

    Private Sub btnDeleteDrugOnLoan_Click(sender As Object, e As EventArgs) Handles btnDeleteDrugOnLoan.Click
        DeleteSelectedDrugsOnLoan()
        GetDrugsOnLoanList()
        DisplayLabelIfCustomerWithoutDebtsOrHairdies()
        ' Ανανεώνει τη λίστα των πελατων, ξαναεπιλέγοντας τον τελευταίο πελάτη
        Dim SelectedCustomer As String = dgvCustomers.SelectedRows(0).Cells(0).Value
        GetCustomersList()

        Dim rowIndex As Integer = SearchDatagrid(dgvCustomers, SelectedCustomer)
        dgvCustomers.CurrentCell = dgvCustomers.Rows(rowIndex).Cells(0)
        DisplayTotalDebtPerCustomer()

    End Sub

    Private Sub lblCustWithoutDrugsOnLoan_Click(sender As Object, e As EventArgs) Handles lblCustWithoutDrugsOnLoan.Click
        ActivateDatagridDrugsOnLoan(True)
    End Sub

    Private Sub cboSearchCustomers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSearchCustomers.SelectedIndexChanged
        GetCustomersList()
    End Sub

    Private Sub dgvCustomers_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles dgvCustomers.DataError
        rtxtCustomersMessage.Text = "DB Error!"
    End Sub

    Private Sub dgvCustomers_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCustomers.RowLeave
        dirty = dgvCustomers.IsCurrentRowDirty

        If dirty Then
            ' Τρέχουμε μόνο όταν αφήνουμε τη γραμμή έχοντας αλλάξει τη στήλη 0 (Όνομα)
            If e.ColumnIndex = 0 Then
                Dim ans = MessageBox.Show("Θέλετε να αποθηκεύσετε τις αλλαγές στο όνομα του πελάτη;", "Επιβεβαίωση", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If ans = DialogResult.Yes Then
                    _confirmingFromRowLeave = True
                    Try
                        UpdateCustomer3(e.RowIndex)
                    Finally
                        _confirmingFromRowLeave = False
                    End Try
                Else
                    ' Ακύρωση αλλαγών στη γραμμή (revert)
                    dgvCustomers.CancelEdit()
                    lblNewRow_Cust.Text = ""
                End If
            End If
        Else
            lblNewRow_Cust.Text = ""
        End If
    End Sub



    Private Sub tmrSel_Tick(sender As Object, e As EventArgs) Handles tmrSel.Tick
        tmrSel.Stop()

        ' Αν άλλαξε ξανά η επιλογή από τότε, περίμενε τον επόμενο κύκλο
        Dim curId = GetSelectedCustomerId()
        If curId <> _pendingCustomerId OrElse curId < 0 Then Exit Sub

        ' Τώρα τρέχουμε ΜΙΑ φορά τις βαριές ρουτίνες
        _forceLastAfterBind = True
        GetDebtsAndHairDiesList()
        GetDrugsOnLoanList()
        GetPrescriptionsList()
        DisplayLabelIfCustomerWithoutDebtsOrHairdies()
    End Sub


    Private Sub dgvCustomers_SelectionChanged(sender As Object, e As EventArgs) Handles dgvCustomers.SelectionChanged
        If _suppressSelectionChanged OrElse _isRebinding Then Return  ' guard για rebind

        _pendingCustomerId = GetSelectedCustomerId()
        If _pendingCustomerId < 0 Then Exit Sub

        ' restart debounce timer
        tmrSel.Stop()
        tmrSel.Interval = 150     ' 200–300ms
        tmrSel.Start()
    End Sub


    Private Const CUSTOMER_ID_COL As Integer = 1  ' <-- άλλαξέ το αν χρειάζεται

    Private Function GetSelectedCustomerId() As Integer
        If dgvCustomers.CurrentRow Is Nothing Then Return -1
        Dim v = dgvCustomers.CurrentRow.Cells(CUSTOMER_ID_COL).Value
        Dim id As Integer
        If v Is Nothing OrElse Not Integer.TryParse(v.ToString(), id) Then Return -1
        Return id
    End Function

    Private Sub RestoreSelectionById(id As Integer)
        If id < 0 Then Exit Sub
        For Each r As DataGridViewRow In dgvCustomers.Rows
            Dim v = r.Cells(CUSTOMER_ID_COL).Value
            Dim cur As Integer
            If v IsNot Nothing AndAlso Integer.TryParse(v.ToString(), cur) AndAlso cur = id Then
                dgvCustomers.CurrentCell = r.Cells(Math.Min(0, r.Cells.Count - 1))
                Exit Sub
            End If
        Next
    End Sub


    Private Sub dgvHairdiesList_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvHairdiesList.CellValueChanged
        UpdateHairDies(e.RowIndex)
    End Sub

    Private Sub tmrSearchCustomers_Tick(sender As Object, e As EventArgs) Handles tmrSearchCustomers.Tick
        tmrSearchCustomers.Stop()

        ' Αν από τότε που άναψε άλλαξε ξανά το κείμενο, αγνόησέ το
        If _debounceSnap <> txtSearchCustomer.Text Then Exit Sub

        ' (προαιρετικό) κράτα τον επιλεγμένο πελάτη για να τον ξαναεπιλέξεις μετά
        Dim prevId As Integer = GetSelectedCustomerId()  ' helper πιο κάτω

        _isRebinding = True
        _suppressSelectionChanged = True
        Try
            GetCustomersList()           ' Κάνει το rebind του grid
        Finally
            _suppressSelectionChanged = False
            _isRebinding = False
        End Try

        ' (προαιρετικό) επανάφερε επιλογή με βάση Id
        RestoreSelectionById(prevId)     ' helper πιο κάτω
    End Sub




    Private Sub txtSearchCustomer_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearchCustomer.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            tmrSearchCustomers.Stop()
            tmrSearchCustomers.Tag = txtSearchCustomer.Text
            tmrSearchCustomers_Tick(tmrSearchCustomers, EventArgs.Empty)
        End If
    End Sub




    Private Sub dgvPrescriptions_DefaultValuesNeeded(sender As Object, e As DataGridViewRowEventArgs) Handles dgvPrescriptions.DefaultValuesNeeded
        ''Ορίζει default τιμές 
        'With e.Row
        '    .Cells(1).Value = Today() ' InitDate
        '    Try
        '        .Cells(5).Value = dgvCustomers.SelectedRows(0).Cells(1).Value 'CustomerId
        '    Catch ex As Exception
        '    End Try


        'End With
    End Sub

    Private Sub dgvPrescriptions_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPrescriptions.CellClick
        If e.ColumnIndex = 0 Then
            frmPrescriptionInfo.Show()
            PrescriptionInfoMode = "insert"
            Me.Enabled = False
        End If
    End Sub

    Private Sub btnDeletePrescriptions_Click(sender As Object, e As EventArgs) Handles btnDeletePrescriptions.Click
        DeleteSelectedPrescriptions()
        GetPrescriptionsList()
        DisplayLabelIfCustomerWithoutDebtsOrHairdies()
        ' Ανανεώνει τη λίστα των πελατων, ξαναεπιλέγοντας τον τελευταίο πελάτη
        Dim SelectedCustomer As String = dgvCustomers.SelectedRows(0).Cells(0).Value
        GetCustomersList()
        Dim rowIndex As Integer = SearchDatagrid(dgvCustomers, SelectedCustomer)
        dgvCustomers.CurrentCell = dgvCustomers.Rows(rowIndex).Cells(0)
    End Sub





    Private Sub lblCustWithPrescriptions_Click(sender As Object, e As EventArgs) Handles lblCustWithPrescriptions.Click
        ActivateDatagridPrescriptions(True)
    End Sub


    Private Sub DisplayTotalDebtPerCustomer()
        Dim debtsMoney As Decimal = 0
        Dim debtsDrugs As Decimal = 0

        Try
            debtsDrugs = CType(lblSumDrugsOnLoan.Text, Decimal)
        Catch ex As Exception
            debtsDrugs = 0
        End Try

        Try
            debtsMoney = CType(lblTotalCustomerDebt.Text, Decimal)
        Catch ex As Exception
            debtsMoney = 0
        End Try

        If debtsDrugs > 0 And debtsMoney > 0 Then
            lblTotalDebtPerCustomer.Visible = True
            lblTotalDebtPerCustomer.Text = (debtsDrugs + debtsMoney).ToString("c")
        Else
            lblTotalDebtPerCustomer.Visible = False
        End If
    End Sub

    Private Sub lblSumDrugsOnLoan_TextChanged(sender As Object, e As EventArgs) Handles lblSumDrugsOnLoan.TextChanged
        DisplayTotalDebtPerCustomer()
    End Sub

    Private Sub lblSumDrugsOnLoan_Click(sender As Object, e As EventArgs) Handles lblSumDrugsOnLoan.Click
        DisplayTotalDebtPerCustomer()
    End Sub


    Private Sub chkSelectAll_CheckedChanged(sender As Object, e As EventArgs) Handles chkSelectAll.CheckedChanged
        GetPrescriptionsList()
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Enabled = False

        ' Ανοίγει το form για να καταχωρήσουμε barcode
        frmPrescriptionsExpiring.Show()
    End Sub

    Private Sub btnExpirations_Click(sender As Object, e As EventArgs) Handles btnExpirations.Click
        OpenExpirationPairingForm()
    End Sub



    Private Sub txtNoExpirations_Click1(sender As Object, e As EventArgs) Handles txtNoExpirations.Click
        HideExpirationDatagrid(False)
    End Sub


    Private Sub DeleteExpiration()
        Dim insertData As String = ""
        Dim id As Integer = 0

        Try
            id = dgvExpirations.SelectedRows(0).Cells(2).Value
        Catch ex As Exception
        End Try

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "DELETE FROM PharmacyCustomFiles.dbo.Expirations " &
                         "WHERE Id = @Id"

            ' Επιβεβαίωση της διαγραφής
            If MessageBox.Show("Θέλετε να διαγράψετε τη λήξη # " & id & " ?", "Διαγραφή", MessageBoxButtons.YesNo) = DialogResult.Yes Then

                ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
                'dgvExchangesFrom.Rows.Remove(dgvExchangesFrom.Rows(selectedRow))

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@Id", id)

                cmd.ExecuteNonQuery()
            End If
        End Using

    End Sub

    Private Sub btnDeleteExpiration_Click(sender As Object, e As EventArgs) Handles btnDeleteExpiration.Click
        DeleteExpiration()
        GetExpirationsList()
    End Sub

    Private Sub tbpPricesParadrugs_Click(sender As Object, e As EventArgs) Handles tbpPricesParadrugs.Click

    End Sub

    Private Sub rbParadrugs_CheckedChanged(sender As Object, e As EventArgs) Handles rbParadrugs.CheckedChanged
        If _loadingChooseFromCatalog = True Then Exit Sub

        HideExpirationDatagrid(True)
        DisplayDrugsOrParadrugs()
        GetExpirationsList()

        If rbParadrugs.Checked = True Then
            grpCalculateLianiki.Visible = True
            grpLastUpdateParadrugs.Visible = True
        ElseIf rbDrugs.Checked = True Then
            grpCalculateLianiki.Visible = False
            grpLastUpdateParadrugs.Visible = False
        End If

    End Sub


    Private Sub btnExpiringDrugs_Click(sender As Object, e As EventArgs) Handles btnExpiringDrugs.Click
        'Me.Enabled = False
        frmDrugsOrParadrugsExpiring.Show()

    End Sub


    Private Sub dgvPricesParadrugs_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvPricesParadrugs.KeyDown
        GetExpirationsList()
    End Sub

    Private Sub dgvPricesParadrugs_KeyUp(sender As Object, e As KeyEventArgs) Handles dgvPricesParadrugs.KeyUp
        GetExpirationsList()
    End Sub

    Private Sub chkPairing_CheckedChanged(sender As Object, e As EventArgs) Handles chkPairing.CheckedChanged
        If chkPairing.Checked = True Then
            btnExpirations.Enabled = True
        Else
            btnExpirations.Enabled = False
        End If
    End Sub

    Private Sub lblLastUpdatedDB1_Click(sender As Object, e As EventArgs) Handles lblLastUpdatedDB1.Click

    End Sub

    Private Sub tmrExchanges_Tick(sender As Object, e As EventArgs) Handles tmrExpirations.Tick
        txtSearchPricesParadrugs.SelectAll()
        txtSearchPricesParadrugs.Focus()
        tmrExpirations.Enabled = False
    End Sub




    Private Sub dtpFromDate_LostFocus(sender As Object, e As EventArgs) Handles dtpFromDate.LostFocus
        GetExchangesList("given")
        GetExchangesList("taken")
        CalculatePreviousTotalBalance()
        DisplayExchangesBalance()
        DisplayFPAPerCurrentIntervall()
        UpdateStartDateExchanges("set")
    End Sub

    Private Sub dtpToDate_LostFocus(sender As Object, e As EventArgs) Handles dtpToDate.LostFocus
        GetExchangesList("given")
        GetExchangesList("taken")
        CalculatePreviousTotalBalance()
        DisplayFPAPerCurrentIntervall()
        DisplayExchangesBalance()
    End Sub



    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Export2Excel()
    End Sub



    Private Sub Export2Excel()
        ' CREATING NEW EXCEL FILE

        '~~> Define your Excel Objects
        Dim xlApp As New Excel.Application
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet
        Dim PTB As Decimal = CalculatePreviousTotalBalance()
        Dim x As Integer = 1
        Dim totalPharm As Decimal = OutPerPharmacist
        Dim totalNiki As Decimal = InPerPharmacist
        Dim WhoOwnsMoney As String = ""
        'Dim FPA65A, FPA13A, FPA23A, FPA0A As Decimal
        'Dim FPA65B, FPA13B, FPA23B, FPA0B As Decimal
        'Dim FPA65, FPA13, FPA23, FPA0 As Decimal

        '~~> Add a New Workbook
        xlWorkBook = xlApp.Workbooks.Add

        '~~> Display Excel
        xlApp.Visible = True

        '~~> Set the relebant sheet that we want to work with
        xlWorkSheet = xlWorkBook.Sheets("Sheet1")

        ' Formatting columns
        With xlWorkSheet.Cells.Font
            .Name = "Arial"
            .Size = 10
        End With

        xlWorkSheet.Cells(1, 1).Font.Size = 16

        With xlWorkSheet.Range("a1:a2000")
            .ColumnWidth = 11
            .HorizontalAlignment = -4131 ' left
        End With

        With xlWorkSheet.Range("B1:B2000")
            .ColumnWidth = 55
            .HorizontalAlignment = -4131 ' left
        End With

        With xlWorkSheet.Range("c1:c2000")
            .ColumnWidth = 9
            .HorizontalAlignment = -4108 ' center
        End With

        With xlWorkSheet.Range("c1:e2000")
            .ColumnWidth = 9
            .HorizontalAlignment = -4108 ' center (-4152 = right)
        End With

        With xlWorkSheet.Range("e7:e9")
            .ColumnWidth = 9
            .HorizontalAlignment = -4131 ' left
        End With

        With xlWorkSheet.Range("b2:b5")
            .ColumnWidth = 55
            .HorizontalAlignment = -4152 ' right
        End With

        With xlWorkSheet
            '~~> Directly type the values that we want
            .Cells(x, 1).Value = "Περίοδος από " & dtpFromDate.Value.ToShortDateString & " εώς " & dtpToDate.Value.ToShortDateString ' χρονική περίοδος

            ' Επικεφαλίδες
            x = x + 2
            .Cells(x, 2).value = "Προηγούμενο υπόλοιπο:"
            .Cells(x, 3).value = "ΦΠΑ"
            .Cells(x, 4).value = "Ποσό"
            .Rows(x).font.bold = True

            ' Προηγούμενο υπόλοιπο κατά ΦΠΑ
            x = x + 1
            .Cells(x, 3).value = "6%"
            .Cells(x, 4).value = Format(FPA65Prev, "f")
            x = x + 1
            .Cells(x, 3).value = "13%"
            .Cells(x, 4).value = Format(FPA13Prev, "f")
            x = x + 1
            .Cells(x, 3).value = "24%"
            .Cells(x, 4).value = Format(FPA23Prev, "f")

            ' Προηγούμενο υπόλοιπο ως σύνολο
            x = x + 1
            .Cells(x, 4).value = "___________"
            x = x + 1
            .Cells(x, 4).value = Format(PTB, "f") ' Χονδρική
            .Cells(x, 3).value = "Σύνολο:"
            If PTB > 0 Then
                .Cells(x, 5).value = "  (" & cbExchangers.Text & " --> Νίκη)"
            Else
                .Cells(x, 5).value = "  (Νίκη --> " & cbExchangers.Text & ")"
            End If


            ' Λίστα ανταλλαγών Συναδέλφου
            x = x + 2
            .Cells(x, 1).value = cbExchangers.Text ' όνομα φαρμακοποιού ανταλλαγής
            .Cells(x, 1).Font.Size = 14
            .Cells(x, 1).font.bold = True
            .Cells(x, 1).Font.Color = Color.Red


            ' Επικεφαλίδες
            x = x + 1
            .Cells(x, 1).value = "Ημερομηνία"
            .Cells(x, 2).value = "Προιόν"
            .Cells(x, 3).value = "ΦΠΑ"
            .Cells(x, 4).value = "Ποσότητα"
            .Cells(x, 5).value = "Τιμή"
            .Rows(x).font.bold = True


            ' Λϊστα φαρμάκων (φάρμακα που δώσαμε)
            For i As Integer = 0 To dgvGivenTo.RowCount - 2
                x = x + 1
                .Cells(x, 1).value = Format(dgvGivenTo.Rows(i).Cells(7).Value, "dd/MM/yyyy") ' Ημερομηνία
                .Cells(x, 2).value = dgvGivenTo.Rows(i).Cells(1).Value ' Προιόν
                .Cells(x, 3).value = dgvGivenTo.Rows(i).Cells(2).Value ' ΦΠΑ
                .Cells(x, 4).value = dgvGivenTo.Rows(i).Cells(3).Value ' Ποσότητα
                .Cells(x, 5).value = Format(dgvGivenTo.Rows(i).Cells(4).Value, "f") ' Χονδρική

            Next

            'Σύνολα φαρμάκων
            x = x + 2
            .Cells(x, 5).value = Format(totalPharm, "C")
            .Cells(x, 5).Font.Size = 12
            .Cells(x, 5).font.bold = True

            ' Σύνολο ΦΠΑ
            x = x + 2
            .Cells(x, 3).value = "ΦΠΑ 6%"
            .Cells(x, 4).value = "ΦΠΑ 13%"
            .Cells(x, 5).value = "ΦΠΑ 24%"
            .Cells(x, 3).font.bold = True
            .Cells(x, 4).font.bold = True
            .Cells(x, 5).font.bold = True

            x = x + 1
            .Cells(x, 3).value = Format(FPA65A, "f")
            .Cells(x, 4).value = Format(FPA13A, "f")
            .Cells(x, 5).value = Format(FPA23A, "f")

            totalPharm = FPA65A + FPA13A + FPA23A

            x = x + 3
            .Cells(x, 1).value = "NIKH" ' όνομα Νικούλας
            .Cells(x, 1).Font.Size = 14
            .Cells(x, 1).font.bold = True
            .Cells(x, 1).Font.Color = Color.Red


            ' Επικεφαλίδες
            x = x + 1
            .Cells(x, 1).value = "Ημερομηνία"
            .Cells(x, 2).value = "Προιόν"
            .Cells(x, 3).value = "ΦΠΑ"
            .Cells(x, 4).value = "Ποσότητα"
            .Cells(x, 5).value = "Τιμή"
            .Rows(x).font.bold = True


            ' Λϊστα φαρμάκων (φάρμακα που πήραμε)
            For i As Integer = 0 To dgvTakenFrom.RowCount - 2
                x = x + 1
                .Cells(x, 1).value = Format(dgvTakenFrom.Rows(i).Cells(7).Value, "dd/MM/yyyy") ' Ημερομηνία
                .Cells(x, 2).value = dgvTakenFrom.Rows(i).Cells(1).Value ' Προιόν
                .Cells(x, 3).value = dgvTakenFrom.Rows(i).Cells(2).Value ' ΦΠΑ
                .Cells(x, 4).value = dgvTakenFrom.Rows(i).Cells(3).Value ' Ποσότητα
                .Cells(x, 5).value = Format(dgvTakenFrom.Rows(i).Cells(4).Value, "f") ' Χονδρική

            Next

            'Υπολογίζει το ισοζύγιο κατά ΦΠΑ
            FPA65 = FPA65A - FPA65B
            FPA13 = FPA13A - FPA13B
            FPA23 = FPA23A - FPA23B
            FPA0 = FPA0A - FPA0B

            'Σύνολα φαρμάκων
            x = x + 2
            .Cells(x, 5).value = Format(totalNiki, "c")
            .Cells(x, 5).Font.Size = 12
            .Cells(x, 5).font.bold = True

            ' Σύνολο ΦΠΑ
            x = x + 2
            .Cells(x, 3).value = "ΦΠΑ 6%"
            .Cells(x, 4).value = "ΦΠΑ 13%"
            .Cells(x, 5).value = "ΦΠΑ 24%"
            .Cells(x, 3).font.bold = True
            .Cells(x, 4).font.bold = True
            .Cells(x, 5).font.bold = True

            x = x + 1
            .Cells(x, 3).value = Format(FPA65B, "f")
            .Cells(x, 4).value = Format(FPA13B, "f")
            .Cells(x, 5).value = Format(FPA23B, "f")

            totalNiki = FPA65B + FPA13B + FPA23B

            'Ισοζύγιο ανταλλαγών περιόδου
            x = x + 3

            'Καθορίζει ποιος χρωστάει σε ποιον
            If totalPharm > totalNiki Then
                WhoOwnsMoney = "   (" & cbExchangers.Text & " ---> Νίκη)"
            ElseIf totalPharm < totalNiki Then
                WhoOwnsMoney = "   (Νίκη ---> " & cbExchangers.Text & ")"
            Else
                WhoOwnsMoney = ""
            End If

            'Συνολικό ποσό
            .Cells(x, 1).value = "Iσοζύγιο ανταλλαγών περιόδου: " & Format((Math.Abs(totalPharm - totalNiki)), "c")
            .Cells(x, 1).Font.Size = 13
            .Cells(x, 1).font.bold = True

            ' Σύνολα κατά ΦΠΑ
            .Cells(x, 3).value = "ΦΠΑ 6%"
            .Cells(x, 4).value = "ΦΠΑ 13%"
            .Cells(x, 5).value = "ΦΠΑ 24%"
            .Cells(x, 3).font.bold = True
            .Cells(x, 4).font.bold = True
            .Cells(x, 5).font.bold = True

            x = x + 1
            .Cells(x, 2).value = WhoOwnsMoney
            .Cells(x, 3).value = Format(FPA65, "f")
            .Cells(x, 4).value = Format(FPA13, "f")
            .Cells(x, 5).value = Format(FPA23, "f")


            'Συνολικό Ισοζύγιο ανταλλαγών 
            x = x + 3

            'Καθορίζει ποιος χρωστάει σε ποιον
            If totalPharm - totalNiki + (FPA65Prev + FPA13Prev + FPA23Prev) > 0 Then
                WhoOwnsMoney = "   (" & cbExchangers.Text & " ---> Νίκη)"
            ElseIf totalPharm - totalNiki + (FPA65Prev + FPA13Prev + FPA23Prev) < 0 Then
                WhoOwnsMoney = "   (Νίκη ---> " & cbExchangers.Text & ")"
            Else
                WhoOwnsMoney = ""
            End If

            'Συνολικό ποσό
            .Cells(x, 1).value = "ΣΥΝΟΛΙΚΟ Iσοζύγιο ανταλλαγών: " & Format((Math.Abs(totalPharm - totalNiki + (FPA65Prev + FPA13Prev + FPA23Prev))), "c")
            .Cells(x, 1).Font.Size = 13
            .Cells(x, 1).font.bold = True

            ' Σύνολα κατά ΦΠΑ
            .Cells(x, 3).value = "ΦΠΑ 6%"
            .Cells(x, 4).value = "ΦΠΑ 13%"
            .Cells(x, 5).value = "ΦΠΑ 24%"
            .Cells(x, 3).font.bold = True
            .Cells(x, 4).font.bold = True
            .Cells(x, 5).font.bold = True

            x = x + 1
            .Cells(x, 2).value = WhoOwnsMoney
            .Cells(x, 3).value = Format(FPA65 + FPA65Prev, "f")
            .Cells(x, 4).value = Format(FPA13 + FPA13Prev, "f")
            .Cells(x, 5).value = Format(FPA23 + FPA23Prev, "f")

            .PageSetup.PrintArea = "A1:E" & x
            .PageSetup.LeftMargin = 30
            .PageSetup.RightMargin = 1
            .PageSetup.FooterMargin = 1
            .PageSetup.HeaderMargin = 1

        End With



    End Sub


    Public Sub DisplayFPAPerCurrentIntervall()
        Dim sqlString1, sqlString2 As String
        Dim FromDate As String = dtpFromDate.Value

        ' Μηδενισμός μεταβλητών
        FPA65A = 0
        FPA13A = 0
        FPA23A = 0
        FPA65B = 0
        FPA13B = 0
        FPA23B = 0

        Try
            ' Λϊστα φαρμάκων (φάρμακα που δώσαμε)
            For i As Integer = 0 To dgvGivenTo.RowCount - 2

                If CType(dgvGivenTo.Rows(i).Cells(2).Value, Decimal) = "6,0" Then
                    FPA65A += dgvGivenTo.Rows(i).Cells(4).Value
                ElseIf CType(dgvGivenTo.Rows(i).Cells(2).Value, Decimal) = "13,0" Then
                    FPA13A += dgvGivenTo.Rows(i).Cells(4).Value
                ElseIf CType(dgvGivenTo.Rows(i).Cells(2).Value, Decimal) = "23,0" Or CType(dgvGivenTo.Rows(i).Cells(2).Value, Decimal) = "24,0" Then
                    FPA23A += dgvGivenTo.Rows(i).Cells(4).Value
                Else
                    FPA0A += dgvGivenTo.Rows(i).Cells(4).Value
                End If
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        Try
            ' Λϊστα φαρμάκων (φάρμακα που πήραμε)
            For i As Integer = 0 To dgvTakenFrom.RowCount - 2

                If CType(dgvTakenFrom.Rows(i).Cells(2).Value, Decimal) = "6,0" Then
                    FPA65B += dgvTakenFrom.Rows(i).Cells(4).Value
                ElseIf CType(dgvTakenFrom.Rows(i).Cells(2).Value, Decimal) = "13,0" Then
                    FPA13B += dgvTakenFrom.Rows(i).Cells(4).Value
                ElseIf CType(dgvTakenFrom.Rows(i).Cells(2).Value, Decimal) = "23,0" Or CType(dgvTakenFrom.Rows(i).Cells(2).Value, Decimal) = "24,0" Then
                    FPA23B += dgvTakenFrom.Rows(i).Cells(4).Value
                Else
                    FPA0B += dgvTakenFrom.Rows(i).Cells(4).Value
                End If
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        'Υπολογίζει το ισοζύγιο κατά ΦΠΑ της τρέχουσας περιόδου
        FPA65 = FPA65A - FPA65B
        FPA13 = FPA13A - FPA13B
        FPA23 = FPA23A - FPA23B
        FPA0 = FPA0A - FPA0B

        'Υπολογίζει το ισοζύγιο κατά ΦΠΑ της προηγούμενης περιόδου
        ' ΦΠΑ 6%
        sqlString1 = "SELECT Xondr FROM [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "WHERE FPA='6' AND Exch ='" & cbExchangers.Text & "' AND FromTo=0 AND Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') >0 "
        sqlString2 = "SELECT Xondr FROM [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "WHERE FPA='6' AND Exch ='" & cbExchangers.Text & "' AND FromTo=1 AND Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') >0 "
        FPA65Prev = CalculateSums(sqlString1, "Xondr") - CalculateSums(sqlString2, "Xondr")
        ' ΦΠΑ 13%
        sqlString1 = "SELECT Xondr FROM [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "WHERE FPA='13' AND Exch ='" & cbExchangers.Text & "' AND FromTo=0 AND Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') >0 "
        sqlString2 = "SELECT Xondr FROM [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "WHERE FPA='13' AND Exch ='" & cbExchangers.Text & "' AND FromTo=1 AND Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') >0 "
        FPA13Prev = CalculateSums(sqlString1, "Xondr") - CalculateSums(sqlString2, "Xondr")
        ' ΦΠΑ 23-24%
        sqlString1 = "SELECT Xondr FROM [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "WHERE (FPA='23' OR FPA='24') AND Exch ='" & cbExchangers.Text & "' AND FromTo=0 AND Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') >0 "
        sqlString2 = "SELECT Xondr FROM [PharmacyCustomFiles].[dbo].[Exchanges] " &
                    "WHERE (FPA='23' OR FPA='24') AND Exch ='" & cbExchangers.Text & "' AND FromTo=1 AND Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') >0 "
        FPA23Prev = CalculateSums(sqlString1, "Xondr") - CalculateSums(sqlString2, "Xondr")

        'Υπολογίζει το συνολικό ισοζύγιο κατά ΦΠΑ
        FPA65Tot = FPA65 + FPA65Prev
        FPA13Tot = FPA13 + FPA13Prev
        FPA23Tot = FPA23 + FPA23Prev

        ' Εμφανίζει τις τιμές στα αντίστοιχα σημεία
        If DateDiff("d", dtpFromDate.Value, "31-05-2016") > 0 Then
            rtxtGivenTo2.Text = "Κατανομή ΦΠΑ:      6%  " & Format(FPA65A, "f") & "      13%  " & Format(FPA13A, "f") & "      23-24%  " & Format(FPA23A, "f")
            rtxtTakenFrom2.Text = "Κατανομή ΦΠΑ:      6%  " & Format(FPA65B, "f") & "      13%  " & Format(FPA13B, "f") & "      23-24%  " & Format(FPA23B, "f")

            rtxtCurrentFPA.Text = "6%  " & Format(Abs(FPA65), "f") & "      13%  " & Format(Abs(FPA13), "f") & "      23-24%  " & Format(Abs(FPA23), "f")
            rtxtCurrentFPA.SelectionAlignment = HorizontalAlignment.Center

            rtxtPreviousFPA.Text = "6%  " & Format(Abs(FPA65Prev), "f") & "      13%  " & Format(Abs(FPA13Prev), "f") & "      23-24%  " & Format(Abs(FPA23Prev), "f")
            rtxtPreviousFPA.SelectionAlignment = HorizontalAlignment.Center

            rtxtTotalFPA.Text = "6%  " & Format(Abs(FPA65Tot), "f") & "      13%  " & Format(Abs(FPA13Tot), "f") & "      23-24%  " & Format(Abs(FPA23Tot), "f")
            rtxtTotalFPA.SelectionAlignment = HorizontalAlignment.Center

            If FPA0A > 0 Then rtxtGivenTo2.Text &= "      0%  " & Format(FPA0A, "f")
            If FPA0B > 0 Then rtxtTakenFrom2.Text &= "      0%  " & Format(FPA0B, "f")

            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
            HightlightInRichTextBox(rtxtGivenTo2, {"6% ", "13% ", "23-24% ", "0% "}, "Blue")
            HightlightInRichTextBox(rtxtCurrentFPA, {"6% ", "13% ", "23-24% ", "0% "}, "Blue")
            HightlightInRichTextBox(rtxtTakenFrom2, {"6% ", "13% ", "23-24% ", "0% "}, "Blue")
            HightlightInRichTextBox(rtxtPreviousFPA, {"6% ", "13% ", "23-24% ", "0% "}, "Blue")
            HightlightInRichTextBox(rtxtTotalFPA, {"6% ", "13% ", "23-24% ", "0% "}, "Blue")
        Else
            rtxtGivenTo2.Text = "Κατανομή ΦΠΑ:      6%  " & Format(FPA65A, "f") & "      13%  " & Format(FPA13A, "f") & "      24%  " & Format(FPA23A, "f")
            rtxtTakenFrom2.Text = "Κατανομή ΦΠΑ:      6%  " & Format(FPA65B, "f") & "      13%  " & Format(FPA13B, "f") & "      24%  " & Format(FPA23B, "f")

            rtxtCurrentFPA.Text = "6%  " & Format(Abs(FPA65), "f") & "      13%  " & Format(Abs(FPA13), "f") & "      24%  " & Format(Abs(FPA23), "f")
            rtxtCurrentFPA.SelectionAlignment = HorizontalAlignment.Center

            rtxtPreviousFPA.Text = "6%  " & Format(Abs(FPA65Prev), "f") & "      13%  " & Format(Abs(FPA13Prev), "f") & "      24%  " & Format(Abs(FPA23Prev), "f")
            rtxtPreviousFPA.SelectionAlignment = HorizontalAlignment.Center

            rtxtTotalFPA.Text = "6%  " & Format(Abs(FPA65Tot), "f") & "      13%  " & Format(Abs(FPA13Tot), "f") & "      24%  " & Format(Abs(FPA23Tot), "f")
            rtxtTotalFPA.SelectionAlignment = HorizontalAlignment.Center

            If FPA0A > 0 Then rtxtGivenTo2.Text &= "      0%  " & Format(FPA0A, "f")
            If FPA0B > 0 Then rtxtTakenFrom2.Text &= "      0%  " & Format(FPA0B, "f")

            ' Αλλάζει με κόκκινο χρώμα τον αριθμό των πελατών
            HightlightInRichTextBox(rtxtGivenTo2, {"6% ", "13% ", "24% ", "0% "}, "Blue")
            HightlightInRichTextBox(rtxtCurrentFPA, {"6% ", "13% ", "24% ", "0% "}, "Blue")
            HightlightInRichTextBox(rtxtTakenFrom2, {"6% ", "13% ", "24% ", "0% "}, "Blue")
            HightlightInRichTextBox(rtxtPreviousFPA, {"6% ", "13% ", "24% ", "0% "}, "Blue")
            HightlightInRichTextBox(rtxtTotalFPA, {"6% ", "13% ", "24% ", "0% "}, "Blue")
        End If
        ' Κάνει bold την επικεφαλίδα
        'FormatInRichTextBox(rtxtGivenTo2, {"Κατανομή ΦΠΑ:"}, "b")
        'FormatInRichTextBox(rtxtTakenFrom2, {"Κατανομή ΦΠΑ:"}, "i")

        'Xρωματίζει τα rich textbox ανάλογα με τις τιμές
        ' Τρέχουσα περίοδος
        If FPA65 < 0 Then
            HightlightInRichTextBox(rtxtCurrentFPA, {Format(Abs(FPA65), "f")}, "Red")
        Else
            HightlightInRichTextBox(rtxtCurrentFPA, {Format(Abs(FPA65), "f")}, "Green")
        End If

        If FPA13 < 0 Then
            HightlightInRichTextBox(rtxtCurrentFPA, {Format(Abs(FPA13), "f")}, "Red")
        Else
            HightlightInRichTextBox(rtxtCurrentFPA, {Format(Abs(FPA13), "f")}, "Green")
        End If

        If FPA23 < 0 Then
            HightlightInRichTextBox(rtxtCurrentFPA, {Format(Abs(FPA23), "f")}, "Red")
        Else
            HightlightInRichTextBox(rtxtCurrentFPA, {Format(Abs(FPA23), "f")}, "Green")
        End If

        ' Προηγούμενη περίοδος
        If FPA65Prev < 0 Then
            HightlightInRichTextBox(rtxtPreviousFPA, {Format(Abs(FPA65Prev), "f")}, "Red")
        Else
            HightlightInRichTextBox(rtxtPreviousFPA, {Format(Abs(FPA65Prev), "f")}, "Green")
        End If
        If FPA13Prev < 0 Then
            HightlightInRichTextBox(rtxtPreviousFPA, {Format(Abs(FPA13Prev), "f")}, "Red")
        Else
            HightlightInRichTextBox(rtxtPreviousFPA, {Format(Abs(FPA13Prev), "f")}, "Green")
        End If
        If FPA23Prev < 0 Then
            HightlightInRichTextBox(rtxtPreviousFPA, {Format(Abs(FPA23Prev), "f")}, "Red")
        Else
            HightlightInRichTextBox(rtxtPreviousFPA, {Format(Abs(FPA23Prev), "f")}, "Green")
        End If

        ' Συνολικό ισοζύγιο
        If FPA65Tot < 0 Then
            HightlightInRichTextBox(rtxtTotalFPA, {Format(Abs(FPA65Tot), "f")}, "Red")
        Else
            HightlightInRichTextBox(rtxtTotalFPA, {Format(Abs(FPA65Tot), "f")}, "Green")
        End If
        If FPA13Tot < 0 Then
            HightlightInRichTextBox(rtxtTotalFPA, {Format(Abs(FPA13Tot), "f")}, "Red")
        Else
            HightlightInRichTextBox(rtxtTotalFPA, {Format(Abs(FPA13Tot), "f")}, "Green")
        End If
        If FPA23Tot < 0 Then
            HightlightInRichTextBox(rtxtTotalFPA, {Format(Abs(FPA23Tot), "f")}, "Red")
        Else
            HightlightInRichTextBox(rtxtTotalFPA, {Format(Abs(FPA23Tot), "f")}, "Green")
        End If

    End Sub



    Private Sub Export2Excel_DebtsList()
        ' CREATING NEW EXCEL FILE

        '~~> Define your Excel Objects
        Dim xlApp As New Excel.Application
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet
        Dim PTB As Decimal = CalculatePreviousTotalBalance()
        Dim x As Integer = 1
        Dim totalPharm As Decimal = OutPerPharmacist
        Dim totalNiki As Decimal = InPerPharmacist
        Dim WhoOwnsMoney As String = ""

        '~~> Add a New Workbook
        xlWorkBook = xlApp.Workbooks.Add

        '~~> Display Excel
        xlApp.Visible = True

        '~~> Set the relebant sheet that we want to work with
        xlWorkSheet = xlWorkBook.Sheets("Sheet1")

        ' Formatting columns
        With xlWorkSheet.Cells.Font
            .Name = "Arial"
            .Size = 10
        End With

        xlWorkSheet.Cells(1, 1).Font.Size = 16

        'Ημερομηνία
        With xlWorkSheet.Range("a1:a2000")
            .ColumnWidth = 11
            .HorizontalAlignment = -4131 ' left
        End With

        'Ποσό
        With xlWorkSheet.Range("B1:B2000")
            .ColumnWidth = 11
            .HorizontalAlignment = -4108 ' center
        End With

        'Περιγραφή
        With xlWorkSheet.Range("c1:c2000")
            .ColumnWidth = 55
            .HorizontalAlignment = -4131 ' left
        End With

        'With xlWorkSheet.Range("d1:d2000")
        '    .ColumnWidth = 10
        '    .HorizontalAlignment = -4152 ' right
        'End With

        With xlWorkSheet
            '~~> Directly type the values that we want
            .Cells(x, 1).Value = grpCustDebts.Text
            .Cells(x + 1, 1).Value = lblTotalDebtLabel.Text & "(" & lblTotalCustomerDebt.Text & ")"

            x = x + 2
            .Cells(x, 1).value = cbExchangers.Text ' όνομα φαρμακοποιού ανταλλαγής
            .Cells(x, 1).Font.Size = 14
            .Cells(x, 1).font.bold = True
            .Cells(x, 1).Font.Color = Color.Red


            ' Επικεφαλίδες
            x = x + 1
            .Cells(x, 1).value = "Ημερομηνία"
            .Cells(x, 2).value = "Ποσό"
            .Cells(x, 3).value = "Περιγραφή"
            '.Cells(x, 4).value = "Χονδρική"
            .Rows(x).font.bold = True


            ' Λϊστα φαρμάκων (φάρμακα που δώσαμε)
            For i As Integer = 0 To dgvDebtsList.RowCount - 2
                x = x + 1
                .Cells(x, 1).value = Format(dgvDebtsList.Rows(i).Cells(0).Value, "dd/MM/yyyy") ' Ημερομηνία
                .Cells(x, 2).value = Format(dgvDebtsList.Rows(i).Cells(1).Value, "f") ' Ποσό
                .Cells(x, 3).value = dgvDebtsList.Rows(i).Cells(2).Value ' Περιγραφή
                '.Cells(x, 3).value = dgvDebtsList.Rows(i).Cells(2).Value ' Ποσότητα
                '.Cells(x, 4).value = Format(dgvDebtsList.Rows(i).Cells(3).Value, "f") ' Χονδρική
            Next

        End With


    End Sub



    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If rbParadrugs.Checked = True Then

            'Είμαστε σε mode Barcode
            If rbByBarcode.Checked = True Then
                Dim myRow As Integer = dgvPricesParadrugs.Rows.Count - 1
                If IsDBNull(dgvPricesParadrugs.Rows(myRow).Cells(0).Value) Then

                    myBarcode = txtSearchPricesParadrugs.Text

                    'Εμφανίζει στο DatagridView
                    dgvPricesParadrugs.Rows(myRow).Cells(0).Value = frmParadrugSelectedDetails.GetOtherDetailsByBarcode("SELECT APOTIKH.AP_DESCRIPTION + ' ' + APOTIKH.AP_MORFI as Fullname ") ' Fullname
                    dgvPricesParadrugs.Rows(myRow).Cells(1).Value = CType(frmParadrugSelectedDetails.GetOtherDetailsByBarcode("SELECT APOTIKH.AP_TIMH_XON "), Decimal) ' Χονδρική
                    dgvPricesParadrugs.Rows(myRow).Cells(2).Value = CType(frmParadrugSelectedDetails.GetOtherDetailsByBarcode("SELECT APOTIKH.AP_TIMH_LIAN "), Decimal) ' Λιανική
                    dgvPricesParadrugs.Rows(myRow).Cells(4).Value = CType(frmParadrugSelectedDetails.GetOtherDetailsByBarcode("SELECT APOTIKH.AP_CODE "), Integer) ' AP_CODE
                    dgvPricesParadrugs.Rows(myRow).Cells(6).Value = CType(frmParadrugSelectedDetails.GetOtherDetailsByBarcode("SELECT APOTIKH.AP_ID "), Integer)  ' AP_ID 
                    dgvPricesParadrugs.Rows(myRow).Cells(7).Value = frmParadrugSelectedDetails.GetOtherDetailsByBarcode("SELECT APOTIKH_BARCODES.BRAP_AP_BARCODE ") ' Barcode

                End If
            End If
        End If
    End Sub

    Private Sub rbByBarcode_CheckedChanged(sender As Object, e As EventArgs) Handles rbByBarcode.CheckedChanged
        If rbByBarcode.Checked = True And _loadingChooseFromCatalog = False Then
            chkManualBarcode.Visible = True
            barcodeType = "barcode"
            GetDrugs("barcode")
        Else
            chkManualBarcode.Visible = False
        End If


    End Sub


    'Private Sub dgvCustomers_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCustomers.CellValueChanged

    '    If e.ColumnIndex = 0 Then
    '        UpdateCustomer3(e.RowIndex)
    '    End If

    'End Sub

    Private Function UpdateCustomer3(ByVal i As Integer) As String

        Dim insertData As String = ""
        Dim ChangedOrExists As String = ""
        Dim CustomerName As String = ""
        Dim id As Integer

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            ChangedOrExists = CheckIfRecordChangedOrExists_Customers(i)

            If ChangedOrExists = "Changed" Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.Customers " &
                          "SET [Name] = @Name " &
                          "WHERE Id = @Id"

            ElseIf ChangedOrExists = "NewRow" Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.Customers " &
                        "([Name]) VALUES (@Name)"

            End If

            If ChangedOrExists = "Changed" Or ChangedOrExists = "NewRow" Then

                CustomerName = dgvCustomers.Rows(i).Cells(0).EditedFormattedValue

                Dim cmd As New SqlCommand(insertData, con)

                If IsDBNull(CustomerName) = True Or CustomerName = "" Then

                Else

                    cmd.Parameters.AddWithValue("@Name", If(CustomerName, DBNull.Value))

                    If ChangedOrExists = "Changed" Then

                        id = If(dgvCustomers.Rows(i).Cells(1).Value, DBNull.Value)
                        cmd.Parameters.AddWithValue("@Id", id)

                        ' Επιβεβαίωση της τροποποίησης (ΜΟΝΟ αν δεν έχει ήδη γίνει στο RowLeave)
                        If Not _confirmingFromRowLeave Then
                            If MessageBox.Show("Θέλετε να τροποποιήσετε το όνομα του πελάτη # " & id & " ?", "Τροποποίηση", MessageBoxButtons.YesNo) = DialogResult.No Then
                                GetCustomersList()
                                Return CustomerName
                            End If
                        End If

                    End If

                    cmd.ExecuteNonQuery()

                    lblNewRow_Cust.Text = "ΕΓΓΡΑΦΗ"

                    DisplayLastUpdate()

                    If ChangedOrExists = "NewRow" Then
                        GetCustomersList()
                        Dim rowIndex As Integer = SearchDatagrid(dgvCustomers, CustomerName)
                        If rowIndex >= 0 Then dgvCustomers.CurrentCell = dgvCustomers.Rows(rowIndex).Cells(0)
                    End If

                End If

            ElseIf ChangedOrExists = "Error" Then

            End If

        End Using

        Return CustomerName

    End Function

    ' Επιστρέφει True αν είναι όλα ΟΚ, αλλιώς False και κάνει focus στο λάθος κελί.
    ' Επιστρέφει True αν όλα ΟΚ, αλλιώς False και κάνει focus στο λάθος κελί.
    Private Function ValidateDebts_DateAndAmount(rowIndex As Integer) As Boolean
        Try
            If rowIndex < 0 OrElse rowIndex >= dgvDebtsList.Rows.Count Then Return True ' <-- ΟΚ: δεν μπλοκάρουμε
            Dim r As DataGridViewRow = dgvDebtsList.Rows(rowIndex)
            If r Is Nothing OrElse r.IsNewRow Then Return True                           ' <-- Κρίσιμο: skip

            ' 0 = Ημερομηνία, 1 = Ποσό
            Dim dateCol As Integer = 0
            Dim amountCol As Integer = 1

            Dim dateText As String = If(r.Cells(dateCol).IsInEditMode,
                                    Convert.ToString(r.Cells(dateCol).EditedFormattedValue),
                                    Convert.ToString(r.Cells(dateCol).Value)).Trim()

            Dim amountText As String = If(r.Cells(amountCol).IsInEditMode,
                                      Convert.ToString(r.Cells(amountCol).EditedFormattedValue),
                                      Convert.ToString(r.Cells(amountCol).Value)).Trim()

            ' ΝΕΟ: Αν και τα δύο είναι κενά (π.χ. φρέσκια γραμμή πριν καταχώρηση), μην ενοχλείς.
            If dateText = "" AndAlso amountText = "" Then Return True

            ' ===== Έλεγχος Ημερομηνίας =====
            Dim el As Globalization.CultureInfo = Globalization.CultureInfo.GetCultureInfo("el-GR")
            Dim dt As System.DateTime
            Dim validDate As Boolean =
            System.DateTime.TryParseExact(
                dateText,
                New String() {"d/M/yyyy", "dd/MM/yyyy", "d-M-yyyy", "dd-MM-yyyy", "yyyy-MM-dd", "d/M/yy", "d-M-yy"},
                el,
                Globalization.DateTimeStyles.None,
                dt)

            If Not validDate Then
                validDate = System.DateTime.TryParse(dateText, el, Globalization.DateTimeStyles.None, dt)
            End If

            If Not validDate Then
                MessageBox.Show("Λάθος καταχώρηση στο πεδίο «Ημερομηνία». Δώσε έγκυρη ημερομηνία (π.χ. 16/10/2025).",
                            "Σφάλμα δεδομένων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Try
                    dgvDebtsList.CurrentCell = r.Cells(dateCol)
                    dgvDebtsList.BeginEdit(True)
                Catch
                End Try
                Return False
            End If

            ' ===== Έλεγχος Ποσού (επιτρέπεται και αρνητικό) =====
            Dim dec As Decimal
            Dim validAmount As Boolean =
            Decimal.TryParse(amountText,
                             Globalization.NumberStyles.Number Or Globalization.NumberStyles.AllowLeadingSign,
                             Globalization.CultureInfo.CurrentCulture,
                             dec)

            If Not validAmount Then
                MessageBox.Show("Λάθος καταχώρηση στο πεδίο «Ποσό». Δώσε έγκυρο αριθμό (π.χ. -12,34).",
                            "Σφάλμα δεδομένων", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Try
                    dgvDebtsList.CurrentCell = r.Cells(amountCol)
                    dgvDebtsList.BeginEdit(True)
                Catch
                End Try
                Return False
            End If

            ' ===== Όριο Ποσού =====
            If Math.Abs(dec) > 10000D Then
                MessageBox.Show("Το ποσό δεν μπορεί να υπερβαίνει τα 10.000,00 €.",
                    "Σφάλμα ποσού", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Try
                    dgvDebtsList.CurrentCell = r.Cells(amountCol)
                    dgvDebtsList.BeginEdit(True)
                Catch
                End Try
                Return False
            End If


            Return True
        Catch
            Return True
        End Try
    End Function




    Private Function GetQRFromScannedCode(ByVal scannedCode As String) As String
        Dim qr As String
        If Len(scannedCode) > 15 Then
            qr = scannedCode.Substring(2, 14)
        End If
        Return qr
    End Function


    Private Function GetTypeScannedCode(txtB As TextBox, tmrKS As Windows.Forms.Timer) As String
        Dim qr As String, result As String
        oldString = newString
        newString = txtB.Text
        result = newString

        If newString = oldString Then
            tmrKS.Enabled = False

            ' Εαν o κωδικός που αναγνωρίστηκε έχει >20 ψηφία μπορεί να είναι QRcode
            If Len(newString) > 20 Then
                qr = GetQRFromScannedCode(newString) ' βρίσκει το πιθανό qrCode
                If IsNumeric(qr) = True Then ' Αν είναι νούμερο --> qrCode
                    barcodeType = "qrcode"
                    result = qr
                    txtSearchPricesParadrugs.Text = qr
                Else ' Αν είναι αλφαρηθμητικό --> Name
                    barcodeType = "name"
                End If
                ' εαν  κωδικός που αναγνωρίστηκε έχει <20 ψηφία..
            ElseIf IsNumeric(newString) = True Then ' και είναι μόνο νούμερα πιθανώς είναι barcode
                barcodeType = "barcode"
            End If
        End If

        Return result

    End Function


    'Private Sub tmrExpirationKeystrokes_Tick(sender As Object, e As EventArgs) Handles tmrExpirationKeystrokes.Tick
    '    GetTypeScannedCode(txtSearchPricesParadrugs, tmrExpirationKeystrokes)
    '    Select Case barcodeType
    '        Case "qrcode"
    '            rbByBarcode.Checked = False
    '            rbByQRcode.Checked = True
    '            rbByName.Checked = False
    '        Case "name"
    '            rbByBarcode.Checked = False
    '            rbByQRcode.Checked = False
    '            rbByName.Checked = True
    '        Case "barcode"
    '            rbByBarcode.Checked = True
    '            rbByQRcode.Checked = False
    '            rbByName.Checked = False
    '    End Select

    '    DisplayDrugsOrParadrugs()
    '    GetExpirationsList()
    '    txtSearchPricesParadrugs.SelectAll()
    '    txtSearchPricesParadrugs.Focus()
    '    barcodeType = ""
    'End Sub


    Private Sub tmrExpirationKeystrokes_Tick(sender As Object, e As EventArgs) Handles tmrExpirationKeystrokes.Tick
        Dim qr As String = "0105200415600405215FFK5K77RCXM0071728013110249645" ' Penrazol

        oldString = newString
        newString = txtSearchPricesParadrugs.Text

        If newString = oldString Then
            tmrExpirationKeystrokes.Enabled = False
            _loadingIncomplete = True

            ' Εαν o κωδικός που αναγνωρίστηκε έχει >20 ψηφία μπορεί να είναι QRcode
            If Len(newString) > 20 Then
                qr = GetQRFromScannedCode(newString) ' βρίσκει το πιθανό qrCode
                If IsNumeric(qr) = True Then ' Αν είναι νούμερο --> qrCode
                    barcodeType = "qrcode"
                    rbByBarcode.Checked = False
                    rbByQRcode.Checked = True
                    rbByName.Checked = False
                    txtSearchPricesParadrugs.Text = qr
                Else ' Αν είναι αλφαρηθμητικό --> Name
                    barcodeType = "name"
                    rbByBarcode.Checked = False
                    rbByQRcode.Checked = False
                    rbByName.Checked = True
                End If
                ' εαν  κωδικός που αναγνωρίστηκε έχει <20 ψηφία..
            ElseIf Len(newString) <= 20 Then
                If IsNumeric(newString) = True Then ' και είναι μόνο νούμερα πιθανώς είναι barcode
                    barcodeType = "barcode"
                    rbByBarcode.Checked = True
                    rbByQRcode.Checked = False
                    rbByName.Checked = False
                Else ' Αν είναι αλφαρηθμητικό --> Name
                    barcodeType = "name"
                    rbByBarcode.Checked = False
                    rbByQRcode.Checked = False
                    rbByName.Checked = True
                End If
            End If
            DisplayDrugsOrParadrugs()
            GetExpirationsList()
            txtSearchPricesParadrugs.SelectAll()
            txtSearchPricesParadrugs.Focus()
            barcodeType = ""
            _loadingIncomplete = False
        End If
    End Sub



    Private Sub UpdateStartDateExchanges(ByVal mode As String)
        Dim sqlString As String = ""
        If mode = "set" Then

            Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
                con.Open()

                sqlString = "UPDATE PharmacyCustomFiles.dbo.Variables " &
                                        "SET [Last_FromDate_Exchanges] = @StartDate " &
                                        "WHERE Id = 0"

                Dim cmd As New SqlCommand(sqlString, con)

                cmd.Parameters.AddWithValue("@StartDate", dtpFromDate.Value)

                cmd.ExecuteNonQuery()

            End Using

        ElseIf mode = "get" Then

            'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
            Using con As New SqlClient.SqlConnection(connectionstring)

                sqlString = "SELECT [Last_FromDate_Exchanges] " &
                            "FROM PharmacyCustomFiles.dbo.Variables " &
                            "WHERE Id = 0"

                'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
                Using cmd As New SqlClient.SqlCommand(sqlString, con)

                    ' Ανοίγει την σύνδεση
                    con.Open()

                    'Ορισμός ExecuteReader 
                    Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                    If myReader.HasRows Then
                        Do While myReader.Read()

                            'Υπολογίζει το συνολικό ποσό
                            dtpFromDate.Value = myReader("Last_FromDate_Exchanges")

                        Loop
                    Else
                    End If

                End Using
            End Using

        End If
    End Sub





    Private Sub dgvPricesParadrugs_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPricesParadrugs.CellContentClick

    End Sub

    Private Sub btnMyBarcodes_Click(sender As Object, e As EventArgs) Handles btnMyBarcodes.Click
        'Me.Enabled = False
        frmMyBarcodes.Show()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        frmIFETprice.Show()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs)
        MsgBox(Me.Location.X & " " & Me.Location.Y)
    End Sub

    Private Sub grpBackupSource_Enter(sender As Object, e As EventArgs) Handles grpBackupSource.Enter

    End Sub

    Private Sub tbpBackup_Click(sender As Object, e As EventArgs) Handles tbpBackup.Click

    End Sub

    Private Sub rbWhereFarm2_CheckedChanged(sender As Object, e As EventArgs) Handles rbWhereFarm2.CheckedChanged
        txtSourceFolderVS.Text = "D:\Visual Studio 2012 - LEARNING\Pharmacy"
        txtDestinationDrive.Text = "F:\MyPharmacy Files"
        If txtDB1.Text <> "" Then
            btnUpdatePharmacy2013C.Text = "Update " & txtDB1.Text
        Else
            btnUpdatePharmacy2013C.Text = "Update Pharmacy2013C"
        End If
        btnBackupRestore.Enabled = False
        btnUpdatePharmacy2013C.Enabled = False
        btnCoppyAppStation1.Enabled = False
    End Sub

    Private Sub Button8_Click_1(sender As Object, e As EventArgs) Handles btnCoppyAppStation1.Click
        Dim folderAppSource As String = "D:\Visual Studio 2012 - LEARNING\Pharmacy\Pharmacy\bin\Debug\"
        Dim folderAppDest As String = "X:\"
        Dim fileName As String = "Pharmacy.exe"
        Dim timeStamp As String = Now.Day & Now.Month & Now.Year & "-" & Now.Hour & Now.Minute & Now.Second

        Try
            My.Computer.FileSystem.CopyFile(folderAppDest & fileName, folderAppDest & timeStamp & "-" & fileName, True)
            My.Computer.FileSystem.CopyFile(folderAppSource & fileName, folderAppDest & fileName, True)
            MsgBox("Το αρχείο " & fileName & " αντιγράφτηκε επιτυχώς στο Station 1")
        Catch ex As Exception
            MsgBox("Αποτυχία αντιγραφής" & vbCrLf & vbCrLf & ex.Message)
        End Try

    End Sub

    Private Sub UpdateParadrugsOnLeave()

        Dim name As String = ""
        Dim index As Integer = 0

        dirty = dgvPricesParadrugs.IsCurrentRowDirty
        txtRowChanged3.Text = dirty.ToString

        Try
            index = dgvPricesParadrugs.SelectedRows(0).Index
        Catch ex As Exception
        End Try

        If dirty = True Then
            UpdateParadrug(index)
        Else
            lblNewRecordAdded.Text = ""
        End If
    End Sub

    Private Sub dgvPricesParadrugs_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPricesParadrugs.RowLeave
        Dim name As String = ""

        dirty = dgvPricesParadrugs.IsCurrentRowDirty
        txtRowChanged3.Text = dirty.ToString

        If dirty = True Then
            UpdateParadrug(e.RowIndex)
        Else
            lblNewRecordAdded.Text = ""
        End If

    End Sub


    Private Sub dgvPricesParadrugs_Leave(sender As Object, e As EventArgs) Handles dgvPricesParadrugs.Leave
        'Dim name As String = ""
        'Dim index As Integer = 0

        '' MsgBox("Lost focus")

        'Try
        '    index = dgvPricesParadrugs.CurrentCell.RowIndex
        'Catch ex As Exception
        'End Try

        'dirty = dgvPricesParadrugs.IsCurrentRowDirty
        'txtRowChanged3.Text = dirty.ToString

        'If dirty = True Then

        '    UpdateParadrug(index)

        'Else
        '    lblNewRecordAdded.Text = ""
        'End If
    End Sub


    Private Function CheckNeedForUpdatingPharmacy2013() As Boolean
        Dim DBCompareResult As String = ComparePharmacy2013VsFarnet()

        If DBCompareResult <> "No" Then
            If MsgBox("Έχουν υπάρξει αλλαγές στο αρχείο φαρμάκων του FarmakoNet! " & vbCrLf & vbCrLf & DBCompareResult & vbCrLf & vbCrLf & "Θέλετε να ενημερώσω τα στοιχεία τώρα;", MsgBoxStyle.OkCancel, "Ενημέρωση database Pharmacy2013") = MsgBoxResult.Ok Then
                UpdatePharmacy2013BasedFarmnet()
            End If
        ElseIf DBCompareResult = "No" Then
            Return False
        End If
        Return True
    End Function

    Private Sub Button8_Click_2(sender As Object, e As EventArgs) Handles Button8.Click
        If CheckNeedForUpdatingPharmacy2013() = False Then
            MsgBox("Το database Pharmacy2013 είναι ανανεωμένο!")
        End If
    End Sub


    Private Sub dgvPricesParadrugs_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPricesParadrugs.RowEnter
        'ParadrugRowEnter = True
        'Dim id As Integer = e.RowIndex
        'txtRowChanged.Clear()
        'Try
        '    For t = 0 To 7
        '        If t = 1 Or t = 2 Then
        '            RowValuesOld(t) = Format(dgvPricesParadrugs.Rows(e.RowIndex).Cells(t).Value, "F")
        '        Else
        '            RowValuesOld(t) = CType(dgvPricesParadrugs.Rows(e.RowIndex).Cells(t).Value, String)
        '        End If
        '    Next
        'Catch ex As Exception
        'End Try
        'For t = 0 To 7
        '    txtRowChanged.Text &= RowValuesOld(t) & vbCrLf
        'Next

    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim id As Integer = 0
        Try
            id = dgvPricesParadrugs.CurrentRow.Index
        Catch ex As Exception
        End Try
        txtRowChanged.Clear()
        Try
            For t = 0 To 7
                RowValuesNew(t) = CType(dgvPricesParadrugs.Rows(id).Cells(t).Value, String)
            Next
        Catch ex As Exception
        End Try
        For t = 0 To 7
            txtRowChanged.Text &= RowValuesNew(t) & vbCrLf
        Next

    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Dim id As Integer = 0
        Try
            id = dgvPricesParadrugs.CurrentRow.Index
        Catch ex As Exception
        End Try
        txtRowChanged.Clear()
        Try
            For t = 0 To 7
                RowValuesNew(t) = CType(dgvPricesParadrugs.Rows(id).Cells(t).EditedFormattedValue, String)
            Next
        Catch ex As Exception
        End Try
        For t = 0 To 7
            txtRowChanged.Text &= RowValuesNew(t) & vbCrLf
        Next
    End Sub

    Private Function IsRowChanged_Paradrugs(ByVal MyIndex As Integer) As Boolean
        Dim result As Boolean = False
        'Dim id As Integer = 0
        'Try
        '    id = dgvPricesParadrugs.CurrentRow.Index
        'Catch ex As Exception
        'End Try

        Try
            For t = 0 To 7
                If t = 1 Or t = 2 Then
                    Try
                        RowValuesNew(t) = CType(dgvPricesParadrugs.Rows(MyIndex).Cells(t).EditedFormattedValue, Decimal).ToString
                    Catch ex As Exception
                        RowValuesNew(t) = ""
                    End Try

                Else
                    RowValuesNew(t) = CType(dgvPricesParadrugs.Rows(MyIndex).Cells(t).EditedFormattedValue, String)
                End If

            Next
        Catch ex As Exception
        End Try

        For t = 0 To 7
            If RowValuesNew(t) <> "" And RowValuesNew(t) <> RowValuesOld(t) Then
                result = True
                Exit For
            Else
                result = False
            End If
        Next

        Return result

    End Function


    Private Sub dgvExpirations_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvExpirations.CellContentClick
        Try
            txtRowChanged.Text = dgvExpirations.SelectedRows(0).Cells(3).Value
        Catch ex As Exception
        End Try

    End Sub

    Private Sub dgvExpirations_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvExpirations.RowLeave
        dirty = dgvExpirations.IsCurrentRowDirty
        lblDirtyState_Exp.Text = dirty.ToString

        If dirty = True Then

            UpdateExpirationList(e.RowIndex, e.ColumnIndex)
            'GetExpirationsList()
        Else
            lblNewRecord_Exp.Text = ""
        End If

    End Sub

    'Private Sub dgvDebtsList_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDebtsList.RowLeave
    '    If _suppressDebtsRowLeave Then Exit Sub

    '    ' Αν φεύγουμε από NewRow, δεν κάνουμε καμία απολύτως αποθήκευση/validation
    '    If e.RowIndex < 0 Then Exit Sub
    '    If dgvDebtsList.Rows(e.RowIndex).IsNewRow Then Exit Sub

    '    If Not ValidateDebts_DateAndAmount(e.RowIndex) Then Exit Sub
    '    SaveDebtRow(e.RowIndex)
    'End Sub




    'Private Sub dgvCustomers_RowsAdded(sender As Object, e As DataGridViewRowsAddedEventArgs) Handles dgvCustomers.RowsAdded
    '    If e.RowIndex = dgvCustomers.Rows.Count - 1 Then
    '        MsgBox("Last")
    '    End If
    'End Sub

    'Private Sub dgvCustomers_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCustomers.CellContentClick

    'End Sub



    Private Sub tmrRerunDatagridV_Tick(sender As Object, e As EventArgs) Handles tmrRerunDatagridV.Tick
        If DatagridEdited = "Customers" AndAlso Not String.IsNullOrEmpty(NewRowName) Then
            ' Βάλε το μόνο αν δεν έχει ήδη γραφτεί από τον χρήστη κάτι άλλο
            If txtSearchCustomer.Text <> NewRowName AndAlso Not txtSearchCustomer.Focused Then
                txtSearchCustomer.Text = NewRowName
            End If
            ' Καθάρισε τα flags για να μην ξαναχτυπήσει
            DatagridEdited = ""
            NewRowName = ""
        End If
        tmrRerunDatagridV.Enabled = False
    End Sub


    Private Sub EnableDoubleBuffer(dgv As DataGridView)
        Dim pi = GetType(DataGridView).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If pi IsNot Nothing Then pi.SetValue(dgv, True, Nothing)
    End Sub


    Private Sub dgvPricesParadrugs_DataError1(sender As Object, e As DataGridViewDataErrorEventArgs) Handles dgvPricesParadrugs.DataError
        rtxtPricesParadrugs.Text = "DB Error!"
    End Sub

    Private Sub dgvPricesParadrugs_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles dgvPricesParadrugs.DataBindingComplete
        ApplyDrugQrCodeVisuals()
    End Sub


    Private Function CurrentRowHasId() As Integer
        Dim index, id As Integer
        Try
            index = dgvPricesParadrugs.CurrentCell.RowIndex
        Catch ex As Exception
        End Try
        Try
            id = dgvPricesParadrugs.CurrentRow.Cells(5).Value
        Catch ex As Exception
        End Try

        If dgvPricesParadrugs.CurrentRow.Cells(5).Value Is DBNull.Value Then
            MsgBox("Το νέο παραφάρμακο δεν έχει καταχωρηθεί ακόμα στο DB")
            Return 0
        Else
            Return id
        End If
    End Function


    Private Sub txtNoExpirations_TextChanged(sender As Object, e As EventArgs) Handles txtNoExpirations.TextChanged

    End Sub

    Private Sub lblDirtyState_Exp_Click(sender As Object, e As EventArgs) Handles lblDirtyState_Exp.Click

    End Sub

    Private Sub grpExpirationList_Click(sender As Object, e As EventArgs) Handles grpExpirationList.Click
        CurrentRowHasId()
    End Sub

    Private Sub rbWhereSaloni_CheckedChanged(sender As Object, e As EventArgs) Handles rbWhereSaloni.CheckedChanged
        btnBackupRestore.Enabled = False
        btnUpdatePharmacy2013C.Enabled = False
        btnCoppyAppStation1.Enabled = False
    End Sub

    Private Sub rbWhereNikoyla_CheckedChanged(sender As Object, e As EventArgs) Handles rbWhereNikoyla.CheckedChanged
        btnBackupRestore.Enabled = False
        btnUpdatePharmacy2013C.Enabled = False
        btnCoppyAppStation1.Enabled = False
    End Sub

    Private Sub btnPrintDebtsList_Click(sender As Object, e As EventArgs) Handles btnPrintDebtsList.Click
        Export2Excel_DebtsList()
    End Sub

    Private Sub txtTotalPrice_Paradrugs_TextChanged(sender As Object, e As EventArgs) Handles txtTotalPrice_Paradrugs.TextChanged

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        MsgBox(GetPercentFromDrug(dgvPricesParadrugs.SelectedRows(0).Cells(4).Value))
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click

        'UpdateFPAExchangesOLDv()

        UpdateFPAOnExchanges()

    End Sub


    Private Sub UpdateFPAExchangesOLDv()
        chkAutoInsertName.Checked = False

        For t = 0 To dgvGivenTo.Rows.Count - 1
            Dim APCode As Integer = 0
            Dim MyPercent As Decimal = 0
            Try
                APCode = CType(dgvGivenTo.Rows(t).Cells(6).Value, Integer)
            Catch ex As Exception
                APCode = 0
            End Try

            If APCode > 0 Then
                MyPercent = GetPercentFromDrug(APCode)
                If MyPercent > 0 Then
                    dgvGivenTo.Rows(t).Cells(2).Value = MyPercent
                End If
            End If

        Next
        For t = 0 To dgvTakenFrom.Rows.Count - 1
            Dim APCode As Integer = 0
            Dim MyPercent As Decimal = 0
            Try
                APCode = CType(dgvTakenFrom.Rows(t).Cells(6).Value, Integer)
            Catch ex As Exception
                APCode = 0
            End Try

            If APCode > 0 Then
                MyPercent = GetPercentFromDrug(APCode)
                If MyPercent > 0 Then
                    dgvTakenFrom.Rows(t).Cells(2).Value = MyPercent
                End If
            End If
        Next

        chkAutoInsertName.Checked = True
    End Sub


    Private Sub btnEditExchangers_Click(sender As Object, e As EventArgs) Handles btnEditExchangers.Click
        'Ελέγχει αν υπάρχει ήδη το όνομα στην λίστα
        If CheckIfExchangerExists() Then
            ' Αν υπάρχει ήδη ζητάει αν θέλουμε να τον διαγράψουμε
            If MsgBox("Υπάρχει ήδη ένας συνάδελφος με αυτό το όνομα. Θέλετε να τον διαγράψετε;", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                ' Εαν έχει ήδη εγγραφές δεν διαγράφεται
                Dim ActiveRecords = CheckIfExchangerHasRecords()
                If ActiveRecords > 0 Then
                    MsgBox("O συνάδελφος [" & cbExchangers.Text & "] ΔΕΝ μπορεί να διαγραφεί γιατί έχει ενεργές εγγραφές (" & ActiveRecords & ")")
                Else
                    ' Αλλιώς τον διαγράφει
                    DeleteFromExchangerList()
                    ' ενημερώνει το combobox
                    FillComboBox(cbExchangers, GetDistinctContentsDBField(
                 "SELECT ExchangerName FROM PharmacyCustomFiles.dbo.ExchangerList ORDER By ExchangerName", "ExchangerName"), {})
                    'και πάει στον πρώτο συνάδλεφο της λίστας
                    cbExchangers.SelectedIndex = 0
                End If
            End If
        Else
            ' Προσθέτει τον νέο συνάδελφο
            Add2ExchangerList()
            'Ενημερώνει την λίστα
            FillComboBox(cbExchangers, GetDistinctContentsDBField(
                 "SELECT ExchangerName FROM PharmacyCustomFiles.dbo.ExchangerList ORDER By ExchangerName", "ExchangerName"), {})
            'Ετοιμάζεται για τις πρώτες ανταλλαγές με τον νέο συνάδελφο
            GetExchangesList("given")
            GetExchangesList("taken")
            CalculatePreviousTotalBalance()
            DisplayExchangesBalance()
        End If

    End Sub

    Private Sub btnImportExcel_Click(sender As Object, e As EventArgs) Handles btnImportExcel.Click
        frmImportExcel2Datagrid.Show()
    End Sub



    Private Sub Button13_Click(sender As Object, e As EventArgs)
        ' Κάνει bold την επικεφαλίδα
        FormatInRichTextBox(rtxtGivenTo2, {"Κατανομή ΦΠΑ:"}, "b")
        FormatInRichTextBox(rtxtTakenFrom2, {"Κατανομή ΦΠΑ:"}, "b")

    End Sub

    Private Sub dgvPrescriptions_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPrescriptions.CellContentClick

    End Sub

    Private Sub Button13_Click_1(sender As Object, e As EventArgs)
        bsPrescriptions.AddNew()
        Dim ROWINDEX As Integer = dgvPrescriptions.Rows.Count - 1
        ' Drug1
        dgvPrescriptions.Rows(ROWINDEX).Cells(1).Value = "Prova"
        ' Drug2
        dgvPrescriptions.Rows(ROWINDEX).Cells(2).Value = "Prova"

        ' Drug3
        dgvPrescriptions.Rows(ROWINDEX).Cells(3).Value = "Prova"
        bsPrescriptions.EndEdit()
    End Sub

    Private Sub grpBackupDestination_Enter(sender As Object, e As EventArgs) Handles grpBackupDestination.Enter

    End Sub

    Private Sub BtnStartService_Click(sender As Object, e As EventArgs) Handles btnStartService.Click
        StartService(txtServiceName.Text)
    End Sub

    Private Sub BtnStopService_Click(sender As Object, e As EventArgs) Handles btnStopService.Click
        StopService(txtServiceName.Text)
    End Sub

    Private Sub Button13_Click_2(sender As Object, e As EventArgs) Handles Button13.Click
        'Process.Start("C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Microsoft SQL Server 2017\Configuration Tools\SQL Server 2017 Configuration Manager") ' CRAZYDR
        Process.Start("C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Microsoft SQL Server 2019\Configuration Tools\SQL Server 2019 Configuration Manager") ' FARMAKEIO
    End Sub

    Private Sub TxtSourceFarmnetDB_TextChanged(sender As Object, e As EventArgs) Handles txtSourceFarmnetDB.TextChanged

    End Sub

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        ' Ανοίγει το παράθυρο των νέων ασθενών 
        frmUFN = New frmUpdateFarmNetDB
        frmUFN.Show()
    End Sub

    Private Sub RbByName_CheckedChanged(sender As Object, e As EventArgs) Handles rbByName.CheckedChanged
        If rbByName.Checked = True And _loadingChooseFromCatalog = False Then
            GetDrugs("name")
        End If

    End Sub

    Private Sub RbByQRcode_CheckedChanged(sender As Object, e As EventArgs) Handles rbByQRcode.CheckedChanged
        If rbByQRcode.Checked = True And _loadingChooseFromCatalog = False Then
            barcodeType = "qrcode"
            GetDrugs("qrcode")
        End If

    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        UpdatePharmacy2025_TRYOUT()
    End Sub

    Private Sub btnAddGivenTo_Click(sender As Object, e As EventArgs) Handles btnAddGivenTo.Click
        ExchangesGivenOrTaken = "given"
        Me.Enabled = False
        OpenDrugSelectionFromCatalogForm("newExchanges")
    End Sub

    ' Ακυρώνει το edit στο NewRow (τελευταία γραμμή placeholder)
    Private Sub dgvGivenTo_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgvGivenTo.CellBeginEdit
        If dgvGivenTo.Rows(e.RowIndex).IsNewRow Then e.Cancel = True
    End Sub

    Private Sub dgvTakenFrom_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgvTakenFrom.CellBeginEdit
        If dgvTakenFrom.Rows(e.RowIndex).IsNewRow Then e.Cancel = True
    End Sub



    ' Customers.vb
    Public Sub AddExchangeDirect(
        ByVal drugName As String,
        ByVal apCode As String,
        ByVal unitXondr As Decimal,
        ByVal qnt As Integer,
        ByVal fpa As Decimal,
        ByVal fromTo As Integer)

        Dim totalXondr As Decimal = unitXondr * qnt

        Using con As New SqlClient.SqlConnection(connectionstring)
            con.Open()

            Dim sql As String =
                "INSERT INTO PharmacyCustomFiles.dbo.Exchanges " &
                "([DrugName],[Xondr],[Qnt],[RP],[AP_Code],[MyDate],[Exch],[FromTo],[FPA]) " &
                "VALUES (@DrugName,@Xondr,@Qnt,@RP,@AP_Code,@MyDate,@Exch,@FromTo,@FPA)"

            Using cmd As New SqlClient.SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@DrugName", drugName)
                cmd.Parameters.AddWithValue("@Xondr", totalXondr)
                cmd.Parameters.AddWithValue("@Qnt", qnt)
                cmd.Parameters.AddWithValue("@RP", DBNull.Value) ' κρατάμε ό,τι ίσχυε
                If String.IsNullOrWhiteSpace(apCode) Then
                    cmd.Parameters.AddWithValue("@AP_Code", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@AP_Code", apCode)
                End If
                cmd.Parameters.AddWithValue("@MyDate", Now)
                cmd.Parameters.AddWithValue("@Exch", cbExchangers.Text) ' τρέχων “συνεργάτης”
                cmd.Parameters.AddWithValue("@FromTo", fromTo)          ' 0=Δώσαμε, 1=Πήραμε
                cmd.Parameters.AddWithValue("@FPA", fpa)

                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' Ανανέωση UI και αθροισμάτων, όπως ήδη κάνεις αλλού
        GetExchangesList("given")
        GetExchangesList("taken")
        CalculatePreviousTotalBalance()
        DisplayExchangesBalance()
        DisplayFPAPerCurrentIntervall()
    End Sub

    Private Sub btnAddManualGivenTo_Click(sender As Object, e As EventArgs) Handles btnAddManualGivenTo.Click
        ExchangesGivenOrTaken = "given"

        Using f As New frmAddManualExchangeNonDrug()
            If f.ShowDialog(Me) = DialogResult.OK Then
                ' ΤΩΡΑ είμαστε μέσα στο frmCustomers, άρα οι Private μέθοδοι είναι ορατές
                GetExchangesList("given")
                UpdateExchangesTotalAndSums()
                DisplayFPAPerCurrentIntervall()
                DisplayExchangesBalance()
                DisplayLastUpdate()
            End If
        End Using
    End Sub

    Private Sub btnAddManualTakenFrom_Click(sender As Object, e As EventArgs) Handles btnAddManualTakenFrom.Click
        ExchangesGivenOrTaken = "taken"

        Using f As New frmAddManualExchangeNonDrug()
            If f.ShowDialog(Me) = DialogResult.OK Then
                ' ΤΩΡΑ είμαστε μέσα στο frmCustomers, άρα οι Private μέθοδοι είναι ορατές
                GetExchangesList("taken")
                UpdateExchangesTotalAndSums()
                DisplayFPAPerCurrentIntervall()
                DisplayExchangesBalance()
                DisplayLastUpdate()
            End If
        End Using
    End Sub

    ' Κάλεσέ το ΜΙΑ φορά π.χ. στο Form_Load
    Private Sub WireUpGridEvents()
        AddHandler dgvDebtsList.DataBindingComplete, AddressOf DebtsGrid_DataBindingComplete
        AddHandler dgvHairdiesList.DataBindingComplete, AddressOf HairdiesGrid_DataBindingComplete
    End Sub

    Private Sub DebtsGrid_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        ApplyDebtsGridFormatting()

        If _forceLastAfterBind Then
            _suppressDebtsRowLeave = True
            Try
                If _forceLastAfterBind AndAlso dgvDebtsList.Rows.Cast(Of DataGridViewRow)().Any(Function(r) Not r.IsNewRow) Then
                    _forceLastAfterBind = False
                    Me.BeginInvoke(Sub() SafeScrollToLastRow(dgvDebtsList))
                End If
            Finally
                _suppressDebtsRowLeave = False
                _forceLastAfterBind = False
            End Try
        End If
    End Sub



    Private Sub HairdiesGrid_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        ' αν έχεις συγκεκριμένη μορφοποίηση για τις Βαφές, βάλε την εδώ
    End Sub

    Private Sub ApplyDebtsGridFormatting()
        With dgvDebtsList
            .EnableHeadersVisualStyles = False
            .AutoGenerateColumns = False
            If .Columns.Count >= 2 Then
                .Columns(0).HeaderText = "Ημερομηνία"
                .Columns(1).HeaderText = "Ποσό"
                .Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(0).DefaultCellStyle.Format = "dd-MM-yyyy"
                .Columns(1).DefaultCellStyle.Format = "N2"
                .Columns(0).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(1).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            End If
        End With
    End Sub


    Private Sub dgvCustomers_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles dgvCustomers.DataBindingComplete
        HideCustomersId()
    End Sub

    Private Sub HideCustomersId()
        Try
            dgvCustomers.AutoGenerateColumns = False
            If dgvCustomers.Columns.Contains("Id") Then
                dgvCustomers.Columns("Id").Visible = False
            ElseIf dgvCustomers.Columns.Count > 1 Then
                ' fallback: στις λίστες που η 2η στήλη είναι το Id απλά κρύψ’ την
                dgvCustomers.Columns(1).Visible = False
            End If
        Catch
        End Try
    End Sub


    Private Sub SetupGridsForMultiDelete()
        For Each g As DataGridView In New DataGridView() {dgvDebtsList, dgvPrescriptions, dgvDrugsOnLoan, dgvHairdiesList}
            g.MultiSelect = True
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Next
    End Sub



    Private Sub btnAddTakenFrom_Click(sender As Object, e As EventArgs) Handles btnAddTakenFrom.Click
        ExchangesGivenOrTaken = "taken"
        Me.Enabled = False
        OpenDrugSelectionFromCatalogForm("newExchanges")
    End Sub

    Private Sub btnAddDebt_Click(sender As Object, e As EventArgs) Handles btnAddDebt.Click
        Dim f As New frmDebtEntry(
        Function(barc As String) LookupFullDescriptionByBarcode(barc),
        Function(qr As String) LookupFullDescriptionByQRCode(qr),
        Function(raw As String) GetRetailFromScanned(raw)
    )

        AddHandler f.DebtCommitted,
        Sub(_s, args)

            ' --- Προσθήκη σε data-bound πηγή (BindingSource/DataTable) ---
            Dim bs As BindingSource = TryCast(dgvDebtsList.DataSource, BindingSource)
            Dim dt As DataTable = Nothing
            Dim newRowIndex As Integer = -1

            _suppressDebtsValidation = True
            _suppressDebtsRowLeave = True
            _suppressSelectionChanged = True

            Try
                If bs IsNot Nothing Then
                    ' BindingSource -> DataTable
                    dt = TryCast(bs.DataSource, DataTable)
                    If dt Is Nothing Then
                        ' BindingSource με DataView / άλλου τύπου: χρησιμοποίησε AddNew
                        If bs.AllowNew Then
                            Dim drv As DataRowView = CType(bs.AddNew(), DataRowView)
                            ' ΥΠΟΘΕΣΗ index: 0=Ημερομηνία, 1=Ποσό, 2=Περιγραφή
                            drv(0) = args.DateIn
                            drv(1) = args.Retail
                            drv(2) = args.Description
                            bs.EndEdit()
                        Else
                            MessageBox.Show("Η πηγή δεδομένων δεν επιτρέπει AddNew.", "Καταχώρηση", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Sub
                        End If
                    Else
                        ' DataTable: πρόσθεσε DataRow
                        Dim r As DataRow = dt.NewRow()
                        r(0) = args.DateIn
                        r(1) = args.Retail
                        r(2) = args.Description
                        dt.Rows.Add(r)
                    End If
                Else
                    ' DataTable απευθείας στο grid;
                    dt = TryCast(dgvDebtsList.DataSource, DataTable)
                    If dt IsNot Nothing Then
                        Dim r As DataRow = dt.NewRow()
                        r(0) = args.DateIn
                        r(1) = args.Retail
                        r(2) = args.Description
                        dt.Rows.Add(r)
                    Else
                        ' Fallback: αν (και μόνο αν) ΔΕΝ είναι data-bound
                        If dgvDebtsList.DataSource Is Nothing Then
                            Dim i = dgvDebtsList.Rows.Add()
                            dgvDebtsList.Rows(i).Cells(0).Value = args.DateIn
                            dgvDebtsList.Rows(i).Cells(1).Value = args.Retail
                            dgvDebtsList.Rows(i).Cells(2).Value = args.Description
                        Else
                            MessageBox.Show("Το dgvDebtsList είναι data-bound και δεν υποστηρίζει Rows.Add().", "Καταχώρηση", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                        End If
                    End If
                End If

                ' --- Βρες index της νέας γραμμής στο grid (τελευταία ορατή που δεν είναι NewRow) ---
                Dim lastVisible = dgvDebtsList.Rows.GetLastRow(DataGridViewElementStates.Visible)
                For i As Integer = lastVisible To 0 Step -1
                    If dgvDebtsList.Rows(i).Visible AndAlso Not dgvDebtsList.Rows(i).IsNewRow Then
                        newRowIndex = i
                        Exit For
                    End If
                Next

            Finally
                _suppressSelectionChanged = False
                _suppressDebtsRowLeave = False
                _suppressDebtsValidation = False
            End Try

            ' --- Προαιρετικά: έλεγχος & αποθήκευση με τις υπάρχουσες ρουτίνες ---
            If newRowIndex >= 0 Then
                If ValidateDebts_DateAndAmount(newRowIndex) Then
                    SaveDebtRow(newRowIndex)
                End If

                ' Εστίαση στη νέα γραμμή
                Try
                    If dgvDebtsList.IsHandleCreated AndAlso Not dgvDebtsList.IsDisposed Then
                        dgvDebtsList.FirstDisplayedScrollingRowIndex = newRowIndex
                        dgvDebtsList.CurrentCell = dgvDebtsList.Rows(newRowIndex).Cells(0)
                        dgvDebtsList.Rows(newRowIndex).Selected = True
                    End If
                Catch
                End Try
            End If
        End Sub

        f.Show(Me)
    End Sub

End Class
