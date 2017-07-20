using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Services
{
    interface IDialogService
    {
        Task<TResult> ShowMessageBox<TResult>();
        void ShowTrayBaloon();
        void ShowToastNotification(); // vedere come si chiama su Windows 10 la notifica che appare scorrevole
    }
}