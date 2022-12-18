using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Views
{
    public abstract class BaseReviewableView : BaseRecordView
    {
        public bool Reviewed { get; set; }
    }
}
