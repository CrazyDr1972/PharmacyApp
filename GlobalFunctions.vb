Imports Pharmacy.GlobalVariables
Imports System.Data.SqlClient
Imports System.Threading
Imports System.ServiceProcess
Imports System.Security.Principal
Imports System.ComponentModel
Imports System.Text.RegularExpressions


Public Class GlobalFunctions


    ' ************************************************************************************************************************
    ' ************************************************************************************************************************
    ''' <summary>FillListBox: Γεμίζει ένα ListBox με data από ένα επιλεγμένο DataBase 
    '''          με καθορισμό των DisplayMember και ValueMember
    ''' </summary>
    ''' <param name="SqlString">SQL Transact Text για τη νέα σύνδεση </param>
    ''' <param name="oListBox">Το όνομα του ListBox που θα γεμίσουμε</param>
    ''' <param name="ListBoxText">To table που θα εμφανίζεται στο ListBox</param>
    ''' <param name="ListBoxValue">OPTIONAL: To table που θα χρησιμοποιείται σαν Value</param>
    ''' <remarks></remarks>
    ''' 

    Public Shared Function FillListBox(ByVal SqlString As String, ByVal oListBox As ListBox, ByVal ListBoxText As String,
                                       Optional ByVal ListBoxValue As String = "Id") As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
            Using cmd As New SqlClient.SqlCommand(SqlString, con)

                'που την ανοίγει εδώ
                con.Open()

                'Ορισμός νέου SqlDataAdapter και νέου DataSet
                Dim da As New SqlClient.SqlDataAdapter(cmd)
                Dim ds As New DataSet
                ' και ο SqlDataAdapter γεμίζει το DataSet
                da.Fill(ds, "tbl")

                'Ορισμός των παραμέτρων του ListBox
                oListBox.DataSource = ds.Tables(0) ' η βάση δεδομένων από το Dataset
                oListBox.DisplayMember = ListBoxText ' τιμή που θα φαίνεται στο ListBox
                oListBox.ValueMember = ListBoxValue ' η τιμή που θα αποδίδεται σαν value

                'Επιστρέφει τον συνολικό αριθμό εγγραφών του ListBox
                Return ds.Tables(0).Rows.Count

            End Using

        End Using
    End Function


    Public Shared Function ExecuteSQLTransact(ByVal sql As String) As String
        Dim myValue As String = ""

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sql, con)

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

        Return myValue

    End Function

    ' ************************************************************************************************************************
    ' ************************************************************************************************************************
    ''' <summary>FillComboBox: Γεμίζει ένα ComboBox με data από ένα επιλεγμένο DataBase 
    '''          με καθορισμό των DisplayMember και ValueMember
    ''' </summary>
    ''' <param name="SqlString">SQL Transact Text για τη νέα σύνδεση </param>
    ''' <param name="oComboBox">Το όνομα του ComboBox που θα γεμίσουμε</param>
    ''' <param name="ComboBoxText">To table που θα εμφανίζεται στο ComboBox</param>
    ''' <param name="ComboBoxValue">OPTIONAL: To table που θα χρησιμοποιείται σαν Value</param>
    ''' <remarks></remarks>
    ''' 

    Public Shared Sub FillComboBox(ByVal SqlString As String, ByVal oComboBox As ComboBox, ByVal ComboBoxText As String,
                                   Optional ByVal ComboBoxValue As String = "")
        'oComboBox.Items.Clear()

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
            Using cmd As New SqlClient.SqlCommand(SqlString, con)

                'που την ανοίγει εδώ
                con.Open()

                'Ορισμός νέου SqlDataAdapter και νέου DataSet
                Dim da As New SqlClient.SqlDataAdapter(cmd)
                Dim ds As New DataSet
                ' και ο SqlDataAdapter γεμίζει το DataSet
                da.Fill(ds, "tbl")

                'Ορισμός των παραμέτρων του ComboBox
                oComboBox.DataSource = ds.Tables(0) ' η βάση δεδομένων από το Dataset
                oComboBox.DisplayMember = ComboBoxText ' τιμή που θα φαίνεται στο ListBox
                If ComboBoxValue <> "" Then oComboBox.ValueMember = ComboBoxValue ' η τιμή που θα αποδίδεται σαν value

            End Using

        End Using
    End Sub



    Public Shared Sub FillMultipleComboBox(ByVal SqlString As String, ByVal oComboBox() As ComboBox, ByVal ComboBoxText() As String,
                                   ByVal ComboBoxValue() As String)
        'oComboBox.Items.Clear()
        Dim LastValue As String
        Try
            LastValue = oComboBox(0).Text
        Catch ex As Exception
        End Try


        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
            Using cmd As New SqlClient.SqlCommand(SqlString, con)

                'που την ανοίγει εδώ
                con.Open()

                'Ορισμός νέου SqlDataAdapter και νέου DataSet
                Dim da As New SqlClient.SqlDataAdapter(cmd)
                Dim ds As New DataSet
                ' και ο SqlDataAdapter γεμίζει το DataSet
                da.Fill(ds, "tbl")

                Try
                    'Ορισμός των παραμέτρων του ComboBox χρησιμοποιώντας το ΙΔΙΟ DATASOURCE
                    oComboBox(0).DataSource = ds.Tables("tbl").DefaultView ' η βάση δεδομένων από το Dataset
                    oComboBox(0).DisplayMember = ComboBoxText(0) ' τιμή που θα φαίνεται στο ListBox
                    If ComboBoxValue(0) <> "" Then oComboBox(0).ValueMember = ComboBoxValue(0) ' η τιμή που θα αποδίδεται σαν value

                    For t = 1 To oComboBox.Length - 1

                        'Ορισμός των παραμέτρων του ComboBox
                        oComboBox(t).BindingContext = New BindingContext ' αποσυνδέει τη βάση δεδομένων από το προηγούμενο combobox
                        oComboBox(t).DataSource = ds.Tables("tbl").DefaultView ' και τη συνδεει στο τρέχων
                        oComboBox(t).DisplayMember = ComboBoxText(t) ' τιμή που θα φαίνεται στο ListBox
                        If ComboBoxValue(t) <> "" Then oComboBox(t).ValueMember = ComboBoxValue(t) ' η τιμή που θα αποδίδεται σαν value

                    Next
                Catch ex As Exception
                End Try


            End Using

        End Using

        If LastValue <> "ΧΩΡΙΣ" Then
            oComboBox(0).Text = LastValue

        End If
    End Sub



    Public Shared Sub FillComboBox(ByVal oComboBox As ComboBox, ByVal itemsList() As String, ByVal otherItemsToAdd() As String)

        oComboBox.Items.Clear()

        For t = 0 To otherItemsToAdd.Length - 1
            If Not (otherItemsToAdd(t) Is Nothing) Then oComboBox.Items.Add(otherItemsToAdd(t))
        Next

        For t = 0 To itemsList.Length - 1
            If Not (itemsList(t) Is Nothing) Then oComboBox.Items.Add(itemsList(t))
        Next

    End Sub



    ' ****************************************************************************************************************************
    ' **********     Ρουτίνες διαχείρησης DataGridView     ***********************************************************************
    ' ****************************************************************************************************************************


    ' ΣΗΜΑΝΤΙΚΟ: Περάστε τις ακόλουθες μεταβλητές στο Global Variables:
    '               Public Shared stringDTG As String
    '               Public Shared cmdDTG As SqlCommand = Nothing
    '               Public Shared daDTG As SqlDataAdapter = Nothing
    '               Public Shared cbDTG As SqlCommandBuilder
    '               Public Shared dsDTG As DataSet = Nothing
    '               Public Shared dtDTG As DataTable = Nothing

    Public Shared Function FillDatagrid(ByVal strSQL As String, ByVal oDataGridView As DataGridView, ByVal oBindingSource As BindingSource,
                                   ByVal columnName() As String, ByVal columnWidth() As Integer, ByVal columnFormat() _
                                   As String, Optional ByVal FieldsToHide() As String = Nothing, Optional ByVal parentIndexField As String = " ", Optional ByVal AlterColor As Boolean = True) As Integer

        'MsgBox(oDataGridView.Name)

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Αντικαθιστά τη μονή απόστροφο με διπλή για να αποφύγει τα λάθη SQL Transact
        stringDTG = Replace(stringDTG, "N'S", "N''S")
        stringDTG = Replace(stringDTG, "L'O", "L''O")
        stringDTG = Replace(stringDTG, "e's", "e''s")

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(strSQL, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)

        dsDTG = New DataSet

        ' και ο SqlDataAdapter γεμίζει το DataSet
        daDTG.Fill(dsDTG, "DTG")

        dtDTG = dsDTG.Tables("DTG")

        oBindingSource.DataSource = dtDTG

        'που την ανοίγει εδώ
        con.Close()

        ' Εναλλαγή του χρωματισμού των rows
        If AlterColor = True Then
            oDataGridView.RowsDefaultCellStyle.BackColor = Color.Bisque
            oDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige
        End If

        'Γεμίζει το DataGrid από το DataSet
        oDataGridView.DataSource = oBindingSource



        Try
            For t = 0 To columnName.Length - 1
                'MsgBox(oDataGridView.Columns(0).HeaderText)
                oDataGridView.Columns(t).HeaderText = columnName(t) 'Βάζει τίτλο σε κάθε Column

                oDataGridView.Columns(t).Width = columnWidth(t) ' Αλλάζει το φάρδος του κάθε Column

                oDataGridView.Columns(t).DefaultCellStyle.Format = columnFormat(t)  ' Formatαρισμα των στοιχείων
            Next
        Catch ex As Exception
        End Try


        Try
            'Εξαφανίζει τα Πεδία που πρέπει να είναι κρυμένα
            If FieldsToHide Is Nothing Then
                ' Do nothing
            Else
                For t = 0 To FieldsToHide.Count - 1
                    oDataGridView.Columns(FieldsToHide(t)).Visible = False
                Next
            End If

        Catch ex As Exception

        End Try

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dsDTG.Tables(0).Rows.Count

    End Function


    Public Shared Function FillDatagrid(ByVal oDataGridView As DataGridView, ByVal oBindingSource As BindingSource,
                                   ByVal columnName() As String, ByVal columnWidth() As Integer, ByVal columnFormat() _
                                   As String, Optional ByVal FieldsToHide() As String = Nothing, Optional ByVal parentIndexField As String = " ", Optional ByVal AlterColor As Boolean = True) As Integer

                                                                                                                                                                                                         _

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Αντικαθιστά τη μονή απόστροφο με διπλή για να αποφύγει τα λάθη SQL Transact
        stringDTG = Replace(stringDTG, "N'S", "N''S")
        stringDTG = Replace(stringDTG, "L'O", "L''O")
        stringDTG = Replace(stringDTG, "e's", "e''s")

        Try
            'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
            cmdDTG = New SqlCommand(stringDTG, con)
            daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
            cbDTG = New SqlCommandBuilder(daDTG)

            dsDTG = New DataSet

            ' και ο SqlDataAdapter γεμίζει το DataSet
            daDTG.Fill(dsDTG, "DTG")

            dtDTG = dsDTG.Tables("DTG")

            oBindingSource.DataSource = dtDTG

            'που την ανοίγει εδώ
            con.Close()

            ' Εναλλαγή του χρωματισμού των rows
            If AlterColor = True Then
                oDataGridView.RowsDefaultCellStyle.BackColor = Color.Bisque
                oDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige
            End If

            'Γεμίζει το DataGrid από το DataSet
            oDataGridView.DataSource = oBindingSource

        Catch ex As Exception
        End Try


        Try
            For t = 0 To columnName.Length - 1
                'MsgBox(oDataGridView.Columns(0).HeaderText)
                oDataGridView.Columns(t).HeaderText = columnName(t) 'Βάζει τίτλο σε κάθε Column

                oDataGridView.Columns(t).Width = columnWidth(t) ' Αλλάζει το φάρδος του κάθε Column

                oDataGridView.Columns(t).DefaultCellStyle.Format = columnFormat(t)  ' Formatαρισμα των στοιχείων
            Next
        Catch ex As Exception
        End Try


        Try
            'Εξαφανίζει τα Πεδία που πρέπει να είναι κρυμένα
            If FieldsToHide Is Nothing Then
                ' Do nothing
            Else
                For t = 0 To FieldsToHide.Count - 1
                    oDataGridView.Columns(FieldsToHide(t)).Visible = False
                Next
            End If

        Catch ex As Exception

        End Try

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Try
            Return dsDTG.Tables(0).Rows.Count
        Catch ex As Exception
            MsgBox("ERROR (FillDatagrid) !!! " & vbCrLf & ex.Message)
        End Try


    End Function

    Public Shared Sub SetVariables()
        If My.Computer.Name = "CRAZYDR" Then
            strDBFolder = strDBFolder_Home
            connectionstring = connectionstring_Home
        ElseIf My.Computer.Name = "DESKTOP-B3F3GNN" Then
            strDBFolder = strDBFolder_Farm
            connectionstring = connectionstring_Farm1
        ElseIf My.Computer.Name = "DESKTOP-T7HMABG" Then
            strDBFolder = strDBFolder_Farm
            connectionstring = connectionstring_Farm
        ElseIf My.Computer.Name = "LAPTOP-4AJPEK4U" Then
            strDBFolder = strDBFolder_Laptop
            connectionstring = connectionstring_Laptop
        ElseIf My.Computer.Name = "NIKOYLA-PC" Or My.Computer.Name = "SALONI-PC" Then
            strDBFolder = strDBFolder_Home2
            connectionstring = connectionstring_Home2
        ElseIf My.Computer.Name = "BEDROOM-PC" Then
            MsgBox("Set Connestrion String values !!!")
        End If
    End Sub





    Public Shared Function GetDistinctContentsDBField(ByVal sqlstring As String, field As String) As Array
        Dim contents() As String

        SetVariables()

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlstring, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                Dim t As Integer = 0

                If myReader.HasRows Then

                    Do While myReader.Read()

                        ReDim Preserve contents(t)
                        contents(t) = myReader(field).ToString
                        t += 1

                    Loop
                Else

                End If

                ReDim Preserve contents(t)
                Return contents
            End Using
        End Using

    End Function



    Public Shared Function IsItAllreadyThere(ByVal sqlstring As String) As Boolean

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlstring, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then

                    Return True

                End If

            End Using
        End Using

        Return False

    End Function



    Public Shared Function IsItAllreadyThere(ByVal sqlstring As String, ByVal fieldId As String) As Integer

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlstring, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then
                    Do While myReader.Read()

                        Return myReader(fieldId)

                    Loop

                End If

            End Using
        End Using

        Return 0

    End Function


    Public Shared Sub UpdateDatagrid(ByVal oButtonArray() As Button, oDatagrid As DataGridView, Optional ByVal parentIndex As Integer = 0,
                                     Optional ByVal childField As String = "")
        'oButtonArray: Save(0), New(1), Delete(2)

        ' Ανανεώνει όλα τα records του datatable με το CustomerId του πελάτη

        If childField <> "" Then ' Εαν δεν πρόκειται για ορφανό datagrid (= χωρίς αναφορά σε άλλο  datagrid ή listbox)
            For t = 0 To dtDTG.Rows.Count - 1

                dtDTG.Rows(t).Item(childField) = parentIndex

                If dtDTG.Rows(t).Item("ExpMonth").ToString = "" And dtDTG.Rows(t).Item("ExpYear").ToString = "" And dtDTG.Rows(t).Item("BarCode").ToString = "" Then
                    If dtDTG.Rows(t).Item("DeliveryCode").ToString = "" And dtDTG.Rows(t).Item("DeliveryDate").ToString = "" And dtDTG.Rows(t).Item("FromWho").ToString = "" Then

                        dtDTG.Rows(t).Delete()

                    End If
                End If

            Next
        End If

        ' Ο dataAdapter περνάει τα δεδομένα στο Database
        daDTG.Update(dtDTG)

        ' Ενεργοποίηση Datatagrid και τροποποίηση κουμπιων
        oDatagrid.[ReadOnly] = True
        oButtonArray(0).Enabled = False
        oButtonArray(1).Enabled = True
        oButtonArray(2).Enabled = True

        ' REturn to the last selected row of datagrid
        Try
            oDatagrid.MultiSelect = False
            oDatagrid.ClearSelection()
            oDatagrid.Rows(lastRow).Cells(0).Selected = True
        Catch ex As Exception
        End Try

    End Sub






    Public Shared Sub UpdateDatagrid_ByName(ByVal oButtonArray() As Button, oDatagrid As DataGridView, Optional ByVal parentIndex As String = "", Optional ByVal childField As String = "")
        'oButtonArray: Save(0), New(1), Delete(2)

        ' Ανανεώνει όλα τα records του datatable με το CustomerId του πελάτη
        Dim row As DataRow
        If childField <> "" Then ' Εαν δεν πρόκειται για ορφανό datagrid (= χωρίς αναφορά σε άλλο  datagrid ή listbox)
            For Each row In dtDTG.Rows
                row.Item(childField) = parentIndex
            Next
        End If

        ' Ο dataAdapter περνάει τα δεδομένα στο Database
        daDTG.Update(dtDTG)

        ' Ενεργοποίηση Datatagrid και τροποποίηση κουμπιων
        oDatagrid.[ReadOnly] = True
        oButtonArray(0).Enabled = False
        oButtonArray(1).Enabled = True
        oButtonArray(2).Enabled = True

    End Sub



    Public Shared Sub DeleteDatagrid(ByVal oDatagrid As DataGridView)

        ' Μεταβλητές
        Dim id As Integer = 0

        Try
            id = oDatagrid.Rows(oDatagrid.SelectedRows(0).Index).Cells("Id").Value
        Catch ex As Exception
        End Try


        ' Επιβεβαίωση της διαγραφής
        If MessageBox.Show("Do you want to delete row # " & id & " ?", "Delete", MessageBoxButtons.YesNo) = DialogResult.Yes Then

            Dim selectedRow As Integer = oDatagrid.SelectedCells.Item(0).RowIndex


            ' Aφαίρεση της επιλεγμένης εγγραφής από το DataGRidView
            oDatagrid.Rows.Remove(oDatagrid.Rows(selectedRow))

            ' Περνάει το νέο DataTable στο Database
            daDTG.Update(dtDTG)

        End If
    End Sub



    Public Shared Sub DeleteRecordById(ByVal sqlString As String)
        Dim Sum As Double = 0
        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlString, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            End Using
        End Using
    End Sub



    Public Shared Sub EditDatagrid(ByVal oButtonArray() As Button, oDatagrid As DataGridView, ByVal selector As Boolean, Optional ByVal goToLast As Integer = 0)
        'oButtonArray: Save(0), Edit(1), Delete(2)
        ' Selector: True -> Edit
        '           False -> Cancel
        ' goToLast: 0 -> Editing στο επιλεγμένο row 
        '           1 -> Editing στο τελευταίο row 

        'Τροποποίηση DataGrid 
        oDatagrid.[ReadOnly] = Not selector
        oDatagrid.AllowUserToAddRows = selector

        'Τροποποίηση κουμπιών
        oButtonArray(0).Enabled = selector  'πλήκτρο Save

        If selector = True Then
            oButtonArray(1).Text = "Cancel" ' Αν το πλήκτρο Edit έχει πατηθεί μια φορά ετοιμάζεται για Editing..
            Try

                Try
                    ' Κρατάει στη μνήμη την τελευταία θέση του datagrid
                    lastRow = oDatagrid.SelectedRows(0).Index
                Catch ex As Exception
                End Try

                If goToLast = 0 Then

                    ' αρχίζει editing στο επιλεγμένο row
                    oDatagrid.BeginEdit(True)

                Else
                    ' Πηγαίνει στην τελευταία εγγραφή του datagrid και μπαίνει σε Edit Mode
                    Dim lastRow As Integer = 0
                    Try
                        lastRow = oDatagrid.Rows.GetLastRow(0)
                        ' If lastRow > 0 Then lastRow -= 1
                    Catch ex As Exception
                    End Try

                    oDatagrid.MultiSelect = False
                    oDatagrid.ClearSelection()

                    oDatagrid.Rows(lastRow).Cells(0).Selected = True

                    oDatagrid.BeginEdit(True)

                End If

            Catch ex As Exception
            End Try
        Else
            oButtonArray(1).Text = "Edit" ' Αν το πλήκτρο Edit έχει πατηθεί δεύτερη φορά ετοιμάζεται για Ακύρωση..

            ' REturn to the last selected row of datagrid
            Try
                oDatagrid.MultiSelect = False
                oDatagrid.ClearSelection()
                oDatagrid.Rows(lastRow).Cells(0).Selected = True
            Catch ex As Exception
            End Try

        End If

        oButtonArray(2).Enabled = selector 'πλήκτρο Delete




    End Sub

    ' ****************************************************************************************************************************
    ' **********     ΔΙΑΦΟΡΕΣ ΑΛΛΕΣ ΡΟΥΤΙΝΕΣ **********    ***********************************************************************
    ' ****************************************************************************************************************************


    Public Shared Sub HightlightInRichTextBox(ByVal oRichTextBox As RichTextBox, stringToFind() As String, Optional MyColor As String = "Red")
        Try

            ' Ψάχνει μια-μια τις λέξεις που πρέπει να χρωματίσει διαφορετικά (από το Array stringToFind)
            For t = 0 To stringToFind.Length - 1
                Dim token As String = If(stringToFind(t), String.Empty)
                If token = "" Then Continue For

                Dim foundAt As Integer = oRichTextBox.Find(token)
                If foundAt < 0 Then Continue For

                oRichTextBox.SelectionStart = foundAt ' θέση του τμήματος μέσα στο Rich TextBox
                oRichTextBox.SelectionLength = token.Length ' μήκος του τμήματος
                oRichTextBox.SelectionColor = Color.FromName(MyColor) ' αλλαγή χρώματος
            Next t

        Catch ex As Exception

        End Try

    End Sub

    Public Shared Function HasWhitespace(ByVal s As String) As Boolean
        If String.IsNullOrEmpty(s) Then Return False
        For Each ch As Char In s
            If Char.IsWhiteSpace(ch) Then Return True
        Next
        Return False
    End Function

    Public Shared Function GetFirstYYMM(input As String, Optional currentYear As Integer? = Nothing) As String
        If String.IsNullOrEmpty(input) OrElse input.Length < 6 Then Return Nothing

        Dim pattern As String = "17(?<yy>\d{2})(?<mm>0[1-9]|1[0-2])"
        Dim yyThreshold As Integer = If(currentYear.HasValue, currentYear.Value Mod 100, Date.Now.Year Mod 100)

        For Each m As Match In Regex.Matches(input, pattern)
            Dim yy As Integer = Integer.Parse(m.Groups("yy").Value)
            If yy >= yyThreshold Then
                Return m.Groups("yy").Value & m.Groups("mm").Value   ' επιστρέφει "YYMM"
            End If
        Next

        Return Nothing
    End Function


    Public Shared Sub FormatInRichTextBox(ByVal oRichTextBox As RichTextBox, stringToFind() As String, typeFormat As String, Optional MyColor As String = "Black")
        Try

            ' Ψάχνει μια-μια τις λέξεις που πρέπει να χρωματίσει διαφορετικά (από το Array stringToFind)
            For t = 0 To stringToFind.Length - 1
                oRichTextBox.SelectionStart = oRichTextBox.Find(stringToFind(t)) ' θέση του τμήματος μέσα στο Rich TextBox
                oRichTextBox.SelectionLength = Len(stringToFind(t)) ' μήκος του τμήματος
                oRichTextBox.SelectionColor = Color.FromName(MyColor)
                If typeFormat = "b" Then
                    oRichTextBox.SelectionFont = New Font(oRichTextBox.SelectionFont, FontStyle.Bold)
                ElseIf typeFormat = "i" Then
                    oRichTextBox.SelectionFont = New Font(oRichTextBox.SelectionFont, FontStyle.Italic)
                End If


                ' αλλαγή χρώματος
            Next t

        Catch ex As Exception

        End Try

    End Sub

    ' Μετατρέπει ελληνικούς look-alike χαρακτήρες σε λατινικούς και κρατά μόνο A-Z, 0-9 και "-"
    Public Shared Function NormalizeLotToLatin(ByVal s As String) As String
        If String.IsNullOrEmpty(s) Then Return ""

        Dim map As New Dictionary(Of Char, Char) From {
            {"Α"c, "A"c}, {"Β"c, "B"c}, {"Ε"c, "E"c}, {"Ζ"c, "Z"c}, {"Η"c, "H"c}, {"Ι"c, "I"c},
            {"Κ"c, "K"c}, {"Μ"c, "M"c}, {"Ν"c, "N"c}, {"Ο"c, "O"c}, {"Ρ"c, "P"c}, {"Τ"c, "T"c},
            {"Υ"c, "Y"c}, {"Χ"c, "X"c}, {"Σ"c, "S"c}, {"΢"c, "S"c}, {"ς"c, "S"c}, {"Δ"c, "D"c},
            {"Λ"c, "L"c}, {"Θ"c, "8"c} ' αν δεν θες Θ→8, άλλαξέ το σε "T" ή "Th" (και προσαρμόζεις το filter πιο κάτω)
        }

        Dim sb As New System.Text.StringBuilder(s.Length)
        For Each ch In s.ToUpperInvariant()
            If Char.IsLetterOrDigit(ch) OrElse ch = "-"c Then
                Dim outCh As Char = ch
                If map.ContainsKey(ch) Then outCh = map(ch)
                ' Κράτα μόνο A-Z, 0-9 και "-"
                If (outCh >= "A"c AndAlso outCh <= "Z"c) OrElse (outCh >= "0"c AndAlso outCh <= "9"c) OrElse outCh = "-"c Then
                    sb.Append(outCh)
                End If
            End If
        Next
        Return sb.ToString()
    End Function

    Public Shared Function CalculateSums(ByVal sqlString As String, ByVal fieldToSum As String) As Double
        Dim Sum As Double = 0
        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlString, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then
                    Do While myReader.Read()

                        If IsDBNull(myReader(fieldToSum)) = False Then
                            'Υπολογίζει το συνολικό ποσό
                            Sum += myReader(fieldToSum)
                        End If
                    Loop
                Else
                End If

                Return Sum
            End Using
        End Using

    End Function


    Public Shared Function GetLastUpdateFarmnet() As DateTime
        Dim LastUpdate As DateTime
        Dim sqlString = "USE [Pharmacy2013C] " &
                        "SELECT create_date, modify_date " &
                        "FROM sys.objects " &
                        "WHERE name = 'APOTIKH' "

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlString, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then
                    Do While myReader.Read()

                        LastUpdate = myReader(1)

                    Loop
                Else
                End If

                Return LastUpdate
            End Using
        End Using

    End Function


    Public Shared Function CalculateTotCount(ByVal sqlString As String) As Integer
        Dim Tot As Double = 0
        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlString, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then

                    Do While myReader.Read()
                        'Υπολογίζει το συνολικό ποσό
                        Tot += 1
                    Loop
                Else
                End If

                Return Tot
            End Using
        End Using

    End Function

    Public Shared Function GetQRFromScannedCode(ByVal scannedCode As String) As String
        Dim qr As String
        If Len(scannedCode) > 15 Then
            qr = scannedCode.Substring(2, 14)
        End If
        Return qr
    End Function

    Public Shared Function GetSerialFromScannedCode(ByVal scannedCode As String) As String
        If String.IsNullOrWhiteSpace(scannedCode) Then Return ""

        Dim startIdx As Integer = 0
        Dim productMatch As Match = Regex.Match(scannedCode, "01(\d{14})", RegexOptions.CultureInvariant)
        If productMatch.Success Then
            startIdx = productMatch.Index + productMatch.Length
        End If

        Dim payload As String = scannedCode.Substring(startIdx)
        If String.IsNullOrEmpty(payload) Then Return ""

        Dim i As Integer = 0
        While i >= 0 AndAlso i < payload.Length
            i = payload.IndexOf("21", i, StringComparison.Ordinal)
            If i < 0 Then Exit While

            Dim valueStart As Integer = i + 2
            If valueStart >= payload.Length Then Exit While

            Dim endIdx As Integer = payload.Length

            Dim fnc1 As Integer = payload.IndexOf(ChrW(29), valueStart)
            If fnc1 >= 0 Then endIdx = Math.Min(endIdx, fnc1)

            Dim nextAIs As String() = {"10", "11", "15", "17", "21", "30", "37", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99"}
            For Each ai In nextAIs
                Dim k As Integer = payload.IndexOf(ai, valueStart, StringComparison.Ordinal)
                If k >= 0 Then endIdx = Math.Min(endIdx, k)
            Next

            Dim serial As String = payload.Substring(valueStart, Math.Max(0, endIdx - valueStart)).Trim()
            If serial <> "" Then
                Return serial
            End If

            i = valueStart
        End While

        Return ""
    End Function

    Private Shared Function GetIDFromQRCodeSerial(serialNumber As String) As Integer
        If String.IsNullOrWhiteSpace(serialNumber) Then Return 0

        Dim result As Integer = 0
        Dim query As String =
            "SELECT TOP 1 D.TD_AP_ID " &
            "FROM dbo.SCANNED_TD_QR_CODES AS S " &
            "INNER JOIN dbo.DETAIL_TD AS D ON D.TD_TM_ID = S.SDQ_TM_ID AND D.TD_ID = S.SDQ_TD_ID " &
            "WHERE S.SDQ_SERIAL_NUMBER = @serial " &
            "AND D.TD_AP_ID IS NOT NULL " &
            "ORDER BY S.SDQ_TM_ID DESC, S.SDQ_TD_ID DESC"

        Using localCon As New SqlConnection(connectionstring)
            Using cmd As New SqlCommand(query, localCon)
                cmd.Parameters.AddWithValue("@serial", serialNumber)

                Try
                    localCon.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                        result = Convert.ToInt32(obj)
                    End If
                Catch ex As Exception
                    MessageBox.Show("SQL Error: " & ex.Message)
                End Try
            End Using
        End Using

        Return result
    End Function

    Public Shared Function GetIDFromQRCode(productCode As String) As Integer
        Dim result As Integer = 0

        If String.IsNullOrWhiteSpace(productCode) Then Return 0

        EnsureDrugQrCodeOverridesTable()

        Dim query As String =
            "SELECT TOP 1 T.AP_ID " &
            "FROM (" &
            "    SELECT QO.AP_ID, 1 AS SortOrder " &
            "    FROM [PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides] AS QO " &
            "    WHERE QO.QRCode = @code " &
            "    UNION ALL " &
            "    SELECT Q.APQ_AP_ID, 2 AS SortOrder " &
            "    FROM dbo.APOTIKH_QRCODES AS Q " &
            "    WHERE Q.APQ_PRODUCT_CODE = @code " &
            "    UNION ALL " &
            "    SELECT D.TD_AP_ID, 3 AS SortOrder " &
            "    FROM dbo.SCANNED_TD_QR_CODES AS S " &
            "    INNER JOIN dbo.DETAIL_TD AS D ON D.TD_TM_ID = S.SDQ_TM_ID AND D.TD_ID = S.SDQ_TD_ID " &
            "    WHERE S.SDQ_PRODUCT_CODE = @code " &
            ") AS T " &
            "WHERE T.AP_ID IS NOT NULL " &
            "ORDER BY T.SortOrder, T.AP_ID"

        Using localCon As New SqlConnection(connectionstring)
            Using cmd As New SqlCommand(query, localCon)
                cmd.Parameters.AddWithValue("@code", productCode)

                Try
                    localCon.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                        result = Convert.ToInt32(obj)
                    End If
                Catch ex As Exception
                    MessageBox.Show("SQL Error: " & ex.Message)
                End Try
            End Using
        End Using

        Return result
    End Function

    Public Shared Function GetIDFromScannedQRCode(scannedCode As String) As Integer
        If String.IsNullOrWhiteSpace(scannedCode) Then Return 0

        Dim productCode As String = GetQRFromScannedCode(scannedCode)
        Dim result As Integer = GetIDFromQRCode(productCode)
        If result > 0 Then Return result

        Dim serialNumber As String = GetSerialFromScannedCode(scannedCode)
        If serialNumber = "" Then Return 0

        Return GetIDFromQRCodeSerial(serialNumber)
    End Function


    Public Shared Function GetIDFromBarCode(productCode As String) As Integer
        Dim result As Integer = 0
        Dim connStr As String = "Data Source=SERVERNAME;Initial Catalog=Pharmacy2013C;Integrated Security=True"
        ' Αν χρησιμοποιείς SQL authentication, βάζεις: User ID=xxx;Password=yyy;

        Dim query As String = "SELECT BRAP_AP_ID FROM dbo.APOTIKH_BARCODES WHERE BRAP_AP_BARCODE = @code"

        'Using conn As New SqlConnection(connStr)
        Using cmd As New SqlCommand(query, con)
            cmd.Parameters.AddWithValue("@code", productCode)

            Try
                con.Open()
                Dim obj = cmd.ExecuteScalar()
                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    result = Convert.ToInt32(obj)
                End If
            Catch ex As Exception
                MessageBox.Show("SQL Error: " & ex.Message)
                End Try
            End Using
        'End Using
        con.Close()
        Return result
    End Function

    Public Shared Function IsProcessElevated() As Boolean
        Using id = WindowsIdentity.GetCurrent()
            Dim p = New WindowsPrincipal(id)
            Return p.IsInRole(WindowsBuiltInRole.Administrator)
        End Using
    End Function


    Public Shared Sub RelaunchAsAdmin()
        Dim psi As New ProcessStartInfo(Application.ExecutablePath) With {
        .UseShellExecute = True,
        .Verb = "runas",                 ' ζητά UAC
        .WorkingDirectory = Application.StartupPath
    }
        Try
            Process.Start(psi)
            Application.Exit()
        Catch ex As Win32Exception When ex.NativeErrorCode = 1223
            ' Ο χρήστης πάτησε “Όχι” στο UAC
            MessageBox.Show("Ακυρώθηκε η ανύψωση δικαιωμάτων (UAC).")
        End Try
    End Sub

    Public Shared Function GetInfoDB(ByVal myDB As String, ByVal table As String, ByVal mode As String) As String
        Dim myDateOrig, myDate As DateTime
        Dim sqlString As String = "USE " & myDB & " " & _
                                      "SELECT OBJECT_NAME(OBJECT_ID) AS DatabaseName, last_user_update,* " & _
                                    "FROM sys.dm_db_index_usage_stats " & _
                                    "WHERE database_id = DB_ID( '" & myDB & "') " & _
                                    "AND OBJECT_ID=OBJECT_ID('" & table & "')"

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlString, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then
                    Do While myReader.Read()
                        If mode = "modified" And IsDBNull(myReader(1)) = False Then
                            myDateOrig = myReader(1)
                            'myDate = Format(myReader(1), "dd-MM-yyyy, HH:mm")
                        ElseIf mode = "scanned" And IsDBNull(myReader(0)) = False Then
                            myDateOrig = myReader(0)
                            'myDate = Format(myReader(0), "dd-MM-yyyy, HH:mm")
                        End If
                    Loop
                Else
                End If

            End Using
        End Using

        If myDateOrig <> "#12:00:00 AM#" Then
            'MsgBox("Result = " & myDate)
            SaveLastUpdatedDate("LastUpdated_" & table, myDateOrig)
            myDate = myDateOrig
        Else
            myDate = GetLastUpdatedDate("LastUpdated_" & table)
        End If

        Return Format(myDate, "dd-MM-yyyy, HH:mm")

    End Function


    Public Shared Sub SaveLastUpdatedDate(ByVal table As String, ByVal lastUpdated As Date)
        Dim insertData As String = ""
        Dim index As Integer = 0

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            insertData = "UPDATE [PharmacyCustomFiles].[dbo].[Variables] " & _
                        "SET [" & table & "] = @table " & _
                        "WHERE Id = 0"

            Dim cmd As New SqlCommand(insertData, con)

            ' Περνάει τις παραμέτρους του SQL 
            cmd.Parameters.AddWithValue("@table", lastUpdated)

            cmd.ExecuteNonQuery()

        End Using
    End Sub


    Public Shared Function GetLastUpdatedDate(ByVal table As String) As Date
        Dim sqlString As String = ""
        Dim lastUpdatedDate As Date

        sqlString = "SELECT " & table & " " & _
                    "FROM [PharmacyCustomFiles].[dbo].[Variables] " & _
                     "WHERE Id = 0"

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlString, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then
                    Do While myReader.Read()
                        lastUpdatedDate = myReader(table)
                    Loop
                Else
                End If

            End Using
        End Using

        Return lastUpdatedDate

    End Function

    Public Shared Function GetDatabaseStatus(ByVal dtb As String) As String
        Dim sqlString As String = ""
        Dim DatabaseStatus As String

        sqlString = "SELECT distinct '" & dtb & "', state_desc FROM sys.databases"

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlString, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then
                    Do While myReader.Read()
                        DatabaseStatus = myReader(1)
                    Loop
                Else
                End If

            End Using
        End Using

        Return DatabaseStatus

    End Function


    Public Shared Function CalculateRecords(ByVal sqlString As String) As Double
        Dim MyCount As Double = 0
        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        Using con As New SqlClient.SqlConnection(connectionstring)

            'Initialization νέου CommandAdapter με την Stringα αναζήτησης και την σύνδεση
            Using cmd As New SqlClient.SqlCommand(sqlString, con)

                ' Ανοίγει την σύνδεση
                con.Open()

                'Ορισμός ExecuteReader 
                Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If myReader.HasRows Then
                    Do While myReader.Read()

                        MyCount += 1
                    Loop
                Else
                End If

                Return MyCount
            End Using
        End Using

    End Function



    Public Shared Sub HightlightInRichTextBoxGreen(ByVal oRichTextBox As RichTextBox, stringToFind() As String)
        Try

            ' Ψάχνει μια-μια τις λέξεις που πρέπει να χρωματίσει διαφορετικά (από το Array stringToFind)
            For t = 0 To stringToFind.Length - 1
                Dim token As String = If(stringToFind(t), String.Empty)
                If token = "" Then Continue For

                Dim foundAt As Integer = oRichTextBox.Find(token)
                If foundAt < 0 Then Continue For

                oRichTextBox.SelectionStart = foundAt ' θέση του τμήματος μέσα στο Rich TextBox
                oRichTextBox.SelectionLength = token.Length ' μήκος του τμήματος
                oRichTextBox.SelectionColor = Color.Green ' αλλαγή χρώματος
            Next t

        Catch ex As Exception

        End Try

    End Sub


    Public Shared Sub PopulateComboWithMonths(ByVal oComboBox As ComboBox)

        Dim months() As String = {"Όλοι οι μήνες", "Ιανουάριος", "Φεβρουάριος", "Μάρτιος", "Απρίλιος", "Μάιος", _
                                  "Ιούνιος", "Ιούλιος", "Αύγουστος", "Σεπτέμβριος", "Οκτώμβριος", "Νοέμβριος", "Δεκέμβριος"}
        For Each Month As String In months
            oComboBox.Items.Add(Month)
        Next

        'Dim months As New Dictionary(Of Int32, [String])()
        'months.Add(0, "Low")
        'months.Add(1, "Ιανουάριος")
        'months.Add(2, "Φεβρουάριος")
        'months.Add(3, "Μάρτιος")
        'months.Add(4, "Απρίλιος")
        'months.Add(5, "Μάιος")
        'months.Add(6, "Ιούνιος")
        'months.Add(7, "Ιούλιος")
        'months.Add(8, "Αύγουστος")
        'months.Add(9, "Σεπτέμβριος")
        'months.Add(10, "Οκτώμβριος")
        'months.Add(11, "Νοέμβριος")
        'months.Add(12, "Δεκέμβριος")

        'oComboBox.DataSource = New BindingSource(months, Nothing)
        'oComboBox.DisplayMember = "Value"
        'oComboBox.ValueMember = "Key"
        ''oComboBox.SelectedIndexChanged += New EventHandler(AddressOf oComboBox_SelectedIndexChanged)
        'oComboBox.DropDownStyle = ComboBoxStyle.DropDownList

    End Sub


    Public Shared Function GetNumericMonthFromName(ByVal month As String) As Integer
        Dim months() As String = {"Όλοι οι μήνες", "Ιανουάριος", "Φεβρουάριος", "Μάρτιος", "Απρίλιος", "Μάιος", _
                                 "Ιούνιος", "Ιούλιος", "Αύγουστος", "Σεπτέμβριος", "Οκτώμβριος", "Νοέμβριος", "Δεκέμβριος"}
        For t = 0 To 12
            If months(t) = month Then
                Return t
            End If
        Next
        Return Nothing
    End Function


    Public Shared Function FillDatatableWithComboBoxItems() As DataTable

        Dim sql As String = "SELECT [AP_DESCRIPTION], [AP_CODE] FROM [dbo].[APOTIKH] order by [AP_DESCRIPTION]"

        'Initialization νεας σύνδεσης με το connectionString που παίρνει από τις GlobalVariables
        con = New SqlConnection(connectionstring)

        'που την ανοίγει εδώ
        con.Open()

        'Initialization νέου CommandAdapter με την SqlString που παίρνει σαν εξωτερική παράμετρο
        cmdDTG = New SqlCommand(sql, con)
        daDTG = New SqlDataAdapter(cmdDTG)  'Ορισμός νέου SqlDataAdapter και νέου DataSet
        cbDTG = New SqlCommandBuilder(daDTG)

        dsDTG = New DataSet
        dtDTG = New DataTable

        ' και ο SqlDataAdapter γεμίζει το DataSet
        daDTG.Fill(dsDTG, "DTG")

        dtDTG = dsDTG.Tables("DTG")

        'που την ανοίγει εδώ
        con.Close()

        'Επιστρέφει τον συνολικό αριθμό εγγραφών του Datagrid
        Return dtDTG

    End Function


    Public Shared Sub UpdatePrescriptionInfoFromDatagrid()
        Dim prescriptionId As String
        Try
            frmPrescriptionInfo.lblCustomerName.Text = frmCustomers.dgvCustomers.SelectedRows(0).Cells(0).Value
        Catch ex As Exception
        End Try

        Try
            prescriptionId = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(6).Value
        Catch ex As Exception
            prescriptionId = ""
        End Try

        If prescriptionId <> "" Then
            frmPrescriptionInfo.btnEnterPrescription.Text = "Τροποποίηση"
            'Ektelesis
            frmPrescriptionInfo.cboEktelesis.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(0).Value
            'InitDate
            frmPrescriptionInfo.dtpInitDate.Value = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(1).Value
            'EndDate
            frmPrescriptionInfo.dtpEndDate.Value = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(2).Value
            'ProcessedDate
            Try
                frmPrescriptionInfo.dtpProcessedDate.Value = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(4).Value
                'MsgBox(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(4).Value)
            Catch ex As Exception
                frmPrescriptionInfo.dtpProcessedDate.Checked = False
            End Try
            ' Barcode
            frmPrescriptionInfo.txtBarcode.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(3).Value

            ' Drugs
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(7).Value) = True Then  ' Drug1
                frmPrescriptionInfo.cboDrug1.Text = ""
            Else
                frmPrescriptionInfo.cboDrug1.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(7).Value
            End If
            '--------------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(8).Value) = True Then  ' Drug2
                frmPrescriptionInfo.cboDrug2.Text = ""
            Else
                frmPrescriptionInfo.cboDrug2.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(8).Value
            End If
            '----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(9).Value) = True Then  ' Drug3
                frmPrescriptionInfo.cboDrug3.Text = ""
            Else
                frmPrescriptionInfo.cboDrug3.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(9).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(10).Value) = True Then  ' Drug4
                frmPrescriptionInfo.cboDrug4.Text = ""
            Else
                frmPrescriptionInfo.cboDrug4.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(10).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(11).Value) = True Then  ' Drug5
                frmPrescriptionInfo.cboDrug5.Text = ""
            Else
                frmPrescriptionInfo.cboDrug5.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(11).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(12).Value) = True Then  ' Drug6
                frmPrescriptionInfo.cboDrug6.Text = ""
            Else
                frmPrescriptionInfo.cboDrug6.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(12).Value
            End If
            '--------------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(13).Value) = True Then  ' Drug7
                frmPrescriptionInfo.cboDrug7.Text = ""
            Else
                frmPrescriptionInfo.cboDrug7.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(13).Value
            End If
            '----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(14).Value) = True Then  ' Drug8
                frmPrescriptionInfo.cboDrug8.Text = ""
            Else
                frmPrescriptionInfo.cboDrug8.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(14).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(15).Value) = True Then  ' Drug9
                frmPrescriptionInfo.cboDrug9.Text = ""
            Else
                frmPrescriptionInfo.cboDrug9.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(15).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(16).Value) = True Then  ' Drug10
                frmPrescriptionInfo.cboDrug10.Text = ""
            Else
                frmPrescriptionInfo.cboDrug10.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(16).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(17).Value) = True Then  ' Drug11
                frmPrescriptionInfo.cboDrug11.Text = ""
            Else
                frmPrescriptionInfo.cboDrug11.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(17).Value
            End If
            '--------------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(18).Value) = True Then  ' Drug12
                frmPrescriptionInfo.cboDrug12.Text = ""
            Else
                frmPrescriptionInfo.cboDrug12.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(18).Value
            End If
            '----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(19).Value) = True Then  ' Drug13
                frmPrescriptionInfo.cboDrug13.Text = ""
            Else
                frmPrescriptionInfo.cboDrug13.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(19).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(20).Value) = True Then  ' Drug14
                frmPrescriptionInfo.cboDrug14.Text = ""
            Else
                frmPrescriptionInfo.cboDrug14.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(20).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(21).Value) = True Then  ' Drug15
                frmPrescriptionInfo.cboDrug15.Text = ""
            Else
                frmPrescriptionInfo.cboDrug15.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(21).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(22).Value) = True Then  ' Drug16
                frmPrescriptionInfo.cboDrug16.Text = ""
            Else
                frmPrescriptionInfo.cboDrug16.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(22).Value
            End If
            '--------------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(23).Value) = True Then  ' Drug17
                frmPrescriptionInfo.cboDrug17.Text = ""
            Else
                frmPrescriptionInfo.cboDrug17.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(23).Value
            End If
            '----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(24).Value) = True Then  ' Drug18
                frmPrescriptionInfo.cboDrug18.Text = ""
            Else
                frmPrescriptionInfo.cboDrug18.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(24).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(25).Value) = True Then  ' Drug19
                frmPrescriptionInfo.cboDrug19.Text = ""
            Else
                frmPrescriptionInfo.cboDrug19.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(25).Value
            End If
            '-----------------------
            If IsDBNull(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(26).Value) = True Then  ' Drug20
                frmPrescriptionInfo.cboDrug20.Text = ""
            Else
                frmPrescriptionInfo.cboDrug20.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(26).Value
            End If
            '-----------------------

            ' Analosima
            Try
                frmPrescriptionInfo.chkAnalosima.Checked = CType(frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(17).Value, Boolean)
            Catch ex As Exception
            End Try

            'Notes
            Try
                frmPrescriptionInfo.txtNotes.Text = frmCustomers.dgvPrescriptions.SelectedRows(0).Cells(18).Value
            Catch ex As Exception
                frmPrescriptionInfo.txtNotes.Text = ""
            End Try



        Else
            frmPrescriptionInfo.btnEnterPrescription.Text = "Νέα εγγραφή"
            'Ektelesis
            frmPrescriptionInfo.cboEktelesis.Text = "2η"
            'InitDate
            If dtInitDate.HasValue Then
                frmPrescriptionInfo.dtpInitDate.Value = dtInitDate
            Else
                frmPrescriptionInfo.dtpInitDate.Value = Today()
            End If

            'EndDate
            If dtEndDate.HasValue Then
                frmPrescriptionInfo.dtpEndDate.Value = dtEndDate
            Else
                frmPrescriptionInfo.dtpEndDate.Value = Today()
            End If

            'ProcessedDate
            If frmPrescriptionInfo.dtpProcessedDate.Checked = True Then
                frmPrescriptionInfo.dtpProcessedDate.Checked = False
            End If
            ' Barcode
            frmPrescriptionInfo.txtBarcode.Text = ""
            ' Drug1
            frmPrescriptionInfo.cboDrug1.Text = "ΧΩΡΙΣ"
            frmPrescriptionInfo.cboDrug2.Text = "ΧΩΡΙΣ"
            frmPrescriptionInfo.cboDrug3.Text = "ΧΩΡΙΣ"
            frmPrescriptionInfo.cboDrug4.Text = "ΧΩΡΙΣ"
            frmPrescriptionInfo.cboDrug5.Text = "ΧΩΡΙΣ"
            frmPrescriptionInfo.cboDrug6.Text = "ΧΩΡΙΣ"
            frmPrescriptionInfo.cboDrug7.Text = "ΧΩΡΙΣ"
            frmPrescriptionInfo.cboDrug8.Text = "ΧΩΡΙΣ"
            frmPrescriptionInfo.cboDrug9.Text = "ΧΩΡΙΣ"
            frmPrescriptionInfo.cboDrug10.Text = "ΧΩΡΙΣ"
            ' Analosima
            'frmPrescriptionInfo.chkAnalosima.Checked = False
            'Notes
            frmPrescriptionInfo.txtNotes.Text = ""

        End If


        For Each ctl As Control In frmPrescriptionInfo.Controls
            For Each cmb As ComboBox In ctl.Controls.OfType(Of ComboBox)()
                If cmb.Text = "ΧΩΡΙΣ" Or cmb.Text = "XΩΡΙΣ" Or cmb.Text = "" Then
                    cmb.Enabled = False
                Else
                    cmb.Enabled = True
                    Dim strSQL As String = "SELECT [DR_CODE], [DR_ID], [DR_DESCRIPTION] FROM [Pharmacy2013C].[dbo].[DROYSIA]"
                    FillMultipleComboBox(strSQL, {cmb}, {"DR_DESCRIPTION"}, {"DR_ID"})
                End If
            Next
        Next

    End Sub



    Public Shared Sub StopService(ByVal myServiceName As String)

        Dim DataSource As String = My.Computer.Name
        Dim sStatus As String
        Dim myController As ServiceController

        myController = New ServiceController
        'myController.MachineName = DataSource
        myController.ServiceName = myServiceName


        frmCustomers.lstMessage.Items.Add("Stopping service """ & myServiceName & """....")
        lstIndex += 1
        frmCustomers.lstMessage.Refresh()
        Try
            If myController.Status = ServiceProcess.ServiceControllerStatus.Stopped Then
                frmCustomers.lstMessage.Items(lstIndex) &= "Already stopped"
            Else

                Try
                    myController.Refresh()
                    sStatus = myController.Status.ToString
                    myController.Stop()
                    myController.WaitForStatus(ServiceControllerStatus.Stopped)
                    frmCustomers.lstMessage.Items(lstIndex) &= "Stopped"
                Catch exp As Exception
                    frmCustomers.lstMessage.Items(lstIndex) &= "Failed"
                End Try
                frmCustomers.lstMessage.Refresh()
            End If
        Catch ex As Exception

            frmCustomers.lstMessage.Items(lstIndex) &= "Service doesn't exist?!"
        End Try

    End Sub


    Public Shared Function IsAlphaNumericKey(e As KeyEventArgs) As Boolean
        ' Ελέγχει αν το πλήκτρο είναι γράμμα ή αριθμός
        ' Αγνοεί λειτουργικά πλήκτρα, arrows, delete, backspace, κλπ.

        ' A–Z
        If e.KeyCode >= Keys.A AndAlso e.KeyCode <= Keys.Z Then
            Return True
        End If

        ' 0–9 (επάνω σειρά)
        If e.KeyCode >= Keys.D0 AndAlso e.KeyCode <= Keys.D9 Then
            Return True
        End If

        ' 0–9 (αριθμητικό πληκτρολόγιο)
        If e.KeyCode >= Keys.NumPad0 AndAlso e.KeyCode <= Keys.NumPad9 Then
            Return True
        End If

        ' Οτιδήποτε άλλο -> False
        Return False
    End Function



    Public Shared Sub StartService(ByVal myServiceName As String)

        Dim DataSource As String = My.Computer.Name
        Dim sStatus As String
        Dim myController As ServiceController

        myController = New ServiceController
        myController.MachineName = DataSource
        myController.ServiceName = myServiceName

        frmCustomers.lstMessage.Items.Add("Starting service """ & myServiceName & """....")
        lstIndex += 1
        frmCustomers.lstMessage.Refresh()
        Try
            myController.Refresh()
            sStatus = myController.Status.ToString
            myController.Start()
            myController.WaitForStatus(ServiceControllerStatus.Running)
            frmCustomers.lstMessage.Items(lstIndex) &= "Started"
        Catch exp As Exception
            frmCustomers.lstMessage.Items(lstIndex) &= "Failed"
        End Try
        frmCustomers.lstMessage.Refresh()

    End Sub


    Public Shared Sub EnsureDrugXondrOverridesTable()
        Dim sql As String = "IF OBJECT_ID('[PharmacyCustomFiles].[dbo].[DrugXondrOverrides]', 'U') IS NULL " & _
                            "BEGIN " & _
                            "CREATE TABLE [PharmacyCustomFiles].[dbo].[DrugXondrOverrides](" & _
                            "[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY, " & _
                            "[AP_Code] [nvarchar](50) NOT NULL, " & _
                            "[DrugName] [nvarchar](255) NULL, " & _
                            "[UnitXondr] [money] NOT NULL, " & _
                            "[LastUpdated] [datetime] NOT NULL CONSTRAINT [DF_DrugXondrOverrides_LastUpdated] DEFAULT (GETDATE())" & _
                            "); " & _
                            "CREATE UNIQUE INDEX [IX_DrugXondrOverrides_AP_Code] ON [PharmacyCustomFiles].[dbo].[DrugXondrOverrides]([AP_Code]); " & _
                            "END"

        Using con As New SqlClient.SqlConnection(connectionstring)
            Using cmd As New SqlClient.SqlCommand(sql, con)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub EnsureDrugQrCodeOverridesTable()
        Dim sql As String = "IF OBJECT_ID('[PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides]', 'U') IS NULL " & _
                            "BEGIN " & _
                            "CREATE TABLE [PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides](" & _
                            "[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY, " & _
                            "[AP_ID] [bigint] NOT NULL, " & _
                            "[AP_Code] [nvarchar](50) NULL, " & _
                            "[DrugName] [nvarchar](255) NULL, " & _
                            "[QRCode] [nvarchar](100) NOT NULL, " & _
                            "[LastUpdated] [datetime] NOT NULL CONSTRAINT [DF_DrugQrCodeOverrides_LastUpdated] DEFAULT (GETDATE())" & _
                            "); " & _
                            "CREATE UNIQUE INDEX [IX_DrugQrCodeOverrides_AP_ID] ON [PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides]([AP_ID]); " & _
                            "CREATE INDEX [IX_DrugQrCodeOverrides_QRCode] ON [PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides]([QRCode]); " & _
                            "END"

        Using con As New SqlClient.SqlConnection(connectionstring)
            Using cmd As New SqlClient.SqlCommand(sql, con)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function GetDrugQrCodeOverride(ByVal apId As Long) As String
        If apId <= 0 Then Return Nothing

        EnsureDrugQrCodeOverridesTable()

        Dim sql As String = "SELECT TOP 1 QRCode FROM [PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides] WHERE AP_ID = @AP_ID"

        Using con As New SqlClient.SqlConnection(connectionstring)
            Using cmd As New SqlClient.SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@AP_ID", apId)
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result Is Nothing OrElse IsDBNull(result) Then Return Nothing

                Return Convert.ToString(result).Trim()
            End Using
        End Using
    End Function

    Public Shared Function HasDrugQrCodeOverride(ByVal apId As Long) As Boolean
        Return Not String.IsNullOrWhiteSpace(GetDrugQrCodeOverride(apId))
    End Function

    Public Shared Function GetEffectiveDrugQrCode(ByVal apId As Long) As String
        If apId <= 0 Then Return Nothing

        EnsureDrugQrCodeOverridesTable()

        Dim sql As String =
            "SELECT TOP 1 ISNULL(NULLIF(QO.QRCode, ''), Q.APQ_PRODUCT_CODE) AS EffectiveQRCode " &
            "FROM dbo.APOTIKH AS A " &
            "LEFT JOIN dbo.APOTIKH_QRCODES AS Q ON Q.APQ_AP_ID = A.AP_ID " &
            "LEFT JOIN [PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides] AS QO ON QO.AP_ID = A.AP_ID " &
            "WHERE A.AP_ID = @AP_ID"

        Using con As New SqlClient.SqlConnection(connectionstring)
            Using cmd As New SqlClient.SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@AP_ID", apId)
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result Is Nothing OrElse IsDBNull(result) Then Return Nothing

                Return Convert.ToString(result).Trim()
            End Using
        End Using
    End Function

    Public Shared Sub SaveDrugQrCodeOverride(ByVal apId As Long, ByVal apCode As String, ByVal drugName As String, ByVal qrCode As String)
        If apId <= 0 Then Exit Sub

        EnsureDrugQrCodeOverridesTable()

        Dim trimmedQr As String = If(qrCode, String.Empty).Trim()

        Using con As New SqlClient.SqlConnection(connectionstring)
            con.Open()

            If trimmedQr = "" Then
                Using cmd As New SqlClient.SqlCommand(
                    "DELETE FROM [PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides] WHERE AP_ID = @AP_ID", con)
                    cmd.Parameters.AddWithValue("@AP_ID", apId)
                    cmd.ExecuteNonQuery()
                End Using
                Exit Sub
            End If

            Dim sql As String = "IF EXISTS (SELECT 1 FROM [PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides] WHERE AP_ID = @AP_ID) " & _
                                "BEGIN " & _
                                "UPDATE [PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides] " & _
                                "SET AP_Code = @AP_Code, DrugName = @DrugName, QRCode = @QRCode, LastUpdated = GETDATE() " & _
                                "WHERE AP_ID = @AP_ID " & _
                                "END " & _
                                "ELSE " & _
                                "BEGIN " & _
                                "INSERT INTO [PharmacyCustomFiles].[dbo].[DrugQrCodeOverrides] ([AP_ID], [AP_Code], [DrugName], [QRCode]) " & _
                                "VALUES (@AP_ID, @AP_Code, @DrugName, @QRCode) " & _
                                "END"

            Using cmd As New SqlClient.SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@AP_ID", apId)
                cmd.Parameters.AddWithValue("@AP_Code", If(String.IsNullOrWhiteSpace(apCode), DBNull.Value, CType(apCode.Trim(), Object)))
                cmd.Parameters.AddWithValue("@DrugName", If(String.IsNullOrWhiteSpace(drugName), DBNull.Value, CType(drugName.Trim(), Object)))
                cmd.Parameters.AddWithValue("@QRCode", trimmedQr)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function GetDrugXondrOverride(ByVal apCode As String) As Nullable(Of Decimal)
        If String.IsNullOrWhiteSpace(apCode) Then Return Nothing

        EnsureDrugXondrOverridesTable()

        Dim sql As String = "SELECT TOP 1 UnitXondr FROM [PharmacyCustomFiles].[dbo].[DrugXondrOverrides] WHERE AP_Code = @AP_Code"

        Using con As New SqlClient.SqlConnection(connectionstring)
            Using cmd As New SqlClient.SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@AP_Code", apCode.Trim())
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result Is Nothing OrElse IsDBNull(result) Then Return Nothing

                Return CType(result, Decimal)
            End Using
        End Using
    End Function

    Public Shared Function GetEffectiveDrugXondr(ByVal apCode As String, Optional ByVal fallbackXondr As Decimal = -1D) As Decimal
        Dim overrideXondr = GetDrugXondrOverride(apCode)
        If overrideXondr.HasValue Then Return overrideXondr.Value

        If fallbackXondr >= 0D Then Return fallbackXondr

        Dim sql As String = "SELECT AP_TIMH_XON FROM APOTIKH WHERE AP_CODE = @AP_CODE"

        Using con As New SqlClient.SqlConnection(connectionstring)
            Using cmd As New SqlClient.SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@AP_CODE", apCode)
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result Is Nothing OrElse IsDBNull(result) Then Return 0D

                Return CType(result, Decimal)
            End Using
        End Using
    End Function

    Public Shared Sub SaveDrugXondrOverride(ByVal apCode As String, ByVal drugName As String, ByVal unitXondr As Decimal)
        If String.IsNullOrWhiteSpace(apCode) Then Exit Sub

        EnsureDrugXondrOverridesTable()

        Dim sql As String = "IF EXISTS (SELECT 1 FROM [PharmacyCustomFiles].[dbo].[DrugXondrOverrides] WHERE AP_Code = @AP_Code) " & _
                            "BEGIN " & _
                            "UPDATE [PharmacyCustomFiles].[dbo].[DrugXondrOverrides] " & _
                            "SET DrugName = @DrugName, UnitXondr = @UnitXondr, LastUpdated = GETDATE() " & _
                            "WHERE AP_Code = @AP_Code " & _
                            "END " & _
                            "ELSE " & _
                            "BEGIN " & _
                            "INSERT INTO [PharmacyCustomFiles].[dbo].[DrugXondrOverrides] ([AP_Code], [DrugName], [UnitXondr]) " & _
                            "VALUES (@AP_Code, @DrugName, @UnitXondr) " & _
                            "END"

        Using con As New SqlClient.SqlConnection(connectionstring)
            Using cmd As New SqlClient.SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@AP_Code", apCode.Trim())
                cmd.Parameters.AddWithValue("@DrugName", If(String.IsNullOrWhiteSpace(drugName), DBNull.Value, CType(drugName.Trim(), Object)))
                cmd.Parameters.AddWithValue("@UnitXondr", unitXondr)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
End Class
