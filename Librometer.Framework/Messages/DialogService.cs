﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

using DeepForest.Phone.Assets.Tools;
using DeepForestAlias = DeepForest.Phone.Assets;
using Librometer.Adapters;
using Rakouncom.WP.IsolatedStorage;
using DeepForest.Phone.Assets.Shell;
using System.Windows.Data;

namespace Librometer.Framework
{
    public class DialogService : IDialogService
    {
        public void OpenSaveOrCancelWindow<T>( 
                    T dataContext, INavigationServiceFacade nav, Predicate<T> actionIfOK,
                    Predicate<T> actionIfKO)
        {
            /*
             * Grosse astuce permettant d'afficher une PhoneApplicationPage 
             * depuis le code C#. Grossièrement je navigue vers la page SaveOrCancel
             * à partir de ma PhoneApplicationFrame, ensuite je l'édite à
             * l'exécution pour y mettre le DataContext désiré.
             */
            System.Windows.Navigation.NavigatedEventHandler handler = null;
            handler = delegate(object s, System.Windows.Navigation.NavigationEventArgs o)
            {
                if (o.NavigationMode == System.Windows.Navigation.NavigationMode.New &&
                    nav.Frame.Content is SaveOrCancelPage)
                {
                    SaveOrCancelPage page = nav.Frame.Content as SaveOrCancelPage;
                    page.DataContext = dataContext;

                    //TODO: il faudrait voit comment intercepter l'évènement
                    // BackKeyPress suite à l'appel de la méthode GoBack
                    // pour que ce soit plus propre à mon avis.
                    page.Unloaded += (_, arg) => 
                        {
                            Predicate<T> predicateToUse = null;
                            switch (page.CloseReason)
                            {
                                case MessageBoxResult.OK:
                                    if (actionIfOK != null)
                                        predicateToUse = actionIfOK;
                                    break;
                                case MessageBoxResult.Cancel:
                                    if (actionIfKO != null)
                                        predicateToUse = actionIfKO;
                                    break;
                                default:
                                    predicateToUse = t => true;
                                    break;
                            }

                            bool okToClose = predicateToUse == null || predicateToUse(dataContext);
                            /*RGE arg.Cancel = !okToClose;*/
                        };
                }
                // il faut bien penser à ce désabonner de l'évènement afin de ne pas 
                // avoir des effets de bord (par exemple des doublons lors des opérations
                // de sauvegarde)
                nav.Frame.Navigated -= handler;

            };
            nav.Frame.Navigated += handler;
            
            nav.Frame.Navigate(
                        new Uri("/Librometer.Framework;component/Messages/SaveOrCancelPage.xaml",
                        UriKind.Relative));
        }

        public void OpenNewPage(Uri uri, INavigationServiceFacade nav)
        {
            nav.Frame.Navigate(uri);
        }
        /* code mort
        public void EditCurrentPage<T>(INavigationServiceFacade nav, string editMethodName, T dataContext)
        {
            switch (editMethodName)
            {
                case "EditAppBar":
                    {
                        EditAppBar<T>(nav, dataContext);
                    }
                    break;
                default:
                    throw new Exception("Méthode inconnue dans EditCurrentPage");
            }
        }
        */

        public bool AskConfirmation(string title, string message)
        {
            return MessageBoxResult.OK ==
                MessageBox.Show(message, title, MessageBoxButton.OKCancel);
        }

        public void DisplayInformation(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK);
        }

        /// <summary>
        /// Lance la tâche de prise de photos
        /// </summary>
        /// <returns>True si la photo a été sauvegardée, false sinon</returns>
        public void LaunchCameraCaptureTask<T>(T dataContext, T updatedDataContext, INavigationServiceFacade nav)
        {
            CameraCaptureTask cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Show();
            (dataContext as BaseViewModel).ParamOne = "";

            cameraCaptureTask.Completed += (s, e) =>
            {
                if (e.ChosenPhoto == null)
                    return;
                // Sauvegarde de l'image dans l'isolated storage
                WriteableBitmap writableBitmap = new WriteableBitmap(100, 80);
                writableBitmap.LoadJpeg(e.ChosenPhoto);

                string imageFolder = "Librometer/images/draft/";//TODO: à mettre dans une ressource
                string imageFileName = "cover.jpg";//TODO: à mettre dans une ressource

                using (var stream = IsolatedStorageHelper.CreateFile(imageFolder + imageFileName))
                {
                    writableBitmap
                                .SaveJpeg(stream, writableBitmap.PixelWidth,
                                        writableBitmap.PixelHeight, 0, 100);
                    (dataContext as BaseViewModel).ParamOne = "NewPhoto";
                }

            };
        }

        #region Méthodes privées
        /* code mort
        private void EditAppBar<T>(INavigationServiceFacade nav, T dataContext)
        {
            Microsoft.Phone.Controls.PhoneApplicationPage page =
                nav.Frame.Content as Microsoft.Phone.Controls.PhoneApplicationPage;
            Microsoft.Phone.Shell.ApplicationBarIconButton btn = 
                        page.ApplicationBar.Buttons[1] as Microsoft.Phone.Shell.ApplicationBarIconButton;

            
            DeepForestAlias.Shell.ApplicationBar appBar =
                DeepForestAlias.Shell.PhoneApplicationPage.GetApplicationBar(
                        nav.Frame.Content as Microsoft.Phone.Controls.PhoneApplicationPage);
            (nav.Frame.Content as Microsoft.Phone.Controls.PhoneApplicationPage).DataContext = dataContext;

            //appBar.Buttons[1].Visibility = Visibility.Collapsed;
            //appBar.Buttons[1].UpdateLayout();
            //Visibility btn2 = appBar.Buttons[2].Visibility;
            //btn = Visibility.Collapsed;
            //(nav.Frame.Content as Microsoft.Phone.Controls.PhoneApplicationPage).UpdateLayout();
            //(nav.Frame.Content as Microsoft.Phone.Controls.PhoneApplicationPage).DataContext = dataContext;
            //BindingExpression bindExp = appBar.Buttons[1].GetBindingExpression(
            //            DeepForestAlias.Shell.ApplicationBarIconButton.VisibilityProperty);
            //Binding bind = bindExp.ParentBinding;
            //appBar.Buttons[1].SetBinding(DeepForestAlias.Shell.ApplicationBarIconButton.VisibilityProperty, bind);
            //appBar.Buttons[1].UpdateLayout();

        }
        */
        #endregion // Méthodes privées

    }
}
