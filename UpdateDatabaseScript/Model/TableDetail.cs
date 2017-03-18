
using _3Sharp.WpfLib;

namespace UpdateDatabaseScript.Model
{
    public class TableDetail : BaseViewModel
    {

        private bool isChecked;
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }


        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; NotifyPropertyChanged("IsChecked"); }
        }

    }
}
