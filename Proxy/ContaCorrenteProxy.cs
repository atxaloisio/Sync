using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.ContaCorrenteCadastroReference;
using BLL;
using Model;
using System.Windows.Forms;

namespace Sync
{
    public class ContaCorrenteProxy : Proxy, IDisposable
    {
        ContaCorrenteCadastroSoapClient soapClient;
        private Conta_CorrenteBLL conta_CorrenteBLL;
        public ContaCorrenteProxy()
        {
            soapClient = new ContaCorrenteCadastroSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);
            conta_CorrenteBLL = new Conta_CorrenteBLL();
        }

        public void SyncContaCorrente(int pagina = -1)
        {
            try
            {
                fin_conta_corrente_pesquisar filtro = new fin_conta_corrente_pesquisar();

                if (Mensagem != null)
                {
                    Mensagem.Text = "Sincronizando cadastro de  Contas Corrente";
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

                fin_conta_corrente_pesquisar_resposta resp = soapClient.PesquisarContaCorrente(filtro);

                if (ProgressBar != null)
                {
                    if (resp != null)
                    {
                        NrTotalRegistro = Convert.ToInt32(resp.total_de_registros);
                    }

                    ProgressBar.Maximum = NrTotalRegistro;
                }

                pagina = Convert.ToInt16(resp.pagina);

                foreach (conta_corrente_lista item in resp.conta_corrente_lista)
                {
                    int nCodCC = Convert.ToInt32(item.nCodCC);
                    List<Conta_Corrente> Conta_CorrenteList = conta_CorrenteBLL.getConta_Corrente(c => c.nCodCC == nCodCC);
                    if (Conta_CorrenteList.Count == 0)
                    {
                        Conta_Corrente conta_Corrente = new Conta_Corrente()
                        {
                            cCodCCInt = item.cCodCCInt,
                            cModalidade = item.cModalidade,
                            codigo_agencia = item.codigo_agencia,
                            codigo_banco = item.codigo_banco,
                            conta_corrente1 = item.conta_corrente,
                            cSincrAnalitica = item.cSincrAnalitica,
                            cUtilPDV = item.cUtilPDV,
                            descricao = item.descricao,
                            nCodAdm = Convert.ToInt32(item.nCodAdm),
                            nCodCC = Convert.ToInt32(item.nCodCC),
                            nDiasVenc = Convert.ToInt32(item.nDiasVenc),
                            nNumParc = Convert.ToInt32(item.nNumParc),
                            nome_gerente = item.nome_gerente,
                            nTaxaAdm = Convert.ToDecimal(item.nTaxaAdm),
                            nTpTef = Convert.ToInt32(item.nTpTef),
                            saldo_data = (item.saldo_data != "  /  /    ")? Convert.ToDateTime(item.saldo_data): new DateTime(),
                            saldo_inicial = Convert.ToDecimal(item.saldo_inicial),
                            tipo = item.tipo,
                            tipo_comunicacao = item.tipo_comunicacao,
                            valor_limite = Convert.ToDecimal(item.valor_limite)
                        };

                        conta_CorrenteBLL.AdicionarConta_Corrente(conta_Corrente);
                    }
                    else
                    {
                        Conta_Corrente conta_Corrente = Conta_CorrenteList.FirstOrDefault();
                        conta_Corrente.cCodCCInt = item.cCodCCInt;
                        conta_Corrente.cModalidade = item.cModalidade;
                        conta_Corrente.codigo_agencia = item.codigo_agencia;
                        conta_Corrente.codigo_banco = item.codigo_banco;
                        conta_Corrente.conta_corrente1 = item.conta_corrente;
                        conta_Corrente.cSincrAnalitica = item.cSincrAnalitica;
                        conta_Corrente.cUtilPDV = item.cUtilPDV;
                        conta_Corrente.descricao = item.descricao;
                        conta_Corrente.nCodAdm = Convert.ToInt32(item.nCodAdm);
                        conta_Corrente.nCodCC = Convert.ToInt32(item.nCodCC);
                        conta_Corrente.nDiasVenc = Convert.ToInt32(item.nDiasVenc);
                        conta_Corrente.nNumParc = Convert.ToInt32(item.nNumParc);
                        conta_Corrente.nome_gerente = item.nome_gerente;
                        conta_Corrente.nTaxaAdm = Convert.ToDecimal(item.nTaxaAdm);
                        conta_Corrente.nTpTef = Convert.ToInt32(item.nTpTef);
                        conta_Corrente.saldo_data = (item.saldo_data != "  /  /    ") ? Convert.ToDateTime(item.saldo_data) : new DateTime();
                        conta_Corrente.saldo_inicial = Convert.ToDecimal(item.saldo_inicial);
                        conta_Corrente.tipo = item.tipo;
                        conta_Corrente.tipo_comunicacao = item.tipo_comunicacao;
                        conta_Corrente.valor_limite = Convert.ToDecimal(item.valor_limite);

                        conta_CorrenteBLL.AlterarConta_Corrente(conta_Corrente);
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
                    SyncContaCorrente(pagina);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public void Dispose()
        {
            conta_CorrenteBLL.Dispose();
        }
    }
}
