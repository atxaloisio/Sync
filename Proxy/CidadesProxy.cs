using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.CidadesCadastroReference;
using Model;
using BLL;
using System.Windows.Forms;

namespace Sync
{
    public class CidadesProxy : Proxy, IDisposable
    {
        private CidadesCadastroSoapClient soapClient;
        private CidadeBLL cidadeBLL;
        public CidadesProxy()
        {
            soapClient = new CidadesCadastroSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);

            cidadeBLL = new CidadeBLL();
        }

        public void SyncCidades(int pagina = -1)
        {
            try
            {
                cidListarRequest filtro = new cidListarRequest();

                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de  Cidades";
                    Application.DoEvents();
                }

                filtro.registros_por_pagina = "50";

                if (pagina == -1)
                {
                    filtro.pagina = "1";
                }
                else
                {
                    filtro.pagina = pagina.ToString();
                }

                cidListarResponse resp = soapClient.PesquisarCidades(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.total_de_registros);
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                pagina = Convert.ToInt16(resp.pagina);

                foreach (lista_cidades item in resp.lista_cidades)
                {
                    if (cidadeBLL.getCidade(p => p.cCod == item.cCod).Count == 0)
                    {
                        Cidade cidade = new Cidade()
                        {
                            cCod = item.cCod,
                            cNome = item.cNome,
                            cUF = item.cUF,
                            nCodIBGE = item.nCodIBGE,
                            nCodSIAFI = item.nCodSIAFI
                        };

                        cidadeBLL.AdicionarCidade(cidade);
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

                if (pagina < Convert.ToInt16(resp.total_de_paginas))
                {
                    pagina++;
                    SyncCidades(pagina);
                }
                //if (formasPagVendaBLL.getFormasPagVenda().Count() < Convert.ToInt32(resp.total_de_registros) )
                //{

                //}                                                                             
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CidadesProxy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            cidadeBLL.Dispose();
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
