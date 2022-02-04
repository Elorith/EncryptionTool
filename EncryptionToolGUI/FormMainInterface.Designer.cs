partial class FormMainInterface
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainInterface));
        this.TextBoxSelectedPath = new System.Windows.Forms.RichTextBox();
        this.ButtonSelectPath = new System.Windows.Forms.Button();
        this.DialogSelectPathFile = new System.Windows.Forms.OpenFileDialog();
        this.CheckBoxEncryptFiles = new System.Windows.Forms.CheckBox();
        this.CheckBoxEncryptDirectories = new System.Windows.Forms.CheckBox();
        this.DialogSelectPathDirectory = new System.Windows.Forms.FolderBrowserDialog();
        this.ButtonEncrypt = new System.Windows.Forms.Button();
        this.ButtonDecrypt = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // TextBoxSelectedPath
        // 
        this.TextBoxSelectedPath.Location = new System.Drawing.Point(12, 15);
        this.TextBoxSelectedPath.Name = "TextBoxSelectedPath";
        this.TextBoxSelectedPath.Size = new System.Drawing.Size(528, 22);
        this.TextBoxSelectedPath.TabIndex = 0;
        this.TextBoxSelectedPath.Text = "";
        // 
        // ButtonSelectPath
        // 
        this.ButtonSelectPath.Location = new System.Drawing.Point(546, 15);
        this.ButtonSelectPath.Name = "ButtonSelectPath";
        this.ButtonSelectPath.Size = new System.Drawing.Size(61, 22);
        this.ButtonSelectPath.TabIndex = 1;
        this.ButtonSelectPath.Text = "Browse";
        this.ButtonSelectPath.UseVisualStyleBackColor = true;
        this.ButtonSelectPath.Click += new System.EventHandler(this.ButtonSelectPath_Click);
        // 
        // CheckBoxEncryptFiles
        // 
        this.CheckBoxEncryptFiles.Checked = true;
        this.CheckBoxEncryptFiles.CheckState = System.Windows.Forms.CheckState.Checked;
        this.CheckBoxEncryptFiles.Location = new System.Drawing.Point(14, 48);
        this.CheckBoxEncryptFiles.Name = "CheckBoxEncryptFiles";
        this.CheckBoxEncryptFiles.Size = new System.Drawing.Size(98, 25);
        this.CheckBoxEncryptFiles.TabIndex = 2;
        this.CheckBoxEncryptFiles.Text = "Encrypt File";
        this.CheckBoxEncryptFiles.UseVisualStyleBackColor = true;
        this.CheckBoxEncryptFiles.CheckedChanged += new System.EventHandler(this.CheckBoxEncryptFiles_CheckedChanged);
        // 
        // CheckBoxEncryptDirectories
        // 
        this.CheckBoxEncryptDirectories.Location = new System.Drawing.Point(129, 48);
        this.CheckBoxEncryptDirectories.Name = "CheckBoxEncryptDirectories";
        this.CheckBoxEncryptDirectories.Size = new System.Drawing.Size(109, 25);
        this.CheckBoxEncryptDirectories.TabIndex = 3;
        this.CheckBoxEncryptDirectories.Text = "Encrypt Directory";
        this.CheckBoxEncryptDirectories.UseVisualStyleBackColor = true;
        this.CheckBoxEncryptDirectories.CheckedChanged += new System.EventHandler(this.CheckBoxEncryptDirectories_CheckedChanged);
        // 
        // ButtonEncrypt
        // 
        this.ButtonEncrypt.Location = new System.Drawing.Point(12, 262);
        this.ButtonEncrypt.Name = "ButtonEncrypt";
        this.ButtonEncrypt.Size = new System.Drawing.Size(138, 34);
        this.ButtonEncrypt.TabIndex = 4;
        this.ButtonEncrypt.Text = "Encrypt";
        this.ButtonEncrypt.UseVisualStyleBackColor = true;
        this.ButtonEncrypt.Click += new System.EventHandler(this.ButtonEncrypt_Click);
        // 
        // ButtonDecrypt
        // 
        this.ButtonDecrypt.Location = new System.Drawing.Point(165, 262);
        this.ButtonDecrypt.Name = "ButtonDecrypt";
        this.ButtonDecrypt.Size = new System.Drawing.Size(138, 34);
        this.ButtonDecrypt.TabIndex = 5;
        this.ButtonDecrypt.Text = "Decrypt";
        this.ButtonDecrypt.UseVisualStyleBackColor = true;
        this.ButtonDecrypt.Click += new System.EventHandler(this.ButtonDecrypt_Click);
        // 
        // FormMainInterface
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.SystemColors.Control;
        this.ClientSize = new System.Drawing.Size(616, 317);
        this.Controls.Add(this.ButtonDecrypt);
        this.Controls.Add(this.ButtonEncrypt);
        this.Controls.Add(this.CheckBoxEncryptDirectories);
        this.Controls.Add(this.CheckBoxEncryptFiles);
        this.Controls.Add(this.ButtonSelectPath);
        this.Controls.Add(this.TextBoxSelectedPath);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
        this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
        this.Location = new System.Drawing.Point(15, 15);
        this.Name = "FormMainInterface";
        this.Text = "Encryption Tool";
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.Button ButtonDecrypt;

    private System.Windows.Forms.Button ButtonEncrypt;

    private System.Windows.Forms.FolderBrowserDialog DialogSelectPathDirectory;

    private System.Windows.Forms.CheckBox CheckBoxEncryptFiles;
    private System.Windows.Forms.CheckBox CheckBoxEncryptDirectories;

    private System.Windows.Forms.OpenFileDialog DialogSelectPathFile;

    private System.Windows.Forms.RichTextBox TextBoxSelectedPath;
    private System.Windows.Forms.Button ButtonSelectPath;

    #endregion
}