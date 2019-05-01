using ReactivePlayer.UI.Services;
using System;

namespace ReactivePlayer.UI.WPF.Services
{
    public class WindowsTaskbarService : ITaskbarService
    {
        public void SetTaskbarProgressStatus()
        {
            throw new NotImplementedException();
        }

        public void SetTaskbarProgressStatus(TaskbarProgressStatus status)
        {
            //TaskbarItemInfo
        }

        public void SetTaskbarProgressValue(double? progressPercentValue)
        {
            //TaskbarItemInfo
        }
    }
}