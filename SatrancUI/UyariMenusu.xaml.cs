using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SatrancUI
{
    public partial class OzelUyariPenceresi : UserControl
    {
        private Action _onaySonrasiIslem; 

        public OzelUyariPenceresi(string mesaj, Action onaySonrasiIslem = null)
        {
            InitializeComponent();
            MesajMetni.Text = mesaj;
            _onaySonrasiIslem = onaySonrasiIslem;
        }

        // Klavye olaylarını dinleyebilmek için kontrol yüklendiğinde odaklanması gerekir.
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focus();
                Keyboard.Focus(this);
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void TamamButon_Click(object sender, RoutedEventArgs e)
        {
            PencereyiKapatVeTetikle();
        }

        // Kodu tekrar etmemek için kapatma ve Action çağırma işlemlerini tek metoda aldık.
        private void PencereyiKapatVeTetikle()
        {
            MainWindow mainWindow = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (mainWindow != null)
            {
                // Container'ı temizle ve menüyü gizle
                mainWindow.UyariContainer.Content = null;

                if (mainWindow.OyunIciMenuContainer.Content is UserControl altMenu)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        altMenu.Focus();
                        Keyboard.Focus(altMenu);
                    }), System.Windows.Threading.DispatcherPriority.Input);
                }
            }

            AnaMenu anaMenu = System.Windows.Application.Current.Windows.OfType<AnaMenu>().FirstOrDefault();

            if (anaMenu != null)
            {
                anaMenu.UyariContainer.Content = null;

                if (anaMenu.MenuContainer.Content is UserControl altMenu)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        altMenu.Focus();
                        Keyboard.Focus(altMenu);
                    }), System.Windows.Threading.DispatcherPriority.Input);
                }
            } 

            // Eğer bekleyen bir işlem (Action) varsa tetikle
            _onaySonrasiIslem?.Invoke();
        }
    }
}