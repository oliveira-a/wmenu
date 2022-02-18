// ****************************************************************************
// <copyright file="App.cs">
//    Copyright © Andre Brasil 2022 <brasil.a@pm.me>
// </copyright>
// ****************************************************************************

using System;

namespace wmenu
{
    public class App : IComparable
    {
        public string name;
        public string path;

        public int CompareTo(object obj)
        {
            return string.Compare(this.name, ((App)obj).name);
        }
    }
}
