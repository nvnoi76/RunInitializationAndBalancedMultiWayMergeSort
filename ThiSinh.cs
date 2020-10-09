using System;
using System.Collections.Generic;
using System.Linq;

namespace RunInitializationAndBalancedMultiWayMergeSort
{
    public class ThiSinh
    {
        public long m_nThiSinhID;
        public decimal m_dDiemToan, m_dDiemVan, m_dDiemNgoaiNgu;
        public decimal m_dDiemTong;
        public int m_nHang = 0;
        public void TinhDiemTong()
        {
            m_dDiemTong = (m_dDiemToan + m_dDiemVan) * 2 + m_dDiemNgoaiNgu;
        }
        public void FromString(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;
            char[] delime = { ',' };
            string[] str = line.Split(delime);
            m_nThiSinhID = Convert.ToInt64(str[0]);
            m_dDiemToan = Convert.ToDecimal(str[1]);
            m_dDiemVan = Convert.ToDecimal(str[2]);
            m_dDiemNgoaiNgu = Convert.ToDecimal(str[3]);
            TinhDiemTong();
        }
        public override string ToString()
        {
            string s = string.Format("{0},{1},{2},{3},{4},{5}", m_nThiSinhID, m_dDiemToan, m_dDiemVan, m_dDiemNgoaiNgu, m_dDiemTong, m_nHang);
            return s;
        }
        public void Test()
        {
            string s = "53010563,9.4,8.25,7.4";
            FromString(s);
            Console.WriteLine(ToString());
        }
        static public int CompareID(ThiSinh first, ThiSinh second)
        {
            return (int)(first.m_nThiSinhID - second.m_nThiSinhID);
        }
        static public int CompareDiemTong(ThiSinh first, ThiSinh second)
        {
            if (first.m_dDiemTong < second.m_dDiemTong)
                return 1;
            else if (first.m_dDiemTong == second.m_dDiemTong)
                return 0;
            return -1;
        }
    }
}
