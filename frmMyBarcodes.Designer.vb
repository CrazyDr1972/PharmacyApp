<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMyBarcodes
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMyBarcodes))
        Me.Button3 = New System.Windows.Forms.Button()
        Me.grpSearchBarcodesOptions = New System.Windows.Forms.GroupBox()
        Me.rbByBarcode = New System.Windows.Forms.RadioButton()
        Me.rbByName = New System.Windows.Forms.RadioButton()
        Me.dgvBarcodes = New System.Windows.Forms.DataGridView()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtSearchBarcode = New System.Windows.Forms.TextBox()
        Me.btnAddBarcode = New System.Windows.Forms.Button()
        Me.btnUseBarcode = New System.Windows.Forms.Button()
        Me.chkContinuous = New System.Windows.Forms.CheckBox()
        Me.lblTotalDrugs = New System.Windows.Forms.Label()
        Me.grpSearchBarcodesOptions.SuspendLayout()
        CType(Me.dgvBarcodes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Button3
        '
        Me.Button3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Button3.Image = CType(resources.GetObject("Button3.Image"), System.Drawing.Image)
        Me.Button3.Location = New System.Drawing.Point(199, 33)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(23, 23)
        Me.Button3.TabIndex = 56
        Me.Button3.UseVisualStyleBackColor = True
        '
        'grpSearchBarcodesOptions
        '
        Me.grpSearchBarcodesOptions.Controls.Add(Me.rbByBarcode)
        Me.grpSearchBarcodesOptions.Controls.Add(Me.rbByName)
        Me.grpSearchBarcodesOptions.Enabled = False
        Me.grpSearchBarcodesOptions.Location = New System.Drawing.Point(240, 11)
        Me.grpSearchBarcodesOptions.Name = "grpSearchBarcodesOptions"
        Me.grpSearchBarcodesOptions.Size = New System.Drawing.Size(73, 59)
        Me.grpSearchBarcodesOptions.TabIndex = 53
        Me.grpSearchBarcodesOptions.TabStop = False
        '
        'rbByBarcode
        '
        Me.rbByBarcode.AutoSize = True
        Me.rbByBarcode.Location = New System.Drawing.Point(5, 34)
        Me.rbByBarcode.Name = "rbByBarcode"
        Me.rbByBarcode.Size = New System.Drawing.Size(65, 17)
        Me.rbByBarcode.TabIndex = 45
        Me.rbByBarcode.Text = "Barcode"
        Me.rbByBarcode.UseVisualStyleBackColor = True
        '
        'rbByName
        '
        Me.rbByName.AutoSize = True
        Me.rbByName.Checked = True
        Me.rbByName.Location = New System.Drawing.Point(5, 11)
        Me.rbByName.Name = "rbByName"
        Me.rbByName.Size = New System.Drawing.Size(59, 17)
        Me.rbByName.TabIndex = 44
        Me.rbByName.TabStop = True
        Me.rbByName.Text = "Όνομα"
        Me.rbByName.UseVisualStyleBackColor = True
        '
        'dgvBarcodes
        '
        Me.dgvBarcodes.AllowUserToAddRows = False
        Me.dgvBarcodes.AllowUserToDeleteRows = False
        Me.dgvBarcodes.AllowUserToOrderColumns = True
        Me.dgvBarcodes.AllowUserToResizeColumns = False
        Me.dgvBarcodes.AllowUserToResizeRows = False
        Me.dgvBarcodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvBarcodes.Location = New System.Drawing.Point(18, 89)
        Me.dgvBarcodes.Name = "dgvBarcodes"
        Me.dgvBarcodes.ReadOnly = True
        Me.dgvBarcodes.RowHeadersVisible = False
        Me.dgvBarcodes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvBarcodes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvBarcodes.ShowEditingIcon = False
        Me.dgvBarcodes.Size = New System.Drawing.Size(326, 242)
        Me.dgvBarcodes.TabIndex = 50
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(15, 19)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(66, 13)
        Me.Label2.TabIndex = 52
        Me.Label2.Text = "Αναζήτηση:"
        '
        'txtSearchBarcode
        '
        Me.txtSearchBarcode.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSearchBarcode.Location = New System.Drawing.Point(18, 35)
        Me.txtSearchBarcode.Name = "txtSearchBarcode"
        Me.txtSearchBarcode.Size = New System.Drawing.Size(175, 20)
        Me.txtSearchBarcode.TabIndex = 51
        '
        'btnAddBarcode
        '
        Me.btnAddBarcode.Location = New System.Drawing.Point(18, 342)
        Me.btnAddBarcode.Name = "btnAddBarcode"
        Me.btnAddBarcode.Size = New System.Drawing.Size(95, 23)
        Me.btnAddBarcode.TabIndex = 67
        Me.btnAddBarcode.Text = "Προσθήκη"
        Me.btnAddBarcode.UseVisualStyleBackColor = True
        '
        'btnUseBarcode
        '
        Me.btnUseBarcode.Location = New System.Drawing.Point(249, 342)
        Me.btnUseBarcode.Name = "btnUseBarcode"
        Me.btnUseBarcode.Size = New System.Drawing.Size(95, 23)
        Me.btnUseBarcode.TabIndex = 68
        Me.btnUseBarcode.Text = "Χρησιμοποίηση"
        Me.btnUseBarcode.UseVisualStyleBackColor = True
        '
        'chkContinuous
        '
        Me.chkContinuous.Location = New System.Drawing.Point(18, 364)
        Me.chkContinuous.Name = "chkContinuous"
        Me.chkContinuous.Size = New System.Drawing.Size(132, 30)
        Me.chkContinuous.TabIndex = 69
        Me.chkContinuous.Text = "Επαναλαμβανόμενη"
        Me.chkContinuous.UseVisualStyleBackColor = True
        '
        'lblTotalDrugs
        '
        Me.lblTotalDrugs.AutoSize = True
        Me.lblTotalDrugs.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblTotalDrugs.Location = New System.Drawing.Point(15, 69)
        Me.lblTotalDrugs.Name = "lblTotalDrugs"
        Me.lblTotalDrugs.Size = New System.Drawing.Size(82, 17)
        Me.lblTotalDrugs.TabIndex = 70
        Me.lblTotalDrugs.Text = "Αναζήτηση:"
        '
        'frmMyBarcodes
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(358, 402)
        Me.Controls.Add(Me.lblTotalDrugs)
        Me.Controls.Add(Me.chkContinuous)
        Me.Controls.Add(Me.btnUseBarcode)
        Me.Controls.Add(Me.btnAddBarcode)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.grpSearchBarcodesOptions)
        Me.Controls.Add(Me.dgvBarcodes)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtSearchBarcode)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMyBarcodes"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Τα δικά μου κουπόνια.."
        Me.grpSearchBarcodesOptions.ResumeLayout(False)
        Me.grpSearchBarcodesOptions.PerformLayout()
        CType(Me.dgvBarcodes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents grpSearchBarcodesOptions As System.Windows.Forms.GroupBox
    Friend WithEvents rbByBarcode As System.Windows.Forms.RadioButton
    Friend WithEvents rbByName As System.Windows.Forms.RadioButton
    Friend WithEvents dgvBarcodes As System.Windows.Forms.DataGridView
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtSearchBarcode As System.Windows.Forms.TextBox
    Friend WithEvents btnAddBarcode As System.Windows.Forms.Button
    Friend WithEvents btnUseBarcode As System.Windows.Forms.Button
    Friend WithEvents chkContinuous As System.Windows.Forms.CheckBox
    Friend WithEvents lblTotalDrugs As System.Windows.Forms.Label
End Class
