using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Services.Client;
using System.Net.Http;
using Newtonsoft.Json;

namespace DynamicClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DynamicEntities meta = null;
        private ProxyODataService odata = null;
        public MainWindow()
        {
            InitializeComponent();

            InitServices();
            DataContext = ViewModel = new DynamicViewModel();
            ViewModel.TableColumns = new ObservableCollection<ColumnDescription>();
        }

        private void InitServices()
        {
            meta = new DynamicEntities(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_meta"]));
            var templates = (from t in meta.DynamicTemplates.Expand("DynamicTemplateAttributes/DynamicAttribute")
                             select t);
            odata = new ProxyODataService(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]));
            odata.LoadTypes(templates);

            TableList.ItemsSource = templates.ToList().Select(x => x.Name);
            IsTest.IsChecked = true;
        }

        public DynamicViewModel ViewModel { get; set; }

        private void BtnLoad_OnClick(object sender, RoutedEventArgs e)
        {
            if (TableList.SelectedValue != null)
            {
                var qry = odata.GetServiceQuery(TableList.SelectedValue.ToString());
                ViewModel.TableRows = new ObservableCollection<dynamic>(qry.AsQueryable()/*.Where("Firstname=\"Igor\"")*/.ToDynamicArray());
                MessageBox.Show("Data loaded!");
            }
            else MessageBox.Show("Table is not selected!");

            //qry.Where("Firstname=\"Igor\"");
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var odata = new ProxyODataService(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]));

            string columns = string.Join(",", ViewModel.TableColumns.Select(c => string.Format("{0}:{1}:{2}:{3}", c.Name, (int)c.Type, c.IsNull, c.DefaultValue)));
            if (string.IsNullOrEmpty(EntityName.Text))
            {
                MessageBox.Show("New Table Name is empty!");
                return;
            }
            if(ViewModel.TableColumns.Count == 0)
            {
                MessageBox.Show("Columns are empty!");
                return;
            }
            var res = odata.Execute<string>(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]
                + "/CreateTableSmo?template='" + EntityName.Text
                + "'&operation='" + "create"
                + "'&tableName='" + EntityName.Text
                + "'&f='" + ((IsTest.IsChecked ?? false) ? "T" : "Y")
                + "'&columns='" + columns + "'"), "POST", true);
            var s = (res?.FirstOrDefault() ?? "");
            EntityName.Text = "";
            ViewModel.TableColumns.Clear();
            InitServices();
            MessageBox.Show(s);
        }

        private void DeleteEntityBtn_Click(object sender, RoutedEventArgs e)
        {
            var odata = new ProxyODataService(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]));

            if (TableList.SelectedValue != null)
            {
                var res = odata.Execute<string>(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]
                    + "/CreateTableSmo?template='" + TableList.SelectedValue.ToString()
                    + "'&operation='" + "drop"
                    + "'&tableName='" + TableList.SelectedValue.ToString()
                    + "'&f='" + ((IsTest.IsChecked ?? false) ? "T'" : "Y'")), "POST", true);
                var s = (res?.FirstOrDefault() ?? "");
                InitServices();
                MessageBox.Show(s);
            }
            else MessageBox.Show("Table is not selected!");
        }
    }

    public class ColumnDescription
    {
        public string Name { get; set; }
        public SqlDataType Type { get; set; }
        public bool IsNull { get; set; } = true;
        public string DefaultValue { get; set; }

        public enum SqlDataType
        {
            Integer = 10,
            Boolean = 3,
            Datetime = 6,
            Decimal = 7,
            Text = 15
        }
    }

    public class DynamicViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<dynamic> _tableRows;
        public ObservableCollection<dynamic> TableRows
        {
            get { return _tableRows; }
            set
            {
                _tableRows = value;
                OnPropertyChanged();
            }
        }



        private ObservableCollection<ColumnDescription> _tableColumns;
        public ObservableCollection<ColumnDescription> TableColumns
        {
            get { return _tableColumns; }
            set
            {
                _tableColumns = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
