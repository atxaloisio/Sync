using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.ProdutosImpostosReference;
using Model;
using BLL;
using System.Windows.Forms;

namespace Sync
{
    public class ProdutosImpostosProxy : Proxy, IDisposable
    {
        private ProdutosImpostosSoapClient soapClient;
        private Produto_ImpostoBLL produto_ImpostoBLL;
        public ProdutosImpostosProxy()
        {
            soapClient = new ProdutosImpostosSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);

            produto_ImpostoBLL = new Produto_ImpostoBLL();
        }

        public void SyncProdutosImpostos(int pagina = -1)
        {
            try
            {
                priaListRequest filtro = new priaListRequest();

                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de Imppostos Aprendidos";
                    Application.DoEvents();
                }

                //cidListarRequest filtro = new cidListarRequest();
                filtro.registros_por_pagina = "50";

                if (pagina == -1)
                {
                    filtro.pagina = "1";
                }
                else
                {
                    filtro.pagina = pagina.ToString();
                }

                priaListResponse resp = soapClient.ListarImpostos(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.total_de_registros);
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                pagina = Convert.ToInt16(resp.pagina);



                foreach (listaImpostos item in resp.listaImpostos)
                {
                    int ncodprod = Convert.ToInt32(item.ident.nCodProd);

                    List<Produto_Imposto> Produto_ImpostoList = produto_ImpostoBLL.getProduto_Imposto(p => p.nCodProd == ncodprod);

                    if (Produto_ImpostoList.Count <= 0)
                    {

                        Produto_Imposto prod = new Produto_Imposto();

                        if (item.ident != null)
                        {
                            prod.nCodProd = Convert.ToInt32(item.ident.nCodProd);
                            prod.cCodIntProd = item.ident.cCodIntProd;
                            prod.cCodIntImp = item.ident.cCodIntImp;
                        }

                        if (item.cabec != null)
                        {
                            prod.cCupomFiscal = item.cabec.cCupomFiscal;
                            prod.cUfOrigem = item.cabec.cUfOrigem;
                            prod.cUfDestino = item.cabec.cUfDestino;
                            prod.cCFOP = item.cabec.cCFOP;
                            prod.cCNAECliente = item.cabec.cCNAECliente;
                            prod.cContribICMS = item.cabec.cContribICMS;
                        }

                        if (item.COFINS != null)
                        {
                            prod.cCSTCOFINS = item.COFINS.cCSTCOFINS;
                            prod.cTpCalcCOFINS = item.COFINS.cTpCalcCOFINS;
                            prod.nAliqCOFINS = item.COFINS.nAliqCOFINS;
                            prod.nValUnTribCOFINS = item.COFINS.nValUnTribCOFINS;
                        }

                        if (item.ECF != null)
                        {
                            prod.cECFNaoTrib = item.ECF.cECFNaoTrib;
                            prod.cECFSubstTrib = item.ECF.cECFSubstTrib;
                            prod.cECFIsento = item.ECF.cECFIsento;
                        }

                        if (item.ICMS != null)
                        {
                            prod.cCSTICMS = item.ICMS.cCSTICMS;
                            prod.cOrigemICMS = item.ICMS.cOrigemICMS;
                            prod.cModBCICMS = item.ICMS.cModBCICMS;
                            prod.nRedBCICMS = item.ICMS.nRedBCICMS;
                            prod.nAliqICMS = item.ICMS.nAliqICMS;
                            prod.nPercDifICMS = item.ICMS.nPercDifICMS;
                        }

                        if (item.ICMSIE != null)
                        {
                            prod.nPercFCP = item.ICMSIE.nPercFCP;
                            prod.nAlIntUFDest = item.ICMSIE.nAlIntUFDest;
                            prod.nAlInterestUFs = item.ICMSIE.nAlInterestUFs;
                            prod.nPercICMSIE = item.ICMSIE.nPercICMSIE;
                        }

                        if (item.ICMSSN != null)
                        {
                            prod.cCSOSNICMS_SN = item.ICMSSN.cCSOSNICMS;
                            prod.cOrigemICMS_SN = item.ICMSSN.cOrigemICMS;
                            prod.cModBCICMS_SN = item.ICMSSN.cModBCICMS;
                            prod.nAlCredICMS_SN = item.ICMSSN.nAlCredICMS;
                            prod.nRedBCICMS_SN = item.ICMSSN.nRedBCICMS;
                            prod.nAliqICMS_SN = item.ICMSSN.nAliqICMS;
                        }

                        if (item.ICMSST != null)
                        {
                            prod.cModBCICMSST = item.ICMSST.cModBCICMSST;
                            prod.nMargValAdICMSST = item.ICMSST.nMargValAdICMSST;
                            prod.nRedBCICMSST = item.ICMSST.nRedBCICMSST;
                            prod.nAliqICMSST = item.ICMSST.nAliqICMSST;
                            prod.nAliqOPICMSST = item.ICMSST.nAliqOPICMSST;
                            prod.cCEST = item.ICMSST.cCEST;
                        }

                        if (item.InfAdic != null)
                        {
                            prod.cInfAdicNF = item.InfAdic.cInfAdicNF;
                        }

                        if (item.IPI != null)
                        {
                            prod.cCSTIPI = item.IPI.cCSTIPI;
                            prod.cTpCalcIPI = item.IPI.cTpCalcIPI;
                            prod.nAliqIPI = item.IPI.nAliqIPI;
                            prod.nValUnTribIPI = item.IPI.nValUnTribIPI;
                            prod.cEnqIPI = item.IPI.cEnqIPI;
                        }

                        if (item.PIS != null)
                        {
                            prod.cCSTPIS = item.PIS.cCSTPIS;
                            prod.cTpCalcPIS = item.PIS.cTpCalcPIS;
                            prod.nAliqPIS = item.PIS.nAliqPIS;
                            prod.nValUnTribPIS = item.PIS.nValUnTribPIS;
                        }

                        produto_ImpostoBLL.AdicionarProduto_Imposto(prod);
                    }
                    else
                    {
                        Produto_Imposto prod = Produto_ImpostoList.First();

                        if (item.ident != null)
                        {
                            prod.nCodProd = Convert.ToInt32(item.ident.nCodProd);
                            prod.cCodIntProd = item.ident.cCodIntProd;
                            prod.cCodIntImp = item.ident.cCodIntImp;
                        }

                        if (item.cabec != null)
                        {
                            prod.cCupomFiscal = item.cabec.cCupomFiscal;
                            prod.cUfOrigem = item.cabec.cUfOrigem;
                            prod.cUfDestino = item.cabec.cUfDestino;
                            prod.cCFOP = item.cabec.cCFOP;
                            prod.cCNAECliente = item.cabec.cCNAECliente;
                            prod.cContribICMS = item.cabec.cContribICMS;
                        }

                        if (item.COFINS != null)
                        {
                            prod.cCSTCOFINS = item.COFINS.cCSTCOFINS;
                            prod.cTpCalcCOFINS = item.COFINS.cTpCalcCOFINS;
                            prod.nAliqCOFINS = item.COFINS.nAliqCOFINS;
                            prod.nValUnTribCOFINS = item.COFINS.nValUnTribCOFINS;
                        }

                        if (item.ECF != null)
                        {
                            prod.cECFNaoTrib = item.ECF.cECFNaoTrib;
                            prod.cECFSubstTrib = item.ECF.cECFSubstTrib;
                            prod.cECFIsento = item.ECF.cECFIsento;
                        }

                        if (item.ICMS != null)
                        {
                            prod.cCSTICMS = item.ICMS.cCSTICMS;
                            prod.cOrigemICMS = item.ICMS.cOrigemICMS;
                            prod.cModBCICMS = item.ICMS.cModBCICMS;
                            prod.nRedBCICMS = item.ICMS.nRedBCICMS;
                            prod.nAliqICMS = item.ICMS.nAliqICMS;
                            prod.nPercDifICMS = item.ICMS.nPercDifICMS;
                        }

                        if (item.ICMSIE != null)
                        {
                            prod.nPercFCP = item.ICMSIE.nPercFCP;
                            prod.nAlIntUFDest = item.ICMSIE.nAlIntUFDest;
                            prod.nAlInterestUFs = item.ICMSIE.nAlInterestUFs;
                            prod.nPercICMSIE = item.ICMSIE.nPercICMSIE;
                        }

                        if (item.ICMSSN != null)
                        {
                            prod.cCSOSNICMS_SN = item.ICMSSN.cCSOSNICMS;
                            prod.cOrigemICMS_SN = item.ICMSSN.cOrigemICMS;
                            prod.cModBCICMS_SN = item.ICMSSN.cModBCICMS;
                            prod.nAlCredICMS_SN = item.ICMSSN.nAlCredICMS;
                            prod.nRedBCICMS_SN = item.ICMSSN.nRedBCICMS;
                            prod.nAliqICMS_SN = item.ICMSSN.nAliqICMS;
                        }

                        if (item.ICMSST != null)
                        {
                            prod.cModBCICMSST = item.ICMSST.cModBCICMSST;
                            prod.nMargValAdICMSST = item.ICMSST.nMargValAdICMSST;
                            prod.nRedBCICMSST = item.ICMSST.nRedBCICMSST;
                            prod.nAliqICMSST = item.ICMSST.nAliqICMSST;
                            prod.nAliqOPICMSST = item.ICMSST.nAliqOPICMSST;
                            prod.cCEST = item.ICMSST.cCEST;
                        }

                        if (item.InfAdic != null)
                        {
                            prod.cInfAdicNF = item.InfAdic.cInfAdicNF;
                        }

                        if (item.IPI != null)
                        {
                            prod.cCSTIPI = item.IPI.cCSTIPI;
                            prod.cTpCalcIPI = item.IPI.cTpCalcIPI;
                            prod.nAliqIPI = item.IPI.nAliqIPI;
                            prod.nValUnTribIPI = item.IPI.nValUnTribIPI;
                            prod.cEnqIPI = item.IPI.cEnqIPI;
                        }

                        if (item.PIS != null)
                        {
                            prod.cCSTPIS = item.PIS.cCSTPIS;
                            prod.cTpCalcPIS = item.PIS.cTpCalcPIS;
                            prod.nAliqPIS = item.PIS.nAliqPIS;
                            prod.nValUnTribPIS = item.PIS.nValUnTribPIS;
                        }


                        produto_ImpostoBLL.AlterarProduto_Imposto(prod);
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
                    SyncProdutosImpostos(pagina);
                }
                                                                                             
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
        // ~ProdutosImpostosProxy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            soapClient.Close();
            produto_ImpostoBLL.Dispose();
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
