using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ModniteServer.Controls
{
    public partial class LogStreamEntry : UserControl
    {
        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register(nameof(Text), typeof(string), typeof(LogStreamEntry));

        public static readonly DependencyProperty PropertiesProperty
            = DependencyProperty.Register(nameof(Properties), typeof(ObservableCollection<(string, string)>), typeof(LogStreamEntry));

        public LogStreamEntry()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ObservableCollection<(string Key, string Value)> Properties
        {
            get { return (ObservableCollection<(string, string)>)GetValue(PropertiesProperty); }
            set
            {
                SetValue(PropertiesProperty, value);

                foreach (var (Key, Value) in Properties)
                {
                    var propertyItem = new TreeViewItem
                    {
                        HeaderTemplate = (DataTemplate)Resources["LogStreamTreeViewItemHeaderTemplate"],
                        Style = (Style)Resources["TreeViewItemStyle"],

                        // ValueTuple only has fields, but Tuple has properties which is required for binding.
                        Header = new Tuple<string, string>(Key + ":", Value)
                    };
                    this.treeRoot.Items.Add(propertyItem);
                }
            }
        }
    }
}