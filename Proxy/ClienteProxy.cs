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
                        
            //Configura os eventos

        }

        public void SyncCadastroCliente(int pagina = -1)
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
                clientes_list_request filtro = new clientes_list_request();
                clienteBLL = new ClienteBLL();


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

                    List<Cliente> ClienteList = clienteBLL.getCliente(c => c.codigo_cliente_omie == codigo_cliente_omie);

                    if (ClienteList.Count() <= 0)
                    {
                        Cliente cliente = toCliente(item);
                        clienteBLL.AdicionarCliente(cliente);
                    }
                    else
                    {
                        Cliente cliente = ClienteList.First();
                        cliente = toCliente(item, cliente);
                        clienteBLL.AlterarCliente(cliente);
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

        private void SyncLocalToOmie()
        {
            try
            {
                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de Clientes";
                    Application.DoEvents();
                }
                clienteBLL = new ClienteBLL();
                List<Cliente> ClienteList = clienteBLL.getCliente(p => p.sincronizar == "S", true);

                if (ProgressBar != null)
                {
                    NrTotalRegistro = ClienteList.Count();
                    NrTotalRegistro += 1;
                    ProgressBar.Maximum = NrTotalRegistro;
                }

                foreach (Cliente item in ClienteList)
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

                    if (item.codigo_cliente_omie <= 0)
                    {
                        IncluirClientes(item);
                    }
                    else
                    {
                        AlterarClientes(item);
                    }

                }
                RegistroAtual = 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string AlterarClientes(Cliente cliente)
        {
            string retorno = string.Empty;
            try
            {                
                clientes_cadastro _cliente = fromCliente(cliente);

                clientes_status resp = soapClient.AlterarCliente(_cliente);

                if (resp != null)
                {
                    clienteBLL = new ClienteBLL();
                    clienteBLL.UsuarioLogado = usuario;
                    Cliente cli = clienteBLL.Localizar(cliente.Id);
                    cli.codigo_cliente_omie = Convert.ToInt64(resp.codigo_cliente_omie);
                    cli.sincronizar = "N";
                    clienteBLL.AlterarCliente(cli);
                    retorno = resp.descricao_status;
                }

                return retorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string IncluirClientes(Cliente cliente)
        {
            string retorno = string.Empty;
            try
            {
                clientes_cadastro _cliente = fromCliente(cliente);

                clientes_status resp = soapClient.IncluirCliente(_cliente);

                if (resp != null)
                {
                    clienteBLL = new ClienteBLL();
                    clienteBLL.UsuarioLogado = usuario;
                    cliente.codigo_cliente_omie = Convert.ToInt64(resp.codigo_cliente_omie);
                    cliente.sincronizar = "N";
                    clienteBLL.AlterarCliente(cliente);
                    retorno = resp.descricao_status;
                }
                
                return retorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ExcluirClientes(Cliente cliente)
        {
            string retorno = string.Empty;
            try
            {
                clientes_cadastro_chave _cliente = new clientes_cadastro_chave();

                _cliente.codigo_cliente_integracao = cliente.codigo_cliente_integracao;
                _cliente.codigo_cliente_omie = cliente.codigo_cliente_omie.ToString();                                
                
                clientes_status resp = soapClient.ExcluirCliente(_cliente);

                if (resp != null)
                {                     
                    retorno = resp.descricao_status;
                }

                return retorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private clientes_cadastro fromCliente(Cliente cliente)
        {
            clientes_cadastro cc = new clientes_cadastro();
            //Campos "chave"
            cc.codigo_cliente_integracao = cliente.codigo_cliente_integracao;
            if (cliente.codigo_cliente_omie > 0)
            {
                cc.codigo_cliente_omie = cliente.codigo_cliente_omie.ToString();
            }
            //Campos de identificação
            cc.cnpj_cpf = cliente.cnpj_cpf;
            cc.razao_social = cliente.razao_social;
            cc.nome_fantasia = cliente.nome_fantasia;
            //Campos de Endereço
            cc.logradouro = cliente.logradouro;
            cc.endereco = cliente.endereco;
            cc.endereco_numero = cliente.endereco_numero;
            cc.complemento = cliente.complemento;
            cc.bairro = cliente.bairro;
            cc.cidade = cliente.cidade;
            cc.estado = cliente.estado;
            cc.cep = cliente.cep;
            cc.codigo_pais = cliente.codigo_pais;
            //Contatos
            cc.contato = cliente.contato;
            cc.telefone1_ddd = cliente.telefone1_ddd;
            cc.telefone1_numero = cliente.telefone1_numero;
            cc.telefone2_ddd = cliente.telefone2_ddd;
            cc.telefone2_numero = cliente.telefone2_numero;
            cc.fax_ddd = cliente.fax_ddd;
            cc.fax_numero = cliente.fax_numero;
            cc.email = cliente.email;
            cc.homepage = cliente.homepage;
            cc.observacao = cliente.observacao;
            cc.inscricao_municipal = cliente.inscricao_municipal;
            cc.inscricao_estadual = cliente.inscricao_estadual;
            cc.inscricao_suframa = cliente.inscricao_suframa;
            cc.pessoa_fisica = cliente.pessoa_fisica;
            cc.optante_simples_nacional = cliente.optante_simples_nacional;
            cc.bloqueado = cliente.bloqueado;
            cc.importado_api = cliente.importado_api;

            tags[] tagsArray = new tags[cliente.cliente_tag.Count()];

            int index = 0;
            foreach (Cliente_Tag item in cliente.cliente_tag)
            {
                tags tg = new tags();
                tg.tag = item.tag;
                tagsArray[index] = tg;
            }
            cc.tags = tagsArray;
            
            return cc;
        }

        private Cliente toCliente(clientes_cadastro p, Cliente cliente = null)
        {
            if (cliente == null)
            {
                cliente = new Cliente();
            }


            //Campos "chave"
            cliente.codigo_cliente_integracao = p.codigo_cliente_integracao;
            cliente.codigo_cliente_omie = Convert.ToInt64(p.codigo_cliente_omie);
            //Campos de identificação
            cliente.cnpj_cpf = p.cnpj_cpf;
            cliente.razao_social = p.razao_social;
            cliente.nome_fantasia = p.nome_fantasia;
            //Campos de Endereço
            cliente.logradouro = p.logradouro;
            cliente.endereco = p.endereco;
            cliente.endereco_numero = p.endereco_numero;
            cliente.complemento = p.complemento;
            cliente.bairro = p.bairro;
            cliente.cidade = p.cidade;
            cliente.estado = p.estado;
            cliente.cep = p.cep;
            cliente.codigo_pais = p.codigo_pais;
            //Contatos
            cliente.contato = p.contato;
            cliente.telefone1_ddd = p.telefone1_ddd;
            cliente.telefone1_numero = p.telefone1_numero;
            cliente.telefone2_ddd = p.telefone2_ddd;
            cliente.telefone2_numero = p.telefone2_numero;
            cliente.fax_ddd = p.fax_ddd;
            cliente.fax_numero = p.fax_numero;
            cliente.email = p.email;
            cliente.homepage = p.homepage;
            cliente.observacao = p.observacao;
            cliente.inscricao_municipal = p.inscricao_municipal;
            cliente.inscricao_estadual = p.inscricao_estadual;
            cliente.inscricao_suframa = p.inscricao_suframa;
            cliente.pessoa_fisica = p.pessoa_fisica;
            cliente.optante_simples_nacional = p.optante_simples_nacional;
            cliente.bloqueado = p.bloqueado;
            cliente.importado_api = p.importado_api;

            TagBLL tagBLL = new TagBLL();

            foreach (var item in p.tags)
            {

                Tag tg = tagBLL.getTag(item.tag.Trim()).FirstOrDefault();
                if (tg != null)
                {
                    if (cliente.cliente_tag.Where(c => c.tag == tg.tag1).Count() <= 0)
                    {
                        Cliente_Tag ct = new Cliente_Tag();
                        //ct.Id_cliente = cliente.Id;
                        ct.Id_tag = tg.Id;
                        ct.tag = tg.tag1;
                        cliente.cliente_tag.Add(ct);
                    }

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
