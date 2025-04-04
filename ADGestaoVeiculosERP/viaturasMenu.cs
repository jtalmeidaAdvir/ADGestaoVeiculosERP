
using Primavera.Extensibility.Base.Editors;
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ADGestaoVeiculosERP
{
    public class viaturasMenu : FichaViaturas
    {
        public override void TeclaPressionada(int KeyCode, int Shift, ExtensibilityEventArgs e)
        {
            
            FormMenu menu = new FormMenu(BSO, PSO);
            menu.Show();
        }

        viaturasMenu()
        {
            FormMenu menu = new FormMenu(BSO, PSO);

            // Aguarde a construção completa antes de exibir
            menu.Load += (s, e) =>
            {
                menu.Show();
            };
        }
    }
}
