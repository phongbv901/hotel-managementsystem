﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotelDAL.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=PHUONGLHK;Initial Catalog=HMS;Persist Security Info=True;User ID=sa;P" +
            "assword=123456")]
        public string qrvn_hotelmanagementConnectionString1 {
            get {
                return ((string)(this["qrvn_hotelmanagementConnectionString1"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=PHUONGLHK;Initial Catalog=HMS;Persist Security Info=True;User ID=sa;P" +
            "assword=123456")]
        public string qrvn_hotelmanagementConnectionString {
            get {
                return ((string)(this["qrvn_hotelmanagementConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=112.213.89.76;Initial Catalog=aduvina_kimnganhotel;User ID=phuonglhk;" +
            "Password=123456")]
        public string aduvina_kimnganhotelConnectionString {
            get {
                return ((string)(this["aduvina_kimnganhotelConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=PHUONGLHK\\PHUONGLHK;Initial Catalog=HMS;User ID=sa;Password=123456")]
        public string HMSConnectionString {
            get {
                return ((string)(this["HMSConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=112.213.89.76;Initial Catalog=phuong_ksminhnhat;User ID=phuong_ksminh" +
            "nhat;Password=khanhphuong99")]
        public string phuong_ksminhnhatConnectionString {
            get {
                return ((string)(this["phuong_ksminhnhatConnectionString"]));
            }
        }
    }
}