using System.Windows.Controls;

namespace Libro.Dialogs
{
    /// <summary>
    /// Interaction logic for BorrowerEditor.xaml
    /// </summary>
    public partial class BorrowerEditor 
    {
        public BorrowerEditor()
        {
            InitializeComponent();
            Title = "NEW BORROWER";
            AffirmativeText = "ADD";
        }

        public BorrowerEditor(string title,string affirmative)
        {
            InitializeComponent();
            Title =title;
            AffirmativeText = affirmative;
        }

        public string Title { get; set; }
        public string AffirmativeText { get; set; }
    }
}
