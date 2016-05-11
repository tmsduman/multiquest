using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Interfaces;

namespace PureMVCImplementations
{
    public class UnityNotification<T> : INotification
    {
        public string Name
        {
            get { return typeof(T).Name; }
        }

        public static string ID
        {
            get { return typeof(T).Name; }
        }

        public object Body
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Type
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
