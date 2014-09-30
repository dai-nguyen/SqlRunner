using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlRunner.DataAccess
{
    public interface IAlertMessageService
    {
        void ShowMessage(string msg);
        void ShowMessage(bool busy, string msg);
    }
}
