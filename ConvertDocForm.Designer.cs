using System.Windows.Forms;

namespace ConvertDoc;

partial class ConvertDocForm
{
	/// <summary>
	///  Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        toolStrip = new ToolStrip();
        toolStripButtonSubSection = new ToolStripButton();
        textBoxSource = new TextBox();
        textBoxResult = new TextBox();
        toolStrip.SuspendLayout();
        SuspendLayout();
        // 
        // toolStrip
        // 
        toolStrip.ImageScalingSize = new Size(20, 20);
        toolStrip.Items.AddRange(new ToolStripItem[] { toolStripButtonSubSection });
        toolStrip.Location = new Point(0, 0);
        toolStrip.Name = "toolStrip";
        toolStrip.Size = new Size(1235, 27);
        toolStrip.TabIndex = 0;
        toolStrip.Text = "toolStrip";
        // 
        // toolStripButtonSubSection
        // 
        toolStripButtonSubSection.CheckOnClick = true;
        toolStripButtonSubSection.DisplayStyle = ToolStripItemDisplayStyle.Text;
        toolStripButtonSubSection.ImageTransparentColor = Color.Magenta;
        toolStripButtonSubSection.Name = "toolStripButtonSubSection";
        toolStripButtonSubSection.Size = new Size(87, 24);
        toolStripButtonSubSection.Text = "SubSection";
        toolStripButtonSubSection.CheckStateChanged += toolStripButtonSubSection_CheckStateChanged;
        // 
        // textBoxSource
        // 
        textBoxSource.AcceptsReturn = true;
        textBoxSource.AcceptsTab = true;
        textBoxSource.AllowDrop = true;
        textBoxSource.Dock = DockStyle.Left;
        textBoxSource.Location = new Point(0, 27);
        textBoxSource.Multiline = true;
        textBoxSource.Name = "textBoxSource";
        textBoxSource.ScrollBars = ScrollBars.Both;
        textBoxSource.Size = new Size(559, 579);
        textBoxSource.TabIndex = 1;
        textBoxSource.WordWrap = false;
        textBoxSource.TextChanged += textBoxSource_TextChanged;
        // 
        // textBoxResult
        // 
        textBoxResult.AcceptsReturn = true;
        textBoxResult.AcceptsTab = true;
        textBoxResult.Dock = DockStyle.Fill;
        textBoxResult.Location = new Point(559, 27);
        textBoxResult.Multiline = true;
        textBoxResult.Name = "textBoxResult";
        textBoxResult.ScrollBars = ScrollBars.Both;
        textBoxResult.Size = new Size(676, 579);
        textBoxResult.TabIndex = 2;
        textBoxResult.WordWrap = false;
        // 
        // ConvertDocForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScroll = true;
        AutoSize = true;
        ClientSize = new Size(1235, 606);
        Controls.Add(textBoxResult);
        Controls.Add(textBoxSource);
        Controls.Add(toolStrip);
        Name = "ConvertDocForm";
        Text = "Converter";
        toolStrip.ResumeLayout(false);
        toolStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private ToolStrip toolStrip;
    private ToolStripButton toolStripButtonSubSection;
    private TextBox textBoxSource;
    private TextBox textBoxResult;
}
