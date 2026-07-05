using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Attributes
{
    /// <summary>
    /// 用來標示物件Property/Field 資料庫char資料長度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class Char_Attribute: Attribute
    {
        /// <summary>
        /// 資料長度
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="length">資料長度</param>
        public Char_Attribute(int length) => Length = length;
    }
}
