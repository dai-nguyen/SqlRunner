using System.Windows.Controls;

namespace SqlRunner.Content
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();

            Populate();
        }

        private void Populate()
        {
            txtContent.Text = @"
Written by Dai Nguyen

Credits:
- SQLITE
- ClosedXML
- Entity Framework
- Json.NET
- Modern UI Icons
- ModernUI
";
        }
    }
}
