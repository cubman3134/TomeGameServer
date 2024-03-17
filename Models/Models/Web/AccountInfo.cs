using CommonData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AccountInfo : ModelBase
    {
        private string _username;
        private string _password;
        private string _firstName;
        private string _lastName;
        private SecurityTypes _securityType;

        public string Username 
        {
            get { return _username; }
            set 
            { 
                _username = value; 
                RaisePropertyChanged(nameof(Username)); 
            }
        }
        public string Password 
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                RaisePropertyChanged(nameof(LastName));
            }
        }

        public SecurityTypes SecurityType
        {
            get { return _securityType; }
            set
            {
                _securityType = value;
                RaisePropertyChanged(nameof(SecurityType));
            }
        }
    }
}
