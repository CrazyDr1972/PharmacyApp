<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPrescriptionsExpiring
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.grpPrescriptionsExpired = New System.Windows.Forms.GroupBox()
        Me.dgvPrescriptionsExpired = New System.Windows.Forms.DataGridView()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.dtpInitDate = New System.Windows.Forms.DateTimePicker()
        Me.btnProcessPrescr = New System.Windows.Forms.Button()
        Me.chkSelectAll = New System.Windows.Forms.CheckBox()
        Me.dtpProcessDate = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.chkDefaultDateProcessed = New System.Windows.Forms.CheckBox()
        Me.grpPrescriptionsExpired.SuspendLayout()
        CType(Me.dgvPrescriptionsExpired, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'grpPrescriptionsExpired
        '
        Me.grpPrescriptionsExpired.Controls.Add(Me.dgvPrescriptionsExpired)
        Me.grpPrescriptionsExpired.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.grpPrescriptionsExpired.Location = New System.Drawing.Point(17, 100)
        Me.grpPrescriptionsExpired.Name = "grpPrescriptionsExpired"
        Me.grpPrescriptionsExpired.Size = New System.Drawing.Size(482, 316)
        Me.grpPrescriptionsExpired.TabIndex = 44
        Me.grpPrescriptionsExpired.TabStop = False
        '
        'dgvPrescriptionsExpired
        '
        Me.dgvPrescriptionsExpired.AllowUserToAddRows = False
        Me.dgvPrescriptionsExpired.AllowUserToDeleteRows = False
        Me.dgvPrescriptionsExpired.AllowUserToResizeColumns = False
        Me.dgvPrescriptionsExpired.AllowUserToResizeRows = False
        Me.dgvPrescriptionsExpired.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPrescriptionsExpired.Location = New System.Drawing.Point(10, 32)
        Me.dgvPrescriptionsExpired.Name = "dgvPrescriptionsExpired"
        Me.dgvPrescriptionsExpired.ReadOnly = True
        Me.dgvPrescriptionsExpired.RowHeadersVisible = False
        Me.dgvPrescriptionsExpired.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvPrescriptionsExpired.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvPrescriptionsExpired.ShowEditingIcon = False
        Me.dgvPrescriptionsExpired.Size = New System.Drawing.Size(460, 264)
        Me.dgvPrescriptionsExpired.TabIndex = 39
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label2.Location = New System.Drawing.Point(18, 15)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(191, 13)
        Me.Label2.TabIndex = 87
        Me.Label2.Text = "Συνταγές με έναρξη  εκτέλεσης την"
        '
        'dtpInitDate
        '
        Me.dtpInitDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpInitDate.Location = New System.Drawing.Point(212, 12)
        Me.dtpInitDate.Name = "dtpInitDate"
        Me.dtpInitDate.Size = New System.Drawing.Size(100, 20)
        Me.dtpInitDate.TabIndex = 89
        '
        'btnProcessPrescr
        '
        Me.btnProcessPrescr.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnProcessPrescr.Location = New System.Drawing.Point(25, 52)
        Me.btnProcessPrescr.Name = "btnProcessPrescr"
        Me.btnProcessPrescr.Size = New System.Drawing.Size(63, 23)
        Me.btnProcessPrescr.TabIndex = 91
        Me.btnProcessPrescr.Text = "Εκτέλεση"
        Me.btnProcessPrescr.UseVisualStyleBackColor = True
        '
        'chkSelectAll
        '
        Me.chkSelectAll.Location = New System.Drawing.Point(381, 13)
        Me.chkSelectAll.Name = "chkSelectAll"
        Me.chkSelectAll.Size = New System.Drawing.Size(104, 25)
        Me.chkSelectAll.TabIndex = 90
        Me.chkSelectAll.Text = "Εμφάνιση όλων"
        Me.chkSelectAll.UseVisualStyleBackColor = True
        '
        'dtpProcessDate
        '
        Me.dtpProcessDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpProcessDate.Location = New System.Drawing.Point(119, 54)
        Me.dtpProcessDate.Name = "dtpProcessDate"
        Me.dtpProcessDate.Size = New System.Drawing.Size(100, 20)
        Me.dtpProcessDate.TabIndex = 92
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label1.Location = New System.Drawing.Point(90, 57)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(29, 13)
        Me.Label1.TabIndex = 93
        Me.Label1.Text = "στις"
        '
        'chkDefaultDateProcessed
        '
        Me.chkDefaultDateProcessed.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.chkDefaultDateProcessed.Location = New System.Drawing.Point(225, 49)
        Me.chkDefaultDateProcessed.Name = "chkDefaultDateProcessed"
        Me.chkDefaultDateProcessed.Size = New System.Drawing.Size(183, 33)
        Me.chkDefaultDateProcessed.TabIndex = 94
        Me.chkDefaultDateProcessed.Text = "Βάλε σαν ημερομηνία εκτέλεσης την ημερομηνία έναρξης"
        Me.chkDefaultDateProcessed.UseVisualStyleBackColor = True
        '
        'frmPrescriptionsExpiring
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(511, 428)
        Me.Controls.Add(Me.chkDefaultDateProcessed)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.dtpProcessDate)
        Me.Controls.Add(Me.btnProcessPrescr)
        Me.Controls.Add(Me.chkSelectAll)
        Me.Controls.Add(Me.dtpInitDate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.grpPrescriptionsExpired)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPrescriptionsExpiring"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Προσεχώς ληξιπρόθεσμες συνταγές"
        Me.grpPrescriptionsExpired.ResumeLayout(False)
        CType(Me.dgvPrescriptionsExpired, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents grpPrescriptionsExpired As System.Windows.Forms.GroupBox
    Friend WithEvents dgvPrescriptionsExpired As System.Windows.Forms.DataGridView
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents dtpInitDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents btnProcessPrescr As System.Windows.Forms.Button
    Friend WithEvents chkSelectAll As System.Windows.Forms.CheckBox
    Friend WithEvents dtpProcessDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents chkDefaultDateProcessed As System.Windows.Forms.CheckBox
End Class
