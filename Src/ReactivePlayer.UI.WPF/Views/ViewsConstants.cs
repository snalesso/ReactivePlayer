using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.Views
{
    public static class ViewsConstants
    {
        private static string GetCallerMemeberName([CallerMemberName] string name = null) => name;

        public static string TagNamesColumn => GetCallerMemeberName();
    }
}