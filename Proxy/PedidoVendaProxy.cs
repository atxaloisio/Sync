using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.PedidoVendaProdutoReference;
using BLL;
using Model;

namespace Sync
{
    public class PedidoVendaProxy : Proxy, IDisposable
    {
        private PedidoVendaProdutoSoapClient soapClient;
        public PedidoVendaProxy()
        {
            soapClient = new PedidoVendaProdutoSoapClient();
            soapClient.Endpoint.Address = setDadosAutenticacao(soapClient.Endpoint.Address);
        }

        public void SyncPedidoVenda(int pagina = -1)
        {
            //REaliza o sincronismo da base do omie com a base do cliente
        }

        public void IncluirPedidoVenda(ref Pedido pedido)
        {
            pedido_venda_produto ped = new pedido_venda_produto();

            //Dados do Cabecalho do pedido de vendas

            cabecalho c = new cabecalho();

            //c.bloqueado = "N";
            c.bloqueado = pedido.bloqueado;
            //c.codigo_cliente = "704740906";
            c.codigo_cliente = pedido.codigo_cliente.ToString();
            //c.codigo_cliente_integracao = "";
            c.codigo_cliente_integracao = pedido.codigo_cliente_integracao;
            //c.codigo_empresa = "500745436";
            c.codigo_empresa = pedido.codigo_empresa.ToString();
            //c.codigo_empresa_integracao = "";
            c.codigo_empresa_integracao = pedido.codigo_empresa_integracao;
            //c.codigo_parcela = "000";
            c.codigo_parcela = pedido.codigo_parcela;
            //c.codigo_pedido = "";
            //c.codigo_pedido_integracao = "1";
            c.codigo_pedido_integracao = pedido.codigo_pedido_integracao;
            //c.data_previsao = DateTime.Now.ToShortDateString();
            c.data_previsao = pedido.data_previsao;
            //c.etapa = "10";
            c.etapa = pedido.etapa;
            //c.importado_api = "S";
            c.importado_api = pedido.importado_api;
            //c.quantidade_itens = "1";
            c.quantidade_itens = pedido.quantidade_itens.ToString();

            ped.cabecalho = c;
            
            //Array de itens do pedido de vendas
            det[] dets = new det[(int)pedido.quantidade_itens];

            //Item do pedido de venda
            int index = 0;
            foreach (ItemPedido item in pedido.itempedidoes)
            {
                det itemped = new det();

                ide i = new ide();

                i.codigo_item = "";
                i.codigo_item_integracao = item.codigo_item_integracao;
                i.simples_nacional = item.simples_nacional;

                itemped.ide = i;

                imposto imp = new imposto();

                

                imp.cofins_padrao = new cofins_padrao();
                imp.cofins_padrao.aliq_cofins = item.itempedido_imposto.FirstOrDefault().aliq_cofins;
                imp.cofins_padrao.aliq_cofinsSpecified = true;
                imp.cofins_padrao.base_cofins = item.itempedido_imposto.FirstOrDefault().base_cofins;
                imp.cofins_padrao.base_cofinsSpecified = true;
                imp.cofins_padrao.cod_sit_trib_cofins = item.itempedido_imposto.FirstOrDefault().cod_sit_trib_cofins;
                imp.cofins_padrao.tipo_calculo_cofins = "B";
                imp.cofins_padrao.valor_cofins = item.itempedido_imposto.FirstOrDefault().valor_cofins;
                imp.cofins_padrao.valor_cofinsSpecified = true;



                imp.cofins_st = new cofins_st();
                imp.cofins_st.cod_sit_trib_cofins_st = item.itempedido_imposto.FirstOrDefault().cod_sit_trib_cofins_st;
                imp.cofins_st.aliq_cofins_st = item.itempedido_imposto.FirstOrDefault().aliq_cofins_st;
                imp.cofins_st.aliq_cofins_stSpecified = true;
                imp.cofins_st.base_cofins_st = item.itempedido_imposto.FirstOrDefault().base_cofins_st;
                imp.cofins_st.base_cofins_stSpecified = true;
                imp.cofins_st.margem_cofins_st = item.itempedido_imposto.FirstOrDefault().margem_cofins_st;
                imp.cofins_st.margem_cofins_stSpecified = true;
                imp.cofins_st.qtde_unid_trib_cofins_st = item.itempedido_imposto.FirstOrDefault().qtde_unid_trib_cofins_st;
                imp.cofins_st.qtde_unid_trib_cofins_stSpecified = true;
                imp.cofins_st.tipo_calculo_cofins_st = item.itempedido_imposto.FirstOrDefault().tipo_calculo_cofins_st;
                imp.cofins_st.valor_cofins_st = item.itempedido_imposto.FirstOrDefault().valor_cofins_st;
                imp.cofins_st.valor_cofins_stSpecified = true;
                imp.cofins_st.valor_unid_trib_cofins_st = item.itempedido_imposto.FirstOrDefault().valor_unid_trib_cofins_st;
                imp.cofins_st.valor_unid_trib_cofins_stSpecified = true;

                imp.csll = new csll();
                imp.csll.aliq_csll = item.itempedido_imposto.FirstOrDefault().aliq_csll;
                imp.csll.aliq_csllSpecified = true;
                imp.csll.valor_csll = item.itempedido_imposto.FirstOrDefault().valor_csll;
                imp.csll.valor_csllSpecified = true;

                imp.icms = new icms();
                imp.icms.aliq_icms = item.itempedido_imposto.FirstOrDefault().aliq_icms;
                imp.icms.aliq_icmsSpecified = true;
                imp.icms.base_icms = item.itempedido_imposto.FirstOrDefault().base_icms;
                imp.icms.base_icmsSpecified = true;
                imp.icms.cod_sit_trib_icms = item.itempedido_imposto.FirstOrDefault().cod_sit_trib_icms;
                imp.icms.modalidade_icms = item.itempedido_imposto.FirstOrDefault().modalidade_icms;
                imp.icms.origem_icms = item.itempedido_imposto.FirstOrDefault().origem_icms;
                imp.icms.perc_red_base_icms = item.itempedido_imposto.FirstOrDefault().perc_red_base_icms;
                imp.icms.perc_red_base_icmsSpecified = true;
                imp.icms.valor_icms = item.itempedido_imposto.FirstOrDefault().valor_icms;
                imp.icms.valor_icmsSpecified = true;

                imp.icms_sn = new icms_sn();
                imp.icms_sn.aliq_icms_sn = item.itempedido_imposto.FirstOrDefault().aliq_icms_sn;
                imp.icms_sn.aliq_icms_snSpecified = true;
                imp.icms_sn.base_icms_sn = item.itempedido_imposto.FirstOrDefault().base_icms_sn;
                imp.icms_sn.base_icms_snSpecified = true;
                imp.icms_sn.cod_sit_trib_icms_sn = item.itempedido_imposto.FirstOrDefault().cod_sit_trib_icms_sn;
                imp.icms_sn.origem_icms_sn = item.itempedido_imposto.FirstOrDefault().origem_icms_sn;
                imp.icms_sn.valor_credito_icms_sn = item.itempedido_imposto.FirstOrDefault().valor_credito_icms_sn;
                imp.icms_sn.valor_credito_icms_snSpecified = true;
                imp.icms_sn.valor_icms_sn = item.itempedido_imposto.FirstOrDefault().valor_icms_sn;
                imp.icms_sn.valor_icms_snSpecified = true;

                imp.icms_st = new icms_st();
                imp.icms_st.aliq_icms_opprop = item.itempedido_imposto.FirstOrDefault().aliq_icms_opprop;
                imp.icms_st.aliq_icms_oppropSpecified = true;
                imp.icms_st.aliq_icms_st = item.itempedido_imposto.FirstOrDefault().aliq_icms_st;
                imp.icms_st.aliq_icms_stSpecified = true;
                imp.icms_st.base_icms_st = item.itempedido_imposto.FirstOrDefault().base_icms_st;
                imp.icms_st.base_icms_stSpecified = true;
                imp.icms_st.cod_sit_trib_icms_st = item.itempedido_imposto.FirstOrDefault().cod_sit_trib_icms_st;
                imp.icms_st.margem_icms_st = item.itempedido_imposto.FirstOrDefault().margem_icms_st;
                imp.icms_st.margem_icms_stSpecified = true;
                imp.icms_st.modalidade_icms_st = item.itempedido_imposto.FirstOrDefault().modalidade_icms_st;
                imp.icms_st.perc_red_base_icms_st = item.itempedido_imposto.FirstOrDefault().perc_red_base_icms_st;
                imp.icms_st.perc_red_base_icms_stSpecified = true;
                imp.icms_st.valor_icms_st = item.itempedido_imposto.FirstOrDefault().valor_icms_st;
                imp.icms_st.valor_icms_stSpecified = true;

                imp.inss = new inss();
                imp.inss.aliq_inss = item.itempedido_imposto.FirstOrDefault().aliq_inss;
                imp.inss.aliq_inssSpecified = true;
                imp.inss.valor_inss = item.itempedido_imposto.FirstOrDefault().valor_inss;
                imp.inss.valor_inssSpecified = true;

                imp.ipi = new ipi();
                imp.ipi.aliq_ipi = item.itempedido_imposto.FirstOrDefault().aliq_ipi;
                imp.ipi.aliq_ipiSpecified = true;
                imp.ipi.base_ipi = item.itempedido_imposto.FirstOrDefault().base_ipi;
                imp.ipi.base_ipiSpecified = true;
                imp.ipi.cod_sit_trib_ipi = item.itempedido_imposto.FirstOrDefault().cod_sit_trib_ipi;
                imp.ipi.enquadramento_ipi = item.itempedido_imposto.FirstOrDefault().enquadramento_ipi;
                imp.ipi.qtde_unid_trib_ipi = item.itempedido_imposto.FirstOrDefault().qtde_unid_trib_ipi;
                imp.ipi.qtde_unid_trib_ipiSpecified = true;
                imp.ipi.tipo_calculo_ipi = item.itempedido_imposto.FirstOrDefault().tipo_calculo_ipi;
                imp.ipi.valor_ipi = item.itempedido_imposto.FirstOrDefault().valor_ipi;
                imp.ipi.valor_ipiSpecified = true;
                imp.ipi.valor_unid_trib_ipi = item.itempedido_imposto.FirstOrDefault().valor_unid_trib_ipi;
                imp.ipi.valor_unid_trib_ipiSpecified = true;

                imp.irrf = new irrf();
                imp.irrf.aliq_irrf = item.itempedido_imposto.FirstOrDefault().aliq_irrf;
                imp.irrf.aliq_irrfSpecified = true;
                imp.irrf.valor_irrf = item.itempedido_imposto.FirstOrDefault().valor_irrf;
                imp.irrf.valor_irrfSpecified = true;

                imp.iss = new iss();
                imp.iss.aliq_iss = item.itempedido_imposto.FirstOrDefault().aliq_iss;
                imp.iss.aliq_issSpecified = true;
                imp.iss.base_iss = item.itempedido_imposto.FirstOrDefault().base_iss;
                imp.iss.base_issSpecified = true;
                imp.iss.retem_iss = item.itempedido_imposto.FirstOrDefault().retem_iss;
                imp.iss.valor_iss = item.itempedido_imposto.FirstOrDefault().valor_iss;
                imp.iss.valor_issSpecified = true;

                imp.inss = new inss();
                imp.inss.aliq_inss = item.itempedido_imposto.FirstOrDefault().aliq_inss;
                imp.inss.aliq_inssSpecified = true;
                imp.inss.valor_inss = item.itempedido_imposto.FirstOrDefault().valor_inss;
                imp.inss.valor_inssSpecified = true;

                imp.pis_padrao = new pis_padrao();
                imp.pis_padrao.aliq_pis = item.itempedido_imposto.FirstOrDefault().aliq_pis;
                imp.pis_padrao.aliq_pisSpecified = true;
                imp.pis_padrao.base_pis = item.itempedido_imposto.FirstOrDefault().base_pis;
                imp.pis_padrao.base_pisSpecified = true;
                imp.pis_padrao.cod_sit_trib_pis = item.itempedido_imposto.FirstOrDefault().cod_sit_trib_pis;
                imp.pis_padrao.qtde_unid_trib_pis = item.itempedido_imposto.FirstOrDefault().qtde_unid_trib_pis;
                imp.pis_padrao.qtde_unid_trib_pisSpecified = true;
                imp.pis_padrao.tipo_calculo_pis = item.itempedido_imposto.FirstOrDefault().tipo_calculo_pis;
                imp.pis_padrao.valor_pis = item.itempedido_imposto.FirstOrDefault().valor_pis;
                imp.pis_padrao.valor_pisSpecified = true;
                imp.pis_padrao.valor_unid_trib_pis = item.itempedido_imposto.FirstOrDefault().valor_unid_trib_pis;
                imp.pis_padrao.valor_unid_trib_pisSpecified = true;

                itemped.imposto = imp;

                produto prod = new produto();

                prod.cfop = item.itempedido_produto.FirstOrDefault().cfop;
                prod.codigo = item.itempedido_produto.FirstOrDefault().codigo;
                prod.codigo_produto = item.itempedido_produto.FirstOrDefault().codigo_produto.ToString();
                prod.codigo_produto_integracao = item.itempedido_produto.FirstOrDefault().codigo_produto_integracao;
                prod.descricao = item.itempedido_produto.FirstOrDefault().descricao;
                prod.ean = item.itempedido_produto.FirstOrDefault().ean;
                prod.ncm = item.itempedido_produto.FirstOrDefault().ncm;
                prod.percentual_desconto = item.itempedido_produto.FirstOrDefault().percentual_desconto;
                prod.percentual_descontoSpecified = true;
                prod.quantidade = item.itempedido_produto.FirstOrDefault().quantidade;
                prod.quantidadeSpecified = true;
                prod.tipo_desconto = item.itempedido_produto.FirstOrDefault().tipo_desconto;
                prod.unidade = item.itempedido_produto.FirstOrDefault().unidade;
                prod.valor_deducao = item.itempedido_produto.FirstOrDefault().valor_deducao;
                prod.valor_deducaoSpecified = true;
                prod.valor_desconto = item.itempedido_produto.FirstOrDefault().valor_desconto;
                prod.valor_descontoSpecified = true;
                prod.valor_mercadoria = item.itempedido_produto.FirstOrDefault().valor_mercadoria;
                prod.valor_mercadoriaSpecified = true;
                prod.valor_total = item.itempedido_produto.FirstOrDefault().valor_total;
                prod.valor_totalSpecified = true;
                prod.valor_unitario = item.itempedido_produto.FirstOrDefault().valor_unitario;
                prod.valor_unitarioSpecified = true;

                itemped.produto = prod;

                //Inlui o item do pedido no array
                dets[index] = itemped;
                index++;
            }
           
            //Associa o array ao array do pedido
            ped.det = dets;

            int nrParcela = pedido.pedido_parcelas.Count();

            parcela[] parcelas = new parcela[nrParcela];
            index = 0;
            foreach (Pedido_Parcelas item in pedido.pedido_parcelas)
            {
                parcela p = new parcela();

                p.numero_parcela = item.numero_parcela.ToString();
                p.data_vencimento = item.data_vencimento;
                p.percentual = item.percentual;
                p.percentualSpecified = true;
                p.quantidade_dias = item.quantidade_dias.ToString();
                p.valor = item.valor;
                p.valorSpecified = true;
                parcelas[index] = p;
                index++;
            }
            
            lista_parcelas lstparc = new lista_parcelas();

            lstparc.parcela = parcelas;

            ped.lista_parcelas = lstparc;

            if (pedido.pedido_infoadic.Count >0)
            {
                ped.informacoes_adicionais = new informacoes_adicionais();
                ped.informacoes_adicionais.codVend = pedido.pedido_infoadic.FirstOrDefault().codVend.ToString();
                ped.informacoes_adicionais.codigo_categoria = pedido.pedido_infoadic.FirstOrDefault().codigo_categoria;
                ped.informacoes_adicionais.codigo_conta_corrente = pedido.pedido_infoadic.FirstOrDefault().codigo_conta_corrente.ToString();                
            }
            
                
            ped.observacoes = new observacoes();
            ped.observacoes.obs_venda = pedido.pedido_observacoes.FirstOrDefault().observacao;
           
            pedido_venda_produto_response resp = soapClient.IncluirPedido(ped);

            if (resp != null)
            {
                pedido.codigo_pedido = Convert.ToInt32(resp.codigo_pedido);
                pedido.numero_pedido = resp.numero_pedido;               
                Console.WriteLine("Pedido incluido com sucesso: " + resp.codigo_pedido);
            }
        }

        public void ExcluirPedido(long codigo)
        {
            pvpConsultarRequest filtro = new pvpConsultarRequest();
            filtro.codigo_pedido = codigo.ToString();

            pvpConsultarResponse resp = soapClient.ConsultarPedido(filtro);

            if(resp != null)
            {
                pedido_venda_produto pedido = resp.pedido_venda_produto;

                det[] detArray = new det[1];

                det d = new det();

                pedido.frete = new frete();
                pedido.frete.codigo_transportadora = " ";
                pedido.frete.codigo_transportadora_integracao = " ";


                detArray[0] = d;

                pedido.det = detArray;

                

                

                pedido_venda_produto_response pvpResp = soapClient.IncluirPedido(pedido);

                if (pvpResp != null)
                {                    
                    Console.WriteLine("Pedido alterado com sucesso: " + pvpResp.codigo_pedido);
                }

            }
        }

        public void Dispose()
        {
            soapClient.Close();
        }
    }
}
