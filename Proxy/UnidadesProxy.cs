using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.UnidadesCadastroReference;
using BLL;
using Model;
using System.Windows.Forms;
using Utils;

namespace Sync
{
    public class UnidadesProxy : Proxy, IDisposable
    {
        UnidadesCadastroSoapClient soapClient;
        private UnidadeBLL UnidadeBLL;
        public UnidadesProxy()
        {
            soapClient = new UnidadesCadastroSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);
            UnidadeBLL = new UnidadeBLL();
        }

        public void SyncUnidades(int pagina = -1)
        {
            try
            {
                unidade_pesquisar filtro = new unidade_pesquisar();

                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de  Unidades";
                    Application.DoEvents();
                }
                
                unidade_cadastro_lista resp = soapClient.ListarUnidades(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.unidade_cadastro.Count());
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                pagina = Convert.ToInt16("1");

                foreach (unidade_cadastro item in resp.unidade_cadastro)
                {                    
                    List<Unidade> UnidadesList = UnidadeBLL.getUnidade(c => c.codigo == item.codigo);
                    if (UnidadesList.Count() == 0)
                    {
                        Unidade Unidade = new Unidade()
                        {
                            codigo = item.codigo,
                            descricao = item.descricao,                            
                        };

                        UnidadeBLL.AdicionarUnidade(Unidade);
                    }

                    RegistroAtual++;
                    if (ProgressBar != null)
                    {
                        ProgressBar.Value = RegistroAtual;
                        ProgressBar.Refresh();
                        Application.DoEvents();
                        if (QtdRegistros != null)
                        {
                            QtdRegistros.Text = RegistroAtual.ToString() + " de " + NrTotalRegistro.ToString();
                            Application.DoEvents();
                        }
                    }
                }                
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        public void Dispose()
        {
            UnidadeBLL.Dispose();
        }
    }
}
