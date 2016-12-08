﻿using System.Collections.Generic;
using ToDoCore.ViewModels;

namespace ToDoCore.ViewModelRetrievers
{
    public interface IViewModelRetriever<T> where T : IViewModel
    {
        IEnumerable<T> Get(int id);
    }
}