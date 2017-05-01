using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;
using Utils;
using Model;

namespace Sync
{
    public class Proxy : IProxy
    {
        public Usuario usuario { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public Label Mensagem { get; set; }
        public Label QtdRegistros { get; set; }

        protected int RegistroAtual = 0;
        protected int NrTotalRegistro = 0;

        public Proxy()
        { }
        public virtual EndpointAddress setDadosAutenticacao(EndpointAddress Adress)
        {
            
            EndpointAddressBuilder builder = new EndpointAddressBuilder(Adress);

            string app_key = Parametro.GetParametro("app_key");
            string app_secret = Parametro.GetParametro("app_secret");

            builder.Headers.Add(AddressHeader.CreateAddressHeader("app_key", "", app_key)); // Coloque aqui a sua KEY de acesso
            builder.Headers.Add(AddressHeader.CreateAddressHeader("app_secret", "", app_secret)); // Coloque aqui a SECRET de acesso            

            //builder.Headers.Add(AddressHeader.CreateAddressHeader("app_key", "", "1142656109")); // Coloque aqui a sua KEY de acesso
            //builder.Headers.Add(AddressHeader.CreateAddressHeader("app_secret", "", "66b2a456a2c92eaf9646bc95587d47f0")); // Coloque aqui a SECRET de acesso            

            return builder.ToEndpointAddress();            
        }
        
    }
}
