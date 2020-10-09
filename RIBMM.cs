using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace RunInitializationAndBalancedMultiWayMergeSort
{
    class RIBMM
    {
        public static int[] MakeRun(string sfi, string[] sfo, int bufsize = 10000)
        {
            const int NUMFILE = 2;
            ThiSinhRun tsr = new ThiSinhRun(bufsize); //Bo dem tao run ban dau, 1000 phan tu
            return tsr.Distribute(sfi, sfo, NUMFILE); // Vua tao run ban dau, vua phan phoi
        }
        public static int CountRun(string filename)
        {
            TSStreamReader tsReader = new TSStreamReader(filename);
            long cuid;
            int num = 0;
            while (!tsReader.EOF())
            {
                num++;
                cuid = tsReader.LookAhead().m_nThiSinhID;
                while ((cuid <= tsReader.LookAhead().m_nThiSinhID) && (!tsReader.EOF()))
                {
                    cuid = tsReader.DisposeCurrentTS().m_nThiSinhID;
                    //Console.WriteLine("{0}", cuid);
                }
                //Console.WriteLine("Het run!");
            }
            Console.WriteLine("Tap tin {0} co {1} run", filename, num);
            return num;
        }
        static void Main(string[] args)
        {
            const int BUFFERSIZE = 10000;  
            //const string sfi = "D:\\TG53NNNVT02.csv"; //Tap tin .csv co 4 cot: MAHS,Toan,Van,NgoaiNgu
            //diemthiTHPT20TVA01.csv            
            const string sfi = "d:\\diemthiTHPT20TVA01.csv";
            string[] sfo = new string[] { "D:\\T01.csv", "D:\\T02.csv", "D:\\T03.csv", "D:\\T04.csv" }; //2 tap tin phan phoi, 2 tron
            Console.WriteLine("Tao run va phan phoi tu tap tin {0} vao {1} va {2}", sfi, sfo[0], sfo[1]);
            int[] numrun = MakeRun(sfi, sfo, BUFFERSIZE);
            for (int i = 0; i < 2; i++)
                Console.WriteLine("Tap tin {0} co {1} run", sfo[i], numrun[i]);
            kwayMergeSort(sfo);
        }
        public static bool FileIsLocked(string strFullFileName)
        {
            bool blnReturn = false;
            FileStream fs;
            try
            {
                fs = File.Open(strFullFileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.None);
                fs.Close();
            }
            catch (System.IO.IOException ex)
            {
                blnReturn = true;
            }
            return blnReturn;
        }
        public static int kwayMergeSort(string[] str)
        {
            int numrun = 0;
            bool state = true;
            TSStreamReader[] fread = new TSStreamReader[2];
            StreamWriter[] fwrite = new StreamWriter[2];
            int k = 0;
            int numfile = 2;
            long cuid;
            string[] s = new string[4];
            do
            {
                numrun = 0;
                if (state)
                {
                    s[0] = str[0];
                    s[1] = str[1];
                    s[2] = str[2];
                    s[3] = str[3];
                }
                else
                {
                    s[0] = str[2];
                    s[1] = str[3];
                    s[2] = str[0];
                    s[3] = str[1];
                }
                state = !state;
                for (int i = 0; i < 4; i++)
                {
                    if (FileIsLocked(s[i]))
                        Console.WriteLine("{0} is blocked !");
                }
                fread[0] = new TSStreamReader(s[0]);
                fread[1] = new TSStreamReader(s[1]);
                fwrite[0] = new StreamWriter(new FileStream(s[2], FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                fwrite[1] = new StreamWriter(new FileStream(s[3], FileMode.Create, FileAccess.Write, FileShare.ReadWrite));

                Console.WriteLine("fw0 = {0}", s[2]);
                Console.WriteLine("fw1 = {0}", s[3]);

                while ((!fread[0].EOF()) && (!fread[1].EOF()))
                {
                    cuid = min(fread[0].LookAhead().m_nThiSinhID, fread[1].LookAhead().m_nThiSinhID);

                    while ((cuid <= fread[0].LookAhead().m_nThiSinhID) &&
                            (cuid <= fread[1].LookAhead().m_nThiSinhID) && (!fread[0].EOF()) && (!fread[1].EOF()))
                    {
                        if (fread[0].LookAhead().m_nThiSinhID <= fread[1].LookAhead().m_nThiSinhID)
                        {
                            cuid = fread[0].LookAhead().m_nThiSinhID;
                            fwrite[k].WriteLine(fread[0].DisposeCurrentTS().ToString());
                        }
                        else
                        {
                            cuid = fread[1].LookAhead().m_nThiSinhID;
                            fwrite[k].WriteLine(fread[1].DisposeCurrentTS().ToString());
                        }
                    }

                    while ((cuid <= fread[0].LookAhead().m_nThiSinhID) && (!fread[0].EOF()))
                    {
                        cuid = fread[0].LookAhead().m_nThiSinhID;
                        fwrite[k].WriteLine(fread[0].DisposeCurrentTS().ToString());
                    }

                    while ((cuid <= fread[1].LookAhead().m_nThiSinhID) && (!fread[1].EOF()))
                    {
                        cuid = fread[1].LookAhead().m_nThiSinhID;
                        fwrite[k].WriteLine(fread[1].DisposeCurrentTS().ToString());
                    }
                    numrun++;
                    k++;
                    if (k >= numfile)
                        k -= numfile;
                } //while ((!fread[0].EOF()) && (!fread[1].EOF()))

                // Chep 1 run chua tron
                //{ }
                if (!fread[0].EOF())
                {
                    while (!fread[0].EOF())
                    {
                        cuid = fread[0].LookAhead().m_nThiSinhID;
                        fwrite[k].WriteLine(fread[0].DisposeCurrentTS().ToString());
                    }
                    numrun++;
                    k++;
                    if (k >= numfile)
                        k -= numfile;
                }

                if (!fread[1].EOF())
                {
                    while (!fread[1].EOF())
                    {
                        cuid = fread[1].LookAhead().m_nThiSinhID;
                        fwrite[k].WriteLine(fread[1].DisposeCurrentTS().ToString());
                    }
                    numrun++;
                    k++;
                    if (k >= numfile)
                        k -= numfile;
                }
                fread[0].Close();
                fread[1].Close();
                fwrite[0].Close();
                fwrite[1].Close();

                Console.WriteLine("Num run = {0}", numrun);
            }
            while (numrun>1);
                        
            Console.WriteLine("Tap tin ket qua {0}", (k==1)?s[2]:s[3]);
            return numrun;
        }

        private static long min(long m, long n)
        {
            return (m <= n) ? m : n;
        }
    }
}
