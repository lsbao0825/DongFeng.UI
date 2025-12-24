using Markdig;
using Markdig.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DongFeng.UI.Extend.Controls
{
    [TemplatePart(Name = "PART_Viewer", Type = typeof(FlowDocumentScrollViewer))]
    public class DFMarkdown : Control
    {
        static DFMarkdown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFMarkdown), new FrameworkPropertyMetadata(typeof(DFMarkdown)));
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DFMarkdown), new PropertyMetadata(string.Empty, OnTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty MarkdownPipelineProperty =
            DependencyProperty.Register("MarkdownPipeline", typeof(MarkdownPipeline), typeof(DFMarkdown), new PropertyMetadata(null, OnTextChanged));

        public MarkdownPipeline MarkdownPipeline
        {
            get { return (MarkdownPipeline)GetValue(MarkdownPipelineProperty); }
            set { SetValue(MarkdownPipelineProperty, value); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DFMarkdown)d;
            control.UpdateMarkdown();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateMarkdown();
        }

        private void UpdateMarkdown()
        {
            var viewer = GetTemplateChild("PART_Viewer") as FlowDocumentScrollViewer;
            if (viewer == null) return;

            if (string.IsNullOrEmpty(Text))
            {
                viewer.Document = null;
                return;
            }

            try
            {
                var pipeline = MarkdownPipeline ?? new MarkdownPipelineBuilder()
                    .UseSupportedExtensions()
                    .Build();

                var document = Markdig.Wpf.Markdown.ToFlowDocument(Text, pipeline);
                viewer.Document = document;
            }
            catch
            {
                // Fallback or error display
            }
        }
    }
}

