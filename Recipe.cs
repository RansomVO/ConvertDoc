using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics.Metrics;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ConvertDoc
{
	// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
	/// <remarks/>
	[System.Serializable()]
	[System.ComponentModel.DesignerCategory("code")]
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	[System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
	public class Recipe
	{
		#region const

		private const string _xmlHeader = (
			"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
			"<?xml-stylesheet type=\"text/xsl\" href=\"../recipe.xsl\"?>\r\n" +
			"<!-- Include entities to be used in the xml -->\r\n" +
			"<!DOCTYPE invoice [\r\n" +
			"\t<!ENTITY section SYSTEM \"section.txt\">\r\n" +
			"\t<!ENTITY % externalEntities SYSTEM \"/entities.dtd\">\r\n" +
			"\t%externalEntities;\r\n" +
			"]>\r\n\r\n"
		);
		private const string _xmlHeaderSubSection = (
			"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
			"<?xml-stylesheet type=\"text/xsl\" href=\"../../recipe.xsl\"?>\r\n" +
			"<!-- Include entities to be used in the xml -->\r\n" +
			"<!DOCTYPE invoice [\r\n" +
			"\t<!ENTITY section SYSTEM \"section.txt\">\r\n" +
			"\t<!ENTITY parentSection SYSTEM \"../section.txt\">\r\n" +
			"\t<!ENTITY % externalEntities SYSTEM \"/entities.dtd\">\r\n" +
			"\t%externalEntities;\r\n" +
			"]>\r\n\r\n"
		);

		private static readonly Dictionary<string, string> _regexReplacements = new Dictionary<string, string>()
		{
			{ "([^=][\\d\"])\\s?x\\s?", "$1 &vector; " }
		};

        private static readonly Dictionary<string, string> _formatMappings = new Dictionary<string, string>()
		{
			{ "&lt;",   "<" },
			{ "&gt;",   ">" },
			{ "&amp;",  "&" },

			{ "`",      "'" },
			{ "’",      "'" },
			{ "“",      "\"" },
			{ "”",      "\"" },

			{ "☺",      "&smiley;" },
			{ "\uF04A", "&smiley;" },  // Smiley face char

            { "½",      "&frac12;" },
			{ "⅓",      "&frac13;" },
			{ "⅔",      "&frac23;" },
			{ "¼",      "&frac14;" },
			{ "¾",      "&frac34;" },
			{ "⅕",      "&frac15;" },
			{ "⅖",      "&frac25;" },
			{ "⅗",      "&frac35;" },
			{ "⅘",      "&frac45;" },
			{ "⅙",      "&frac16;" },
			{ "⅚",      "&frac56;" },
			{ "⅛",      "&frac18;" },
			{ "⅜",      "&frac38;" },
			{ "⅝",      "&frac58;" },
			{ "⅞",      "&frac78;" },
			{ "⅐",      "&frac17;" },
			{ "⅑",      "&frac19;" },
			{ "⅒",      "&frac110;" },

			{ "°",          "&degrees;" },
			{ "&#xB0",      "&degrees;" },
            { " ",			"&hardSpace;" },
            { "&nbsp;",     "&hardSpace;" },
            { "&#xA0;",     "&hardSpace;" },
            { "≈",			"&aboutEquals;" },
            { "&#x2248;",   "&aboutEquals;" },

            { "<Description>", "<Description id=\"Description\">"},
            { "<Ingredients>", "<Ingredients id=\"Ingredients\">"},
            { "<Instructions>", "<Instructions id=\"Instructions\">"},
			{ "<Notes", "<Notes id=\"Notes\""},
			{ "<Modifications", "<Modifications id=\"Modifications\""},

            { "<FinalNotes", "<FinalNotes id=\"FinalNotes\""},
        };

		private static readonly Dictionary<string, string> _formatAttributeMappings = new Dictionary<string, string>()
		{
			{ "\"", "&quote2;" },
			{ "”",  "&quote2;" },
			{ "'",  "&quote1;" },
			{ "’",  "&quote1;" }
		};

		private static readonly string[] _sections = {
			"Section",
			"Description",
			"Ingredients",
			"Instructions",
			"Notes",
			"Modifications",
		};
		private const string _regExSectionTitle = @"(?<indent>^\s*)<section title=""(?<title>[^""]+)""\s*>\s*$";

        private const string _regExBullet = @"\s*[•o-]\s*";
		private const string _regExEOL = @"[\?\!\.\):°]$";
		#region _regExSourceYields
		private const string _regExSourceYields = (
            @"^" +
            @"(\(?(?<Source>[^\(\)]+[^\(\s\)])\)?)" +
			@"(\s+\((?<Note>[^\)]+)\))?" +
			@"(\s+((Yields)|(Serves)|(Servings)):?\s+(?<Yields>.*))?" +
			@"$"
        );
		//private const string _regExSourceYields = (
		//	@"^\(?" +
		//	@"(?<Source>(([^\(\)]+[^\(\s\)])))" +
		//	@"\)?(\s+\(" +
		//	@"(?<Note>[^\)]+)\))?" +
		//	@"(" +
		//	@"\sYields:?\s*" +
		//	@"(?<yields>.*)" +
		//	@")" +
		//	@"$"
		//);
		#endregion _regExSourceYields
		#region _regExIngredient
		private const string _regExIngredient = (
			// Start of Line
			@"^" +

			// Prefix that is not needed.
			_regExBullet +

			// preAmountNote
			@"(" +
				@"\(" +
				@"(?<preAmountNote>(" +
					@"([^\)]+)" +   // Option 1
				@"))" +
				@"\)\s+" +
			@")?" +

			// amount
			@"(" +
				@"(?<amount>(" +
					@"([^\s\(]+\s+[^\s\)]+)" +  // Option 1
					@"|([^\s\(]*)" +            // Option 2 (E.G.: Handle "3 Eggs")
				@")[^\s\(])\s+" +
			@")?" +

			// postAmountNote
			@"(" +
				@"\(" +
				@"(?<postAmountNote>(" +
					@"[^\)]+" + // Option 1
				@"))" +
				@"\)\s*" +
			@")?" +

			// name
			@"(" +
				@"(?<name>(" +
					@"([^\(]+)" +   // Option 1
				@"))" +
			@")" +

			// note
			@"(" +
				@"\s+\(" +
				@"(?<note>(" +
					@"([^\)]+)" +   // Option 1
				@"))" +
				@"\)" +
			@")?" +

			// End of Line so nothing is omitted.
			@"\s*$"
		);
		#endregion _regExIngredient
		#region _regExStepItem
		private const string _regExStepItem = (
			// Start of Line
			@"^" +

			// Prefix that is not needed.
			@"\s*" +

			// Step # (Currently not used.)
			@"(?<step>(" +
				@"(\d+)" +  // Option 1
			@"))" +

			// Step Separator
			@"(" +
				@"(\))" +   // Option 1
				@"|(\.)" +  // Option 2
			@")" +
			@"\s*" +

			// Instruction
			@"(?<instruction>(" +
				@"(.*)" +   // Option 1
			@"))" +

			// End of Line
			@"\s*$"
		);
		#endregion _regExStepItem
		#region _regExListItem
		private const string _regExListItem = (
			// Start of Line
			@"^" +

			// Prefix that is not needed.
			_regExBullet +

			// item
			@"(?<item>(" +
				@"(.+)" +   // Everything as-is.
			@"))" +

            // End of Line so nothing is omitted.
            @"\s*$"
        );

        #endregion _regExListItem

		#endregion const

        #region XML Attributes

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
		public string title { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute()]
		public string yields { get; set; }

		#endregion XML Attributes

		#region XML Elements

		/// <remarks/>
		public string Source { get; set; }

		/// <remarks/>
		public string Note { get; set; }

		/// <remarks/>
		public string Section { get { return "&section;"; } set { } }

		/// <remarks/>
		public RecipeLastModified LastModified { get; set; } = new RecipeLastModified();

		/// <remarks/>
		public string Description { get; set; }

		/// <remarks>"set" must be public for XML to serialize it</remarks>
		[System.Xml.Serialization.XmlArrayItem("section", IsNullable = false)]
		public List<RecipeIngredientsSection> Ingredients { get; set; } = new List<RecipeIngredientsSection>();

		/// <remarks>"set" must be public for XML to serialize it</remarks>
		[System.Xml.Serialization.XmlArrayItem("section", IsNullable = false)]
		public List<RecipeInstructionsSection> Instructions { get; set; } = new List<RecipeInstructionsSection>();

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItem("note", IsNullable = true)]
		public List<string> Notes { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItem("modification", IsNullable = true)]
		public List<string> Modifications { get; set; }

		#endregion  XML Elements

		public RecipeIngredientsSection AddIngredientsSection(string title = null)
		{
			var section = new RecipeIngredientsSection(title);
			Ingredients.Add(section);

			return section;
		}

		public RecipeInstructionsSection AddInstructionsSection(string title = null)
		{
			var section = new RecipeInstructionsSection(title);
			Instructions.Add(section);

			return section;
		}

		public void AddNote(string note)
		{
			if (Notes == null)
			{
				Notes = new List<string>();
			}

			Notes.Add(note);
		}

		public void AppendNote(string noteAddition)
		{
			var note = Notes.Last();
			Notes.Remove(note);

			Notes.Add(note + "<br />" + noteAddition);
		}

		public void AddModification(string modification)
		{
			if (Modifications == null)
			{
				Modifications = new List<string>();
			}

			Modifications.Add(modification);
		}

		public void AppendModification(string modificationAddition)
		{
			var modification = Notes.Last();
			Modifications.Remove(modification);

			Modifications.Add(modification + "<br />" + modificationAddition);
		}

		public static string FormatAttribute(string attribute)
		{
			if (attribute != null)
			{
				foreach (var entry in _formatAttributeMappings)
				{
					attribute = attribute.Replace(entry.Key, entry.Value);
				}
			}

			return attribute;
		}

        private enum SectionType
        {
			None = 0,
            Description,
            Ingredients,
            Instructions,
            Notes,
            Modifications,
            FinalNote
        };

        private SectionType IsSectionHeader(string line)
		{
            if (Regex.IsMatch(line, @"^\s*Ingredients?:?\s*$")) { return SectionType.Ingredients; }
            if (Regex.IsMatch(line, @"^\s*Instructions?:?\s*$")) { return SectionType.Instructions; }
            if (Regex.IsMatch(line, @"^\s*Notes?:?\s*$")) { return SectionType.Notes; }
            if (Regex.IsMatch(line, @"^\s*Modifications?:?\s*$")) { return SectionType.Modifications; }

            return SectionType.None;
		}

		/// <summary>
		/// Parse the Recipe text into a Recipe object;
		/// </summary>
		/// <param name="recipeText">Text for the Recipe</param>
		public Recipe(string recipeText)
		{
			try
			{
				var lines = recipeText.Split("\r\n");
				int i = 0;

				#region Gather basic details

				title = FormatAttribute(lines[i++].Trim());

				var regExMatches = Regex.Match(lines[i++], _regExSourceYields);
				yields = FormatAttribute(regExMatches.Groups["yields"].Value);
				Source = regExMatches.Groups["Source"].Value;
				Note = string.IsNullOrWhiteSpace(regExMatches.Groups["Note"].Value) ? null : regExMatches.Groups["Note"].Value;

				while (lines[i].Trim().ToLower() != "ingredients")
				{
					Description += lines[i++].Trim();
				}

				#endregion Gather basic details

				#region Gather ingredients

				i++;
				RecipeIngredientsSection ingredientsSection = null;

				// Handle case where the first line is an ingredient.
				if (Regex.IsMatch(lines[i], _regExIngredient))
				{
					ingredientsSection = AddIngredientsSection(null);
				}

				// Loop through the lines for this section.
				do
				{
					lines[i] = lines[i].Trim();

					// If a line is blank, it means the end/beginning of something.
					if (string.IsNullOrWhiteSpace(lines[i]))
					{
						// Skip all blank lines.
						while (string.IsNullOrWhiteSpace(lines[i])) { i++; }

						// If the next line is "Instructions", it is the end of this section.
						if (IsSectionHeader(lines[i]) != SectionType.None) { break; }

						// If the next line is an ingredient, then there is no Section title.
						if (Regex.IsMatch(lines[i], _regExIngredient))
						{
							ingredientsSection = AddIngredientsSection(null);
						}
						else
						{
							ingredientsSection = AddIngredientsSection(lines[i].Trim(':'));
						}
					}
					// Parse the Ingredient.
					else
					{
						try
						{
							regExMatches = Regex.Match(lines[i], _regExIngredient);
							if (regExMatches.Success)
							{
								var name = regExMatches.Groups["name"].Value;
								var amount = regExMatches.Groups["amount"].Value;
								var nameNote = regExMatches.Groups["note"].Value;
								var amountNote = regExMatches.Groups["preAmountNote"].Value + regExMatches.Groups["postAmountNote"].Value;

								if (ingredientsSection == null)
								{
                                    ingredientsSection = AddIngredientsSection(null);
                                }
								ingredientsSection.Add(name, amount, nameNote, amountNote);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
				} while (
                    ++i < lines.Count()
                    && IsSectionHeader(lines[i]) == SectionType.None
				);

				#endregion Gather ingredients

				#region Gather instructions

				i++;
				RecipeInstructionsSection instructionsSection = null;

				// Handle case where the first line is an ingredient.
				if (Regex.IsMatch(lines[i], _regExStepItem))
				{
					instructionsSection = AddInstructionsSection(null);
				}

				// Loop through the lines for this section.
				do
				{
					lines[i] = lines[i].Trim();

					// If a line is blank, it means the end/beginning of something.
					if (string.IsNullOrWhiteSpace(lines[i]) || !Regex.IsMatch(lines[i], $"({_regExStepItem})|({_regExBullet})"))
					{
						// Skip all blank lines.
						while (string.IsNullOrWhiteSpace(lines[i])) { i++; }

                        // If the next line is "Notes" or "Modifications", it is the end of this section.
                        if (IsSectionHeader(lines[i]) != SectionType.None) { break; }

						if (Regex.IsMatch(lines[i], _regExStepItem))
						{
							instructionsSection = AddInstructionsSection(null);
						}
						else
						{
							instructionsSection = AddInstructionsSection(lines[i].Trim(':'));
						}
					}
					else
					{
						try
						{
							regExMatches = Regex.Match(lines[i], _regExStepItem);
							if (regExMatches.Success)
							{
								var instruction = regExMatches.Groups["instruction"].Value;
								if (!Regex.IsMatch(instruction, _regExEOL))
								{
									instruction += ".";
								}
								if (Regex.IsMatch(lines[i + 1], _regExListItem))
								{
									instruction += "<ul>";
									do
									{
										i++;
										regExMatches = Regex.Match(lines[i], _regExListItem);
										instruction += $"<li>{regExMatches.Groups["item"].Value}</li>";

									} while (Regex.IsMatch(lines[i + 1], _regExListItem));
									instruction += "</ul>";

								}

								instructionsSection.Add(instruction);
							}
							else
							{
								instructionsSection.Append(lines[i]);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
				} while (
					++i < lines.Count()
                    && IsSectionHeader(lines[i]) == SectionType.None
                );

				#endregion Gather instructions

				#region Gather notes

				if (i < lines.Count() && IsSectionHeader(lines[i]) == SectionType.Notes)
				{
					i++;

					string noteItem = null;
					do
					{
						lines[i] = lines[i].Trim();

						try
						{
							regExMatches = Regex.Match(lines[i], _regExListItem);
							if (regExMatches.Success)
							{
								noteItem = regExMatches.Groups["item"].Value;
								if (!Regex.IsMatch(noteItem, _regExEOL))
								{
									noteItem += ".";
								}
								AddNote(noteItem);
							}
							else if (!string.IsNullOrWhiteSpace(lines[i]))
							{
								AppendNote(lines[i]);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					} while (
						++i < lines.Count()
						&& IsSectionHeader(lines[i]) == SectionType.None
                    );
				}

				#endregion Gather notes

				#region Gather modifications

				if (i < lines.Count() && IsSectionHeader(lines[i]) == SectionType.Modifications)
				{
					i++;

					string modificationItem = null;
					do
					{
						lines[i] = lines[i].Trim();

						try
						{
							regExMatches = Regex.Match(lines[i], _regExListItem);
							if (regExMatches.Success)
							{
								modificationItem = regExMatches.Groups["item"].Value;
								if (!Regex.IsMatch(modificationItem, _regExEOL))
								{
									modificationItem += ".";
								}
								AddModification(modificationItem);
							}
							else if (!string.IsNullOrWhiteSpace(lines[i]))
							{
								AppendModification(lines[i]);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					} while (++i < lines.Count());
				}

				#endregion Gather modifications
			}
			catch { }
		}

		/// <summary>
		/// For XML Exporting only.
		/// </summary>
		private Recipe() { }

		/// <summary>
		///     Reformats the provided XML to match styling guide
		/// </summary>
		/// <param name="xml">The XML string to format.</param>
		/// <returns>String of reformatted XML</returns>
		private string Format(string xml, bool subSection)
		{
			// Do the initial formatting.
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Unicode))
				{
					XmlDocument xmlDocument = new XmlDocument();

					// Load the XmlDocument with the XML.
					xmlDocument.LoadXml(xml);

					// Write the XML into a formatting XmlTextWriter
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.IndentChar = '\t';
					xmlTextWriter.Indentation = 1;
					xmlDocument.WriteContentTo(xmlTextWriter);

					// Rewind the MemoryStream in order to read its contents.
					xmlTextWriter.Flush();
					memoryStream.Flush();
					memoryStream.Position = 0;

					// Read MemoryStream contents via a StreamReader.
					xml = new StreamReader(memoryStream).ReadToEnd();
				}
			}
			catch (Exception)
			{ /* If there was a problem, just use the initial xml text. */ }

			// Remove any headers. (The ones we want will be added later.)
			if (xml.StartsWith("<?xml"))
			{
				xml = xml.Substring(xml.IndexOf("\n") + 1);
			}

			// Replace the strings that were encoded to what they represent.
			foreach (string key in _formatMappings.Keys)
			{
				xml = xml.Replace(key, _formatMappings[key]);
			}

            // Do replacements that should be done early on.
            foreach (string key in _regexReplacements.Keys)
			{
				xml = Regex.Replace(xml, key, _regexReplacements[key]);
			}

            // Handle all cases where a character isn't in ASCII
            List<char> handled = new List<char>();
			foreach (char c in xml.ToCharArray())
			{
				if (!handled.Contains(c) && (int)c > 0x7F)
				{
					xml = xml.Replace($"{c}", $"&#x{((int)c):X};");
					handled.Add(c);
				}
			}

			string[] xmlLines = xml.Split("\r\n");
			for (int i = 0; i < xmlLines.Length; i++)
			{
				// Split multiple attributes to separate lines with indent.
				var attributes = Regex.Match(xmlLines[i], @"(\s*)(<\w*\s*[^=]*=""[^""]*"")(\s*[^=]*=""[^""]*"")*(.*>)");
				if (attributes.Groups.Count > 4 && attributes.Groups[3].Captures.Count > 1)
				{
					var indent = attributes.Groups[1].Captures[0].Value;
					xmlLines[i] = indent + attributes.Groups[2].Captures[0].Value;
					foreach (Capture capture in attributes.Groups[3].Captures)
					{
						xmlLines[i] += "\r\n" + indent + "\t" + capture.Value.Trim();
					}
					xmlLines[i] += attributes.Groups[4].Captures[0].Value;
				}

				// Add a ParentSection if this is in a Recipie category subSection
				if (subSection && xmlLines[i] == "\t<Section>&section;</Section>")
				{
					xmlLines[i] += "\r\n\t<ParentSection>&parentSection;</ParentSection>";
				}

                // Reformat Description to make it more readable.
                var description = Regex.Match(xmlLines[i], @"^(?<indent>\s*)<Description>(?<text>.*)</Description>$");
                if (description.Success)
				{
					xmlLines[i] = $"{description.Groups["indent"].Value}<Description>\r\n";
					foreach (var line in Regex.Split(description.Groups["text"].Value, @"(?<=[\.\!\?:])"))
					{
						if (!string.IsNullOrWhiteSpace(line))
						{
							xmlLines[i] += $"{description.Groups["indent"].Value}\t{line.Trim()}\r\n";
						}
					}
					xmlLines[i] += $"{description.Groups["indent"].Value}</Description>";
				}

				// Add blank lines to separate related nodes.
				foreach (var sectionStarter in _sections)
				{
					if (Regex.IsMatch(xmlLines[i], $"^\\s*<{sectionStarter}"))
					{
						xmlLines[i] = "\r\n" + xmlLines[i];
					}
				}

				var titledSection = Regex.Match(xmlLines[i], _regExSectionTitle);
                if (titledSection.Success)
                {
					string id = "";
					var matches = Regex.Split(titledSection.Groups["title"].Value, @"[^\w]+");
					foreach (var match in matches) {
						id += string.Concat(match[0].ToString().ToUpper(), match.AsSpan(1));
					}
					
                    xmlLines[i] = (
						$"{titledSection.Groups["indent"].Value}" +
						$"<section id=\"{id}\"" +
						$" title=\"{titledSection.Groups["title"].Value}\">"
					);
                }
			}
			xml = string.Join("\r\n", xmlLines);

			// Use the desired XML headers.
			xml = (subSection ? _xmlHeaderSubSection : _xmlHeader) + xml;

			return xml;
		}

		/// <summary>
		///     Converts the object to a string of XML
		/// </summary>
		/// <returns>String of XML</returns>
		public string Export(bool subSection)
		{
			using (var stringWriter = new StringWriter())
			{
				// The documentation says that this is not supported: https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializernamespaces?view=net-9.0#remarks
				//  However, it does work to get rid of the xmlns:xsi and xmlns:xsd attributes.
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", "");
				new XmlSerializer(this.GetType()).Serialize(stringWriter, this, ns);

				return Format(stringWriter.ToString(), subSection);
			}
		}
	}

	/// <remarks/>
	[System.Serializable()]
	[System.ComponentModel.DesignerCategory("code")]
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class RecipeLastModified
	{
		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute()]
		public int year { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute()]
		public int month { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute()]
		public int day { get; set; }

		public RecipeLastModified(DateOnly? date = null)
		{
			if (date == null)
			{
				date = DateOnly.FromDateTime(DateTime.Now);
			}

			year = date.Value.Year;
			month = date.Value.Month;
			day = date.Value.Day;
		}

		/// <summary>
		/// For XML Exporting only.
		/// </summary>
		private RecipeLastModified() { }
	}

	// ----------------------------------------------------------------------------------------------------
	#region Ingredients

	/// <remarks/>
	[System.Serializable()]
	[System.ComponentModel.DesignerCategory("code")]
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class RecipeIngredientsSection
	{
		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute()]
		public string title { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlElement("ingredient")]
		public List<RecipeSectionIngredient> ingredient { get; set; }

		public RecipeSectionIngredient Add(string name, string amount, string nameNote = null, string amountNote = null)
		{
			if (this.ingredient == null)
			{
				this.ingredient = new List<RecipeSectionIngredient>();
			}

			if (amountNote == "Optional")
			{
				nameNote = amountNote;
				amountNote = null;
			}
			var ingredient = new RecipeSectionIngredient(
				Recipe.FormatAttribute(name),
				string.IsNullOrEmpty(amount) ? null : Recipe.FormatAttribute(amount),
				string.IsNullOrEmpty(nameNote) ? null : Recipe.FormatAttribute(nameNote),
				string.IsNullOrEmpty(amountNote) ? null : Recipe.FormatAttribute(amountNote)
				);
			this.ingredient.Add(ingredient);

			return ingredient;
		}


		public RecipeIngredientsSection(string title = null)
		{
			this.title = Recipe.FormatAttribute(title);
		}

		/// <summary>
		/// For XML Exporting only.
		/// </summary>
		private RecipeIngredientsSection() { }
	}

	/// <remarks/>
	[System.Serializable()]
	[System.ComponentModel.DesignerCategory("code")]
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class RecipeSectionIngredient
	{
		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute()]
		public string name { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute()]
		public string nameNote { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute()]
		public string amount { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute()]
		public string amountNote { get; set; }

		public RecipeSectionIngredient(string name, string amount, string nameNote = null, string amountNote = null)
		{
			this.name = Recipe.FormatAttribute(name);
			this.amount = Recipe.FormatAttribute(amount);
			this.nameNote = Recipe.FormatAttribute(nameNote);
			this.amountNote = Recipe.FormatAttribute(amountNote);
		}

		/// <summary>
		/// For XML Exporting only.
		/// </summary>
		private RecipeSectionIngredient() { }
	}

	#endregion Ingredients

	// ----------------------------------------------------------------------------------------------------
	#region Instructions

	/// <remarks/>
	[System.Serializable()]
	[System.ComponentModel.DesignerCategory("code")]
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class RecipeInstructionsSection
	{
		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute()]
		public string title { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlElement("instruction")]
		public List<string> instruction { get; } = new List<string>();

		public void Add(string instruction)
		{
			this.instruction.Add(instruction);
		}

		public void Append(string instructionAddition)
		{
			var instruction = this.instruction.Last();
			this.instruction.Remove(instruction);
			instruction = $"{instruction}<br />{instructionAddition}";
			this.instruction.Add(instruction);
		}

		public RecipeInstructionsSection(string title = null)
		{
			this.title = Recipe.FormatAttribute(title);
		}

		/// <summary>
		/// For XML Exporting only.
		/// </summary>
		private RecipeInstructionsSection() { }
	}

	#endregion Instructions
}
