﻿using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace ReactivePlayer.App.Services
{
    public interface IImageAnalyzer
    {
        ImageMimeType CalculateImageDataMimeType(IEnumerable<byte> data);
        Tuple<int, int> CalculateDimensions(IEnumerable<byte> data);
    }
}