﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Services
{
    public class AddedEvent<T>
    {
        public AddedEvent(T data)
        {
            this.Data = data;
        }

        public T Data { get; }
    }
}