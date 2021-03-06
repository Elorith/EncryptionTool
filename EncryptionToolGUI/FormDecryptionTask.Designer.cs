using System.ComponentModel;

partial class FormDecryptionTask
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDecryptionTask));
        this.ButtonSubmitTask = new System.Windows.Forms.Button();
        this.LabelPasswordField = new System.Windows.Forms.Label();
        this.TextBoxPassword = new System.Windows.Forms.TextBox();
        this.SuspendLayout();
        // 
        // ButtonSubmitTask
        // 
        this.ButtonSubmitTask.Location = new System.Drawing.Point(75, 64);
        this.ButtonSubmitTask.Name = "ButtonSubmitTask";
        this.ButtonSubmitTask.Size = new System.Drawing.Size(113, 26);
        this.ButtonSubmitTask.TabIndex = 0;
        this.ButtonSubmitTask.Text = "Submit";
        this.ButtonSubmitTask.UseVisualStyleBackColor = true;
        this.ButtonSubmitTask.Click += new System.EventHandler(this.ButtonSubmitTask_Click);
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
        // TextBoxPassword
        // 
        this.TextBoxPassword.Location = new System.Drawing.Point(12, 33);
        this.TextBoxPassword.Name = "TextBoxPassword";
        this.TextBoxPassword.Size = new System.Drawing.Size(235, 20);
        this.TextBoxPassword.TabIndex = 4;
        // 
        // FormDecryptionTask
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(260, 102);
        this.Controls.Add(this.TextBoxPassword);
        this.Controls.Add(this.LabelPasswordField);
        this.Controls.Add(this.ButtonSubmitTask);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
        this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
        this.Name = "FormDecryptionTask";
        this.Text = "Config";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.TextBox TextBoxPassword;

    private System.Windows.Forms.Label LabelPasswordField;

    private System.Windows.Forms.Button ButtonSubmitTask;

    #endregion
}