// Updated by XamlIntelliSenseFileGenerator 10/27/2022 8:17:53 PM
#pragma checksum "..\..\AreaCalculation.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "8953965DE4FBF4F51E27263D4D63082D6994DA8524D7CE8EFFAE98988B64A433"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Substructure_Area;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Substructure_Area
{


    /// <summary>
    /// areaCalculation
    /// </summary>
    public partial class areaCalculation : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {


#line 43 "..\..\AreaCalculation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Elements_Selection;

#line default
#line hidden


#line 49 "..\..\AreaCalculation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid datagrid;

#line default
#line hidden


#line 53 "..\..\AreaCalculation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox Select_Family;

#line default
#line hidden


#line 69 "..\..\AreaCalculation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Cancel;

#line default
#line hidden

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Substructure Area;component/areacalculation.xaml", System.UriKind.Relative);

#line 1 "..\..\AreaCalculation.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.Elements_Selection = ((System.Windows.Controls.Button)(target));

#line 46 "..\..\AreaCalculation.xaml"
                    this.Elements_Selection.Click += new System.Windows.RoutedEventHandler(this.Element_Selection_Click);

#line default
#line hidden
                    return;
                case 2:
                    this.datagrid = ((System.Windows.Controls.DataGrid)(target));
                    return;
                case 3:
                    this.Select_Family = ((System.Windows.Controls.ListBox)(target));
                    return;
                case 4:
                    this.Cancel = ((System.Windows.Controls.Button)(target));

#line 70 "..\..\AreaCalculation.xaml"
                    this.Cancel.Click += new System.Windows.RoutedEventHandler(this.Cancel_Click);

#line default
#line hidden
                    return;
                case 5:
                    this.splitcolumn = ((System.Windows.Controls.Button)(target));

#line 80 "..\..\AreaCalculation.xaml"
                    this.splitcolumn.Click += new System.Windows.RoutedEventHandler(this.splitcolumn_Click);

#line default
#line hidden
                    return;
            }
            this._contentLoaded = true;
        }

        internal System.Windows.Controls.Button splitcolumn;
    }
}
