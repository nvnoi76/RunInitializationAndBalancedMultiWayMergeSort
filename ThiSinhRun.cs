using System;
using System.IO;

namespace RunInitializationAndBalancedMultiWayMergeSort
{
    class ThiSinhRun
    {
        public ThiSinh[] m_arrThiSinh;
        public int m_nSize;
        public ThiSinhRun(int n)
        {
            m_nSize = n;
            m_arrThiSinh = new ThiSinh[n];
        }
        public static void shift<T>(T[] a, int q, int r, Comparison<T> comparison)
        {
            int i = q, j = 2 * i + 1;
            T x = a[i];
            while (j <= r)
            {
                if ((j < r) && (comparison(a[j], a[j + 1]) > 0))
                    j++;
                if (comparison(x, a[j]) < 0)
                    break;
                else
                {
                    a[i] = a[j];
                    i = j;
                    j = i * 2 + 1;
                }
            }
            a[i] = x;
        }
        public int [] Distribute(string sfi, string[] sfo, int numfile)
        {
            int n = 0;

            int i, q, r;
            int m, mh;
            int k = 0;
            int[] numrun = new int[numfile];

            ThiSinh b; 

            StreamReader fi = new StreamReader(new FileStream(sfi,FileMode.Open,FileAccess.Read));
            while ((n < m_nSize) && (!fi.EndOfStream))
            {
                m_arrThiSinh[n] = new ThiSinh();
                m_arrThiSinh[n].FromString(fi.ReadLine());
                //Console.WriteLine(m_arrThiSinh[n].ToString());
                n++;
            }

            m = min(m_nSize, n);
            mh = m / 2;
            i = mh - 1;
            while (i >= 0)
            {
                shift<ThiSinh>(m_arrThiSinh, i, m - 1, ThiSinh.CompareID);
                i--;
            }
            StreamWriter[] fo = new StreamWriter[numfile];
            for (int f = 0; f < numfile; f++)
            {
                fo[f] = new StreamWriter(new FileStream(sfo[f],FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                numrun[f] = 0;
            }

            q = m - 1;
            while (!fi.EndOfStream)
            {
                b = new ThiSinh();
                b.FromString(fi.ReadLine());
                //Console.WriteLine(b.ToString());
                n++;
                fo[k].WriteLine(m_arrThiSinh[0].ToString());

                if (ThiSinh.CompareID(m_arrThiSinh[0], b)<0)
                {
                    m_arrThiSinh[0] = b;
                    shift<ThiSinh>(m_arrThiSinh, 0, q, ThiSinh.CompareID);
                }
                else
                {
                    m_arrThiSinh[0] = m_arrThiSinh[q];
                    shift<ThiSinh>(m_arrThiSinh, 0, q - 1, ThiSinh.CompareID);
                    m_arrThiSinh[q] = b;
                    if (q < mh)
                        shift<ThiSinh>(m_arrThiSinh, q, m - 1, ThiSinh.CompareID);
                    q = q - 1;
                    if (q < 0)
                    {
                        q = m - 1;
                        numrun[k]++;
                        k++;
                        if (k >= numfile)
                            k -= numfile;
                    }
                }
            }
            r = m - 1;
            while (q >= 0)
            {                
                fo[k].WriteLine(m_arrThiSinh[0].ToString());
                m_arrThiSinh[0] = m_arrThiSinh[q];
                shift<ThiSinh>(m_arrThiSinh, 0, q - 1, ThiSinh.CompareID);
                m_arrThiSinh[q] = m_arrThiSinh[r];
                r = r - 1;
                if (q < mh)
                    shift<ThiSinh>(m_arrThiSinh, q, r, ThiSinh.CompareID);
                q = q - 1;
            }

            numrun[k]++;
            k++;
            if (k >= numfile)
                k -= numfile;
            while (r >= 0)
            {
                fo[k].WriteLine(m_arrThiSinh[0].ToString());
                m_arrThiSinh[0] = m_arrThiSinh[r];
                r = r - 1;
                shift<ThiSinh>(m_arrThiSinh, 0, r, ThiSinh.CompareID);
            }

            numrun[k]++;
            k++;
            if (k >= numfile)
                k -= numfile;

            for (int f = 0; f < numfile; f++)
            {
                //Console.WriteLine("Tap tin {0} co {1} run", sfo[f], numrun[f]);
                fo[f].Close();
            }

            fi.Close();
            return numrun;
        }
        public static int min(int m, int n)
        {
            return (m < n) ? m : n;
        }
        public void ReadFromCSVFile(string filename)
        {
            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    file.ReadLine();
                    int i = 0;
                    while (!file.EndOfStream && (i < m_nSize))
                    {
                        string s = file.ReadLine();
                        m_arrThiSinh[i].FromString(s);
                        i++;
                    }
                    Console.WriteLine("Tong so {0}", i);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void ShowThiSinhArray()
        {
            for (int i = 0; i < m_nSize; i++)
            {
                Console.WriteLine(m_arrThiSinh[i].ToString());
            }
        }
        public void WriteToCSVFile(string filename)
        {
            using (StreamWriter file = new StreamWriter(filename))
            {

                file.WriteLine("MAHS,Toan,Van,NgoaiNgu,Tong,Hang");
                for (int i = 0; i < m_nSize; i++)
                {
                    file.WriteLine(m_arrThiSinh[i].ToString());
                }
            }
        }
        public void WriteTopToCSVFile(string filename, int n)
        {
            using (StreamWriter file = new StreamWriter(filename))
            {
                file.WriteLine("MAHS,Toan,Van,NgoaiNgu,Tong,Hang");
                int top = (n < m_nSize) ? n : m_nSize;
                int i = 0;
                while ((i < m_nSize) && (m_arrThiSinh[i].m_nHang <= top))
                {
                    file.WriteLine(m_arrThiSinh[i].ToString());
                    i++;
                }
            }
        }
    }
}