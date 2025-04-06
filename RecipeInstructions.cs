    // ----------------------------------------------------------------------------------------------------
    #region Instructions

    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public class RecipeInstructions
    {
        /// <remarks/>
        public List<RecipeInstructionsSection> section { get; } = new List<RecipeInstructionsSection>();

        public RecipeInstructionsSection AddSection(string sectionTitle = null)
        {
            var newSection = new RecipeInstructionsSection(sectionTitle);
            section.Add(newSection);

            return newSection;
        }

        public RecipeInstructions()
        {
        }
    }

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

        public RecipeInstructionsSection(string title = null)
        {
            this.title = title;
        }

        /// <summary>
        /// For XML Exporting only.
        /// </summary>
        private RecipeInstructionsSection() { }
    }

    #endregion Instructions
