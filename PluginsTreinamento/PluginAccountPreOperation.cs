using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;


namespace PluginsTreinamento
{
    public class PluginAccountPreOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //Variavel contendo o contexto da execução
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //Variavel contendo o Service Factory da Organização
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            //Variavel contendo o Service Admin que estabelce os serviços de conexao com o Dataverse
            IOrganizationService serviceAdmin = serviceFactory.CreateOrganizationService(null);

            //Variavel do Trace que armazena informaçoes de Log
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            //Verifica se contem dados para o destino e se corresponde a uma entity
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                //Variavel do tipo entity herdando a entidade do contexo.
                Entity entidadeContexto = (Entity)context.InputParameters["Target"];

                if(entidadeContexto.LogicalName == "account") //verifica se a entidade do contexto é account
                {
                    if (entidadeContexto.Attributes.Contains("telephone1")) //verifica se contem telephone1
                    {
                        //variavel para herdar o conteudo do atributo telehpone1 do contexto
                        var phone1 = entidadeContexto["telephone1"].ToString();

                        //variavel string contendo FetchXML para consulta do contato
                        string FetchContact = @"<?xml version='1.0'?>" +
                            "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>" +
                            "<entity name='contact'>" +
                            "<attribute name='fullname'/>" +
                            "<attribute name='telephone1'/>" +
                            "<attribute name='contactid'/>" +
                            "<order descending='false' attribute='fullname'/>" +
                            "<filter type ='and'>" +
                            "<condition attribute='telephone1' value='" + phone1 + "' operator='eq'/>" +
                            "</filter>" +
                            "</entity>" +
                            "</fetch>";

                        trace.Trace("FetchContact: " + FetchContact); //armazena informaçoes de LOG

                        //variavel contendo o retorno da consulta FetchXML
                        var primarycontact = serviceAdmin.RetrieveMultiple(new FetchExpression(FetchContact));

                        if(primarycontact.Entities.Count > 0) //verifica se contem entidade
                        {
                            //para cada entidade retornada atribui a variavel entityContact
                            foreach (var entityContact in primarycontact.Entities)
                            {
                                //atribui referencia de entidade para o atributo primarycontactid (contato primario)
                                entidadeContexto["primarycontactid"] = new EntityReference("contact", entityContact.Id);
                            }
                        }
                    }
                }
            }
        }
    }
}
