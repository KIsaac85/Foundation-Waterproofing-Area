using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Substructure_Area
{
    class RegistrationVM : ObserveData, IDataErrorInfo
    {
        public string Error { get { return null; } }
        private string _elevation;
        private List<string> _datafaces = new List<string>();


        public Dictionary<string, string> ErrorCollection { get; private set; } = new Dictionary<string, string>();

        public string this[string elev]
        {
            get
            {
                string result = null;

                switch (elev)
                {
                    case "Elevation":
                        if (string.IsNullOrWhiteSpace(Elevation))
                            result = "Elevation cannot be empty";
                        else if (!double.TryParse(_elevation, out double x))
                            result = "Elevation must be a valid number";
                        break;

                }

                if (ErrorCollection.ContainsKey(elev))
                    ErrorCollection[elev] = result;
                else if (result != null)
                    ErrorCollection.Add(elev, result);

                OnPropertyChanged("ErrorCollection");
                return result;
            }
        }

        public string Elevation
        {
            get { return _elevation; }
            set
            {
                OnPropertyChanged(ref _elevation, value);
            }
        }



    }



}

