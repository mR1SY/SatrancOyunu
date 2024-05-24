using SatrancMantigi;
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
    public partial class TerfiMenusu : UserControl
    {
        public event Action<TasTuru> SecilenTas;

        public TerfiMenusu(Oyuncu oyuncu)
        {
            InitializeComponent();

            VezirResmi.Source = Resimler.ResimAl(oyuncu, TasTuru.Vezir);
            FilResmi.Source = Resimler.ResimAl(oyuncu, TasTuru.Fil);
            KaleResmi.Source = Resimler.ResimAl(oyuncu, TasTuru.Kale);
            AtResmi.Source = Resimler.ResimAl(oyuncu, TasTuru.At);
        }

        private void VezirResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SecilenTas?.Invoke(TasTuru.Vezir);
        }

        private void FilResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SecilenTas?.Invoke(TasTuru.Fil);
        }

        private void KaleResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SecilenTas?.Invoke(TasTuru.Kale);
        }

        private void AtResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SecilenTas?.Invoke(TasTuru.At);
        }
    }
}
