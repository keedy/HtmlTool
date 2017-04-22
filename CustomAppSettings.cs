
using System.ComponentModel;
using System.Configuration;
namespace HtmlTool
{
    [global::System.Configuration.SettingsProvider(typeof(LocalFileSettingsProvider))]
    internal sealed class CustomAppSettings:global::System.Configuration.ApplicationSettingsBase
    {
        private static CustomAppSettings defaultInstance = ((CustomAppSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new CustomAppSettings())));

        public static CustomAppSettings Default
        {
            get
            {
                return defaultInstance;
            }
        }
        [UserScopedSetting()]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public BindingList<ContentSetting> contentSettings
        {
            get
            {
                var setInXML = this["contentSettings"];
                if (setInXML == null) { contentSettings = new BindingList<ContentSetting>(); }
                return (BindingList<ContentSetting>)this["contentSettings"];
            }
            set
            {
                this["contentSettings"] = value;
            }
        }
    }
}
