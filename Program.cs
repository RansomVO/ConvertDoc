namespace ConvertDoc;

static class Program
{
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{
		// To customize application configuration such as set high DPI settings or default font,
		// see https://aka.ms/applicationconfiguration.
		ApplicationConfiguration.Initialize();
		Application.Run(new ConvertDocForm());
	}
	private static Dictionary<char, string> myDict = new Dictionary<char, string>
	{
		{ '½', "&frac12;" },

		{ '⅓', "&frac13;" },
		{ '⅔', "&frac23;" },

		{ '¼', "&frac14;" },
		{ '¾', "&frac34;" },

		{ '⅕', "&frac15;" },
		{ '⅖', "&frac25;" },
		{ '⅗', "&frac35;" },
		{ '⅘', "&frac45;" },

		{ '⅙', "&frac16;" },
		{ '⅚', "&frac56;" },

		{ '⅛', "&frac18;" },
		{ '⅜', "&frac38;" },
		{ '⅝', "&frac58;" },
		{ '⅞', "&frac78;" },
		{ '⅐', "&frac17;" },
		{ '⅑', "&frac19;" },
		{ '⅒', "&frac110;" }
	};
}
