using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.EmpresaCadastroReference;
using BLL;
using Model;
using System.Windows.Forms;

namespace Sync
{
    public class EmpresaProxy : Proxy, IDisposable
    {
        EmpresasCadastroSoapClient soapClient;
        private EmpresaBLL EmpresaBLL;
        public EmpresaProxy()
        {
            soapClient = new EmpresasCadastroSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);
            EmpresaBLL = new EmpresaBLL();
        }

        public void SyncEmpresa(int pagina = -1)
        {
            try
            {
                empresas_list_request filtro = new empresas_list_request();

                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de  Empresas";
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

                empresas_list_response resp = soapClient.ListarEmpresas(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.total_de_registros);
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                pagina = Convert.ToInt16(resp.pagina);

                foreach (empresas_cadastro item in resp.empresas_cadastro)
                {
                    long? codigo = Convert.ToInt64(item.codigo_empresa);
                    List<Empresa> EmpresaList = EmpresaBLL.getEmpresa(c => c.codigo_empresa == codigo);
                    if (EmpresaList.Count() == 0)
                    {
                        Empresa Empresa = new Empresa()
                        {
                            bairro = item.bairro,
                            cep = item.cep,
                            cidade = item.cidade,
                            cnae = item.cnae,
                            cnae_municipal = item.cnae_municipal,
                            cnpj = item.cnpj,
                            codigo_empresa = Convert.ToInt64(item.codigo_empresa),
                            codigo_empresa_integracao = item.codigo_empresa_integracao,
                            codigo_pais = item.codigo_pais,
                            complemento = item.complemento,                            
                            email = item.email,
                            endereco = item.endereco,
                            endereco_numero = item.endereco_numero,
                            estado = item.estado,
                            fax_ddd = item.fax_ddd,
                            fax_numero = item.fax_numero,
                            gera_nfse = item.gera_nfse,
                            inscricao_estadual = item.inscricao_estadual,
                            inscricao_municipal = item.inscricao_municipal,
                            inscricao_suframa = item.inscricao_suframa,
                            inativa = item.inativa,
                            logradouro = item.logradouro,
                            nome_fantasia = item.nome_fantasia,
                            optante_simples_nacional = item.optante_simples_nacional,
                            razao_social = item.razao_social,
                            regime_tributario = Convert.ToSByte(item.regime_tributario),
                            telefone1_ddd = item.telefone1_ddd,
                            telefone1_numero = item.telefone1_numero,
                            telefone2_ddd = item.telefone2_ddd,
                            telefone2_numero = item.telefone2_numero,
                            website = item.website
                        };

                        if (!string.IsNullOrEmpty(item.data_adesao_sn))
                        {
                            Empresa.data_adesao_sn = Convert.ToDateTime(item.data_adesao_sn);
                        }
                        
                        
                        EmpresaBLL.AdicionarEmpresa(Empresa);
                    }
                    else
                    {
                        Empresa Empresa = EmpresaList.FirstOrDefault();

                        Empresa.bairro = item.bairro;
                        Empresa.cep = item.cep;
                        Empresa.cidade = item.cidade;
                        Empresa.cnae = item.cnae;
                        Empresa.cnae_municipal = item.cnae_municipal;
                        Empresa.cnpj = item.cnpj;
                        Empresa.codigo_empresa = Convert.ToInt64(item.codigo_empresa);
                        Empresa.codigo_empresa_integracao = item.codigo_empresa_integracao;
                        Empresa.codigo_pais = item.codigo_pais;
                        Empresa.complemento = item.complemento;

                        if (!string.IsNullOrEmpty(item.data_adesao_sn))
                        {
                            Empresa.data_adesao_sn = Convert.ToDateTime(item.data_adesao_sn);
                        }
                            
                        Empresa.email = item.email;
                        Empresa.endereco = item.endereco;
                        Empresa.endereco_numero = item.endereco_numero;
                        Empresa.estado = item.estado;
                        Empresa.fax_ddd = item.fax_ddd;
                        Empresa.fax_numero = item.fax_numero;
                        Empresa.gera_nfse = item.gera_nfse;
                        Empresa.inscricao_estadual = item.inscricao_estadual;
                        Empresa.inscricao_municipal = item.inscricao_municipal;
                        Empresa.inscricao_suframa = item.inscricao_suframa;
                        Empresa.inativa = item.inativa;
                        Empresa.logradouro = item.logradouro;
                        Empresa.nome_fantasia = item.nome_fantasia;
                        Empresa.optante_simples_nacional = item.optante_simples_nacional;
                        Empresa.razao_social = item.razao_social;
                        Empresa.regime_tributario = Convert.ToSByte(item.regime_tributario);
                        Empresa.telefone1_ddd = item.telefone1_ddd;
                        Empresa.telefone1_numero = item.telefone1_numero;
                        Empresa.telefone2_ddd = item.telefone2_ddd;
                        Empresa.telefone2_numero = item.telefone2_numero;
                        Empresa.website = item.website;
                        
                        EmpresaBLL.AlterarEmpresa(Empresa);
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
                    SyncEmpresa(pagina);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public void Dispose()
        {
            EmpresaBLL.Dispose();
        }
    }
}
