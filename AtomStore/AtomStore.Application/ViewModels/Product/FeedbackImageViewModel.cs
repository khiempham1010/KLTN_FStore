using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.ViewModels.Product
{
    public class FeedbackImageViewModel
    {
        public int Id { get; set; }

        public int ProductFeedbackId { get; set; }

        public ProductFeedbackViewModel Feedback { get; set; }

        public string Path { get; set; }

        public string Caption { get; set; }
    }
}
