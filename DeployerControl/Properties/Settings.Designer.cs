﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Stamats.buildhelper.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://team:8080/tfs/WebSites")]
        public string TFSServerUrl {
            get {
                return ((string)(this["TFSServerUrl"]));
            }
            set {
                this["TFSServerUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("$/BuildingsDev")]
        public string BaseTFSDir {
            get {
                return ((string)(this["BaseTFSDir"]));
            }
            set {
                this["BaseTFSDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\development.stamats.com\\BuildingsDev")]
        public string BaseDeploymentDir {
            get {
                return ((string)(this["BaseDeploymentDir"]));
            }
            set {
                this["BaseDeploymentDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\development.stamats.com\\BuildingsDev\\_build")]
        public string BaseZipDeploymentDir {
            get {
                return ((string)(this["BaseZipDeploymentDir"]));
            }
            set {
                this["BaseZipDeploymentDir"] = value;
            }
        }
    }
}
