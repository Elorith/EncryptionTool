using System.ComponentModel;

partial class FormEncryptionTask
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEncryptionTask));
        this.ButtonSubmitTask = new System.Windows.Forms.Button();
        this.TextBoxPassword = new System.Windows.Forms.RichTextBox();
        this.TextBoxConfirmPassword = new System.Windows.Forms.RichTextBox();
        this.LabelPasswordField = new System.Windows.Forms.Label();
        this.LabelConfirmPasswordField = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // ButtonSubmitTask
        // 
        this.ButtonSubmitTask.Location = new System.Drawing.Point(76, 114);
        this.ButtonSubmitTask.Name = "ButtonSubmitTask";
        this.ButtonSubmitTask.Size = new System.Drawing.Size(113, 26);
        this.ButtonSubmitTask.TabIndex = 0;
        this.ButtonSubmitTask.Text = "Submit";
        this.ButtonSubmitTask.UseVisualStyleBackColor = true;
        this.ButtonSubmitTask.Click += new System.EventHandler(this.ButtonSubmitTask_Click);
        // 
        // TextBoxPassword
        // 
        this.TextBoxPassword.Location = new System.Drawing.Point(12, 33);
        this.TextBoxPassword.Name = "TextBoxPassword";
        this.TextBoxPassword.Size = new System.Drawing.Size(236, 21);
        this.TextBoxPassword.TabIndex = 1;
        this.TextBoxPassword.Text = "";
        // 
        // TextBoxConfirmPassword
        // 
        this.TextBoxConfirmPassword.Location = new System.Drawing.Point(12, 81);
        this.TextBoxConfirmPassword.Name = "TextBoxConfirmPassword";
        this.TextBoxConfirmPassword.Size = new System.Drawing.Size(236, 21);
        this.TextBoxConfirmPassword.TabIndex = 2;
        this.TextBoxConfirmPassword.Text = "";
        // 
        // LabelPasswordField
        // 
        this.LabelPasswordField.BackColor = System.Drawing.Color.Transparent;
        this.LabelPasswordField.Location = new System.Drawing.Point(12, 9);
        this.LabelPasswordField.Name = "LabelPasswordField";
        this.LabelPasswordField.Size = new System.Drawing.Size(87, 21);
        this.LabelPasswordField.TabIndex = 3;
        this.LabelPasswordField.Text = "Enter password";
        this.LabelPasswordField.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // LabelConfirmPasswordField
        // 
        this.LabelConfirmPasswordField.Location = new System.Drawing.Point(12, 57);
        this.LabelConfirmPasswordField.Name = "LabelConfirmPasswordField";
        this.LabelConfirmPasswordField.Size = new System.Drawing.Size(236, 21);
        this.LabelConfirmPasswordField.TabIndex = 4;
        this.LabelConfirmPasswordField.Text = "Confirm password";
        this.LabelConfirmPasswordField.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // FormEncryptionTask
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(260, 152);
        this.Controls.Add(this.LabelConfirmPasswordField);
        this.Controls.Add(this.LabelPasswordField);
        this.Controls.Add(this.TextBoxConfirmPassword);
        this.Controls.Add(this.TextBoxPassword);
        this.Controls.Add(this.ButtonSubmitTask);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
        this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
        this.Name = "FormEncryptionTask";
        this.Text = "Config";
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.Label LabelConfirmPasswordField;

    private System.Windows.Forms.Label LabelPasswordField;

    private System.Windows.Forms.RichTextBox TextBoxPassword;
    private System.Windows.Forms.RichTextBox TextBoxConfirmPassword;

    private System.Windows.Forms.Button ButtonSubmitTask;

    #endregion
}