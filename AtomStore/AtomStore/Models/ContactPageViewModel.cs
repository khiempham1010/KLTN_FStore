﻿using AtomStore.Application.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtomStore.Models
{
    public class ContactPageViewModel
    {
        public ContactViewModel Contact { set; get; }
        public FeedbackViewModel Feedback { set; get; }
    }
}
