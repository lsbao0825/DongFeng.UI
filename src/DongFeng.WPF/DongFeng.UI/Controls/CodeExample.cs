using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace DongFeng.UI.Controls
{
    public class CodeExample : ContentControl
    {
        static CodeExample()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CodeExample), new FrameworkPropertyMetadata(typeof(CodeExample)));
        }

        public CodeExample()
        {
            Loaded += CodeExample_Loaded;
        }

        private void CodeExample_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCode();
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(CodeExample), new PropertyMetadata(null));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(CodeExample), new PropertyMetadata(null, OnCodeChanged));

        private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CodeExample control && !control._isInternalUpdate)
            {
                // If Code is set in XAML (likely before Loaded), we mark it as manual.
                // However, we should check if this change is happening during initialization.
                control._isManualCode = true;
            }
        }

        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        private bool _isInternalUpdate;
        private bool _isManualCode;

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            UpdateCode();
        }

        private void UpdateCode()
        {
            // Only update if not manually set, OR if we want to force update (removed the _isManualCode check for now to test user theory, 
            // or we can make the manual check less strict if content changes).
            // Actually, if the user explicitly REMOVED the Code property in XAML, _isManualCode shouldn't be true.
            // But if they had it before, it might have been set.
            
            if (_isManualCode || Content == null) return;

            try
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "    ",
                    OmitXmlDeclaration = true,
                    NewLineOnAttributes = false
                };

                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb, settings))
                {
                    XamlWriter.Save(Content, writer);
                }

                string xaml = sb.ToString();

                // Simple cleanup to make it more readable
                // Remove default namespaces which clutter the output
                xaml = xaml.Replace(" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", "")
                           .Replace(" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"", "");

                _isInternalUpdate = true;
                Code = xaml;
            }
            catch
            {
                _isInternalUpdate = true;
                Code = "Error generating XAML.";
            }
            finally
            {
                _isInternalUpdate = false;
            }
        }
    }
}
