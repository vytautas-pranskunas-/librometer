﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Librometer.ServicesSQLCE.datas;

namespace Librometer.ServicesSQLCE
{
    public partial class AuthorService
    {
        protected override System.Collections.Generic.List<string> ValidationDeleting(Model.AuthorModel toDelete)
        {
            var errors = new List<string>();
            IQueryable<Book> books =
                books = from Book book in Context.Books
                             select book;
            if (books.Where(b => b.IdAuthor == toDelete.Id).Count() != 0)
            {
                errors.Add("Un ou plusieurs auteurs sont liés à un livre");//TODO: mettre dans une ressource
                return errors;
            }

            return errors;
        }
    }
}
