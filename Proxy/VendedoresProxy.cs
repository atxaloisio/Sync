using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.VendedoresCadastroReference;
using BLL;
using Model;
using System.Windows.Forms;
using Utils;

namespace Sync
{
    public class VendedorProxy : Proxy, IDisposable
    {
        VendedoresCadastroSoapClient soapClient;
        private VendedorBLL VendedorBLL;
        public VendedorProxy()
        {
            soapClient = new VendedoresCadastroSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);
            VendedorBLL = new VendedorBLL();
        }

        public void SyncVendedor(int pagina = -1)
        {
            try
            {
                vendListarRequest filtro = new vendListarRequest();

                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de  Vendedores";
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

                vendListarResponse resp = soapClient.ListarVendedores(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.total_de_registros);
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                pagina = Convert.ToInt16(resp.pagina);

                foreach (cadastro item in resp.cadastro)
                {
                    long codigo = Convert.ToInt64(item.codigo);
                    List<Vendedor> VendedorList = VendedorBLL.getVendedor(c => c.codigo == codigo);
                    if (VendedorList.Count() == 0)
                    {
                        Vendedor Vendedor = new Vendedor()
                        {
                            codigo = Convert.ToInt64(item.codigo),
                            codInt = item.codInt,
                            inativo = item.inativo,
                            nome = item.nome.ToUpper()                            
                        };
                        
                        VendedorBLL.AdicionarVendedor(Vendedor);
                    }
                    else
                    {
                        Vendedor Vendedor = VendedorList.FirstOrDefault();
                        Vendedor.codigo = Convert.ToInt64(item.codigo);
                        Vendedor.codInt = item.codInt;
                        Vendedor.inativo = item.inativo;
                        Vendedor.nome = item.nome.ToUpper();
                        
                        VendedorBLL.AlterarVendedor(Vendedor);
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
                    SyncVendedor(pagina);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public string IncluirVendedor(Vendedor vendedor)
        {
            string retorno = string.Empty;          
            try
            {
                vendincluirRequest _vendedor = new vendincluirRequest();

                _vendedor.codInt = vendedor.codInt;
                _vendedor.nome = vendedor.nome;
                _vendedor.inativo = vendedor.inativo;

                vendIncluirResponse resp = soapClient.IncluirVendedor(_vendedor);

                if (resp != null)
                {
                    vendedor.codigo = Convert.ToInt64(resp.codigo);
                    VendedorBLL.AlterarVendedor(vendedor);
                    retorno = resp.descricao;
                }
                return retorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }            
        }

        public string AlterarVendedor(Vendedor vendedor)
        {
            string retorno = string.Empty;
            try
            {
                vendAlterarRequest _vendedor = new vendAlterarRequest();

                _vendedor.codigo = vendedor.codigo.ToString();
                _vendedor.codInt = vendedor.codInt;
                _vendedor.nome = vendedor.nome;
                _vendedor.inativo = vendedor.inativo;

                vendAlterarResponse resp = soapClient.AlterarVendedor(_vendedor);                

                if (resp != null)
                {
                    vendedor.codigo = Convert.ToInt64(resp.codigo);
                    VendedorBLL.AlterarVendedor(vendedor);
                    retorno = resp.descricao;
                }
                return retorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ExcluirVendedor(Vendedor vendedor)
        {
            string retorno = string.Empty;
            try
            {
                vendExcluirRequest _vendedor = new vendExcluirRequest();

                _vendedor.codigo = vendedor.codigo.ToString();
                _vendedor.codInt = vendedor.codInt;
                
                vendExcluirResponse resp = soapClient.ExcluirVendedor(_vendedor);

                if (resp != null)
                {                    
                    retorno = resp.descricao;
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
            VendedorBLL.Dispose();
        }
    }
}
