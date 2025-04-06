
    // ----------------------------------------------------------------------------------------------------
    #region Ingredients

    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public class RecipeIngredients
    {
        /// <remarks/>
        public List<RecipeIngredientsSection> section { get; } = new List<RecipeIngredientsSection>();

        public RecipeIngredientsSection AddSection(string sectionTitle = null)
        {
            var newSection = new RecipeIngredientsSection(sectionTitle);
            section.Add(newSection);

            return newSection;
        }

        public RecipeIngredients()
        {
        }
    }

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
        public List<RecipeIngredientsSectionIngredient> ingredient { get; } = new List<RecipeIngredientsSectionIngredient>();

        public RecipeIngredientsSection(string title = null)
        {
            this.title = title;
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
    public class RecipeIngredientsSectionIngredient
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

        public RecipeIngredientsSectionIngredient(string name, string amount, string nameNote = null, string amountNote = null)
        {
            this.name = name;
            this.amount = amount;
            this.nameNote = nameNote;
            this.amountNote = amountNote;
        }

        /// <summary>
        /// For XML Exporting only.
        /// </summary>
        private RecipeIngredientsSectionIngredient() { }
    }

    #endregion Ingredients
