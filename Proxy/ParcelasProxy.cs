using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.ParcelasReference;
using Model;
using BLL;
using System.Windows.Forms;

namespace Sync
{
    public class ParcelaProxy : Proxy, IDisposable
    {
        private ParcelasSoapClient soapClient;
        private ParcelaBLL ParcelaBLL;
        public ParcelaProxy()
        {
            soapClient = new ParcelasSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);

            ParcelaBLL = new ParcelaBLL();
        }

        public void SyncParcela(int pagina = -1)
        {
            try
            {
                parcelaListarRequest filtro = new parcelaListarRequest();

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

                parcelaListarResponse resp = soapClient.ListarParcelas(filtro);

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
                    if (ParcelaBLL.getParcela(p => p.codigo == item.nCodigo).Count == 0)
                    {
                        Parcela Parcela = new Parcela()
                        {
                            codigo = item.nCodigo,
                            parcelas = Convert.ToInt16(item.nParcelas),
                            descricao = item.cDescricao                                                        
                        };

                        ParcelaBLL.AdicionarParcela(Parcela);
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
                    SyncParcela(pagina);
                }
                //if (ParcelaBLL.getParcela().Count() < Convert.ToInt32(resp.total_de_registros) )
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
        // ~ParcelaProxy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            ParcelaBLL.Dispose();
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
