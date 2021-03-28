using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Markup;

namespace KuromeKuroKit_WPF.Localization
{
    // You exclude the 'Extension' suffix when using in XAML
    [ContentProperty("Text")]
    public class TranslateExtension : System.Windows.Markup.MarkupExtension
    {
        readonly CultureInfo ci = null;
        const string ResourceId = "KuromeKuroKit_WPF.Properties.LocaleRes";

        static readonly Lazy<ResourceManager> ResMgr = new Lazy<ResourceManager>(
            () => new ResourceManager(ResourceId, IntrospectionExtensions.GetTypeInfo(typeof(TranslateExtension)).Assembly));

        public string Text { get; set; }

        public TranslateExtension()
        {
            // var app = (GeoPager.App)App.Current;
            //if (app.CurrentCulture == null)
            //{
            //    var cul = GeoPager.Helpers.PreferencesHelper.GetUserDefinedCulture();
            //    app.CurrentCulture = cul;
            //}
            //ci = app.CurrentCulture;


                //ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            

            //var assembly = typeof(Properties.UIResources.UIResx).GetTypeInfo().Assembly; // "EmbeddedImages" should be a class in your app
            //foreach (var res in assembly.GetManifestResourceNames())
            //{
            //    System.Diagnostics.Debug.WriteLine("found resource: " + res);
            //}
        }

        public TranslateExtension(string text) 
        {
            Text = text;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return string.Empty;

            var translation = ResMgr.Value.GetString(Text, ci);
            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    string.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.",
                                  Text,
                                  ResourceId,
                                  ci.Name),
                    "Text");
#else
                translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
            }
            return translation;
        }
    }
}
