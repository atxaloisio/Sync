using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows.Forms;
using Sync.ClientesCadastroReference;
using BLL;
using Model;

namespace Sync
{
    public class ClienteProxy : Proxy, IDisposable
    {
        private ClientesCadastroSoapClient soapClient;
        private ClienteBLL clienteBLL;
        public ClienteProxy()
        {
            soapClient = new ClientesCadastroSoapClient();

            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);

            clienteBLL = new ClienteBLL();
            //Configura os eventos

        }

        public void SyncCadastroCliente(int pagina = -1)
        {
            try
            {
                clientes_list_request filtro = new clientes_list_request();


                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de  Clientes";
                    Application.DoEvents();
                }

                filtro.apenas_importado_api = "N";
                filtro.filtrar_apenas_alteracao = "S";
                if (pagina == -1)
                {
                    filtro.pagina = "1";
                    pagina = 1;
                }
                else
                {
                    filtro.pagina = Convert.ToString(pagina);
                }

                filtro.registros_por_pagina = "200";

                clientes_listfull_response resp = soapClient.ListarClientes(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.total_de_registros);
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                foreach (clientes_cadastro item in resp.clientes_cadastro)
                {
                    //chama o metodo que faz o inset da cliente na base.
                    long? codigo_cliente_omie = Convert.ToInt64(item.codigo_cliente_omie);

                    if (clienteBLL.getCliente(c => c.codigo_cliente_omie == codigo_cliente_omie).Count() <= 0)
                    {
                        Cliente cliente = toCliente(item);
                        clienteBLL.AdicionarCliente(cliente);
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
                    SyncCadastroCliente(pagina);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateClientes(Int64 id = -1)
        {
        }

        private Cliente toCliente(clientes_cadastro p, Int64 id = -1)
        {
            Cliente cliente = new Cliente()
            {
                //Campos "chave"
                codigo_cliente_integracao = p.codigo_cliente_integracao,
                codigo_cliente_omie = Convert.ToInt64(p.codigo_cliente_omie),
                //Campos de identificação
                cnpj_cpf = p.cnpj_cpf,
                razao_social = p.razao_social,
                nome_fantasia = p.nome_fantasia,
                //Campos de Endereço
                logradouro = p.logradouro,
                endereco = p.endereco,
                endereco_numero = p.endereco_numero,
                complemento = p.complemento,
                bairro = p.bairro,
                cidade = p.cidade,
                estado = p.estado,
                cep = p.cep,
                codigo_pais = p.codigo_pais,
                //Contatos
                contato = p.contato,
                telefone1_ddd = p.telefone1_ddd,
                telefone1_numero = p.telefone1_numero,
                telefone2_ddd = p.telefone2_ddd,
                telefone2_numero = p.telefone2_numero,
                fax_ddd = p.fax_ddd,
                fax_numero = p.fax_numero,
                email = p.email,
                homepage = p.homepage,
                observacao = p.observacao,
                inscricao_municipal = p.inscricao_municipal,
                inscricao_estadual = p.inscricao_estadual,
                inscricao_suframa = p.inscricao_suframa,
                pessoa_fisica = p.pessoa_fisica,
                optante_simples_nacional = p.optante_simples_nacional,
                bloqueado = p.bloqueado,
                importado_api = p.importado_api
            };

            if (id != -1)
            {
                cliente.Id = id;
            }

            TagBLL tagBLL = new TagBLL();

            foreach (var item in p.tags)
            {

                Tag tg = tagBLL.getTag(item.tag.Trim()).FirstOrDefault();
                if (tg != null)
                {
                    Cliente_Tag ct = new Cliente_Tag();
                    ct.Id_tag = tg.Id;
                    ct.tag = tg.tag1;
                    cliente.cliente_tag.Add(ct);
                }

            }

            return cliente;


        }

        public void Dispose()
        {
            clienteBLL.Dispose();
        }
    }
}
