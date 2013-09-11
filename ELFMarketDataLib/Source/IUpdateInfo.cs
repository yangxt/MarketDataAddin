using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ELFMarketDataLib
{
    interface IUpdateInfo
    {
        string ItemName { get; }
        int ItemPos { get; }
        int NumFields { get; }
        bool Snapshot { get; }

        string GetNewValue(int fieldPos);
        string GetNewValue(string fieldName);
        string GetOldValue(int fieldPos);
        string GetOldValue(string fieldName);
        bool IsValueChanged(int fieldPos);
        bool IsValueChanged(string fieldName);
    }
}
