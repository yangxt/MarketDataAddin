using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text;

namespace MarketDataAddin
{
    public class Macro
    {

        public static readonly Macro instance = new Macro();

        public bool isStarted = false;

        public LinkedList<string> formulas = new LinkedList<string>();
        public Dictionary<string, StringBuilder> valueDict = new Dictionary<string, StringBuilder>();
        
        
        public Excel.Application excelApp;
        public Excel.Workbooks workbooks;
        public Excel.Sheets worksheets;

        public string userID;

        public int interval;
        public string hostname;
        public int hostport;
        public string username;
        public string password;
        public string debug;

        public Macro()
        {

        }
    }
}
