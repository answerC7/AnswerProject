//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcApplication1
{
    using System;
    using System.Collections.Generic;
    
    public partial class CalendarInfo
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public string Remark { get; set; }
        public string ContactNumber { get; set; }
        public string ContactName { get; set; }
        public int CalendarType { get; set; }
        public string CalendarCode { get; set; }

        public string JoinAccounts { get; set; }
    }
}
