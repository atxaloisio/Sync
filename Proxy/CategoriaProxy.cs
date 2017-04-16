using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.CategoriaCadastroReference;
using BLL;
using Model;
using System.Windows.Forms;

namespace Sync
{
    public class CategoriaProxy : Proxy, IDisposable
    {
        CategoriasCadastroSoapClient soapClient;
        private CategoriaBLL categoriaBLL;
        public CategoriaProxy()
        {
            soapClient = new CategoriasCadastroSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);
            categoriaBLL = new CategoriaBLL();
        }

        public void SyncCategoria(int pagina = -1)
        {
            try
            {
                categoria_list_request filtro = new categoria_list_request();

                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de  Categorias";
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

                categoria_listfull_response resp = soapClient.ListarCategorias(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.total_de_registros);
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                pagina = Convert.ToInt16(resp.pagina);

                foreach (categoria_cadastro item in resp.categoria_cadastro)
                {
                    List<Categoria> CategoriaList = categoriaBLL.getCategoria(c => c.codigo == item.codigo);
                    if (CategoriaList.Count() == 0)
                    {
                        Categoria categoria = new Categoria()
                        {
                            codigo = item.codigo,
                            conta_despesa = item.conta_despesa,
                            conta_inativa = item.conta_inativa,
                            conta_receita = item.conta_receita,
                            definida_pelo_usuario = item.definida_pelo_usuario,
                            descricao = item.descricao,
                            descricao_padrao = item.descricao_padrao,
                            id_conta_contabil = Convert.ToInt16(item.id_conta_contabil),
                            nao_exibir = item.nao_exibir,
                            natureza = item.natureza,
                            tag_conta_contabil = item.tag_conta_contabil,
                            totalizadora = item.totalizadora,
                            transferencia = item.transferencia
                        };

                        categoriaBLL.AdicionarCategoria(categoria);                        
                    }
                    else
                    {
                        Categoria categoria = CategoriaList.FirstOrDefault();
                        categoria.codigo = item.codigo;
                        categoria.conta_despesa = item.conta_despesa;
                        categoria.conta_inativa = item.conta_inativa;
                        categoria.conta_receita = item.conta_receita;
                        categoria.definida_pelo_usuario = item.definida_pelo_usuario;
                        categoria.descricao = item.descricao;
                        categoria.descricao_padrao = item.descricao_padrao;
                        categoria.id_conta_contabil = Convert.ToInt16(item.id_conta_contabil);
                        categoria.nao_exibir = item.nao_exibir;
                        categoria.natureza = item.natureza;
                        categoria.tag_conta_contabil = item.tag_conta_contabil;
                        categoria.totalizadora = item.totalizadora;
                        categoria.transferencia = item.transferencia;

                        categoriaBLL.AlterarCategoria(categoria);
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
                    SyncCategoria(pagina);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public void Dispose()
        {
            categoriaBLL.Dispose();
        }
    }
}
