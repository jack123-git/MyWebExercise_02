using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Attributes
{
    /// <summary>
    /// 用來標示物件Property/Field 資料庫decimal型別，有效位數(全部)/小數位長度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class Decimal_Attribute : Attribute
    {
        /// <summary>
        /// 有效位數
        /// </summary>
        public int Percision { get; }

        /// <summary>
        /// 小數有效位數
        /// </summary>
        public int Scale { get; }

        /// <summary>
        /// 建構子
        /// </summary>
        public Decimal_Attribute(int percision, int scale)
        {
            Percision = percision;
            Scale = scale;
        }
    }
}
