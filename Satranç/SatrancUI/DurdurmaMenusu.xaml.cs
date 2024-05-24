using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SatrancUI
{
    /// <summary>
    /// DurdurmaMenusu.xaml etkileşim mantığı
    /// </summary>
    public partial class DurdurmaMenusu : UserControl
    {
        public event Action<Secenek, MainWindow> SecilenSecenek; // MainWindow parametresi eklendi
        public MainWindow mainWindow;

        public DurdurmaMenusu(MainWindow mainWindow) // Kurucuya MainWindow parametresi eklendi
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void AnaMenu_Click(object sender, RoutedEventArgs e)
        {
            SecilenSecenek?.Invoke(Secenek.AnaMenu, mainWindow);
        }
        private void DevamEtMenu_Click(object sender, RoutedEventArgs e)
        {
            SecilenSecenek?.Invoke(Secenek.DevamEt, mainWindow);
        }

        private void CikisMenu_Click(object sender, RoutedEventArgs e)
        {
            SecilenSecenek?.Invoke(Secenek.Cikis, mainWindow);
        }
    }
}
