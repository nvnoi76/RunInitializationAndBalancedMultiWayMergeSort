using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace RunInitializationAndBalancedMultiWayMergeSort
{
    public class TSStreamReader
    {
        public  ThiSinh currentThisinh;
        bool bRead;
        StreamReader streamReader;
        FileStream fs;
        public TSStreamReader(string filename)
        {
            fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite|FileShare.Delete);
            streamReader = new StreamReader(fs);
            bRead = false;
            currentThisinh = new ThiSinh();
        }
        public ThiSinh LookAhead()
        {
            if (!bRead)
                FetchNext();
            return currentThisinh;
        }
        public ThiSinh DisposeCurrentTS()
        {
            bRead = false;
            return currentThisinh;
        }
        public bool FetchNext()
        {
            //bRead must be false
            if (streamReader.EndOfStream)
                return false;
            currentThisinh.FromString(streamReader.ReadLine());
            bRead = true;
            return bRead;
        }
        public bool EOF()
        {
            if (bRead)
                return false;
            return streamReader.EndOfStream;
        }
        public void Close()
        {
            fs.Close();
            streamReader.Dispose();
            streamReader.Close();
            bRead = false;
        }
        ~TSStreamReader()
        {
            this.Close();
        }
    }
}
