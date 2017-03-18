using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Input;
using Microsoft.SqlServer.Management.Smo;
using UpdateDatabaseScript.Model;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Windows;
using _3Sharp.WpfLib;

namespace UpdateDatabaseScript.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {
        public ICommand RefreshListBox { get; set; }
        public ICommand SelectionChangeCommand { get; set; }
        public ICommand GenerateScript { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand DatabaseSelectionChangeCommand { get; set; }



        public ObservableCollection<string> Databases { get; set; }

        private const string CollateString = "COLLATE Latin1_General_CI_AS";


        private string _selectedDatabase;

        public string SelectedDatabase
        {
            get { return _selectedDatabase; }
            set { _selectedDatabase = value; NotifyPropertyChanged("SelectedDatabase"); }
        }


        private bool _isInsertStatement;

        public bool IsInsertStatement
        {
            get { return _isInsertStatement; }
            set { _isInsertStatement = value; NotifyPropertyChanged("IsInsertStatement"); }
        }

        private bool _isVO;

        public bool IsVO
        {
            get { return _isVO; }
            set { _isVO = value; NotifyPropertyChanged("IsVO"); }
        }



        private string _searchString;

        public string SearchString
        {
            get { return _searchString; }
            set { _searchString = value; NotifyPropertyChanged("SearchString"); }
        }


        private ObservableCollection<TableDetail> _allSqlTables;

        public ObservableCollection<TableDetail> AllSqlTables
        {
            get { return _allSqlTables; }
            set { _allSqlTables = value; NotifyPropertyChanged("AllSQLTables"); }
        }


        private ObservableCollection<TableDetail> _tableSource;

        public ObservableCollection<TableDetail> TableSource
        {
            get { return _tableSource; }
            set { _tableSource = value; NotifyPropertyChanged("TableSource"); }
        }

        private ObservableCollection<TableDetail> _selectedTables;

        public ObservableCollection<TableDetail> SelectedTables
        {
            get { return _selectedTables; }
            set { _selectedTables = value; NotifyPropertyChanged("SelectedTables"); }
        }


        public MainWindowViewModel()
        {
            Server server;
            Database db;

            IsVO = true;

            Databases = new ObservableCollection<string> { "NORTHWND", "PLURALSIGHT" };

            SelectedDatabase = Databases.FirstOrDefault();

            GetAllTablesFromDb(out server, out db);



            RefreshListBox = new RelayCommand(x =>
            {
                if (SearchString != null)
                {
                    TableSource = new ObservableCollection<TableDetail>(AllSqlTables.Where(param => param.Name.Contains(SearchString.ToUpperInvariant())));

                }

            });


            GenerateScript = new RelayCommand(
                x =>
                {
                    StringBuilder output = GenerateScriptFromdatabase(server, db);

                    string fileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

                    StreamWriter fs = File.CreateText(fileName);
                    fs.Write(output.ToString());
                    fs.Close();

                    Process.Start(fileName);
                },
                canExecuteParam =>
                {
                    if (SelectedTables == null || SelectedTables.Count == 0)
                    {
                        return false;
                    }
                    return true;
                });

            SelectionChangeCommand = new RelayCommand(y =>
            {

                SelectedTables = new ObservableCollection<TableDetail>(AllSqlTables.Where(z => z.IsChecked));
            });


            DatabaseSelectionChangeCommand = new RelayCommand(x => {

                GetAllTablesFromDb(out server, out db);


            });

            CancelCommand = new RelayCommand(x => { Application.Current.Shutdown(); });
        }

        private StringBuilder GenerateScriptFromdatabase(Server server, Database db)
        {
            try
            {
                if (server == default(Server) || db == default(Database))
                {
                    //
                }

                StringBuilder output = new StringBuilder();

                //if (db.Tables == null || db.Tables.Count == 0)
                //{
                //    MessageBox.Show("No tables selected","BP Utility",MessageBoxButton.OK);
                //    return;
                //}

                foreach (Table dbTables in db.Tables)
                {
                    //TableSource.Where(z=>z.IsChecked).ToList().ForEach(y => {
                    //if (SelectedTables == null)
                    //{
                    //    MessageBox.Show("No tables selected", "BP Utility", MessageBoxButton.OK);
                    //    return;
                    //}
                    SelectedTables.ToList().ForEach(y =>
                    {
                        if (dbTables.Name == y.Name)
                        {
                            var scripter = new Scripter(server) { Options = { ScriptData = IsInsertStatement, Default = true, DriAll = true } };
                            var scriptRows = scripter.EnumScript(new SqlSmoObject[] { dbTables });
                            int totalLines = scriptRows.Count();


                            if (!IsVO)
                            {
                                foreach (var line in scriptRows)
                                    output.AppendLine(line);
                            }

                            else
                            {
                                //ScriptingOptions options = new ScriptingOptions
                                //{
                                ////    ClusteredIndexes = true,
                                //    Default = true,
                                //    DriAll = true,
                                //   // Indexes = true,
                                //    IncludeHeaders = true,
                                //   // ScriptData = IsInsertStatement
                                //};

                                //StringCollection scriptRows = dbTables.Script(options);
                                //int totalLines = scriptRows.Count;

                                for (int i = 0; i < totalLines; i++)
                                {
                                    if (i >= 2)
                                    {
                                        //  string scripText = scriptRows[i];
                                        string scripText = scriptRows.ElementAt(i);

                                        //string FormmatedString = "";

                                        if (i == 2)
                                        {


                                            var scriptArray = scripText.Split('\n');

                                            if (scriptArray.Any())
                                            {
                                                for (int j = 0; j < scriptArray.Count(); j++)
                                                {
                                                    if (!String.IsNullOrEmpty(scriptArray.ElementAt(j)))
                                                    {
                                                        if (j == 0)
                                                        {
                                                            scripText = String.Format("{0} {1} {2} {1} {3}", "sStatement :=", '"', scriptArray.ElementAt(j), "+ CRLF ");
                                                        }
                                                        else
                                                            scripText = String.Format("{0} {1} {2} {1} {3}", "sStatement +=", '"', scriptArray.ElementAt(j), "+ CRLF ");


                                                        scripText = scripText.Replace(CollateString, "");

                                                        output.Append(scripText);
                                                        output.Append(Environment.NewLine);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                scripText = String.Format("{0} {1} {2} {1} {3}", "sStatement :=", '"', scripText, "+ CRLF ");

                                                output.Append(scripText);
                                                output.Append(Environment.NewLine);
                                            }
                                        }
                                        else
                                        {
                                            scripText = String.Format("{0} {1} {2} {1} {3}", "sStatement +=", '"', scripText, "+ CRLF ");

                                            output.Append(scripText);

                                            output.Append(Environment.NewLine);
                                        }

                                    }
                                }
                                output.Append("AAdd(aStatements, sStatement)");
                            }



                            output.Append(Environment.NewLine);
                            output.Append(Environment.NewLine);
                        }

                    });





                }

                return output;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GetAllTablesFromDb(out Server server, out Database db)
        {
            AllSqlTables = new ObservableCollection<TableDetail>();



            //  string dbName = ConfigurationManager.AppSettings["databaseName"];

            server = DbConnect();
            db = server.Databases[SelectedDatabase];

            //AllSqlTables.Clear();

            foreach (Table item in db.Tables)
            {
                AllSqlTables.Add(new TableDetail { Name = item.Name, IsChecked = false });
            }

            TableSource = AllSqlTables;
        }

        private static Server DbConnect()
        {
            Server server = new Server();
            server.ConnectionContext.LoginSecure = false;
            server.ConnectionContext.Login = ConfigurationManager.AppSettings["loginName"];
            server.ConnectionContext.Password = ConfigurationManager.AppSettings["loginPassword"];
            server.ConnectionContext.ServerInstance = ConfigurationManager.AppSettings["serverName"];
            return server;
        }
    }
}