using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ELFMarketDataLib
{
    class UpdateInfo : IUpdateInfo
    {
        public string ItemName
        {
            get { throw new NotImplementedException(); }
        }

        public int ItemPos
        {
            get { throw new NotImplementedException(); }
        }

        public int NumFields
        {
            get { throw new NotImplementedException(); }
        }

        public bool Snapshot
        {
            get { throw new NotImplementedException(); }
        }

        public string GetNewValue(int fieldPos)
        {
            throw new NotImplementedException();
        }

        public string GetNewValue(string fieldName)
        {
            throw new NotImplementedException();
        }

        public string GetOldValue(int fieldPos)
        {
            throw new NotImplementedException();
        }

        public string GetOldValue(string fieldName)
        {
            throw new NotImplementedException();
        }

        public bool IsValueChanged(int fieldPos)
        {
            throw new NotImplementedException();
        }

        public bool IsValueChanged(string fieldName)
        {
            throw new NotImplementedException();
        }
    }
}
