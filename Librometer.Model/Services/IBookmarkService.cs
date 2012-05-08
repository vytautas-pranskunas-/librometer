﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Librometer.Model.Services
{
    public interface IBookmarkService:
        IBaseService<BookmarkModel, BookmarkCriteria>
    {
    }
}
