using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Classes
{
    public class CutText
    {
        public static string Cutter(string text,int MaxLength)
        {
            if (text.Length> MaxLength)
            {
                string shortText = text.Substring(0, MaxLength) +"...";
                return shortText;
            }
            return text;
        }
    }
}