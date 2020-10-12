using System.Runtime.CompilerServices;

namespace ReactivePlayer.UI.WPF.Views
{
    public static class ViewsConstants
    {
        private static string GetCallerMemeberName([CallerMemberName] string name = null) => name;

        public static string TagNamesColumn => GetCallerMemeberName();
    }
}