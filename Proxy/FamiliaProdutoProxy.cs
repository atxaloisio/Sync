using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.FamiliasProdutoReference;
using BLL;
using Model;
using System.Windows.Forms;
using Utils;

namespace Sync
{
    public class FamiliaProdutoProxy : Proxy, IDisposable
    {
        FamiliasCadastroSoapClient soapClient;
        private Familia_ProdutoBLL Familia_ProdutoBLL;
        public FamiliaProdutoProxy()
        {
            soapClient = new FamiliasCadastroSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);
            Familia_ProdutoBLL = new Familia_ProdutoBLL();
        }

        public void SyncFamilia_Produto(int pagina = -1)
        {
            try
            {
                SyncLocalToOmie();
                SyncOmieToLocal(pagina);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private void SyncOmieToLocal(int pagina = -1)
        {
            try
            {
                famListarRequest filtro = new famListarRequest();

                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de Familia de Produtos";
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

                famListarResponse resp = soapClient.PesquisarFamilias(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.total_de_registros);
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                pagina = Convert.ToInt16(resp.pagina);

                foreach (famCadastro item in resp.famCadastro)
                {
                    long codigo = Convert.ToInt64(item.codigo);
                    List<Familia_Produto> Familia_ProdutoList = Familia_ProdutoBLL.getFamilia_Produto(c => c.codigo == codigo);
                    if (Familia_ProdutoList.Count() == 0)
                    {
                        Familia_Produto Familia_Produto = new Familia_Produto()
                        {
                            codigo = Convert.ToInt32(item.codigo),
                            codInt = item.codInt,
                            codFamilia = item.codFamilia,
                            nomeFamilia = item.nomeFamilia,
                            inativo = item.inativo,
                            sincronizar = "N"
                        };

                        Familia_ProdutoBLL.AdicionarFamilia_Produto(Familia_Produto);
                    }
                    else
                    {
                        Familia_Produto Familia_Produto = Familia_ProdutoList.FirstOrDefault();
                        Familia_Produto.codigo = Convert.ToInt32(item.codigo);
                        Familia_Produto.codInt = item.codInt;
                        Familia_Produto.nomeFamilia = item.nomeFamilia;
                        Familia_Produto.inativo = item.inativo;
                        Familia_Produto.sincronizar = "N";

                        Familia_ProdutoBLL.AlterarFamilia_Produto(Familia_Produto);
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
                    SyncOmieToLocal(pagina);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SyncLocalToOmie()
        {
            try
            {
                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de Familia de Produtos";
                    Application.DoEvents();
                }

                Familia_ProdutoBLL familia_ProdutoBLL = new Familia_ProdutoBLL();
                List<Familia_Produto> Familia_ProdutoList = familia_ProdutoBLL.getFamilia_Produto(p => p.sincronizar == "S",true);

                if (ProgressBar != null)
                {
                    NrTotalRegistro = Familia_ProdutoList.Count();
                    NrTotalRegistro += 1;
                    ProgressBar.Maximum = NrTotalRegistro;
                }

                foreach (Familia_Produto item in Familia_ProdutoList)
                {
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

                    if (item.codigo == null)
                    {
                        IncluirFamilia_Produto(item);
                    }
                    else
                    {
                        AlterarFamilia_Produto(item);
                    }
                    
                }
                RegistroAtual = 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }            
        }

        public string IncluirFamilia_Produto(Familia_Produto Familia_Produto)
        {
            string retorno = string.Empty;
            try
            {
                famCadastro _Familia_Produto = new famCadastro();

                _Familia_Produto.codInt = Familia_Produto.codInt;
                _Familia_Produto.codFamilia = Familia_Produto.codFamilia;
                _Familia_Produto.nomeFamilia = Familia_Produto.nomeFamilia;
                _Familia_Produto.inativo = Familia_Produto.inativo;
                
                famStatus resp = soapClient.IncluirFamilia(_Familia_Produto);

                if (resp != null)
                {
                    Familia_Produto.codigo = Convert.ToInt32(resp.codigo);
                    Familia_Produto.sincronizar = "N";
                    Familia_ProdutoBLL.AlterarFamilia_Produto(Familia_Produto);
                    retorno = resp.cDesStatus;
                }
                return retorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string AlterarFamilia_Produto(Familia_Produto Familia_Produto)
        {
            string retorno = string.Empty;
            try
            {
                famCadastro _Familia_Produto = new famCadastro();

                _Familia_Produto.codigo = Familia_Produto.codigo.ToString();
                _Familia_Produto.codInt = Familia_Produto.codInt;
                _Familia_Produto.codFamilia = Familia_Produto.codFamilia;
                _Familia_Produto.nomeFamilia = Familia_Produto.nomeFamilia;
                _Familia_Produto.inativo = Familia_Produto.inativo;
                
                famStatus resp = soapClient.AlterarFamilia(_Familia_Produto);

                if (resp != null)
                {
                    Familia_Produto.codigo = Convert.ToInt32(resp.codigo);
                    Familia_Produto.sincronizar = "N";
                    Familia_ProdutoBLL.AlterarFamilia_Produto(Familia_Produto);
                    retorno = resp.cDesStatus;
                }
                return retorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ExcluirFamilia_Produto(Familia_Produto Familia_Produto)
        {
            string retorno = string.Empty;
            try
            {
                famChave _Familia_Produto = new famChave();

                _Familia_Produto.codigo = Familia_Produto.codigo.ToString();
                _Familia_Produto.codInt = Familia_Produto.codInt;

                famStatus resp = soapClient.ExcluirFamilia(_Familia_Produto);

                if (resp != null)
                {
                    retorno = resp.cDesStatus;
                }
                return retorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void Dispose()
        {
            Familia_ProdutoBLL.Dispose();
        }
    }
}
