namespace ServiceInsight.Highlighting
{
    using System;
    using System.Linq;
    using System.Xml;
    using ICSharpCode.AvalonEdit.Highlighting;

    /// <summary>
    /// Registers bespoke highlighting for MvvmTextEditor
    /// </summary>
    static class Resources
    {
        public static void RegisterHighlightings()
        {
            if (HighlightingManager.Instance.HighlightingDefinitions.ToList().Exists(p => p.Name == "StackTrace"))
            {
                return;
            }

            // Load our custom highlighting definition
            IHighlightingDefinition stackTraceHighlighting;
            using (var s = typeof(Resources).Assembly.GetManifestResourceStream("ServiceInsight.Highlighting.StackTrace.xshd"))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not find embedded resource");
                }

                using (XmlReader reader = new XmlTextReader(s))
                {
                    stackTraceHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("StackTrace", new[]
            {
                ".trace"
            }, stackTraceHighlighting);
        }
    }
}