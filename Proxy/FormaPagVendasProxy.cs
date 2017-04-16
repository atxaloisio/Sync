using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.FormaPagVendasReference;
using Model;
using BLL;
using System.Windows.Forms;

namespace Sync
{
    public class FormaPagVendasProxy : Proxy, IDisposable
    {
        private FormasPagVendasSoapClient soapClient;
        private FormasPagVendaBLL formasPagVendaBLL;
        public FormaPagVendasProxy()
        {
            soapClient = new FormasPagVendasSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);

            formasPagVendaBLL = new FormasPagVendaBLL();
        }

        public void SyncFormaPagVendas(int pagina = -1)
        {
            try
            {
                venparListarRequest filtro = new venparListarRequest();

                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de  Forma de Pagamento de Vendas";
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

                venparListarResponse resp = soapClient.ListarFormasPagVendas(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.total_de_registros);
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                pagina = Convert.ToInt16(resp.pagina);

                foreach (cadastros item in resp.cadastros)
                {
                    if (formasPagVendaBLL.getFormasPagVenda(p => p.cDescricao == item.cDescricao).Count == 0)
                    {
                        FormasPagVenda formasPagVenda = new FormasPagVenda()
                        {
                            cCodigo = item.cCodigo,
                            cDescricao = item.cDescricao,
                            nQtdeParc = Convert.ToInt32(item.nQtdeParc)
                        };

                        formasPagVendaBLL.AdicionarFormasPagVenda(formasPagVenda);
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
                    SyncFormaPagVendas(pagina);
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
        // ~FormaPagVendasProxy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            formasPagVendaBLL.Dispose();
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
