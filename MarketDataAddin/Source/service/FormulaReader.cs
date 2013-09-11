
using Excel = Microsoft.Office.Interop.Excel;

namespace MarketDataAddin
{
    class FormulaReader
    {
        private Logger logger = Logger.instance;
        private Macro macro = Macro.instance;

        public FormulaReader()
        {
            macro.formulas.Clear();
        }

        public void ReadFormula()
        {
            foreach (Excel.Worksheet worksheet in macro.worksheets)
            {
                addKeyValue(worksheet);
            }
        }
        private void addKeyValue(Excel.Worksheet worksheet)
        {
            int iRow = 1;
            int maxRows = 1;
            int iCol = 1;
            int maxCols = 1;

            if (worksheet.UsedRange.Count < 1)
            {
                return;
            }
            try
            {
                maxRows = worksheet.UsedRange.CurrentRegion.Rows.Count;
                maxCols = worksheet.UsedRange.CurrentRegion.Columns.Count;
            }
            catch (System.Exception ex)
            {
                logger.WriteInfo("Exception" + ex.Message);
            }


            for (int i = 1; i <= maxRows; i++)
            {
                iRow = i;
                for (int j = 1; j <= maxCols; j++)
                {
                    iCol = j;
                    Excel.Range range = (Excel.Range)worksheet.UsedRange.CurrentRegion.Cells[i, j];
                    add(range);
                }
            }
        }
        private void add(Excel.Range range)
        {
            string iValue = null;
            if (range.Formula.ToString().Contains("="))
            {
                string[] value = range.Formula.ToString().Split(new char[] { '=' });
                iValue = value[1];
            }
            if (iValue == null)
            {
                return;
            }
            macro.formulas.AddLast(iValue);
        }
    }
}
