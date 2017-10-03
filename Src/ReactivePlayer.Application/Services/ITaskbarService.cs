using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Application.Services
{
    public interface ITaskbarService
    {
        // show playback progress on taskbar: playing green, pause yellow
        // jumplist buttons
        // thumb buttons
        // possible dependencies: IVisualizerService -> show animation in thumbnail
        void SetTaskbarProgressValue(double value);

        void SetTaskbarProgressStatus();
    }
}