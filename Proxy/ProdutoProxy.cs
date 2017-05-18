using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Sync.ProdutosCadastroReference;
using BLL;
using Model;
using System.Windows.Forms;
using Utils;

namespace Sync
{
    public class ProdutoProxy : Proxy, IDisposable
    {
        private ProdutosCadastroSoapClient soapClient;
        private ProdutoBLL ProdutoBLL;

        public ProdutoProxy()
        {
            soapClient = new ProdutosCadastroSoapClient();

            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);

            ProdutoBLL = new ProdutoBLL();
            //Configura os eventos

        }

        public void SyncCadastroProduto(int pagina = -1)
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

        private void SyncLocalToOmie()
        {
            try
            {
                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de Produtos";
                    Application.DoEvents();
                }
                ProdutoBLL = new ProdutoBLL();
                List<Produto> produtoList = ProdutoBLL.getProduto(p => p.sincronizar == "S", true);

                if (ProgressBar != null)
                {
                    NrTotalRegistro = produtoList.Count();
                    NrTotalRegistro += 1;
                    ProgressBar.Maximum = NrTotalRegistro;
                }

                foreach (Produto item in produtoList)
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

                    if (item.codigo_produto <= 0)
                    {
                        IncluirProduto(item);
                    }
                    else
                    {
                        AlterarProduto(item);
                    }

                }
                RegistroAtual = 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void SyncOmieToLocal(int pagina = -1)
        {
            try
            {
                produto_servico_list_request filtro = new produto_servico_list_request();

                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de  Produtos";
                    Application.DoEvents();
                }

                filtro.apenas_importado_api = "N";
                if (pagina == -1)
                {
                    filtro.pagina = "1";
                    pagina = 1;
                }
                else
                {
                    filtro.pagina = Convert.ToString(pagina);
                }
                //filtro.pagina = "1";
                filtro.apenas_importado_api = "N";
                filtro.filtrar_apenas_omiepdv = "N";
                filtro.registros_por_pagina = "50";
                filtro.ordenar_por = "codigo_produto";

                produto_servico_listfull_response resp = soapClient.ListarProdutos(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.total_de_registros);
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                foreach (produto_servico_cadastro item in resp.produto_servico_cadastro)
                {
                    //chama o metodo que faz o inset da Produto na base.
                    int codigo_Produto_omie = Convert.ToInt32(item.codigo_produto);
                    if (ProdutoBLL.getProduto(p => p.codigo_produto == codigo_Produto_omie).Count <= 0)
                    {
                        Produto Produto = toProduto(item);
                        if (string.IsNullOrEmpty(Produto.codigo_produto_integracao))
                        {
                            Produto.codigo_produto_integracao = Sequence.GetNextVal("sq_produto_sequence").ToString();
                        }

                        ProdutoBLL.AdicionarProduto(Produto);
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

                if (Convert.ToInt32(resp.total_de_paginas) > pagina)
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

        public string AlterarProduto(Produto produto)
        {
            string retorno = string.Empty;
            try
            {
                if (produto != null)
                {
                    produto_servico_cadastro prod = fromProduto(produto);
                    
                    produto_servico_status resp = soapClient.AlterarProduto(prod);

                    if (resp != null)
                    {
                        ProdutoBLL produtoBLL = new ProdutoBLL();
                        List<Produto> prdList = produtoBLL.getProduto(p => p.id == produto.id, true);
                        if (prdList.Count()>0)
                        {
                            Produto prd = prdList.First();
                            prd.codigo_produto = Convert.ToInt32(resp.codigo_produto);
                            prd.sincronizar = "N";
                            ProdutoBLL.AlterarProduto(prd);
                        }
                        
                        retorno = resp.descricao_status;
                    }
                    
                }
                return retorno;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string IncluirProduto(Produto produto)
        {
            string retorno = string.Empty;
            try
            {
                if (produto != null)
                {
                    produto_servico_cadastro prod = fromProduto(produto);

                    produto_servico_status resp = soapClient.IncluirProduto(prod);

                    if (resp != null)
                    {
                        ProdutoBLL produtoBLL = new ProdutoBLL();
                        Produto prd = produtoBLL.Localizar(produto.id);
                        prd.codigo_produto = Convert.ToInt32(resp.codigo_produto);
                        prd.sincronizar = "N";
                        ProdutoBLL.AlterarProduto(prd);
                        retorno = resp.descricao_status;
                    }

                }
                return retorno;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string ExcluirProduto(Produto produto)
        {
            string retorno = string.Empty;
            try
            {
                if (produto != null)
                {
                    produto_servico_cadastro_chave filtro = new produto_servico_cadastro_chave();
                    filtro.codigo = produto.codigo;
                    filtro.codigo_produto = produto.codigo_produto.ToString();
                    filtro.codigo_produto_integracao = produto.codigo_produto_integracao;

                    produto_servico_status resp = soapClient.ExcluirProduto(filtro);

                    retorno = resp.descricao_status;
                }
                return retorno;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<string> ExcluiProdutoLote()
        {
            List<string> retornoList = new List<string>();
            List<Produto> ProdutoList = ProdutoBLL.getProduto();
            try
            {
                foreach (Produto item in ProdutoList)
                {
                    string retorno = string.Empty;
                    retorno = ExcluirProduto(item);
                    if (!string.IsNullOrEmpty(retorno))
                    {
                        retornoList.Add(retorno);
                    }
                }
                return retornoList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private produto_servico_cadastro fromProduto(Produto p)
        {
            produto_servico_cadastro produto = new produto_servico_cadastro()
            {
                codigo_produto_integracao = p.codigo_produto_integracao,
                codigo_produto = p.codigo_produto.ToString(),
                codigo = p.codigo,
                descricao = p.descricao,
                ean = p.ean,
                ncm = p.ncm,
                quantidade_estoque = p.quantidade_estoque,
                quantidade_estoqueSpecified = true,
                csosn_icms = p.csosn_icms,
                unidade = p.unidade,
                valor_unitario = p.valor_unitario,
                valor_unitarioSpecified = true,
                cst_icms = p.cst_icms,
                aliquota_icms = p.aliquota_icms,
                aliquota_icmsSpecified = true,                
                red_base_icms = p.red_base_icms,
                red_base_icmsSpecified = true,
                aliquota_ibpt = p.aliquota_ibpt,
                aliquota_ibptSpecified = true,
                tipoItem = p.tipoItem,
                cst_pis = p.cst_pis,
                aliquota_pis = p.aliquota_pis,
                aliquota_pisSpecified = true,
                cst_cofins = p.cst_cofins,
                aliquota_cofins = p.aliquota_cofins,
                aliquota_cofinsSpecified = true,
                bloqueado = p.bloqueado,
                importado_api = p.importado_api,
                codigo_familia = p.codigo_familia.ToString(),                
                codInt_familia = p.codInt_familia,
                descricao_familia = p.descricao_familia,
                inativo = p.inativo,
                cest = p.cest,
                cfop = p.cfop,
                peso_liq = p.peso_liq,
                peso_liqSpecified = true,
                peso_bruto = p.peso_bruto,
                peso_brutoSpecified = true,
                estoque_minimo = p.estoque_minimo,
                estoque_minimoSpecified = false,
                descr_detalhada = p.descr_detalhada,
                obs_internas = p.obs_internas
            };

            return produto;
        }

        private Produto toProduto(produto_servico_cadastro p, Int64 id = -1)
        {
            Produto produto = new Produto()
            {
                codigo_produto_integracao = p.codigo_produto_integracao,
                codigo_produto = Convert.ToInt32(p.codigo_produto),
                codigo = p.codigo,
                descricao = p.descricao,
                ean = p.ean,
                ncm = p.ncm,
                quantidade_estoque = p.quantidade_estoque,
                csosn_icms = p.csosn_icms,
                unidade = p.unidade,
                valor_unitario = p.valor_unitario,
                cst_icms = p.cst_icms,
                aliquota_icms = p.aliquota_icms,
                red_base_icms = p.red_base_icms,
                aliquota_ibpt = p.aliquota_ibpt,
                tipoItem = p.tipoItem,
                cst_pis = p.cst_pis,
                aliquota_pis = p.aliquota_pis,
                cst_cofins = p.cst_cofins,
                aliquota_cofins = p.aliquota_cofins,
                bloqueado = p.bloqueado,
                importado_api = p.importado_api,
                codigo_familia = Convert.ToInt32(p.codigo_familia),
                codInt_familia = p.codInt_familia,
                descricao_familia = p.descricao_familia,
                inativo = p.inativo,
                cest = p.cest,
                cfop = p.cfop,
                peso_liq = p.peso_liq,
                peso_bruto = p.peso_bruto,
                estoque_minimo = p.estoque_minimo,
                descr_detalhada = p.descr_detalhada,
                obs_internas = p.obs_internas
            };

            if (id != -1)
            {
                produto.id = id;
            }

            if (p.dadosIbpt != null)
            {
                Produto_Ibpt ibpt = new Produto_Ibpt()
                {
                    aliqEstadual = p.dadosIbpt.aliqEstadual,
                    aliqMunicipal = p.dadosIbpt.aliqMunicipal,
                    aliqFederal = p.dadosIbpt.aliqFederal,
                    chave = p.dadosIbpt.chave,
                    fonte = p.dadosIbpt.fonte
                };
                produto.produto_ibpt.Add(ibpt);
            }

            return produto;
        }

        public void Dispose()
        {
            ProdutoBLL.Dispose();
        }
    }
}
