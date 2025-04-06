namespace ConvertDoc;

public partial class ConvertDocForm : Form
{
    public ConvertDocForm()
    {
        InitializeComponent();
    }

    private void textBoxSource_TextChanged(object sender, EventArgs e)
    {
        textBoxResult.Text = ConvertToXML(textBoxSource.Text, toolStripButtonSubSection.Checked);
    }

    private void toolStripButtonSubSection_CheckStateChanged(object sender, EventArgs e)
    {
        textBoxResult.Text = ConvertToXML(textBoxSource.Text, toolStripButtonSubSection.Checked);
    }

    private static string ConvertToXML(string text, bool subSection)
    {
        return new Recipe(text).Export(subSection);
    }
}
